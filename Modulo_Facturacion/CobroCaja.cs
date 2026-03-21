using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class CobroCaja : Form
    {
        private readonly Conexion con = new Conexion();
        private readonly long usuarioId;

        private long facturaIdSeleccionada = 0;
        private decimal totalFactura = 0m;
        private decimal pagadoFactura = 0m;
        private decimal saldoFactura = 0m;

        public CobroCaja(long usuarioId)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;

            DataGridViewEstilo.AplicarEstiloDashboard(dgvFacturas);


            // eventos
            this.Load += CobroCaja_Load;
            btnBuscar.Click += (s, e) => CargarFacturasPendientes(txtBuscar.Text.Trim());
            btnRefrescar.Click += (s, e) => { txtBuscar.Clear(); CargarFacturasPendientes(); };

            dgvFacturas.SelectionChanged += (s, e) => TomarFacturaSeleccionada();

            cmbMetodoPago.SelectedIndexChanged += (s, e) => RecalcularCambio();
            txtMontoRecibido.TextChanged += (s, e) => RecalcularCambio();

            btnCobrar.Click += (s, e) => Cobrar();

            EstilizarGridCobro();
        }

        private void CobroCaja_Load(object sender, EventArgs e)
        {
            ConfigurarGrid();
            CargarMetodosPago();
            CargarFacturasPendientes();
        }

        private void ConfigurarGrid()
        {
            dgvFacturas.AutoGenerateColumns = true;
            dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFacturas.MultiSelect = false;
            dgvFacturas.ReadOnly = true;
            dgvFacturas.AllowUserToAddRows = false;
            dgvFacturas.AllowUserToDeleteRows = false;
        }

        private void CargarMetodosPago()
        {
            using (var cn = con.CrearConexionAbierta())
            using (var da = new SqlDataAdapter(
                "SELECT id, nombre FROM MetodosPago WHERE activo = 1 ORDER BY nombre", cn))
            {
                var dt = new DataTable();
                da.Fill(dt);

                cmbMetodoPago.DisplayMember = "nombre";
                cmbMetodoPago.ValueMember = "id";
                cmbMetodoPago.DataSource = dt;
                if (cmbMetodoPago.Items.Count > 0) cmbMetodoPago.SelectedIndex = 0;
            }
        }

        private void CargarFacturasPendientes(string filtro = "")
        {
            string sql = @"
SELECT 
    f.id,
    CONCAT(ISNULL(f.establecimiento,'001'), '-', ISNULL(f.punto_emision,'001'), '-', f.secuencial) AS numero,
    f.fecha,
    c.nombre AS cliente,
    f.total_final AS total,
    ISNULL(SUM(ic.monto), 0) AS pagado,
    (f.total_final - ISNULL(SUM(ic.monto), 0)) AS saldo
FROM Facturas f
INNER JOIN Clientes c ON c.id = f.cliente_id
LEFT JOIN IngresosCaja ic ON ic.factura_id = f.id
GROUP BY f.id, f.establecimiento, f.punto_emision, f.secuencial, f.fecha, c.nombre, f.total_final
HAVING (f.total_final - ISNULL(SUM(ic.monto), 0)) > 0
";

            // filtro por número o cliente
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                sql += @"
AND (
    CONCAT(ISNULL(f.establecimiento,'001'), '-', ISNULL(f.punto_emision,'001'), '-', f.secuencial) LIKE @filtro
    OR c.nombre LIKE @filtro
)
";
            }

            sql += " ORDER BY f.fecha DESC;";

            using (var cn = con.CrearConexionAbierta())
            using (var cmd = new SqlCommand(sql, cn))
            {
                if (!string.IsNullOrWhiteSpace(filtro))
                    cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);

                dgvFacturas.DataSource = dt;
            }

            LimpiarDetalle();
        }

        private void TomarFacturaSeleccionada()
        {
            if (dgvFacturas.CurrentRow == null || dgvFacturas.CurrentRow.DataBoundItem == null)
                return;

            var row = dgvFacturas.CurrentRow;

            facturaIdSeleccionada = Convert.ToInt64(row.Cells["id"].Value);
            totalFactura = Convert.ToDecimal(row.Cells["total"].Value);
            pagadoFactura = Convert.ToDecimal(row.Cells["pagado"].Value);
            saldoFactura = Convert.ToDecimal(row.Cells["saldo"].Value);

            lblFacturaSel.Text = row.Cells["numero"].Value?.ToString() ?? "-";
            lblTotal.Text = totalFactura.ToString("0.00");
            lblPagado.Text = pagadoFactura.ToString("0.00");
            lblSaldo.Text = saldoFactura.ToString("0.00");

            txtMontoRecibido.Text = saldoFactura.ToString("0.00");
            txtReferencia.Text = "";
            RecalcularCambio();
        }

        private void RecalcularCambio()
        {
            decimal recibido = ParseDecimal(txtMontoRecibido.Text);
            decimal cambio = 0m;

            // Si el método es Efectivo (o contiene "efect"), permitimos recibido >= saldo y calculamos cambio
            string metodoNombre = (cmbMetodoPago.Text ?? "").ToLowerInvariant();
            bool esEfectivo = metodoNombre.Contains("efect");

            if (facturaIdSeleccionada > 0 && esEfectivo && recibido > saldoFactura)
                cambio = recibido - saldoFactura;

            lblCambio.Text = cambio.ToString("0.00");
        }

        private void Cobrar()
        {
            if (facturaIdSeleccionada <= 0)
            {
                MessageBox.Show("Selecciona una factura pendiente.", "Cobro en caja",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbMetodoPago.SelectedValue == null)
            {
                MessageBox.Show("Selecciona un método de pago.", "Cobro en caja",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal recibido = ParseDecimal(txtMontoRecibido.Text);
            if (recibido <= 0)
            {
                MessageBox.Show("Ingresa un monto válido.", "Cobro en caja",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string metodoNombre = (cmbMetodoPago.Text ?? "").ToLowerInvariant();
            bool esEfectivo = metodoNombre.Contains("efect");

            decimal montoRegistrar;

            if (esEfectivo)
            {
                // En efectivo: puede recibir más y dar cambio, pero registra SOLO el saldo
                if (recibido < saldoFactura)
                {
                    MessageBox.Show("En efectivo el monto recibido no puede ser menor al saldo.", "Cobro en caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                montoRegistrar = saldoFactura;
            }
            else
            {
                // No efectivo: por defecto cobramos el saldo exacto (o puedes permitir parcial si quieres)
                if (recibido != saldoFactura)
                {
                    MessageBox.Show("Para este método, el monto debe ser igual al saldo.", "Cobro en caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                montoRegistrar = recibido;
            }

            int metodoId = Convert.ToInt32(cmbMetodoPago.SelectedValue);
            string referencia = string.IsNullOrWhiteSpace(txtReferencia.Text) ? null : txtReferencia.Text.Trim();

            using (var cn = con.CrearConexionAbierta())
            using (var tx = cn.BeginTransaction())
            {
                try
                {
                    string insert = @"
INSERT INTO IngresosCaja (factura_id, metodo_pago_id, monto, referencia, usuario_id)
VALUES (@facturaId, @metodoId, @monto, @referencia, @usuarioId);
";
                    using (var cmd = new SqlCommand(insert, cn, tx))
                    {
                        cmd.Parameters.AddWithValue("@facturaId", facturaIdSeleccionada);
                        cmd.Parameters.AddWithValue("@metodoId", metodoId);
                        cmd.Parameters.AddWithValue("@monto", montoRegistrar);
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@referencia", (object)referencia ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch (Exception ex)
                {
                    tx.Rollback();
                    MessageBox.Show("Error al registrar cobro:\n" + ex.Message, "Cobro en caja",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            MessageBox.Show("Cobro registrado correctamente.", "Cobro en caja",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            // refrescar lista
            CargarFacturasPendientes(txtBuscar.Text.Trim());
        }

        private void LimpiarDetalle()
        {
            facturaIdSeleccionada = 0;
            totalFactura = pagadoFactura = saldoFactura = 0m;

            lblFacturaSel.Text = "-";
            lblTotal.Text = "0.00";
            lblPagado.Text = "0.00";
            lblSaldo.Text = "0.00";
            lblCambio.Text = "0.00";

            txtMontoRecibido.Text = "";
            txtReferencia.Text = "";
        }

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;

            // Soporta coma o punto
            text = text.Trim().Replace(",", ".");
            decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val);
            return val;
        }

        private void EstilizarGridCobro()
        {
            // Si tu control es Guna2DataGridView, esto igual funciona.


            dgvFacturas.AutoGenerateColumns = true;

            dgvFacturas.AllowUserToAddRows = false;
            dgvFacturas.AllowUserToDeleteRows = false;
            dgvFacturas.AllowUserToResizeRows = false;

            dgvFacturas.RowHeadersVisible = false;

            // Para que se ajuste al bloque (y no quede “hueco”)
            dgvFacturas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvFacturas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            // Si tienes muchas columnas, que permita scroll horizontal
            dgvFacturas.ScrollBars = ScrollBars.Both;

            // Mejor lectura
            dgvFacturas.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvFacturas.MultiSelect = false;

            // Cabecera visible siempre
            dgvFacturas.EnableHeadersVisualStyles = false;
            dgvFacturas.ColumnHeadersVisible = true;
                dgvFacturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvFacturas.ColumnHeadersHeight = 38;

            // Alto de filas consistente
            dgvFacturas.RowTemplate.Height = 34;

            // Evita que “se comprima” y quede feo cuando hay poco contenido
            dgvFacturas.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvFacturas.AutoSize = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void CobroCaja_Load_1(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvFacturas);

        }
    }
}