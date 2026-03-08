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

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormRepVentas : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable _dtVentas = new DataTable();
        private long? clienteSeleccionadoId = null;

        public FormRepVentas()
        {
            InitializeComponent();

            this.Load += FormRepVentas_Load;
            btnBuscar.Click += (s, e) => CargarReporte();
            btnLimpiar.Click += (s, e) => LimpiarFiltros();
            btnExportar.Click += (s, e) => PrevisualizarPDF();
            dtpHasta.ValueChanged += (s, e) => AjustarDesdeAlMesDeHasta();
            dgvVentas.CellDoubleClick += dgvVentas_CellDoubleClick;
        }

        private void AjustarDesdeAlMesActual()
        {
            var hoy = DateTime.Today;
            dtpDesde.Value = new DateTime(hoy.Year, hoy.Month, 1);
        }

        private void dgvVentas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvVentas.Rows[e.RowIndex];
            if (row.DataBoundItem is DataRowView drv)
            {
                if (!drv.Row.Table.Columns.Contains("FacturaID"))
                {
                    MessageBox.Show("El DataSource no trae FacturaID.");
                    return;
                }

                long facturaId = Convert.ToInt64(drv["FacturaID"]);
                AbrirPdfFacturaDesdeBd(facturaId);
            }
        }

        private void AbrirPdfFacturaDesdeBd(long facturaId)
        {
            byte[] pdf;
            string nombre;

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("SELECT pdf_data, pdf_nombre FROM Facturas WHERE id=@id;", cn))
            {
                cmd.Parameters.AddWithValue("@id", facturaId);

                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        throw new Exception("No existe la factura " + facturaId);

                    if (rd["pdf_data"] == DBNull.Value)
                        throw new Exception("Esta factura no tiene PDF guardado.");

                    pdf = (byte[])rd["pdf_data"];
                    nombre = rd["pdf_nombre"]?.ToString();
                    if (string.IsNullOrWhiteSpace(nombre)) nombre = $"Factura_{facturaId}.pdf";
                }
            }

            string carpeta = Path.Combine(Path.GetTempPath(), "TallerMecanicoERP");
            Directory.CreateDirectory(carpeta);

            string pdfPath = Path.Combine(carpeta, nombre);
            File.WriteAllBytes(pdfPath, pdf);

            using (var visor = new FormPdfViewer(
                pdfPath,
                title: "Vista previa - Factura",
                defaultSaveName: nombre))
            {
                visor.StartPosition = FormStartPosition.CenterParent;
                visor.WindowState = FormWindowState.Maximized;
                visor.ShowDialog(this);
            }
        }

        private void AjustarDesdeAlMesDeHasta()
        {
            var h = dtpHasta.Value;
            var inicioMes = new DateTime(h.Year, h.Month, 1);

            if (dtpDesde.Value.Year != inicioMes.Year || dtpDesde.Value.Month != inicioMes.Month)
                dtpDesde.Value = inicioMes;
        }

        private void FormRepVentas_Load(object sender, EventArgs e)
        {
            var hoy = DateTime.Today;
            dtpHasta.Value = DateTime.Today;
            AjustarDesdeAlMesActual();

            EstilizarGrid();
            InicializarChart();
            CargarCombos();
            CargarReporte();
        }

        private void CargarCombos()
        {
            using (var cn = con.CrearConexionAbierta())
            {
                var dtU = new DataTable();
                using (var da = new SqlDataAdapter("SELECT id, nombre_usuario FROM Usuarios ORDER BY nombre_usuario", cn))
                    da.Fill(dtU);

                var filaTodosU = dtU.NewRow();
                filaTodosU["id"] = 0;
                filaTodosU["nombre_usuario"] = "Todos";
                dtU.Rows.InsertAt(filaTodosU, 0);

                cbUsuario.DisplayMember = "nombre_usuario";
                cbUsuario.ValueMember = "id";
                cbUsuario.DataSource = dtU;
                cbUsuario.SelectedIndex = 0;
            }
        }

        private void btnBuscadorCliente_Click(object sender, EventArgs e)
        {
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Clientes);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long clienteId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();

                clienteSeleccionadoId = clienteId;
                txtBuscarCliente.Text = nombre;
                CargarReporte();
            }
        }

        private void CargarReporte()
        {
            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1);

                long usuarioId = Convert.ToInt64(cbUsuario.SelectedValue ?? 0);
                string buscar = (txtBuscar.Text ?? "").Trim();

                CargarGraficoVentasSemanal(desde, hasta, clienteSeleccionadoId, usuarioId, buscar);

                string sqlGrid = @"
