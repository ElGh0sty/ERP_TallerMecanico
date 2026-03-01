using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace PROYECTOMECANICO
{
    public partial class FormPdfViewer : Form
    {
        private readonly string _pdfPath;
        private readonly string _defaultSaveName;

        public FormPdfViewer(string pdfPath, string title = "Vista previa PDF", string defaultSaveName = "Documento.pdf")
        {
            InitializeComponent();

            _pdfPath = pdfPath;
            _defaultSaveName = defaultSaveName;

            lblTitulo.Text = title;

            this.Load += FormPdfViewer_Load;
            btnCerrar.Click += (s, e) => this.Close();
            btnGuardarComo.Click += (s, e) => GuardarComo();
        }

        private async void FormPdfViewer_Load(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_pdfPath) || !File.Exists(_pdfPath))
                {
                    MessageBox.Show("No se encontró el archivo PDF.", "PDF",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Close();
                    return;
                }

                // Inicializa WebView2 correctamente
                await webViewPdf.EnsureCoreWebView2Async();

                // Carga el PDF
                webViewPdf.Source = new Uri(_pdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando PDF:\n" + ex.Message, "PDF",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void GuardarComo()
        {
            try
            {
                if (!File.Exists(_pdfPath))
                {
                    MessageBox.Show("El archivo PDF no existe.", "Guardar",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF (*.pdf)|*.pdf";
                    sfd.FileName = _defaultSaveName;

                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    File.Copy(_pdfPath, sfd.FileName, true);

                    MessageBox.Show("PDF guardado correctamente.", "Guardar",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar:\n" + ex.Message, "Guardar",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
