using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Net.Mail;


namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormProveePopup : Form
    {
        private readonly Conexion con = new Conexion();
        private readonly long? proveedorId; 

        public FormProveePopup(long? proveedorId)
        {
            InitializeComponent();

            this.proveedorId = proveedorId;

            AplicarEstilos();

            btnGuardar.Click += (s, e) => Guardar();
            btnLimpiar.Click += (s, e) => Limpiar();
            btnDesactivar.Click += (s, e) => Desactivar();
            txtRuc.KeyPress += SoloNumeros_KeyPress;
            txtTelefono.KeyPress += Telefono_KeyPress;

            txtRuc.TextChanged += (s, e) => ValidarRucLive();
            txtEmail.TextChanged += (s, e) => ValidarEmailLive();
            txtTelefono.TextChanged += (s, e) => ValidarTelefonoLive();
            txtNombreEmpresa.TextChanged += (s, e) => ValidarNombreLive();

            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;



            if (btnCerrar != null) btnCerrar.Click += (s, e) => Close();

            if (proveedorId.HasValue)
                CargarProveedor(proveedorId.Value);
            else
            {
                Limpiar();
                btnDesactivar.Enabled = false; 
            }
        }

        private void AplicarEstilos()
        {
            

            EstiloBoton(btnGuardar, Color.FromArgb(0, 123, 255));
            EstiloBoton(btnLimpiar, Color.DarkSlateBlue);
            EstiloBoton(btnDesactivar, Color.FromArgb(220, 60, 60));

            chkActivo.Checked = true;
            chkEspecial.Checked = false;

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
        }

        private void EstiloBoton(Button btn, Color color)
        {
            if (btn == null) return;

            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Height = Math.Max(btn.Height, 40);
            btn.Cursor = Cursors.Hand;
        }

        private void CargarProveedor(long id)
        {
            try
            {
                con.Abrir();
                string sql = @"
SELECT id, ruc, nombre_empresa, contacto_nombre, telefono, email, estado, contribuyente_especial
FROM dbo.Proveedores
WHERE id=@id;";

                using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                        {
                            MessageBox.Show("Proveedor no encontrado.");
                            DialogResult = DialogResult.Cancel;
                            Close();
                            return;
                        }

                        txtRuc.Text = dr["ruc"].ToString().Trim();
                        txtNombreEmpresa.Text = dr["nombre_empresa"].ToString();
                        txtContacto.Text = dr["contacto_nombre"].ToString();
                        txtTelefono.Text = dr["telefono"].ToString();
                        txtEmail.Text = dr["email"].ToString();
                        chkActivo.Checked = Convert.ToBoolean(dr["estado"]);
                        chkEspecial.Checked = Convert.ToBoolean(dr["contribuyente_especial"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proveedor: " + ex.Message);
                DialogResult = DialogResult.Cancel;
                Close();
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void Guardar()
        {

            ValidarRucLive();
            ValidarNombreLive();
            ValidarTelefonoLive();
            ValidarEmailLive();

            
            if (!string.IsNullOrEmpty(errorProvider1.GetError(txtRuc)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtNombreEmpresa)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtTelefono)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtEmail)))
            {
                MessageBox.Show("Corrige los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ruc = (txtRuc.Text ?? "").Trim();
            string nombre = (txtNombreEmpresa.Text ?? "").Trim();

            if (ruc.Length != 13)
            {
                MessageBox.Show("El RUC debe tener 13 dígitos.");
                txtRuc.Focus();
                return;
            }

            if (nombre.Length < 2)
            {
                MessageBox.Show("El nombre de empresa es obligatorio.");
                txtNombreEmpresa.Focus();
                return;
            }

            try
            {
                con.Abrir();

                if (!proveedorId.HasValue)
                {
                    string sql = @"
INSERT INTO dbo.Proveedores
(ruc, nombre_empresa, contacto_nombre, telefono, email, estado, contribuyente_especial)
VALUES
(@ruc, @nom, @cont, @tel, @email, @estado, @esp);";

                    ValidarRucLive();
                    ValidarNombreLive();
                    ValidarTelefonoLive();
                    ValidarEmailLive();

                    if (!string.IsNullOrEmpty(errorProvider1.GetError(txtRuc)) ||
                        !string.IsNullOrEmpty(errorProvider1.GetError(txtNombreEmpresa)) ||
                        !string.IsNullOrEmpty(errorProvider1.GetError(txtTelefono)) ||
                        !string.IsNullOrEmpty(errorProvider1.GetError(txtEmail)))
                    {
                        MessageBox.Show("Corrige los campos marcados en rojo.");
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                    {
                        cmd.Parameters.AddWithValue("@ruc", ruc);
                        cmd.Parameters.AddWithValue("@nom", nombre);
                        cmd.Parameters.AddWithValue("@cont", string.IsNullOrWhiteSpace(txtContacto.Text) ? (object)DBNull.Value : txtContacto.Text.Trim());
                        cmd.Parameters.AddWithValue("@tel", string.IsNullOrWhiteSpace(txtTelefono.Text) ? (object)DBNull.Value : txtTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@estado", chkActivo.Checked);
                        cmd.Parameters.AddWithValue("@esp", chkEspecial.Checked);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Proveedor registrado ✅");
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }
                else
                {
                    string sql = @"
UPDATE dbo.Proveedores
SET
    ruc=@ruc,
    nombre_empresa=@nom,
    contacto_nombre=@cont,
    telefono=@tel,
    email=@email,
    estado=@estado,
    contribuyente_especial=@esp
WHERE id=@id;";


                    ValidarRucLive();
                    ValidarNombreLive();
                    ValidarTelefonoLive();
                    ValidarEmailLive();

                    if (!string.IsNullOrEmpty(errorProvider1.GetError(txtRuc)) ||
                        !string.IsNullOrEmpty(errorProvider1.GetError(txtNombreEmpresa)) ||
                        !string.IsNullOrEmpty(errorProvider1.GetError(txtTelefono)) ||
                        !string.IsNullOrEmpty(errorProvider1.GetError(txtEmail)))
                    {
                        MessageBox.Show("Corrige los campos marcados en rojo.");
                        return;
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                    {
                        cmd.Parameters.AddWithValue("@id", proveedorId.Value);
                        cmd.Parameters.AddWithValue("@ruc", ruc);
                        cmd.Parameters.AddWithValue("@nom", nombre);
                        cmd.Parameters.AddWithValue("@cont", string.IsNullOrWhiteSpace(txtContacto.Text) ? (object)DBNull.Value : txtContacto.Text.Trim());
                        cmd.Parameters.AddWithValue("@tel", string.IsNullOrWhiteSpace(txtTelefono.Text) ? (object)DBNull.Value : txtTelefono.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(txtEmail.Text) ? (object)DBNull.Value : txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@estado", chkActivo.Checked);
                        cmd.Parameters.AddWithValue("@esp", chkEspecial.Checked);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Proveedor actualizado ✅");
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error SQL: " + ex.Message);
                if (ex.Message.ToLower().Contains("unique") && ex.Message.ToLower().Contains("ruc"))
                {
                    MessageBox.Show("Ya existe un proveedor con ese RUC.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRuc.Focus();
                    return;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void Desactivar()
        {
            if (!proveedorId.HasValue)
            {
                MessageBox.Show("Primero guarda el proveedor.");
                return;
            }

            if (MessageBox.Show("¿Desactivar este proveedor?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();
                using (SqlCommand cmd = new SqlCommand("UPDATE dbo.Proveedores SET estado=0 WHERE id=@id", con.leer))
                {
                    cmd.Parameters.AddWithValue("@id", proveedorId.Value);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Proveedor desactivado ✅");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desactivar: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void Limpiar()
        {
            txtRuc.Clear();
            txtNombreEmpresa.Clear();
            txtContacto.Clear();
            txtTelefono.Clear();
            txtEmail.Clear();
            chkActivo.Checked = true;
            chkEspecial.Checked = false;
            txtRuc.Focus();
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            // solo dígitos y backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void Telefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            // permitir + solo al inicio
            if (e.KeyChar == '+' && ((TextBox)sender).SelectionStart == 0)
                return;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }


        

        private bool EmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);

                string dominio = addr.Host;

                if (!dominio.Contains("."))
                    return false;

                if (dominio.StartsWith(".") || dominio.EndsWith("."))
                    return false;

                string[] partes = dominio.Split('.');
                if (partes.Last().Length < 2)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }



        private bool EsCedulaEcuatorianaValida(string cedula)
        {
            if (cedula == null || cedula.Length != 10 || !cedula.All(char.IsDigit))
                return false;

            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24) return false;

            int tercer = cedula[2] - '0';
            if (tercer > 5) return false; // persona natural: 0..5

            int[] coef = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
            int suma = 0;

            for (int i = 0; i < 9; i++)
            {
                int dig = cedula[i] - '0';
                int prod = dig * coef[i];
                if (prod >= 10) prod -= 9;
                suma += prod;
            }

            int verificador = cedula[9] - '0';
            int decena = ((suma + 9) / 10) * 10;
            int resultado = decena - suma;
            if (resultado == 10) resultado = 0;

            return resultado == verificador;
        }

        private void MarcarCorrecto(TextBox txt)
        {
            txt.BackColor = Color.White;
            errorProvider1.SetError(txt, "");
        }

        private void MarcarError(TextBox txt, string mensaje)
        {
            txt.BackColor = Color.FromArgb(255, 220, 220); // rosado suave
            errorProvider1.SetError(txt, mensaje);
        }

        private void ValidarRucLive()
        {
            string ruc = txtRuc.Text.Trim();

            if (ruc.Length == 0)
            {
                MarcarError(txtRuc, "RUC requerido");
                return;
            }

            if (ruc.Length != 13 || !ruc.All(char.IsDigit))
            {
                MarcarError(txtRuc, "Debe tener 13 dígitos numéricos");
                return;
            }

            string suc = ruc.Substring(10, 3);
            if (suc == "000")
            {
                MarcarError(txtRuc, "No puede terminar en 000");
                return;
            }

            string cedula = ruc.Substring(0, 10);
            if (!EsCedulaEcuatorianaValida(cedula))
            {
                MarcarError(txtRuc, "RUC inválido");
                return;
            }

            MarcarCorrecto(txtRuc);
        }

        private void ValidarEmailLive()
        {
            string email = txtEmail.Text.Trim();

            if (email.Length == 0)
            {
                MarcarCorrecto(txtEmail); 
                return;
            }

            if (!EmailValido(email))
            {
                MarcarError(txtEmail, "Email inválido");
                return;
            }

            MarcarCorrecto(txtEmail);
        }

        private void ValidarTelefonoLive()
        {
            string tel = txtTelefono.Text.Trim();

            if (tel.Length == 0)
            {
                MarcarCorrecto(txtTelefono); // opcional
                return;
            }

            string telNorm = tel.StartsWith("+") ? tel.Substring(1) : tel;

            if (!telNorm.All(char.IsDigit))
            {
                MarcarError(txtTelefono, "Solo números");
                return;
            }

            if (telNorm.Length < 7 || telNorm.Length > 15)
            {
                MarcarError(txtTelefono, "Teléfono inválido");
                return;
            }

            MarcarCorrecto(txtTelefono);
        }

        private void ValidarNombreLive()
        {
            string nombre = txtNombreEmpresa.Text.Trim();

            if (nombre.Length < 2)
            {
                MarcarError(txtNombreEmpresa, "Nombre obligatorio");
                return;
            }

            MarcarCorrecto(txtNombreEmpresa);
        }


    }
}
