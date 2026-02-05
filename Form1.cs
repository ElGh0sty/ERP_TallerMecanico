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
        private string rolUsuario;
        private string usuarioActual;

        public Form1(string rol, string usuario) 
        {
            InitializeComponent();
            this.rolUsuario = rol;
            this.usuarioActual = usuario;

            lblSesion.Text = $"Usuario: {usuarioActual} \t Rol: {rolUsuario}";
            
        }

        public void AbrirFormularioEnPanel(object formularioHijo)
        {
            if (this.panel6.Controls.Count > 0)
                this.panel6.Controls.RemoveAt(0);

            Form fh = formularioHijo as Form;
            fh.TopLevel = false;
            fh.FormBorderStyle = FormBorderStyle.None;
            fh.Dock = DockStyle.Fill;

            this.panel6.Controls.Add(fh);
            this.panel6.Tag = fh;
            fh.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnTaller_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Taller.FormTaller(rolUsuario));
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Inventario.FormInventario(rolUsuario));
        }

        private void btnFacturacion_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Facturacion.FormFacturacion(rolUsuario));
        }

        private void btnPersonal_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Personal.FormPersonal());
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
           AbrirFormularioEnPanel(new Modulo_Config.FormConfiguracion());
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Inicio.FormInicio());
        }
        private void btnClientes_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Clientes.FormClientes(rolUsuario));
        }

        private bool TienePermiso(params string[] rolesPermitidos)
        {
            return rolesPermitidos.Contains(rolUsuario);
        }

        

    }
}
