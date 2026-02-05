using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PROYECTOMECANICO
{
    public partial class Form1 : Form
    {
        private Form formularioActivo = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void AbrirFormularioHijo(Form formularioHijo)
        {
            if (this.panel6.Controls.Count > 0)
            {
                this.panel6.Controls.Clear();
            }

            formularioActivo = formularioHijo;
            formularioHijo.TopLevel = false;
            formularioHijo.FormBorderStyle = FormBorderStyle.None;

            formularioHijo.Size = this.panel6.ClientSize;
            formularioHijo.Dock = DockStyle.Fill;

            this.panel6.Controls.Add(formularioHijo);
            this.panel6.Tag = formularioHijo;
            this.panel6.BringToFront();
            formularioHijo.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnTaller_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new PROYECTOMECANICO.Modulo_Taller.FormTaller());
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new PROYECTOMECANICO.Modulo_Inventario.FormInventario());
        }

        private void btnFacturacion_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new PROYECTOMECANICO.Modulo_Facturacion.FormFacturacion());
        }

        private void btnPersonal_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new PROYECTOMECANICO.Modulo_Personal.FormPersonal());
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
           AbrirFormularioHijo(new Modulo_Config.FormConfiguracion());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new PROYECTOMECANICO.Modulo_Inicio.FormInicio());
        }
        private void btnClientes_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new PROYECTOMECANICO.Modulo_Clientes.FormClientes());
        }
    }
}
