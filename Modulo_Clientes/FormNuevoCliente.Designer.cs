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
            this.btnGuardar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNuevo)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvNuevo
            // 
            this.dgvNuevo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNuevo.Location = new System.Drawing.Point(12, 12);
            this.dgvNuevo.Name = "dgvNuevo";
            this.dgvNuevo.Size = new System.Drawing.Size(884, 387);
            this.dgvNuevo.TabIndex = 0;
            this.dgvNuevo.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvNuevo_CellContentClick);
            this.dgvNuevo.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvNuevo_CellPainting);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(721, 420);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(175, 74);
            this.btnGuardar.TabIndex = 1;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // FormNuevoCliente
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.dgvNuevo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormNuevoCliente";
            this.Text = "FormNuevoCliente";
            ((System.ComponentModel.ISupportInitialize)(this.dgvNuevo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView dgvNuevo;
        public System.Windows.Forms.Button btnGuardar;
    }
}