using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormNuevoCliente : Form
    {
        private readonly Conexion con = new Conexion();

        private SqlDataAdapter adaptador;
        private DataTable dtClientes;

        private string rolUsuario;

        public FormNuevoCliente(string rol)
        {
            InitializeComponent();

            rolUsuario = rol;


            dgvNuevo.CellContentClick += dgvNuevo_CellContentClick;

            // Botón fuera del DGV (Nuevo Cliente)
            btnNuevoCliente.Click += (s, e) => AbrirPopupClienteNuevo();

            CargarBaseDeDatosCompleta();
            DataGridViewEstilo.AplicarEstiloDashboard(dgvNuevo);
        }

        private void CargarBaseDeDatosCompleta()
        {
            try
            {
                con.Abrir();

                string query = @"
SELECT 
    id AS cliente_id,
    tipo_documento,
    numero_documento,
    nombre,
    direccion,
    telefono,
    email
FROM Clientes
ORDER BY nombre;";

                adaptador = new SqlDataAdapter(query, con.leer);

                dtClientes = new DataTable();
                adaptador.Fill(dtClientes);

                dgvNuevo.Columns.Clear();
                dgvNuevo.AutoGenerateColumns = false;
                dgvNuevo.DataSource = dtClientes;

                // Columnas (solo lectura)
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "tipo_documento", HeaderText = "Tipo Doc.", Name = "tipo_documento" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "numero_documento", HeaderText = "Número Doc.", Name = "numero_documento" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "nombre", HeaderText = "Nombre Completo", Name = "nombre" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "telefono", HeaderText = "Teléfono", Name = "telefono" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "email", HeaderText = "Email", Name = "email" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "direccion", HeaderText = "Dirección", Name = "direccion" });

                // Botón Editar
                var btnEditar = new DataGridViewButtonColumn
                {
                    Name = "btnEditar",
                    HeaderText = "",
                    Text = "Editar",
                    UseColumnTextForButtonValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 85
                };
                dgvNuevo.Columns.Add(btnEditar);

                // Botón Eliminar
                var btnEliminar = new DataGridViewButtonColumn
                {
                    Name = "btnEliminar",
                    HeaderText = "",
                    Text = "Eliminar",
                    UseColumnTextForButtonValue = true,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    Width = 95
                };
                dgvNuevo.Columns.Add(btnEliminar);

                // Estilos + solo lectura
                EstilizarDataGridView();
                EstilizarBotonEditarClientes();
                EstilizarBotonEliminarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void dgvNuevo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string col = dgvNuevo.Columns[e.ColumnIndex].Name;
            if (col != "btnEliminar" && col != "btnEditar") return;

            var rowView = dgvNuevo.Rows[e.RowIndex].DataBoundItem as DataRowView;
            if (rowView == null)
            {
                MessageBox.Show("No se pudo obtener el registro seleccionado.");
                return;
            }

            long clienteId = Convert.ToInt64(rowView["cliente_id"]);

            if (col == "btnEliminar")
                EliminarCliente(clienteId);
            else
                AbrirPopupClienteEditar(clienteId);
        }

        private void AbrirPopupClienteNuevo()
        {
            using (var pop = new FormClientePopup())
            {
                if (pop.ShowDialog(this) == DialogResult.OK)
                    CargarBaseDeDatosCompleta();
            }
        }

        private void AbrirPopupClienteEditar(long clienteId)
        {
            using (var pop = new FormClientePopup(clienteId))
            {
                if (pop.ShowDialog(this) == DialogResult.OK)
                    CargarBaseDeDatosCompleta();
            }
        }

        private void EliminarCliente(long clienteId)
        {
            if (MessageBox.Show(
                "¿Seguro que deseas eliminar este cliente?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                con.Abrir();

                // 1) ¿Tiene vehículos?
                using (var cmdVeh = new SqlCommand("SELECT COUNT(*) FROM Vehiculos WHERE cliente_id = @id", con.leer))
                {
                    cmdVeh.Parameters.AddWithValue("@id", clienteId);
                    int cantVeh = Convert.ToInt32(cmdVeh.ExecuteScalar());

                    if (cantVeh > 0)
                    {
                        // 2) ¿Tiene órdenes asociadas?
                        using (var cmdOT = new SqlCommand(@"
SELECT COUNT(*) 
FROM OrdenesTrabajo ot
INNER JOIN Vehiculos v ON ot.vehiculo_id = v.id
WHERE v.cliente_id = @id", con.leer))
                        {
                            cmdOT.Parameters.AddWithValue("@id", clienteId);
                            int cantOT = Convert.ToInt32(cmdOT.ExecuteScalar());

                            if (cantOT > 0)
                            {
                                MessageBox.Show(
                                    $"No se puede eliminar el cliente.\n" +
                                    $"Tiene {cantVeh} vehículo(s) y {cantOT} orden(es) de trabajo asociadas.\n\n" +
                                    $"Primero elimina las órdenes en el módulo Taller, luego elimina los vehículos.",
                                    "Acción no permitida",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show(
                                    $"No se puede eliminar el cliente.\n" +
                                    $"Tiene {cantVeh} vehículo(s) registrados.\n\n" +
                                    $"Primero elimina los vehículos en el catálogo.",
                                    "Acción no permitida",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }
                        }
                        return;
                    }
                }

                // 3) Eliminar cliente
                using (var cmdDel = new SqlCommand("DELETE FROM Clientes WHERE id = @id", con.leer))
                {
                    cmdDel.Parameters.AddWithValue("@id", clienteId);
                    int filas = cmdDel.ExecuteNonQuery();

                    MessageBox.Show(filas > 0 ? "✅ Cliente eliminado." : "No se encontró el cliente.");
                }

                CargarBaseDeDatosCompleta();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        //  ESTILOS 

        private void EstilizarDataGridView()
        {
            dgvNuevo.AllowUserToAddRows = false;
            dgvNuevo.AllowUserToDeleteRows = false;
            dgvNuevo.ReadOnly = true;

            dgvNuevo.AllowUserToResizeRows = false;
            dgvNuevo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNuevo.MultiSelect = false;

            dgvNuevo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNuevo.ScrollBars = ScrollBars.Both;

            dgvNuevo.RowHeadersVisible = false;
            dgvNuevo.BorderStyle = BorderStyle.None;
            dgvNuevo.BackgroundColor = System.Drawing.Color.White;

            dgvNuevo.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNuevo.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvNuevo.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);

            dgvNuevo.EnableHeadersVisualStyles = false;
            dgvNuevo.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(24, 24, 28);
            dgvNuevo.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvNuevo.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            dgvNuevo.ColumnHeadersHeight = 46;

            dgvNuevo.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 11F);
            dgvNuevo.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(35, 35, 35);
            dgvNuevo.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvNuevo.DefaultCellStyle.Padding = new Padding(10, 3, 10, 3);

            dgvNuevo.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(220, 235, 255);
            dgvNuevo.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(10, 10, 10);

            dgvNuevo.AlternatingRowsDefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(247, 247, 250);

            dgvNuevo.RowTemplate.Height = 44;
        }

        private void EstilizarBotonEditarClientes()
        {
            if (!dgvNuevo.Columns.Contains("btnEditar")) return;

            var c = dgvNuevo.Columns["btnEditar"] as DataGridViewButtonColumn;
            if (c == null) return;

            c.FlatStyle = FlatStyle.Flat;
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            c.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);

            c.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(235, 245, 255);
            c.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(0, 90, 160);

            c.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(210, 235, 255);
            c.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(0, 70, 130);

            c.Width = 85;
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        private void EstilizarBotonEliminarClientes()
        {
            if (!dgvNuevo.Columns.Contains("btnEliminar")) return;

            var c = dgvNuevo.Columns["btnEliminar"] as DataGridViewButtonColumn;
            if (c == null) return;

            c.FlatStyle = FlatStyle.Flat;
            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            c.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);

            c.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
            c.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(160, 0, 0);

            c.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 210, 210);
            c.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(120, 0, 0);

            c.Width = 95;
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        private void FormNuevoCliente_Load(object sender, EventArgs e)
        {
            DataGridViewEstilo.AplicarEstiloDashboard(dgvNuevo);

        }

   
    
    }
}