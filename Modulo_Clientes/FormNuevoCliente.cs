using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormNuevoCliente : Form
    {
        Conexion con = new Conexion();

        public FormNuevoCliente()
        {
            InitializeComponent();
            ConfigurarGridReal();
        }

        private void ConfigurarGridReal()
        {
            

            // 1. Crear el ComboBox para Tipo de Documento
            DataGridViewComboBoxColumn comboTipo = new DataGridViewComboBoxColumn();
            comboTipo.Name = "tipo_documento";
            comboTipo.HeaderText = "Tipo Doc.";
            // Agrega aquí las opciones EXACTAS que permite tu base de datos
            comboTipo.Items.Add("Cédula");
            comboTipo.Items.Add("RUC");
            comboTipo.Items.Add("Pasaporte");
            dgvNuevo.Columns.Add(comboTipo);

            // 2. Resto de columnas normales
            dgvNuevo.Columns.Add("numero_documento", "Número Doc.");
            dgvNuevo.Columns.Add("nombre", "Nombre Completo");
            dgvNuevo.Columns.Add("telefono", "Teléfono");
            dgvNuevo.Columns.Add("email", "Email");
            dgvNuevo.Columns.Add("direccion", "Dirección");

            dgvNuevo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvNuevo.AllowUserToAddRows = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                con.Abrir();
                int filasInsertadas = 0;

                foreach (DataGridViewRow fila in dgvNuevo.Rows)
                {
                    // Saltamos la fila vacía del final
                    if (fila.IsNewRow) continue;

                    // Validamos que al menos tenga el número de documento para no insertar basura
                    if (fila.Cells["numero_documento"].Value == null) continue;

                    string sql = @"INSERT INTO Clientes (tipo_documento, numero_documento, nombre, telefono, email, direccion) 
                                   VALUES (@tipo, @num, @nom, @tel, @mail, @dir)";

                    using (SqlCommand cmd = new SqlCommand(sql, con.leer))
                    {
                        cmd.Parameters.AddWithValue("@tipo", fila.Cells["tipo_documento"].Value ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@num", fila.Cells["numero_documento"].Value ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@nom", fila.Cells["nombre"].Value ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@tel", fila.Cells["telefono"].Value ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@mail", fila.Cells["email"].Value ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@dir", fila.Cells["direccion"].Value ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                        filasInsertadas++;
                    }
                }

                if (filasInsertadas > 0)
                {
                    MessageBox.Show($"Se registraron {filasInsertadas} clientes con éxito.", "ERP Taller");
                    VolverAlMenu();
                }
                else
                {
                    MessageBox.Show("No hay datos válidos para registrar.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de SQL: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void VolverAlMenu()
        {
            Form1 principal = (Form1)this.ParentForm;
            if (principal != null) principal.AbrirFormularioEnPanel(new FormClientes());
        }
    }
}