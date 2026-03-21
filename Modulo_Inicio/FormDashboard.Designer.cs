namespace PROYECTOMECANICO.Modulo_Inicio
{
    partial class FormDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.btnThisMonth = new System.Windows.Forms.Button();
            this.btnLast30Days = new System.Windows.Forms.Button();
            this.btnLast7Days = new System.Windows.Forms.Button();
            this.btnToday = new System.Windows.Forms.Button();
            this.btnCustomDate = new System.Windows.Forms.Button();
            this.btnOkCustomDate = new System.Windows.Forms.Button();
            this.panelOrdenes = new System.Windows.Forms.Panel();
            this.lblNumOrdenes = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panelVentas = new System.Windows.Forms.Panel();
            this.lblTotalVentas = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelGastos = new System.Windows.Forms.Panel();
            this.lblTotalGastos = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelGanancia = new System.Windows.Forms.Panel();
            this.lblGanancia = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.chartVentas = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chartTopProductos = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panelContadores = new System.Windows.Forms.Panel();
            this.lblOrdenesActivas = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblTotalProductos = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblTotalProveedores = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTotalClientes = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panelBajoStock = new System.Windows.Forms.Panel();
            this.dgvBajoStock = new System.Windows.Forms.DataGridView();
            this.lblSinStock = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblStartDate = new System.Windows.Forms.Label();
            this.lblEndDate = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.panelOrdenes.SuspendLayout();
            this.panelVentas.SuspendLayout();
            this.panelGastos.SuspendLayout();
            this.panelGanancia.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTopProductos)).BeginInit();
            this.panelContadores.SuspendLayout();
            this.panelBajoStock.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBajoStock)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.CustomFormat = "dd/MM/yyyy";
            this.dtpStartDate.Enabled = false;
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStartDate.Location = new System.Drawing.Point(77, 17);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(100, 23);
            this.dtpStartDate.TabIndex = 1;
            this.dtpStartDate.ValueChanged += new System.EventHandler(this.dtpStartDate_ValueChanged);
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.CustomFormat = "dd/MM/yyyy";
            this.dtpEndDate.Enabled = false;
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEndDate.Location = new System.Drawing.Point(191, 17);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(100, 23);
            this.dtpEndDate.TabIndex = 2;
            this.dtpEndDate.ValueChanged += new System.EventHandler(this.dtpEndDate_ValueChanged);
            // 
            // btnThisMonth
            // 
            this.btnThisMonth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThisMonth.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnThisMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThisMonth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.btnThisMonth.Location = new System.Drawing.Point(781, 12);
            this.btnThisMonth.Name = "btnThisMonth";
            this.btnThisMonth.Size = new System.Drawing.Size(100, 30);
            this.btnThisMonth.TabIndex = 3;
            this.btnThisMonth.Text = "Este mes";
            this.btnThisMonth.UseVisualStyleBackColor = true;
            this.btnThisMonth.Click += new System.EventHandler(this.btnThisMonth_Click);
            // 
            // btnLast30Days
            // 
            this.btnLast30Days.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLast30Days.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnLast30Days.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLast30Days.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.btnLast30Days.Location = new System.Drawing.Point(682, 12);
            this.btnLast30Days.Name = "btnLast30Days";
            this.btnLast30Days.Size = new System.Drawing.Size(100, 30);
            this.btnLast30Days.TabIndex = 4;
            this.btnLast30Days.Text = "30 días";
            this.btnLast30Days.UseVisualStyleBackColor = true;
            this.btnLast30Days.Click += new System.EventHandler(this.btnLast30Days_Click);
            // 
            // btnLast7Days
            // 
            this.btnLast7Days.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLast7Days.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnLast7Days.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLast7Days.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.btnLast7Days.Location = new System.Drawing.Point(583, 12);
            this.btnLast7Days.Name = "btnLast7Days";
            this.btnLast7Days.Size = new System.Drawing.Size(100, 30);
            this.btnLast7Days.TabIndex = 5;
            this.btnLast7Days.Text = "7 días";
            this.btnLast7Days.UseVisualStyleBackColor = true;
            this.btnLast7Days.Click += new System.EventHandler(this.btnLast7Days_Click);
            // 
            // btnToday
            // 
            this.btnToday.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToday.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnToday.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToday.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.btnToday.Location = new System.Drawing.Point(484, 12);
            this.btnToday.Name = "btnToday";
            this.btnToday.Size = new System.Drawing.Size(100, 30);
            this.btnToday.TabIndex = 6;
            this.btnToday.Text = "Hoy";
            this.btnToday.UseVisualStyleBackColor = true;
            this.btnToday.Click += new System.EventHandler(this.btnToday_Click);
            // 
            // btnCustomDate
            // 
            this.btnCustomDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCustomDate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnCustomDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCustomDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.btnCustomDate.Location = new System.Drawing.Point(385, 12);
            this.btnCustomDate.Name = "btnCustomDate";
            this.btnCustomDate.Size = new System.Drawing.Size(100, 30);
            this.btnCustomDate.TabIndex = 7;
            this.btnCustomDate.Text = "Personalizar";
            this.btnCustomDate.UseVisualStyleBackColor = true;
            this.btnCustomDate.Click += new System.EventHandler(this.btnCustomDate_Click);
            // 
            // btnOkCustomDate
            // 
            this.btnOkCustomDate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOkCustomDate.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            this.btnOkCustomDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkCustomDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.btnOkCustomDate.Location = new System.Drawing.Point(356, 12);
            this.btnOkCustomDate.Name = "btnOkCustomDate";
            this.btnOkCustomDate.Size = new System.Drawing.Size(30, 30);
            this.btnOkCustomDate.TabIndex = 8;
            this.btnOkCustomDate.Text = "✓";
            this.btnOkCustomDate.UseVisualStyleBackColor = true;
            this.btnOkCustomDate.Visible = false;
            this.btnOkCustomDate.Click += new System.EventHandler(this.btnOkCustomDate_Click);
            // 
            // panelOrdenes
            // 
            this.panelOrdenes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelOrdenes.Controls.Add(this.lblNumOrdenes);
            this.panelOrdenes.Controls.Add(this.label2);
            this.panelOrdenes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.panelOrdenes.Location = new System.Drawing.Point(18, 52);
            this.panelOrdenes.Name = "panelOrdenes";
            this.panelOrdenes.Size = new System.Drawing.Size(220, 70);
            this.panelOrdenes.TabIndex = 9;
            // 
            // lblNumOrdenes
            // 
            this.lblNumOrdenes.AutoSize = true;
            this.lblNumOrdenes.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblNumOrdenes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.lblNumOrdenes.Location = new System.Drawing.Point(15, 30);
            this.lblNumOrdenes.Name = "lblNumOrdenes";
            this.lblNumOrdenes.Size = new System.Drawing.Size(28, 32);
            this.lblNumOrdenes.TabIndex = 1;
            this.lblNumOrdenes.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label2.Location = new System.Drawing.Point(15, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Órdenes de trabajo";
            // 
            // panelVentas
            // 
            this.panelVentas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelVentas.Controls.Add(this.lblTotalVentas);
            this.panelVentas.Controls.Add(this.label4);
            this.panelVentas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.panelVentas.Location = new System.Drawing.Point(244, 52);
            this.panelVentas.Name = "panelVentas";
            this.panelVentas.Size = new System.Drawing.Size(220, 70);
            this.panelVentas.TabIndex = 10;
            // 
            // lblTotalVentas
            // 
            this.lblTotalVentas.AutoSize = true;
            this.lblTotalVentas.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalVentas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.lblTotalVentas.Location = new System.Drawing.Point(15, 30);
            this.lblTotalVentas.Name = "lblTotalVentas";
            this.lblTotalVentas.Size = new System.Drawing.Size(42, 32);
            this.lblTotalVentas.TabIndex = 1;
            this.lblTotalVentas.Text = "$0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label4.Location = new System.Drawing.Point(15, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "Total ventas";
            // 
            // panelGastos
            // 
            this.panelGastos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelGastos.Controls.Add(this.lblTotalGastos);
            this.panelGastos.Controls.Add(this.label5);
            this.panelGastos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.panelGastos.Location = new System.Drawing.Point(470, 52);
            this.panelGastos.Name = "panelGastos";
            this.panelGastos.Size = new System.Drawing.Size(220, 70);
            this.panelGastos.TabIndex = 11;
            // 
            // lblTotalGastos
            // 
            this.lblTotalGastos.AutoSize = true;
            this.lblTotalGastos.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTotalGastos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.lblTotalGastos.Location = new System.Drawing.Point(15, 30);
            this.lblTotalGastos.Name = "lblTotalGastos";
            this.lblTotalGastos.Size = new System.Drawing.Size(42, 32);
            this.lblTotalGastos.TabIndex = 1;
            this.lblTotalGastos.Text = "$0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label5.Location = new System.Drawing.Point(15, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "Total gastos";
            // 
            // panelGanancia
            // 
            this.panelGanancia.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelGanancia.Controls.Add(this.lblGanancia);
            this.panelGanancia.Controls.Add(this.label7);
            this.panelGanancia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.panelGanancia.Location = new System.Drawing.Point(696, 52);
            this.panelGanancia.Name = "panelGanancia";
            this.panelGanancia.Size = new System.Drawing.Size(216, 70);
            this.panelGanancia.TabIndex = 12;
            // 
            // lblGanancia
            // 
            this.lblGanancia.AutoSize = true;
            this.lblGanancia.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblGanancia.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblGanancia.Location = new System.Drawing.Point(15, 30);
            this.lblGanancia.Name = "lblGanancia";
            this.lblGanancia.Size = new System.Drawing.Size(42, 32);
            this.lblGanancia.TabIndex = 1;
            this.lblGanancia.Text = "$0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label7.Location = new System.Drawing.Point(15, 10);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(103, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Ganancia neta";
            // 
            // chartVentas
            // 
            this.chartVentas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            chartArea3.AxisX.IsMarginVisible = false;
            chartArea3.AxisX.LabelStyle.ForeColor = System.Drawing.Color.Silver;
            chartArea3.AxisX.LineColor = System.Drawing.Color.White;
            chartArea3.AxisX.LineWidth = 0;
            chartArea3.AxisX.MajorGrid.LineWidth = 0;
            chartArea3.AxisX.MajorTickMark.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(74)))), ((int)(((byte)(110)))));
            chartArea3.AxisX.MajorTickMark.LineWidth = 2;
            chartArea3.AxisX.MajorTickMark.Size = 3F;
            chartArea3.AxisY.LabelStyle.ForeColor = System.Drawing.Color.Silver;
            chartArea3.AxisY.LabelStyle.Format = "$(0:0,)K";
            chartArea3.AxisY.LineWidth = 0;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(74)))), ((int)(((byte)(110)))));
            chartArea3.AxisY.MajorTickMark.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(74)))), ((int)(((byte)(110)))));
            chartArea3.AxisY.MajorTickMark.LineWidth = 0;
            chartArea3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            chartArea3.Name = "ChartArea1";
            this.chartVentas.ChartAreas.Add(chartArea3);
            legend3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            legend3.IsDockedInsideChartArea = false;
            legend3.Name = "Legend1";
            this.chartVentas.Legends.Add(legend3);
            this.chartVentas.Location = new System.Drawing.Point(18, 128);
            this.chartVentas.Name = "chartVentas";
            series3.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            series3.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(83)))), ((int)(((byte)(255)))));
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.SplineArea;
            series3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(89)))), ((int)(((byte)(126)))));
            series3.LabelForeColor = System.Drawing.Color.White;
            series3.Legend = "Legend1";
            series3.MarkerSize = 10;
            series3.Name = "Series1";
            this.chartVentas.Series.Add(series3);
            this.chartVentas.Size = new System.Drawing.Size(550, 334);
            this.chartVentas.TabIndex = 13;
            this.chartVentas.Text = "chartVentas";
            title3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title3.ForeColor = System.Drawing.Color.White;
            title3.Name = "Title1";
            title3.Text = "Ventas por día";
            this.chartVentas.Titles.Add(title3);
            // 
            // chartTopProductos
            // 
            this.chartTopProductos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            chartArea4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            chartArea4.BorderColor = System.Drawing.Color.White;
            chartArea4.Name = "ChartArea1";
            this.chartTopProductos.ChartAreas.Add(chartArea4);
            legend4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            legend4.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend4.ForeColor = System.Drawing.Color.Silver;
            legend4.Name = "Legend1";
            this.chartTopProductos.Legends.Add(legend4);
            this.chartTopProductos.Location = new System.Drawing.Point(574, 128);
            this.chartTopProductos.Name = "chartTopProductos";
            this.chartTopProductos.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.chartTopProductos.PaletteCustomColors = new System.Drawing.Color[] {
        System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(153)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(106)))), ((int)(((byte)(83)))), ((int)(((byte)(255))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(219)))), ((int)(((byte)(207))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(88)))), ((int)(((byte)(128))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(188)))), ((int)(((byte)(0))))),
        System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(160)))), ((int)(((byte)(140)))))};
            series4.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.DiagonalLeft;
            series4.BackSecondaryColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            series4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            series4.BorderWidth = 3;
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Doughnut;
            series4.IsValueShownAsLabel = true;
            series4.LabelBackColor = System.Drawing.Color.Transparent;
            series4.LabelForeColor = System.Drawing.Color.Silver;
            series4.Legend = "Legend1";
            series4.Name = "Series1";
            this.chartTopProductos.Series.Add(series4);
            this.chartTopProductos.Size = new System.Drawing.Size(338, 334);
            this.chartTopProductos.TabIndex = 14;
            this.chartTopProductos.Text = "chartTopProductos";
            title4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title4.ForeColor = System.Drawing.Color.White;
            title4.Name = "Title1";
            title4.Text = "Top 5 productos más vendidos";
            this.chartTopProductos.Titles.Add(title4);
            // 
            // panelContadores
            // 
            this.panelContadores.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelContadores.Controls.Add(this.lblOrdenesActivas);
            this.panelContadores.Controls.Add(this.label13);
            this.panelContadores.Controls.Add(this.lblTotalProductos);
            this.panelContadores.Controls.Add(this.label11);
            this.panelContadores.Controls.Add(this.lblTotalProveedores);
            this.panelContadores.Controls.Add(this.label9);
            this.panelContadores.Controls.Add(this.lblTotalClientes);
            this.panelContadores.Controls.Add(this.label6);
            this.panelContadores.Controls.Add(this.label8);
            this.panelContadores.Location = new System.Drawing.Point(18, 468);
            this.panelContadores.Name = "panelContadores";
            this.panelContadores.Size = new System.Drawing.Size(280, 160);
            this.panelContadores.TabIndex = 15;
            // 
            // lblOrdenesActivas
            // 
            this.lblOrdenesActivas.AutoSize = true;
            this.lblOrdenesActivas.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblOrdenesActivas.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblOrdenesActivas.Location = new System.Drawing.Point(160, 124);
            this.lblOrdenesActivas.Name = "lblOrdenesActivas";
            this.lblOrdenesActivas.Size = new System.Drawing.Size(23, 25);
            this.lblOrdenesActivas.TabIndex = 8;
            this.lblOrdenesActivas.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label13.Location = new System.Drawing.Point(15, 127);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(109, 19);
            this.label13.TabIndex = 7;
            this.label13.Text = "Órdenes activas:";
            // 
            // lblTotalProductos
            // 
            this.lblTotalProductos.AutoSize = true;
            this.lblTotalProductos.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalProductos.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTotalProductos.Location = new System.Drawing.Point(160, 94);
            this.lblTotalProductos.Name = "lblTotalProductos";
            this.lblTotalProductos.Size = new System.Drawing.Size(23, 25);
            this.lblTotalProductos.TabIndex = 6;
            this.lblTotalProductos.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label11.Location = new System.Drawing.Point(15, 97);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 19);
            this.label11.TabIndex = 5;
            this.label11.Text = "Total productos:";
            // 
            // lblTotalProveedores
            // 
            this.lblTotalProveedores.AutoSize = true;
            this.lblTotalProveedores.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalProveedores.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTotalProveedores.Location = new System.Drawing.Point(160, 64);
            this.lblTotalProveedores.Name = "lblTotalProveedores";
            this.lblTotalProveedores.Size = new System.Drawing.Size(23, 25);
            this.lblTotalProveedores.TabIndex = 4;
            this.lblTotalProveedores.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label9.Location = new System.Drawing.Point(15, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 19);
            this.label9.TabIndex = 3;
            this.label9.Text = "Total proveedores:";
            // 
            // lblTotalClientes
            // 
            this.lblTotalClientes.AutoSize = true;
            this.lblTotalClientes.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTotalClientes.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblTotalClientes.Location = new System.Drawing.Point(160, 34);
            this.lblTotalClientes.Name = "lblTotalClientes";
            this.lblTotalClientes.Size = new System.Drawing.Size(23, 25);
            this.lblTotalClientes.TabIndex = 2;
            this.lblTotalClientes.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label6.Location = new System.Drawing.Point(15, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(90, 19);
            this.label6.TabIndex = 1;
            this.label6.Text = "Total clientes:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label8.Location = new System.Drawing.Point(14, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(98, 21);
            this.label8.TabIndex = 0;
            this.label8.Text = "Estadísticas";
            // 
            // panelBajoStock
            // 
            this.panelBajoStock.AutoScroll = true;
            this.panelBajoStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.panelBajoStock.Controls.Add(this.dgvBajoStock);
            this.panelBajoStock.Controls.Add(this.lblSinStock);
            this.panelBajoStock.Controls.Add(this.label3);
            this.panelBajoStock.Location = new System.Drawing.Point(304, 468);
            this.panelBajoStock.Name = "panelBajoStock";
            this.panelBajoStock.Size = new System.Drawing.Size(608, 160);
            this.panelBajoStock.TabIndex = 16;
            // 
            // dgvBajoStock
            // 
            this.dgvBajoStock.AllowUserToResizeColumns = false;
            this.dgvBajoStock.AllowUserToResizeRows = false;
            this.dgvBajoStock.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvBajoStock.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            this.dgvBajoStock.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvBajoStock.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvBajoStock.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBajoStock.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvBajoStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(45)))), ((int)(((byte)(86)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Silver;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(89)))), ((int)(((byte)(126)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvBajoStock.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvBajoStock.EnableHeadersVisualStyles = false;
            this.dgvBajoStock.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(74)))), ((int)(((byte)(110)))));
            this.dgvBajoStock.Location = new System.Drawing.Point(15, 37);
            this.dgvBajoStock.Name = "dgvBajoStock";
            this.dgvBajoStock.ReadOnly = true;
            this.dgvBajoStock.RowHeadersVisible = false;
            this.dgvBajoStock.RowTemplate.Height = 35;
            this.dgvBajoStock.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvBajoStock.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBajoStock.Size = new System.Drawing.Size(582, 115);
            this.dgvBajoStock.TabIndex = 2;
            // 
            // lblSinStock
            // 
            this.lblSinStock.AutoSize = true;
            this.lblSinStock.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSinStock.ForeColor = System.Drawing.Color.Green;
            this.lblSinStock.Location = new System.Drawing.Point(15, 90);
            this.lblSinStock.Name = "lblSinStock";
            this.lblSinStock.Size = new System.Drawing.Size(253, 21);
            this.lblSinStock.TabIndex = 1;
            this.lblSinStock.Text = "✓ No hay productos con bajo stock";
            this.lblSinStock.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label3.Location = new System.Drawing.Point(15, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(201, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Productos con bajo stock";
            // 
            // lblStartDate
            // 
            this.lblStartDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblStartDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.lblStartDate.Location = new System.Drawing.Point(73, 16);
            this.lblStartDate.Name = "lblStartDate";
            this.lblStartDate.Size = new System.Drawing.Size(104, 23);
            this.lblStartDate.TabIndex = 9;
            this.lblStartDate.Text = "Mar 21. 2026";
            this.lblStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStartDate.Click += new System.EventHandler(this.label10_Click);
            // 
            // lblEndDate
            // 
            this.lblEndDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEndDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.lblEndDate.Location = new System.Drawing.Point(187, 16);
            this.lblEndDate.Name = "lblEndDate";
            this.lblEndDate.Size = new System.Drawing.Size(104, 23);
            this.lblEndDate.TabIndex = 17;
            this.lblEndDate.Text = "Mar 21. 2026";
            this.lblEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEndDate.Click += new System.EventHandler(this.label12_Click);
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(139)))), ((int)(((byte)(182)))));
            this.label14.Location = new System.Drawing.Point(176, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(13, 23);
            this.label14.TabIndex = 18;
            this.label14.Text = "-";
            // 
            // FormDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(27)))), ((int)(((byte)(62)))));
            this.ClientSize = new System.Drawing.Size(924, 640);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.lblEndDate);
            this.Controls.Add(this.lblStartDate);
            this.Controls.Add(this.panelBajoStock);
            this.Controls.Add(this.panelContadores);
            this.Controls.Add(this.chartTopProductos);
            this.Controls.Add(this.chartVentas);
            this.Controls.Add(this.panelGanancia);
            this.Controls.Add(this.panelGastos);
            this.Controls.Add(this.panelVentas);
            this.Controls.Add(this.panelOrdenes);
            this.Controls.Add(this.btnOkCustomDate);
            this.Controls.Add(this.btnCustomDate);
            this.Controls.Add(this.btnToday);
            this.Controls.Add(this.btnLast7Days);
            this.Controls.Add(this.btnLast30Days);
            this.Controls.Add(this.btnThisMonth);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.dtpStartDate);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormDashboard";
            this.Text = "Dashboard";
            this.Load += new System.EventHandler(this.FormDashboard_Load);
            this.panelOrdenes.ResumeLayout(false);
            this.panelOrdenes.PerformLayout();
            this.panelVentas.ResumeLayout(false);
            this.panelVentas.PerformLayout();
            this.panelGastos.ResumeLayout(false);
            this.panelGastos.PerformLayout();
            this.panelGanancia.ResumeLayout(false);
            this.panelGanancia.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartVentas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chartTopProductos)).EndInit();
            this.panelContadores.ResumeLayout(false);
            this.panelContadores.PerformLayout();
            this.panelBajoStock.ResumeLayout(false);
            this.panelBajoStock.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBajoStock)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.Button btnThisMonth;
        private System.Windows.Forms.Button btnLast30Days;
        private System.Windows.Forms.Button btnLast7Days;
        private System.Windows.Forms.Button btnToday;
        private System.Windows.Forms.Button btnCustomDate;
        private System.Windows.Forms.Button btnOkCustomDate;
        private System.Windows.Forms.Panel panelOrdenes;
        private System.Windows.Forms.Label lblNumOrdenes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelVentas;
        private System.Windows.Forms.Label lblTotalVentas;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panelGastos;
        private System.Windows.Forms.Label lblTotalGastos;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panelGanancia;
        private System.Windows.Forms.Label lblGanancia;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartVentas;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTopProductos;
        private System.Windows.Forms.Panel panelContadores;
        private System.Windows.Forms.Label lblTotalClientes;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblTotalProductos;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblTotalProveedores;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblOrdenesActivas;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panelBajoStock;
        private System.Windows.Forms.DataGridView dgvBajoStock;
        private System.Windows.Forms.Label lblSinStock;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblStartDate;
        private System.Windows.Forms.Label lblEndDate;
        private System.Windows.Forms.Label label14;
    }
}