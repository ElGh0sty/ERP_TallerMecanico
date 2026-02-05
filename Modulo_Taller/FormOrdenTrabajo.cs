using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormOrdenTrabajo : Form
    {
        Conexion con = new Conexion();

        public FormOrdenTrabajo()
        {
            InitializeComponent();
            CargarOrdenes();
            CargarEstados();
            EstiloGrid();
        }

        private void CargarOrdenes()
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    ot.id,
    v.placa,
    c.nombre AS cliente,
    u.nombre_usuario AS mecanico,
    ot.fecha_ingreso,
    ot.estado
FROM OrdenesTrabajo ot
INNER JOIN Vehiculos v ON ot.vehiculo_id = v.id
INNER JOIN Clientes c ON v.cliente_id = c.id
INNER JOIN Usuarios u ON ot.mecanico_id = u.id
ORDER BY ot.fecha_ingreso DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvOrdenes.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar órdenes: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void CargarEstados()
        {
            cmbEstado.Items.AddRange(new string[]
            {
                "Ingresado",
                "En Diagnóstico",
                "En Reparación",
                "Esperando Repuestos",
                "Terminado",
                "Entregado"
            });

            cmbEstado.SelectedIndex = 0;
        }

        private void EstiloGrid()
        {
            dgvOrdenes.ReadOnly = true;
            dgvOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrdenes.MultiSelect = false;
            dgvOrdenes.AllowUserToAddRows = false;
            dgvOrdenes.AllowUserToDeleteRows = false;
            dgvOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            if (dgvOrdenes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una orden.");
                return;
            }

            long ordenId = Convert.ToInt64(dgvOrdenes.SelectedRows[0].Cells["id"].Value);
            string nuevoEstado = cmbEstado.SelectedItem.ToString();

            try
            {
                con.Abrir();

                SqlCommand cmd = new SqlCommand(
                    "UPDATE OrdenesTrabajo SET estado = @estado WHERE id = @id",
                    con.leer
                );

                cmd.Parameters.AddWithValue("@estado", nuevoEstado);
                cmd.Parameters.AddWithValue("@id", ordenId);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Estado actualizado correctamente");
                CargarOrdenes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }
    }
}

