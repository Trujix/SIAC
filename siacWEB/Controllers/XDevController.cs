using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Mvc;
using siacWEB.Models;
using Ionic.Zip;

namespace siacWEB.Controllers
{
    public class XDevController : Controller
    {
        // ::::::::::: CLASES Y VARIABLES :::::::::::
        MXDev MiXDev = new MXDev();
        public static string Key = "23a561f94af1886aa690530cd3b75fb2";
        // CLASE CON PERMISOS ESPECIALES
        public class PermisosEspeciales
        {
            public string Usuario { get; set; }
            public string Password { get; set; }
            public bool AccesoEsp { get; set; }
        }

        // ::::::::::: VISTA PRINCIPAL DEL GESTOR ::::::::::: 
        public ActionResult Index()
        {
            return View();
        }

        // ::::::::::: FUNCIONES GENERALES :::::::::::
        // FUNCION QUE VALIDA LA CLAVE DE ACCESO AL PANEL
        public string ValidarKey(string Clave)
        {
            if(MISC.CrearMD5(Clave) == Key)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }

        // FUNCION QUE DEVUELVE EL ESQUEMA DE LA BD
        public string EsquemaBD()
        {
            return MiXDev.EsquemaBD();
        }

        // FUNCION QUE EJECUTA UNA QUERY
        public string QueryBD(string Query, bool Consulta)
        {
            return MiXDev.QueryBD(Query, Consulta);
        }

        // FUNCION QUE ACTUALIZA LA APLICACION
        public string ActualizarApp(string URL, PermisosEspeciales Credenciales)
        {
            try
            {
                using (WebClient instalador = new WebClient())
                {
                    if (Credenciales.AccesoEsp)
                        instalador.Credentials = new NetworkCredential(Credenciales.Usuario, Credenciales.Password);

                    instalador.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                    instalador.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                    instalador.DownloadFile(new Uri(URL), Server.MapPath("~/Instalador.zip"));
                }
                using (ZipFile actualizador = ZipFile.Read(Server.MapPath("~/Instalador.zip")))
                {
                    foreach (ZipEntry archivo in actualizador)
                    {
                        archivo.Extract(Server.MapPath("~"), ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                System.IO.File.Delete(Server.MapPath("~/Instalador.zip"));
                return "true";
            }
            catch (Exception e)
            {
                System.IO.File.Delete(Server.MapPath("~/Instalador.zip"));
                return e.ToString();
            }
        }
    }
}