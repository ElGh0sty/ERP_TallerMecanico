namespace PROYECTOMECANICO.Modulo_Config
{
    partial class FormConfiguracion
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
            this.tabMetodosPago = new Guna.UI2.WinForms.Guna2TabControl();
            this.tabImpuestos = new System.Windows.Forms.TabPage();
            this.tabEmpresa = new System.Windows.Forms.TabPage();
            this.tabMetodosPago.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabMetodosPago
            // 
            this.tabMetodosPago.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabMetodosPago.Controls.Add(this.tabImpuestos);
            this.tabMetodosPago.Controls.Add(this.tabEmpresa);
            this.tabMetodosPago.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMetodosPago.ItemSize = new System.Drawing.Size(180, 40);
            this.tabMetodosPago.Location = new System.Drawing.Point(0, 0);
            this.tabMetodosPago.Name = "tabMetodosPago";
            this.tabMetodosPago.SelectedIndex = 0;
            this.tabMetodosPago.Size = new System.Drawing.Size(908, 601);
            this.tabMetodosPago.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.tabMetodosPago.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.tabMetodosPago.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabMetodosPago.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.tabMetodosPago.TabButtonHoverState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.tabMetodosPago.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.tabMetodosPago.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabMetodosPago.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabMetodosPago.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.tabMetodosPago.TabButtonIdleState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.tabMetodosPago.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.tabMetodosPago.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(37)))), ((int)(((byte)(49)))));
            this.tabMetodosPago.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.tabMetodosPago.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.tabMetodosPago.TabButtonSelectedState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(132)))), ((int)(((byte)(255)))));
            this.tabMetodosPago.TabButtonSize = new System.Drawing.Size(180, 40);
            this.tabMetodosPago.TabIndex = 0;
            this.tabMetodosPago.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            // 
            // tabImpuestos
            // 
            this.tabImpuestos.Font = new System.Drawing.Font("Yu Gothic UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabImpuestos.Location = new System.Drawing.Point(184, 4);
            this.tabImpuestos.Name = "tabImpuestos";
            this.tabImpuestos.Padding = new System.Windows.Forms.Padding(10);
            this.tabImpuestos.Size = new System.Drawing.Size(720, 593);
            this.tabImpuestos.TabIndex = 0;
            this.tabImpuestos.Text = "Impuestos";
            this.tabImpuestos.UseVisualStyleBackColor = true;
            // 
            // tabEmpresa
            // 
            this.tabEmpresa.Location = new System.Drawing.Point(184, 4);
            this.tabEmpresa.Name = "tabEmpresa";
            this.tabEmpresa.Padding = new System.Windows.Forms.Padding(3);
            this.tabEmpresa.Size = new System.Drawing.Size(720, 593);
            this.tabEmpresa.TabIndex = 1;
            this.tabEmpresa.Text = "Metodos de Pago";
            this.tabEmpresa.UseVisualStyleBackColor = true;
            // 
            // FormConfiguracion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(908, 601);
            this.Controls.Add(this.tabMetodosPago);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormConfiguracion";
            this.Text = "Configuracion";
            this.tabMetodosPago.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TabControl tabMetodosPago;
        private System.Windows.Forms.TabPage tabImpuestos;
        private System.Windows.Forms.TabPage tabEmpresa;
    }
}