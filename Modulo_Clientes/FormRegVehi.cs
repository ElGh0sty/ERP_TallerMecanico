using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormRegVehi : Form
    {
        private readonly Conexion con = new Conexion();
        private DataTable dtClientes;

        private readonly Form1 formPrincipal;
        private long? vehiculoIdEditar = null;
        private readonly string rolUsuario;

        private long clienteSeleccionadoId = 0;
        private ValidadorTiempoReal validador;

        // Item para ListBox
        private class ClienteItem
        {
            public long Id { get; set; }
            public string Nombre { get; set; }
            public string TipoDoc { get; set; }
            public string Documento { get; set; }
            public override string ToString() => Nombre;
        }

        public FormRegVehi(Form1 principal, string rolUsuario)
        {
            InitializeComponent();
            formPrincipal = principal;
            this.rolUsuario = rolUsuario;

            // Hacer ComboBox solo de selección
            cmbTipoVehiculo.DropDownStyle = ComboBoxStyle.DropDownList;

            InicializarValidador();
            CargarClientesCompleto();
            CargarTiposVehiculo();

            txtBuscarCliente.TextChanged += txtBuscarCliente_TextChanged;
            lstClientes.Click += lstClientes_Click;
            txtBuscarCliente.Leave += txtBuscarCliente_Leave;

            this.Click += (s, e) => lstClientes.Visible = false;

            txtPlaca.CharacterCasing = CharacterCasing.Upper;
            txtChasis.CharacterCasing = CharacterCasing.Upper;

            txtAño.KeyPress += SoloNumeros_KeyPress;
            txtPlaca.KeyPress += Placa_KeyPress;
            txtChasis.KeyPress += Vin_KeyPress;

            InicializarValidacionLive();
        }

        public FormRegVehi(Form1 principal, string rolUsuario, long vehiculoId)
        {
            InitializeComponent();
            formPrincipal = principal;
            this.rolUsuario = rolUsuario;
            vehiculoIdEditar = vehiculoId;

            InicializarValidador();
            btnBuscadorCliente.Visible = false;
            CargarClientesCompleto();
            CargarTiposVehiculo();

            txtBuscarCliente.ReadOnly = true;

            this.Click += (s, e) => lstClientes.Visible = false;

            txtPlaca.CharacterCasing = CharacterCasing.Upper;
            txtChasis.CharacterCasing = CharacterCasing.Upper;

            txtAño.KeyPress += SoloNumeros_KeyPress;
            txtPlaca.KeyPress += Placa_KeyPress;
            txtChasis.KeyPress += Vin_KeyPress;

            InicializarValidacionLive();

            CargarVehiculoParaEditar(vehiculoId);
            btnGuardarVehiculo.Text = "Guardar edición";
        }

        private void InicializarValidador()
        {
            validador = new ValidadorTiempoReal(errorProvider1);
        }

        private void CargarClientesCompleto()
        {
            try
            {
                con.Abrir();
                string query = "SELECT id, nombre, tipo_documento, numero_documento FROM Clientes ORDER BY nombre ASC";
                SqlDataAdapter da = new SqlDataAdapter(query, con.leer);
                dtClientes = new DataTable();
                da.Fill(dtClientes);

                // Estado inicial del buscador
                lstClientes.Visible = false;
                lstClientes.DisplayMember = "Nombre";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando clientes: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void txtBuscarCliente_TextChanged(object sender, EventArgs e)
        {
            string texto = (txtBuscarCliente.Text ?? "").Trim();

            // Si el usuario borra o escribe poco, ocultamos lista y limpiamos selección
            if (texto.Length < 1)
            {
                lstClientes.Visible = false;
                clienteSeleccionadoId = 0;
                txtTipoDoc.Clear();
                txtDocumento.Clear();
                ValidarDuenioLive();
                return;
            }

            // Si el usuario está escribiendo, invalidamos selección previa
            clienteSeleccionadoId = 0;
            txtTipoDoc.Clear();
            txtDocumento.Clear();

            if (dtClientes == null || dtClientes.Rows.Count == 0)
            {
                lstClientes.Visible = false;
                return;
            }

            // Filtrado en memoria (rápido)
            var resultados = dtClientes.AsEnumerable()
                .Where(r =>
                    (r["nombre"]?.ToString() ?? "")
                    .IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                .Take(20)
                .Select(r => new ClienteItem
                {
                    Id = Convert.ToInt64(r["id"]),
                    Nombre = r["nombre"]?.ToString(),
                    TipoDoc = r["tipo_documento"]?.ToString(),
                    Documento = r["numero_documento"]?.ToString()
                })
                .ToList();

            lstClientes.DataSource = null;
            lstClientes.Items.Clear();

            if (resultados.Count == 0)
            {
                lstClientes.Visible = false;
                return;
            }

            lstClientes.DataSource = resultados;

            lstClientes.Visible = true;
            lstClientes.BringToFront();

            ValidarDuenioLive();
        }

        private void lstClientes_Click(object sender, EventArgs e)
        {
            if (lstClientes.SelectedItem is ClienteItem item)
            {
                clienteSeleccionadoId = item.Id;

                // Poner nombre en textbox
                txtBuscarCliente.TextChanged -= txtBuscarCliente_TextChanged;
                txtBuscarCliente.Text = item.Nombre;
                txtBuscarCliente.SelectionStart = txtBuscarCliente.Text.Length;
                txtBuscarCliente.TextChanged += txtBuscarCliente_TextChanged;

                // Llenar doc
                txtTipoDoc.Text = item.TipoDoc;
                txtDocumento.Text = item.Documento;

                lstClientes.Visible = false;

                ValidarDuenioLive();
            }
        }

        private void txtBuscarCliente_Leave(object sender, EventArgs e)
        {
            if (!lstClientes.Focused)
                lstClientes.Visible = false;
        }

        private void CargarTiposVehiculo()
        {
            try
            {
                con.Abrir();
                string sql = "SELECT nombre FROM TiposVehiculo WHERE activo = 1 ORDER BY id";
                SqlDataAdapter da = new SqlDataAdapter(sql, con.leer);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbTipoVehiculo.DataSource = dt;
                cmbTipoVehiculo.DisplayMember = "nombre";
                cmbTipoVehiculo.ValueMember = "nombre";
                cmbTipoVehiculo.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando tipos de vehículo: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void CargarVehiculoParaEditar(long vehiculoId)
        {
            try
            {
                con.Abrir();

                string sql = @"
SELECT 
    cliente_id, placa, marca, modelo, tipo, [año], chasis_vin, kilometraje_actual
FROM Vehiculos
WHERE id = @id";

                SqlCommand cmd = new SqlCommand(sql, con.leer);
                cmd.Parameters.AddWithValue("@id", vehiculoId);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // Cliente
                        clienteSeleccionadoId = Convert.ToInt64(dr["cliente_id"]);

                        if (dtClientes != null)
                        {
                            var row = dtClientes.AsEnumerable()
                                .FirstOrDefault(r => Convert.ToInt64(r["id"]) == clienteSeleccionadoId);

                            if (row != null)
                            {
                                txtBuscarCliente.Text = row["nombre"]?.ToString();
                                txtTipoDoc.Text = row["tipo_documento"]?.ToString();
                                txtDocumento.Text = row["numero_documento"]?.ToString();
                            }
                        }

                        // Tipo
                        cmbTipoVehiculo.SelectedValue = dr["tipo"].ToString();

                        txtPlaca.Text = dr["placa"].ToString();
                        txtMarca.Text = dr["marca"].ToString();
                        txtModelo.Text = dr["modelo"].ToString();
                        txtAño.Text = dr["año"].ToString();
                        txtChasis.Text = dr["chasis_vin"].ToString();
                        txtKilometraje.Text = dr["kilometraje_actual"].ToString();

                        btnGuardarVehiculo.Text = "Guardar cambios";

                        // Validar después de cargar
                        ValidarDuenioLive();
                        ValidarTipoLive();
                        ValidarPlacaLive();
                        ValidarMarcaLive();
                        ValidarModeloLive();
                        ValidarAnioLive();
                        ValidarVinLive();
                        ValidarKmLive();
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el vehículo para editar.");
                        vehiculoIdEditar = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando vehículo: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void btnGuardarVehiculo_Click(object sender, EventArgs e)
        {
            // Validar todos los campos antes de guardar
            if (!ValidarTodo())
            {
                MessageBox.Show("Corrige los campos marcados en rojo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarFormulario(out int anio, out int kilometraje))
                return;

            string placa = txtPlaca.Text.Trim().ToUpper();
            string vin = txtChasis.Text.Trim().ToUpper();

            try
            {
                con.Abrir();

                if (ExistePlaca(placa))
                {
                    MessageBox.Show("Ya existe un vehículo registrado con esa placa.");
                    validador.MarcarError(txtPlaca, "Placa ya registrada");
                    txtPlaca.Focus();
                    return;
                }

                if (ExisteVIN(vin))
                {
                    MessageBox.Show("Ya existe un vehículo registrado con ese Chasis/VIN.");
                    validador.MarcarError(txtChasis, "VIN ya registrado");
                    txtChasis.Focus();
                    return;
                }

                string sql;

                if (vehiculoIdEditar == null)
                {
                    sql = @"
INSERT INTO Vehiculos
(cliente_id, placa, marca, modelo, tipo, [año], chasis_vin, kilometraje_actual)
VALUES
(@cliente_id, @placa, @marca, @modelo, @tipo, @anio, @chasis, @kilometraje)";
                }
                else
                {
                    sql = @"
UPDATE Vehiculos SET
    cliente_id = @cliente_id,
    placa = @placa,
    marca = @marca,
    modelo = @modelo,
    tipo = @tipo,
    [año] = @anio,
    chasis_vin = @chasis,
    kilometraje_actual = @kilometraje
WHERE id = @id";
                }

                SqlCommand cmd = new SqlCommand(sql, con.leer);

                cmd.Parameters.Add("@cliente_id", SqlDbType.BigInt).Value = clienteSeleccionadoId;
                cmd.Parameters.Add("@placa", SqlDbType.Char, 8).Value = placa;
                cmd.Parameters.Add("@marca", SqlDbType.NVarChar, 255).Value = txtMarca.Text.Trim();
                cmd.Parameters.Add("@modelo", SqlDbType.NVarChar, 255).Value = txtModelo.Text.Trim();
                cmd.Parameters.Add("@tipo", SqlDbType.NVarChar, 30).Value = cmbTipoVehiculo.SelectedValue.ToString();
                cmd.Parameters.Add("@anio", SqlDbType.Int).Value = anio;
                cmd.Parameters.Add("@chasis", SqlDbType.NVarChar, 17).Value = vin;
                cmd.Parameters.Add("@kilometraje", SqlDbType.Int).Value = kilometraje;

                if (vehiculoIdEditar != null)
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = vehiculoIdEditar.Value;

                cmd.ExecuteNonQuery();

                if (vehiculoIdEditar == null)
                {
                    MessageBox.Show("¡Vehículo registrado!");
                    Limpiar();
                }
                else
                {
                    MessageBox.Show("Cambios guardados correctamente.");
                    formPrincipal.AbrirFormularioEnPanel(new FormCatalogo(formPrincipal, rolUsuario));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private bool ValidarTodo()
        {
            return ValidarDuenioLive() &&
                   ValidarTipoLive() &&
                   ValidarPlacaLive() &&
                   ValidarMarcaLive() &&
                   ValidarModeloLive() &&
                   ValidarAnioLive() &&
                   ValidarVinLive() &&
                   ValidarKmLive();
        }

        private void Limpiar()
        {
            foreach (Control c in this.Controls)
            {
                if (c is TextBox) (c as TextBox).Clear();
            }
            clienteSeleccionadoId = 0;
            cmbTipoVehiculo.SelectedIndex = -1;
            lstClientes.Visible = false;

            // Limpiar validaciones
            validador.LimpiarErrores(errorProvider1,
                txtBuscarCliente, txtPlaca, txtMarca, txtModelo, txtAño, txtChasis, txtKilometraje);
        }

        private void txtKilometraje_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        // ===== MÉTODOS DE VALIDACIÓN EN TIEMPO REAL =====

        private bool ValidarDuenioLive()
        {
            if (clienteSeleccionadoId == 0)
            {
                validador.MarcarError(txtBuscarCliente, "Seleccione un cliente de la lista.");
                return false;
            }
            else
            {
                validador.MarcarOk(txtBuscarCliente);
                return true;
            }
        }

        private bool ValidarTipoLive()
        {
            if (cmbTipoVehiculo.SelectedIndex == -1 || cmbTipoVehiculo.SelectedValue == null)
            {
                validador.MarcarError(cmbTipoVehiculo, "Seleccione un tipo de vehículo.");
                return false;
            }
            else
            {
                validador.MarcarOk(cmbTipoVehiculo);
                return true;
            }
        }

        private bool ValidarPlacaLive()
        {
            string placa = NormalizarPlaca(txtPlaca.Text);

            if (string.IsNullOrWhiteSpace(placa))
            {
                validador.MarcarError(txtPlaca, "Ingrese la placa.");
                return false;
            }

            if (!EsPlacaEcuadorValida(placa))
            {
                validador.MarcarError(txtPlaca, "Formato válido: ABC-123 o ABC-1234.");
                return false;
            }

            // Si es válida, actualiza el textbox con formato normalizado
            if (txtPlaca.Text.Trim().ToUpper() != placa)
                txtPlaca.Text = placa;

            validador.MarcarOk(txtPlaca);
            return true;
        }

        private bool ValidarMarcaLive()
        {
            string marca = NormalizarTextoVehiculo(txtMarca.Text);

            // refleja normalización en UI sin molestar (solo si cambió)
            if (txtMarca.Text != marca) txtMarca.Text = marca;

            if (string.IsNullOrWhiteSpace(marca))
            {
                validador.MarcarError(txtMarca, "Ingrese la marca.");
                return false;
            }

            if (marca.Length < 2 || marca.Length > 30)
            {
                validador.MarcarError(txtMarca, "Marca: 2 a 30 caracteres.");
                return false;
            }

            // Caracteres permitidos
            if (!Regex.IsMatch(marca, @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9 .'\-]+$"))
            {
                validador.MarcarError(txtMarca, "Marca inválida (caracteres no permitidos).");
                return false;
            }

            // Debe tener al menos 2 letras
            if (!TieneMinimoLetras(marca, 2))
            {
                validador.MarcarError(txtMarca, "Marca inválida (muy corta o sin letras).");
                return false;
            }

            // Evitar basura tipo AAAAAA / ----- / 111111
            if (EsTextoRepetidoBasura(marca))
            {
                validador.MarcarError(txtMarca, "Marca inválida.");
                return false;
            }

            validador.MarcarOk(txtMarca);
            return true;
        }

        private bool ValidarModeloLive()
        {
            string modelo = NormalizarTextoVehiculo(txtModelo.Text);

            if (txtModelo.Text != modelo) txtModelo.Text = modelo;

            if (string.IsNullOrWhiteSpace(modelo))
            {
                validador.MarcarError(txtModelo, "Ingrese el modelo.");
                return false;
            }

            if (modelo.Length < 1 || modelo.Length > 40)
            {
                validador.MarcarError(txtModelo, "Modelo: 1 a 40 caracteres.");
                return false;
            }

            // Modelo permite más cosas: / (ej: FZ/2.0), - (ej: CX-5)
            if (!Regex.IsMatch(modelo, @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9 .'\-\/]+$"))
            {
                validador.MarcarError(txtModelo, "Modelo inválido (caracteres no permitidos).");
                return false;
            }

            // Evitar basura repetida
            if (EsTextoRepetidoBasura(modelo))
            {
                validador.MarcarError(txtModelo, "Modelo inválido.");
                return false;
            }

            validador.MarcarOk(txtModelo);
            return true;
        }

        private bool ValidarAnioLive()
        {
            if (!int.TryParse((txtAño.Text ?? "").Trim(), out int anio))
            {
                validador.MarcarError(txtAño, "Ingrese un año válido.");
                return false;
            }

            int anioActual = DateTime.Now.Year;
            if (anio < 1950 || anio > anioActual + 1)
            {
                validador.MarcarError(txtAño, $"Año fuera de rango (1950 - {anioActual + 1}).");
                return false;
            }

            validador.MarcarOk(txtAño);
            return true;
        }

        private bool ValidarKmLive()
        {
            string kmTxt = (txtKilometraje.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(kmTxt))
            {
                validador.MarcarOk(txtKilometraje);
                return true;
            }

            if (!int.TryParse(kmTxt, out int km) || km < 0 || km > 3000000)
            {
                validador.MarcarError(txtKilometraje, "Kilometraje inválido (0 - 3,000,000).");
                return false;
            }

            validador.MarcarOk(txtKilometraje);
            return true;
        }

        private bool ValidarVinLive()
        {
            string vin = (txtChasis.Text ?? "").Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(vin))
            {
                validador.MarcarError(txtChasis, "Ingrese el Chasis/VIN.");
                return false;
            }

            if (!EsVinValido(vin))
            {
                validador.MarcarError(txtChasis, "VIN inválido: 17 caracteres, sin I/O/Q.");
                return false;
            }

            validador.MarcarOk(txtChasis);
            return true;
        }

        // ===== MÉTODOS DE UTILIDAD =====

        private string NormalizarPlaca(string placa)
        {
            placa = (placa ?? "").Trim().ToUpper().Replace(" ", "");
            // Si viene sin guión y cumple 3 letras + 3/4 números, lo normalizamos a ABC-1234
            if (Regex.IsMatch(placa, @"^[A-Z]{3}\d{3,4}$"))
                placa = placa.Substring(0, 3) + "-" + placa.Substring(3);
            return placa;
        }

        private bool EsPlacaEcuadorValida(string placaNormalizada)
        {
            // Formatos aceptados: ABC-123 o ABC-1234
            return Regex.IsMatch(placaNormalizada, @"^[A-Z]{3}-\d{3,4}$");
        }

        private bool EsVinValido(string vin)
        {
            vin = (vin ?? "").Trim().ToUpper();

            if (vin.Length != 17) return false;

            // Solo alfanumérico
            if (!Regex.IsMatch(vin, @"^[A-Z0-9]{17}$")) return false;

            // VIN estándar: no permite I, O, Q
            if (vin.Contains("I") || vin.Contains("O") || vin.Contains("Q")) return false;

            return true;
        }

        private void InicializarValidacionLive()
        {
            errorProvider1.BlinkStyle = ErrorBlinkStyle.NeverBlink;

            txtBuscarCliente.TextChanged += (s, e) => ValidarDuenioLive();
            txtBuscarCliente.Leave += (s, e) => ValidarDuenioLive();

            cmbTipoVehiculo.SelectedIndexChanged += (s, e) => ValidarTipoLive();

            txtPlaca.TextChanged += (s, e) => ValidarPlacaLive();
            txtPlaca.Leave += (s, e) => ValidarPlacaLive();

            txtMarca.TextChanged += (s, e) => ValidarMarcaLive();
            txtMarca.Leave += (s, e) => ValidarMarcaLive();

            txtModelo.TextChanged += (s, e) => ValidarModeloLive();
            txtModelo.Leave += (s, e) => ValidarModeloLive();

            txtAño.TextChanged += (s, e) => ValidarAnioLive();
            txtAño.Leave += (s, e) => ValidarAnioLive();

            txtChasis.TextChanged += (s, e) => ValidarVinLive();
            txtChasis.Leave += (s, e) => ValidarVinLive();

            txtKilometraje.TextChanged += (s, e) => ValidarKmLive();
            txtKilometraje.Leave += (s, e) => ValidarKmLive();
        }

        private bool ValidarFormulario(out int anio, out int kilometraje)
        {
            anio = 0;
            kilometraje = 0;

            // Cliente seleccionado
            if (clienteSeleccionadoId == 0)
            {
                MessageBox.Show("Seleccione un cliente desde la lista.");
                txtBuscarCliente.Focus();
                return false;
            }

            // Tipo
            if (cmbTipoVehiculo.SelectedIndex == -1 || cmbTipoVehiculo.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un tipo de vehículo.");
                cmbTipoVehiculo.Focus();
                return false;
            }

            // Placa
            string placa = NormalizarPlaca(txtPlaca.Text);

            if (string.IsNullOrWhiteSpace(placa))
            {
                MessageBox.Show("Ingrese la placa.");
                txtPlaca.Focus();
                return false;
            }
            if (!EsPlacaEcuadorValida(placa))
            {
                MessageBox.Show("Placa inválida. Formato válido: ABC-123 o ABC-1234.");
                txtPlaca.Focus();
                return false;
            }
            txtPlaca.Text = placa;

            // Marca
            string marca = NormalizarTextoVehiculo(txtMarca.Text);
            if (txtMarca.Text != marca) txtMarca.Text = marca;

            if (string.IsNullOrWhiteSpace(marca) || marca.Length < 2 || marca.Length > 30 ||
                !Regex.IsMatch(marca, @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9 .'\-]+$") ||
                !TieneMinimoLetras(marca, 2) ||
                EsTextoRepetidoBasura(marca))
            {
                MessageBox.Show("Ingrese una marca válida.");
                txtMarca.Focus();
                return false;
            }

            // Modelo
            string modelo = NormalizarTextoVehiculo(txtModelo.Text);
            if (txtModelo.Text != modelo) txtModelo.Text = modelo;

            if (string.IsNullOrWhiteSpace(modelo) || modelo.Length < 1 || modelo.Length > 40 ||
                !Regex.IsMatch(modelo, @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9 .'\-\/]+$") ||
                EsTextoRepetidoBasura(modelo))
            {
                MessageBox.Show("Ingrese un modelo válido.");
                txtModelo.Focus();
                return false;
            }

            // Año
            if (!int.TryParse((txtAño.Text ?? "").Trim(), out anio))
            {
                MessageBox.Show("Ingrese un año válido.");
                txtAño.Focus();
                return false;
            }

            int anioActual = DateTime.Now.Year;
            if (anio < 1970 || anio > anioActual + 1)
            {
                MessageBox.Show($"El año debe estar entre 1970 y {anioActual + 1}.");
                txtAño.Focus();
                return false;
            }

            // VIN
            string vin = (txtChasis.Text ?? "").Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(vin))
            {
                MessageBox.Show("Ingrese el Chasis/VIN.");
                txtChasis.Focus();
                return false;
            }
            if (!EsVinValido(vin))
            {
                MessageBox.Show("VIN inválido: 17 caracteres alfanuméricos, sin I/O/Q.");
                txtChasis.Focus();
                return false;
            }

            // Kilometraje
            string kmTxt = (txtKilometraje.Text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(kmTxt))
                kmTxt = "0";

            if (!int.TryParse(kmTxt, out kilometraje) || kilometraje < 0)
            {
                MessageBox.Show("Ingrese un kilometraje válido (0 o mayor).");
                txtKilometraje.Focus();
                return false;
            }

            return true;
        }

        private bool ExistePlaca(string placa)
        {
            string sql = vehiculoIdEditar == null
                ? "SELECT COUNT(*) FROM Vehiculos WHERE placa = @placa"
                : "SELECT COUNT(*) FROM Vehiculos WHERE placa = @placa AND id <> @id";

            using (SqlCommand cmd = new SqlCommand(sql, con.leer))
            {
                cmd.Parameters.Add("@placa", SqlDbType.Char, 8).Value = placa;
                if (vehiculoIdEditar != null)
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = vehiculoIdEditar.Value;

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private bool ExisteVIN(string vin)
        {
            string sql = vehiculoIdEditar == null
                ? "SELECT COUNT(*) FROM Vehiculos WHERE chasis_vin = @vin"
                : "SELECT COUNT(*) FROM Vehiculos WHERE chasis_vin = @vin AND id <> @id";

            using (SqlCommand cmd = new SqlCommand(sql, con.leer))
            {
                cmd.Parameters.Add("@vin", SqlDbType.NVarChar, 17).Value = vin;
                if (vehiculoIdEditar != null)
                    cmd.Parameters.Add("@id", SqlDbType.BigInt).Value = vehiculoIdEditar.Value;

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void Placa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;

            if (!char.IsControl(e.KeyChar) && txtPlaca.Text.Length >= 8)
                e.Handled = true;
        }

        private void Vin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsLetterOrDigit(e.KeyChar))
                e.Handled = true;

            if (!char.IsControl(e.KeyChar) && txtChasis.Text.Length >= 17)
                e.Handled = true;
        }

        private string NormalizarTextoVehiculo(string s)
        {
            s = (s ?? "").Trim();

            // colapsa espacios múltiples
            s = Regex.Replace(s, @"\s+", " ");

            // quita dobles guiones/ puntos repetidos excesivos
            s = Regex.Replace(s, @"[-]{3,}", "--");
            s = Regex.Replace(s, @"[.]{3,}", "..");

            return s;
        }

        private bool EsTextoRepetidoBasura(string s)
        {
            s = (s ?? "").Trim();
            if (s.Length < 4) return false;

            // Todo el string es el mismo caracter (AAAAA, 11111, -----)
            return s.All(ch => ch == s[0]);
        }

        private bool TieneMinimoLetras(string s, int minLetras)
        {
            int letras = Regex.Matches(s ?? "", @"[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]").Count;
            return letras >= minLetras;
        }

        private void btnBuscadorCliente_Click(object sender, EventArgs e)
        {
            FormBuscador buscador = new FormBuscador(FormBuscador.TipoBusqueda.Clientes);

            if (buscador.ShowDialog() == DialogResult.OK)
            {
                DataRow fila = buscador.ResultadoSeleccionado;

                long clienteId = Convert.ToInt64(fila["id"]);
                string nombre = fila["nombre"].ToString();

                clienteSeleccionadoId = clienteId;

                txtBuscarCliente.TextChanged -= txtBuscarCliente_TextChanged;

                txtBuscarCliente.Text = nombre;
                txtBuscarCliente.SelectionStart = txtBuscarCliente.Text.Length;

                txtBuscarCliente.TextChanged += txtBuscarCliente_TextChanged;

                if (dtClientes != null)
                {
                    var clienteRow = dtClientes.AsEnumerable()
                        .FirstOrDefault(r => Convert.ToInt64(r["id"]) == clienteId);

                    if (clienteRow != null)
                    {
                        txtTipoDoc.Text = clienteRow["tipo_documento"]?.ToString();
                        txtDocumento.Text = clienteRow["numero_documento"]?.ToString();
                    }
                }

                lstClientes.Visible = false;
                ValidarDuenioLive();
            }
        }
    }
}

