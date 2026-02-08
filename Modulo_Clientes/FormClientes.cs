using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PROYECTOMECANICO.Seguridad;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormClientes : Form
    {
        private string rolUsuario;

        public FormClientes(string rolUsuario)
        {
            InitializeComponent();

            this.rolUsuario = rolUsuario;
            AplicarBotonesRedondos();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrar_Click(object sender, EventArgs e)
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
                objetoPadre.AbrirFormularioEnPanel(
                    new FormNuevoCliente(rolUsuario)
                    );
            }
        }

        private void btnRegistrarVe_Click(object sender, EventArgs e)
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
                objetoPadre.AbrirFormularioEnPanel(new FormRegVehi(objetoPadre, rolUsuario));


            }
        }

        private void btnCatalogo_Click(object sender, EventArgs e)
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
                objetoPadre.AbrirFormularioEnPanel(new FormCatalogo(objetoPadre, rolUsuario));

            }
        }

        private void btnOrden_Click(object sender, EventArgs e)
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
                objetoPadre.AbrirFormularioEnPanel(new FormOrden(rolUsuario));
            }
        }

        private bool PuedeUsarEsteModulo()
        {
            return Permisos.TienePermiso(rolUsuario, "CLIENTES");
        }

        private void AplicarBotonesRedondos()
        {
            // Ejemplo: botón Guardar
            BotonRedondo(btnCatalogo , 20);
            BotonRedondo(btnOrden, 20);
             BotonRedondo(btnRegistrar, 20);
             BotonRedondo(btnRegistrarVe, 20);
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
