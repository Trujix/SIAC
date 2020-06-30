// :::::::::::::::::::::::: VARIABLES GLOBALES ::::::::::::::::::::::::
var TablaCitasHTML;
var EspecialidadesMedicosJSON = {};
var HorariosMedicosJSON = [];
var MedicoHorarioNCitaJSON = {};
var MedicoHorasDispNCitaJSON = [];
var IdMedicoNuevaCitaGLOBAL = 0;
var IdPacienteNuevaCitaGLOBAL = 0;
var MedicoCitasHrsARR = [];
var CitaAltaJSON = {};

// :::::::::::::::::::::::: DOCUMENTS - INPUTS ::::::::::::::::::::::::

// ----------------- REGISTRO DE CONSULTAS -----------------
// DOCUMENT - BOTON QUE CARGA LA LISTA DE CITAS
$(document).on('click', '#btnCargarCitas', function () {
    cargarListaCitas(false, []);
});

// DOCUMENT - BOTON EJECUTA EL MODAL PARA REGISTRAR UNA NUEVA CITA
$(document).on('click', '#btnNuevaCita', function () {
    LoadingOn("Cargando Formulario...");
    $('#modalNuevaCita').modal('show');
});

// DOCUMENT - CHECK CONTROLA SI SE HABILITA EL TEXT DE ENVIO DE CORREO A PACIENTE LA CONFIRMACION
$(document).on('change', '#modalNuevaCitaEmailSi', function () {
    if ($(this).is(":checked")) {
        $('#modalNuevaCitaEmail').removeAttr("disabled");
        $('#modalNuevaCitaEmail').focus();
    } else {
        $('#modalNuevaCitaEmail').attr("disabled", true);
        $('#modalNuevaCitaEmail').blur();
    }
});

// DOCUMENT - SELECT CONTROLA LA  ELECCION DE ESPECIALIDAD Y LLENA EL SELECT CON LOS  MEDICOS
$(document).on('change', '#modalNuevaCitaEspecialidad', function () {
    var idEsp = $(this).val();
    $('#modalNuevaCitaMedico').html('<option value="-1">- SELECC. MÉDICO -</option>');
    if (idEsp !== "-1") {
        var medicos = '';
        $(EspecialidadesMedicosJSON[idEsp]).each(function (key, value) {
            medicos = '<option value="' + value.IdMedico + '">' + value.NombreCompleto + '</option>';
        });
        $('#modalNuevaCitaMedico').append(medicos);
    }
    $('#modalNuevaCitaMedico').change();
});

// DOCUMENT - SELECT CONTROLA LA SELECCION DE LOS PARAMETROS DEL MEDICO [ HORARIOS Y DISPONIBILIDAD ]
$(document).on('change', '#modalNuevaCitaMedico', function () {
    var idMedico = $(this).val();
    if (idMedico !== "-1") {
        $(HorariosMedicosJSON).each(function (key, value) {
            if (value.IdMedico === parseInt(idMedico)) {
                MedicoHorarioNCitaJSON = value;
                IdMedicoNuevaCitaGLOBAL = value.IdMedico;
                return false;
            }
        });
        $('#modalNuevaCitaDivMedicoParams').html(nuevaCitaPanelConfig);
        $('#modalNuevaCitaFecha').val(FechaInput());
        $('#modalNuevaCitaFecha').attr("min", FechaInput());
        iniciarNuevaCitaPanel();
    } else {
        $('#modalNuevaCitaDivMedicoParams').html(nuevaCitaSinParamsMedico);
    }
});

// DOCUMENT - INPUT DATE QUE CONTROLA LA CONFIGURACION DEL PANEL DE AGENDAR CITA DE ACUERDO AL HORARIO DEL MEDICO
$(document).on('change keyup', '#modalNuevaCitaFecha', function () {
    if ($(this).val() !== "") {
        iniciarNuevaCitaPanel();
    }
});

