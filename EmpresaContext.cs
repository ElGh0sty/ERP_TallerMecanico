using System;
using System.Data.SqlClient;

namespace PROYECTOMECANICO
{
    public static class EmpresaContext
    {
        public static event Action EmpresaActualizada;

        public static string NombreEmpresa { get; private set; } = "Taller";

        public static void Cargar()
        {
            try
            {
                var con = new Conexion();
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 nombre FROM Empresa ORDER BY id", cn))
                {
                    var val = cmd.ExecuteScalar();
                    if (val != null && val != DBNull.Value)
                        NombreEmpresa = val.ToString();
                }
            }
            catch
            {
                // Si no hay empresa aún, no rompemos
            }
        }

        public static void NotificarCambio()
        {
            Cargar();
            EmpresaActualizada?.Invoke();
        }
    }
}
