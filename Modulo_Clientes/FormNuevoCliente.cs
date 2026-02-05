using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using PROYECTOMECANICO.Seguridad;


namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormNuevoCliente : Form
    {
        SqlDataAdapter adaptador;
        DataTable dtClientes;
        Conexion con = new Conexion();
        private string rolUsuario;

        public FormNuevoCliente(string rol)
        {
            InitializeComponent();
            dgvNuevo.DataError += (s, e) => { e.ThrowException = false; };
            CargarBaseDeDatosCompleta();
            rolUsuario = rol;
        }

        private void CargarBaseDeDatosCompleta()
        {
            try
            {
                con.Abrir();
                string query = "SELECT id, tipo_documento, numero_documento, nombre, direccion, telefono, email FROM Clientes";

                adaptador = new SqlDataAdapter(query, con.leer);
                SqlCommandBuilder builder = new SqlCommandBuilder(adaptador);

                builder.GetInsertCommand();
                builder.GetUpdateCommand();
                builder.GetDeleteCommand(); 

                dtClientes = new DataTable();
                adaptador.Fill(dtClientes);

                dgvNuevo.Columns.Clear();
                dgvNuevo.AutoGenerateColumns = false;
                dgvNuevo.DataSource = dtClientes;

                DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn();
                btnEliminar.Name = "btnEliminar";
                btnEliminar.HeaderText = "Acción";
                btnEliminar.Text = "Eliminar";
                btnEliminar.UseColumnTextForButtonValue = true;
                dgvNuevo.Columns.Add(btnEliminar);

                DataGridViewComboBoxColumn colCombo = new DataGridViewComboBoxColumn();
                colCombo.DataPropertyName = "tipo_documento";
                colCombo.HeaderText = "Tipo Doc.";
                colCombo.Name = "tipo_documento";
                colCombo.Items.AddRange(new string[] { "Cédula", "RUC", "Pasaporte" });
                colCombo.FlatStyle = FlatStyle.Flat;
                dgvNuevo.Columns.Add(colCombo);

                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "numero_documento", HeaderText = "Número Doc.", Name = "numero_documento" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "nombre", HeaderText = "Nombre Completo", Name = "nombre" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "telefono", HeaderText = "Teléfono", Name = "telefono" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "email", HeaderText = "Email", Name = "email" });
                dgvNuevo.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "direccion", HeaderText = "Dirección", Name = "direccion" });

                dgvNuevo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }

        private void dgvNuevo_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvNuevo.Columns[e.ColumnIndex].Name == "btnEliminar" && e.RowIndex >= 0)
            {
                if (dgvNuevo.Rows[e.RowIndex].IsNewRow) return;

                DialogResult result = MessageBox.Show("¿Eliminar este cliente permanentemente de la base de datos?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        DataRowView filaActual = (DataRowView)dgvNuevo.Rows[e.RowIndex].DataBoundItem;
                        filaActual.Row.Delete();

                        con.Abrir();
                        adaptador.Update(dtClientes);
                        con.Cerrar();

                        MessageBox.Show("Eliminado con éxito de la base de datos.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al eliminar: " + ex.Message);
                    }
                }
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvNuevo.EndEdit();
                con.Abrir();
                int cambios = adaptador.Update(dtClientes);
                if (cambios > 0) MessageBox.Show("¡Datos sincronizados!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally { con.Cerrar(); }
        }
    }
}