// DOCUMENT - BOTON QUE CONTROLA LA VERIFICACION DE UNA HORA A  LA HORA DE CREAR UNA NUEVA CITA
$(document).on('click', '#modalNuevaCitaVerifHora', function () {
    if (veriFormHrNuevaCita()) {
        MsgAlerta("Ok!", "La <b>Hora</b> establecida está <b>disponible</b>", 2500, "success");
    }
});

// DOCUMENT - BOTON QUE GUARDA LA CITA
$(document).on('click', '#modalNuevaCitaGuardar', function () {
    if (veriFormNuevaCita()) {
        MsgPregunta("Guardar Cita", "¿Desea continuar?", function (si) {
            if (si) {
                $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    url: "/Consultas/AltaCita",
                    data: { CitaInfo: CitaAltaJSON },
                    dataType: 'JSON',
                    beforeSend: function () {
                        LoadingOn("Guardando Cita...");
                    },
                    success: function (data) {
                        console.log(data);
                        if (data.AltaCita === "true") {
                            $('#modalNuevaCita').modal('hide');
                            LoadingOff();
                            arrMsg = [];
                            if (data.AltaCita === "true") {
                                arrMsg = ["Ok!", "La cita se almacenó <b>correctamente</b>", 1800, "success"];
                            } else {
                                arrMsg = ["Info!", "La cita se almacenó <b>correctamente</b>, pero hubo un problema al <b>enviar correo</b>", 3500, "info"];
                            }
                            cargarListaCitas(false, arrMsg);
                        } else {
                            errLog("E007", data.AltaCita);
                        }
                    },
                    error: function (error) {
                        errLog("E007", error.responseText);
                    }
                });
            }
        });
    }
});
// ----------------- REGISTRO DE CONSULTAS -----------------

// :::::::::::::::::::::::: FUNCIONES GLOBALES ::::::::::::::::::::::::

// ----------------- REGISTRO DE CONSULTAS -----------------
// FUNCION QUE CARGA LA LISTA COMPLETA DE CITAS (SOLO DEL DIA ACTUAL EN ADELANTE)
function cargarListaCitas(alta, msg) {
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Consultas/ConsCitas",
        dataType: 'JSON',
        beforeSend: function () {
            LoadingOn("Cargando Citas...");
        },
        success: function (data) {
            if (data.CitasTabla !== undefined) {
                console.log(data);
                TablaCitasHTML.clear().draw();
                TablaCitasHTML.rows.add(data.CitasTabla);
                TablaCitasHTML.columns.adjust().draw();
                LoadingOff();
                if (alta) {
                    MsgAlerta(msg[0], msg[1], msg[2], msg[3]);
                }
            } else {
                errLog("E008", data.responseText);
            }
        },
        error: function (error) {
            errLog("E008", error.responseText);
        }
    });
}

// FUNCION INICIAL DE REGISTRO DE CONSULTAS
function iniRegistrarConsulta() {
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Consultas/ParamsRegistroConsultas",
        dataType: 'JSON',
        beforeSend: function () {
            LoadingOn("Cargando Parametros...");
            EspecialidadesMedicosJSON = {};
        },
        success: function (data) {
            if (data.Correcto !== undefined) {
                var especialidades = '<option value="-1">- SELECC. ESPECIALIDAD -</option>';
                $(data.Especialidades).each(function (key, value) {
                    var medicos = [], idEsp = value.IdEspecialidad;
                    $(data.Medicos).each(function (k1, v1) {
                        if (idEsp === v1.IdEspecialidad) {
                            medicos.push({
                                IdMedico: v1.IdMedico,
                                NombreCompleto: v1.NombreCompleto,
                                Consultorio: v1.Consultorio,
                                IdEspecialidad: v1.IdEspecialidad,
                            });
                        }
                    });
                    EspecialidadesMedicosJSON[idEsp] = medicos;
                    especialidades += '<option value="' + idEsp + '">' + value.Nombre + '</option>';
                });
                $('#modalNuevaCitaEspecialidad').html(especialidades);
                $('#modalNuevaCitaMedico').html('<option value="-1">- SELECC. MÉDICO -</option>');
                HorariosMedicosJSON = data.HorariosMedicos;
                TablaCitasHTML = $('#tablaCitas').DataTable({
                    scrollY: "65vh",
                    data: [],
                    columns: [
                        { title: "Hora" },
                        { title: "Fecha" },
                        { title: "Nombre Paciente" },
                        { title: "Nombre Médico" },
                        { title: "Opciones", "orderable": false }
                    ],
                    info: false,
                    search: true,
                    "search": {
                        "regex": true
                    }
                });
                $('#modalNuevaCita').on('shown.bs.modal', function (e) {
                    IdPacienteNuevaCitaGLOBAL = 0;
                    $('.nuevacitatxt').val('');
                    $('.nuevacitanum').val('');
                    $('.nuevacitasel').val('-1');
                    $('#modalNuevaCitaNombre').focus();
                    $('#modalNuevaCitaEmailSi').attr("checked", false);
                    $('#modalNuevaCitaEmail').attr("disabled", true);
                    LoadingOff();
                });
                $('#modalNuevaCita').on('hidden.bs.modal', function (e) {
                    $('#modalNuevaCitaDivMedicoParams').html(nuevaCitaSinParamsMedico);
                });
                LoadingOff();
            } else {
                errLog("E005", data.responseText);
            }
        },
        error: function (error) {
            errLog("E005", error.responseText);
        }
    });
}

