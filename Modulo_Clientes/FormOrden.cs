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

        long recepcionistaId = 9;
        private string rolUsuario;

        public FormOrden(string rolUsuario)
        {
            InitializeComponent();
            CargarVehiculos();
            CargarMecanicos();
            this.rolUsuario = rolUsuario;
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

        private void GenerarNombreOrden()
        {
            try
            {
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
            cmbVehiculo.SelectedIndex = -1;
            cmbMecanico.SelectedIndex = -1;

            txtCliente.Clear();
            txtDocumento.Clear();
            txtPlaca.Clear();
            txtTipoVehiculo.Clear();
            txtNombreOrden.Clear();
            txtDescripcion.Clear();
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


        private void txtNombreOrden_TextChanged(object sender, EventArgs e)
        {

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
                        break;
                    }
                }
            }
        }
    }
}




