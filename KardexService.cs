using System;
using System.Data;
using System.Data.SqlClient;

namespace PROYECTOMECANICO.Servicios
{
    public static class KardexService
    {
        public static void RegistrarMovimiento(
            SqlConnection cn,
            SqlTransaction tx,
            long productoId,
            long usuarioId,
            string tipoMovimiento,   // "ENTRADA" / "SALIDA"
            string origen,           // "OT", "COMPRA", etc.
            long? referenciaId,      // ordenId, compraId, etc.
            int cantidad,
            DateTime? fecha = null)
        {
            using (var cmd = new SqlCommand("dbo.sp_Kardex_RegistrarMovimiento", cn, tx))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ProductoId", SqlDbType.BigInt).Value = productoId;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.BigInt).Value = usuarioId;
                cmd.Parameters.Add("@TipoMovimiento", SqlDbType.NVarChar, 10).Value = tipoMovimiento;
                cmd.Parameters.Add("@Origen", SqlDbType.NVarChar, 50).Value = origen;

                var pRef = cmd.Parameters.Add("@ReferenciaId", SqlDbType.BigInt);
                pRef.Value = (object)referenciaId ?? DBNull.Value;

                cmd.Parameters.Add("@Cantidad", SqlDbType.Int).Value = cantidad;

                var pFecha = cmd.Parameters.Add("@Fecha", SqlDbType.DateTime);
                pFecha.Value = (object)fecha ?? DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }
    }
}

