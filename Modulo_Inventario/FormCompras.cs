using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormCompras : Form
    {
        private readonly Conexion con = new Conexion();

        private readonly long usuarioId; 

        private DataTable dtProveedores = new DataTable();
        private DataTable dtProductos = new DataTable();
        private DataTable dtItems = new DataTable();

        private long productoIdSeleccionado = 0;
        private const decimal IVA_ECUADOR = 0.15m; 


        public FormCompras(long usuarioId)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;

            PrepararTablaItems();
            ConfigurarGrid();
            AplicarEstilos();

            CargarProveedores();
            CargarProductosBuscador();

            // Defaults
            dtFechaCompra.Value = DateTime.Today;
            lstProductos.Visible = false;

            // Eventos
            txtBuscarProducto.TextChanged += (s, e) => FiltrarProductos(txtBuscarProducto.Text);

            lstProductos.DoubleClick += (s, e) => ConfirmarProductoSeleccionado();
            lstProductos.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ConfirmarProductoSeleccionado();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    lstProductos.Visible = false;
                }
            };

            btnAgregarItem.Click += (s, e) => AgregarItem();
            btnGuardarCompra.Click += (s, e) => GuardarCompra();
            btnLimpiar.Click += (s, e) => LimpiarTodo();

            dgvItems.CellClick += dgvItems_CellClick;

            // IVA% opcional
            

            RecalcularTotales();
        }

        // ---------------- UI / Estilos ----------------

        private void AplicarEstilos()
        {
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new Font("Segoe UI", 10F);

            cmbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;

            txtFactura.BorderStyle = BorderStyle.FixedSingle;
            txtBuscarProducto.BorderStyle = BorderStyle.FixedSingle;

            nudCantidad.Minimum = 1;
            nudCantidad.Maximum = 999999;
            nudCantidad.DecimalPlaces = 0;
            nudCantidad.Value = 1;

            nudCosto.Minimum = 0;
            nudCosto.Maximum = 999999;
            nudCosto.DecimalPlaces = 4;
            nudCosto.Value = 0;

            // Botones
            EstiloBoton(btnGuardarCompra, Color.FromArgb(0, 123, 255));
            EstiloBoton(btnAgregarItem, Color.DarkSlateBlue);
            EstiloBoton(btnLimpiar, Color.FromArgb(60, 60, 60));

            // Listbox
            lstProductos.BorderStyle = BorderStyle.FixedSingle;
            lstProductos.BackColor = Color.White;
            lstProductos.ForeColor = Color.FromArgb(30, 30, 30);
            lstProductos.IntegralHeight = false;
            lstProductos.ItemHeight = 22;

            // Labels totales (si quieres)
            lblSubtotal.ForeColor = Color.FromArgb(40, 40, 40);
            lblIVA.ForeColor = Color.FromArgb(40, 40, 40);
            lblTotal.ForeColor = Color.FromArgb(15, 15, 15);
            lblTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        }

        private void EstiloBoton(Button btn, Color color)
        {
            if (btn == null) return;

            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Height = Math.Max(btn.Height, 40);
            btn.Cursor = Cursors.Hand;

            Color hover = Color.FromArgb(
                Math.Min(color.R + 15, 255),
                Math.Min(color.G + 15, 255),
                Math.Min(color.B + 15, 255)
            );

            btn.MouseEnter += (s, e) => btn.BackColor = hover;
            btn.MouseLeave += (s, e) => btn.BackColor = color;
        }

        private void PrepararTablaItems()
        {
            dtItems.Columns.Clear();
            dtItems.Columns.Add("producto_id", typeof(long));
            dtItems.Columns.Add("producto", typeof(string));
            dtItems.Columns.Add("cantidad", typeof(int));
            dtItems.Columns.Add("costo_unitario", typeof(decimal));
            dtItems.Columns.Add("subtotal", typeof(decimal));
        }

        private void ConfigurarGrid()
        {
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToDeleteRows = false;
            dgvItems.AllowUserToResizeRows = false;
            dgvItems.ReadOnly = true;

            dgvItems.AutoGenerateColumns = false;
            dgvItems.Columns.Clear();

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "producto_id",
                DataPropertyName = "producto_id",
                Visible = false
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "producto",
                DataPropertyName = "producto",
                HeaderText = "Producto"
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "cantidad",
                DataPropertyName = "cantidad",
                HeaderText = "Cant."
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "costo_unitario",
                DataPropertyName = "costo_unitario",
                HeaderText = "Costo U."
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "subtotal",
                DataPropertyName = "subtotal",
                HeaderText = "Subtotal"
            });

            dgvItems.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEliminar",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true
            });

            dgvItems.DataSource = dtItems;

            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
            dgvItems.RowHeadersVisible = false;

            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.BackgroundColor = Color.White;

            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 40;

            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvItems.GridColor = Color.FromArgb(230, 230, 230);

            dgvItems.DefaultCellStyle.Padding = new Padding(8, 3, 8, 3);
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);
            dgvItems.RowTemplate.Height = 38;

            dgvItems.Columns["cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItems.Columns["costo_unitario"].DefaultCellStyle.Format = "N4";
            dgvItems.Columns["subtotal"].DefaultCellStyle.Format = "N4";
        }


        private void dgvItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvItems.Columns[e.ColumnIndex].Name != "btnEliminar") return;

            if (MessageBox.Show("¿Eliminar este item?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            dtItems.Rows[e.RowIndex].Delete();
            RecalcularTotales();
        }

        // ---------------- Cargas ----------------

        private void CargarProveedores()
        {
            try
            {
                con.Abrir();
                dtProveedores.Clear();

                using (var da = new SqlDataAdapter(
                    "SELECT id, nombre_empresa FROM Proveedores WHERE estado = 1 ORDER BY nombre_empresa",
                    con.leer))
                {
                    da.Fill(dtProveedores);
                }

                cmbProveedor.DisplayMember = "nombre_empresa";
                cmbProveedor.ValueMember = "id";
                cmbProveedor.DataSource = dtProveedores;
                cmbProveedor.SelectedIndex = dtProveedores.Rows.Count > 0 ? 0 : -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando proveedores: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarProductosBuscador()
        {
            try
            {
                con.Abrir();
                dtProductos.Clear();

                string sql = @"
SELECT 
    id,
    nombre,
    tipo,
    ISNULL(stock,0) AS stock
FROM Productos
ORDER BY nombre;";

                using (var da = new SqlDataAdapter(sql, con.leer))
                {
                    da.Fill(dtProductos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }


        private void FiltrarProductos(string texto)
        {
            texto = (texto ?? "").Trim().Replace("'", "''");

            // Si no hay texto, ocultamos lista
            if (texto.Length < 1)
            {
                lstProductos.Visible = false;
                lstProductos.DataSource = null;
                productoIdSeleccionado = 0;
                lblStockSel.Text = "Stock: -";
                return;
            }

            // Filtrar productos por nombre o tipo
            var dv = new DataView(dtProductos);
            dv.RowFilter = $"nombre LIKE '%{texto}%' OR tipo LIKE '%{texto}%'";

            // Si no hay resultados
            if (dv.Count == 0)
            {
                lstProductos.Visible = false;
                lstProductos.DataSource = null;
                productoIdSeleccionado = 0;
                lblStockSel.Text = "Stock: -";
                return;
            }

            lstProductos.DisplayMember = "nombre";
            lstProductos.ValueMember = "id";
            lstProductos.DataSource = dv;

            // Mostrar lista debajo del textbox
            lstProductos.Visible = true;
            lstProductos.Left = txtBuscarProducto.Left;
            lstProductos.Top = txtBuscarProducto.Bottom + 2;
            lstProductos.Width = txtBuscarProducto.Width;
            lstProductos.BringToFront();

            // Tamaño dinámico
            lstProductos.Height = Math.Min(200, (dv.Count * 22) + 4);
        }


        private void ConfirmarProductoSeleccionado()
        {
            if (lstProductos.SelectedItem == null) return;

            var rv = (DataRowView)lstProductos.SelectedItem;
            productoIdSeleccionado = Convert.ToInt64(rv["id"]);

            txtBuscarProducto.Text = rv["nombre"].ToString();
            lblStockSel.Text = $"Stock: {rv["stock"]}";
            lstProductos.Visible = false;
        }

    
        private void AgregarItem()
        {
            if (productoIdSeleccionado <= 0)
            {
                MessageBox.Show("Selecciona un producto válido.");
                return;
            }

            int cantidad = (int)nudCantidad.Value;
            decimal costo = nudCosto.Value;

            if (cantidad <= 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }
            decimal minimo = 0.50m;

            string nombre = txtBuscarProducto.Text.ToLower();

            if (nombre.Contains("aceite")) minimo = 5;
            if (nombre.Contains("filtro")) minimo = 2;
            if (nombre.Contains("bujia")) minimo = 1;

            if (costo < minimo)
            {
                MessageBox.Show($"Precio demasiado bajo. Mínimo sugerido: ${minimo}");
                return;
            }

            if (costo < 1)
            {
                var r = MessageBox.Show(
                    "El costo ingresado es muy bajo. ¿Estás seguro?",
                    "Confirmar precio",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (r == DialogResult.No)
                    return;
            }




            // Si ya está, sumamos cantidad (y mantenemos el último costo)
            var existentes = dtItems.Select($"producto_id = {productoIdSeleccionado}");
            if (existentes.Length > 0)
            {
                var row = existentes[0];
                int cantActual = Convert.ToInt32(row["cantidad"]);
                row["cantidad"] = cantActual + cantidad;
                row["costo_unitario"] = costo;
                row["subtotal"] = Convert.ToInt32(row["cantidad"]) * Convert.ToDecimal(row["costo_unitario"]);
            }
            else
            {
                var r = dtItems.NewRow();
                r["producto_id"] = productoIdSeleccionado;
                r["producto"] = txtBuscarProducto.Text.Trim();
                r["cantidad"] = cantidad;
                r["costo_unitario"] = costo;
                r["subtotal"] = cantidad * costo;
                dtItems.Rows.Add(r);
            }

            productoIdSeleccionado = 0;
            txtBuscarProducto.Clear();
            lblStockSel.Text = "Stock: -";
            nudCantidad.Value = 1;
            nudCosto.Value = 0;

            RecalcularTotales();
        }

        private void RecalcularTotales()
        {
            decimal subtotal = 0;

            foreach (DataRow row in dtItems.Rows)
            {
                subtotal += Convert.ToDecimal(row["subtotal"]);
            }

            decimal iva = subtotal * IVA_ECUADOR;
            decimal total = subtotal + iva;

            lblSubtotal.Text = $"Subtotal: {subtotal:C2}";
            lblIVA.Text = $"IVA (15%): {iva:C2}";
            lblTotal.Text = $"Total: {total:C2}";
        }



        private void GuardarCompra()
        {
            if (cmbProveedor.SelectedValue == null || cmbProveedor.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un proveedor.");
                return;
            }

            if (dtItems.Rows.Count == 0)
            {
                MessageBox.Show("Agrega al menos un producto.");
                return;
            }

            long proveedorId = Convert.ToInt64(cmbProveedor.SelectedValue);
            string factura = (txtFactura.Text ?? "").Trim();
            DateTime fecha = dtFechaCompra.Value;

            // Totales
            decimal subtotal = 0;

            foreach (DataRow row in dtItems.Rows)
            {
                subtotal += Convert.ToDecimal(row["subtotal"]);
            }

            decimal ivaTotal = subtotal * IVA_ECUADOR;
            decimal totalCompra = subtotal + ivaTotal;


            try
            {
                con.Abrir();

                using (SqlTransaction tx = con.leer.BeginTransaction())
                {
                    try
                    {
                        // 1) Insert cabecera Compras
                        long compraId;
                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO Compras
(proveedor_id, usuario_id, numero_factura_proveedor, fecha_compra, subtotal, iva_total, total_compra)
VALUES
(@prov, @user, NULLIF(@factura,''), @fecha, @sub, @iva, @total);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", con.leer, tx))
                        {
                            cmd.Parameters.AddWithValue("@prov", proveedorId);
                            cmd.Parameters.AddWithValue("@user", usuarioId);
                            cmd.Parameters.AddWithValue("@factura", factura);
                            cmd.Parameters.AddWithValue("@fecha", fecha);
                            cmd.Parameters.AddWithValue("@sub", subtotal);
                            cmd.Parameters.AddWithValue("@iva", ivaTotal);
                            cmd.Parameters.AddWithValue("@tot", totalCompra);

                            compraId = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        // 2) por cada item: detalle + stock + kardex
                        foreach (DataRow r in dtItems.Rows)
                        {
                            long prodId = Convert.ToInt64(r["producto_id"]);
                            int cant = Convert.ToInt32(r["cantidad"]);
                            decimal costo = Convert.ToDecimal(r["costo_unitario"]);

                            // detalle
                            using (SqlCommand cmdDet = new SqlCommand(@"
INSERT INTO DetalleCompras(compra_id, producto_id, cantidad, precio_costo_unitario)
VALUES(@c, @p, @cant, @costo);", con.leer, tx))
                            {
                                cmdDet.Parameters.AddWithValue("@c", compraId);
                                cmdDet.Parameters.AddWithValue("@p", prodId);
                                cmdDet.Parameters.AddWithValue("@cant", cant);
                                cmdDet.Parameters.AddWithValue("@costo", costo);
                                cmdDet.ExecuteNonQuery();
                            }

                            // stock
                            using (SqlCommand cmdStock = new SqlCommand(@"
UPDATE Productos
SET stock = ISNULL(stock,0) + @cant
WHERE id = @p;", con.leer, tx))
                            {
                                cmdStock.Parameters.AddWithValue("@cant", cant);
                                cmdStock.Parameters.AddWithValue("@p", prodId);
                                cmdStock.ExecuteNonQuery();
                            }

                            // kardex (ENTRADA)
                            using (SqlCommand cmdK = new SqlCommand(@"
INSERT INTO Kardex(producto_id, usuario_id, tipo_movimiento, origen, referencia_id, cantidad)
VALUES(@p, @u, 'ENTRADA', 'COMPRA', @ref, @cant);", con.leer, tx))
                            {
                                cmdK.Parameters.AddWithValue("@p", prodId);
                                cmdK.Parameters.AddWithValue("@u", usuarioId);
                                cmdK.Parameters.AddWithValue("@ref", compraId);
                                cmdK.Parameters.AddWithValue("@cant", cant);
                                cmdK.ExecuteNonQuery();
                            }
                        }



                        tx.Commit();
                        MessageBox.Show("✅ Compra guardada. Stock y Kardex actualizados.");

                        // refresca cache de productos por stock
                        CargarProductosBuscador();
                        LimpiarTodo();
                    }
                    catch (SqlException ex)
                    {
                        tx.Rollback();

                        if (ex.Number == 2601 || ex.Number == 2627)
                        {
                            MessageBox.Show("Ya existe una compra con ese proveedor y número de factura.");
                            return;
                        }

                        MessageBox.Show("Error SQL: " + ex.Message);
                    }

                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void LimpiarTodo()
        {
            txtFactura.Clear();
            txtBuscarProducto.Clear();
            lstProductos.Visible = false;

            dtItems.Clear();
            productoIdSeleccionado = 0;

            nudCantidad.Value = 1;
            nudCosto.Value = 0;

            dtFechaCompra.Value = DateTime.Today;

            lblStockSel.Text = "Stock: -";
            RecalcularTotales();
        }

        private void lstProductos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtBuscarProducto_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
