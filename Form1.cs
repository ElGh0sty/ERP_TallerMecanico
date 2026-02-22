using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace PROYECTOMECANICO
{
    public partial class Form1 : Form
    {
        private Form formularioActivo = null;
        private string rolUsuario;
        private string usuarioActual;
        private readonly long usuarioId;

        public Form1(long usuarioId, string rol, string usuario)
        {
            InitializeComponent();

            this.usuarioId = usuarioId;   
            this.rolUsuario = rol;
            this.usuarioActual = usuario;

            lblSesion.Text = $"Usuario: {usuarioActual} \t Rol: {rolUsuario}";
            BotonRedondo(btnCerrarSesion, 7);
        }


        private void ActualizarSesionConFormulario(Form fh)
        {
            string nombreForm = string.IsNullOrWhiteSpace(fh.Text) ? fh.Name : fh.Text;

            lblSesion.Text = $"Usuario: {usuarioActual} \t Rol: {rolUsuario} \t Vista: {nombreForm}";
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
            ActualizarSesionConFormulario(fh);

            fh.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnTaller_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Taller.FormTaller(usuarioId ,rolUsuario));
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Inventario.FormInventario(usuarioId, rolUsuario));
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

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult r = MessageBox.Show(
                "¿Deseas cerrar sesión?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (r == DialogResult.Yes)
            {
                FormLogin login = new FormLogin();
                login.Show();

                this.Close();
            }
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
