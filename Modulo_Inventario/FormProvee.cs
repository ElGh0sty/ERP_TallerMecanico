using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormProvee : Form
    {
        private readonly Conexion con = new Conexion();
        private readonly DataTable dt = new DataTable();
        private long proveedorIdSeleccionado = 0;

        public FormProvee()
        {
            InitializeComponent();

            ConfigurarGrid();
            AplicarEstilos();

            txtBuscar.TextChanged += (s, e) => CargarProveedores(txtBuscar.Text);

            dgvProveedores.CellClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (!(dgvProveedores.Rows[e.RowIndex].DataBoundItem is DataRowView rv)) return;
                proveedorIdSeleccionado = Convert.ToInt64(rv["id"]);
            };

            dgvProveedores.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                AbrirPopupEditar();
            };

            btnAgregar.Click += (s, e) => AbrirPopupNuevo();
            btnEditar.Click += (s, e) => AbrirPopupEditar();

            CargarProveedores("");
        }

        private void AplicarEstilos()
        {
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 10F);

            txtBuscar.BorderStyle = BorderStyle.FixedSingle;

            EstiloBoton(btnAgregar, Color.FromArgb(0, 123, 255));
            EstiloBoton(btnEditar, Color.DarkSlateBlue);
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

        private void ConfigurarGrid()
        {
            dgvProveedores.AllowUserToAddRows = false;
            dgvProveedores.AllowUserToDeleteRows = false;
            dgvProveedores.AllowUserToResizeRows = false;
            dgvProveedores.ReadOnly = true;

            dgvProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProveedores.MultiSelect = false;

            dgvProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvProveedores.RowHeadersVisible = false;

            dgvProveedores.BorderStyle = BorderStyle.None;
            dgvProveedores.BackgroundColor = Color.White;

            dgvProveedores.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProveedores.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvProveedores.GridColor = Color.FromArgb(230, 230, 230);

            dgvProveedores.EnableHeadersVisualStyles = false;
            dgvProveedores.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvProveedores.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProveedores.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProveedores.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvProveedores.ColumnHeadersHeight = 40;

            dgvProveedores.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvProveedores.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvProveedores.DefaultCellStyle.BackColor = Color.White;
            dgvProveedores.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvProveedores.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);

            dgvProveedores.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvProveedores.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);

            dgvProveedores.RowTemplate.Height = 38;

            // Formateo después del bind
            dgvProveedores.DataBindingComplete -= dgvProveedores_DataBindingComplete;
            dgvProveedores.DataBindingComplete += dgvProveedores_DataBindingComplete;

            dgvProveedores.RowPrePaint -= dgvProveedores_RowPrePaint;
            dgvProveedores.RowPrePaint += dgvProveedores_RowPrePaint;

        }

        private void dgvProveedores_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Opcional: centrar columnas tipo bool
            if (dgvProveedores.Columns.Contains("estado"))
                dgvProveedores.Columns["estado"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (dgvProveedores.Columns.Contains("contribuyente_especial"))
                dgvProveedores.Columns["contribuyente_especial"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Opcional: ancho más “bonito”
            if (dgvProveedores.Columns.Contains("ruc"))
                dgvProveedores.Columns["ruc"].FillWeight = 90;

            if (dgvProveedores.Columns.Contains("nombre_empresa"))
                dgvProveedores.Columns["nombre_empresa"].FillWeight = 170;

            if (dgvProveedores.Columns.Contains("contacto_nombre"))
                dgvProveedores.Columns["contacto_nombre"].FillWeight = 120;

            if (dgvProveedores.Columns.Contains("telefono"))
                dgvProveedores.Columns["telefono"].FillWeight = 90;

            if (dgvProveedores.Columns.Contains("email"))
                dgvProveedores.Columns["email"].FillWeight = 140;

            if (dgvProveedores.Columns.Contains("estado"))
                dgvProveedores.Columns["estado"].FillWeight = 60;

            if (dgvProveedores.Columns.Contains("contribuyente_especial"))
                dgvProveedores.Columns["contribuyente_especial"].FillWeight = 70;
        }

        private void dgvProveedores_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvProveedores.Rows[e.RowIndex];
            if (row.Cells["estado"]?.Value == null) return;

            bool activo = Convert.ToBoolean(row.Cells["estado"].Value);

            if (!activo)
            {
                row.DefaultCellStyle.ForeColor = Color.FromArgb(140, 140, 140);
                row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            }
        }



        private void CargarProveedores(string filtro)
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    id,
    ruc,
    nombre_empresa,
    contacto_nombre,
    telefono,
    email,
    estado,
    contribuyente_especial
FROM dbo.Proveedores
WHERE
    (@f = '' OR ruc LIKE '%' + @f + '%' OR nombre_empresa LIKE '%' + @f + '%' OR contacto_nombre LIKE '%' + @f + '%')
ORDER BY nombre_empresa;";

                using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.AddWithValue("@f", (filtro ?? "").Trim());

                    dt.Clear();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }

                dgvProveedores.DataSource = dt;

                if (dgvProveedores.Columns.Contains("id"))
                    dgvProveedores.Columns["id"].Visible = false;

                if (dgvProveedores.Columns.Contains("ruc"))
                    dgvProveedores.Columns["ruc"].HeaderText = "RUC";

                if (dgvProveedores.Columns.Contains("nombre_empresa"))
                    dgvProveedores.Columns["nombre_empresa"].HeaderText = "Empresa";

                if (dgvProveedores.Columns.Contains("contacto_nombre"))
                    dgvProveedores.Columns["contacto_nombre"].HeaderText = "Contacto";

                if (dgvProveedores.Columns.Contains("telefono"))
                    dgvProveedores.Columns["telefono"].HeaderText = "Teléfono";

                if (dgvProveedores.Columns.Contains("email"))
                    dgvProveedores.Columns["email"].HeaderText = "Email";

                if (dgvProveedores.Columns.Contains("estado"))
                    dgvProveedores.Columns["estado"].HeaderText = "Activo";

                if (dgvProveedores.Columns.Contains("contribuyente_especial"))
                    dgvProveedores.Columns["contribuyente_especial"].HeaderText = "C. Esp.";

                lblTotal.Text = $"Proveedores: {dt.Rows.Count}";
                proveedorIdSeleccionado = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando proveedores: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void AbrirPopupNuevo()
        {
            using (var pop = new FormProveePopup(null)) 
            {
                if (pop.ShowDialog() == DialogResult.OK)
                    CargarProveedores(txtBuscar.Text);
            }
        }

        private void AbrirPopupEditar()
        {
            if (proveedorIdSeleccionado <= 0)
            {
                MessageBox.Show("Selecciona un proveedor (o doble clic en la fila).");
                return;
            }

            using (var pop = new FormProveePopup(proveedorIdSeleccionado))
            {
                if (pop.ShowDialog() == DialogResult.OK)
                    CargarProveedores(txtBuscar.Text);
            }
        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }
    }
}
