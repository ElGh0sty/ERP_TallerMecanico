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
        private DataTable dtImpuestos = new DataTable();

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
                "Consumible","Repuesto","Aceite","Filtro","Bujia","Bateria","Neumatico",
                "Accesorio","Herramienta","Quimico","Lubricante","Otro"
            });
            cmbTipo.SelectedIndex = 0;

            nudPvp.DecimalPlaces = 4;
            nudPvp.Maximum = 999999;

            CargarImpuestos();

            btnGuardar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        public FormProductoPopup(string nombreSugerido) : this()
        {
            txtNombre.Text = (nombreSugerido ?? "").Trim();
            txtNombre.SelectionStart = txtNombre.TextLength;
            txtNombre.Focus();
        }

        private void CargarImpuestos()
        {
            try
            {
                con.Abrir();
                string sql = "SELECT id, nombre, porcentaje FROM Impuestos WHERE activo = 1 ORDER BY id";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                da.Fill(dtImpuestos);

                cmbImpuesto.DataSource = dtImpuestos;
                cmbImpuesto.DisplayMember = "nombre";
                cmbImpuesto.ValueMember = "id";
                cmbImpuesto.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando impuestos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
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

            decimal pvp = nudPvp.Value;
            if (pvp < 0)
            {
                MessageBox.Show("Precio inválido.");
                return;
            }

            if (cmbImpuesto.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un impuesto.");
                return;
            }

            int impuestoId = Convert.ToInt32(cmbImpuesto.SelectedValue);

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

                using (var cmd = new SqlCommand(@"
                    INSERT INTO Productos(nombre, tipo, stock, precio_costo, precio_pvp, impuesto_id)
                    VALUES(@n, @t, 0, 0, @p, @imp);
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", con.leer))
                {
                    cmd.Parameters.AddWithValue("@n", nombre);
                    cmd.Parameters.AddWithValue("@t", tipo);
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
    }
}
