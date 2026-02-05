using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PROYECTOMECANICO.Modulo_Clientes;
using PROYECTOMECANICO.Seguridad;

namespace PROYECTOMECANICO.Modulo_Taller
{
    public partial class FormTaller : Form
    {
        private string rolUsuario;

        public FormTaller(string rolUsuario)
        {
            InitializeComponent();
            this.rolUsuario = rolUsuario;
        }
        private bool PuedeUsarEsteModulo()
        {
            return Permisos.TienePermiso(rolUsuario, "TALLER");
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
            Form1 objetoPadre = (Form1)this.ParentForm;
            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormOrdenTrabajo(rolUsuario));
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
    }
}
