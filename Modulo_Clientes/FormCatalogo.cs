using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormCatalogo : Form
    {
        Conexion con = new Conexion();
        DataTable dtVehiculos;
        private string rolUsuario;
        private Form1 formPrincipal;

        public FormCatalogo(Form1 principal, string rol)
        {
            InitializeComponent();

            formPrincipal = principal;
            rolUsuario = rol;
            DataGridViewEstilo.AplicarEstiloDashboard(dgvVehiculos);
            CargarCatalogoVehiculos();
        }




        private void CargarCatalogoVehiculos(string filtro = "")
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT
    v.id AS vehiculo_id,
    c.nombre AS dueño,
    c.tipo_documento,
    c.numero_documento,
    v.placa,
    v.marca,
    v.modelo,
    v.tipo,
    v.[año],
    v.chasis_vin
FROM Vehiculos v
INNER JOIN Clientes c ON c.id = v.cliente_id
WHERE
    c.nombre LIKE @filtro OR
    v.placa LIKE @filtro
ORDER BY c.nombre, v.placa";

                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.SelectCommand.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                dtVehiculos = new DataTable();
                da.Fill(dtVehiculos);

                dgvVehiculos.DataSource = dtVehiculos;

                if (dgvVehiculos.Columns.Contains("vehiculo_id"))
                    dgvVehiculos.Columns["vehiculo_id"].Visible = false;
                AgregarBotonesAccionGrid();
                EstilizarGridCompleto();
                ConfigurarColumnasGrid();
                EstilizarBotonesAccionGrid();
                



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando catálogo: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarCatalogoVehiculos(txtBuscar.Text.Trim());
        }

        //Esta es una prueba no esta ACTIVA

        private void EliminarVehiculo(long vehiculoId)
        {
            if (MessageBox.Show(
                "¿Seguro que deseas eliminar este vehículo?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();

                // ✅ Validar si tiene órdenes asociadas
                string sqlValida = "SELECT COUNT(*) FROM OrdenesTrabajo WHERE vehiculo_id = @id";
                SqlCommand cmdValida = new SqlCommand(sqlValida, con.leer);
                cmdValida.Parameters.AddWithValue("@id", vehiculoId);

                int cantOT = Convert.ToInt32(cmdValida.ExecuteScalar());

                if (cantOT > 0)
                {
                    MessageBox.Show(
                        "No se puede eliminar este vehículo porque tiene órdenes de trabajo registradas.\n" +
                        "Si necesitas removerlo, primero elimina o reasigna sus órdenes.",
                        "Acción no permitida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                // Si no tiene órdenes, se elimina
                string sqlDel = "DELETE FROM Vehiculos WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sqlDel, con.leer);
                cmd.Parameters.AddWithValue("@id", vehiculoId);

                int filas = cmd.ExecuteNonQuery();

                MessageBox.Show(filas > 0 ? "✅ Vehículo eliminado correctamente." : "No se encontró el vehículo.");
                CargarCatalogoVehiculos(txtBuscar.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }


        private void EditarVehiculo(long vehiculoId)
        {
            formPrincipal.AbrirFormularioEnPanel(new FormRegVehi(formPrincipal, rolUsuario, vehiculoId));
        }


        private void EstilizarGridCompleto()
        {
            dgvVehiculos.AllowUserToAddRows = false;
            dgvVehiculos.AllowUserToDeleteRows = false;
            dgvVehiculos.AllowUserToResizeRows = false;
            dgvVehiculos.ReadOnly = true;

            dgvVehiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVehiculos.MultiSelect = false;
            dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvVehiculos.ScrollBars = ScrollBars.Both;
            dgvVehiculos.DefaultCellStyle.WrapMode = DataGridViewTriState.False;


            dgvVehiculos.RowHeadersVisible = false;
            dgvVehiculos.BorderStyle = BorderStyle.None;
            dgvVehiculos.BackgroundColor = System.Drawing.Color.White;

            dgvVehiculos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvVehiculos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvVehiculos.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);

            dgvVehiculos.EnableHeadersVisualStyles = false;
            dgvVehiculos.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(24, 24, 28);
            dgvVehiculos.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvVehiculos.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            dgvVehiculos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvVehiculos.ColumnHeadersHeight = 45;

            dgvVehiculos.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11.5F);
            dgvVehiculos.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(35, 35, 35);
            dgvVehiculos.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvVehiculos.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvVehiculos.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(247, 247, 250);

            dgvVehiculos.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(220, 235, 255);
            dgvVehiculos.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(10, 10, 10);

            dgvVehiculos.RowTemplate.Height = 45;
        }

        private void ConfigurarColumnasGrid()
        {
            // Ocultar ID (debe existir aunque esté oculto)
            if (dgvVehiculos.Columns.Contains("vehiculo_id"))
                dgvVehiculos.Columns["vehiculo_id"].Visible = false;

            // Headers más bonitos (ajusta según tus columnas reales)
            if (dgvVehiculos.Columns.Contains("cliente")) dgvVehiculos.Columns["cliente"].HeaderText = "Cliente";
            if (dgvVehiculos.Columns.Contains("placa")) dgvVehiculos.Columns["placa"].HeaderText = "Placa";
            if (dgvVehiculos.Columns.Contains("marca")) dgvVehiculos.Columns["marca"].HeaderText = "Marca";
            if (dgvVehiculos.Columns.Contains("modelo")) dgvVehiculos.Columns["modelo"].HeaderText = "Modelo";
            if (dgvVehiculos.Columns.Contains("tipo")) dgvVehiculos.Columns["tipo"].HeaderText = "Tipo";
            if (dgvVehiculos.Columns.Contains("año")) dgvVehiculos.Columns["año"].HeaderText = "Año";
            if (dgvVehiculos.Columns.Contains("kilometraje_actual")) dgvVehiculos.Columns["kilometraje_actual"].HeaderText = "Km";
        }


        private void AgregarBotonesAccionGrid()
        {
            // Evitar duplicados al recargar
            if (!dgvVehiculos.Columns.Contains("btnEditar"))
            {
                DataGridViewButtonColumn btnEditar = new DataGridViewButtonColumn();
                btnEditar.Name = "btnEditar";
                btnEditar.HeaderText = "";
                btnEditar.Text = "Editar";
                btnEditar.UseColumnTextForButtonValue = true;
                btnEditar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnEditar.Width = 95;
                dgvVehiculos.Columns.Add(btnEditar);
            }

            if (!dgvVehiculos.Columns.Contains("btnEliminar"))
            {
                DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn();
                btnEliminar.Name = "btnEliminar";
                btnEliminar.HeaderText = "";
                btnEliminar.Text = "Eliminar";
                btnEliminar.UseColumnTextForButtonValue = true;
                btnEliminar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnEliminar.Width = 95;
                dgvVehiculos.Columns.Add(btnEliminar);
            }
        }

        private void EstilizarBotonesAccionGrid()
        {
            if (dgvVehiculos.Columns.Contains("btnEditar"))
            {
                var c = dgvVehiculos.Columns["btnEditar"] as DataGridViewButtonColumn;
                c.FlatStyle = FlatStyle.Flat;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(230, 242, 255);
                c.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(0, 84, 166);
                c.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(210, 230, 255);
                c.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(0, 60, 120);
                c.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                c.Width = 95;
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }

            if (dgvVehiculos.Columns.Contains("btnEliminar"))
            {
                var c = dgvVehiculos.Columns["btnEliminar"] as DataGridViewButtonColumn;
                c.FlatStyle = FlatStyle.Flat;
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                c.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
                c.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(160, 0, 0);
                c.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 210, 210);
                c.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(120, 0, 0);
                c.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
                c.Width = 95;
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
        }

        // suaviza el render de botones
        private void dgvVehiculos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvVehiculos.Columns[e.ColumnIndex].Name;
            if (colName == "btnEditar" || colName == "btnEliminar")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                e.Handled = true;
            }
        }

        private void dgvVehiculos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvVehiculos.Columns[e.ColumnIndex].Name;

            // Asegúrate de que tu columna ID se llame "vehiculo_id"
            long vehiculoId = Convert.ToInt64(dgvVehiculos.Rows[e.RowIndex].Cells["vehiculo_id"].Value);

            if (colName == "btnEditar")
            {
                EditarVehiculo(vehiculoId);   
            }
            else if (colName == "btnEliminar")
            {
                EliminarVehiculo(vehiculoId); 
            }
        }

        private void FormCatalogo_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvVehiculos);
        }
    }
}

