namespace PROYECTOMECANICO.Modulo_Clientes
{
    partial class FormClientes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClientes));
            this.btnCatalogo = new System.Windows.Forms.Button();
            this.btnOrden = new System.Windows.Forms.Button();
            this.btnRegistrarVe = new System.Windows.Forms.Button();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnCatalogo
            // 
            this.btnCatalogo.BackColor = System.Drawing.Color.Orchid;
            this.btnCatalogo.FlatAppearance.BorderSize = 0;
            this.btnCatalogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatalogo.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCatalogo.ForeColor = System.Drawing.Color.White;
            this.btnCatalogo.Image = global::PROYECTOMECANICO.Properties.Resources.file_document_new_contract_icon_131249;
            this.btnCatalogo.Location = new System.Drawing.Point(43, 268);
            this.btnCatalogo.Name = "btnCatalogo";
            this.btnCatalogo.Size = new System.Drawing.Size(372, 169);
            this.btnCatalogo.TabIndex = 16;
            this.btnCatalogo.Text = "Catalogo de Registrados";
            this.btnCatalogo.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnCatalogo.UseVisualStyleBackColor = false;
            this.btnCatalogo.Click += new System.EventHandler(this.btnCatalogo_Click);
            // 
            // btnOrden
            // 
            this.btnOrden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOrden.BackColor = System.Drawing.Color.Gold;
            this.btnOrden.FlatAppearance.BorderSize = 0;
            this.btnOrden.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOrden.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrden.ForeColor = System.Drawing.Color.White;
            this.btnOrden.Image = global::PROYECTOMECANICO.Properties.Resources.search_find_magnify_icon_131253;
            this.btnOrden.Location = new System.Drawing.Point(493, 268);
            this.btnOrden.Name = "btnOrden";
            this.btnOrden.Size = new System.Drawing.Size(372, 169);
            this.btnOrden.TabIndex = 17;
            this.btnOrden.Text = "Generar Orden de Trabajo";
            this.btnOrden.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnOrden.UseVisualStyleBackColor = false;
            this.btnOrden.Click += new System.EventHandler(this.btnOrden_Click);
            // 
            // btnRegistrarVe
            // 
            this.btnRegistrarVe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRegistrarVe.BackColor = System.Drawing.Color.LightSalmon;
            this.btnRegistrarVe.FlatAppearance.BorderSize = 0;
            this.btnRegistrarVe.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistrarVe.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistrarVe.ForeColor = System.Drawing.Color.White;
            this.btnRegistrarVe.Image = global::PROYECTOMECANICO.Properties.Resources.car_23964;
            this.btnRegistrarVe.Location = new System.Drawing.Point(493, 40);
            this.btnRegistrarVe.Name = "btnRegistrarVe";
            this.btnRegistrarVe.Size = new System.Drawing.Size(372, 169);
            this.btnRegistrarVe.TabIndex = 15;
            this.btnRegistrarVe.Text = "Registrar Vehiculo";
            this.btnRegistrarVe.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnRegistrarVe.UseVisualStyleBackColor = false;
            this.btnRegistrarVe.Click += new System.EventHandler(this.btnRegistrarVe_Click);
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnRegistrar.FlatAppearance.BorderSize = 0;
            this.btnRegistrar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegistrar.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistrar.ForeColor = System.Drawing.Color.White;
            this.btnRegistrar.Image = global::PROYECTOMECANICO.Properties.Resources.user_account_person_avatar_icon_131248;
            this.btnRegistrar.Location = new System.Drawing.Point(43, 40);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(372, 169);
            this.btnRegistrar.TabIndex = 14;
            this.btnRegistrar.Text = "Registrar Nuevo Cliente";
            this.btnRegistrar.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.btnRegistrar.UseVisualStyleBackColor = false;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            // 
            // FormClientes
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(79)))));
            this.ClientSize = new System.Drawing.Size(908, 603);
            this.Controls.Add(this.btnCatalogo);
            this.Controls.Add(this.btnOrden);
            this.Controls.Add(this.btnRegistrarVe);
            this.Controls.Add(this.btnRegistrar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormClientes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modulo Clientes";
            this.Resize += new System.EventHandler(this.FormClientes_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCatalogo;
        private System.Windows.Forms.Button btnOrden;
        private System.Windows.Forms.Button btnRegistrarVe;
        private System.Windows.Forms.Button btnRegistrar;
    }
}