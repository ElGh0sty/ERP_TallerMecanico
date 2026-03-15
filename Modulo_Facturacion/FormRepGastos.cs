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

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormRepGastos : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable _dtGastos = new DataTable();

        public FormRepGastos()
        {
            InitializeComponent();

            this.Load += FormRepGastos_Load;
            btnBuscar.Click += (s, e) => CargarReporte();
            btnLimpiar.Click += (s, e) => LimpiarFiltros();
            btnExportar.Click += (s, e) => PrevisualizarPDF();

            dtpHasta.ValueChanged += (s, e) => AjustarDesdeAlMesDeHasta();
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(dgvGastos, "Doble clic para ver la factura (solo compras)");
        }

        private void FormRepGastos_Load(object sender, EventArgs e)
        {
            dtpHasta.Value = DateTime.Today;
            AjustarDesdeAlMesActual();

            EstilizarGrid();
            InicializarChart();
            CargarCombos();
            CargarReporte();
        }

        // Método auxiliar para obtener el logo de la empresa
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

        private void AjustarDesdeAlMesActual()
        {
            var h = DateTime.Today;
            dtpDesde.Value = new DateTime(h.Year, h.Month, 1);
        }

        private void AjustarDesdeAlMesDeHasta()
        {
            var h = dtpHasta.Value;
            var inicio = new DateTime(h.Year, h.Month, 1);

            if (dtpDesde.Value.Year != inicio.Year || dtpDesde.Value.Month != inicio.Month)
                dtpDesde.Value = inicio;
        }

        private void CargarCombos()
        {
            using (var cn = con.CrearConexionAbierta())
            {
                // USUARIOS (si aplica)
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

                
            }
        }

        private void CargarReporte()
        {
            try
            {
                DateTime desde = dtpDesde.Value.Date;
                DateTime hasta = dtpHasta.Value.Date.AddDays(1); // exclusivo

                long usuarioId = Convert.ToInt64(cbUsuario.SelectedValue ?? 0);
                string buscar = (txtBuscar.Text ?? "").Trim();

                CargarGraficoGastosSemanal(desde, hasta, usuarioId, buscar);

                // MODIFICADA: Agregar columnas ID para poder abrir los PDFs
                string sqlGrid = @"
SELECT
    t.ID,
    t.Tipo,
    t.Fecha,
    t.Descripcion,
    t.Usuario,
    t.MetodoPago,
    t.Subtotal,
    t.IVA,
    t.Total
FROM
(
    -- COMPRAS
    SELECT
        fc.id AS ID,  -- ID de FacturaCompra
        'Compra' AS Tipo,
        c.fecha_compra AS Fecha,
        CONCAT('Factura: ', c.numero_factura_proveedor, ' - ', p.nombre_empresa) AS Descripcion,
        u.nombre_usuario AS Usuario,
        'Transferencia/Cheque' AS MetodoPago,
        c.subtotal AS Subtotal,
        c.iva_total AS IVA,
        c.total_compra AS Total
    FROM Compras c
    INNER JOIN FacturaCompra fc ON fc.compra_id = c.id
    INNER JOIN Usuarios u ON u.id = c.usuario_id
    INNER JOIN Proveedores p ON p.id = c.proveedor_id
    WHERE
        c.fecha_compra >= @desde AND c.fecha_compra < @hasta
        AND (@usuarioId = 0 OR c.usuario_id = @usuarioId)
        AND (@buscar = '' OR c.numero_factura_proveedor LIKE '%' + @buscar + '%' OR p.nombre_empresa LIKE '%' + @buscar + '%')

    UNION ALL

    -- EGRESOS CAJA
    SELECT
        e.id AS ID,
        'Egreso Caja' AS Tipo,
        e.fecha_pago AS Fecha,
        ISNULL(e.referencia, 'Sin referencia') AS Descripcion,
        u.nombre_usuario AS Usuario,
        mp.nombre AS MetodoPago,
        e.monto AS Subtotal,
        CAST(0 AS decimal(18,4)) AS IVA,
        e.monto AS Total
    FROM EgresosCaja e
    INNER JOIN Usuarios u ON u.id = e.usuario_id
    INNER JOIN MetodosPago mp ON mp.id = e.metodo_pago_id
    WHERE
        e.fecha_pago >= @desde AND e.fecha_pago < @hasta
        AND (@usuarioId = 0 OR e.usuario_id = @usuarioId)
        AND (@buscar = '' OR ISNULL(e.referencia,'') LIKE '%' + @buscar + '%')
) t
ORDER BY t.Fecha DESC;
";

                string sqlResumen = @"
SELECT
    COUNT(*) AS CantMov,
    SUM(Total) AS TotalGastos,
    SUM(IVA) AS TotalIVA,
    AVG(NULLIF(Total,0)) AS Promedio
FROM
(
    SELECT
        c.total_compra AS Total,
        c.iva_total AS IVA
    FROM Compras c
    WHERE
        c.fecha_compra >= @desde AND c.fecha_compra < @hasta
        AND (@usuarioId = 0 OR c.usuario_id = @usuarioId)
        AND (@buscar = '' OR c.numero_factura_proveedor LIKE '%' + @buscar + '%')

    UNION ALL

    SELECT
        e.monto AS Total,
        CAST(0 AS decimal(18,4)) AS IVA
    FROM EgresosCaja e
    WHERE
        e.fecha_pago >= @desde AND e.fecha_pago < @hasta
        AND (@usuarioId = 0 OR e.usuario_id = @usuarioId)
        AND (@buscar = '' OR ISNULL(e.referencia,'') LIKE '%' + @buscar + '%')
) x;
";

                using (var cn = con.CrearConexionAbierta())
                {
                    using (var cmd = new SqlCommand(sqlGrid, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            _dtGastos = new DataTable();
                            da.Fill(_dtGastos);
                            dgvGastos.DataSource = _dtGastos;

                            dgvGastos.ColumnHeadersVisible = true;
                            dgvGastos.Refresh();
                            AplicarAnchosColumnas();
                        }
                    }

                    using (var cmd = new SqlCommand(sqlResumen, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);
                        cmd.Parameters.AddWithValue("@hasta", hasta);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var rd = cmd.ExecuteReader())
                        {
                            if (rd.Read())
                            {
                                int cant = Convert.ToInt32(rd["CantMov"]);
                                decimal totalG = rd["TotalGastos"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["TotalGastos"]);
                                decimal totalI = rd["TotalIVA"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["TotalIVA"]);
                                decimal prom = rd["Promedio"] == DBNull.Value ? 0 : Convert.ToDecimal(rd["Promedio"]);

                                lblCantGastos.Text = cant.ToString();
                                lblTotalGastos.Text = totalG.ToString("C2", CultureInfo.GetCultureInfo("es-EC"));
                                lblTotalIVA.Text = totalI.ToString("C2", CultureInfo.GetCultureInfo("es-EC"));
                                lblPromedio.Text = prom.ToString("C2", CultureInfo.GetCultureInfo("es-EC"));
                            }
                        }
                    }
                }

                FormatearColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar reporte:\n" + ex.Message, "Reporte de gastos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EstilizarGrid()
        {
            dgvGastos.AutoGenerateColumns = true;

            dgvGastos.ColumnHeadersVisible = true;
            dgvGastos.EnableHeadersVisualStyles = false;
            dgvGastos.ColumnHeadersHeight = 36;
            dgvGastos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            dgvGastos.RowHeadersVisible = false;
            dgvGastos.AllowUserToAddRows = false;
            dgvGastos.AllowUserToResizeRows = false;
            dgvGastos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGastos.MultiSelect = false;

            // Scroll horizontal + no apretado
            dgvGastos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvGastos.ScrollBars = ScrollBars.Both;
            dgvGastos.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void AplicarAnchosColumnas()
        {
            if (dgvGastos.Columns.Count == 0) return;

            if (dgvGastos.Columns["ID"] != null) dgvGastos.Columns["ID"].Visible = false;

            if (dgvGastos.Columns["Fecha"] != null) dgvGastos.Columns["Fecha"].Width = 140;
            if (dgvGastos.Columns["Tipo"] != null) dgvGastos.Columns["Tipo"].Width = 110;
            if (dgvGastos.Columns["Descripcion"] != null) dgvGastos.Columns["Descripcion"].Width = 300;
            if (dgvGastos.Columns["Usuario"] != null) dgvGastos.Columns["Usuario"].Width = 90;
            if (dgvGastos.Columns["MetodoPago"] != null) dgvGastos.Columns["MetodoPago"].Width = 120;
            if (dgvGastos.Columns["Subtotal"] != null) dgvGastos.Columns["Subtotal"].Width = 110;
            if (dgvGastos.Columns["IVA"] != null) dgvGastos.Columns["IVA"].Width = 90;
            if (dgvGastos.Columns["Total"] != null) dgvGastos.Columns["Total"].Width = 110;
        }

        private void FormatearColumnas()
        {
            if (dgvGastos.Columns.Count == 0) return;

            OcultarCol("GastoID");

            FormatoFecha("Fecha");
            FormatoMoneda("Subtotal");
            FormatoMoneda("IVA");
            FormatoMoneda("Total");
        }

        private void OcultarCol(string col)
        {
            if (dgvGastos.Columns[col] != null) dgvGastos.Columns[col].Visible = false;
        }

        private void FormatoFecha(string col)
        {
            if (dgvGastos.Columns[col] != null)
                dgvGastos.Columns[col].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        }

        private void FormatoMoneda(string col)
        {
            if (dgvGastos.Columns[col] != null)
                dgvGastos.Columns[col].DefaultCellStyle.Format = "N2";
        }

        //  CHART SEMANAL GASTOS 

        private void InicializarChart()
        {
            ChartGastos.Datasets.Clear();
            ChartGastos.Legend.Display = false;
            ChartGastos.Title.Text = "";
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

        private void CargarGraficoGastosSemanal(DateTime desde, DateTime hasta, long usuarioId, string buscar)
        {
            ChartGastos.Datasets.Clear();

            var ds = new GunaBarDataset
            {
                Label = "Gastos por semana",
                YFormat = "C"
            };

            string sqlSemanal = @"
SELECT
    Semana,
    SUM(Total) AS Total
FROM
(
    -- Compras
    SELECT
        CONCAT(YEAR(c.fecha_compra), '-W', RIGHT('0' + CAST(DATEPART(ISO_WEEK, c.fecha_compra) AS varchar(2)), 2)) AS Semana,
        c.total_compra AS Total
    FROM Compras c
    WHERE
        c.fecha_compra >= @desde AND c.fecha_compra < @hasta
        AND (@usuarioId = 0 OR c.usuario_id = @usuarioId)
        AND (@buscar = '' OR c.numero_factura_proveedor LIKE '%' + @buscar + '%')

    UNION ALL

    -- Egresos Caja
    SELECT
        CONCAT(YEAR(e.fecha_pago), '-W', RIGHT('0' + CAST(DATEPART(ISO_WEEK, e.fecha_pago) AS varchar(2)), 2)) AS Semana,
        e.monto AS Total
    FROM EgresosCaja e
    WHERE
        e.fecha_pago >= @desde AND e.fecha_pago < @hasta
        AND (@usuarioId = 0 OR e.usuario_id = @usuarioId)
        AND (@buscar = '' OR ISNULL(e.referencia,'') LIKE '%' + @buscar + '%')
) t
GROUP BY Semana
ORDER BY Semana;
";

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(sqlSemanal, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                cmd.Parameters.AddWithValue("@buscar", buscar);

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string semana = rd["Semana"].ToString();
                        double total = rd["Total"] == DBNull.Value ? 0 : Convert.ToDouble(rd["Total"]);
                        ds.DataPoints.Add(new LPoint { Label = semana, Y = total });
                    }
                }
            }

            ChartGastos.Datasets.Add(ds);
            ChartGastos.Update();
        }

        // FormPdfViewer

        private void PrevisualizarPDF()
        {
            try
            {
                if (_dtGastos == null || _dtGastos.Rows.Count == 0)
                {
                    MessageBox.Show("No hay datos para exportar.", "Exportar",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string tempPdf = Path.Combine(Path.GetTempPath(),
                    $"ReporteGastos_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

                GenerarPDF(tempPdf);

                var visor = new FormPdfViewer(
                    tempPdf,
                    title: "Vista previa - Reporte de Gastos",
                    defaultSaveName: $"ReporteGastos_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
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
            cellTitulo.AddElement(new Paragraph("REPORTE DE\nGASTOS", titleFont) { Alignment = Element.ALIGN_RIGHT });
            head.AddCell(cellTitulo);

            doc.Add(head);
            doc.Add(new Paragraph(" "));

            // Rango de fechas y filtros
            string filtros = $"Desde: {dtpDesde.Value:dd/MM/yyyy} - Hasta: {dtpHasta.Value:dd/MM/yyyy}";
            if (cbUsuario.SelectedItem != null && cbUsuario.SelectedIndex > 0)
                filtros += $" | Usuario: {cbUsuario.Text}";
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
                filtros += $" | Búsqueda: {txtBuscar.Text}";

            Paragraph rango = new Paragraph(filtros, bodyFont);
            rango.Alignment = Element.ALIGN_CENTER;
            rango.SpacingAfter = 20;
            doc.Add(rango);

            // Obtener datos para el gráfico de gastos
            DataTable dtGrafico = new DataTable();
            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    string sqlGrafico = @"
SELECT TOP 12
    CONCAT('Semana ', DATEPART(week, Fecha)) AS Semana,
    SUM(Total) AS Total,
    MIN(Fecha) AS FechaOrden
FROM
(
    -- Compras
    SELECT 
        c.fecha_compra AS Fecha,
        c.total_compra AS Total
    FROM Compras c
    WHERE
        c.fecha_compra >= @desde AND c.fecha_compra < @hasta
        AND (@usuarioId = 0 OR c.usuario_id = @usuarioId)
        AND (@buscar = '' OR c.numero_factura_proveedor LIKE '%' + @buscar + '%')

    UNION ALL

    -- Egresos Caja
    SELECT 
        e.fecha_pago AS Fecha,
        e.monto AS Total
    FROM EgresosCaja e
    WHERE
        e.fecha_pago >= @desde AND e.fecha_pago < @hasta
        AND (@usuarioId = 0 OR e.usuario_id = @usuarioId)
        AND (@buscar = '' OR ISNULL(e.referencia,'') LIKE '%' + @buscar + '%')
) t
GROUP BY YEAR(Fecha), DATEPART(week, Fecha)
ORDER BY MIN(Fecha);";

                    using (var cmd = new SqlCommand(sqlGrafico, cn))
                    {
                        cmd.Parameters.AddWithValue("@desde", dtpDesde.Value.Date);
                        cmd.Parameters.AddWithValue("@hasta", dtpHasta.Value.Date.AddDays(1));
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
                Paragraph chartTitle = new Paragraph("GRÁFICO DE GASTOS POR SEMANA", subtitleFont);
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

                PdfPCell headerSemana = new PdfPCell(new Phrase("Semana", headerFont));
                headerSemana.BackgroundColor = new BaseColor(230, 230, 230);
                headerSemana.Padding = 5;
                dataTable.AddCell(headerSemana);

                PdfPCell headerTotal = new PdfPCell(new Phrase("Total Gastos", headerFont));
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
            Paragraph tableTitle = new Paragraph("DETALLE DE GASTOS", subtitleFont);
            tableTitle.SpacingAfter = 10;
            doc.Add(tableTitle);

            var visibleCols = dgvGastos.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);
            PdfPTable table = new PdfPTable(visibleCols);
            table.WidthPercentage = 100;
            table.SpacingBefore = 5;

            foreach (DataGridViewColumn col in dgvGastos.Columns)
            {
                if (!col.Visible) continue;
                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, headerFont));
                cell.BackgroundColor = new BaseColor(230, 230, 230);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            int rowCount = 0;
            foreach (DataGridViewRow row in dgvGastos.Rows)
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
                PdfPCell emptyCell = new PdfPCell(new Phrase("No hay gastos en el período seleccionado", bodyFont));
                emptyCell.Colspan = visibleCols;
                emptyCell.HorizontalAlignment = Element.ALIGN_CENTER;
                emptyCell.Padding = 10;
                table.AddCell(emptyCell);
            }

            doc.Add(table);

            // Resumen final
            doc.Add(new Paragraph(" "));
            Paragraph resumenFinal = new Paragraph(
                $"Total Movimientos: {lblCantGastos.Text}   |   " +
                $"Total Gastos: {lblTotalGastos.Text}   |   " +
                $"IVA: {lblTotalIVA.Text}   |   " +
                $"Promedio: {lblPromedio.Text}",
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

                    double maxValor = 0;
                    foreach (DataRow row in dtGrafico.Rows)
                    {
                        double valor = Convert.ToDouble(row["Total"]);
                        if (valor > maxValor) maxValor = valor;
                    }

                    if (maxValor <= 0)
                    {
                        maxValor = 1000;
                    }
                    else
                    {
                        maxValor = Math.Ceiling(maxValor * 1.1 / 100) * 100;
                    }

                    using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(250, 250, 255)))
                    {
                        g.FillRectangle(brush, marginLeft, marginTop, graphWidth, graphHeight);
                    }

                    using (GdiPen pen = new GdiPen(GdiColor.FromArgb(200, 200, 200)))
                    {
                        g.DrawRectangle(pen, marginLeft, marginTop, graphWidth, graphHeight);
                    }

                    using (GdiPen pen = new GdiPen(GdiColor.FromArgb(220, 220, 220)))
                    {
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

                        int numLineas = 5;
                        for (int i = 0; i <= numLineas; i++)
                        {
                            float y = marginTop + (graphHeight * i / numLineas);
                            g.DrawLine(pen, marginLeft, y, marginLeft + graphWidth, y);

                            double valor = maxValor * (1 - (double)i / numLineas);
                            string etiqueta = valor.ToString("C0", CultureInfo.GetCultureInfo("es-EC"));
                            g.DrawString(etiqueta, new GdiFont("Arial", 8),
                                System.Drawing.Brushes.Gray, marginLeft - 50, y - 6);
                        }
                    }

                    using (GdiPen pen = new GdiPen(GdiColor.Black, 2))
                    {
                        g.DrawLine(pen, marginLeft, marginTop, marginLeft, marginTop + graphHeight);
                        g.DrawLine(pen, marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight);
                    }

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

                        using (GdiPen pen = new GdiPen(GdiColor.FromArgb(255, 89, 129), 3))
                        {
                            g.DrawLines(pen, puntos);
                        }

                        using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(255, 89, 129)))
                        using (GdiPen whitePen = new GdiPen(GdiColor.White, 2))
                        using (GdiFont valorFont = new GdiFont("Arial", 8, System.Drawing.FontStyle.Bold))
                        {
                            foreach (var punto in puntos)
                            {
                                g.FillEllipse(brush, punto.X - 5, punto.Y - 5, 10, 10);
                                g.DrawEllipse(whitePen, punto.X - 5, punto.Y - 5, 10, 10);
                            }

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
                        double valor = Convert.ToDouble(dtGrafico.Rows[0]["Total"]);
                        float x = marginLeft + graphWidth / 2;
                        float y = marginTop + graphHeight / 2;

                        using (GdiBrush brush = new GdiBrush(GdiColor.FromArgb(255, 89, 129)))
                        using (GdiPen whitePen = new GdiPen(GdiColor.White, 2))
                        {
                            g.FillEllipse(brush, x - 8, y - 8, 16, 16);
                            g.DrawEllipse(whitePen, x - 8, y - 8, 16, 16);
                        }

                        string valorStr = valor.ToString("C0", CultureInfo.GetCultureInfo("es-EC"));
                        g.DrawString(valorStr, new GdiFont("Arial", 10, System.Drawing.FontStyle.Bold),
                            System.Drawing.Brushes.Black, x - 20, y - 30);
                    }

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

                    using (GdiFont titleFont = new GdiFont("Arial", 12, System.Drawing.FontStyle.Bold))
                    {
                        g.DrawString("Evolución de Gastos Semanales", titleFont,
                            System.Drawing.Brushes.Black, width / 2 - 120, 10);
                    }

                    using (GdiFont infoFont = new GdiFont("Arial", 8))
                    {
                        string info = $"Total de semanas: {numPuntos} | Gasto máximo: {maxValor:C0}";
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

        private void LimpiarFiltros()
        {
            dtpHasta.Value = DateTime.Today;
            AjustarDesdeAlMesActual();

            if (cbUsuario.Items.Count > 0) cbUsuario.SelectedIndex = 0;

            txtBuscar.Text = "";
            CargarReporte();
        }

        private void dgvGastos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Validar que no sea el header
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvGastos.Rows[e.RowIndex];
                if (row.DataBoundItem == null) return;

                // Obtener el tipo de gasto y el ID
                string tipo = "";
                long id = 0;

                if (row.DataBoundItem is DataRowView drv)
                {
                    tipo = drv["Tipo"]?.ToString() ?? "";
                    if (drv.Row.Table.Columns.Contains("ID"))
                        id = Convert.ToInt64(drv["ID"]);
                }
                else
                {
                    if (dgvGastos.Columns["Tipo"] != null)
                        tipo = row.Cells["Tipo"].Value?.ToString() ?? "";

                    if (dgvGastos.Columns["ID"] != null)
                        id = Convert.ToInt64(row.Cells["ID"].Value);
                }

                // Solo las compras tienen PDF (los egresos de caja no)
                if (tipo == "Compra")
                {
                    if (id <= 0)
                    {
                        MessageBox.Show("No se pudo identificar la factura.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    Cursor = Cursors.WaitCursor;
                    try
                    {
                        AbrirPdfFacturaCompraDesdeBd(id);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    MessageBox.Show("Los egresos de caja no tienen factura asociada.",
                        "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
            }
        }

        private void AbrirPdfFacturaCompraDesdeBd(long facturaCompraId)
        {
            byte[] pdf;
            string nombre;

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand("SELECT pdf_data, pdf_nombre FROM FacturaCompra WHERE id=@id;", cn))
            {
                cmd.Parameters.AddWithValue("@id", facturaCompraId);

                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        throw new Exception("No existe la factura de compra " + facturaCompraId);

                    if (rd["pdf_data"] == DBNull.Value)
                        throw new Exception("Esta factura no tiene PDF guardado.");

                    pdf = (byte[])rd["pdf_data"];
                    nombre = rd["pdf_nombre"]?.ToString();
                    if (string.IsNullOrWhiteSpace(nombre)) nombre = $"FacturaCompra_{facturaCompraId}.pdf";
                }
            }

            string carpeta = Path.Combine(Path.GetTempPath(), "TallerMecanicoERP");
            Directory.CreateDirectory(carpeta);

            string pdfPath = Path.Combine(carpeta, nombre);
            File.WriteAllBytes(pdfPath, pdf);

            using (var visor = new FormPdfViewer(
                pdfPath,
                title: "Vista previa - Factura de Compra",
                defaultSaveName: nombre))
            {
                visor.StartPosition = FormStartPosition.CenterParent;
                visor.WindowState = FormWindowState.Maximized;
                visor.ShowDialog(this);
            }
        }
    }
}
