namespace PROYECTOMECANICO
{
    partial class FormPdfViewer
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
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnCerrar = new Guna.UI2.WinForms.Guna2Button();
            this.btnGuardarComo = new Guna.UI2.WinForms.Guna2Button();
            this.webViewPdf = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webViewPdf)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.Controls.Add(this.lblTitulo);
            this.guna2Panel1.Controls.Add(this.btnCerrar);
            this.guna2Panel1.Controls.Add(this.btnGuardarComo);
            this.guna2Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.guna2Panel1.Location = new System.Drawing.Point(0, 0);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.Padding = new System.Windows.Forms.Padding(5);
            this.guna2Panel1.Size = new System.Drawing.Size(800, 50);
            this.guna2Panel1.TabIndex = 0;
            // 
            // lblTitulo
            // 
            this.lblTitulo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(317, 13);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(152, 25);
            this.lblTitulo.TabIndex = 2;
            this.lblTitulo.Text = "Vista previa PDF";
            // 
            // btnCerrar
            // 
            this.btnCerrar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCerrar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCerrar.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(615, 5);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(180, 40);
            this.btnCerrar.TabIndex = 1;
            this.btnCerrar.Text = "Cerrar";
            // 
            // btnGuardarComo
            // 
            this.btnGuardarComo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardarComo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardarComo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGuardarComo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGuardarComo.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnGuardarComo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGuardarComo.ForeColor = System.Drawing.Color.White;
            this.btnGuardarComo.Location = new System.Drawing.Point(5, 5);
            this.btnGuardarComo.Name = "btnGuardarComo";
            this.btnGuardarComo.Size = new System.Drawing.Size(180, 40);
            this.btnGuardarComo.TabIndex = 0;
            this.btnGuardarComo.Text = "Guardar como";
            // 
            // webViewPdf
            // 
            this.webViewPdf.AllowExternalDrop = true;
            this.webViewPdf.CreationProperties = null;
            this.webViewPdf.DefaultBackgroundColor = System.Drawing.Color.White;
            this.webViewPdf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webViewPdf.Location = new System.Drawing.Point(0, 50);
            this.webViewPdf.Name = "webViewPdf";
            this.webViewPdf.Size = new System.Drawing.Size(800, 400);
            this.webViewPdf.TabIndex = 1;
            this.webViewPdf.ZoomFactor = 1D;
            // 
            // FormPdfViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webViewPdf);
            this.Controls.Add(this.guna2Panel1);
            this.Name = "FormPdfViewer";
            this.Text = "FormPdfViewer";
            this.guna2Panel1.ResumeLayout(false);
            this.guna2Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webViewPdf)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Button btnGuardarComo;
        private Guna.UI2.WinForms.Guna2Button btnCerrar;
        private System.Windows.Forms.Label lblTitulo;
        private Microsoft.Web.WebView2.WinForms.WebView2 webViewPdf;
    }
}