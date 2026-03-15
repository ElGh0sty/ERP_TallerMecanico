using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public static class LogoEmpresa
    {
        private static Image _logoActual = null;
        private static readonly List<PictureBox> _pictureBoxes = new List<PictureBox>();

        // Evento que se dispara cuando el logo cambia
        public static event EventHandler LogoCambiado;

        // Propiedad para obtener el logo actual
        public static Image LogoActual
        {
            get { return _logoActual; }
            private set
            {
                _logoActual = value;
                OnLogoCambiado(EventArgs.Empty);
            }
        }

        // Cargar el logo desde la base de datos
        public static void CargarLogoDesdeBD(Conexion con)
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new System.Data.SqlClient.SqlCommand("SELECT TOP 1 logo FROM Empresa WHERE id = 1", cn))
                {
                    var result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        byte[] logoBytes = (byte[])result;
                        using (MemoryStream ms = new MemoryStream(logoBytes))
                        {
                            LogoActual = Image.FromStream(ms);
                        }
                    }
                    else
                    {
                        LogoActual = null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar logo: " + ex.Message);
                LogoActual = null;
            }
        }

        // Registrar un PictureBox para que se actualice automáticamente
        public static void RegistrarPictureBox(PictureBox pb)
        {
            if (!_pictureBoxes.Contains(pb))
            {
                _pictureBoxes.Add(pb);

                // Actualizar inmediatamente si ya hay un logo
                if (_logoActual != null)
                {
                    pb.Image = (Image)_logoActual.Clone();
                }

                // Manejar el evento de disposed para removerlo de la lista
                pb.Disposed += (s, e) => _pictureBoxes.Remove(pb);
            }
        }

        // Actualizar todos los PictureBox registrados
        private static void ActualizarTodosPictureBox()
        {
            foreach (var pb in _pictureBoxes)
            {
                if (pb != null && !pb.IsDisposed)
                {
                    if (_logoActual != null)
                    {
                        pb.Image = (Image)_logoActual.Clone();
                    }
                    else
                    {
                        pb.Image = null;
                    }
                }
            }
        }

        // Disparar el evento de cambio
        private static void OnLogoCambiado(EventArgs e)
        {
            ActualizarTodosPictureBox();
            LogoCambiado?.Invoke(null, e);
        }

        // Método para actualizar el logo (llamar después de guardar en configuración)
        public static void ActualizarLogo(byte[] nuevoLogoBytes)
        {
            if (nuevoLogoBytes != null)
            {
                using (MemoryStream ms = new MemoryStream(nuevoLogoBytes))
                {
                    LogoActual = Image.FromStream(ms);
                }
            }
            else
            {
                LogoActual = null;
            }
        }
    }
}