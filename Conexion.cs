using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public class Conexion
    {
        private string cadena = "Server=(localdb)\\MSSQLLocalDB;Database=TallerMecanico_ERP;Integrated Security=True";

        public SqlConnection leer = new SqlConnection();

        public string CadenaConexion => cadena;

        public Conexion()
        {
            leer = new SqlConnection(cadena);
        }


        public string ConnectionString
        {
            get { return cadena; }
        }
        // NUEVO: crea una conexión NUEVA (no compartida)
        public SqlConnection CrearConexion()
        {
            return new SqlConnection(cadena);
        }

        // NUEVO: crea y abre una conexión NUEVA (no compartida)
        public SqlConnection CrearConexionAbierta()
        {
            var cn = new SqlConnection(cadena);
            cn.Open();
            return cn;
        }

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