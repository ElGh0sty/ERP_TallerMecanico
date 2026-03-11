namespace PROYECTOMECANICO.Modulo_Taller
{
    partial class FormNovedades
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
            this.cmbOrdenes = new System.Windows.Forms.ComboBox();
            this.lblOrden = new System.Windows.Forms.Label();
            this.dgvNovedades = new System.Windows.Forms.DataGridView();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.chkRequiereExtra = new System.Windows.Forms.CheckBox();
            this.nudCantidadExtra = new System.Windows.Forms.NumericUpDown();
            this.btnNuevaNovedad = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnAceptar = new System.Windows.Forms.Button();
            this.btnRechazar = new System.Windows.Forms.Button();
            this.lblPendientes = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBuscarProducto = new System.Windows.Forms.TextBox();
            this.lblRepuesto = new System.Windows.Forms.Label();
            this.lstProductos = new System.Windows.Forms.ListBox();
            this.lblProductoSel = new System.Windows.Forms.Label();
            this.nudMontoExtra = new System.Windows.Forms.NumericUpDown();
            this.btnBuscadorOrden = new FontAwesome.Sharp.IconButton();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNovedades)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadExtra)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMontoExtra)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbOrdenes
            // 
            this.cmbOrdenes.FormattingEnabled = true;
            this.cmbOrdenes.Location = new System.Drawing.Point(113, 26);
            this.cmbOrdenes.Name = "cmbOrdenes";
            this.cmbOrdenes.Size = new System.Drawing.Size(192, 21);
            this.cmbOrdenes.TabIndex = 0;
            // 
            // lblOrden
            // 
            this.lblOrden.AutoSize = true;
            this.lblOrden.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.lblOrden.ForeColor = System.Drawing.Color.White;
            this.lblOrden.Location = new System.Drawing.Point(43, 26);
            this.lblOrden.Name = "lblOrden";
            this.lblOrden.Size = new System.Drawing.Size(39, 13);
            this.lblOrden.TabIndex = 1;
            this.lblOrden.Text = "Orden:";
            // 
            // dgvNovedades
            // 
            this.dgvNovedades.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvNovedades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNovedades.Location = new System.Drawing.Point(-1, 71);
            this.dgvNovedades.Name = "dgvNovedades";
            this.dgvNovedades.Size = new System.Drawing.Size(911, 265);
            this.dgvNovedades.TabIndex = 2;
            this.dgvNovedades.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNovedades_CellContentClick);
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(46, 414);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(354, 38);
            this.txtDescripcion.TabIndex = 3;
            // 
            // chkRequiereExtra
            // 
            this.chkRequiereExtra.AutoSize = true;
            this.chkRequiereExtra.Location = new System.Drawing.Point(46, 482);
            this.chkRequiereExtra.Name = "chkRequiereExtra";
            this.chkRequiereExtra.Size = new System.Drawing.Size(156, 17);
            this.chkRequiereExtra.TabIndex = 4;
            this.chkRequiereExtra.Text = "Requiere presupuesto extra";
            this.chkRequiereExtra.UseVisualStyleBackColor = true;
            // 
            // nudCantidadExtra
            // 
            this.nudCantidadExtra.DecimalPlaces = 2;
            this.nudCantidadExtra.Location = new System.Drawing.Point(704, 395);
            this.nudCantidadExtra.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudCantidadExtra.Name = "nudCantidadExtra";
            this.nudCantidadExtra.Size = new System.Drawing.Size(144, 20);
            this.nudCantidadExtra.TabIndex = 5;
            // 
            // btnNuevaNovedad
            // 
            this.btnNuevaNovedad.Location = new System.Drawing.Point(46, 342);
            this.btnNuevaNovedad.Name = "btnNuevaNovedad";
            this.btnNuevaNovedad.Size = new System.Drawing.Size(130, 55);
            this.btnNuevaNovedad.TabIndex = 6;
            this.btnNuevaNovedad.Text = "Nueva novedad";
            this.btnNuevaNovedad.UseVisualStyleBackColor = true;
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(528, 526);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(130, 35);
            this.btnGuardar.TabIndex = 7;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(718, 526);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(130, 35);
            this.btnCancelar.TabIndex = 8;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            // 
            // btnAceptar
            // 
            this.btnAceptar.Location = new System.Drawing.Point(46, 556);
            this.btnAceptar.Name = "btnAceptar";
            this.btnAceptar.Size = new System.Drawing.Size(130, 35);
            this.btnAceptar.TabIndex = 9;
            this.btnAceptar.Text = "Aceptar";
            this.btnAceptar.UseVisualStyleBackColor = true;
            // 
            // btnRechazar
            // 
            this.btnRechazar.Location = new System.Drawing.Point(237, 556);
            this.btnRechazar.Name = "btnRechazar";
            this.btnRechazar.Size = new System.Drawing.Size(130, 35);
            this.btnRechazar.TabIndex = 10;
            this.btnRechazar.Text = "Rechazar";
            this.btnRechazar.UseVisualStyleBackColor = true;
            // 
            // lblPendientes
            // 
            this.lblPendientes.AutoSize = true;
            this.lblPendientes.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.lblPendientes.ForeColor = System.Drawing.Color.White;
            this.lblPendientes.Location = new System.Drawing.Point(753, 34);
            this.lblPendientes.Name = "lblPendientes";
            this.lblPendientes.Size = new System.Drawing.Size(78, 13);
            this.lblPendientes.TabIndex = 11;
            this.lblPendientes.Text = "🔔 Pendientes:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(43, 526);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 17);
            this.label1.TabIndex = 12;
            this.label1.Text = "Decision del Cliente:";
            // 
            // txtBuscarProducto
            // 
            this.txtBuscarProducto.Location = new System.Drawing.Point(471, 394);
            this.txtBuscarProducto.Name = "txtBuscarProducto";
            this.txtBuscarProducto.Size = new System.Drawing.Size(202, 20);
            this.txtBuscarProducto.TabIndex = 13;
            // 
            // lblRepuesto
            // 
            this.lblRepuesto.AutoSize = true;
            this.lblRepuesto.Location = new System.Drawing.Point(525, 363);
            this.lblRepuesto.Name = "lblRepuesto";
            this.lblRepuesto.Size = new System.Drawing.Size(104, 13);
            this.lblRepuesto.TabIndex = 14;
            this.lblRepuesto.Text = "Repuesto/Producto:";
            // 
            // lstProductos
            // 
            this.lstProductos.FormattingEnabled = true;
            this.lstProductos.Location = new System.Drawing.Point(471, 414);
            this.lstProductos.Name = "lstProductos";
            this.lstProductos.Size = new System.Drawing.Size(202, 95);
            this.lstProductos.TabIndex = 15;
            // 
            // lblProductoSel
            // 
            this.lblProductoSel.AutoSize = true;
            this.lblProductoSel.Location = new System.Drawing.Point(715, 363);
            this.lblProductoSel.Name = "lblProductoSel";
            this.lblProductoSel.Size = new System.Drawing.Size(118, 13);
            this.lblProductoSel.TabIndex = 16;
            this.lblProductoSel.Text = "Seleccionado: Ninguno";
            // 
            // nudMontoExtra
            // 
            this.nudMontoExtra.DecimalPlaces = 2;
            this.nudMontoExtra.Location = new System.Drawing.Point(704, 432);
            this.nudMontoExtra.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.nudMontoExtra.Name = "nudMontoExtra";
            this.nudMontoExtra.Size = new System.Drawing.Size(85, 20);
            this.nudMontoExtra.TabIndex = 17;
            // 
            // btnBuscadorOrden
            // 
            this.btnBuscadorOrden.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorOrden.IconColor = System.Drawing.Color.Black;
            this.btnBuscadorOrden.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorOrden.IconSize = 30;
            this.btnBuscadorOrden.Location = new System.Drawing.Point(325, 24);
            this.btnBuscadorOrden.Name = "btnBuscadorOrden";
            this.btnBuscadorOrden.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadorOrden.TabIndex = 18;
            this.btnBuscadorOrden.UseVisualStyleBackColor = true;
            this.btnBuscadorOrden.Click += new System.EventHandler(this.btnBuscadorOrden_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.panel1.Location = new System.Drawing.Point(-1, -5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(911, 79);
            this.panel1.TabIndex = 19;
            // 
            // FormNovedades
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.btnBuscadorOrden);
            this.Controls.Add(this.nudMontoExtra);
            this.Controls.Add(this.lblProductoSel);
            this.Controls.Add(this.lstProductos);
            this.Controls.Add(this.lblRepuesto);
            this.Controls.Add(this.txtBuscarProducto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPendientes);
            this.Controls.Add(this.btnRechazar);
            this.Controls.Add(this.btnAceptar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnNuevaNovedad);
            this.Controls.Add(this.nudCantidadExtra);
            this.Controls.Add(this.chkRequiereExtra);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.dgvNovedades);
            this.Controls.Add(this.lblOrden);
            this.Controls.Add(this.cmbOrdenes);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormNovedades";
            this.Text = "Novedades";
            ((System.ComponentModel.ISupportInitialize)(this.dgvNovedades)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidadExtra)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMontoExtra)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbOrdenes;
        private System.Windows.Forms.Label lblOrden;
        private System.Windows.Forms.DataGridView dgvNovedades;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.CheckBox chkRequiereExtra;
        private System.Windows.Forms.NumericUpDown nudCantidadExtra;
        private System.Windows.Forms.Button btnNuevaNovedad;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnAceptar;
        private System.Windows.Forms.Button btnRechazar;
        private System.Windows.Forms.Label lblPendientes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBuscarProducto;
        private System.Windows.Forms.Label lblRepuesto;
        private System.Windows.Forms.ListBox lstProductos;
        private System.Windows.Forms.Label lblProductoSel;
        private System.Windows.Forms.NumericUpDown nudMontoExtra;
        private FontAwesome.Sharp.IconButton btnBuscadorOrden;
        private System.Windows.Forms.Panel panel1;
    }
}