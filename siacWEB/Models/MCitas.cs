using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace siacWEB.Models
{
    public class MCitas : Controller
    {
        // ::::::::::::::: CLASES Y VARIABLES :::::::::::::::
        // ---- REGISTRO DE CITAS
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
            public string Nombre { get; set; }
            public string ApellidoP { get; set; }
            public string ApellidoM { get; set; }
            public double Telefono { get; set; }
            public string NombreMedico { get; set; }
            public string HoraCita { get; set; }
            public DateTime FechaCita { get; set; }
            public DateTime FechaHoraCita { get; set; }
            public string FechaCitaTxt { get; set; }
            public string FechaHoraCitaTxt { get; set; }
            public bool EnviarCorreo { get; set; }
            public string Correo { get; set; }
        }
        // ---- PAGO DE CITAS
        // CLASE DE PAGO DE CITAS
        public class PagoCitas
        {
            public int IdPagoCita { get; set; }
            public int IdCita{ get; set; }
            public double MontoPago { get; set; }
        }

        // ::::::::::::::: FUNCIONES GENERALES :::::::::::::::

        // ------------ REGISTRO DE CITAS ------------
        // FUNCION QUE DEVUELVE LOS PARAMETROS DE INICIO DEL MENU  DE REGISTRO DE CITAS
        public string ParamsRegistroCitas(int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("RegistroCitasParams");
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
                SQL.commandoSQL = new SqlCommand("SELECT U.*, UI.idespecialidad, UI.consultorio FROM dbo.usuarios U JOIN dbo.usuarioinfo UI ON UI.idusuario = U.id WHERE U.tipo = 'M' AND U.tokenclinica = (SELECT token FROM dbo.clinica WHERE id = @IDClinicaParam)", SQL.conSQL, SQL.transaccionSQL);
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
                            IdMedico = int.Parse(lector["idusuario"].ToString()),
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
                SQL.commandoSQL = new SqlCommand("SELECT C.*, (M.nombre + ' ' + M.apellido) AS nombremedico FROM dbo.citasregistros C JOIN dbo.usuarios M ON M.id = C.idusuario WHERE C.idclinica = @IDClinicaParam AND fechacita > @FechaCitaParam", SQL.conSQL, SQL.transaccionSQL);
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
                            IdMedico = int.Parse(lector["idusuario"].ToString()),
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
                                !bool.Parse(lector["pagada"].ToString()) ? "<button class='btn badge badge-pill badge-warning' title='Reagendar Cita'" + " onclick='reagendarCita(" + lector["id"].ToString() + ");'><i class='fa fa-edit'></i></button>&nbsp;<button name='cita" + lector["id"].ToString() + "' class='btn badge badge-pill badge-dark' title='Reenviar Correo'" + " onclick='reenviarCorreoCita(" + lector["id"].ToString() + ");'><i class='fa fa-envelope'></i></button>&nbsp;<button class='btn badge badge-pill badge-danger' title='Cancelar Cita'" + " onclick='cancelarCita(" + lector["id"].ToString() + ");'><i class='fa fa-trash'></i></button>" : "<button class='btn badge badge-pill badge-success'><i class='fa fa-check-circle'></i> Consulta Concluida</button>",
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
                SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.citasregistros WHERE idusuario = @IDMedicoParam AND fechacita = @FechaCitaParam AND idclinica = @IDClinicaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] citasMedicoRegistro =
                {
                    new SqlParameter("@IDMedicoParam", SqlDbType.Int) { Value = citasinfo.IdMedico },
                    new SqlParameter("@FechaCitaParam", SqlDbType.DateTime) { Value = citasinfo.FechaCita },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(citasMedicoRegistro);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaCitasRegistros.Add(new CitasRegistros()
                        {
                            IdCitaRegistro = int.Parse(lector["id"].ToString()),
                            IdMedico = int.Parse(lector["idusuario"].ToString()),
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
                if (citainfo.IdPaciente == 0)
                {
                    SQL.commandoSQL = new SqlCommand("SELECT * FROM dbo.pacientes WHERE idclinica = @IDClinicaParam AND nombre = @NombreParam AND apellidop = @ApellidoPParam AND apellidom = @ApellidoMParam", SQL.conSQL, SQL.transaccionSQL);
                    SqlParameter[] verifPaciente =
                    {
                        new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                        new SqlParameter("@NombreParam", SqlDbType.VarChar) { Value = citainfo.Nombre },
                        new SqlParameter("@ApellidoPParam", SqlDbType.VarChar) { Value = citainfo.ApellidoP },
                        new SqlParameter("@ApellidoMParam", SqlDbType.VarChar) { Value = citainfo.ApellidoM },
                    };
                    SQL.commandoSQL.Parameters.AddRange(verifPaciente);
                    using (var lector = SQL.commandoSQL.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            citainfo.IdPaciente = int.Parse(lector["id"].ToString());
                        }
                    }

                    if (citainfo.IdPaciente == 0)
                    {
                        SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.pacientes (idclinica, nombre, apellidop, apellidom, telefono, correo, fechahora, admusuario) VALUES (@IDClinicaParam, @NombreParam, @ApellidoPParam, @ApellidoMParam, @TelefonoParam, @CorreoParam, @FechaHoraParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
                        SqlParameter[] altaPaciente =
                        {
                            new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                            new SqlParameter("@NombreParam", SqlDbType.VarChar) { Value = citainfo.Nombre },
                            new SqlParameter("@ApellidoPParam", SqlDbType.VarChar) { Value = citainfo.ApellidoP },
                            new SqlParameter("@ApellidoMParam", SqlDbType.VarChar) { Value = citainfo.ApellidoM },
                            new SqlParameter("@TelefonoParam", SqlDbType.Float) { Value = citainfo.Telefono },
                            new SqlParameter("@CorreoParam", SqlDbType.VarChar) { Value = citainfo.Correo },
                            new SqlParameter("@FechaHoraParam", SqlDbType.DateTime) { Value = MISC.FechaHoy() },
                            new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
                        };
                        SQL.commandoSQL.Parameters.AddRange(altaPaciente);
                        SQL.commandoSQL.ExecuteNonQuery();
                    }
                }

                SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.citasregistros (idclinica, idusuario, idpaciente, nombrepaciente, horacita, fechacita, fechahoracita, correo, fechahora, admusuario) VALUES (@IDClinicaParam, @IDMedicoParam, @IDPacienteParam, @NombrePacienteParam, @HoraCitaParam, @FechaCitaParam, @FechaHoraCitaParam, @CorreoParam, @FechaHoraParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
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
                    new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
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

        // FUNCION QUE ELIMINA/CANCELA UNA CITA
        public string ElimCita(int idcita, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ElimCita");
                SQL.commandoSQL = new SqlCommand("DELETE FROM dbo.citasregistros WHERE idclinica = @IDClinicaParam AND id = @IDCitaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] eliminarCita =
                {
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                    new SqlParameter("@IDCitaParam", SqlDbType.Int) { Value = idcita },
                };
                SQL.commandoSQL.Parameters.AddRange(eliminarCita);
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
        // ------------ REGISTRO DE CITAS ------------

        // --------------- PAGAR CITAS ---------------
        // FUNCION QUE DEVUELVE LA LISTA DE CITAS PENDIENTES DE PAGO
        public string ConCitasPagar(int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("ConCitasPagar");
                DateTime Hoy = MISC.FechaHoy();
                List<CitasRegistros> ListaCitas = new List<CitasRegistros>();
                List<string[]> CitasTabla = new List<string[]>();
                SQL.commandoSQL = new SqlCommand("SELECT C.*, (M.nombre + ' ' + M.apellido) AS nombremedico FROM dbo.citasregistros C JOIN dbo.usuarios M ON M.id = C.idusuario WHERE C.fechacita = @FechaCitaParam AND C.idclinica = @IDClinicaParam AND C.pagada = 'False'", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] citasMedicoPagar =
                {
                    new SqlParameter("@FechaCitaParam", SqlDbType.DateTime) { Value = new DateTime(Hoy.Year, Hoy.Month, Hoy.Day) },
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                };
                SQL.commandoSQL.Parameters.AddRange(citasMedicoPagar);
                using (var lector = SQL.commandoSQL.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        ListaCitas.Add(new CitasRegistros()
                        {
                            IdCitaRegistro = int.Parse(lector["id"].ToString()),
                            IdMedico = int.Parse(lector["idusuario"].ToString()),
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
                                "<button class='btn badge badge-pill badge-success' title='Pagar Consulta' onclick='pagarCita(" + lector["id"].ToString() + ");'><i class='fa fa-dollar-sign'></i>&nbsp;Pagar Consulta</button>",
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

        // FUNCION QUE REALIZA EL PAGO DE LA CONSULTA
        public string AltaPagoCita(PagoCitas pagoconsulta, string tokenusuario, int idclinica)
        {
            try
            {
                SQL.comandoSQLTrans("AltaPagoCita");
                SQL.commandoSQL = new SqlCommand("INSERT INTO dbo.pagosconsultas (idclinica, idconsulta, montopago, fechahora, admusuario) VALUES (@IDClinicaParam, @IDConsultaParam, @MontoPagoParam, @FechaHoraParam, (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam))", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] pagoConsulta =
                {
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                    new SqlParameter("@IDConsultaParam", SqlDbType.Int) { Value = pagoconsulta.IdCita },
                    new SqlParameter("@MontoPagoParam", SqlDbType.Float) { Value = pagoconsulta.MontoPago },
                    new SqlParameter("@FechaHoraParam", SqlDbType.DateTime) { Value = MISC.FechaHoy() },
                    new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
                };
                SQL.commandoSQL.Parameters.AddRange(pagoConsulta);
                SQL.commandoSQL.ExecuteNonQuery();

                SQL.commandoSQL = new SqlCommand("UPDATE dbo.citasregistros SET pagada = 'True', admusuario = (SELECT usuario FROM dbo.usuarios WHERE tokenusuario = @TokenParam) WHERE idclinica = @IDClinicaParam AND id = @IDConsultaParam", SQL.conSQL, SQL.transaccionSQL);
                SqlParameter[] actualizarEstatusConsulta =
                {
                    new SqlParameter("@IDClinicaParam", SqlDbType.Int) { Value = idclinica },
                    new SqlParameter("@IDConsultaParam", SqlDbType.Int) { Value = pagoconsulta.IdCita },
                    new SqlParameter("@TokenParam", SqlDbType.VarChar) { Value = tokenusuario },
                };
                SQL.commandoSQL.Parameters.AddRange(actualizarEstatusConsulta);
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
        // --------------- PAGAR CITAS ---------------
    }
}