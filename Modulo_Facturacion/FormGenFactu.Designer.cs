namespace PROYECTOMECANICO.Modulo_Facturacion
{
    partial class FormGenFactu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.rbDesdeOT = new System.Windows.Forms.RadioButton();
            this.rbVentaDirecta = new System.Windows.Forms.RadioButton();
            this.txtBuscarOT = new System.Windows.Forms.TextBox();
            this.lstOTResultados = new System.Windows.Forms.ListBox();
            this.btnCargarItemsOT = new System.Windows.Forms.Button();
            this.rbClienteExistente = new System.Windows.Forms.RadioButton();
            this.rbNuevoCliente = new System.Windows.Forms.RadioButton();
            this.rbConsumidorFinal = new System.Windows.Forms.RadioButton();
            this.txtBuscarCliente = new System.Windows.Forms.TextBox();
            this.lstClientes = new System.Windows.Forms.ListBox();
            this.btnNuevoCliente = new System.Windows.Forms.Button();
            this.txtTipoDoc = new System.Windows.Forms.TextBox();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.txtDireccion = new System.Windows.Forms.TextBox();
            this.txtNumDoc = new System.Windows.Forms.TextBox();
            this.txtTelefono = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.btnAddItem = new System.Windows.Forms.Button();
            this.btnDelItem = new System.Windows.Forms.Button();
            this.cmbImpuesto = new System.Windows.Forms.ComboBox();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnGenerarFactura = new System.Windows.Forms.Button();
            this.btnVistaPrevia = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.dgvItems = new Guna.UI2.WinForms.Guna2DataGridView();
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnBuscadorOrdenTrabajo = new FontAwesome.Sharp.IconButton();
            this.btnBuscadorClientes = new FontAwesome.Sharp.IconButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.guna2Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbDesdeOT
            // 
            this.rbDesdeOT.AutoSize = true;
            this.rbDesdeOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbDesdeOT.Location = new System.Drawing.Point(23, 60);
            this.rbDesdeOT.Name = "rbDesdeOT";
            this.rbDesdeOT.Size = new System.Drawing.Size(99, 25);
            this.rbDesdeOT.TabIndex = 0;
            this.rbDesdeOT.TabStop = true;
            this.rbDesdeOT.Text = "Desde OT";
            this.rbDesdeOT.UseVisualStyleBackColor = true;
            // 
            // rbVentaDirecta
            // 
            this.rbVentaDirecta.AutoSize = true;
            this.rbVentaDirecta.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbVentaDirecta.Location = new System.Drawing.Point(138, 60);
            this.rbVentaDirecta.Name = "rbVentaDirecta";
            this.rbVentaDirecta.Size = new System.Drawing.Size(124, 25);
            this.rbVentaDirecta.TabIndex = 1;
            this.rbVentaDirecta.TabStop = true;
            this.rbVentaDirecta.Text = "Venta directa";
            this.rbVentaDirecta.UseVisualStyleBackColor = true;
            // 
            // txtBuscarOT
            // 
            this.txtBuscarOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarOT.Location = new System.Drawing.Point(21, 112);
            this.txtBuscarOT.Name = "txtBuscarOT";
            this.txtBuscarOT.Size = new System.Drawing.Size(300, 29);
            this.txtBuscarOT.TabIndex = 2;
            // 
            // lstOTResultados
            // 
            this.lstOTResultados.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstOTResultados.FormattingEnabled = true;
            this.lstOTResultados.ItemHeight = 21;
            this.lstOTResultados.Location = new System.Drawing.Point(20, 140);
            this.lstOTResultados.Name = "lstOTResultados";
            this.lstOTResultados.Size = new System.Drawing.Size(300, 88);
            this.lstOTResultados.TabIndex = 3;
            // 
            // btnCargarItemsOT
            // 
            this.btnCargarItemsOT.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarItemsOT.Location = new System.Drawing.Point(365, 112);
            this.btnCargarItemsOT.Name = "btnCargarItemsOT";
            this.btnCargarItemsOT.Size = new System.Drawing.Size(111, 29);
            this.btnCargarItemsOT.TabIndex = 4;
            this.btnCargarItemsOT.Text = "Cargar items";
            this.btnCargarItemsOT.UseVisualStyleBackColor = true;
            // 
            // rbClienteExistente
            // 
            this.rbClienteExistente.AutoSize = true;
            this.rbClienteExistente.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbClienteExistente.Location = new System.Drawing.Point(20, 253);
            this.rbClienteExistente.Name = "rbClienteExistente";
            this.rbClienteExistente.Size = new System.Drawing.Size(149, 25);
            this.rbClienteExistente.TabIndex = 5;
            this.rbClienteExistente.TabStop = true;
            this.rbClienteExistente.Text = "Cliente Existente";
            this.rbClienteExistente.UseVisualStyleBackColor = true;
            // 
            // rbNuevoCliente
            // 
            this.rbNuevoCliente.AutoSize = true;
            this.rbNuevoCliente.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbNuevoCliente.Location = new System.Drawing.Point(175, 253);
            this.rbNuevoCliente.Name = "rbNuevoCliente";
            this.rbNuevoCliente.Size = new System.Drawing.Size(131, 25);
            this.rbNuevoCliente.TabIndex = 6;
            this.rbNuevoCliente.TabStop = true;
            this.rbNuevoCliente.Text = "Nuevo Cliente";
            this.rbNuevoCliente.UseVisualStyleBackColor = true;
            // 
            // rbConsumidorFinal
            // 
            this.rbConsumidorFinal.AutoSize = true;
            this.rbConsumidorFinal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbConsumidorFinal.Location = new System.Drawing.Point(312, 253);
            this.rbConsumidorFinal.Name = "rbConsumidorFinal";
            this.rbConsumidorFinal.Size = new System.Drawing.Size(154, 25);
            this.rbConsumidorFinal.TabIndex = 7;
            this.rbConsumidorFinal.TabStop = true;
            this.rbConsumidorFinal.Text = "Consumidor Final";
            this.rbConsumidorFinal.UseVisualStyleBackColor = true;
            // 
            // txtBuscarCliente
            // 
            this.txtBuscarCliente.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarCliente.Location = new System.Drawing.Point(122, 289);
            this.txtBuscarCliente.Name = "txtBuscarCliente";
            this.txtBuscarCliente.Size = new System.Drawing.Size(300, 29);
            this.txtBuscarCliente.TabIndex = 8;
            // 
            // lstClientes
            // 
            this.lstClientes.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstClientes.FormattingEnabled = true;
            this.lstClientes.ItemHeight = 21;
            this.lstClientes.Location = new System.Drawing.Point(121, 317);
            this.lstClientes.Name = "lstClientes";
            this.lstClientes.Size = new System.Drawing.Size(300, 88);
            this.lstClientes.TabIndex = 9;
            // 
            // btnNuevoCliente
            // 
            this.btnNuevoCliente.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevoCliente.Location = new System.Drawing.Point(426, 327);
            this.btnNuevoCliente.Name = "btnNuevoCliente";
            this.btnNuevoCliente.Size = new System.Drawing.Size(82, 32);
            this.btnNuevoCliente.TabIndex = 10;
            this.btnNuevoCliente.Text = "+ Nuevo";
            this.btnNuevoCliente.UseVisualStyleBackColor = true;
            // 
            // txtTipoDoc
            // 
            this.txtTipoDoc.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTipoDoc.Location = new System.Drawing.Point(100, 426);
            this.txtTipoDoc.Name = "txtTipoDoc";
            this.txtTipoDoc.Size = new System.Drawing.Size(125, 29);
            this.txtTipoDoc.TabIndex = 11;
            // 
            // txtNombre
            // 
            this.txtNombre.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombre.Location = new System.Drawing.Point(100, 473);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(330, 29);
            this.txtNombre.TabIndex = 12;
            // 
            // txtDireccion
            // 
            this.txtDireccion.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDireccion.Location = new System.Drawing.Point(100, 515);
            this.txtDireccion.Name = "txtDireccion";
            this.txtDireccion.Size = new System.Drawing.Size(331, 29);
            this.txtDireccion.TabIndex = 13;
            // 
            // txtNumDoc
            // 
            this.txtNumDoc.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumDoc.Location = new System.Drawing.Point(296, 426);
            this.txtNumDoc.Name = "txtNumDoc";
            this.txtNumDoc.Size = new System.Drawing.Size(133, 29);
            this.txtNumDoc.TabIndex = 14;
            // 
            // txtTelefono
            // 
            this.txtTelefono.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTelefono.Location = new System.Drawing.Point(100, 564);
            this.txtTelefono.Name = "txtTelefono";
            this.txtTelefono.Size = new System.Drawing.Size(125, 29);
            this.txtTelefono.TabIndex = 15;
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEmail.Location = new System.Drawing.Point(282, 564);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(149, 29);
            this.txtEmail.TabIndex = 16;
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddItem.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddItem.ForeColor = System.Drawing.Color.Black;
            this.btnAddItem.Location = new System.Drawing.Point(17, 25);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(88, 31);
            this.btnAddItem.TabIndex = 18;
            this.btnAddItem.Text = "+ Item";
            this.btnAddItem.UseVisualStyleBackColor = true;
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnDelItem
            // 
            this.btnDelItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelItem.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelItem.ForeColor = System.Drawing.Color.Black;
            this.btnDelItem.Location = new System.Drawing.Point(138, 25);
            this.btnDelItem.Name = "btnDelItem";
            this.btnDelItem.Size = new System.Drawing.Size(89, 31);
            this.btnDelItem.TabIndex = 19;
            this.btnDelItem.Text = "Eliminar";
            this.btnDelItem.UseVisualStyleBackColor = true;
            // 
            // cmbImpuesto
            // 
            this.cmbImpuesto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbImpuesto.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbImpuesto.ForeColor = System.Drawing.Color.Black;
            this.cmbImpuesto.FormattingEnabled = true;
            this.cmbImpuesto.Location = new System.Drawing.Point(40, 89);
            this.cmbImpuesto.Name = "cmbImpuesto";
            this.cmbImpuesto.Size = new System.Drawing.Size(137, 29);
            this.cmbImpuesto.TabIndex = 20;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.lblSubtotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotal.ForeColor = System.Drawing.Color.White;
            this.lblSubtotal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSubtotal.Location = new System.Drawing.Point(244, 54);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(51, 21);
            this.lblSubtotal.TabIndex = 21;
            this.lblSubtotal.Text = "label1";
            // 
            // lblIVA
            // 
            this.lblIVA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblIVA.AutoSize = true;
            this.lblIVA.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.lblIVA.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIVA.ForeColor = System.Drawing.Color.White;
            this.lblIVA.Location = new System.Drawing.Point(244, 89);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(54, 21);
            this.lblIVA.TabIndex = 22;
            this.lblIVA.Text = "label2";
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.lblTotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.White;
            this.lblTotal.Location = new System.Drawing.Point(244, 124);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(54, 21);
            this.lblTotal.TabIndex = 23;
            this.lblTotal.Text = "label3";
            // 
            // btnGenerarFactura
            // 
            this.btnGenerarFactura.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGenerarFactura.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerarFactura.ForeColor = System.Drawing.Color.Black;
            this.btnGenerarFactura.Location = new System.Drawing.Point(16, 150);
            this.btnGenerarFactura.Name = "btnGenerarFactura";
            this.btnGenerarFactura.Size = new System.Drawing.Size(89, 31);
            this.btnGenerarFactura.TabIndex = 24;
            this.btnGenerarFactura.Text = "Generar Factura";
            this.btnGenerarFactura.UseVisualStyleBackColor = true;
            // 
            // btnVistaPrevia
            // 
            this.btnVistaPrevia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVistaPrevia.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVistaPrevia.ForeColor = System.Drawing.Color.Black;
            this.btnVistaPrevia.Location = new System.Drawing.Point(138, 150);
            this.btnVistaPrevia.Name = "btnVistaPrevia";
            this.btnVistaPrevia.Size = new System.Drawing.Size(89, 31);
            this.btnVistaPrevia.TabIndex = 25;
            this.btnVistaPrevia.Text = "Vista Previa";
            this.btnVistaPrevia.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(269, 32);
            this.label1.TabIndex = 26;
            this.label1.Text = "Generacion de Facturas";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 21);
            this.label2.TabIndex = 27;
            this.label2.Text = "Origen de la factura";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(19, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(135, 21);
            this.label3.TabIndex = 28;
            this.label3.Text = "Orden de trabajo";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(9, 293);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 21);
            this.label4.TabIndex = 29;
            this.label4.Text = "Buscar Cliente:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 229);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(350, 21);
            this.label5.TabIndex = 30;
            this.label5.Text = "Datos del receptor (cliente  / consumidor final)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.label6.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(18, 429);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(80, 21);
            this.label6.TabIndex = 31;
            this.label6.Text = "Tipo Doc:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.label7.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(228, 429);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 21);
            this.label7.TabIndex = 32;
            this.label7.Text = "Nº Doc:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.label8.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(18, 476);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 21);
            this.label8.TabIndex = 33;
            this.label8.Text = "Nombre:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.label9.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(19, 517);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 21);
            this.label9.TabIndex = 34;
            this.label9.Text = "Direccion:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.label10.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(19, 567);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(78, 21);
            this.label10.TabIndex = 35;
            this.label10.Text = "Telefono:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.label11.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(224, 567);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 21);
            this.label11.TabIndex = 36;
            this.label11.Text = "Email:";
            // 
            // dgvItems
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvItems.ColumnHeadersHeight = 4;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItems.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvItems.Location = new System.Drawing.Point(536, 1);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.Size = new System.Drawing.Size(359, 329);
            this.dgvItems.TabIndex = 37;
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
            this.dgvItems.ThemeStyle.HeaderStyle.Height = 4;
            this.dgvItems.ThemeStyle.ReadOnly = false;
            this.dgvItems.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvItems.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvItems.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvItems.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvItems.ThemeStyle.RowsStyle.Height = 22;
            this.dgvItems.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvItems.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.guna2Panel1.ForeColor = System.Drawing.Color.White;
            this.guna2Panel1.Location = new System.Drawing.Point(5, 411);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Size = new System.Drawing.Size(459, 198);
            this.guna2Panel1.TabIndex = 38;
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Panel2.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.guna2Panel2.Controls.Add(this.lblSubtotal);
            this.guna2Panel2.Controls.Add(this.lblIVA);
            this.guna2Panel2.Controls.Add(this.lblTotal);
            this.guna2Panel2.Controls.Add(this.btnAddItem);
            this.guna2Panel2.Controls.Add(this.btnDelItem);
            this.guna2Panel2.Controls.Add(this.cmbImpuesto);
            this.guna2Panel2.Controls.Add(this.btnVistaPrevia);
            this.guna2Panel2.Controls.Add(this.btnGenerarFactura);
            this.guna2Panel2.ForeColor = System.Drawing.Color.White;
            this.guna2Panel2.Location = new System.Drawing.Point(514, 355);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(370, 199);
            this.guna2Panel2.TabIndex = 39;
            // 
            // btnBuscadorOrdenTrabajo
            // 
            this.btnBuscadorOrdenTrabajo.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorOrdenTrabajo.IconColor = System.Drawing.Color.Black;
            this.btnBuscadorOrdenTrabajo.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorOrdenTrabajo.IconSize = 30;
            this.btnBuscadorOrdenTrabajo.Location = new System.Drawing.Point(326, 110);
            this.btnBuscadorOrdenTrabajo.Name = "btnBuscadorOrdenTrabajo";
            this.btnBuscadorOrdenTrabajo.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadorOrdenTrabajo.TabIndex = 26;
            this.btnBuscadorOrdenTrabajo.UseVisualStyleBackColor = true;
            this.btnBuscadorOrdenTrabajo.Click += new System.EventHandler(this.btnBuscadorOrdenTrabajo_Click);
            // 
            // btnBuscadorClientes
            // 
            this.btnBuscadorClientes.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorClientes.IconColor = System.Drawing.Color.Black;
            this.btnBuscadorClientes.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorClientes.IconSize = 30;
            this.btnBuscadorClientes.Location = new System.Drawing.Point(443, 288);
            this.btnBuscadorClientes.Name = "btnBuscadorClientes";
            this.btnBuscadorClientes.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadorClientes.TabIndex = 40;
            this.btnBuscadorClientes.UseVisualStyleBackColor = true;
            this.btnBuscadorClientes.Click += new System.EventHandler(this.btnBuscadorClientes_Click);
            // 
            // FormGenFactu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(896, 621);
            this.Controls.Add(this.btnBuscadorClientes);
            this.Controls.Add(this.btnBuscadorOrdenTrabajo);
            this.Controls.Add(this.lstClientes);
            this.Controls.Add(this.txtNumDoc);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtTelefono);
            this.Controls.Add(this.txtDireccion);
            this.Controls.Add(this.txtNombre);
            this.Controls.Add(this.txtTipoDoc);
            this.Controls.Add(this.btnNuevoCliente);
            this.Controls.Add(this.txtBuscarCliente);
            this.Controls.Add(this.rbConsumidorFinal);
            this.Controls.Add(this.rbNuevoCliente);
            this.Controls.Add(this.rbClienteExistente);
            this.Controls.Add(this.btnCargarItemsOT);
            this.Controls.Add(this.lstOTResultados);
            this.Controls.Add(this.txtBuscarOT);
            this.Controls.Add(this.rbVentaDirecta);
            this.Controls.Add(this.rbDesdeOT);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.guna2Panel2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.guna2Panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormGenFactu";
            this.Text = "Generacion de Facturas";
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbDesdeOT;
        private System.Windows.Forms.RadioButton rbVentaDirecta;
        private System.Windows.Forms.TextBox txtBuscarOT;
        private System.Windows.Forms.ListBox lstOTResultados;
        private System.Windows.Forms.Button btnCargarItemsOT;
        private System.Windows.Forms.RadioButton rbClienteExistente;
        private System.Windows.Forms.RadioButton rbNuevoCliente;
        private System.Windows.Forms.RadioButton rbConsumidorFinal;
        private System.Windows.Forms.TextBox txtBuscarCliente;
        private System.Windows.Forms.ListBox lstClientes;
        private System.Windows.Forms.Button btnNuevoCliente;
        private System.Windows.Forms.TextBox txtTipoDoc;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtDireccion;
        private System.Windows.Forms.TextBox txtNumDoc;
        private System.Windows.Forms.TextBox txtTelefono;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Button btnAddItem;
        private System.Windows.Forms.Button btnDelItem;
        private System.Windows.Forms.ComboBox cmbImpuesto;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblIVA;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnGenerarFactura;
        private System.Windows.Forms.Button btnVistaPrevia;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private Guna.UI2.WinForms.Guna2DataGridView dgvItems;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private FontAwesome.Sharp.IconButton btnBuscadorOrdenTrabajo;
        private FontAwesome.Sharp.IconButton btnBuscadorClientes;
    }
}