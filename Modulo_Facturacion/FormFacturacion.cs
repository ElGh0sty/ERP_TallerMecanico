using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROYECTOMECANICO.Seguridad;
using System.Drawing.Drawing2D;


namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormFacturacion : Form
    {
        private string rolUsuario;

        public FormFacturacion(string rolUsuario)
        {
            InitializeComponent();
            this.rolUsuario = rolUsuario;
            AplicarBotonesRedondos();
        }

        private bool PuedeUsarEsteModulo()
        {
            return Permisos.TienePermiso(rolUsuario, "FACTURACION");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!PuedeUsarEsteModulo())
            {
                MessageBox.Show(
                    "Esta acción no corresponde a tu rol de trabajo.",
                    "Acceso restringido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!PuedeUsarEsteModulo())
            {
                MessageBox.Show(
                    "Esta acción no corresponde a tu rol de trabajo.",
                    "Acceso restringido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!PuedeUsarEsteModulo())
            {
                MessageBox.Show(
                    "Esta acción no corresponde a tu rol de trabajo.",
                    "Acceso restringido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!PuedeUsarEsteModulo())
            {
                MessageBox.Show(
                    "Esta acción no corresponde a tu rol de trabajo.",
                    "Acceso restringido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
        }

        private void AplicarBotonesRedondos()
        {
            BotonRedondo(button1, 20);
            BotonRedondo(button2, 20);
            BotonRedondo(button3, 20);
            BotonRedondo(button4, 20);
        }


        private void BotonRedondo(Button btn, int radio)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();

            // Esquinas redondeadas
            path.AddArc(new Rectangle(0, 0, radio, radio), 180, 90);
            path.AddArc(new Rectangle(btn.Width - radio, 0, radio, radio), 270, 90);
            path.AddArc(new Rectangle(btn.Width - radio, btn.Height - radio, radio, radio), 0, 90);
            path.AddArc(new Rectangle(0, btn.Height - radio, radio, radio), 90, 90);

            path.CloseFigure();

            btn.Region = new Region(path);

            // Extra visual
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }

    }
}
