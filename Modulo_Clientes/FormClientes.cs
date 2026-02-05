using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Clientes
{
    public partial class FormClientes : Form
    {
        public FormClientes()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            Form1 objetoPadre = (Form1)this.ParentForm;
            if (objetoPadre != null)
            {
                // Abrimos el formulario donde realmente estará el Grid
                objetoPadre.AbrirFormularioEnPanel(new FormNuevoCliente());
            }
        }

        private void btnRegistrarVe_Click(object sender, EventArgs e)
        {
            Form1 objetoPadre = (Form1)this.ParentForm;

            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormRegVehi());
            }
        }

        private void btnCatalogo_Click(object sender, EventArgs e)
        {
            Form1 objetoPadre = (Form1)this.ParentForm;

            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormCatalogo());
            }
        }

        private void btnOrden_Click(object sender, EventArgs e)
        {
            Form1 objetoPadre = (Form1)this.ParentForm;

            if (objetoPadre != null)
            {
                objetoPadre.AbrirFormularioEnPanel(new FormOrden());
            }
        }
    }
}
