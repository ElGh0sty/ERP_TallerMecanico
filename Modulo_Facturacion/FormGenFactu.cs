using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROYECTOMECANICO.Modulo_Clientes;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

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

        // Timer búsqueda OT (Forms)
        private readonly System.Windows.Forms.Timer _tmBuscarOT = new System.Windows.Forms.Timer();
        private CancellationTokenSource _otCts;

        // Print
        private readonly PrintDocument printDoc = new PrintDocument();
        private readonly PrintPreviewDialog preview = new PrintPreviewDialog();

        // Constructor diseñador
        public FormGenFactu()
        {
            InitializeComponent();
        }

        // Constructor real
        public FormGenFactu(long usuarioId)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;
            InitCommon(usuarioId);
        }

        private void InitCommon(long usuarioId)
        {
            // Grid + tabla items
            PrepararTablaItems();
            ConfigurarGrid();
            EstilizarDgvItems();
            dgvItems.DataSource = dtItems;

            // Impuestos
            CargarImpuestos();

            // Print
            

            // Defaults
            rbDesdeOT.Checked = true;
            rbClienteExistente.Checked = true;

            //  OT: txtBuscarOT NO se habilita hasta que el modo OT esté activo
            // (como rbDesdeOT es default true, inicia habilitado; si quieres que inicie apagado,
            // cambia rbDesdeOT.Checked = false y rbVentaDirecta.Checked = true arriba).
            CambiarModo(rbDesdeOT.Checked);

            // Eventos modo
            rbDesdeOT.CheckedChanged += (s, e) =>
            {
                if (rbDesdeOT.Checked) CambiarModo(true);
            };

            rbVentaDirecta.CheckedChanged += (s, e) =>
            {
                if (rbVentaDirecta.Checked) CambiarModo(false);
            };

            // OT búsqueda (con debounce)
            _tmBuscarOT.Interval = 350;
            _tmBuscarOT.Tick += async (s, e) =>
            {
                _tmBuscarOT.Stop();
                await BuscarOTAsync();
            };

            txtBuscarOT.TextChanged += (s, e) =>
            {
                if (!modoDesdeOT) return;
                _tmBuscarOT.Stop();
                _tmBuscarOT.Start();
            };

            lstOTResultados.SelectedIndexChanged += (s, e) => SeleccionarOT();
            lstOTResultados.Click += (s, e) => SeleccionarOT();

            btnCargarItemsOT.Click += (s, e) => CargarItemsOT();

            // Receptor
            rbConsumidorFinal.CheckedChanged += (s, e) =>
            {
                if (rbConsumidorFinal.Checked)
                {
                    SetConsumidorFinal();
                }
            };

            rbClienteExistente.CheckedChanged += (s, e) =>
            {
                if (rbClienteExistente.Checked)
                {
                    if (snapTipoDoc == "CF") LimpiarSnapshotReceptor();
                    ActualizarUIReceptor();
                }
            };

            rbNuevoCliente.CheckedChanged += (s, e) =>
            {
                if (rbNuevoCliente.Checked)
                {
                    if (snapTipoDoc == "CF") LimpiarSnapshotReceptor();
                    ActualizarUIReceptor();
                }
            };

            txtBuscarCliente.TextChanged += (s, e) => BuscarCliente();
            lstClientes.Click += (s, e) => SeleccionarCliente();
            btnNuevoCliente.Click += (s, e) => CrearClientePopup();

            // Items manuales
            btnAddItem.Click += (s, e) => AgregarItemDesdeCatalogo();
            btnDelItem.Click += (s, e) => EliminarItemSeleccionado();

            //  Mantener botones correctos según selección
            dgvItems.SelectionChanged += (s, e) => ActualizarBotonesItems();
            dgvItems.RowsAdded += (s, e) => ActualizarBotonesItems();
            dgvItems.RowsRemoved += (s, e) => ActualizarBotonesItems();

            // Totales
            cmbImpuesto.SelectedIndexChanged += (s, e) => RecalcularTotales();
            dgvItems.CellEndEdit += (s, e) => { RecalcularSubtotalFila(e.RowIndex); RecalcularTotales(); };

            // Acciones (sin Task.Run que toque UI)
            btnGenerarFactura.Click += async (s, e) =>
            {
                btnGenerarFactura.Enabled = false;
                Cursor = Cursors.WaitCursor;
                try
                {
                    await GenerarFacturaAsync();
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btnGenerarFactura.Enabled = true;
                }
            };

            btnVistaPrevia.Click += (s, e) => VerPdfFactura();

            // Estado inicial UI
            ActualizarUIReceptor();
            RecalcularTotales();
            ActualizarBotonesItems();
        }

        //  MODO
        private void CambiarModo(bool desdeOT)
        {
            modoDesdeOT = desdeOT;
            ordenTrabajoId = null;

            // txtBuscarOT solo activo si modo OT está activo
            txtBuscarOT.Enabled = desdeOT;
            btnCargarItemsOT.Enabled = desdeOT;
            lstOTResultados.Visible = false;
            lstOTResultados.DataSource = null;

            // Items manuales solo en venta directa
            dtItems.Clear();
            RecalcularTotales();

            ActualizarBotonesItems();
        }

        private void ActualizarBotonesItems()
        {
            bool enVentaDirecta = !modoDesdeOT;

            btnAddItem.Enabled = enVentaDirecta;
            btnDelItem.Enabled = enVentaDirecta && dgvItems.CurrentRow != null && dgvItems.Rows.Count > 0;
        }

        
        //ITEMS
        private void PrepararTablaItems()
        {
            dtItems = new DataTable();

            dtItems.Columns.Add("tipo_item", typeof(string));
            dtItems.Columns.Add("nombre_item", typeof(string));
            dtItems.Columns.Add("cantidad", typeof(decimal));
            dtItems.Columns.Add("precio_unitario", typeof(decimal));
            dtItems.Columns.Add("subtotal", typeof(decimal));
            dtItems.Columns.Add("producto_id", typeof(long));
            dtItems.Columns.Add("servicio_id", typeof(long));
        }

        private void ConfigurarGrid()
        {
            dgvItems.Columns.Clear();
            dgvItems.AutoGenerateColumns = false;

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipo", DataPropertyName = "tipo_item" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nombre", DataPropertyName = "nombre_item" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cant", DataPropertyName = "cantidad" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "P.Unit", DataPropertyName = "precio_unitario" });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = "subtotal", ReadOnly = true });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "producto_id", DataPropertyName = "producto_id", Visible = false });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "servicio_id", DataPropertyName = "servicio_id", Visible = false });
        }

        private void EstilizarDgvItems()
        {
            

            dgvItems.ColumnHeadersVisible = true;
            dgvItems.RowHeadersVisible = false;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AllowUserToResizeRows = false;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;

            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            dgvItems.ScrollBars = ScrollBars.Both;

            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvItems.ColumnHeadersHeight = 34;

            dgvItems.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvItems.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;

            // estilos
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgvItems.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            dgvItems.GridColor = System.Drawing.Color.Gainsboro;
            dgvItems.BorderStyle = BorderStyle.FixedSingle;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgvItems.RowTemplate.Height = 28;

            // Formatos numéricos
            if (dgvItems.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvItems.Columns)
                {
                    if (col.DataPropertyName == "cantidad")
                        col.DefaultCellStyle.Format = "0.##";

                    if (col.DataPropertyName == "precio_unitario" || col.DataPropertyName == "subtotal")
                        col.DefaultCellStyle.Format = "0.00";
                }
            }
        }

        private (string nombre, string ruc, string direccion, string telefono, string email) ObtenerEmpresa()
        {
            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("SELECT TOP 1 nombre,ruc,direccion,telefono,email FROM Empresa WHERE id=1;", cn))
            using (var rd = cmd.ExecuteReader())
            {
                if (!rd.Read()) return ("EMPRESA NO CONFIGURADA", "", "", "", "");

                return (
                    rd["nombre"]?.ToString() ?? "",
                    rd["ruc"]?.ToString() ?? "",
                    rd["direccion"]?.ToString() ?? "",
                    rd["telefono"]?.ToString() ?? "",
                    rd["email"]?.ToString() ?? ""
                );
            }
        }
        private void AgregarItemManual()
        {
            if (modoDesdeOT) return;
            dtItems.Rows.Add("Item", "Servicio / Producto", 1m, 0m, 0m, DBNull.Value, DBNull.Value);
            RecalcularTotales();
            ActualizarBotonesItems();
        }

        private void EliminarItemSeleccionado()
        {
            if (modoDesdeOT) return;

            if (dgvItems.CurrentRow != null && dgvItems.CurrentRow.Index >= 0)
            {
                dgvItems.Rows.RemoveAt(dgvItems.CurrentRow.Index);
                RecalcularTotales();
                ActualizarBotonesItems();
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
            catch { }
        }

        private void AgregarItemDesdeCatalogo()
        {
            if (modoDesdeOT)
            {
                MessageBox.Show("En modo 'Desde OT' los ítems se cargan desde la orden de trabajo.");
                return;
            }

            using (var pop = new PROYECTOMECANICO.Modulo_Facturacion.FormItemPicker(con))
            {
                if (pop.ShowDialog() != DialogResult.OK) return;
                if (pop.Result == null) return;

                var r = pop.Result;
                decimal sub = Math.Round(r.Cantidad * r.PrecioUnitario, 4);

                dtItems.Rows.Add(
                    r.TipoItem,
                    r.NombreItem,
                    r.Cantidad,
                    r.PrecioUnitario,
                    sub,
                    (object)r.ProductoId ?? DBNull.Value,
                    (object)r.ServicioId ?? DBNull.Value
                );

                RecalcularTotales();
            }
        }

        //  IMPUESTO / TOTALES

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
            var drv = cmbImpuesto.SelectedItem as DataRowView;
            if (drv != null)
            {
                decimal p;
                if (decimal.TryParse(drv["porcentaje"].ToString(), out p))
                    return p;
            }
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

        
        // OT
        
        private class OTItem
        {
            public long Id { get; set; }
            public string Cliente { get; set; }
            public string Placa { get; set; }
            public override string ToString() { return $"OT #{Id} | {Placa} | {Cliente}"; }
        }

        private async Task BuscarOTAsync()
        {
            if (!modoDesdeOT) return;

            string q = (txtBuscarOT.Text ?? "").Trim();
            if (q.Length < 1)
            {
                lstOTResultados.Visible = false;
                lstOTResultados.DataSource = null;
                return;
            }

            // Cancelar búsqueda anterior
            if (_otCts != null) _otCts.Cancel();
            _otCts = new CancellationTokenSource();
            var token = _otCts.Token;

            try
            {
                var list = new List<OTItem>();

                using (var cn = con.CrearConexion())
                using (var cmd = new SqlCommand(@"
SELECT TOP 15 
    OT.id,
    C.nombre AS cliente,
    V.placa
FROM OrdenesTrabajo OT
JOIN Vehiculos V ON V.id = OT.vehiculo_id
JOIN Clientes C ON C.id = V.cliente_id
WHERE (CAST(OT.id AS NVARCHAR(20)) LIKE @q OR V.placa LIKE @q OR C.nombre LIKE @q)
  AND ISNULL(OT.facturada,0) = 0
  AND OT.estado IN ('Terminado','Entregado')
  AND NOT EXISTS (SELECT 1 FROM dbo.Facturas F WHERE F.orden_trabajo_id = OT.id)
ORDER BY OT.id DESC", cn))
                {
                    cmd.CommandTimeout = 8;
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = "%" + q + "%";

                    await cn.OpenAsync(token);
                    using (var dr = await cmd.ExecuteReaderAsync(token))
                    {
                        while (await dr.ReadAsync(token))
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

                if (token.IsCancellationRequested) return;

                // hilo UI (porque el Tick es de WinForms),
                
                if (IsHandleCreated)
                {
                    BeginInvoke((Action)(() =>
                    {
                        lstOTResultados.DataSource = list;
                        lstOTResultados.Visible = list.Count > 0;
                    }));
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show("Error buscando OT:\n" + ex.Message);
            }
        }

        private void SeleccionarOT()
        {
            var item = lstOTResultados.SelectedItem as OTItem;
            if (item == null) return;

            ordenTrabajoId = item.Id;
            lstOTResultados.Visible = false;

            CargarReceptorDesdeOT(item.Id);
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
    CASE WHEN i.producto_id IS NOT NULL THEN 'Producto' ELSE 'Servicio' END AS tipo_item,
    COALESCE(p.nombre, NULLIF(LTRIM(RTRIM(i.descripcion)), ''), NULLIF(LTRIM(RTRIM(ot.descripcion)), ''), 'SERVICIO') AS nombre_item,
    CAST(i.cantidad AS DECIMAL(18,2)) AS cantidad,
    CAST(i.precio_unitario AS DECIMAL(18,2)) AS precio_unitario,
    CAST(i.subtotal AS DECIMAL(18,2)) AS subtotal,
    i.producto_id,
    CAST(NULL AS BIGINT) AS servicio_id
FROM OrdenesTrabajo_Items i
INNER JOIN OrdenesTrabajo ot ON ot.id = i.orden_id
LEFT JOIN Productos p ON p.id = i.producto_id
WHERE i.orden_id = @orden
ORDER BY i.id ASC";

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

        
        //RECEPTOR (con limpieza CF)
        
        private void ActualizarUIReceptor()
        {
            bool existente = rbClienteExistente.Checked;
            bool nuevo = rbNuevoCliente.Checked;
            bool cf = rbConsumidorFinal.Checked;

            txtBuscarCliente.Enabled = existente;
            lstClientes.Visible = false;

            btnNuevoCliente.Enabled = nuevo;

            bool enableSnap = !cf;
            txtTipoDoc.Enabled = enableSnap;
            txtNumDoc.Enabled = enableSnap;
            txtNombre.Enabled = enableSnap;
            txtDireccion.Enabled = enableSnap;
            txtTelefono.Enabled = enableSnap;
            txtEmail.Enabled = enableSnap;

            if (cf) SetConsumidorFinal();
        }

        private void LimpiarSnapshotReceptor()
        {
            clienteId = null;
            snapTipoDoc = "";
            snapNumDoc = "";
            snapNombre = "";
            snapDireccion = "";
            snapTelefono = "";
            snapEmail = "";
            snapContribuyenteEspecial = false;

            ReflejarSnapshotEnUI();
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

        // CLIENTE EXISTENTE
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
            public override string ToString() { return $"{Nombre} ({TipoDoc} {NumDoc})"; }
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
            var c = lstClientes.SelectedItem as ClienteItem;
            if (c == null) return;

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

        private void CrearClientePopup()
        {
            using (var pop = new FormClientePopup())
            {
                if (pop.ShowDialog() == DialogResult.OK)
                {
                    if (pop.ClienteIdCreado != null)
                    {
                        CargarClientePorId(pop.ClienteIdCreado.Value);
                        rbClienteExistente.Checked = true;
                        return;
                    }

                    MessageBox.Show("Cliente guardado. Búscalo por nombre o documento para seleccionarlo.");
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

        //  FACTURA (sin con.leer dentro de transacción)
        private bool ExisteFacturaParaOT(long otId)
        {
            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(
                "SELECT COUNT(1) FROM dbo.Facturas WHERE orden_trabajo_id = @ot", cn))
            {
                cmd.CommandTimeout = 5;
                cmd.Parameters.Add("@ot", SqlDbType.BigInt).Value = otId;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private async Task GenerarFacturaAsync()
        {
            // Esta validación usa UI (debe ser antes del await)
            if (modoDesdeOT && ordenTrabajoId.HasValue)
            {
                if (ExisteFacturaParaOT(ordenTrabajoId.Value))
                {
                    MessageBox.Show("Esta OT ya tiene una factura generada.");
                    return;
                }
            }

            if (usuarioId == 0)
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
                //  Todo con una conexión propia (cn) y esa misma en todos los comandos
                using (var cn = con.CrearConexionAbierta())
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
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
                        using (var cmd = new SqlCommand(sqlF, cn, tx))
                        {
                            cmd.CommandTimeout = 15;

                            cmd.Parameters.Add("@ot", SqlDbType.BigInt).Value = (object)ordenTrabajoId ?? DBNull.Value;
                            cmd.Parameters.Add("@user", SqlDbType.BigInt).Value = usuarioId;
                            cmd.Parameters.Add("@clave", SqlDbType.Char, 49).Value = claveAccesoGenerada;
                            cmd.Parameters.Add("@punto", SqlDbType.Char, 3).Value = "001";
                            cmd.Parameters.Add("@est", SqlDbType.Char, 3).Value = "001";
                            cmd.Parameters.Add("@sec", SqlDbType.Char, 9).Value = secuencialGenerado;

                            cmd.Parameters.Add("@sub15", SqlDbType.Decimal).Value = subtotal;
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
                            int cantInt = Math.Max(1, (int)Math.Round(cantDec, 0));
                            decimal punit = Convert.ToDecimal(r["precio_unitario"]);
                            decimal subItem = Convert.ToDecimal(r["subtotal"]);
                            decimal ivaItem = Math.Round(subItem * pct, 4);

                            using (var cmd = new SqlCommand(sqlI, cn, tx))
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

                        if (modoDesdeOT && ordenTrabajoId.HasValue)
                        {
                            
                            using (var cmd = new SqlCommand(
                                "UPDATE dbo.OrdenesTrabajo SET facturada = 1 WHERE id = @id", cn, tx))
                            {
                                cmd.CommandTimeout = 10;
                                cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = ordenTrabajoId.Value;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                        facturaIdGenerada = facturaId;

                        // Generar/guardar PDF FUERA de la transacción
                        string pdfPath = GenerarPdfFactura();
                        GuardarPdfEnFactura(facturaId, pdfPath);

                        MessageBox.Show("Factura generada\nID: " + facturaId + "\nSecuencial: " + secuencialGenerado);
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MessageBox.Show("Error generando factura:\n" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error BD:\n" + ex.Message);
            }

            await Task.CompletedTask;
        }


        private void GuardarPdfEnFactura(long facturaId, string pdfPath)
        {
            byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPath);
            string nombre = System.IO.Path.GetFileName(pdfPath);

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(@"
UPDATE Facturas
SET pdf_data = @pdf,
    pdf_nombre = @nom
WHERE id = @id;", cn))
            {
                cmd.Parameters.Add("@pdf", SqlDbType.VarBinary, -1).Value = pdfBytes;
                cmd.Parameters.AddWithValue("@nom", nombre);
                cmd.Parameters.AddWithValue("@id", facturaId);
                cmd.ExecuteNonQuery();
            }
        }

        private string GenerarSiguienteSecuencial()
        {
            try
            {
                con.Abrir();
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(secuencial AS INT)), 0) + 1 FROM dbo.Facturas", con.leer))
                {
                    cmd.CommandTimeout = 5;
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

       
        // PRINT 
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

        private void VerPdfFactura()
        {
            try
            {
                string pdfPath = GenerarPdfFactura();

                using (var v = new PROYECTOMECANICO.FormPdfViewer(
                    pdfPath,
                    title: "Vista previa - Factura",
                    defaultSaveName: Path.GetFileName(pdfPath)))
                {
                    v.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar/ver el PDF:\n" + ex.Message, "PDF",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerarPdfFactura()
        {
            if (dtItems == null || dtItems.Rows.Count == 0)
                throw new Exception("No hay items para generar el PDF.");

            TomarSnapshotDesdeUI();

            var emp = ObtenerEmpresa();

            
            string secTmp = string.IsNullOrWhiteSpace(secuencialGenerado) ? "000000001" : secuencialGenerado;
            string numeroFactura = $"001-001-{secTmp}";

            
            decimal subtotal = dtItems.AsEnumerable().Sum(r => Convert.ToDecimal(r["subtotal"]));
            decimal pct = GetImpuestoPorcentajeActual() / 100m;
            decimal iva = Math.Round(subtotal * pct, 2);
            decimal total = Math.Round(subtotal + iva, 2);

            string carpeta = Path.Combine(Path.GetTempPath(), "TallerMecanicoERP");
            Directory.CreateDirectory(carpeta);

            string fileName = $"Factura_{numeroFactura.Replace("-", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string pdfPath = Path.Combine(carpeta, fileName);

            using (var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                var fontSub = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var font = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                //  Encabezado empresa + factura 
                PdfPTable head = new PdfPTable(2);
                head.WidthPercentage = 100;
                head.SetWidths(new float[] { 65, 35 });

                var cellEmp = new PdfPCell();
                cellEmp.Border = Rectangle.BOX;
                cellEmp.Padding = 8;
                cellEmp.AddElement(new Paragraph(emp.nombre, fontSub));
                if (!string.IsNullOrWhiteSpace(emp.ruc)) cellEmp.AddElement(new Paragraph($"RUC: {emp.ruc}", font));
                if (!string.IsNullOrWhiteSpace(emp.direccion)) cellEmp.AddElement(new Paragraph($"Dirección: {emp.direccion}", font));
                if (!string.IsNullOrWhiteSpace(emp.telefono)) cellEmp.AddElement(new Paragraph($"Tel: {emp.telefono}", font));
                if (!string.IsNullOrWhiteSpace(emp.email)) cellEmp.AddElement(new Paragraph($"Email: {emp.email}", font));
                head.AddCell(cellEmp);

                var cellFac = new PdfPCell();
                cellFac.Border = Rectangle.BOX;
                cellFac.Padding = 8;
                cellFac.AddElement(new Paragraph("FACTURA", fontTitle));
                cellFac.AddElement(new Paragraph($"No.: {numeroFactura}", fontSub));
                cellFac.AddElement(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", font));
                cellFac.AddElement(new Paragraph($"OT: {(ordenTrabajoId == null ? "N/A" : ordenTrabajoId.ToString())}", font));
                if (!string.IsNullOrWhiteSpace(claveAccesoGenerada))
                    cellFac.AddElement(new Paragraph($"Clave Acceso: {claveAccesoGenerada}", font));
                head.AddCell(cellFac);

                doc.Add(head);
                doc.Add(new Paragraph(" "));

                //  Receptor 
                PdfPTable rec = new PdfPTable(2);
                rec.WidthPercentage = 100;
                rec.SetWidths(new float[] { 50, 50 });

                PdfPCell r1 = new PdfPCell(new Phrase($"Cliente: {snapNombre}", font));
                r1.Padding = 6;
                r1.Border = Rectangle.BOX;
                rec.AddCell(r1);

                PdfPCell r2 = new PdfPCell(new Phrase($"Identificación: {snapTipoDoc} {snapNumDoc}", font));
                r2.Padding = 6;
                r2.Border = Rectangle.BOX;
                rec.AddCell(r2);

                PdfPCell r3 = new PdfPCell(new Phrase($"Dirección: {(string.IsNullOrWhiteSpace(snapDireccion) ? "-" : snapDireccion)}", font));
                r3.Padding = 6;
                r3.Border = Rectangle.BOX;
                rec.AddCell(r3);

                PdfPCell r4 = new PdfPCell(new Phrase($"Email: {(string.IsNullOrWhiteSpace(snapEmail) ? "-" : snapEmail)}", font));
                r4.Padding = 6;
                r4.Border = Rectangle.BOX;
                rec.AddCell(r4);

                doc.Add(rec);
                doc.Add(new Paragraph(" "));

                //  Items 
                PdfPTable items = new PdfPTable(4);
                items.WidthPercentage = 100;
                items.SetWidths(new float[] { 10, 60, 15, 15 });

                void AddHeader(string t)
                {
                    var c = new PdfPCell(new Phrase(t, fontSub));
                    c.Padding = 6;
                    c.BackgroundColor = BaseColor.LIGHT_GRAY;
                    items.AddCell(c);
                }

                AddHeader("Cant");
                AddHeader("Descripción");
                AddHeader("P.Unit");
                AddHeader("Subtotal");

                foreach (DataRow row in dtItems.Rows)
                {
                    decimal cant = Convert.ToDecimal(row["cantidad"]);
                    string desc = row["nombre_item"]?.ToString() ?? "";
                    decimal pu = Convert.ToDecimal(row["precio_unitario"]);
                    decimal sub = Convert.ToDecimal(row["subtotal"]);

                    items.AddCell(new PdfPCell(new Phrase(cant.ToString("0.##"), font)) { Padding = 5 });
                    items.AddCell(new PdfPCell(new Phrase(desc, font)) { Padding = 5 });
                    items.AddCell(new PdfPCell(new Phrase(pu.ToString("0.00"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    items.AddCell(new PdfPCell(new Phrase(sub.ToString("0.00"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(items);
                doc.Add(new Paragraph(" "));

                //  Totales 
                PdfPTable tot = new PdfPTable(2);
                tot.HorizontalAlignment = Element.ALIGN_RIGHT;
                tot.WidthPercentage = 40;
                tot.SetWidths(new float[] { 60, 40 });

                void AddTotalRow(string k, string v, bool bold = false)
                {
                    var fk = bold ? fontSub : font;
                    var fv = bold ? fontSub : font;

                    tot.AddCell(new PdfPCell(new Phrase(k, fk)) { Padding = 6, Border = Rectangle.BOX });
                    tot.AddCell(new PdfPCell(new Phrase(v, fv)) { Padding = 6, Border = Rectangle.BOX, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                AddTotalRow("Subtotal", subtotal.ToString("0.00"));
                AddTotalRow($"IVA ({GetImpuestoPorcentajeActual():0.##}%)", iva.ToString("0.00"));
                AddTotalRow("TOTAL", total.ToString("0.00"), bold: true);

                doc.Add(tot);

                doc.Close();
                writer.Close();
            }

            return pdfPath;
        }

        
        
    }
}