SELECT
    f.id AS FacturaID,
    f.fecha AS Fecha,
    f.secuencial AS Secuencial,
    c.nombre AS Cliente,
    u.nombre_usuario AS Usuario,
    f.subtotal_15_iva AS Subtotal15,
    f.subtotal_0_iva AS Subtotal0,
    f.valor_iva AS IVA,
    ISNULL(f.total_descuento, 0) AS Descuento,
    f.total_final AS Total,
    f.estado AS Estado
FROM Facturas f
INNER JOIN Clientes c ON c.id = f.cliente_id
INNER JOIN Usuarios u ON u.id = f.usuario_id
WHERE
    f.fecha >= @desde AND f.fecha < @hasta
    AND (@clienteId = 0 OR f.cliente_id = @clienteId)
    AND (@usuarioId = 0 OR f.usuario_id = @usuarioId)
    AND (
        @buscar = '' OR
        f.secuencial LIKE '%' + @buscar + '%' OR
        f.clave_acceso LIKE '%' + @buscar + '%' OR
        ISNULL(f.numero_autorizacion,'') LIKE '%' + @buscar + '%'
    )
ORDER BY f.fecha DESC, f.id DESC;";

                string sqlResumen = @"
SELECT
    COUNT(*) AS CantFacturas,
    SUM(f.total_final) AS TotalVentas,
    SUM(f.valor_iva) AS TotalIVA,
    SUM(ISNULL(f.total_descuento, 0)) AS TotalDesc
