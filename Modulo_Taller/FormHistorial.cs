using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormHistorial : Form
    {
        private readonly long usuarioId;
        private readonly string rolUsuario;
        private long ordenIdActual = 0;

        Conexion con = new Conexion();

        public FormHistorial(long usuarioId, string rolUsuario)
        {
            InitializeComponent();

            this.usuarioId = usuarioId;
            this.rolUsuario = rolUsuario;

            ConfigurarFiltros();
            ConfigurarGrid();
            EstilizarGrid();

            CargarOrdenes();
            CargarHistorial(); 

            
            cmbOrdenes.SelectedIndexChanged += (s, e) => CargarHistorial();
            cmbTipo.SelectedIndexChanged += (s, e) => CargarHistorial();
            btnRefrescar.Click += (s, e) => CargarHistorial();
        }

       
        public FormHistorial()
        {
            InitializeComponent();
        }

        private void ConfigurarFiltros()
        {
            cmbOrdenes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("TODOS");
            cmbTipo.Items.Add("ESTADO");
            cmbTipo.Items.Add("ITEM");
            cmbTipo.Items.Add("TAREA");
            cmbTipo.Items.Add("NOVEDAD");
            cmbTipo.SelectedIndex = 0;
        }

        private void ConfigurarGrid()
        {
            dgvHistorial.AllowUserToAddRows = false;
            dgvHistorial.AllowUserToDeleteRows = false;
            dgvHistorial.AllowUserToResizeRows = false;
            dgvHistorial.ReadOnly = true;

            dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorial.MultiSelect = false;

            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.RowHeadersVisible = false;
        }

        private void EstilizarGrid()
        {
            dgvHistorial.BorderStyle = BorderStyle.None;
            dgvHistorial.BackgroundColor = Color.White;

            dgvHistorial.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvHistorial.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvHistorial.GridColor = Color.FromArgb(230, 230, 230);

            dgvHistorial.EnableHeadersVisualStyles = false;
            dgvHistorial.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvHistorial.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistorial.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvHistorial.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvHistorial.ColumnHeadersHeight = 40;

            dgvHistorial.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvHistorial.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvHistorial.DefaultCellStyle.BackColor = Color.White;
            dgvHistorial.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvHistorial.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);

            dgvHistorial.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvHistorial.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);

            dgvHistorial.RowTemplate.Height = 38;
        }

        private void CargarOrdenes()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    ot.id,
    CONCAT('OT #', ot.id, ' - ', ISNULL(v.placa,'SIN-PLACA'), ' - ', ISNULL(ot.estado,'(sin estado)')) AS display
FROM OrdenesTrabajo ot
LEFT JOIN Vehiculos v ON v.id = ot.vehiculo_id
ORDER BY ot.id DESC;";

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(sql, con.leer))
                {
                    da.Fill(dt);
                }

                cmbOrdenes.DisplayMember = "display";
                cmbOrdenes.ValueMember = "id";
                cmbOrdenes.DataSource = dt;

                cmbOrdenes.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar órdenes: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void CargarHistorial()
        {
            if (cmbOrdenes.SelectedValue == null) return;

            ordenIdActual = Convert.ToInt64(cmbOrdenes.SelectedValue);
            string tipo = cmbTipo.SelectedItem?.ToString() ?? "TODOS";

            try
            {
                con.Abrir();

                // ✅ Ajusta el nombre de tabla si le pusiste otro
                string sql = @"
SELECT 
    h.fecha,
    h.tipo_evento,
    h.titulo,
    h.detalle,
    ISNULL(u.nombre_usuario,'') AS usuario
FROM OrdenesTrabajo_Historial h
LEFT JOIN Usuarios u ON h.usuario_id = u.id
WHERE h.orden_id = @ordenId
  AND (@tipo = 'TODOS' OR h.tipo_evento = @tipo)
ORDER BY h.fecha DESC;";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@ordenId", ordenIdActual);
                cmd.Parameters.AddWithValue("@tipo", tipo);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                dgvHistorial.DataSource = dt;

                // headers
                if (dgvHistorial.Columns.Contains("fecha"))
                {
                    dgvHistorial.Columns["fecha"].HeaderText = "Fecha";
                    dgvHistorial.Columns["fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                }
                if (dgvHistorial.Columns.Contains("tipo_evento"))
                    dgvHistorial.Columns["tipo_evento"].HeaderText = "Tipo";
                if (dgvHistorial.Columns.Contains("titulo"))
                    dgvHistorial.Columns["titulo"].HeaderText = "Título";
                if (dgvHistorial.Columns.Contains("detalle"))
                    dgvHistorial.Columns["detalle"].HeaderText = "Detalle";
                if (dgvHistorial.Columns.Contains("usuario"))
                    dgvHistorial.Columns["usuario"].HeaderText = "Usuario";

                // Label opcional (si no tienes lblTotal, borra estas 2 líneas)
                if (lblTotal != null)
                    lblTotal.Text = $"Registros: {dt.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar historial: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void cmbOrdenes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
