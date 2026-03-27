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
    public partial class FormNotaCredito : Form
    {
        private readonly Conexion con = new Conexion();
        private long usuarioId;
        private string rolUsuario;

        // Datos de la factura seleccionada
        private DataTable dtFactura = new DataTable();
        private DataTable dtFacturaItems = new DataTable();
        private long? facturaIdSeleccionada = null;
        private string secuencialFactura = "";
        private string claveAccesoFactura = "";

        // Datos de la nota de crédito
        private long notaCreditoId = 0;
        private string secuencialGenerado = "";
        private string claveAccesoGenerada = "";

        // Variables para control
        private bool modoEdicion = false;
        private decimal subtotalOriginal = 0;
        private decimal ivaOriginal = 0;
        private decimal totalOriginal = 0;

        public FormNotaCredito(long usuarioId, string rolUsuario)
        {
            // Configurar cultura para aceptar punto y coma como decimal
            System.Threading.Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("es-EC");
            System.Threading.Thread.CurrentThread.CurrentUICulture =
                new System.Globalization.CultureInfo("es-EC");

            InitializeComponent();
            this.usuarioId = usuarioId;
            this.rolUsuario = rolUsuario;
            InicializarFormulario();
        }

        private void InicializarFormulario()
        {
            // Configurar grid de items de la factura
            ConfigurarGridFactura();

            // Configurar grid de items a devolver
            ConfigurarGridDevolucion();

            // Cargar combos
            CargarMotivosModificacion();

            // Eventos
            btnBuscarFactura.Click += BtnBuscarFactura_Click;
            btnCargarFactura.Click += BtnCargarFactura_Click;
            btnCalcular.Click += BtnCalcular_Click;
            btnGenerarNota.Click += BtnGenerarNota_Click;
            btnVistaPrevia.Click += BtnVistaPrevia_Click;
            btnLimpiar.Click += BtnLimpiar_Click;

            // Eventos del grid de devolución
            dgvDevolucion.CellValueChanged += DgvDevolucion_CellValueChanged;
            dgvDevolucion.CellEndEdit += DgvDevolucion_CellEndEdit;

            // AGREGAR ESTE MANEJADOR PARA ERRORES DE DATOS
            dgvDevolucion.DataError += DgvDevolucion_DataError;

            // Estado inicial
            btnCalcular.Enabled = false;
            btnGenerarNota.Enabled = false;
            btnVistaPrevia.Enabled = false;
            txtMotivo.Enabled = false;
            txtObservacion.Enabled = false;
        }

        private void DgvDevolucion_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Manejar el error sin mostrar el diálogo predeterminado
            if (e.ColumnIndex >= 0 && dgvDevolucion.Columns[e.ColumnIndex].Name == "CantDevolver")
            {
                if (e.RowIndex >= 0 && e.RowIndex < dgvDevolucion.Rows.Count)
                {
                    var row = dgvDevolucion.Rows[e.RowIndex];
                    var cell = row.Cells["CantDevolver"];

                    try
                    {
                        // Intentar convertir a decimal
                        if (cell.Value != null && cell.Value.ToString().Trim() != "")
                        {
                            decimal test = Convert.ToDecimal(cell.Value);
                        }
                        else
                        {
                            cell.Value = 0m;
                        }
                    }
                    catch
                    {
                        cell.Value = 0m;
                    }
                }
            }

            e.ThrowException = false;
        }

        private void BtnCargarFactura_Click(object sender, EventArgs e)
        {
            // Este método ya no es necesario porque la carga se hace automáticamente en BtnBuscarFactura_Click
            // Pero lo dejamos para que no dé error
            if (facturaIdSeleccionada.HasValue)
            {
                Task.Run(async () => await CargarFacturaCompleta(facturaIdSeleccionada.Value));
            }
        }

        private void ConfigurarGridFactura()
        {
            dgvFactura.AutoGenerateColumns = false;
            dgvFactura.ReadOnly = true;
            dgvFactura.AllowUserToAddRows = false;
            dgvFactura.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFactura.MultiSelect = false;

            dgvFactura.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 50 });
            dgvFactura.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Secuencial", DataPropertyName = "Secuencial", Width = 100 });
            dgvFactura.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Fecha", DataPropertyName = "Fecha", Width = 100 });
            dgvFactura.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Cliente", DataPropertyName = "Cliente", Width = 200 });
            dgvFactura.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "Total", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
        }

        private void ConfigurarGridDevolucion()
        {
            dgvDevolucion.Columns.Clear();
            dgvDevolucion.AutoGenerateColumns = false;
            dgvDevolucion.AllowUserToAddRows = false;
            dgvDevolucion.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDevolucion.MultiSelect = false;

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ItemId",
                HeaderText = "ID Item",
                DataPropertyName = "ItemId",
                Visible = false
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Tipo",
                HeaderText = "Tipo",
                DataPropertyName = "Tipo",
                Width = 80,
                ReadOnly = true
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = "Producto/Servicio",
                DataPropertyName = "Nombre",
                Width = 250,
                ReadOnly = true
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CantOriginal",
                HeaderText = "Cant. Original",
                DataPropertyName = "CantOriginal",
                Width = 80,
                ReadOnly = true
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CantDevolver",
                HeaderText = "Cant. a Devolver",
                DataPropertyName = "CantDevolver",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    NullValue = "0",
                    Format = "N0",  // Formato sin decimales
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PrecioUnitario",
                HeaderText = "Precio Unit.",
                DataPropertyName = "PrecioUnitario",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Subtotal",
                HeaderText = "Subtotal",
                DataPropertyName = "Subtotal",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Iva",
                HeaderText = "IVA",
                DataPropertyName = "Iva",
                Width = 80,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "producto_id",
                HeaderText = "ProductoId",
                DataPropertyName = "producto_id",
                Visible = false
            });

            dgvDevolucion.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "servicio_id",
                HeaderText = "ServicioId",
                DataPropertyName = "servicio_id",
                Visible = false
            });
        }

        private void CargarMotivosModificacion()
        {
            cmbMotivo.Items.Clear();
            cmbMotivo.Items.Add("Devolución de producto");
            cmbMotivo.Items.Add("Producto en mal estado");
            cmbMotivo.Items.Add("Producto no solicitado");
            cmbMotivo.Items.Add("Error en facturación");
            cmbMotivo.Items.Add("Descuento no aplicado");
            cmbMotivo.Items.Add("Otros");
            cmbMotivo.SelectedIndex = 0;
        }

        private async void BtnBuscarFactura_Click(object sender, EventArgs e)
        {
            using (var buscador = new FormBuscador(FormBuscador.TipoBusqueda.Facturas))
            {
                if (buscador.ShowDialog() == DialogResult.OK)
                {
                    DataRow row = buscador.ResultadoSeleccionado;

                    // Verificar que la fila tenga las columnas esperadas
                    if (!row.Table.Columns.Contains("id") || !row.Table.Columns.Contains("secuencial"))
                    {
                        MessageBox.Show("La consulta no devolvió los datos esperados.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    facturaIdSeleccionada = Convert.ToInt64(row["id"]);
                    secuencialFactura = row["secuencial"].ToString();

                    // Obtener establecimiento y punto_emision (pueden ser nulos)
                    string establecimiento = row["establecimiento"]?.ToString() ?? "001";
                    string puntoEmision = row["punto_emision"]?.ToString() ?? "001";
                    claveAccesoFactura = row["clave_acceso"]?.ToString() ?? "";

                    // Formatear número de factura
                    txtNumFactura.Text = $"{establecimiento}-{puntoEmision}-{secuencialFactura}";

                    // Obtener nombre del cliente
                    string clienteNombre = row.Table.Columns.Contains("cliente_nombre") ?
                        row["cliente_nombre"]?.ToString() ?? "" :
                        row["cliente_nombre_snap"]?.ToString() ?? "";
                    txtCliente.Text = clienteNombre;

                    // Obtener fecha
                    DateTime fechaFactura = Convert.ToDateTime(row["fecha"]);
                    txtFechaFactura.Text = fechaFactura.ToString("dd/MM/yyyy HH:mm");

                    // Cargar datos de la factura (ya es async, no necesita Task.Run)
                    await CargarFacturaCompleta(facturaIdSeleccionada.Value);

                    btnCargarFactura.Enabled = true;
                }
            }
        }

        private async Task CargarFacturaCompleta(long facturaId)
        {
            try
            {
                DataTable dtFacturaTemp = new DataTable();
                DataTable dtFacturaItemsTemp = new DataTable();

                using (var cn = con.CrearConexionAbierta())
                {
                    // Cargar cabecera de la factura
                    string sqlFactura = @"
                SELECT f.*, 
                       ISNULL(f.subtotal_15_iva, 0) + ISNULL(f.subtotal_0_iva, 0) AS subtotal,
                       f.valor_iva,
                       f.total_final
                FROM Facturas f
                WHERE f.id = @id AND f.estado = 'PENDIENTE'";

                    using (var cmd = new SqlCommand(sqlFactura, cn))
                    {
                        cmd.Parameters.AddWithValue("@id", facturaId);
                        var da = new SqlDataAdapter(cmd);
                        da.Fill(dtFacturaTemp);

                        if (dtFacturaTemp.Rows.Count == 0)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show("La factura no existe o ya fue anulada.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                btnCargarFactura.Enabled = false;
                            }));
                            return;
                        }

                        var row = dtFacturaTemp.Rows[0];
                        subtotalOriginal = Convert.ToDecimal(row["subtotal"]);
                        ivaOriginal = Convert.ToDecimal(row["valor_iva"]);
                        totalOriginal = Convert.ToDecimal(row["total_final"]);

                        this.Invoke(new Action(() =>
                        {
                            txtSubtotal.Text = subtotalOriginal.ToString("C2");
                            txtIVA.Text = ivaOriginal.ToString("C2");
                            txtTotal.Text = totalOriginal.ToString("C2");
                        }));
                    }

                    // Cargar items de la factura
                    string sqlItems = @"
                SELECT fi.id AS ItemId,
                       CASE WHEN fi.producto_id IS NOT NULL THEN 'Producto' ELSE 'Servicio' END AS Tipo,
                       ISNULL(p.nombre, s.nombre) AS Nombre,
                       fi.cantidad AS CantOriginal,
                       fi.precio_unitario AS PrecioUnitario,
                       (fi.cantidad * fi.precio_unitario) AS SubtotalItem,
                       fi.valor_iva_item AS IvaItem,
                       fi.producto_id,
                       fi.servicio_id
                FROM FacturaItems fi
                LEFT JOIN Productos p ON p.id = fi.producto_id
                LEFT JOIN Servicios s ON s.id = fi.servicio_id
                WHERE fi.factura_id = @facturaId";

                    using (var cmd = new SqlCommand(sqlItems, cn))
                    {
                        cmd.Parameters.AddWithValue("@facturaId", facturaId);
                        var da = new SqlDataAdapter(cmd);
                        da.Fill(dtFacturaItemsTemp);
                    }
                }

                // AGREGAR LA COLUMNA CantDevolver como INT
                if (!dtFacturaItemsTemp.Columns.Contains("CantDevolver"))
                {
                    dtFacturaItemsTemp.Columns.Add("CantDevolver", typeof(int));
                }

                // Inicializar CantDevolver en 0
                foreach (DataRow row in dtFacturaItemsTemp.Rows)
                {
                    row["CantDevolver"] = 0;
                }

                // Guardar los datos
                dtFacturaItems = dtFacturaItemsTemp;
                dtFactura = dtFacturaTemp;

                // ACTUALIZAR UI
                this.Invoke(new Action(() =>
                {
                    dgvDevolucion.DataSource = null;
                    dgvDevolucion.Columns.Clear();
                    ConfigurarGridDevolucion();
                    dgvDevolucion.DataSource = dtFacturaItems;

                    txtMotivo.Enabled = true;
                    txtObservacion.Enabled = true;
                    btnCalcular.Enabled = true;
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show("Error al cargar la factura: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private void DgvDevolucion_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvDevolucion.Columns.Count > 0)
            {
                if (dgvDevolucion.Columns[e.ColumnIndex].Name == "CantDevolver")
                {
                    // ACTUALIZAR EL DATATABLE
                    if (dtFacturaItems != null && e.RowIndex < dtFacturaItems.Rows.Count)
                    {
                        var value = dgvDevolucion.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        dtFacturaItems.Rows[e.RowIndex]["CantDevolver"] = value;
                        System.Diagnostics.Debug.WriteLine($"CellValueChanged: Fila {e.RowIndex}, Valor = {value}");
                    }

                    RecalcularDevolucion();
                }
            }
        }

        private void DgvDevolucion_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dgvDevolucion.Columns[e.ColumnIndex].Name == "CantDevolver")
                {
                    var row = dgvDevolucion.Rows[e.RowIndex];
                    var cell = row.Cells["CantDevolver"];

                    int valorIngresado = 0;
                    bool esNumero = false;

                    if (cell.Value != null && cell.Value.ToString().Trim() != "")
                    {
                        esNumero = int.TryParse(cell.Value.ToString(), out valorIngresado);
                    }

                    if (!esNumero || valorIngresado < 0)
                    {
                        cell.Value = 0;
                        MessageBox.Show("Por favor ingrese un número entero válido (ejemplo: 1, 2, 3).",
                            "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        ValidarCantidadDevolver(e.RowIndex);
                    }

                    RecalcularDevolucion();
                }
            }
        }

        private void ValidarCantidadDevolver(int rowIndex)
        {
            try
            {
                if (rowIndex < 0 || rowIndex >= dgvDevolucion.Rows.Count)
                    return;

                var row = dgvDevolucion.Rows[rowIndex];
                if (row == null || row.IsNewRow) return;

                if (!dgvDevolucion.Columns.Contains("CantOriginal") ||
                    !dgvDevolucion.Columns.Contains("CantDevolver"))
                    return;

                var cellCantOriginal = row.Cells["CantOriginal"];
                var cellCantDevolver = row.Cells["CantDevolver"];

                if (cellCantOriginal.Value == null || cellCantOriginal.Value == DBNull.Value)
                    return;

                if (cellCantDevolver.Value == null || cellCantDevolver.Value == DBNull.Value)
                {
                    cellCantDevolver.Value = 0;
                    return;
                }

                int cantOriginal = 0;
                int cantDevolver = 0;

                try
                {
                    cantOriginal = Convert.ToInt32(cellCantOriginal.Value);
                    cantDevolver = Convert.ToInt32(cellCantDevolver.Value);
                }
                catch
                {
                    cellCantDevolver.Value = 0;
                    return;
                }

                if (cantDevolver > cantOriginal)
                {
                    MessageBox.Show($"No puede devolver más de lo que compró.\n\n" +
                        $"Cantidad comprada: {cantOriginal} unidad(es)\n" +
                        $"Cantidad solicitada: {cantDevolver}\n\n" +
                        $"Máximo permitido: {cantOriginal}",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cellCantDevolver.Value = cantOriginal;
                }
                else if (cantDevolver < 0)
                {
                    cellCantDevolver.Value = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en ValidarCantidadDevolver: {ex.Message}");
            }
        }

        private void RecalcularDevolucion()
        {
            decimal subtotalDevolucion = 0;
            decimal ivaDevolucion = 0;

            if (dgvDevolucion.Columns.Count == 0 || dgvDevolucion.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No hay columnas o filas en el grid");
                return;
            }

            if (!dgvDevolucion.Columns.Contains("CantDevolver") ||
                !dgvDevolucion.Columns.Contains("PrecioUnitario") ||
                !dgvDevolucion.Columns.Contains("IvaItem") ||
                !dgvDevolucion.Columns.Contains("CantOriginal"))
            {
                System.Diagnostics.Debug.WriteLine("Faltan columnas necesarias");
                return;
            }

            int colCantDevolver = dgvDevolucion.Columns["CantDevolver"].Index;
            int colPrecioUnitario = dgvDevolucion.Columns["PrecioUnitario"].Index;
            int colIvaItem = dgvDevolucion.Columns["IvaItem"].Index;
            int colCantOriginal = dgvDevolucion.Columns["CantOriginal"].Index;
            int colSubtotal = dgvDevolucion.Columns.Contains("Subtotal") ? dgvDevolucion.Columns["Subtotal"].Index : -1;
            int colIva = dgvDevolucion.Columns.Contains("Iva") ? dgvDevolucion.Columns["Iva"].Index : -1;

            int itemsConDevolucion = 0;

            foreach (DataGridViewRow row in dgvDevolucion.Rows)
            {
                try
                {
                    if (row.IsNewRow) continue;
                    if (row == null) continue;

                    if (row.Cells.Count <= colCantDevolver || row.Cells.Count <= colPrecioUnitario ||
                        row.Cells.Count <= colIvaItem || row.Cells.Count <= colCantOriginal)
                        continue;

                    var cellCantDevolver = row.Cells[colCantDevolver];
                    var cellPrecioUnitario = row.Cells[colPrecioUnitario];
                    var cellIvaItem = row.Cells[colIvaItem];
                    var cellCantOriginal = row.Cells[colCantOriginal];

                    if (cellCantDevolver.Value == null || cellCantDevolver.Value == DBNull.Value ||
                        cellPrecioUnitario.Value == null || cellPrecioUnitario.Value == DBNull.Value ||
                        cellIvaItem.Value == null || cellIvaItem.Value == DBNull.Value ||
                        cellCantOriginal.Value == null || cellCantOriginal.Value == DBNull.Value)
                        continue;

                    int cantDevolver = Convert.ToInt32(cellCantDevolver.Value);

                    // MOSTRAR DEPURACIÓN
                    System.Diagnostics.Debug.WriteLine($"Fila {row.Index}: CantDevolver = {cantDevolver}");

                    if (cantDevolver <= 0) continue;

                    itemsConDevolucion++;

                    decimal preciounitario = Convert.ToDecimal(cellPrecioUnitario.Value);
                    decimal ivaItem = Convert.ToDecimal(cellIvaItem.Value);
                    int cantOriginal = Convert.ToInt32(cellCantOriginal.Value);

                    decimal ivaPorUnidad = cantOriginal > 0 ? ivaItem / cantOriginal : 0;

                    decimal subtotalItem = cantDevolver * preciounitario;
                    decimal ivaItemDevuelto = cantDevolver * ivaPorUnidad;

                    if (colSubtotal >= 0 && row.Cells.Count > colSubtotal && row.Cells[colSubtotal] != null)
                        row.Cells[colSubtotal].Value = subtotalItem;

                    if (colIva >= 0 && row.Cells.Count > colIva && row.Cells[colIva] != null)
                        row.Cells[colIva].Value = ivaItemDevuelto;

                    subtotalDevolucion += subtotalItem;
                    ivaDevolucion += ivaItemDevuelto;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error procesando fila {row.Index}: {ex.Message}");
                }
            }

            txtSubtotalDev.Text = subtotalDevolucion.ToString("C2");
            txtIVADev.Text = ivaDevolucion.ToString("C2");
            txtTotalDev.Text = (subtotalDevolucion + ivaDevolucion).ToString("C2");

            System.Diagnostics.Debug.WriteLine($"Items con devolución: {itemsConDevolucion}");
            System.Diagnostics.Debug.WriteLine($"Total calculado: {subtotalDevolucion + ivaDevolucion}");

            // MOSTRAR MENSAJE PARA DEPURACIÓN
            if (itemsConDevolucion == 0)
            {
                MessageBox.Show($"No se encontraron items con cantidad a devolver.\n\n" +
                    $"Verifique que:\n" +
                    $"1. Haya ingresado un número en la columna 'Cant. a Devolver'\n" +
                    $"2. Haya presionado ENTER después de escribir el número\n" +
                    $"3. El número sea mayor a 0",
                    "Depuración", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void BtnCalcular_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                txtMotivo.Text = cmbMotivo.SelectedItem?.ToString();
            }

            // Calcular directamente desde el DataGridView
            decimal totalDevuelto = 0;
            int filasConDevolucion = 0;

            foreach (DataGridViewRow row in dgvDevolucion.Rows)
            {
                if (row.IsNewRow) continue;

                var cellCant = row.Cells["CantDevolver"];
                var cellPrecio = row.Cells["PrecioUnitario"];

                if (cellCant != null && cellCant.Value != null && cellCant.Value != DBNull.Value &&
                    cellPrecio != null && cellPrecio.Value != null && cellPrecio.Value != DBNull.Value)
                {
                    int cant = 0;
                    int.TryParse(cellCant.Value.ToString(), out cant);

                    if (cant > 0)
                    {
                        filasConDevolucion++;
                        decimal precio = Convert.ToDecimal(cellPrecio.Value);
                        totalDevuelto += cant * precio;
                    }
                }
            }

            if (filasConDevolucion == 0 || totalDevuelto <= 0)
            {
                MessageBox.Show("Debe ingresar una cantidad mayor a 0 en la columna 'Cant. a Devolver'.\n\n" +
                    "Ejemplo: Si compró 2 unidades, ingrese 1 o 2.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnGenerarNota.Enabled = false;
                return;
            }

            // Actualizar los totales
            txtTotalDev.Text = totalDevuelto.ToString("C2");

            // Calcular subtotal y IVA (aproximado si no hay IVA por item)
            // Para simplificar, asumimos que el total es subtotal + IVA
            // Si necesitas el IVA exacto, deberías usar RecalcularDevolucion()

            RecalcularDevolucion(); // Esto actualizará subtotal e IVA

            btnGenerarNota.Enabled = true;
            btnVistaPrevia.Enabled = true;

            MessageBox.Show($"Devolución calculada correctamente.\n\n" +
                $"Total a devolver: {totalDevuelto:C2}",
                "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnGenerarNota_Click(object sender, EventArgs e)
        {
            if (facturaIdSeleccionada == null)
            {
                MessageBox.Show("Debe seleccionar una factura.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                MessageBox.Show("Debe ingresar un motivo para la nota de crédito.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalDevuelto = 0;
            decimal subtotalDevuelto = 0;
            decimal ivaDevuelto = 0;

            try
            {
                totalDevuelto = Convert.ToDecimal(txtTotalDev.Text.Replace("$", "").Replace(" ", "").Replace(",", "."),
                    System.Globalization.CultureInfo.InvariantCulture);
                subtotalDevuelto = Convert.ToDecimal(txtSubtotalDev.Text.Replace("$", "").Replace(" ", "").Replace(",", "."),
                    System.Globalization.CultureInfo.InvariantCulture);
                ivaDevuelto = Convert.ToDecimal(txtIVADev.Text.Replace("$", "").Replace(" ", "").Replace(",", "."),
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                totalDevuelto = 0;
                subtotalDevuelto = 0;
                ivaDevuelto = 0;
            }

            if (totalDevuelto <= 0)
            {
                MessageBox.Show("No hay items para devolver.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlConnection cn = null;
            SqlTransaction tx = null;

            try
            {
                cn = con.CrearConexionAbierta();
                tx = cn.BeginTransaction();

                // Generar secuencial para la nota de crédito
                secuencialGenerado = GenerarSiguienteSecuencialNotaCredito();
                claveAccesoGenerada = GenerarClaveAccesoNotaCredito();

                // Insertar nota de crédito
                string sqlNota = @"
            INSERT INTO NotasCredito
            (factura_id, usuario_id, clave_acceso, secuencial, fecha, 
             motivo_modificacion, valor_modificado_subtotal, valor_modificado_iva, total_nota_credito)
            OUTPUT INSERTED.id
            VALUES
            (@facturaId, @usuarioId, @claveAcceso, @secuencial, GETDATE(),
             @motivo, @subtotal, @iva, @total)";

                using (var cmd = new SqlCommand(sqlNota, cn, tx))
                {
                    cmd.Parameters.AddWithValue("@facturaId", facturaIdSeleccionada.Value);
                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@claveAcceso", claveAccesoGenerada);
                    cmd.Parameters.AddWithValue("@secuencial", secuencialGenerado);
                    cmd.Parameters.AddWithValue("@motivo", txtMotivo.Text.Trim());
                    cmd.Parameters.AddWithValue("@subtotal", subtotalDevuelto);
                    cmd.Parameters.AddWithValue("@iva", ivaDevuelto);
                    cmd.Parameters.AddWithValue("@total", totalDevuelto);

                    notaCreditoId = Convert.ToInt64(cmd.ExecuteScalar());
                }

                // Registrar items devueltos y devolver stock
                foreach (DataGridViewRow row in dgvDevolucion.Rows)
                {
                    if (row.IsNewRow) continue;

                    var cellCantDevolver = row.Cells["CantDevolver"];
                    if (cellCantDevolver.Value == null || cellCantDevolver.Value == DBNull.Value)
                        continue;

                    int cantDevolver = Convert.ToInt32(cellCantDevolver.Value);
                    if (cantDevolver <= 0) continue;

                    // Obtener producto_id
                    long? productoId = null;
                    if (dgvDevolucion.Columns.Contains("producto_id"))
                    {
                        var cellProducto = row.Cells["producto_id"];
                        if (cellProducto.Value != null && cellProducto.Value != DBNull.Value)
                            productoId = Convert.ToInt64(cellProducto.Value);
                    }

                    // Si es producto, devolver stock
                    if (productoId.HasValue && productoId.Value > 0)
                    {
                        // Actualizar stock
                        string sqlStock = @"UPDATE Productos SET stock = stock + @cantidad WHERE id = @productoId";
                        using (var cmd = new SqlCommand(sqlStock, cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@cantidad", cantDevolver);
                            cmd.Parameters.AddWithValue("@productoId", productoId.Value);
                            cmd.ExecuteNonQuery();
                        }

                        // Registrar en Kardex
                        string sqlKardex = @"
                    INSERT INTO Kardex (producto_id, usuario_id, tipo_movimiento, origen, referencia_id, cantidad, fecha, precio_costo)
                    VALUES (@productoId, @usuarioId, 'ENTRADA', 'NOTA_CREDITO', @notaId, @cantidad, GETDATE(), 
                            (SELECT precio_costo FROM Productos WHERE id = @productoId))";
                        using (var cmd = new SqlCommand(sqlKardex, cn, tx))
                        {
                            cmd.Parameters.AddWithValue("@productoId", productoId.Value);
                            cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                            cmd.Parameters.AddWithValue("@notaId", notaCreditoId);
                            cmd.Parameters.AddWithValue("@cantidad", cantDevolver);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                // Actualizar el estado de la factura si se devolvió todo
                if (totalDevuelto >= totalOriginal - 0.01m) // Margen de error
                {
                    string sqlUpdateFactura = @"UPDATE Facturas SET estado = 'ANULADA' WHERE id = @id";
                    using (var cmd = new SqlCommand(sqlUpdateFactura, cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@id", facturaIdSeleccionada.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                // COMMIT de la transacción
                tx.Commit();

                // Generar PDF después de commit (fuera de la transacción)
                string pdfPath = GenerarPdfNotaCredito();
                GuardarPdfEnNotaCredito(notaCreditoId, pdfPath);

                MessageBox.Show($"Nota de Crédito generada exitosamente!\n\nNúmero: {secuencialGenerado}\nTotal devuelto: {totalDevuelto:C2}",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                btnVistaPrevia.Enabled = true;
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                // Rollback solo si la transacción aún está activa
                if (tx != null)
                {
                    try
                    {
                        tx.Rollback();
                    }
                    catch { }
                }

                MessageBox.Show("Error al generar la nota de crédito: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Cerrar conexión
                if (cn != null && cn.State == ConnectionState.Open)
                {
                    cn.Close();
                    cn.Dispose();
                }
            }
        }

        private void BtnVistaPrevia_Click(object sender, EventArgs e)
        {
            try
            {
                string pdfPath = GenerarPdfNotaCredito();
                using (var visor = new FormPdfViewer(pdfPath,
                    title: "Vista previa - Nota de Crédito",
                    defaultSaveName: $"NotaCredito_{secuencialGenerado}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"))
                {
                    visor.StartPosition = FormStartPosition.CenterParent;
                    visor.WindowState = FormWindowState.Maximized;
                    visor.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la vista previa: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }

        private void LimpiarFormulario()
        {
            facturaIdSeleccionada = null;
            secuencialFactura = "";
            dtFactura = new DataTable();
            dtFacturaItems = new DataTable();

            txtNumFactura.Text = "";
            txtCliente.Text = "";
            txtFechaFactura.Text = "";
            txtSubtotal.Text = "";
            txtIVA.Text = "";
            txtTotal.Text = "";
            txtSubtotalDev.Text = "";
            txtIVADev.Text = "";
            txtTotalDev.Text = "";
            txtMotivo.Text = "";
            txtObservacion.Text = "";
            cmbMotivo.SelectedIndex = 0;

            dgvDevolucion.DataSource = null;

            txtMotivo.Enabled = false;
            txtObservacion.Enabled = false;
            btnCalcular.Enabled = false;
            btnGenerarNota.Enabled = false;
            btnVistaPrevia.Enabled = false;
        }

        private string GenerarSiguienteSecuencialNotaCredito()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    string sqlCheck = @"
                IF NOT EXISTS (SELECT 1 FROM Secuenciales WHERE tipo_documento = 'NOTA_CREDITO')
                BEGIN
                    INSERT INTO Secuenciales (tipo_documento, establecimiento, punto_emision, secuencia_actual)
                    VALUES ('NOTA_CREDITO', '001', '001', 0);
                END";

                    using (var cmdCheck = new SqlCommand(sqlCheck, cn))
                        cmdCheck.ExecuteNonQuery();

                    string sqlUpdate = @"
                DECLARE @nuevaSecuencia INT;
                UPDATE Secuenciales 
                SET @nuevaSecuencia = secuencia_actual = secuencia_actual + 1
                WHERE tipo_documento = 'NOTA_CREDITO';
                SELECT @nuevaSecuencia AS NuevaSecuencia;";

                    using (var cmdUpdate = new SqlCommand(sqlUpdate, cn))
                    {
                        var result = cmdUpdate.ExecuteScalar();
                        int nuevaSecuencia = result != null ? Convert.ToInt32(result) : 1;
                        return nuevaSecuencia.ToString().PadLeft(9, '0');
                    }
                }
            }
            catch
            {
                try
                {
                    using (var cn = con.CrearConexionAbierta())
                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CAST(secuencial AS INT)), 0) + 1 FROM NotasCredito", cn))
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

        private string GenerarClaveAccesoNotaCredito()
        {
            string baseStr = DateTime.Now.ToString("yyyyMMddHHmmss") + usuarioId.ToString().PadLeft(6, '0') + Guid.NewGuid().ToString("N");
            string digits = new string(baseStr.Where(char.IsDigit).ToArray());
            if (digits.Length < 49) digits = digits.PadRight(49, '7');
            if (digits.Length > 49) digits = digits.Substring(0, 49);
            return digits;
        }

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

        private (string nombre, string ruc, string direccion, string telefono, string email, string establecimiento, string puntoEmision) ObtenerDatosEmpresa()
        {
            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("SELECT TOP 1 nombre, ruc, direccion, telefono, email, establecimiento, punto_emision FROM Empresa WHERE id = 1;", cn))
            using (var rd = cmd.ExecuteReader())
            {
                if (!rd.Read()) return ("EMPRESA NO CONFIGURADA", "", "", "", "", "001", "001");
                return (
                    rd["nombre"]?.ToString() ?? "EMPRESA NO CONFIGURADA",
                    rd["ruc"]?.ToString() ?? "",
                    rd["direccion"]?.ToString() ?? "",
                    rd["telefono"]?.ToString() ?? "",
                    rd["email"]?.ToString() ?? "",
                    rd["establecimiento"]?.ToString()?.PadLeft(3, '0') ?? "001",
                    rd["punto_emision"]?.ToString()?.PadLeft(3, '0') ?? "001"
                );
            }
        }

        private string GenerarPdfNotaCredito()
        {
            if (notaCreditoId == 0)
                throw new Exception("No hay nota de crédito generada para exportar.");

            var empresa = ObtenerDatosEmpresa();
            byte[] logoBytes = ObtenerLogoEmpresa();

            string numeroNota = $"{empresa.establecimiento}-{empresa.puntoEmision}-{secuencialGenerado}";
            decimal subtotalDev = Convert.ToDecimal(txtSubtotalDev.Text.Replace("$", "").Replace(" ", ""));
            decimal ivaDev = Convert.ToDecimal(txtIVADev.Text.Replace("$", "").Replace(" ", ""));
            decimal totalDev = Convert.ToDecimal(txtTotalDev.Text.Replace("$", "").Replace(" ", ""));

            string carpeta = Path.Combine(Path.GetTempPath(), "TallerMecanicoERP");
            Directory.CreateDirectory(carpeta);

            string fileName = $"NotaCredito_{numeroNota.Replace("-", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string pdfPath = Path.Combine(carpeta, fileName);

            using (var fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                var doc = new Document(PageSize.A4, 36, 36, 36, 36);
                var writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var fontSub = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                var fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var font = FontFactory.GetFont(FontFactory.HELVETICA, 9);

                // Encabezado
                PdfPTable head = new PdfPTable(logoBytes != null ? 3 : 2);
                head.WidthPercentage = 100;
                if (logoBytes != null)
                    head.SetWidths(new float[] { 15, 55, 30 });
                else
                    head.SetWidths(new float[] { 70, 30 });

                if (logoBytes != null)
                {
                    var cellLogo = new PdfPCell { Border = Rectangle.NO_BORDER, Padding = 5, VerticalAlignment = Element.ALIGN_MIDDLE, HorizontalAlignment = Element.ALIGN_CENTER };
                    try
                    {
                        var logo = iTextSharp.text.Image.GetInstance(logoBytes);
                        logo.ScaleToFit(80, 80);
                        cellLogo.AddElement(logo);
                    }
                    catch
                    {
                        cellLogo.AddElement(new Paragraph("Logo no disponible", font));
                    }
                    head.AddCell(cellLogo);
                }

                var cellEmp = new PdfPCell { Border = Rectangle.NO_BORDER, Padding = 8 };
                cellEmp.AddElement(new Paragraph(empresa.nombre?.ToUpper() ?? "EMPRESA", fontHeader));
                if (!string.IsNullOrWhiteSpace(empresa.ruc))
                    cellEmp.AddElement(new Paragraph($"RUC: {empresa.ruc}", font));
                if (!string.IsNullOrWhiteSpace(empresa.direccion))
                    cellEmp.AddElement(new Paragraph($"Dirección: {empresa.direccion}", font));
                if (!string.IsNullOrWhiteSpace(empresa.telefono))
                    cellEmp.AddElement(new Paragraph($"Tel: {empresa.telefono}", font));
                if (!string.IsNullOrWhiteSpace(empresa.email))
                    cellEmp.AddElement(new Paragraph($"Email: {empresa.email}", font));
                head.AddCell(cellEmp);

                var cellNota = new PdfPCell { Border = Rectangle.NO_BORDER, Padding = 8, VerticalAlignment = Element.ALIGN_MIDDLE };
                cellNota.AddElement(new Paragraph("NOTA DE CRÉDITO", fontTitle) { Alignment = Element.ALIGN_RIGHT });
                cellNota.AddElement(new Paragraph($"No.: {numeroNota}", fontSub) { Alignment = Element.ALIGN_RIGHT });
                cellNota.AddElement(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", font) { Alignment = Element.ALIGN_RIGHT });
                head.AddCell(cellNota);

                doc.Add(head);
                doc.Add(new Paragraph(" "));

                // Información de la factura original
                PdfPTable infoFactura = new PdfPTable(2);
                infoFactura.WidthPercentage = 100;
                infoFactura.SetWidths(new float[] { 30, 70 });

                infoFactura.AddCell(new PdfPCell(new Phrase("Factura Original:", fontHeader)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase(txtNumFactura.Text, font)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase("Fecha Factura:", fontHeader)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase(txtFechaFactura.Text, font)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase("Cliente:", fontHeader)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase(txtCliente.Text, font)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase("Motivo:", fontHeader)) { Padding = 5, Border = Rectangle.BOX });
                infoFactura.AddCell(new PdfPCell(new Phrase(txtMotivo.Text, font)) { Padding = 5, Border = Rectangle.BOX });

                if (!string.IsNullOrWhiteSpace(txtObservacion.Text))
                {
                    infoFactura.AddCell(new PdfPCell(new Phrase("Observación:", fontHeader)) { Padding = 5, Border = Rectangle.BOX });
                    infoFactura.AddCell(new PdfPCell(new Phrase(txtObservacion.Text, font)) { Padding = 5, Border = Rectangle.BOX });
                }

                doc.Add(infoFactura);
                doc.Add(new Paragraph(" "));

                // Items devueltos
                PdfPTable itemsTable = new PdfPTable(5);
                itemsTable.WidthPercentage = 100;
                itemsTable.SetWidths(new float[] { 8, 8, 44, 15, 15 });

                void AddHeader(string t)
                {
                    var c = new PdfPCell(new Phrase(t, fontHeader)) { Padding = 6, BackgroundColor = BaseColor.LIGHT_GRAY };
                    itemsTable.AddCell(c);
                }

                AddHeader("Tipo");
                AddHeader("Cant");
                AddHeader("Descripción");
                AddHeader("P.Unit");
                AddHeader("Subtotal");

                foreach (DataGridViewRow row in dgvDevolucion.Rows)
                {
                    int cantDevolver = Convert.ToInt32(row.Cells["CantDevolver"].Value);
                    if (cantDevolver <= 0) continue;

                    string tipo = row.Cells["Tipo"].Value?.ToString() ?? "";
                    string nombre = row.Cells["Nombre"].Value?.ToString() ?? "";
                    decimal precioUnitario = Convert.ToDecimal(row.Cells["PrecioUnitario"].Value);
                    decimal subtotal = Convert.ToDecimal(row.Cells["Subtotal"].Value);

                    itemsTable.AddCell(new PdfPCell(new Phrase(tipo, font)) { Padding = 5 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(cantDevolver.ToString(), font)) { Padding = 5 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(nombre, font)) { Padding = 5 });
                    itemsTable.AddCell(new PdfPCell(new Phrase(precioUnitario.ToString("C2"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    itemsTable.AddCell(new PdfPCell(new Phrase(subtotal.ToString("C2"), font)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(itemsTable);
                doc.Add(new Paragraph(" "));

                // Totales
                PdfPTable tot = new PdfPTable(2);
                tot.HorizontalAlignment = Element.ALIGN_RIGHT;
                tot.WidthPercentage = 40;
                tot.SetWidths(new float[] { 60, 40 });

                tot.AddCell(new PdfPCell(new Phrase("Subtotal devuelto:", fontHeader)) { Padding = 6, Border = Rectangle.BOX });
                tot.AddCell(new PdfPCell(new Phrase(subtotalDev.ToString("C2"), font)) { Padding = 6, Border = Rectangle.BOX, HorizontalAlignment = Element.ALIGN_RIGHT });
                tot.AddCell(new PdfPCell(new Phrase("IVA devuelto:", fontHeader)) { Padding = 6, Border = Rectangle.BOX });
                tot.AddCell(new PdfPCell(new Phrase(ivaDev.ToString("C2"), font)) { Padding = 6, Border = Rectangle.BOX, HorizontalAlignment = Element.ALIGN_RIGHT });
                tot.AddCell(new PdfPCell(new Phrase("TOTAL A CREDITAR:", fontHeader)) { Padding = 6, Border = Rectangle.BOX, BackgroundColor = BaseColor.LIGHT_GRAY });
                tot.AddCell(new PdfPCell(new Phrase(totalDev.ToString("C2"), fontSub)) { Padding = 6, Border = Rectangle.BOX, HorizontalAlignment = Element.ALIGN_RIGHT, BackgroundColor = BaseColor.LIGHT_GRAY });

                doc.Add(tot);
                doc.Add(new Paragraph(" "));

                // Mensaje de nota de crédito
                PdfPTable mensaje = new PdfPTable(1);
                mensaje.WidthPercentage = 100;
                var cellMensaje = new PdfPCell(new Phrase("Este documento es una NOTA DE CRÉDITO que acredita el valor indicado.", font));
                cellMensaje.Padding = 10;
                cellMensaje.BackgroundColor = new BaseColor(240, 240, 240);
                cellMensaje.HorizontalAlignment = Element.ALIGN_CENTER;
                mensaje.AddCell(cellMensaje);
                doc.Add(mensaje);

                doc.Close();
            }

            return pdfPath;
        }

        private void GuardarPdfEnNotaCredito(long notaId, string pdfPath)
        {
            byte[] pdfBytes = File.ReadAllBytes(pdfPath);
            string nombre = Path.GetFileName(pdfPath);

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("UPDATE NotasCredito SET pdf_data = @pdf, pdf_nombre = @nom WHERE id = @id;", cn))
            {
                cmd.Parameters.Add("@pdf", SqlDbType.VarBinary, -1).Value = pdfBytes;
                cmd.Parameters.AddWithValue("@nom", nombre);
                cmd.Parameters.AddWithValue("@id", notaId);
                cmd.ExecuteNonQuery();
            }
        }
    }
}