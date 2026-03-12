using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
// Alias para evitar ambigüedad
using PdfFont = iTextSharp.text.Font;
using PdfRectangle = iTextSharp.text.Rectangle;
using GdiFont = System.Drawing.Font;

namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormCompras : Form
    {
        private readonly Conexion con = new Conexion();
        private readonly long usuarioId;

        private DataTable dtProveedores = new DataTable();
        private DataTable dtProductos = new DataTable();
        private DataTable dtItems = new DataTable();
        private DataTable dtImpuestos = new DataTable();

        private long productoIdSeleccionado = 0;
        private decimal costoOriginalProducto = 0;
        private int impuestoIdOriginal = 0;
        private bool _actualizandoLista = false;

        private DataTable dtSugerencias = new DataTable();

        public FormCompras(long usuarioId)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;

            PrepararTablaItems();
            PrepararTablaSugerencias();
            ConfigurarGrid();
            AplicarEstilos();

            CargarProveedores();
            CargarProductosBuscador();
            CargarImpuestos();

            

            // Defaults
            lstProductos.Visible = false;

            // Eventos
            txtBuscarProducto.TextChanged += (s, e) => FiltrarProductos(txtBuscarProducto.Text);
            cmbImpuestoCompra.SelectedIndexChanged += (s, e) => VerificarCambioImpuesto();

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

            nudCosto.ValueChanged += (s, e) => VerificarCambioCosto();

            btnAgregarItem.Click += (s, e) => AgregarItem();
            btnGuardarCompra.Click += (s, e) => GuardarCompra();
            btnLimpiar.Click += (s, e) => LimpiarTodo();
            btnNuevoProducto.Click += (s, e) => AbrirPopupNuevoProducto(txtBuscarProducto.Text);

            dgvItems.CellClick += dgvItems_CellClick;

            RecalcularTotales();
        }

        private void AplicarEstilos()
        {
            BackColor = Color.FromArgb(245, 246, 250);
            Font = new GdiFont("Segoe UI", 10F);

            cmbProveedor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImpuestoCompra.DropDownStyle = ComboBoxStyle.DropDownList;

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
            EstiloBoton(btnAgregarItem, Color.MediumSlateBlue);
            EstiloBoton(btnLimpiar, Color.FromArgb(60, 60, 60));
            EstiloBoton(btnNuevoProducto, Color.FromArgb(40, 167, 69));

            // Listbox
            lstProductos.BorderStyle = BorderStyle.FixedSingle;
            lstProductos.BackColor = Color.White;
            lstProductos.ForeColor = Color.FromArgb(30, 30, 30);
            lstProductos.IntegralHeight = false;
            lstProductos.ItemHeight = 22;

            
            lblTotal.Font = new GdiFont("Segoe UI", 11F, FontStyle.Bold);
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

        private void CargarImpuestos()
        {
            try
            {
                con.Abrir();
                string sql = "SELECT id, nombre, porcentaje FROM Impuestos WHERE activo = 1 ORDER BY id";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.Fill(dtImpuestos);

                cmbImpuestoCompra.DataSource = dtImpuestos;
                cmbImpuestoCompra.DisplayMember = "nombre";
                cmbImpuestoCompra.ValueMember = "id";
                cmbImpuestoCompra.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando impuestos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void VerificarCambioCosto()
        {
            if (productoIdSeleccionado > 0 && costoOriginalProducto > 0)
            {
                decimal nuevoCosto = nudCosto.Value;
                decimal diferencia = Math.Abs(nuevoCosto - costoOriginalProducto);
                decimal porcentaje = (diferencia / costoOriginalProducto) * 100;

                if (porcentaje > 10) // Si la diferencia es mayor al 10%
                {
                    var result = MessageBox.Show(
                        $"El costo está cambiando de {costoOriginalProducto:C4} a {nuevoCosto:C4} " +
                        $"({porcentaje:N1}% de diferencia). ¿Está seguro de continuar?",
                        "Confirmar cambio de costo",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        nudCosto.Value = costoOriginalProducto;
                    }
                }
            }
        }

        private void VerificarCambioImpuesto()
        {
            if (productoIdSeleccionado > 0 && impuestoIdOriginal > 0)
            {
                int nuevoImpuestoId = Convert.ToInt32(cmbImpuestoCompra.SelectedValue);

                if (nuevoImpuestoId != impuestoIdOriginal)
                {
                    var result = MessageBox.Show(
                        "Está cambiando el impuesto del producto. ¿Desea continuar?",
                        "Confirmar cambio de impuesto",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        // Restaurar el impuesto original
                        for (int i = 0; i < cmbImpuestoCompra.Items.Count; i++)
                        {
                            var row = ((DataRowView)cmbImpuestoCompra.Items[i]).Row;
                            if (Convert.ToInt32(row["id"]) == impuestoIdOriginal)
                            {
                                cmbImpuestoCompra.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
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

        private void PrepararTablaSugerencias()
        {
            dtSugerencias.Columns.Clear();
            dtSugerencias.Columns.Add("id", typeof(long));
            dtSugerencias.Columns.Add("nombre", typeof(string));
            dtSugerencias.Columns.Add("stock", typeof(int));
            dtSugerencias.Columns.Add("precio_costo", typeof(decimal));
            dtSugerencias.Columns.Add("impuesto_id", typeof(int));
            dtSugerencias.Columns.Add("is_create", typeof(bool));
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
                HeaderText = "Producto",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "cantidad",
                DataPropertyName = "cantidad",
                HeaderText = "Cant.",
                Width = 70
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "costo_unitario",
                DataPropertyName = "costo_unitario",
                HeaderText = "Costo U.",
                Width = 110
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "subtotal",
                DataPropertyName = "subtotal",
                HeaderText = "Subtotal",
                Width = 110
            });

            dgvItems.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "btnEliminar",
                HeaderText = "",
                Text = "Eliminar",
                UseColumnTextForButtonValue = true,
                Width = 90
            });

            dgvItems.DataSource = dtItems;

            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
            dgvItems.RowHeadersVisible = false;

            dgvItems.BorderStyle = BorderStyle.None;
            dgvItems.BackgroundColor = Color.White;

            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 28);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new GdiFont("Segoe UI", 10F, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 42;

            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvItems.GridColor = Color.FromArgb(230, 230, 230);

            dgvItems.DefaultCellStyle.Font = new GdiFont("Segoe UI", 10F);
            dgvItems.DefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            dgvItems.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgvItems.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 247, 250);
            dgvItems.RowTemplate.Height = 40;

            dgvItems.Columns["cantidad"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItems.Columns["costo_unitario"].DefaultCellStyle.Format = "N4";
            dgvItems.Columns["subtotal"].DefaultCellStyle.Format = "N4";

            // Botón eliminar estilo
            var btnCol = (DataGridViewButtonColumn)dgvItems.Columns["btnEliminar"];
            btnCol.FlatStyle = FlatStyle.Flat;

            dgvItems.CellPainting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                if (dgvItems.Columns[e.ColumnIndex].Name != "btnEliminar") return;

                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var r = e.CellBounds;
                r.Inflate(-8, -8);

                using (var b = new SolidBrush(Color.FromArgb(220, 53, 69)))
                    e.Graphics.FillRectangle(b, r);

                TextRenderer.DrawText(
                    e.Graphics,
                    "Eliminar",
                    dgvItems.Font,
                    r,
                    Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );

                e.Handled = true;
            };
        }

        private void CargarProveedores()
        {
            try
            {
                con.Abrir();
                dtProveedores.Clear();

                using (var da = new SqlDataAdapter(
                    "SELECT id, nombre_empresa, ruc, direccion, telefono, email, contacto_nombre FROM Proveedores WHERE estado = 1 ORDER BY nombre_empresa",
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
                        p.id,
                        p.nombre,
                        p.tipo,
                        ISNULL(p.stock,0) AS stock,
                        p.precio_costo,
                        p.impuesto_id,
                        i.nombre AS impuesto_nombre
                    FROM Productos p
                    LEFT JOIN Impuestos i ON p.impuesto_id = i.id
                    ORDER BY p.nombre;";

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
            texto = (texto ?? "").Trim();

            if (texto.Length < 1)
            {
                OcultarListaProductos();
                return;
            }

            _actualizandoLista = true;

            try
            {
                var sugerencias = dtSugerencias.Clone();

                string safe = texto.Replace("'", "''");
                var dv = new DataView(dtProductos);
                dv.RowFilter = $"nombre LIKE '%{safe}%' OR tipo LIKE '%{safe}%'";

                if (dv.Count > 0)
                {
                    foreach (DataRowView rv in dv)
                    {
                        var r = sugerencias.NewRow();
                        r["id"] = Convert.ToInt64(rv["id"]);
                        r["nombre"] = rv["nombre"].ToString();
                        r["stock"] = Convert.ToInt32(rv["stock"]);
                        r["precio_costo"] = Convert.ToDecimal(rv["precio_costo"]);
                        r["impuesto_id"] = Convert.ToInt32(rv["impuesto_id"]);
                        r["is_create"] = false;
                        sugerencias.Rows.Add(r);
                    }
                }
                else
                {
                    var r = sugerencias.NewRow();
                    r["id"] = 0L;
                    r["nombre"] = "➕ Crear producto: " + texto;
                    r["stock"] = 0;
                    r["precio_costo"] = 0;
                    r["impuesto_id"] = 0;
                    r["is_create"] = true;
                    sugerencias.Rows.Add(r);
                }

                dtSugerencias = sugerencias;

                lstProductos.DataSource = null;
                lstProductos.DisplayMember = "nombre";
                lstProductos.ValueMember = "id";
                lstProductos.DataSource = dtSugerencias;

                MostrarListaDebajo();
            }
            finally
            {
                _actualizandoLista = false;
            }
        }

        private void MostrarListaDebajo()
        {
            lstProductos.Visible = true;
            lstProductos.Left = txtBuscarProducto.Left;
            lstProductos.Top = txtBuscarProducto.Bottom + 2;
            lstProductos.Width = txtBuscarProducto.Width;
            lstProductos.BringToFront();
            lstProductos.Height = Math.Min(200, (dtSugerencias.Rows.Count * 22) + 4);
        }

        private void OcultarListaProductos()
        {
            lstProductos.Visible = false;
            lstProductos.DataSource = null;
            productoIdSeleccionado = 0;
            lblStockSel.Text = "Stock: -";
            nudCosto.Value = 0;
            costoOriginalProducto = 0;
        }

        private void ConfirmarProductoSeleccionado()
        {
            if (_actualizandoLista) return;

            var drv = lstProductos.SelectedItem as DataRowView;
            if (drv == null) return;

            bool isCreate = Convert.ToBoolean(drv["is_create"]);
            long id = Convert.ToInt64(drv["id"]);
            string nombre = drv["nombre"].ToString();
            int stock = Convert.ToInt32(drv["stock"]);
            decimal precioCosto = Convert.ToDecimal(drv["precio_costo"]);
            int impuestoId = Convert.ToInt32(drv["impuesto_id"]);

            if (isCreate)
            {
                string texto = (txtBuscarProducto.Text ?? "").Trim();
                AbrirPopupNuevoProducto(texto);
                return;
            }

            productoIdSeleccionado = id;
            txtBuscarProducto.Text = nombre;
            lblStockSel.Text = $"Stock: {stock}";

            // Precargar el costo y el impuesto
            nudCosto.Value = precioCosto;
            costoOriginalProducto = precioCosto;

            // Seleccionar el impuesto correspondiente
            impuestoIdOriginal = impuestoId;
            for (int i = 0; i < cmbImpuestoCompra.Items.Count; i++)
            {
                var row = ((DataRowView)cmbImpuestoCompra.Items[i]).Row;
                if (Convert.ToInt32(row["id"]) == impuestoId)
                {
                    cmbImpuestoCompra.SelectedIndex = i;
                    break;
                }
            }

            lstProductos.Visible = false;
        }

        private string GenerarNumeroCompraAuto()
        {
            return "CMP" + DateTime.Now.ToString("yyMMddHHmmss");
        }

        private void AbrirPopupNuevoProducto(string nombreSugerido)
        {
            using (var pop = new FormProductoPopup(nombreSugerido))
            {
                if (pop.ShowDialog(this) == DialogResult.OK)
                {
                    CargarProductosBuscador();

                    productoIdSeleccionado = pop.ProductoCreadoId;
                    txtBuscarProducto.Text = pop.ProductoCreadoNombre;

                    lblStockSel.Text = "Stock: 0";
                    lstProductos.Visible = false;
                }
            }
        }

        private decimal? ObtenerCostoReferencia(long productoId)
        {
            var rows = dtProductos.Select($"id = {productoId}");
            if (rows.Length == 0) return null;

            object v = rows[0]["precio_costo"];
            if (v == null || v == DBNull.Value) return null;

            return Convert.ToDecimal(v);
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

            if (costo <= 0)
            {
                MessageBox.Show("El costo debe ser mayor a 0.");
                return;
            }

            // Si ya está, sumamos cantidad
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

            // Limpiar selección de producto para el siguiente item
            productoIdSeleccionado = 0;
            txtBuscarProducto.Clear();
            lblStockSel.Text = "Stock: -";
            nudCantidad.Value = 1;
            nudCosto.Value = 0;
            costoOriginalProducto = 0;
            lstProductos.Visible = false;

            RecalcularTotales();
        }

        private void RecalcularTotales()
        {
            decimal subtotal = 0;

            foreach (DataRow row in dtItems.Rows)
                subtotal += Convert.ToDecimal(row["subtotal"]);

            // Obtener el porcentaje de IVA seleccionado
            decimal porcentajeIVA = 0;
            if (cmbImpuestoCompra.SelectedItem != null)
            {
                var row = ((DataRowView)cmbImpuestoCompra.SelectedItem).Row;
                porcentajeIVA = Convert.ToDecimal(row["porcentaje"]) / 100m;
            }

            decimal iva = subtotal * porcentajeIVA;
            decimal total = subtotal + iva;

            lblSubtotal.Text = $"Subtotal: {subtotal:C2}";
            lblIVA.Text = $"IVA ({porcentajeIVA * 100:0}%): {iva:C2}";
            lblTotal.Text = $"Total: {total:C2}";
        }

        private void GuardarCompra()
        {
            if (usuarioId <= 0)
            {
                MessageBox.Show("Error: Sesión inválida (usuarioId). Vuelve a iniciar sesión.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
            string factura = GenerarNumeroCompraAuto();
            DateTime fecha = DateTime.Now;

            decimal subtotal = 0;
            foreach (DataRow row in dtItems.Rows)
                subtotal += Convert.ToDecimal(row["subtotal"]);

            // Obtener el porcentaje de IVA seleccionado para la compra (para la factura)
            decimal porcentajeIVACompra = 0;
            int impuestoIdCompra = 0;
            string nombreIVACompra = "IVA";
            if (cmbImpuestoCompra.SelectedItem != null)
            {
                var row = ((DataRowView)cmbImpuestoCompra.SelectedItem).Row;
                porcentajeIVACompra = Convert.ToDecimal(row["porcentaje"]) / 100m;
                impuestoIdCompra = Convert.ToInt32(row["id"]);
                nombreIVACompra = row["nombre"].ToString();
            }

            decimal ivaTotal = subtotal * porcentajeIVACompra;
            decimal totalCompra = subtotal + ivaTotal;

            try
            {
                con.Abrir();

                using (SqlTransaction tx = con.leer.BeginTransaction())
                {
                    try
                    {
                        // 1. Insertar en Compras
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
                            cmd.Parameters.AddWithValue("@total", totalCompra);

                            compraId = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        // 2. Insertar en FacturaCompra y obtener el ID generado
                        long facturaCompraId;
                        using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO FacturaCompra
                    (proveedor_id, usuario_id, clave_acceso_proveedor, serie_comprobante, secuencial_proveedor, 
                     fecha_emision_proveedor, fecha_registro_sistema, subtotal_15_iva, subtotal_0_iva, 
                     descuento_total, valor_iva, total_factura, estado, compra_id)
                    OUTPUT INSERTED.id
                    VALUES
                    (@prov, @user, @clave, @serie, @sec, @fecha, GETDATE(), @sub15, @sub0, 0, @iva, @total, 'Registrada', @compra)",
                            con.leer, tx))
                        {
                            // Datos del proveedor seleccionado
                            DataRow proveedorRow = dtProveedores.Select($"id = {proveedorId}").FirstOrDefault();
                            string ruc = proveedorRow?["ruc"]?.ToString() ?? "9999999999999";

                            cmd.Parameters.AddWithValue("@prov", proveedorId);
                            cmd.Parameters.AddWithValue("@user", usuarioId);
                            cmd.Parameters.AddWithValue("@clave", GenerarClaveAccesoCompra(ruc, fecha));
                            cmd.Parameters.AddWithValue("@serie", "001-001");
                            cmd.Parameters.AddWithValue("@sec", GenerarSecuencialCompra());
                            cmd.Parameters.AddWithValue("@fecha", fecha);
                            cmd.Parameters.AddWithValue("@sub15", subtotal);
                            cmd.Parameters.AddWithValue("@sub0", 0m);
                            cmd.Parameters.AddWithValue("@iva", ivaTotal);
                            cmd.Parameters.AddWithValue("@total", totalCompra);
                            cmd.Parameters.AddWithValue("@compra", compraId);

                            facturaCompraId = (long)cmd.ExecuteScalar();
                        }

                        // 3. Insertar detalles y actualizar productos
                        foreach (DataRow r in dtItems.Rows)
                        {
                            long prodId = Convert.ToInt64(r["producto_id"]);
                            int cant = Convert.ToInt32(r["cantidad"]);
                            decimal costo = Convert.ToDecimal(r["costo_unitario"]); // Este es el costo SIN IVA

                            // Obtener el impuesto del producto (no el de la compra)
                            int impuestoIdProducto = impuestoIdOriginal; // El que se cargó al seleccionar el producto

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

                            // stock - Guardamos el costo SIN IVA y el impuesto del producto
                            using (SqlCommand cmdStock = new SqlCommand(@"
                        UPDATE Productos
                        SET stock = ISNULL(stock,0) + @cant,
                            precio_costo = @costo,
                            impuesto_id = @impuestoProducto
                        WHERE id = @p;", con.leer, tx))
                            {
                                cmdStock.Parameters.AddWithValue("@cant", cant);
                                cmdStock.Parameters.AddWithValue("@costo", costo); // Costo SIN IVA
                                cmdStock.Parameters.AddWithValue("@impuestoProducto", impuestoIdProducto); // IVA del producto
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

                        // 4. Commit de la transacción
                        tx.Commit();

                        // 5. AHORA generar y guardar el PDF (FUERA de la transacción)
                        try
                        {
                            string pdfPath = GenerarPdfFacturaCompra(paraGuardar: true);
                            GuardarPdfEnFacturaCompra(facturaCompraId, pdfPath);

                            MessageBox.Show("✅ Compra guardada. PDF generado correctamente.",
                                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception exPdf)
                        {
                            MessageBox.Show($"⚠️ Compra guardada pero hubo un error al generar el PDF:\n{exPdf.Message}",
                                "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }

                        // 6. Recargar datos y limpiar formulario
                        CargarProductosBuscador();
                        LimpiarTodo();
                    }
                    catch (SqlException ex)
                    {
                        tx.Rollback();
                        MessageBox.Show($"Error SQL: {ex.Message}\n\nDetalle: {ex.StackTrace}",
                            "Error de Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MessageBox.Show($"Error: {ex.Message}\n\nDetalle: {ex.StackTrace}",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                con.Cerrar();
            }
        }

        private string GenerarClaveAccesoCompra(string ruc, DateTime fecha)
        {
            // Formato simplificado para clave de acceso de compra
            string baseStr = fecha.ToString("ddMMyyyy") + ruc.PadLeft(13, '0') +
                            "001001" + DateTime.Now.ToString("HHmmss") + "12345678";
            return baseStr.PadRight(49, '1').Substring(0, 49);
        }

        private string GenerarSecuencialCompra()
        {
            try
            {
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(secuencial_proveedor AS INT)), 0) + 1 FROM FacturaCompra", con.leer))
                {
                    int next = Convert.ToInt32(cmd.ExecuteScalar());
                    return next.ToString().PadLeft(9, '0');
                }
            }
            catch
            {
                return "000000001";
            }
        }

        private void LimpiarTodo()
        {
            
            txtBuscarProducto.Clear();
            lstProductos.Visible = false;

            dtItems.Clear();
            productoIdSeleccionado = 0;

            nudCantidad.Value = 1;
            nudCosto.Value = 0;
            costoOriginalProducto = 0;

            lblStockSel.Text = "Stock: -";
            RecalcularTotales();
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

        private void btnBuscadorProductos_Click(object sender, EventArgs e)
        {
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Productos);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long productoId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();
                decimal precio = Convert.ToDecimal(fila["precio_pvp"]);
                int cantidad = buscador.CantidadSeleccionada;

                // Buscar el producto en dtProductos para obtener más datos
                DataRow[] rows = dtProductos.Select($"id = {productoId}");
                if (rows.Length > 0)
                {
                    DataRow productoRow = rows[0];

                    productoIdSeleccionado = productoId;
                    txtBuscarProducto.Text = nombre;

                    int stock = Convert.ToInt32(productoRow["stock"]);
                    lblStockSel.Text = $"Stock: {stock}";

                    decimal costoActual = Convert.ToDecimal(productoRow["precio_costo"]);
                    nudCosto.Value = costoActual;
                    costoOriginalProducto = costoActual;

                    // IMPORTANTE: Guardar el impuesto del producto
                    int impuestoId = Convert.ToInt32(productoRow["impuesto_id"]);
                    impuestoIdOriginal = impuestoId;

                    // Seleccionar el impuesto correspondiente en el ComboBox de compra
                    // (esto es solo para la factura de compra, no afecta al producto)
                    for (int i = 0; i < cmbImpuestoCompra.Items.Count; i++)
                    {
                        var row = ((DataRowView)cmbImpuestoCompra.Items[i]).Row;
                        if (Convert.ToInt32(row["id"]) == impuestoId)
                        {
                            cmbImpuestoCompra.SelectedIndex = i;
                            break;
                        }
                    }

                    // Opcional: Preguntar si quiere agregar el producto automáticamente
                    DialogResult agregarAuto = MessageBox.Show(
                        $"¿Desea agregar {cantidad} unidad(es) de '{nombre}' a la compra?\n\n" +
                        $"Costo SIN IVA: {costoActual:C4}\n" +
                        $"IVA del producto: {ObtenerNombreImpuesto(impuestoId)}",
                        "Agregar automáticamente",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (agregarAuto == DialogResult.Yes)
                    {
                        nudCantidad.Value = cantidad;
                        AgregarItem();
                    }
                }
                else
                {
                    CargarProductosBuscador();
                    MessageBox.Show("Producto seleccionado. Por favor, selecciónelo nuevamente de la lista.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        // Método auxiliar para obtener nombre del impuesto
        private string ObtenerNombreImpuesto(int impuestoId)
        {
            foreach (DataRowView item in cmbImpuestoCompra.Items)
            {
                if (Convert.ToInt32(item.Row["id"]) == impuestoId)
                    return item.Row["nombre"].ToString();
            }
            return "IVA";
        }

        private void btnVistaPreviaCompra_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtItems == null || dtItems.Rows.Count == 0)
                {
                    MessageBox.Show("No hay items para generar la factura.");
                    return;
                }

                if (cmbProveedor.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un proveedor.");
                    return;
                }

                string pdfPath = GenerarPdfFacturaCompra();

                using (var viewer = new FormPdfViewer(
                    pdfPath,
                    title: $"Vista previa - Factura de Compra",
                    defaultSaveName: $"Factura_Compra_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"))
                {
                    viewer.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generando vista previa: " + ex.Message);
            }
        }

        private string GenerarPdfFacturaCompra(bool paraGuardar = false)
        {
            if (dtItems == null || dtItems.Rows.Count == 0)
                throw new Exception("No hay items para generar el PDF.");

            // Obtener datos de la empresa
            var empresa = ObtenerEmpresa();

            // Obtener datos del proveedor seleccionado
            long proveedorId = Convert.ToInt64(cmbProveedor.SelectedValue);
            DataRow proveedorRow = dtProveedores.Select($"id = {proveedorId}").FirstOrDefault();

            string numFactura = GenerarNumeroCompraAuto();

            decimal subtotal = dtItems.AsEnumerable().Sum(r => Convert.ToDecimal(r["subtotal"]));

            // Obtener el porcentaje de IVA de la COMPRA (el que pagamos al proveedor)
            decimal porcentajeIVACompra = 0;
            string nombreIVACompra = "IVA";
            if (cmbImpuestoCompra.SelectedItem != null)
            {
                var row = ((DataRowView)cmbImpuestoCompra.SelectedItem).Row;
                porcentajeIVACompra = Convert.ToDecimal(row["porcentaje"]) / 100m;
                nombreIVACompra = row["nombre"].ToString();
            }

            decimal ivaCompra = subtotal * porcentajeIVACompra;
            decimal totalConIVA = subtotal + ivaCompra;

            string carpeta = Path.Combine(Path.GetTempPath(), "TallerMecanicoERP", "Compras");
            Directory.CreateDirectory(carpeta);

            string fileName = $"FacturaCompra_{numFactura}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string pdfPath = Path.Combine(carpeta, fileName);

            using (var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // Definir fuentes
                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                var fontSub = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var font = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                var fontSmall = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                var fontNote = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY);

                //  ENCABEZADO 
                PdfPTable head = new PdfPTable(2);
                head.WidthPercentage = 100;
                head.SetWidths(new float[] { 65, 35 });

                // Celda Empresa
                var cellEmp = new PdfPCell();
                cellEmp.Border = PdfRectangle.BOX;
                cellEmp.Padding = 8;

                Phrase phraseEmp = new Phrase();
                phraseEmp.Add(new Chunk(empresa.nombre?.ToUpper() ?? "EMPRESA\n", fontSub));
                phraseEmp.Add(new Chunk("\n", fontSmall)); // Espacio
                if (!string.IsNullOrWhiteSpace(empresa.ruc))
                    phraseEmp.Add(new Chunk($"RUC: {empresa.ruc}\n", font));
                if (!string.IsNullOrWhiteSpace(empresa.direccion))
                    phraseEmp.Add(new Chunk($"Dirección: {empresa.direccion}\n", font));
                if (!string.IsNullOrWhiteSpace(empresa.telefono))
                    phraseEmp.Add(new Chunk($"Tel: {empresa.telefono}\n", font));
                if (!string.IsNullOrWhiteSpace(empresa.email))
                    phraseEmp.Add(new Chunk($"Email: {empresa.email}\n", font));

                cellEmp.AddElement(phraseEmp);
                head.AddCell(cellEmp);

                // Celda Factura
                var cellFac = new PdfPCell();
                cellFac.Border = PdfRectangle.BOX;
                cellFac.Padding = 8;

                Phrase phraseFac = new Phrase();
                phraseFac.Add(new Chunk("FACTURA DE COMPRA\n", fontTitle));
                phraseFac.Add(new Chunk($"No. Documento: {numFactura}\n", fontSub));
                phraseFac.Add(new Chunk($"Fecha Emisión: {DateTime.Now:dd/MM/yyyy HH:mm}\n", font));

                cellFac.AddElement(phraseFac);
                head.AddCell(cellFac);

                doc.Add(head);
                doc.Add(new Paragraph(" "));

                //  DATOS DEL PROVEEDOR 
                PdfPTable proveedorTable = new PdfPTable(2);
                proveedorTable.WidthPercentage = 100;
                proveedorTable.SetWidths(new float[] { 50, 50 });

                var cellProv1 = new PdfPCell(new Phrase("PROVEEDOR:", fontSub));
                cellProv1.Colspan = 2;
                cellProv1.Padding = 5;
                cellProv1.Border = PdfRectangle.BOX;
                proveedorTable.AddCell(cellProv1);

                var cellProv2 = new PdfPCell(new Phrase($"Razón Social: {proveedorRow["nombre_empresa"]}", font));
                cellProv2.Padding = 5;
                cellProv2.Border = PdfRectangle.BOX;
                proveedorTable.AddCell(cellProv2);

                var cellProv3 = new PdfPCell(new Phrase($"RUC: {proveedorRow["ruc"]}", font));
                cellProv3.Padding = 5;
                cellProv3.Border = PdfRectangle.BOX;
                proveedorTable.AddCell(cellProv3);

                var cellProv4 = new PdfPCell(new Phrase($"Dirección: {proveedorRow["direccion"]?.ToString() ?? "-"}", font));
                cellProv4.Padding = 5;
                cellProv4.Border = PdfRectangle.BOX;
                proveedorTable.AddCell(cellProv4);

                var cellProv5 = new PdfPCell(new Phrase($"Teléfono: {proveedorRow["telefono"]?.ToString() ?? "-"}", font));
                cellProv5.Padding = 5;
                cellProv5.Border = PdfRectangle.BOX;
                proveedorTable.AddCell(cellProv5);

                var cellProv6 = new PdfPCell(new Phrase($"Email: {proveedorRow["email"]?.ToString() ?? "-"}", font));
                cellProv6.Padding = 5;
                cellProv6.Border = PdfRectangle.BOX;
                proveedorTable.AddCell(cellProv6);

                if (proveedorRow["contacto_nombre"] != DBNull.Value)
                {
                    var cellProv7 = new PdfPCell(new Phrase($"Contacto: {proveedorRow["contacto_nombre"]}", font));
                    cellProv7.Colspan = 2;
                    cellProv7.Padding = 5;
                    cellProv7.Border = PdfRectangle.BOX;
                    proveedorTable.AddCell(cellProv7);
                }

                doc.Add(proveedorTable);
                doc.Add(new Paragraph(" "));

                //  TABLA DE ITEMS 
                PdfPTable items = new PdfPTable(6); // Aumentamos a 6 columnas
                items.WidthPercentage = 100;
                items.SetWidths(new float[] { 8, 32, 12, 12, 12, 24 });

                void AddHeader(string t)
                {
                    var c = new PdfPCell(new Phrase(t, fontSub));
                    c.Padding = 6;
                    c.BackgroundColor = BaseColor.LIGHT_GRAY;
                    c.HorizontalAlignment = Element.ALIGN_CENTER;
                    items.AddCell(c);
                }

                AddHeader("Cant.");
                AddHeader("Descripción");
                AddHeader("Costo U.");
                AddHeader("Subtotal");
                AddHeader("% IVA Compra");
                AddHeader("IVA Venta (Producto)");

                foreach (DataRow row in dtItems.Rows)
                {
                    decimal cant = Convert.ToDecimal(row["cantidad"]);
                    string desc = row["producto"]?.ToString() ?? "";
                    decimal costo = Convert.ToDecimal(row["costo_unitario"]);
                    decimal sub = Convert.ToDecimal(row["subtotal"]);

                    // Obtener el IVA del producto (para venta)
                    int impuestoIdProducto = impuestoIdOriginal;
                    string nombreIVAProducto = "IVA";
                    decimal porcentajeIVAProducto = 0;

                    // Buscar el porcentaje del IVA del producto
                    foreach (DataRowView item in cmbImpuestoCompra.Items)
                    {
                        if (Convert.ToInt32(item.Row["id"]) == impuestoIdProducto)
                        {
                            nombreIVAProducto = item.Row["nombre"].ToString();
                            porcentajeIVAProducto = Convert.ToDecimal(item.Row["porcentaje"]);
                            break;
                        }
                    }

                    items.AddCell(new PdfPCell(new Phrase(cant.ToString("0.##"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER });
                    items.AddCell(new PdfPCell(new Phrase(desc, font)) { Padding = 5 });
                    items.AddCell(new PdfPCell(new Phrase(costo.ToString("N4"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    items.AddCell(new PdfPCell(new Phrase(sub.ToString("N4"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    items.AddCell(new PdfPCell(new Phrase($"{nombreIVACompra} ({porcentajeIVACompra * 100:0}%)", font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER });
                    items.AddCell(new PdfPCell(new Phrase($"{nombreIVAProducto} ({porcentajeIVAProducto:0}%)", font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER });
                }

                doc.Add(items);
                doc.Add(new Paragraph(" "));

                //  TOTALES 
                PdfPTable tot = new PdfPTable(2);
                tot.HorizontalAlignment = Element.ALIGN_RIGHT;
                tot.WidthPercentage = 45;
                tot.SetWidths(new float[] { 70, 30 });

                void AddTotalRow(string k, string v, bool bold = false)
                {
                    var fk = bold ? fontSub : font;
                    var fv = bold ? fontSub : font;

                    var cellK = new PdfPCell(new Phrase(k, fk)) { Padding = 6, Border = PdfRectangle.BOX };
                    var cellV = new PdfPCell(new Phrase(v, fv)) { Padding = 6, Border = PdfRectangle.BOX, HorizontalAlignment = Element.ALIGN_RIGHT };

                    if (bold)
                    {
                        cellK.BackgroundColor = BaseColor.LIGHT_GRAY;
                        cellV.BackgroundColor = BaseColor.LIGHT_GRAY;
                    }

                    tot.AddCell(cellK);
                    tot.AddCell(cellV);
                }

                AddTotalRow("SUBTOTAL (sin IVA)", $"$ {subtotal:N4}");
                AddTotalRow($"{nombreIVACompra} ({porcentajeIVACompra * 100:0}%) - DÉBITO FISCAL", $"$ {ivaCompra:N4}");

                // Línea separadora visual
                var cellSep = new PdfPCell(new Phrase(""));
                cellSep.Colspan = 2;
                cellSep.Border = PdfRectangle.NO_BORDER;
                cellSep.FixedHeight = 5;
                tot.AddCell(cellSep);

                AddTotalRow("TOTAL A PAGAR", $"$ {totalConIVA:N4}", bold: true);

                doc.Add(tot);
                doc.Add(new Paragraph(" "));

                //  NOTA INFORMATIVA SOBRE IVA 
                PdfPTable noteTable = new PdfPTable(1);
                noteTable.WidthPercentage = 100;

                var cellNote = new PdfPCell();
                cellNote.Border = PdfRectangle.BOX;
                cellNote.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellNote.Padding = 6;

                Phrase phraseNote = new Phrase();
                phraseNote.Add(new Chunk("NOTA INFORMATIVA SOBRE IVA:\n", fontSub));
                phraseNote.Add(new Chunk("• El IVA mostrado en 'IVA Compra' es el Impuesto al Valor Agregado pagado al proveedor (DÉBITO FISCAL).\n", fontNote));
                phraseNote.Add(new Chunk("• El IVA mostrado en 'IVA Venta' es el impuesto que se aplicará al vender el producto (CRÉDITO FISCAL).\n", fontNote));
                phraseNote.Add(new Chunk("• El costo del producto se registra SIN IVA para efectos de inventario y cálculo de ganancias.", fontNote));

                cellNote.AddElement(phraseNote);
                noteTable.AddCell(cellNote);

                doc.Add(noteTable);
                doc.Add(new Paragraph(" "));

                //  PIE DE PÁGINA 
                PdfPTable footer = new PdfPTable(1);
                footer.WidthPercentage = 100;

                var cellFooter = new PdfPCell();
                cellFooter.Border = PdfRectangle.TOP_BORDER;
                cellFooter.PaddingTop = 8;

                Phrase phraseFooter = new Phrase();
                phraseFooter.Add(new Chunk("Documento generado electrónicamente.\n", fontSmall));
                phraseFooter.Add(new Chunk($"Usuario: {usuarioId} - Fecha impresión: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontSmall));

                cellFooter.AddElement(phraseFooter);
                footer.AddCell(cellFooter);

                doc.Add(footer);

                doc.Close();
                writer.Close();
            }

            return pdfPath;
        }

        private (string nombre, string ruc, string direccion, string telefono, string email) ObtenerEmpresa()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 nombre, ruc, direccion, telefono, email FROM Empresa WHERE id = 1", cn))
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        return (
                            rd["nombre"]?.ToString() ?? "EMPRESA",
                            rd["ruc"]?.ToString() ?? "",
                            rd["direccion"]?.ToString() ?? "",
                            rd["telefono"]?.ToString() ?? "",
                            rd["email"]?.ToString() ?? ""
                        );
                    }
                }
            }
            catch { }

            return ("EMPRESA NO CONFIGURADA", "", "", "", "");
        }

        private void GuardarPdfEnFacturaCompra(long facturaCompraId, string pdfPath)
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string nombre = Path.GetFileName(pdfPath);

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(@"
        UPDATE FacturaCompra
        SET pdf_data = @pdf,
            pdf_nombre = @nom
        WHERE id = @id;", cn))
            {
                cmd.Parameters.Add("@pdf", SqlDbType.VarBinary, -1).Value = pdfBytes;
                cmd.Parameters.AddWithValue("@nom", nombre);
                cmd.Parameters.AddWithValue("@id", facturaCompraId);
                cmd.ExecuteNonQuery();
            }

            try
            {
                if (File.Exists(pdfPath))
                    File.Delete(pdfPath);
            }
            catch { }
        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }

        private void lblIVA_Click(object sender, EventArgs e)
        {

        }

        private void lblSubtotal_Click(object sender, EventArgs e)
        {

        }
    }
}




