using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using PROYECTOMECANICO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing;
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
    public partial class FormRepClientes : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable _dtClientes = new DataTable();

        public FormRepClientes()
        {
            InitializeComponent();

            this.Load += FormRepClientes_Load;
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

        private void FormRepClientes_Load(object sender, EventArgs e)
        {
            dtpHasta.Value = DateTime.Today;
            AjustarDesdeAlMesActual();

            EstilizarGrid();
            InicializarChart();
            CargarReporte();
        }

        private void EstilizarGrid()
        {
            dgvClientes.AutoGenerateColumns = true;
            dgvClientes.ColumnHeadersVisible = true;
            dgvClientes.EnableHeadersVisualStyles = false;
            dgvClientes.ColumnHeadersHeight = 36;
            dgvClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvClientes.RowHeadersVisible = false;
            dgvClientes.AllowUserToAddRows = false;
            dgvClientes.AllowUserToResizeRows = false;
            dgvClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClientes.MultiSelect = false;
            dgvClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvClientes.ScrollBars = ScrollBars.Both;
            dgvClientes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvClientes.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void InicializarChart()
        {
            ChartClientes.Datasets.Clear();
            ChartClientes.Legend.Display = true;
            ChartClientes.Legend.Position = Guna.Charts.WinForms.LegendPosition.Top;
            ChartClientes.Title.Text = "Clientes registrados por mes";
            ChartClientes.XAxes.GridLines.Display = true;
            ChartClientes.YAxes.GridLines.Display = true;
            ChartClientes.YAxes.Ticks.BeginAtZero = true;
        }

        private void CargarReporte()
        {
            try
            {
                string buscar = (txtBuscar.Text ?? "").Trim();

                string sqlGrid = @"
SELECT
    id,
    tipo_documento AS [Tipo Doc],
    numero_documento AS [Número Doc],
    nombre AS [Nombre],
    direccion AS [Dirección],
    telefono AS [Teléfono],
    email AS [Email],
    CASE WHEN contribuyente_especial = 1 THEN 'Sí' ELSE 'No' END AS [Contribuyente Especial]
FROM Clientes
WHERE
    @buscar = '' OR
    nombre LIKE '%' + @buscar + '%' OR
    numero_documento LIKE '%' + @buscar + '%' OR
    email LIKE '%' + @buscar + '%' OR
    telefono LIKE '%' + @buscar + '%'
ORDER BY id DESC;";

                // Consulta corregida para el resumen
                string sqlResumen = @"
SELECT 
    COUNT(*) AS TotalClientes,
    SUM(CASE WHEN v.cliente_id IS NOT NULL THEN 1 ELSE 0 END) AS ConVehiculos,
    SUM(CASE WHEN v.cliente_id IS NULL THEN 1 ELSE 0 END) AS SinVehiculos
FROM Clientes c
LEFT JOIN (
    SELECT DISTINCT cliente_id FROM Vehiculos
) v ON c.id = v.cliente_id
WHERE
    @buscar = '' OR
    c.nombre LIKE '%' + @buscar + '%' OR
    c.numero_documento LIKE '%' + @buscar + '%' OR
    c.email LIKE '%' + @buscar + '%' OR
    c.telefono LIKE '%' + @buscar + '%';";

                using (var cn = con.CrearConexionAbierta())
                {
                    using (var cmd = new SqlCommand(sqlGrid, cn))
                    {
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            _dtClientes = new DataTable();
                            da.Fill(_dtClientes);
                            dgvClientes.DataSource = _dtClientes;

                            dgvClientes.ColumnHeadersVisible = true;
                            dgvClientes.Refresh();

                            // Ajustar anchos de columnas
                            if (dgvClientes.Columns["Tipo Doc"] != null) dgvClientes.Columns["Tipo Doc"].Width = 90;
                            if (dgvClientes.Columns["Número Doc"] != null) dgvClientes.Columns["Número Doc"].Width = 120;
                            if (dgvClientes.Columns["Nombre"] != null) dgvClientes.Columns["Nombre"].Width = 200;
                            if (dgvClientes.Columns["Dirección"] != null) dgvClientes.Columns["Dirección"].Width = 200;
                            if (dgvClientes.Columns["Teléfono"] != null) dgvClientes.Columns["Teléfono"].Width = 100;
                            if (dgvClientes.Columns["Email"] != null) dgvClientes.Columns["Email"].Width = 150;
                            if (dgvClientes.Columns["Contribuyente Especial"] != null) dgvClientes.Columns["Contribuyente Especial"].Width = 120;
                        }
                    }

                    using (var cmd = new SqlCommand(sqlResumen, cn))
                    {
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                int total = rd["TotalClientes"] == DBNull.Value ? 0 : Convert.ToInt32(rd["TotalClientes"]);
                                int conVehiculos = rd["ConVehiculos"] == DBNull.Value ? 0 : Convert.ToInt32(rd["ConVehiculos"]);
                                int sinVehiculos = rd["SinVehiculos"] == DBNull.Value ? 0 : Convert.ToInt32(rd["SinVehiculos"]);

                                lblTotalClientes.Text = total.ToString();
                                lblConVehiculos.Text = conVehiculos.ToString();
                                lblSinVehiculos.Text = sinVehiculos.ToString();
                            }
                        }
                    }
                }

                FormatearColumnas();

                // Actualizar el gráfico con los datos reales
                CargarGraficoResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reporte:\n" + ex.Message, "Reporte de clientes",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGraficoResumen()
        {
            ChartClientes.Datasets.Clear();

            var ds = new Guna.Charts.WinForms.GunaBarDataset
            {
                Label = "Distribución de clientes",
                BorderWidth = 2
            };

            // Usar los datos de los labels
            int total = int.Parse(lblTotalClientes.Text);
            int conVehiculos = int.Parse(lblConVehiculos.Text);
            int sinVehiculos = int.Parse(lblSinVehiculos.Text);

            ds.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Con Vehículos", Y = conVehiculos });
            ds.DataPoints.Add(new Guna.Charts.WinForms.LPoint { Label = "Sin Vehículos", Y = sinVehiculos });

            ChartClientes.Datasets.Add(ds);
            ChartClientes.Update();
        }
        private void FormatearColumnas()
        {
            if (dgvClientes.Columns.Count == 0) return;

            // Ocultar columna ID si existe
            if (dgvClientes.Columns["id"] != null)
                dgvClientes.Columns["id"].Visible = false;
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

            // Título
            Paragraph titulo = new Paragraph("REPORTE DE CLIENTES", titleFont);
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 10;
            doc.Add(titulo);

            // Filtros
            string filtros = "Listado general de clientes";
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                filtros += $" | Búsqueda: {txtBuscar.Text}";

            Paragraph rango = new Paragraph(filtros, bodyFont);
            rango.Alignment = Element.ALIGN_CENTER;
            rango.SpacingAfter = 20;
            doc.Add(rango);

            // ===== GRÁFICO DE BARRAS =====
            Paragraph chartTitle = new Paragraph("DISTRIBUCIÓN DE CLIENTES", subtitleFont);
            chartTitle.SpacingAfter = 10;
            doc.Add(chartTitle);

            try
            {
                // Crear datos para el gráfico
                DataTable dtGrafico = new DataTable();
                dtGrafico.Columns.Add("Categoria", typeof(string));
                dtGrafico.Columns.Add("Total", typeof(int));

                dtGrafico.Rows.Add("Con Vehículos", int.Parse(lblConVehiculos.Text));
                dtGrafico.Rows.Add("Sin Vehículos", int.Parse(lblSinVehiculos.Text));

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

            dataTable.AddCell(new PdfPCell(new Phrase("Con Vehículos", bodyFont)) { Padding = 4 });
            dataTable.AddCell(new PdfPCell(new Phrase(lblConVehiculos.Text, bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });

            dataTable.AddCell(new PdfPCell(new Phrase("Sin Vehículos", bodyFont)) { Padding = 4 });
            dataTable.AddCell(new PdfPCell(new Phrase(lblSinVehiculos.Text, bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });

            doc.Add(dataTable);

            // ===== TABLA DE DETALLE =====
            Paragraph tableTitle = new Paragraph("LISTADO DE CLIENTES", subtitleFont);
            tableTitle.SpacingAfter = 10;
            doc.Add(tableTitle);

            var visibleCols = dgvClientes.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            PdfPTable table = new PdfPTable(visibleCols);
            table.WidthPercentage = 100;
            table.SpacingBefore = 5;

            foreach (DataGridViewColumn col in dgvClientes.Columns)
            {
                if (!col.Visible) continue;
                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, headerFont));
                cell.BackgroundColor = new BaseColor(230, 230, 230);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            int rowCount = 0;
            foreach (DataGridViewRow row in dgvClientes.Rows)
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
                PdfPCell emptyCell = new PdfPCell(new Phrase("No hay clientes para mostrar", bodyFont));
                emptyCell.Colspan = visibleCols;
                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                emptyCell.Padding = 10;
                table.AddCell(emptyCell);
            }

            doc.Add(table);

            // Resumen final
            doc.Add(new Paragraph(" "));
            Paragraph resumenFinal = new Paragraph(
                $"Total Clientes: {lblTotalClientes.Text}   |   " +
                $"Con Vehículos: {lblConVehiculos.Text}   |   " +
                $"Sin Vehículos: {lblSinVehiculos.Text}",
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

                    for (int i = 0; i < numPuntos; i++)
                    {
                        double valor = Convert.ToDouble(dtGrafico.Rows[i]["Total"]);
                        float x = marginLeft + (i * graphWidth / numPuntos) + barWidth;
                        float barHeight = (float)((valor / maxValor) * graphHeight);
                        float y = marginTop + graphHeight - barHeight;

                        using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(97, 101, 244)))
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
                if (_dtClientes == null || _dtClientes.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Exportar",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string tempPdf = Path.Combine(Path.GetTempPath(),
                    $"ReporteClientes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                GenerarPDF(tempPdf);

                var visor = new FormPdfViewer(
                    tempPdf,
                    title: "Vista previa - Reporte de Clientes",
                    defaultSaveName: $"ReporteClientes_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
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
            txtBuscar.Text = "";
            CargarReporte();
        }
    }
}