namespace PROYECTOMECANICO.Modulo_Facturacion
{
    partial class FormNotaCredito
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.guna2Panel2 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnBuscarFactura = new Guna.UI2.WinForms.Guna2Button();
            this.txtNumFactura = new Guna.UI2.WinForms.Guna2TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCargarFactura = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel3 = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvFactura = new Guna.UI2.WinForms.Guna2DataGridView();
            this.guna2Panel4 = new Guna.UI2.WinForms.Guna2Panel();
            this.dgvDevolucion = new Guna.UI2.WinForms.Guna2DataGridView();
            this.guna2Panel5 = new Guna.UI2.WinForms.Guna2Panel();
            this.txtObservacion = new Guna.UI2.WinForms.Guna2TextBox();
            this.cmbMotivo = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtMotivo = new Guna.UI2.WinForms.Guna2TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCalcular = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel6 = new Guna.UI2.WinForms.Guna2Panel();
            this.txtTotalDev = new System.Windows.Forms.Label();
            this.txtIVADev = new System.Windows.Forms.Label();
            this.txtSubtotalDev = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.Label();
            this.txtIVA = new System.Windows.Forms.Label();
            this.txtSubtotal = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFechaFactura = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtCliente = new Guna.UI2.WinForms.Guna2TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnGenerarNota = new Guna.UI2.WinForms.Guna2Button();
            this.btnVistaPrevia = new Guna.UI2.WinForms.Guna2Button();
            this.btnLimpiar = new Guna.UI2.WinForms.Guna2Button();
            this.guna2Panel2.SuspendLayout();
            this.guna2Panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFactura)).BeginInit();
            this.guna2Panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevolucion)).BeginInit();
            this.guna2Panel5.SuspendLayout();
            this.guna2Panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2Panel2
            // 
            this.guna2Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.guna2Panel2.Controls.Add(this.btnBuscarFactura);
            this.guna2Panel2.Controls.Add(this.txtNumFactura);
            this.guna2Panel2.Controls.Add(this.label1);
            this.guna2Panel2.Controls.Add(this.btnCargarFactura);
            this.guna2Panel2.Location = new System.Drawing.Point(12, 12);
            this.guna2Panel2.Name = "guna2Panel2";
            this.guna2Panel2.Size = new System.Drawing.Size(500, 80);
            this.guna2Panel2.TabIndex = 1;
            // 
            // btnBuscarFactura
            // 
            this.btnBuscarFactura.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnBuscarFactura.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnBuscarFactura.ForeColor = System.Drawing.Color.White;
            this.btnBuscarFactura.Location = new System.Drawing.Point(330, 35);
            this.btnBuscarFactura.Name = "btnBuscarFactura";
            this.btnBuscarFactura.Size = new System.Drawing.Size(80, 36);
            this.btnBuscarFactura.TabIndex = 0;
            this.btnBuscarFactura.Text = "Buscar";
            this.btnBuscarFactura.Click += new System.EventHandler(this.BtnBuscarFactura_Click);
            // 
            // txtNumFactura
            // 
            this.txtNumFactura.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtNumFactura.DefaultText = "";
            this.txtNumFactura.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F);
            this.txtNumFactura.Location = new System.Drawing.Point(15, 35);
            this.txtNumFactura.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNumFactura.Name = "txtNumFactura";
            this.txtNumFactura.PasswordChar = '\0';
            this.txtNumFactura.PlaceholderText = "";
            this.txtNumFactura.ReadOnly = true;
            this.txtNumFactura.SelectedText = "";
            this.txtNumFactura.Size = new System.Drawing.Size(300, 36);
            this.txtNumFactura.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(15, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 21);
            this.label1.TabIndex = 2;
            this.label1.Text = "Número de Factura:";
            // 
            // btnCargarFactura
            // 
            this.btnCargarFactura.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnCargarFactura.Enabled = false;
            this.btnCargarFactura.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.btnCargarFactura.ForeColor = System.Drawing.Color.White;
            this.btnCargarFactura.Location = new System.Drawing.Point(420, 35);
            this.btnCargarFactura.Name = "btnCargarFactura";
            this.btnCargarFactura.Size = new System.Drawing.Size(70, 36);
            this.btnCargarFactura.TabIndex = 3;
            this.btnCargarFactura.Text = "Cargar";
            // 
            // guna2Panel3
            // 
            this.guna2Panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.guna2Panel3.Controls.Add(this.dgvFactura);
            this.guna2Panel3.Location = new System.Drawing.Point(12, 102);
            this.guna2Panel3.Name = "guna2Panel3";
            this.guna2Panel3.Size = new System.Drawing.Size(500, 200);
            this.guna2Panel3.TabIndex = 2;
            // 
            // dgvFactura
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvFactura.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFactura.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFactura.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvFactura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFactura.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFactura.Location = new System.Drawing.Point(0, 0);
            this.dgvFactura.Name = "dgvFactura";
            this.dgvFactura.RowHeadersVisible = false;
            this.dgvFactura.Size = new System.Drawing.Size(500, 200);
            this.dgvFactura.TabIndex = 0;
            this.dgvFactura.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvFactura.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvFactura.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvFactura.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvFactura.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvFactura.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvFactura.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFactura.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvFactura.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvFactura.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvFactura.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvFactura.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvFactura.ThemeStyle.HeaderStyle.Height = 23;
            this.dgvFactura.ThemeStyle.ReadOnly = false;
            this.dgvFactura.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvFactura.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvFactura.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvFactura.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvFactura.ThemeStyle.RowsStyle.Height = 22;
            this.dgvFactura.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFactura.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // guna2Panel4
            // 
            this.guna2Panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.guna2Panel4.Controls.Add(this.dgvDevolucion);
            this.guna2Panel4.Location = new System.Drawing.Point(12, 312);
            this.guna2Panel4.Name = "guna2Panel4";
            this.guna2Panel4.Size = new System.Drawing.Size(1076, 200);
            this.guna2Panel4.TabIndex = 3;
            // 
            // dgvDevolucion
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            this.dgvDevolucion.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDevolucion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvDevolucion.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDevolucion.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvDevolucion.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvDevolucion.Location = new System.Drawing.Point(0, 0);
            this.dgvDevolucion.Name = "dgvDevolucion";
            this.dgvDevolucion.RowHeadersVisible = false;
            this.dgvDevolucion.Size = new System.Drawing.Size(983, 200);
            this.dgvDevolucion.TabIndex = 0;
            this.dgvDevolucion.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvDevolucion.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvDevolucion.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvDevolucion.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvDevolucion.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvDevolucion.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvDevolucion.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvDevolucion.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvDevolucion.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvDevolucion.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvDevolucion.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvDevolucion.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDevolucion.ThemeStyle.HeaderStyle.Height = 23;
            this.dgvDevolucion.ThemeStyle.ReadOnly = false;
            this.dgvDevolucion.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvDevolucion.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvDevolucion.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvDevolucion.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvDevolucion.ThemeStyle.RowsStyle.Height = 22;
            this.dgvDevolucion.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvDevolucion.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvDevolucion.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvDevolucion_CellEndEdit);
            this.dgvDevolucion.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvDevolucion_CellValueChanged);
            // 
            // guna2Panel5
            // 
            this.guna2Panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.guna2Panel5.Controls.Add(this.txtObservacion);
            this.guna2Panel5.Controls.Add(this.cmbMotivo);
            this.guna2Panel5.Controls.Add(this.txtMotivo);
            this.guna2Panel5.Controls.Add(this.label6);
            this.guna2Panel5.Controls.Add(this.label5);
            this.guna2Panel5.Controls.Add(this.btnCalcular);
            this.guna2Panel5.Location = new System.Drawing.Point(520, 12);
            this.guna2Panel5.Name = "guna2Panel5";
            this.guna2Panel5.Size = new System.Drawing.Size(475, 290);
            this.guna2Panel5.TabIndex = 4;
            // 
            // txtObservacion
            // 
            this.txtObservacion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtObservacion.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtObservacion.DefaultText = "";
            this.txtObservacion.Enabled = false;
            this.txtObservacion.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F);
            this.txtObservacion.Location = new System.Drawing.Point(15, 125);
            this.txtObservacion.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtObservacion.Multiline = true;
            this.txtObservacion.Name = "txtObservacion";
            this.txtObservacion.PasswordChar = '\0';
            this.txtObservacion.PlaceholderText = "";
            this.txtObservacion.SelectedText = "";
            this.txtObservacion.Size = new System.Drawing.Size(445, 90);
            this.txtObservacion.TabIndex = 0;
            // 
            // cmbMotivo
            // 
            this.cmbMotivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMotivo.BackColor = System.Drawing.Color.Transparent;
            this.cmbMotivo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbMotivo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMotivo.FocusedColor = System.Drawing.Color.Empty;
            this.cmbMotivo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMotivo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbMotivo.ItemHeight = 30;
            this.cmbMotivo.Location = new System.Drawing.Point(15, 50);
            this.cmbMotivo.Name = "cmbMotivo";
            this.cmbMotivo.Size = new System.Drawing.Size(205, 36);
            this.cmbMotivo.TabIndex = 1;
            // 
            // txtMotivo
            // 
            this.txtMotivo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMotivo.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMotivo.DefaultText = "";
            this.txtMotivo.Enabled = false;
            this.txtMotivo.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F);
            this.txtMotivo.Location = new System.Drawing.Point(263, 50);
            this.txtMotivo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtMotivo.Name = "txtMotivo";
            this.txtMotivo.PasswordChar = '\0';
            this.txtMotivo.PlaceholderText = "Otro motivo...";
            this.txtMotivo.SelectedText = "";
            this.txtMotivo.Size = new System.Drawing.Size(197, 36);
            this.txtMotivo.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F);
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(15, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(182, 21);
            this.label6.TabIndex = 3;
            this.label6.Text = "Observación (opcional):";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F);
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(15, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(228, 21);
            this.label5.TabIndex = 4;
            this.label5.Text = "Motivo de la Nota de Crédito:";
            // 
            // btnCalcular
            // 
            this.btnCalcular.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalcular.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnCalcular.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnCalcular.Enabled = false;
            this.btnCalcular.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.btnCalcular.ForeColor = System.Drawing.Color.White;
            this.btnCalcular.Location = new System.Drawing.Point(15, 230);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(445, 45);
            this.btnCalcular.TabIndex = 5;
            this.btnCalcular.Text = "Calcular Devolución";
            this.btnCalcular.Click += new System.EventHandler(this.BtnCalcular_Click);
            // 
            // guna2Panel6
            // 
            this.guna2Panel6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.guna2Panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.guna2Panel6.Controls.Add(this.txtTotalDev);
            this.guna2Panel6.Controls.Add(this.txtIVADev);
            this.guna2Panel6.Controls.Add(this.txtSubtotalDev);
            this.guna2Panel6.Controls.Add(this.label11);
            this.guna2Panel6.Controls.Add(this.label10);
            this.guna2Panel6.Controls.Add(this.label9);
            this.guna2Panel6.Controls.Add(this.txtTotal);
            this.guna2Panel6.Controls.Add(this.txtIVA);
            this.guna2Panel6.Controls.Add(this.txtSubtotal);
            this.guna2Panel6.Controls.Add(this.label8);
            this.guna2Panel6.Controls.Add(this.label7);
            this.guna2Panel6.Controls.Add(this.label4);
            this.guna2Panel6.Controls.Add(this.txtFechaFactura);
            this.guna2Panel6.Controls.Add(this.txtCliente);
            this.guna2Panel6.Controls.Add(this.label3);
            this.guna2Panel6.Controls.Add(this.label2);
            this.guna2Panel6.Location = new System.Drawing.Point(427, 518);
            this.guna2Panel6.Name = "guna2Panel6";
            this.guna2Panel6.Size = new System.Drawing.Size(568, 200);
            this.guna2Panel6.TabIndex = 5;
            // 
            // txtTotalDev
            // 
            this.txtTotalDev.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotalDev.AutoSize = true;
            this.txtTotalDev.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.txtTotalDev.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(175)))), ((int)(((byte)(80)))));
            this.txtTotalDev.Location = new System.Drawing.Point(425, 175);
            this.txtTotalDev.Name = "txtTotalDev";
            this.txtTotalDev.Size = new System.Drawing.Size(50, 21);
            this.txtTotalDev.TabIndex = 0;
            this.txtTotalDev.Text = "$0.00";
            // 
            // txtIVADev
            // 
            this.txtIVADev.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIVADev.AutoSize = true;
            this.txtIVADev.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtIVADev.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtIVADev.Location = new System.Drawing.Point(420, 150);
            this.txtIVADev.Name = "txtIVADev";
            this.txtIVADev.Size = new System.Drawing.Size(45, 20);
            this.txtIVADev.TabIndex = 1;
            this.txtIVADev.Text = "$0.00";
            // 
            // txtSubtotalDev
            // 
            this.txtSubtotalDev.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtotalDev.AutoSize = true;
            this.txtSubtotalDev.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtSubtotalDev.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtSubtotalDev.Location = new System.Drawing.Point(455, 125);
            this.txtSubtotalDev.Name = "txtSubtotalDev";
            this.txtSubtotalDev.Size = new System.Drawing.Size(45, 20);
            this.txtSubtotalDev.TabIndex = 2;
            this.txtSubtotalDev.Text = "$0.00";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(380, 125);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 20);
            this.label11.TabIndex = 3;
            this.label11.Text = "Subtotal:";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(380, 90);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(141, 20);
            this.label10.TabIndex = 4;
            this.label10.Text = "Valores a Devolver:";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(15, 175);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 20);
            this.label9.TabIndex = 5;
            this.label9.Text = "Total:";
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotal.AutoSize = true;
            this.txtTotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtTotal.Location = new System.Drawing.Point(60, 175);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(45, 20);
            this.txtTotal.TabIndex = 6;
            this.txtTotal.Text = "$0.00";
            // 
            // txtIVA
            // 
            this.txtIVA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtIVA.AutoSize = true;
            this.txtIVA.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtIVA.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtIVA.Location = new System.Drawing.Point(55, 150);
            this.txtIVA.Name = "txtIVA";
            this.txtIVA.Size = new System.Drawing.Size(45, 20);
            this.txtIVA.TabIndex = 7;
            this.txtIVA.Text = "$0.00";
            // 
            // txtSubtotal
            // 
            this.txtSubtotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubtotal.AutoSize = true;
            this.txtSubtotal.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtSubtotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.txtSubtotal.Location = new System.Drawing.Point(90, 125);
            this.txtSubtotal.Name = "txtSubtotal";
            this.txtSubtotal.Size = new System.Drawing.Size(45, 20);
            this.txtSubtotal.TabIndex = 8;
            this.txtSubtotal.Text = "$0.00";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(15, 150);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 20);
            this.label8.TabIndex = 9;
            this.label8.Text = "IVA:";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(15, 125);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 20);
            this.label7.TabIndex = 10;
            this.label7.Text = "Subtotal:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(136, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "Valores Originales:";
            // 
            // txtFechaFactura
            // 
            this.txtFechaFactura.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFechaFactura.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtFechaFactura.DefaultText = "";
            this.txtFechaFactura.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtFechaFactura.Location = new System.Drawing.Point(280, 40);
            this.txtFechaFactura.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFechaFactura.Name = "txtFechaFactura";
            this.txtFechaFactura.PasswordChar = '\0';
            this.txtFechaFactura.PlaceholderText = "";
            this.txtFechaFactura.ReadOnly = true;
            this.txtFechaFactura.SelectedText = "";
            this.txtFechaFactura.Size = new System.Drawing.Size(273, 36);
            this.txtFechaFactura.TabIndex = 12;
            // 
            // txtCliente
            // 
            this.txtCliente.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCliente.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtCliente.DefaultText = "";
            this.txtCliente.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.txtCliente.Location = new System.Drawing.Point(15, 40);
            this.txtCliente.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtCliente.Name = "txtCliente";
            this.txtCliente.PasswordChar = '\0';
            this.txtCliente.PlaceholderText = "";
            this.txtCliente.ReadOnly = true;
            this.txtCliente.SelectedText = "";
            this.txtCliente.Size = new System.Drawing.Size(250, 36);
            this.txtCliente.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(280, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "Fecha Factura:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI Semibold", 11F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 20);
            this.label2.TabIndex = 15;
            this.label2.Text = "Cliente:";
            // 
            // btnGenerarNota
            // 
            this.btnGenerarNota.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnGenerarNota.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnGenerarNota.Enabled = false;
            this.btnGenerarNota.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.btnGenerarNota.ForeColor = System.Drawing.Color.White;
            this.btnGenerarNota.Location = new System.Drawing.Point(16, 548);
            this.btnGenerarNota.Name = "btnGenerarNota";
            this.btnGenerarNota.Size = new System.Drawing.Size(180, 50);
            this.btnGenerarNota.TabIndex = 2;
            this.btnGenerarNota.Text = "Generar Nota de Crédito";
            // 
            // btnVistaPrevia
            // 
            this.btnVistaPrevia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnVistaPrevia.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnVistaPrevia.Enabled = false;
            this.btnVistaPrevia.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.btnVistaPrevia.ForeColor = System.Drawing.Color.White;
            this.btnVistaPrevia.Location = new System.Drawing.Point(204, 548);
            this.btnVistaPrevia.Name = "btnVistaPrevia";
            this.btnVistaPrevia.Size = new System.Drawing.Size(150, 50);
            this.btnVistaPrevia.TabIndex = 1;
            this.btnVistaPrevia.Text = "Vista Previa";
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.btnLimpiar.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI Semibold", 12F);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(120, 604);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(150, 50);
            this.btnLimpiar.TabIndex = 0;
            this.btnLimpiar.Text = "Limpiar";
            // 
            // FormNotaCredito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.ClientSize = new System.Drawing.Size(1005, 733);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnVistaPrevia);
            this.Controls.Add(this.btnGenerarNota);
            this.Controls.Add(this.guna2Panel6);
            this.Controls.Add(this.guna2Panel5);
            this.Controls.Add(this.guna2Panel4);
            this.Controls.Add(this.guna2Panel3);
            this.Controls.Add(this.guna2Panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormNotaCredito";
            this.Text = "Nota de Crédito";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.guna2Panel2.ResumeLayout(false);
            this.guna2Panel2.PerformLayout();
            this.guna2Panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFactura)).EndInit();
            this.guna2Panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevolucion)).EndInit();
            this.guna2Panel5.ResumeLayout(false);
            this.guna2Panel5.PerformLayout();
            this.guna2Panel6.ResumeLayout(false);
            this.guna2Panel6.PerformLayout();
            this.ResumeLayout(false);

        }
        private Guna.UI2.WinForms.Guna2Panel guna2Panel2;
        private Guna.UI2.WinForms.Guna2Button btnBuscarFactura;
        private Guna.UI2.WinForms.Guna2TextBox txtNumFactura;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Button btnCargarFactura;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel3;
        private Guna.UI2.WinForms.Guna2DataGridView dgvFactura;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel4;
        private Guna.UI2.WinForms.Guna2DataGridView dgvDevolucion;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel5;
        private Guna.UI2.WinForms.Guna2ComboBox cmbMotivo;
        private Guna.UI2.WinForms.Guna2TextBox txtMotivo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private Guna.UI2.WinForms.Guna2Button btnCalcular;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel6;
        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2TextBox txtCliente;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2TextBox txtFechaFactura;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label txtSubtotal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label txtIVA;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label txtTotal;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label txtSubtotalDev;
        private System.Windows.Forms.Label txtIVADev;
        private System.Windows.Forms.Label txtTotalDev;
        private Guna.UI2.WinForms.Guna2TextBox txtObservacion;
        private Guna.UI2.WinForms.Guna2Button btnGenerarNota;
        private Guna.UI2.WinForms.Guna2Button btnVistaPrevia;
        private Guna.UI2.WinForms.Guna2Button btnLimpiar;
    }
}