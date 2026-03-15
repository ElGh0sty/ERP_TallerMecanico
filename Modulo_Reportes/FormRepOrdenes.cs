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
    public partial class FormRepOrdenes : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable _dtOrdenes = new DataTable();

        public FormRepOrdenes()
        {
            InitializeComponent();

            this.Load += FormRepOrdenes_Load;
            btnBuscar.Click += (s, e) => CargarReporte();
            btnLimpiar.Click += (s, e) => LimpiarFiltros();
            btnExportar.Click += (s, e) => PrevisualizarPDF();
            dtpHasta.ValueChanged += (s, e) => AjustarDesdeAlMesDeHasta();
        }

        private void AjustarDesdeAlMesActual()
        {
            var hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
        }

        private void AjustarDesdeAlMesDeHasta()
        {
            var h = dtpHasta.Value;
            var inicioMes = new DateTime(h.Year, h.Month, 1);

            if (dtpDesde.Value.Year != inicioMes.Year || dtpDesde.Value.Month != inicioMes.Month)
                dtpDesde.Value = inicioMes;
        }

        private void FormRepOrdenes_Load(object sender, EventArgs e)
        {
            dtpHasta.Value = DateTime.Today;
            AjustarDesdeAlMesActual();

            EstilizarGrid();
            InicializarChart();
            CargarReporte();
        }

        private void EstilizarGrid()
        {
            dgvOrdenes.AutoGenerateColumns = true;
            dgvOrdenes.ColumnHeadersVisible = true;
            dgvOrdenes.EnableHeadersVisualStyles = false;
            dgvOrdenes.ColumnHeadersHeight = 36;
            dgvOrdenes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvOrdenes.RowHeadersVisible = false;
            dgvOrdenes.AllowUserToAddRows = false;
            dgvOrdenes.AllowUserToResizeRows = false;
            dgvOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenes.MultiSelect = false;
            dgvOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvOrdenes.ScrollBars = ScrollBars.Both;
            dgvOrdenes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvOrdenes.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void InicializarChart()
        {
            ChartOrdenes.Datasets.Clear();
            ChartOrdenes.Legend.Display = true;
            ChartOrdenes.Legend.Position = Guna.Charts.WinForms.LegendPosition.Top;
            ChartOrdenes.Title.Text = "Órdenes por estado";
            ChartOrdenes.XAxes.GridLines.Display = true;
            ChartOrdenes.YAxes.GridLines.Display = true;
            ChartOrdenes.YAxes.Ticks.BeginAtZero = true;
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
                // Si hay error, simplemente no mostramos logo
                Console.WriteLine("Error al obtener logo: " + ex.Message);
            }
            return null;
        }
        private void CargarReporte()
        {
            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1);
                string estado = cbEstado.SelectedItem?.ToString() ?? "Todos";
                string buscar = (txtBuscar.Text ?? "").Trim();

                string sqlGrid = @"
SELECT
    ot.id AS [N° OT],
    ot.fecha_ingreso AS [Fecha Ingreso],
    c.nombre AS [Cliente],
    v.placa AS [Vehículo],
    u.nombre_usuario AS [Mecánico],
    ot.estado AS [Estado],
    CASE WHEN ot.facturada = 1 THEN 'Sí' ELSE 'No' END AS [Facturada],
    ot.descripcion AS [Descripción]
FROM OrdenesTrabajo ot
INNER JOIN Vehiculos v ON v.id = ot.vehiculo_id
INNER JOIN Clientes c ON c.id = v.cliente_id
INNER JOIN Usuarios u ON u.id = ot.mecanico_id
WHERE
    ot.fecha_ingreso >= @desde AND ot.fecha_ingreso < @hasta
    AND (@estado = 'Todos' OR ot.estado = @estado)
    AND (
        @buscar = '' OR
        c.nombre LIKE '%' + @buscar + '%' OR
        v.placa LIKE '%' + @buscar + '%' OR
        ot.descripcion LIKE '%' + @buscar + '%' OR
        CAST(ot.id AS NVARCHAR) LIKE '%' + @buscar + '%'
    )
ORDER BY ot.fecha_ingreso DESC, ot.id DESC;";

                string sqlResumen = @"
