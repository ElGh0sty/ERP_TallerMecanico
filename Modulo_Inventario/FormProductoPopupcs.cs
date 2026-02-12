using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormProductoPopup : Form
    {
        private readonly Conexion con = new Conexion();

        public long ProductoCreadoId { get; private set; } = 0;
        public string ProductoCreadoNombre { get; private set; } = "";

        public FormProductoPopup()
        {
            
            InitializeComponent();
            Text = "Nuevo producto";
            StartPosition = FormStartPosition.CenterParent;

            cmbTipo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipo.Items.Clear();
            cmbTipo.Items.AddRange(new object[]
            {
                "Consumible","Repuesto","Aceite","Filtro","Bujia","Bateria","Neumatico","Accesorio","Herramienta","Quimico","Lubricante","Otro"
            });
            cmbTipo.SelectedIndex = 0;

            nudCosto.DecimalPlaces = 4;
            nudCosto.Maximum = 999999;
            nudPvp.DecimalPlaces = 4;
            nudPvp.Maximum = 999999;

            nudCosto.ValueChanged += (s, e) =>
            {
                // sugerencia pvp (30%)
                nudPvp.Value = Math.Min(nudPvp.Maximum, Math.Round(nudCosto.Value * 1.30m, 4));
            };

            btnGuardar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        public FormProductoPopup(string nombreSugerido) : this()
        {
            txtNombre.Text = (nombreSugerido ?? "").Trim();
            txtNombre.SelectionStart = txtNombre.TextLength;
            txtNombre.Focus();
        }


        private int ObtenerImpuestoIdPorDefecto()
        {
            using (var cmd = new SqlCommand("SELECT TOP 1 id FROM Impuestos ORDER BY id", con.leer))
            {
                object v = cmd.ExecuteScalar();
                if (v == null || v == DBNull.Value)
                    throw new Exception("No existe ningún impuesto en la tabla Impuestos. Crea uno primero.");
                return Convert.ToInt32(v);
            }
        }

        private void Guardar()
        {
            string nombre = (txtNombre.Text ?? "").Trim();
            if (nombre.Length < 2)
            {
                MessageBox.Show("Nombre inválido.");
                return;
            }

            string tipo = (cmbTipo.SelectedItem ?? "Otro").ToString();

            decimal costo = nudCosto.Value;
            decimal pvp = nudPvp.Value;
            if (costo < 0 || pvp < 0)
            {
                MessageBox.Show("Precios inválidos.");
                return;
            }

            try
            {
                con.Abrir();

                // evita duplicado exacto
                using (var cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE nombre=@n", con.leer))
                {
                    cmdCheck.Parameters.AddWithValue("@n", nombre);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("Ya existe un producto con ese nombre.");
                        return;
                    }
                }

                int impuestoId = ObtenerImpuestoIdPorDefecto();

                using (var cmd = new SqlCommand(@"
INSERT INTO Productos(nombre, tipo, stock, precio_costo, precio_pvp, impuesto_id)
VALUES(@n, @t, 0, @c, @p, @imp);
SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", con.leer))
                {
                    cmd.Parameters.AddWithValue("@n", nombre);
                    cmd.Parameters.AddWithValue("@t", tipo);
                    cmd.Parameters.AddWithValue("@c", costo);
                    cmd.Parameters.AddWithValue("@p", pvp);
                    cmd.Parameters.AddWithValue("@imp", impuestoId);

                    ProductoCreadoId = Convert.ToInt64(cmd.ExecuteScalar());
                    ProductoCreadoNombre = nombre;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creando producto: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }
    }
}
