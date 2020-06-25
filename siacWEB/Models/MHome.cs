using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace siacWEB.Models
{
    public class MHome : Controller
    {
        // ------------- CLASES GENERALES -------------
        // CLASE DE LOGIN
        public class LoginData
        {
            public string Usuario { get; set; }
            public string Pass { get; set; }
        }
    }
}