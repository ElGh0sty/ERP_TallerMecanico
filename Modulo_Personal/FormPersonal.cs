using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Guna.Charts.WinForms;
using PROYECTOMECANICO;

namespace PROYECTOMECANICO.Modulo_Personal
{
    public partial class FormPersonal : Form
    {
        private readonly Conexion con = new Conexion();

        private long _idSeleccionado = 0;
        private DataTable _dt = new DataTable();

        public FormPersonal()
        {
            InitializeComponent();

            this.Load += FormPersonal_Load;

            btnBuscar.Click += (s, e) => CargarUsuarios();
            btnLimpiar.Click += (s, e) => LimpiarFiltros();
            btnNuevo.Click += (s, e) => Nuevo();

            btnGuardar.Click += (s, e) => Guardar();
            btnDesactivar.Click += (s, e) => ActivarDesactivar();

            dgvUsuarios.CellClick += DgvUsuarios_CellClick;
        }

        private void FormPersonal_Load(object sender, EventArgs e)
        {
            EstilizarGrid();
            InicializarChart();

            CargarRolesCombos();
            Nuevo();
            CargarUsuarios();
            SetModoEdicion(false);
        }

        private void SetModoEdicion(bool activo)
        {
            txtUsuario.Enabled = activo;
            txtContrasena.Enabled = activo;
            cbRol.Enabled = activo;
            swActivo.Enabled = activo;
            btnGuardar.Enabled = activo;
        }

        //  COMBOS ROLES 

        private void CargarRolesCombos()
        {
            using (var cn = con.CrearConexionAbierta())
            {
                var dtR = new DataTable();
                using (var da = new SqlDataAdapter("SELECT id, nombre FROM Roles ORDER BY nombre", cn))
                    da.Fill(dtR);

                // Combo de edición (sin "Todos")
                cbRol.DisplayMember = "nombre";
                cbRol.ValueMember = "id";
                cbRol.DataSource = dtR;

                // Combo filtro (con "Todos")
                var dtFiltro = dtR.Copy();
                var row = dtFiltro.NewRow();
                row["id"] = 0;
                row["nombre"] = "Todos";
                dtFiltro.Rows.InsertAt(row, 0);

                cbFiltroRol.DisplayMember = "nombre";
                cbFiltroRol.ValueMember = "id";
                cbFiltroRol.DataSource = dtFiltro;
                cbFiltroRol.SelectedIndex = 0;
            }
        }

        //  GRID 

        private void EstilizarGrid()
        {
            dgvUsuarios.AutoGenerateColumns = true;
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;

            dgvUsuarios.ColumnHeadersVisible = true;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.ColumnHeadersHeight = 36;

            // Scroll
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.ScrollBars = ScrollBars.Vertical; 
            dgvUsuarios.ScrollBars = ScrollBars.Both;
        }