// FUNCION QUE INICALIZA EL PANEL DE CONFIGURACION DE AGENDAR CITA (MULTIPLES ENTRADAS)
function iniciarNuevaCitaPanel() {
    MedicoHorasDispNCitaJSON = [];
    MedicoCitasHrsARR = [];
    $('#modalNuevaCitaConfig').html('');
    if (MedicoHorarioNCitaJSON[fechaInfo($('#modalNuevaCitaFecha').val(), 1).DiaTxt] !== "--") {
        var citasInfo = {
            IdMedico: IdMedicoNuevaCitaGLOBAL,
            FechaCita: $('#modalNuevaCitaFecha').val(),
        }
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/Consultas/ConsCitasMedicoFecha",
            data: { CitasInfo: citasInfo },
            dataType: 'JSON',
            beforeSend: function () {
                LoadingOn("Cargando Citas...");
            },
            success: function (data) {
                if (Array.isArray(data)) {
                    var tablaCitas = (data.length > 0) ? '' : '<tr class="table-active"><td colspan="3" style="text-align: center;"><h6><b><i class="fa fa-info-circle"></i>&nbsp;No tiene citas agendadas</b></h6></td></tr>';
                    var hrsArr = MedicoHorarioNCitaJSON[fechaInfo($('#modalNuevaCitaFecha').val(), 1).DiaTxt].split(","), horario = "";
                    $(data).each(function (key, value) {
                        tablaCitas += '<tr><td>' + reloj12hrs(value.HoraCita) + '</td><td>' + value.FechaCitaTxt.toUpperCase() + '</td><td>' + value.NombrePaciente + '</td></tr>';
                    });
                    MedicoCitasHrsARR = data;
                    for (i = 0; i < hrsArr.length; i++) {
                        var hrs = hrsArr[i].split("-");
                        MedicoHorasDispNCitaJSON.push({
                            HrInicio: hrs[0],
                            HrFin: hrs[1],
                        });
                        if (horario !== "") {
                            if (i === (hrsArr.length - 1)) {
                                horario += " y ";
                            } else {
                                horario += ", ";
                            }
                        }
                        horario += 'de ' + reloj12hrs(hrs[0]) + ' a ' + reloj12hrs(hrs[1]);
                    }
                    $('#modalNuevaCitaMedicoHorario').html(nuevaCitaHorasMedico.replace("×ØDIAØ×", fechaInfo($('#modalNuevaCitaFecha').val(), 1).DiaTxt).replace("×ØHORARIOØ×", horario));
                    $('#modalNuevaCitaConfig').html(nuevaCitaPanelConfig2);
                    $('#modalNuevaCitaTablaCitas').html(tablaCitas);
                    LoadingOff();
                } else {
                    errLog("E006", data.responseText);
                }
            },
            error: function (error) {
                errLog("E006", error.responseText);
            }
        });
    } else {
        $('#modalNuevaCitaMedicoHorario').html(nuevaCitaMedicoNoTrabaja.replace("×ØDIAØ×", fechaInfo($('#modalNuevaCitaFecha').val(), 1).DiaTxt));
    }
}

