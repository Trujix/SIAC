// :::::::::::::::::::::::::::: VARIABLES GLOBALES ::::::::::::::::::::::::::::
var TablaMedicosHTML;
var ListaMedicosJSON = [];
var MuevoMedEspecialidadesJSON = [];
var MedicosUsusARR = [];
var MedicoUsuarioGLOBAL = '';
var MedicoAltaJSON = {};
var TablaMedicosARR = [];
var IdNuevoMedicoSelecGLOBAL = 0;

// :::::::::::::::::::::::::::: DOCUMENTS E INPUTS ::::::::::::::::::::::::::::
// ----------------------- [ MEDICOS ] -----------------------
// DOCUMENT - BOTON QUE ABRE UN MODAL PARA DAR DE ALTA UN NUEVO MEDICO
$(document).on('click', '#btnNuevoMedico', function () {
    LoadingOn("Cargando Formulario...");
    IdNuevoMedicoSelecGLOBAL = 0;
    $('#modalNuevoMedico').modal('show');
});

// DOCUMENT - BOTON QUE CONTROLA EL ALTA DEL USUARIO MEDICO
$(document).on('click', '#modalNuevoMedicoGuardar', function () {
    if (veriFormNuevoMedico()) {
        MsgPregunta("Agregar Nuevo Medico", "¿Desea continuar?", function (si) {
            if (si) {
                $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    url: "/Administracion/AltaUsuario",
                    data: { UsuarioInfo: MedicoAltaJSON },
                    dataType: 'JSON',
                    beforeSend: function () {
                        LoadingOn("Guardando Medico...");
                    },
                    success: function (data) {
                        if (data.AltaUsuario === "true") {
                            console.log(data);
                            ListaMedicosJSON.push({
                                Apellido: MedicoAltaJSON.Apellido,
                                Consultorio: MedicoAltaJSON.Consultorio,
                                Correo: MedicoAltaJSON.Correo,
                                IdEspecialidad: MedicoAltaJSON.IdEspecialidad,
                                IdMedico: data.IdUsuario,
                                IdUsuario: 0,
                                Nombre: MedicoAltaJSON.Nombre,
                                NombreCompleto: MedicoAltaJSON.Nombre + " " + MedicoAltaJSON.Apellido,
                                Tipo: "M",
                                Usuario: MedicoUsuarioGLOBAL,
                            });
                            var nuevoMed = [MedicoAltaJSON.Nombre + " " + MedicoAltaJSON.Apellido, $('#modalNuevoMedicoEspecialidad option:selected').text(), MedicoAltaJSON.Consultorio, "<button class='btn badge badge-pill badge-dark' title='Reestablecer Contraseña'" + " onclick='reasignarPassMedico(" + data.IdUsuario + ");'><i class='fa fa-envelope'></i></button>"];
                            TablaMedicosARR.push(nuevoMed);
                            TablaMedicosHTML.clear().draw();
                            TablaMedicosHTML.rows.add(TablaMedicosARR);
                            TablaMedicosHTML.columns.adjust().draw();
                            MedicosUsusARR.push(MedicoUsuarioGLOBAL);
                            $('#modalNuevoMedico').modal('hide');
                            if (data.MailUsuario === "true") {
                                MsgAlerta("Ok!", "Medico <b>guardado correctamente</b>", 2000, "success");
                            } else {
                                MsgAlerta("Info!", "El Medico <b>fue guardado correctamente</b> pero ocurrió un problema al <b>enviar correo</b>\n\nInténtelo más tarde.", 4500, "info");
                            }
                            LoadingOff();
                        } else {
                            errLog("E014", data.AltaUsuario);
                        }
                    },
                    error: function (error) {
                        errLog("E014", error.responseText);
                    }
                });
            }
        });
    }
});
// ----------------------- [ MEDICOS ] -----------------------

