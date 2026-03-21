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

using GdiPen = System.Drawing.Pen;
using GdiFont = System.Drawing.Font;
using GdiBrush = System.Drawing.SolidBrush;
using GdiGraphics = System.Drawing.Graphics;
using GdiImage = System.Drawing.Image;
using GdiBitmap = System.Drawing.Bitmap;
using GdiColor = System.Drawing.Color;
using GdiPointF = System.Drawing.PointF;

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormGenFactu : Form
    {
        private readonly Conexion con = new Conexion();
        private long usuarioId;
        private ErrorProvider errorProvider;

        private bool modoDesdeOT = true;
        private long? ordenTrabajoId = null;

        private long? clienteId = null;
        private string snapTipoDoc = "";
        private string snapNumDoc = "";
        private string snapNombre = "";
        private string snapDireccion = "";
        private string snapTelefono = "";
        private string snapEmail = "";
        private bool snapContribuyenteEspecial = false;

        private DataTable dtItems = new DataTable();

        private long facturaIdGenerada = 0;
        private string secuencialGenerado = "";
        private string claveAccesoGenerada = "";

        private readonly System.Windows.Forms.Timer _tmBuscarOT = new System.Windows.Forms.Timer();
        private CancellationTokenSource _otCts;

        private readonly PrintDocument printDoc = new PrintDocument();
        private readonly PrintPreviewDialog preview = new PrintPreviewDialog();

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
            DataGridViewEstilo.AplicarEstiloDashboard(dgvItems);


        }

        private void InitCommon(long usuarioId)
        {
            // Inicializar ErrorProvider
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // Configurar ComboBox como solo selección
            cmbImpuesto.DropDownStyle = ComboBoxStyle.DropDownList;

            // Configurar campos de cliente como solo lectura (no editables manualmente)
            ConfigurarCamposClienteSoloLectura();

            // Grid + tabla items
            PrepararTablaItems();
            ConfigurarGrid();
            EstilizarDgvItems();
            dgvItems.DataSource = dtItems;

            // Impuestos
            CargarImpuestos();

            // Defaults
            rbDesdeOT.Checked = true;
            rbClienteExistente.Checked = true;

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
                ActualizarEstadoCamposCliente();
            };

            rbClienteExistente.CheckedChanged += (s, e) =>
            {
                if (rbClienteExistente.Checked)
                {
                    if (snapTipoDoc == "CF") LimpiarSnapshotReceptor();
                    ActualizarUIReceptor();
                }
                ActualizarEstadoCamposCliente();
            };

            rbNuevoCliente.CheckedChanged += (s, e) =>
            {
                if (rbNuevoCliente.Checked)
                {
                    if (snapTipoDoc == "CF") LimpiarSnapshotReceptor();
                    ActualizarUIReceptor();
                }
                ActualizarEstadoCamposCliente();
            };

            txtBuscarCliente.TextChanged += (s, e) => BuscarCliente();
            lstClientes.Click += (s, e) => SeleccionarCliente();
            btnNuevoCliente.Click += (s, e) => CrearClientePopup();

            // Items manuales
            btnDelItem.Click += (s, e) => EliminarItemSeleccionado();

            // Mantener botones correctos según selección
            dgvItems.SelectionChanged += (s, e) => ActualizarBotonesItems();
            dgvItems.RowsAdded += (s, e) => ActualizarBotonesItems();
            dgvItems.RowsRemoved += (s, e) => ActualizarBotonesItems();

            // Totales
            cmbImpuesto.SelectedIndexChanged += (s, e) => RecalcularTotales();
            dgvItems.CellEndEdit += (s, e) => { RecalcularSubtotalFila(e.RowIndex); RecalcularTotales(); };

            // Validaciones en tiempo real
            cmbImpuesto.SelectedIndexChanged += (s, e) => ValidarImpuesto();
            dgvItems.RowsAdded += (s, e) => ValidarItems();
            dgvItems.RowsRemoved += (s, e) => ValidarItems();

            // Acciones
            btnGenerarFactura.Click += async (s, e) =>
            {
                if (!ValidarTodo())
                {
                    MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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

        // ===== CONFIGURACIÓN DE CAMPOS SOLO LECTURA =====

        private void ConfigurarCamposClienteSoloLectura()
        {
            // Estos campos son solo de visualización, no se pueden editar manualmente
            txtTipoDoc.ReadOnly = true;
            txtNumDoc.ReadOnly = true;
            txtNombre.ReadOnly = true;
            txtDireccion.ReadOnly = true;
            txtTelefono.ReadOnly = true;
            txtEmail.ReadOnly = true;

            // Cambiar color de fondo para indicar que son solo lectura
            txtTipoDoc.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            txtNumDoc.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            txtNombre.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            txtDireccion.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            txtTelefono.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            txtEmail.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
        }

        private void ActualizarEstadoCamposCliente()
        {
            bool esClienteExistente = rbClienteExistente.Checked && clienteId.HasValue;
            bool esNuevoCliente = rbNuevoCliente.Checked;
            bool esConsumidorFinal = rbConsumidorFinal.Checked;

            // En modo Consumidor Final, los campos se llenan automáticamente y son solo lectura
            // En modo Cliente Existente, se llenan al seleccionar y son solo lectura
            // En modo Nuevo Cliente, deberían ser editables (pero por ahora los mantenemos solo lectura)

            if (esNuevoCliente)
            {
                // Si algún día implementas creación de cliente desde aquí, podrías habilitarlos
                txtTipoDoc.ReadOnly = true;
                txtNumDoc.ReadOnly = true;
                txtNombre.ReadOnly = true;
                txtDireccion.ReadOnly = true;
                txtTelefono.ReadOnly = true;
                txtEmail.ReadOnly = true;
            }
        }

        // ===== MÉTODOS DE VALIDACIÓN =====

        private void MarcarError(Control control, string mensaje)
        {
            control.BackColor = System.Drawing.Color.FromArgb(255, 220, 220);
            errorProvider.SetError(control, mensaje);
        }

        private void MarcarOk(Control control)
        {
            control.BackColor = System.Drawing.Color.White;
            errorProvider.SetError(control, "");
        }

        private bool ValidarImpuesto()
        {
            if (cmbImpuesto.SelectedIndex == -1 || cmbImpuesto.SelectedValue == null)
            {
                MarcarError(cmbImpuesto, "Seleccione un impuesto");
                return false;
            }

            MarcarOk(cmbImpuesto);
            return true;
        }

        private bool ValidarItems()
        {
            if (dtItems.Rows.Count == 0)
            {
                errorProvider.SetError(btnAddItem, "Agregue al menos un item a la factura");
                return false;
            }

            errorProvider.SetError(btnAddItem, "");
            return true;
        }

        private bool ValidarCliente()
        {
            if (rbConsumidorFinal.Checked)
                return true;

            if (string.IsNullOrWhiteSpace(snapNombre) ||
                string.IsNullOrWhiteSpace(snapTipoDoc) ||
                string.IsNullOrWhiteSpace(snapNumDoc))
            {
                MarcarError(txtNombre, "Debe seleccionar un cliente válido");
                return false;
            }

            MarcarOk(txtNombre);
            MarcarOk(txtTipoDoc);
            MarcarOk(txtNumDoc);
            return true;
        }

        private bool ValidarTodo()
        {
            bool itemsValidos = ValidarItems();
            bool impuestoValido = ValidarImpuesto();
            bool clienteValido = ValidarCliente();

            return itemsValidos && impuestoValido && clienteValido;
        }

        private void LimpiarValidaciones()
        {
            MarcarOk(cmbImpuesto);
            MarcarOk(txtNombre);
            MarcarOk(txtTipoDoc);
            MarcarOk(txtNumDoc);
            errorProvider.SetError(btnAddItem, "");
        }

        // ===== MÉTODO AUXILIAR PARA OBTENER LOGO =====

        private byte[] ObtenerLogoEmpresa()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 logo FROM Empresa WHERE id = 1", cn))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return (byte[])result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener logo: " + ex.Message);
            }
            return null;
        }

        //  MODO
        private void CambiarModo(bool desdeOT)
        {
            modoDesdeOT = desdeOT;
            ordenTrabajoId = null;

            txtBuscarOT.Enabled = desdeOT;
            btnCargarItemsOT.Enabled = desdeOT;
            lstOTResultados.Visible = false;
            lstOTResultados.DataSource = null;

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

            dgvItems.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgvItems.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);

            dgvItems.GridColor = System.Drawing.Color.Gainsboro;
            dgvItems.BorderStyle = BorderStyle.FixedSingle;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgvItems.RowTemplate.Height = 28;

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

        // IMPUESTO / TOTALES

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

                dtItems.Clear();

                string sqlProductos = @"
            SELECT 
                'Producto' AS tipo_item,
                p.nombre AS nombre_item,
                CAST(i.cantidad AS DECIMAL(18,2)) AS cantidad,
                CAST(i.precio_unitario AS DECIMAL(18,2)) AS precio_unitario,
                CAST(i.subtotal AS DECIMAL(18,2)) AS subtotal,
                i.producto_id,
                CAST(NULL AS BIGINT) AS servicio_id
            FROM OrdenesTrabajo_Items i
            INNER JOIN OrdenesTrabajo ot ON ot.id = i.orden_id
            INNER JOIN Productos p ON p.id = i.producto_id
            WHERE i.orden_id = @orden AND i.producto_id IS NOT NULL
            
            UNION ALL
            
            -- Cargar servicios de la orden (directamente desde Servicios)
            SELECT 
                'Servicio' AS tipo_item,
                s.nombre AS nombre_item,
                1 AS cantidad,
                os.precio AS precio_unitario,
                os.precio AS subtotal,
                CAST(NULL AS BIGINT) AS producto_id,
                os.servicio_id
            FROM OrdenesTrabajo_Servicios os
            INNER JOIN Servicios s ON s.id = os.servicio_id
            WHERE os.orden_id = @orden
            
            ORDER BY tipo_item DESC, nombre_item ASC";

                using (var da = new SqlDataAdapter(sqlProductos, con.leer))
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
            // No tomamos de UI porque los campos son solo lectura
            // Mantenemos los valores actuales
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

        // FACTURA
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

        
        

        private string GenerarClaveAcceso49()
        {
            string baseStr = DateTime.Now.ToString("yyyyMMddHHmmss") + usuarioId.ToString().PadLeft(6, '0') + Guid.NewGuid().ToString("N");
            string digits = new string(baseStr.Where(char.IsDigit).ToArray());
            if (digits.Length < 49) digits = digits.PadRight(49, '7');
            if (digits.Length > 49) digits = digits.Substring(0, 49);
            return digits;
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

            var emp = ObtenerEmpresa();
            byte[] logoBytes = ObtenerLogoEmpresa();

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

                // ===== ENCABEZADO CON LOGO =====
                PdfPTable head = new PdfPTable(logoBytes != null ? 3 : 2);
                head.WidthPercentage = 100;
                if (logoBytes != null)
                    head.SetWidths(new float[] { 20, 45, 35 });
                else
                    head.SetWidths(new float[] { 65, 35 });

                if (logoBytes != null)
                {
                    var cellLogo = new PdfPCell();
                    cellLogo.Border = Rectangle.BOX;
                    cellLogo.Padding = 5;
                    cellLogo.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cellLogo.HorizontalAlignment = Element.ALIGN_CENTER;

                    try
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(logoBytes);
                        logo.ScaleToFit(80, 80);
                        cellLogo.AddElement(logo);
                    }
                    catch
                    {
                        cellLogo.AddElement(new Paragraph("Logo no disponible", font));
                    }
                    head.AddCell(cellLogo);
                }

                var cellEmp = new PdfPCell();
                cellEmp.Border = Rectangle.BOX;
                cellEmp.Padding = 8;
                cellEmp.AddElement(new Paragraph(emp.nombre?.ToUpper() ?? "EMPRESA", fontSub));
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

                // Receptor 
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

                // Items
                PdfPTable items = new PdfPTable(5);
                items.WidthPercentage = 100;
                items.SetWidths(new float[] { 8, 8, 44, 15, 15 });

                void AddHeader(string t)
                {
                    var c = new PdfPCell(new Phrase(t, fontSub));
                    c.Padding = 6;
                    c.BackgroundColor = BaseColor.LIGHT_GRAY;
                    items.AddCell(c);
                }

                AddHeader("Tipo");
                AddHeader("Cant");
                AddHeader("Descripción");
                AddHeader("P.Unit");
                AddHeader("Subtotal");

                foreach (DataRow row in dtItems.Rows)
                {
                    string tipo = row["tipo_item"]?.ToString() ?? "Item";
                    decimal cant = Convert.ToDecimal(row["cantidad"]);
                    string desc = row["nombre_item"]?.ToString() ?? "";
                    decimal pu = Convert.ToDecimal(row["precio_unitario"]);
                    decimal sub = Convert.ToDecimal(row["subtotal"]);

                    items.AddCell(new PdfPCell(new Phrase(tipo, font)) { Padding = 5 });
                    items.AddCell(new PdfPCell(new Phrase(cant.ToString("0.##"), font)) { Padding = 5 });
                    items.AddCell(new PdfPCell(new Phrase(desc, font)) { Padding = 5 });
                    items.AddCell(new PdfPCell(new Phrase(pu.ToString("0.00"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    items.AddCell(new PdfPCell(new Phrase(sub.ToString("0.00"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(items);
                doc.Add(new Paragraph(" "));

                // Totales 
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

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (modoDesdeOT)
            {
                MessageBox.Show("En modo 'Desde OT' los ítems se cargan desde la orden de trabajo.");
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

                dtItems.Rows.Add(
                    "Producto",
                    nombre,
                    cantidad,
                    precio,
                    cantidad * precio,
                    productoId,
                    DBNull.Value
                );

                RecalcularTotales();
            }
        }

        private void btnBuscadorOrdenTrabajo_Click(object sender, EventArgs e)
        {
            if (!rbDesdeOT.Checked)
            {
                rbDesdeOT.Checked = true;
            }

            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.OrdenesTrabajo);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long ordenId = Convert.ToInt64(fila["id"]);
                string placa = fila["placa"].ToString();
                string cliente = fila["cliente"].ToString();
                string estado = fila["estado"].ToString();

                if (estado != "Terminado" && estado != "Entregado")
                {
                    MessageBox.Show($"La orden seleccionada está en estado '{estado}'. Solo se pueden facturar órdenes en estado 'Terminado' o 'Entregado'.",
                        "Estado no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ExisteFacturaParaOT(ordenId))
                {
                    MessageBox.Show("Esta OT ya tiene una factura generada.",
                        "OT ya facturada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ordenTrabajoId = ordenId;
                txtBuscarOT.Text = $"OT #{ordenId} - {placa} - {cliente}";
                CargarReceptorDesdeOT(ordenId);

                DialogResult cargarItems = MessageBox.Show("¿Desea cargar los items de esta orden ahora?",
                    "Cargar items", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (cargarItems == DialogResult.Yes)
                {
                    CargarItemsOT();
                }
            }
        }

        private void btnBuscadorClientes_Click(object sender, EventArgs e)
        {
            if (!rbClienteExistente.Checked)
            {
                rbClienteExistente.Checked = true;
            }

            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Clientes);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long clienteId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();

                using (var cn = con.CrearConexionAbierta())
                {
                    try
                    {
                        string sql = @"
                    SELECT id, tipo_documento, numero_documento, nombre, direccion, telefono, email, contribuyente_especial
                    FROM Clientes
                    WHERE id = @id";

                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@id", clienteId);
                            using (var dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    this.clienteId = clienteId;
                                    snapTipoDoc = dr["tipo_documento"].ToString();
                                    snapNumDoc = dr["numero_documento"].ToString();
                                    snapNombre = dr["nombre"].ToString();
                                    snapDireccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString();
                                    snapTelefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString();
                                    snapEmail = dr["email"] == DBNull.Value ? "" : dr["email"].ToString();
                                    snapContribuyenteEspecial = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"]);

                                    txtBuscarCliente.Text = snapNombre;
                                    ReflejarSnapshotEnUI();
                                    lstClientes.Visible = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error cargando datos del cliente:\n" + ex.Message);
                    }
                }
            }
        }

        // Clase para almacenar la configuración del secuencial
        // Clase para almacenar la configuración del secuencial
        private class SecuencialConfig
        {
            public string TipoDocumento { get; set; }
            public string Establecimiento { get; set; }
            public string PuntoEmision { get; set; }
            public int SecuenciaActual { get; set; }
        }

        // Método para obtener la configuración del secuencial para facturas
        private SecuencialConfig ObtenerSecuencialFactura()
        {
            SecuencialConfig config = null;

            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    // Buscar el secuencial para FACTURA
                    string sql = @"
SELECT TOP 1 
    tipo_documento, 
    establecimiento, 
    punto_emision, 
    secuencia_actual
FROM Secuenciales 
WHERE tipo_documento = 'FACTURA' 
   OR tipo_documento LIKE '%FACTURA%'
   OR UPPER(tipo_documento) = 'FACTURA'
ORDER BY id DESC";

                    using (var cmd = new SqlCommand(sql, cn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            config = new SecuencialConfig
                            {
                                TipoDocumento = rd["tipo_documento"].ToString(),
                                Establecimiento = rd["establecimiento"].ToString().PadLeft(3, '0'),
                                PuntoEmision = rd["punto_emision"].ToString().PadLeft(3, '0'),
                                SecuenciaActual = Convert.ToInt32(rd["secuencia_actual"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener secuencial: " + ex.Message);
            }

            // Si no hay configuración, usar valores por defecto
            if (config == null)
            {
                config = new SecuencialConfig
                {
                    TipoDocumento = "FACTURA",
                    Establecimiento = "001",
                    PuntoEmision = "001",
                    SecuenciaActual = 0
                };
            }

            return config;
        }

        // Método para generar el siguiente secuencial (versión mejorada)
        private string GenerarSiguienteSecuencial()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        // Bloquear la fila del secuencial para evitar concurrencia
                        string sqlSelect = @"
SELECT secuencia_actual 
FROM Secuenciales 
WHERE tipo_documento = 'FACTURA' 
   OR tipo_documento LIKE '%FACTURA%'
WITH (UPDLOCK, ROWLOCK)";

                        int secuenciaActual = 0;
                        long secuencialId = 0;

                        using (var cmd = new SqlCommand(sqlSelect, cn, tx))
                        {
                            using (var rd = cmd.ExecuteReader())
                            {
                                if (rd.Read())
                                {
                                    secuenciaActual = Convert.ToInt32(rd["secuencia_actual"]);
                                    // No podemos obtener el ID aquí porque no lo seleccionamos
                                }
                                else
                                {
                                    // No existe, crear registro por defecto
                                    rd.Close();
                                    string sqlInsert = @"
INSERT INTO Secuenciales (tipo_documento, establecimiento, punto_emision, secuencia_actual)
VALUES ('FACTURA', '001', '001', 0);
SELECT SCOPE_IDENTITY();";

                                    using (var cmdInsert = new SqlCommand(sqlInsert, cn, tx))
                                    {
                                        secuencialId = Convert.ToInt64(cmdInsert.ExecuteScalar());
                                        secuenciaActual = 0;
                                    }
                                }
                            }
                        }

                        // Si no obtuvimos el ID en el primer bloque, lo buscamos ahora
                        if (secuencialId == 0)
                        {
                            string sqlId = "SELECT id FROM Secuenciales WHERE tipo_documento = 'FACTURA'";
                            using (var cmdId = new SqlCommand(sqlId, cn, tx))
                            {
                                secuencialId = Convert.ToInt64(cmdId.ExecuteScalar());
                            }
                        }

                        // Incrementar secuencia
                        int nuevaSecuencia = secuenciaActual + 1;

                        string sqlUpdate = @"
UPDATE Secuenciales 
SET secuencia_actual = @nueva
WHERE id = @id";

                        using (var cmdUpdate = new SqlCommand(sqlUpdate, cn, tx))
                        {
                            cmdUpdate.Parameters.AddWithValue("@nueva", nuevaSecuencia);
                            cmdUpdate.Parameters.AddWithValue("@id", secuencialId);
                            cmdUpdate.ExecuteNonQuery();
                        }

                        tx.Commit();

                        return nuevaSecuencia.ToString().PadLeft(9, '0');
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
                MessageBox.Show("Error al generar secuencial: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Fallback: generar secuencial basado en MAX+1 (como antes)
                try
                {
                    using (var cn = con.CrearConexionAbierta())
                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(secuencial AS INT)), 0) + 1 FROM dbo.Facturas", cn))
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
        }

        // Método para obtener establecimiento y punto de emisión (para mostrar en la factura)
        private (string establecimiento, string puntoEmision) ObtenerEstablecimientoPunto()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    string sql = @"
SELECT TOP 1 
    establecimiento, 
    punto_emision
FROM Secuenciales 
WHERE tipo_documento = 'FACTURA' 
   OR tipo_documento LIKE '%FACTURA%'
ORDER BY id DESC";

                    using (var cmd = new SqlCommand(sql, cn))
                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            string est = rd["establecimiento"].ToString().PadLeft(3, '0');
                            string pto = rd["punto_emision"].ToString().PadLeft(3, '0');
                            return (est, pto);
                        }
                    }
                }
            }
            catch { }

            // Valores por defecto
            return ("001", "001");
        }

        private void FormGenFactu_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvItems);

        }




    }
}