using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using PROYECTOMECANICO.Seguridad;


namespace PROYECTOMECANICO
{
    public partial class Form1 : Form
    {
        private Timer menuTimer;
        private bool menuCollapsed;     // estado actual
        private bool menuTargetCollapsed; 
        private int menuExpandedWidth = 276;
        private int menuCollapsedWidth = 110;
        private int menuStep = 15;

        private int borderSize = 2;
        private Size formSize;

        private Form formularioActivo = null;
        private string rolUsuario;
        private string usuarioActual;
        private readonly long usuarioId;

        public Form1(long usuarioId, string rol, string usuario)
        {
            InitializeComponent();

            LogoEmpresa.RegistrarPictureBox(picMiLogo);

            // También puedes acceder al logo directamente si lo necesitas
            if (LogoEmpresa.LogoActual != null)
            {
                picMiLogo.Image = (Image)LogoEmpresa.LogoActual.Clone();
            }
            menuCollapsed = true; 
            menuTargetCollapsed = true;

            menuTimer = new Timer();
            menuTimer.Interval = 15; // 10-20ms se ve fluido
            menuTimer.Tick += MenuTimer_Tick;
            this.Padding = new Padding(borderSize);//Border size
            this.BackColor = Color.FromArgb(107, 83, 255);

            

            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Inicio.FormDashboard());
            this.usuarioId = usuarioId;   
            this.rolUsuario = rol;
            this.usuarioActual = usuario;

            lblSesion.Text = $"Usuario: {usuarioActual} \t Rol: {rolUsuario}";
            BotonRedondo(btnCerrarSesion, 7);

            _empresaHandler = () =>
            {
                if (lblNombreTaller.IsDisposed) return;

                if (lblNombreTaller.InvokeRequired)
                    lblNombreTaller.BeginInvoke(new Action(() => lblNombreTaller.Text = EmpresaContext.NombreEmpresa));
                else
                    lblNombreTaller.Text = EmpresaContext.NombreEmpresa;
            };

            EmpresaContext.EmpresaActualizada += _empresaHandler;

            EmpresaContext.Cargar();   
            _empresaHandler();

        }


