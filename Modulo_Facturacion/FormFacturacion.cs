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

namespace PROYECTOMECANICO.Modulo_Facturacion
{
    public partial class FormFacturacion : Form
    {
        private string rolUsuario;

        public FormFacturacion(string rolUsuario)
        {
            InitializeComponent();
            this.rolUsuario = rolUsuario;
        }

        private bool PuedeUsarEsteModulo()
        {
            return Permisos.TienePermiso(rolUsuario, "CLIENTES");
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
    }
}
