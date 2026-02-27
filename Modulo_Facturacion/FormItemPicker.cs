using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormItemPicker : Form
    {
        private readonly string _cs;

        // ✅ Esto es lo que tu FormGenFactu espera
        public ItemPickResult Result { get; private set; }

        // ✅ Ajusta ESTE nombre según tu tabla Productos (ver sección 3)
        private const string PRODUCT_PRICE_COLUMN = "precio_pvp"; // <-- CAMBIA a: "precio_unitario" o el que tengas

        public FormItemPicker(PROYECTOMECANICO.Conexion con)
        {
            InitializeComponent();
            _cs = con.ConnectionString;

            cmbTipo.SelectedIndexChanged += (s, e) => CargarListado();
            txtBuscar.TextChanged += (s, e) => CargarListado();
            btnAgregar.Click += (s, e) => ElegirItem();
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; Close(); };

            cmbTipo.SelectedIndex = 0; // Productos por defecto
            nudCantidad.Value = 1;
            CargarListado();
        }

        private void CargarListado()
        {
            try
            {
                string q = (txtBuscar.Text ?? "").Trim();

                string tipo = (cmbTipo.SelectedItem ?? "").ToString();
                bool esProducto = tipo == "Producto";

                string sql;

                if (esProducto)
                {
                    // ✅ OJO: PRODUCT_PRICE_COLUMN debe existir en tu tabla Productos
                    sql = @"
SELECT TOP 200
    id,
    nombre,
    CAST(" + PRODUCT_PRICE_COLUMN + @" AS decimal(18,2)) AS precio
FROM Productos
WHERE nombre LIKE @q
ORDER BY nombre ASC";
                }
                else
                {
                    // Ajusta nombres si tu tabla de servicios se llama diferente
                    sql = @"
SELECT TOP 200
    id,
    nombre,
    CAST(precio AS decimal(18,2)) AS precio
FROM Servicios
WHERE nombre LIKE @q
ORDER BY nombre ASC";
                }

                var dt = new DataTable();

                using (var cn = new SqlConnection(_cs))
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = "%" + q + "%";
                    cn.Open();
                    using (var da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }

                dgvListado.DataSource = dt;

                if (dgvListado.Columns.Count > 0)
                {
                    dgvListado.Columns["id"].Visible = false;
                    dgvListado.Columns["nombre"].HeaderText = "Nombre";
                    dgvListado.Columns["precio"].HeaderText = "Precio";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando listado:\n" + ex.Message);
            }
        }

        private void ElegirItem()
        {
            var dt = dgvListado.DataSource as DataTable;
            if (dt == null || dgvListado.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un ítem.");
                return;
            }

            long id = Convert.ToInt64(dgvListado.CurrentRow.Cells["id"].Value);
            string nombre = Convert.ToString(dgvListado.CurrentRow.Cells["nombre"].Value);
            decimal precio = Convert.ToDecimal(dgvListado.CurrentRow.Cells["precio"].Value);
            decimal cantidad = (decimal)nudCantidad.Value;

            string tipo = (cmbTipo.SelectedItem ?? "").ToString();

            Result = new ItemPickResult
            {
                TipoItem = tipo,
                NombreItem = nombre,
                Cantidad = cantidad,
                PrecioUnitario = precio,
                ProductoId = (tipo == "Producto") ? (long?)id : null,
                ServicioId = (tipo == "Servicio") ? (long?)id : null
            };

            this.DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class ItemPickResult
    {
        public string TipoItem { get; set; }
        public string NombreItem { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public long? ProductoId { get; set; }
        public long? ServicioId { get; set; }
    }
}