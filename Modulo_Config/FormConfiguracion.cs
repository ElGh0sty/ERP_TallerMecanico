using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PROYECTOMECANICO
{
    public partial class FormConfiguracion : Form
    {
        private readonly Conexion con = new Conexion();

        //  Impuestos 
        private long _impId = 0;

        //  MétodosPago 
        private long _mpId = 0;

        //  Secuenciales 
        private int _secId = 0;

        public FormConfiguracion()
        {
            InitializeComponent();

            this.Load += FormConfiguracion_Load;

            // Impuestos
            btnImpBuscar.Click += (s, e) => Imp_Cargar();
            btnImpLimpiar.Click += (s, e) => { txtImpBuscar.Text = ""; Imp_Cargar(); };
            btnImpNuevo.Click += (s, e) => Imp_Nuevo();
            btnImpGuardar.Click += (s, e) => Imp_Guardar();
            btnImpActivarDesactivar.Click += (s, e) => Imp_ActivarDesactivar();
            dgvImpuestos.CellClick += Imp_GridClick;

            // Métodos pago
            btnMpBuscar.Click += (s, e) => MP_Cargar();
            btnMpLimpiar.Click += (s, e) => { txtMpBuscar.Text = ""; MP_Cargar(); };
            btnMpNuevo.Click += (s, e) => MP_Nuevo();
            btnMpGuardar.Click += (s, e) => MP_Guardar();
            btnMpActivarDesactivar.Click += (s, e) => MP_ActivarDesactivar();
            dgvMetodosPago.CellClick += MP_GridClick;

            // Empresa
            btnEmpGuardar.Click += (s, e) => Emp_Guardar();

            // Secuenciales
            btnSecBuscar.Click += (s, e) => Sec_Cargar();
            btnSecLimpiar.Click += (s, e) => { txtSecBuscar.Text = ""; Sec_Cargar(); };
            btnSecNuevo.Click += (s, e) => Sec_Nuevo();
            btnSecGuardar.Click += (s, e) => Sec_Guardar();
            btnSecEliminar.Click += (s, e) => Sec_Eliminar();
            dgvSecuenciales.CellClick += Sec_GridClick;
        }

        private void FormConfiguracion_Load(object sender, EventArgs e)
        {
            

            // estilo grids
            GridStyle(dgvImpuestos);
            GridStyle(dgvMetodosPago);
            GridStyle(dgvSecuenciales);

            // defaults
            nudImpPorcentaje.DecimalPlaces = 2;
            nudImpPorcentaje.Minimum = 0;
            nudImpPorcentaje.Maximum = 100;
            nudImpPorcentaje.Increment = 0.5M;

            nudSecActual.Minimum = 0;
            nudSecActual.Maximum = 999999999;

            // cargar todo
            Imp_SetModo(false);
            MP_SetModo(false);
            Sec_SetModo(false);

            Imp_Cargar();
            MP_Cargar();
            Emp_Cargar();
            Sec_Cargar();
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

        // IMPUESTOS

        private void Imp_SetModo(bool activo)
        {
            txtImpNombre.Enabled = activo;
            nudImpPorcentaje.Enabled = activo;
            txtImpCodigoSri.Enabled = activo;
            swImpActivo.Enabled = activo;
            btnImpGuardar.Enabled = activo;
            btnImpActivarDesactivar.Enabled = (_impId != 0);
        }

        private void Imp_LimpiarCampos()
        {
            txtImpNombre.Text = "";
            nudImpPorcentaje.Value = 0;
            txtImpCodigoSri.Text = "";
            swImpActivo.Checked = true;
            _impId = 0;
        }

        private void Imp_Nuevo()
        {
            Imp_LimpiarCampos();
            Imp_SetModo(true);
            btnImpActivarDesactivar.Enabled = false;
            txtImpNombre.Focus();
        }

        private void Imp_Cargar()
        {
            try
            {
                string buscar = (txtImpBuscar.Text ?? "").Trim();

                string sql = @"
SELECT id AS Id, nombre AS Nombre, porcentaje AS Porcentaje, codigo_sri AS CodigoSRI, ISNULL(activo,1) AS Activo
FROM Impuestos
WHERE (@b='' OR nombre LIKE '%' + @b + '%' OR codigo_sri LIKE '%' + @b + '%')
ORDER BY id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvImpuestos.DataSource = dt;
                }

                if (dgvImpuestos.Columns["Id"] != null) dgvImpuestos.Columns["Id"].Visible = false;
                if (dgvImpuestos.Columns["Porcentaje"] != null) dgvImpuestos.Columns["Porcentaje"].DefaultCellStyle.Format = "N2";

                Imp_SetModo(false);
                Imp_LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar impuestos:\n" + ex.Message, "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Imp_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvImpuestos.Rows[e.RowIndex];

            _impId = Convert.ToInt64(row.Cells["Id"].Value);
            txtImpNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
            txtImpCodigoSri.Text = row.Cells["CodigoSRI"].Value?.ToString() ?? "";
            nudImpPorcentaje.Value = Convert.ToDecimal(row.Cells["Porcentaje"].Value ?? 0);
            swImpActivo.Checked = Convert.ToBoolean(row.Cells["Activo"].Value);

            Imp_SetModo(true);
        }

        private void Imp_Guardar()
        {
            try
            {
                string nombre = (txtImpNombre.Text ?? "").Trim();
                string codigo = (txtImpCodigoSri.Text ?? "").Trim();
                decimal porcentaje = nudImpPorcentaje.Value;
                bool activo = swImpActivo.Checked;

                if (nombre == "")
                {
                    MessageBox.Show("El nombre es obligatorio.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var cn = con.CrearConexionAbierta())
                {
                    string dup = @"SELECT COUNT(*) FROM Impuestos WHERE (nombre=@n OR codigo_sri=@c) AND (@id=0 OR id<>@id);";
                    using (var chk = new SqlCommand(dup, cn))
                    {
                        chk.Parameters.AddWithValue("@n", nombre);
                        chk.Parameters.AddWithValue("@c", codigo);
                        chk.Parameters.AddWithValue("@id", _impId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe un impuesto con ese nombre o código.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_impId == 0)
                    {
                        string ins = @"INSERT INTO Impuestos(nombre,porcentaje,codigo_sri,activo) VALUES(@n,@p,@c,@a);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@p", porcentaje);
                            cmd.Parameters.AddWithValue("@c", codigo);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"UPDATE Impuestos SET nombre=@n,porcentaje=@p,codigo_sri=@c,activo=@a WHERE id=@id;";
                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@p", porcentaje);
                            cmd.Parameters.AddWithValue("@c", codigo);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.Parameters.AddWithValue("@id", _impId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Guardado correctamente.", "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Imp_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Imp_ActivarDesactivar()
        {
            try
            {
                if (_impId == 0) return;

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(
                    "UPDATE Impuestos SET activo = CASE WHEN ISNULL(activo,1)=1 THEN 0 ELSE 1 END WHERE id=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _impId);
                    cmd.ExecuteNonQuery();
                }

                Imp_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Impuestos",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // METODOS PAGO

        private void MP_SetModo(bool activo)
        {
            txtMpCodigoSri.Enabled = activo;
            txtMpNombre.Enabled = activo;
            swMpActivo.Enabled = activo;
            btnMpGuardar.Enabled = activo;
            btnMpActivarDesactivar.Enabled = (_mpId != 0);
        }

        private void MP_LimpiarCampos()
        {
            txtMpCodigoSri.Text = "";
            txtMpNombre.Text = "";
            swMpActivo.Checked = true;
            _mpId = 0;
        }

        private void MP_Nuevo()
        {
            MP_LimpiarCampos();
            MP_SetModo(true);
            btnMpActivarDesactivar.Enabled = false;
            txtMpCodigoSri.Focus();
        }

        private void MP_Cargar()
        {
            try
            {
                string buscar = (txtMpBuscar.Text ?? "").Trim();

                string sql = @"
SELECT id AS Id, codigo_sri AS CodigoSRI, nombre AS Nombre, ISNULL(activo,1) AS Activo
FROM MetodosPago
WHERE (@b='' OR nombre LIKE '%' + @b + '%' OR codigo_sri LIKE '%' + @b + '%')
ORDER BY id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvMetodosPago.DataSource = dt;
                }

                if (dgvMetodosPago.Columns["Id"] != null) dgvMetodosPago.Columns["Id"].Visible = false;

                MP_SetModo(false);
                MP_LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar métodos de pago:\n" + ex.Message, "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MP_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvMetodosPago.Rows[e.RowIndex];

            _mpId = Convert.ToInt64(row.Cells["Id"].Value);
            txtMpCodigoSri.Text = row.Cells["CodigoSRI"].Value?.ToString() ?? "";
            txtMpNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
            swMpActivo.Checked = Convert.ToBoolean(row.Cells["Activo"].Value);

            MP_SetModo(true);
        }

        private void MP_Guardar()
        {
            try
            {
                string cod = (txtMpCodigoSri.Text ?? "").Trim();
                string nom = (txtMpNombre.Text ?? "").Trim();
                bool activo = swMpActivo.Checked;

                if (cod.Length != 2)
                {
                    MessageBox.Show("El código SRI debe tener exactamente 2 caracteres.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (nom == "")
                {
                    MessageBox.Show("El nombre es obligatorio.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var cn = con.CrearConexionAbierta())
                {
                    string dup = @"SELECT COUNT(*) FROM MetodosPago WHERE codigo_sri=@c AND (@id=0 OR id<>@id);";
                    using (var chk = new SqlCommand(dup, cn))
                    {
                        chk.Parameters.AddWithValue("@c", cod);
                        chk.Parameters.AddWithValue("@id", _mpId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe un método con ese código SRI.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_mpId == 0)
                    {
                        string ins = @"INSERT INTO MetodosPago(codigo_sri,nombre,activo) VALUES(@c,@n,@a);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@c", cod);
                            cmd.Parameters.AddWithValue("@n", nom);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"UPDATE MetodosPago SET codigo_sri=@c,nombre=@n,activo=@a WHERE id=@id;";
                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@c", cod);
                            cmd.Parameters.AddWithValue("@n", nom);
                            cmd.Parameters.AddWithValue("@a", activo ? 1 : 0);
                            cmd.Parameters.AddWithValue("@id", _mpId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Guardado correctamente.", "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MP_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MP_ActivarDesactivar()
        {
            try
            {
                if (_mpId == 0) return;

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(
                    "UPDATE MetodosPago SET activo = CASE WHEN ISNULL(activo,1)=1 THEN 0 ELSE 1 END WHERE id=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _mpId);
                    cmd.ExecuteNonQuery();
                }

                MP_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Métodos de pago",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // EMPRESA

        private void Emp_Cargar()
        {
            try
            {
                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("SELECT TOP 1 id,nombre,ruc,direccion,telefono,email FROM Empresa ORDER BY id;", cn))
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtEmpNombre.Text = rd["nombre"]?.ToString() ?? "";
                        txtEmpRuc.Text = rd["ruc"]?.ToString() ?? "";
                        txtEmpDireccion.Text = rd["direccion"]?.ToString() ?? "";
                        txtEmpTelefono.Text = rd["telefono"]?.ToString() ?? "";
                        txtEmpEmail.Text = rd["email"]?.ToString() ?? "";
                    }
                    else
                    {
                        // vacío -> deja campos en blanco
                        txtEmpNombre.Text = "";
                        txtEmpRuc.Text = "";
                        txtEmpDireccion.Text = "";
                        txtEmpTelefono.Text = "";
                        txtEmpEmail.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empresa:\n" + ex.Message, "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Emp_Guardar()
        {
            try
            {
                string nombre = (txtEmpNombre.Text ?? "").Trim();
                string ruc = (txtEmpRuc.Text ?? "").Trim();
                string dir = (txtEmpDireccion.Text ?? "").Trim();
                string tel = (txtEmpTelefono.Text ?? "").Trim();
                string email = (txtEmpEmail.Text ?? "").Trim();

                if (nombre == "")
                {
                    MessageBox.Show("El nombre del taller/empresa es obligatorio.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var cn = con.CrearConexionAbierta())
                {
                    // existe fila?
                    int existe;
                    using (var chk = new SqlCommand("SELECT COUNT(*) FROM Empresa;", cn))
                        existe = Convert.ToInt32(chk.ExecuteScalar());

                    if (existe == 0)
                    {
                        string ins = @"INSERT INTO Empresa(nombre,ruc,direccion,telefono,email) VALUES(@n,@r,@d,@t,@e);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@r", (object)ruc ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@d", (object)dir ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@t", (object)tel ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@e", (object)email ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // actualiza la primera fila
                        string upd = @"
UPDATE Empresa
SET nombre=@n, ruc=@r, direccion=@d, telefono=@t, email=@e
WHERE id = (SELECT TOP 1 id FROM Empresa ORDER BY id);";

                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@n", nombre);
                            cmd.Parameters.AddWithValue("@r", (object)ruc ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@d", (object)dir ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@t", (object)tel ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@e", (object)email ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Empresa actualizada.", "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar empresa:\n" + ex.Message, "Empresa",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // SECUENCIALES

        private void Sec_SetModo(bool activo)
        {
            txtSecTipoDoc.Enabled = activo;
            txtSecEstablecimiento.Enabled = activo;
            txtSecPuntoEmision.Enabled = activo;
            nudSecActual.Enabled = activo;

            btnSecGuardar.Enabled = activo;
            btnSecEliminar.Enabled = (_secId != 0);
        }

        private void Sec_LimpiarCampos()
        {
            _secId = 0;
            txtSecTipoDoc.Text = "";
            txtSecEstablecimiento.Text = "";
            txtSecPuntoEmision.Text = "";
            nudSecActual.Value = 0;
        }

        private void Sec_Nuevo()
        {
            Sec_LimpiarCampos();
            Sec_SetModo(true);
            btnSecEliminar.Enabled = false;
            txtSecTipoDoc.Focus();
        }

        private void Sec_Cargar()
        {
            try
            {
                string buscar = (txtSecBuscar.Text ?? "").Trim();

                string sql = @"
SELECT id AS Id, tipo_documento AS TipoDocumento, establecimiento AS Establecimiento, punto_emision AS PuntoEmision, secuencia_actual AS SecuenciaActual
FROM Secuenciales
WHERE (@b='' OR tipo_documento LIKE '%' + @b + '%')
ORDER BY id DESC;";

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@b", buscar);
                    var dt = new DataTable();
                    new SqlDataAdapter(cmd).Fill(dt);
                    dgvSecuenciales.DataSource = dt;
                }

                if (dgvSecuenciales.Columns["Id"] != null) dgvSecuenciales.Columns["Id"].Visible = false;

                Sec_SetModo(false);
                Sec_LimpiarCampos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar secuenciales:\n" + ex.Message, "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Sec_GridClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvSecuenciales.Rows[e.RowIndex];

            _secId = Convert.ToInt32(row.Cells["Id"].Value);
            txtSecTipoDoc.Text = row.Cells["TipoDocumento"].Value?.ToString() ?? "";
            txtSecEstablecimiento.Text = row.Cells["Establecimiento"].Value?.ToString() ?? "";
            txtSecPuntoEmision.Text = row.Cells["PuntoEmision"].Value?.ToString() ?? "";
            nudSecActual.Value = Convert.ToDecimal(row.Cells["SecuenciaActual"].Value ?? 0);

            Sec_SetModo(true);
        }

        private void Sec_Guardar()
        {
            try
            {
                string tipo = (txtSecTipoDoc.Text ?? "").Trim().ToUpper();
                string est = (txtSecEstablecimiento.Text ?? "").Trim();
                string pto = (txtSecPuntoEmision.Text ?? "").Trim();
                int sec = Convert.ToInt32(nudSecActual.Value);

                if (tipo == "")
                {
                    MessageBox.Show("Tipo de documento es obligatorio (ej: FACTURA).", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (est.Length != 3 || pto.Length != 3)
                {
                    MessageBox.Show("Establecimiento y Punto de emisión deben tener 3 dígitos (ej: 001).", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var cn = con.CrearConexionAbierta())
                {
                    // evitar duplicados (tipo+est+pto)
                    string dup = @"
SELECT COUNT(*) FROM Secuenciales
WHERE tipo_documento=@t AND establecimiento=@e AND punto_emision=@p
AND (@id=0 OR id<>@id);";

                    using (var chk = new SqlCommand(dup, cn))
                    {
                        chk.Parameters.AddWithValue("@t", tipo);
                        chk.Parameters.AddWithValue("@e", est);
                        chk.Parameters.AddWithValue("@p", pto);
                        chk.Parameters.AddWithValue("@id", _secId);
                        if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Ya existe un secuencial para ese documento/establecimiento/punto.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (_secId == 0)
                    {
                        string ins = @"INSERT INTO Secuenciales(tipo_documento,establecimiento,punto_emision,secuencia_actual)
                                       VALUES(@t,@e,@p,@s);";
                        using (var cmd = new SqlCommand(ins, cn))
                        {
                            cmd.Parameters.AddWithValue("@t", tipo);
                            cmd.Parameters.AddWithValue("@e", est);
                            cmd.Parameters.AddWithValue("@p", pto);
                            cmd.Parameters.AddWithValue("@s", sec);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string upd = @"UPDATE Secuenciales SET tipo_documento=@t,establecimiento=@e,punto_emision=@p,secuencia_actual=@s
                                       WHERE id=@id;";
                        using (var cmd = new SqlCommand(upd, cn))
                        {
                            cmd.Parameters.AddWithValue("@t", tipo);
                            cmd.Parameters.AddWithValue("@e", est);
                            cmd.Parameters.AddWithValue("@p", pto);
                            cmd.Parameters.AddWithValue("@s", sec);
                            cmd.Parameters.AddWithValue("@id", _secId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Secuencial guardado.", "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Sec_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Sec_Eliminar()
        {
            try
            {
                if (_secId == 0) return;

                var r = MessageBox.Show("¿Eliminar este secuencial?", "Confirmar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (r != DialogResult.Yes) return;

                using (var cn = con.CrearConexionAbierta())
                using (var cmd = new SqlCommand("DELETE FROM Secuenciales WHERE id=@id;", cn))
                {
                    cmd.Parameters.AddWithValue("@id", _secId);
                    cmd.ExecuteNonQuery();
                }

                Sec_Cargar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Secuenciales",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}