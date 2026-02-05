using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            // 1. Validar campos vacíos
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContraseña.Text))
            {
                MessageBox.Show("Por favor, ingrese usuario y contraseña.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Conexion objetoConexion = new Conexion();
            try
            {
                objetoConexion.Abrir();

                
                string query = @"SELECT U.nombre_usuario, R.nombre 
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
                    
                    string userLogueado = leer["nombre_usuario"].ToString();
                    string rolNombre = leer["nombre"].ToString(); 

                    MessageBox.Show("¡Bienvenido " + userLogueado + "!\n\nRol: " + rolNombre,
                                    "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // 3. Abrir el Formulario Principal
                    Form1 principal = new Form1(rolNombre);
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
    }
}
