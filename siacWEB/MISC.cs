using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace siacWEB
{
    public class MISC
    {
        // ::::::::::::::::::: FUNCIONES DE USO Y MULTIVARIADO :::::::::::::::::::
        // FUNCION VERIFICA LA COOKIE DEL USUARIO
        public static bool VerifSesion()
        {
            if (HttpContext.Current.Request.Cookies["usuariodata"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // FUNCION QUE CREA UNA COOKIE CON VALORES DE USUARIO
        public static void CrearCookie(string Token, int Tiempo)
        {
            Dictionary<string, object> Respuesta = new Dictionary<string, object>()
            {
                { "TokenUsuario", Token }
            };
            HttpContext.Current.Response.Cookies["usuariodata"].Value = JsonConvert.SerializeObject(Respuesta);
            HttpContext.Current.Response.Cookies["usuariodata"].Expires = DateTime.Now.AddMinutes(Tiempo);
        }
    }
}