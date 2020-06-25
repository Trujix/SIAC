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

        // ------------------ FUNCIONES ------------------
        // FUNCION QUE INICIA SESION
        public string IniciarSesion(MHome.LoginData LoginData)
        {
            if(LoginData.Usuario == "adm" && LoginData.Pass == "123")
            {
                MISC.CrearCookie("1a2b3c", 60);
                return "true";
            }
            else
            {
                return "false";
            }
        }

        // FUNCION QUE CIERRA LA SESION
        public string CerrarSesion()
        {
            try
            {
                Response.Cookies["usuariodata"].Expires = DateTime.Now.AddDays(-1);
                return "true";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}