using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace siacWEB.Models
{
    public class MXDev : Controller
    {
        // ::::::::::: CLASES Y VARIABLES :::::::::::
        // CLASE DE ESQUEMA DE BASE DE DATOS
        public class DBSchema
        {
            public string NombreColumna { get; set; }
            public string NombreTabla { get; set; }
            public string TipoDato { get; set; }
        }

        // ::::::::::: FUNCIONES GENERALES :::::::::::
        // FUNCION QUE DEVUELVE EL ESQUEMA DE LA BD
        public string EsquemaBD()
        {
            try
            {
                SQL.comandoSQLTrans("SchemaInfoGeneral");
                List<DBSchema> Tablas = new List<DBSchema>();
                SQL.commandoSQL = new SqlCommand("SELECT TABLE_NAME AS 'NombreTabla' FROM INFORMATION_SCHEMA.COLUMNS GROUP BY TABLE_NAME", SQL.conSQL, SQL.transaccionSQL);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Tablas.Add(new DBSchema() {
                            NombreTabla = lector["NombreTabla"].ToString(),
                        });
                    }
                }
                Dictionary<string, object> SchemaLista = new Dictionary<string, object>();
                foreach (var tabla in Tablas)
                {
                    List<DBSchema> ColumnasDataTipo = new List<DBSchema>();
                    SQL.commandoSQL = new SqlCommand("SELECT COLUMN_NAME AS 'NombreColumna', DATA_TYPE AS 'TipoDato'  FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TablaNombreParam", SQL.conSQL, SQL.transaccionSQL);
                    SQL.commandoSQL.Parameters.Add(new SqlParameter("TablaNombreParam", SqlDbType.VarChar) { Value = tabla.NombreTabla });
                    using (var lector = SQL.commandoSQL.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            ColumnasDataTipo.Add(new DBSchema()
                            {
                                NombreColumna = lector["NombreColumna"].ToString(),
                                TipoDato = lector["TipoDato"].ToString(),
                            });
                        }
                    }
                    SchemaLista.Add(tabla.NombreTabla, ColumnasDataTipo);
                }

                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(SchemaLista);
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }

        // FUNCION QUE GENERA UNA  QUERY ABIERTA EN LA BD
        // FUNCION QUE EJECUTA UNA QUERY
        public string QueryBD(string Query, bool Consulta)
        {
            try
            {
                SQL.comandoSQLTrans("ExecuteQuery");
                SQL.commandoSQL = new SqlCommand(Query, SQL.conSQL, SQL.transaccionSQL);
                if (Consulta)
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(SQL.commandoSQL);
                    da.Fill(dt);
                    List<object> ConsultaResult = new List<object>();
                    foreach (DataRow row in dt.Rows)
                    {
                        Dictionary<object, object> lector = new Dictionary<object, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            lector.Add(col.ColumnName, row[col.ColumnName].ToString());
                        }
                        ConsultaResult.Add(lector);
                    }

                    SQL.transaccionSQL.Commit();
                    da.Dispose();
                    return JsonConvert.SerializeObject(ConsultaResult);
                }
                else
                {
                    SQL.commandoSQL.ExecuteNonQuery();

                    SQL.transaccionSQL.Commit();
                    return "true";
                }
            }
            catch (Exception e)
            {
                SQL.transaccionSQL.Rollback();
                return e.ToString();
            }
            finally
            {
                SQL.conSQL.Close();
            }
        }
    }
}