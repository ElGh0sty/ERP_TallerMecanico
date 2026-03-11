namespace PROYECTOMECANICO
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblSesion = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNombreTaller = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.iconButton3 = new FontAwesome.Sharp.IconButton();
            this.iconButton2 = new FontAwesome.Sharp.IconButton();
            this.btnFacturacion = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnPersonal = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnConfiguracion = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnInventario = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnTaller = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnClientes = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnInicio = new Guna.UI2.WinForms.Guna2GradientButton();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnMenu = new FontAwesome.Sharp.IconButton();
            this.btnReportes = new Guna.UI2.WinForms.Guna2GradientButton();
            this.btnCerrarSesion = new FontAwesome.Sharp.IconButton();
            this.panel5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.iconButton3);
            this.panel5.Controls.Add(this.iconButton2);
            this.panel5.Controls.Add(this.lblSesion);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(276, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(924, 60);
            this.panel5.TabIndex = 1;
            this.panel5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTitleBar_MouseDown);
            // 
            // lblSesion
            // 
            this.lblSesion.AutoSize = true;
            this.lblSesion.Font = new System.Drawing.Font("Yu Gothic UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSesion.ForeColor = System.Drawing.Color.Gray;
            this.lblSesion.Location = new System.Drawing.Point(6, 12);
            this.lblSesion.Name = "lblSesion";
            this.lblSesion.Size = new System.Drawing.Size(35, 30);
            this.lblSesion.TabIndex = 3;
            this.lblSesion.Text = "N ";
            // 
            // panel6
            // 
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(276, 60);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(924, 640);
            this.panel6.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnMenu);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.lblNombreTaller);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(15, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(246, 118);
            this.panel1.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(75, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 11);
            this.label2.TabIndex = 8;
            this.label2.Text = "PROYECTO DEL GRUPO CHALACAN";
            // 
            // lblNombreTaller
            // 
            this.lblNombreTaller.AutoSize = true;
            this.lblNombreTaller.Font = new System.Drawing.Font("Yu Gothic UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNombreTaller.ForeColor = System.Drawing.Color.White;
            this.lblNombreTaller.Location = new System.Drawing.Point(58, 71);
            this.lblNombreTaller.Name = "lblNombreTaller";
            this.lblNombreTaller.Size = new System.Drawing.Size(160, 30);
            this.lblNombreTaller.TabIndex = 7;
            this.lblNombreTaller.Text = "Taller Mecanico";
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(15, 700);
            this.panel3.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(261, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(15, 700);
            this.panel4.TabIndex = 2;
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(102)))), ((int)(((byte)(244)))));
            this.panelMenu.Controls.Add(this.btnFacturacion);
            this.panelMenu.Controls.Add(this.btnPersonal);
            this.panelMenu.Controls.Add(this.btnConfiguracion);
            this.panelMenu.Controls.Add(this.btnInventario);
            this.panelMenu.Controls.Add(this.btnTaller);
            this.panelMenu.Controls.Add(this.btnClientes);
            this.panelMenu.Controls.Add(this.btnInicio);
            this.panelMenu.Controls.Add(this.panel1);
            this.panelMenu.Controls.Add(this.btnReportes);
            this.panelMenu.Controls.Add(this.btnCerrarSesion);
            this.panelMenu.Controls.Add(this.panel3);
            this.panelMenu.Controls.Add(this.panel4);
            this.panelMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenu.ForeColor = System.Drawing.Color.White;
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.panelMenu.Size = new System.Drawing.Size(276, 700);
            this.panelMenu.TabIndex = 0;
            // 
            // iconButton3
            // 
            this.iconButton3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconButton3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(202)))), ((int)(((byte)(210)))));
            this.iconButton3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton3.FlatAppearance.BorderSize = 0;
            this.iconButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton3.IconChar = FontAwesome.Sharp.IconChar.WindowMinimize;
            this.iconButton3.IconColor = System.Drawing.Color.White;
            this.iconButton3.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton3.IconSize = 20;
            this.iconButton3.Location = new System.Drawing.Point(835, 0);
            this.iconButton3.Name = "iconButton3";
            this.iconButton3.Size = new System.Drawing.Size(45, 25);
            this.iconButton3.TabIndex = 10;
            this.iconButton3.UseVisualStyleBackColor = false;
            this.iconButton3.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // iconButton2
            // 
            this.iconButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.iconButton2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(74)))), ((int)(((byte)(130)))));
            this.iconButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.iconButton2.FlatAppearance.BorderSize = 0;
            this.iconButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconButton2.IconChar = FontAwesome.Sharp.IconChar.CircleXmark;
            this.iconButton2.IconColor = System.Drawing.Color.White;
            this.iconButton2.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.iconButton2.IconSize = 26;
            this.iconButton2.Location = new System.Drawing.Point(879, 0);
            this.iconButton2.Name = "iconButton2";
            this.iconButton2.Size = new System.Drawing.Size(45, 25);
            this.iconButton2.TabIndex = 9;
            this.iconButton2.UseVisualStyleBackColor = false;
            this.iconButton2.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnFacturacion
            // 
            this.btnFacturacion.Animated = true;
            this.btnFacturacion.BorderRadius = 10;
            this.btnFacturacion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFacturacion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnFacturacion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnFacturacion.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFacturacion.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnFacturacion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnFacturacion.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFacturacion.FillColor = System.Drawing.Color.Transparent;
            this.btnFacturacion.FillColor2 = System.Drawing.Color.Transparent;
            this.btnFacturacion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFacturacion.ForeColor = System.Drawing.Color.White;
            this.btnFacturacion.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnFacturacion.Image = global::PROYECTOMECANICO.Properties.Resources.layout_102871;
            this.btnFacturacion.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFacturacion.ImageSize = new System.Drawing.Size(50, 50);
            this.btnFacturacion.Location = new System.Drawing.Point(15, 390);
            this.btnFacturacion.Name = "btnFacturacion";
            this.btnFacturacion.Size = new System.Drawing.Size(246, 64);
            this.btnFacturacion.TabIndex = 39;
            this.btnFacturacion.Tag = "Facturacion";
            this.btnFacturacion.Text = " Facturacion";
            this.btnFacturacion.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnFacturacion.Click += new System.EventHandler(this.btnFacturacion_Click);
            // 
            // btnPersonal
            // 
            this.btnPersonal.Animated = true;
            this.btnPersonal.BorderRadius = 10;
            this.btnPersonal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPersonal.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnPersonal.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnPersonal.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPersonal.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnPersonal.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnPersonal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnPersonal.FillColor = System.Drawing.Color.Transparent;
            this.btnPersonal.FillColor2 = System.Drawing.Color.Transparent;
            this.btnPersonal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPersonal.ForeColor = System.Drawing.Color.White;
            this.btnPersonal.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnPersonal.Image = global::PROYECTOMECANICO.Properties.Resources.agent_102942;
            this.btnPersonal.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPersonal.ImageSize = new System.Drawing.Size(50, 50);
            this.btnPersonal.Location = new System.Drawing.Point(15, 467);
            this.btnPersonal.Name = "btnPersonal";
            this.btnPersonal.Size = new System.Drawing.Size(246, 57);
            this.btnPersonal.TabIndex = 37;
            this.btnPersonal.Tag = "Personal";
            this.btnPersonal.Text = " Personal";
            this.btnPersonal.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnPersonal.Click += new System.EventHandler(this.btnPersonal_Click);
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.Animated = true;
            this.btnConfiguracion.BorderRadius = 10;
            this.btnConfiguracion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfiguracion.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnConfiguracion.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnConfiguracion.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnConfiguracion.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnConfiguracion.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnConfiguracion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnConfiguracion.FillColor = System.Drawing.Color.Transparent;
            this.btnConfiguracion.FillColor2 = System.Drawing.Color.Transparent;
            this.btnConfiguracion.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguracion.ForeColor = System.Drawing.Color.White;
            this.btnConfiguracion.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnConfiguracion.Image = global::PROYECTOMECANICO.Properties.Resources.configurations_102859;
            this.btnConfiguracion.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnConfiguracion.ImageSize = new System.Drawing.Size(50, 50);
            this.btnConfiguracion.Location = new System.Drawing.Point(15, 524);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(246, 61);
            this.btnConfiguracion.TabIndex = 36;
            this.btnConfiguracion.Tag = "Ajustes";
            this.btnConfiguracion.Text = "Ajustes";
            this.btnConfiguracion.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            // 
            // btnInventario
            // 
            this.btnInventario.Animated = true;
            this.btnInventario.BorderRadius = 10;
            this.btnInventario.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInventario.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnInventario.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnInventario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnInventario.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnInventario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnInventario.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInventario.FillColor = System.Drawing.Color.Transparent;
            this.btnInventario.FillColor2 = System.Drawing.Color.Transparent;
            this.btnInventario.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInventario.ForeColor = System.Drawing.Color.White;
            this.btnInventario.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnInventario.Image = global::PROYECTOMECANICO.Properties.Resources.box_open_102904;
            this.btnInventario.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInventario.ImageSize = new System.Drawing.Size(50, 50);
            this.btnInventario.Location = new System.Drawing.Point(15, 322);
            this.btnInventario.Name = "btnInventario";
            this.btnInventario.Size = new System.Drawing.Size(246, 68);
            this.btnInventario.TabIndex = 35;
            this.btnInventario.Tag = " Inventario";
            this.btnInventario.Text = " Inventario";
            this.btnInventario.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInventario.Click += new System.EventHandler(this.btnInventario_Click);
            // 
            // btnTaller
            // 
            this.btnTaller.Animated = true;
            this.btnTaller.BorderRadius = 10;
            this.btnTaller.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTaller.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnTaller.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnTaller.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnTaller.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnTaller.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnTaller.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnTaller.FillColor = System.Drawing.Color.Transparent;
            this.btnTaller.FillColor2 = System.Drawing.Color.Transparent;
            this.btnTaller.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTaller.ForeColor = System.Drawing.Color.White;
            this.btnTaller.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnTaller.Image = global::PROYECTOMECANICO.Properties.Resources.tools2_102885;
            this.btnTaller.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnTaller.ImageSize = new System.Drawing.Size(50, 50);
            this.btnTaller.Location = new System.Drawing.Point(15, 254);
            this.btnTaller.Name = "btnTaller";
            this.btnTaller.Size = new System.Drawing.Size(246, 68);
            this.btnTaller.TabIndex = 34;
            this.btnTaller.Tag = "Taller";
            this.btnTaller.Text = " Taller";
            this.btnTaller.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnTaller.Click += new System.EventHandler(this.btnTaller_Click);
            // 
            // btnClientes
            // 
            this.btnClientes.Animated = true;
            this.btnClientes.BorderRadius = 10;
            this.btnClientes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClientes.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnClientes.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnClientes.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClientes.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnClientes.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnClientes.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnClientes.FillColor = System.Drawing.Color.Transparent;
            this.btnClientes.FillColor2 = System.Drawing.Color.Transparent;
            this.btnClientes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClientes.ForeColor = System.Drawing.Color.White;
            this.btnClientes.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnClientes.Image = global::PROYECTOMECANICO.Properties.Resources.user_female_102881;
            this.btnClientes.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnClientes.ImageSize = new System.Drawing.Size(50, 50);
            this.btnClientes.Location = new System.Drawing.Point(15, 186);
            this.btnClientes.Name = "btnClientes";
            this.btnClientes.Size = new System.Drawing.Size(246, 68);
            this.btnClientes.TabIndex = 33;
            this.btnClientes.Tag = "Clientes";
            this.btnClientes.Text = "Clientes";
            this.btnClientes.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnClientes.Click += new System.EventHandler(this.btnClientes_Click);
            // 
            // btnInicio
            // 
            this.btnInicio.Animated = true;
            this.btnInicio.BorderRadius = 10;
            this.btnInicio.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInicio.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnInicio.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnInicio.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnInicio.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnInicio.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnInicio.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnInicio.FillColor = System.Drawing.Color.Transparent;
            this.btnInicio.FillColor2 = System.Drawing.Color.Transparent;
            this.btnInicio.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInicio.ForeColor = System.Drawing.Color.White;
            this.btnInicio.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnInicio.Image = global::PROYECTOMECANICO.Properties.Resources.laptop_content_102849;
            this.btnInicio.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInicio.ImageSize = new System.Drawing.Size(50, 50);
            this.btnInicio.Location = new System.Drawing.Point(15, 118);
            this.btnInicio.Name = "btnInicio";
            this.btnInicio.Size = new System.Drawing.Size(246, 68);
            this.btnInicio.TabIndex = 32;
            this.btnInicio.Tag = "Inicio";
            this.btnInicio.Text = " Inicio";
            this.btnInicio.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnInicio.Click += new System.EventHandler(this.button1_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PROYECTOMECANICO.Properties.Resources.unnamed__1_1;
            this.pictureBox1.Location = new System.Drawing.Point(100, 9);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(76, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // btnMenu
            // 
            this.btnMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(102)))), ((int)(((byte)(244)))));
            this.btnMenu.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMenu.FlatAppearance.BorderSize = 0;
            this.btnMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenu.IconChar = FontAwesome.Sharp.IconChar.AlignJustify;
            this.btnMenu.IconColor = System.Drawing.Color.White;
            this.btnMenu.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnMenu.IconSize = 35;
            this.btnMenu.Location = new System.Drawing.Point(199, 0);
            this.btnMenu.Name = "btnMenu";
            this.btnMenu.Size = new System.Drawing.Size(62, 60);
            this.btnMenu.TabIndex = 5;
            this.btnMenu.UseVisualStyleBackColor = false;
            this.btnMenu.Click += new System.EventHandler(this.btnMenu_Click);
            // 
            // btnReportes
            // 
            this.btnReportes.Animated = true;
            this.btnReportes.BorderRadius = 10;
            this.btnReportes.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReportes.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnReportes.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnReportes.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReportes.DisabledState.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnReportes.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnReportes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnReportes.FillColor = System.Drawing.Color.Transparent;
            this.btnReportes.FillColor2 = System.Drawing.Color.Transparent;
            this.btnReportes.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportes.ForeColor = System.Drawing.Color.White;
            this.btnReportes.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal;
            this.btnReportes.Image = global::PROYECTOMECANICO.Properties.Resources.checklist_report_analysis_doctor_hospital_clipboard_checkup_reports_icon_210692;
            this.btnReportes.ImageAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReportes.ImageSize = new System.Drawing.Size(50, 50);
            this.btnReportes.Location = new System.Drawing.Point(15, 585);
            this.btnReportes.Name = "btnReportes";
            this.btnReportes.Size = new System.Drawing.Size(246, 60);
            this.btnReportes.TabIndex = 40;
            this.btnReportes.Tag = "Ajustes";
            this.btnReportes.Text = "Reportes";
            this.btnReportes.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.btnReportes.Click += new System.EventHandler(this.btnReportes_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(102)))), ((int)(((byte)(244)))));
            this.btnCerrarSesion.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnCerrarSesion.FlatAppearance.BorderSize = 0;
            this.btnCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarSesion.IconChar = FontAwesome.Sharp.IconChar.RightFromBracket;
            this.btnCerrarSesion.IconColor = System.Drawing.Color.Black;
            this.btnCerrarSesion.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnCerrarSesion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarSesion.Location = new System.Drawing.Point(15, 645);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Size = new System.Drawing.Size(246, 55);
            this.btnCerrarSesion.TabIndex = 38;
            this.btnCerrarSesion.Tag = "     Cerrar Sesion";
            this.btnCerrarSesion.Text = "     Cerrar Sesion";
            this.btnCerrarSesion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCerrarSesion.UseVisualStyleBackColor = false;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panelMenu);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Taller Mecanico";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label lblSesion;
        public System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblNombreTaller;
        private FontAwesome.Sharp.IconButton btnMenu;
        private System.Windows.Forms.Label label2;
        private FontAwesome.Sharp.IconButton iconButton3;
        private FontAwesome.Sharp.IconButton iconButton2;
        private Guna.UI2.WinForms.Guna2GradientButton btnPersonal;
        private Guna.UI2.WinForms.Guna2GradientButton btnConfiguracion;
        private Guna.UI2.WinForms.Guna2GradientButton btnInventario;
        private Guna.UI2.WinForms.Guna2GradientButton btnTaller;
        private Guna.UI2.WinForms.Guna2GradientButton btnClientes;
        private Guna.UI2.WinForms.Guna2GradientButton btnInicio;
        private FontAwesome.Sharp.IconButton btnCerrarSesion;
        private Guna.UI2.WinForms.Guna2GradientButton btnFacturacion;
        private Guna.UI2.WinForms.Guna2GradientButton btnReportes;
    }
}

