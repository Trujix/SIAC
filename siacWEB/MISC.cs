﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using siacWEB.Models;
using System.Net.Mail;
using System.Net;
using System.Drawing;

namespace siacWEB
{
    public class MISC
    {
        // VARIABLES GLOBALES
        // VARIABLE QUE LLAMA LA CLASE DE HOME PARA UN INICIO DE SESION ESPECIAL
        static MHome MiHomeMISC = new MHome();
        // CLASE DE COOKIES
        public class Cookies
        {
            public string TokenUsuario { get; set; }
            public string TokenClinica { get; set; }
        }

        // ::::::::::::::::::: FUNCIONES DE USO Y MULTIVARIADO :::::::::::::::::::
        // FUNCION VERIFICA LOS PARAMETROS DE LA SESION DEL USUARIO
        public static bool VerifSesion()
        {
            if (HttpContext.Current.Request.Cookies["usuariodata"] != null)
            {
                VerifSesionParams();
                return true;
            }
            else
            {
                HttpContext.Current.Session.Clear();
                HttpContext.Current.Session.Abandon();
                return false;
            }
        }

        // FUNCION QUE CREA UNA COOKIE CON VALORES DE USUARIO
        public static void CrearParamsSesion(string TokenUsuario, string TokenClinica, int IDCentro, Dictionary<string, bool> Perfiles, string NombreUsuario, int Tiempo)
        {
            foreach(KeyValuePair<string, bool> Perfil in Perfiles)
            {
                HttpContext.Current.Session[Perfil.Key] = Perfil.Value;
            }
            HttpContext.Current.Session["NombreUsuario"] = NombreUsuario;
            HttpContext.Current.Session["IdClinica"] = IDCentro;
            Dictionary<string, object> Respuesta = new Dictionary<string, object>()
            {
                { "TokenUsuario", TokenUsuario },
                { "TokenClinica", TokenClinica },
            };
            HttpContext.Current.Response.Cookies["usuariodata"].Value = JsonConvert.SerializeObject(Respuesta);
            HttpContext.Current.Response.Cookies["usuariodata"].Expires = DateTime.Now.AddMinutes(Tiempo);
        }

        // FUNCION QUE REESTRUCTURA LOS PARAMETROS DE LA SESION (SESSION PARAMS)
        public static void VerifSesionParams()
        {
            if(HttpContext.Current.Session["NombreUsuario"] == null)
            {
                Cookies Galleta = JsonConvert.DeserializeObject<Cookies>(HttpContext.Current.Request.Cookies["usuariodata"].Value);
                MHome.LoginInfo LoginVerif = MiHomeMISC.IniciarSesion(null, Galleta.TokenClinica, Galleta.TokenUsuario);
                foreach (KeyValuePair<string, bool> Perfil in LoginVerif.PerfilUsuario)
                {
                    HttpContext.Current.Session[Perfil.Key] = Perfil.Value;
                }
                HttpContext.Current.Session["NombreUsuario"] = LoginVerif.NombreUsuario;
                HttpContext.Current.Session["IdClinica"] = LoginVerif.IdClinica;
            }
        }

        // FUNCION QUE DEVUELVE EL TOKEN DE USUARIO PARA USOS DEL SISTEMA
        public static string TokenUsuario()
        {
            return JsonConvert.DeserializeObject<Cookies>(HttpContext.Current.Request.Cookies["usuariodata"].Value).TokenUsuario;
        }

        // FUNCION QUE DEVUELVE EL TOKEN DE LA CLINICA PARA USOS DEL SISTEMA
        public static string TokenClinica()
        {
            return JsonConvert.DeserializeObject<Cookies>(HttpContext.Current.Request.Cookies["usuariodata"].Value).TokenClinica;
        }

