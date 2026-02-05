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

        public FormCatalogo(string rol)
        {
            InitializeComponent();
            CargarCatalogoVehiculos();
            rolUsuario = rol;
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

                dgvVehiculos.Columns["vehiculo_id"].Visible = false;
                dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
        private void dgvVehiculos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            long vehiculoId = Convert.ToInt64(
                dgvVehiculos.Rows[e.RowIndex].Cells["vehiculo_id"].Value
            );

            
            FormRegVehi frm = new FormRegVehi(rolUsuario);
            frm.Tag = vehiculoId; 
            frm.ShowDialog();

            
            CargarCatalogoVehiculos(txtBuscar.Text.Trim());
        }

        
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvVehiculos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un vehículo.");
                return;
            }

            long vehiculoId = Convert.ToInt64(
                dgvVehiculos.SelectedRows[0].Cells["vehiculo_id"].Value
            );

            DialogResult r = MessageBox.Show(
                "¿Está seguro de eliminar este registro?",
                "Confirmar",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (r != DialogResult.Yes) return;

            try
            {
                con.Abrir();
                string sql = "DELETE FROM Vehiculos WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", vehiculoId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Vehículo eliminado.");
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
    }
}

