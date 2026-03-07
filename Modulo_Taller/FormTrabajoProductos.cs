using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormTrabajoProductos : Form
    {
        private readonly long usuarioId;
        private string rolUsuario;
        private long ordenIdActual = 0;

        private DataTable dtProductosFull = new DataTable();
        private DataTable dtServicios = new DataTable();
        private DataTable dtServiciosDisponibles = new DataTable();
        private DataTable dtItems = new DataTable();

        Conexion con = new Conexion();

        public FormTrabajoProductos(long usuarioActual, string rolUsuario)
        {
            InitializeComponent();

            this.usuarioId = usuarioActual;
            this.rolUsuario = rolUsuario;

            dgvServicios.DataError += (s, e) => { e.ThrowException = false; };
            dgvItems.DataError += (s, e) => { e.ThrowException = false; };

            txtBuscarProducto.TextChanged += (s, e) => FiltrarProductos(txtBuscarProducto.Text);
            lstProductos.SelectionMode = SelectionMode.One;
            lstProductos.SelectedIndexChanged += lstProductos_SelectedIndexChanged;

            PrepararGrids();
            AplicarEstilos();

            CargarProductos();
            CargarOrdenesEnCombo();

            HabilitarSecciones(false);
            cmbOrdenes.Enabled = true;
            btnCargarOrden.Enabled = true;

            txtBuscarServicio.TextChanged += (s, e) => {
                if (!string.IsNullOrWhiteSpace(txtBuscarServicio.Text))
                {
                    lstServicios.Visible = true;
                    FiltrarServiciosDisponibles(txtBuscarServicio.Text);
                }
                else
                {
                    lstServicios.Visible = false;
                }
            };

            txtBuscarServicio.Leave += (s, e) => {
                Timer timer = new Timer();
                timer.Interval = 200;
                timer.Tick += (sender, args) => {
                    if (!lstServicios.Focused && !txtBuscarServicio.Focused)
                    {
                        lstServicios.Visible = false;
                    }
                    timer.Stop();
                };
                timer.Start();
            };

            lstServicios.Leave += (s, e) => {
                lstServicios.Visible = false;
            };

            lstServicios.Click += (s, e) => {
                if (lstServicios.SelectedItem != null)
                {
                    DataRowView rv = lstServicios.SelectedItem as DataRowView;
                    if (rv != null)
                    {
                        txtBuscarServicio.Text = rv["nombre"].ToString();
                        txtBuscarServicio.SelectionStart = txtBuscarServicio.Text.Length;
                    }
                    lstServicios.Visible = false;
                }
            };
        }

        private void AplicarEstilos()
        {
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            EstilizarBoton(btnCargarOrden, true);
            EstilizarBoton(btnAgregarServicio, true);
            EstilizarBoton(btnAgregarProducto, true);

            lblOrdenInfo.ForeColor = Color.White;
            lblOrdenInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblStock.ForeColor = Color.White;
            lblStock.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            txtBuscarServicio.Font = new Font("Segoe UI", 10.5F);
            txtBuscarProducto.Font = new Font("Segoe UI", 10.5F);

            txtBuscarServicio.BorderStyle = BorderStyle.FixedSingle;
            txtBuscarProducto.BorderStyle = BorderStyle.FixedSingle;

            lstServicios.BorderStyle = BorderStyle.FixedSingle;
            lstServicios.Font = new Font("Segoe UI", 10.5F);
            lstServicios.ItemHeight = 22;

            lstProductos.BorderStyle = BorderStyle.FixedSingle;
            lstProductos.Font = new Font("Segoe UI", 10.5F);
            lstProductos.ItemHeight = 22;

            cmbOrdenes.Font = new Font("Segoe UI", 10.5F);
            cmbOrdenes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrdenes.FlatStyle = FlatStyle.Flat;

            nudCantidad.Font = new Font("Segoe UI", 10.5F);

            EstilizarGrid(dgvServicios);
            EstilizarGrid(dgvItems);
        }

        private void EstilizarGrid(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            dgv.RowHeadersVisible = false;
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;

            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.GridColor = Color.FromArgb(230, 230, 230);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 44;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(35, 35, 35);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(10, 10, 10);
            dgv.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);
            dgv.RowTemplate.Height = 42;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ScrollBars = ScrollBars.Both;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
        }

        private void EstilizarBoton(Button btn, bool primario)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btn.Height = Math.Max(btn.Height, 40);

            Color baseColor = primario ? Color.DarkSlateBlue : Color.FromArgb(60, 60, 60);
            Color hoverColor = primario ? Color.FromArgb(24, 80, 180) : Color.FromArgb(60, 60, 60);

            btn.BackColor = baseColor;
            btn.ForeColor = Color.White;

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = baseColor;
        }

        private void PrepararGrids()
        {
            dgvServicios.AutoGenerateColumns = false;
            dgvServicios.Columns.Clear();

            dgvServicios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dgvServicios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "servicio",
                HeaderText = "Servicio",
                Width = 300
            });

            dgvServicios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "precio",
                HeaderText = "Precio",
                DefaultCellStyle = { Format = "C2" },
                Width = 120
            });

            dgvServicios.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "fecha_agregado",
                HeaderText = "Fecha",
                DefaultCellStyle = { Format = "dd/MM/yyyy HH:mm" },
                Width = 150
            });

            dgvServicios.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEliminarServicio",
                HeaderText = "Acción",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 100
            });

            dgvServicios.DataSource = dtServicios;

            dgvItems.AutoGenerateColumns = false;
            dgvItems.Columns.Clear();

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "producto",
                HeaderText = "Producto"
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "cantidad",
                HeaderText = "Cantidad"
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "precio_unitario",
                HeaderText = "Precio",
                DefaultCellStyle = { Format = "C2" }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "subtotal",
                HeaderText = "Subtotal",
                DefaultCellStyle = { Format = "C2" }
            });

            dgvItems.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEliminarItem",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 95
            });

            dgvItems.DataSource = dtItems;

            nudCantidad.Minimum = 1;
            nudCantidad.Value = 1;
        }

        private void CargarProductos()
        {
            try
            {
                con.Abrir();
                string sql = "SELECT id, nombre, ISNULL(stock,0) AS stock, ISNULL(precio_pvp,0) AS precio_pvp FROM Productos ORDER BY nombre";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);

                dtProductosFull.Clear();
                da.Fill(dtProductosFull);

                lstProductos.DisplayMember = "nombre";
                lstProductos.ValueMember = "id";
                FiltrarProductos("");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarServiciosDisponibles()
        {
            try
            {
                con.Abrir();

                long tipoVehiculoId = ObtenerTipoVehiculoDeOrden(ordenIdActual);

                string sql = @"
            SELECT s.id, s.nombre, st.precio_mano_obra AS precio
            FROM Servicios s
            INNER JOIN ServicioTarifas st ON s.id = st.servicio_id
            WHERE st.tipo_vehiculo_id = @tipoVehiculo AND st.activo = 1
            ORDER BY s.nombre";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculoId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dtServiciosDisponibles.Clear();
                da.Fill(dtServiciosDisponibles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar servicios: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void FiltrarProductos(string texto)
        {
            if (dtProductosFull == null) return;

            texto = (texto ?? "").Trim().Replace("'", "''");

            DataView dv = new DataView(dtProductosFull);

            if (string.IsNullOrWhiteSpace(texto))
                dv.RowFilter = "1=0";
            else
                dv.RowFilter = $"nombre LIKE '%{texto}%'";

            lstProductos.DisplayMember = "nombre";
            lstProductos.ValueMember = "id";
            lstProductos.DataSource = dv;

            if (dv.Count > 0)
                lstProductos.SelectedIndex = 0;
            else
                lblStock.Text = "Stock: -";
        }

        private void lstProductos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstProductos.SelectedItem is DataRowView rv)
            {
                int stock = Convert.ToInt32(rv["stock"]);
                lblStock.Text = $"Stock: {stock}";
            }
        }

        private void CargarServiciosDeOrden()
        {
            if (ordenIdActual <= 0) return;

            try
            {
                con.Abrir();
                string sql = @"
                    SELECT os.id, 
                           s.nombre AS servicio, 
                           os.precio, 
                           os.fecha_agregado
                    FROM OrdenesTrabajo_Servicios os
                    INNER JOIN Servicios s ON os.servicio_id = s.id
                    WHERE os.orden_id = @id
                    ORDER BY os.fecha_agregado DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.SelectCommand.Parameters.AddWithValue("@id", ordenIdActual);

                dtServicios.Clear();
                da.Fill(dtServicios);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void CargarItemsDeOrden()
        {
            if (ordenIdActual <= 0) return;

            try
            {
                con.Abrir();
                string sql = @"
                    SELECT i.id,
                           p.nombre AS producto,
                           i.cantidad,
                           i.precio_unitario,
                           (i.cantidad * i.precio_unitario) AS subtotal
                    FROM OrdenesTrabajo_Items i
                    INNER JOIN Productos p ON i.producto_id = p.id
                    WHERE i.orden_id=@id
                    ORDER BY i.id DESC";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.SelectCommand.Parameters.AddWithValue("@id", ordenIdActual);

                dtItems.Clear();
                da.Fill(dtItems);
            }
            finally { con.Cerrar(); }
        }

        private void btnAgregarServicio_Click(object sender, EventArgs e)
        {
            if (ordenIdActual <= 0)
            {
                MessageBox.Show("Primero selecciona una orden.");
                return;
            }

            if (lstServicios.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un servicio de la lista.");
                return;
            }

            DataRowView rv = lstServicios.SelectedItem as DataRowView;
            if (rv == null) return;

            long servicioId = Convert.ToInt64(rv["id"]);
            string nombreServicio = rv["nombre"].ToString();
            decimal precio = Convert.ToDecimal(rv["precio"]);

            AgregarServicioAOrden(servicioId, nombreServicio, precio);
        }

        private void AgregarServicioAOrden(long servicioId, string nombre, decimal precio)
        {
            try
            {
                con.Abrir();
                SqlTransaction tx = con.leer.BeginTransaction();

                try
                {
                    string sql = @"
                        INSERT INTO OrdenesTrabajo_Servicios
                        (orden_id, servicio_id, precio, fecha_agregado)
                        VALUES (@orden, @servicio, @precio, GETDATE())";

                    SqlCommand cmd = new SqlCommand(sql, con.leer, tx);
                    cmd.Parameters.AddWithValue("@orden", ordenIdActual);
                    cmd.Parameters.AddWithValue("@servicio", servicioId);
                    cmd.Parameters.AddWithValue("@precio", precio);
                    cmd.ExecuteNonQuery();

                    RegistrarHistorial(
                        tx,
                        ordenIdActual,
                        usuarioId,
                        "SERVICIO",
                        "Servicio agregado",
                        $"{nombre} - Precio: {precio:C}"
                    );

                    tx.Commit();

                    MessageBox.Show("Servicio agregado correctamente.");

                    CargarServiciosDeOrden();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error agregando servicio: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void dgvServicios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvServicios.Columns[e.ColumnIndex].Name == "btnEliminarServicio")
            {
                if (!(dgvServicios.Rows[e.RowIndex].DataBoundItem is DataRowView rv)) return;

                long servicioId = Convert.ToInt64(rv["id"]);
                string nombreServicio = rv["servicio"].ToString();

                if (MessageBox.Show($"¿Eliminar el servicio '{nombreServicio}' de la orden?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                    return;

                try
                {
                    con.Abrir();
                    SqlTransaction tx = con.leer.BeginTransaction();

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("DELETE FROM OrdenesTrabajo_Servicios WHERE id=@id", con.leer, tx))
                        {
                            cmd.Parameters.AddWithValue("@id", servicioId);
                            cmd.ExecuteNonQuery();
                        }

                        RegistrarHistorial(
                            tx,
                            ordenIdActual,
                            usuarioId,
                            "SERVICIO",
                            "Servicio eliminado",
                            $"{nombreServicio} (ID: {servicioId})"
                        );

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar servicio: " + ex.Message);
                }
                finally
                {
                    con.Cerrar();
                }

                CargarServiciosDeOrden();
            }
        }

        private long ObtenerProductoIdSeleccionado()
        {
            if (lstProductos.SelectedItem is DataRowView rv)
                return Convert.ToInt64(rv["id"]);

            object val = lstProductos.SelectedValue;

            if (val == null) return 0;

            if (val is DataRowView rv2)
                return Convert.ToInt64(rv2["id"]);

            if (long.TryParse(val.ToString(), out long id))
                return id;

            return 0;
        }

        private void btnAgregarProducto_Click(object sender, EventArgs e)
        {
            if (ordenIdActual <= 0)
            {
                MessageBox.Show("Primero selecciona una orden.");
                return;
            }

            long productoId = ObtenerProductoIdSeleccionado();
            var rv = lstProductos.SelectedItem as DataRowView;
            if (rv == null)
            {
                MessageBox.Show("Selecciona un producto válido.");
                return;
            }

            int cantidad = (int)nudCantidad.Value;

            try
            {
                con.Abrir();
                SqlTransaction tx = con.leer.BeginTransaction();

                try
                {
                    SqlCommand cmdStock = new SqlCommand("SELECT ISNULL(stock,0) FROM Productos WHERE id=@id", con.leer, tx);
                    cmdStock.Parameters.AddWithValue("@id", productoId);
                    int stock = Convert.ToInt32(cmdStock.ExecuteScalar());

                    if (stock < cantidad)
                    {
                        RegistrarNovedad(tx, productoId, cantidad, stock);
                        tx.Rollback();
                        MessageBox.Show("Stock insuficiente.");
                        return;
                    }

                    SqlCommand cmdInsert = new SqlCommand(@"
                        INSERT INTO OrdenesTrabajo_Items
                        (orden_id, producto_id, cantidad, precio_unitario, fecha_agregado)
                        SELECT @o, @p, @c, precio_pvp, GETDATE()
                        FROM Productos
                        WHERE id=@p;", con.leer, tx);

                    cmdInsert.Parameters.AddWithValue("@o", ordenIdActual);
                    cmdInsert.Parameters.AddWithValue("@p", productoId);
                    cmdInsert.Parameters.AddWithValue("@c", Convert.ToDecimal(cantidad));
                    cmdInsert.ExecuteNonQuery();

                    using (SqlCommand cmdK = new SqlCommand("dbo.sp_Kardex_RegistrarMovimiento", con.leer, tx))
                    {
                        cmdK.CommandType = CommandType.StoredProcedure;
                        cmdK.Parameters.AddWithValue("@ProductoId", productoId);
                        cmdK.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmdK.Parameters.AddWithValue("@TipoMovimiento", "SALIDA");
                        cmdK.Parameters.AddWithValue("@Origen", "OT");
                        cmdK.Parameters.AddWithValue("@ReferenciaId", ordenIdActual);
                        cmdK.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmdK.Parameters.AddWithValue("@Fecha", DBNull.Value);
                        cmdK.ExecuteNonQuery();
                    }

                    string nombreProd = rv["nombre"].ToString();
                    RegistrarHistorial(tx, ordenIdActual, usuarioId, "ITEM", "Producto agregado",
                        $"{nombreProd} (ID {productoId}) - Cantidad: {cantidad}");

                    tx.Commit();

                    MessageBox.Show("Producto agregado correctamente.");
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            CargarProductos();
            CargarItemsDeOrden();
        }

        private void RegistrarNovedad(SqlTransaction tx, long productoId, int requerido, int stock)
        {
            if (usuarioId <= 0) return;

            SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Novedades(orden_trabajo_id, usuario_id, descripcion, fecha)
                VALUES(@o,@u,@d,GETDATE())", con.leer, tx);

            cmd.Parameters.AddWithValue("@o", ordenIdActual);
            cmd.Parameters.AddWithValue("@u", usuarioId);
            cmd.Parameters.AddWithValue("@d", $"Falta stock producto {productoId}. Requerido {requerido}, disponible {stock}");
            cmd.ExecuteNonQuery();

            RegistrarHistorial(
                tx,
                ordenIdActual,
                usuarioId,
                "NOVEDAD",
                "Stock insuficiente",
                $"ProductoID: {productoId}. Requerido: {requerido}. Disponible: {stock}"
            );
        }

        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvItems.Columns[e.ColumnIndex].Name != "btnEliminarItem") return;

            if (!(dgvItems.Rows[e.RowIndex].DataBoundItem is DataRowView rv)) return;

            long itemId = Convert.ToInt64(rv["id"]);

            if (MessageBox.Show("¿Eliminar este producto de la orden?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();
                SqlTransaction tx = con.leer.BeginTransaction();

                try
                {
                    long prodId;
                    decimal cantDec;

                    using (SqlCommand cmdGet = new SqlCommand("SELECT producto_id, cantidad FROM OrdenesTrabajo_Items WHERE id=@id", con.leer, tx))
                    {
                        cmdGet.Parameters.AddWithValue("@id", itemId);
                        using (SqlDataReader dr = cmdGet.ExecuteReader())
                        {
                            if (!dr.Read())
                            {
                                tx.Rollback();
                                return;
                            }
                            prodId = Convert.ToInt64(dr["producto_id"]);
                            cantDec = Convert.ToDecimal(dr["cantidad"]);
                        }
                    }

                    using (SqlCommand cmdDel = new SqlCommand("DELETE FROM OrdenesTrabajo_Items WHERE id=@id", con.leer, tx))
                    {
                        cmdDel.Parameters.AddWithValue("@id", itemId);
                        cmdDel.ExecuteNonQuery();
                    }

                    int cantInt = Convert.ToInt32(cantDec);

                    using (SqlCommand cmdK = new SqlCommand("dbo.sp_Kardex_RegistrarMovimiento", con.leer, tx))
                    {
                        cmdK.CommandType = CommandType.StoredProcedure;
                        cmdK.Parameters.AddWithValue("@ProductoId", prodId);
                        cmdK.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmdK.Parameters.AddWithValue("@TipoMovimiento", "ENTRADA");
                        cmdK.Parameters.AddWithValue("@Origen", "OT_DEVOLUCION");
                        cmdK.Parameters.AddWithValue("@ReferenciaId", ordenIdActual);
                        cmdK.Parameters.AddWithValue("@Cantidad", cantInt);
                        cmdK.Parameters.AddWithValue("@Fecha", DBNull.Value);
                        cmdK.ExecuteNonQuery();
                    }

                    RegistrarHistorial(
                        tx,
                        ordenIdActual,
                        usuarioId,
                        "ITEM",
                        "Producto eliminado",
                        $"ItemID: {itemId}, ProductoID: {prodId}, Cantidad: {cantDec} (stock devuelto)"
                    );

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar item: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            CargarProductos();
            CargarItemsDeOrden();
        }

        private void CargarOrdenesEnCombo()
        {
            try
            {
                con.Abrir();

                string sql = @"
                    SELECT 
                        ot.id,
                        CONCAT('OT #', ot.id, ' - ', ISNULL(v.placa,'SIN-PLACA'), ' - ', ISNULL(ot.estado,'(sin estado)')) AS display_text
                    FROM OrdenesTrabajo ot
                    LEFT JOIN Vehiculos v ON v.id = ot.vehiculo_id
                    WHERE ot.facturada = 0
                    ORDER BY ot.id DESC";

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.Fill(dt);

                cmbOrdenes.DisplayMember = "display_text";
                cmbOrdenes.ValueMember = "id";
                cmbOrdenes.DataSource = dt;

                cmbOrdenes.SelectedIndex = dt.Rows.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar órdenes: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void btnCargarOrden_Click(object sender, EventArgs e)
        {
            if (cmbOrdenes.SelectedValue == null)
            {
                MessageBox.Show("Selecciona una orden de trabajo.");
                return;
            }

            ordenIdActual = Convert.ToInt64(cmbOrdenes.SelectedValue);

            lblOrdenInfo.Text = $"Orden seleccionada: #{ordenIdActual}";

            CargarServiciosDeOrden();
            CargarItemsDeOrden();
            CargarServiciosDisponibles();

            HabilitarSecciones(true);
        }

        private void HabilitarSecciones(bool habilitar)
        {
            btnAgregarServicio.Enabled = habilitar;
            btnAgregarProducto.Enabled = habilitar;
            txtBuscarServicio.Enabled = habilitar;
            nudCantidad.Enabled = habilitar;

            txtBuscarProducto.Enabled = habilitar;
            lstProductos.Enabled = habilitar;
            lstServicios.Enabled = habilitar;

            dgvServicios.Enabled = habilitar;
            dgvItems.Enabled = habilitar;
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

        private long ObtenerTipoVehiculoDeOrden(long ordenId)
        {
            long tipoVehiculo = 0;

            try
            {
                con.Abrir();

                string sql = @"
                    SELECT v.tipo
                    FROM OrdenesTrabajo ot
                    INNER JOIN Vehiculos v ON v.id = ot.vehiculo_id
                    WHERE ot.id = @id";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", ordenId);

                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    string tipoVehiculoStr = result.ToString();

                    string sqlTipoId = "SELECT id FROM TiposVehiculo WHERE nombre = @nombre AND activo = 1";
                    SqlCommand cmdTipo = new SqlCommand(sqlTipoId, con.leer);
                    cmdTipo.Parameters.AddWithValue("@nombre", tipoVehiculoStr);

                    object tipoId = cmdTipo.ExecuteScalar();
                    if (tipoId != null)
                        tipoVehiculo = Convert.ToInt64(tipoId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error obteniendo tipo de vehículo: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            return tipoVehiculo;
        }

        private string ObtenerNombreTipoVehiculo(long tipoVehiculoId)
        {
            string nombre = "";

            try
            {
                con.Abrir();
                string sql = "SELECT nombre FROM TiposVehiculo WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", tipoVehiculoId);

                object result = cmd.ExecuteScalar();
                if (result != null)
                    nombre = result.ToString();
            }
            catch { }
            finally { con.Cerrar(); }

            return nombre;
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

        private void btnBuscadorServicios_Click(object sender, EventArgs e)
        {
            if (ordenIdActual <= 0)
            {
                MessageBox.Show("Primero selecciona una orden.");
                return;
            }

            long tipoVehiculo = ObtenerTipoVehiculoDeOrden(ordenIdActual);

            FormBuscador buscador = new FormBuscador(
                FormBuscador.TipoBusqueda.Servicios,
                tipoVehiculo
            );

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long servicioId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();
                decimal precio = Convert.ToDecimal(fila["precio"]);

                AgregarServicioAOrden(servicioId, nombre, precio);
            }
        }

        private void FiltrarServiciosDisponibles(string texto)
        {
            if (dtServiciosDisponibles == null || ordenIdActual <= 0) return;

            texto = (texto ?? "").Trim().Replace("'", "''");

            DataView dv = new DataView(dtServiciosDisponibles);

            if (string.IsNullOrWhiteSpace(texto))
            {
                lstServicios.Visible = false;
                return;
            }
            else
            {
                dv.RowFilter = $"nombre LIKE '%{texto}%'";
                lstServicios.Visible = dv.Count > 0;
            }

            lstServicios.DisplayMember = "nombre";
            lstServicios.ValueMember = "id";
            lstServicios.DataSource = dv;
        }

        private void btnBuscadorProductos_Click(object sender, EventArgs e)
        {
            if (ordenIdActual <= 0)
            {
                MessageBox.Show("Primero selecciona una orden.");
                return;
            }

            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Productos);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long productoId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();
                int cantidad = buscador.CantidadSeleccionada;
                decimal precio = Convert.ToDecimal(fila["precio_pvp"]);

                AgregarProductoAOrden(productoId, nombre, cantidad, precio);
            }
        }

        private void AgregarProductoAOrden(long productoId, string nombre, int cantidad, decimal precio)
        {
            try
            {
                con.Abrir();
                SqlTransaction tx = con.leer.BeginTransaction();

                try
                {
                    SqlCommand cmdStock = new SqlCommand("SELECT ISNULL(stock,0) FROM Productos WHERE id=@id", con.leer, tx);
                    cmdStock.Parameters.AddWithValue("@id", productoId);
                    int stock = Convert.ToInt32(cmdStock.ExecuteScalar());

                    if (stock < cantidad)
                    {
                        RegistrarNovedad(tx, productoId, cantidad, stock);
                        tx.Rollback();
                        MessageBox.Show("Stock insuficiente.");
                        return;
                    }

                    SqlCommand cmdInsert = new SqlCommand(@"
                INSERT INTO OrdenesTrabajo_Items
                (orden_id, producto_id, cantidad, precio_unitario, fecha_agregado)
                VALUES (@orden, @producto, @cantidad, @precio, GETDATE())", con.leer, tx);

                    cmdInsert.Parameters.AddWithValue("@orden", ordenIdActual);
                    cmdInsert.Parameters.AddWithValue("@producto", productoId);
                    cmdInsert.Parameters.AddWithValue("@cantidad", Convert.ToDecimal(cantidad));
                    cmdInsert.Parameters.AddWithValue("@precio", precio);
                    cmdInsert.ExecuteNonQuery();

                    using (SqlCommand cmdK = new SqlCommand("dbo.sp_Kardex_RegistrarMovimiento", con.leer, tx))
                    {
                        cmdK.CommandType = CommandType.StoredProcedure;
                        cmdK.Parameters.AddWithValue("@ProductoId", productoId);
                        cmdK.Parameters.AddWithValue("@UsuarioId", usuarioId);
                        cmdK.Parameters.AddWithValue("@TipoMovimiento", "SALIDA");
                        cmdK.Parameters.AddWithValue("@Origen", "OT");
                        cmdK.Parameters.AddWithValue("@ReferenciaId", ordenIdActual);
                        cmdK.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmdK.Parameters.AddWithValue("@Fecha", DBNull.Value);
                        cmdK.ExecuteNonQuery();
                    }

                    RegistrarHistorial(tx, ordenIdActual, usuarioId, "ITEM", "Producto agregado",
                        $"{nombre} (ID {productoId}) - Cantidad: {cantidad} - Precio: {precio:C}");

                    tx.Commit();

                    MessageBox.Show("Producto agregado correctamente.");

                    CargarItemsDeOrden();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error agregando producto: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }
    }
}



