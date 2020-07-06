using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using siacWEB.Models;

namespace siacWEB.Controllers
{
    public class AdministracionController : Controller
    {
        // ::::::::::::::::: CLASES Y VARIABLES :::::::::::::::::
        MAdministracion MiAdministracion = new MAdministracion();

        // ::::::::::::::::: VISTAS :::::::::::::::::
        // FUNCION QUE DEVUELVE LA VISTA DE MEDICOS
        public ActionResult Medicos()
        {
            if ((bool)Session["medicos"])
            {
                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "Home");
            }
        }

        // --------------------- [ MEDICOS ] ---------------------
        // --------------------- [ MEDICOS ] ---------------------
    }
}