SELECT
    COUNT(*) AS TotalOrdenes,
    SUM(CASE WHEN ot.estado IN ('Terminado', 'Entregado') THEN 1 ELSE 0 END) AS Completadas,
    SUM(CASE WHEN ot.estado NOT IN ('Terminado', 'Entregado') THEN 1 ELSE 0 END) AS Pendientes,
    SUM(CASE WHEN ot.facturada = 1 THEN 1 ELSE 0 END) AS Facturadas
FROM OrdenesTrabajo ot
WHERE
    ot.fecha_ingreso >= @desde AND ot.fecha_ingreso < @hasta
    AND (@estado = 'Todos' OR ot.estado = @estado)
    AND (
        @buscar = '' OR
        EXISTS (SELECT 1 FROM Vehiculos v INNER JOIN Clientes c ON c.id = v.cliente_id 
                WHERE v.id = ot.vehiculo_id AND (c.nombre LIKE '%' + @buscar + '%' OR v.placa LIKE '%' + @buscar + '%'))
        OR ot.descripcion LIKE '%' + @buscar + '%'
        OR CAST(ot.id AS NVARCHAR) LIKE '%' + @buscar + '%'
    );";

                using (var cn = con.CrearConexionAbierta())
                {
                    using (var cmd = new SqlCommand(sqlGrid, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        cmd.Parameters.AddWithValue("@estado", estado);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            _dtOrdenes = new DataTable();
                            da.Fill(_dtOrdenes);
                            dgvOrdenes.DataSource = _dtOrdenes;

                            dgvOrdenes.ColumnHeadersVisible = true;
                            dgvOrdenes.Refresh();

                            // Ajustar anchos de columnas
                            if (dgvOrdenes.Columns["N° OT"] != null) dgvOrdenes.Columns["N° OT"].Width = 80;
                            if (dgvOrdenes.Columns["Fecha Ingreso"] != null) dgvOrdenes.Columns["Fecha Ingreso"].Width = 120;
                            if (dgvOrdenes.Columns["Cliente"] != null) dgvOrdenes.Columns["Cliente"].Width = 180;
                            if (dgvOrdenes.Columns["Vehículo"] != null) dgvOrdenes.Columns["Vehículo"].Width = 100;
                            if (dgvOrdenes.Columns["Mecánico"] != null) dgvOrdenes.Columns["Mecánico"].Width = 100;
                            if (dgvOrdenes.Columns["Estado"] != null) dgvOrdenes.Columns["Estado"].Width = 120;
                            if (dgvOrdenes.Columns["Facturada"] != null) dgvOrdenes.Columns["Facturada"].Width = 80;
                            if (dgvOrdenes.Columns["Descripción"] != null) dgvOrdenes.Columns["Descripción"].Width = 250;
                        }
                    }

                    using (var cmd = new SqlCommand(sqlResumen, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        cmd.Parameters.AddWithValue("@estado", estado);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                int total = rd["TotalOrdenes"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TotalOrdenes"]);
                                int completadas = rd["Completadas"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Completadas"]);
                                int pendientes = rd["Pendientes"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Pendientes"]);
                                int facturadas = rd["Facturadas"] == DBNull.Value ? 0 : Convert.ToInt32(rd["Facturadas"]);

                                lblTotalOrdenes.Text = total.ToString();
                                lblCompletadas.Text = completadas.ToString();
                                lblPendientes.Text = pendientes.ToString();
                                lblFacturadas.Text = facturadas.ToString();
                            }
                        }
                    }
                }

                FormatearColumnas();
                CargarGraficoEstados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reporte:\n" + ex.Message, "Reporte de órdenes",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGraficoEstados()
        {
            ChartOrdenes.Datasets.Clear();

            var ds = new Guna.Charts.WinForms.GunaBarDataset
            {
                Label = "Órdenes por estado",
                BorderWidth = 2
            };

            // Usar los datos de los labels
            int total = int.Parse(lblTotalOrdenes.Text);
            int completadas = int.Parse(lblCompletadas.Text);
            int pendientes = int.Parse(lblPendientes.Text);
            int facturadas = int.Parse(lblFacturadas.Text);

            ds.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Completadas", Y = completadas });
            ds.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Pendientes", Y = pendientes });
            ds.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Facturadas", Y = facturadas });

            ChartOrdenes.Datasets.Add(ds);
            ChartOrdenes.Update();
        }

        private void FormatearColumnas()
        {
            if (dgvOrdenes.Columns.Count == 0) return;

            // Formato de fecha
            if (dgvOrdenes.Columns["Fecha Ingreso"] != null)
                dgvOrdenes.Columns["Fecha Ingreso"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        }

        private void GenerarPDF(string filePath)
        {
            Document doc = new Document(PageSize.A4, 20, 20, 30, 30);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var subtitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            // Obtener datos de la empresa y logo
            var empresa = ObtenerEmpresa();
            byte[] logoBytes = ObtenerLogoEmpresa();

            // ===== ENCABEZADO CON LOGO =====
            PdfPTable head = new PdfPTable(logoBytes != null ? 3 : 2);
            head.WidthPercentage = 100;
            if (logoBytes != null)
                head.SetWidths(new float[] { 20, 50, 30 }); // Logo | Info Empresa | Título
            else
                head.SetWidths(new float[] { 70, 30 }); // Info Empresa | Título

            // Celda del Logo (si existe)
            if (logoBytes != null)
            {
                var cellLogo = new PdfPCell();
                cellLogo.Border = Rectangle.NO_BORDER;
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
                    cellLogo.AddElement(new Paragraph("Logo no disponible", bodyFont));
                }
                head.AddCell(cellLogo);
            }

            // Celda Información de la Empresa
            var cellEmpresa = new PdfPCell();
            cellEmpresa.Border = Rectangle.NO_BORDER;
            cellEmpresa.Padding = 5;
            cellEmpresa.AddElement(new Paragraph(empresa.nombre?.ToUpper() ?? "EMPRESA", headerFont));
            if (!string.IsNullOrWhiteSpace(empresa.ruc))
                cellEmpresa.AddElement(new Paragraph($"RUC: {empresa.ruc}", bodyFont));
            if (!string.IsNullOrWhiteSpace(empresa.direccion))
                cellEmpresa.AddElement(new Paragraph(empresa.direccion, bodyFont));
            if (!string.IsNullOrWhiteSpace(empresa.telefono))
                cellEmpresa.AddElement(new Paragraph($"Tel: {empresa.telefono}", bodyFont));
            if (!string.IsNullOrWhiteSpace(empresa.email))
                cellEmpresa.AddElement(new Paragraph(empresa.email, bodyFont));
            head.AddCell(cellEmpresa);

            // Celda Título del Reporte
            var cellTitulo = new PdfPCell();
            cellTitulo.Border = Rectangle.NO_BORDER;
            cellTitulo.Padding = 5;
            cellTitulo.VerticalAlignment = Element.ALIGN_MIDDLE;
            cellTitulo.AddElement(new Paragraph("REPORTE DE\nÓRDENES", titleFont) { Alignment = Element.ALIGN_RIGHT });
            head.AddCell(cellTitulo);

            doc.Add(head);
            doc.Add(new Paragraph(" "));

            // Filtros
            string filtros = $"Desde: {dtpDesde.Value:dd/MM/yyyy} - Hasta: {dtpHasta.Value:dd/MM/yyyy}";
            if (cbEstado.SelectedItem != null && cbEstado.SelectedIndex > 0)
                filtros += $" | Estado: {cbEstado.Text}";
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                filtros += $" | Búsqueda: {txtBuscar.Text}";

            Paragraph rango = new Paragraph(filtros, bodyFont);
            rango.Alignment = Element.ALIGN_CENTER;
            rango.SpacingAfter = 20;
            doc.Add(rango);

            // ===== GRÁFICO DE BARRAS =====
            Paragraph chartTitle = new Paragraph("DISTRIBUCIÓN DE ÓRDENES", subtitleFont);
            chartTitle.SpacingAfter = 10;
            doc.Add(chartTitle);

            try
            {
                // Crear datos para el gráfico
                DataTable dtGrafico = new DataTable();
                dtGrafico.Columns.Add("Categoria", typeof(string));
                dtGrafico.Columns.Add("Total", typeof(int));

                dtGrafico.Rows.Add("Completadas", int.Parse(lblCompletadas.Text));
                dtGrafico.Rows.Add("Pendientes", int.Parse(lblPendientes.Text));
                dtGrafico.Rows.Add("Facturadas", int.Parse(lblFacturadas.Text));

                // Crear imagen del gráfico
                using (var chartImage = GenerarGraficoBarras(dtGrafico))
                {
                    if (chartImage != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            chartImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            ms.Position = 0;

                            iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(ms.ToArray());
                            img.ScaleToFit(500, 300);
                            img.Alignment = Element.ALIGN_CENTER;
                            img.SpacingAfter = 20;
                            doc.Add(img);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Paragraph errorImg = new Paragraph("No se pudo generar la imagen del gráfico: " + ex.Message, bodyFont);
                errorImg.SpacingAfter = 20;
                doc.Add(errorImg);
            }

            // Tabla de datos del gráfico
            PdfPTable dataTable = new PdfPTable(2);
            dataTable.WidthPercentage = 60;
            dataTable.HorizontalAlignment = Element.ALIGN_CENTER;
            dataTable.SpacingAfter = 20;

            PdfPCell headerCategoria = new PdfPCell(new Phrase("Categoría", headerFont));
            headerCategoria.BackgroundColor = new BaseColor(230, 230, 230);
            headerCategoria.Padding = 5;
            dataTable.AddCell(headerCategoria);

            PdfPCell headerTotal = new PdfPCell(new Phrase("Cantidad", headerFont));
            headerTotal.BackgroundColor = new BaseColor(230, 230, 230);
            headerTotal.Padding = 5;
            dataTable.AddCell(headerTotal);

            dataTable.AddCell(new PdfPCell(new Phrase("Completadas", bodyFont)) { Padding = 4 });
            dataTable.AddCell(new PdfPCell(new Phrase(lblCompletadas.Text, bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });

            dataTable.AddCell(new PdfPCell(new Phrase("Pendientes", bodyFont)) { Padding = 4 });
            dataTable.AddCell(new PdfPCell(new Phrase(lblPendientes.Text, bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });

            dataTable.AddCell(new PdfPCell(new Phrase("Facturadas", bodyFont)) { Padding = 4 });
            dataTable.AddCell(new PdfPCell(new Phrase(lblFacturadas.Text, bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });

            doc.Add(dataTable);

            // ===== TABLA DE DETALLE =====
            Paragraph tableTitle = new Paragraph("LISTADO DE ÓRDENES", subtitleFont);
            tableTitle.SpacingAfter = 10;
            doc.Add(tableTitle);

            var visibleCols = dgvOrdenes.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            PdfPTable table = new PdfPTable(visibleCols);
            table.WidthPercentage = 100;
            table.SpacingBefore = 5;

            foreach (DataGridViewColumn col in dgvOrdenes.Columns)
            {
                if (!col.Visible) continue;
                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, headerFont));
                cell.BackgroundColor = new BaseColor(230, 230, 230);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            int rowCount = 0;
            foreach (DataGridViewRow row in dgvOrdenes.Rows)
            {
                if (row.IsNewRow) continue;
                if (rowCount++ >= 50) break;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!cell.Visible) continue;

                    string texto = cell.Value?.ToString() ?? "";
                    PdfPCell pdfCell = new PdfPCell(new Phrase(texto, bodyFont));
                    pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pdfCell.Padding = 4;
                    table.AddCell(pdfCell);
                }
            }

            if (rowCount == 0)
            {
                PdfPCell emptyCell = new PdfPCell(new Phrase("No hay órdenes en el período seleccionado", bodyFont));
                emptyCell.Colspan = visibleCols;
                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                emptyCell.Padding = 10;
                table.AddCell(emptyCell);
            }

            doc.Add(table);

            // Resumen final
            doc.Add(new Paragraph(" "));
            Paragraph resumenFinal = new Paragraph(
                $"Total Órdenes: {lblTotalOrdenes.Text}   |   " +
                $"Completadas: {lblCompletadas.Text}   |   " +
                $"Pendientes: {lblPendientes.Text}   |   " +
                $"Facturadas: {lblFacturadas.Text}",
                headerFont);
            resumenFinal.Alignment = Element.ALIGN_RIGHT;
            resumenFinal.SpacingBefore = 15;
            doc.Add(resumenFinal);

            doc.Close();
        }

        private GdiImage GenerarGraficoBarras(DataTable dtGrafico)
        {
            try
            {
                int width = 600;
                int height = 400;
                int marginLeft = 100;
                int marginRight = 60;
                int marginTop = 50;
                int marginBottom = 70;
                int graphWidth = width - marginLeft - marginRight;
                int graphHeight = height - marginTop - marginBottom;

                GdiBitmap bmp = new GdiBitmap(width, height);
                using (GdiGraphics g = GdiGraphics.FromImage(bmp))
                {
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    g.Clear(GdiColor.White);

                    // Encontrar valor máximo
                    double maxValor = 0;
                    foreach (DataRow row in dtGrafico.Rows)
                    {
                        double valor = Convert.ToDouble(row["Total"]);
                        if (valor > maxValor) maxValor = valor;
                    }
                    maxValor = Math.Ceiling(maxValor * 1.2);

                    // Dibujar fondo
                    using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(250, 250, 255)))
                    {
                        g.FillRectangle(brush, marginLeft, marginTop, graphWidth, graphHeight);
                    }

                    // Dibujar grid
                    using (GdiPen pen = new GdiPen(GdiColor.FromArgb(220, 220, 220)))
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        int numLineas = 5;
                        for (int i = 0; i <= numLineas; i++)
                        {
                            float y = marginTop + (graphHeight * i / numLineas);
                            g.DrawLine(pen, marginLeft, y, marginLeft + graphWidth, y);

                            double valor = maxValor * (1 - (double)i / numLineas);
                            g.DrawString(valor.ToString("0"), new GdiFont("Arial", 8),
                                System.Drawing.Brushes.Gray, marginLeft - 40, y - 6);
                        }
                    }

                    // Dibujar ejes
                    using (GdiPen pen = new GdiPen(GdiColor.Black, 2))
                    {
                        g.DrawLine(pen, marginLeft, marginTop, marginLeft, marginTop + graphHeight);
                        g.DrawLine(pen, marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight);
                    }

                    // Dibujar barras
                    int numPuntos = dtGrafico.Rows.Count;
                    float barWidth = graphWidth / (float)(numPuntos * 2);
                    System.Drawing.Color[] colores = new System.Drawing.Color[]
                    {
                        System.Drawing.Color.FromArgb(6, 215, 155),   // Verde para completadas
                        System.Drawing.Color.FromArgb(255, 89, 129),  // Rosa para pendientes
                        System.Drawing.Color.FromArgb(97, 101, 244)   // Azul para facturadas
                    };

                    for (int i = 0; i < numPuntos; i++)
                    {
                        double valor = Convert.ToDouble(dtGrafico.Rows[i]["Total"]);
                        float x = marginLeft + (i * graphWidth / numPuntos) + barWidth;
                        float barHeight = (float)((valor / maxValor) * graphHeight);
                        float y = marginTop + graphHeight - barHeight;

                        using (GdiBrush brush = new GdiBrush(colores[i % colores.Length]))
                        {
                            g.FillRectangle(brush, x, y, barWidth, barHeight);
                        }

                        string categoria = dtGrafico.Rows[i]["Categoria"].ToString();
                        g.DrawString(categoria, new GdiFont("Arial", 9),
                            System.Drawing.Brushes.Black, x - 10, marginTop + graphHeight + 5);

                        g.DrawString(valor.ToString("0"), new GdiFont("Arial", 9, System.Drawing.FontStyle.Bold),
                            System.Drawing.Brushes.Black, x + 5, y - 15);
                    }
                }
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        private void PrevisualizarPDF()
        {
            try
            {
                if (_dtOrdenes == null || _dtOrdenes.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Exportar",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string tempPdf = Path.Combine(Path.GetTempPath(),
                    $"ReporteOrdenes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                GenerarPDF(tempPdf);

                var visor = new FormPdfViewer(
                    tempPdf,
                    title: "Vista previa - Reporte de Órdenes",
                    defaultSaveName: $"ReporteOrdenes_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
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

        private void LimpiarFiltros()
        {
            var hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
            dtpHasta.Value = hoy;
            cbEstado.SelectedIndex = 0;
            txtBuscar.Text = "";
            CargarReporte();
        }
    }
}
