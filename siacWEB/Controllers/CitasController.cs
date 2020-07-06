using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using siacWEB.Models;
using Newtonsoft.Json;

namespace siacWEB.Controllers
{
    public class CitasController : Controller
    {
        // ::::::::::::::: CLASES Y VARIABLES :::::::::::::::
        MCitas MiCita = new MCitas();

        // ::::::::::::::: VISTAS :::::::::::::::
        // FUNCION QUE DEVUELVE LA VISTA DEL REGISTRO A UNA CITA MEDICA [ CITAS MEDICAS ]
        public ActionResult RegistrarCita()
        {
            if ((bool)Session["registrarcita"])
            {
                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "Home");
            }
        }

        // FUNCION QUE DEVUELVE LA VISTA PARA PAGAR CITA MEDICA [ PAGO DE CITAS ]
        public ActionResult PagarCita()
        {
            if ((bool)Session["pagarcita"])
            {
                return View();
            }
            else
            {
                return RedirectToAction("SinPermiso", "Home");
            }
        }

        // ::::::::::::::: FUNCIONES GENERALES :::::::::::::::

        // ------------ REGISTRO DE CITAS MEDICAS ------------
        // FUNCION QUE DEVUELVE LOS PARAMETROS DE INICIO DEL MENU  DE REGISTRO DE CITAS
        public string ParamsRegistroCitas()
        {
            bool vs = MISC.VerifSesion();
            return MiCita.ParamsRegistroCitas((int)Session["IdClinica"]);
        }

        // FUNCION QUE DEVUELVE LA LISTA DE CITAS
        public string ConsCitas()
        {
            bool vs = MISC.VerifSesion();
            return MiCita.ConsCitas((int)Session["IdClinica"]);
        }

        // FUNCION QUE DEVUELVE LA LISTA DE CITAS DE UN MEDICO EN UNA FECHA RESPECTIVA
        public string ConsCitasMedicoFecha(MCitas.CitasRegistros CitasInfo)
        {
            bool vs = MISC.VerifSesion();
            return MiCita.ConsCitasMedicoFecha(CitasInfo, (int)Session["IdClinica"]);
        }

        // FUNCION QUE GUARA UNA CITA
        public string AltaCita(MCitas.CitasRegistros CitaInfo)
        {
            bool vs = MISC.VerifSesion();
            string Alta = MiCita.AltaCita(CitaInfo, (int)Session["IdClinica"], MISC.TokenUsuario());
            Dictionary<string, object> NuevaCita = new Dictionary<string, object>()
            {
                { "AltaCita", Alta },
                { "MailCita", (CitaInfo.Correo != "--" && Alta == "true") ? MISC.EnviarMail(0, CitaInfo.Correo, new string[] { CitaInfo.NombrePaciente, CitaInfo.FechaHoraCitaTxt, CitaInfo.FechaCita.ToString("dddd, dd MMMM").ToUpper(), CitaInfo.FechaCitaTxt }) : "true" },
            };
            return JsonConvert.SerializeObject(NuevaCita);
        }

        // FUNCION QUE REENVIA EL MAIL DE UNA CITA
        public string ReenviarMailCita(MCitas.CitasRegistros CitaInfo)
        {
            return MISC.EnviarMail(0, CitaInfo.Correo, new string[] { CitaInfo.NombrePaciente, CitaInfo.FechaHoraCitaTxt, CitaInfo.FechaCitaTxt.ToUpper(), CitaInfo.NombreMedico.ToUpper() });
        }

        // FUNCION QUE ELIMINA/CANCELA UNA CITA
        public string ElimCita(int IDCita)
        {
            return MiCita.ElimCita(IDCita, (int)Session["IdClinica"]);
        }
        // ------------ REGISTRO DE CITAS MEDICAS ------------

        // --------------- PAGAR CITAS ---------------
        // FUNCION QUE DEVUELVE LA LISTA DE CITAS PENDIENTES DE PAGO
        public string ConCitasPagar()
        {
            return MiCita.ConCitasPagar((int)Session["IdClinica"]);
        }

        // FUNCION QUE REALIZA EL PAGO DE LA CITA
        public string AltaPagoCita(MCitas.PagoCitas PagoCita)
        {
            return MiCita.AltaPagoCita(PagoCita, MISC.TokenUsuario(), (int)Session["IdClinica"]);
        }
        // --------------- PAGAR CITAS ---------------
    }
}