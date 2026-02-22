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

        // ✅ Cliente seleccionado desde el buscador
        private long clienteSeleccionadoId = 0;

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

            CargarClientesCompleto();
            CargarTiposVehiculo();

            // ✅ Eventos buscador
            txtBuscarCliente.TextChanged += txtBuscarCliente_TextChanged;
            lstClientes.Click += lstClientes_Click;
            txtBuscarCliente.Leave += txtBuscarCliente_Leave;

            // Para que el ListBox no quede “pegado” siempre visible
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

            CargarClientesCompleto();
            CargarTiposVehiculo();

            // ✅ Eventos buscador
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

            // Mostrar debajo del textbox (si quieres ajustar, lo haces en Designer)
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
            // Si el foco pasa al listbox, no lo cierres inmediatamente
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

                        // Buscar nombre del cliente en tu dtClientes
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
            ValidarDuenioLive();
            ValidarTipoLive();
            ValidarPlacaLive();
            ValidarMarcaLive();
            ValidarModeloLive();
            ValidarAnioLive();
            ValidarVinLive();
            ValidarKmLive();

            if (!string.IsNullOrEmpty(errorProvider1.GetError(txtBuscarCliente)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(cmbTipoVehiculo)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtPlaca)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtMarca)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtModelo)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtAño)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtChasis)) ||
                !string.IsNullOrEmpty(errorProvider1.GetError(txtKilometraje)))
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
                    txtPlaca.Focus();
                    return;
                }

                if (ExisteVIN(vin))
                {
                    MessageBox.Show("Ya existe un vehículo registrado con ese Chasis/VIN.");
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
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
            finally { con.Cerrar(); }
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
        }

        private void txtKilometraje_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void txtModelo_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void MarcarOk(Control c)
        {
            c.BackColor = Color.White;
            errorProvider1.SetError(c, "");
        }

        private void MarcarError(Control c, string msg)
        {
            c.BackColor = Color.FromArgb(255, 220, 220); 
            errorProvider1.SetError(c, msg);
        }


        private void ValidarDuenioLive()
        {
            if (clienteSeleccionadoId == 0)
                MarcarError(txtBuscarCliente, "Seleccione un cliente (de la lista).");
            else
                MarcarOk(txtBuscarCliente);
        }

        private void ValidarTipoLive()
        {
            if (cmbTipoVehiculo.SelectedIndex == -1 || cmbTipoVehiculo.SelectedValue == null)
                MarcarError(cmbTipoVehiculo, "Seleccione un tipo de vehículo.");
            else
                MarcarOk(cmbTipoVehiculo);
        }

        private void ValidarPlacaLive()
        {
            string placa = NormalizarPlaca(txtPlaca.Text);

            if (string.IsNullOrWhiteSpace(placa))
            {
                MarcarError(txtPlaca, "Ingrese la placa.");
                return;
            }

            if (!EsPlacaEcuadorValida(placa))
            {
                MarcarError(txtPlaca, "Formato válido: ABC-123 o ABC-1234.");
                return;
            }

            // Si es válida, actualiza el textbox con formato normalizado
            if (txtPlaca.Text.Trim().ToUpper() != placa)
                txtPlaca.Text = placa;

            MarcarOk(txtPlaca);
        }


        private void ValidarMarcaLive()
        {
            string marca = NormalizarTextoVehiculo(txtMarca.Text);

            // refleja normalización en UI sin molestar (solo si cambió)
            if (txtMarca.Text != marca) txtMarca.Text = marca;

            if (string.IsNullOrWhiteSpace(marca))
            {
                MarcarError(txtMarca, "Ingrese la marca.");
                return;
            }

            if (marca.Length < 2 || marca.Length > 30)
            {
                MarcarError(txtMarca, "Marca: 2 a 30 caracteres.");
                return;
            }

            // Caracteres permitidos
            if (!Regex.IsMatch(marca, @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9 .'\-]+$"))
            {
                MarcarError(txtMarca, "Marca inválida (caracteres no permitidos).");
                return;
            }

            // Debe tener al menos 2 letras (evita "12", "X1" si quieres permitir, baja a 1)
            if (!TieneMinimoLetras(marca, 2))
            {
                MarcarError(txtMarca, "Marca inválida (muy corta o sin letras).");
                return;
            }

            // Evitar basura tipo AAAAAA / ----- / 111111
            if (EsTextoRepetidoBasura(marca))
            {
                MarcarError(txtMarca, "Marca inválida.");
                return;
            }

            MarcarOk(txtMarca);
        }

        private void ValidarModeloLive()
        {
            string modelo = NormalizarTextoVehiculo(txtModelo.Text);

            if (txtModelo.Text != modelo) txtModelo.Text = modelo;

            if (string.IsNullOrWhiteSpace(modelo))
            {
                MarcarError(txtModelo, "Ingrese el modelo.");
                return;
            }

            if (modelo.Length < 1 || modelo.Length > 40)
            {
                MarcarError(txtModelo, "Modelo: 1 a 40 caracteres.");
                return;
            }

            // Modelo permite más cosas: / (ej: FZ/2.0), - (ej: CX-5)
            if (!Regex.IsMatch(modelo, @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9 .'\-\/]+$"))
            {
                MarcarError(txtModelo, "Modelo inválido (caracteres no permitidos).");
                return;
            }

            // Evitar basura repetida
            if (EsTextoRepetidoBasura(modelo))
            {
                MarcarError(txtModelo, "Modelo inválido.");
                return;
            }

            // (opcional) obliga a que tenga al menos 1 letra o 1 número (ya lo tiene por regex)
            MarcarOk(txtModelo);
        }



        private void ValidarAnioLive()
        {
            if (!int.TryParse((txtAño.Text ?? "").Trim(), out int anio))
            {
                MarcarError(txtAño, "Ingrese un año válido.");
                return;
            }

            int anioActual = DateTime.Now.Year;
            if (anio < 1950 || anio > anioActual + 1)
                MarcarError(txtAño, $"Año fuera de rango (1950 - {anioActual + 1}).");
            else
                MarcarOk(txtAño);
        }

        private void ValidarKmLive()
        {
            string kmTxt = (txtKilometraje.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(kmTxt))
            {
                // Si quieres obligarlo, cámbialo a MarcarError(...)
                MarcarOk(txtKilometraje);
                return;
            }

            if (!int.TryParse(kmTxt, out int km) || km < 0 || km > 3000000)
                MarcarError(txtKilometraje, "Kilometraje inválido (0 - 3,000,000).");
            else
                MarcarOk(txtKilometraje);
        }




        private void ValidarVinLive()
        {
            string vin = (txtChasis.Text ?? "").Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(vin))
            {
                MarcarError(txtChasis, "Ingrese el Chasis/VIN.");
                return;
            }

            if (!EsVinValido(vin))
            {
                MarcarError(txtChasis, "VIN inválido: 17 caracteres, sin I/O/Q.");
                return;
            }

            MarcarOk(txtChasis);
        }


        


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

            cmbTipoVehiculo.SelectedIndexChanged += (s, e) => ValidarTipoLive();

            txtPlaca.TextChanged += (s, e) => ValidarPlacaLive();
            txtMarca.TextChanged += (s, e) => ValidarMarcaLive();
            txtModelo.TextChanged += (s, e) => ValidarModeloLive();
            txtAño.TextChanged += (s, e) => ValidarAnioLive();
            txtChasis.TextChanged += (s, e) => ValidarVinLive();
            txtKilometraje.TextChanged += (s, e) => ValidarKmLive();
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

            // Placa (en tu BD es char(8) y no permite null según tu captura)
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
            if (anio < 1970 || anio > anioActual + 1) // margen por modelos próximos
            {
                MessageBox.Show($"El año debe estar entre 1970 y {anioActual + 1}.");
                txtAño.Focus();
                return false;
            }

            // Chasis/VIN (en tu BD es nvarchar(17) y lo estás tratando como VIN)
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


            // Kilometraje (en tu BD int NOT NULL)
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
            // Excluir el mismo vehículo cuando editas
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}

