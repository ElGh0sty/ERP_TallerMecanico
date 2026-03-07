namespace PROYECTOMECANICO.Modulo_Inventario
{
    partial class FormCompras
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
            this.cmbProveedor = new System.Windows.Forms.ComboBox();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtBuscarProducto = new System.Windows.Forms.TextBox();
            this.lstProductos = new System.Windows.Forms.ListBox();
            this.lblStockSel = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.nudCosto = new System.Windows.Forms.NumericUpDown();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.btnAgregarItem = new System.Windows.Forms.Button();
            this.btnGuardarCompra = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnNuevoProducto = new System.Windows.Forms.Button();
            this.btnBuscadorProducto = new FontAwesome.Sharp.IconButton();
            this.cmbImpuestoCompra = new System.Windows.Forms.ComboBox();
            this.btnVistaPrevia = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbProveedor
            // 
            this.cmbProveedor.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbProveedor.FormattingEnabled = true;
            this.cmbProveedor.Location = new System.Drawing.Point(10, 46);
            this.cmbProveedor.Name = "cmbProveedor";
            this.cmbProveedor.Size = new System.Drawing.Size(256, 29);
            this.cmbProveedor.TabIndex = 0;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotal.Location = new System.Drawing.Point(41, 531);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(51, 21);
            this.lblSubtotal.TabIndex = 2;
            this.lblSubtotal.Text = "label1";
            // 
            // lblIVA
            // 
            this.lblIVA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblIVA.AutoSize = true;
            this.lblIVA.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIVA.Location = new System.Drawing.Point(187, 531);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(54, 21);
            this.lblIVA.TabIndex = 3;
            this.lblIVA.Text = "label2";
            // 
            // lblTotal
            // 
            this.lblTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(340, 531);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(54, 21);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "label3";
            // 
            // txtBuscarProducto
            // 
            this.txtBuscarProducto.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBuscarProducto.Location = new System.Drawing.Point(10, 84);
            this.txtBuscarProducto.Name = "txtBuscarProducto";
            this.txtBuscarProducto.Size = new System.Drawing.Size(256, 29);
            this.txtBuscarProducto.TabIndex = 5;
            // 
            // lstProductos
            // 
            this.lstProductos.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstProductos.FormattingEnabled = true;
            this.lstProductos.ItemHeight = 21;
            this.lstProductos.Location = new System.Drawing.Point(10, 111);
            this.lstProductos.Name = "lstProductos";
            this.lstProductos.Size = new System.Drawing.Size(256, 109);
            this.lstProductos.TabIndex = 6;
            // 
            // lblStockSel
            // 
            this.lblStockSel.AutoSize = true;
            this.lblStockSel.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStockSel.Location = new System.Drawing.Point(272, 91);
            this.lblStockSel.Name = "lblStockSel";
            this.lblStockSel.Size = new System.Drawing.Size(59, 21);
            this.lblStockSel.TabIndex = 7;
            this.lblStockSel.Text = "Stock: ";
            // 
            // nudCantidad
            // 
            this.nudCantidad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.nudCantidad.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCantidad.Location = new System.Drawing.Point(355, 86);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(74, 29);
            this.nudCantidad.TabIndex = 8;
            // 
            // nudCosto
            // 
            this.nudCosto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.nudCosto.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudCosto.Location = new System.Drawing.Point(456, 86);
            this.nudCosto.Name = "nudCosto";
            this.nudCosto.Size = new System.Drawing.Size(74, 29);
            this.nudCosto.TabIndex = 9;
            // 
            // dgvItems
            // 
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Location = new System.Drawing.Point(-2, 175);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.Size = new System.Drawing.Size(913, 302);
            this.dgvItems.TabIndex = 10;
            // 
            // btnAgregarItem
            // 
            this.btnAgregarItem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgregarItem.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarItem.Location = new System.Drawing.Point(651, 67);
            this.btnAgregarItem.Name = "btnAgregarItem";
            this.btnAgregarItem.Size = new System.Drawing.Size(104, 61);
            this.btnAgregarItem.TabIndex = 11;
            this.btnAgregarItem.Text = "Agregar Item";
            this.btnAgregarItem.UseVisualStyleBackColor = true;
            // 
            // btnGuardarCompra
            // 
            this.btnGuardarCompra.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGuardarCompra.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarCompra.Location = new System.Drawing.Point(615, 511);
            this.btnGuardarCompra.Name = "btnGuardarCompra";
            this.btnGuardarCompra.Size = new System.Drawing.Size(104, 61);
            this.btnGuardarCompra.TabIndex = 12;
            this.btnGuardarCompra.Text = "Guardar Compra";
            this.btnGuardarCompra.UseVisualStyleBackColor = true;
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLimpiar.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.Location = new System.Drawing.Point(752, 511);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(104, 61);
            this.btnLimpiar.TabIndex = 13;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(351, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 21);
            this.label2.TabIndex = 16;
            this.label2.Text = "Cantidad:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(451, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 21);
            this.label3.TabIndex = 17;
            this.label3.Text = "Costo:";
            // 
            // btnNuevoProducto
            // 
            this.btnNuevoProducto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNuevoProducto.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevoProducto.Location = new System.Drawing.Point(770, 67);
            this.btnNuevoProducto.Name = "btnNuevoProducto";
            this.btnNuevoProducto.Size = new System.Drawing.Size(104, 61);
            this.btnNuevoProducto.TabIndex = 18;
            this.btnNuevoProducto.Text = "Nuevo Producto";
            this.btnNuevoProducto.UseVisualStyleBackColor = true;
            // 
            // btnBuscadorProducto
            // 
            this.btnBuscadorProducto.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.btnBuscadorProducto.IconColor = System.Drawing.Color.Black;
            this.btnBuscadorProducto.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnBuscadorProducto.IconSize = 30;
            this.btnBuscadorProducto.Location = new System.Drawing.Point(270, 42);
            this.btnBuscadorProducto.Name = "btnBuscadorProducto";
            this.btnBuscadorProducto.Size = new System.Drawing.Size(33, 33);
            this.btnBuscadorProducto.TabIndex = 27;
            this.btnBuscadorProducto.UseVisualStyleBackColor = true;
            this.btnBuscadorProducto.Click += new System.EventHandler(this.btnBuscadorProductos_Click);
            // 
            // cmbImpuestoCompra
            // 
            this.cmbImpuestoCompra.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbImpuestoCompra.FormattingEnabled = true;
            this.cmbImpuestoCompra.Location = new System.Drawing.Point(555, 85);
            this.cmbImpuestoCompra.Name = "cmbImpuestoCompra";
            this.cmbImpuestoCompra.Size = new System.Drawing.Size(78, 29);
            this.cmbImpuestoCompra.TabIndex = 29;
            // 
            // btnVistaPrevia
            // 
            this.btnVistaPrevia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVistaPrevia.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVistaPrevia.Location = new System.Drawing.Point(483, 511);
            this.btnVistaPrevia.Name = "btnVistaPrevia";
            this.btnVistaPrevia.Size = new System.Drawing.Size(104, 61);
            this.btnVistaPrevia.TabIndex = 30;
            this.btnVistaPrevia.Text = "Vista Previa";
            this.btnVistaPrevia.UseVisualStyleBackColor = true;
            this.btnVistaPrevia.Click += new System.EventHandler(this.btnVistaPreviaCompra_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(551, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 21);
            this.label4.TabIndex = 31;
            this.label4.Text = "IVA:";
            // 
            // FormCompras
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnVistaPrevia);
            this.Controls.Add(this.cmbImpuestoCompra);
            this.Controls.Add(this.btnBuscadorProducto);
            this.Controls.Add(this.btnNuevoProducto);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnGuardarCompra);
            this.Controls.Add(this.btnAgregarItem);
            this.Controls.Add(this.nudCosto);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.lblStockSel);
            this.Controls.Add(this.lstProductos);
            this.Controls.Add(this.txtBuscarProducto);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblIVA);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.cmbProveedor);
            this.Controls.Add(this.dgvItems);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormCompras";
            this.Text = "Registro de Compras";
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCosto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbProveedor;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblIVA;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.TextBox txtBuscarProducto;
        private System.Windows.Forms.ListBox lstProductos;
        private System.Windows.Forms.Label lblStockSel;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.NumericUpDown nudCosto;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.Button btnAgregarItem;
        private System.Windows.Forms.Button btnGuardarCompra;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnNuevoProducto;
        private FontAwesome.Sharp.IconButton btnBuscadorProducto;
        private System.Windows.Forms.ComboBox cmbImpuestoCompra;
        private System.Windows.Forms.Button btnVistaPrevia;
        private System.Windows.Forms.Label label4;
    }
}