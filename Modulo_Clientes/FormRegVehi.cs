using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormRegVehi : Form
    {
        Conexion con = new Conexion();
        DataTable dtClientes;

        private Form1 formPrincipal;
        private long? vehiculoIdEditar = null;
        private string rolUsuario;


        public FormRegVehi(Form1 principal, string rolUsuario)
        {
            InitializeComponent();
            formPrincipal = principal;
            this.rolUsuario = rolUsuario;

            CargarClientesCompleto();
            CargarTiposVehiculo();

            cmbDuenio.SelectedIndexChanged += cmbDuenio_SelectedIndexChanged;

            txtPlaca.CharacterCasing = CharacterCasing.Upper;
            txtChasis.CharacterCasing = CharacterCasing.Upper;

            txtAño.KeyPress += SoloNumeros_KeyPress;
            txtPlaca.KeyPress += Placa_KeyPress;
            txtChasis.KeyPress += Vin_KeyPress;


        }



        public FormRegVehi(Form1 principal, string rolUsuario, long vehiculoId)
        {
            InitializeComponent();
            formPrincipal = principal;
            this.rolUsuario = rolUsuario;
            vehiculoIdEditar = vehiculoId;

            CargarClientesCompleto();
            CargarTiposVehiculo();

            cmbDuenio.SelectedIndexChanged += cmbDuenio_SelectedIndexChanged;

            CargarVehiculoParaEditar(vehiculoId);

            txtPlaca.CharacterCasing = CharacterCasing.Upper;
            txtChasis.CharacterCasing = CharacterCasing.Upper;

            txtAño.KeyPress += SoloNumeros_KeyPress;
            txtPlaca.KeyPress += Placa_KeyPress;
            txtChasis.KeyPress += Vin_KeyPress;


            btnGuardarVehiculo.Text = "Guardar edición"; 
        }


        private void CargarClientesCompleto()
        {
            try
            {
                con.Abrir();
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
                string sql = "SELECT nombre FROM TiposVehiculo WHERE activo = 1 ORDER BY id";
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

        private void cmbDuenio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDuenio.SelectedIndex != -1 && dtClientes != null && cmbDuenio.SelectedItem is DataRowView)
            {
                DataRowView clienteSeleccionado = (DataRowView)cmbDuenio.SelectedItem;

                txtTipoDoc.Text = clienteSeleccionado["tipo_documento"].ToString();
                txtDocumento.Text = clienteSeleccionado["numero_documento"].ToString();
            }
            else
            {
                txtTipoDoc.Clear();
                txtDocumento.Clear();
            }
        }

        // ✅ Carga los datos del vehículo (para editar)
        private void CargarVehiculoParaEditar(long vehiculoId)
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    cliente_id, placa, marca, modelo, tipo, [año], chasis_vin, kilometraje_actual
FROM Vehiculos
WHERE id = @id";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", vehiculoId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // ⚠️ Primero se setean combos (ya cargados)
                        cmbDuenio.SelectedValue = Convert.ToInt64(dr["cliente_id"]);
                        cmbTipoVehiculo.SelectedValue = dr["tipo"].ToString();

                        txtPlaca.Text = dr["placa"].ToString();
                        txtMarca.Text = dr["marca"].ToString();
                        txtModelo.Text = dr["modelo"].ToString();
                        txtAño.Text = dr["año"].ToString();
                        txtChasis.Text = dr["chasis_vin"].ToString();
                        txtKilometraje.Text = dr["kilometraje_actual"].ToString();

                        // Cambiar texto del botón (opcional)
                        btnGuardarVehiculo.Text = "Guardar cambios";
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el vehículo para editar.");
                        vehiculoIdEditar = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando vehículo: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void btnGuardarVehiculo_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario(out int anio, out int kilometraje))
                return;

            string placa = txtPlaca.Text.Trim().ToUpper();
            string vin = txtChasis.Text.Trim().ToUpper();

            try
            {
                con.Abrir();

                if (ExistePlaca(placa))
                {
                    MessageBox.Show("Ya existe un vehículo registrado con esa placa.");
                    txtPlaca.Focus();
                    return;
                }

                if (ExisteVIN(vin))
                {
                    MessageBox.Show("Ya existe un vehículo registrado con ese Chasis/VIN.");
                    txtChasis.Focus();
                    return;
                }

                string sql;

                if (vehiculoIdEditar == null)
                {
                    sql = @"
INSERT INTO Vehiculos
(cliente_id, placa, marca, modelo, tipo, [año], chasis_vin, kilometraje_actual)
VALUES
(@cliente_id, @placa, @marca, @modelo, @tipo, @anio, @chasis, @kilometraje)";
                }
                else
                {
                    sql = @"
UPDATE Vehiculos SET
    cliente_id = @cliente_id,
    placa = @placa,
    marca = @marca,
    modelo = @modelo,
    tipo = @tipo,
    [año] = @anio,
    chasis_vin = @chasis,
    kilometraje_actual = @kilometraje
WHERE id = @id";
                }

                SqlCommand cmd = new SqlCommand(sql, con.leer);

                cmd.Parameters.Add("@cliente_id", SqlDbType.BigInt).Value = cmbDuenio.SelectedValue;
                cmd.Parameters.Add("@placa", SqlDbType.Char, 8).Value = placa;
                cmd.Parameters.Add("@marca", SqlDbType.NVarChar, 255).Value = txtMarca.Text.Trim();
                cmd.Parameters.Add("@modelo", SqlDbType.NVarChar, 255).Value = txtModelo.Text.Trim();
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 30).Value = cmbTipoVehiculo.SelectedValue.ToString();
                cmd.Parameters.Add("@anio", SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("@chasis", SqlDbType.NVarChar, 17).Value = vin;
                cmd.Parameters.Add("@kilometraje", SqlDbType.Int).Value = kilometraje;

                if (vehiculoIdEditar != null)
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = vehiculoIdEditar.Value;

                cmd.ExecuteNonQuery();

                if (vehiculoIdEditar == null)
                {
                    MessageBox.Show("¡Vehículo registrado!");
                    Limpiar();
                }
                else
                {
                    MessageBox.Show("Cambios guardados correctamente.");
                    formPrincipal.AbrirFormularioEnPanel(new FormCatalogo(formPrincipal, rolUsuario));
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { con.Cerrar(); }
        }


        private void Limpiar()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox) (c as TextBox).Clear();
            }
            cmbDuenio.SelectedIndex = -1;
            cmbTipoVehiculo.SelectedIndex = -1;
        }

        private void txtKilometraje_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtModelo_TextChanged(object sender, EventArgs e)
        {
            
        }

        private bool ValidarFormulario(out int anio, out int kilometraje)
        {
            anio = 0;
            kilometraje = 0;

            // Dueño
            if (cmbDuenio.SelectedIndex == -1 || cmbDuenio.SelectedValue == null)
            {
                MessageBox.Show("Seleccione el dueño del vehículo.");
                cmbDuenio.Focus();
                return false;
            }

            // Tipo
            if (cmbTipoVehiculo.SelectedIndex == -1 || cmbTipoVehiculo.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un tipo de vehículo.");
                cmbTipoVehiculo.Focus();
                return false;
            }

            // Placa (en tu BD es char(8) y no permite null según tu captura)
            string placa = (txtPlaca.Text ?? "").Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(placa))
            {
                MessageBox.Show("Ingrese la placa.");
                txtPlaca.Focus();
                return false;
            }
            if (placa.Length < 6 || placa.Length > 8)
            {
                MessageBox.Show("La placa debe tener entre 6 y 8 caracteres.");
                txtPlaca.Focus();
                return false;
            }

            // Marca / Modelo (en tu BD permiten null, pero UX mejor pedirlos)
            string marca = (txtMarca.Text ?? "").Trim();
            if (marca.Length < 2)
            {
                MessageBox.Show("Ingrese una marca válida.");
                txtMarca.Focus();
                return false;
            }

            string modelo = (txtModelo.Text ?? "").Trim();
            if (modelo.Length < 1)
            {
                MessageBox.Show("Ingrese un modelo válido.");
                txtModelo.Focus();
                return false;
            }

            // Año
            if (!int.TryParse((txtAño.Text ?? "").Trim(), out anio))
            {
                MessageBox.Show("Ingrese un año válido.");
                txtAño.Focus();
                return false;
            }

            int anioActual = DateTime.Now.Year;
            if (anio < 1970 || anio > anioActual + 1) // margen por modelos próximos
            {
                MessageBox.Show($"El año debe estar entre 1970 y {anioActual + 1}.");
                txtAño.Focus();
                return false;
            }

            // Chasis/VIN (en tu BD es nvarchar(17) y lo estás tratando como VIN)
            string vin = (txtChasis.Text ?? "").Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(vin))
            {
                MessageBox.Show("Ingrese el Chasis/VIN.");
                txtChasis.Focus();
                return false;
            }
            if (vin.Length != 17)
            {
                MessageBox.Show("El Chasis/VIN debe tener exactamente 17 caracteres.");
                txtChasis.Focus();
                return false;
            }

            // Kilometraje (en tu BD int NOT NULL)
            string kmTxt = (txtKilometraje.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(kmTxt))
                kmTxt = "0";

            if (!int.TryParse(kmTxt, out kilometraje) || kilometraje < 0)
            {
                MessageBox.Show("Ingrese un kilometraje válido (0 o mayor).");
                txtKilometraje.Focus();
                return false;
            }

            return true;
        }

        private bool ExistePlaca(string placa)
        {
            // Excluir el mismo vehículo cuando editas
            string sql = vehiculoIdEditar == null
                ? "SELECT COUNT(*) FROM Vehiculos WHERE placa = @placa"
                : "SELECT COUNT(*) FROM Vehiculos WHERE placa = @placa AND id <> @id";

            using (SqlCommand cmd = new SqlCommand(sql, con.leer))
            {
                cmd.Parameters.Add("@placa", SqlDbType.Char, 8).Value = placa;
                if (vehiculoIdEditar != null)
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = vehiculoIdEditar.Value;

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private bool ExisteVIN(string vin)
        {
            string sql = vehiculoIdEditar == null
                ? "SELECT COUNT(*) FROM Vehiculos WHERE chasis_vin = @vin"
                : "SELECT COUNT(*) FROM Vehiculos WHERE chasis_vin = @vin AND id <> @id";

            using (SqlCommand cmd = new SqlCommand(sql, con.leer))
            {
                cmd.Parameters.Add("@vin", SqlDbType.NVarChar, 17).Value = vin;
                if (vehiculoIdEditar != null)
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = vehiculoIdEditar.Value;

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void Placa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;

            if (!char.IsControl(e.KeyChar) && txtPlaca.Text.Length >= 8)
                e.Handled = true;
        }

        private void Vin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;

            if (!char.IsControl(e.KeyChar) && txtChasis.Text.Length >= 17)
                e.Handled = true;
        }


    }
}