// -----  VALIDACIONES
// FUNCION QUE VALIDA LA INCERSION DE UNA HORA PARA UNA NUEVA CITA
function veriFormHrNuevaCita() {
    var correcto = true, t = 2500, msg = '';
    if ($('#modalNuevaCitaHora').val() === "") {
        correcto = false;
        msg = 'Coloque la <b>Hora</b>';
        $('#modalNuevaCitaHora').focus();
    } else if (MedicoCitasHrsARR.length > 0) {
        $(MedicoCitasHrsARR).each(function (key, value) {
            var fecha = new Date(new Date().getFullYear(), 1, 1, parseInt(value.HoraCita.split(":")[0]), parseInt(value.HoraCita.split(":")[1]), 0, 0),
                hoy = new Date(new Date().getFullYear(), 1, 1, parseInt($('#modalNuevaCitaHora').val().split(":")[0]), parseInt($('#modalNuevaCitaHora').val().split(":")[1]), 0, 0);
            if (fecha.toString() === hoy.toString() && correcto) {
                correcto = false;
                msg = 'Ya tiene una <b>Cita</b> agendada a esa <b>Hora</b>\n\n<b>Paciente: </b>' + value.NombrePaciente;
                t = 4500;
            }
        });
    } else {
        var correcto1 = false;
        $(MedicoHorasDispNCitaJSON).each(function (key, value) {
            var ini = new Date(new Date().getFullYear(), 1, 1, parseInt(value.HrInicio.split(":")[0]), parseInt(value.HrInicio.split(":")[1]), 0, 0),
                fin = new Date(new Date().getFullYear(), 1, 1, parseInt(value.HrFin.split(":")[0]), parseInt(value.HrFin.split(":")[1]), 0, 0),
                hoy = new Date(new Date().getFullYear(), 1, 1, parseInt($('#modalNuevaCitaHora').val().split(":")[0]), parseInt($('#modalNuevaCitaHora').val().split(":")[1]), 0, 0);
            if (hoy >= ini && hoy <= fin) {
                correcto1 = true;
            }
        });
        if (!correcto1) {
            correcto = false;
            t = 4000;
            msg = 'La Hora seleccionada <b>está fuera del horario del Médico</b>';
        }
    }
    if (!correcto) {
        MsgAlerta("Atención", msg, t, "default");
    }
    return correcto;
}

