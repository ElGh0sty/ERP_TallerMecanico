using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormKardex : Form
    {
        Conexion con = new Conexion();
        DataTable dtMovimientos = new DataTable();
        private DataTable dtProductos = new DataTable();
        private long productoIdSeleccionado = 0;


        public FormKardex()
        {
            InitializeComponent();

            ConfigurarGrid();
            CargarProductos();
            ConfigurarFiltros();

            DataGridViewEstilo.AplicarEstiloDashboard(dgvKardex);


            btnFiltrar.Click += (s, e) => CargarKardex();
            btnRefrescar.Click += (s, e) =>
            {
                cmbTipo.SelectedIndex = 0;
                productoIdSeleccionado = 0;
                txtBuscarProducto.Clear();
                lstProductos.Visible = false;

                if (lblProductoSel != null)
                    lblProductoSel.Text = "Producto seleccionado: TODOS";

                CargarKardex();
            };


            CargarKardex();

            CargarProductosBuscador();

            lstProductos.Visible = false;

            txtBuscarProducto.TextChanged += (s, e) => FiltrarProductos(txtBuscarProducto.Text);
            txtBuscarProducto.KeyDown += txtBuscarProducto_KeyDown;

            lstProductos.DoubleClick += (s, e) => ConfirmarProductoSeleccionado();
            lstProductos.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ConfirmarProductoSeleccionado();
                }
            };

            AplicarEstilosGenerales();
            EstilizarControles();
            EstilizarGridKardex();
            EstilizarListaBuscador();

        }

        private void ConfigurarFiltros()
        {
            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("TODOS");
            cmbTipo.Items.Add("ENTRADA");
            cmbTipo.Items.Add("SALIDA");
            cmbTipo.SelectedIndex = 0;

            dtDesde.Value = DateTime.Today.AddMonths(-1);
            dtHasta.Value = DateTime.Today;
        }

        private void ConfigurarGrid()
        {
            dgvKardex.AllowUserToAddRows = false;
            dgvKardex.ReadOnly = true;
            dgvKardex.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKardex.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKardex.RowHeadersVisible = false;

            dgvKardex.BackgroundColor = Color.White;
            dgvKardex.BorderStyle = BorderStyle.None;

            dgvKardex.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvKardex.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKardex.EnableHeadersVisualStyles = false;
        }

        private void CargarProductosBuscador()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    id,
    nombre,
    tipo,
    ISNULL(stock,0) AS stock,
    CONCAT(nombre, '  |  ', tipo, '  |  Stock: ', ISNULL(stock,0)) AS display
