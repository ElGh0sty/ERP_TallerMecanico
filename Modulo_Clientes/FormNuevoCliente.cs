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
                string query = "SELECT id AS cliente_id, tipo_documento, numero_documento, nombre, direccion, telefono, email FROM Clientes";

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
                if (dgvNuevo.Columns.Contains("cliente_id"))
                    dgvNuevo.Columns["cliente_id"].Visible = false;

                EstilizarDataGridView();

                DataGridViewButtonColumn btnEliminar = new DataGridViewButtonColumn();
                btnEliminar.Name = "btnEliminar";
                btnEliminar.HeaderText = "";
                btnEliminar.Text = "Eliminar";
                btnEliminar.UseColumnTextForButtonValue = true;
                btnEliminar.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                btnEliminar.Width = 95;

                dgvNuevo.Columns.Add(btnEliminar);
                EstilizarBotonEliminarClientes();


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
            if (e.RowIndex < 0) return;

            if (dgvNuevo.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                // ✅ Tomar el ID desde el DataTable (aunque no exista columna visible)
                DataRowView rowView = dgvNuevo.Rows[e.RowIndex].DataBoundItem as DataRowView;

                if (rowView == null)
                {
                    MessageBox.Show("No se pudo obtener el registro seleccionado.");
                    return;
                }

                long clienteId = Convert.ToInt64(rowView["cliente_id"]);
                EliminarCliente(clienteId);
            }
        }



        private void dgvNuevo_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvNuevo.Columns[e.ColumnIndex].Name == "btnEliminar")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                e.Handled = true;
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
                string sqlVeh = "SELECT COUNT(*) FROM Vehiculos WHERE cliente_id = @id";
                SqlCommand cmdVeh = new SqlCommand(sqlVeh, con.leer);
                cmdVeh.Parameters.AddWithValue("@id", clienteId);
                int cantVeh = Convert.ToInt32(cmdVeh.ExecuteScalar());

                if (cantVeh > 0)
                {
                    // 2) ¿Tiene órdenes asociadas a esos vehículos?
                    string sqlOT = @"
SELECT COUNT(*) 
FROM OrdenesTrabajo ot
INNER JOIN Vehiculos v ON ot.vehiculo_id = v.id
WHERE v.cliente_id = @id";
                    SqlCommand cmdOT = new SqlCommand(sqlOT, con.leer);
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
                    return;
                }

                // 3) Si no tiene vehículos, eliminar cliente
                string sqlDel = "DELETE FROM Clientes WHERE id = @id";
                SqlCommand cmdDel = new SqlCommand(sqlDel, con.leer);
                cmdDel.Parameters.AddWithValue("@id", clienteId);

                int filas = cmdDel.ExecuteNonQuery();

                MessageBox.Show(filas > 0 ? "✅ Cliente eliminado." : "No se encontró el cliente.");
                CargarBaseDeDatosCompleta(); // <-- CAMBIA por tu método real de recargar dgvNuevo
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


        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                dgvNuevo.EndEdit();

                // ✅ Validar antes de guardar
                if (!ValidarClientesPendientes(out string mensajeError))
                {
                    MessageBox.Show(mensajeError, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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

        private bool ValidarClientesPendientes(out string mensaje)
        {
            mensaje = "";

            if (dtClientes == null) return true;

            int filaMostrada = 0;

            foreach (DataRow row in dtClientes.Rows)
            {
                if (row.RowState != DataRowState.Added && row.RowState != DataRowState.Modified)
                    continue;

                filaMostrada++;

                string tipoDoc = (row["tipo_documento"] == DBNull.Value) ? "" : row["tipo_documento"].ToString().Trim();
                string numDoc = (row["numero_documento"] == DBNull.Value) ? "" : row["numero_documento"].ToString().Trim();
                string nombre = (row["nombre"] == DBNull.Value) ? "" : row["nombre"].ToString().Trim();
                string telefono = (row["telefono"] == DBNull.Value) ? "" : row["telefono"].ToString().Trim();
                string email = (row["email"] == DBNull.Value) ? "" : row["email"].ToString().Trim();
                string direccion = (row["direccion"] == DBNull.Value) ? "" : row["direccion"].ToString().Trim();

                // --- Reglas mínimas ---
                if (string.IsNullOrWhiteSpace(tipoDoc))
                    return Fail($"Fila {filaMostrada}: Selecciona el Tipo de documento.", out mensaje);

                if (string.IsNullOrWhiteSpace(numDoc))
                    return Fail($"Fila {filaMostrada}: El Número de documento es obligatorio.", out mensaje);

                if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 3)
                    return Fail($"Fila {filaMostrada}: El nombre es obligatorio (mín. 3 caracteres).", out mensaje);

                if (!EsTelefonoValido(telefono))
                    return Fail($"Fila {filaMostrada}: Teléfono inválido (usa solo números, 7 a 10 dígitos).", out mensaje);

                if (string.IsNullOrWhiteSpace(email) || !EsEmailValido(email))
                    return Fail($"Fila {filaMostrada}: Email inválido u obligatorio.", out mensaje);


                if (string.IsNullOrWhiteSpace(direccion) || direccion.Length < 5)
                    return Fail($"Fila {filaMostrada}: Dirección obligatoria (mín. 5 caracteres).", out mensaje);

                // --- Validación por tipo de documento ---
                if (tipoDoc.Equals("Cédula", StringComparison.OrdinalIgnoreCase))
                {
                    if (!EsCedulaEcuatorianaValida(numDoc))
                        return Fail($"Fila {filaMostrada}: Cédula inválida.", out mensaje);
                }
                else if (tipoDoc.Equals("RUC", StringComparison.OrdinalIgnoreCase))
                {
                    if (!EsRucEcuatorianoValido(numDoc))
                        return Fail($"Fila {filaMostrada}: RUC inválido.", out mensaje);
                }
                else if (tipoDoc.Equals("Pasaporte", StringComparison.OrdinalIgnoreCase))
                {
                    if (!EsPasaporteValido(numDoc))
                        return Fail($"Fila {filaMostrada}: Pasaporte inválido (mín. 6, máx. 20; letras/números).", out mensaje);
                }
                else
                {
                    return Fail($"Fila {filaMostrada}: Tipo de documento no reconocido.", out mensaje);
                }
            }

            return true;
        }

        private bool EsCedulaEcuatorianaValida(string cedula)
        {
            cedula = SoloDigitos(cedula);

            if (cedula.Length != 10) return false;

            if (!int.TryParse(cedula.Substring(0, 2), out int provincia)) return false;
            if (provincia < 1 || provincia > 24) return false;

            int tercer = cedula[2] - '0';
            if (tercer > 5) return false;

            int suma = 0;
            for (int i = 0; i < 9; i++)
            {
                int dig = cedula[i] - '0';
                if (i % 2 == 0) // posiciones 0,2,4,6,8
                {
                    dig *= 2;
                    if (dig > 9) dig -= 9;
                }
                suma += dig;
            }

            int verificador = (10 - (suma % 10)) % 10;
            int ultimo = cedula[9] - '0';

            return verificador == ultimo;
        }

        private bool EsRucEcuatorianoValido(string ruc)
        {
            ruc = SoloDigitos(ruc);

            if (ruc.Length != 13) return false;

            // provincia válida
            if (!int.TryParse(ruc.Substring(0, 2), out int provincia)) return false;
            if (provincia < 1 || provincia > 24) return false;

            int tercer = ruc[2] - '0';
            if (tercer > 6) return false; // 0-6 se usa en muchas validaciones base

            // establecimiento (últimos 3)
            string establecimiento = ruc.Substring(10, 3);
            if (establecimiento != "001") return false;

            // Si es persona natural (tercer dígito 0-5), valida primeros 10 como cédula
            if (tercer >= 0 && tercer <= 5)
            {
                string cedula = ruc.Substring(0, 10);
                return EsCedulaEcuatorianaValida(cedula);
            }

            // Para tercer dígito 6: lo aceptamos como válido base (simplificado para proyecto)
            return true;
        }

        private bool EsPasaporteValido(string pasaporte)
        {
            if (string.IsNullOrWhiteSpace(pasaporte)) return false;

            pasaporte = pasaporte.Trim();

            if (pasaporte.Length < 6 || pasaporte.Length > 20) return false;

            foreach (char c in pasaporte)
            {
                if (!char.IsLetterOrDigit(c)) return false;
            }

            return true;
        }

        private string SoloDigitos(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";
            var chars = new System.Text.StringBuilder();
            foreach (char c in s)
                if (char.IsDigit(c)) chars.Append(c);
            return chars.ToString();
        }

        private bool EsTelefonoValido(string tel)
        {
            tel = SoloDigitos(tel);

            // Ecuador: 9-10 típicamente, pero dejamos 7-10 por flexibilidad
            return tel.Length >= 7 && tel.Length <= 10;
        }

        private bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            email = email.Trim();

            // Reglas rápidas anti-errores comunes
            if (email.Contains("..")) return false;
            if (email.StartsWith(".") || email.EndsWith(".")) return false;
            if (!email.Contains("@")) return false;

            int at = email.IndexOf("@");
            if (at <= 0) return false;                 // nada antes de @
            if (at != email.LastIndexOf("@")) return false; // más de un @

            string local = email.Substring(0, at);
            string domain = email.Substring(at + 1);

            if (string.IsNullOrWhiteSpace(domain)) return false;
            if (domain.StartsWith(".") || domain.EndsWith(".")) return false;
            if (!domain.Contains(".")) return false;   // debe tener TLD

            // caracteres permitidos (simplificado, pero estricto)
            foreach (char c in email)
            {
                bool ok = char.IsLetterOrDigit(c) || c == '@' || c == '.' || c == '_' || c == '-' || c == '+';
                if (!ok) return false;
            }

            // dominio debe tener al menos 2 chars en TLD
            int lastDot = domain.LastIndexOf(".");
            if (lastDot < 1) return false;
            string tld = domain.Substring(lastDot + 1);
            if (tld.Length < 2) return false;

            return true;
        }



        private bool Fail(string msg, out string mensaje)
        {
            mensaje = msg;
            return false;
        }

        private void EstilizarDataGridView()
        {
            // ✅ Permitir agregar y editar
            dgvNuevo.AllowUserToAddRows = true;
            dgvNuevo.AllowUserToDeleteRows = true;
            dgvNuevo.ReadOnly = false;

            dgvNuevo.AllowUserToResizeRows = false;

            dgvNuevo.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvNuevo.MultiSelect = false;

            // Scroll horizontal y vertical
            dgvNuevo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvNuevo.ScrollBars = ScrollBars.Both;

            dgvNuevo.RowHeadersVisible = false;
            dgvNuevo.BorderStyle = BorderStyle.None;
            dgvNuevo.BackgroundColor = System.Drawing.Color.White;

            dgvNuevo.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvNuevo.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvNuevo.GridColor = System.Drawing.Color.FromArgb(230, 230, 230);

            // Encabezado moderno
            dgvNuevo.EnableHeadersVisualStyles = false;
            dgvNuevo.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(24, 24, 28);
            dgvNuevo.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvNuevo.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);

            dgvNuevo.ColumnHeadersHeight = 46;

            // Texto grande
            dgvNuevo.DefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 11F);

            dgvNuevo.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(35, 35, 35);
            dgvNuevo.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            dgvNuevo.DefaultCellStyle.Padding = new Padding(10, 3, 10, 3);

            // Selección bonita
            dgvNuevo.DefaultCellStyle.SelectionBackColor =
                System.Drawing.Color.FromArgb(220, 235, 255);

            dgvNuevo.DefaultCellStyle.SelectionForeColor =
                System.Drawing.Color.FromArgb(10, 10, 10);

            // Filas alternadas suaves
            dgvNuevo.AlternatingRowsDefaultCellStyle.BackColor =
                System.Drawing.Color.FromArgb(247, 247, 250);

            dgvNuevo.RowTemplate.Height = 44;
        }

        private void EstilizarBotonEliminarClientes()
        {
            if (!dgvNuevo.Columns.Contains("btnEliminar")) return;

            var c = dgvNuevo.Columns["btnEliminar"] as DataGridViewButtonColumn;
            if (c == null) return;

            c.FlatStyle = FlatStyle.Flat;

            c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            c.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);

            // Estilo "danger" suave (rojo)
            c.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 235);
            c.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(160, 0, 0);

            c.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(255, 210, 210);
            c.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(120, 0, 0);

            // Mantener ancho fijo
            c.Width = 95;
            c.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }










    }



}