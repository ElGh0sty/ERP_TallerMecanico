using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormOrdenTrabajo : Form
    {
        private readonly long usuarioId;

        Conexion con = new Conexion();
        private string rolUsuario;

        public FormOrdenTrabajo(long usuarioId,string rolUsuario)
        {
            InitializeComponent();
            CargarOrdenes();
            CargarEstados();
            EstiloGrid();
            this.usuarioId = usuarioId;
            this.rolUsuario = rolUsuario;
            DataGridViewEstilo.AplicarEstiloDashboard(dgvOrdenes);
        }

        private void CargarOrdenes()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    ot.id,
    v.placa,
    c.nombre AS cliente,
    u.nombre_usuario AS mecanico,
    ot.fecha_ingreso,
    ot.estado,
    ot.descripcion
FROM OrdenesTrabajo ot
INNER JOIN Vehiculos v ON ot.vehiculo_id = v.id
INNER JOIN Clientes c ON v.cliente_id = c.id
INNER JOIN Usuarios u ON ot.mecanico_id = u.id
WHERE 
    ISNULL(ot.facturada,0) = 0
    AND NOT EXISTS (
        SELECT 1 
        FROM dbo.Facturas f 
        WHERE f.orden_trabajo_id = ot.id
    )
ORDER BY ot.fecha_ingreso DESC;";

                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvOrdenes.DataSource = dt;

                if (dgvOrdenes.Columns.Contains("descripcion"))
                    dgvOrdenes.Columns["descripcion"].Visible = false;

                AgregarBotonEliminarOT();
                EstilizarGridOrdenes();
                EstilizarBotonEliminarOT();
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

        private void CargarEstados()
        {
            cmbEstado.Items.AddRange(new string[]
            {
                "Ingresado",
                "En Diagnóstico",
                "En Reparación",
                "Esperando Repuestos",
                "Terminado",
                "Entregado"
            });

            cmbEstado.SelectedIndex = 0;
        }

        private void EstiloGrid()
        {
            dgvOrdenes.ReadOnly = true;
            dgvOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenes.MultiSelect = false;
            dgvOrdenes.AllowUserToAddRows = false;
            dgvOrdenes.AllowUserToDeleteRows = false;
            dgvOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            if (dgvOrdenes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una orden.");
                return;
            }

            long ordenId = Convert.ToInt64(dgvOrdenes.SelectedRows[0].Cells["id"].Value);
            string nuevoEstado = cmbEstado.SelectedItem.ToString();

            try
            {
                con.Abrir();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE OrdenesTrabajo SET estado = @estado WHERE id = @id",
                    con.leer
                );

                cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@id", ordenId);
                string estadoAnterior = dgvOrdenes.SelectedRows[0].Cells["estado"].Value?.ToString() ?? "";

                cmd.ExecuteNonQuery();

                RegistrarHistorial(
    ordenId,
    usuarioId,
    "ESTADO",
    "Cambio de estado",
    $"Estado: {estadoAnterior} -> {nuevoEstado}"
);


                MessageBox.Show("Estado actualizado correctamente");
                CargarOrdenes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }
        private void dgvOrdenes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var valor = dgvOrdenes.Rows[e.RowIndex].Cells["descripcion"].Value;
                rtbDescripcion.Text = (valor == null || valor == DBNull.Value) ? "" : valor.ToString();
            }
        }
        private void dgvOrdenes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvOrdenes.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                long ordenId = Convert.ToInt64(dgvOrdenes.Rows[e.RowIndex].Cells["id"].Value);
                EliminarOrdenTrabajo(ordenId);
            }
        }


        private void AgregarBotonEliminarOT()
        {
            if (dgvOrdenes.Columns.Contains("btnEliminar"))
                return;

            DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn();
            btnEliminar.Name = "btnEliminar";
            btnEliminar.HeaderText = "";
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseColumnTextForButtonValue = true;
            btnEliminar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            btnEliminar.Width = 95;

            dgvOrdenes.Columns.Add(btnEliminar);
        }

        private void EliminarOrdenTrabajo(long ordenId)
        {
            if (MessageBox.Show(
                "¿Seguro que deseas eliminar esta orden de trabajo?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();

                string sql = "DELETE FROM OrdenesTrabajo WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", ordenId);

                int filas = cmd.ExecuteNonQuery();

                MessageBox.Show(filas > 0 ? "✅ Orden eliminada." : "No se encontró la orden.");
                CargarOrdenes(); 
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    int tareas = 0, items = 0, novedades = 0; int historial = 0;

                    try
                    {
                        tareas = ContarDependencias("OrdenesTrabajo_Tareas", "orden_id", ordenId);
                        items = ContarDependencias("OrdenesTrabajo_Items", "orden_id", ordenId);
                        novedades = ContarDependencias("Novedades", "orden_trabajo_id", ordenId);
                        historial = ContarDependencias("OrdenesTrabajo_Historial", "orden_id", ordenId);

                    }
                    catch
                    {
                    }

                    string detalle = "";
                    if (tareas > 0) detalle += $"• Tiene {tareas} tarea(s) asignada(s)\n";
                    if (items > 0) detalle += $"• Tiene {items} producto(s)/servicio(s) asignado(s)\n";
                    if (novedades > 0) detalle += $"• Tiene {novedades} novedad(es) registrada(s)\n";
                    if (historial > 0) detalle += $"• Tiene {historial} registro(s) en historial\n";


                    if (string.IsNullOrWhiteSpace(detalle))
                        detalle = "• Tiene registros relacionados (tareas / productos / novedades).";

                    MessageBox.Show(
                        "No se puede eliminar esta orden de trabajo porque ya tiene información asociada.\n\n" +
                        detalle +
                        "\nPrimero elimina/retira esos registros o crea una orden nueva.",
                        "No se puede eliminar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                MessageBox.Show("Error SQL al eliminar orden: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar orden: " + ex.Message);
            }

        }

        private int ContarDependencias(string tabla, string columnaFk, long ordenId)
        {
            string sql = $"SELECT COUNT(*) FROM {tabla} WHERE {columnaFk} = @id";
            using (SqlCommand cmd = new SqlCommand(sql, con.leer))
            {
                cmd.Parameters.AddWithValue("@id", ordenId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void RegistrarHistorial(long ordenId, long? usuarioId, string tipo, string titulo, string detalle)
        {
            try
            {
                using (SqlCommand cmdH = new SqlCommand(
                    "EXEC dbo.sp_historial_registrar @orden_id, @usuario_id, @tipo_evento, @titulo, @detalle",
                    con.leer))
                {
                    cmdH.Parameters.AddWithValue("@orden_id", ordenId);
                    cmdH.Parameters.AddWithValue("@usuario_id", (object)usuarioId ?? DBNull.Value);
                    cmdH.Parameters.AddWithValue("@tipo_evento", tipo);
                    cmdH.Parameters.AddWithValue("@titulo", titulo);
                    cmdH.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                    cmdH.ExecuteNonQuery();
                }
            }
            catch
            {
                // no romper nada
            }
        }


        private void EstilizarGridOrdenes()
        {
            dgvOrdenes.AllowUserToAddRows = false;
            dgvOrdenes.AllowUserToDeleteRows = false;
            dgvOrdenes.AllowUserToResizeRows = false;
            dgvOrdenes.ReadOnly = true;

            dgvOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenes.MultiSelect = false;

            dgvOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrdenes.RowHeadersVisible = false;

            dgvOrdenes.BorderStyle = BorderStyle.None;
            dgvOrdenes.BackgroundColor = System.Drawing.Color.White;

            dgvOrdenes.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvOrdenes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvOrdenes.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);

            dgvOrdenes.EnableHeadersVisualStyles = false;
            dgvOrdenes.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(24, 24, 28);
            dgvOrdenes.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvOrdenes.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgvOrdenes.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvOrdenes.ColumnHeadersHeight = 40;

            dgvOrdenes.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F);
            dgvOrdenes.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(35, 35, 35);
            dgvOrdenes.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvOrdenes.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvOrdenes.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(247, 247, 250);

            dgvOrdenes.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(220, 235, 255);
            dgvOrdenes.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(10, 10, 10);

            dgvOrdenes.RowTemplate.Height = 38;
        }

        private void EstilizarBotonEliminarOT()
        {
            if (!dgvOrdenes.Columns.Contains("btnEliminar")) return;

            var c = dgvOrdenes.Columns["btnEliminar"] as DataGridViewButtonColumn;
            c.FlatStyle = FlatStyle.Flat;

            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            c.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);

            c.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
            c.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(160, 0, 0);
            c.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 210, 210);
            c.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(120, 0, 0);

            c.Width = 95;
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        private void rtbDescripcion_TextChanged(object sender, EventArgs e)
        {

        }

        private void FormOrdenTrabajo_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvOrdenes);
        }
    }
}