FROM Productos
ORDER BY nombre;";

                dtProductos.Clear();

                using (var da = new SqlDataAdapter(sql, con.leer))
                {
                    da.Fill(dtProductos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }



        private void FiltrarProductos(string texto)
        {
            texto = (texto ?? "").Trim().Replace("'", "''");

            if (texto.Length < 1)
            {
                lstProductos.Visible = false;
                lstProductos.DataSource = null;
                return;
            }

            var dv = new DataView(dtProductos);
            dv.RowFilter = $"nombre LIKE '%{texto}%' OR tipo LIKE '%{texto}%'";

            if (dv.Count == 0)
            {
                lstProductos.Visible = false;
                lstProductos.DataSource = null;
                return;
            }

            lstProductos.DisplayMember = "nombre";
            lstProductos.ValueMember = "id";
            lstProductos.DataSource = dv;

            lstProductos.Visible = true;
            lstProductos.Left = txtBuscarProducto.Left;
            lstProductos.Top = txtBuscarProducto.Bottom + 2;
            lstProductos.Width = txtBuscarProducto.Width;
            lstProductos.BringToFront();

            lstProductos.Height = Math.Min(200, (dv.Count * 22) + 4);
        }


        private void ConfirmarProductoSeleccionado()
        {
            if (lstProductos.SelectedItem == null) return;

            DataRowView row = (DataRowView)lstProductos.SelectedItem;

            productoIdSeleccionado = Convert.ToInt64(row["id"]);

            txtBuscarProducto.Text = row["nombre"].ToString();

            lstProductos.Visible = false;

            if (lblProductoSel != null)
                lblProductoSel.Text = "Producto seleccionado: " + lstProductos.Text;
        }


        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (!lstProductos.Visible) return;

            if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                lstProductos.Focus();
                if (lstProductos.Items.Count > 0) lstProductos.SelectedIndex = 0;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                lstProductos.Visible = false;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ConfirmarProductoSeleccionado();
            }
        }


        private void CargarProductos()
        {
            try
            {
                con.Abrir();

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT id, nombre FROM Productos ORDER BY nombre", con.leer);

                da.Fill(dt);

                DataRow row = dt.NewRow();
                row["id"] = 0;  
                row["nombre"] = "TODOS";
                dt.Rows.InsertAt(row, 0);

                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarKardex()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT
    k.id AS kardex_id,
    k.fecha,
    k.producto_id,
    p.nombre AS producto,
    k.tipo_movimiento,
    k.cantidad,
    u.nombre_usuario AS usuario,
    k.origen,
    k.referencia_id,
    CASE 
        WHEN k.referencia_id IS NULL THEN k.origen
        ELSE CONCAT(k.origen, ' #', k.referencia_id)
    END AS documento
FROM Kardex k
INNER JOIN Productos p ON k.producto_id = p.id
INNER JOIN Usuarios u ON k.usuario_id = u.id
WHERE
    (@prod = 0 OR k.producto_id = @prod)
    AND (@tipo = 'TODOS' OR k.tipo_movimiento = @tipo)
    AND k.fecha >= @desde AND k.fecha < @hasta
ORDER BY k.fecha DESC, k.id DESC;

";

                using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                {
                    long prod = productoIdSeleccionado; // 0 = TODOS

                    cmd.Parameters.AddWithValue("@prod", prod);
                    cmd.Parameters.AddWithValue("@tipo", cmbTipo.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@desde", dtDesde.Value.Date);
                    cmd.Parameters.AddWithValue("@hasta", dtHasta.Value.Date.AddDays(1)); // exclusivo

                    dtMovimientos.Clear();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dtMovimientos);
                    }
                }

                dgvKardex.DataSource = dtMovimientos;
                lblTotal.Text = $"Movimientos: {dtMovimientos.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando Kardex: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void AplicarEstilosGenerales()
        {
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            // opcional: evita parpadeo
            this.DoubleBuffered = true;
        }

        private void EstilizarControles()
        {
            
            
            txtBuscarProducto.BorderStyle = BorderStyle.FixedSingle;

            
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;

           

            
            
        }

        private void EstilizarGridKardex()
        {
            dgvKardex.AllowUserToAddRows = false;
            dgvKardex.AllowUserToDeleteRows = false;
            dgvKardex.AllowUserToResizeRows = false;
            dgvKardex.ReadOnly = true;

            dgvKardex.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKardex.MultiSelect = false;

            dgvKardex.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKardex.RowHeadersVisible = false;

            dgvKardex.BorderStyle = BorderStyle.None;
            dgvKardex.BackgroundColor = Color.White;

            dgvKardex.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvKardex.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvKardex.GridColor = Color.FromArgb(230, 230, 230);

            dgvKardex.EnableHeadersVisualStyles = false;
            dgvKardex.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvKardex.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvKardex.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvKardex.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvKardex.ColumnHeadersHeight = 40;

            dgvKardex.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvKardex.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvKardex.DefaultCellStyle.BackColor = Color.White;
            dgvKardex.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvKardex.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);

            dgvKardex.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvKardex.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);

            dgvKardex.RowTemplate.Height = 38;

            // ✅ formatos y headers al bindear
            dgvKardex.DataBindingComplete -= dgvKardex_DataBindingComplete;
            dgvKardex.DataBindingComplete += dgvKardex_DataBindingComplete;
        }

        private void dgvKardex_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            
            if (dgvKardex.Columns.Contains("fecha"))
            {
                dgvKardex.Columns["fecha"].HeaderText = "Fecha";
                dgvKardex.Columns["fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }
            if (dgvKardex.Columns.Contains("producto"))
                dgvKardex.Columns["producto"].HeaderText = "Producto";
            if (dgvKardex.Columns.Contains("tipo_movimiento"))
                dgvKardex.Columns["tipo_movimiento"].HeaderText = "Tipo";
            if (dgvKardex.Columns.Contains("cantidad"))
            {
                dgvKardex.Columns["cantidad"].HeaderText = "Cantidad";
                dgvKardex.Columns["cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            if (dgvKardex.Columns.Contains("usuario"))
                dgvKardex.Columns["usuario"].HeaderText = "Usuario";
            
            if (dgvKardex.Columns.Contains("kardex_id"))
                dgvKardex.Columns["kardex_id"].Visible = false;

            if (dgvKardex.Columns.Contains("producto_id"))
                dgvKardex.Columns["producto_id"].Visible = false;

            if (dgvKardex.Columns.Contains("documento"))
            {
                dgvKardex.Columns["documento"].HeaderText = "Documento";
                dgvKardex.Columns["documento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }


            // Ocultar IDs (para que no ensucie la UI)
            if (dgvKardex.Columns.Contains("kardex_id"))
                dgvKardex.Columns["kardex_id"].Visible = false;

            if (dgvKardex.Columns.Contains("producto_id"))
                dgvKardex.Columns["producto_id"].Visible = false;

            if (dgvKardex.Columns.Contains("documento"))
            {
                dgvKardex.Columns["documento"].HeaderText = "Documento";
                dgvKardex.Columns["documento"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }


            // (opcional) centra tipo
            if (dgvKardex.Columns.Contains("tipo_movimiento"))
                dgvKardex.Columns["tipo_movimiento"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void EstilizarBotones()
        {
            EstiloBoton(btnFiltrar, Color.FromArgb(0, 123, 255));     
            EstiloBoton(btnRefrescar, Color.DarkSlateBlue);           
        }

        private void EstiloBoton(Button btn, Color color)
        {
            if (btn == null) return;

            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Height = Math.Max(btn.Height, 40);
            btn.Cursor = Cursors.Hand;

            Color hover = Color.FromArgb(
                Math.Min(color.R + 15, 255),
                Math.Min(color.G + 15, 255),
                Math.Min(color.B + 15, 255)
            );

            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = color;
        }

        private void EstilizarListaBuscador()
        {
            if (lstProductos == null) return;

            
            lstProductos.BorderStyle = BorderStyle.FixedSingle;
            lstProductos.BackColor = Color.White;
            lstProductos.ForeColor = Color.FromArgb(30, 30, 30);

            lstProductos.IntegralHeight = false;
            lstProductos.ItemHeight = 22;

            lstProductos.BringToFront();
        }


        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormKardex_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvKardex);

        }

        private void lstProductos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }
    }
}
