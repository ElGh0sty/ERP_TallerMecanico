using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using PROYECTOMECANICO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormRepVentas : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable _dtVentas = new DataTable();

        public FormRepVentas()
        {
            InitializeComponent();

            this.Load += FormRepVentas_Load;
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

        private void cbCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            // CargarReporte();
        }

        private void CargarCombos()
        {
            using (var cn = con.CrearConexionAbierta())
            {
                // CLIENTES
                var dtC = new DataTable();
                using (var da = new SqlDataAdapter("SELECT id, nombre FROM Clientes ORDER BY nombre", cn))
                    da.Fill(dtC);

                var filaTodosC = dtC.NewRow();
                filaTodosC["id"] = 0;
                filaTodosC["nombre"] = "Todos";
                dtC.Rows.InsertAt(filaTodosC, 0);

                cbCliente.DisplayMember = "nombre";
                cbCliente.ValueMember = "id";
                cbCliente.DataSource = dtC;

                // USUARIOS
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

                long clienteId = Convert.ToInt64(cbCliente.SelectedValue ?? 0);
                long usuarioId = Convert.ToInt64(cbUsuario.SelectedValue ?? 0);
                string buscar = (txtBuscar.Text ?? "").Trim();

                // 1) Cargar gráfico semanal
                CargarGraficoVentasSemanal(desde, hasta, clienteId, usuarioId, buscar);

                // 2) Cargar grid
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

                // 3) Resumen
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
                        cmd.Parameters.AddWithValue("@clienteId", clienteId);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@buscar", buscar);

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            _dtVentas = new DataTable();
                            da.Fill(_dtVentas);
                            dgvVentas.DataSource = _dtVentas;

                            // headers visibles + auto ajuste
                            dgvVentas.ColumnHeadersVisible = true;
                            dgvVentas.Refresh();
                           

                            // para que se vea bien y no todo mini
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
                        cmd.Parameters.AddWithValue("@clienteId", clienteId);
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

            // FIX HEADERS
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
            ChartVentas.Legend.Display = false;   // más limpio
            ChartVentas.Title.Text = "";          
        }

        private void CargarGraficoVentasSemanal(DateTime desde, DateTime hasta, long clienteId, long usuarioId, string buscar)
        {
            ChartVentas.Datasets.Clear();

            var ds = new GunaBarDataset
            {
                Label = "Ventas por semana",
                YFormat = "C" // formato moneda (según GunaCharts)
            };

            string sqlSemanal = @"
SELECT
    CONCAT(YEAR(f.fecha), '-W', RIGHT('0' + CAST(DATEPART(ISO_WEEK, f.fecha) AS varchar(2)), 2)) AS Semana,
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
GROUP BY YEAR(f.fecha), DATEPART(ISO_WEEK, f.fecha)
ORDER BY YEAR(f.fecha), DATEPART(ISO_WEEK, f.fecha);";

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(sqlSemanal, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                cmd.Parameters.AddWithValue("@clienteId", clienteId);
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

            ChartVentas.Datasets.Add(ds);
            ChartVentas.Update();
        }

        private void GenerarPDF(string filePath)
        {
            Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 40, 40);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            Paragraph titulo = new Paragraph("REPORTE DE VENTAS", titleFont);
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
            var visibleCols = 0;
            foreach (DataGridViewColumn col in dgvVentas.Columns)
                if (col.Visible) visibleCols++;

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

            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
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

            doc.Add(new Paragraph(" "));
            Paragraph resumen = new Paragraph(
                $"Total Facturas: {lblCantFacturas.Text}   |   " +
                $"Total Ventas: {lblTotalVentas.Text}   |   " +
                $"IVA: {lblTotalIVA.Text}",
                headerFont);
            resumen.Alignment = Element.ALIGN_RIGHT;
            resumen.SpacingBefore = 15;
            doc.Add(resumen);

            doc.Close();
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

                GenerarPDF(tempPdf); // tu método GenerarPDF(filePath)

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

            if (cbCliente.Items.Count > 0) cbCliente.SelectedIndex = 0;
            if (cbUsuario.Items.Count > 0) cbUsuario.SelectedIndex = 0;

            txtBuscar.Text = "";
            CargarReporte();
        }

        private void guna2Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
    }
}
