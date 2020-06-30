﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace siacWEB.Models
{
    public class MConsultas : Controller
    {
        // ::::::::::::::: CLASES Y VARIABLES :::::::::::::::
        // CLASE DE ESPECIALIDADES
        public class EspecialidadesCitas
        {
            public int IdEspecialidad { get; set; }
            public string Nombre { get; set; }
        }
        // CLASE DE MEDICOS
        public class MedicosCitas
        {
            public int IdMedico { get; set; }
            public int IdEspecialidad { get; set; }
            public string NombreCompleto { get; set; }
            public string Consultorio { get; set; }
        }
        // CLASE DE HORARIOS DE MEDICOS
        public class HorariosMedicosCitas
        {
            public int IdMedico { get; set; }
            public string Lunes { get; set; }
            public string Martes { get; set; }
            public string Miercoles { get; set; }
            public string Jueves { get; set; }
            public string Viernes { get; set; }
            public string Sabado { get; set; }
            public string Domingo { get; set; }
        }
        // CLASE DE CITAS REGISTROS
        public class CitasRegistros
        {
            public int IdCitaRegistro { get; set; }
            public int IdMedico { get; set; }
            public int IdPaciente { get; set; }
            public string NombrePaciente { get; set; }
            public string NombreMedico { get; set; }
            public string HoraCita { get; set; }
            public DateTime FechaCita { get; set; }
            public DateTime FechaHoraCita { get; set; }
            public string FechaCitaTxt { get; set; }
            public string FechaHoraCitaTxt { get; set; }
            public bool EnviarCorreo { get; set; }
            public string Correo { get; set; }
        }

        // ::::::::::::::: FUNCIONES GENERALES :::::::::::::::

        // ------------ REGISTRO DE CONSULTAS ------------
        // FUNCION QUE DEVUELVE LOS PARAMETROS DE INICIO DEL MENU  DE REGISTRO DE CONSULTAS
        public string ParamsRegistroConsultas(int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("RegistroConsultasParams");
                List<EspecialidadesCitas> ListaEspecialidades = new List<EspecialidadesCitas>();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.especialidades WHERE idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaEspecialidades.Add(new EspecialidadesCitas()
                        {
                            IdEspecialidad = int.Parse(lector["id"].ToString()),
                            Nombre = lector["nombre"].ToString(),
                        });
                    }
                }

                List<MedicosCitas> ListaMedicos = new List<MedicosCitas>();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.medicos WHERE idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaMedicos.Add(new MedicosCitas()
                        {
                            IdMedico = int.Parse(lector["id"].ToString()),
                            IdEspecialidad = int.Parse(lector["idespecialidad"].ToString()),
                            NombreCompleto = lector["nombre"].ToString() + " " + lector["apellido"].ToString(),
                            Consultorio = lector["consultorio"].ToString(),
                        });
                    }
                }

                List<HorariosMedicosCitas> ListaHorariosMedicos = new List<HorariosMedicosCitas>();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.horariosmedicos WHERE idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SQL.commandoSQL.Parameters.Add(new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica });
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaHorariosMedicos.Add(new HorariosMedicosCitas()
                        {
                            IdMedico = int.Parse(lector["id"].ToString()),
                            Lunes = lector["lunes"].ToString(),
                            Martes = lector["martes"].ToString(),
                            Miercoles = lector["miercoles"].ToString(),
                            Jueves = lector["jueves"].ToString(),
                            Viernes = lector["viernes"].ToString(),
                            Sabado = lector["sabado"].ToString(),
                            Domingo = lector["domingo"].ToString(),
                        });
                    }
                }

                Dictionary<string, object> Parametros = new Dictionary<string, object>()
                {
                    { "Correcto", true },
                    { "Especialidades", ListaEspecialidades },
                    { "Medicos", ListaMedicos },
                    { "HorariosMedicos", ListaHorariosMedicos },
                };
                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(Parametros);
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

        // FUNCION QUE DEVUELVE LA LISTA DE CITAS
        public string ConsCitas(int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ConsCitas");
                List<CitasRegistros> ListaCitas = new List<CitasRegistros>();
                List<string[]> CitasTabla = new List<string[]>();
                SQL.commandoSQL = new SqlCommand("SELECT C.*, (M.nombre + ' ' + M.apellido) AS nombremedico FROM dbo.citasregistros C JOIN dbo.medicos M ON M.id = C.idmedico WHERE C.idclinica = @IDClinicaParam AND fechacita > @FechaCitaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] consultaCitas =
                {
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                    new SqlParameter("@FechaCitaParam", SqlDbType.DateTime) { Value = MISC.FechaHoy().AddDays(-1) },
                };
                SQL.commandoSQL.Parameters.AddRange(consultaCitas);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaCitas.Add(new CitasRegistros()
                        {
                            IdCitaRegistro = int.Parse(lector["id"].ToString()),
                            IdMedico = int.Parse(lector["idmedico"].ToString()),
                            NombreMedico = lector["nombremedico"].ToString(),
                            IdPaciente = int.Parse(lector["idpaciente"].ToString()),
                            NombrePaciente = lector["nombrepaciente"].ToString(),
                            HoraCita = lector["horacita"].ToString(),
                            FechaCita = DateTime.Parse(lector["fechacita"].ToString()),
                            FechaHoraCita = DateTime.Parse(lector["fechahoracita"].ToString()),
                            FechaCitaTxt = DateTime.Parse(lector["fechacita"].ToString()).ToString("dddd, dd MMMM yyyy"),
                            FechaHoraCitaTxt = DateTime.Parse(lector["fechahoracita"].ToString()).ToString("dddd, dd MMMM yyyy hh:mm tt"),
                            Correo = lector["correo"].ToString(),
                        });
                        CitasTabla.Add(
                            new string[]
                            {
                                DateTime.Parse(lector["fechahoracita"].ToString()).ToString("hh:mm tt"),
                                DateTime.Parse(lector["fechacita"].ToString()).ToString("dddd, dd MMMM yyyy").ToUpper(),
                                lector["nombrepaciente"].ToString(),
                                lector["nombremedico"].ToString().ToUpper(),
                                "<button class='btn badge badge-pill badge-warning' title='Reagendar Cita' onclick='reagendarCita(" + lector["id"].ToString() + ");'><i class='fa fa-edit'></i></button>&nbsp;<button class='btn badge badge-pill badge-dark' title='Reenviar Correo' onclick='reenviarCorreoCita(" + lector["id"].ToString() + ");'><i class='fa fa-edit'></i></button>&nbsp;<button class='btn badge badge-pill badge-danger' title='Cancelar Cita' onclick='cancelarCita(" + lector["id"].ToString() + ");'><i class='fa fa-trash'></i></button>",
                            }
                        );
                    }
                }

                Dictionary<string, object> CitasData = new Dictionary<string, object>()
                {
                    { "ListaCitas", ListaCitas },
                    { "CitasTabla", CitasTabla },
                };
                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(CitasData);
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

        // FUNCION QUE DEVUELVE LA LISTA DE CITAS DE TODO EL CONSULTORIO
        public string ConsCitasMedicoFecha(CitasRegistros citasinfo, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("CitasMedicoFecha");
                List<CitasRegistros> ListaCitasRegistros = new List<CitasRegistros>();
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.citasregistros WHERE idmedico = @IDMedicoParam AND fechacita = @FechaCitaParam AND idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] consultaCitasMedicoRegistro =
                {
                    new SqlParameter("@IDMedicoParam", SqlDbType.Int) { Value = citasinfo.IdMedico },
                    new SqlParameter("@FechaCitaParam", SqlDbType.DateTime) { Value = citasinfo.FechaCita },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(consultaCitasMedicoRegistro);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaCitasRegistros.Add(new CitasRegistros()
                        {
                            IdCitaRegistro = int.Parse(lector["id"].ToString()),
                            IdMedico = int.Parse(lector["idmedico"].ToString()),
                            IdPaciente = int.Parse(lector["idpaciente"].ToString()),
                            NombrePaciente = lector["nombrepaciente"].ToString(),
                            HoraCita = lector["horacita"].ToString(),
                            FechaCita = DateTime.Parse(lector["fechacita"].ToString()),
                            FechaHoraCita = DateTime.Parse(lector["fechahoracita"].ToString()),
                            FechaCitaTxt = DateTime.Parse(lector["fechacita"].ToString()).ToString("dddd, dd MMMM yyyy"),
                            FechaHoraCitaTxt = DateTime.Parse(lector["fechahoracita"].ToString()).ToString("dddd, dd MMMM yyyy hh:mm tt"),
                            Correo = lector["correo"].ToString(),
                        });
                    }
                }

                SQL.transaccionSQL.Commit();
                return JsonConvert.SerializeObject(ListaCitasRegistros);
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

        // FUNCION QUE GUARA UNA CITA
        public string AltaCita(CitasRegistros citainfo, int idclinica, string tokenusuario)
        {
            try
            {
                SQL.comandoSQLTrans("AltaCita");
                SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.citasregistros (idclinica, idmedico, idpaciente, nombrepaciente, horacita, fechacita, fechahoracita, correo, fechahora, admusuario) VALUES (@IDClinicaParam, @IDMedicoParam, @IDPacienteParam, @NombrePacienteParam, @HoraCitaParam, @FechaCitaParam, @FechaHoraCitaParam, @CorreoParam, @FechaHoraParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] altaNuevaCita =
                {
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                    new SqlParameter("@IDMedicoParam", SqlDbType.Int) { Value = citainfo.IdMedico },
                    new SqlParameter("@IDPacienteParam", SqlDbType.Int) { Value = citainfo.IdPaciente },
                    new SqlParameter("@NombrePacienteParam", SqlDbType.VarChar) { Value = citainfo.NombrePaciente },
                    new SqlParameter("@HoraCitaParam", SqlDbType.VarChar) { Value = citainfo.HoraCita },
                    new SqlParameter("@FechaCitaParam", SqlDbType.DateTime) { Value = citainfo.FechaCita },
                    new SqlParameter("@FechaHoraCitaParam", SqlDbType.DateTime) { Value = citainfo.FechaHoraCita },
                    new SqlParameter("@CorreoParam", SqlDbType.VarChar) { Value = citainfo.Correo },
                    new SqlParameter("@FechaHoraParam", SqlDbType.DateTime) { Value = MISC.FechaHoy() },
                    new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = citainfo.FechaCita },
                };
                SQL.commandoSQL.Parameters.AddRange(altaNuevaCita);
                SQL.commandoSQL.ExecuteNonQuery();

                SQL.transaccionSQL.Commit();
                return "true";
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
        // ------------ REGISTRO DE CONSULTAS ------------
    }
}