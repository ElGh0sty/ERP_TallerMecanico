using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using PROYECTOMECANICO.Modulo_Clientes;

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormGenFactu : Form
    {
        private readonly Conexion con = new Conexion();
        private long usuarioId;

        // Estado origen
        private bool modoDesdeOT = true;
        private long? ordenTrabajoId = null;

        // Receptor
        private long? clienteId = null; // NULL permitido para consumidor final real
        private string snapTipoDoc = "";
        private string snapNumDoc = "";
        private string snapNombre = "";
        private string snapDireccion = "";
        private string snapTelefono = "";
        private string snapEmail = "";
        private bool snapContribuyenteEspecial = false;

        // Items
        private DataTable dtItems = new DataTable();

        // Factura generada
        private long facturaIdGenerada = 0;
        private string secuencialGenerado = "";
        private string claveAccesoGenerada = "";

        // Print
        private readonly PrintDocument printDoc = new PrintDocument();
        private readonly PrintPreviewDialog preview = new PrintPreviewDialog();

        // Constructor para diseñador
        public FormGenFactu()
        {
            InitializeComponent();
            InitCommon(0); // no usar para producción
        }

        // Constructor real
        public FormGenFactu(long usuarioId)
        {
            InitializeComponent();
            InitCommon(usuarioId);
        }

        private void InitCommon(long userId)
        {
            this.usuarioId = userId;

            // Grid + tabla items
            PrepararTablaItems();
            ConfigurarGrid();
            dgvItems.DataSource = dtItems;

            // Impuestos
            CargarImpuestos();

            // Print
            preview.Document = printDoc;
            printDoc.PrintPage += PrintDoc_PrintPage;

            // Defaults
            rbDesdeOT.Checked = true;
            rbClienteExistente.Checked = true;

            // Eventos modo
            rbDesdeOT.CheckedChanged += (s, e) => { if (rbDesdeOT.Checked) CambiarModo(true); };
            rbVentaDirecta.CheckedChanged += (s, e) => { if (rbVentaDirecta.Checked) CambiarModo(false); };

            // OT
            txtBuscarOT.TextChanged += (s, e) => BuscarOT();
            lstOTResultados.Click += (s, e) => SeleccionarOT();
            btnCargarItemsOT.Click += (s, e) => CargarItemsOT();

            // Receptor
            rbClienteExistente.CheckedChanged += (s, e) => { if (rbClienteExistente.Checked) ActualizarUIReceptor(); };
            rbNuevoCliente.CheckedChanged += (s, e) => { if (rbNuevoCliente.Checked) ActualizarUIReceptor(); };
            rbConsumidorFinal.CheckedChanged += (s, e) => { if (rbConsumidorFinal.Checked) SetConsumidorFinal(); };

            txtBuscarCliente.TextChanged += (s, e) => BuscarCliente();
            lstClientes.Click += (s, e) => SeleccionarCliente();
            btnNuevoCliente.Click += (s, e) => CrearClientePopup();

            // Items manuales
            btnAddItem.Click += (s, e) => AgregarItemManual();
            btnDelItem.Click += (s, e) => EliminarItemSeleccionado();

            // Totales
            cmbImpuesto.SelectedIndexChanged += (s, e) => RecalcularTotales();
            dgvItems.CellEndEdit += (s, e) => { RecalcularSubtotalFila(e.RowIndex); RecalcularTotales(); };
            dgvItems.RowsRemoved += (s, e) => RecalcularTotales();

            // Acciones
            btnGenerarFactura.Click += (s, e) => GenerarFactura();
            btnVistaPrevia.Click += (s, e) => MostrarVistaPrevia();

            // Estado inicial UI
            CambiarModo(true);
            ActualizarUIReceptor();
            RecalcularTotales();
        }

        // ======================= MODO =======================
        private void CambiarModo(bool desdeOT)
        {
            modoDesdeOT = desdeOT;
            ordenTrabajoId = null;

            txtBuscarOT.Enabled = desdeOT;
            btnCargarItemsOT.Enabled = desdeOT;
            lstOTResultados.Visible = false;

            btnAddItem.Enabled = !desdeOT;
            btnDelItem.Enabled = !desdeOT;

            dtItems.Clear();
            RecalcularTotales();
        }

        // ======================= ITEMS =======================
        private void PrepararTablaItems()
        {
            dtItems = new DataTable();
            dtItems.Columns.Add("descripcion", typeof(string));
            dtItems.Columns.Add("cantidad", typeof(decimal));
            dtItems.Columns.Add("precio_unitario", typeof(decimal));
            dtItems.Columns.Add("subtotal", typeof(decimal));
            dtItems.Columns.Add("producto_id", typeof(long));
            dtItems.Columns.Add("servicio_id", typeof(long));
        }

        private void ConfigurarGrid()
        {
            dgvItems.AutoGenerateColumns = false;
            dgvItems.Columns.Clear();

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = "descripcion" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cant", DataPropertyName = "cantidad" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "P.Unit", DataPropertyName = "precio_unitario" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = "subtotal", ReadOnly = true });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "producto_id", DataPropertyName = "producto_id", Visible = false });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "servicio_id", DataPropertyName = "servicio_id", Visible = false });
        }

        private void AgregarItemManual()
        {
            dtItems.Rows.Add("Servicio / Producto", 1m, 0m, 0m, DBNull.Value, DBNull.Value);
            RecalcularTotales();
        }

        private void EliminarItemSeleccionado()
        {
            if (dgvItems.CurrentRow != null && dgvItems.CurrentRow.Index >= 0)
            {
                dgvItems.Rows.RemoveAt(dgvItems.CurrentRow.Index);
                RecalcularTotales();
            }
        }

        private void RecalcularSubtotalFila(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= dtItems.Rows.Count) return;
            try
            {
                var r = dtItems.Rows[rowIndex];
                decimal cant = Convert.ToDecimal(r["cantidad"]);
                decimal pu = Convert.ToDecimal(r["precio_unitario"]);
                r["subtotal"] = Math.Round(cant * pu, 4);
            }
            catch { /* ignore */ }
        }

        // ======================= IMPUESTO / TOTALES =======================
        private void CargarImpuestos()
        {
            try
            {
                con.Abrir();
                var dt = new DataTable();
                using (var da = new SqlDataAdapter("SELECT id, nombre, porcentaje FROM Impuestos WHERE activo = 1 ORDER BY id", con.leer))
                    da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    dt.Columns.Add("id", typeof(int));
                    dt.Columns.Add("nombre", typeof(string));
                    dt.Columns.Add("porcentaje", typeof(decimal));
                    dt.Rows.Add(1, "IVA (15%)", 15m);
                }

                cmbImpuesto.DataSource = dt;
                cmbImpuesto.DisplayMember = "nombre";
                cmbImpuesto.ValueMember = "id";
                cmbImpuesto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando impuestos:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private decimal GetImpuestoPorcentajeActual()
        {
            if (cmbImpuesto.SelectedItem is DataRowView drv &&
                decimal.TryParse(drv["porcentaje"].ToString(), out var p))
                return p;
            return 0m;
        }

        private void RecalcularTotales()
        {
            if (dtItems == null || dtItems.Rows.Count == 0)
            {
                lblSubtotal.Text = "Subtotal: 0.00";
                lblIVA.Text = "IVA: 0.00";
                lblTotal.Text = "Total: 0.00";
                return;
            }

            for (int i = 0; i < dtItems.Rows.Count; i++)
                RecalcularSubtotalFila(i);

            decimal subtotal = dtItems.AsEnumerable().Sum(r => Convert.ToDecimal(r["subtotal"]));
            decimal pct = GetImpuestoPorcentajeActual() / 100m;
            decimal iva = Math.Round(subtotal * pct, 2);
            decimal total = Math.Round(subtotal + iva, 2);

            lblSubtotal.Text = $"Subtotal: {subtotal:0.00}";
            lblIVA.Text = $"IVA: {iva:0.00}";
            lblTotal.Text = $"Total: {total:0.00}";
        }

        // ======================= OT =======================
        private class OTItem
        {
            public long Id { get; set; }
            public string Cliente { get; set; }
            public string Placa { get; set; }
            public override string ToString() => $"OT #{Id} | {Placa} | {Cliente}";
        }

        private void BuscarOT()
        {
            if (!modoDesdeOT) return;

            string q = (txtBuscarOT.Text ?? "").Trim();
            if (q.Length < 1)
            {
                lstOTResultados.Visible = false;
                lstOTResultados.DataSource = null;
                return;
            }

            try
            {
                con.Abrir();
                string sql = @"
SELECT TOP 15 
    OT.id,
    C.nombre AS cliente,
    V.placa
FROM OrdenesTrabajo OT
JOIN Vehiculos V ON V.id = OT.vehiculo_id
JOIN Clientes C ON C.id = V.cliente_id
WHERE CAST(OT.id AS NVARCHAR(20)) LIKE @q
   OR V.placa LIKE @q
   OR C.nombre LIKE @q
ORDER BY OT.id DESC";

                var list = new List<OTItem>();
                using (var cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = "%" + q + "%";
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new OTItem
                            {
                                Id = Convert.ToInt64(dr["id"]),
                                Cliente = dr["cliente"].ToString(),
                                Placa = dr["placa"].ToString()
                            });
                        }
                    }
                }

                lstOTResultados.DataSource = list;
                lstOTResultados.Visible = list.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error buscando OT:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void SeleccionarOT()
        {
            if (lstOTResultados.SelectedItem is OTItem item)
            {
                ordenTrabajoId = item.Id;
                lstOTResultados.Visible = false;

                // Cargar receptor por defecto desde OT
                CargarReceptorDesdeOT(item.Id);
            }
        }

        private void CargarReceptorDesdeOT(long otId)
        {
            try
            {
                con.Abrir();
                string sql = @"
SELECT TOP 1
    C.id AS cliente_id,
    C.tipo_documento, C.numero_documento, C.nombre,
    C.direccion, C.telefono, C.email, C.contribuyente_especial
FROM OrdenesTrabajo OT
JOIN Vehiculos V ON V.id = OT.vehiculo_id
JOIN Clientes C ON C.id = V.cliente_id
WHERE OT.id = @id";

                using (var cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = otId;
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            clienteId = Convert.ToInt64(dr["cliente_id"]);
                            snapTipoDoc = dr["tipo_documento"].ToString();
                            snapNumDoc = dr["numero_documento"].ToString();
                            snapNombre = dr["nombre"].ToString();
                            snapDireccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString();
                            snapTelefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString();
                            snapEmail = dr["email"] == DBNull.Value ? "" : dr["email"].ToString();
                            snapContribuyenteEspecial = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"]);

                            rbClienteExistente.Checked = true;
                            ReflejarSnapshotEnUI();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando receptor desde OT:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarItemsOT()
        {
            if (!modoDesdeOT)
            {
                MessageBox.Show("Estás en modo Venta directa.");
                return;
            }

            if (ordenTrabajoId == null || ordenTrabajoId <= 0)
            {
                MessageBox.Show("Seleccione una OT primero.");
                return;
            }

            try
            {
                con.Abrir();
                string sql = @"
SELECT 
    COALESCE(descripcion, '') AS descripcion,
    CAST(cantidad AS DECIMAL(18,2)) AS cantidad,
    CAST(precio_unitario AS DECIMAL(18,2)) AS precio_unitario,
    CAST(subtotal AS DECIMAL(18,2)) AS subtotal,
    producto_id,
    servicio_id
FROM OrdenesTrabajo_Items
WHERE orden_id = @orden
ORDER BY id ASC";

                dtItems.Clear();
                using (var da = new SqlDataAdapter(sql, con.leer))
                {
                    da.SelectCommand.Parameters.Add("@orden", SqlDbType.BigInt).Value = ordenTrabajoId.Value;
                    da.Fill(dtItems);
                }

                RecalcularTotales();

                if (dtItems.Rows.Count == 0)
                    MessageBox.Show("Esta OT no tiene items todavía.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando items OT:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        // ======================= RECEPTOR =======================
        private void ActualizarUIReceptor()
        {
            bool existente = rbClienteExistente.Checked;
            bool nuevo = rbNuevoCliente.Checked;
            bool cf = rbConsumidorFinal.Checked;

            txtBuscarCliente.Enabled = existente;
            lstClientes.Visible = false;

            btnNuevoCliente.Enabled = nuevo;

            // Campos snapshot habilitados salvo CF (si quieres editables en CF, dímelo)
            bool enableSnap = !cf;
            txtTipoDoc.Enabled = enableSnap;
            txtNumDoc.Enabled = enableSnap;
            txtNombre.Enabled = enableSnap;
            txtDireccion.Enabled = enableSnap;
            txtTelefono.Enabled = enableSnap;
            txtEmail.Enabled = enableSnap;
            

            if (cf) SetConsumidorFinal();
        }

        private class ClienteItem
        {
            public long Id { get; set; }
            public string TipoDoc { get; set; }
            public string NumDoc { get; set; }
            public string Nombre { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
            public bool CE { get; set; }
            public override string ToString() => $"{Nombre} ({TipoDoc} {NumDoc})";
        }

        private void BuscarCliente()
        {
            if (!rbClienteExistente.Checked) return;

            string q = (txtBuscarCliente.Text ?? "").Trim();
            if (q.Length < 1)
            {
                lstClientes.Visible = false;
                lstClientes.DataSource = null;
                return;
            }

            try
            {
                con.Abrir();
                string sql = @"
SELECT TOP 15
    id, tipo_documento, numero_documento, nombre,
    direccion, telefono, email, contribuyente_especial
FROM Clientes
WHERE nombre LIKE @q OR numero_documento LIKE @q
ORDER BY nombre ASC";

                var list = new List<ClienteItem>();
                using (var cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = "%" + q + "%";
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ClienteItem
                            {
                                Id = Convert.ToInt64(dr["id"]),
                                TipoDoc = dr["tipo_documento"].ToString(),
                                NumDoc = dr["numero_documento"].ToString(),
                                Nombre = dr["nombre"].ToString(),
                                Direccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString(),
                                Telefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString(),
                                Email = dr["email"] == DBNull.Value ? "" : dr["email"].ToString(),
                                CE = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"])
                            });
                        }
                    }
                }

                lstClientes.DataSource = list;
                lstClientes.Visible = list.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error buscando cliente:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void SeleccionarCliente()
        {
            if (lstClientes.SelectedItem is ClienteItem c)
            {
                clienteId = c.Id;
                snapTipoDoc = c.TipoDoc;
                snapNumDoc = c.NumDoc;
                snapNombre = c.Nombre;
                snapDireccion = c.Direccion;
                snapTelefono = c.Telefono;
                snapEmail = c.Email;
                snapContribuyenteEspecial = c.CE;

                lstClientes.Visible = false;
                ReflejarSnapshotEnUI();
            }
        }

        private void SetConsumidorFinal()
        {
            clienteId = null;

            snapTipoDoc = "CF";
            snapNumDoc = "9999999999999";
            snapNombre = "CONSUMIDOR FINAL";
            snapDireccion = "";
            snapTelefono = "";
            snapEmail = "";
            snapContribuyenteEspecial = false;

            ReflejarSnapshotEnUI();
        }

        private void ReflejarSnapshotEnUI()
        {
            txtTipoDoc.Text = snapTipoDoc;
            txtNumDoc.Text = snapNumDoc;
            txtNombre.Text = snapNombre;
            txtDireccion.Text = snapDireccion;
            txtTelefono.Text = snapTelefono;
            txtEmail.Text = snapEmail;
            
        }

        private void TomarSnapshotDesdeUI()
        {
            snapTipoDoc = (txtTipoDoc.Text ?? "").Trim();
            snapNumDoc = (txtNumDoc.Text ?? "").Trim();
            snapNombre = (txtNombre.Text ?? "").Trim();
            snapDireccion = (txtDireccion.Text ?? "").Trim();
            snapTelefono = (txtTelefono.Text ?? "").Trim();
            snapEmail = (txtEmail.Text ?? "").Trim();
            
        }

        private void CrearClientePopup()
        {
            using (var pop = new FormClientePopup())
            {
                if (pop.ShowDialog() == DialogResult.OK)
                {
                    // Si aplicaste el mini-ajuste, esto existe:
                    if (pop.ClienteIdCreado != null)
                    {
                        CargarClientePorId(pop.ClienteIdCreado.Value);
                        rbClienteExistente.Checked = true;
                        return;
                    }

                    // Fallback (si no modificaste el popup): pide que lo busques por documento
                    MessageBox.Show("Cliente guardado. Búscalo por nombre o documento en el buscador para seleccionarlo.");
                    rbClienteExistente.Checked = true;
                }
            }
        }

        private void CargarClientePorId(long id)
        {
            try
            {
                con.Abrir();
                string sql = @"
SELECT id, tipo_documento, numero_documento, nombre, direccion, telefono, email, contribuyente_especial
FROM Clientes
WHERE id = @id";

                using (var cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = id;
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            clienteId = Convert.ToInt64(dr["id"]);
                            snapTipoDoc = dr["tipo_documento"].ToString();
                            snapNumDoc = dr["numero_documento"].ToString();
                            snapNombre = dr["nombre"].ToString();
                            snapDireccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString();
                            snapTelefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString();
                            snapEmail = dr["email"] == DBNull.Value ? "" : dr["email"].ToString();
                            snapContribuyenteEspecial = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"]);

                            ReflejarSnapshotEnUI();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando cliente:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        // ======================= GUARDAR FACTURA =======================
        private void GenerarFactura()
        {
            if (usuarioId <= 0)
            {
                MessageBox.Show("Sesión inválida (usuarioId).");
                return;
            }

            if (modoDesdeOT && (ordenTrabajoId == null || ordenTrabajoId <= 0))
            {
                MessageBox.Show("Seleccione una OT.");
                return;
            }

            if (dtItems == null || dtItems.Rows.Count == 0)
            {
                MessageBox.Show("No hay items para facturar.");
                return;
            }

            TomarSnapshotDesdeUI();

            // Si NO es consumidor final, validamos datos mínimos
            if (!rbConsumidorFinal.Checked)
            {
                if (string.IsNullOrWhiteSpace(snapTipoDoc) ||
                    string.IsNullOrWhiteSpace(snapNumDoc) ||
                    string.IsNullOrWhiteSpace(snapNombre))
                {
                    MessageBox.Show("Complete tipo doc, número doc y nombre del receptor.");
                    return;
                }
            }

            RecalcularTotales();

            decimal subtotal = dtItems.AsEnumerable().Sum(r => Convert.ToDecimal(r["subtotal"]));
            decimal pct = GetImpuestoPorcentajeActual() / 100m;
            decimal iva = Math.Round(subtotal * pct, 4);
            decimal total = Math.Round(subtotal + iva, 4);

            secuencialGenerado = GenerarSiguienteSecuencial();
            claveAccesoGenerada = GenerarClaveAcceso49();

            int impuestoId = Convert.ToInt32(cmbImpuesto.SelectedValue);

            try
            {
                con.Abrir();
                using (SqlTransaction tx = con.leer.BeginTransaction())
                {
                    try
                    {
                        // Facturas + snapshot
                        string sqlF = @"
INSERT INTO dbo.Facturas
(orden_trabajo_id, usuario_id, clave_acceso, numero_autorizacion, punto_emision, establecimiento, secuencial,
 subtotal_15_iva, subtotal_0_iva, valor_iva, total_final, fecha, cliente_id, total_descuento, propina,
 cliente_tipo_documento_snap, cliente_numero_documento_snap, cliente_nombre_snap, cliente_direccion_snap,
 cliente_telefono_snap, cliente_email_snap, cliente_contribuyente_especial_snap)
OUTPUT INSERTED.id
VALUES
(@ot, @user, @clave, NULL, @punto, @est, @sec,
 @sub15, @sub0, @iva, @total, GETDATE(), @cliente, 0, 0,
 @tdoc, @ndoc, @cnom, @cdir, @ctel, @cemail, @cces)";

                        long facturaId;
                        using (var cmd = new SqlCommand(sqlF, con.leer, tx))
                        {
                            cmd.Parameters.Add("@ot", SqlDbType.BigInt).Value = (object)ordenTrabajoId ?? DBNull.Value;
                            cmd.Parameters.Add("@user", SqlDbType.BigInt).Value = usuarioId;
                            cmd.Parameters.Add("@clave", SqlDbType.Char, 49).Value = claveAccesoGenerada;
                            cmd.Parameters.Add("@punto", SqlDbType.Char, 3).Value = "001";
                            cmd.Parameters.Add("@est", SqlDbType.Char, 3).Value = "001";
                            cmd.Parameters.Add("@sec", SqlDbType.Char, 9).Value = secuencialGenerado;

                            cmd.Parameters.Add("@sub15", SqlDbType.Decimal).Value = subtotal; // simplificado
                            cmd.Parameters.Add("@sub0", SqlDbType.Decimal).Value = 0m;
                            cmd.Parameters.Add("@iva", SqlDbType.Decimal).Value = iva;
                            cmd.Parameters.Add("@total", SqlDbType.Decimal).Value = total;

                            cmd.Parameters.Add("@cliente", SqlDbType.BigInt).Value = (object)clienteId ?? DBNull.Value;

                            cmd.Parameters.Add("@tdoc", SqlDbType.NVarChar, 10).Value = (object)snapTipoDoc ?? DBNull.Value;
                            cmd.Parameters.Add("@ndoc", SqlDbType.NVarChar, 13).Value = (object)snapNumDoc ?? DBNull.Value;
                            cmd.Parameters.Add("@cnom", SqlDbType.NVarChar, 255).Value = (object)snapNombre ?? DBNull.Value;
                            cmd.Parameters.Add("@cdir", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(snapDireccion) ? (object)DBNull.Value : snapDireccion;
                            cmd.Parameters.Add("@ctel", SqlDbType.NVarChar, 15).Value = string.IsNullOrWhiteSpace(snapTelefono) ? (object)DBNull.Value : snapTelefono;
                            cmd.Parameters.Add("@cemail", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(snapEmail) ? (object)DBNull.Value : snapEmail;
                            cmd.Parameters.Add("@cces", SqlDbType.Bit).Value = snapContribuyenteEspecial;

                            facturaId = Convert.ToInt64(cmd.ExecuteScalar());
                        }

                        // Items
                        string sqlI = @"
INSERT INTO dbo.FacturaItems
(factura_id, producto_id, servicio_id, cantidad, precio_unitario, descuento_item, impuesto_id, valor_iva_item)
VALUES
(@factura, @prod, @serv, @cant, @punit, 0, @imp, @ivaItem)";

                        foreach (DataRow r in dtItems.Rows)
                        {
                            long? prod = r["producto_id"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["producto_id"]);
                            long? serv = r["servicio_id"] == DBNull.Value ? (long?)null : Convert.ToInt64(r["servicio_id"]);

                            decimal cantDec = Convert.ToDecimal(r["cantidad"]);
                            int cantInt = Math.Max(1, (int)Math.Round(cantDec, 0)); // tu columna es INT
                            decimal punit = Convert.ToDecimal(r["precio_unitario"]);
                            decimal subItem = Convert.ToDecimal(r["subtotal"]);
                            decimal ivaItem = Math.Round(subItem * pct, 4);

                            using (var cmd = new SqlCommand(sqlI, con.leer, tx))
                            {
                                cmd.Parameters.Add("@factura", SqlDbType.BigInt).Value = facturaId;
                                cmd.Parameters.Add("@prod", SqlDbType.BigInt).Value = (object)prod ?? DBNull.Value;
                                cmd.Parameters.Add("@serv", SqlDbType.BigInt).Value = (object)serv ?? DBNull.Value;
                                cmd.Parameters.Add("@cant", SqlDbType.Int).Value = cantInt;
                                cmd.Parameters.Add("@punit", SqlDbType.Decimal).Value = punit;
                                cmd.Parameters.Add("@imp", SqlDbType.Int).Value = impuestoId;
                                cmd.Parameters.Add("@ivaItem", SqlDbType.Decimal).Value = ivaItem;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();

                        facturaIdGenerada = facturaId;
                        MessageBox.Show($"Factura generada ✅\nID: {facturaIdGenerada}\nSecuencial: {secuencialGenerado}");
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        MessageBox.Show("Error generando factura:\n" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error BD:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private string GenerarSiguienteSecuencial()
        {
            try
            {
                con.Abrir();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(secuencial AS INT)), 0) + 1 FROM dbo.Facturas", con.leer))
                {
                    int next = Convert.ToInt32(cmd.ExecuteScalar());
                    return next.ToString().PadLeft(9, '0');
                }
            }
            catch { return "000000001"; }
            finally { con.Cerrar(); }
        }

        private string GenerarClaveAcceso49()
        {
            string baseStr = DateTime.Now.ToString("yyyyMMddHHmmss") + usuarioId.ToString().PadLeft(6, '0') + Guid.NewGuid().ToString("N");
            string digits = new string(baseStr.Where(char.IsDigit).ToArray());
            if (digits.Length < 49) digits = digits.PadRight(49, '7');
            if (digits.Length > 49) digits = digits.Substring(0, 49);
            return digits;
        }

        // ======================= PRINT =======================
        private void MostrarVistaPrevia()
        {
            if (dtItems == null || dtItems.Rows.Count == 0)
            {
                MessageBox.Show("No hay items para imprimir.");
                return;
            }

            TomarSnapshotDesdeUI();

            preview.Width = 1000;
            preview.Height = 800;
            preview.ShowDialog();
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var fTitle = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
            var f = new System.Drawing.Font("Arial", 10);
            var fBold = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);

            int left = 50;
            int y = 50;

            e.Graphics.DrawString("TALLER MECÁNICO - FACTURA", fTitle, System.Drawing.Brushes.Black, left, y);
            y += 35;

            e.Graphics.DrawString($"Secuencial: {(string.IsNullOrWhiteSpace(secuencialGenerado) ? "(no generado)" : secuencialGenerado)}", fBold, System.Drawing.Brushes.Black, left, y);
            y += 18;

            e.Graphics.DrawString($"Fecha: {DateTime.Now:yyyy-MM-dd HH:mm}", f, System.Drawing.Brushes.Black, left, y);
            y += 18;

            e.Graphics.DrawString($"OT: {(ordenTrabajoId == null ? "N/A (venta directa)" : ordenTrabajoId.ToString())}", f, System.Drawing.Brushes.Black, left, y);
            y += 18;

            e.Graphics.DrawString($"Receptor: {snapNombre}", fBold, System.Drawing.Brushes.Black, left, y); y += 18;
            e.Graphics.DrawString($"Documento: {snapTipoDoc} {snapNumDoc}", f, System.Drawing.Brushes.Black, left, y); y += 18;

            y += 10;
            e.Graphics.DrawLine(System.Drawing.Pens.Black, left, y, left + e.MarginBounds.Width, y);
            y += 12;

            int colDesc = left;
            int colCant = left + 320;
            int colPU = left + 390;
            int colSub = left + 480;

            e.Graphics.DrawString("Descripción", fBold, System.Drawing.Brushes.Black, colDesc, y);
            e.Graphics.DrawString("Cant", fBold, System.Drawing.Brushes.Black, colCant, y);
            e.Graphics.DrawString("P.Unit", fBold, System.Drawing.Brushes.Black, colPU, y);
            e.Graphics.DrawString("Subtotal", fBold, System.Drawing.Brushes.Black, colSub, y);
            y += 18;

            e.Graphics.DrawLine(System.Drawing.Pens.Black, left, y, left + e.MarginBounds.Width, y);
            y += 8;

            foreach (DataRow r in dtItems.Rows)
            {
                string desc = r["descripcion"].ToString();
                string cant = Convert.ToDecimal(r["cantidad"]).ToString("0.##");
                string pu = Convert.ToDecimal(r["precio_unitario"]).ToString("0.00");
                string sub = Convert.ToDecimal(r["subtotal"]).ToString("0.00");

                e.Graphics.DrawString(desc, f, System.Drawing.Brushes.Black, colDesc, y);
                e.Graphics.DrawString(cant, f, System.Drawing.Brushes.Black, colCant, y);
                e.Graphics.DrawString(pu, f, System.Drawing.Brushes.Black, colPU, y);
                e.Graphics.DrawString(sub, f, System.Drawing.Brushes.Black, colSub, y);

                y += 18;
                if (y > e.MarginBounds.Bottom - 120)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            y += 10;
            e.Graphics.DrawLine(System.Drawing.Pens.Black, left, y, left + e.MarginBounds.Width, y);
            y += 12;

            decimal subtotal = dtItems.AsEnumerable().Sum(rr => Convert.ToDecimal(rr["subtotal"]));
            decimal pct = GetImpuestoPorcentajeActual() / 100m;
            decimal iva = Math.Round(subtotal * pct, 2);
            decimal total = Math.Round(subtotal + iva, 2);

            e.Graphics.DrawString($"Subtotal: {subtotal:0.00}", fBold, System.Drawing.Brushes.Black, colSub, y); y += 18;
            e.Graphics.DrawString($"IVA ({GetImpuestoPorcentajeActual():0.##}%): {iva:0.00}", fBold, System.Drawing.Brushes.Black, colSub, y); y += 18;
            e.Graphics.DrawString($"TOTAL: {total:0.00}", new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold), System.Drawing.Brushes.Black, colSub, y);

            e.HasMorePages = false;
        }
    }
}