        private void CargarUsuarios()
        {
            try
            {
                string buscar = (txtBuscar.Text ?? "").Trim();
                long rolFiltro = Convert.ToInt64(cbFiltroRol.SelectedValue ?? 0);

                string sql = @"
SELECT
    u.id AS Id,
    u.nombre_usuario AS Usuario,
    r.nombre AS Rol,
    u.activo AS Activo,
    u.rol_id AS RolId
FROM Usuarios u
INNER JOIN Roles r ON r.id = u.rol_id
WHERE
    (@buscar = '' OR u.nombre_usuario LIKE '%' + @buscar + '%')
    AND (@rolId = 0 OR u.rol_id = @rolId)
ORDER BY u.id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@buscar", buscar);
                    cmd.Parameters.AddWithValue("@rolId", rolFiltro);

                    using (var da = new SqlDataAdapter(cmd))
                    {
                        _dt = new DataTable();
                        da.Fill(_dt);
                        dgvUsuarios.DataSource = _dt;
                    }
                }

                FormatearColumnas();
                CargarChartRolesActivos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios:\n" + ex.Message,
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatearColumnas()
        {
            if (dgvUsuarios.Columns.Count == 0) return;

            // Oculta técnicos
            if (dgvUsuarios.Columns["Id"] != null) dgvUsuarios.Columns["Id"].Visible = false;
            if (dgvUsuarios.Columns["RolId"] != null) dgvUsuarios.Columns["RolId"].Visible = false;

            // Anchos
            if (dgvUsuarios.Columns["Usuario"] != null) dgvUsuarios.Columns["Usuario"].Width = 200;
            if (dgvUsuarios.Columns["Rol"] != null) dgvUsuarios.Columns["Rol"].Width = 160;
            if (dgvUsuarios.Columns["Activo"] != null) dgvUsuarios.Columns["Activo"].Width = 80;

            // Que llene todo el ancho sin dejar hueco a la derecha
            if (dgvUsuarios.Columns["Usuario"] != null)
            {
                dgvUsuarios.Columns["Usuario"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUsuarios.Columns["Usuario"].FillWeight = 55; // más grande
            }

            if (dgvUsuarios.Columns["Rol"] != null)
            {
                dgvUsuarios.Columns["Rol"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvUsuarios.Columns["Rol"].FillWeight = 35;
            }

            if (dgvUsuarios.Columns["Activo"] != null)
            {
                dgvUsuarios.Columns["Activo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvUsuarios.Columns["Activo"].MinimumWidth = 70; // que no se aplaste
            }
        }

        private void DgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvUsuarios.Rows[e.RowIndex];
            if (row == null) return;

            _idSeleccionado = Convert.ToInt64(row.Cells["Id"].Value);

            txtUsuario.Text = row.Cells["Usuario"].Value?.ToString() ?? "";
            cbRol.SelectedValue = Convert.ToInt64(row.Cells["RolId"].Value);
            swActivo.Checked = Convert.ToBoolean(row.Cells["Activo"].Value);

            txtContrasena.Text = "";
        }

        //  CRUD 

        private void Nuevo()
        {
            _idSeleccionado = 0;

            txtUsuario.Text = "";
            txtContrasena.Text = "";
            swActivo.Checked = true;

            if (cbRol.Items.Count > 0)
                cbRol.SelectedIndex = 0;

            SetModoEdicion(true);
            txtUsuario.Focus();
        }

        private void Guardar()
        {
            try
            {
                string usuario = (txtUsuario.Text ?? "").Trim();
                string pass = (txtContrasena.Text ?? "").Trim();
                long rolId = Convert.ToInt64(cbRol.SelectedValue ?? 0);
                bool activo = swActivo.Checked;

                if (usuario == "")
                {
                    MessageBox.Show("Escribe un nombre de usuario.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_idSeleccionado == 0 && pass == "")
                {
                    MessageBox.Show("Escribe una contraseña para el nuevo usuario.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var cn = con.CrearConexionAbierta())
                {
                    // Si es nuevo: INSERT
                    if (_idSeleccionado == 0)
                    {
                        // evita duplicados
                        using (var chk = new SqlCommand("SELECT COUNT(*) FROM Usuarios WHERE nombre_usuario=@u", cn))
                        {
                            chk.Parameters.AddWithValue("@u", usuario);
                            int existe = Convert.ToInt32(chk.ExecuteScalar());
                            if (existe > 0)
                            {
                                MessageBox.Show("Ya existe ese usuario.", "Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        string sqlIns = @"
INSERT INTO Usuarios(nombre_usuario, contrasena, rol_id, activo)
VALUES(@u, @p, @rol, @act);";

                        using (var cmd = new SqlCommand(sqlIns, cn))
                        {
                            cmd.Parameters.AddWithValue("@u", usuario);
                            cmd.Parameters.AddWithValue("@p", pass); 
                            cmd.Parameters.AddWithValue("@rol", rolId);
                            cmd.Parameters.AddWithValue("@act", activo ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else // UPDATE
                    {
                        // Si pass viene vacío, no la cambia
                        string sqlUpd = @"
UPDATE Usuarios
SET nombre_usuario=@u,
    rol_id=@rol,
    activo=@act
    " + (pass != "" ? ", contrasena=@p" : "") + @"
WHERE id=@id;";

                        using (var cmd = new SqlCommand(sqlUpd, cn))
                        {
                            cmd.Parameters.AddWithValue("@u", usuario);
                            cmd.Parameters.AddWithValue("@rol", rolId);
                            cmd.Parameters.AddWithValue("@act", activo ? 1 : 0);
                            cmd.Parameters.AddWithValue("@id", _idSeleccionado);
                            if (pass != "") cmd.Parameters.AddWithValue("@p", pass);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Guardado correctamente.", "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarUsuarios();
                Nuevo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar:\n" + ex.Message,
                    "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            SetModoEdicion(false);
        }

        private void ActivarDesactivar()
        {
            try
            {
                if (_idSeleccionado == 0)
                {
                    MessageBox.Show("Selecciona un usuario del listado.", "Usuarios",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("UPDATE Usuarios SET activo = CASE WHEN activo=1 THEN 0 ELSE 1 END WHERE id=@id", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _idSeleccionado);
                    cmd.ExecuteNonQuery();
                }

                CargarUsuarios();
                Nuevo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Usuarios",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFiltros()
        {
            txtBuscar.Text = "";
            if (cbFiltroRol.Items.Count > 0) cbFiltroRol.SelectedIndex = 0;
            CargarUsuarios();
        }

        //  CHART 

        private void InicializarChart()
        {
            chartRoles.Datasets.Clear();
            chartRoles.Legend.Display = true;
            chartRoles.Title.Text = "";
        }

        private void CargarChartRolesActivos()
        {
            try
            {
                chartRoles.Datasets.Clear();

                var ds = new GunaBarDataset
                {
                    Label = "Activos",
                };

                string sql = @"
SELECT r.nombre AS Rol, COUNT(*) AS Cant
FROM Usuarios u
INNER JOIN Roles r ON r.id = u.rol_id
WHERE u.activo = 1
GROUP BY r.nombre
ORDER BY r.nombre;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string rol = rd["Rol"].ToString();
                        double cant = rd["Cant"] == DBNull.Value ? 0 : Convert.ToDouble(rd["Cant"]);
                        ds.DataPoints.Add(new LPoint { Label = rol, Y = cant });
                    }
                }

                chartRoles.Datasets.Add(ds);
                chartRoles.Update();
            }
            catch
            {
            }
        }
    }
}
