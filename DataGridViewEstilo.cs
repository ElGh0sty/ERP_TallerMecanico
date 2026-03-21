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
            public static Color BotonEliminar = Color.FromArgb(220, 53, 69);      // Rojo para eliminar
            public static Color BotonEditar = Color.FromArgb(40, 167, 69);        // Verde para editar
            public static Color BotonVer = Color.FromArgb(0, 123, 255);           // Azul para ver
            public static Color BotonHover = Color.FromArgb(255, 89, 129);        // Rosa para hover
        }

        /// <summary>
        /// Aplica el estilo oscuro del dashboard a un DataGridView con textos blancos
        /// </summary>
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

            // Alternar colores entre filas
            if (alternarColores)
            {
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 38, 75);
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = Colores.TextoBlanco;
            }

            // ScrollBars
            dgv.ScrollBars = ScrollBars.Vertical;
        }

        /// <summary>
        /// Configura un botón columna con estilo personalizado
        /// </summary>
        /// <param name="dgv">DataGridView que contiene la columna</param>
        /// <param name="nombreColumna">Nombre de la columna botón</param>
        /// <param name="color">Color del botón</param>
        /// <param name="texto">Texto del botón</param>
        /// <param name="ancho">Ancho de la columna (opcional)</param>
        public static void ConfigurarBotonColumna(DataGridView dgv, string nombreColumna, Color color, string texto = "Acción", int? ancho = null)
        {
            if (dgv.Columns[nombreColumna] is DataGridViewButtonColumn btnCol)
            {
                btnCol.Text = texto;
                btnCol.UseColumnTextForButtonValue = true;
                btnCol.FlatStyle = FlatStyle.Flat;

                if (ancho.HasValue)
                    btnCol.Width = ancho.Value;

                // Estilo del botón
                btnCol.DefaultCellStyle.BackColor = color;
                btnCol.DefaultCellStyle.ForeColor = Color.White;
                btnCol.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                btnCol.DefaultCellStyle.SelectionBackColor = color;
                btnCol.DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        /// <summary>
        /// Configura un botón de eliminar en el DataGridView
        /// </summary>
        public static void ConfigurarBotonEliminar(DataGridView dgv, string nombreColumna, int ancho = 80)
        {
            ConfigurarBotonColumna(dgv, nombreColumna, Colores.BotonEliminar, "Eliminar", ancho);

            // Personalizar el evento CellPainting para mejor efecto visual (opcional)
            dgv.CellPainting += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == nombreColumna)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                    var rect = e.CellBounds;
                    rect.Inflate(-8, -8);

                    using (var brush = new SolidBrush(Colores.BotonEliminar))
                    using (var pen = new Pen(Color.FromArgb(200, 40, 40)))
                    {
                        e.Graphics.FillRectangle(brush, rect);
                        e.Graphics.DrawRectangle(pen, rect);

                        var sf = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        };

                        e.Graphics.DrawString("Eliminar", new Font("Segoe UI", 9F, FontStyle.Bold), Brushes.White, rect, sf);
                    }
                    e.Handled = true;
                }
            };
        }

        /// <summary>
        /// Configura un botón de editar en el DataGridView
        /// </summary>
        public static void ConfigurarBotonEditar(DataGridView dgv, string nombreColumna, int ancho = 80)
        {
            ConfigurarBotonColumna(dgv, nombreColumna, Colores.BotonEditar, "Editar", ancho);
        }

        /// <summary>
        /// Configura un botón de ver/detalle en el DataGridView
        /// </summary>
        public static void ConfigurarBotonVer(DataGridView dgv, string nombreColumna, int ancho = 80)
        {
            ConfigurarBotonColumna(dgv, nombreColumna, Colores.BotonVer, "Ver", ancho);
        }

        /// <summary>
        /// Configura un botón personalizado con evento de hover
        /// </summary>
        public static void ConfigurarBotonPersonalizado(DataGridView dgv, string nombreColumna, Color color, string texto, int ancho = 80)
        {
            ConfigurarBotonColumna(dgv, nombreColumna, color, texto, ancho);
        }

        /// <summary>
        /// Formatea una columna como moneda
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
        /// Formatea una columna como fecha
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
        /// Formatea una columna como número entero
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
        /// Formatea una columna como porcentaje
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

        /// <summary>
        /// Aplica el estilo completo a un DataGridView con botones incluidos
        /// </summary>
        public static void AplicarEstiloCompleto(DataGridView dgv, bool alternarColores = true)
        {
            AplicarEstiloDashboard(dgv, alternarColores);

            // Buscar columnas de botón y configurarlas
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col is DataGridViewButtonColumn btnCol)
                {
                    // Configurar botones por defecto
                    btnCol.FlatStyle = FlatStyle.Flat;
                    btnCol.DefaultCellStyle.BackColor = Colores.BotonEliminar;
                    btnCol.DefaultCellStyle.ForeColor = Color.White;
                    btnCol.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    btnCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    btnCol.DefaultCellStyle.SelectionBackColor = Colores.BotonEliminar;
                }
            }
        }
    }
}