        // FUNCION QUE DEVUELVE LA FECHA DE HOY
        public static DateTime FechaHoy()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)"));
        }

        // FUNCION QUE GENERAUNA CADENA ALEATORIA (EXPERIEMNTAL)
        public static string GenerarCadAleatoria(int length, Random random)
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }

        // FUNCION QUE CREA UNA CADENA ALEATORIA 
        // [ TIPO::: 1 - 3 NUMEROS , 2 - ALFANUMERICA (CUALQUIER TAMAÑO) , 3 - SOLO NUMEROS (CUALQUIER TAMAÑO) , 4 - SOLO LETRAS (CUALQUIER TAMAÑO) ]
        public static string CrearCadAleatoria(int tipo, int longitud)
        {
            Random aleatorio = new Random();
            string cadenaRetorno = "";
            if (tipo == 1)
            {
                int NumAl = aleatorio.Next(100, 999);
                cadenaRetorno = NumAl.ToString();
            }
            else if (tipo == 2)
            {
                const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                cadenaRetorno = new string(Enumerable.Range(1, longitud).Select(_ => caracteres[aleatorio.Next(caracteres.Length)]).ToArray());
            }
            else if (tipo == 3)
            {
                const string caracteres = "0123456789";
                cadenaRetorno = new string(Enumerable.Repeat(caracteres, longitud).Select(s => s[aleatorio.Next(s.Length)]).ToArray());
            }
            else if (tipo == 4)
            {
                const string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                cadenaRetorno = new string(Enumerable.Repeat(caracteres, longitud).Select(s => s[aleatorio.Next(s.Length)]).ToArray());
            }
            return cadenaRetorno;
        }

        // FUNCION QUE CREA UNA CADENA DE CARACTERES LARGA PARA EL USO DE REALIZAR TOKENS
        public static string CrearIdSession()
        {
            string IdSession = ""; Int64 cadFecha = 0; string cadFechaTXT = ""; Random num = new Random();
            var f = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.ToUniversalTime() - f);
            cadFecha = (Int64)(t.TotalMilliseconds + 0.5);
            cadFechaTXT = cadFecha.ToString();
            foreach (char c in cadFechaTXT)
            {
                int l = num.Next(0, 26);
                char let = (char)('a' + l);
                IdSession += c + "" + let;
            }
            return IdSession;
        }

        // FUNCION QUE CREA UNA CADENA MD5
        public static string CrearMD5(string cadena)
        {
            StringBuilder constructor = new StringBuilder();
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] dato = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(cadena));
                for (int i = 0; i < dato.Length; i++)
                {
                    constructor.Append(dato[i].ToString("x2"));
                }
            }
            return constructor.ToString();
        }

        // ::::::::::::::::::::::::::::::: CORREOS ELECTRONICOS :::::::::::::::::::::::::::::::
        // VARIABLES DE CORREO ELECTRONICO
        public static string CorreoCuerpo = "<!DOCTYPE html><html lang='es'><head><meta charset='UTF-8' /></head><body style='font-family: -apple-system,BlinkMacSystemFont,Roboto,Arial,sans-serif;'><center><div align='left' style='width: 600px; position: relative;display: -webkit-box;display: -ms-flexbox;display: flex;-webkit-box-orient: vertical;-webkit-box-direction: normal;-ms-flex-direction: column;flex-direction: column;min-width: 0;word-wrap: break-word;background-color: #fff;background-clip: border-box;border: 1px solid rgba(0,0,0,.125);border-radius: .25rem;'><div style='-webkit-box-flex: 1;-ms-flex: 1 1 auto;flex: 1 1 auto;padding: 1.25rem;'><img src='https://drive.google.com/uc?export=view&id=1PAFd98pO15r4-Tle4VGeM-G7M6ZSRoAO' style='height: 50px;' /><br /><hr />×ØCLINICAINFOØ×<br />×ØCUERPOMAILØ×<center><p style='font-size: 10px;'><i>La salud es la mayor posesión. La alegría es el mayor tesoro. La confianza es el mayor amigo - Lao Tzu</i></p><center><br /><h6>Correo generado automáticamente desde el sistema SIAC &copy;" + DateTime.Now.ToString("yyyy") + ". No es necesario contestar a este correo, ya que no recibirá respuesta. Si desea información, consulte con el usuario administrador de la clínica.</h6></div></div></center></body></html>";
        public static string[] CorreoHTML =
        {
            "<p>Estimado <b>Ø×NOMBREPACIENTE×Ø</b></p><br /><p></b>. Por medio del presente correo le informamos que su cita ha sido agendada correctamente; al mismo tiempo que le anexamos la información de la misma a continuación:</p><p><b>HORA: </b>Ø×HORA×Ø<br><b>FECHA: </b>Ø×FECHA×Ø<br><b>NOMBRE MÉDICO: </b>Ø×NOMBREMEDICO×Ø</p><br /><p><br>Cualquier duda o aclaración pongase en contacto con nuestros asistentes y con gusto lo atenderemos.</p><br>",
            "<p>Estimado <b>Ø×NOMBREUSUARIO×Ø</b></p><br /><p></b>. Por medio del presente correo le damos la bienvenida y le informamos que su registro en nuestro sistema ha sido exitoso; le anexamos a continuación los datos para su inicio de sesión:</p><p><b>USUARIO: </b>Ø×USUARIO×Ø<br><b>CONTRASEÑA: </b>Ø×PASSWORD×Ø</p><br /><p><br>La información anterior mostrada solo es conocida por <b>usted</b>. Cualquier duda o aclaración le recomendamos ponerse en contacto con el administrador del sistema.</p><br>"
        };
        // FUNCION QUE ENVIA CORREO ELECTRONICO
        public static string EnviarMail(int MailHTML, string Correo, string[] CadsAdic)
        {
            try
            {
                Cookies Galleta = JsonConvert.DeserializeObject<Cookies>(HttpContext.Current.Request.Cookies["usuariodata"].Value);
                MHome.ClinicaInfo ClinicaData = JsonConvert.DeserializeObject<MHome.ClinicaInfo>(MiHomeMISC.ClinicaData((int)HttpContext.Current.Session["IdClinica"]));
                string ClinicaDataHTML = "<center><h5><b>" + ClinicaData.NombreClinica + "<br />" + ClinicaData.ClaveClinica + "<br />" + ClinicaData.Direccion + ", " + ClinicaData.Colonia + " - CP: " + ClinicaData.CP + "<br />Tel: " + ClinicaData.Telefono + "<br />" + ClinicaData.Municipio + ", " + ClinicaData.Estado + "</b></h5></center>";
                string UrlUsuarioDocs = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath, "") + "/Docs/" + Galleta.TokenClinica + "/";
                string CorreoHTMLCodigo = CorreoHTML[MailHTML];
                string Motivo = "";
                if(MailHTML == 0)
                {
                    CorreoHTMLCodigo = CorreoHTMLCodigo.Replace("Ø×NOMBREPACIENTE×Ø", CadsAdic[0]).Replace("Ø×HORA×Ø", CadsAdic[1]).Replace("Ø×FECHA×Ø", CadsAdic[2]).Replace("Ø×NOMBREMEDICO×Ø", CadsAdic[3]);
                    Motivo = "Cita Médica";
                }
                else if(MailHTML == 1)
                {
                    CorreoHTMLCodigo = CorreoHTMLCodigo.Replace("Ø×NOMBREUSUARIO×Ø", CadsAdic[0]).Replace("Ø×USUARIO×Ø", CadsAdic[1]).Replace("Ø×PASSWORD×Ø", CadsAdic[2]);
                    Motivo = "Información Usuario";
                }

                var smtpUsuario = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential("siac.adm.gestionmail@gmail.com", "siac2020"),
                    EnableSsl = true
                };
                MailMessage msg = new MailMessage();
                MailAddress mailKiosko = new MailAddress("siac.adm.gestionmail@gmail.com");
                MailAddress mailCategorie = new MailAddress(Correo);
                msg.From = mailKiosko;
                msg.To.Add(mailCategorie);
                msg.Subject = Motivo;
                msg.Body = CorreoCuerpo.Replace("×ØCLINICAINFOØ×", ClinicaDataHTML).Replace("×ØCUERPOMAILØ×", CorreoHTMLCodigo);
                msg.IsBodyHtml = true;
                smtpUsuario.Send(msg);
                return "true";
            }
            catch(Exception e)
            {
                return e.ToString();
            }
        }

        // FUNCION QUE DEVUELVE UNA IMAGEN DE UN CODIGO BASE64
        public static Image Base64ToImage(string Base64Codigo)
        {
            byte[] ImgBytes = Convert.FromBase64String(Base64Codigo);
            using (var ms = new MemoryStream(ImgBytes, 0, ImgBytes.Length))
            {
                Image Imagen = Image.FromStream(ms, true);
                return Imagen;
            }
        }

        // FUNCION INDEPENDIENTE QUE GUARDA UN ARCHIVO
        public static string GuardarArchivo(string Nombre, string ServerDir, Image Archivo)
        {
            try
            {
                string rutaClinica = "/Docs/" + TokenClinica() + "/";
                Directory.CreateDirectory(ServerDir + rutaClinica);
                Archivo.Save(ServerDir + rutaClinica + Nombre);
                return "true";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}