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

        // ------------------ VISTAS DE CONFIGURACION
        // FUNCION QUE DEVUELVE LA VISTA DE CONFIGURACION [ ADMIN ]
        public ActionResult ConfigAdmin()
        {
            return View();
        }

        // FUNCION QUE DEVUELVE LA VISTA DE CONFIGURACION [ ADMIN ]
        public ActionResult ConfigMedico()
        {
            return View();
        }
        // FUNCION QUE DEVUELVE LA VISTA DE CONFIGURACION [ ADMIN ]
        public ActionResult ConfigUsuario()
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
                    MHome.UsuarioInfo Usuario = JsonConvert.DeserializeObject<MHome.UsuarioInfo>(MiHome.ConUsuarioInfo(MISC.TokenUsuario(), (int)Session["IdClinica"], MISC.TokenClinica()));
                    Dictionary<string, object> UsuarioInfo = new Dictionary<string, object>()
                    {
                        { "NombreUsuario", Session["NombreUsuario"] },
                        { "ImgUsuario", (Usuario.ImagenUsuario) ? "../Docs/" + Usuario.Folder + "/" + Usuario.ImgNombre : "../Media/usuariodefault.png" },
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

        // ::::::::::::::::::: MENU CONFIGURACION DE USUARIOS :::::::::::::::::::
        // FUNCION QUE DEVUELVE LA INFORMACION DEL PANEL DE CONFIGURACION DEL [ USUARIO MEDICO ]
        public string ConfMedicoParams(int IdUsuario)
        {
            bool vs = MISC.VerifSesion();
            return MiHome.ConfMedicoParams(IdUsuario, (int)Session["IdClinica"]);
        }

        // FUNCION QUE GUARDA EL HORARIO DEL MEDICO [ USUARIO  MEDICO ]
        public string EditHorarioMedico(MHome.HorarioMedicoAlta HorarioData)
        {
            return MiHome.EditHorarioMedico(HorarioData, (int)Session["IdClinica"]);
        }
        // ::::::::::::::::::: MENU CONFIGURACION DE USUARIOS :::::::::::::::::::

        // ::::::::::::::::::: MENU INFERIOR USUARIO INFO :::::::::::::::::::
        // FUNCION QUE DEVUELVE LA INFO DEL USUARIO
        public string ConUsuarioInfo()
        {
            bool vs = MISC.VerifSesion();
            return MiHome.ConUsuarioInfo(MISC.TokenUsuario(), (int)Session["IdClinica"], MISC.TokenClinica());
        }

        // FUNCION QUE GUARDA LA INFO DEL USUARIO
        public string GuardarInfoUsuario(MHome.UsuarioInfo UsuarioInfo)
        {
            bool vs = MISC.VerifSesion();
            string Guardar = MiHome.GuardarInfoUsuario(UsuarioInfo, MISC.TokenUsuario(), MISC.TokenClinica(), (int)Session["IdClinica"]);
            if(Guardar == "true")
            {
                Session["NombreUsuario"] = UsuarioInfo.Nombre + " " + UsuarioInfo.Apellido;
            }
            return Guardar;
        }

        // FUNCION QUE GUARDA UNA IMAGEN DE USUARIO
        public string AltaImgUsuario(MHome.ImagenUsuario ImgData)
        {
            bool vs = MISC.VerifSesion();
            string Alta = MiHome.ImgUsuarioEstatus(ImgData, (int)Session["IdClinica"]);
            Dictionary<string, object> AltaImg = new Dictionary<string, object>()
            {
                { "AltaImgEstatus", Alta },
                { "GuardarImg", (Alta == "true") ? MISC.GuardarArchivo(ImgData.ImgNombre, Server.MapPath("~"), MISC.Base64ToImage(ImgData.Base64Codigo)) : "true" },
            };
            return JsonConvert.SerializeObject(AltaImg);
        }

        // FUNCION QUE ELIMINA UNA IMAGEN DE USUARIO
        public string ElimImgUsuario(MHome.ImagenUsuario ImgData)
        {
            bool vs = MISC.VerifSesion();
            return MiHome.ImgUsuarioEstatus(ImgData, (int)Session["IdClinica"]);
        }

        // FUNCION QUE CAMBIA LA CONTRASEÑA DEL USUARIO
        public string EditUsuarioPass(MHome.UsuarioNuevaPass NuevaPassData)
        {
            bool vs = MISC.VerifSesion();
            return MiHome.EditUsuarioPass(NuevaPassData, MISC.TokenUsuario(), MISC.TokenClinica());
        }
        // ::::::::::::::::::: MENU INFERIOR USUARIO INFO :::::::::::::::::::
    }
}