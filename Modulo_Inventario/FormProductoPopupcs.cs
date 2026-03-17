using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormProductoPopup : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable dtImpuestos = new DataTable();
        private ErrorProvider errorProvider;

        public long ProductoCreadoId { get; private set; } = 0;
        public string ProductoCreadoNombre { get; private set; } = "";

        public FormProductoPopup()
        {
            InitializeComponent();
            Text = "Nuevo producto";
            StartPosition = FormStartPosition.CenterParent;

            InicializarValidaciones();
            ConfigurarCombos();
            CargarImpuestos();

            btnGuardar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        public FormProductoPopup(string nombreSugerido) : this()
        {
            txtNombre.Text = (nombreSugerido ?? "").Trim();
            txtNombre.SelectionStart = txtNombre.TextLength;
            txtNombre.Focus();

            // Validar después de cargar el nombre sugerido
            ValidarNombre();
        }

        private void InicializarValidaciones()
        {
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // Eventos de validación en tiempo real
            txtNombre.TextChanged += (s, e) => ValidarNombre();
            txtNombre.Leave += (s, e) => ValidarNombre();

            nudPvp.ValueChanged += (s, e) => ValidarPvp();
            nudPvp.Leave += (s, e) => ValidarPvp();

            cmbImpuesto.SelectedIndexChanged += (s, e) => ValidarImpuesto();

            // Evento KeyPress para evitar caracteres no deseados en el nombre
            txtNombre.KeyPress += (s, e) =>
            {
                // Permitir letras, números, espacios y algunos caracteres comunes
                char c = e.KeyChar;
                if (!char.IsControl(c) && !char.IsLetterOrDigit(c) && c != ' ' && c != '-' && c != '.' && c != '/')
                {
                    e.Handled = true;
                }
            };
        }

        private void ConfigurarCombos()
        {
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
            nudPvp.Minimum = 0.0001m;  // Cambiar el mínimo a un valor muy pequeño pero positivo
            nudPvp.Value = 0.01m;       // Valor por defecto sugerido
        }

        // ===== MÉTODOS DE VALIDACIÓN =====

        private void MarcarError(Control control, string mensaje)
        {
            control.BackColor = Color.FromArgb(255, 220, 220);
            errorProvider.SetError(control, mensaje);
        }

        private void MarcarOk(Control control)
        {
            control.BackColor = Color.White;
            errorProvider.SetError(control, "");
        }

        private bool ValidarNombre()
        {
            string nombre = txtNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MarcarError(txtNombre, "El nombre del producto es obligatorio.");
                return false;
            }

            if (nombre.Length < 2)
            {
                MarcarError(txtNombre, "El nombre debe tener al menos 2 caracteres.");
                return false;
            }

            if (nombre.Length > 255)
            {
                MarcarError(txtNombre, "El nombre no puede exceder 255 caracteres.");
                return false;
            }

            MarcarOk(txtNombre);
            return true;
        }

        private bool ValidarPvp()
        {
            if (nudPvp.Value <= 0)  // Cambiado de < 0 a <= 0
            {
                MarcarError(nudPvp, "El precio debe ser mayor a 0.");
                return false;
            }

            MarcarOk(nudPvp);
            return true;
        }

        private bool ValidarImpuesto()
        {
            if (cmbImpuesto.SelectedIndex == -1 || cmbImpuesto.SelectedValue == null)
            {
                MarcarError(cmbImpuesto, "Seleccione un impuesto.");
                return false;
            }

            MarcarOk(cmbImpuesto);
            return true;
        }

        private bool ValidarTodo()
        {
            return ValidarNombre() && ValidarPvp() && ValidarImpuesto();
        }

        private void LimpiarValidaciones()
        {
            MarcarOk(txtNombre);
            MarcarOk(nudPvp);
            MarcarOk(cmbImpuesto);
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

                if (dtImpuestos.Rows.Count > 0)
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
            // Validar todos los campos antes de guardar
            if (!ValidarTodo())
            {
                MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Obtener el nombre y formatearlo a "Formato Título"
            string nombreOriginal = txtNombre.Text.Trim();
            string nombreFormateado = FormatearATitulo(nombreOriginal);

            // Actualizar el TextBox con el nombre formateado (opcional)
            txtNombre.Text = nombreFormateado;

            string tipo = cmbTipo.SelectedItem?.ToString() ?? "Otro";
            decimal pvp = nudPvp.Value;
            int impuestoId = Convert.ToInt32(cmbImpuesto.SelectedValue);

            // Validación adicional por si acaso
            if (pvp <= 0)
            {
                MessageBox.Show("El precio de venta debe ser mayor a 0.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudPvp.Focus();
                return;
            }

            try
            {
                con.Abrir();

                // Verificar si ya existe un producto con ese nombre
                using (var cmdCheck = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE nombre = @n", con.leer))
                {
                    cmdCheck.Parameters.AddWithValue("@n", nombreFormateado);
                    if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                    {
                        MessageBox.Show("Ya existe un producto con ese nombre.", "Duplicado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MarcarError(txtNombre, "Nombre ya existe");
                        return;
                    }
                }

                using (var cmd = new SqlCommand(@"
            INSERT INTO Productos(nombre, tipo, stock, precio_costo, precio_pvp, impuesto_id)
            VALUES(@n, @t, 0, 0, @p, @imp);
            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);", con.leer))
                {
                    cmd.Parameters.AddWithValue("@n", nombreFormateado);
                    cmd.Parameters.AddWithValue("@t", tipo);
                    cmd.Parameters.AddWithValue("@p", pvp);
                    cmd.Parameters.AddWithValue("@imp", impuestoId);

                    ProductoCreadoId = Convert.ToInt64(cmd.ExecuteScalar());
                    ProductoCreadoNombre = nombreFormateado;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creando producto: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Cerrar();
            }
        }

        /// <summary>
        /// Convierte un texto a formato título (primera letra de cada palabra en mayúscula)
        /// Ejemplo: "ACEITE DE MOTOR" → "Aceite De Motor"
        /// </summary>
        private string FormatearATitulo(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return texto;

            string[] palabras = texto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] conectores = { "de", "la", "el", "y", "del", "los", "las" };

            for (int i = 0; i < palabras.Length; i++)
            {
                string palabra = palabras[i].ToLower();

                if (palabra.Length > 0)
                {
                    // Verificar si es un conector y no es la primera palabra
                    if (i > 0 && conectores.Contains(palabra))
                    {
                        palabras[i] = palabra; // Mantener en minúscula
                    }
                    else
                    {
                        palabras[i] = char.ToUpper(palabra[0]) + palabra.Substring(1);
                    }
                }
            }

            return string.Join(" ", palabras);
        }

    }
}