        //PARA QUE EL PANEL SEA ARRASTABLE
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        //Overridden methods
        protected override void WndProc(ref Message m)
        {
            const int WM_NCCALCSIZE = 0x0083;//Standar Title Bar - Snap Window
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020; //Minimize form (Before)
            const int SC_RESTORE = 0xF120; //Restore form (Before)
            const int WM_NCHITTEST = 0x0084;//Win32, Mouse Input Notification: Determine what part of the window corresponds to a point, allows to resize the form.
            const int resizeAreaSize = 10;

            #region Form Resize
            // Resize/WM_NCHITTEST values
            const int HTCLIENT = 1; //Represents the client area of the window
            const int HTLEFT = 10;  //Left border of a window, allows resize horizontally to the left
            const int HTRIGHT = 11; //Right border of a window, allows resize horizontally to the right
            const int HTTOP = 12;   //Upper-horizontal border of a window, allows resize vertically up
            const int HTTOPLEFT = 13;//Upper-left corner of a window border, allows resize diagonally to the left
            const int HTTOPRIGHT = 14;//Upper-right corner of a window border, allows resize diagonally to the right
            const int HTBOTTOM = 15; //Lower-horizontal border of a window, allows resize vertically down
            const int HTBOTTOMLEFT = 16;//Lower-left corner of a window border, allows resize diagonally to the left
            const int HTBOTTOMRIGHT = 17;//Lower-right corner of a window border, allows resize diagonally to the right

            ///<Doc> More Information: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-nchittest </Doc>

            if (m.Msg == WM_NCHITTEST)
            { //If the windows m is WM_NCHITTEST
                base.WndProc(ref m);
                if (this.WindowState == FormWindowState.Normal)//Resize the form if it is in normal state
                {
                    if ((int)m.Result == HTCLIENT)//If the result of the m (mouse pointer) is in the client area of the window
                    {
                        Point screenPoint = new Point(m.LParam.ToInt32()); //Gets screen point coordinates(X and Y coordinate of the pointer)                           
                        Point clientPoint = this.PointToClient(screenPoint); //Computes the location of the screen point into client coordinates                          

                        if (clientPoint.Y <= resizeAreaSize)//If the pointer is at the top of the form (within the resize area- X coordinate)
                        {
                            if (clientPoint.X <= resizeAreaSize) //If the pointer is at the coordinate X=0 or less than the resizing area(X=10) in 
                                m.Result = (IntPtr)HTTOPLEFT; //Resize diagonally to the left
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize))//If the pointer is at the coordinate X=11 or less than the width of the form(X=Form.Width-resizeArea)
                                m.Result = (IntPtr)HTTOP; //Resize vertically up
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTTOPRIGHT;
                        }
                        else if (clientPoint.Y <= (this.Size.Height - resizeAreaSize)) //If the pointer is inside the form at the Y coordinate(discounting the resize area size)
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize horizontally to the left
                                m.Result = (IntPtr)HTLEFT;
                            else if (clientPoint.X > (this.Width - resizeAreaSize))//Resize horizontally to the right
                                m.Result = (IntPtr)HTRIGHT;
                        }
                        else
                        {
                            if (clientPoint.X <= resizeAreaSize)//Resize diagonally to the left
                                m.Result = (IntPtr)HTBOTTOMLEFT;
                            else if (clientPoint.X < (this.Size.Width - resizeAreaSize)) //Resize vertically down
                                m.Result = (IntPtr)HTBOTTOM;
                            else //Resize diagonally to the right
                                m.Result = (IntPtr)HTBOTTOMRIGHT;
                        }
                    }
                }
                return;
            }
            #endregion

            //Remove border and keep snap window
            if (m.Msg == WM_NCCALCSIZE && m.WParam.ToInt32() == 1)
            {
                return;
            }

            //Keep form size when it is minimized and restored. Since the form is resized because it takes into account the size of the title bar and borders.
            if (m.Msg == WM_SYSCOMMAND)
            {
                /// <see cref="https://docs.microsoft.com/en-us/windows/win32/menurc/wm-syscommand"/>
                /// Quote:
                /// In WM_SYSCOMMAND messages, the four low - order bits of the wParam parameter 
                /// are used internally by the system.To obtain the correct result when testing 
                /// the value of wParam, an application must combine the value 0xFFF0 with the 
                /// wParam value by using the bitwise AND operator.
                int wParam = (m.WParam.ToInt32() & 0xFFF0);

                if (wParam == SC_MINIMIZE)  //Before
                    formSize = this.ClientSize;
                if (wParam == SC_RESTORE)// Restored form(Before)
                    this.Size = formSize;
            }
            base.WndProc(ref m);
        }


        private Action _empresaHandler;

        

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (_empresaHandler != null)
                EmpresaContext.EmpresaActualizada -= _empresaHandler;

            base.OnFormClosed(e);
        }

        private void ActualizarSesionConFormulario(Form fh)
        {
            string nombreForm = string.IsNullOrWhiteSpace(fh.Text) ? fh.Name : fh.Text;

            lblSesion.Text = $"Usuario: {usuarioActual} \t Rol: {rolUsuario} \t Vista: {nombreForm}";
        }
        private Form _formActivo;
        public void AbrirFormularioEnPanel(object formularioHijo)
        {
            // Cerrar/limpiar anterior
            if (_formActivo != null)
            {
                _formActivo.Close();
                _formActivo.Dispose();
                _formActivo = null;
            }

            panel6.SuspendLayout();
            panel6.Controls.Clear();

            Form frm = formularioHijo as Form;
            // Configurar para que se comporte como "contenido"
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;          // <- esto es lo más importante
            frm.AutoScroll = true;              // útil si en tamaños pequeños no cabe
            frm.AutoScaleMode = AutoScaleMode.Dpi; // o Font, ver punto 3

            panel6.Controls.Add(frm);
            ActualizarSesionConFormulario(frm);
            frm.Show();

            _formActivo = frm;
            panel6.ResumeLayout();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnTaller_Click(object sender, EventArgs e)
        {
            buttonof();
            btnTaller.FillColor = Color.FromArgb(50, 100, 201);
            btnTaller.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Taller.FormTaller(usuarioId ,rolUsuario));
        }

        private void btnInventario_Click(object sender, EventArgs e)
        {
            buttonof();
            btnInventario.FillColor = Color.FromArgb(50, 100, 201);
            btnInventario.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Inventario.FormInventario(usuarioId, rolUsuario));
        }

        private void btnFacturacion_Click(object sender, EventArgs e)
        {
            buttonof();
            btnFacturacion.FillColor = Color.FromArgb(50, 100, 201);
            btnFacturacion.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Facturacion.FormFacturacion(usuarioId,rolUsuario));
        }

        private bool PuedeUsarEsteModulo()
        {
            return Permisos.TienePermiso(rolUsuario, "PERSONAL");
        }
        private void btnPersonal_Click(object sender, EventArgs e)
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
            buttonof();
            btnPersonal.FillColor = Color.FromArgb(50, 100, 201);
            btnPersonal.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Personal.FormPersonal());
        }

        private void btnConfiguracion_Click(object sender, EventArgs e)
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
            buttonof();
            btnConfiguracion.FillColor = Color.FromArgb(50, 100, 201);
            btnConfiguracion.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new FormConfiguracion());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LogoEmpresa.RegistrarPictureBox(picMiLogo);

            // También puedes acceder al logo directamente si lo necesitas
            if (LogoEmpresa.LogoActual != null)
            {
                picMiLogo.Image = (Image)LogoEmpresa.LogoActual.Clone();
            }
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttonof();
            btnInicio.FillColor = Color.FromArgb(50, 100, 201);
            btnInicio.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Inicio.FormDashboard());
        }
        private void btnClientes_Click(object sender, EventArgs e)
        {
            buttonof();
            btnClientes.FillColor = Color.FromArgb(50, 100, 201);
            btnClientes.FillColor2 = Color.FromArgb(255, 77, 165);
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

        private void Form1_Resize(object sender, EventArgs e)
        {
            AdjustForm();
        }

        private void AdjustForm()
        {
            switch (this.WindowState)
            {
                case FormWindowState.Maximized: 
                    this.Padding = new Padding(8, 8, 8, 0);
                    break;
                case FormWindowState.Normal: 
                    if (this.Padding.Top != borderSize)
                        this.Padding = new Padding(borderSize);
                    break;
            }
        }

        //MENU ABRIR Y CERRAR
        private void AplicarEstadoMenu(bool collapsed)
        {
            if (collapsed)
            {
                panel3.Visible = false;
                panel4.Visible = false;
                lblNombreTaller.Visible = false;
                label2.Visible = false;
                picMiLogo.Visible = false;

                btnMenu.Dock = DockStyle.Top;

                foreach (Button menuButton in panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "";
                    menuButton.ImageAlign = ContentAlignment.MiddleCenter;
                    menuButton.Padding = new Padding(0);
                }

                foreach (Guna.UI2.WinForms.Guna2GradientButton menuButton2 in panelMenu.Controls.OfType<Guna.UI2.WinForms.Guna2GradientButton>())
                {
                    menuButton2.Text = "";
                    menuButton2.ImageAlign = HorizontalAlignment.Center;
                    menuButton2.Padding = new Padding(0);
                }
            }
            else
            {
                panel3.Visible = true;
                panel4.Visible = true;
                lblNombreTaller.Visible = true;
                label2.Visible = true;
                picMiLogo.Visible = true;

                btnMenu.Dock = DockStyle.None;

                foreach (Button menuButton in panelMenu.Controls.OfType<Button>())
                {
                    menuButton.Text = "   " + menuButton.Tag?.ToString();
                    menuButton.ImageAlign = ContentAlignment.MiddleLeft;
                    menuButton.Padding = new Padding(10, 0, 0, 0);
                }

                foreach (Guna.UI2.WinForms.Guna2GradientButton menuButton2 in panelMenu.Controls.OfType<Guna.UI2.WinForms.Guna2GradientButton>())
                {
                    menuButton2.Text = "   " + menuButton2.Tag?.ToString();
                    menuButton2.ImageAlign = HorizontalAlignment.Left;
                    menuButton2.Padding = new Padding(10, 0, 0, 0);
                }
            }
        }

        private void ToggleMenuAnimated()
        {
            if (menuTimer.Enabled) return; 

            menuTargetCollapsed = panelMenu.Width > 200;

            if (menuTargetCollapsed)
                AplicarEstadoMenu(true); // quita textos desde el inicio del cierre

            menuTimer.Start();
        }

        private void MenuTimer_Tick(object sender, EventArgs e)
        {
            int targetWidth = menuTargetCollapsed ? menuCollapsedWidth : menuExpandedWidth;

            if (panelMenu.Width == targetWidth)
            {
                menuTimer.Stop();
                menuCollapsed = menuTargetCollapsed;

                // Al terminar la apertura, recién ponemos texto y visibles
                AplicarEstadoMenu(menuCollapsed);
                return;
            }

            // Animación: acercarse al ancho objetivo
            if (panelMenu.Width < targetWidth)
                panelMenu.Width = Math.Min(panelMenu.Width + menuStep, targetWidth);
            else
                panelMenu.Width = Math.Max(panelMenu.Width - menuStep, targetWidth);
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            ToggleMenuAnimated();
        }
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            formSize = this.ClientSize;
            this.WindowState = FormWindowState.Minimized;
        }
        private void btnMaximize_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                formSize = this.ClientSize;
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Size = formSize;
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        

        private void buttonof()
        {
            foreach (Guna2GradientButton menuButton2 in panelMenu.Controls.OfType<Guna2GradientButton>())
            {
                menuButton2.FillColor = Color.Transparent;
                menuButton2.FillColor2 = Color.Transparent;
            }

        }

        private void btnReportes_Click(object sender, EventArgs e)
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
            buttonof();
            btnReportes.FillColor = Color.FromArgb(50, 100, 201);
            btnReportes.FillColor2 = Color.FromArgb(255, 77, 165);
            AbrirFormularioEnPanel(new PROYECTOMECANICO.Modulo_Reportes.FormReportes(rolUsuario));

        }
    }
}

