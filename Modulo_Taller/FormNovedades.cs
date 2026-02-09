using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormNovedades : Form
    {
        private readonly long usuarioId;
        private readonly string rolUsuario;
        private long ordenIdActual = 0;

        Conexion con = new Conexion();

        public FormNovedades(long usuarioId, string rolUsuario)
        {
            InitializeComponent();

            this.usuarioId = usuarioId;
            this.rolUsuario = rolUsuario;

            ConfigurarGrid();
            CargarOrdenes();
            EstilizarGrid();
            EstilizarBotones();


            BloquearEdicion();

            cmbOrdenes.SelectedIndexChanged += (s, e) => CargarNovedades();
            btnNuevaNovedad.Click += (s, e) => HabilitarEdicion();
            btnCancelar.Click += (s, e) => { Limpiar(); BloquearEdicion(); };
            btnGuardar.Click += (s, e) => GuardarNovedad();

            btnAceptar.Click += (s, e) => CambiarEstadoNovedad("Aceptado");
            btnRechazar.Click += (s, e) => CambiarEstadoNovedad("Rechazado");

            chkRequiereExtra.CheckedChanged += (s, e) =>
            {
                nudMontoExtra.Enabled = chkRequiereExtra.Checked;
            };

        }

        private void ConfigurarGrid()
        {
            dgvNovedades.AllowUserToAddRows = false;
            dgvNovedades.ReadOnly = true;
            dgvNovedades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNovedades.MultiSelect = false;
            dgvNovedades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNovedades.RowHeadersVisible = false;

            dgvNovedades.CellFormatting += DgvNovedades_CellFormatting;
        }

        private void dgvNovedades_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvNovedades.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                long novedadId = Convert.ToInt64(dgvNovedades.Rows[e.RowIndex].Cells["id"].Value);

                
                string estadoTxt = dgvNovedades.Rows[e.RowIndex].Cells["estado_cliente"].Value?.ToString() ?? "";

                EliminarNovedad(novedadId, estadoTxt);
            }
        }

        private void EliminarNovedad(long novedadId, string estadoTxt)
        {
            if (!estadoTxt.Contains("Pendiente"))
            {
                MessageBox.Show(
                    "No se puede eliminar una novedad que ya fue respondida (Aceptada/Rechazada).\n" +
                    "Si fue Aceptada, probablemente ya generó un extra en la orden.",
                    "Acción no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (MessageBox.Show(
                "¿Seguro que deseas eliminar esta novedad?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();

                string sql = "DELETE FROM Novedades WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", novedadId);

                int filas = cmd.ExecuteNonQuery();

                MessageBox.Show(filas > 0 ? "Novedad eliminada." : "No se encontró la novedad.");
                CargarNovedades();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar novedad: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }



        private void DgvNovedades_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvNovedades.Rows.Count == 0) return;

            var row = dgvNovedades.Rows[e.RowIndex];
            if (!dgvNovedades.Columns.Contains("estado_cliente")) return;

            string estado = row.Cells["estado_cliente"].Value?.ToString() ?? "";
            bool requiereExtra = false;

            if (dgvNovedades.Columns.Contains("requiere_presupuesto_extra"))
                requiereExtra = Convert.ToBoolean(row.Cells["requiere_presupuesto_extra"].Value);

            if (estado == "Pendiente" && requiereExtra)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(183, 28, 28);
                row.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            }
            else if (estado == "Aceptado")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(27, 94, 32);
            }
            else if (estado == "Rechazado")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);
            }
        }

        private void CargarOrdenes()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    ot.id,
    CONCAT('OT #', ot.id, ' - ', v.placa) AS display
