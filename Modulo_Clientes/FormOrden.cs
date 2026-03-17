using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormOrden : Form
    {
        Conexion con = new Conexion();
        DataTable dtVehiculos;

        long recepcionistaId = 9;
        private string rolUsuario;
        private ErrorProvider errorProvider;
        private ValidadorTiempoReal validador;

        public FormOrden(string rolUsuario)
        {
            InitializeComponent();
            this.rolUsuario = rolUsuario;

            InicializarValidaciones();
            ConfigurarCombos();
            CargarVehiculos();
            CargarMecanicos();
        }

        private void InicializarValidaciones()
        {
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            validador = new ValidadorTiempoReal(errorProvider);

            // Configurar ComboBox como solo selección
            cmbVehiculo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMecanico.DropDownStyle = ComboBoxStyle.DropDownList;

            // Eventos de validación en tiempo real
            cmbVehiculo.SelectedIndexChanged += (s, e) => ValidarVehiculo();
            cmbMecanico.SelectedIndexChanged += (s, e) => ValidarMecanico();
            txtDescripcion.TextChanged += (s, e) => ValidarDescripcion();
            txtDescripcion.Leave += (s, e) => ValidarDescripcion();

            // Validar al perder foco
            cmbVehiculo.Leave += (s, e) => ValidarVehiculo();
            cmbMecanico.Leave += (s, e) => ValidarMecanico();
        }

        private void ConfigurarCombos()
        {
            // Los ComboBox ya están configurados en el diseñador como DropDownList
            // pero lo reforzamos aquí
            cmbVehiculo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMecanico.DropDownStyle = ComboBoxStyle.DropDownList;
        }

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

                // Agregar opción por defecto
                DataRow filaVacia = dtVehiculos.NewRow();
                filaVacia["vehiculo_id"] = 0;
                filaVacia["vehiculo_mostrar"] = "Seleccione un vehículo";
                filaVacia["cliente"] = "";
                filaVacia["numero_documento"] = "";
                filaVacia["placa"] = "";
                filaVacia["tipo"] = "";
                dtVehiculos.Rows.InsertAt(filaVacia, 0);

                cmbVehiculo.DataSource = dtVehiculos;
                cmbVehiculo.DisplayMember = "vehiculo_mostrar";
                cmbVehiculo.ValueMember = "vehiculo_id";
                cmbVehiculo.SelectedIndex = 0; // Seleccionar la opción por defecto
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
            if (cmbVehiculo.SelectedIndex <= 0 || cmbVehiculo.SelectedItem == null)
            {
                // Si es la opción por defecto, limpiar campos
                txtCliente.Clear();
                txtDocumento.Clear();
                txtPlaca.Clear();
                txtTipoVehiculo.Clear();
                txtNombreOrden.Clear();
                return;
            }

            if (!(cmbVehiculo.SelectedItem is DataRowView row))
                return;

            txtCliente.Text = row["cliente"].ToString();
            txtDocumento.Text = row["numero_documento"].ToString();
            txtPlaca.Text = row["placa"].ToString();
            txtTipoVehiculo.Text = row["tipo"].ToString();

            GenerarNombreOrden();
        }

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

                // Agregar opción por defecto
                DataRow filaVacia = dt.NewRow();
                filaVacia["id"] = 0;
                filaVacia["nombre_usuario"] = "Seleccione un mecánico";
                dt.Rows.InsertAt(filaVacia, 0);

                cmbMecanico.DataSource = dt;
                cmbMecanico.DisplayMember = "nombre_usuario";
                cmbMecanico.ValueMember = "id";
                cmbMecanico.SelectedIndex = 0;
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

        private void GenerarNombreOrden()
        {
            try
            {
                if (cmbVehiculo.SelectedIndex <= 0 || cmbVehiculo.SelectedValue == null)
                    return;

                if (!long.TryParse(cmbVehiculo.SelectedValue.ToString(), out long vehiculoId) || vehiculoId <= 0)
                    return;

                string cliente = txtCliente.Text.Trim();
                string tipo = txtTipoVehiculo.Text.Trim();

                if (string.IsNullOrWhiteSpace(cliente) || string.IsNullOrWhiteSpace(tipo))
                    return;

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

        // ===== MÉTODOS DE VALIDACIÓN =====

        private bool ValidarVehiculo()
        {
            if (cmbVehiculo.SelectedIndex <= 0 || cmbVehiculo.SelectedValue == null)
            {
                validador.MarcarError(cmbVehiculo, "Seleccione un vehículo.");
                return false;
            }

            if (!long.TryParse(cmbVehiculo.SelectedValue.ToString(), out long vehiculoId) || vehiculoId <= 0)
            {
                validador.MarcarError(cmbVehiculo, "Seleccione un vehículo válido.");
                return false;
            }

            validador.MarcarOk(cmbVehiculo);
            return true;
        }

        private bool ValidarMecanico()
        {
            if (cmbMecanico.SelectedIndex <= 0 || cmbMecanico.SelectedValue == null)
            {
                validador.MarcarError(cmbMecanico, "Seleccione un mecánico.");
                return false;
            }

            if (!long.TryParse(cmbMecanico.SelectedValue.ToString(), out long mecanicoId) || mecanicoId <= 0)
            {
                validador.MarcarError(cmbMecanico, "Seleccione un mecánico válido.");
                return false;
            }

            validador.MarcarOk(cmbMecanico);
            return true;
        }

        private bool ValidarDescripcion()
        {
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                validador.MarcarError(txtDescripcion, "La descripción es obligatoria.");
                return false;
            }

            if (descripcion.Length < 5)
            {
                validador.MarcarError(txtDescripcion, "La descripción debe tener al menos 5 caracteres.");
                return false;
            }

            if (descripcion.Length > 500)
            {
                validador.MarcarError(txtDescripcion, "La descripción no puede exceder 500 caracteres.");
                return false;
            }

            validador.MarcarOk(txtDescripcion);
            return true;
        }

        private bool ValidarTodo()
        {
            return ValidarVehiculo() && ValidarMecanico() && ValidarDescripcion();
        }

        private void btnGuardarOrden_Click(object sender, EventArgs e)
        {
            // Validar todos los campos
            if (!ValidarTodo())
            {
                MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                con.Abrir();

                using (SqlTransaction tx = con.leer.BeginTransaction())
                {
                    try
                    {
                        string estadoInicial = "Ingresado";

                        string sql = @"
INSERT INTO OrdenesTrabajo
(vehiculo_id, recepcionista_id, mecanico_id, fecha_ingreso, estado, descripcion)
VALUES
(@vehiculo_id, @recepcionista_id, @mecanico_id, GETDATE(), @estado, @descripcion);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                        SqlCommand cmd = new SqlCommand(sql, con.leer, tx);

                        cmd.Parameters.Add("@vehiculo_id", SqlDbType.BigInt).Value = cmbVehiculo.SelectedValue;
                        cmd.Parameters.Add("@recepcionista_id", SqlDbType.BigInt).Value = recepcionistaId;
                        cmd.Parameters.Add("@mecanico_id", SqlDbType.BigInt).Value = cmbMecanico.SelectedValue;
                        cmd.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = estadoInicial;
                        cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar).Value = txtDescripcion.Text.Trim();

                        long ordenIdNueva = Convert.ToInt64(cmd.ExecuteScalar());

                        RegistrarHistorial(tx, ordenIdNueva, recepcionistaId,
                            "ESTADO", "Orden creada", $"Estado inicial: {estadoInicial}");

                        tx.Commit();

                        MessageBox.Show("✅ Orden de trabajo creada correctamente");
                        LimpiarFormulario();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
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
            cmbVehiculo.SelectedIndex = 0;
            cmbMecanico.SelectedIndex = 0;

            txtCliente.Clear();
            txtDocumento.Clear();
            txtPlaca.Clear();
            txtTipoVehiculo.Clear();
            txtNombreOrden.Clear();
            txtDescripcion.Clear();

            // Limpiar validaciones
            validador.MarcarOk(cmbVehiculo);
            validador.MarcarOk(cmbMecanico);
            validador.MarcarOk(txtDescripcion);
        }

        private void RegistrarHistorial(SqlTransaction tx, long ordenId, long? usuarioId, string tipo, string titulo, string detalle)
        {
            using (SqlCommand cmdH = new SqlCommand(
                "EXEC dbo.sp_historial_registrar @orden_id, @usuario_id, @tipo_evento, @titulo, @detalle",
                con.leer, tx))
            {
                cmdH.Parameters.AddWithValue("@orden_id", ordenId);
                cmdH.Parameters.AddWithValue("@usuario_id", (object)usuarioId ?? DBNull.Value);
                cmdH.Parameters.AddWithValue("@tipo_evento", tipo);
                cmdH.Parameters.AddWithValue("@titulo", titulo);
                cmdH.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                cmdH.ExecuteNonQuery();
            }
        }

        private void btnBuscadorVehiculosRegistrados_Click(object sender, EventArgs e)
        {
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Vehiculos);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long vehiculoId = Convert.ToInt64(fila["id"]);
                string placa = fila["placa"].ToString();
                string cliente = fila["cliente"].ToString();

                for (int i = 0; i < cmbVehiculo.Items.Count; i++)
                {
                    DataRowView item = cmbVehiculo.Items[i] as DataRowView;
                    if (item != null && Convert.ToInt64(item["vehiculo_id"]) == vehiculoId)
                    {
                        cmbVehiculo.SelectedIndex = i;

                        txtCliente.Text = item["cliente"].ToString();
                        txtDocumento.Text = item["numero_documento"].ToString();
                        txtPlaca.Text = item["placa"].ToString();
                        txtTipoVehiculo.Text = item["tipo"].ToString();

                        GenerarNombreOrden();
                        ValidarVehiculo(); // Re-validar después de seleccionar
                        break;
                    }
                }
            }
        }

        // Eventos del diseñador
        private void txtNombreOrden_TextChanged(object sender, EventArgs e) { }
    }
}

// Clase ValidadorTiempoReal (si no la tienes en un archivo separado)




