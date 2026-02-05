namespace PROYECTOMECANICO.Modulo_Taller
{
    partial class FormOrdenTrabajo
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
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.cmbEstado = new System.Windows.Forms.ComboBox();
            this.btnCambiarEstado = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvOrdenes
            // 
            this.dgvOrdenes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrdenes.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvOrdenes.Location = new System.Drawing.Point(0, 0);
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.Size = new System.Drawing.Size(908, 300);
            this.dgvOrdenes.TabIndex = 0;
            // 
            // cmbEstado
            // 
            this.cmbEstado.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEstado.FormattingEnabled = true;
            this.cmbEstado.Location = new System.Drawing.Point(170, 405);
            this.cmbEstado.Name = "cmbEstado";
            this.cmbEstado.Size = new System.Drawing.Size(217, 33);
            this.cmbEstado.TabIndex = 1;
            // 
            // btnCambiarEstado
            // 
            this.btnCambiarEstado.BackColor = System.Drawing.Color.DarkSlateBlue;
            this.btnCambiarEstado.FlatAppearance.BorderSize = 0;
            this.btnCambiarEstado.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCambiarEstado.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCambiarEstado.ForeColor = System.Drawing.Color.White;
            this.btnCambiarEstado.Location = new System.Drawing.Point(469, 377);
            this.btnCambiarEstado.Name = "btnCambiarEstado";
            this.btnCambiarEstado.Size = new System.Drawing.Size(231, 86);
            this.btnCambiarEstado.TabIndex = 2;
            this.btnCambiarEstado.Text = "Cambiar Estado";
            this.btnCambiarEstado.UseVisualStyleBackColor = false;
            this.btnCambiarEstado.Click += new System.EventHandler(this.btnCambiarEstado_Click);
            // 
            // FormOrdenTrabajo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.btnCambiarEstado);
            this.Controls.Add(this.cmbEstado);
            this.Controls.Add(this.dgvOrdenes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormOrdenTrabajo";
            this.Text = "FormOrdenTrabajo";
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.ComboBox cmbEstado;
        private System.Windows.Forms.Button btnCambiarEstado;
    }
}