FROM OrdenesTrabajo ot
INNER JOIN Vehiculos v ON ot.vehiculo_id = v.id
ORDER BY ot.fecha_ingreso DESC;";

                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbOrdenes.DisplayMember = "display";
                cmbOrdenes.ValueMember = "id";
                cmbOrdenes.DataSource = dt;
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

        private void CargarNovedades()
        {
            if (cmbOrdenes.SelectedValue == null) return;

            ordenIdActual = Convert.ToInt64(cmbOrdenes.SelectedValue);

            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    id,
    descripcion,
    fecha,
    requiere_presupuesto_extra,
    monto_extra,
    estado_cliente
FROM Novedades
WHERE orden_trabajo_id = @ordenId
ORDER BY fecha DESC;";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@ordenId", ordenIdActual);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvNovedades.DataSource = dt;
                AgregarBotonEliminarNovedad();

                if (dgvNovedades.Columns.Contains("id"))
                    dgvNovedades.Columns["id"].Visible = false;
                if (dgvNovedades.Columns.Contains("descripcion"))
                    dgvNovedades.Columns["descripcion"].HeaderText = "Novedad";
                if (dgvNovedades.Columns.Contains("fecha"))
                    dgvNovedades.Columns["fecha"].HeaderText = "Fecha";
                if (dgvNovedades.Columns.Contains("requiere_presupuesto_extra"))
                    dgvNovedades.Columns["requiere_presupuesto_extra"].HeaderText = "Requiere Extra";
                if (dgvNovedades.Columns.Contains("monto_extra"))
                    dgvNovedades.Columns["monto_extra"].HeaderText = "Monto Extra";
                if (dgvNovedades.Columns.Contains("estado_cliente"))
                    dgvNovedades.Columns["estado_cliente"].HeaderText = "Estado";

                BloquearEdicion();
                Limpiar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar novedades: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            foreach (DataGridViewRow row in dgvNovedades.Rows)
            {
                string estado = row.Cells["estado_cliente"].Value.ToString();

                if (estado == "Pendiente")
                    row.Cells["estado_cliente"].Value = "🔔 Pendiente";
                else if (estado == "Aceptado")
                    row.Cells["estado_cliente"].Value = "✅ Aceptado";
                else if (estado == "Rechazado")
                    row.Cells["estado_cliente"].Value = "❌ Rechazado";
            }

            int pendientes = 0;

            foreach (DataGridViewRow row in dgvNovedades.Rows)
            {
                string estado = row.Cells["estado_cliente"].Value.ToString();
                if (estado.Contains("Pendiente"))
                    pendientes++;
            }

            lblPendientes.Text = $"🔔 Pendientes: {pendientes}";

            lblPendientes.ForeColor = pendientes > 0
                ? Color.Red
                : Color.Green;


        }

        private void GuardarNovedad()
        {
            if (ordenIdActual <= 0)
            {
                MessageBox.Show("Selecciona una orden primero.");
                return;
            }

            string desc = txtDescripcion.Text.Trim();
            if (desc.Length < 5)
            {
                MessageBox.Show("Describe la novedad con más detalle.");
                return;
            }

            bool requiereExtra = chkRequiereExtra.Checked;
            decimal? montoExtra = null;

            if (requiereExtra)
            {
                if (nudMontoExtra.Value <= 0)
                {
                    MessageBox.Show("Si requiere extra, ingresa un monto mayor a 0.");
                    return;
                }
                montoExtra = nudMontoExtra.Value;
            }

            try
            {
                

                con.Abrir();

                string sql = @"
INSERT INTO Novedades
(orden_trabajo_id, usuario_id, descripcion, fecha, requiere_presupuesto_extra, monto_extra, estado_cliente)
VALUES
(@ordenId, @usuarioId, @descripcion, GETDATE(), @requiereExtra, @montoExtra, 'Pendiente');";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@ordenId", ordenIdActual);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@descripcion", desc);
                cmd.Parameters.AddWithValue("@requiereExtra", requiereExtra);
                if (montoExtra.HasValue)
                    cmd.Parameters.AddWithValue("@montoExtra", montoExtra.Value);
                else
                    cmd.Parameters.AddWithValue("@montoExtra", DBNull.Value);


                cmd.ExecuteNonQuery();

                MessageBox.Show("Novedad registrada (pendiente).");
                Limpiar();
                BloquearEdicion();
                CargarNovedades();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar novedad: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void CambiarEstadoNovedad(string nuevoEstado)
        {
            if (dgvNovedades.CurrentRow == null)
            {
                MessageBox.Show("Selecciona una novedad.");
                return;
            }

            long novedadId = Convert.ToInt64(dgvNovedades.CurrentRow.Cells["id"].Value);
            string estadoActual = dgvNovedades.CurrentRow.Cells["estado_cliente"].Value?.ToString() ?? "";

            if (estadoActual != "Pendiente")
            {
                MessageBox.Show("Esta novedad ya fue respondida.");
                return;
            }

            bool requiereExtra = Convert.ToBoolean(dgvNovedades.CurrentRow.Cells["requiere_presupuesto_extra"].Value);
            object montoObj = dgvNovedades.CurrentRow.Cells["monto_extra"].Value;

            decimal montoExtra = 0;
            if (montoObj != DBNull.Value && montoObj != null)
                montoExtra = Convert.ToDecimal(montoObj);

            try
            {
                con.Abrir();
                using (SqlTransaction tx = con.leer.BeginTransaction())
                {
                    try
                    {
                        string sqlUpdate = @"
UPDATE Novedades
SET estado_cliente = @estado, fecha_respuesta = GETDATE()
WHERE id = @id;";

                        SqlCommand cmd = new SqlCommand(sqlUpdate, con.leer, tx);
                        cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                        cmd.Parameters.AddWithValue("@id", novedadId);
                        cmd.ExecuteNonQuery();

                        if (nuevoEstado == "Aceptado" && requiereExtra)
                        {
                            if (montoExtra <= 0)
                                throw new Exception("La novedad no tiene monto_extra válido.");

                            InsertarItemExtra(tx, ordenIdActual, montoExtra,
                                "EXTRA: " + dgvNovedades.CurrentRow.Cells["descripcion"].Value?.ToString());
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

                MessageBox.Show($"✅ Novedad marcada como {nuevoEstado}.");
                CargarNovedades();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar estado: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void InsertarItemExtra(SqlTransaction tx, long ordenId, decimal monto, string descripcion)
        {
            string sql = @"
INSERT INTO OrdenesTrabajo_Items
(orden_id, producto_id, servicio_id, descripcion, cantidad, precio_unit, subtotal, fecha_agregado)
VALUES
(@ordenId, NULL, NULL, @desc, 1, @precio, @subtotal, GETDATE());";

            SqlCommand cmd = new SqlCommand(sql, con.leer, tx);
            cmd.Parameters.AddWithValue("@ordenId", ordenId);
            cmd.Parameters.AddWithValue("@desc", descripcion);
            cmd.Parameters.AddWithValue("@precio", monto);
            cmd.Parameters.AddWithValue("@subtotal", monto);

            cmd.ExecuteNonQuery();
        }

        private void HabilitarEdicion()
        {
            txtDescripcion.Enabled = true;
            chkRequiereExtra.Enabled = true;
            nudMontoExtra.Enabled = true;

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtDescripcion.Focus();
            nudMontoExtra.Enabled = chkRequiereExtra.Checked;

        }

        private void BloquearEdicion()
        {
            txtDescripcion.Enabled = false;
            chkRequiereExtra.Enabled = false;
            nudMontoExtra.Enabled = false;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void Limpiar()
        {
            txtDescripcion.Clear();
            chkRequiereExtra.Checked = false;
            nudMontoExtra.Value = 0;
        }

        private void EstilizarGrid()
        {
            dgvNovedades.BorderStyle = BorderStyle.None;
            dgvNovedades.BackgroundColor = Color.White;

            dgvNovedades.EnableHeadersVisualStyles = false;
            dgvNovedades.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 45, 60);
            dgvNovedades.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNovedades.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgvNovedades.DefaultCellStyle.Font =
                new Font("Segoe UI", 9.5f);

            dgvNovedades.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 230, 241);

            dgvNovedades.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvNovedades.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(245, 245, 245);

            dgvNovedades.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNovedades.GridColor = Color.LightGray;

            dgvNovedades.RowTemplate.Height = 35;
        }

        private void EstilizarBotones()
        {
            EstiloBoton(btnNuevaNovedad, Color.DarkSlateBlue); // Azul del programa
            EstiloBoton(btnGuardar, Color.FromArgb(40, 167, 69));      // Verde guardar
            EstiloBoton(btnCancelar, Color.FromArgb(108, 117, 125));   // Gris cancelar
            EstiloBoton(btnAceptar, Color.FromArgb(0, 123, 255));      // Azul aceptar
            EstiloBoton(btnRechazar, Color.FromArgb(220, 53, 69));     // Rojo rechazar
        }

        private void EstiloBoton(Button btn, Color color)
        {
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Height = 40;
        }

        private void AgregarBotonEliminarNovedad()
        {
            if (dgvNovedades.Columns.Contains("btnEliminar"))
                return;

            DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn();
            btnEliminar.Name = "btnEliminar";
            btnEliminar.HeaderText = "";
            btnEliminar.Text = "Eliminar";
            btnEliminar.UseColumnTextForButtonValue = true;
            btnEliminar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            btnEliminar.Width = 90;

            dgvNovedades.Columns.Add(btnEliminar);

            btnEliminar.FlatStyle = FlatStyle.Flat;
            dgvNovedades.Columns["btnEliminar"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvNovedades.Columns["btnEliminar"].DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
            dgvNovedades.Columns["btnEliminar"].DefaultCellStyle.ForeColor = Color.FromArgb(160, 0, 0);
            dgvNovedades.Columns["btnEliminar"].DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 210, 210);
            dgvNovedades.Columns["btnEliminar"].DefaultCellStyle.SelectionForeColor = Color.FromArgb(120, 0, 0);
            dgvNovedades.Columns["btnEliminar"].DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        }



    }
}
