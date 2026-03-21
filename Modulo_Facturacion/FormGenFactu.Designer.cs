namespace PROYECTOMECANICO.Modulo_Facturacion
{
    partial class FormGenFactu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlPrincipal = new Guna.UI2.WinForms.Guna2TabControl();
            this.tabPageOT = new System.Windows.Forms.TabPage();
            this.panelOTInfo = new System.Windows.Forms.Panel();
            this.labelOT_Titulo = new System.Windows.Forms.Label();
            this.labelOT_Buscar = new System.Windows.Forms.Label();
            this.txtBuscarOT = new System.Windows.Forms.TextBox();
            this.btnBuscadorOrdenTrabajo = new FontAwesome.Sharp.IconButton();
            this.lstOTResultados = new System.Windows.Forms.ListBox();
            this.btnCargarItemsOT = new System.Windows.Forms.Button();
            this.labelOT_TipoDoc = new System.Windows.Forms.Label();
            this.labelOT_NumDoc = new System.Windows.Forms.Label();
            this.labelOT_Nombre = new System.Windows.Forms.Label();
            this.labelOT_Direccion = new System.Windows.Forms.Label();
            this.labelOT_Telefono = new System.Windows.Forms.Label();
            this.labelOT_Email = new System.Windows.Forms.Label();
            this.txtTipoDocOT = new System.Windows.Forms.TextBox();
            this.txtNumDocOT = new System.Windows.Forms.TextBox();
            this.txtNombreOT = new System.Windows.Forms.TextBox();
            this.txtDireccionOT = new System.Windows.Forms.TextBox();
            this.txtTelefonoOT = new System.Windows.Forms.TextBox();
            this.txtEmailOT = new System.Windows.Forms.TextBox();
            this.btnConfirmarReceptorOT = new System.Windows.Forms.Button();
            this.tabPageVentaDirecta = new System.Windows.Forms.TabPage();
            this.panelVDInfo = new System.Windows.Forms.Panel();
            this.labelVD_Titulo = new System.Windows.Forms.Label();
            this.rbClienteExistenteVD = new System.Windows.Forms.RadioButton();
            this.rbNuevoClienteVD = new System.Windows.Forms.RadioButton();
            this.rbConsumidorFinalVD = new System.Windows.Forms.RadioButton();
            this.labelVD_Buscar = new System.Windows.Forms.Label();
            this.txtBuscarClienteVD = new System.Windows.Forms.TextBox();
            this.btnBuscadorClientesVD = new FontAwesome.Sharp.IconButton();
            this.lstClientesVD = new System.Windows.Forms.ListBox();
            this.btnNuevoClienteVD = new System.Windows.Forms.Button();
            this.labelVD_TipoDoc = new System.Windows.Forms.Label();
            this.labelVD_NumDoc = new System.Windows.Forms.Label();
            this.labelVD_Nombre = new System.Windows.Forms.Label();
            this.labelVD_Direccion = new System.Windows.Forms.Label();
            this.labelVD_Telefono = new System.Windows.Forms.Label();
            this.labelVD_Email = new System.Windows.Forms.Label();
            this.txtTipoDocVD = new System.Windows.Forms.TextBox();
            this.txtNumDocVD = new System.Windows.Forms.TextBox();
            this.txtNombreVD = new System.Windows.Forms.TextBox();
            this.txtDireccionVD = new System.Windows.Forms.TextBox();
            this.txtTelefonoVD = new System.Windows.Forms.TextBox();
            this.txtEmailVD = new System.Windows.Forms.TextBox();
            this.dgvItems = new Guna.UI2.WinForms.Guna2DataGridView();
            this.guna2PanelTotales = new Guna.UI2.WinForms.Guna2Panel();
            this.lblDescuentoInfo = new System.Windows.Forms.Label();
            this.btnAplicarDescuento = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbDescuentoItem = new Guna.UI2.WinForms.Guna2ComboBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnDelItem = new System.Windows.Forms.Button();
            this.cmbImpuesto = new System.Windows.Forms.ComboBox();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGenerarFactura = new System.Windows.Forms.Button();
            this.btnVistaPrevia = new System.Windows.Forms.Button();
            this.tabControlPrincipal.SuspendLayout();
            this.tabPageOT.SuspendLayout();
            this.panelOTInfo.SuspendLayout();
            this.tabPageVentaDirecta.SuspendLayout();
            this.panelVDInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.guna2PanelTotales.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlPrincipal
            // 
            this.tabControlPrincipal.Controls.Add(this.tabPageOT);
            this.tabControlPrincipal.Controls.Add(this.tabPageVentaDirecta);
            this.tabControlPrincipal.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControlPrincipal.ItemSize = new System.Drawing.Size(150, 50);
            this.tabControlPrincipal.Location = new System.Drawing.Point(0, 0);
            this.tabControlPrincipal.Name = "tabControlPrincipal";
            this.tabControlPrincipal.SelectedIndex = 0;
            this.tabControlPrincipal.Size = new System.Drawing.Size(896, 380);
            this.tabControlPrincipal.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.tabControlPrincipal.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.tabControlPrincipal.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabControlPrincipal.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.tabControlPrincipal.TabButtonHoverState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.tabControlPrincipal.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.tabControlPrincipal.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabControlPrincipal.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabControlPrincipal.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.tabControlPrincipal.TabButtonIdleState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabControlPrincipal.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.tabControlPrincipal.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(37)))), ((int)(((byte)(49)))));
            this.tabControlPrincipal.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabControlPrincipal.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.tabControlPrincipal.TabButtonSelectedState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(132)))), ((int)(((byte)(255)))));
            this.tabControlPrincipal.TabButtonSize = new System.Drawing.Size(150, 50);
            this.tabControlPrincipal.TabIndex = 0;
            this.tabControlPrincipal.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.tabControlPrincipal.TabMenuOrientation = Guna.UI2.WinForms.TabMenuOrientation.HorizontalTop;
            // 
            // tabPageOT
            // 
            this.tabPageOT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.tabPageOT.Controls.Add(this.panelOTInfo);
            this.tabPageOT.Location = new System.Drawing.Point(4, 54);
            this.tabPageOT.Name = "tabPageOT";
            this.tabPageOT.Padding = new System.Windows.Forms.Padding(15);
            this.tabPageOT.Size = new System.Drawing.Size(888, 322);
            this.tabPageOT.TabIndex = 0;
            this.tabPageOT.Text = "Orden de Trabajo";
            // 
            // panelOTInfo
            // 
            this.panelOTInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelOTInfo.Controls.Add(this.labelOT_Titulo);
            this.panelOTInfo.Controls.Add(this.labelOT_Buscar);
            this.panelOTInfo.Controls.Add(this.txtBuscarOT);
            this.panelOTInfo.Controls.Add(this.btnBuscadorOrdenTrabajo);
            this.panelOTInfo.Controls.Add(this.lstOTResultados);
            this.panelOTInfo.Controls.Add(this.btnCargarItemsOT);
            this.panelOTInfo.Controls.Add(this.labelOT_TipoDoc);
            this.panelOTInfo.Controls.Add(this.labelOT_NumDoc);
            this.panelOTInfo.Controls.Add(this.labelOT_Nombre);
            this.panelOTInfo.Controls.Add(this.labelOT_Direccion);
            this.panelOTInfo.Controls.Add(this.labelOT_Telefono);
            this.panelOTInfo.Controls.Add(this.labelOT_Email);
            this.panelOTInfo.Controls.Add(this.txtTipoDocOT);
            this.panelOTInfo.Controls.Add(this.txtNumDocOT);
            this.panelOTInfo.Controls.Add(this.txtNombreOT);
            this.panelOTInfo.Controls.Add(this.txtDireccionOT);
            this.panelOTInfo.Controls.Add(this.txtTelefonoOT);
            this.panelOTInfo.Controls.Add(this.txtEmailOT);
            this.panelOTInfo.Controls.Add(this.btnConfirmarReceptorOT);
            this.panelOTInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelOTInfo.Location = new System.Drawing.Point(15, 15);
            this.panelOTInfo.Name = "panelOTInfo";
            this.panelOTInfo.Size = new System.Drawing.Size(858, 292);
            this.panelOTInfo.TabIndex = 0;
            // 
            // labelOT_Titulo
            // 
            this.labelOT_Titulo.AutoSize = true;
            this.labelOT_Titulo.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.labelOT_Titulo.ForeColor = System.Drawing.Color.White;
            this.labelOT_Titulo.Location = new System.Drawing.Point(10, 10);
            this.labelOT_Titulo.Name = "labelOT_Titulo";
            this.labelOT_Titulo.Size = new System.Drawing.Size(288, 25);
            this.labelOT_Titulo.TabIndex = 0;
            this.labelOT_Titulo.Text = "Facturar desde Orden de Trabajo";
            // 
            // labelOT_Buscar
            // 
            this.labelOT_Buscar.AutoSize = true;
            this.labelOT_Buscar.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.labelOT_Buscar.ForeColor = System.Drawing.Color.White;
            this.labelOT_Buscar.Location = new System.Drawing.Point(10, 50);
            this.labelOT_Buscar.Name = "labelOT_Buscar";
            this.labelOT_Buscar.Size = new System.Drawing.Size(180, 20);
            this.labelOT_Buscar.TabIndex = 1;
            this.labelOT_Buscar.Text = "Buscar Orden de Trabajo:";
            // 
            // txtBuscarOT
            // 
            this.txtBuscarOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.txtBuscarOT.Location = new System.Drawing.Point(10, 75);
            this.txtBuscarOT.Name = "txtBuscarOT";
            this.txtBuscarOT.Size = new System.Drawing.Size(320, 27);
            this.txtBuscarOT.TabIndex = 2;
            // 
            // btnBuscadorOrdenTrabajo
            // 
            this.btnBuscadorOrdenTrabajo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnBuscadorOrdenTrabajo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscadorOrdenTrabajo.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorOrdenTrabajo.IconColor = System.Drawing.Color.White;
            this.btnBuscadorOrdenTrabajo.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorOrdenTrabajo.IconSize = 28;
            this.btnBuscadorOrdenTrabajo.Location = new System.Drawing.Point(336, 71);
            this.btnBuscadorOrdenTrabajo.Name = "btnBuscadorOrdenTrabajo";
            this.btnBuscadorOrdenTrabajo.Size = new System.Drawing.Size(35, 35);
            this.btnBuscadorOrdenTrabajo.TabIndex = 3;
            this.btnBuscadorOrdenTrabajo.UseVisualStyleBackColor = true;
            this.btnBuscadorOrdenTrabajo.Click += new System.EventHandler(this.btnBuscadorOrdenTrabajo_Click);
            // 
            // lstOTResultados
            // 
            this.lstOTResultados.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lstOTResultados.FormattingEnabled = true;
            this.lstOTResultados.ItemHeight = 17;
            this.lstOTResultados.Location = new System.Drawing.Point(10, 110);
            this.lstOTResultados.Name = "lstOTResultados";
            this.lstOTResultados.Size = new System.Drawing.Size(361, 72);
            this.lstOTResultados.TabIndex = 4;
            this.lstOTResultados.Visible = false;
            // 
            // btnCargarItemsOT
            // 
            this.btnCargarItemsOT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnCargarItemsOT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarItemsOT.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCargarItemsOT.ForeColor = System.Drawing.Color.White;
            this.btnCargarItemsOT.Location = new System.Drawing.Point(10, 195);
            this.btnCargarItemsOT.Name = "btnCargarItemsOT";
            this.btnCargarItemsOT.Size = new System.Drawing.Size(120, 35);
            this.btnCargarItemsOT.TabIndex = 5;
            this.btnCargarItemsOT.Text = "Cargar Items";
            this.btnCargarItemsOT.UseVisualStyleBackColor = false;
            // 
            // labelOT_TipoDoc
            // 
            this.labelOT_TipoDoc.AutoSize = true;
            this.labelOT_TipoDoc.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelOT_TipoDoc.ForeColor = System.Drawing.Color.White;
            this.labelOT_TipoDoc.Location = new System.Drawing.Point(400, 20);
            this.labelOT_TipoDoc.Name = "labelOT_TipoDoc";
            this.labelOT_TipoDoc.Size = new System.Drawing.Size(69, 19);
            this.labelOT_TipoDoc.TabIndex = 6;
            this.labelOT_TipoDoc.Text = "Tipo Doc:";
            // 
            // labelOT_NumDoc
            // 
            this.labelOT_NumDoc.AutoSize = true;
            this.labelOT_NumDoc.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelOT_NumDoc.ForeColor = System.Drawing.Color.White;
            this.labelOT_NumDoc.Location = new System.Drawing.Point(610, 20);
            this.labelOT_NumDoc.Name = "labelOT_NumDoc";
            this.labelOT_NumDoc.Size = new System.Drawing.Size(58, 19);
            this.labelOT_NumDoc.TabIndex = 7;
            this.labelOT_NumDoc.Text = "Nº Doc:";
            // 
            // labelOT_Nombre
            // 
            this.labelOT_Nombre.AutoSize = true;
            this.labelOT_Nombre.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelOT_Nombre.ForeColor = System.Drawing.Color.White;
            this.labelOT_Nombre.Location = new System.Drawing.Point(400, 70);
            this.labelOT_Nombre.Name = "labelOT_Nombre";
            this.labelOT_Nombre.Size = new System.Drawing.Size(63, 19);
            this.labelOT_Nombre.TabIndex = 8;
            this.labelOT_Nombre.Text = "Nombre:";
            // 
            // labelOT_Direccion
            // 
            this.labelOT_Direccion.AutoSize = true;
            this.labelOT_Direccion.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelOT_Direccion.ForeColor = System.Drawing.Color.White;
            this.labelOT_Direccion.Location = new System.Drawing.Point(400, 120);
            this.labelOT_Direccion.Name = "labelOT_Direccion";
            this.labelOT_Direccion.Size = new System.Drawing.Size(72, 19);
            this.labelOT_Direccion.TabIndex = 9;
            this.labelOT_Direccion.Text = "Dirección:";
            // 
            // labelOT_Telefono
            // 
            this.labelOT_Telefono.AutoSize = true;
            this.labelOT_Telefono.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelOT_Telefono.ForeColor = System.Drawing.Color.White;
            this.labelOT_Telefono.Location = new System.Drawing.Point(400, 170);
            this.labelOT_Telefono.Name = "labelOT_Telefono";
            this.labelOT_Telefono.Size = new System.Drawing.Size(66, 19);
            this.labelOT_Telefono.TabIndex = 10;
            this.labelOT_Telefono.Text = "Teléfono:";
            // 
            // labelOT_Email
            // 
            this.labelOT_Email.AutoSize = true;
            this.labelOT_Email.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelOT_Email.ForeColor = System.Drawing.Color.White;
            this.labelOT_Email.Location = new System.Drawing.Point(610, 170);
            this.labelOT_Email.Name = "labelOT_Email";
            this.labelOT_Email.Size = new System.Drawing.Size(46, 19);
            this.labelOT_Email.TabIndex = 11;
            this.labelOT_Email.Text = "Email:";
            // 
            // txtTipoDocOT
            // 
            this.txtTipoDocOT.BackColor = System.Drawing.Color.White;
            this.txtTipoDocOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtTipoDocOT.Location = new System.Drawing.Point(400, 42);
            this.txtTipoDocOT.Name = "txtTipoDocOT";
            this.txtTipoDocOT.Size = new System.Drawing.Size(100, 25);
            this.txtTipoDocOT.TabIndex = 12;
            // 
            // txtNumDocOT
            // 
            this.txtNumDocOT.BackColor = System.Drawing.Color.White;
            this.txtNumDocOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtNumDocOT.Location = new System.Drawing.Point(610, 42);
            this.txtNumDocOT.Name = "txtNumDocOT";
            this.txtNumDocOT.Size = new System.Drawing.Size(200, 25);
            this.txtNumDocOT.TabIndex = 13;
            // 
            // txtNombreOT
            // 
            this.txtNombreOT.BackColor = System.Drawing.Color.White;
            this.txtNombreOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtNombreOT.Location = new System.Drawing.Point(400, 92);
            this.txtNombreOT.Name = "txtNombreOT";
            this.txtNombreOT.Size = new System.Drawing.Size(410, 25);
            this.txtNombreOT.TabIndex = 14;
            // 
            // txtDireccionOT
            // 
            this.txtDireccionOT.BackColor = System.Drawing.Color.White;
            this.txtDireccionOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtDireccionOT.Location = new System.Drawing.Point(400, 142);
            this.txtDireccionOT.Name = "txtDireccionOT";
            this.txtDireccionOT.Size = new System.Drawing.Size(410, 25);
            this.txtDireccionOT.TabIndex = 15;
            // 
            // txtTelefonoOT
            // 
            this.txtTelefonoOT.BackColor = System.Drawing.Color.White;
            this.txtTelefonoOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtTelefonoOT.Location = new System.Drawing.Point(400, 192);
            this.txtTelefonoOT.Name = "txtTelefonoOT";
            this.txtTelefonoOT.Size = new System.Drawing.Size(150, 25);
            this.txtTelefonoOT.TabIndex = 16;
            // 
            // txtEmailOT
            // 
            this.txtEmailOT.BackColor = System.Drawing.Color.White;
            this.txtEmailOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtEmailOT.Location = new System.Drawing.Point(610, 192);
            this.txtEmailOT.Name = "txtEmailOT";
            this.txtEmailOT.Size = new System.Drawing.Size(200, 25);
            this.txtEmailOT.TabIndex = 17;
            // 
            // btnConfirmarReceptorOT
            // 
            this.btnConfirmarReceptorOT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnConfirmarReceptorOT.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfirmarReceptorOT.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnConfirmarReceptorOT.ForeColor = System.Drawing.Color.White;
            this.btnConfirmarReceptorOT.Location = new System.Drawing.Point(690, 235);
            this.btnConfirmarReceptorOT.Name = "btnConfirmarReceptorOT";
            this.btnConfirmarReceptorOT.Size = new System.Drawing.Size(120, 35);
            this.btnConfirmarReceptorOT.TabIndex = 18;
            this.btnConfirmarReceptorOT.Text = "Confirmar";
            this.btnConfirmarReceptorOT.UseVisualStyleBackColor = false;
            // 
            // tabPageVentaDirecta
            // 
            this.tabPageVentaDirecta.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.tabPageVentaDirecta.Controls.Add(this.panelVDInfo);
            this.tabPageVentaDirecta.Location = new System.Drawing.Point(4, 54);
            this.tabPageVentaDirecta.Name = "tabPageVentaDirecta";
            this.tabPageVentaDirecta.Padding = new System.Windows.Forms.Padding(15);
            this.tabPageVentaDirecta.Size = new System.Drawing.Size(888, 322);
            this.tabPageVentaDirecta.TabIndex = 1;
            this.tabPageVentaDirecta.Text = "Venta Directa";
            // 
            // panelVDInfo
            // 
            this.panelVDInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelVDInfo.Controls.Add(this.labelVD_Titulo);
            this.panelVDInfo.Controls.Add(this.rbClienteExistenteVD);
            this.panelVDInfo.Controls.Add(this.rbNuevoClienteVD);
            this.panelVDInfo.Controls.Add(this.rbConsumidorFinalVD);
            this.panelVDInfo.Controls.Add(this.labelVD_Buscar);
            this.panelVDInfo.Controls.Add(this.txtBuscarClienteVD);
            this.panelVDInfo.Controls.Add(this.btnBuscadorClientesVD);
            this.panelVDInfo.Controls.Add(this.lstClientesVD);
            this.panelVDInfo.Controls.Add(this.btnNuevoClienteVD);
            this.panelVDInfo.Controls.Add(this.labelVD_TipoDoc);
            this.panelVDInfo.Controls.Add(this.labelVD_NumDoc);
            this.panelVDInfo.Controls.Add(this.labelVD_Nombre);
            this.panelVDInfo.Controls.Add(this.labelVD_Direccion);
            this.panelVDInfo.Controls.Add(this.labelVD_Telefono);
            this.panelVDInfo.Controls.Add(this.labelVD_Email);
            this.panelVDInfo.Controls.Add(this.txtTipoDocVD);
            this.panelVDInfo.Controls.Add(this.txtNumDocVD);
            this.panelVDInfo.Controls.Add(this.txtNombreVD);
            this.panelVDInfo.Controls.Add(this.txtDireccionVD);
            this.panelVDInfo.Controls.Add(this.txtTelefonoVD);
            this.panelVDInfo.Controls.Add(this.txtEmailVD);
            this.panelVDInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelVDInfo.Location = new System.Drawing.Point(15, 15);
            this.panelVDInfo.Name = "panelVDInfo";
            this.panelVDInfo.Size = new System.Drawing.Size(858, 292);
            this.panelVDInfo.TabIndex = 0;
            // 
            // labelVD_Titulo
            // 
            this.labelVD_Titulo.AutoSize = true;
            this.labelVD_Titulo.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.labelVD_Titulo.ForeColor = System.Drawing.Color.White;
            this.labelVD_Titulo.Location = new System.Drawing.Point(10, 10);
            this.labelVD_Titulo.Name = "labelVD_Titulo";
            this.labelVD_Titulo.Size = new System.Drawing.Size(232, 25);
            this.labelVD_Titulo.TabIndex = 0;
            this.labelVD_Titulo.Text = "Facturación Venta Directa";
            // 
            // rbClienteExistenteVD
            // 
            this.rbClienteExistenteVD.AutoSize = true;
            this.rbClienteExistenteVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.rbClienteExistenteVD.ForeColor = System.Drawing.Color.White;
            this.rbClienteExistenteVD.Location = new System.Drawing.Point(10, 50);
            this.rbClienteExistenteVD.Name = "rbClienteExistenteVD";
            this.rbClienteExistenteVD.Size = new System.Drawing.Size(131, 23);
            this.rbClienteExistenteVD.TabIndex = 1;
            this.rbClienteExistenteVD.TabStop = true;
            this.rbClienteExistenteVD.Text = "Cliente Existente";
            this.rbClienteExistenteVD.UseVisualStyleBackColor = true;
            // 
            // rbNuevoClienteVD
            // 
            this.rbNuevoClienteVD.AutoSize = true;
            this.rbNuevoClienteVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.rbNuevoClienteVD.ForeColor = System.Drawing.Color.White;
            this.rbNuevoClienteVD.Location = new System.Drawing.Point(150, 50);
            this.rbNuevoClienteVD.Name = "rbNuevoClienteVD";
            this.rbNuevoClienteVD.Size = new System.Drawing.Size(116, 23);
            this.rbNuevoClienteVD.TabIndex = 2;
            this.rbNuevoClienteVD.TabStop = true;
            this.rbNuevoClienteVD.Text = "Nuevo Cliente";
            this.rbNuevoClienteVD.UseVisualStyleBackColor = true;
            // 
            // rbConsumidorFinalVD
            // 
            this.rbConsumidorFinalVD.AutoSize = true;
            this.rbConsumidorFinalVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.rbConsumidorFinalVD.ForeColor = System.Drawing.Color.White;
            this.rbConsumidorFinalVD.Location = new System.Drawing.Point(272, 50);
            this.rbConsumidorFinalVD.Name = "rbConsumidorFinalVD";
            this.rbConsumidorFinalVD.Size = new System.Drawing.Size(137, 23);
            this.rbConsumidorFinalVD.TabIndex = 3;
            this.rbConsumidorFinalVD.TabStop = true;
            this.rbConsumidorFinalVD.Text = "Consumidor Final";
            this.rbConsumidorFinalVD.UseVisualStyleBackColor = true;
            // 
            // labelVD_Buscar
            // 
            this.labelVD_Buscar.AutoSize = true;
            this.labelVD_Buscar.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.labelVD_Buscar.ForeColor = System.Drawing.Color.White;
            this.labelVD_Buscar.Location = new System.Drawing.Point(10, 90);
            this.labelVD_Buscar.Name = "labelVD_Buscar";
            this.labelVD_Buscar.Size = new System.Drawing.Size(109, 20);
            this.labelVD_Buscar.TabIndex = 4;
            this.labelVD_Buscar.Text = "Buscar Cliente:";
            // 
            // txtBuscarClienteVD
            // 
            this.txtBuscarClienteVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.txtBuscarClienteVD.Location = new System.Drawing.Point(10, 115);
            this.txtBuscarClienteVD.Name = "txtBuscarClienteVD";
            this.txtBuscarClienteVD.Size = new System.Drawing.Size(320, 27);
            this.txtBuscarClienteVD.TabIndex = 5;
            // 
            // btnBuscadorClientesVD
            // 
            this.btnBuscadorClientesVD.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnBuscadorClientesVD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscadorClientesVD.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorClientesVD.IconColor = System.Drawing.Color.White;
            this.btnBuscadorClientesVD.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorClientesVD.IconSize = 28;
            this.btnBuscadorClientesVD.Location = new System.Drawing.Point(336, 111);
            this.btnBuscadorClientesVD.Name = "btnBuscadorClientesVD";
            this.btnBuscadorClientesVD.Size = new System.Drawing.Size(35, 35);
            this.btnBuscadorClientesVD.TabIndex = 6;
            this.btnBuscadorClientesVD.UseVisualStyleBackColor = true;
            this.btnBuscadorClientesVD.Click += new System.EventHandler(this.btnBuscadorClientes_Click);
            // 
            // lstClientesVD
            // 
            this.lstClientesVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lstClientesVD.FormattingEnabled = true;
            this.lstClientesVD.ItemHeight = 17;
            this.lstClientesVD.Location = new System.Drawing.Point(10, 150);
            this.lstClientesVD.Name = "lstClientesVD";
            this.lstClientesVD.Size = new System.Drawing.Size(361, 72);
            this.lstClientesVD.TabIndex = 7;
            this.lstClientesVD.Visible = false;
            // 
            // btnNuevoClienteVD
            // 
            this.btnNuevoClienteVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnNuevoClienteVD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNuevoClienteVD.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnNuevoClienteVD.ForeColor = System.Drawing.Color.White;
            this.btnNuevoClienteVD.Location = new System.Drawing.Point(377, 190);
            this.btnNuevoClienteVD.Name = "btnNuevoClienteVD";
            this.btnNuevoClienteVD.Size = new System.Drawing.Size(40, 36);
            this.btnNuevoClienteVD.TabIndex = 8;
            this.btnNuevoClienteVD.Text = "+";
            this.btnNuevoClienteVD.UseVisualStyleBackColor = false;
            // 
            // labelVD_TipoDoc
            // 
            this.labelVD_TipoDoc.AutoSize = true;
            this.labelVD_TipoDoc.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelVD_TipoDoc.ForeColor = System.Drawing.Color.White;
            this.labelVD_TipoDoc.Location = new System.Drawing.Point(479, 50);
            this.labelVD_TipoDoc.Name = "labelVD_TipoDoc";
            this.labelVD_TipoDoc.Size = new System.Drawing.Size(69, 19);
            this.labelVD_TipoDoc.TabIndex = 9;
            this.labelVD_TipoDoc.Text = "Tipo Doc:";
            // 
            // labelVD_NumDoc
            // 
            this.labelVD_NumDoc.AutoSize = true;
            this.labelVD_NumDoc.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelVD_NumDoc.ForeColor = System.Drawing.Color.White;
            this.labelVD_NumDoc.Location = new System.Drawing.Point(632, 50);
            this.labelVD_NumDoc.Name = "labelVD_NumDoc";
            this.labelVD_NumDoc.Size = new System.Drawing.Size(58, 19);
            this.labelVD_NumDoc.TabIndex = 10;
            this.labelVD_NumDoc.Text = "Nº Doc:";
            // 
            // labelVD_Nombre
            // 
            this.labelVD_Nombre.AutoSize = true;
            this.labelVD_Nombre.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelVD_Nombre.ForeColor = System.Drawing.Color.White;
            this.labelVD_Nombre.Location = new System.Drawing.Point(477, 110);
            this.labelVD_Nombre.Name = "labelVD_Nombre";
            this.labelVD_Nombre.Size = new System.Drawing.Size(63, 19);
            this.labelVD_Nombre.TabIndex = 11;
            this.labelVD_Nombre.Text = "Nombre:";
            // 
            // labelVD_Direccion
            // 
            this.labelVD_Direccion.AutoSize = true;
            this.labelVD_Direccion.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelVD_Direccion.ForeColor = System.Drawing.Color.White;
            this.labelVD_Direccion.Location = new System.Drawing.Point(479, 158);
            this.labelVD_Direccion.Name = "labelVD_Direccion";
            this.labelVD_Direccion.Size = new System.Drawing.Size(72, 19);
            this.labelVD_Direccion.TabIndex = 12;
            this.labelVD_Direccion.Text = "Dirección:";
            // 
            // labelVD_Telefono
            // 
            this.labelVD_Telefono.AutoSize = true;
            this.labelVD_Telefono.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelVD_Telefono.ForeColor = System.Drawing.Color.White;
            this.labelVD_Telefono.Location = new System.Drawing.Point(477, 208);
            this.labelVD_Telefono.Name = "labelVD_Telefono";
            this.labelVD_Telefono.Size = new System.Drawing.Size(66, 19);
            this.labelVD_Telefono.TabIndex = 13;
            this.labelVD_Telefono.Text = "Teléfono:";
            // 
            // labelVD_Email
            // 
            this.labelVD_Email.AutoSize = true;
            this.labelVD_Email.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelVD_Email.ForeColor = System.Drawing.Color.White;
            this.labelVD_Email.Location = new System.Drawing.Point(632, 207);
            this.labelVD_Email.Name = "labelVD_Email";
            this.labelVD_Email.Size = new System.Drawing.Size(46, 19);
            this.labelVD_Email.TabIndex = 14;
            this.labelVD_Email.Text = "Email:";
            // 
            // txtTipoDocVD
            // 
            this.txtTipoDocVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtTipoDocVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtTipoDocVD.Location = new System.Drawing.Point(483, 72);
            this.txtTipoDocVD.Name = "txtTipoDocVD";
            this.txtTipoDocVD.ReadOnly = true;
            this.txtTipoDocVD.Size = new System.Drawing.Size(100, 25);
            this.txtTipoDocVD.TabIndex = 15;
            // 
            // txtNumDocVD
            // 
            this.txtNumDocVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtNumDocVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtNumDocVD.Location = new System.Drawing.Point(625, 72);
            this.txtNumDocVD.Name = "txtNumDocVD";
            this.txtNumDocVD.ReadOnly = true;
            this.txtNumDocVD.Size = new System.Drawing.Size(170, 25);
            this.txtNumDocVD.TabIndex = 16;
            // 
            // txtNombreVD
            // 
            this.txtNombreVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtNombreVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtNombreVD.Location = new System.Drawing.Point(483, 132);
            this.txtNombreVD.Name = "txtNombreVD";
            this.txtNombreVD.ReadOnly = true;
            this.txtNombreVD.Size = new System.Drawing.Size(321, 25);
            this.txtNombreVD.TabIndex = 17;
            // 
            // txtDireccionVD
            // 
            this.txtDireccionVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtDireccionVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtDireccionVD.Location = new System.Drawing.Point(483, 180);
            this.txtDireccionVD.Name = "txtDireccionVD";
            this.txtDireccionVD.ReadOnly = true;
            this.txtDireccionVD.Size = new System.Drawing.Size(321, 25);
            this.txtDireccionVD.TabIndex = 18;
            // 
            // txtTelefonoVD
            // 
            this.txtTelefonoVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtTelefonoVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtTelefonoVD.Location = new System.Drawing.Point(481, 230);
            this.txtTelefonoVD.Name = "txtTelefonoVD";
            this.txtTelefonoVD.ReadOnly = true;
            this.txtTelefonoVD.Size = new System.Drawing.Size(140, 25);
            this.txtTelefonoVD.TabIndex = 19;
            // 
            // txtEmailVD
            // 
            this.txtEmailVD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.txtEmailVD.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtEmailVD.Location = new System.Drawing.Point(636, 230);
            this.txtEmailVD.Name = "txtEmailVD";
            this.txtEmailVD.ReadOnly = true;
            this.txtEmailVD.Size = new System.Drawing.Size(200, 25);
            this.txtEmailVD.TabIndex = 20;
            // 
            // dgvItems
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 10F);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            this.dgvItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvItems.ColumnHeadersHeight = 35;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItems.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvItems.Location = new System.Drawing.Point(5, 382);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.Size = new System.Drawing.Size(887, 150);
            this.dgvItems.TabIndex = 1;
            this.dgvItems.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvItems.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvItems.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvItems.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvItems.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvItems.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvItems.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvItems.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvItems.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvItems.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvItems.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvItems.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvItems.ThemeStyle.HeaderStyle.Height = 35;
            this.dgvItems.ThemeStyle.ReadOnly = false;
            this.dgvItems.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvItems.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvItems.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvItems.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvItems.ThemeStyle.RowsStyle.Height = 22;
            this.dgvItems.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvItems.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // guna2PanelTotales
            // 
            this.guna2PanelTotales.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.guna2PanelTotales.Controls.Add(this.lblDescuentoInfo);
            this.guna2PanelTotales.Controls.Add(this.btnAplicarDescuento);
            this.guna2PanelTotales.Controls.Add(this.label3);
            this.guna2PanelTotales.Controls.Add(this.cmbDescuentoItem);
            this.guna2PanelTotales.Controls.Add(this.btnAddItem);
            this.guna2PanelTotales.Controls.Add(this.btnDelItem);
            this.guna2PanelTotales.Controls.Add(this.cmbImpuesto);
            this.guna2PanelTotales.Controls.Add(this.lblSubtotal);
            this.guna2PanelTotales.Controls.Add(this.lblIVA);
            this.guna2PanelTotales.Controls.Add(this.lblTotal);
            this.guna2PanelTotales.Controls.Add(this.btnGenerarFactura);
            this.guna2PanelTotales.Controls.Add(this.btnVistaPrevia);
            this.guna2PanelTotales.ForeColor = System.Drawing.Color.White;
            this.guna2PanelTotales.Location = new System.Drawing.Point(188, 538);
            this.guna2PanelTotales.Name = "guna2PanelTotales";
            this.guna2PanelTotales.Size = new System.Drawing.Size(529, 203);
            this.guna2PanelTotales.TabIndex = 2;
            // 
            // lblDescuentoInfo
            // 
            this.lblDescuentoInfo.AutoSize = true;
            this.lblDescuentoInfo.Font = new System.Drawing.Font("Yu Gothic UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblDescuentoInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblDescuentoInfo.Location = new System.Drawing.Point(320, 130);
            this.lblDescuentoInfo.Name = "lblDescuentoInfo";
            this.lblDescuentoInfo.Size = new System.Drawing.Size(168, 13);
            this.lblDescuentoInfo.TabIndex = 30;
            this.lblDescuentoInfo.Text = "* Seleccione un item para aplicar";
            // 
            // btnAplicarDescuento
            // 
            this.btnAplicarDescuento.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAplicarDescuento.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnAplicarDescuento.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAplicarDescuento.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.btnAplicarDescuento.ForeColor = System.Drawing.Color.White;
            this.btnAplicarDescuento.Location = new System.Drawing.Point(320, 90);
            this.btnAplicarDescuento.Name = "btnAplicarDescuento";
            this.btnAplicarDescuento.Size = new System.Drawing.Size(130, 32);
            this.btnAplicarDescuento.TabIndex = 26;
            this.btnAplicarDescuento.Text = "Aplicar Descuento";
            this.btnAplicarDescuento.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(320, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(121, 19);
            this.label3.TabIndex = 27;
            this.label3.Text = "Descuento a item:";
            // 
            // cmbDescuentoItem
            // 
            this.cmbDescuentoItem.BackColor = System.Drawing.Color.Transparent;
            this.cmbDescuentoItem.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbDescuentoItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDescuentoItem.FocusedColor = System.Drawing.Color.Empty;
            this.cmbDescuentoItem.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbDescuentoItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbDescuentoItem.ItemHeight = 30;
            this.cmbDescuentoItem.Location = new System.Drawing.Point(320, 45);
            this.cmbDescuentoItem.Name = "cmbDescuentoItem";
            this.cmbDescuentoItem.Size = new System.Drawing.Size(180, 36);
            this.cmbDescuentoItem.TabIndex = 26;
            // 
            // btnAddItem
            // 
            this.btnAddItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnAddItem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnAddItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddItem.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnAddItem.ForeColor = System.Drawing.Color.White;
            this.btnAddItem.Location = new System.Drawing.Point(24, 33);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(110, 35);
            this.btnAddItem.TabIndex = 18;
            this.btnAddItem.Text = "+ Agregar Item";
            this.btnAddItem.UseVisualStyleBackColor = false;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnDelItem
            // 
            this.btnDelItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnDelItem.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnDelItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelItem.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelItem.ForeColor = System.Drawing.Color.White;
            this.btnDelItem.Location = new System.Drawing.Point(24, 73);
            this.btnDelItem.Name = "btnDelItem";
            this.btnDelItem.Size = new System.Drawing.Size(110, 35);
            this.btnDelItem.TabIndex = 19;
            this.btnDelItem.Text = "Eliminar";
            this.btnDelItem.UseVisualStyleBackColor = false;
            this.btnDelItem.Click += new System.EventHandler(this.btnDelItem_Click);
            // 
            // cmbImpuesto
            // 
            this.cmbImpuesto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbImpuesto.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.cmbImpuesto.FormattingEnabled = true;
            this.cmbImpuesto.Location = new System.Drawing.Point(14, 118);
            this.cmbImpuesto.Name = "cmbImpuesto";
            this.cmbImpuesto.Size = new System.Drawing.Size(137, 25);
            this.cmbImpuesto.TabIndex = 20;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblSubtotal.ForeColor = System.Drawing.Color.White;
            this.lblSubtotal.Location = new System.Drawing.Point(180, 20);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(102, 20);
            this.lblSubtotal.TabIndex = 21;
            this.lblSubtotal.Text = "Subtotal: 0.00";
            // 
            // lblIVA
            // 
            this.lblIVA.AutoSize = true;
            this.lblIVA.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblIVA.ForeColor = System.Drawing.Color.White;
            this.lblIVA.Location = new System.Drawing.Point(180, 50);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(68, 20);
            this.lblIVA.TabIndex = 22;
            this.lblIVA.Text = "IVA: 0.00";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.White;
            this.lblTotal.Location = new System.Drawing.Point(180, 80);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(77, 20);
            this.lblTotal.TabIndex = 23;
            this.lblTotal.Text = "Total: 0.00";
            // 
            // btnGenerarFactura
            // 
            this.btnGenerarFactura.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnGenerarFactura.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnGenerarFactura.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarFactura.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnGenerarFactura.ForeColor = System.Drawing.Color.White;
            this.btnGenerarFactura.Location = new System.Drawing.Point(172, 112);
            this.btnGenerarFactura.Name = "btnGenerarFactura";
            this.btnGenerarFactura.Size = new System.Drawing.Size(110, 35);
            this.btnGenerarFactura.TabIndex = 24;
            this.btnGenerarFactura.Text = "💰 Generar";
            this.btnGenerarFactura.UseVisualStyleBackColor = false;
            // 
            // btnVistaPrevia
            // 
            this.btnVistaPrevia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnVistaPrevia.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnVistaPrevia.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVistaPrevia.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnVistaPrevia.ForeColor = System.Drawing.Color.White;
            this.btnVistaPrevia.Location = new System.Drawing.Point(172, 153);
            this.btnVistaPrevia.Name = "btnVistaPrevia";
            this.btnVistaPrevia.Size = new System.Drawing.Size(110, 35);
            this.btnVistaPrevia.TabIndex = 25;
            this.btnVistaPrevia.Text = "👁️ Vista Previa";
            this.btnVistaPrevia.UseVisualStyleBackColor = false;
            // 
            // FormGenFactu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.ClientSize = new System.Drawing.Size(896, 767);
            this.Controls.Add(this.guna2PanelTotales);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.tabControlPrincipal);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormGenFactu";
            this.Text = "Generación de Facturas";
            this.Load += new System.EventHandler(this.FormGenFactu_Load);
            this.tabControlPrincipal.ResumeLayout(false);
            this.tabPageOT.ResumeLayout(false);
            this.panelOTInfo.ResumeLayout(false);
            this.panelOTInfo.PerformLayout();
            this.tabPageVentaDirecta.ResumeLayout(false);
            this.panelVDInfo.ResumeLayout(false);
            this.panelVDInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.guna2PanelTotales.ResumeLayout(false);
            this.guna2PanelTotales.PerformLayout();
            this.ResumeLayout(false);

        }

        // Declaración de controles
        private Guna.UI2.WinForms.Guna2TabControl tabControlPrincipal;
        private System.Windows.Forms.TabPage tabPageOT;
        private System.Windows.Forms.TabPage tabPageVentaDirecta;

        // Controles Tab OT
        private System.Windows.Forms.Panel panelOTInfo;
        private System.Windows.Forms.Label labelOT_Titulo;
        private System.Windows.Forms.Label labelOT_Buscar;
        private System.Windows.Forms.TextBox txtBuscarOT;
        private FontAwesome.Sharp.IconButton btnBuscadorOrdenTrabajo;
        private System.Windows.Forms.ListBox lstOTResultados;
        private System.Windows.Forms.Button btnCargarItemsOT;
        private System.Windows.Forms.Label labelOT_TipoDoc;
        private System.Windows.Forms.Label labelOT_NumDoc;
        private System.Windows.Forms.Label labelOT_Nombre;
        private System.Windows.Forms.Label labelOT_Direccion;
        private System.Windows.Forms.Label labelOT_Telefono;
        private System.Windows.Forms.Label labelOT_Email;
        private System.Windows.Forms.TextBox txtTipoDocOT;
        private System.Windows.Forms.TextBox txtNumDocOT;
        private System.Windows.Forms.TextBox txtNombreOT;
        private System.Windows.Forms.TextBox txtDireccionOT;
        private System.Windows.Forms.TextBox txtTelefonoOT;
        private System.Windows.Forms.TextBox txtEmailOT;
        private System.Windows.Forms.Button btnConfirmarReceptorOT;

        // Controles Tab Venta Directa
        private System.Windows.Forms.Panel panelVDInfo;
        private System.Windows.Forms.Label labelVD_Titulo;
        private System.Windows.Forms.RadioButton rbClienteExistenteVD;
        private System.Windows.Forms.RadioButton rbNuevoClienteVD;
        private System.Windows.Forms.RadioButton rbConsumidorFinalVD;
        private System.Windows.Forms.Label labelVD_Buscar;
        private System.Windows.Forms.TextBox txtBuscarClienteVD;
        private FontAwesome.Sharp.IconButton btnBuscadorClientesVD;
        private System.Windows.Forms.ListBox lstClientesVD;
        private System.Windows.Forms.Button btnNuevoClienteVD;
        private System.Windows.Forms.Label labelVD_TipoDoc;
        private System.Windows.Forms.Label labelVD_NumDoc;
        private System.Windows.Forms.Label labelVD_Nombre;
        private System.Windows.Forms.Label labelVD_Direccion;
        private System.Windows.Forms.Label labelVD_Telefono;
        private System.Windows.Forms.Label labelVD_Email;
        private System.Windows.Forms.TextBox txtTipoDocVD;
        private System.Windows.Forms.TextBox txtNumDocVD;
        private System.Windows.Forms.TextBox txtNombreVD;
        private System.Windows.Forms.TextBox txtDireccionVD;
        private System.Windows.Forms.TextBox txtTelefonoVD;
        private System.Windows.Forms.TextBox txtEmailVD;

        // Controles comunes
        private Guna.UI2.WinForms.Guna2DataGridView dgvItems;
        private Guna.UI2.WinForms.Guna2Panel guna2PanelTotales;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnDelItem;
        private System.Windows.Forms.ComboBox cmbImpuesto;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblIVA;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnGenerarFactura;
        private System.Windows.Forms.Button btnVistaPrevia;

        // Controles de descuentos
        private Guna.UI2.WinForms.Guna2ComboBox cmbDescuentoItem;
        private System.Windows.Forms.Button btnAplicarDescuento;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDescuentoInfo;
    }

}