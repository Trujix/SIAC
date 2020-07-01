// ::::::::::::::: VARIABLES GLOBALES :::::::::::::::
var MenuOpsJSON = {
    registrarconsulta: {
        Controller: "Consultas",
        Vista: "RegistrarConsulta",
        FuncionInicial: true,
    },
    pagarconsulta: {
        Controller: "Consultas",
        Vista: "PagarConsulta",
        FuncionInicial: true,
    },
}

// ::::::::::::::: DOCUMENT - INPUTS GENERALES :::::::::::::::
// DOCUMENT - QUE CIERRA LA SESION
$(document).on('click', '#cerrarSesion', function () {
    MsgPregunta("Cerrar Sesión", "¿Desea continuar?", function (si) {
        if (si) {
            $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                url: "/Home/CerrarSesion",
                beforeSend: function () {
                    LoadingOn("Favor Espere...");
                },
                success: function (data) {
                    if (data === "true") {
                        location.reload();
                    } else {
                        errLog("E004", data);
                    }
                },
                error: function (error) {
                    errLog("E004", error);
                }
            });
        }
    });
});

// DOCUMENT - CONTROLA LAS EJECUCIONES DEL MENU
$(document).on('click', '.child_menu li a', function () {
    var op = $(this).attr("op");
    if (op !== undefined) {
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/" + MenuOpsJSON[op].Controller + "/" + MenuOpsJSON[op].Vista,
            beforeSend: function () {
                LoadingOn("Cargando Parametros...");
            },
            success: function (data) {
                $('#vistaPrincipal').html(data);
                LoadingOff();
                if (MenuOpsJSON[op].FuncionInicial) {
                    window["ini" + MenuOpsJSON[op].Vista]();
                }
            },
            error: function (error) {
                errLog("E002", error);
            }
        });
    }
});

// DOCUMENT - BOTON QUE EJECUTA UNA ACCION ESPECIAL  DE REASIGNACION DE TAMAÑO DE TABLAS AL MINIMIZAR MENU
$(document).on('click', '.site_title', function () {
    TablaCitasHTML.columns.adjust();
});

// ::::::::::::::: FUNCIONES GENERALES :::::::::::::::
// FUNCION DE ARRANQUE
$(function () {
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Home/UsuarioParams",
        dataType: 'JSON',
        beforeSend: function () {
            LoadingOn("Favor Espere...");
        },
        success: function (data) {
            if (data.NombreUsuario !== undefined) {
                $('#menuUsuarioNombre').html(data.NombreUsuario);
                LoadingOff();
            } else {
                errLog("E003", data.responseText);
            }
        },
        error: function (error) {
            errLog("E003", error.responseText);
        }
    });
});