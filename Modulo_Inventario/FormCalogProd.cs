using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;


namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormCalogProd : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable dtCatalogo = new DataTable();

        public FormCalogProd()
        {
            InitializeComponent();

            ConfigurarGrid();
            CargarFiltros();
            CargarCatalogo();
            DataGridViewEstilo.AplicarEstiloDashboard(dgvCatalogo);


            txtBuscar.TextChanged += (s, e) => AplicarFiltrosLocal();
            cmbTipo.SelectedIndexChanged += (s, e) => AplicarFiltrosLocal();
            cmbImpuesto.SelectedIndexChanged += (s, e) => AplicarFiltrosLocal();

            btnRefrescar.Click += (s, e) =>
            {
                txtBuscar.Clear();
                CargarFiltros();
                CargarCatalogo();
            };

            btnVerDetalle.Click += (s, e) => AbrirDetalleSeleccionado();

            dgvCatalogo.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) AbrirDetalleSeleccionado();
            };

            AplicarEstilosGenerales();
            EstilizarControles();
            EstilizarGridCatalogo();
            EstilizarBotones();

            
            dgvCatalogo.Margin = new Padding(0);

            // si está dentro de un panel contenedor:
            if (dgvCatalogo.Parent != null)
            {
                dgvCatalogo.Parent.Padding = new Padding(0);
            }

        }

        private void ConfigurarGrid()
        {
            dgvCatalogo.AllowUserToAddRows = false;
            dgvCatalogo.ReadOnly = true;
            dgvCatalogo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCatalogo.MultiSelect = false;
            dgvCatalogo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCatalogo.RowHeadersVisible = false;
        }

        private void CargarFiltros()
        {
            CargarTiposDesdeBD();
            CargarImpuestosDesdeBD();
        }

        private void CargarTiposDesdeBD()
        {
            try
            {
                con.Abrir();

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(
                    "SELECT DISTINCT tipo FROM Productos WHERE tipo IS NOT NULL AND LTRIM(RTRIM(tipo))<>'' ORDER BY tipo",
                    con.leer))
                {
                    da.Fill(dt);
                }

                cmbTipo.Items.Clear();
                cmbTipo.Items.Add("TODOS");

                foreach (DataRow r in dt.Rows)
                    cmbTipo.Items.Add(r["tipo"].ToString());

                cmbTipo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando tipos: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarImpuestosDesdeBD()
        {
            try
            {
                con.Abrir();

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(@"
SELECT id,
       CONCAT(nombre,' (', porcentaje, '%)') AS display
FROM Impuestos
WHERE ISNULL(activo,1)=1
ORDER BY id;", con.leer))
                {
                    da.Fill(dt);
                }

                // “TODOS”
                var dt2 = dt.Clone();
                var rowTodos = dt2.NewRow();
                rowTodos["id"] = 0;
                rowTodos["display"] = "TODOS";
                dt2.Rows.Add(rowTodos);

                foreach (DataRow r in dt.Rows)
                    dt2.ImportRow(r);

                cmbImpuesto.DataSource = dt2;
                cmbImpuesto.DisplayMember = "display";
                cmbImpuesto.ValueMember = "id";
                cmbImpuesto.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando impuestos: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarCatalogo()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT
    p.id,
    p.nombre,
    p.tipo,
    ISNULL(p.stock,0) AS stock,
    p.precio_costo,
    p.precio_pvp,
    p.impuesto_id,
    i.nombre AS impuesto_nombre,
    i.porcentaje AS impuesto_porcentaje
FROM Productos p
INNER JOIN Impuestos i ON p.impuesto_id = i.id
ORDER BY p.nombre ASC;";

                dtCatalogo.Clear();
                using (var da = new SqlDataAdapter(sql, con.leer))
                {
                    da.Fill(dtCatalogo);
                }

                dgvCatalogo.DataSource = dtCatalogo;

                // ocultar internas
                if (dgvCatalogo.Columns.Contains("id")) dgvCatalogo.Columns["id"].Visible = false;
                if (dgvCatalogo.Columns.Contains("precio_costo")) dgvCatalogo.Columns["precio_costo"].Visible = false;
                if (dgvCatalogo.Columns.Contains("impuesto_id")) dgvCatalogo.Columns["impuesto_id"].Visible = false;
                if (dgvCatalogo.Columns.Contains("impuesto_porcentaje")) dgvCatalogo.Columns["impuesto_porcentaje"].Visible = false;

                // headers
                if (dgvCatalogo.Columns.Contains("nombre")) dgvCatalogo.Columns["nombre"].HeaderText = "Producto";
                if (dgvCatalogo.Columns.Contains("tipo")) dgvCatalogo.Columns["tipo"].HeaderText = "Tipo";
                if (dgvCatalogo.Columns.Contains("stock")) dgvCatalogo.Columns["stock"].HeaderText = "Stock";
                if (dgvCatalogo.Columns.Contains("precio_pvp")) dgvCatalogo.Columns["precio_pvp"].HeaderText = "PVP";
                if (dgvCatalogo.Columns.Contains("impuesto_nombre")) dgvCatalogo.Columns["impuesto_nombre"].HeaderText = "Impuesto";

                AplicarFiltrosLocal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar catálogo: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void AplicarFiltrosLocal()
        {
            if (dtCatalogo == null) return;

            string texto = (txtBuscar.Text ?? "").Trim().Replace("'", "''");
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "TODOS";

            int impId = 0;
            if (cmbImpuesto.SelectedValue != null)
                int.TryParse(cmbImpuesto.SelectedValue.ToString(), out impId);

            var dv = new DataView(dtCatalogo);

            string filtro = "1=1";

            if (!string.IsNullOrWhiteSpace(texto))
                filtro += $" AND (nombre LIKE '%{texto}%' OR tipo LIKE '%{texto}%')";

            if (tipo != "TODOS")
                filtro += $" AND tipo = '{tipo.Replace("'", "''")}'";

            if (impId != 0)
                filtro += $" AND impuesto_id = {impId}";

            dv.RowFilter = filtro;

            dgvCatalogo.DataSource = dv;
            lblTotal.Text = $"Mostrando: {dv.Count}";
        }

        private void AbrirDetalleSeleccionado()
        {
            if (dgvCatalogo.CurrentRow == null)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            var row = dgvCatalogo.CurrentRow;

            string nombre = row.Cells["nombre"].Value?.ToString() ?? "";
            string tipo = row.Cells["tipo"].Value?.ToString() ?? "";
            int stock = Convert.ToInt32(row.Cells["stock"].Value ?? 0);
            decimal costo = Convert.ToDecimal(row.Cells["precio_costo"].Value ?? 0m);
            decimal pvp = Convert.ToDecimal(row.Cells["precio_pvp"].Value ?? 0m);
            string impNombre = row.Cells["impuesto_nombre"].Value?.ToString() ?? "";
            decimal porc = Convert.ToDecimal(row.Cells["impuesto_porcentaje"].Value ?? 0m);

            decimal precioFinal = pvp + (pvp * (porc / 100m));

            MessageBox.Show(
                $"Producto: {nombre}\nTipo: {tipo}\nStock: {stock}\n\n" +
                $"Costo: {costo:N2}\nPVP: {pvp:N2}\n\n" +
                $"Impuesto: {impNombre} ({porc:N2}%)\n" +
                $"Precio final: {precioFinal:N2}",
                "Detalle del producto",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void AplicarEstilosGenerales()
        {
            
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        private void EstilizarControles()
        {
            txtBuscar.Font = new Font("Segoe UI", 10.5F);
            txtBuscar.BorderStyle = BorderStyle.FixedSingle;

            cmbTipo.Font = new Font("Segoe UI", 10.5F);
            cmbImpuesto.Font = new Font("Segoe UI", 10.5F);

            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImpuesto.DropDownStyle = ComboBoxStyle.DropDownList;

            if (lblTotal != null)
            {
                lblTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                
            }
        }

        private void EstilizarGridCatalogo()
        {
            dgvCatalogo.AllowUserToAddRows = false;
            dgvCatalogo.AllowUserToDeleteRows = false;
            dgvCatalogo.AllowUserToResizeRows = false;
            dgvCatalogo.ReadOnly = true;

            dgvCatalogo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCatalogo.MultiSelect = false;

            dgvCatalogo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCatalogo.RowHeadersVisible = false;

            dgvCatalogo.BorderStyle = BorderStyle.None;
            dgvCatalogo.BackgroundColor = Color.White;

            dgvCatalogo.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvCatalogo.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCatalogo.GridColor = Color.FromArgb(230, 230, 230);

            dgvCatalogo.EnableHeadersVisualStyles = false;
            dgvCatalogo.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvCatalogo.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCatalogo.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvCatalogo.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvCatalogo.ColumnHeadersHeight = 40;

            dgvCatalogo.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvCatalogo.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvCatalogo.DefaultCellStyle.BackColor = Color.White;
            dgvCatalogo.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvCatalogo.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);

            dgvCatalogo.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvCatalogo.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);

            dgvCatalogo.RowTemplate.Height = 38;

            dgvCatalogo.DataBindingComplete -= dgvCatalogo_DataBindingComplete;
            dgvCatalogo.DataBindingComplete += dgvCatalogo_DataBindingComplete;

            dgvCatalogo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCatalogo.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvCatalogo.ScrollBars = ScrollBars.Both;
            dgvCatalogo.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvCatalogo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dgvCatalogo.Columns.Count > 0)
                dgvCatalogo.Columns[dgvCatalogo.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

        private void dgvCatalogo_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvCatalogo.Columns.Contains("precio_pvp"))
            {
                dgvCatalogo.Columns["precio_pvp"].DefaultCellStyle.Format = "N2";
                dgvCatalogo.Columns["precio_pvp"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvCatalogo.Columns.Contains("stock"))
            {
                dgvCatalogo.Columns["stock"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void EstilizarBotones()
        {
            
        }

        private void EstiloBoton(Button btn, Color color)
        {
            if (btn == null) return;

            
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            
            btn.Cursor = Cursors.Hand;

            Color hover = Color.FromArgb(
                Math.Min(color.R + 15, 255),
                Math.Min(color.G + 15, 255),
                Math.Min(color.B + 15, 255)
            );

            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = color;
        }

        private void FormCalogProd_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvCatalogo);

        }
    }
}


