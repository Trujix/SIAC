using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using siacWEB.Models;
using Newtonsoft.Json;

namespace siacWEB.Controllers
{
    public class ConsultasController : Controller
    {
        // ::::::::::::::: CLASES Y VARIABLES :::::::::::::::
        MConsultas MiConsulta = new MConsultas();

        // ::::::::::::::: VISTAS :::::::::::::::
        // FUNCION QUE DEVUELVE LA VISTA DEL REGISTRO A UNA CONSULTA MEDICA
        public ActionResult RegistrarConsulta()
        {
            if ((bool)Session["registroconsulta"])
            {
                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "Home");
            }
        }

        // ::::::::::::::: FUNCIONES GENERALES :::::::::::::::

        // ------------ REGISTRO DE CONSULTAS ------------
        // FUNCION QUE DEVUELVE LOS PARAMETROS DE INICIO DEL MENU  DE REGISTRO DE CONSULTAS
        public string ParamsRegistroConsultas()
        {
            bool vs = MISC.VerifSesion();
            return MiConsulta.ParamsRegistroConsultas((int)Session["IdClinica"]);
        }

        // FUNCION QUE DEVUELVE LA LISTA DE CITAS
        public string ConsCitas()
        {
            bool vs = MISC.VerifSesion();
            return MiConsulta.ConsCitas((int)Session["IdClinica"]);
        }

        // FUNCION QUE DEVUELVE LA LISTA DE CITAS DE UN MEDICO EN UNA FECHA RESPECTIVA
        public string ConsCitasMedicoFecha(MConsultas.CitasRegistros CitasInfo)
        {
            bool vs = MISC.VerifSesion();
            return MiConsulta.ConsCitasMedicoFecha(CitasInfo, (int)Session["IdClinica"]);
        }

        // FUNCION QUE GUARA UNA CITA
        public string AltaCita(MConsultas.CitasRegistros CitaInfo)
        {
            bool vs = MISC.VerifSesion();
            string Alta = MiConsulta.AltaCita(CitaInfo, (int)Session["IdClinica"], MISC.TokenUsuario());
            Dictionary<string, object> NuevaCita = new Dictionary<string, object>()
            {
                { "AltaCita", Alta },
                { "MailCita", (CitaInfo.Correo != "--" && Alta == "true") ? MISC.EnviarMail(0, CitaInfo.Correo, new string[] { CitaInfo.NombrePaciente, CitaInfo.FechaHoraCitaTxt, CitaInfo.FechaCita.ToString("dddd, dd MMMM").ToUpper(), CitaInfo.FechaCitaTxt }) : "true" },
            };
            return JsonConvert.SerializeObject(NuevaCita);
        }
        // ------------ REGISTRO DE CONSULTAS ------------
    }
}