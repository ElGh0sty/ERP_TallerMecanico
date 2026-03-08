namespace PROYECTOMECANICO.Modulo_Taller
{
    partial class FormTrabajoProductos
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
            this.TabControl1 = new Guna.UI2.WinForms.Guna2TabControl();
            this.tabControl3 = new System.Windows.Forms.TabPage();
            this.dgvServicios = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lstServicios = new System.Windows.Forms.ListBox();
            this.btnBuscadorOrden = new FontAwesome.Sharp.IconButton();
            this.btnBuscadorServicios = new FontAwesome.Sharp.IconButton();
            this.label2 = new System.Windows.Forms.Label();
            this.lblOrdenInfo = new System.Windows.Forms.Label();
            this.btnAgregarServicio = new System.Windows.Forms.Button();
            this.btnCargarOrden = new System.Windows.Forms.Button();
            this.txtBuscarServicio = new System.Windows.Forms.TextBox();
            this.cmbOrdenes = new System.Windows.Forms.ComboBox();
            this.tabControl4 = new System.Windows.Forms.TabPage();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnBuscadoProductos = new FontAwesome.Sharp.IconButton();
            this.label1 = new System.Windows.Forms.Label();
            this.lstProductos = new System.Windows.Forms.ListBox();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.txtBuscarProducto = new System.Windows.Forms.TextBox();
            this.lblStock = new System.Windows.Forms.Label();
            this.btnAgregarProducto = new System.Windows.Forms.Button();
            this.TabControl1.SuspendLayout();
            this.tabControl3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabControl4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabControl3);
            this.TabControl1.Controls.Add(this.tabControl4);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.ItemSize = new System.Drawing.Size(180, 40);
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(908, 603);
            this.TabControl1.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.TabControl1.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.TabControl1.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.TabControl1.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.TabControl1.TabButtonHoverState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.TabControl1.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.TabControl1.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.TabControl1.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.TabControl1.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.TabControl1.TabButtonIdleState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.TabControl1.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.TabControl1.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(98)))), ((int)(((byte)(102)))), ((int)(((byte)(244)))));
            this.TabControl1.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.TabControl1.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.TabControl1.TabButtonSelectedState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(132)))), ((int)(((byte)(255)))));
            this.TabControl1.TabButtonSize = new System.Drawing.Size(180, 40);
            this.TabControl1.TabIndex = 2;
            this.TabControl1.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.TabControl1.TabMenuOrientation = Guna.UI2.WinForms.TabMenuOrientation.HorizontalTop;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.dgvServicios);
            this.tabControl3.Controls.Add(this.panel1);
            this.tabControl3.Location = new System.Drawing.Point(4, 44);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.Padding = new System.Windows.Forms.Padding(3);
            this.tabControl3.Size = new System.Drawing.Size(900, 555);
            this.tabControl3.TabIndex = 2;
            this.tabControl3.Text = "Añadir Servicios";
            this.tabControl3.UseVisualStyleBackColor = true;
            // 
            // dgvServicios
            // 
            this.dgvServicios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServicios.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvServicios.Location = new System.Drawing.Point(3, 3);
            this.dgvServicios.Name = "dgvServicios";
            this.dgvServicios.Size = new System.Drawing.Size(894, 266);
            this.dgvServicios.TabIndex = 2;
            this.dgvServicios.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvServicios_CellClick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panel1.BackColor = System.Drawing.Color.LightSlateGray;
            this.panel1.Controls.Add(this.lstServicios);
            this.panel1.Controls.Add(this.btnBuscadorOrden);
            this.panel1.Controls.Add(this.btnBuscadorServicios);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblOrdenInfo);
            this.panel1.Controls.Add(this.btnAgregarServicio);
            this.panel1.Controls.Add(this.btnCargarOrden);
            this.panel1.Controls.Add(this.txtBuscarServicio);
            this.panel1.Controls.Add(this.cmbOrdenes);
            this.panel1.Location = new System.Drawing.Point(96, 288);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(706, 259);
            this.panel1.TabIndex = 6;
            // 
            // lstServicios
            // 
            this.lstServicios.FormattingEnabled = true;
            this.lstServicios.Location = new System.Drawing.Point(392, 121);
            this.lstServicios.Name = "lstServicios";
            this.lstServicios.Size = new System.Drawing.Size(253, 95);
            this.lstServicios.TabIndex = 10;
            this.lstServicios.Visible = false;
            this.lstServicios.SelectedIndexChanged += new System.EventHandler(this.lstProductos_SelectedIndexChanged);
            // 
            // btnBuscadorOrden
            // 
            this.btnBuscadorOrden.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorOrden.IconColor = System.Drawing.Color.Black;
            this.btnBuscadorOrden.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorOrden.IconSize = 30;
            this.btnBuscadorOrden.Location = new System.Drawing.Point(349, 96);
            this.btnBuscadorOrden.Name = "btnBuscadorOrden";
            this.btnBuscadorOrden.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadorOrden.TabIndex = 9;
            this.btnBuscadorOrden.UseVisualStyleBackColor = true;
            this.btnBuscadorOrden.Click += new System.EventHandler(this.btnBuscadorOrden_Click);
            // 
            // btnBuscadorServicios
            // 
            this.btnBuscadorServicios.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorServicios.IconColor = System.Drawing.Color.Black;
            this.btnBuscadorServicios.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorServicios.IconSize = 30;
            this.btnBuscadorServicios.Location = new System.Drawing.Point(649, 96);
            this.btnBuscadorServicios.Name = "btnBuscadorServicios";
            this.btnBuscadorServicios.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadorServicios.TabIndex = 7;
            this.btnBuscadorServicios.UseVisualStyleBackColor = true;
            this.btnBuscadorServicios.Click += new System.EventHandler(this.btnBuscadorServicios_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.LightSlateGray;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(436, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(195, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Agrega Servicios a la Lista:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOrdenInfo
            // 
            this.lblOrdenInfo.AutoSize = true;
            this.lblOrdenInfo.BackColor = System.Drawing.Color.LightSlateGray;
            this.lblOrdenInfo.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOrdenInfo.ForeColor = System.Drawing.Color.White;
            this.lblOrdenInfo.Location = new System.Drawing.Point(119, 52);
            this.lblOrdenInfo.Name = "lblOrdenInfo";
            this.lblOrdenInfo.Size = new System.Drawing.Size(159, 20);
            this.lblOrdenInfo.TabIndex = 5;
            this.lblOrdenInfo.Text = "Orden Seleccionada: -";
            this.lblOrdenInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAgregarServicio
            // 
            this.btnAgregarServicio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarServicio.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarServicio.Location = new System.Drawing.Point(443, 172);
            this.btnAgregarServicio.Name = "btnAgregarServicio";
            this.btnAgregarServicio.Size = new System.Drawing.Size(173, 70);
            this.btnAgregarServicio.TabIndex = 1;
            this.btnAgregarServicio.Text = "Agregar Servicio";
            this.btnAgregarServicio.UseVisualStyleBackColor = true;
            this.btnAgregarServicio.Click += new System.EventHandler(this.btnAgregarServicio_Click);
            // 
            // btnCargarOrden
            // 
            this.btnCargarOrden.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarOrden.Location = new System.Drawing.Point(123, 172);
            this.btnCargarOrden.Name = "btnCargarOrden";
            this.btnCargarOrden.Size = new System.Drawing.Size(173, 70);
            this.btnCargarOrden.TabIndex = 4;
            this.btnCargarOrden.Text = "Cargar Orden";
            this.btnCargarOrden.UseVisualStyleBackColor = true;
            this.btnCargarOrden.Click += new System.EventHandler(this.btnCargarOrden_Click);
            // 
            // txtBuscarServicio
            // 
            this.txtBuscarServicio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBuscarServicio.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarServicio.Location = new System.Drawing.Point(392, 96);
            this.txtBuscarServicio.Name = "txtBuscarServicio";
            this.txtBuscarServicio.Size = new System.Drawing.Size(253, 33);
            this.txtBuscarServicio.TabIndex = 0;
            // 
            // cmbOrdenes
            // 
            this.cmbOrdenes.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbOrdenes.FormattingEnabled = true;
            this.cmbOrdenes.Location = new System.Drawing.Point(35, 96);
            this.cmbOrdenes.MaxDropDownItems = 20;
            this.cmbOrdenes.Name = "cmbOrdenes";
            this.cmbOrdenes.Size = new System.Drawing.Size(310, 33);
            this.cmbOrdenes.TabIndex = 3;
            // 
            // tabControl4
            // 
            this.tabControl4.Controls.Add(this.dgvItems);
            this.tabControl4.Controls.Add(this.panel2);
            this.tabControl4.Location = new System.Drawing.Point(4, 44);
            this.tabControl4.Name = "tabControl4";
            this.tabControl4.Padding = new System.Windows.Forms.Padding(3);
            this.tabControl4.Size = new System.Drawing.Size(900, 555);
            this.tabControl4.TabIndex = 3;
            this.tabControl4.Text = "Añadir Productos";
            this.tabControl4.UseVisualStyleBackColor = true;
            // 
            // dgvItems
            // 
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvItems.Location = new System.Drawing.Point(3, 3);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.Size = new System.Drawing.Size(894, 290);
            this.dgvItems.TabIndex = 4;
            this.dgvItems.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellClick);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panel2.BackColor = System.Drawing.Color.LightSlateGray;
            this.panel2.Controls.Add(this.btnBuscadoProductos);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lstProductos);
            this.panel2.Controls.Add(this.nudCantidad);
            this.panel2.Controls.Add(this.txtBuscarProducto);
            this.panel2.Controls.Add(this.lblStock);
            this.panel2.Controls.Add(this.btnAgregarProducto);
            this.panel2.Location = new System.Drawing.Point(126, 310);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(631, 232);
            this.panel2.TabIndex = 8;
            // 
            // btnBuscadoProductos
            // 
            this.btnBuscadoProductos.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadoProductos.IconColor = System.Drawing.Color.Black;
            this.btnBuscadoProductos.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadoProductos.IconSize = 30;
            this.btnBuscadoProductos.Location = new System.Drawing.Point(398, 65);
            this.btnBuscadoProductos.Name = "btnBuscadoProductos";
            this.btnBuscadoProductos.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadoProductos.TabIndex = 10;
            this.btnBuscadoProductos.UseVisualStyleBackColor = true;
            this.btnBuscadoProductos.Click += new System.EventHandler(this.btnBuscadorProductos_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(436, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Elije la cantidad:";
            // 
            // lstProductos
            // 
            this.lstProductos.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstProductos.FormattingEnabled = true;
            this.lstProductos.ItemHeight = 21;
            this.lstProductos.Location = new System.Drawing.Point(50, 104);
            this.lstProductos.Name = "lstProductos";
            this.lstProductos.Size = new System.Drawing.Size(347, 109);
            this.lstProductos.TabIndex = 6;
            this.lstProductos.SelectedIndexChanged += new System.EventHandler(this.lstProductos_SelectedIndexChanged);
            // 
            // nudCantidad
            // 
            this.nudCantidad.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCantidad.Location = new System.Drawing.Point(437, 66);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(120, 33);
            this.nudCantidad.TabIndex = 1;
            // 
            // txtBuscarProducto
            // 
            this.txtBuscarProducto.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarProducto.Location = new System.Drawing.Point(50, 65);
            this.txtBuscarProducto.Name = "txtBuscarProducto";
            this.txtBuscarProducto.Size = new System.Drawing.Size(347, 33);
            this.txtBuscarProducto.TabIndex = 5;
            // 
            // lblStock
            // 
            this.lblStock.AutoSize = true;
            this.lblStock.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStock.ForeColor = System.Drawing.Color.White;
            this.lblStock.Location = new System.Drawing.Point(182, 16);
            this.lblStock.Name = "lblStock";
            this.lblStock.Size = new System.Drawing.Size(63, 25);
            this.lblStock.TabIndex = 2;
            this.lblStock.Text = "label1";
            // 
            // btnAgregarProducto
            // 
            this.btnAgregarProducto.Font = new System.Drawing.Font("Yu Gothic UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarProducto.Location = new System.Drawing.Point(413, 126);
            this.btnAgregarProducto.Name = "btnAgregarProducto";
            this.btnAgregarProducto.Size = new System.Drawing.Size(180, 54);
            this.btnAgregarProducto.TabIndex = 3;
            this.btnAgregarProducto.Text = "Agregar Producto";
            this.btnAgregarProducto.UseVisualStyleBackColor = true;
            this.btnAgregarProducto.Click += new System.EventHandler(this.btnAgregarProducto_Click);
            // 
            // FormTrabajoProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.TabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormTrabajoProductos";
            this.Text = "Asignar Trabajo y Productos";
            this.TabControl1.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvServicios)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControl4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TabControl TabControl1;
        private System.Windows.Forms.TabPage tabControl3;
        private System.Windows.Forms.DataGridView dgvServicios;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblOrdenInfo;
        private System.Windows.Forms.Button btnAgregarServicio;
        private System.Windows.Forms.Button btnCargarOrden;
        private System.Windows.Forms.TextBox txtBuscarServicio;
        private System.Windows.Forms.ComboBox cmbOrdenes;
        private System.Windows.Forms.TabPage tabControl4;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstProductos;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.TextBox txtBuscarProducto;
        private System.Windows.Forms.Label lblStock;
        private System.Windows.Forms.Button btnAgregarProducto;
        private FontAwesome.Sharp.IconButton btnBuscadorServicios;
        private FontAwesome.Sharp.IconButton btnBuscadorOrden;
        private FontAwesome.Sharp.IconButton btnBuscadoProductos;
        private System.Windows.Forms.ListBox lstServicios;
    }
}