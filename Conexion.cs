using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;

namespace PROYECTOMECANICO
{
    public class Conexion
    {
        private readonly string cadena;

        
        public SqlConnection leer;

        public string CadenaConexion => cadena;
        public string ConnectionString => cadena;

        public Conexion()
        {
            // Lee del App.config
            cadena = ConfigurationManager.ConnectionStrings["TallerDB"]?.ConnectionString;

            if (string.IsNullOrWhiteSpace(cadena))
            {
                // Mensaje claro para tu compañero
                cadena = "Server=.;Database=TallerMecanico_ERP;Integrated Security=True;TrustServerCertificate=True";
                MessageBox.Show(
                    "No se encontró la cadena de conexión 'TallerDB' en App.config.\n\n" +
                    "Se usará una cadena por defecto, pero debes configurar el archivo:\n" +
                    "PROYECTOMECANICO.exe.config",
                    "Configuración de BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            leer = new SqlConnection(cadena);
        }

        // Crea conexión NUEVA (recomendado para usar con using)
        public SqlConnection CrearConexion()
        {
            return new SqlConnection(cadena);
        }

        // Crea y abre conexión NUEVA (recomendado)
        public SqlConnection CrearConexionAbierta()
        {
            try
            {
                var cn = new SqlConnection(cadena);
                cn.Open();
                return cn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo conectar a la base de datos.\n\n" +
                    "Revisa el archivo PROYECTOMECANICO.exe.config y ajusta:\n" +
                    "- Server (instancia)\n" +
                    "- Database\n\n" +
                    "Detalle:\n" + ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw;
            }
        }

        // Mantengo Abrir/Cerrar por compatibilidad, pero evita usarlo en código nuevo.
        public void Abrir()
        {
            try
            {
                if (leer.State == ConnectionState.Closed)
                    leer.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir la base de datos:\n" + ex.Message,
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public void Cerrar()
        {
            try
            {
                if (leer.State == ConnectionState.Open)
                    leer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar la base de datos:\n" + ex.Message,
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        public void ResetConnection()
        {
            try
            {
                if (leer != null)
                {
                    if (leer.State != ConnectionState.Closed) leer.Close();
                    leer.Dispose();
                }
            }
            catch { }

            leer = new SqlConnection(cadena);
        }
    }
}