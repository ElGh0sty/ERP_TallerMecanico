using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormNovedades : Form
    {
        private readonly long usuarioId;
        private readonly string rolUsuario;
        private long ordenIdActual = 0;

        private System.Collections.Generic.List<ProductoItem> _productosBase =
    new System.Collections.Generic.List<ProductoItem>();

        private ProductoItem _productoSeleccionado = null;

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


            txtBuscarProducto.TextChanged += (s, e) => FiltrarProductos();
            txtBuscarProducto.Enter += (s, e) =>
            {
                if (chkRequiereExtra.Checked)
                {
                    FiltrarProductos();
                    lstProductos.Visible = (lstProductos.Items.Count > 0);
                }
            };
            lstProductos.Click += (s, e) => SeleccionarProductoDeLista();

            chkRequiereExtra.CheckedChanged += (s, e) =>
            {
                bool on = chkRequiereExtra.Checked;

                nudMontoExtra.Enabled = on; // lo vamos a dejar en false luego (ver punto 3)
                nudCantidadExtra.Enabled = on;

                txtBuscarProducto.Enabled = on;
                lstProductos.Enabled = on;
                lblProductoSel.Enabled = on;

                if (!on)
                {
                    txtBuscarProducto.Text = "";
                    lstProductos.Visible = false;
                    _productoSeleccionado = null;
                    lblProductoSel.Text = "Seleccionado: Ninguno";
                    nudCantidadExtra.Value = 1;
                    nudMontoExtra.Value = 0;
                }
                else
                {
                    if (_productosBase == null || _productosBase.Count == 0)
                        CargarProductosBase();

                    lstProductos.Visible = true;
                    txtBuscarProducto.Focus();
                }
            };

            nudMontoExtra.Enabled = false;  
            nudMontoExtra.ReadOnly = true;  
            nudMontoExtra.Increment = 0;    


            txtBuscarProducto.Enabled = false;
            lstProductos.Enabled = false;
            lstProductos.Visible = false;
            lblProductoSel.Enabled = false;
            nudCantidadExtra.Enabled = false;

        }

        private void CargarProductosBase()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT id, nombre, ISNULL(precio_pvp, 0) AS precio_pvp
