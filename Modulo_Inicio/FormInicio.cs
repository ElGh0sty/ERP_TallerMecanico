using System;
using System.Collections.Generic;
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
            InicializarCharts(); // Primero configurar charts
            CargarDashboard();   // Luego cargar datos

            _timer = new Timer { Interval = 60000 }; // Actualizar cada minuto
            _timer.Tick += (s, ev) => CargarDashboard();
            _timer.Start();
        }

        private void CargarDashboard()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                {
                    // KPIs 
                    lblClientesValor.Text = EjecutarScalarInt(cn, "SELECT COUNT(*) FROM Clientes;").ToString();
                    lblVehiculosValor.Text = EjecutarScalarInt(cn, "SELECT COUNT(*) FROM Vehiculos;").ToString();

                    // Órdenes activas (NO terminadas ni entregadas)
                    string sqlActivas = @"
SELECT COUNT(*)
FROM OrdenesTrabajo
WHERE UPPER(estado) NOT IN ('TERMINADO', 'ENTREGADO')
  AND UPPER(estado) NOT LIKE '%FINAL%'
  AND UPPER(estado) NOT LIKE '%CERR%';";

                    lblOTActivasValor.Text = EjecutarScalarInt(cn, sqlActivas).ToString();

                    // Órdenes terminadas HOY (usando la misma lógica)
                    DateTime ini = DateTime.Today;
                    DateTime fin = DateTime.Today.AddDays(1);

                    string sqlHoy = @"
SELECT COUNT(*)
FROM OrdenesTrabajo
WHERE fecha_ingreso >= @ini 
  AND fecha_ingreso < @fin
  AND UPPER(estado) IN ('TERMINADO', 'ENTREGADO');";

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
            catch (Exception ex)
            {
                Console.WriteLine("Error en dashboard: " + ex.Message);
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
            // Configurar Chart Line
            chartLine.Datasets.Clear();
            chartLine.Legend.Display = true;
            chartLine.Legend.Position = Guna.Charts.WinForms.LegendPosition.Top;
            chartLine.Title.Text = "";

            // Configurar eje X
            chartLine.XAxes.GridLines.Display = true;

            // Configurar eje Y
            chartLine.YAxes.GridLines.Display = true;
            chartLine.YAxes.Ticks.BeginAtZero = true;
        

            // Configurar Chart Pie
            chartPie.Datasets.Clear();
            chartPie.Legend.Display = true;
            chartPie.Legend.Position = Guna.Charts.WinForms.LegendPosition.Right;
            chartPie.Title.Text = "";
        }

        private void CargarLineFinalizadasUltimosDias(SqlConnection cn, int dias)
        {
            chartLine.Datasets.Clear();

            var ds = new GunaLineDataset
            {
                Label = "Órdenes Finalizadas",
                BorderWidth = 2,
                PointRadius = 3,
            };

            DateTime desde = DateTime.Today.AddDays(-dias + 1);
            DateTime hasta = DateTime.Today.AddDays(1);

            string sql = @"
SELECT 
    CAST(fecha_ingreso AS date) AS Dia, 
    COUNT(*) AS Cant
FROM OrdenesTrabajo
WHERE fecha_ingreso >= @desde 
  AND fecha_ingreso < @hasta
  AND UPPER(estado) IN ('TERMINADO', 'ENTREGADO')
GROUP BY CAST(fecha_ingreso AS date)
ORDER BY CAST(fecha_ingreso AS date);";

            var dt = new DataTable();
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@desde", desde);
                cmd.Parameters.AddWithValue("@hasta", hasta);
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            var datosPorDia = new Dictionary<DateTime, int>();
            foreach (DataRow r in dt.Rows)
            {
                DateTime dia = Convert.ToDateTime(r["Dia"]).Date;
                int cant = Convert.ToInt32(r["Cant"]);
                datosPorDia[dia] = cant;
            }

            // Verificar si hay datos
            bool hayDatos = false;

            for (int i = 0; i < dias; i++)
            {
                DateTime diaActual = desde.AddDays(i).Date;
                int cantidad = datosPorDia.ContainsKey(diaActual) ? datosPorDia[diaActual] : 0;

                if (cantidad > 0) hayDatos = true;

                ds.DataPoints.Add(new LPoint
                {
                    Label = diaActual.ToString("dd/MM"),
                    Y = cantidad
                });
            }

            chartLine.Datasets.Add(ds);

            // Ajustar el eje Y para que muestre valores enteros

            chartLine.Update();

            // Si no hay datos, mostrar un mensaje (opcional)
            if (!hayDatos)
            {
                // Puedes agregar un label de "Sin datos" si lo deseas
                Console.WriteLine("No hay órdenes terminadas/entregadas en los últimos 14 días");
            }
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
