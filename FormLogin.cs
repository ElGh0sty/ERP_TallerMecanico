using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using PROYECTOMECANICO.Seguridad;
using System.Data.SqlClient;

namespace PROYECTOMECANICO
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
            _empresaHandler = () =>
            {
                if (lblNombreTaller.IsDisposed) return;

                if (lblNombreTaller.InvokeRequired)
                    lblNombreTaller.BeginInvoke(new Action(() => lblNombreTaller.Text = EmpresaContext.NombreEmpresa));
                else
                    lblNombreTaller.Text = EmpresaContext.NombreEmpresa;
            };

            EmpresaContext.EmpresaActualizada += _empresaHandler;

            EmpresaContext.Cargar();   // esto debe cargar desde BD y disparar el evento
            _empresaHandler();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Por favor, ingrese usuario y contraseña.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Conexion objetoConexion = new Conexion();
            try
            {
                objetoConexion.Abrir();


                string query = @"SELECT U.id, U.nombre_usuario, R.nombre 
FROM Usuarios U 
INNER JOIN Roles R ON U.rol_id = R.id 
WHERE U.nombre_usuario = @user 
AND U.contrasena = @pass 
AND U.activo = 1";


                SqlCommand comando = new SqlCommand(query, objetoConexion.leer);
                comando.Parameters.AddWithValue("@user", txtUsuario.Text);
                comando.Parameters.AddWithValue("@pass", txtContraseña.Text);

                SqlDataReader leer = comando.ExecuteReader();

                if (leer.Read())
                {
                    long userId = Convert.ToInt64(leer["id"]);  

                    string userLogueado = leer["nombre_usuario"].ToString();
                    string rolNombre = leer["nombre"].ToString();

                    Form1 principal = new Form1(userId, rolNombre, userLogueado);
                    principal.Show();
                    this.Hide();

                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
            finally
            {
                objetoConexion.Cerrar();
            }


        }

        private Action _empresaHandler;

        

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_empresaHandler != null)
                EmpresaContext.EmpresaActualizada -= _empresaHandler;

            base.OnFormClosed(e);
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            LogoEmpresa.RegistrarPictureBox(picMiLogo);

            // También puedes acceder al logo directamente si lo necesitas
            if (LogoEmpresa.LogoActual != null)
            {
                picMiLogo.Image = (Image)LogoEmpresa.LogoActual.Clone();
            }
        }
    }
}
