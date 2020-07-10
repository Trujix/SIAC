using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace siacWEB.Models
{
    public class MHome : Controller
    {
        // ------------- CLASES GENERALES -------------
        // CLASE DE LOGIN
        public class LoginData
        {
            public string Usuario { get; set; }
            public string Pass { get; set; }
            public string ClaveClinica { get; set; }
        }
        // CLASE DE LOGIN INFO
        public class LoginInfo
        {
            public bool Correcto { get; set; }
            public string TokenUsuario { get; set; }
            public string TokenClinica { get; set; }
            public string NombreUsuario { get; set; }
            public int IdClinica { get; set; }
            public int DuracionSesion { get; set; }
            public Dictionary<string, bool> PerfilUsuario { get; set; }
            public string Error { get; set; }
        }
        // CLASE DE CLINICA INFO
        public class ClinicaInfo
        {
            public int IdClinica { get; set; }
            public string NombreClinica { get; set; }
            public string ClaveClinica { get; set; }
            public string Direccion { get; set; }
            public string CP { get; set; }
            public double Telefono { get; set; }
            public string Colonia { get; set; }
            public string Localidad { get; set; }
            public string EstadoIndx { get; set; }
            public string Estado { get; set; }
            public string MunicipioIndx { get; set; }
            public string Municipio { get; set; }
            public bool LogoPersoalizado { get; set; }
            public string NombreDirector { get; set; }
            public string SiglaLegal { get; set; }
        }
        // CLASE DE USUARIO INFO
        public class UsuarioInfo
        {
            public int IdUsuario { get; set; }
            public string Usuario { get; set; }
            public string TokenUsuario { get; set; }
            public string TokenClinica { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Correo { get; set; }
            public bool Administrador { get; set; }
            public int Activo { get; set; }
            public string Estatus { get; set; }
            public string IdNotificacion { get; set; }
            public string Tipo { get; set; }
            public string Direccion { get; set; }
            public double Telefono { get; set; }
            public double Celular { get; set; }
            public int IdEspecialidad { get; set; }
            public string Especialidad { get; set; }
            public bool ImagenUsuario { get; set; }
            public string Folder { get; set; }
            public string ImgNombre { get; set; }
        }
        // CLASE DE IMAGEN DE USUARIO
        public class ImagenUsuario
        {
            public string IdUsuario { get; set; }
            public string ImgNombre { get; set; }
            public string Base64Codigo { get; set; }
            public bool EstatusImg { get; set; }
        }
        // CLASE PARA CAMBIO DE CONTRASEÑA
        public class UsuarioNuevaPass
        {
            public string NuevaPass { get; set; }
            public string AntiguaPass { get; set; }
        }
        // CLASE DE ALTA DE HORAIRO
        public class HorarioMedicoAlta
        {
            public int IdUsuario { get; set; }
            public string Lunes { get; set; }
            public string Martes { get; set; }
            public string Miercoles { get; set; }
            public string Jueves { get; set; }
            public string Viernes { get; set; }
            public string Sabado { get; set; }
            public string Domingo { get; set; }
        }

        // ARRAY DE PERFILES
        readonly string[] PerfilesArr = {
            "registrarcita", "pagarcita",
            "medicos",
        };
        // ARRAY DE PERFILES [ MODO DE TEXTO - PARA FORMULARIOS ]
        public string[] PerfilesArrTxt =
        {
            "Registrar Cita", "Pagar Cita",
            "Médicos",
        };

        // ::::::::::::::::::::::::: INICIO DE SESION :::::::::::::::::::::::::
        // FUNCION QUE REALIZA EL INICIO DE SESION
        public LoginInfo IniciarSesion(LoginData logindata, string tokenclinica, string tokenusuario)
        {
            LoginInfo LoginVals = new LoginInfo()
            {
                Correcto = false,
                DuracionSesion = 60,
                Error = "errLogin",
            };
            try
            {
                SQL.comandoSQLTrans("IniciarSesion");
                SQL.commandoSQL = new SqlCommand((tokenclinica == "NA") ? "SELECT * FROM dbo.clinica WHERE clave = @QueryParam" : "SELECT * FROM dbo.clinica WHERE token = @QueryParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@QueryParam", SqlDbType.VarChar) { Value = (tokenclinica == "NA") ? logindata.ClaveClinica : tokenclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        LoginVals.IdClinica = int.Parse(lector["id"].ToString());
                        LoginVals.TokenClinica = lector["token"].ToString();
                    }
                }

                bool Administrador = false;
                SQL.commandoSQL = new SqlCommand((tokenusuario == "NA") ? "SELECT * FROM dbo.usuarios WHERE usuario = @UsuarioParam AND pass = @PassParam AND tokenclinica = @TokenClinicaParam" : "SELECT * FROM dbo.usuarios WHERE tokenusuario = @TokenUsuarioParam AND tokenclinica = @TokenClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                if((tokenusuario == "NA"))
                {
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("@UsuarioParam", SqlDbType.VarChar) { Value = logindata.Usuario });
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("@PassParam", SqlDbType.VarChar) { Value = MISC.CrearMD5(logindata.Pass) });
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = LoginVals.TokenClinica });
                }
                else
                {
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenUsuarioParam", SqlDbType.VarChar) { Value = tokenusuario });
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica });

                }
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        LoginVals.Correcto = true;
                        LoginVals.TokenUsuario = lector["tokenusuario"].ToString();
                        LoginVals.NombreUsuario = lector["nombre"].ToString() + " " + lector["apellido"].ToString();
                        Administrador = bool.Parse(lector["administrador"].ToString());
                    }
                }

                if (LoginVals.Correcto)
                {
                    if (Administrador)
                    {
                        Dictionary<string, bool> Perfiles = new Dictionary<string, bool>();
                        foreach(string Perfil in PerfilesArr)
                        {
                            Perfiles[Perfil] = true;
                        }
                        LoginVals.PerfilUsuario = Perfiles;
                    }
                    else
                    {
                        // PROVISIONAL
                        Dictionary<string, bool> Perfiles = new Dictionary<string, bool>();
                        foreach (string Perfil in PerfilesArr)
                        {
                            Perfiles[Perfil] = false;
                        }
                        LoginVals.PerfilUsuario = Perfiles;
                    }
                }

                SQL.transaccionSQL.Commit();
                return LoginVals;
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                LoginVals.Error = e.ToString();
                return LoginVals;
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }
        // ::::::::::::::::::::::::: INICIO DE SESION :::::::::::::::::::::::::

        // FUNCION QUE DEVUELVE LA INFO GENERAL DE LA CLINICA
        public string ClinicaData(int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ClinicaInfo");
                ClinicaInfo Clinica = new ClinicaInfo();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.usuariosclinica WHERE idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Clinica.IdClinica = int.Parse(lector["id"].ToString());
                        Clinica.NombreClinica = lector["nombreclinica"].ToString();
                        Clinica.ClaveClinica = lector["claveclinica"].ToString();
                        Clinica.Direccion = lector["direccion"].ToString();
                        Clinica.CP = lector["cp"].ToString();
                        Clinica.Telefono = double.Parse(lector["telefono"].ToString());
                        Clinica.Colonia = lector["colonia"].ToString();
                        Clinica.Localidad = lector["localidad"].ToString();
                        Clinica.MunicipioIndx = lector["municipioindx"].ToString();
                        Clinica.Municipio = lector["municipio"].ToString();
                        Clinica.Estado = lector["estado"].ToString();
                        Clinica.EstadoIndx = lector["estadoindx"].ToString();
                        Clinica.LogoPersoalizado = bool.Parse(lector["logopersonalizado"].ToString());
                        Clinica.NombreDirector = lector["nombredirector"].ToString();
                        Clinica.SiglaLegal = lector["siglalegal"].ToString();
                    }
                }

                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(Clinica);
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // ::::::::::::::::::: MENU CONFIGURACION DE USUARIOS :::::::::::::::::::
        // FUNCION QUE DEVUELVE LA INFORMACION DEL PANEL DE CONFIGURACION DEL [ USUARIO MEDICO ]
        public string ConfMedicoParams(int idusuario, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ConfMedicoParams");
                Dictionary<string, object> HorariosMedico = new Dictionary<string, object>();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.horariosmedicos WHERE idusuario = @IDUsuarioParam AND idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = idusuario });
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        HorariosMedico = new Dictionary<string, object>()
                        {
                            { "Lunes", lector["lunes"].ToString() },
                            { "Martes", lector["martes"].ToString() },
                            { "Miercoles", lector["miercoles"].ToString() },
                            { "Jueves", lector["jueves"].ToString() },
                            { "Viernes", lector["viernes"].ToString() },
                            { "Sabado", lector["sabado"].ToString() },
                            { "Domingo", lector["domingo"].ToString() },
                        };
                    }
                }

                Dictionary<string, object> Params = new Dictionary<string, object>()
                {
                    { "HorariosMedico", HorariosMedico },
                };
                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(Params);
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // FUNCION QUE GUARDA EL HORARIO DEL MEDICO [ USUARIO  MEDICO ]
        public string EditHorarioMedico(HorarioMedicoAlta horariodata, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("EditHorarioMedico");
                SQL.commandoSQL = new SqlCommand("UPDATE dbo.horariosmedicos SET lunes = @LunesParam, martes = @MartesParam, miercoles = @MiercolesParam, jueves = @JuevesParam, viernes = @ViernesParam, sabado = @SabadoParam, domingo = @DomingoParam WHERE idusuario = @IDUsuarioParam AND idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] actPassUsuario =
                {
                    new SqlParameter("@LunesParam", SqlDbType.VarChar) { Value = horariodata.Lunes },
                    new SqlParameter("@MartesParam", SqlDbType.VarChar) { Value = horariodata.Martes },
                    new SqlParameter("@MiercolesParam", SqlDbType.VarChar) { Value = horariodata.Miercoles },
                    new SqlParameter("@JuevesParam", SqlDbType.VarChar) { Value = horariodata.Jueves },
                    new SqlParameter("@ViernesParam", SqlDbType.VarChar) { Value = horariodata.Viernes },
                    new SqlParameter("@SabadoParam", SqlDbType.VarChar) { Value = horariodata.Sabado },
                    new SqlParameter("@DomingoParam", SqlDbType.VarChar) { Value = horariodata.Domingo },
                    new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = horariodata.IdUsuario },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(actPassUsuario);
                SQL.commandoSQL.ExecuteNonQuery();

                SQL.transaccionSQL.Commit();
                return "true";
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }
        // ::::::::::::::::::: MENU CONFIGURACION DE USUARIOS :::::::::::::::::::

        // ::::::::::::::::::: MENU INFERIOR USUARIO INFO :::::::::::::::::::
        // FUNCION QUE DEVUELVE LA INFO DEL USUARIO
        public string ConUsuarioInfo(string tokenusuario, int idclinica, string tokenclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ConUsuarioInfo");
                UsuarioInfo UsuarioData = new UsuarioInfo() {
                    Folder = tokenclinica,
                };
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.usuarios WHERE tokenusuario = @TokenUsuarioParam AND tokenclinica = (SELECT token FROM dbo.clinica WHERE id = @IDClinicaParam)", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenUsuarioParam", SqlDbType.VarChar) { Value = tokenusuario });
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        UsuarioData.IdUsuario = int.Parse(lector["id"].ToString());
                        UsuarioData.Usuario = lector["usuario"].ToString();
                        UsuarioData.TokenUsuario = lector["tokenusuario"].ToString();
                        UsuarioData.TokenClinica = tokenclinica;
                        UsuarioData.Nombre = lector["nombre"].ToString();
                        UsuarioData.Apellido = lector["apellido"].ToString();
                        UsuarioData.Correo = lector["correo"].ToString();
                        UsuarioData.Administrador = bool.Parse(lector["administrador"].ToString());
                        UsuarioData.Activo = int.Parse(lector["activo"].ToString());
                        UsuarioData.Estatus = lector["estatus"].ToString();
                        UsuarioData.IdNotificacion = lector["idnotificacion"].ToString();
                        UsuarioData.Tipo = lector["tipo"].ToString();

                        UsuarioData.ImgNombre = "U" + lector["id"].ToString() + "_CID" + idclinica.ToString() + ".png";
                    }
                }

                SQL.commandoSQL = new SqlCommand("SELECT UI.*, (SELECT nombre FROM dbo.especialidades WHERE id = UI.idespecialidad) AS Especialidad FROM dbo.usuarioinfo UI WHERE UI.idusuario = @IDUsuarioParam AND UI.idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = UsuarioData.IdUsuario });
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        UsuarioData.Direccion = lector["direccion"].ToString();
                        UsuarioData.Telefono = double.Parse(lector["telefono"].ToString());
                        UsuarioData.Celular = double.Parse(lector["celular"].ToString());
                        UsuarioData.IdEspecialidad = int.Parse(lector["idespecialidad"].ToString());
                        UsuarioData.Especialidad = lector["Especialidad"].ToString();
                        UsuarioData.ImagenUsuario = bool.Parse(lector["imagenusuario"].ToString());
                    }
                }

                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(UsuarioData);
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // FUNCION QUE GUARDA LA INFO DEL USUARIO
        public string GuardarInfoUsuario(UsuarioInfo usuarioinfo, string tokenusuario, string tokenclinica, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("GuardarInfoUsuario");
                int IdUsuario = 0;
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.usuarios WHERE tokenusuario = @TokenUsuarioParam AND tokenclinica = @TokenClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenUsuarioParam", SqlDbType.VarChar) { Value = tokenusuario });
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        IdUsuario = int.Parse(lector["id"].ToString());
                    }
                }

                SQL.commandoSQL = new SqlCommand("UPDATE dbo.usuarios SET nombre = @NombreParam, apellido = @ApellidoParam, correo = @CorreoParam WHERE tokenusuario = @TokenUsuarioParam AND tokenclinica = @TokenClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] usuarioAct =
                {
                    new SqlParameter("@NombreParam", SqlDbType.VarChar) { Value = usuarioinfo.Nombre },
                    new SqlParameter("@ApellidoParam", SqlDbType.VarChar) { Value = usuarioinfo.Apellido },
                    new SqlParameter("@CorreoParam", SqlDbType.VarChar) { Value = usuarioinfo.Correo },
                    new SqlParameter("@TokenUsuarioParam", SqlDbType.VarChar) { Value = tokenusuario },
                    new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(usuarioAct);
                SQL.commandoSQL.ExecuteNonQuery();

                SQL.commandoSQL = new SqlCommand("UPDATE dbo.usuarioinfo SET direccion = @DireccionParam, telefono = @TelefonoParam, celular = @CelularParam WHERE idusuario = @IDUsuarioParam AND idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] infoUsuarioAct =
                {
                    new SqlParameter("@DireccionParam", SqlDbType.VarChar) { Value = usuarioinfo.Direccion },
                    new SqlParameter("@TelefonoParam", SqlDbType.Float) { Value = usuarioinfo.Telefono },
                    new SqlParameter("@CelularParam", SqlDbType.Float) { Value = usuarioinfo.Celular },
                    new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = IdUsuario },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(infoUsuarioAct);
                SQL.commandoSQL.ExecuteNonQuery();

                SQL.transaccionSQL.Commit();
                return "true";
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // FUNCION QUE GUARDA UNA IMAGEN DE USUARIO (CAMBIA ESTATUS)
        public string ImgUsuarioEstatus(ImagenUsuario imginfo, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ImgUsuarioEstatus");
                SQL.commandoSQL = new SqlCommand("UPDATE dbo.usuarioinfo SET imagenusuario = @EstatusParam WHERE idusuario = @IDUsuarioParam AND idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] estatusIgmUsuario =
                {
                    new SqlParameter("@EstatusParam", SqlDbType.Bit) { Value = imginfo.EstatusImg },
                    new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = imginfo.IdUsuario },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(estatusIgmUsuario);
                SQL.commandoSQL.ExecuteNonQuery();

                SQL.transaccionSQL.Commit();
                return "true";
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // FUNCION QUE CAMBIA LA CONTRASEÑA DEL USUARIO
        public string EditUsuarioPass(UsuarioNuevaPass nuevapassdata, string tokenusuario, string tokenclinica)
        {
            try
            {
                SQL.comandoSQLTrans("EditUsuarioPass");
                int IdUsuario = 0; string Respuesta = "";
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.usuarios WHERE tokenusuario = @TokenUsuarioParam AND pass = @PassUsuarioParam AND tokenclinica = @TokenClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenUsuarioParam", SqlDbType.VarChar) { Value = tokenusuario });
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@PassUsuarioParam", SqlDbType.VarChar) { Value = MISC.CrearMD5(nuevapassdata.AntiguaPass) });
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        IdUsuario = int.Parse(lector["id"].ToString());
                    }
                }

                if (IdUsuario > 0)
                {
                    SQL.commandoSQL = new SqlCommand("UPDATE dbo.usuarios SET pass = @PassUsuarioParam WHERE id = @IDUsuarioParam AND tokenclinica = @TokenClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                    SqlParameter[] actPassUsuario =
                    {
                        new SqlParameter("@PassUsuarioParam", SqlDbType.VarChar) { Value = MISC.CrearMD5(nuevapassdata.NuevaPass) },
                        new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = IdUsuario },
                        new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica },
                    };
                    SQL.commandoSQL.Parameters.AddRange(actPassUsuario);
                    SQL.commandoSQL.ExecuteNonQuery();
                    Respuesta = "true";
                }
                else
                {
                    Respuesta = "errLogin";
                }

                SQL.transaccionSQL.Commit();
                return Respuesta;
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }
        // ::::::::::::::::::: MENU INFERIOR USUARIO INFO :::::::::::::::::::
    }
}