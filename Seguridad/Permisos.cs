using System.Collections.Generic;

namespace PROYECTOMECANICO.Seguridad
{
    public static class Permisos
    {
        private static Dictionary<string, HashSet<string>> permisosPorRol =
            new Dictionary<string, HashSet<string>>
        {
            { "Administrador", new HashSet<string>
                {
                    "TALLER",
                    "CLIENTES",
                    "INVENTARIO",
                    "FACTURACION",
                    "PERSONAL",
                    "CONFIGURACION"
                }
            },

            { "Recepcionista", new HashSet<string>
                {
                    "TALLER",
                    "CLIENTES",
                    "FACTURACION"
                }
            },

            { "Mecanico", new HashSet<string>
                {
                    "TALLER"
                }
            },

            { "Bodeguero", new HashSet<string>
                {
                    "INVENTARIO"
                }
            }
        };

        public static bool TienePermiso(string rol, string modulo)
        {
            if (!permisosPorRol.ContainsKey(rol))
                return false;

            return permisosPorRol[rol].Contains(modulo);
        }
    }
}
