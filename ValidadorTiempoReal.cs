using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public class ValidadorTiempoReal
    {
        private readonly ErrorProvider _errorProvider;

        public ValidadorTiempoReal(ErrorProvider errorProvider)
        {
            _errorProvider = errorProvider;
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        // Método para marcar error
        public void MarcarError(Control control, string mensaje)
        {
            control.BackColor = Color.FromArgb(255, 220, 220); // Rojo claro
            _errorProvider.SetError(control, mensaje);
        }

        public void MarcarOk(Control control)
        {
            control.BackColor = Color.White;
            _errorProvider.SetError(control, "");
        }


        public bool ValidarRequerido(TextBox txt, string nombreCampo)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarError(txt, $"El {nombreCampo} es obligatorio.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }

        public bool ValidarLongitudMinima(TextBox txt, int min, string nombreCampo)
        {
            if (txt.Text.Length < min)
            {
                MarcarError(txt, $"El {nombreCampo} debe tener al menos {min} caracteres.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }

        public bool ValidarLongitudMaxima(TextBox txt, int max, string nombreCampo)
        {
            if (txt.Text.Length > max)
            {
                MarcarError(txt, $"El {nombreCampo} no puede tener más de {max} caracteres.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }

        public bool ValidarLongitudExacta(TextBox txt, int exacta, string nombreCampo)
        {
            if (txt.Text.Length != exacta)
            {
                MarcarError(txt, $"El {nombreCampo} debe tener exactamente {exacta} caracteres.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }

        public bool ValidarNumerico(TextBox txt, string nombreCampo)
        {
            if (!string.IsNullOrWhiteSpace(txt.Text) && !int.TryParse(txt.Text, out _))
            {
                MarcarError(txt, $"El {nombreCampo} debe ser numérico.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }

        public bool ValidarDecimal(TextBox txt, string nombreCampo)
        {
            if (!string.IsNullOrWhiteSpace(txt.Text) && !decimal.TryParse(txt.Text, out _))
            {
                MarcarError(txt, $"El {nombreCampo} debe ser un número válido.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }


        public bool ValidarEmail(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarOk(txt);
                return true; // Email opcional
            }

            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(txt.Text, patron))
            {
                MarcarError(txt, "El formato del email no es válido (ej: usuario@dominio.com).");
                return false;
            }
            MarcarOk(txt);
            return true;
        }

        public bool ValidarTelefono(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarOk(txt);
                return true; // Teléfono opcional
            }

            string patron = @"^[0-9\-\+\(\)\s]{7,15}$";
            if (!Regex.IsMatch(txt.Text, patron))
            {
                MarcarError(txt, "Teléfono inválido. Use solo números, +, -, (), espacios.");
                return false;
            }
            MarcarOk(txt);
            return true;
        }


        public bool ValidarCedulaEcuatoriana(TextBox txt)
        {
            string cedula = txt.Text.Trim();

            if (cedula.Length != 10)
            {
                MarcarError(txt, "La cédula debe tener 10 dígitos.");
                return false;
            }

            if (!Regex.IsMatch(cedula, @"^\d{10}$"))
            {
                MarcarError(txt, "La cédula debe contener solo números.");
                return false;
            }

            int provincia = int.Parse(cedula.Substring(0, 2));
            if (provincia < 1 || provincia > 24)
            {
                MarcarError(txt, "Los primeros 2 dígitos deben estar entre 01 y 24.");
                return false;
            }

            int tercerDigito = int.Parse(cedula[2].ToString());
            if (tercerDigito > 5)
            {
                MarcarError(txt, "El tercer dígito debe ser 0-5 para cédulas.");
                return false;
            }

            int total = 0;
            int[] coeficientes = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };

            for (int i = 0; i < 9; i++)
            {
                int valor = int.Parse(cedula[i].ToString()) * coeficientes[i];
                total += valor > 9 ? valor - 9 : valor;
            }

            int digitoVerificador = int.Parse(cedula[9].ToString());
            int decenaSuperior = ((total / 10) + 1) * 10;
            int digitoCalculado = (total % 10 == 0) ? 0 : decenaSuperior - total;

            if (digitoCalculado != digitoVerificador)
            {
                MarcarError(txt, "La cédula no es válida (dígito verificador incorrecto).");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarRUC(TextBox txt)
        {
            string ruc = txt.Text.Trim();

            if (ruc.Length != 13)
            {
                MarcarError(txt, "El RUC debe tener 13 dígitos.");
                return false;
            }

            if (!Regex.IsMatch(ruc, @"^\d{13}$"))
            {
                MarcarError(txt, "El RUC debe contener solo números.");
                return false;
            }

            string cedula = ruc.Substring(0, 10);

            // Guardar texto temporal para validar cédula
            string textoOriginal = txt.Text;
            txt.Text = cedula;
            bool cedulaValida = ValidarCedulaEcuatoriana(txt);
            txt.Text = textoOriginal;

            if (!cedulaValida)
            {
                MarcarError(txt, "Los primeros 10 dígitos no forman una cédula válida.");
                return false;
            }

            // Validar últimos 3 dígitos (001 para sociedades)
            string ultimosTres = ruc.Substring(10, 3);
            if (ultimosTres != "001")
            {
                MarcarError(txt, "Los últimos 3 dígitos deben ser 001 para RUC.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarPasaporte(TextBox txt)
        {
            string pasaporte = txt.Text.Trim();

            if (pasaporte.Length < 6 || pasaporte.Length > 9)
            {
                MarcarError(txt, "El pasaporte debe tener entre 6 y 9 caracteres.");
                return false;
            }

            // Pasaporte: letras y números
            if (!Regex.IsMatch(pasaporte, @"^[A-Z0-9]{6,9}$", RegexOptions.IgnoreCase))
            {
                MarcarError(txt, "El pasaporte debe contener solo letras y números.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarPlacaVehiculo(TextBox txt)
        {
            string placa = txt.Text.Trim().ToUpper();

            // Auto-formato: convertir abc123 a ABC-123
            if (Regex.IsMatch(placa, @"^[A-Z]{3}\d{3,4}$"))
            {
                placa = placa.Substring(0, 3) + "-" + placa.Substring(3);
                txt.Text = placa;
                txt.SelectionStart = txt.Text.Length;
            }

            string patron = @"^[A-Z]{3}-\d{3,4}$";
            if (!Regex.IsMatch(placa, patron))
            {
                MarcarError(txt, "Formato inválido. Use: ABC-123 o ABC-1234");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarVIN(TextBox txt)
        {
            string vin = txt.Text.Trim().ToUpper();

            if (vin.Length != 17)
            {
                MarcarError(txt, "El VIN debe tener exactamente 17 caracteres.");
                return false;
            }

            if (!Regex.IsMatch(vin, @"^[A-HJ-NPR-Z0-9]{17}$")) // Sin I, O, Q
            {
                MarcarError(txt, "VIN inválido. Use solo letras y números (excepto I, O, Q).");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarAnioVehiculo(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarError(txt, "El año es obligatorio.");
                return false;
            }

            if (!int.TryParse(txt.Text, out int anio))
            {
                MarcarError(txt, "El año debe ser numérico.");
                return false;
            }

            int anioActual = DateTime.Now.Year;
            if (anio < 1900 || anio > anioActual + 1)
            {
                MarcarError(txt, $"El año debe estar entre 1900 y {anioActual + 1}.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarKilometraje(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarOk(txt); // Kilometraje opcional
                return true;
            }

            if (!int.TryParse(txt.Text, out int km))
            {
                MarcarError(txt, "El kilometraje debe ser numérico.");
                return false;
            }

            if (km < 0 || km > 9999999)
            {
                MarcarError(txt, "Kilometraje inválido (0 - 9,999,999).");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarCodigoSRI(TextBox txt, int longitudEsperada)
        {
            string codigo = txt.Text.Trim();

            if (codigo.Length != longitudEsperada)
            {
                MarcarError(txt, $"El código SRI debe tener {longitudEsperada} caracteres.");
                return false;
            }

            if (!Regex.IsMatch(codigo, @"^[A-Z0-9]+$"))
            {
                MarcarError(txt, "El código SRI debe contener solo letras mayúsculas y números.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarNombreEmpresa(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarError(txt, "El nombre de la empresa es obligatorio.");
                return false;
            }

            if (txt.Text.Length < 3)
            {
                MarcarError(txt, "El nombre debe tener al menos 3 caracteres.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarContacto(TextBox txt)
        {
            if (string.IsNullOrWhiteSpace(txt.Text))
            {
                MarcarOk(txt); // Contacto opcional
                return true;
            }

            if (txt.Text.Length < 3)
            {
                MarcarError(txt, "El nombre de contacto debe tener al menos 3 caracteres.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarSecuencial(TextBox txt, string nombreCampo)
        {
            string valor = txt.Text.Trim();

            if (valor.Length != 3)
            {
                MarcarError(txt, $"El {nombreCampo} debe tener 3 dígitos.");
                return false;
            }

            if (!Regex.IsMatch(valor, @"^\d{3}$"))
            {
                MarcarError(txt, $"El {nombreCampo} debe contener solo números.");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

        public bool ValidarTipoDocumento(TextBox txt)
        {
            string tipo = txt.Text.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(tipo))
            {
                MarcarError(txt, "El tipo de documento es obligatorio.");
                return false;
            }

            if (!Regex.IsMatch(tipo, @"^[A-Z\s]{3,20}$"))
            {
                MarcarError(txt, "Use solo letras mayúsculas (ej: FACTURA, NOTA_CREDITO).");
                return false;
            }

            MarcarOk(txt);
            return true;
        }

    

        

        public void LimpiarErrores(ErrorProvider errorProvider, params Control[] controles)
        {
            foreach (var control in controles)
            {
                errorProvider.SetError(control, "");
            }
        }
    }
}