FROM Facturas f
WHERE
    f.fecha >= @desde AND f.fecha < @hasta
    AND (@clienteId = 0 OR f.cliente_id = @clienteId)
    AND (@usuarioId = 0 OR f.usuario_id = @usuarioId)
    AND (
        @buscar = '' OR
        f.secuencial LIKE '%' + @buscar + '%' OR
        f.clave_acceso LIKE '%' + @buscar + '%' OR
        ISNULL(f.numero_autorizacion,'') LIKE '%' + @buscar + '%'
    );";

                using (var cn = con.CrearConexionAbierta())
                {
                    using (var cmd = new SqlCommand(sqlGrid, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        cmd.Parameters.AddWithValue("@clienteId", clienteSeleccionadoId ?? 0);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            _dtVentas = new DataTable();
                            da.Fill(_dtVentas);
                            dgvVentas.DataSource = _dtVentas;

                            dgvVentas.ColumnHeadersVisible = true;
                            dgvVentas.Refresh();

                            if (dgvVentas.Columns["Fecha"] != null) dgvVentas.Columns["Fecha"].Width = 140;
                            if (dgvVentas.Columns["Secuencial"] != null) dgvVentas.Columns["Secuencial"].Width = 90;
                            if (dgvVentas.Columns["Cliente"] != null) dgvVentas.Columns["Cliente"].Width = 220;
                            if (dgvVentas.Columns["Usuario"] != null) dgvVentas.Columns["Usuario"].Width = 90;
                            if (dgvVentas.Columns["Subtotal15"] != null) dgvVentas.Columns["Subtotal15"].Width = 110;
                            if (dgvVentas.Columns["IVA"] != null) dgvVentas.Columns["IVA"].Width = 90;
                            if (dgvVentas.Columns["Descuento"] != null) dgvVentas.Columns["Descuento"].Width = 100;
                            if (dgvVentas.Columns["Total"] != null) dgvVentas.Columns["Total"].Width = 110;
                            if (dgvVentas.Columns["Estado"] != null) dgvVentas.Columns["Estado"].Width = 90;
                        }
                    }

                    using (var cmd = new SqlCommand(sqlResumen, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        cmd.Parameters.AddWithValue("@clienteId", clienteSeleccionadoId ?? 0);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                int cant = rd["CantFacturas"] == DBNull.Value ? 0 : Convert.ToInt32(rd["CantFacturas"]);
                                decimal totalVentas = rd["TotalVentas"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["TotalVentas"]);
                                decimal totalIva = rd["TotalIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["TotalIVA"]);
                                decimal totalDesc = rd["TotalDesc"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["TotalDesc"]);

                                lblCantFacturas.Text = cant.ToString();
                                lblTotalVentas.Text = totalVentas.ToString("C2", CultureInfo.GetCultureInfo("es-EC"));
                                lblTotalIVA.Text = totalIva.ToString("C2", CultureInfo.GetCultureInfo("es-EC"));
                                lblTotalDesc.Text = totalDesc.ToString("C2", CultureInfo.GetCultureInfo("es-EC"));
                            }
                        }
                    }
                }

                FormatearColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reporte:\n" + ex.Message, "Reporte de ventas",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EstilizarGrid()
        {
            dgvVentas.AutoGenerateColumns = true;
            dgvVentas.ColumnHeadersVisible = true;
            dgvVentas.EnableHeadersVisualStyles = false;
            dgvVentas.ColumnHeadersHeight = 36;
            dgvVentas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvVentas.RowHeadersVisible = false;
            dgvVentas.AllowUserToAddRows = false;
            dgvVentas.AllowUserToResizeRows = false;
            dgvVentas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvVentas.MultiSelect = false;
            dgvVentas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvVentas.ScrollBars = ScrollBars.Both;
            dgvVentas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvVentas.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void FormatearColumnas()
        {
            if (dgvVentas.Columns.Count == 0) return;

            OcultarCol("FacturaID");
            FormatoFecha("Fecha");
            FormatoMoneda("Subtotal15");
            FormatoMoneda("Subtotal0");
            FormatoMoneda("IVA");
            FormatoMoneda("Descuento");
            FormatoMoneda("Total");
            Renombrar("Subtotal15", "Subtotal 15%");
            Renombrar("Subtotal0", "Subtotal 0%");
        }

        private void OcultarCol(string col)
        {
            if (dgvVentas.Columns[col] != null) dgvVentas.Columns[col].Visible = false;
        }

        private void Renombrar(string col, string header)
        {
            if (dgvVentas.Columns[col] != null) dgvVentas.Columns[col].HeaderText = header;
        }

        private void FormatoFecha(string col)
        {
            if (dgvVentas.Columns[col] != null) dgvVentas.Columns[col].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        }

        private void FormatoMoneda(string col)
        {
            if (dgvVentas.Columns[col] != null) dgvVentas.Columns[col].DefaultCellStyle.Format = "N2";
        }

        private void InicializarChart()
        {
            ChartVentas.Datasets.Clear();
            ChartVentas.Legend.Display = true;
            ChartVentas.Legend.Position = Guna.Charts.WinForms.LegendPosition.Top;
            ChartVentas.Title.Text = "Ventas por semana";
            

            ChartVentas.XAxes.GridLines.Display = true;
            ChartVentas.YAxes.GridLines.Display = true;
            ChartVentas.YAxes.Ticks.BeginAtZero = true;
        }

        private void CargarGraficoVentasSemanal(DateTime desde, DateTime hasta, long? clienteId, long usuarioId, string buscar)
        {
            ChartVentas.Datasets.Clear();

            // Usar el tipo correcto de Guna.UI2
            var ds = new Guna.Charts.WinForms.GunaBarDataset
            {
                Label = "Ventas por semana",
                BorderWidth = 2
            };

            string sqlSemanal = @"
SELECT
    CONCAT('Semana ', DATEPART(week, f.fecha)) AS Semana,
    SUM(f.total_final) AS Total
FROM Facturas f
WHERE
    f.fecha >= @desde AND f.fecha < @hasta
    AND (@clienteId = 0 OR f.cliente_id = @clienteId)
    AND (@usuarioId = 0 OR f.usuario_id = @usuarioId)
    AND (
        @buscar = '' OR
        f.secuencial LIKE '%' + @buscar + '%' OR
        f.clave_acceso LIKE '%' + @buscar + '%' OR
        ISNULL(f.numero_autorizacion,'') LIKE '%' + @buscar + '%'
    )
GROUP BY YEAR(f.fecha), DATEPART(week, f.fecha)
ORDER BY YEAR(f.fecha), DATEPART(week, f.fecha);";

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(sqlSemanal, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                cmd.Parameters.AddWithValue("@clienteId", clienteId ?? 0);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@buscar", buscar);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string semana = rd["Semana"].ToString();
                        double total = rd["Total"] == DBNull.Value ? 0 : Convert.ToDouble(rd["Total"]);

                        ds.DataPoints.Add(new Guna.Charts.WinForms.LPoint
                        {
                            Label = semana,
                            Y = total
                        });
                    }
                }
            }

            if (ds.DataPoints.Count > 0)
            {
                ChartVentas.Datasets.Add(ds);
            }
            ChartVentas.Update();
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
            Paragraph titulo = new Paragraph("REPORTE DE VENTAS", titleFont);
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 10;
            doc.Add(titulo);

            // Rango de fechas y filtros
            string filtros = $"Desde: {dtpDesde.Value:dd/MM/yyyy} - Hasta: {dtpHasta.Value:dd/MM/yyyy}";
            if (!string.IsNullOrWhiteSpace(txtBuscarCliente.Text))
                filtros += $" | Cliente: {txtBuscarCliente.Text}";
            if (cbUsuario.SelectedItem != null && cbUsuario.SelectedIndex > 0)
                filtros += $" | Usuario: {cbUsuario.Text}";

            Paragraph rango = new Paragraph(filtros, bodyFont);
            rango.Alignment = Element.ALIGN_CENTER;
            rango.SpacingAfter = 20;
            doc.Add(rango);

            // Obtener datos para el gráfico
            DataTable dtGrafico = new DataTable();
            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    string sqlGrafico = @"
