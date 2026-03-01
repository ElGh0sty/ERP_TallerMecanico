namespace PROYECTOMECANICO.Modulo_Personal
{
    partial class FormPersonal
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
            Guna.Charts.WinForms.ChartFont chartFont25 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.ChartFont chartFont26 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.ChartFont chartFont27 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.ChartFont chartFont28 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.Grid grid10 = new Guna.Charts.WinForms.Grid();
            Guna.Charts.WinForms.Tick tick10 = new Guna.Charts.WinForms.Tick();
            Guna.Charts.WinForms.ChartFont chartFont29 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.Grid grid11 = new Guna.Charts.WinForms.Grid();
            Guna.Charts.WinForms.Tick tick11 = new Guna.Charts.WinForms.Tick();
            Guna.Charts.WinForms.ChartFont chartFont30 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.Grid grid12 = new Guna.Charts.WinForms.Grid();
            Guna.Charts.WinForms.PointLabel pointLabel4 = new Guna.Charts.WinForms.PointLabel();
            Guna.Charts.WinForms.ChartFont chartFont31 = new Guna.Charts.WinForms.ChartFont();
            Guna.Charts.WinForms.Tick tick12 = new Guna.Charts.WinForms.Tick();
            Guna.Charts.WinForms.ChartFont chartFont32 = new Guna.Charts.WinForms.ChartFont();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new Guna.UI2.WinForms.Guna2Panel();
            this.btnNuevo = new Guna.UI2.WinForms.Guna2Button();
            this.btnLimpiar = new Guna.UI2.WinForms.Guna2Button();
            this.btnBuscar = new Guna.UI2.WinForms.Guna2Button();
            this.cbFiltroRol = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtBuscar = new Guna.UI2.WinForms.Guna2TextBox();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelChart = new Guna.UI2.WinForms.Guna2Panel();
            this.panelEdit = new Guna.UI2.WinForms.Guna2Panel();
            this.chartRoles = new Guna.Charts.WinForms.GunaChart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvUsuarios = new Guna.UI2.WinForms.Guna2DataGridView();
            this.txtUsuario = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtContrasena = new Guna.UI2.WinForms.Guna2TextBox();
            this.cbRol = new Guna.UI2.WinForms.Guna2ComboBox();
            this.swActivo = new Guna.UI2.WinForms.Guna2ToggleSwitch();
            this.btnGuardar = new Guna.UI2.WinForms.Guna2Button();
            this.btnDesactivar = new Guna.UI2.WinForms.Guna2Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelChart.SuspendLayout();
            this.panelEdit.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnNuevo);
            this.panelTop.Controls.Add(this.btnLimpiar);
            this.panelTop.Controls.Add(this.btnBuscar);
            this.panelTop.Controls.Add(this.cbFiltroRol);
            this.panelTop.Controls.Add(this.txtBuscar);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(908, 100);
            this.panelTop.TabIndex = 0;
            // 
            // btnNuevo
            // 
            this.btnNuevo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnNuevo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnNuevo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnNuevo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnNuevo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnNuevo.ForeColor = System.Drawing.Color.White;
            this.btnNuevo.Location = new System.Drawing.Point(747, 27);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(106, 45);
            this.btnNuevo.TabIndex = 4;
            this.btnNuevo.Text = "Nuevo";
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnLimpiar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnLimpiar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnLimpiar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLimpiar.ForeColor = System.Drawing.Color.White;
            this.btnLimpiar.Location = new System.Drawing.Point(598, 27);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(106, 45);
            this.btnLimpiar.TabIndex = 3;
            this.btnLimpiar.Text = "Limpiar";
            // 
            // btnBuscar
            // 
            this.btnBuscar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnBuscar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnBuscar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnBuscar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnBuscar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnBuscar.ForeColor = System.Drawing.Color.White;
            this.btnBuscar.Location = new System.Drawing.Point(454, 27);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(106, 45);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Buscar";
            // 
            // cbFiltroRol
            // 
            this.cbFiltroRol.BackColor = System.Drawing.Color.Transparent;
            this.cbFiltroRol.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbFiltroRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFiltroRol.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbFiltroRol.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbFiltroRol.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbFiltroRol.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbFiltroRol.ItemHeight = 30;
            this.cbFiltroRol.Location = new System.Drawing.Point(274, 32);
            this.cbFiltroRol.Name = "cbFiltroRol";
            this.cbFiltroRol.Size = new System.Drawing.Size(140, 36);
            this.cbFiltroRol.TabIndex = 1;
            // 
            // txtBuscar
            // 
            this.txtBuscar.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBuscar.DefaultText = "";
            this.txtBuscar.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtBuscar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtBuscar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBuscar.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtBuscar.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBuscar.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtBuscar.Location = new System.Drawing.Point(33, 32);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.PasswordChar = '\0';
            this.txtBuscar.PlaceholderText = "Buscar usuario…";
            this.txtBuscar.SelectedText = "";
            this.txtBuscar.Size = new System.Drawing.Size(200, 36);
            this.txtBuscar.TabIndex = 0;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 100);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.panelChart);
            this.splitMain.Panel1MinSize = 320;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.panel1);
            this.splitMain.Panel2MinSize = 400;
            this.splitMain.Size = new System.Drawing.Size(908, 361);
            this.splitMain.SplitterDistance = 333;
            this.splitMain.TabIndex = 6;
            // 
            // panelChart
            // 
            this.panelChart.BorderRadius = 12;
            this.panelChart.Controls.Add(this.chartRoles);
            this.panelChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChart.Location = new System.Drawing.Point(0, 0);
            this.panelChart.Name = "panelChart";
            this.panelChart.Padding = new System.Windows.Forms.Padding(10);
            this.panelChart.Size = new System.Drawing.Size(333, 361);
            this.panelChart.TabIndex = 0;
            // 
            // panelEdit
            // 
            this.panelEdit.Controls.Add(this.label1);
            this.panelEdit.Controls.Add(this.btnDesactivar);
            this.panelEdit.Controls.Add(this.btnGuardar);
            this.panelEdit.Controls.Add(this.swActivo);
            this.panelEdit.Controls.Add(this.cbRol);
            this.panelEdit.Controls.Add(this.txtContrasena);
            this.panelEdit.Controls.Add(this.txtUsuario);
            this.panelEdit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelEdit.Location = new System.Drawing.Point(0, 461);
            this.panelEdit.Name = "panelEdit";
            this.panelEdit.Padding = new System.Windows.Forms.Padding(10);
            this.panelEdit.Size = new System.Drawing.Size(908, 140);
            this.panelEdit.TabIndex = 4;
            // 
            // chartRoles
            // 
            this.chartRoles.Dock = System.Windows.Forms.DockStyle.Fill;
            chartFont25.FontName = "Arial";
            this.chartRoles.Legend.LabelFont = chartFont25;
            this.chartRoles.Location = new System.Drawing.Point(10, 10);
            this.chartRoles.Name = "chartRoles";
            this.chartRoles.Size = new System.Drawing.Size(313, 341);
            this.chartRoles.TabIndex = 1;
            chartFont26.FontName = "Arial";
            chartFont26.Size = 12;
            chartFont26.Style = Guna.Charts.WinForms.ChartFontStyle.Bold;
            this.chartRoles.Title.Font = chartFont26;
            chartFont27.FontName = "Arial";
            this.chartRoles.Tooltips.BodyFont = chartFont27;
            chartFont28.FontName = "Arial";
            chartFont28.Size = 9;
            chartFont28.Style = Guna.Charts.WinForms.ChartFontStyle.Bold;
            this.chartRoles.Tooltips.TitleFont = chartFont28;
            this.chartRoles.XAxes.GridLines = grid10;
            chartFont29.FontName = "Arial";
            tick10.Font = chartFont29;
            this.chartRoles.XAxes.Ticks = tick10;
            this.chartRoles.YAxes.GridLines = grid11;
            chartFont30.FontName = "Arial";
            tick11.Font = chartFont30;
            this.chartRoles.YAxes.Ticks = tick11;
            this.chartRoles.ZAxes.GridLines = grid12;
            chartFont31.FontName = "Arial";
            pointLabel4.Font = chartFont31;
            this.chartRoles.ZAxes.PointLabels = pointLabel4;
            chartFont32.FontName = "Arial";
            tick12.Font = chartFont32;
            this.chartRoles.ZAxes.Ticks = tick12;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvUsuarios);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(10);
            this.panel1.Size = new System.Drawing.Size(571, 361);
            this.panel1.TabIndex = 0;
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AllowUserToAddRows = false;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
            this.dgvUsuarios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle10;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dgvUsuarios.ColumnHeadersHeight = 4;
            this.dgvUsuarios.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvUsuarios.DefaultCellStyle = dataGridViewCellStyle12;
            this.dgvUsuarios.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvUsuarios.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvUsuarios.Location = new System.Drawing.Point(10, 10);
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.RowHeadersVisible = false;
            this.dgvUsuarios.Size = new System.Drawing.Size(551, 341);
            this.dgvUsuarios.TabIndex = 3;
            this.dgvUsuarios.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvUsuarios.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dgvUsuarios.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dgvUsuarios.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dgvUsuarios.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dgvUsuarios.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dgvUsuarios.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvUsuarios.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(88)))), ((int)(((byte)(255)))));
            this.dgvUsuarios.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvUsuarios.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvUsuarios.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dgvUsuarios.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dgvUsuarios.ThemeStyle.HeaderStyle.Height = 4;
            this.dgvUsuarios.ThemeStyle.ReadOnly = true;
            this.dgvUsuarios.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.White;
            this.dgvUsuarios.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvUsuarios.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgvUsuarios.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            this.dgvUsuarios.ThemeStyle.RowsStyle.Height = 22;
            this.dgvUsuarios.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvUsuarios.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(69)))), ((int)(((byte)(94)))));
            // 
            // txtUsuario
            // 
            this.txtUsuario.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtUsuario.DefaultText = "";
            this.txtUsuario.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtUsuario.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtUsuario.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtUsuario.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtUsuario.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtUsuario.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtUsuario.Location = new System.Drawing.Point(33, 55);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.PasswordChar = '\0';
            this.txtUsuario.PlaceholderText = "Usuario";
            this.txtUsuario.SelectedText = "";
            this.txtUsuario.Size = new System.Drawing.Size(200, 36);
            this.txtUsuario.TabIndex = 0;
            // 
            // txtContrasena
            // 
            this.txtContrasena.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtContrasena.DefaultText = "";
            this.txtContrasena.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtContrasena.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtContrasena.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtContrasena.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtContrasena.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtContrasena.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtContrasena.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtContrasena.Location = new System.Drawing.Point(274, 55);
            this.txtContrasena.Name = "txtContrasena";
            this.txtContrasena.PasswordChar = '\0';
            this.txtContrasena.PlaceholderText = "Contraseña";
            this.txtContrasena.SelectedText = "";
            this.txtContrasena.Size = new System.Drawing.Size(200, 36);
            this.txtContrasena.TabIndex = 1;
            // 
            // cbRol
            // 
            this.cbRol.BackColor = System.Drawing.Color.Transparent;
            this.cbRol.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRol.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbRol.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cbRol.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cbRol.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cbRol.ItemHeight = 30;
            this.cbRol.Location = new System.Drawing.Point(515, 55);
            this.cbRol.Name = "cbRol";
            this.cbRol.Size = new System.Drawing.Size(140, 36);
            this.cbRol.TabIndex = 5;
            // 
            // swActivo
            // 
            this.swActivo.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.swActivo.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.swActivo.CheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.swActivo.CheckedState.InnerColor = System.Drawing.Color.White;
            this.swActivo.Location = new System.Drawing.Point(683, 63);
            this.swActivo.Name = "swActivo";
            this.swActivo.Size = new System.Drawing.Size(35, 20);
            this.swActivo.TabIndex = 6;
            this.swActivo.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.swActivo.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
            this.swActivo.UncheckedState.InnerBorderColor = System.Drawing.Color.White;
            this.swActivo.UncheckedState.InnerColor = System.Drawing.Color.White;
            // 
            // btnGuardar
            // 
            this.btnGuardar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGuardar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(763, 13);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(106, 45);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "Guardar";
            // 
            // btnDesactivar
            // 
            this.btnDesactivar.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnDesactivar.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnDesactivar.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnDesactivar.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnDesactivar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDesactivar.ForeColor = System.Drawing.Color.White;
            this.btnDesactivar.Location = new System.Drawing.Point(763, 83);
            this.btnDesactivar.Name = "btnDesactivar";
            this.btnDesactivar.Size = new System.Drawing.Size(106, 45);
            this.btnDesactivar.TabIndex = 7;
            this.btnDesactivar.Text = "Activar/Desactivar";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(678, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Activo";
            // 
            // FormPersonal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(908, 601);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelEdit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPersonal";
            this.Text = "Personal";
            this.panelTop.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelChart.ResumeLayout(false);
            this.panelEdit.ResumeLayout(false);
            this.panelEdit.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel panelTop;
        private Guna.UI2.WinForms.Guna2Button btnNuevo;
        private Guna.UI2.WinForms.Guna2Button btnLimpiar;
        private Guna.UI2.WinForms.Guna2Button btnBuscar;
        private Guna.UI2.WinForms.Guna2ComboBox cbFiltroRol;
        private Guna.UI2.WinForms.Guna2TextBox txtBuscar;
        private System.Windows.Forms.SplitContainer splitMain;
        private Guna.UI2.WinForms.Guna2Panel panelChart;
        private Guna.Charts.WinForms.GunaChart chartRoles;
        private Guna.UI2.WinForms.Guna2DataGridView dgvUsuarios;
        private System.Windows.Forms.Panel panel1;
        private Guna.UI2.WinForms.Guna2Panel panelEdit;
        private Guna.UI2.WinForms.Guna2Button btnDesactivar;
        private Guna.UI2.WinForms.Guna2Button btnGuardar;
        private Guna.UI2.WinForms.Guna2ToggleSwitch swActivo;
        private Guna.UI2.WinForms.Guna2ComboBox cbRol;
        private Guna.UI2.WinForms.Guna2TextBox txtContrasena;
        private Guna.UI2.WinForms.Guna2TextBox txtUsuario;
        private System.Windows.Forms.Label label1;
    }
}