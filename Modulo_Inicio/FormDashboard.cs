using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace PROYECTOMECANICO.Modulo_Inicio
{
    public partial class FormDashboard : Form
    {
        private readonly Conexion con = new Conexion();
        private Button currentButton;
        public FormDashboard()
        {
            InitializeComponent();
            // Default - Last 7 days
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            btnLast7Days.Select();
            LoadData();
            SetdDateMenuButtonsUI(btnLast7Days);
        }

        private void LoadData()
        {
            try
            {
                

                DateTime desde = dtpStartDate.Value.Date;
                DateTime hasta = dtpEndDate.Value.Date.AddDays(1);

                // ===== KPI PRINCIPALES =====
                using (var cn = con.CrearConexionAbierta())
                {
                    // Número de órdenes de trabajo
                    string sqlOrdenes = @"
                        SELECT COUNT(*) FROM OrdenesTrabajo 
                        WHERE fecha_ingreso >= @desde AND fecha_ingreso < @hasta";
                    using (var cmd = new SqlCommand(sqlOrdenes, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        lblNumOrdenes.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Total de ventas (facturas)
                    string sqlVentas = @"
                        SELECT ISNULL(SUM(total_final), 0) FROM Facturas 
                        WHERE fecha >= @desde AND fecha < @hasta";
                    using (var cmd = new SqlCommand(sqlVentas, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        decimal totalVentas = Convert.ToDecimal(cmd.ExecuteScalar());
                        lblTotalVentas.Text = totalVentas.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC"));
                    }

                    // Total de gastos (compras)
                    string sqlGastos = @"
                        SELECT ISNULL(SUM(total_compra), 0) FROM Compras 
                        WHERE fecha_compra >= @desde AND fecha_compra < @hasta";
                    using (var cmd = new SqlCommand(sqlGastos, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        decimal totalGastos = Convert.ToDecimal(cmd.ExecuteScalar());
                        lblTotalGastos.Text = totalGastos.ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC"));
                    }

                    // Ganancia (ventas - gastos)
                    decimal totalVentasNum = Convert.ToDecimal(lblTotalVentas.Text.Trim('$', ' ', ','));
                    decimal totalGastosNum = Convert.ToDecimal(lblTotalGastos.Text.Trim('$', ' ', ','));
                    lblGanancia.Text = (totalVentasNum - totalGastosNum).ToString("C2", System.Globalization.CultureInfo.GetCultureInfo("es-EC"));

                    // ===== CONTADORES GENERALES =====
                    // Total clientes
                    string sqlClientes = "SELECT COUNT(*) FROM Clientes";
                    using (var cmd = new SqlCommand(sqlClientes, cn))
                    {
                        lblTotalClientes.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Total proveedores
                    string sqlProveedores = "SELECT COUNT(*) FROM Proveedores WHERE estado = 1";
                    using (var cmd = new SqlCommand(sqlProveedores, cn))
                    {
                        lblTotalProveedores.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Total productos
                    string sqlProductos = "SELECT COUNT(*) FROM Productos";
                    using (var cmd = new SqlCommand(sqlProductos, cn))
                    {
                        lblTotalProductos.Text = cmd.ExecuteScalar().ToString();
                    }

                    // Órdenes activas
                    string sqlOrdenesActivas = @"
                        SELECT COUNT(*) FROM OrdenesTrabajo 
                        WHERE estado NOT IN ('Terminado', 'Entregado', 'Finalizado')";
                    using (var cmd = new SqlCommand(sqlOrdenesActivas, cn))
                    {
                        lblOrdenesActivas.Text = cmd.ExecuteScalar().ToString();
                    }
                }

                // ===== GRÁFICO DE VENTAS (Línea) =====
                CargarGraficoVentas(desde, hasta);

                // ===== GRÁFICO DE PRODUCTOS MÁS VENDIDOS (Dona) =====
                CargarGraficoTopProductos(desde, hasta);

                // ===== PRODUCTOS CON BAJO STOCK =====
                CargarProductosBajoStock();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message, "Dashboard",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void CargarGraficoVentas(DateTime desde, DateTime hasta)
        {
            try
            {
                string sql = @"
            SELECT 
                CAST(fecha AS date) AS Fecha,
                SUM(total_final) AS Total
            FROM Facturas
            WHERE fecha >= @desde AND fecha < @hasta
            GROUP BY CAST(fecha AS date)
            ORDER BY CAST(fecha AS date)";

                var dt = new DataTable();
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@desde", desde);
                    cmd.Parameters.AddWithValue("@hasta", hasta);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                // SOLO ESTO: Obtener la serie que ya existe y limpiar sus puntos
                var series = chartVentas.Series["Series1"];
                series.Points.Clear();

                // Agregar los nuevos puntos
                foreach (DataRow row in dt.Rows)
                {
                    DateTime fecha = Convert.ToDateTime(row["Fecha"]);
                    decimal total = Convert.ToDecimal(row["Total"]);
                    series.Points.AddXY(fecha.ToString("dd/MM"), total);
                }

                chartVentas.Invalidate();
                chartVentas.Update();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void CargarGraficoTopProductos(DateTime desde, DateTime hasta)
        {
            try
            {
                string sql = @"
                    SELECT TOP 5
                        p.nombre AS Producto,
                        SUM(fi.cantidad) AS CantidadVendida
                    FROM FacturaItems fi
                    INNER JOIN Facturas f ON f.id = fi.factura_id
                    INNER JOIN Productos p ON p.id = fi.producto_id
                    WHERE f.fecha >= @desde AND f.fecha < @hasta
                      AND fi.producto_id IS NOT NULL
                    GROUP BY p.nombre
                    ORDER BY SUM(fi.cantidad) DESC";

                var dt = new DataTable();
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@desde", desde);
                    cmd.Parameters.AddWithValue("@hasta", hasta);
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                chartTopProductos.Series.Clear();
                var series = new Series("Productos más vendidos")
                {
                    ChartType = SeriesChartType.Doughnut,
                    IsValueShownAsLabel = true,
                    LabelFormat = "C0"
                };

                foreach (DataRow row in dt.Rows)
                {
                    string producto = row["Producto"].ToString();
                    int cantidad = Convert.ToInt32(row["CantidadVendida"]);
                    series.Points.AddXY(producto, cantidad);
                }

                if (series.Points.Count == 0)
                {
                    series.Points.AddXY("Sin datos", 1);
                }

                chartTopProductos.Series.Add(series);
                chartTopProductos.Titles[0].Text = "Top 5 Productos más vendidos";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cargando gráfico de productos: " + ex.Message);
            }
        }

        private void CargarProductosBajoStock()
        {
            try
            {
                string sql = @"
                    SELECT 
                        nombre AS Producto,
                        stock AS StockActual
                    FROM Productos
                    WHERE stock <= 10
                    ORDER BY stock ASC";

                var dt = new DataTable();
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                dgvBajoStock.DataSource = dt;
                dgvBajoStock.Columns[0].HeaderText = "Producto";
                dgvBajoStock.Columns[1].HeaderText = "Stock Actual";
                dgvBajoStock.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                if (dt.Rows.Count == 0)
                {
                    lblSinStock.Text = "✓ No hay productos con bajo stock";
                    lblSinStock.Visible = true;
                    dgvBajoStock.Visible = false;
                }
                else
                {
                    lblSinStock.Visible = false;
                    dgvBajoStock.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cargando productos bajo stock: " + ex.Message);
            }
        }

        private void SetdDateMenuButtonsUI(object button)
        {
            var btn = (Button)button;

            btn.BackColor = btnLast30Days.FlatAppearance.BorderColor;
            btn.ForeColor = Color.White;

            if (currentButton != null && currentButton != btn)
            {
                currentButton.BackColor = this.BackColor;
                currentButton.ForeColor = Color.FromArgb(128, 139, 182);
            }
            currentButton = btn;

            if (btn == btnCustomDate)
            {
                dtpStartDate.Enabled = true;
                dtpEndDate.Enabled = true;
                btnOkCustomDate.Visible = true;
                lblEndDate.Cursor = Cursors.Hand;
                lblStartDate.Cursor = Cursors.Hand;
            }
             else
            {
                dtpStartDate.Enabled = false;
                dtpEndDate.Enabled = false;
                btnOkCustomDate.Visible = false;
                lblEndDate.Cursor = Cursors.Default;
                lblStartDate.Cursor = Cursors.Default;
            }
            //dtpStartDate.Enabled = false;
            //dtpEndDate.Enabled = false;
            //btnOkCustomDate.Visible = false;
        }

        // ===== EVENTOS DE FILTROS =====

        private void btnToday_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetdDateMenuButtonsUI(sender);
        }

        private void btnLast7Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-7);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetdDateMenuButtonsUI(sender);
        }

        private void btnLast30Days_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetdDateMenuButtonsUI(sender);
        }

        private void btnThisMonth_Click(object sender, EventArgs e)
        {
            dtpStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            dtpEndDate.Value = DateTime.Now;
            LoadData();
            SetdDateMenuButtonsUI(sender);
        }

        private void btnCustomDate_Click(object sender, EventArgs e)
        {
            SetdDateMenuButtonsUI(sender);
            dtpStartDate.Enabled = true;
            dtpEndDate.Enabled = true;
            btnOkCustomDate.Visible = true;
        }

        private void btnOkCustomDate_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void label10_Click(object sender, EventArgs e)
        {
            if(currentButton == btnCustomDate)
            {
                dtpStartDate.Select();
                SendKeys.Send("%{DOWN}");
            }
        }

        private void FormDashboard_Load(object sender, EventArgs e)
        {
            lblEndDate.Text = dtpEndDate.Text;
            lblStartDate.Text = dtpStartDate.Text;
            dgvBajoStock.Columns[1].Width = 50;
        }

        private void label12_Click(object sender, EventArgs e)
        {
            if (currentButton == btnCustomDate)
            {
                dtpEndDate.Select();
                SendKeys.Send("%{DOWN}");
            }
        }

        private void dtpStartDate_ValueChanged(object sender, EventArgs e)
        {
           lblStartDate.Text = dtpStartDate.Text;
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {
            lblEndDate.Text = dtpEndDate.Text;
        }
    }
}