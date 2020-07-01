// ::::::::::::::::::::::::: VARIABLES GLOBALES :::::::::::::::::::::::::
// ---- VARIABLES GLOBALES ----
var pacienteBusqSelectJSON = {};
var pacienteBusqSelect;

var ejecutarModalBusqPaciente = false;
var ejecutarFuncqBusqPaciente = "";

var modalBusqPacienteCreado = false;
var busqLimiteChrPaciente = 4;

// ::::::::::::::::::::::::: DOCUMENTS - INPUTS :::::::::::::::::::::::::
// ------------------ MANEJO DE BOTONES ESPECIALES ------------------
// ---------- OPERACIONES CON BOTONES [TIENEN EFECTO GLOBAL] ----------
$(document).on('keydown', function (e) {
    var tecla = e.keyCode;
    if (tecla === 113) {
        if (ejecutarModalBusqPaciente) {
            modalBusquedaPaciente();
            e.preventDefault();
        }
    }

    if ($('#modalBusqPaciente').hasClass('show')) {
        if (tecla == 38) {
            var elemento = pacienteBusqSelect.previousElementSibling;
            manejarElemTablaBusqPacientes(elemento);
        } else if (tecla == 40) {
            var elemento = pacienteBusqSelect.nextElementSibling;
            manejarElemTablaBusqPacientes(elemento);
        }

        if (tecla == 17) {
            elegirPacienteBusq();
        }

        if (tecla == 9) {
            e.preventDefault();
        }
    }
});

// DOCUMENT - CONTROLA EL BOTON QUE EJECUTA LA CONSULTA DE PACIENTE  DESDE EL INPUT
$(document).on('click', '#btnNombreModalBusqPaciente', function () {
    consultarPacienteBYNombreClave();
});
// DOCUMENT - CONTROLA LAS TECLAS EJCUTADAS EN EL INPUT DE BUSQUEDA DE PACIENTE
$(document).on('keyup', '#txtNombreModalBusqPaciente', function (e) {
    if (e.keyCode == 13) {
        consultarPacienteBYNombreClave();
    }
});
// DOCUMENT - CONTROLA LA TABLA AL DAR  CLICK SOBRE EL ELEMENTO DE PACIENTE Y APLICA LOS ESTILOS
$(document).on('click', '#tablaCuerpoModalBusqPaciente tr', function () {
    $('tr[name="pacienteActivo"]').css("background-color", "");
    $('tr[name="pacienteActivo"]').attr("name", "");
    $(this).attr("name", "pacienteActivo");
    $(this).css("background-color", "#c3e6cb");
    pacienteBusqSelect = document.getElementsByName("pacienteActivo")[0];
    $('#txtNombreModalBusqPaciente').focus();
});
// DOCUMENT - CONTROLA LA TABLA AL DAR DOBLE  CLICK SOBRE EL ELEMENTO DE PACIENTE Y EJECUTA TECLAR 'CTRL'
$(document).on('dblclick', '#tablaCuerpoModalBusqPaciente tr', function () {
    $('tr[name="pacienteActivo"]').attr("name", "");
    $(this).attr("name", "pacienteActivo");
    elegirPacienteBusq();
});
// DOCUMENT - LLAMAR EL MODAL DE BUSQUEDA PACIENTE (SE REQUIERE QUE EL INPUT Y/O BOTON CONTENGA LA CLASE 'catalogopacientes')
$(document).on('click', '.catalogopacientes', function () {
    modalBusquedaPaciente();
});

// ::::::::::::::::::::::::: FUNCIONES GLOBALES :::::::::::::::::::::::::
// FUNCION QUE EJECUTA LA CREACION DEL MODULO PARA CONSULTAR UN PACIENTE
function modalBusquedaPaciente() {
    if (!modalBusqPacienteCreado) {
        $('body').append(ModalBusqPacienteHTML);
        $('#modalBusqPaciente').on('hidden.bs.modal', function () {
            $('#txtNombreModalBusqPaciente').val('');
            $('#tablaCuerpoModalBusqPaciente').html('');
        });
        modalBusqPacienteCreado = true;
    }
    $('#modalBusqPaciente').modal('show');
    setTimeout(function () {
        $('#txtNombreModalBusqPaciente').focus();
    }, 500);
}

