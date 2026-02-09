using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormTrabajoProductos : Form
    {
        private readonly long usuarioId;
        private string rolUsuario;
        private long ordenIdActual = 0;

        private DataTable dtProductosFull = new DataTable();
        private DataTable dtTareas = new DataTable();
        private DataTable dtItems = new DataTable();

        Conexion con = new Conexion();

        public FormTrabajoProductos(long usuarioActual, string rolUsuario)
        {
            InitializeComponent();

            this.usuarioId = usuarioActual;
            this.rolUsuario = rolUsuario;

            dgvTareas.DataError += (s, e) => { e.ThrowException = false; };
            dgvItems.DataError += (s, e) => { e.ThrowException = false; };

            txtBuscarProducto.TextChanged += (s, e) => FiltrarProductos(txtBuscarProducto.Text);
            lstProductos.SelectionMode = SelectionMode.One;
            lstProductos.SelectedIndexChanged += lstProductos_SelectedIndexChanged;

            dgvTareas.CurrentCellDirtyStateChanged += dgvTareas_CurrentCellDirtyStateChanged;
            dgvTareas.CellValueChanged += dgvTareas_CellValueChanged;
            dgvTareas.CellContentClick += dgvTareas_CellContentClick;

            dgvItems.CellContentClick += dgvItems_CellContentClick;

            PrepararGrids();
            AplicarEstilos();

            CargarProductos();
            CargarOrdenesEnCombo();

            HabilitarSecciones(false);
            cmbOrdenes.Enabled = true;
            btnCargarOrden.Enabled = true;
        }

        private void AplicarEstilos()
        {
            BackColor = Color.FromArgb(245, 246, 250);

            Font = new Font("Segoe UI", 10F, FontStyle.Regular);

            EstilizarBoton(btnCargarOrden, true);
            EstilizarBoton(btnAgregarTarea, true);
            EstilizarBoton(btnAgregarProducto, true);

            lblOrdenInfo.ForeColor = Color.FromArgb(35, 35, 35);
            lblOrdenInfo.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblStock.ForeColor = Color.FromArgb(35, 35, 35);
            lblStock.Font = new Font("Segoe UI", 11F, FontStyle.Bold);

            txtTarea.Font = new Font("Segoe UI", 10.5F);
            txtBuscarProducto.Font = new Font("Segoe UI", 10.5F);

            txtTarea.BorderStyle = BorderStyle.FixedSingle;
            txtBuscarProducto.BorderStyle = BorderStyle.FixedSingle;

            lstProductos.BorderStyle = BorderStyle.FixedSingle;
            lstProductos.Font = new Font("Segoe UI", 10.5F);
            lstProductos.ItemHeight = 22;

            cmbOrdenes.Font = new Font("Segoe UI", 10.5F);
            cmbOrdenes.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbOrdenes.FlatStyle = FlatStyle.Flat;

            nudCantidad.Font = new Font("Segoe UI", 10.5F);

            EstilizarGrid(dgvTareas);
            EstilizarGrid(dgvItems);
            EstilizarTabControl(tabControl1);

        }

        private void EstilizarTabControl(TabControl tab)
        {
            tab.DrawMode = TabDrawMode.OwnerDrawFixed;
            tab.SizeMode = TabSizeMode.Fixed;
            tab.ItemSize = new Size(180, 42);
            tab.Appearance = TabAppearance.Normal;

            tab.DrawItem -= tabControl1_DrawItem;
            tab.DrawItem += tabControl1_DrawItem;

            tab.Padding = new Point(20, 6);
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab = sender as TabControl;
            if (tab == null) return;

            bool selected = (e.Index == tab.SelectedIndex);

            Rectangle r = e.Bounds;
            r.Inflate(-6, -6);

            Color bg = selected ? Color.FromArgb(30, 96, 210) : Color.FromArgb(235, 236, 240);
            Color fg = selected ? Color.White : Color.FromArgb(40, 40, 40);

            using (var br = new SolidBrush(bg))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var font = new Font("Segoe UI", 10.5F, FontStyle.Bold))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var path = Redondear(r, 14))
                {
                    e.Graphics.FillPath(br, path);
                }

                e.Graphics.DrawString(tab.TabPages[e.Index].Text, font, new SolidBrush(fg), r, sf);
            }
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

        private void dgv_CellPaintingRedondearBotones(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var dgv = sender as DataGridView;
            if (dgv == null) return;

            if (!(dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn)) return;

            e.PaintBackground(e.CellBounds, true);

            Rectangle rect = e.CellBounds;
            rect.Inflate(-10, -10);

            using (var path = Redondear(rect, 10))
            using (var br = new SolidBrush(Color.FromArgb(220, 60, 60)))
            using (var pen = new Pen(Color.FromArgb(200, 40, 40)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.FillPath(br, path);
                e.Graphics.DrawPath(pen, path);

                string text = "Eliminar";
                e.Graphics.DrawString(text, new Font("Segoe UI", 9.5F, FontStyle.Bold), Brushes.White, rect, sf);
            }

            e.Handled = true;
        }


        private System.Drawing.Drawing2D.GraphicsPath Redondear(Rectangle r, int radius)
        {
            int d = radius * 2;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void EstilizarBoton(Button btn, bool primario)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btn.Height = Math.Max(btn.Height, 40);

            Color baseColor = primario ? Color.FromArgb(30, 96, 210) : Color.FromArgb(80, 80, 80);
            Color hoverColor = primario ? Color.FromArgb(24, 80, 180) : Color.FromArgb(60, 60, 60);

            btn.BackColor = baseColor;
            btn.ForeColor = Color.White;

            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = baseColor;
        }



        private void PrepararGrids()
        {
            dgvTareas.AutoGenerateColumns = false;
            dgvTareas.Columns.Clear();

            dgvTareas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "id",
                Name = "id",
                Visible = false
            });

            dgvTareas.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "descripcion",
                HeaderText = "Tarea"
            });

            dgvTareas.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "completada",
                HeaderText = "Hecho"
            });

            dgvTareas.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEliminarTarea",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            });

            dgvTareas.DataSource = dtTareas;

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
                HeaderText = "Precio"
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "subtotal",
                HeaderText = "Subtotal"
            });

            dgvItems.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEliminarItem",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            });

            dgvItems.DataSource = dtItems;

            nudCantidad.Minimum = 1;
            nudCantidad.Value = 1;

            AjustarBotonEliminar(dgvTareas, "btnEliminarTarea");
            AjustarBotonEliminar(dgvItems, "btnEliminarItem");

        }

        private void dgvTareas_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvTareas.IsCurrentCellDirty)
                dgvTareas.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvTareas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvTareas.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn &&
                dgvTareas.Columns[e.ColumnIndex].DataPropertyName == "completada")
            {
                long tareaId = Convert.ToInt64(dgvTareas.Rows[e.RowIndex].Cells["id"].Value);
                bool completada = Convert.ToBoolean(dgvTareas.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);

                try
                {
                    con.Abrir();
                    SqlCommand cmd = new SqlCommand("UPDATE OrdenesTrabajo_Tareas SET completada=@c WHERE id=@id", con.leer);
                    cmd.Parameters.AddWithValue("@c", completada);
                    cmd.Parameters.AddWithValue("@id", tareaId);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar estado: " + ex.Message);
                }
                finally { con.Cerrar(); }
            }
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

        private void CargarTareasDeOrden()
        {
            if (ordenIdActual <= 0) return;

            try
            {
                con.Abrir();
                string sql = "SELECT id, descripcion, completada FROM OrdenesTrabajo_Tareas WHERE orden_id=@id ORDER BY id DESC";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.SelectCommand.Parameters.AddWithValue("@id", ordenIdActual);

                dtTareas.Clear();
                da.Fill(dtTareas);
            }
            finally { con.Cerrar(); }
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

        private void btnAgregarTarea_Click(object sender, EventArgs e)
        {
            if (ordenIdActual <= 0)
            {
                MessageBox.Show("Primero selecciona una orden.");
                return;
            }

            string tarea = (txtTarea.Text ?? "").Trim();

            if (tarea.Length == 0)
            {
                MessageBox.Show("Escribe una tarea.");
                txtTarea.Focus();
                return;
            }

            try
            {
                con.Abrir();
                SqlCommand cmd = new SqlCommand("INSERT INTO OrdenesTrabajo_Tareas(orden_id, descripcion) VALUES(@o,@d)", con.leer);
                cmd.Parameters.AddWithValue("@o", ordenIdActual);
                cmd.Parameters.AddWithValue("@d", tarea);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar tarea: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            txtTarea.Clear();
            CargarTareasDeOrden();
        }

        private void dgvTareas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvTareas.Columns[e.ColumnIndex].Name != "btnEliminarTarea") return;

            if (!(dgvTareas.Rows[e.RowIndex].DataBoundItem is DataRowView rv)) return;

            long tareaId = Convert.ToInt64(rv["id"]);

            if (MessageBox.Show("¿Eliminar esta tarea?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM OrdenesTrabajo_Tareas WHERE id=@id", con.leer))
                {
                    cmd.Parameters.AddWithValue("@id", tareaId);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar tarea: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            CargarTareasDeOrden();
        }


        private void dgvTareas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvTareas.Columns[e.ColumnIndex].Name != "btnEliminarTarea") return;

            long tareaId = Convert.ToInt64(dgvTareas.Rows[e.RowIndex].Cells["id"].Value);

            try
            {
                con.Abrir();
                SqlCommand cmd = new SqlCommand("DELETE FROM OrdenesTrabajo_Tareas WHERE id=@id", con.leer);
                cmd.Parameters.AddWithValue("@id", tareaId);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar tarea: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }

            CargarTareasDeOrden();
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

                    SqlCommand cmdUpd = new SqlCommand("UPDATE Productos SET stock = ISNULL(stock,0) - @c WHERE id=@p", con.leer, tx);
                    cmdUpd.Parameters.AddWithValue("@c", cantidad);
                    cmdUpd.Parameters.AddWithValue("@p", productoId);
                    cmdUpd.ExecuteNonQuery();

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

                    using (SqlCommand cmdBack = new SqlCommand("UPDATE Productos SET stock = ISNULL(stock,0) + @c WHERE id=@p", con.leer, tx))
                    {
                        cmdBack.Parameters.AddWithValue("@c", cantDec);
                        cmdBack.Parameters.AddWithValue("@p", prodId);
                        cmdBack.ExecuteNonQuery();
                    }

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


        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvItems.Columns[e.ColumnIndex].Name != "btnEliminarItem") return;

            long itemId = Convert.ToInt64(dgvItems.Rows[e.RowIndex].Cells["id"].Value);

            try
            {
                con.Abrir();
                SqlTransaction tx = con.leer.BeginTransaction();

                try
                {
                    SqlCommand cmdGet = new SqlCommand(
                        "SELECT producto_id, cantidad FROM OrdenesTrabajo_Items WHERE id=@id",
                        con.leer, tx
                    );
                    cmdGet.Parameters.AddWithValue("@id", itemId);

                    long prodId;
                    int cant;

                    using (SqlDataReader dr = cmdGet.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            tx.Rollback();
                            return;
                        }

                        prodId = Convert.ToInt64(dr["producto_id"]);
                        cant = Convert.ToInt32(Convert.ToDecimal(dr["cantidad"]));
                    }

                    SqlCommand cmdDel = new SqlCommand(
                        "DELETE FROM OrdenesTrabajo_Items WHERE id=@id",
                        con.leer, tx
                    );
                    cmdDel.Parameters.AddWithValue("@id", itemId);
                    cmdDel.ExecuteNonQuery();

                    SqlCommand cmdBack = new SqlCommand(
                        "UPDATE Productos SET stock = ISNULL(stock,0) + @c WHERE id=@p",
                        con.leer, tx
                    );
                    cmdBack.Parameters.AddWithValue("@c", cant);
                    cmdBack.Parameters.AddWithValue("@p", prodId);
                    cmdBack.ExecuteNonQuery();

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

            CargarTareasDeOrden();
            CargarItemsDeOrden();

            HabilitarSecciones(true);
        }

        private void AjustarBotonEliminar(DataGridView dgv, string colName)
        {
            if (!dgv.Columns.Contains(colName)) return;

            var col = dgv.Columns[colName] as DataGridViewButtonColumn;
            if (col == null) return;

            col.FlatStyle = FlatStyle.Flat;
            col.Width = 95;

            dgv.RowTemplate.Height = 44;
            dgv.DefaultCellStyle.Padding = new Padding(6, 2, 6, 2);
        }


        private void HabilitarSecciones(bool habilitar)
        {
            btnAgregarTarea.Enabled = habilitar;
            btnAgregarProducto.Enabled = habilitar;
            txtTarea.Enabled = habilitar;
            nudCantidad.Enabled = habilitar;

            txtBuscarProducto.Enabled = habilitar;
            lstProductos.Enabled = habilitar;

            dgvTareas.Enabled = habilitar;
            dgvItems.Enabled = habilitar;
        }
    }
}



