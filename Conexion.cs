using System;
using System.Data.SqlClient;

namespace PROYECTOMECANICO
{
    public class Conexion
    {
        // El punto (.) indica que use el servidor local de cada integrante
        protected SqlConnection conectar = new SqlConnection("Server=.;Database=TallerMecanico_ERP;Integrated Security=True");

        public void Abrir()
        {
            try { conectar.Open(); }
            catch (Exception ex) { Console.WriteLine("Error al abrir: " + ex.Message); }
        }

        public void Cerrar()
        {
            try { conectar.Close(); }
            catch (Exception ex) { Console.WriteLine("Error al cerrar: " + ex.Message); }
        }
    }
}
