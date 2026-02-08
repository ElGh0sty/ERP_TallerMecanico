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


        // ✅ Constructor para REGISTRAR NUEVO
        public FormRegVehi(Form1 principal, string rolUsuario)
        {
            InitializeComponent();
            formPrincipal = principal;
            this.rolUsuario = rolUsuario;

            CargarClientesCompleto();
            CargarTiposVehiculo();

            cmbDuenio.SelectedIndexChanged += cmbDuenio_SelectedIndexChanged;
        }

        // ✅ Constructor para EDITAR (recibe el ID del vehículo)
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

            if (!int.TryParse(txtAño.Text.Trim(), out int anio))
            {
                MessageBox.Show("Ingrese un año válido.");
                return;
            }

            int kilometraje = 0;
            int.TryParse(txtKilometraje.Text.Trim(), out kilometraje);

            try
            {
                con.Abrir();

                string sql;

                if (vehiculoIdEditar == null)
                {
                    // ✅ INSERT
                    sql = @"
INSERT INTO Vehiculos
(cliente_id, placa, marca, modelo, tipo, [año], chasis_vin, kilometraje_actual)
VALUES
(@cliente_id, @placa, @marca, @modelo, @tipo, @anio, @chasis, @kilometraje)";
                }
                else
                {
                    // ✅ UPDATE
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
                cmd.Parameters.Add("@placa", SqlDbType.Char, 8).Value = txtPlaca.Text.Trim().ToUpper();
                cmd.Parameters.Add("@marca", SqlDbType.NVarChar, 255).Value = txtMarca.Text.Trim();
                cmd.Parameters.Add("@modelo", SqlDbType.NVarChar, 255).Value = txtModelo.Text.Trim();
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 10).Value = cmbTipoVehiculo.SelectedValue;
                cmd.Parameters.Add("@anio", SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("@chasis", SqlDbType.NVarChar, 17).Value = txtChasis.Text.Trim().ToUpper();
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
                    MessageBox.Show("✅ Cambios guardados correctamente.");

                    // ✅ volver al catálogo y cerrar este form (porque reemplaza el panel)
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
            // vacío
        }
    }
}

