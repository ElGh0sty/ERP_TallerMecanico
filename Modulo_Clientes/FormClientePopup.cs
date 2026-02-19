using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormClientePopup : Form
    {
        private readonly Conexion con = new Conexion();
        private readonly long? _clienteId; 

        public FormClientePopup()
        {
            InitializeComponent();
            _clienteId = null;
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
            // Tomar valores
            string tipoDoc = (cmbTipoDoc.SelectedItem ?? "").ToString().Trim();
            string numDoc = (txtNumDoc.Text ?? "").Trim();
            string nombre = (txtNombre.Text ?? "").Trim();
            string telefono = (txtTelefono.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string direccion = (txtDireccion.Text ?? "").Trim();

            // ✅ Validaciones (las mismas que ya tenías)
            if (string.IsNullOrWhiteSpace(tipoDoc))
            { MessageBox.Show("Selecciona el Tipo de documento."); return; }

            if (string.IsNullOrWhiteSpace(numDoc))
            { MessageBox.Show("El Número de documento es obligatorio."); return; }

            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 3)
            { MessageBox.Show("El nombre es obligatorio (mín. 3 caracteres)."); return; }

            if (!EsTelefonoValido(telefono))
            { MessageBox.Show("Teléfono inválido (usa solo números, 7 a 10 dígitos)."); return; }

            if (string.IsNullOrWhiteSpace(email) || !EsEmailValido(email))
            { MessageBox.Show("Email inválido u obligatorio."); return; }

            if (string.IsNullOrWhiteSpace(direccion) || direccion.Length < 5)
            { MessageBox.Show("Dirección obligatoria (mín. 5 caracteres)."); return; }

            if (tipoDoc.Equals("Cédula", StringComparison.OrdinalIgnoreCase))
            {
                if (!EsCedulaEcuatorianaValida(numDoc))
                { MessageBox.Show("Cédula inválida."); return; }
            }
            else if (tipoDoc.Equals("RUC", StringComparison.OrdinalIgnoreCase))
            {
                if (!EsRucEcuatorianoValido(numDoc))
                { MessageBox.Show("RUC inválido."); return; }
            }
            else if (tipoDoc.Equals("Pasaporte", StringComparison.OrdinalIgnoreCase))
            {
                if (!EsPasaporteValido(numDoc))
                { MessageBox.Show("Pasaporte inválido (mín. 6, máx. 20; letras/números)."); return; }
            }
            else
            {
                MessageBox.Show("Tipo de documento no reconocido.");
                return;
            }

            try
            {
                con.Abrir();

                // ✅ evitar duplicados por numero_documento (excepto si es el mismo en edición)
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
                        return;
                    }
                }

                if (_clienteId == null)
                {
                    // INSERT
                    using (var cmd = new SqlCommand(@"
INSERT INTO Clientes(tipo_documento, numero_documento, nombre, direccion, telefono, email)
VALUES(@t, @n, @nom, @dir, @tel, @em);", con.leer))
                    {
                        cmd.Parameters.AddWithValue("@t", tipoDoc);
                        cmd.Parameters.AddWithValue("@n", numDoc);
                        cmd.Parameters.AddWithValue("@nom", nombre);
                        cmd.Parameters.AddWithValue("@dir", direccion);
                        cmd.Parameters.AddWithValue("@tel", telefono);
                        cmd.Parameters.AddWithValue("@em", email);
                        cmd.ExecuteNonQuery();
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

        // ---------------- VALIDACIONES (copiadas) ----------------

        private string SoloDigitos(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var chars = new System.Text.StringBuilder();
            foreach (char c in s)
                if (char.IsDigit(c)) chars.Append(c);
            return chars.ToString();
        }

        private bool EsTelefonoValido(string tel)
        {
            tel = SoloDigitos(tel);
            return tel.Length >= 7 && tel.Length <= 10;
        }

        private bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            email = email.Trim();

            if (email.Contains("..")) return false;
            if (email.StartsWith(".") || email.EndsWith(".")) return false;
            if (!email.Contains("@")) return false;

            int at = email.IndexOf("@");
            if (at <= 0) return false;
            if (at != email.LastIndexOf("@")) return false;

            string domain = email.Substring(at + 1);
            if (string.IsNullOrWhiteSpace(domain)) return false;
            if (domain.StartsWith(".") || domain.EndsWith(".")) return false;
            if (!domain.Contains(".")) return false;

            foreach (char c in email)
            {
                bool ok = char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '_' || c == '-' || c == '+';
                if (!ok) return false;
            }

            int lastDot = domain.LastIndexOf(".");
            if (lastDot < 1) return false;
            string tld = domain.Substring(lastDot + 1);
            if (tld.Length < 2) return false;

            return true;
        }

        private bool EsPasaporteValido(string pasaporte)
        {
            if (string.IsNullOrWhiteSpace(pasaporte)) return false;
            pasaporte = pasaporte.Trim();
            if (pasaporte.Length < 6 || pasaporte.Length > 20) return false;

            foreach (char c in pasaporte)
                if (!char.IsLetterOrDigit(c)) return false;

            return true;
        }

        private bool EsCedulaEcuatorianaValida(string cedula)
        {
            cedula = SoloDigitos(cedula);
            if (cedula.Length != 10) return false;

            if (!int.TryParse(cedula.Substring(0, 2), out int provincia)) return false;
            if (provincia < 1 || provincia > 24) return false;

            int tercer = cedula[2] - '0';
            if (tercer > 5) return false;

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int dig = cedula[i] - '0';
                if (i % 2 == 0)
                {
                    dig *= 2;
                    if (dig > 9) dig -= 9;
                }
                suma += dig;
            }

            int verificador = (10 - (suma % 10)) % 10;
            int ultimo = cedula[9] - '0';
            return verificador == ultimo;
        }

        private bool EsRucEcuatorianoValido(string ruc)
        {
            ruc = SoloDigitos(ruc);
            if (ruc.Length != 13) return false;

            if (!int.TryParse(ruc.Substring(0, 2), out int provincia)) return false;
            if (provincia < 1 || provincia > 24) return false;

            int tercer = ruc[2] - '0';
            if (tercer > 6) return false;

            string establecimiento = ruc.Substring(10, 3);
            if (establecimiento != "001") return false;

            if (tercer >= 0 && tercer <= 5)
            {
                string cedula = ruc.Substring(0, 10);
                return EsCedulaEcuatorianaValida(cedula);
            }

            return true;
        }

        private void cmbTipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
