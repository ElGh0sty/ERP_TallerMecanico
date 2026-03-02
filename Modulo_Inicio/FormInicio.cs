using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using PROYECTOMECANICO;

namespace PROYECTOMECANICO.Modulo_Inicio
{
    public partial class FormInicio : Form
    {
        private readonly Conexion con = new Conexion();
        private Timer _timer;

        public FormInicio()
        {
            InitializeComponent();
            this.Load += FormInicio_Load;
        }

        private void FormInicio_Load(object sender, EventArgs e)
        {
            InicializarCharts();
            CargarDashboard();

            _timer = new Timer { Interval = 60000 };
            _timer.Tick += (s, ev) => CargarDashboard();
            _timer.Start();
        }

        private void CargarDashboard()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    //  KPIs 
                    lblClientesValor.Text = EjecutarScalarInt(cn, "SELECT COUNT(*) FROM Clientes;").ToString();
                    lblVehiculosValor.Text = EjecutarScalarInt(cn, "SELECT COUNT(*) FROM Vehiculos;").ToString();

                    // Órdenes activas (no finalizadas)
                    string[] estadosFinal = { "FINALIZADA", "FINALIZADO", "ENTREGADA", "ENTREGADO", "CERRADA", "CERRADO", "TERMINADA", "TERMINADO" };
                    string inFinal = string.Join(",", estadosFinal.Select(s => $"'{s}'"));

                    string sqlActivas = $@"
SELECT COUNT(*)
FROM OrdenesTrabajo
WHERE ISNULL(UPPER(estado),'') NOT IN ({inFinal});";

                    lblOTActivasValor.Text = EjecutarScalarInt(cn, sqlActivas).ToString();

                    // Órdenes terminadas hoy: desde historial
                    DateTime ini = DateTime.Today;
                    DateTime fin = DateTime.Today.AddDays(1);

                    string[] eventosFinal = { "FINALIZADA", "FINALIZADO", "ENTREGADA", "ENTREGADO", "CERRADA", "CERRADO", "TERMINADA", "TERMINADO", "OT_FINALIZADA" };
                    string inEventos = string.Join(",", eventosFinal.Select(s => $"'{s}'"));

                    string sqlHoy = $@"
SELECT COUNT(DISTINCT orden_id)
FROM OrdenesTrabajo_Historial
WHERE fecha >= @ini AND fecha < @fin
AND (
    UPPER(tipo_evento) IN ({inEventos})
    OR UPPER(titulo) LIKE '%FINAL%'
    OR UPPER(titulo) LIKE '%ENTREG%'
    OR UPPER(titulo) LIKE '%CERR%'
);";

                    using (var cmd = new SqlCommand(sqlHoy, cn))
                    {
                        cmd.Parameters.AddWithValue("@ini", ini);
                        cmd.Parameters.AddWithValue("@fin", fin);
                        object v = cmd.ExecuteScalar();
                        lblOTHoyValor.Text = (v == null || v == DBNull.Value) ? "0" : Convert.ToInt32(v).ToString();
                    }

                    // CHARTS 
                    CargarLineFinalizadasUltimosDias(cn, dias: 14);
                    CargarPieEstadosOrdenes(cn);
                }
            }
            catch
            {
                // no crashear inicio
            }
        }

        private int EjecutarScalarInt(SqlConnection cn, string sql)
        {
            using (var cmd = new SqlCommand(sql, cn))
            {
                object v = cmd.ExecuteScalar();
                return (v == null || v == DBNull.Value) ? 0 : Convert.ToInt32(v);
            }
        }

        //  CHARTS 

        private void InicializarCharts()
        {
            // LINE
            chartLine.Datasets.Clear();
            chartLine.Legend.Display = false;
            chartLine.Title.Text = "";

            // PIE
            chartPie.Datasets.Clear();
            chartPie.Legend.Display = true;
            chartPie.Title.Text = "";
        }

        private void CargarLineFinalizadasUltimosDias(SqlConnection cn, int dias)
        {
            chartLine.Datasets.Clear();

            var ds = new GunaLineDataset
            {
                Label = "Finalizadas"
            };

            DateTime desde = DateTime.Today.AddDays(-dias + 1);
            DateTime hasta = DateTime.Today.AddDays(1);

            string[] eventosFinal = { "FINALIZADA", "FINALIZADO", "ENTREGADA", "ENTREGADO", "CERRADA", "CERRADO", "TERMINADA", "TERMINADO", "OT_FINALIZADA" };
            string inEventos = string.Join(",", eventosFinal.Select(s => $"'{s}'"));

            string sql = $@"
SELECT CAST(fecha AS date) AS Dia, COUNT(DISTINCT orden_id) AS Cant
FROM OrdenesTrabajo_Historial
WHERE fecha >= @desde AND fecha < @hasta
AND (
    UPPER(tipo_evento) IN ({inEventos})
    OR UPPER(titulo) LIKE '%FINAL%'
    OR UPPER(titulo) LIKE '%ENTREG%'
    OR UPPER(titulo) LIKE '%CERR%'
)
GROUP BY CAST(fecha AS date)
ORDER BY CAST(fecha AS date);";

            var dt = new DataTable();
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                new SqlDataAdapter(cmd).Fill(dt);
            }

            // llenar días faltantes con 0
            for (int i = 0; i < dias; i++)
            {
                var dia = desde.AddDays(i).Date;

                int cant = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (Convert.ToDateTime(r["Dia"]).Date == dia)
                    {
                        cant = Convert.ToInt32(r["Cant"]);
                        break;
                    }
                }

                ds.DataPoints.Add(new LPoint { Label = dia.ToString("dd/MM"), Y = cant });
            }

            chartLine.Datasets.Add(ds);
            chartLine.Update();
        }

        private void CargarPieEstadosOrdenes(SqlConnection cn)
        {
            chartPie.Datasets.Clear();

            var ds = new GunaPieDataset
            {
                Label = "Estados"
            };

            // Top 6 estados para que sea legible
            string sql = @"
SELECT TOP 6
    CASE WHEN LTRIM(RTRIM(ISNULL(estado,''))) = '' THEN 'SIN ESTADO' ELSE UPPER(estado) END AS Estado,
    COUNT(*) AS Cant
FROM OrdenesTrabajo
GROUP BY CASE WHEN LTRIM(RTRIM(ISNULL(estado,''))) = '' THEN 'SIN ESTADO' ELSE UPPER(estado) END
ORDER BY COUNT(*) DESC;";

            using (var cmd = new SqlCommand(sql, cn))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    string estado = rd["Estado"].ToString();
                    double cant = Convert.ToDouble(rd["Cant"]);
                    ds.DataPoints.Add(new LPoint { Label = estado, Y = cant });
                }
            }

            chartPie.Datasets.Add(ds);
            chartPie.Update();
        }

        
    }
}
