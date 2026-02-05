using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormRegVehi : Form
    {
        Conexion con = new Conexion();
        DataTable dtClientes; // Lo guardamos a nivel de clase para buscar en él

        public FormRegVehi()
        {
            InitializeComponent();
            CargarClientesCompleto();
            CargarTiposVehiculo();
            cmbDuenio.SelectedIndexChanged += new EventHandler(cmbDuenio_SelectedIndexChanged);
        }


        private void CargarClientesCompleto()
        {
            try
            {
                con.Abrir();
                // Traemos todos los datos que necesitamos mostrar automáticamente
                string query = "SELECT id, nombre, tipo_documento, numero_documento FROM Clientes ORDER BY nombre ASC";
                SqlDataAdapter da = new SqlDataAdapter(query, con.leer);
                dtClientes = new DataTable();
                da.Fill(dtClientes);

                cmbDuenio.DataSource = dtClientes;
                cmbDuenio.DisplayMember = "nombre";
                cmbDuenio.ValueMember = "id";

                cmbDuenio.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { con.Cerrar(); }
        }

        private void CargarTiposVehiculo()
        {
            try
            {
                con.Abrir();
                string sql = "SELECT nombre FROM TiposVehiculo WHERE activo = 1 ORDER BY nombre";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbTipoVehiculo.DataSource = dt;
                cmbTipoVehiculo.DisplayMember = "nombre";
                cmbTipoVehiculo.ValueMember = "nombre";
                cmbTipoVehiculo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando tipos de vehículo: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }


        // ESTO HACE LA MAGIA: Se dispara cuando eliges un cliente
        private void cmbDuenio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDuenio.SelectedIndex != -1 && dtClientes != null)
            {
                // Obtenemos la fila seleccionada del DataTable
                DataRowView clienteSeleccionado = (DataRowView)cmbDuenio.SelectedItem;

                // Llenamos los TextBox automáticamente con los nombres exactos de tu tabla Clientes
                txtTipoDoc.Text = clienteSeleccionado["tipo_documento"].ToString();
                txtDocumento.Text = clienteSeleccionado["numero_documento"].ToString();
            }
            else
            {
                txtTipoDoc.Clear();
                txtDocumento.Clear();
            }
        }

        private void btnGuardarVehiculo_Click(object sender, EventArgs e)
        {
            if (cmbTipoVehiculo.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un tipo de vehículo.");
                return;
            }

            if (cmbDuenio.SelectedValue == null || string.IsNullOrWhiteSpace(txtChasis.Text))
            {
                MessageBox.Show("Faltan datos críticos (Dueño o Chasis/VIN).");
                return;
            }

            int kilometraje = 0;
            int.TryParse(txtKilometraje.Text, out kilometraje);

            try
            {
                con.Abrir();
                // Añadimos chasis_vin a tu INSERT
                string sql = @"
INSERT INTO Vehiculos
(cliente_id, placa, marca, modelo, tipo, [año], chasis_vin, kilometraje_actual)
VALUES
(@cliente_id, @placa, @marca, @modelo, @tipo, @anio, @chasis, @kilometraje)";

                SqlCommand cmd = new SqlCommand(sql, con.leer);

                cmd.Parameters.Add("@cliente_id", SqlDbType.BigInt).Value = cmbDuenio.SelectedValue;
                cmd.Parameters.Add("@placa", SqlDbType.Char, 8).Value = txtPlaca.Text.Trim().ToUpper();
                cmd.Parameters.Add("@marca", SqlDbType.NVarChar, 255).Value = txtMarca.Text.Trim();
                cmd.Parameters.Add("@modelo", SqlDbType.NVarChar, 255).Value = txtModelo.Text.Trim();
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 10).Value = cmbTipoVehiculo.SelectedValue;
                cmd.Parameters.Add("@anio", SqlDbType.Int).Value = int.Parse(txtAño.Text);
                cmd.Parameters.Add("@chasis", SqlDbType.NVarChar, 17).Value = txtChasis.Text.Trim().ToUpper();
                cmd.Parameters.Add("@kilometraje", SqlDbType.Int).Value = kilometraje;

                cmd.ExecuteNonQuery();

                MessageBox.Show("¡Vehículo registrado!");
                Limpiar();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { con.Cerrar(); }
        }

        private void Limpiar()
        {
            // Limpia todo para el siguiente registro
            foreach (Control c in this.Controls) { if (c is TextBox) (c as TextBox).Clear(); }
            cmbDuenio.SelectedIndex = -1;
        }

        private void txtKilometraje_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }   

        private void txtModelo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
