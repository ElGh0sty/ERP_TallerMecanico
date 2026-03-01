namespace PROYECTOMECANICO.Modulo_Clientes
{
    partial class FormNuevoCliente
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
            this.dgvNuevo = new System.Windows.Forms.DataGridView();
            this.btnNuevoCliente = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNuevo)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvNuevo
            // 
            this.dgvNuevo.AllowUserToAddRows = false;
            this.dgvNuevo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvNuevo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNuevo.Location = new System.Drawing.Point(12, 12);
            this.dgvNuevo.Name = "dgvNuevo";
            this.dgvNuevo.Size = new System.Drawing.Size(884, 387);
            this.dgvNuevo.TabIndex = 0;
            this.dgvNuevo.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNuevo_CellContentClick);
            // 
            // btnNuevoCliente
            // 
            this.btnNuevoCliente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNuevoCliente.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnNuevoCliente.FlatAppearance.BorderSize = 0;
            this.btnNuevoCliente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNuevoCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNuevoCliente.ForeColor = System.Drawing.Color.White;
            this.btnNuevoCliente.Location = new System.Drawing.Point(721, 417);
            this.btnNuevoCliente.Name = "btnNuevoCliente";
            this.btnNuevoCliente.Size = new System.Drawing.Size(175, 74);
            this.btnNuevoCliente.TabIndex = 2;
            this.btnNuevoCliente.Text = "Agregar Cliente";
            this.btnNuevoCliente.UseVisualStyleBackColor = false;
            // 
            // FormNuevoCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.btnNuevoCliente);
            this.Controls.Add(this.dgvNuevo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormNuevoCliente";
            this.Text = "Registrar Nuevo Cliente";
            ((System.ComponentModel.ISupportInitialize)(this.dgvNuevo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvNuevo;
        public System.Windows.Forms.Button btnNuevoCliente;
    }
}