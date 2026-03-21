using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public partial class FormConfiguracion : Form
    {
        private readonly Conexion con = new Conexion();
        private ErrorProvider errorProvider;

        // Impuestos 
        private long _impId = 0;

        // MétodosPago 
        private long _mpId = 0;

        // Secuenciales 
        private int _secId = 0;

        // Variable para almacenar la imagen temporalmente
        private byte[] _imagenEmpresaBytes = null;

        public FormConfiguracion()
        {
            InitializeComponent();

            ConfigurarEventosDescuentos();
            ConfigurarEventosPromociones();

            // Cargar datos
            Descuento_Cargar();
            Promocion_Cargar();

            DataGridViewEstilo.AplicarEstiloDashboard(dgvImpuestos);
            DataGridViewEstilo.AplicarEstiloDashboard(dgvMetodosPago);
            DataGridViewEstilo.AplicarEstiloDashboard(dgvSecuenciales);
            DataGridViewEstilo.AplicarEstiloDashboard(dgvPromociones);
            DataGridViewEstilo.AplicarEstiloDashboard(dgvDescuentos);
            InicializarValidaciones();

            this.Load += FormConfiguracion_Load;

            // Impuestos
            btnImpBuscar.Click += (s, e) => Imp_Cargar();
            btnImpLimpiar.Click += (s, e) => { txtImpBuscar.Text = ""; Imp_Cargar(); };
            btnImpNuevo.Click += (s, e) => Imp_Nuevo();
            btnImpGuardar.Click += (s, e) => Imp_Guardar();
            btnImpActivarDesactivar.Click += (s, e) => Imp_ActivarDesactivar();
            dgvImpuestos.CellClick += Imp_GridClick;

            // Métodos pago
            btnMpBuscar.Click += (s, e) => MP_Cargar();
            btnMpLimpiar.Click += (s, e) => { txtMpBuscar.Text = ""; MP_Cargar(); };
            btnMpNuevo.Click += (s, e) => MP_Nuevo();
            btnMpGuardar.Click += (s, e) => MP_Guardar();
            btnMpActivarDesactivar.Click += (s, e) => MP_ActivarDesactivar();
            dgvMetodosPago.CellClick += MP_GridClick;

            // Empresa
            btnEmpGuardar.Click += (s, e) => Emp_Guardar();
            btnSeleccionarImagen.Click += (s, e) => btnSeleccionarImagen_Click(s, e);
            btnEliminarImagen.Click += (s, e) => btnEliminarImagen_Click(s, e);

            // Secuenciales
            btnSecBuscar.Click += (s, e) => Sec_Cargar();
            btnSecLimpiar.Click += (s, e) => { txtSecBuscar.Text = ""; Sec_Cargar(); };
            btnSecNuevo.Click += (s, e) => Sec_Nuevo();
            btnSecGuardar.Click += (s, e) => Sec_Guardar();
            btnSecEliminar.Click += (s, e) => Sec_Eliminar();
            dgvSecuenciales.CellClick += Sec_GridClick;

            CargarEmpresaEnLabel();
            CargarImagenEmpresa();
        }

        private void InicializarValidaciones()
        {
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            

            // ===== VALIDACIONES IMPUESTOS =====
            txtImpNombre.TextChanged += (s, e) => ValidarImpuestoNombre();
            txtImpNombre.Leave += (s, e) => ValidarImpuestoNombre();

            nudImpPorcentaje.ValueChanged += (s, e) => ValidarImpuestoPorcentaje();
            nudImpPorcentaje.Leave += (s, e) => ValidarImpuestoPorcentaje();

            txtImpCodigoSri.TextChanged += (s, e) => ValidarImpuestoCodigoSri();
            txtImpCodigoSri.Leave += (s, e) => ValidarImpuestoCodigoSri();

            // ===== VALIDACIONES MÉTODOS DE PAGO =====
            txtMpCodigoSri.TextChanged += (s, e) => ValidarMpCodigoSri();
            txtMpCodigoSri.Leave += (s, e) => ValidarMpCodigoSri();

            txtMpNombre.TextChanged += (s, e) => ValidarMpNombre();
            txtMpNombre.Leave += (s, e) => ValidarMpNombre();

            // ===== VALIDACIONES EMPRESA =====
            txtEmpNombre.TextChanged += (s, e) => ValidarEmpresaNombre();
            txtEmpNombre.Leave += (s, e) => ValidarEmpresaNombre();

            txtEmpRuc.TextChanged += (s, e) => ValidarEmpresaRuc();
            txtEmpRuc.Leave += (s, e) => ValidarEmpresaRuc();

            txtEmpTelefono.TextChanged += (s, e) => ValidarEmpresaTelefono();
            txtEmpTelefono.Leave += (s, e) => ValidarEmpresaTelefono();

            txtEmpEmail.TextChanged += (s, e) => ValidarEmpresaEmail();
            txtEmpEmail.Leave += (s, e) => ValidarEmpresaEmail();

            // ===== VALIDACIONES SECUENCIALES =====
            txtSecTipoDoc.TextChanged += (s, e) => ValidarSecTipoDoc();
            txtSecTipoDoc.Leave += (s, e) => ValidarSecTipoDoc();

            txtSecEstablecimiento.TextChanged += (s, e) => ValidarSecEstablecimiento();
            txtSecEstablecimiento.Leave += (s, e) => ValidarSecEstablecimiento();

            txtSecPuntoEmision.TextChanged += (s, e) => ValidarSecPuntoEmision();
            txtSecPuntoEmision.Leave += (s, e) => ValidarSecPuntoEmision();

            nudSecActual.ValueChanged += (s, e) => ValidarSecSecuencia();
            nudSecActual.Leave += (s, e) => ValidarSecSecuencia();
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

        // Validaciones Impuestos
        private bool ValidarImpuestoNombre()
        {
            string nombre = txtImpNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MarcarError(txtImpNombre, "El nombre del impuesto es obligatorio.");
                return false;
            }

            if (nombre.Length < 3)
            {
                MarcarError(txtImpNombre, "El nombre debe tener al menos 3 caracteres.");
                return false;
            }

            if (nombre.Length > 50)
            {
                MarcarError(txtImpNombre, "El nombre no puede exceder 50 caracteres.");
                return false;
            }

            MarcarOk(txtImpNombre);
            return true;
        }

        private bool ValidarImpuestoPorcentaje()
        {
            if (nudImpPorcentaje.Value < 0 || nudImpPorcentaje.Value > 100)
            {
                MarcarError(nudImpPorcentaje, "El porcentaje debe estar entre 0 y 100.");
                return false;
            }

            MarcarOk(nudImpPorcentaje);
            return true;
        }

        private bool ValidarImpuestoCodigoSri()
        {
            string codigo = txtImpCodigoSri.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                MarcarError(txtImpCodigoSri, "El código SRI es obligatorio.");
                return false;
            }

            if (codigo.Length > 10)
            {
                MarcarError(txtImpCodigoSri, "El código SRI no puede exceder 10 caracteres.");
                return false;
            }

            MarcarOk(txtImpCodigoSri);
            return true;
        }

        // Validaciones Métodos de Pago
        private bool ValidarMpCodigoSri()
        {
            string codigo = txtMpCodigoSri.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                MarcarError(txtMpCodigoSri, "El código SRI es obligatorio.");
                return false;
            }

            if (codigo.Length != 2)
            {
                MarcarError(txtMpCodigoSri, "El código SRI debe tener exactamente 2 caracteres.");
                return false;
            }

            // Validar que sean solo números (para códigos SRI numéricos)
            if (!codigo.All(char.IsDigit))
            {
                MarcarError(txtMpCodigoSri, "El código SRI debe contener solo números.");
                return false;
            }

            MarcarOk(txtMpCodigoSri);
            return true;
        }

        private bool ValidarMpNombre()
        {
            string nombre = txtMpNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MarcarError(txtMpNombre, "El nombre del método de pago es obligatorio.");
                return false;
            }

            if (nombre.Length < 4) // Cambiado de 3 a 4
            {
                MarcarError(txtMpNombre, "El nombre debe tener al menos 4 caracteres.");
                return false;
            }

            if (nombre.Length > 150)
            {
                MarcarError(txtMpNombre, "El nombre no puede exceder 150 caracteres.");
                return false;
            }

            // Validar que no sean solo números
            if (nombre.All(char.IsDigit))
            {
                MarcarError(txtMpNombre, "El nombre no puede contener solo números.");
                return false;
            }

            // Validar que tenga al menos una letra
            if (!nombre.Any(char.IsLetter))
            {
                MarcarError(txtMpNombre, "El nombre debe contener al menos una letra.");
                return false;
            }

            MarcarOk(txtMpNombre);
            return true;
        }

        // Validaciones Empresa
        private bool ValidarEmpresaNombre()
        {
            string nombre = txtEmpNombre.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MarcarError(txtEmpNombre, "El nombre de la empresa es obligatorio.");
                return false;
            }

            if (nombre.Length < 3)
            {
                MarcarError(txtEmpNombre, "El nombre debe tener al menos 3 caracteres.");
                return false;
            }

            if (nombre.Length > 255)
            {
                MarcarError(txtEmpNombre, "El nombre no puede exceder 255 caracteres.");
                return false;
            }

            // Validar que no sean solo números
            if (nombre.All(char.IsDigit))
            {
                MarcarError(txtEmpNombre, "El nombre no puede contener solo números.");
                return false;
            }

            // Validar que tenga al menos 2 letras (para evitar "A1")
            int letras = nombre.Count(char.IsLetter);
            if (letras < 2)
            {
                MarcarError(txtEmpNombre, "El nombre debe contener al menos 2 letras.");
                return false;
            }

            // Caracteres permitidos: letras, números, espacios, puntos, guiones
            if (!nombre.All(c => char.IsLetterOrDigit(c) || c == ' ' || c == '.' || c == '-' || c == '&' || c == ','))
            {
                MarcarError(txtEmpNombre, "El nombre contiene caracteres no válidos.");
                return false;
            }

            MarcarOk(txtEmpNombre);
            return true;
        }

        private bool ValidarEmpresaRuc()
        {
            string ruc = txtEmpRuc.Text.Trim();

            if (string.IsNullOrWhiteSpace(ruc))
            {
                MarcarError(txtEmpRuc, "El RUC es obligatorio.");
                return false;
            }

            if (ruc.Length != 13 || !ruc.All(char.IsDigit))
            {
                MarcarError(txtEmpRuc, "El RUC debe tener 13 dígitos numéricos.");
                return false;
            }

            MarcarOk(txtEmpRuc);
            return true;
        }

        private bool ValidarEmpresaTelefono()
        {
            string telefono = txtEmpTelefono.Text.Trim();

            if (string.IsNullOrWhiteSpace(telefono))
            {
                MarcarError(txtEmpTelefono, "El teléfono es obligatorio.");
                return false;
            }

            string soloDigitos = new string(telefono.Where(char.IsDigit).ToArray());
            if (soloDigitos.Length < 7 || soloDigitos.Length > 15)
            {
                MarcarError(txtEmpTelefono, "El teléfono debe tener entre 7 y 15 dígitos.");
                return false;
            }

            MarcarOk(txtEmpTelefono);
            return true;
        }

        private bool ValidarEmpresaEmail()
        {
            string email = txtEmpEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MarcarError(txtEmpEmail, "El email es obligatorio.");
                return false;
            }

            // Expresión regular más estricta para email
            string patron = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!System.Text.RegularExpressions.Regex.IsMatch(email, patron))
            {
                MarcarError(txtEmpEmail, "El formato del email no es válido (ej: nombre@dominio.com).");
                return false;
            }

            // Validaciones adicionales
            if (email.Contains(".."))
            {
                MarcarError(txtEmpEmail, "El email no puede contener puntos dobles.");
                return false;
            }

            string[] parts = email.Split('@');
            if (parts.Length != 2)
            {
                MarcarError(txtEmpEmail, "El email debe tener un @.");
                return false;
            }

            string localPart = parts[0];
            string domain = parts[1];

            if (string.IsNullOrWhiteSpace(localPart) || string.IsNullOrWhiteSpace(domain))
            {
                MarcarError(txtEmpEmail, "El email tiene partes vacías.");
                return false;
            }

            if (domain.StartsWith(".") || domain.EndsWith("."))
            {
                MarcarError(txtEmpEmail, "El dominio no puede empezar o terminar con punto.");
                return false;
            }

            string[] domainParts = domain.Split('.');
            if (domainParts.Length < 2)
            {
                MarcarError(txtEmpEmail, "El dominio debe tener al menos un punto (ej: dominio.com).");
                return false;
            }

            string tld = domainParts[domainParts.Length - 1];
            if (tld.Length < 2)
            {
                MarcarError(txtEmpEmail, "La extensión del dominio debe tener al menos 2 caracteres (ej: .com, .ec).");
                return false;
            }

            MarcarOk(txtEmpEmail);
            return true;
        }

        // Validaciones Secuenciales
        private bool ValidarSecTipoDoc()
        {
            string tipo = txtSecTipoDoc.Text.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(tipo))
            {
                MarcarError(txtSecTipoDoc, "El tipo de documento es obligatorio.");
                return false;
            }

            if (tipo.Length < 3 || tipo.Length > 20)
            {
                MarcarError(txtSecTipoDoc, "El tipo de documento debe tener entre 3 y 20 caracteres.");
                return false;
            }

            MarcarOk(txtSecTipoDoc);
            return true;
        }

        private bool ValidarSecEstablecimiento()
        {
            string est = txtSecEstablecimiento.Text.Trim();

            if (string.IsNullOrWhiteSpace(est))
            {
                MarcarError(txtSecEstablecimiento, "El establecimiento es obligatorio.");
                return false;
            }

            if (est.Length != 3 || !est.All(char.IsDigit))
            {
                MarcarError(txtSecEstablecimiento, "El establecimiento debe tener 3 dígitos (ej: 001).");
                return false;
            }

            MarcarOk(txtSecEstablecimiento);
            return true;
        }

        private bool ValidarSecPuntoEmision()
        {
            string pto = txtSecPuntoEmision.Text.Trim();

            if (string.IsNullOrWhiteSpace(pto))
            {
                MarcarError(txtSecPuntoEmision, "El punto de emisión es obligatorio.");
                return false;
            }

            if (pto.Length != 3 || !pto.All(char.IsDigit))
            {
                MarcarError(txtSecPuntoEmision, "El punto de emisión debe tener 3 dígitos (ej: 001).");
                return false;
            }

            MarcarOk(txtSecPuntoEmision);
            return true;
        }

        private bool ValidarSecSecuencia()
        {
            if (nudSecActual.Value < 0)
            {
                MarcarError(nudSecActual, "La secuencia no puede ser negativa.");
                return false;
            }

            MarcarOk(nudSecActual);
            return true;
        }

        private bool ValidarImpuestoTodo()
        {
            return ValidarImpuestoNombre() && ValidarImpuestoPorcentaje() && ValidarImpuestoCodigoSri();
        }

        private bool ValidarMpTodo()
        {
            return ValidarMpCodigoSri() && ValidarMpNombre();
        }

        private bool ValidarEmpresaTodo()
        {
            return ValidarEmpresaNombre() && ValidarEmpresaRuc() && ValidarEmpresaTelefono() && ValidarEmpresaEmail();
        }

        private bool ValidarSecTodo()
        {
            return ValidarSecTipoDoc() && ValidarSecEstablecimiento() && ValidarSecPuntoEmision() && ValidarSecSecuencia();
        }

        private void LimpiarValidaciones()
        {
            // Impuestos
            MarcarOk(txtImpNombre);
            MarcarOk(nudImpPorcentaje);
            MarcarOk(txtImpCodigoSri);

            // Métodos Pago
            MarcarOk(txtMpCodigoSri);
            MarcarOk(txtMpNombre);

            // Empresa
            MarcarOk(txtEmpNombre);
            MarcarOk(txtEmpRuc);
            MarcarOk(txtEmpTelefono);
            MarcarOk(txtEmpEmail);

            // Secuenciales
            MarcarOk(txtSecTipoDoc);
            MarcarOk(txtSecEstablecimiento);
            MarcarOk(txtSecPuntoEmision);
            MarcarOk(nudSecActual);
        }

        private void FormConfiguracion_Load(object sender, EventArgs e)
        {
            // estilo grids
            GridStyle(dgvImpuestos);
            GridStyle(dgvMetodosPago);
            GridStyle(dgvSecuenciales);

            // defaults
            nudImpPorcentaje.DecimalPlaces = 2;
            nudImpPorcentaje.Minimum = 0;
            nudImpPorcentaje.Maximum = 100;
            nudImpPorcentaje.Increment = 0.5M;

            nudSecActual.Minimum = 0;
            nudSecActual.Maximum = 999999999;

            // cargar todo
            Imp_SetModo(false);
            MP_SetModo(false);
            Sec_SetModo(false);

            Imp_Cargar();
            MP_Cargar();
            Emp_Cargar();
            Sec_Cargar();
        }

        private void GridStyle(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = true;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ScrollBars = ScrollBars.Both;
            dgv.ColumnHeadersHeight = 36;
        }

        // IMPUESTOS

        private void Imp_SetModo(bool activo)
        {
            txtImpNombre.Enabled = activo;
            nudImpPorcentaje.Enabled = activo;
            txtImpCodigoSri.Enabled = activo;
            swImpActivo.Enabled = activo;
            btnImpGuardar.Enabled = activo;
            btnImpActivarDesactivar.Enabled = (_impId != 0);

            if (!activo)
            {
                LimpiarValidaciones();
            }
        }

        private void Imp_LimpiarCampos()
        {
            txtImpNombre.Text = "";
            nudImpPorcentaje.Value = 0;
            txtImpCodigoSri.Text = "";
            swImpActivo.Checked = true;
            _impId = 0;
        }

        private void Imp_Nuevo()
        {
            Imp_LimpiarCampos();
            Imp_SetModo(true);
            btnImpActivarDesactivar.Enabled = false;
            txtImpNombre.Focus();
        }

        private void Imp_Cargar()
        {
            try
            {
                string buscar = (txtImpBuscar.Text ?? "").Trim();

                string sql = @"
SELECT id AS Id, nombre AS Nombre, porcentaje AS Porcentaje, codigo_sri AS CodigoSRI, ISNULL(activo,1) AS Activo
FROM Impuestos
WHERE (@b='' OR nombre LIKE '%' + @b + '%' OR codigo_sri LIKE '%' + @b + '%')
ORDER BY id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvImpuestos.DataSource = dt;
                }

                if (dgvImpuestos.Columns["Id"] != null) dgvImpuestos.Columns["Id"].Visible = false;
                if (dgvImpuestos.Columns["Porcentaje"] != null) dgvImpuestos.Columns["Porcentaje"].DefaultCellStyle.Format = "N2";

                Imp_SetModo(false);
                Imp_LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar impuestos:\n" + ex.Message, "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Imp_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvImpuestos.Rows[e.RowIndex];

            _impId = Convert.ToInt64(row.Cells["Id"].Value);
            txtImpNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
            txtImpCodigoSri.Text = row.Cells["CodigoSRI"].Value?.ToString() ?? "";
            nudImpPorcentaje.Value = Convert.ToDecimal(row.Cells["Porcentaje"].Value ?? 0);
            swImpActivo.Checked = Convert.ToBoolean(row.Cells["Activo"].Value);

            Imp_SetModo(true);
        }

        private void Imp_Guardar()
        {
            if (!ValidarImpuestoTodo())
            {
                MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string nombre = (txtImpNombre.Text ?? "").Trim();
                string codigo = (txtImpCodigoSri.Text ?? "").Trim();
                decimal porcentaje = nudImpPorcentaje.Value;
                bool activo = swImpActivo.Checked;

                using (var cn = con.CrearConexionAbierta())
                {
                    string dup = @"SELECT COUNT(*) FROM Impuestos WHERE (nombre=@n OR codigo_sri=@c) AND (@id=0 OR id<>@id);";
                    using (var chk = new SqlCommand(dup, cn))
                    {
                        chk.Parameters.AddWithValue("@n", nombre);
                        chk.Parameters.AddWithValue("@c", codigo);
                        chk.Parameters.AddWithValue("@id", _impId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe un impuesto con ese nombre o código.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_impId == 0)
                    {
                        string ins = @"INSERT INTO Impuestos(nombre,porcentaje,codigo_sri,activo) VALUES(@n,@p,@c,@a);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@p", porcentaje);
                            cmd.Parameters.AddWithValue("@c", codigo);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"UPDATE Impuestos SET nombre=@n,porcentaje=@p,codigo_sri=@c,activo=@a WHERE id=@id;";
                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@p", porcentaje);
                            cmd.Parameters.AddWithValue("@c", codigo);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.Parameters.AddWithValue("@id", _impId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Guardado correctamente.", "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Imp_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Imp_ActivarDesactivar()
        {
            try
            {
                if (_impId == 0) return;

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(
                    "UPDATE Impuestos SET activo = CASE WHEN ISNULL(activo,1)=1 THEN 0 ELSE 1 END WHERE id=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _impId);
                    cmd.ExecuteNonQuery();
                }

                Imp_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // METODOS PAGO

        private void MP_SetModo(bool activo)
        {
            txtMpCodigoSri.Enabled = activo;
            txtMpNombre.Enabled = activo;
            swMpActivo.Enabled = activo;
            btnMpGuardar.Enabled = activo;
            btnMpActivarDesactivar.Enabled = (_mpId != 0);

            if (!activo)
            {
                LimpiarValidaciones();
            }
        }

        private void MP_LimpiarCampos()
        {
            txtMpCodigoSri.Text = "";
            txtMpNombre.Text = "";
            swMpActivo.Checked = true;
            _mpId = 0;
        }

        private void MP_Nuevo()
        {
            MP_LimpiarCampos();
            MP_SetModo(true);
            btnMpActivarDesactivar.Enabled = false;
            txtMpCodigoSri.Focus();
        }

        private void MP_Cargar()
        {
            try
            {
                string buscar = (txtMpBuscar.Text ?? "").Trim();

                string sql = @"
SELECT id AS Id, codigo_sri AS CodigoSRI, nombre AS Nombre, ISNULL(activo,1) AS Activo
FROM MetodosPago
WHERE (@b='' OR nombre LIKE '%' + @b + '%' OR codigo_sri LIKE '%' + @b + '%')
ORDER BY id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvMetodosPago.DataSource = dt;
                }

                if (dgvMetodosPago.Columns["Id"] != null) dgvMetodosPago.Columns["Id"].Visible = false;

                MP_SetModo(false);
                MP_LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar métodos de pago:\n" + ex.Message, "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MP_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvMetodosPago.Rows[e.RowIndex];

            _mpId = Convert.ToInt64(row.Cells["Id"].Value);
            txtMpCodigoSri.Text = row.Cells["CodigoSRI"].Value?.ToString() ?? "";
            txtMpNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
            swMpActivo.Checked = Convert.ToBoolean(row.Cells["Activo"].Value);

            MP_SetModo(true);
        }

        private void MP_Guardar()
        {
            if (!ValidarMpTodo())
            {
                MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string cod = (txtMpCodigoSri.Text ?? "").Trim();
                string nom = (txtMpNombre.Text ?? "").Trim();
                bool activo = swMpActivo.Checked;

                using (var cn = con.CrearConexionAbierta())
                {
                    string dup = @"SELECT COUNT(*) FROM MetodosPago WHERE codigo_sri=@c AND (@id=0 OR id<>@id);";
                    using (var chk = new SqlCommand(dup, cn))
                    {
                        chk.Parameters.AddWithValue("@c", cod);
                        chk.Parameters.AddWithValue("@id", _mpId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe un método con ese código SRI.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_mpId == 0)
                    {
                        string ins = @"INSERT INTO MetodosPago(codigo_sri,nombre,activo) VALUES(@c,@n,@a);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@c", cod);
                            cmd.Parameters.AddWithValue("@n", nom);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"UPDATE MetodosPago SET codigo_sri=@c,nombre=@n,activo=@a WHERE id=@id;";
                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@c", cod);
                            cmd.Parameters.AddWithValue("@n", nom);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.Parameters.AddWithValue("@id", _mpId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Guardado correctamente.", "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MP_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MP_ActivarDesactivar()
        {
            try
            {
                if (_mpId == 0) return;

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(
                    "UPDATE MetodosPago SET activo = CASE WHEN ISNULL(activo,1)=1 THEN 0 ELSE 1 END WHERE id=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _mpId);
                    cmd.ExecuteNonQuery();
                }

                MP_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EMPRESA

        private void CargarImagenEmpresa()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 logo FROM Empresa WHERE id = 1;", cn))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        _imagenEmpresaBytes = (byte[])result;
                        using (MemoryStream ms = new MemoryStream(_imagenEmpresaBytes))
                        {
                            picEmpresa.Image = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        picEmpresa.Image = null;
                        picEmpresa.BackColor = Color.LightGray;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la imagen: " + ex.Message);
            }
        }

        private void btnSeleccionarImagen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                ofd.Title = "Seleccionar logo de la empresa";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        picEmpresa.Image = Image.FromFile(ofd.FileName);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            picEmpresa.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            _imagenEmpresaBytes = ms.ToArray();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al cargar la imagen: " + ex.Message);
                    }
                }
            }
        }

        private void btnEliminarImagen_Click(object sender, EventArgs e)
        {
            picEmpresa.Image = null;
            picEmpresa.BackColor = Color.LightGray;
            _imagenEmpresaBytes = null;
        }

        private void Emp_Guardar()
        {
            if (!ValidarEmpresaTodo())
            {
                MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string nombre = (txtEmpNombre.Text ?? "").Trim();
                string ruc = (txtEmpRuc.Text ?? "").Trim();
                string dir = (txtEmpDireccion.Text ?? "").Trim();
                string tel = (txtEmpTelefono.Text ?? "").Trim();
                string email = (txtEmpEmail.Text ?? "").Trim();

                using (var cn = con.CrearConexionAbierta())
                {
                    int existe;
                    using (var chk = new SqlCommand("SELECT COUNT(*) FROM Empresa;", cn))
                        existe = Convert.ToInt32(chk.ExecuteScalar());

                    if (existe == 0)
                    {
                        string ins = @"INSERT INTO Empresa(id, nombre, ruc, direccion, telefono, email, logo)
                               VALUES(1, @n, @r, @d, @t, @e, @logo);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@r", (object)ruc ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@d", (object)dir ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@t", (object)tel ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@e", (object)email ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@logo", (object)_imagenEmpresaBytes ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"
UPDATE Empresa 
SET nombre = @n, 
    ruc = @r, 
    direccion = @d, 
    telefono = @t, 
    email = @e,
    logo = @logo
WHERE id = (SELECT TOP 1 id FROM Empresa ORDER BY id);";

                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@r", string.IsNullOrWhiteSpace(ruc) ? (object)DBNull.Value : ruc);
                            cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(dir) ? (object)DBNull.Value : dir);
                            cmd.Parameters.AddWithValue("@t", string.IsNullOrWhiteSpace(tel) ? (object)DBNull.Value : tel);
                            cmd.Parameters.AddWithValue("@e", string.IsNullOrWhiteSpace(email) ? (object)DBNull.Value : email);
                            cmd.Parameters.AddWithValue("@logo", (object)_imagenEmpresaBytes ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                LogoEmpresa.ActualizarLogo(_imagenEmpresaBytes);

                MessageBox.Show("Empresa actualizada.", "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Emp_Cargar();
                CargarEmpresaEnLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar empresa:\n" + ex.Message, "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Emp_Cargar()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 id, nombre, ruc, direccion, telefono, email, logo FROM Empresa ORDER BY id;", cn))
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtEmpNombre.Text = rd["nombre"]?.ToString() ?? "";
                        txtEmpRuc.Text = rd["ruc"]?.ToString() ?? "";
                        txtEmpDireccion.Text = rd["direccion"]?.ToString() ?? "";
                        txtEmpTelefono.Text = rd["telefono"]?.ToString() ?? "";
                        txtEmpEmail.Text = rd["email"]?.ToString() ?? "";

                        if (rd["logo"] != DBNull.Value)
                        {
                            _imagenEmpresaBytes = (byte[])rd["logo"];
                            using (MemoryStream ms = new MemoryStream(_imagenEmpresaBytes))
                            {
                                picEmpresa.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            picEmpresa.Image = null;
                            picEmpresa.BackColor = Color.LightGray;
                            _imagenEmpresaBytes = null;
                        }
                    }
                    else
                    {
                        txtEmpNombre.Text = "";
                        txtEmpRuc.Text = "";
                        txtEmpDireccion.Text = "";
                        txtEmpTelefono.Text = "";
                        txtEmpEmail.Text = "";
                        picEmpresa.Image = null;
                        picEmpresa.BackColor = Color.LightGray;
                        _imagenEmpresaBytes = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empresa:\n" + ex.Message, "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEmpresaEnLabel()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(
                    "SELECT TOP 1 nombre, ruc, direccion, telefono, email, " +
                    "CASE WHEN logo IS NOT NULL THEN 1 ELSE 0 END AS tiene_logo FROM Empresa WHERE id = 1;", cn))
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                    {
                        lblEmpInfo.Text = "Empresa no configurada.";
                        return;
                    }

                    string nombre = rd["nombre"]?.ToString() ?? "";
                    string ruc = rd["ruc"]?.ToString() ?? "";
                    string dir = rd["direccion"]?.ToString() ?? "";
                    string tel = rd["telefono"]?.ToString() ?? "";
                    string mail = rd["email"]?.ToString() ?? "";

                    lblEmpInfo.Text =
                        $"{nombre}\n" +
                        $"RUC: {ruc} | Tel: {tel}\n" +
                        $"{dir}\n" +
                        $"{mail}\n";
                }
            }
            catch (Exception ex)
            {
                lblEmpInfo.Text = "No se pudo cargar la empresa.";
                MessageBox.Show("Error cargando Empresa: " + ex.Message, "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // SECUENCIALES

        private void Sec_SetModo(bool activo)
        {
            txtSecTipoDoc.Enabled = activo;
            txtSecEstablecimiento.Enabled = activo;
            txtSecPuntoEmision.Enabled = activo;
            nudSecActual.Enabled = activo;

            btnSecGuardar.Enabled = activo;
            btnSecEliminar.Enabled = (_secId != 0);

            if (!activo)
            {
                LimpiarValidaciones();
            }
        }

        private void Sec_LimpiarCampos()
        {
            _secId = 0;
            txtSecTipoDoc.Text = "";
            txtSecEstablecimiento.Text = "";
            txtSecPuntoEmision.Text = "";
            nudSecActual.Value = 0;
        }

        private void Sec_Nuevo()
        {
            Sec_LimpiarCampos();
            Sec_SetModo(true);
            btnSecEliminar.Enabled = false;
            txtSecTipoDoc.Focus();
        }

        private void Sec_Cargar()
        {
            try
            {
                string buscar = (txtSecBuscar.Text ?? "").Trim();

                string sql = @"
SELECT id AS Id, tipo_documento AS TipoDocumento, establecimiento AS Establecimiento, punto_emision AS PuntoEmision, secuencia_actual AS SecuenciaActual
FROM Secuenciales
WHERE (@b='' OR tipo_documento LIKE '%' + @b + '%')
ORDER BY id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvSecuenciales.DataSource = dt;
                }

                if (dgvSecuenciales.Columns["Id"] != null) dgvSecuenciales.Columns["Id"].Visible = false;

                Sec_SetModo(false);
                Sec_LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar secuenciales:\n" + ex.Message, "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Sec_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvSecuenciales.Rows[e.RowIndex];

            _secId = Convert.ToInt32(row.Cells["Id"].Value);
            txtSecTipoDoc.Text = row.Cells["TipoDocumento"].Value?.ToString() ?? "";
            txtSecEstablecimiento.Text = row.Cells["Establecimiento"].Value?.ToString() ?? "";
            txtSecPuntoEmision.Text = row.Cells["PuntoEmision"].Value?.ToString() ?? "";
            nudSecActual.Value = Convert.ToDecimal(row.Cells["SecuenciaActual"].Value ?? 0);

            Sec_SetModo(true);
        }

        private void Sec_Guardar()
        {
            if (!ValidarSecTodo())
            {
                MessageBox.Show("Corrija los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string tipo = (txtSecTipoDoc.Text ?? "").Trim().ToUpper();
                string est = (txtSecEstablecimiento.Text ?? "").Trim();
                string pto = (txtSecPuntoEmision.Text ?? "").Trim();
                int sec = Convert.ToInt32(nudSecActual.Value);

                using (var cn = con.CrearConexionAbierta())
                {
                    string dup = @"
SELECT COUNT(*) FROM Secuenciales
WHERE tipo_documento=@t AND establecimiento=@e AND punto_emision=@p
AND (@id=0 OR id<>@id);";

                    using (var chk = new SqlCommand(dup, cn))
                    {
                        chk.Parameters.AddWithValue("@t", tipo);
                        chk.Parameters.AddWithValue("@e", est);
                        chk.Parameters.AddWithValue("@p", pto);
                        chk.Parameters.AddWithValue("@id", _secId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe un secuencial para ese documento/establecimiento/punto.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_secId == 0)
                    {
                        string ins = @"INSERT INTO Secuenciales(tipo_documento,establecimiento,punto_emision,secuencia_actual)
                                       VALUES(@t,@e,@p,@s);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@t", tipo);
                            cmd.Parameters.AddWithValue("@e", est);
                            cmd.Parameters.AddWithValue("@p", pto);
                            cmd.Parameters.AddWithValue("@s", sec);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"UPDATE Secuenciales SET tipo_documento=@t,establecimiento=@e,punto_emision=@p,secuencia_actual=@s
                                       WHERE id=@id;";
                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@t", tipo);
                            cmd.Parameters.AddWithValue("@e", est);
                            cmd.Parameters.AddWithValue("@p", pto);
                            cmd.Parameters.AddWithValue("@s", sec);
                            cmd.Parameters.AddWithValue("@id", _secId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Secuencial guardado.", "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Sec_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Sec_Eliminar()
        {
            try
            {
                if (_secId == 0) return;

                var r = MessageBox.Show("¿Eliminar este secuencial?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r != DialogResult.Yes) return;

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("DELETE FROM Secuenciales WHERE id=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _secId);
                    cmd.ExecuteNonQuery();
                }

                Sec_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormConfiguracion_Load_1(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvImpuestos);
            DataGridViewEstilo.AplicarEstiloDashboard(dgvMetodosPago);
            DataGridViewEstilo.AplicarEstiloDashboard(dgvSecuenciales);



        }

        // ==================== DESCUENTOS ====================

        private long _descuentoId = 0;

        private void ConfigurarEventosDescuentos()
        {
            // Agregar eventos para los controles de descuentos
            btnDescuentoNuevo.Click += (s, e) => Descuento_Nuevo();
            btnDescuentoGuardar.Click += (s, e) => Descuento_Guardar();
            btnDescuentoEliminar.Click += (s, e) => Descuento_Eliminar();
            btnDescuentoBuscar.Click += (s, e) => Descuento_Cargar();
            btnDescuentoLimpiar.Click += (s, e) => { txtDescuentoBuscar.Text = ""; Descuento_Cargar(); };
            dgvDescuentos.CellClick += Descuento_GridClick;
        }

        private void Descuento_Nuevo()
        {
            _descuentoId = 0;
            txtDescuentoNombre.Text = "";
            cmbDescuentoTipo.SelectedIndex = 0;
            nudDescuentoPorcentaje.Value = 0;
            dtpDescuentoInicio.Value = DateTime.Now;
            dtpDescuentoFin.Value = DateTime.Now.AddMonths(1);
            swDescuentoActivo.Checked = true;
            HabilitarControlesDescuento(true);
            txtDescuentoNombre.Focus();
        }

        private void Descuento_Guardar()
        {
            if (string.IsNullOrWhiteSpace(txtDescuentoNombre.Text))
            {
                MessageBox.Show("Ingrese un nombre para el descuento.");
                return;
            }

            if (nudDescuentoPorcentaje.Value <= 0 || nudDescuentoPorcentaje.Value > 100)
            {
                MessageBox.Show("El porcentaje debe estar entre 1 y 100.");
                return;
            }

            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    if (_descuentoId == 0)
                    {
                        string sql = @"INSERT INTO Descuentos (nombre, tipo_aplicacion, porcentaje, fecha_inicio, fecha_fin, activo)
                               VALUES (@nombre, @tipo, @porcentaje, @inicio, @fin, @activo)";
                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@nombre", txtDescuentoNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@tipo", cmbDescuentoTipo.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@porcentaje", nudDescuentoPorcentaje.Value);
                            cmd.Parameters.AddWithValue("@inicio", dtpDescuentoInicio.Value.Date);
                            cmd.Parameters.AddWithValue("@fin", dtpDescuentoFin.Value.Date);
                            cmd.Parameters.AddWithValue("@activo", swDescuentoActivo.Checked ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sql = @"UPDATE Descuentos SET nombre=@nombre, tipo_aplicacion=@tipo, porcentaje=@porcentaje,
                               fecha_inicio=@inicio, fecha_fin=@fin, activo=@activo WHERE id=@id";
                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            cmd.Parameters.AddWithValue("@nombre", txtDescuentoNombre.Text.Trim());
                            cmd.Parameters.AddWithValue("@tipo", cmbDescuentoTipo.SelectedItem.ToString());
                            cmd.Parameters.AddWithValue("@porcentaje", nudDescuentoPorcentaje.Value);
                            cmd.Parameters.AddWithValue("@inicio", dtpDescuentoInicio.Value.Date);
                            cmd.Parameters.AddWithValue("@fin", dtpDescuentoFin.Value.Date);
                            cmd.Parameters.AddWithValue("@activo", swDescuentoActivo.Checked ? 1 : 0);
                            cmd.Parameters.AddWithValue("@id", _descuentoId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Descuento guardado correctamente.");
                Descuento_Cargar();
                Descuento_Nuevo();
                HabilitarControlesDescuento(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void Descuento_Eliminar()
        {
            if (_descuentoId == 0) return;

            var result = MessageBox.Show("¿Eliminar este descuento?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("DELETE FROM Descuentos WHERE id=@id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _descuentoId);
                    cmd.ExecuteNonQuery();
                }
                Descuento_Cargar();
                Descuento_Nuevo();
                HabilitarControlesDescuento(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void Descuento_Cargar()
        {
            try
            {
                string buscar = txtDescuentoBuscar.Text.Trim();
                string sql = @"SELECT id, nombre, tipo_aplicacion, porcentaje, fecha_inicio, fecha_fin, 
                              CASE WHEN activo=1 THEN 'Activo' ELSE 'Inactivo' END AS estado
                       FROM Descuentos 
                       WHERE @b='' OR nombre LIKE '%' + @b + '%'
                       ORDER BY id DESC";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvDescuentos.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar descuentos: " + ex.Message);
            }
        }

        private void Descuento_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvDescuentos.Rows[e.RowIndex];

            _descuentoId = Convert.ToInt32(row.Cells["id"].Value);
            txtDescuentoNombre.Text = row.Cells["nombre"].Value?.ToString() ?? "";

            string tipo = row.Cells["tipo_aplicacion"].Value?.ToString() ?? "";
            cmbDescuentoTipo.SelectedItem = tipo;

            nudDescuentoPorcentaje.Value = Convert.ToDecimal(row.Cells["porcentaje"].Value ?? 0);
            dtpDescuentoInicio.Value = Convert.ToDateTime(row.Cells["fecha_inicio"].Value ?? DateTime.Now);
            dtpDescuentoFin.Value = Convert.ToDateTime(row.Cells["fecha_fin"].Value ?? DateTime.Now);
            swDescuentoActivo.Checked = row.Cells["estado"].Value?.ToString() == "Activo";

            HabilitarControlesDescuento(true);
            btnDescuentoEliminar.Enabled = true;
        }

        private void HabilitarControlesDescuento(bool habilitar)
        {
            txtDescuentoNombre.Enabled = habilitar;
            cmbDescuentoTipo.Enabled = habilitar;
            nudDescuentoPorcentaje.Enabled = habilitar;
            dtpDescuentoInicio.Enabled = habilitar;
            dtpDescuentoFin.Enabled = habilitar;
            swDescuentoActivo.Enabled = habilitar;
            btnDescuentoGuardar.Enabled = habilitar;

            if (!habilitar)
            {
                btnDescuentoEliminar.Enabled = false;
            }
        }

        // ==================== PROMOCIONES ====================

        private long _promocionId = 0;

        private void ConfigurarEventosPromociones()
        {
            btnPromocionNuevo.Click += (s, e) => Promocion_Nuevo();
            btnPromocionGuardar.Click += (s, e) => Promocion_Guardar();
            btnPromocionEliminar.Click += (s, e) => Promocion_Eliminar();
            btnPromocionBuscar.Click += (s, e) => Promocion_Cargar();
            btnPromocionLimpiar.Click += (s, e) => { txtPromocionBuscar.Text = ""; Promocion_Cargar(); };
            dgvPromociones.CellClick += Promocion_GridClick;

            cmbPromocionTipo.SelectedIndexChanged += (s, e) => Promocion_CambiarTipo();
        }

        private void Promocion_CambiarTipo()
        {
            string tipo = cmbPromocionTipo.SelectedItem?.ToString() ?? "";

            
        }

        private void Promocion_Nuevo()
        {
            _promocionId = 0;
            txtPromocionNombre.Text = "";
            txtPromocionDescripcion.Text = "";
            cmbPromocionTipo.SelectedIndex = 0;

            // Resetear Combo
            cmbPromocionDiaSemana.SelectedIndex = 0;
            dtpPromocionFechaEspecifica.Value = DateTime.Now;

            // Resetear Combo
            cmbPromocionProductoPrincipal.SelectedIndex = -1;
            cmbPromocionServicioPrincipal.SelectedIndex = -1;
            cmbPromocionProductoObsequio.SelectedIndex = -1;
            cmbPromocionServicioObsequio.SelectedIndex = -1;
            nudPromocionCantidadObsequio.Value = 1;

            dtpPromocionInicio.Value = DateTime.Now;
            dtpPromocionFin.Value = DateTime.Now.AddMonths(3);
            swPromocionActivo.Checked = true;

            Promocion_CambiarTipo();
            HabilitarControlesPromocion(true);
            txtPromocionNombre.Focus();
        }

        private void Promocion_Guardar()
        {
            if (string.IsNullOrWhiteSpace(txtPromocionNombre.Text))
            {
                MessageBox.Show("Ingrese un nombre para la promoción.");
                return;
            }

            string tipo = cmbPromocionTipo.SelectedItem?.ToString() ?? "";

            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    if (_promocionId == 0)
                    {
                        string sql = @"INSERT INTO Promociones (nombre, descripcion, tipo, dia_semana, fecha_especifica,
                               producto_principal_id, servicio_principal_id, producto_obsequio_id, 
                               servicio_obsequio_id, cantidad_obsequio, fecha_inicio, fecha_fin, activo)
                               VALUES (@nombre, @descripcion, @tipo, @diaSemana, @fechaEsp, @prodPrincipal, 
                               @servPrincipal, @prodObsequio, @servObsequio, @cantObsequio, @inicio, @fin, @activo)";

                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            AgregarParametrosPromocion(cmd, tipo);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string sql = @"UPDATE Promociones SET nombre=@nombre, descripcion=@descripcion, tipo=@tipo, 
                               dia_semana=@diaSemana, fecha_especifica=@fechaEsp, producto_principal_id=@prodPrincipal,
                               servicio_principal_id=@servPrincipal, producto_obsequio_id=@prodObsequio,
                               servicio_obsequio_id=@servObsequio, cantidad_obsequio=@cantObsequio,
                               fecha_inicio=@inicio, fecha_fin=@fin, activo=@activo WHERE id=@id";

                        using (var cmd = new SqlCommand(sql, cn))
                        {
                            AgregarParametrosPromocion(cmd, tipo);
                            cmd.Parameters.AddWithValue("@id", _promocionId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Promoción guardada correctamente.");
                Promocion_Cargar();
                Promocion_Nuevo();
                HabilitarControlesPromocion(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void AgregarParametrosPromocion(SqlCommand cmd, string tipo)
        {
            cmd.Parameters.AddWithValue("@nombre", txtPromocionNombre.Text.Trim());
            cmd.Parameters.AddWithValue("@descripcion", (object)txtPromocionDescripcion.Text.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@tipo", tipo);

            // DiaSemana
            int? diaSemana = tipo == "DiaSemana" ? cmbPromocionDiaSemana.SelectedIndex + 1 : (int?)null;
            cmd.Parameters.AddWithValue("@diaSemana", (object)diaSemana ?? DBNull.Value);

            // FechaEspecifica
            DateTime? fechaEsp = tipo == "FechaEspecifica" ? dtpPromocionFechaEspecifica.Value.Date : (DateTime?)null;
            cmd.Parameters.AddWithValue("@fechaEsp", (object)fechaEsp ?? DBNull.Value);

            // Combo
            long? prodPrincipal = cmbPromocionProductoPrincipal.SelectedValue != null ?
                Convert.ToInt64(cmbPromocionProductoPrincipal.SelectedValue) : (long?)null;
            long? servPrincipal = cmbPromocionServicioPrincipal.SelectedValue != null ?
                Convert.ToInt64(cmbPromocionServicioPrincipal.SelectedValue) : (long?)null;
            long? prodObsequio = cmbPromocionProductoObsequio.SelectedValue != null ?
                Convert.ToInt64(cmbPromocionProductoObsequio.SelectedValue) : (long?)null;
            long? servObsequio = cmbPromocionServicioObsequio.SelectedValue != null ?
                Convert.ToInt64(cmbPromocionServicioObsequio.SelectedValue) : (long?)null;

            cmd.Parameters.AddWithValue("@prodPrincipal", (object)prodPrincipal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@servPrincipal", (object)servPrincipal ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@prodObsequio", (object)prodObsequio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@servObsequio", (object)servObsequio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@cantObsequio", nudPromocionCantidadObsequio.Value);
            cmd.Parameters.AddWithValue("@inicio", dtpPromocionInicio.Value.Date);
            cmd.Parameters.AddWithValue("@fin", dtpPromocionFin.Value.Date);
            cmd.Parameters.AddWithValue("@activo", swPromocionActivo.Checked ? 1 : 0);
        }

        private void Promocion_Eliminar()
        {
            if (_promocionId == 0) return;

            var result = MessageBox.Show("¿Eliminar esta promoción?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("DELETE FROM Promociones WHERE id=@id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _promocionId);
                    cmd.ExecuteNonQuery();
                }
                Promocion_Cargar();
                Promocion_Nuevo();
                HabilitarControlesPromocion(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }

        private void Promocion_Cargar()
        {
            try
            {
                string buscar = txtPromocionBuscar.Text.Trim();
                string sql = @"SELECT id, nombre, descripcion, tipo, 
                              CASE 
                                  WHEN tipo='DiaSemana' THEN 
                                      CASE dia_semana 
                                          WHEN 1 THEN 'Lunes' WHEN 2 THEN 'Martes' WHEN 3 THEN 'Miércoles'
                                          WHEN 4 THEN 'Jueves' WHEN 5 THEN 'Viernes' WHEN 6 THEN 'Sábado'
                                          WHEN 7 THEN 'Domingo' ELSE ''
                                      END
                                  WHEN tipo='FechaEspecifica' THEN CONVERT(VARCHAR, fecha_especifica, 103)
                                  WHEN tipo='Combo' THEN 'Combo'
                                  ELSE ''
                              END AS detalle,
                              fecha_inicio, fecha_fin,
                              CASE WHEN activo=1 THEN 'Activo' ELSE 'Inactivo' END AS estado
                       FROM Promociones 
                       WHERE @b='' OR nombre LIKE '%' + @b + '%'
                       ORDER BY id DESC";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvPromociones.DataSource = dt;
                }

                // Cargar combos de productos y servicios para promociones
                CargarCombosPromocion();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar promociones: " + ex.Message);
            }
        }

        private void CargarCombosPromocion()
        {
            try
            {
                // Productos
                DataTable dtProductos = new DataTable();
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT id, nombre FROM Productos ORDER BY nombre", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtProductos);
                }

                cmbPromocionProductoPrincipal.DataSource = dtProductos.Copy();
                cmbPromocionProductoPrincipal.DisplayMember = "nombre";
                cmbPromocionProductoPrincipal.ValueMember = "id";

                cmbPromocionProductoObsequio.DataSource = dtProductos.Copy();
                cmbPromocionProductoObsequio.DisplayMember = "nombre";
                cmbPromocionProductoObsequio.ValueMember = "id";

                // Servicios
                DataTable dtServicios = new DataTable();
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT id, nombre FROM Servicios WHERE activo=1 ORDER BY nombre", cn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dtServicios);
                }

                cmbPromocionServicioPrincipal.DataSource = dtServicios.Copy();
                cmbPromocionServicioPrincipal.DisplayMember = "nombre";
                cmbPromocionServicioPrincipal.ValueMember = "id";

                cmbPromocionServicioObsequio.DataSource = dtServicios.Copy();
                cmbPromocionServicioObsequio.DisplayMember = "nombre";
                cmbPromocionServicioObsequio.ValueMember = "id";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error cargando combos: " + ex.Message);
            }
        }

        private void Promocion_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvPromociones.Rows[e.RowIndex];

            _promocionId = Convert.ToInt32(row.Cells["id"].Value);
            txtPromocionNombre.Text = row.Cells["nombre"].Value?.ToString() ?? "";
            txtPromocionDescripcion.Text = row.Cells["descripcion"].Value?.ToString() ?? "";

            string tipo = row.Cells["tipo"].Value?.ToString() ?? "";
            cmbPromocionTipo.SelectedItem = tipo;

            Promocion_CambiarTipo();

            // Cargar datos específicos según tipo (necesitarías cargar desde BD los valores)
            HabilitarControlesPromocion(true);
            btnPromocionEliminar.Enabled = true;
        }

        private void HabilitarControlesPromocion(bool habilitar)
        {
            txtPromocionNombre.Enabled = habilitar;
            txtPromocionDescripcion.Enabled = habilitar;
            cmbPromocionTipo.Enabled = habilitar;
            dtpPromocionInicio.Enabled = habilitar;
            dtpPromocionFin.Enabled = habilitar;
            swPromocionActivo.Enabled = habilitar;
            btnPromocionGuardar.Enabled = habilitar;

            if (!habilitar)
            {
                btnPromocionEliminar.Enabled = false;
            }
        }

        private void cmbPromocionProductoPrincipal_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmbPromocionServicioPrincipal_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
    }
}