// FUNCION QUE GENERA LA CONSULTA A BD DEL PACIENTE
function consultarPacienteBYNombreClave() {
    if ((isNaN(parseInt($('#txtNombreModalBusqPaciente').val())) && $('#txtNombreModalBusqPaciente').val().length >= busqLimiteChrPaciente) || (!isNaN(parseInt($('#txtNombreModalBusqPaciente').val())) && $('#txtNombreModalBusqPaciente').val().length >= busqLimiteChrPaciente)) {
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/Dinamicos/BusqPacienteNombre",
            data: { PacienteBusqueda: $('#txtNombreModalBusqPaciente').val() },
            beforeSend: function () {
                LoadingOn("Realizando Busqueda...");
            },
            success: function (data) {
                $('#tablaCuerpoModalBusqPaciente').html(data);
                if (data !== "") {
                    pacienteBusqSelect = document.getElementById('pacienteSeleccionado');
                    pacienteBusqSelect.style.backgroundColor = '#c3e6cb';
                    pacienteBusqSelect.focus();
                    $('#divConsejosModalBusqPaciente').html('<p>Use las teclas <b>arriba</b> y <b>abajo</b>...<br>Tecla <b>CTRL</b> para elegir a un paciente... <b>(o doble click)</b></p>')
                } else {
                    $('#divConsejosModalBusqPaciente').html('<span class="badge badge-pill badge-danger">No tiene pacientes para elegir</span>');
                }
                LoadingOff();
            },
            error: function (error) {
                errLog("E20001", error);
            }
        });
    }
}
// FUNCION QUE ELIGE AL PACIENTE DESPUES DE PRESIONAR CTRL (O DOBLE CLICK)
function elegirPacienteBusq() {
    var idPaciente = $('tr[name="pacienteActivo"]').attr("idpaciente");
    if (idPaciente !== undefined) {
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/Dinamicos/BusqPacienteID",
            data: { IDPaciente: idPaciente },
            dataType: 'JSON',
            success: function (data) {
                pacienteBusqSelectJSON = data;
                $('#modalBusqPaciente').modal('hide');
                if (ejecutarFuncqBusqPaciente !== "") {
                    window[ejecutarFuncqBusqPaciente]();
                }
            },
            error: function (error) {
                errLog("E20002", error);
            }
        });
    } else {
        MsgAlerta('Atención!', 'No tiene <b>Paciente</b> para elegir', 2000, 'default');
    }
}
// FUNCION QUE CONTROLA EL ELEMENTO DE PACIENTE SELECCIONADO
function manejarElemTablaBusqPacientes(elem) {
    if (elem != null) {
        pacienteBusqSelect.focus();
        pacienteBusqSelect.style.backgroundColor = '';
        pacienteBusqSelect.setAttribute("name", "");
        elem.focus();
        elem.style.backgroundColor = '#c3e6cb';
        elem.setAttribute("name", "pacienteActivo");
        pacienteBusqSelect = elem;
    }
    $("#tablaModalBusqPaciente").scrollTop($('tr[name="pacienteActivo"]').offset().top + $('#tablaModalBusqPaciente').scrollTop() - $('#tablaModalBusqPaciente').offset().top);
    setTimeout(function () {
        var valor = $('#txtNombreModalBusqPaciente').val();
        $('#txtNombreModalBusqPaciente').val('');
        $('#txtNombreModalBusqPaciente').val(valor);
    }, 500);
}

// ::::::::::::::::::::::::: VARIABLES HTML :::::::::::::::::::::::::
var ModalBusqPacienteHTML = '' +
    '<div id="modalBusqPaciente" class="modal fade" tabindex="-1" role="dialog" data-keyboard="true" data-backdrop="static">' +
    '<div class="modal-dialog" role ="document">' +
    '<div class="modal-content">' +
    '<div class="modal-header"><h5 class="modal-title">Buscar Paciente</h5></div>' +
    '<div id="cuerpoModalBusqPaciente" class="modal-body">' +

    '<div class="row" style="margin-bottom: 5px;">' +
    '<div class="col-md-12">' +
    '<div class="input-group input-group-sm mb-3">' +
    '<input id="txtNombreModalBusqPaciente" type="text" class="form-control" placeholder="Escriba el nombre..." autocomplete="off" />' +
    '<div class="input-group-append">' +
    '<button id="btnNombreModalBusqPaciente" class="btn btn-info btn-secondary" type="button" id="button-addon2"><span class="fa fa-search"></span></button>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '<div class="row" style="margin-bottom: 5px;">' +
    '<div class="col-md-12">' +
    '<div class="card">' +
    '<div class="card-body"> ' +
    '<div id="tablaModalBusqPaciente" class="table table-sm" style="overflow: scroll; max-height: 30vh;">' +
    '<table class="table table-sm table-bordered">' +
    '<tbody id="tablaCuerpoModalBusqPaciente"></tbody>' +
    '</table>' +
    '</div>' +
    '</div> ' +
    '</div>' +
    '</div>' +
    '</div>' +
    '<div class="row" style="margin-bottom: 10px;">' +
    '<div class="col-md-12" id="divConsejosModalBusqPaciente" align="center"><span class="badge badge-pill badge-danger">No tiene pacientes para elegir</span></div>' +
    '</div>' +

    '</div>' +
    '<div class="modal-footer">' +
    '<button id="btnCerrarModalBusqPaciente" class="btn btn-sm btn-secondary" data-dismiss="modal">Cerrar (Tecla ESC)</button>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>';