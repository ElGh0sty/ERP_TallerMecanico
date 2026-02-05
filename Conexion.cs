using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public class Conexion
    {
        // El punto (.) indica servidor local. Integrated Security=True usa tu cuenta de Windows.
        // Asegúrate de que el nombre de la base de datos sea el mismo que creaste.
        private string cadena = "Server=(localdb)\\MSSQLLocalDB;Database=TallerMecanico_ERP;Integrated Security=True";
        public SqlConnection leer = new SqlConnection();

        public Conexion()
        {
            leer.ConnectionString = cadena;
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
                MessageBox.Show("Error al abrir la base de datos: " + ex.Message);
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
                MessageBox.Show("Error al cerrar la base de datos: " + ex.Message);
            }
        }
    }
}