// FUNCION QUE VALIDA EL FORMULARIO DE ALTA DE CITA
function veriFormNuevaCita() {
    var correcto = true, msg = '';
    if ($('#modalNuevaCitaNombre').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Nombre</b>';
        $('#modalNuevaCitaNombre').focus();
    } else if ($('#modalNuevaCitaApellidoP').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Apellido Paterno</b>';
        $('#modalNuevaCitaApellidoP').focus();
    } else if ($('#modalNuevaCitaApellidoM').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Apellido Materno</b>';
        $('#modalNuevaCitaApellidoM').focus();
    } else if ($('#modalNuevaCitaEmailSi').is(":checked") && $('#modalNuevaCitaEmail').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Correo Electronico</b>';
        $('#modalNuevaCitaEmail').focus();
    } else if ($('#modalNuevaCitaEmailSi').is(":checked") && !esEmail($('#modalNuevaCitaEmail').val())) {
        correcto = false;
        msg = 'El formato del <b>Correo Electronico es Incorrecto</b>';
        $('#modalNuevaCitaEmail').focus();
    } else if ($('#modalNuevaCitaEspecialidad').val() === "-1") {
        correcto = false;
        msg = 'Seleccione la <b>Especialidad</b>';
        $('#modalNuevaCitaEspecialidad').focus();
    } else if ($('#modalNuevaCitaMedico').val() === "-1") {
        correcto = false;
        msg = 'Seleccione el <b>Médico</b>';
        $('#modalNuevaCitaMedico').focus();
    } else if (MedicoHorarioNCitaJSON[fechaInfo($('#modalNuevaCitaFecha').val(), 1).DiaTxt] === "--") {
        correcto = false;
        msg = 'No ha configurado la <b>Cita</b>';
        $('#modalNuevaCitaFecha').focus();
    }
    if (!correcto) {
        MsgAlerta("Atención!", msg, 2900, "default");
    } else {
        if (!veriFormHrNuevaCita()) {
            correcto = false;
        } else {
            CitaAltaJSON = {
                IdMedico: IdMedicoNuevaCitaGLOBAL,
                IdPaciente: IdPacienteNuevaCitaGLOBAL,
                NombrePaciente: $('#modalNuevaCitaNombre').val().toUpperCase().trim() + " " + $('#modalNuevaCitaApellidoP').val().toUpperCase().trim() + " " + $('#modalNuevaCitaApellidoM').val().trim().toUpperCase(),
                HoraCita: $('#modalNuevaCitaHora').val(),
                FechaCita: $('#modalNuevaCitaFecha').val(),
                FechaHoraCita: $('#modalNuevaCitaFecha').val() + " " + $('#modalNuevaCitaHora').val() + ":00",
                Correo: ($('#modalNuevaCitaEmailSi').is(":checked")) ? $('#modalNuevaCitaEmail').val() : "--",
                FechaHoraCitaTxt: reloj12hrs($('#modalNuevaCitaHora').val()),
                FechaCitaTxt: $('#modalNuevaCitaMedico option:selected').text().trim().toUpperCase(),
            };
        }
    }
    return correcto;
}
// ----------------- REGISTRO DE CONSULTAS -----------------

// ::::::::::::::::::: VARIABLES DOM HTML :::::::::::::::::::
// ----------------- REGISTRO DE CONSULTAS -----------------
var nuevaCitaSinParamsMedico = '<div class="col-sm-12" align="center"><div class="alert2 alert2-secondary" role="alert"><b><i class="fa fa-clock"></i> Seleccione un Médico para configurar la cita</b></div></div>';
var nuevaCitaMedicoNoTrabaja = '<div class="col-sm-12" align="center"><div class="alert2 alert2-warning" style="margin-top: 10px;" role="alert"><b><i class="fa fa-info-circle"></i> El médico NO labora este día (×ØDIAØ×)</b></div></div>';
var nuevaCitaHorasMedico = '<div class="col-sm-12" align="center"><div class="alert2 alert2-info" role="alert"><b><i class="fa fa-clock"></i> Horario del ×ØDIAØ×: ×ØHORARIOØ×</b></div></div>';
var nuevaCitaPanelConfig = '<div class="col-sm-12"><div class="row"><div class="col-sm-4"><div class="form-group"><label for="modalNuevaCitaFecha"><i class="fa fa-date"></i>&nbsp;Fecha</label><input id="modalNuevaCitaFecha" type="date" class="form-control form-control-sm" /></div></div><div id="modalNuevaCitaMedicoHorario" class="col-sm-8"></div></div><div class="row" id="modalNuevaCitaConfig"></div></div>';
var nuevaCitaPanelConfig2 = '<div class="col-sm-2" align="right"><b>Selecc. Hora:</b></div><div class="col-sm-3"><input id="modalNuevaCitaHora" type="time" class="form-control form-control-sm" /></div><div class="col-sm-3"><button id="modalNuevaCitaVerifHora" class="btn btn-sm btn-block btn-info"><i class="fa fa-calendar-plus"></i> Verificar Disponibilidad</button></div><div class="col-sm-12"><div class="table-responsive"><table class="table table-sm table-bordered"><thead><tr><th>Hora</th><th>Fecha</th><th>Nombre Paciente</th></tr></thead><tbody id="modalNuevaCitaTablaCitas"></tbody></table></div></div>';
// ----------------- REGISTRO DE CONSULTAS -----------------