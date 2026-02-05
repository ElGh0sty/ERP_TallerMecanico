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

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormClientes : Form
    {
        private string rolUsuario;

        public FormClientes(string rolUsuario)
        {
            InitializeComponent();
            this.rolUsuario = rolUsuario;
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
                objetoPadre.AbrirFormularioEnPanel(new FormRegVehi(rolUsuario));
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
                objetoPadre.AbrirFormularioEnPanel(new FormCatalogo(rolUsuario));
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
    }
}
