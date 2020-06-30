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

        // ARRAY DE PERFILES
        readonly string[] PerfilesArr = {
            "registroconsulta",
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
    }
}