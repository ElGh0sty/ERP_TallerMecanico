using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public partial class FormBuscador : Form
    {
        private Conexion con = new Conexion();
        private DataTable dt = new DataTable();

        public enum TipoBusqueda
        {
            Clientes,
            Productos,
            Servicios,
            Vehiculos,
            OrdenesTrabajo,
            Facturas
        }

        private TipoBusqueda tipo;
        private long tipoVehiculoId;

        public DataRow ResultadoSeleccionado { get; private set; }
        public int CantidadSeleccionada { get; private set; } = 1;

        public FormBuscador(TipoBusqueda tipoBusqueda, long tipoVehiculo = 0)
        {
            InitializeComponent();

            tipo = tipoBusqueda;
            tipoVehiculoId = tipoVehiculo;

            ConfigurarGrid();
            GridStyle(dgvDatos);

            txtBuscar.TextChanged += (s, e) => Filtrar();
            dgvDatos.CellDoubleClick += (s, e) => Seleccionar();
            btnSeleccionar.Click += (s, e) => Seleccionar();

            CargarDatos();
            // En el constructor de FormBuscador, después de CargarDatos(), agrega:
            if (tipo == TipoBusqueda.Facturas)
            {
                dgvDatos.Columns["id"].Visible = false;
                dgvDatos.Columns["secuencial"].HeaderText = "Secuencial";
                dgvDatos.Columns["fecha"].HeaderText = "Fecha";
                dgvDatos.Columns["cliente_nombre"].HeaderText = "Cliente";
                dgvDatos.Columns["total_final"].HeaderText = "Total";
                dgvDatos.Columns["total_final"].DefaultCellStyle.Format = "C2";
            }
            

            switch (tipo)
            {
                case TipoBusqueda.Clientes:
                    Text = "Buscar Cliente";
                    break;
                case TipoBusqueda.Productos:
                    Text = "Buscar Producto";
                    nudCantidad.Visible = true;
                    lblCantidad.Visible = true;
                    break;
                case TipoBusqueda.Servicios:
                    Text = "Buscar Servicio";
                    break;
                case TipoBusqueda.Vehiculos:
                    Text = "Buscar Vehículo";
                    break;
                case TipoBusqueda.OrdenesTrabajo:
                    Text = "Buscar Orden de Trabajo";
                    break;
            }
        }

        private void ConfigurarGrid()
        {
            dgvDatos.ReadOnly = true;
            dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDatos.MultiSelect = false;
            dgvDatos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDatos.RowHeadersVisible = false;
        }

        private void CargarDatos()
        {
            try
            {
                con.Abrir();

                string sql = "";

                switch (tipo)
                {
                    case TipoBusqueda.Clientes:
                        sql = @"SELECT id, nombre, telefono, email 
                                FROM Clientes
                                ORDER BY nombre";
                        break;

                    case TipoBusqueda.Productos:
                        sql = @"SELECT id, nombre, precio_pvp, stock
                                FROM Productos
                                ORDER BY nombre";
                        break;

                    case TipoBusqueda.Servicios:
                        sql = @"SELECT 
                                s.id,
                                s.nombre,
                                st.precio_mano_obra AS precio
                                FROM Servicios s
                                INNER JOIN ServicioTarifas st ON s.id = st.servicio_id
                                WHERE st.tipo_vehiculo_id = @tipoVehiculo AND st.activo = 1
                                ORDER BY s.nombre";
                        break;

                    case TipoBusqueda.Vehiculos:
                        sql = @"SELECT v.id, v.placa, c.nombre AS cliente
                                FROM Vehiculos v
                                INNER JOIN Clientes c ON v.cliente_id = c.id
                                ORDER BY v.placa";
                        break;

                    case TipoBusqueda.OrdenesTrabajo:
                        sql = @"SELECT 
                                ot.id,
                                v.placa,
                                c.nombre AS cliente,
                                ot.fecha_ingreso,
                                ot.estado
                                FROM OrdenesTrabajo ot
                                INNER JOIN Vehiculos v ON ot.vehiculo_id = v.id
                                INNER JOIN Clientes c ON v.cliente_id = c.id
                                WHERE ot.facturada = 0
                                ORDER BY ot.id DESC";
                        break;
                    case TipoBusqueda.Facturas:
                        sql = @"
        SELECT 
            f.id, 
            f.secuencial,
            f.establecimiento,
            f.punto_emision,
            f.clave_acceso,
            f.fecha,
            f.cliente_nombre_snap AS cliente_nombre,
            f.cliente_numero_documento_snap AS cliente_documento,
            f.total_final,
            f.estado
        FROM Facturas f
        WHERE f.estado = 'PENDIENTE'
        ORDER BY f.id DESC";
                        break;
                }

                SqlCommand cmd = new SqlCommand(sql, con.leer);

                if (tipo == TipoBusqueda.Servicios)
                    cmd.Parameters.AddWithValue("@tipoVehiculo", tipoVehiculoId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                dgvDatos.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando datos: " + ex.Message);
            }
            finally
            {
                con.Cerrar();
            }
        }

        private void GridStyle(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = true;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ScrollBars = ScrollBars.Both;
            dgv.ColumnHeadersHeight = 36;
        }
        private void Filtrar()
        {
            if (dt == null) return;

            string filtro = txtBuscar.Text.Replace("'", "''");

            if (string.IsNullOrWhiteSpace(filtro))
            {
                dt.DefaultView.RowFilter = "";
                return;
            }

            switch (tipo)
            {
                case TipoBusqueda.Clientes:
                case TipoBusqueda.Productos:
                case TipoBusqueda.Servicios:
                    dt.DefaultView.RowFilter =
                        $"Convert(id,'System.String') LIKE '%{filtro}%' OR nombre LIKE '%{filtro}%'";
                    break;

                case TipoBusqueda.Vehiculos:
                case TipoBusqueda.OrdenesTrabajo:
                    dt.DefaultView.RowFilter =
                        $"Convert(id,'System.String') LIKE '%{filtro}%' OR placa LIKE '%{filtro}%' OR cliente LIKE '%{filtro}%'";
                    break;
            }
        }

        private void Seleccionar()
        {
            if (dgvDatos.CurrentRow == null) return;

            ResultadoSeleccionado =
                ((DataRowView)dgvDatos.CurrentRow.DataBoundItem).Row;

            if (nudCantidad.Visible)
                CantidadSeleccionada = (int)nudCantidad.Value;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}