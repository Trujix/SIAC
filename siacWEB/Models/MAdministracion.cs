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
    public class MAdministracion : Controller
    {
        // ::::::::::: CLASES Y VARIABLES :::::::::::::
        // ------ MEDICOS
        // CLASE DE ESPECIALIDADES PARA MEDICOS
        // CLASE DE ESPECIALIDADES
        public class EspecialidadesMedicos
        {
            public int IdEspecialidad { get; set; }
            public string Nombre { get; set; }
        }
        // CLASE DE ALTA DE USUARIO [ MULTI ]
        public class UsuarioGRAL
        {
            public string Usuario { get; set; }
            public int IdUsuario { get; set; }
            public int IdMedico { get; set; }
            public int IdEspecialidad { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Correo { get; set; }
            public string NombreCompleto { get; set; }
            public string Consultorio { get; set; }
            public string Tipo { get; set; }
        }
        // CLASE DE RESPUESTA DE ALTA/EDICION DE USUARIO
        public class RespuestaNuevoUsuario
        {
            public int IdUsuario { get; set; }
            public bool Correcto { get; set; }
            public bool Nuevo { get; set; }
            public string Pass { get; set; }
            public string Error { get; set; }
        }

        // ::::::::::::::::::: FUNCIONES GENERALES :::::::::::::::::::
        // ------------------ [ MEDICOS ] ------------------
        // FUNCION QUE DEVUELVE LOS PARAMETROS DEL MODULO DE NUEVO MEDICO
        public string ConMedicoParams(string tokenclinica, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ConMedicoParams");
                List<EspecialidadesMedicos> ListaEspecialidades = new List<EspecialidadesMedicos>();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.especialidades WHERE idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaEspecialidades.Add(new EspecialidadesMedicos()
                        {
                            IdEspecialidad = int.Parse(lector["id"].ToString()),
                            Nombre = lector["nombre"].ToString(),
                        });
                    }
                }

                List<UsuarioGRAL> ListaMedicos = new List<UsuarioGRAL>();
                List<string[]> ListaMedicosTabla = new List<string[]>();
                List<string> ListaIdsUsuarios = new List<string>();
                SQL.commandoSQL = new SqlCommand("SELECT U.*, UI.idespecialidad, UI.consultorio, E.nombre AS NombreEspecialidad FROM dbo.usuarios U JOIN dbo.usuarioinfo UI ON UI.idusuario = U.id JOIN dbo.especialidades E ON E.id = UI.idespecialidad WHERE U.tipo = 'M' AND U.tokenclinica = (SELECT token FROM dbo.clinica WHERE id = @IDClinicaParam)", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaMedicos.Add(new UsuarioGRAL()
                        {
                            IdMedico = int.Parse(lector["id"].ToString()),
                            Nombre = lector["nombre"].ToString(),
                            Apellido = lector["apellido"].ToString(),
                            Correo = lector["correo"].ToString(),
                            Tipo = lector["tipo"].ToString(),
                            IdEspecialidad = int.Parse(lector["idespecialidad"].ToString()),
                            NombreCompleto = lector["nombre"].ToString() + " " + lector["apellido"].ToString(),
                            Consultorio = lector["consultorio"].ToString(),
                            Usuario = lector["usuario"].ToString(),
                        });
                        ListaMedicosTabla.Add(new string[]
                        {
                            lector["nombre"].ToString() + " " + lector["apellido"].ToString(),
                            lector["NombreEspecialidad"].ToString(),
                            lector["consultorio"].ToString(),
                            "<button class='btn badge badge-pill badge-dark' title='Reestablecer Contraseña'" + " onclick='reasignarPassMedico(" + lector["id"].ToString() + ");'><i class='fa fa-envelope'></i></button>",
                        });

                        ListaIdsUsuarios.Add(lector["usuario"].ToString());
                    }
                }

                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    { "Correcto", true },
                    { "Especialidades", ListaEspecialidades },
                    { "Medicos", ListaMedicos },
                    { "MedicosTabla", ListaMedicosTabla },
                    { "UsuariosArray", ListaIdsUsuarios },
                };
                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(Parametros);
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

        // FUNCION QUE GUARDA EL  NUEVO MEDICO
        public RespuestaNuevoUsuario AltaUsuario(UsuarioGRAL usuarioinfo, string tokenusuario, string tokenclinica, int idclinica)
        {
            RespuestaNuevoUsuario Respuesta = new RespuestaNuevoUsuario()
            {
                Correcto = false,
                Nuevo = true,
            };
            try
            {
                SQL.comandoSQLTrans("AltaUsuario");
                if(usuarioinfo.IdMedico == 0 && usuarioinfo.IdUsuario == 0)
                {
                    string nuevaPass = MISC.CrearCadAleatoria(3, 4);
                    SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.usuarios (usuario, tokenusuario, tokenclinica, nombre, apellido, correo, pass, idnotificacion, tipo, fechahora, admusuario) VALUES (@UsuarioParam, @TokenUsuParam, @TokenClinicaParam, @NombreParam, @ApellidoParam, @CorreoParam, @PassParam, @IDNotifParam, @TipoParam, @FechaParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
                    SqlParameter[] nuevoUsuario =
                    {
                        new SqlParameter("@UsuarioParam", SqlDbType.VarChar) { Value = usuarioinfo.Usuario },
                        new SqlParameter("@TokenUsuParam", SqlDbType.VarChar) { Value = MISC.CrearIdSession() },
                        new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica },
                        new SqlParameter("@NombreParam", SqlDbType.VarChar) { Value = usuarioinfo.Nombre },
                        new SqlParameter("@ApellidoParam", SqlDbType.VarChar) { Value = usuarioinfo.Apellido },
                        new SqlParameter("@CorreoParam", SqlDbType.VarChar) { Value = usuarioinfo.Correo },
                        new SqlParameter("@PassParam", SqlDbType.VarChar) { Value = MISC.CrearMD5(nuevaPass) },
                        new SqlParameter("@IDNotifParam", SqlDbType.VarChar) { Value = "--" },
                        new SqlParameter("@TipoParam", SqlDbType.VarChar) { Value = usuarioinfo.Tipo },
                        new SqlParameter("@FechaParam", SqlDbType.DateTime) { Value = MISC.FechaHoy() },
                        new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
                    };
                    SQL.commandoSQL.Parameters.AddRange(nuevoUsuario);
                    SQL.commandoSQL.ExecuteNonQuery();

                    int IdUsuario = 0;
                    SQL.commandoSQL = new SqlCommand("SELECT MAX(id) AS Maximo FROM dbo.usuarios WHERE tokenclinica = @TokenClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("@TokenClinicaParam", SqlDbType.VarChar) { Value = tokenclinica });
                    using (var lector = SQL.commandoSQL.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            IdUsuario = int.Parse(lector["Maximo"].ToString());
                        }
                    }

                    SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.usuarioinfo (idusuario, idclinica, idespecialidad, consultorio, fechahora, admusuario) VALUES (@IDUsuarioParam, @IDClinicaParam, @IDEspecialidadParam, @ConsultorioParam, @FechaParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
                    SqlParameter[] nuevoUsuarioInfo =
                    {
                        new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = IdUsuario },
                        new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                        new SqlParameter("@IDEspecialidadParam", SqlDbType.Int) { Value = (usuarioinfo.Tipo == "M") ? usuarioinfo.IdEspecialidad : 0 },
                        new SqlParameter("@ConsultorioParam", SqlDbType.VarChar) { Value = (usuarioinfo.Tipo == "M") ? usuarioinfo.Consultorio : "--" },
                        new SqlParameter("@FechaParam", SqlDbType.DateTime) { Value = MISC.FechaHoy() },
                        new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
                    };
                    SQL.commandoSQL.Parameters.AddRange(nuevoUsuarioInfo);
                    SQL.commandoSQL.ExecuteNonQuery();

                    if (usuarioinfo.Tipo == "M")
                    {
                        SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.horariosmedicos (idusuario, idclinica, fechahora, admusuario) VALUES (@IDUsuarioParam, @IDClinicaParam, @FechaParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
                        SqlParameter[] medicoHorario =
                        {
                        new SqlParameter("@IDUsuarioParam", SqlDbType.Int) { Value = IdUsuario },
                        new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                        new SqlParameter("@FechaParam", SqlDbType.DateTime) { Value = MISC.FechaHoy() },
                        new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
                    };
                        SQL.commandoSQL.Parameters.AddRange(medicoHorario);
                        SQL.commandoSQL.ExecuteNonQuery();
                    }

                        Respuesta.Pass = nuevaPass;
                    Respuesta.IdUsuario = IdUsuario;
                }
                else
                {
                    Respuesta.Nuevo = false;                    
                }

                SQL.transaccionSQL.Commit();
                Respuesta.Correcto = true;
                return Respuesta;
            }
            catch (Exception e)
            {
                Respuesta.Error = e.ToString();
                SQL.transaccionSQL.Rollback();
                return Respuesta;
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }
        // ------------------ [ MEDICOS ] ------------------
    }
}