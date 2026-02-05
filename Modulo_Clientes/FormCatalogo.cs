using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormCatalogo : Form
    {
        Conexion con = new Conexion();

        public FormCatalogo()
        {
            InitializeComponent();
            CargarCatalogoVehiculos();
        }

        private void CargarCatalogoVehiculos()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT
    v.id AS vehiculo_id,
    c.nombre AS duenio,
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
ORDER BY c.nombre, v.placa";

                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvVehiculos.DataSource = dt;

                // Ajustes visuales
                dgvVehiculos.Columns["vehiculo_id"].Visible = false;
                dgvVehiculos.Columns["duenio"].HeaderText = "Dueño";
                dgvVehiculos.Columns["tipo_documento"].HeaderText = "Tipo Doc";
                dgvVehiculos.Columns["numero_documento"].HeaderText = "Documento";
                dgvVehiculos.Columns["placa"].HeaderText = "Placa";
                dgvVehiculos.Columns["marca"].HeaderText = "Marca";
                dgvVehiculos.Columns["modelo"].HeaderText = "Modelo";
                dgvVehiculos.Columns["tipo"].HeaderText = "Tipo";
                dgvVehiculos.Columns["año"].HeaderText = "Año";
                dgvVehiculos.Columns["chasis_vin"].HeaderText = "Chasis / VIN";
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
    }
}

