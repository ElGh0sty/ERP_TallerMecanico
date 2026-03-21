using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        // Variables para descuentos
        private DataTable dtDescuentos = new DataTable();
        private long _descuentoSeleccionadoId = 0;
        private decimal _descuentoPorcentaje = 0;
        private string _descuentoTipoAplicacion = "";

        // Variables para promociones aplicadas
        private List<long> _promocionesAplicadas = new List<long>();


        private readonly Conexion con = new Conexion();
        private long usuarioId;
        private ErrorProvider errorProvider;

        // Variables comunes
        private DataTable dtItems = new DataTable();
        private long facturaIdGenerada = 0;
        private string secuencialGenerado = "";
        private string claveAccesoGenerada = "";

        // Variables para Tab OT
        private long? ordenTrabajoId = null;
        private OTItem ordenTrabajoSeleccionada = null;
        private long? clienteIdOT = null;
        private string snapTipoDocOT = "";
        private string snapNumDocOT = "";
        private string snapNombreOT = "";
        private string snapDireccionOT = "";
        private string snapTelefonoOT = "";
        private string snapEmailOT = "";
        private bool snapContribuyenteEspecialOT = false;
        private bool receptorConfirmadoOT = false;

        // Variables para Tab Venta Directa
        private long? clienteIdVD = null;
        private string clienteVDTipoDoc = "";
        private string clienteVDNumDoc = "";
        private string clienteVDNombre = "";
        private string clienteVDDireccion = "";
        private string clienteVDTelefono = "";
        private string clienteVDEmail = "";
        private bool clienteVDContribuyenteEspecial = false;

        private string _establecimientoActual = "001";
        private string _puntoEmisionActual = "001";

        // Timer para búsqueda OT
        private readonly System.Windows.Forms.Timer _tmBuscarOT = new System.Windows.Forms.Timer();
        private CancellationTokenSource _otCts;

        public FormGenFactu()
        {
            InitializeComponent();
        }

        public FormGenFactu(long usuarioId)
        {

            InitializeComponent();
            this.usuarioId = usuarioId;
            InitCommon();
            DataGridViewEstilo.AplicarEstiloDashboard(dgvItems);
        }

        private void InitCommon()
        {
            
            CargarDescuentos();
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // Configurar ComboBox
            cmbImpuesto.DropDownStyle = ComboBoxStyle.DropDownList;

            // Configurar grid de items
            PrepararTablaItems();
            ConfigurarGrid();
            EstilizarDgvItems();
            dgvItems.DataSource = dtItems;

            // Cargar impuestos
            CargarImpuestos();

            // Configurar eventos del Tab OT
            ConfigurarEventosTabOT();

            // Configurar eventos del Tab Venta Directa
            ConfigurarEventosTabVD();

            // Configurar eventos comunes
            ConfigurarEventosComunes();

            this.cmbDescuentoItem.SelectedIndexChanged += (s, e) =>
            {
                // Filtrar descuentos por tipo cuando cambie la selección
                if (cmbDescuentoItem.SelectedValue != null)
                {
                    long tipoId = Convert.ToInt64(cmbDescuentoItem.SelectedValue);
                    // Filtrar descuentos que apliquen a este tipo
                }
            };

            // Configurar evento de cambio de pestaña
            tabControlPrincipal.SelectedIndexChanged += (s, e) =>
            {
                ActualizarBotonesItemsPorTab();
                // Limpiar items al cambiar de pestaña (opcional)
                // dtItems.Clear();
                // RecalcularTotales();
            };

            tabControlPrincipal.SelectedIndexChanged += (s, e) =>
            {
                ActualizarBotonesItemsPorTab();

                // Limpiar completamente al cambiar de pestaña
                if (tabControlPrincipal.SelectedTab == tabPageOT)
                {
                    LimpiarTabOT();
                }
                else if (tabControlPrincipal.SelectedTab == tabPageVentaDirecta)
                {
                    LimpiarTabVD();
                }
            };

            // Estado inicial
            RecalcularTotales();
            ActualizarBotonesItemsPorTab();
            LimpiarTabOT();
            LimpiarTabVD();
        }


        private void CargarDescuentos()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(@"
            SELECT id, nombre, tipo_aplicacion, porcentaje 
            FROM Descuentos 
            WHERE activo = 1 
            AND (fecha_inicio IS NULL OR fecha_inicio <= GETDATE())
            AND (fecha_fin IS NULL OR fecha_fin >= GETDATE())
            ORDER BY nombre", cn))
                {
                    dtDescuentos = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dtDescuentos);
                }

                // Llenar combo de descuentos para aplicar a items
                cmbDescuentoItem.DataSource = dtDescuentos.Copy();
                cmbDescuentoItem.DisplayMember = "nombre";
                cmbDescuentoItem.ValueMember = "id";

                // Llenar combo de filtro por tipo
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cargando descuentos: " + ex.Message);
            }
        }

        private void AplicarDescuentoAItem()
        {
            if (dgvItems.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un item primero.", "Descuento",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (cmbDescuentoItem.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un descuento.", "Descuento",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long descuentoId = Convert.ToInt64(cmbDescuentoItem.SelectedValue);
            DataRow[] descuentos = dtDescuentos.Select($"id = {descuentoId}");
            if (descuentos.Length == 0) return;

            decimal porcentaje = Convert.ToDecimal(descuentos[0]["porcentaje"]);
            string tipoAplicacion = descuentos[0]["tipo_aplicacion"].ToString();

            DataRow row = dtItems.Rows[dgvItems.CurrentRow.Index];
            string tipoItem = row["tipo_item"].ToString();

            // Validar si el descuento aplica a este tipo de item
            if (tipoAplicacion != "Ambos" && tipoAplicacion != tipoItem)
            {
                MessageBox.Show($"Este descuento solo aplica a {tipoAplicacion}s.", "Descuento no válido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal precioUnitario = Convert.ToDecimal(row["precio_unitario"]);
            decimal nuevoPrecio = precioUnitario * (1 - (porcentaje / 100));

            row["precio_unitario"] = Math.Round(nuevoPrecio, 4);
            RecalcularSubtotalFila(dgvItems.CurrentRow.Index);
            RecalcularTotales();

            // Guardar el descuento aplicado
            if (!dtItems.Columns.Contains("descuento_id"))
                dtItems.Columns.Add("descuento_id", typeof(long));
            if (!dtItems.Columns.Contains("descuento_porcentaje"))
                dtItems.Columns.Add("descuento_porcentaje", typeof(decimal));

            row["descuento_id"] = descuentoId;
            row["descuento_porcentaje"] = porcentaje;

            MessageBox.Show($"Descuento del {porcentaje}% aplicado al item seleccionado.", "Descuento aplicado",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private async Task AplicarPromocionesAutomaticas()
        {
            if (tabControlPrincipal.SelectedTab != tabPageVentaDirecta) return;

            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    int diaSemana = (int)DateTime.Now.DayOfWeek;
                    if (diaSemana == 0) diaSemana = 7;
                    DateTime fechaActual = DateTime.Now.Date;

                    // Buscar promociones activas
                    string sql = @"
                SELECT p.* 
                FROM Promociones p
                WHERE p.activo = 1 
                AND p.fecha_inicio <= @fechaActual 
                AND p.fecha_fin >= @fechaActual
                AND (
                    (p.tipo = 'DiaSemana' AND p.dia_semana = @diaSemana)
                    OR (p.tipo = 'FechaEspecifica' AND p.fecha_especifica = @fechaActual)
                    OR (p.tipo = 'Combo')
                )
                AND NOT EXISTS (
                    SELECT 1 FROM PromocionesAplicadas pa 
                    WHERE pa.promocion_id = p.id 
                    AND pa.factura_id = 0 
                    AND pa.fecha_aplicacion >= @fechaActual
                )";

                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@diaSemana", diaSemana);
                        cmd.Parameters.AddWithValue("@fechaActual", fechaActual);

                        using (var dr = await cmd.ExecuteReaderAsync())
                        {
                            while (await dr.ReadAsync())
                            {
                                string tipo = dr["tipo"].ToString();
                                string nombrePromo = dr["nombre"].ToString();

                                if (tipo == "Combo")
                                {
                                    // Verificar combo
                                    long? prodPrincipal = dr["producto_principal_id"] != DBNull.Value ?
                                        Convert.ToInt64(dr["producto_principal_id"]) : (long?)null;
                                    long? servPrincipal = dr["servicio_principal_id"] != DBNull.Value ?
                                        Convert.ToInt64(dr["servicio_principal_id"]) : (long?)null;
                                    long? prodObsequio = dr["producto_obsequio_id"] != DBNull.Value ?
                                        Convert.ToInt64(dr["producto_obsequio_id"]) : (long?)null;
                                    long? servObsequio = dr["servicio_obsequio_id"] != DBNull.Value ?
                                        Convert.ToInt64(dr["servicio_obsequio_id"]) : (long?)null;
                                    int cantidadObsequio = Convert.ToInt32(dr["cantidad_obsequio"]);

                                    // Verificar si hay producto o servicio principal en los items
                                    bool tienePrincipal = false;
                                    foreach (DataRow row in dtItems.Rows)
                                    {
                                        if (prodPrincipal.HasValue && row["producto_id"] != DBNull.Value &&
                                            Convert.ToInt64(row["producto_id"]) == prodPrincipal.Value)
                                        {
                                            tienePrincipal = true;
                                            break;
                                        }
                                        if (servPrincipal.HasValue && row["servicio_id"] != DBNull.Value &&
                                            Convert.ToInt64(row["servicio_id"]) == servPrincipal.Value)
                                        {
                                            tienePrincipal = true;
                                            break;
                                        }
                                    }

                                    if (tienePrincipal)
                                    {
                                        // Agregar obsequio
                                        if (prodObsequio.HasValue)
                                        {
                                            string nombreObsequio = ObtenerNombreProducto(prodObsequio.Value);
                                            dtItems.Rows.Add("Producto", nombreObsequio, cantidadObsequio, 0, 0,
                                                prodObsequio.Value, DBNull.Value);
                                            _promocionesAplicadas.Add(Convert.ToInt32(dr["id"]));
                                            MessageBox.Show($"¡Promoción aplicada! Por comprar el producto/servicio, recibes {cantidadObsequio}x {nombreObsequio} como obsequio.",
                                                "Promoción aplicada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        else if (servObsequio.HasValue)
                                        {
                                            string nombreObsequio = ObtenerNombreServicio(servObsequio.Value);
                                            dtItems.Rows.Add("Servicio", nombreObsequio, cantidadObsequio, 0, 0,
                                                DBNull.Value, servObsequio.Value);
                                            _promocionesAplicadas.Add(Convert.ToInt32(dr["id"]));
                                            MessageBox.Show($"¡Promoción aplicada! Por comprar el producto/servicio, recibes {cantidadObsequio}x {nombreObsequio} como obsequio.",
                                                "Promoción aplicada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                        RecalcularTotales();
                                    }
                                }
                                else if (tipo == "DiaSemana" || tipo == "FechaEspecifica")
                                {
                                    // Descuento general
                                    if (dr["producto_principal_id"] != DBNull.Value)
                                    {
                                        // Descuento a producto específico
                                        long productoId = Convert.ToInt64(dr["producto_principal_id"]);
                                        decimal porcentaje = Convert.ToDecimal(dr["porcentaje"] ?? 0);

                                        foreach (DataRow row in dtItems.Rows)
                                        {
                                            if (row["producto_id"] != DBNull.Value &&
                                                Convert.ToInt64(row["producto_id"]) == productoId)
                                            {
                                                decimal precio = Convert.ToDecimal(row["precio_unitario"]);
                                                decimal nuevoPrecio = precio * (1 - (porcentaje / 100));
                                                row["precio_unitario"] = Math.Round(nuevoPrecio, 4);
                                                RecalcularSubtotalFila(dtItems.Rows.IndexOf(row));
                                            }
                                        }
                                        _promocionesAplicadas.Add(Convert.ToInt32(dr["id"]));
                                        MessageBox.Show($"¡Promoción aplicada! Descuento del {porcentaje}% en productos seleccionados.",
                                            "Promoción aplicada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                            }
                        }
                    }

                    RecalcularTotales();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error aplicando promociones: " + ex.Message);
            }
        }

        private string ObtenerNombreServicio(long servicioId)
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT nombre FROM Servicios WHERE id = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", servicioId);
                    return cmd.ExecuteScalar()?.ToString() ?? "Servicio desconocido";
                }
            }
            catch { return "Servicio desconocido"; }
        }



        private void RegistrarPromocionesAplicadas(SqlTransaction tx, long facturaId)
        {
            foreach (long promocionId in _promocionesAplicadas)
            {
                string sql = @"
INSERT INTO PromocionesAplicadas (promocion_id, factura_id, orden_trabajo_id, fecha_aplicacion)
VALUES (@promocionId, @facturaId, @ordenTrabajoId, GETDATE())";

                using (var cmd = new SqlCommand(sql, tx.Connection, tx))
                {
                    cmd.Parameters.AddWithValue("@promocionId", promocionId);
                    cmd.Parameters.AddWithValue("@facturaId", facturaId);
                    cmd.Parameters.AddWithValue("@ordenTrabajoId", (object)ordenTrabajoId ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ==================== CONFIGURACIÓN DE EVENTOS ====================

        private void ConfigurarEventosTabOT()
        {
            // Timer para búsqueda con debounce
            _tmBuscarOT.Interval = 350;
            _tmBuscarOT.Tick += async (s, e) =>
            {
                _tmBuscarOT.Stop();
                await BuscarOTAsync();
            };

            txtBuscarOT.TextChanged += (s, e) =>
            {
                _tmBuscarOT.Stop();
                _tmBuscarOT.Start();
            };

            lstOTResultados.SelectedIndexChanged += (s, e) => SeleccionarOT();
            lstOTResultados.Click += (s, e) => SeleccionarOT();

            btnCargarItemsOT.Click += (s, e) => CargarItemsOT();
            btnConfirmarReceptorOT.Click += (s, e) => ConfirmarReceptorOT();
        }

        private void ConfigurarEventosTabVD()
        {
            // Radio buttons
            rbClienteExistenteVD.CheckedChanged += (s, e) =>
            {
                if (rbClienteExistenteVD.Checked)
                {
                    txtBuscarClienteVD.Enabled = true;
                    btnBuscadorClientesVD.Enabled = true;
                    LimpiarClienteVD();
                    ReflejarClienteVDEnUI();
                }
            };

            rbNuevoClienteVD.CheckedChanged += (s, e) =>
            {
                if (rbNuevoClienteVD.Checked)
                {
                    txtBuscarClienteVD.Enabled = false;
                    btnBuscadorClientesVD.Enabled = false;
                    LimpiarClienteVD();
                }
            };

            rbConsumidorFinalVD.CheckedChanged += (s, e) =>
            {
                if (rbConsumidorFinalVD.Checked)
                {
                    txtBuscarClienteVD.Enabled = false;
                    btnBuscadorClientesVD.Enabled = false;
                    SetConsumidorFinalVD();
                }
            };

            // Búsqueda de cliente
            txtBuscarClienteVD.TextChanged += (s, e) => BuscarClienteVD();
            lstClientesVD.Click += (s, e) => SeleccionarClienteVD();
            btnBuscadorClientesVD.Click += (s, e) => BuscarClienteVD();
            btnNuevoClienteVD.Click += (s, e) => CrearClientePopupVD();
        }

        private void ConfigurarEventosComunes()
        {
            // Botones de items
            btnAddItem.Click += (s, e) => AgregarItemManual();
            btnDelItem.Click += (s, e) => EliminarItemSeleccionado();

            // Totales
            cmbImpuesto.SelectedIndexChanged += (s, e) => RecalcularTotales();
            dgvItems.CellEndEdit += (s, e) =>
            {
                RecalcularSubtotalFila(e.RowIndex);
                RecalcularTotales();
            };

            // Validaciones
            cmbImpuesto.SelectedIndexChanged += (s, e) => ValidarImpuesto();
            dgvItems.RowsAdded += (s, e) => ValidarItems();
            dgvItems.RowsRemoved += (s, e) => ValidarItems();

            // Botones de acción
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
        }

        // ==================== CONTROL DE BOTONES SEGÚN TAB ====================

        private void ActualizarBotonesItemsPorTab()
        {
            bool esVentaDirecta = tabControlPrincipal.SelectedTab == tabPageVentaDirecta;

            // En Venta Directa: botones habilitados y visibles para agregar items
            btnAddItem.Visible = esVentaDirecta;
            btnDelItem.Visible = esVentaDirecta;

            if (esVentaDirecta)
            {
                btnAddItem.Enabled = true;
                btnDelItem.Enabled = dgvItems.CurrentRow != null && dgvItems.Rows.Count > 0;
                btnAddItem.Text = "+ Agregar Item";
                
            }
            else
            {
                // En modo OT: ocultar botones de items manuales
                btnAddItem.Enabled = false;
                btnDelItem.Enabled = false;
            }
        }

        // ==================== MÉTODOS DEL TAB OT ====================

        private class OTItem
        {
            public long Id { get; set; }
            public string Cliente { get; set; }
            public string Placa { get; set; }
            public string TipoDoc { get; set; }
            public string NumDoc { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
            public bool ContribuyenteEspecial { get; set; }
            public override string ToString() { return $"OT #{Id} | {Placa} | {Cliente}"; }
        }

        private async Task BuscarOTAsync()
        {
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
    V.placa,
    C.tipo_documento,
    C.numero_documento,
    C.direccion,
    C.telefono,
    C.email,
    C.contribuyente_especial
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
                                Placa = dr["placa"].ToString(),
                                TipoDoc = dr["tipo_documento"].ToString(),
                                NumDoc = dr["numero_documento"].ToString(),
                                Direccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString(),
                                Telefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString(),
                                Email = dr["email"] == DBNull.Value ? "" : dr["email"].ToString(),
                                ContribuyenteEspecial = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"])
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

            ordenTrabajoSeleccionada = item;
            ordenTrabajoId = item.Id;
            lstOTResultados.Visible = false;
            txtBuscarOT.Text = item.ToString();

            // Cargar datos en campos editables
            txtTipoDocOT.Text = item.TipoDoc;
            txtNumDocOT.Text = item.NumDoc;
            txtNombreOT.Text = item.Cliente;
            txtDireccionOT.Text = item.Direccion;
            txtTelefonoOT.Text = item.Telefono;
            txtEmailOT.Text = item.Email;

            receptorConfirmadoOT = false;
            btnConfirmarReceptorOT.BackColor = System.Drawing.Color.FromArgb(107, 83, 255);
            btnConfirmarReceptorOT.Text = "✓ Confirmar";
        }

        private void ConfirmarReceptorOT()
        {
            // Validar campos
            if (string.IsNullOrWhiteSpace(txtNombreOT.Text))
            {
                MessageBox.Show("Debe ingresar un nombre para el receptor.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTipoDocOT.Text) || string.IsNullOrWhiteSpace(txtNumDocOT.Text))
            {
                MessageBox.Show("Debe ingresar tipo y número de documento.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guardar datos confirmados
            snapTipoDocOT = txtTipoDocOT.Text.Trim();
            snapNumDocOT = txtNumDocOT.Text.Trim();
            snapNombreOT = txtNombreOT.Text.Trim();
            snapDireccionOT = txtDireccionOT.Text.Trim();
            snapTelefonoOT = txtTelefonoOT.Text.Trim();
            snapEmailOT = txtEmailOT.Text.Trim();
            snapContribuyenteEspecialOT = false;

            // Buscar si el cliente ya existe
            clienteIdOT = BuscarClientePorDocumento(snapNumDocOT);

            receptorConfirmadoOT = true;
            btnConfirmarReceptorOT.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            btnConfirmarReceptorOT.Text = "✓ Confirmado";

            MessageBox.Show("Datos del receptor confirmados.", "Confirmación",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CargarItemsOT()
        {
            if (ordenTrabajoId == null || ordenTrabajoId <= 0)
            {
                MessageBox.Show("Seleccione una OT primero.");
                return;
            }

            if (!receptorConfirmadoOT)
            {
                MessageBox.Show("Debe confirmar los datos del receptor antes de cargar los items.",
                    "Confirmación requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
INNER JOIN Productos p ON p.id = i.producto_id
WHERE i.orden_id = @orden AND i.producto_id IS NOT NULL

UNION ALL

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
                    MessageBox.Show("Esta OT no tiene items para facturar.");
                else
                    MessageBox.Show($"Se cargaron {dtItems.Rows.Count} items de la OT.",
                        "Items cargados", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando items OT:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void LimpiarTabOT()
        {
            // Limpiar items
            dtItems.Clear();
            RecalcularTotales();

            // Limpiar datos de OT
            ordenTrabajoId = null;
            ordenTrabajoSeleccionada = null;
            receptorConfirmadoOT = false;
            txtBuscarOT.Text = "";
            lstOTResultados.DataSource = null;
            lstOTResultados.Visible = false;

            // Limpiar campos de receptor
            txtTipoDocOT.Text = "";
            txtNumDocOT.Text = "";
            txtNombreOT.Text = "";
            txtDireccionOT.Text = "";
            txtTelefonoOT.Text = "";
            txtEmailOT.Text = "";

            // Resetear datos guardados
            snapTipoDocOT = "";
            snapNumDocOT = "";
            snapNombreOT = "";
            snapDireccionOT = "";
            snapTelefonoOT = "";
            snapEmailOT = "";
            clienteIdOT = null;

            // Resetear botón confirmar
            btnConfirmarReceptorOT.BackColor = System.Drawing.Color.FromArgb(107, 83, 255);
            btnConfirmarReceptorOT.Text = "✓ Confirmar";

            // Deshabilitar botón cargar items hasta que se seleccione OT
            btnCargarItemsOT.Enabled = true; // Se habilita cuando se selecciona OT
        }

        // ==================== MÉTODOS DEL TAB VENTA DIRECTA ====================

        private class ClienteVDItem
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

        private void BuscarClienteVD()
        {
            if (!rbClienteExistenteVD.Checked) return;

            string q = (txtBuscarClienteVD.Text ?? "").Trim();
            if (q.Length < 1)
            {
                lstClientesVD.Visible = false;
                lstClientesVD.DataSource = null;
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

                var list = new List<ClienteVDItem>();
                using (var cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 100).Value = "%" + q + "%";
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ClienteVDItem
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

                lstClientesVD.DataSource = list;
                lstClientesVD.Visible = list.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error buscando cliente:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void SeleccionarClienteVD()
        {
            var c = lstClientesVD.SelectedItem as ClienteVDItem;
            if (c == null) return;

            clienteIdVD = c.Id;
            clienteVDTipoDoc = c.TipoDoc;
            clienteVDNumDoc = c.NumDoc;
            clienteVDNombre = c.Nombre;
            clienteVDDireccion = c.Direccion;
            clienteVDTelefono = c.Telefono;
            clienteVDEmail = c.Email;
            clienteVDContribuyenteEspecial = c.CE;

            ReflejarClienteVDEnUI();
            lstClientesVD.Visible = false;
        }

        private void ReflejarClienteVDEnUI()
        {
            txtTipoDocVD.Text = clienteVDTipoDoc;
            txtNumDocVD.Text = clienteVDNumDoc;
            txtNombreVD.Text = clienteVDNombre;
            txtDireccionVD.Text = clienteVDDireccion;
            txtTelefonoVD.Text = clienteVDTelefono;
            txtEmailVD.Text = clienteVDEmail;
        }

        private void CrearClientePopupVD()
        {
            using (var pop = new FormClientePopup())
            {
                if (pop.ShowDialog() == DialogResult.OK && pop.ClienteIdCreado.HasValue)
                {
                    CargarClienteVDPorId(pop.ClienteIdCreado.Value);
                    rbClienteExistenteVD.Checked = true;
                }
            }
        }

        private void CargarClienteVDPorId(long id)
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
                            clienteIdVD = Convert.ToInt64(dr["id"]);
                            clienteVDTipoDoc = dr["tipo_documento"].ToString();
                            clienteVDNumDoc = dr["numero_documento"].ToString();
                            clienteVDNombre = dr["nombre"].ToString();
                            clienteVDDireccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString();
                            clienteVDTelefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString();
                            clienteVDEmail = dr["email"] == DBNull.Value ? "" : dr["email"].ToString();
                            clienteVDContribuyenteEspecial = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"]);

                            ReflejarClienteVDEnUI();
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

        private void SetConsumidorFinalVD()
        {
            clienteIdVD = null;
            clienteVDTipoDoc = "CF";
            clienteVDNumDoc = "9999999999999";
            clienteVDNombre = "CONSUMIDOR FINAL";
            clienteVDDireccion = "";
            clienteVDTelefono = "";
            clienteVDEmail = "";
            clienteVDContribuyenteEspecial = false;

            ReflejarClienteVDEnUI();
        }

        private void LimpiarClienteVD()
        {
            clienteIdVD = null;
            clienteVDTipoDoc = "";
            clienteVDNumDoc = "";
            clienteVDNombre = "";
            clienteVDDireccion = "";
            clienteVDTelefono = "";
            clienteVDEmail = "";
            clienteVDContribuyenteEspecial = false;

            ReflejarClienteVDEnUI();
        }

        private void LimpiarTabVD()
        {
            // Limpiar items
            dtItems.Clear();
            RecalcularTotales();

            // Limpiar datos de cliente
            clienteIdVD = null;
            clienteVDTipoDoc = "";
            clienteVDNumDoc = "";
            clienteVDNombre = "";
            clienteVDDireccion = "";
            clienteVDTelefono = "";
            clienteVDEmail = "";
            clienteVDContribuyenteEspecial = false;

            // Resetear UI
            txtTipoDocVD.Text = "";
            txtNumDocVD.Text = "";
            txtNombreVD.Text = "";
            txtDireccionVD.Text = "";
            txtTelefonoVD.Text = "";
            txtEmailVD.Text = "";
            txtBuscarClienteVD.Text = "";
            lstClientesVD.DataSource = null;
            lstClientesVD.Visible = false;

            // Resetear radio buttons a estado por defecto
            rbClienteExistenteVD.Checked = true;
            rbNuevoClienteVD.Checked = false;
            rbConsumidorFinalVD.Checked = false;
        }

        // ==================== MÉTODOS COMUNES (Items, Totales, Factura) ====================

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

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tipo", DataPropertyName = "tipo_item", Width = 80 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cant", DataPropertyName = "cantidad", Width = 60 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Descripción", DataPropertyName = "nombre_item", Width = 350 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "P.Unit", DataPropertyName = "precio_unitario", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Subtotal", DataPropertyName = "subtotal", Width = 100, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

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
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvItems.RowTemplate.Height = 30;
            dgvItems.ColumnHeadersHeight = 35;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgvItems.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            dgvItems.GridColor = System.Drawing.Color.Gainsboro;
            dgvItems.BorderStyle = BorderStyle.FixedSingle;
            dgvItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }

        private void AgregarItemManual()
        {
            // Verificar que estamos en Venta Directa
            if (tabControlPrincipal.SelectedTab != tabPageVentaDirecta)
            {
                MessageBox.Show("Los items solo se pueden agregar manualmente en el modo 'Venta Directa'.",
                    "Modo incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Verificar que hay un cliente seleccionado (opcional)
            if (string.IsNullOrWhiteSpace(clienteVDNombre) && !rbConsumidorFinalVD.Checked)
            {
                DialogResult result = MessageBox.Show("No ha seleccionado un cliente. ¿Desea continuar con la venta?",
                    "Cliente no seleccionado", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
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

                dtItems.Rows.Add("Producto", nombre, cantidad, precio, cantidad * precio, productoId, DBNull.Value);
                RecalcularTotales();
                ActualizarBotonesItemsPorTab();
            }
        }

        private void EliminarItemSeleccionado()
        {
            if (tabControlPrincipal.SelectedTab != tabPageVentaDirecta)
            {
                MessageBox.Show("Solo puede eliminar items en el modo 'Venta Directa'.",
                    "Modo incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (dgvItems.CurrentRow != null && dgvItems.CurrentRow.Index >= 0)
            {
                dgvItems.Rows.RemoveAt(dgvItems.CurrentRow.Index);
                RecalcularTotales();
                ActualizarBotonesItemsPorTab();
            }
        }

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
            if (drv != null && decimal.TryParse(drv["porcentaje"].ToString(), out decimal p))
                return p;
            return 0m;
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

        // ==================== VALIDACIONES ====================

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
            if (tabControlPrincipal.SelectedTab == tabPageOT)
            {
                if (!receptorConfirmadoOT)
                {
                    MarcarError(txtNombreOT, "Debe confirmar los datos del receptor");
                    return false;
                }
                MarcarOk(txtNombreOT);
                return true;
            }
            else
            {
                if (rbConsumidorFinalVD.Checked)
                    return true;

                if (string.IsNullOrWhiteSpace(clienteVDNombre) ||
                    string.IsNullOrWhiteSpace(clienteVDTipoDoc) ||
                    string.IsNullOrWhiteSpace(clienteVDNumDoc))
                {
                    MarcarError(txtNombreVD, "Debe seleccionar un cliente válido");
                    return false;
                }
                MarcarOk(txtNombreVD);
                return true;
            }
        }

        private bool ValidarTodo()
        {
            return ValidarItems() && ValidarImpuesto() && ValidarCliente();
        }

        // ==================== GENERACIÓN DE FACTURA ====================

        private bool ExisteFacturaParaOT(long otId)
        {
            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("SELECT COUNT(1) FROM dbo.Facturas WHERE orden_trabajo_id = @ot", cn))
            {
                cmd.Parameters.Add("@ot", SqlDbType.BigInt).Value = otId;
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private long? BuscarClientePorDocumento(string numeroDocumento)
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT id FROM Clientes WHERE numero_documento = @doc", cn))
                {
                    cmd.Parameters.AddWithValue("@doc", numeroDocumento);
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return Convert.ToInt64(result);
                }
            }
            catch { }
            return null;
        }

        private async Task GenerarFacturaAsync()
        {
            if (usuarioId == 0)
            {
                MessageBox.Show("Sesión inválida.");
                return;
            }

            // Validaciones según el tab activo
            if (tabControlPrincipal.SelectedTab == tabPageOT)
            {
                if (ordenTrabajoId == null || ordenTrabajoId <= 0)
                {
                    MessageBox.Show("Seleccione una OT.");
                    return;
                }
                if (!receptorConfirmadoOT)
                {
                    MessageBox.Show("Debe confirmar los datos del receptor antes de generar la factura.");
                    return;
                }
                if (ExisteFacturaParaOT(ordenTrabajoId.Value))
                {
                    MessageBox.Show("Esta OT ya tiene una factura generada.");
                    return;
                }
            }
            else
            {
                if (dtItems.Rows.Count == 0)
                {
                    MessageBox.Show("No hay items para facturar.");
                    return;
                }
                if (!rbConsumidorFinalVD.Checked && clienteIdVD == null && string.IsNullOrWhiteSpace(clienteVDNombre))
                {
                    MessageBox.Show("Debe seleccionar un cliente.");
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

            // Obtener datos del cliente según el tab activo
            long? clienteId = null;
            string tipoDoc = "", numDoc = "", nombre = "", direccion = "", telefono = "", email = "";
            bool contribuyenteEspecial = false;

            if (tabControlPrincipal.SelectedTab == tabPageOT)
            {
                tipoDoc = snapTipoDocOT;
                numDoc = snapNumDocOT;
                nombre = snapNombreOT;
                direccion = snapDireccionOT;
                telefono = snapTelefonoOT;
                email = snapEmailOT;
                contribuyenteEspecial = snapContribuyenteEspecialOT;
                clienteId = clienteIdOT;
            }
            else
            {
                if (rbConsumidorFinalVD.Checked)
                {
                    tipoDoc = "CF";
                    numDoc = "9999999999999";
                    nombre = "CONSUMIDOR FINAL";
                    direccion = "";
                    telefono = "";
                    email = "";
                    contribuyenteEspecial = false;
                    clienteId = null;
                }
                else
                {
                    tipoDoc = clienteVDTipoDoc;
                    numDoc = clienteVDNumDoc;
                    nombre = clienteVDNombre;
                    direccion = clienteVDDireccion;
                    telefono = clienteVDTelefono;
                    email = clienteVDEmail;
                    contribuyenteEspecial = clienteVDContribuyenteEspecial;
                    clienteId = clienteIdVD;
                }
            }

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
                            cmd.Parameters.Add("@ot", SqlDbType.BigInt).Value = (object)(tabControlPrincipal.SelectedTab == tabPageOT ? ordenTrabajoId : null) ?? DBNull.Value;
                            cmd.Parameters.Add("@user", SqlDbType.BigInt).Value = usuarioId;
                            cmd.Parameters.Add("@clave", SqlDbType.Char, 49).Value = claveAccesoGenerada;
                            cmd.Parameters.Add("@punto", SqlDbType.Char, 3).Value = _puntoEmisionActual;
                            cmd.Parameters.Add("@est", SqlDbType.Char, 3).Value = _establecimientoActual;
                            cmd.Parameters.Add("@sec", SqlDbType.Char, 9).Value = secuencialGenerado;
                            await AplicarPromocionesAutomaticas();

                            // Recalcular totales después de promociones
                            RecalcularTotales();

                            // Actualizar subtotal, iva y total
                            subtotal = dtItems.AsEnumerable().Sum(r => Convert.ToDecimal(r["subtotal"]));
                            iva = Math.Round(subtotal * pct, 4);
                            total = Math.Round(subtotal + iva, 4);

                            // Actualizar parámetros de la factura
                            cmd.Parameters["@sub15"].Value = subtotal;
                            cmd.Parameters["@iva"].Value = iva;
                            cmd.Parameters["@total"].Value = total;
                            cmd.Parameters.Add("@sub0", SqlDbType.Decimal).Value = 0m;
                            
                            cmd.Parameters.Add("@cliente", SqlDbType.BigInt).Value = (object)clienteId ?? DBNull.Value;
                            cmd.Parameters.Add("@tdoc", SqlDbType.NVarChar, 10).Value = tipoDoc;
                            cmd.Parameters.Add("@ndoc", SqlDbType.NVarChar, 13).Value = numDoc;
                            cmd.Parameters.Add("@cnom", SqlDbType.NVarChar, 255).Value = nombre;
                            cmd.Parameters.Add("@cdir", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(direccion) ? (object)DBNull.Value : direccion;
                            cmd.Parameters.Add("@ctel", SqlDbType.NVarChar, 15).Value = string.IsNullOrWhiteSpace(telefono) ? (object)DBNull.Value : telefono;
                            cmd.Parameters.Add("@cemail", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email;
                            cmd.Parameters.Add("@cces", SqlDbType.Bit).Value = contribuyenteEspecial;

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

                        // Descontar stock
                        if (dtItems.Rows.Count > 0)
                        {
                            DescontarStockProductos(tx, dtItems, facturaId);
                        }

                        if (tabControlPrincipal.SelectedTab == tabPageOT && ordenTrabajoId.HasValue)
                        {
                            using (var cmd = new SqlCommand(
                                "UPDATE dbo.OrdenesTrabajo SET facturada = 1 WHERE id = @id", cn, tx))
                            {
                                cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = ordenTrabajoId.Value;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                        facturaIdGenerada = facturaId;

                        string pdfPath = GenerarPdfFactura();
                        GuardarPdfEnFactura(facturaId, pdfPath);

                        // Limpiar después de generar
                        dtItems.Clear();
                        if (tabControlPrincipal.SelectedTab == tabPageOT)
                            LimpiarTabOT();
                        else
                            LimpiarTabVD();

                        RecalcularTotales();
                        ActualizarBotonesItemsPorTab();

                        MessageBox.Show($"Factura generada exitosamente!\n\nID: {facturaId}\nSecuencial: {secuencialGenerado}\nCliente: {nombre}",
                            "Factura Generada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        try { tx.Rollback(); } catch { }
                        MessageBox.Show("Error generando factura:\n" + ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            await Task.CompletedTask;
        }

        // ==================== MÉTODOS AUXILIARES ====================

        private byte[] ObtenerLogoEmpresa()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 logo FROM Empresa WHERE id = 1", cn))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return (byte[])result;
                }
            }
            catch { }
            return null;
        }

        private (string nombre, string ruc, string direccion, string telefono, string email) ObtenerEmpresa()
        {
            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("SELECT TOP 1 nombre,ruc,direccion,telefono,email FROM Empresa WHERE id=1;", cn))
            using (var rd = cmd.ExecuteReader())
            {
                if (!rd.Read()) return ("EMPRESA NO CONFIGURADA", "", "", "", "");
                return (rd["nombre"]?.ToString() ?? "", rd["ruc"]?.ToString() ?? "",
                        rd["direccion"]?.ToString() ?? "", rd["telefono"]?.ToString() ?? "",
                        rd["email"]?.ToString() ?? "");
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

        private string GenerarSiguienteSecuencial()
        {
            try
            {
                // Obtener datos de la empresa
                var empresa = ObtenerDatosEmpresa();

                using (var cn = con.CrearConexionAbierta())
                using (var tx = cn.BeginTransaction())
                {
                    // Verificar si existe el secuencial para esta combinación de establecimiento/punto_emision
                    string sqlCheck = @"
IF NOT EXISTS (SELECT 1 FROM Secuenciales 
               WHERE tipo_documento = 'FACTURA' 
               AND establecimiento = @establecimiento 
               AND punto_emision = @puntoEmision)
BEGIN
    INSERT INTO Secuenciales (tipo_documento, establecimiento, punto_emision, secuencia_actual)
    VALUES ('FACTURA', @establecimiento, @puntoEmision, 0);
END";

                    using (var cmdCheck = new SqlCommand(sqlCheck, cn, tx))
                    {
                        cmdCheck.Parameters.AddWithValue("@establecimiento", empresa.Establecimiento);
                        cmdCheck.Parameters.AddWithValue("@puntoEmision", empresa.PuntoEmision);
                        cmdCheck.ExecuteNonQuery();
                    }

                    // Obtener y actualizar el secuencial
                    string sqlUpdate = @"
DECLARE @nuevaSecuencia INT;

UPDATE Secuenciales 
SET @nuevaSecuencia = secuencia_actual = secuencia_actual + 1
WHERE tipo_documento = 'FACTURA' 
AND establecimiento = @establecimiento 
AND punto_emision = @puntoEmision;

SELECT @nuevaSecuencia AS NuevaSecuencia;";

                    int nuevaSecuencia;
                    using (var cmdUpdate = new SqlCommand(sqlUpdate, cn, tx))
                    {
                        cmdUpdate.Parameters.AddWithValue("@establecimiento", empresa.Establecimiento);
                        cmdUpdate.Parameters.AddWithValue("@puntoEmision", empresa.PuntoEmision);
                        var result = cmdUpdate.ExecuteScalar();
                        nuevaSecuencia = result != null ? Convert.ToInt32(result) : 1;
                    }

                    tx.Commit();

                    // Guardar el establecimiento y punto de emisión para usarlos después
                    _establecimientoActual = empresa.Establecimiento;
                    _puntoEmisionActual = empresa.PuntoEmision;

                    return nuevaSecuencia.ToString().PadLeft(9, '0');
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar secuencial: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Fallback: generar secuencial basado en MAX+1
                try
                {
                    using (var cn = con.CrearConexionAbierta())
                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(secuencial AS INT)), 0) + 1 FROM Facturas", cn))
                    {
                        int next = Convert.ToInt32(cmd.ExecuteScalar());
                        _establecimientoActual = "001";
                        _puntoEmisionActual = "001";
                        return next.ToString().PadLeft(9, '0');
                    }
                }
                catch
                {
                    _establecimientoActual = "001";
                    _puntoEmisionActual = "001";
                    return "000000001";
                }
            }
        }

        private void DescontarStockProductos(SqlTransaction tx, DataTable items, long facturaId)
        {
            foreach (DataRow row in items.Rows)
            {
                if (row["producto_id"] != DBNull.Value)
                {
                    long productoId = Convert.ToInt64(row["producto_id"]);
                    int cantidad = Convert.ToInt32(Math.Round(Convert.ToDecimal(row["cantidad"]), 0));
                    if (cantidad <= 0) continue;

                    string sqlCheckStock = @"SELECT stock FROM Productos WITH (UPDLOCK, ROWLOCK) WHERE id = @productoId";
                    using (var cmdCheck = new SqlCommand(sqlCheckStock, tx.Connection, tx))
                    {
                        cmdCheck.Parameters.AddWithValue("@productoId", productoId);
                        var stockActual = cmdCheck.ExecuteScalar();
                        if (stockActual == null) throw new Exception($"Producto ID {productoId} no encontrado");
                        int stock = Convert.ToInt32(stockActual);
                        if (stock < cantidad)
                        {
                            string nombreProducto = ObtenerNombreProducto(productoId);
                            throw new Exception($"Stock insuficiente para '{nombreProducto}'. Disponible: {stock}, Requerido: {cantidad}");
                        }
                    }

                    string sqlUpdateStock = @"UPDATE Productos SET stock = stock - @cantidad WHERE id = @productoId";
                    using (var cmdUpdate = new SqlCommand(sqlUpdateStock, tx.Connection, tx))
                    {
                        cmdUpdate.Parameters.AddWithValue("@cantidad", cantidad);
                        cmdUpdate.Parameters.AddWithValue("@productoId", productoId);
                        cmdUpdate.ExecuteNonQuery();
                    }

                    RegistrarMovimientoKardex(tx, productoId, "SALIDA", "FACTURA", facturaId, cantidad, usuarioId);
                }
            }
        }

        private string ObtenerNombreProducto(long productoId)
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT nombre FROM Productos WHERE id = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", productoId);
                    return cmd.ExecuteScalar()?.ToString() ?? "Producto desconocido";
                }
            }
            catch { return "Producto desconocido"; }
        }

        private void RegistrarMovimientoKardex(SqlTransaction tx, long productoId, string tipoMovimiento, string origen, long? referenciaId, int cantidad, long usuarioId)
        {
            string sql = @"
INSERT INTO Kardex (producto_id, usuario_id, tipo_movimiento, origen, referencia_id, cantidad, fecha)
VALUES (@productoId, @usuarioId, @tipoMovimiento, @origen, @referenciaId, @cantidad, GETDATE())";

            using (var cmd = new SqlCommand(sql, tx.Connection, tx))
            {
                cmd.Parameters.AddWithValue("@productoId", productoId);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@tipoMovimiento", tipoMovimiento);
                cmd.Parameters.AddWithValue("@origen", origen);
                cmd.Parameters.AddWithValue("@referenciaId", (object)referenciaId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@cantidad", cantidad);
                cmd.ExecuteNonQuery();
            }
        }

        private void GuardarPdfEnFactura(long facturaId, string pdfPath)
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string nombre = Path.GetFileName(pdfPath);

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("UPDATE Facturas SET pdf_data = @pdf, pdf_nombre = @nom WHERE id = @id;", cn))
            {
                cmd.Parameters.Add("@pdf", SqlDbType.VarBinary, -1).Value = pdfBytes;
                cmd.Parameters.AddWithValue("@nom", nombre);
                cmd.Parameters.AddWithValue("@id", facturaId);
                cmd.ExecuteNonQuery();
            }
        }

        private string GenerarPdfFactura()
        {
            if (dtItems == null || dtItems.Rows.Count == 0)
                throw new Exception("No hay items para generar el PDF.");

            var empresa = ObtenerDatosEmpresa();

            // Usar el establecimiento y punto de emisión reales
            string secTmp = string.IsNullOrWhiteSpace(secuencialGenerado) ? "000000001" : secuencialGenerado;
            string numeroFactura = $"{_establecimientoActual}-{_puntoEmisionActual}-{secTmp}";

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

                // Encabezado con logo
                PdfPTable head = new PdfPTable(empresa.Logo != null ? 3 : 2);
                head.WidthPercentage = 100;
                if (empresa.Logo != null)
                    head.SetWidths(new float[] { 20, 45, 35 });
                else
                    head.SetWidths(new float[] { 65, 35 });

                if (empresa.Logo != null)
                {
                    var cellLogo = new PdfPCell { Border = Rectangle.BOX, Padding = 5, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                    try
                    {
                        iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(empresa.Logo);
                        logo.ScaleToFit(80, 80);
                        cellLogo.AddElement(logo);
                    }
                    catch
                    {
                        cellLogo.AddElement(new Paragraph("Logo no disponible", font));
                    }
                    head.AddCell(cellLogo);
                }

                // Datos de la empresa
                var cellEmp = new PdfPCell { Border = Rectangle.BOX, Padding = 8 };
                cellEmp.AddElement(new Paragraph(empresa.Nombre?.ToUpper() ?? "EMPRESA", fontSub));
                if (!string.IsNullOrWhiteSpace(empresa.Ruc))
                    cellEmp.AddElement(new Paragraph($"RUC: {empresa.Ruc}", font));
                if (!string.IsNullOrWhiteSpace(empresa.Direccion))
                    cellEmp.AddElement(new Paragraph($"Dirección: {empresa.Direccion}", font));
                if (!string.IsNullOrWhiteSpace(empresa.Telefono))
                    cellEmp.AddElement(new Paragraph($"Tel: {empresa.Telefono}", font));
                if (!string.IsNullOrWhiteSpace(empresa.Email))
                    cellEmp.AddElement(new Paragraph($"Email: {empresa.Email}", font));
                head.AddCell(cellEmp);

                // Datos de la factura
                var cellFac = new PdfPCell { Border = Rectangle.BOX, Padding = 8 };
                cellFac.AddElement(new Paragraph("FACTURA", fontTitle));
                cellFac.AddElement(new Paragraph($"No.: {numeroFactura}", fontSub));
                cellFac.AddElement(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", font));
                cellFac.AddElement(new Paragraph($"OT: {(ordenTrabajoId == null ? "N/A" : ordenTrabajoId.ToString())}", font));
                if (!string.IsNullOrWhiteSpace(claveAccesoGenerada))
                    cellFac.AddElement(new Paragraph($"Clave Acceso: {claveAccesoGenerada}", font));
                head.AddCell(cellFac);

                doc.Add(head);
                doc.Add(new Paragraph(" "));

                // Datos del receptor (igual que antes)
                string receptorNombre, receptorTipoDoc, receptorNumDoc, receptorDireccion, receptorEmail;

                if (tabControlPrincipal.SelectedTab == tabPageOT)
                {
                    receptorNombre = snapNombreOT;
                    receptorTipoDoc = snapTipoDocOT;
                    receptorNumDoc = snapNumDocOT;
                    receptorDireccion = snapDireccionOT;
                    receptorEmail = snapEmailOT;
                }
                else
                {
                    receptorNombre = clienteVDNombre;
                    receptorTipoDoc = clienteVDTipoDoc;
                    receptorNumDoc = clienteVDNumDoc;
                    receptorDireccion = clienteVDDireccion;
                    receptorEmail = clienteVDEmail;
                }

                PdfPTable rec = new PdfPTable(2);
                rec.WidthPercentage = 100;
                rec.SetWidths(new float[] { 50, 50 });

                rec.AddCell(new PdfPCell(new Phrase($"Cliente: {receptorNombre}", font)) { Padding = 6, Border = Rectangle.BOX });
                rec.AddCell(new PdfPCell(new Phrase($"Identificación: {receptorTipoDoc} {receptorNumDoc}", font)) { Padding = 6, Border = Rectangle.BOX });
                rec.AddCell(new PdfPCell(new Phrase($"Dirección: {(string.IsNullOrWhiteSpace(receptorDireccion) ? "-" : receptorDireccion)}", font)) { Padding = 6, Border = Rectangle.BOX });
                rec.AddCell(new PdfPCell(new Phrase($"Email: {(string.IsNullOrWhiteSpace(receptorEmail) ? "-" : receptorEmail)}", font)) { Padding = 6, Border = Rectangle.BOX });

                doc.Add(rec);
                doc.Add(new Paragraph(" "));

                // Items (igual que antes)
                PdfPTable itemsTable = new PdfPTable(6); // Ahora 6 columnas
                itemsTable.WidthPercentage = 100;
                itemsTable.SetWidths(new float[] { 7, 7, 40, 12, 12, 12 });

                void AddHeader(string t)
                {
                    var c = new PdfPCell(new Phrase(t, fontSub)) { Padding = 6, BackgroundColor = BaseColor.LIGHT_GRAY };
                    itemsTable.AddCell(c);
                }

                AddHeader("Tipo");
                AddHeader("Cant");
                AddHeader("Descripción");
                AddHeader("P.Unit");
                AddHeader("Dto.%");
                AddHeader("Subtotal");

                foreach (DataRow row in dtItems.Rows)
                {
                    string tipo = row["tipo_item"]?.ToString() ?? "Item";
                    decimal cant = Convert.ToDecimal(row["cantidad"]);
                    string desc = row["nombre_item"]?.ToString() ?? "";
                    decimal pu = Convert.ToDecimal(row["precio_unitario"]);
                    decimal sub = Convert.ToDecimal(row["subtotal"]);

                    // Obtener porcentaje de descuento si existe
                    string descuentoTexto = "0%";
                    if (dtItems.Columns.Contains("descuento_porcentaje") && row["descuento_porcentaje"] != DBNull.Value)
                    {
                        decimal dto = Convert.ToDecimal(row["descuento_porcentaje"]);
                        descuentoTexto = $"{dto}%";
                    }

                    itemsTable.AddCell(new PdfPCell(new Phrase(tipo, font)) { Padding = 5 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(cant.ToString("0.##"), font)) { Padding = 5 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(desc, font)) { Padding = 5 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(pu.ToString("0.00"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    itemsTable.AddCell(new PdfPCell(new Phrase(descuentoTexto, font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER });
                    itemsTable.AddCell(new PdfPCell(new Phrase(sub.ToString("0.00"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(itemsTable);
                doc.Add(new Paragraph(" "));
                // Mostrar promociones aplicadas
                if (_promocionesAplicadas.Count > 0)
                {
                    doc.Add(new Paragraph("Promociones Aplicadas:", fontSub));
                    doc.Add(new Paragraph(" "));

                    PdfPTable promosTable = new PdfPTable(1);
                    promosTable.WidthPercentage = 100;

                    foreach (long promoId in _promocionesAplicadas)
                    {
                        string nombrePromo = ObtenerNombrePromocion(promoId);
                        var cellPromo = new PdfPCell(new Phrase($"✓ {nombrePromo}", font))
                        {
                            Padding = 5,
                            Border = Rectangle.BOX,
                            BackgroundColor = new BaseColor(240, 240, 240)
                        };
                        promosTable.AddCell(cellPromo);
                    }

                    doc.Add(promosTable);
                    doc.Add(new Paragraph(" "));
                }
                // Totales (igual que antes)
                PdfPTable tot = new PdfPTable(2);
                tot.HorizontalAlignment = Element.ALIGN_RIGHT;
                tot.WidthPercentage = 40;
                tot.SetWidths(new float[] { 60, 40 });

                void AddTotalRow(string k, string v, bool bold = false)
                {
                    var f = bold ? fontSub : font;
                    tot.AddCell(new PdfPCell(new Phrase(k, f)) { Padding = 6, Border = Rectangle.BOX });
                    tot.AddCell(new PdfPCell(new Phrase(v, f)) { Padding = 6, Border = Rectangle.BOX, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                AddTotalRow("Subtotal", subtotal.ToString("0.00"));
                AddTotalRow($"IVA ({GetImpuestoPorcentajeActual():0.##}%)", iva.ToString("0.00"));
                AddTotalRow("TOTAL", total.ToString("0.00"), bold: true);

                doc.Add(tot);
                doc.Close();
            }

            return pdfPath;
        }

        private string ObtenerNombrePromocion(long promocionId)
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT nombre FROM Promociones WHERE id = @id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", promocionId);
                    return cmd.ExecuteScalar()?.ToString() ?? "Promoción desconocida";
                }
            }
            catch { return "Promoción desconocida"; }
        }

        private void VerPdfFactura()
        {
            try
            {
                string pdfPath = GenerarPdfFactura();
                using (var v = new PROYECTOMECANICO.FormPdfViewer(pdfPath, title: "Vista previa - Factura", defaultSaveName: Path.GetFileName(pdfPath)))
                {
                    v.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar/ver el PDF:\n" + ex.Message);
            }
        }

        private void FormGenFactu_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvItems);
        }

        private void btnBuscadorOrdenTrabajo_Click(object sender, EventArgs e)
        {
            // Asegurarse que estamos en la pestaña OT
            if (tabControlPrincipal.SelectedTab != tabPageOT)
            {
                tabControlPrincipal.SelectedTab = tabPageOT;
            }

            // Crear el buscador de órdenes de trabajo
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.OrdenesTrabajo);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long ordenId = Convert.ToInt64(fila["id"]);
                string placa = fila["placa"].ToString();
                string cliente = fila["cliente"].ToString();
                string estado = fila["estado"].ToString();

                // Validar estado de la OT
                if (estado != "Terminado" && estado != "Entregado")
                {
                    MessageBox.Show($"La orden seleccionada está en estado '{estado}'. Solo se pueden facturar órdenes en estado 'Terminado' o 'Entregado'.",
                        "Estado no válido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verificar si ya tiene factura
                if (ExisteFacturaParaOT(ordenId))
                {
                    MessageBox.Show("Esta OT ya tiene una factura generada.",
                        "OT ya facturada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cargar datos de la OT
                ordenTrabajoId = ordenId;
                txtBuscarOT.Text = $"OT #{ordenId} - {placa} - {cliente}";

                // Buscar y cargar los datos del cliente de la OT
                CargarDatosOTCompletos(ordenId);

                // Preguntar si quiere cargar los items
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
            // Asegurarse que estamos en la pestaña Venta Directa
            if (tabControlPrincipal.SelectedTab != tabPageVentaDirecta)
            {
                tabControlPrincipal.SelectedTab = tabPageVentaDirecta;
            }

            // Asegurarse que está seleccionado cliente existente
            rbClienteExistenteVD.Checked = true;

            // Crear el buscador de clientes
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Clientes);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long clienteId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();

                // Cargar datos completos del cliente
                CargarDatosClienteCompletos(clienteId);

                txtBuscarClienteVD.Text = nombre;
                lstClientesVD.Visible = false;
            }
        }

        private void CargarDatosClienteCompletos(long clienteId)
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
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = clienteId;
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            clienteIdVD = Convert.ToInt64(dr["id"]);
                            clienteVDTipoDoc = dr["tipo_documento"].ToString();
                            clienteVDNumDoc = dr["numero_documento"].ToString();
                            clienteVDNombre = dr["nombre"].ToString();
                            clienteVDDireccion = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString();
                            clienteVDTelefono = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString();
                            clienteVDEmail = dr["email"] == DBNull.Value ? "" : dr["email"].ToString();
                            clienteVDContribuyenteEspecial = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"]);

                            ReflejarClienteVDEnUI();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos del cliente:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void CargarDatosOTCompletos(long otId)
        {
            try
            {
                con.Abrir();
                string sql = @"
SELECT TOP 1
    C.id AS cliente_id,
    C.tipo_documento, 
    C.numero_documento, 
    C.nombre,
    C.direccion, 
    C.telefono, 
    C.email, 
    C.contribuyente_especial
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
                            clienteIdOT = Convert.ToInt64(dr["cliente_id"]);
                            snapTipoDocOT = dr["tipo_documento"].ToString();
                            snapNumDocOT = dr["numero_documento"].ToString();
                            snapNombreOT = dr["nombre"].ToString();
                            snapDireccionOT = dr["direccion"] == DBNull.Value ? "" : dr["direccion"].ToString();
                            snapTelefonoOT = dr["telefono"] == DBNull.Value ? "" : dr["telefono"].ToString();
                            snapEmailOT = dr["email"] == DBNull.Value ? "" : dr["email"].ToString();
                            snapContribuyenteEspecialOT = dr["contribuyente_especial"] != DBNull.Value && Convert.ToBoolean(dr["contribuyente_especial"]);

                            // Cargar en los campos editables
                            txtTipoDocOT.Text = snapTipoDocOT;
                            txtNumDocOT.Text = snapNumDocOT;
                            txtNombreOT.Text = snapNombreOT;
                            txtDireccionOT.Text = snapDireccionOT;
                            txtTelefonoOT.Text = snapTelefonoOT;
                            txtEmailOT.Text = snapEmailOT;

                            receptorConfirmadoOT = true;
                            btnConfirmarReceptorOT.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
                            btnConfirmarReceptorOT.Text = "✓ Confirmado";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos de la OT:\n" + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private class DatosEmpresa
        {
            public string Nombre { get; set; }
            public string Ruc { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
            public string Email { get; set; }
            public string Establecimiento { get; set; }
            public string PuntoEmision { get; set; }
            public byte[] Logo { get; set; }
        }


        private DatosEmpresa ObtenerDatosEmpresa()
        {
            var datos = new DatosEmpresa();
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(@"
            SELECT TOP 1 
                nombre, ruc, direccion, telefono, email, 
                establecimiento, punto_emision, logo 
            FROM Empresa WHERE id = 1", cn))
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        datos.Nombre = rd["nombre"]?.ToString() ?? "EMPRESA NO CONFIGURADA";
                        datos.Ruc = rd["ruc"]?.ToString() ?? "";
                        datos.Direccion = rd["direccion"]?.ToString() ?? "";
                        datos.Telefono = rd["telefono"]?.ToString() ?? "";
                        datos.Email = rd["email"]?.ToString() ?? "";
                        datos.Establecimiento = rd["establecimiento"]?.ToString()?.PadLeft(3, '0') ?? "001";
                        datos.PuntoEmision = rd["punto_emision"]?.ToString()?.PadLeft(3, '0') ?? "001";
                        datos.Logo = rd["logo"] != DBNull.Value ? (byte[])rd["logo"] : null;
                    }
                    else
                    {
                        datos.Establecimiento = "001";
                        datos.PuntoEmision = "001";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al obtener datos empresa: " + ex.Message);
                datos.Establecimiento = "001";
                datos.PuntoEmision = "001";
            }
            return datos;
        }

        private void btnAplicarDescuento_Click(object sender, EventArgs e)
        {
            AplicarDescuentoAItem();
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            AgregarItemManual();
        }

        private void btnDelItem_Click(object sender, EventArgs e)
        {
            EliminarItemSeleccionado();
        }

        private void btnGenerarFactura_Click(object sender, EventArgs e)
        {
            // El evento ya está configurado en ConfigurarEventosComunes()
            // Este método es solo para que el diseñador no dé error
        }

        private void btnVistaPrevia_Click(object sender, EventArgs e)
        {
            // El evento ya está configurado en ConfigurarEventosComunes()
            // Este método es solo para que el diseñador no dé error
        }

        
    }
}