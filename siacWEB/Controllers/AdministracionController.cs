using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using siacWEB.Models;
using Newtonsoft.Json;

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

        // --------------- [ FUNCION MAESTRA DE ALTA DE USUARIO ] ---------------
        // FUNCION QUE GUARDA EL  NUEVO USUARIO
        public string AltaUsuario(MAdministracion.UsuarioGRAL UsuarioInfo)
        {
            bool vs = MISC.VerifSesion();
            MAdministracion.RespuestaNuevoUsuario Alta = MiAdministracion.AltaUsuario(UsuarioInfo, MISC.TokenUsuario(), MISC.TokenClinica(), (int)Session["IdClinica"]);
            Dictionary<string, object> Respuesta = new Dictionary<string, object>()
            {
                { "AltaUsuario", (Alta.Correcto) ? "true" : Alta.Error },
                { "MailUsuario", (Alta.Correcto && Alta.Nuevo) ? MISC.EnviarMail(1, UsuarioInfo.Correo, new string[] { UsuarioInfo.Nombre + " " + UsuarioInfo.Apellido, UsuarioInfo.Usuario, Alta.Pass }) : "true" },
                { "NuevoUsuario", Alta.Nuevo },
                { "IdUsuario", Alta.IdUsuario },
            };
            return JsonConvert.SerializeObject(Respuesta);
        }

        // --------------------- [ MEDICOS ] ---------------------
        // FUNCION QUE DEVUELVE LOS PARAMETROS INICIALES DEL MODULO DE MEDICOS
        public string ConMedicoParams()
        {
            bool vs = MISC.VerifSesion();
            return MiAdministracion.ConMedicoParams(MISC.TokenClinica(), (int)Session["IdClinica"]);
        }
        // --------------------- [ MEDICOS ] ---------------------
    }
}