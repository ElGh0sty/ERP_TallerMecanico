using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormOrden : Form
    {
        Conexion con = new Conexion();
        DataTable dtVehiculos;

        // ⚠️ AJUSTA ESTE ID AL USUARIO LOGUEADO
        long recepcionistaId = 9; // admin

        public FormOrden()
        {
            InitializeComponent();
            CargarVehiculos();
            CargarMecanicos();
        }

        // ================== VEHÍCULOS ==================
        private void CargarVehiculos()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    v.id AS vehiculo_id,
    (v.placa + ' - ' + c.nombre) AS vehiculo_mostrar,
    c.nombre AS cliente,
    c.numero_documento,
    v.placa,
    v.tipo
FROM Vehiculos v
INNER JOIN Clientes c ON v.cliente_id = c.id";


                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                dtVehiculos = new DataTable();
                da.Fill(dtVehiculos);

                cmbVehiculo.DataSource = dtVehiculos;
                cmbVehiculo.DisplayMember = "vehiculo_mostrar";
                cmbVehiculo.ValueMember = "vehiculo_id";
                cmbVehiculo.SelectedIndex = -1;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar vehículos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void cmbVehiculo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 🔒 Protección total
            if (cmbVehiculo.SelectedItem == null)
                return;

            if (!(cmbVehiculo.SelectedItem is DataRowView row))
                return;

            txtCliente.Text = row["cliente"].ToString();
            txtDocumento.Text = row["numero_documento"].ToString();
            txtPlaca.Text = row["placa"].ToString();
            txtTipoVehiculo.Text = row["tipo"].ToString();


            GenerarNombreOrden();
        }


        // ================== MECÁNICOS ==================
        private void CargarMecanicos()
        {
            try
            {
                con.Abrir();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT id, nombre_usuario FROM Usuarios WHERE rol_id = 3",
                    con.leer
                );

                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbMecanico.DataSource = dt;
                cmbMecanico.DisplayMember = "nombre_usuario";
                cmbMecanico.ValueMember = "id";
                cmbMecanico.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar mecánicos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        // ================== NOMBRE ORDEN ==================
        private void GenerarNombreOrden()
        {
            try
            {
                // 🔒 Protección REAL
                if (cmbVehiculo.SelectedValue == null)
                    return;

                if (!(cmbVehiculo.SelectedValue is long vehiculoId))
                    return;

                string cliente = txtCliente.Text.Trim();
                string tipo = txtTipoVehiculo.Text.Trim();

                con.Abrir();

                SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM OrdenesTrabajo WHERE vehiculo_id = @vehiculo",
                    con.leer
                );

                cmd.Parameters.Add("@vehiculo", SqlDbType.BigInt).Value = vehiculoId;

                int total = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                txtNombreOrden.Text = $"{cliente}-{tipo}-{total}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar nombre de orden: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }


        // ================== GUARDAR ORDEN ==================
        private void btnGuardarOrden_Click(object sender, EventArgs e)
        {
            if (cmbVehiculo.SelectedIndex == -1 ||
                cmbMecanico.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Completa todos los datos obligatorios.");
                return;
            }

            try
            {
                con.Abrir();

                string sql = @"
INSERT INTO OrdenesTrabajo
(vehiculo_id, mecanico_id, recepcionista_id, fecha_ingreso, estado)
VALUES
(@vehiculo, @mecanico, @recepcionista, GETDATE(), @estado)";

                SqlCommand cmd = new SqlCommand(sql, con.leer);

                cmd.Parameters.Add("@vehiculo", SqlDbType.BigInt).Value = cmbVehiculo.SelectedValue;
                cmd.Parameters.Add("@mecanico", SqlDbType.BigInt).Value = cmbMecanico.SelectedValue;
                cmd.Parameters.Add("@recepcionista", SqlDbType.BigInt).Value = recepcionistaId;
                cmd.Parameters.Add("@estado", SqlDbType.NVarChar).Value = "Ingresado";

                cmd.ExecuteNonQuery();

                MessageBox.Show("✅ Orden de trabajo creada correctamente");
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la orden: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void LimpiarFormulario()
        {
            cmbVehiculo.SelectedIndex = -1;
            cmbMecanico.SelectedIndex = -1;

            txtCliente.Clear();
            txtDocumento.Clear();
            txtPlaca.Clear();
            txtTipoVehiculo.Clear();
            txtNombreOrden.Clear();
            txtDescripcion.Clear();
        }
    }
}




