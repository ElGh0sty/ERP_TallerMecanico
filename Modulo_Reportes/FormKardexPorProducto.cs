using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

using System.Linq;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using PROYECTOMECANICO;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
// Alias para resolver ambigüedades
using GdiPen = System.Drawing.Pen;
using GdiFont = System.Drawing.Font;
using GdiBrush = System.Drawing.SolidBrush;
using GdiGraphics = System.Drawing.Graphics;
using GdiImage = System.Drawing.Image;
using GdiBitmap = System.Drawing.Bitmap;
using GdiColor = System.Drawing.Color;
using GdiPointF = System.Drawing.PointF;

namespace PROYECTOMECANICO.Modulo_Reportes
{
    public partial class FormKardexPorProducto : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable _dtMovimientos = new DataTable();
        private DataTable _dtProductos = new DataTable();
        private long? _productoSeleccionadoId = null;
        private string _productoSeleccionadoNombre = null;

        public FormKardexPorProducto()
        {
            InitializeComponent();
            DataGridViewEstilo.AplicarEstiloDashboard(dgvKardex);
            this.Load += FormKardexPorProducto_Load;
            btnBuscar.Click += (s, e) => CargarKardex();
            btnLimpiar.Click += (s, e) => LimpiarFiltros();
            btnExportar.Click += (s, e) => PrevisualizarPDF();
            dtpHasta.ValueChanged += (s, e) => AjustarDesdeAlMesDeHasta();
            txtBuscarProducto.TextChanged += FiltrarProductos;
            txtBuscarProducto.KeyDown += txtBuscarProducto_KeyDown;
            lstProductos.DoubleClick += (s, e) => ConfirmarProductoSeleccionado();
            lstProductos.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    ConfirmarProductoSeleccionado();
                }
            };
        }

        private void FormKardexPorProducto_Load(object sender, EventArgs e)
        {
            var hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHasta.Value = hoy;

            ConfigurarFiltros();
            CargarProductosBuscador();
            EstilizarGrid();
        }

        private void ConfigurarFiltros()
        {
            cmbTipo.Items.Clear();
            cmbTipo.Items.Add("TODOS");
            cmbTipo.Items.Add("ENTRADA");
            cmbTipo.Items.Add("SALIDA");
            cmbTipo.SelectedIndex = 0;
        }

        private void AjustarDesdeAlMesDeHasta()
        {
            var h = dtpHasta.Value;
            var inicioMes = new DateTime(h.Year, h.Month, 1);
            if (dtpDesde.Value.Year != inicioMes.Year || dtpDesde.Value.Month != inicioMes.Month)
                dtpDesde.Value = inicioMes;
        }

        private void CargarProductosBuscador()
        {
            try
            {
                con.Abrir();
                string sql = @"
                    SELECT id, nombre, tipo, ISNULL(stock,0) AS stock,
                           CONCAT(nombre, '  |  ', tipo, '  |  Stock: ', ISNULL(stock,0)) AS display
                    FROM Productos
                    ORDER BY nombre;";

                _dtProductos.Clear();
                using (var da = new SqlDataAdapter(sql, con.leer))
                {
                    da.Fill(_dtProductos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando productos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void FiltrarProductos(object sender, EventArgs e)
        {
            string texto = txtBuscarProducto.Text.Trim().Replace("'", "''");
            if (texto.Length < 1)
            {
                lstProductos.Visible = false;
                lstProductos.DataSource = null;
                return;
            }

            var dv = new DataView(_dtProductos);
            dv.RowFilter = $"nombre LIKE '%{texto}%' OR tipo LIKE '%{texto}%'";

            if (dv.Count == 0)
            {
                lstProductos.Visible = false;
                lstProductos.DataSource = null;
                return;
            }

            lstProductos.DisplayMember = "display";
            lstProductos.ValueMember = "id";
            lstProductos.DataSource = dv;

            lstProductos.Visible = true;
            lstProductos.Left = txtBuscarProducto.Left;
            lstProductos.Top = txtBuscarProducto.Bottom + 2;
            lstProductos.Width = txtBuscarProducto.Width;
            lstProductos.BringToFront();
            lstProductos.Height = Math.Min(200, (dv.Count * 22) + 4);
        }

        private void ConfirmarProductoSeleccionado()
        {
            if (lstProductos.SelectedItem == null) return;
            DataRowView row = (DataRowView)lstProductos.SelectedItem;

            _productoSeleccionadoId = Convert.ToInt64(row["id"]);
            _productoSeleccionadoNombre = row["nombre"].ToString();
            txtBuscarProducto.Text = _productoSeleccionadoNombre;
            lstProductos.Visible = false;
            lblProductoSel.Text = $"Producto: {_productoSeleccionadoNombre}";
            CargarKardex();
        }

        private void txtBuscarProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (!lstProductos.Visible) return;

            if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                lstProductos.Focus();
                if (lstProductos.Items.Count > 0) lstProductos.SelectedIndex = 0;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                lstProductos.Visible = false;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ConfirmarProductoSeleccionado();
            }
        }

        private void LimpiarFiltros()
        {
            var hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHasta.Value = hoy;
            cmbTipo.SelectedIndex = 0;
            _productoSeleccionadoId = null;
            _productoSeleccionadoNombre = null;
            txtBuscarProducto.Clear();
            lstProductos.Visible = false;
            lblProductoSel.Text = "Producto: (No seleccionado)";
            _dtMovimientos.Clear();
            dgvKardex.DataSource = null;
        }

        private void CargarKardex()
        {
            if (_productoSeleccionadoId == null)
            {
                MessageBox.Show("Debe seleccionar un producto.", "Kardex por Producto",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1);
                string tipoFiltro = cmbTipo.SelectedItem.ToString();

                string sql = @"
            SELECT
                k.fecha AS Fecha,
                k.tipo_movimiento AS Tipo,
                k.cantidad AS Cantidad,
                p.precio_costo AS CostoUnitario,  -- ← NUEVO: Traer el costo del producto
                (k.cantidad * p.precio_costo) AS CostoTotalMov, -- ← NUEVO: Calcular costo total del movimiento
                SUM(k.cantidad) OVER (PARTITION BY k.producto_id ORDER BY k.fecha, k.id ROWS UNBOUNDED PRECEDING) AS StockResultante,
                CASE 
                    WHEN k.origen = 'COMPRA' THEN CONCAT('Compra #', k.referencia_id)
                    WHEN k.origen = 'VENTA' THEN CONCAT('Factura #', (SELECT secuencial FROM Facturas WHERE id = k.referencia_id))
                    ELSE k.origen
                END AS Documento,
                u.nombre_usuario AS Usuario
            FROM Kardex k
            INNER JOIN Productos p ON k.producto_id = p.id  -- ← Aseguramos el JOIN
            INNER JOIN Usuarios u ON k.usuario_id = u.id
            WHERE k.producto_id = @productoId
              AND k.fecha >= @desde AND k.fecha < @hasta
              AND (@tipo = 'TODOS' OR k.tipo_movimiento = @tipo)
            ORDER BY k.fecha DESC, k.id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@productoId", _productoSeleccionadoId.Value);
                    cmd.Parameters.AddWithValue("@desde", desde);
                    cmd.Parameters.AddWithValue("@hasta", hasta);
                    cmd.Parameters.AddWithValue("@tipo", tipoFiltro);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        _dtMovimientos = new DataTable();
                        da.Fill(_dtMovimientos);
                        dgvKardex.DataSource = _dtMovimientos;
                    }
                }

                lblTotal.Text = $"Movimientos: {_dtMovimientos.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Kardex: " + ex.Message);
            }
        }

        private void EstilizarGrid()
        {
            dgvKardex.AutoGenerateColumns = true;
            dgvKardex.ReadOnly = true;
            dgvKardex.AllowUserToAddRows = false;
            dgvKardex.AllowUserToDeleteRows = false;
            dgvKardex.AllowUserToResizeRows = false;
            dgvKardex.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvKardex.MultiSelect = false;
            dgvKardex.RowHeadersVisible = false;
            dgvKardex.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvKardex.BorderStyle = BorderStyle.None;
            dgvKardex.EnableHeadersVisualStyles = false;

            // Estilo de encabezados
            dgvKardex.ColumnHeadersDefaultCellStyle.BackColor = GdiColor.FromArgb(24, 24, 28);
            dgvKardex.ColumnHeadersDefaultCellStyle.ForeColor = GdiColor.White;
            dgvKardex.ColumnHeadersDefaultCellStyle.Font = new GdiFont("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            dgvKardex.ColumnHeadersHeight = 40;

            // Estilo de celdas
            dgvKardex.DefaultCellStyle.Font = new GdiFont("Segoe UI", 10F);
            dgvKardex.DefaultCellStyle.ForeColor = GdiColor.FromArgb(35, 35, 35);
            dgvKardex.DefaultCellStyle.BackColor = GdiColor.White;
            dgvKardex.AlternatingRowsDefaultCellStyle.BackColor = GdiColor.FromArgb(247, 247, 250);
            dgvKardex.DefaultCellStyle.SelectionBackColor = GdiColor.FromArgb(220, 235, 255);
            dgvKardex.DefaultCellStyle.SelectionForeColor = GdiColor.FromArgb(10, 10, 10);
            dgvKardex.RowTemplate.Height = 38;
            dgvKardex.GridColor = GdiColor.FromArgb(230, 230, 230);
        }

        // ==================== LÓGICA DE REPORTE PDF (Adaptada de FormRepVentas) ====================

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
            catch { /* Silently fail */ }
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

        private void PrevisualizarPDF()
        {
            if (_productoSeleccionadoId == null)
            {
                MessageBox.Show("Debe seleccionar un producto para generar el reporte.", "Kardex por Producto",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dtMovimientos == null || _dtMovimientos.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar con los filtros actuales.", "Exportar",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                string tempPdf = Path.Combine(Path.GetTempPath(),
                    $"Kardex_{_productoSeleccionadoNombre}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                GenerarPDF(tempPdf);

                var visor = new FormPdfViewer(
                    tempPdf,
                    title: "Vista previa - Kardex por Producto",
                    defaultSaveName: $"Kardex_{_productoSeleccionadoNombre}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
                );

                visor.StartPosition = FormStartPosition.CenterParent;
                visor.WindowState = FormWindowState.Maximized;
                visor.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en vista previa:\n" + ex.Message, "PDF",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GenerarPDF(string filePath)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            var empresa = ObtenerEmpresa();
            byte[] logoBytes = ObtenerLogoEmpresa();

            // ===== ENCABEZADO (igual que antes) =====
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
                    cellLogo.AddElement(new Paragraph("Logo no disponible", bodyFont));
                }
                head.AddCell(cellLogo);
            }

            var cellEmpresa = new PdfPCell { Border = Rectangle.NO_BORDER, Padding = 5 };
            cellEmpresa.AddElement(new Paragraph(empresa.nombre?.ToUpper() ?? "EMPRESA", headerFont));
            if (!string.IsNullOrWhiteSpace(empresa.ruc)) cellEmpresa.AddElement(new Paragraph($"RUC: {empresa.ruc}", bodyFont));
            if (!string.IsNullOrWhiteSpace(empresa.direccion)) cellEmpresa.AddElement(new Paragraph(empresa.direccion, bodyFont));
            if (!string.IsNullOrWhiteSpace(empresa.telefono)) cellEmpresa.AddElement(new Paragraph($"Tel: {empresa.telefono}", bodyFont));
            if (!string.IsNullOrWhiteSpace(empresa.email)) cellEmpresa.AddElement(new Paragraph(empresa.email, bodyFont));
            head.AddCell(cellEmpresa);

            var cellTitulo = new PdfPCell { Border = Rectangle.NO_BORDER, Padding = 5, VerticalAlignment = Element.ALIGN_MIDDLE };
            cellTitulo.AddElement(new Paragraph("KARDEX POR\nPRODUCTO", titleFont) { Alignment = Element.ALIGN_RIGHT });
            head.AddCell(cellTitulo);
            doc.Add(head);
            doc.Add(new Paragraph(" "));

            // ===== FILTROS APLICADOS =====
            string filtros = $"Producto: {_productoSeleccionadoNombre} | Desde: {dtpDesde.Value:dd/MM/yyyy} - Hasta: {dtpHasta.Value:dd/MM/yyyy} | Tipo: {cmbTipo.SelectedItem}";
            Paragraph rango = new Paragraph(filtros, bodyFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 };
            doc.Add(rango);

            // ===== GRÁFICO DE STOCK Y EVOLUCIÓN DE COSTO (Opcional: podrías hacer dos gráficos) =====
            DataTable dtGrafico = ObtenerDatosGraficoStock();
            if (dtGrafico.Rows.Count > 0)
            {
                Paragraph chartTitle = new Paragraph("EVOLUCIÓN DEL STOCK Y COSTO", subtitleFont) { SpacingAfter = 10 };
                doc.Add(chartTitle);
                try
                {
                    using (var chartImage = GenerarGraficoStockConDrawing(dtGrafico))
                    {
                        if (chartImage != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                chartImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0;
                                iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                                img.ScaleToFit(700, 300);
                                img.Alignment = Element.ALIGN_CENTER;
                                img.SpacingAfter = 20;
                                doc.Add(img);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    doc.Add(new Paragraph("No se pudo generar la imagen del gráfico: " + ex.Message, bodyFont) { SpacingAfter = 20 });
                }
            }
            else
            {
                doc.Add(new Paragraph("No hay datos suficientes para generar el gráfico de stock.", bodyFont) { SpacingAfter = 20 });
            }

            // ===== TABLA DE MOVIMIENTOS (AHORA CON COSTOS) =====
            Paragraph tableTitle = new Paragraph("DETALLE DE MOVIMIENTOS", subtitleFont) { SpacingAfter = 10 };
            doc.Add(tableTitle);

            // Definir las columnas que queremos mostrar
            var columnasAMostrar = new[] { "Fecha", "Tipo", "Cantidad", "CostoUnitario", "CostoTotalMov", "StockResultante", "Documento", "Usuario" };
            int visibleCols = columnasAMostrar.Length;

            PdfPTable table = new PdfPTable(visibleCols) { WidthPercentage = 100, SpacingBefore = 5 };

            // Encabezados personalizados
            table.AddCell(new PdfPCell(new Phrase("Fecha", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Tipo", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Cantidad", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Costo Unit.", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Costo Total", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Stock", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Documento", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Usuario", headerFont)) { BackgroundColor = new BaseColor(230, 230, 230), HorizontalAlignment = Element.ALIGN_CENTER, Padding = 5 });

            int rowCount = 0;
            foreach (DataRow row in _dtMovimientos.Rows)
            {
                if (rowCount++ >= 50) break; // Límite de filas para el PDF

                table.AddCell(new PdfPCell(new Phrase(Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"), bodyFont)) { Padding = 4 });
                table.AddCell(new PdfPCell(new Phrase(row["Tipo"].ToString(), bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                table.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(row["Cantidad"]).ToString("N0"), bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(row["CostoUnitario"]).ToString("C2"), bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(row["CostoTotalMov"]).ToString("C2"), bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(Convert.ToDecimal(row["StockResultante"]).ToString("N0"), bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });
                table.AddCell(new PdfPCell(new Phrase(row["Documento"].ToString(), bodyFont)) { Padding = 4 });
                table.AddCell(new PdfPCell(new Phrase(row["Usuario"].ToString(), bodyFont)) { Padding = 4 });
            }

            if (rowCount == 0)
            {
                PdfPCell emptyCell = new PdfPCell(new Phrase("No hay movimientos en el período seleccionado", bodyFont))
                {
                    Colspan = visibleCols,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 10
                };
                table.AddCell(emptyCell);
            }
            doc.Add(table);

            // ===== RESUMEN CON VALOR DEL INVENTARIO =====
            var resumen = CalcularResumen();
            doc.Add(new Paragraph(" "));
            Paragraph resumenFinal = new Paragraph(
                $"Stock Actual: {resumen.stockActual:N0}   |   " +
                $"Valor del Inventario: {resumen.valorTotalInventario:C2}   |   " +  // ← NUEVO
                $"Total Entradas: {resumen.totalEntradas:N0}   |   " +
                $"Total Salidas: {resumen.totalSalidas:N0}",
                headerFont)
            { Alignment = Element.ALIGN_RIGHT, SpacingBefore = 15 };
            doc.Add(resumenFinal);

            doc.Close();
        }

        private GdiImage GenerarGraficoStockConDrawing(DataTable dtGrafico)
        {
            if (dtGrafico == null || dtGrafico.Rows.Count == 0)
            {
                GdiBitmap bmpNoData = new GdiBitmap(800, 400);
                using (GdiGraphics g = GdiGraphics.FromImage(bmpNoData))
                {
                    g.Clear(GdiColor.White);
                    g.DrawString("No hay datos suficientes para el gráfico de stock.",
                        new GdiFont("Arial", 14, System.Drawing.FontStyle.Bold),
                        System.Drawing.Brushes.Gray, new System.Drawing.PointF(200, 180));
                }
                return bmpNoData;
            }

            try
            {
                int width = 800, height = 400;
                int marginLeft = 80, marginRight = 60, marginTop = 50, marginBottom = 70;
                int graphWidth = width - marginLeft - marginRight;
                int graphHeight = height - marginTop - marginBottom;

                if (graphWidth <= 0 || graphHeight <= 0)
                {
                    graphWidth = 600;
                    graphHeight = 250;
                }

                GdiBitmap bmp = new GdiBitmap(width, height);
                using (GdiGraphics g = GdiGraphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.Clear(GdiColor.White);

                    // ===== CALCULAR VALORES MÁXIMOS PARA AMBAS ESCALAS =====
                    double maxStock = dtGrafico.AsEnumerable().Max(r => Convert.ToDouble(r["Stock"]));
                    double maxCosto = dtGrafico.AsEnumerable().Max(r => Convert.ToDouble(r["CostoUnitario"]));

                    // Ajustar para dar un poco de espacio arriba
                    if (maxStock <= 0) maxStock = 100;
                    else maxStock = Math.Ceiling(maxStock * 1.1 / 10) * 10;

                    if (maxCosto <= 0) maxCosto = 10;
                    else maxCosto = Math.Ceiling(maxCosto * 1.1);

                    // ===== ÁREA DEL GRÁFICO =====
                    using (var brush = new GdiBrush(GdiColor.FromArgb(250, 250, 255)))
                        g.FillRectangle(brush, marginLeft, marginTop, graphWidth, graphHeight);

                    using (var pen = new System.Drawing.Pen(GdiColor.FromArgb(200, 200, 200)))
                        g.DrawRectangle(pen, marginLeft, marginTop, graphWidth, graphHeight);

                    // ===== LÍNEAS DE GRID HORIZONTALES =====
                    using (var pen = new System.Drawing.Pen(GdiColor.FromArgb(220, 220, 220))
                    { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                    {
                        int numLineas = 5;
                        for (int i = 0; i <= numLineas; i++)
                        {
                            float y = marginTop + (graphHeight * i / numLineas);
                            g.DrawLine(pen, marginLeft, y, marginLeft + graphWidth, y);

                            // Etiquetas del eje Y (Stock - lado izquierdo)
                            double valorStock = maxStock * (1 - (double)i / numLineas);
                            g.DrawString(valorStock.ToString("N0"), new GdiFont("Arial", 8),
                                System.Drawing.Brushes.Gray, marginLeft - 50, y - 6);

                            // Etiquetas del eje Y secundario (Costo - lado derecho)
                            double valorCosto = maxCosto * (1 - (double)i / numLineas);
                            g.DrawString(valorCosto.ToString("C0"), new GdiFont("Arial", 8),
                                System.Drawing.Brushes.Gray, marginLeft + graphWidth + 5, y - 6);
                        }
                    }

                    // ===== EJES =====
                    using (var pen = new System.Drawing.Pen(GdiColor.Black, 2))
                    {
                        g.DrawLine(pen, marginLeft, marginTop, marginLeft, marginTop + graphHeight); // Eje Y izquierdo
                        g.DrawLine(pen, marginLeft + graphWidth, marginTop, marginLeft + graphWidth, marginTop + graphHeight); // Eje Y derecho
                        g.DrawLine(pen, marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight); // Eje X
                    }

                    // ===== DIBUJAR LÍNEA DE STOCK =====
                    int numPuntos = dtGrafico.Rows.Count;
                    float pasoX = graphWidth / (float)(Math.Max(numPuntos - 1, 1));

                    if (numPuntos > 1)
                    {
                        GdiPointF[] puntosStock = new GdiPointF[numPuntos];
                        for (int i = 0; i < numPuntos; i++)
                        {
                            double stock = Convert.ToDouble(dtGrafico.Rows[i]["Stock"]);
                            float x = marginLeft + i * pasoX;
                            float y = marginTop + graphHeight - (float)((stock / maxStock) * graphHeight);
                            puntosStock[i] = new GdiPointF(x, y);
                        }

                        // Línea de stock (azul)
                        using (var pen = new System.Drawing.Pen(GdiColor.FromArgb(97, 101, 244), 3))
                            g.DrawLines(pen, puntosStock);

                        // Puntos de stock
                        using (var brush = new GdiBrush(GdiColor.FromArgb(97, 101, 244)))
                        using (var whitePen = new System.Drawing.Pen(GdiColor.White, 2))
                        {
                            foreach (var punto in puntosStock)
                            {
                                g.FillEllipse(brush, punto.X - 5, punto.Y - 5, 10, 10);
                                g.DrawEllipse(whitePen, punto.X - 5, punto.Y - 5, 10, 10);
                            }
                        }
                    }

                    // ===== DIBUJAR LÍNEA DE COSTO =====
                    if (numPuntos > 1)
                    {
                        GdiPointF[] puntosCosto = new GdiPointF[numPuntos];
                        for (int i = 0; i < numPuntos; i++)
                        {
                            double costo = Convert.ToDouble(dtGrafico.Rows[i]["CostoUnitario"]);
                            float x = marginLeft + i * pasoX;
                            float y = marginTop + graphHeight - (float)((costo / maxCosto) * graphHeight);
                            puntosCosto[i] = new GdiPointF(x, y);
                        }

                        // Línea de costo (verde) - discontinua para distinguirla
                        using (var pen = new System.Drawing.Pen(GdiColor.FromArgb(76, 175, 80), 2))
                        {
                            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                            g.DrawLines(pen, puntosCosto);
                        }

                        // Puntos de costo
                        using (var brush = new GdiBrush(GdiColor.FromArgb(76, 175, 80)))
                        using (var whitePen = new System.Drawing.Pen(GdiColor.White, 2))
                        {
                            foreach (var punto in puntosCosto)
                            {
                                g.FillEllipse(brush, punto.X - 4, punto.Y - 4, 8, 8);
                                g.DrawEllipse(whitePen, punto.X - 4, punto.Y - 4, 8, 8);
                            }
                        }

                        // Etiquetas de valores de costo (solo para algunos puntos para no saturar)
                        using (var valorFont = new GdiFont("Arial", 7))
                        {
                            for (int i = 0; i < numPuntos; i += Math.Max(1, numPuntos / 5))
                            {
                                double costo = Convert.ToDouble(dtGrafico.Rows[i]["CostoUnitario"]);
                                g.DrawString(costo.ToString("C0"), valorFont, System.Drawing.Brushes.DarkGreen,
                                    puntosCosto[i].X - 20, puntosCosto[i].Y - 25);
                            }
                        }
                    }

                    // ===== LEYENDA =====
                    int leyendaX = marginLeft + 10;
                    int leyendaY = marginTop + 10;

                    // Leyenda de Stock
                    using (var brush = new GdiBrush(GdiColor.FromArgb(97, 101, 244)))
                        g.FillRectangle(brush, leyendaX, leyendaY, 20, 10);
                    g.DrawString("Stock", new GdiFont("Arial", 9), System.Drawing.Brushes.Black, leyendaX + 25, leyendaY - 2);

                    // Leyenda de Costo
                    using (var brush = new GdiBrush(GdiColor.FromArgb(76, 175, 80)))
                        g.FillRectangle(brush, leyendaX + 80, leyendaY, 20, 10);
                    g.DrawString("Costo Unit.", new GdiFont("Arial", 9), System.Drawing.Brushes.Black, leyendaX + 105, leyendaY - 2);

                    // ===== ETIQUETAS DE LOS EJES =====
                    using (var axisFont = new GdiFont("Arial", 9, System.Drawing.FontStyle.Bold))
                    {
                        g.DrawString("Stock (unidades)", axisFont, System.Drawing.Brushes.Black, marginLeft - 70, marginTop - 20);
                        g.DrawString("Costo ($)", axisFont, System.Drawing.Brushes.Black, marginLeft + graphWidth + 10, marginTop - 20);
                    }

                    // ===== ETIQUETAS DEL EJE X =====
                    using (var fechaFont = new GdiFont("Arial", 8))
                    {
                        for (int i = 0; i < numPuntos; i += Math.Max(1, numPuntos / 6))
                        {
                            string fecha = dtGrafico.Rows[i]["Fecha"].ToString();
                            float x = marginLeft + i * pasoX;
                            g.DrawString(fecha, fechaFont, System.Drawing.Brushes.Black,
                                x - 20, marginTop + graphHeight + 5);
                        }
                    }

                    // ===== TÍTULO DEL GRÁFICO =====
                    using (var titleFont = new GdiFont("Arial", 12, System.Drawing.FontStyle.Bold))
                        g.DrawString("Evolución del Stock y Costo Unitario", titleFont,
                            System.Drawing.Brushes.Black, width / 2 - 130, 10);

                    // ===== INFORMACIÓN ADICIONAL =====
                    using (var infoFont = new GdiFont("Arial", 8))
                    {
                        string info = $"Stock máximo: {maxStock:N0} unidades | Costo máximo: {maxCosto:C2} | Período: {dtpDesde.Value:dd/MM/yyyy} - {dtpHasta.Value:dd/MM/yyyy}";
                        g.DrawString(info, infoFont, System.Drawing.Brushes.Gray, 20, height - 20);
                    }
                }

                return bmp;
            }
            catch (Exception ex)
            {
                GdiBitmap bmpError = new GdiBitmap(800, 400);
                using (GdiGraphics g = GdiGraphics.FromImage(bmpError))
                {
                    g.Clear(GdiColor.White);
                    g.DrawString($"Error al generar gráfico: {ex.Message}",
                        new GdiFont("Arial", 10), System.Drawing.Brushes.Red,
                        new System.Drawing.PointF(50, 180));
                }
                return bmpError;
            }
        }

        private DataTable ObtenerDatosGraficoStock()
        {
            DataTable dt = new DataTable();
            if (_productoSeleccionadoId == null) return dt;

            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    string sql = @"
                SELECT TOP 20
                    CONCAT(DATEPART(day, k.fecha), '/', DATEPART(month, k.fecha)) AS Fecha,
                    SUM(k.cantidad) OVER (PARTITION BY k.producto_id ORDER BY k.fecha, k.id ROWS UNBOUNDED PRECEDING) AS Stock,
                    p.precio_costo AS CostoUnitario
                FROM Kardex k
                INNER JOIN Productos p ON k.producto_id = p.id
                WHERE k.producto_id = @productoId
                  AND k.fecha >= @desde AND k.fecha < @hasta
                ORDER BY k.fecha;";

                    using (var cmd = new SqlCommand(sql, cn))
                    {
                        cmd.Parameters.AddWithValue("@productoId", _productoSeleccionadoId.Value);
                        cmd.Parameters.AddWithValue("@desde", dtpDesde.Value.Date);
                        cmd.Parameters.AddWithValue("@hasta", dtpHasta.Value.Date.AddDays(1));

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener datos para gráfico: " + ex.Message);
            }
            return dt;
        }

        private (decimal stockActual, decimal totalEntradas, decimal totalSalidas, decimal valorTotalInventario) CalcularResumen()
        {
            decimal entradas = 0, salidas = 0;
            decimal stockActual = 0;
            decimal costoUnitarioActual = 0;

            // Necesitamos el último costo unitario para calcular el valor del inventario
            if (_dtMovimientos.Rows.Count > 0)
            {
                // Tomamos el costo de la última fila (la más reciente, ya que el grid está ordenado DESC)
                // Pero para el stock actual, usamos el stock de la primera fila (la más reciente)
                DataRow ultimaFila = _dtMovimientos.Rows[0];
                stockActual = Convert.ToDecimal(ultimaFila["StockResultante"]);

                // Buscamos el costo unitario del producto (de cualquier fila, debería ser el mismo)
                // Usamos la columna CostoUnitario que agregamos
                costoUnitarioActual = Convert.ToDecimal(ultimaFila["CostoUnitario"]);
            }

            // Calculamos totales de entradas y salidas
            foreach (DataRow row in _dtMovimientos.Rows)
            {
                string tipo = row["Tipo"].ToString();
                decimal cant = Convert.ToDecimal(row["Cantidad"]);
                if (tipo == "ENTRADA") entradas += cant;
                else if (tipo == "SALIDA") salidas += cant;
            }

            decimal valorTotalInventario = stockActual * costoUnitarioActual;

            return (stockActual, entradas, salidas, valorTotalInventario);
        }

        private void FormKardexPorProducto_Load_1(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvKardex);

        }






        // ==================== FIN LÓGICA PDF ====================
    }
}