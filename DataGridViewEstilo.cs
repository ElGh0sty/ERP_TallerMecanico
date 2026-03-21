using System;
using System.Drawing;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public static class DataGridViewEstilo
    {
        // Paleta de colores del dashboard
        public static class Colores
        {
            public static Color FondoPrincipal = Color.FromArgb(23, 27, 62);      // Fondo principal oscuro
            public static Color FondoPaneles = Color.FromArgb(42, 45, 86);       // Fondo de paneles
            public static Color TextoBlanco = Color.White;                        // Texto blanco principal
            public static Color TextoSecundario = Color.FromArgb(220, 220, 220); // Texto gris claro
            public static Color LineasGrid = Color.FromArgb(73, 74, 110);         // Color de líneas del grid
            public static Color SeleccionFila = Color.FromArgb(241, 89, 126);     // Color rosa al seleccionar
            public static Color EncabezadoFondo = Color.FromArgb(42, 45, 86);     // Fondo del encabezado
            public static Color EncabezadoTexto = Color.White;                    // Texto del encabezado blanco
        }

        /// <summary>
        /// Aplica el estilo oscuro del dashboard a un DataGridView con textos blancos
        /// </summary>
        /// <param name="dgv">DataGridView a estilizar</param>
        /// <param name="alternarColores">Si se debe alternar colores entre filas</param>
        public static void AplicarEstiloDashboard(DataGridView dgv, bool alternarColores = true)
        {
            if (dgv == null) return;

            // Configuración básica
            dgv.BackgroundColor = Colores.FondoPaneles;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Colores.LineasGrid;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Estilo de filas - Texto blanco
            dgv.RowTemplate.Height = 35;
            dgv.RowTemplate.DefaultCellStyle.ForeColor = Colores.TextoBlanco;
            dgv.RowTemplate.DefaultCellStyle.BackColor = Colores.FondoPaneles;
            dgv.RowTemplate.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = Colores.SeleccionFila;
            dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = Color.White;

            // Estilo de encabezados - Texto blanco y negrita
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Colores.EncabezadoFondo;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Colores.EncabezadoTexto;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;

            // Alternar colores entre filas (con texto blanco)
            if (alternarColores)
            {
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 38, 75);
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = Colores.TextoBlanco;
            }

            // ScrollBars verticales visibles, horizontales ocultos
            dgv.ScrollBars = ScrollBars.Vertical;
        }

        /// <summary>
        /// Aplica el estilo con barras de scroll personalizadas
        /// </summary>
        public static void AplicarEstiloConScroll(DataGridView dgv, bool alternarColores = true)
        {
            AplicarEstiloDashboard(dgv, alternarColores);

            // Personalizar apariencia de las barras de scroll
            dgv.ScrollBars = ScrollBars.Vertical;
        }

        /// <summary>
        /// Aplica el estilo sin barras de scroll (para grids con poca información)
        /// </summary>
        public static void AplicarEstiloSinScroll(DataGridView dgv, bool alternarColores = true)
        {
            AplicarEstiloDashboard(dgv, alternarColores);
            dgv.ScrollBars = ScrollBars.None;
        }

        /// <summary>
        /// Configura un DataGridView para mostrar datos con formato de moneda
        /// </summary>
        public static void FormatearColumnaMoneda(DataGridView dgv, string columna, string formato = "C2")
        {
            if (dgv.Columns[columna] != null)
            {
                dgv.Columns[columna].DefaultCellStyle.Format = formato;
                dgv.Columns[columna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[columna].DefaultCellStyle.ForeColor = Colores.TextoBlanco;
            }
        }

        /// <summary>
        /// Configura una columna para mostrar fechas
        /// </summary>
        public static void FormatearColumnaFecha(DataGridView dgv, string columna, string formato = "dd/MM/yyyy HH:mm")
        {
            if (dgv.Columns[columna] != null)
            {
                dgv.Columns[columna].DefaultCellStyle.Format = formato;
                dgv.Columns[columna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns[columna].DefaultCellStyle.ForeColor = Colores.TextoBlanco;
            }
        }

        /// <summary>
        /// Configura una columna para mostrar números enteros
        /// </summary>
        public static void FormatearColumnaNumero(DataGridView dgv, string columna)
        {
            if (dgv.Columns[columna] != null)
            {
                dgv.Columns[columna].DefaultCellStyle.Format = "N0";
                dgv.Columns[columna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[columna].DefaultCellStyle.ForeColor = Colores.TextoBlanco;
            }
        }

        /// <summary>
        /// Configura una columna para mostrar porcentajes
        /// </summary>
        public static void FormatearColumnaPorcentaje(DataGridView dgv, string columna)
        {
            if (dgv.Columns[columna] != null)
            {
                dgv.Columns[columna].DefaultCellStyle.Format = "P2";
                dgv.Columns[columna].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgv.Columns[columna].DefaultCellStyle.ForeColor = Colores.TextoBlanco;
            }
        }
    }
}