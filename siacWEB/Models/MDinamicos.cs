using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace siacWEB.Models
{
    public class MDinamicos : Controller
    {
        // CLASES  Y VARIABLES

        // :::::::::::::::::::::: FUNCIONES - CONSULTAS DE PACIENTES ::::::::::::::::::::::
        // FUNCION QUE DEVUELVE EL RESULTADO DE LA BUSQUEDA DE PACIENTES POR NOMBRE
        public string BusqPacienteNombre(string pacientebusqueda, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("BusqPacienteNombre");
                string tablaPacientes = ""; int c = 0;
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.pacientes WHERE idclinica = @IDClinicaParam AND (nombre + ' ' + apellidop + ' ' + apellidom) LIKE @BusquedaParam AND activo = 'True'", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] busqPacienteByNombre =
                {
                    new SqlParameter("@BusquedaParam", SqlDbType.VarChar) { Value = "%" + pacientebusqueda.ToUpper() + "%" },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(busqPacienteByNombre);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string id = "", nombrePaciente = lector["nombre"].ToString() + " " + lector["apellidop"].ToString() + " " + lector["apellidom"].ToString();
                        if (c == 0)
                        {
                            id = " id='pacienteSeleccionado'";
                        }
                        tablaPacientes += "<tr" + id + " idpaciente='" + lector["id"].ToString() + "' name='pacienteActivo' class='busqpaciente' consulta='" + lector["telefono"].ToString() + " " + nombrePaciente + "' title='Doble click para elegir...'><td>" + nombrePaciente + "</td><td>" + lector["telefono"].ToString() + "</td></tr>";
                        c++;
                    }
                }

                SQL.transaccionSQL.Commit();
                return tablaPacientes;
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // FUNCION QUE DEVUELVE EL PACIENTE SELECCIONADO DE LA BUSQUEDA DINAMICA
        public string BusqPacienteID(int idpaciente, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("BusqPacienteID");
                Dictionary<string, object> ListaPacientes = new Dictionary<string, object>();
                SQL.commandoSQL = new SqlCommand("SELECT P.* FROM dbo.pacientes P WHERE P.idclinica = @IDClinicaParam AND P.id = @IDPacienteParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] busqPacienteByNombre =
                {
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                    new SqlParameter("@IDPacienteParam", SqlDbType.Int) { Value = idpaciente },
                };
                SQL.commandoSQL.Parameters.AddRange(busqPacienteByNombre);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaPacientes = new Dictionary<string, object>()
                        {
                            { "IdPaciente", int.Parse(lector["id"].ToString()) },
                            { "NombreCompleto", lector["nombre"].ToString() + " " + lector["apellidop"].ToString() + " " + lector["apellidom"].ToString() },
                            { "Nombre", lector["nombre"].ToString() },
                            { "ApellidoP", lector["apellidop"].ToString() },
                            { "ApellidoM", lector["apellidom"].ToString() },
                            { "Telefono", double.Parse(lector["telefono"].ToString()) },
                            { "Correo", lector["correo"].ToString() },
                        };
                    }
                }

                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(ListaPacientes);
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }
        // :::::::::::::::::::::: FUNCIONES - CONSULTAS DE PACIENTES ::::::::::::::::::::::
    }
}