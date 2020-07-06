using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using siacWEB.Models;
using Newtonsoft.Json;

namespace siacWEB.Controllers
{
    public class HomeController : Controller
    {
        // -------------- VARIABLES GLOBALES ---------------
        MHome MiHome = new MHome();

        // ------------------ VISTAS ------------------
        // FUNCION QUE DEVUELVE LA VISTA PRINCIPAL
        public ActionResult Index()
        {
            if (MISC.VerifSesion())
            {
                return RedirectToAction("Menu");
            }
            else
            {
                return View();
            }
        }

        // FUNCION QUE DEVUELVELA VISTA PRINCIPAL
        public ActionResult Menu()
        {
            if (MISC.VerifSesion())
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // FUNCION QUE DEVUELVE  LA VISTA SIN PERMISO
        public ActionResult SinPermiso()
        {
            return View();
        }

        // ------------ VISTAS DE MENU INFERIOR --------
        // FUNCION QUE DEVUELVE LA VISTA DE INFO DE USUARIO
        public ActionResult UsuarioInfo()
        {
            return View();
        }

        // ------------------ FUNCIONES ------------------
        // FUNCION QUE INICIA SESION
        public string IniciarSesion(MHome.LoginData LoginData)
        {
            Dictionary<string, object> Respuesta = new Dictionary<string, object>();
            MHome.LoginInfo Login = MiHome.IniciarSesion(LoginData, "NA", "NA");
            if (Login.Correcto)
            {
                MISC.CrearParamsSesion(Login.TokenUsuario, Login.TokenClinica, Login.IdClinica, Login.PerfilUsuario, Login.NombreUsuario, Login.DuracionSesion);
                Respuesta = new Dictionary<string, object>() {
                    { "Respuesta", true },
                };
                return JsonConvert.SerializeObject(Respuesta);
            }
            else
            {
                Respuesta = new Dictionary<string, object>() {
                    { "Respuesta", false },
                    { "Error", Login.Error },
                };
                return JsonConvert.SerializeObject(Respuesta);
            }
        }

        // FUNCION QUE CIERRA LA SESION
        public string CerrarSesion()
        {
            try
            {
                Response.Cookies["usuariodata"].Expires = DateTime.Now.AddDays(-1);
                Session.Clear();
                Session.Abandon();
                return "true";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        // FUNCION QUE DEVUELVE LOS PARAMETROS DE USUARIO
        public string UsuarioParams()
        {
            try
            {
                if (MISC.VerifSesion())
                {
                    Dictionary<string, object> UsuarioInfo = new Dictionary<string, object>()
                    {
                        { "NombreUsuario", Session["NombreUsuario"] },
                    };
                    return JsonConvert.SerializeObject(UsuarioInfo);
                }
                else
                {
                    return "error";
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        // ::::::::::::::::::: MENU INFERIOR USUARIO INFO :::::::::::::::::::
        // FUNCION QUE DEVUELVE LA INFO DEL USUARIO
        public string ConUsuarioInfo()
        {
            bool vs = MISC.VerifSesion();
            return MiHome.ConUsuarioInfo(MISC.TokenUsuario(), (int)Session["IdClinica"]);
        }
        // ::::::::::::::::::: MENU INFERIOR USUARIO INFO :::::::::::::::::::
    }
}