// :::::::::::::::::::::::::::: FUNCIONES GLOBALES ::::::::::::::::::::::::::::
// ----------------------- [ MEDICOS ] -----------------------
// FUNCION INICIAL DE MEDICOS
function iniMedicos() {
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Administracion/ConMedicoParams",
        dataType: 'JSON',
        beforeSend: function () {
            LoadingOn("Cargando Parametros...");
            $('#modalNuevoMedico').on('shown.bs.modal', function (e) {
                MedicoUsuarioGLOBAL = generarUsuarioID(MedicosUsusARR, "med");
                $('#modalNuevoMedicoUsuario').html(MedicoUsuarioGLOBAL);
                $('.formnmedico').val('');
                $('#modalNuevoMedicoEspecialidad').val("-1");
                $('#modalNuevoMedicoNombre').focus();
                LoadingOff();
            });
            $('#modalNuevoMedico').on('hidden.bs.modal', function (e) {

            });
        },
        success: function (data) {
            if (data.Correcto !== undefined) {
                ListaMedicosJSON = data.Medicos;
                MuevoMedEspecialidadesJSON = data.Especialidades;
                MedicosUsusARR = data.UsuariosArray;
                TablaMedicosARR = data.MedicosTabla;
                var espOpts = '<option value="-1">- Eliga Especialidad -</option>';
                $(data.Especialidades).each(function (key, value) {
                    espOpts += '<option value="' + value.IdEspecialidad + '">' + value.Nombre + '</option>';
                });
                $('#modalNuevoMedicoEspecialidad').html(espOpts);
                TablaMedicosHTML = $('#tablaMedicos').DataTable({
                    scrollY: ($('.top_nav').prop('outerHTML') !== "") ? "55vh" : "65vh",
                    data: data.MedicosTabla,
                    columns: [
                        { title: "Nombre Médico" },
                        { title: "Especialidad" },
                        { title: "Consultorio" },
                        { title: "Opciones", "orderable": false }
                    ],
                    info: false,
                    search: true,
                    "search": {
                        "regex": true
                    }
                });
                LoadingOff();
            } else {
                errLog("E013", data.responseText);
            }
        },
        error: function (error) {
            errLog("E013", error.responseText);
        }
    });
}

// ------- VALIDACIONES
// FUNCION QUE VALIDA EL FORMULARIO DE ALTA DE UN MEDICO
function veriFormNuevoMedico() {
    var correcto = true, msg = '';
    if ($('#modalNuevoMedicoNombre').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Nombre</b>';
        $('#modalNuevoMedicoNombre').focus();
    } else if ($('#modalNuevoMedicoApellido').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Apellido</b>';
        $('#modalNuevoMedicoApellido').focus();
    } else if ($('#modalNuevoMedicoEspecialidad').val() === "-1") {
        correcto = false;
        msg = 'Seleccione la <b>Especialidad</b>';
        $('#modalNuevoMedicoEspecialidad').focus();
    } else if ($('#modalNuevoMedicoCorreo').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Correo</b>';
        $('#modalNuevoMedicoCorreo').focus();
    } else if (!esEmail($('#modalNuevoMedicoCorreo').val())) {
        correcto = false;
        msg = 'El formato del <b>Correo es incorrecto</b>';
        $('#modalNuevoMedicoCorreo').focus();
    } else if ($('#modalNuevoMedicoConsultorio').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Consultorio</b>';
        $('#modalNuevoMedicoConsultorio').focus();
    } else {
        MedicoAltaJSON = {
            Usuario: MedicoUsuarioGLOBAL,
            IdMedico: IdNuevoMedicoSelecGLOBAL,
            IdUsuario: 0,
            IdEspecialidad: $('#modalNuevoMedicoEspecialidad').val(),
            Nombre: $('#modalNuevoMedicoNombre').val().trim(),
            Apellido: $('#modalNuevoMedicoApellido').val().trim(),
            Correo: $('#modalNuevoMedicoCorreo').val().trim(),
            Consultorio: $('#modalNuevoMedicoConsultorio').val().trim(),
            Tipo: 'M',
        };
    }
    if (!correcto) {
        MsgAlerta("Atención!", msg, 2000, "default");
    }
    return correcto;
}
// ----------------------- [ MEDICOS ] -----------------------