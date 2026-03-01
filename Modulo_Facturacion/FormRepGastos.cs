using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PROYECTOMECANICO;

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



                string sqlGrid = @"
SELECT
    t.Fecha,
    t.Tipo,
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
        c.fecha_compra AS Fecha,
        'Compra' AS Tipo,
        CONCAT('Factura Proveedor: ', c.numero_factura_proveedor) AS Descripcion,
        u.nombre_usuario AS Usuario,
        CAST('—' AS nvarchar(50)) AS MetodoPago,
        c.subtotal AS Subtotal,
        c.iva_total AS IVA,
        c.total_compra AS Total
    FROM Compras c
    INNER JOIN Usuarios u ON u.id = c.usuario_id
    WHERE
        c.fecha_compra >= @desde AND c.fecha_compra < @hasta
        AND (@usuarioId = 0 OR c.usuario_id = @usuarioId)
        AND (@buscar = '' OR c.numero_factura_proveedor LIKE '%' + @buscar + '%')

    UNION ALL

    -- EGRESOS CAJA
    SELECT
        e.fecha_pago AS Fecha,
        'Egreso Caja' AS Tipo,
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
            Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 40, 40);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            Paragraph titulo = new Paragraph("REPORTE DE GASTOS", titleFont);
            titulo.Alignment = Element.ALIGN_CENTER;
            titulo.SpacingAfter = 10;
            doc.Add(titulo);

            Paragraph rango = new Paragraph(
                $"Desde: {dtpDesde.Value:dd/MM/yyyy}   Hasta: {dtpHasta.Value:dd/MM/yyyy}",
                bodyFont);
            rango.Alignment = Element.ALIGN_CENTER;
            rango.SpacingAfter = 15;
            doc.Add(rango);

            // Tabla SOLO columnas visibles
            int visibleCols = 0;
            foreach (DataGridViewColumn col in dgvGastos.Columns)
                if (col.Visible) visibleCols++;

            PdfPTable table = new PdfPTable(visibleCols);
            table.WidthPercentage = 100;
            table.SpacingBefore = 5;

            // Headers
            foreach (DataGridViewColumn col in dgvGastos.Columns)
            {
                if (!col.Visible) continue;

                PdfPCell cell = new PdfPCell(new Phrase(col.HeaderText, headerFont));
                cell.BackgroundColor = new BaseColor(230, 230, 230);
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                table.AddCell(cell);
            }

            // Rows
            foreach (DataGridViewRow row in dgvGastos.Rows)
            {
                if (row.IsNewRow) continue;

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

            doc.Add(table);

            // Resumen final
            doc.Add(new Paragraph(" "));

            Paragraph resumen = new Paragraph(
                $"Total Movimientos: {lblCantGastos.Text}   |   " +
                $"Total Gastos: {lblTotalGastos.Text}   |   " +
                $"IVA: {lblTotalIVA.Text}   |   " +
                $"Promedio: {lblPromedio.Text}",
                headerFont);

            resumen.Alignment = Element.ALIGN_RIGHT;
            resumen.SpacingBefore = 15;
            doc.Add(resumen);

            doc.Close();
        }

        private void LimpiarFiltros()
        {
            dtpHasta.Value = DateTime.Today;
            AjustarDesdeAlMesActual();

            if (cbUsuario.Items.Count > 0) cbUsuario.SelectedIndex = 0;
            // if (cbCategoria.Items.Count > 0) cbCategoria.SelectedIndex = 0; // si aplica

            txtBuscar.Text = "";
            CargarReporte();
        }
    }
}
