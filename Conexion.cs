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

        public Conexion()
        {
            leer = new SqlConnection(cadena);
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
    }
}