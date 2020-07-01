using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using siacWEB.Models;

namespace siacWEB.Controllers
{
    public class DinamicosController : Controller
    {
        // CLASES  Y VARIABLES

        // :::::::::::::::::::::: VISTAS ::::::::::::::::::::::
        MDinamicos MiDinamicos = new MDinamicos();

        // :::::::::::::::::::::: FUNCIONES - CONSULTAS DE PACIENTES ::::::::::::::::::::::
        // FUNCION QUE DEVUELVE EL RESULTADO DE LA BUSQUEDA DE PACIENTES POR NOMBRE
        public string BusqPacienteNombre(string PacienteBusqueda)
        {
            bool vs = MISC.VerifSesion();
            return MiDinamicos.BusqPacienteNombre(PacienteBusqueda, (int)Session["IdClinica"]);
        }

        // FUNCION QUE DEVUELVE EL RESULTADO DE LA BUSQUEDA DE PACIENTES POR ID
        public string BusqPacienteID(int IDPaciente)
        {
            return MiDinamicos.BusqPacienteID(IDPaciente, (int)Session["IdClinica"]);
        }
        // :::::::::::::::::::::: FUNCIONES - CONSULTAS DE PACIENTES ::::::::::::::::::::::
    }
}