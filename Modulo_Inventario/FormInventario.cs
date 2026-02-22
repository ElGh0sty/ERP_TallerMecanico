using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROYECTOMECANICO.Modulo_Taller;
using PROYECTOMECANICO.Seguridad;


namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormInventario : Form
    {
        private readonly long usuarioId;
        private readonly string rolUsuario;

        public FormInventario(long usuarioId, string rolUsuario)
        {
            InitializeComponent();
            this.usuarioId = usuarioId;
            this.rolUsuario = rolUsuario;
            AplicarBotonesRedondos();
        }

        private bool PuedeUsarEsteModulo()
        {
            return Permisos.TienePermiso(rolUsuario, "INVENTARIO");
        }
        private void label1_Click(object sender, EventArgs e)
        {

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
            Form1 objetoPadre = (Form1)this.ParentForm;
            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormKardex());
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
            Form1 objetoPadre = (Form1)this.ParentForm;
            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormProvee());
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
            Form1 objetoPadre = (Form1)this.ParentForm;
            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormCompras(this.usuarioId));
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
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
            Form1 objetoPadre = (Form1)this.ParentForm;
            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormCalogProd());
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
