using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTOMECANICO.Modulo_Inventario
{
    public partial class FormInventario : Form
    {
        public FormInventario()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Conexion objetoConexion = new Conexion();
            objetoConexion.Abrir();

            if (objetoConexion.leer.State == System.Data.ConnectionState.Open)
            {
                MessageBox.Show("¡Conexión Exitosa con TallerMecanicoERP!");
                objetoConexion.Cerrar();
            }
        }
    
    }
}