FROM Productos
ORDER BY nombre;";

                var lista = new System.Collections.Generic.List<ProductoItem>();

                using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new ProductoItem
                        {
                            Id = Convert.ToInt64(rd["id"]),
                            Nombre = rd["nombre"].ToString(),
                            Precio = Convert.ToDecimal(rd["precio_pvp"])
                        });
                    }
                }

                _productosBase = lista;

                // mostrar todo al inicio
                lstProductos.DataSource = _productosBase;
                lstProductos.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }


        private void SeleccionarProductoDeLista()
        {
            if (lstProductos.SelectedItem == null) return;

            _productoSeleccionado = lstProductos.SelectedItem as ProductoItem;

            if (_productoSeleccionado != null)
            {
                lblProductoSel.Text = "Seleccionado: " + _productoSeleccionado.Nombre;

                if (_productoSeleccionado.Precio > 0)
                    nudMontoExtra.Value = _productoSeleccionado.Precio;

                lstProductos.Visible = false; // se oculta al elegir
            }
        }

        private void FiltrarProductos()
        {
            if (!chkRequiereExtra.Checked) return;

            string q = (txtBuscarProducto.Text ?? "").Trim().ToLower();

            // si no hay texto, muestra todo
            var filtrada = string.IsNullOrWhiteSpace(q)
                ? _productosBase
                : _productosBase.Where(p => p.Nombre.ToLower().Contains(q)).ToList();

            lstProductos.DataSource = null;
            lstProductos.DataSource = filtrada;

            lstProductos.Visible = filtrada.Count > 0;
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

                if (filas > 0)
                {
                    RegistrarHistorial(
                        ordenIdActual,
                        usuarioId,
                        "NOVEDAD",
                        "Novedad eliminada",
                        $"Novedad #{novedadId} eliminada (pendiente)."
                    );
                }

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
WHERE ot.facturada = 0
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
    estado_cliente, 
    producto_id
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
                EstilizarBotonEliminarNovedad();

                if (dgvNovedades.Columns.Contains("id"))
                    dgvNovedades.Columns["id"].Visible = false;
                if (dgvNovedades.Columns.Contains("descripcion"))
                    dgvNovedades.Columns["descripcion"].HeaderText = "Novedad";
                if (dgvNovedades.Columns.Contains("fecha"))
                    dgvNovedades.Columns["fecha"].HeaderText = "Fecha";
                if (dgvNovedades.Columns.Contains("requiere_presupuesto_extra"))
                    dgvNovedades.Columns["requiere_presupuesto_extra"].HeaderText = "Requiere Extra";
                if (dgvNovedades.Columns.Contains("monto_extra"))
                {
                    dgvNovedades.Columns["monto_extra"].DefaultCellStyle.Format = "N2";
                    dgvNovedades.Columns["monto_extra"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

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
                if (_productoSeleccionado == null)
                {
                    MessageBox.Show("Selecciona un producto/repuesto para el extra.");
                    return;
                }

                if (nudCantidadExtra.Value <= 0)
                {
                    MessageBox.Show("Ingresa una cantidad válida.");
                    return;
                }

                decimal cantidad = nudCantidadExtra.Value;
                decimal precio = _productoSeleccionado.Precio;

                montoExtra = cantidad * precio;
                nudMontoExtra.Value = montoExtra.Value; 
            }

            try
            {
                

                con.Abrir();

                string sql = @"
INSERT INTO Novedades
(orden_trabajo_id, usuario_id, descripcion, fecha, requiere_presupuesto_extra, monto_extra, estado_cliente, producto_id)
VALUES
(@ordenId, @usuarioId, @descripcion, GETDATE(), @requiereExtra, @montoExtra, 'Pendiente', @productoId);";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@productoId",
    requiereExtra ? (object)_productoSeleccionado?.Id ?? DBNull.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@ordenId", ordenIdActual);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@descripcion", desc);
                cmd.Parameters.AddWithValue("@requiereExtra", requiereExtra);
                if (montoExtra.HasValue)
                    cmd.Parameters.AddWithValue("@montoExtra", montoExtra.Value);
                else
                    cmd.Parameters.AddWithValue("@montoExtra", DBNull.Value);


                cmd.ExecuteNonQuery();
                RegistrarHistorial(
    ordenIdActual,
    usuarioId,
    "NOVEDAD",
    "Novedad registrada",
    $"Pendiente. Extra: {(requiereExtra ? "Sí" : "No")}. Monto: {(montoExtra.HasValue ? montoExtra.Value.ToString("N2") : "0.00")}. Desc: {desc}"
);

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

            if (!estadoActual.Contains("Pendiente"))
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

                        object prodObj = dgvNovedades.CurrentRow.Cells["producto_id"].Value;
                        long? productoId = (prodObj == null || prodObj == DBNull.Value) ? (long?)null : Convert.ToInt64(prodObj);

                        if (nuevoEstado == "Aceptado" && requiereExtra)
                        {
                            if (productoId == null)
                                throw new Exception("Esta novedad no tiene producto asignado. Edítala o vuelve a crearla.");

                            if (montoExtra <= 0)
                                throw new Exception("El monto extra debe ser mayor a 0.");

                            decimal cantidad = 1; 
                            InsertarItemExtraProducto(tx, ordenIdActual, productoId.Value, "Repuesto extra", cantidad, montoExtra);
                        }

                        
                        
                        RegistrarHistorial(
    tx,
    ordenIdActual,
    usuarioId,
    "NOVEDAD",
    "Respuesta a novedad",
    $"Novedad #{novedadId} -> {nuevoEstado}. Desc: {dgvNovedades.CurrentRow.Cells["descripcion"].Value}"
);



                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }

                MessageBox.Show($" Novedad marcada como {nuevoEstado}.");
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

        private void InsertarItemExtraProducto(SqlTransaction tx, long ordenId, long productoId, string nombreProducto, decimal cantidad, decimal montoTotal)
        {
            decimal precioUnit = (cantidad <= 0) ? 0 : (montoTotal / cantidad);

            string sql = @"
INSERT INTO OrdenesTrabajo_Items
(orden_id, producto_id, servicio_id, descripcion, cantidad, precio_unitario, fecha_agregado)
VALUES
(@ordenId, @productoId, NULL, @desc, @cant, @precioUnit, GETDATE());";

            using (SqlCommand cmd = new SqlCommand(sql, con.leer, tx))
            {
                cmd.Parameters.AddWithValue("@ordenId", ordenId);
                cmd.Parameters.AddWithValue("@productoId", productoId);
                cmd.Parameters.AddWithValue("@desc", "EXTRA REPUESTO: " + nombreProducto);
                cmd.Parameters.AddWithValue("@cant", cantidad);
                cmd.Parameters.AddWithValue("@precioUnit", precioUnit);
                cmd.ExecuteNonQuery();
            }

            RegistrarHistorial(
                tx,
                ordenId,
                usuarioId,
                "ITEM",
                "Extra agregado",
                $"Producto: {nombreProducto} | Cant: {cantidad:N2} | Total: {montoTotal:N2}"
            );
        }



        private void HabilitarEdicion()
        {
            txtDescripcion.Enabled = true;
            chkRequiereExtra.Enabled = true;
            nudCantidadExtra.Enabled = true;

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtDescripcion.Focus();
            nudCantidadExtra.Enabled = chkRequiereExtra.Checked;

        }

        private void BloquearEdicion()
        {
            txtDescripcion.Enabled = false;
            chkRequiereExtra.Enabled = false;
            nudCantidadExtra.Enabled = false;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void Limpiar()
        {
            txtDescripcion.Clear();
            chkRequiereExtra.Checked = false;
            nudCantidadExtra.Value = 0;
        }

        private void EstilizarGrid()
        {
            dgvNovedades.AllowUserToAddRows = false;
            dgvNovedades.AllowUserToDeleteRows = false;
            dgvNovedades.AllowUserToResizeRows = false;
            dgvNovedades.ReadOnly = true;

            dgvNovedades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNovedades.MultiSelect = false;

            dgvNovedades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNovedades.RowHeadersVisible = false;

            dgvNovedades.BorderStyle = BorderStyle.None;
            dgvNovedades.BackgroundColor = Color.White;

            dgvNovedades.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNovedades.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvNovedades.GridColor = Color.FromArgb(230, 230, 230);

            dgvNovedades.EnableHeadersVisualStyles = false;
            dgvNovedades.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvNovedades.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvNovedades.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvNovedades.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvNovedades.ColumnHeadersHeight = 40;

            dgvNovedades.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvNovedades.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgvNovedades.DefaultCellStyle.BackColor = Color.White;
            dgvNovedades.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgvNovedades.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);

            dgvNovedades.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgvNovedades.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);

            dgvNovedades.RowTemplate.Height = 38;
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

        private void EstilizarBotonEliminarNovedad()
        {
            if (!dgvNovedades.Columns.Contains("btnEliminar")) return;

            var c = dgvNovedades.Columns["btnEliminar"] as DataGridViewButtonColumn;
            c.FlatStyle = FlatStyle.Flat;

            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            c.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            c.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
            c.DefaultCellStyle.ForeColor = Color.FromArgb(160, 0, 0);
            c.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 210, 210);
            c.DefaultCellStyle.SelectionForeColor = Color.FromArgb(120, 0, 0);

            c.Width = 90;
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        private void RegistrarHistorial(SqlTransaction tx, long ordenId, long? usuarioId, string tipo, string titulo, string detalle)
        {
            using (SqlCommand cmd = new SqlCommand(
                "EXEC dbo.sp_historial_registrar @orden_id, @usuario_id, @tipo_evento, @titulo, @detalle",
                con.leer, tx))
            {
                cmd.Parameters.AddWithValue("@orden_id", ordenId);
                cmd.Parameters.AddWithValue("@usuario_id", (object)usuarioId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@tipo_evento", tipo);
                cmd.Parameters.AddWithValue("@titulo", titulo);
                cmd.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                cmd.ExecuteNonQuery();
            }
        }

        private void RegistrarHistorial(long ordenId, long? usuarioId, string tipo, string titulo, string detalle)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                    "EXEC dbo.sp_historial_registrar @orden_id, @usuario_id, @tipo_evento, @titulo, @detalle",
                    con.leer))
                {
                    cmd.Parameters.AddWithValue("@orden_id", ordenId);
                    cmd.Parameters.AddWithValue("@usuario_id", (object)usuarioId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@tipo_evento", tipo);
                    cmd.Parameters.AddWithValue("@titulo", titulo);
                    cmd.Parameters.AddWithValue("@detalle", (object)detalle ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // No romper flujo
            }
        }

        private class ProductoItem
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; } 
            public override string ToString() => Nombre;
        }

        private void btnBuscadorOrden_Click(object sender, EventArgs e)
        {
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.OrdenesTrabajo);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;
                long ordenId = Convert.ToInt64(fila["id"]);

                for (int i = 0; i < cmbOrdenes.Items.Count; i++)
                {
                    DataRowView item = cmbOrdenes.Items[i] as DataRowView;
                    if (item != null && Convert.ToInt64(item["id"]) == ordenId)
                    {
                        cmbOrdenes.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        
    }
}