SELECT TOP 12
    CONCAT('Semana ', DATEPART(week, f.fecha)) AS Semana,
    SUM(f.total_final) AS Total,
    MIN(f.fecha) AS FechaOrden
FROM Facturas f
WHERE
    f.fecha >= @desde AND f.fecha < @hasta
    AND (@clienteId = 0 OR f.cliente_id = @clienteId)
    AND (@usuarioId = 0 OR f.usuario_id = @usuarioId)
    AND (
        @buscar = '' OR
        f.secuencial LIKE '%' + @buscar + '%' OR
        f.clave_acceso LIKE '%' + @buscar + '%' OR
        ISNULL(f.numero_autorizacion,'') LIKE '%' + @buscar + '%'
    )
GROUP BY YEAR(f.fecha), DATEPART(week, f.fecha)
ORDER BY MIN(f.fecha);";

                    using (var cmd = new SqlCommand(sqlGrafico, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", dtpDesde.Value.Date);
                        cmd.Parameters.AddWithValue("@hasta", dtpHasta.Value.Date.AddDays(1));
                        cmd.Parameters.AddWithValue("@clienteId", clienteSeleccionadoId ?? 0);
                        cmd.Parameters.AddWithValue("@usuarioId", Convert.ToInt64(cbUsuario.SelectedValue ?? 0));
                        cmd.Parameters.AddWithValue("@buscar", (txtBuscar.Text ?? "").Trim());

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dtGrafico);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Paragraph errorChart = new Paragraph("Error al cargar datos del gráfico: " + ex.Message, bodyFont);
                errorChart.SpacingAfter = 10;
                doc.Add(errorChart);
            }

            if (dtGrafico.Rows.Count > 0)
            {
                // ===== GRÁFICO DE LÍNEAS CON SYSTEM.DRAWING =====
                Paragraph chartTitle = new Paragraph("GRÁFICO DE VENTAS POR SEMANA", subtitleFont);
                chartTitle.SpacingAfter = 10;
                doc.Add(chartTitle);

                try
                {
                    // Crear imagen del gráfico
                    using (var chartImage = GenerarGraficoConDrawing(dtGrafico))
                    {
                        if (chartImage != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                chartImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0; // Importante: resetear la posición del stream

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

                PdfPCell headerSemana = new PdfPCell(new Phrase("Semana", headerFont));
                headerSemana.BackgroundColor = new BaseColor(230, 230, 230);
                headerSemana.Padding = 5;
                dataTable.AddCell(headerSemana);

                PdfPCell headerTotal = new PdfPCell(new Phrase("Total Ventas", headerFont));
                headerTotal.BackgroundColor = new BaseColor(230, 230, 230);
                headerTotal.Padding = 5;
                dataTable.AddCell(headerTotal);

                foreach (DataRow row in dtGrafico.Rows)
                {
                    string semana = row["Semana"].ToString();
                    decimal total = Convert.ToDecimal(row["Total"]);

                    dataTable.AddCell(new PdfPCell(new Phrase(semana, bodyFont)) { Padding = 4 });
                    dataTable.AddCell(new PdfPCell(new Phrase(total.ToString("C2", CultureInfo.GetCultureInfo("es-EC")), bodyFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(dataTable);
            }
            else
            {
                Paragraph noData = new Paragraph("No hay datos suficientes para generar el gráfico", bodyFont);
                noData.SpacingAfter = 20;
                doc.Add(noData);
            }

            // ===== TABLA DE DETALLE =====
            Paragraph tableTitle = new Paragraph("DETALLE DE FACTURAS", subtitleFont);
            tableTitle.SpacingAfter = 10;
            doc.Add(tableTitle);

            var visibleCols = dgvVentas.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            PdfPTable table = new PdfPTable(visibleCols);
            table.WidthPercentage = 100;
            table.SpacingBefore = 5;

            foreach (DataGridViewColumn col in dgvVentas.Columns)
            {
                if (!col.Visible) continue;
                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, headerFont));
                cell.BackgroundColor = new BaseColor(230, 230, 230);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            int rowCount = 0;
            foreach (DataGridViewRow row in dgvVentas.Rows)
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
                PdfPCell emptyCell = new PdfPCell(new Phrase("No hay facturas en el período seleccionado", bodyFont));
                emptyCell.Colspan = visibleCols;
                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                emptyCell.Padding = 10;
                table.AddCell(emptyCell);
            }

            doc.Add(table);

            // Resumen final
            doc.Add(new Paragraph(" "));
            Paragraph resumenFinal = new Paragraph(
                $"Total Facturas: {lblCantFacturas.Text}   |   " +
                $"Total Ventas: {lblTotalVentas.Text}   |   " +
                $"IVA: {lblTotalIVA.Text}   |   " +
                $"Descuentos: {lblTotalDesc.Text}",
                headerFont);
            resumenFinal.Alignment = Element.ALIGN_RIGHT;
            resumenFinal.SpacingBefore = 15;
            doc.Add(resumenFinal);

            doc.Close();
        }

        private GdiImage GenerarGraficoConDrawing(DataTable dtGrafico)
        {
            if (dtGrafico == null || dtGrafico.Rows.Count == 0)
            {
                // Crear una imagen con mensaje de "Sin datos"
                GdiBitmap bmpNoData = new GdiBitmap(800, 400);
                using (GdiGraphics g = GdiGraphics.FromImage(bmpNoData))
                {
                    g.Clear(GdiColor.White);
                    g.DrawString("No hay datos disponibles para el período seleccionado",
                        new GdiFont("Arial", 14, System.Drawing.FontStyle.Bold),
                        System.Drawing.Brushes.Gray,
                        new System.Drawing.PointF(200, 180));
                }
                return bmpNoData;
            }

            try
            {
                int width = 800;
                int height = 400;
                int marginLeft = 80;
                int marginRight = 60;
                int marginTop = 50;
                int marginBottom = 70;
                int graphWidth = width - marginLeft - marginRight;
                int graphHeight = height - marginTop - marginBottom;

                // Validar que el área del gráfico sea positiva
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

                    // Encontrar valores máximos y mínimos
                    double maxValor = 0;
                    foreach (DataRow row in dtGrafico.Rows)
                    {
                        double valor = Convert.ToDouble(row["Total"]);
                        if (valor > maxValor) maxValor = valor;
                    }

                    // Si maxValor es 0, establecer un valor por defecto
                    if (maxValor <= 0)
                    {
                        maxValor = 1000; // Valor por defecto
                    }
                    else
                    {
                        maxValor = Math.Ceiling(maxValor * 1.1 / 100) * 100;
                    }

                    // Dibujar fondo del área del gráfico
                    using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(250, 250, 255)))
                    {
                        g.FillRectangle(brush, marginLeft, marginTop, graphWidth, graphHeight);
                    }

                    // Dibujar bordes del área del gráfico
                    using (GdiPen pen = new GdiPen(GdiColor.FromArgb(200, 200, 200)))
                    {
                        g.DrawRectangle(pen, marginLeft, marginTop, graphWidth, graphHeight);
                    }

                    // Dibujar líneas de grid horizontal
                    using (GdiPen pen = new GdiPen(GdiColor.FromArgb(220, 220, 220)))
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                        int numLineas = 5;
                        for (int i = 0; i <= numLineas; i++)
                        {
                            float y = marginTop + (graphHeight * i / numLineas);
                            g.DrawLine(pen, marginLeft, y, marginLeft + graphWidth, y);

                            // Etiquetas del eje Y
                            double valor = maxValor * (1 - (double)i / numLineas);
                            string etiqueta = valor.ToString("C0", CultureInfo.GetCultureInfo("es-EC"));
                            g.DrawString(etiqueta, new GdiFont("Arial", 8),
                                System.Drawing.Brushes.Gray, marginLeft - 50, y - 6);
                        }
                    }

                    // Dibujar ejes
                    using (GdiPen pen = new GdiPen(GdiColor.Black, 2))
                    {
                        g.DrawLine(pen, marginLeft, marginTop, marginLeft, marginTop + graphHeight); // Eje Y
                        g.DrawLine(pen, marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight); // Eje X
                    }

                    // Calcular puntos solo si hay más de 1 punto
                    int numPuntos = dtGrafico.Rows.Count;
                    if (numPuntos > 1)
                    {
                        float pasoX = graphWidth / (float)(numPuntos - 1);

                        GdiPointF[] puntos = new GdiPointF[numPuntos];
                        for (int i = 0; i < numPuntos; i++)
                        {
                            double valor = Convert.ToDouble(dtGrafico.Rows[i]["Total"]);
                            float x = marginLeft + i * pasoX;
                            float y = marginTop + graphHeight - (float)((valor / maxValor) * graphHeight);
                            puntos[i] = new GdiPointF(x, y);
                        }

                        // Dibujar línea
                        using (GdiPen pen = new GdiPen(GdiColor.FromArgb(97, 101, 244), 3))
                        {
                            g.DrawLines(pen, puntos);
                        }

                        // Dibujar puntos y valores
                        using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(97, 101, 244)))
                        using (GdiPen whitePen = new GdiPen(GdiColor.White, 2))
                        using (GdiFont valorFont = new GdiFont("Arial", 8, System.Drawing.FontStyle.Bold))
                        {
                            foreach (var punto in puntos)
                            {
                                g.FillEllipse(brush, punto.X - 5, punto.Y - 5, 10, 10);
                                g.DrawEllipse(whitePen, punto.X - 5, punto.Y - 5, 10, 10);
                            }

                            // Etiquetas de valores
                            for (int i = 0; i < numPuntos; i++)
                            {
                                double valor = Convert.ToDouble(dtGrafico.Rows[i]["Total"]);
                                string valorStr = valor.ToString("C0", CultureInfo.GetCultureInfo("es-EC"));
                                g.DrawString(valorStr, valorFont, System.Drawing.Brushes.Black,
                                    puntos[i].X - 20, puntos[i].Y - 20);
                            }
                        }
                    }
                    else if (numPuntos == 1)
                    {
                        // Caso especial: solo un punto
                        double valor = Convert.ToDouble(dtGrafico.Rows[0]["Total"]);
                        float x = marginLeft + graphWidth / 2;
                        float y = marginTop + graphHeight / 2;

                        using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(97, 101, 244)))
                        using (GdiPen whitePen = new GdiPen(GdiColor.White, 2))
                        {
                            g.FillEllipse(brush, x - 8, y - 8, 16, 16);
                            g.DrawEllipse(whitePen, x - 8, y - 8, 16, 16);
                        }

                        string valorStr = valor.ToString("C0", CultureInfo.GetCultureInfo("es-EC"));
                        g.DrawString(valorStr, new GdiFont("Arial", 10, System.Drawing.FontStyle.Bold),
                            System.Drawing.Brushes.Black, x - 20, y - 30);
                    }

                    // Etiquetas del eje X
                    using (GdiFont semanaFont = new GdiFont("Arial", 8))
                    {
                        float pasoX = graphWidth / (float)(Math.Max(numPuntos - 1, 1));
                        for (int i = 0; i < numPuntos; i++)
                        {
                            string semana = dtGrafico.Rows[i]["Semana"].ToString();
                            float x = marginLeft + i * pasoX;
                            g.DrawString(semana, semanaFont, System.Drawing.Brushes.Black,
                                x - 20, marginTop + graphHeight + 5);
                        }
                    }

                    // Título del gráfico
                    using (GdiFont titleFont = new GdiFont("Arial", 12, System.Drawing.FontStyle.Bold))
                    {
                        g.DrawString("Evolución de Ventas Semanales", titleFont,
                            System.Drawing.Brushes.Black, width / 2 - 120, 10);
                    }

                    // Información adicional
                    using (GdiFont infoFont = new GdiFont("Arial", 8))
                    {
                        string info = $"Total de semanas: {numPuntos} | Venta máxima: {maxValor:C0}";
                        g.DrawString(info, infoFont, System.Drawing.Brushes.Gray, 20, height - 20);
                    }
                }

                return bmp;
            }
            catch (Exception ex)
            {
                // Si hay error, crear una imagen con el mensaje de error
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

        private void PrevisualizarPDF()
        {
            try
            {
                if (_dtVentas == null || _dtVentas.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Exportar",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string tempPdf = Path.Combine(Path.GetTempPath(),
                    $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                GenerarPDF(tempPdf);

                var visor = new FormPdfViewer(
                    tempPdf,
                    title: "Vista previa - Reporte de Ventas",
                    defaultSaveName: $"ReporteVentas_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
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

            txtBuscarCliente.Text = "";
            clienteSeleccionadoId = null;

            if (cbUsuario.Items.Count > 0) cbUsuario.SelectedIndex = 0;

            txtBuscar.Text = "";
            CargarReporte();
        }

       

        private void guna2Panel1_Paint(object sender, PaintEventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e) { }
    }
}
