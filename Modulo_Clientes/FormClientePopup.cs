using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormClientePopup : Form
    {
        private readonly Conexion con = new Conexion();
        private readonly long? _clienteId;
        private ErrorProvider errorProvider;
        public long? ClienteIdCreado { get; private set; } = null;
        public string NumDocCreado { get; private set; } = null;

        public FormClientePopup()
        {
            InitializeComponent();
            _clienteId = null;
            InicializarValidaciones();
            ConfigUI();
        }

        public FormClientePopup(long clienteId) : this()
        {
            _clienteId = clienteId;
            CargarCliente(clienteId);
            Text = "Editar cliente";
        }

        private void ConfigUI()
        {
            Text = "Nuevo cliente";
            StartPosition = FormStartPosition.CenterParent;

            cmbTipoDoc.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipoDoc.Items.Clear();
            cmbTipoDoc.Items.AddRange(new object[] { "Cédula", "RUC", "Pasaporte" });
            cmbTipoDoc.SelectedIndex = 0;

            btnGuardar.Click += (s, e) => Guardar();
            btnCancelar.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        private void InicializarValidaciones()
        {
            // Inicializar ErrorProvider
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            // Eventos en tiempo real
            txtNombre.TextChanged += (s, e) => ValidarNombre();
            txtNumDoc.TextChanged += (s, e) => ValidarDocumento();
            cmbTipoDoc.SelectedIndexChanged += (s, e) => ValidarDocumento();
            txtTelefono.TextChanged += (s, e) => ValidarTelefono();
            txtEmail.TextChanged += (s, e) => ValidarEmail();
            txtDireccion.TextChanged += (s, e) => ValidarDireccion();

            // Validar al perder el foco también
            txtNombre.Leave += (s, e) => ValidarNombre();
            txtNumDoc.Leave += (s, e) => ValidarDocumento();
            txtTelefono.Leave += (s, e) => ValidarTelefono();
            txtEmail.Leave += (s, e) => ValidarEmail();
            txtDireccion.Leave += (s, e) => ValidarDireccion();

            // Configurar eventos KeyPress para campos numéricos
            ConfigurarKeyPressEvents();
        }

        private void ConfigurarKeyPressEvents()
        {
            // Para el teléfono (solo números)
            txtTelefono.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                    e.Handled = true;
            };

            // Para el documento (depende del tipo, pero dejamos que el validador maneje)
        }

        // ===== MÉTODOS DE VALIDACIÓN EN TIEMPO REAL =====

        private void MarcarError(Control control, string mensaje)
        {
            control.BackColor = Color.FromArgb(255, 220, 220); // Rojo claro
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
                MarcarError(txtNombre, "El nombre es obligatorio.");
                return false;
            }

            if (nombre.Length < 3)
            {
                MarcarError(txtNombre, "El nombre debe tener al menos 3 caracteres.");
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

        private bool ValidarDocumento()
        {
            string numDoc = txtNumDoc.Text.Trim();
            string tipoDoc = cmbTipoDoc.SelectedItem?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(numDoc))
            {
                MarcarError(txtNumDoc, "El número de documento es obligatorio.");
                return false;
            }

            bool valido = false;
            string mensaje = "";

            switch (tipoDoc)
            {
                case "Cédula":
                    valido = EsCedulaEcuatorianaValida(numDoc, out mensaje);
                    break;
                case "RUC":
                    valido = EsRucEcuatorianoValido(numDoc, out mensaje);
                    break;
                case "Pasaporte":
                    valido = EsPasaporteValido(numDoc, out mensaje);
                    break;
                default:
                    mensaje = "Seleccione un tipo de documento válido.";
                    break;
            }

            if (!valido)
            {
                MarcarError(txtNumDoc, mensaje);
                return false;
            }

            MarcarOk(txtNumDoc);
            return true;
        }

        private bool ValidarTelefono()
        {
            string telefono = txtTelefono.Text.Trim();

            if (string.IsNullOrWhiteSpace(telefono))
            {
                MarcarError(txtTelefono, "El teléfono es obligatorio.");
                return false;
            }

            string soloDigitos = SoloDigitos(telefono);
            if (soloDigitos.Length < 7 || soloDigitos.Length > 10)
            {
                MarcarError(txtTelefono, "El teléfono debe tener entre 7 y 10 dígitos.");
                return false;
            }

            // Formatear automáticamente si es necesario
            if (telefono != soloDigitos)
            {
                txtTelefono.Text = soloDigitos;
                txtTelefono.SelectionStart = txtTelefono.Text.Length;
            }

            MarcarOk(txtTelefono);
            return true;
        }

        private bool ValidarEmail()
        {
            string email = txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MarcarError(txtEmail, "El email es obligatorio.");
                return false;
            }

            if (!EsEmailValido(email))
            {
                MarcarError(txtEmail, "El formato del email no es válido.");
                return false;
            }

            MarcarOk(txtEmail);
            return true;
        }

        private bool ValidarDireccion()
        {
            string direccion = txtDireccion.Text.Trim();

            if (string.IsNullOrWhiteSpace(direccion))
            {
                MarcarError(txtDireccion, "La dirección es obligatoria.");
                return false;
            }

            if (direccion.Length < 5)
            {
                MarcarError(txtDireccion, "La dirección debe tener al menos 5 caracteres.");
                return false;
            }

            MarcarOk(txtDireccion);
            return true;
        }

        private bool ValidarTodo()
        {
            return ValidarNombre() &&
                   ValidarDocumento() &&
                   ValidarTelefono() &&
                   ValidarEmail() &&
                   ValidarDireccion();
        }

        // ===== MÉTODOS DE VALIDACIÓN ESPECÍFICOS (con mensajes de error) =====

        private string SoloDigitos(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var chars = new StringBuilder();
            foreach (char c in s)
                if (char.IsDigit(c)) chars.Append(c);
            return chars.ToString();
        }

        private bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            email = email.Trim().ToLower();

            // Expresión regular más robusta
            string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(email, patron))
                return false;

            // Validaciones adicionales
            if (email.Contains("..")) return false;
            if (email.StartsWith(".") || email.EndsWith(".")) return false;

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0) return false;

            string domain = email.Substring(atIndex + 1);
            if (domain.Contains("..")) return false;
            if (domain.StartsWith(".") || domain.EndsWith(".")) return false;

            string[] parts = domain.Split('.');
            if (parts.Length < 2) return false;
            if (parts[parts.Length - 1].Length < 2) return false;

            return true;
        }

        private bool EsPasaporteValido(string pasaporte, out string mensaje)
        {
            mensaje = "";
            pasaporte = pasaporte.Trim();

            if (pasaporte.Length < 6 || pasaporte.Length > 20)
            {
                mensaje = "El pasaporte debe tener entre 6 y 20 caracteres.";
                return false;
            }

            foreach (char c in pasaporte)
                if (!char.IsLetterOrDigit(c))
                {
                    mensaje = "El pasaporte solo puede contener letras y números.";
                    return false;
                }

            return true;
        }

        private bool EsCedulaEcuatorianaValida(string cedula, out string mensaje)
        {
            mensaje = "";
            cedula = SoloDigitos(cedula);

            if (cedula.Length != 10)
            {
                mensaje = "La cédula debe tener 10 dígitos.";
                return false;
            }

            // Validar provincia (01-24 son válidas, 30 también es válido para nuevas)
            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
            {
                // 30 también es válido para nuevas provincias
                if (provincia != 30)
                {
                    mensaje = "Los primeros 2 dígitos deben estar entre 01 y 24 (o 30).";
                    return false;
                }
            }

            // El tercer dígito debe ser menor a 6 (0-5)
            int tercerDigito = int.Parse(cedula[2].ToString());
            if (tercerDigito > 5)
            {
                mensaje = "El tercer dígito debe ser menor a 6 (0-5).";
                return false;
            }

            // Algoritmo de validación CORREGIDO
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int total = 0;

            for (int i = 0; i < 9; i++)
            {
                int digito = int.Parse(cedula[i].ToString());
                int valor = digito * coeficientes[i];

                if (valor >= 10)
                    valor -= 9;

                total += valor;
            }

            int digitoVerificador = int.Parse(cedula[9].ToString());
            int digitoCalculado = 10 - (total % 10);

            // Si el resultado es 10, el dígito verificador debe ser 0
            if (digitoCalculado == 10)
                digitoCalculado = 0;

            if (digitoCalculado != digitoVerificador)
            {
                mensaje = $"Dígito verificador incorrecto. Debería ser {digitoCalculado}.";
                return false;
            }

            return true;
        }

        private bool EsRucEcuatorianoValido(string ruc, out string mensaje)
        {
            mensaje = "";
            ruc = SoloDigitos(ruc);

            if (ruc.Length != 13)
            {
                mensaje = "El RUC debe tener 13 dígitos.";
                return false;
            }

            if (!int.TryParse(ruc.Substring(0, 2), out int provincia) || provincia < 1 || provincia > 24)
            {
                mensaje = "Los primeros 2 dígitos deben estar entre 01 y 24.";
                return false;
            }

            int tercer = ruc[2] - '0';
            if (tercer > 6)
            {
                mensaje = "El tercer dígito debe ser 0-6 para RUC.";
                return false;
            }

            string establecimiento = ruc.Substring(10, 3);
            if (establecimiento != "001")
            {
                mensaje = "Los últimos 3 dígitos deben ser 001 para RUC.";
                return false;
            }

            // Validar cédula si el tercer dígito es 0-5
            if (tercer >= 0 && tercer <= 5)
            {
                string cedula = ruc.Substring(0, 10);
                bool cedulaValida = EsCedulaEcuatorianaValida(cedula, out mensaje);
                if (!cedulaValida)
                    return false;
            }

            return true;
        }

        // ===== MÉTODOS DE CARGA Y GUARDADO =====

        private void CargarCliente(long id)
        {
            try
            {
                con.Abrir();
                using (var cmd = new SqlCommand(@"
                    SELECT tipo_documento, numero_documento, nombre, direccion, telefono, email
                    FROM Clientes
                    WHERE id = @id;", con.leer))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (!rd.Read())
                        {
                            MessageBox.Show("No se encontró el cliente.");
                            Close();
                            return;
                        }

                        cmbTipoDoc.SelectedItem = rd["tipo_documento"].ToString();
                        txtNumDoc.Text = rd["numero_documento"].ToString();
                        txtNombre.Text = rd["nombre"].ToString();
                        txtDireccion.Text = rd["direccion"].ToString();
                        txtTelefono.Text = rd["telefono"].ToString();
                        txtEmail.Text = rd["email"].ToString();

                        // Validar campos después de cargar
                        ValidarNombre();
                        ValidarDocumento();
                        ValidarTelefono();
                        ValidarEmail();
                        ValidarDireccion();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando cliente: " + ex.Message);
                Close();
            }
            finally { con.Cerrar(); }
        }

        private void Guardar()
        {
            // Validar todos los campos antes de guardar
            if (!ValidarTodo())
            {
                MessageBox.Show("Corrija los errores marcados en rojo antes de guardar.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipoDoc = cmbTipoDoc.SelectedItem?.ToString() ?? "";
            string numDoc = txtNumDoc.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string email = txtEmail.Text.Trim();
            string direccion = txtDireccion.Text.Trim();

            try
            {
                con.Abrir();

                // Evitar duplicados por numero_documento
                using (var cmdDup = new SqlCommand(@"
                    SELECT COUNT(*) FROM Clientes
                    WHERE numero_documento = @num AND (@id IS NULL OR id <> @id);", con.leer))
                {
                    cmdDup.Parameters.AddWithValue("@num", numDoc);
                    cmdDup.Parameters.AddWithValue("@id", (object)_clienteId ?? DBNull.Value);

                    int existe = Convert.ToInt32(cmdDup.ExecuteScalar());
                    if (existe > 0)
                    {
                        MessageBox.Show("Ya existe un cliente con ese número de documento.");
                        MarcarError(txtNumDoc, "Documento ya registrado");
                        return;
                    }
                }

                if (_clienteId == null)
                {
                    // INSERT
                    using (var cmd = new SqlCommand(@"
                        INSERT INTO Clientes(tipo_documento, numero_documento, nombre, direccion, telefono, email)
                        OUTPUT INSERTED.id
                        VALUES(@t, @n, @nom, @dir, @tel, @em);", con.leer))
                    {
                        cmd.Parameters.AddWithValue("@t", tipoDoc);
                        cmd.Parameters.AddWithValue("@n", numDoc);
                        cmd.Parameters.AddWithValue("@nom", nombre);
                        cmd.Parameters.AddWithValue("@dir", direccion);
                        cmd.Parameters.AddWithValue("@tel", telefono);
                        cmd.Parameters.AddWithValue("@em", email);

                        ClienteIdCreado = Convert.ToInt64(cmd.ExecuteScalar());
                        NumDocCreado = numDoc;
                    }
                }
                else
                {
                    // UPDATE
                    using (var cmd = new SqlCommand(@"
                        UPDATE Clientes
                        SET tipo_documento=@t, numero_documento=@n, nombre=@nom, direccion=@dir, telefono=@tel, email=@em
                        WHERE id=@id;", con.leer))
                    {
                        cmd.Parameters.AddWithValue("@t", tipoDoc);
                        cmd.Parameters.AddWithValue("@n", numDoc);
                        cmd.Parameters.AddWithValue("@nom", nombre);
                        cmd.Parameters.AddWithValue("@dir", direccion);
                        cmd.Parameters.AddWithValue("@tel", telefono);
                        cmd.Parameters.AddWithValue("@em", email);
                        cmd.Parameters.AddWithValue("@id", _clienteId.Value);
                        cmd.ExecuteNonQuery();
                    }
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error guardando cliente: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        // Eventos del diseñador (no eliminar)
        private void cmbTipoDoc_SelectedIndexChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
    }
}
