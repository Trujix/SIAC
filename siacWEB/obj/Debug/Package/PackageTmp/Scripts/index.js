// ::::::::::::::: VARIABLES GLOBALES :::::::::::::::
var MenuOpsJSON = {
    registrarcita: {
        Controller: "Citas",
        Vista: "RegistrarCita",
        FuncionInicial: true,
    },
    pagarconsulta: {
        Controller: "Citas",
        Vista: "PagarCita",
        FuncionInicial: true,
    },
}

// ::::::::::::::: DOCUMENT - INPUTS GENERALES :::::::::::::::
// ---------------- ACCIONES ESPECIALES ----------------
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
    if (TablaCitasHTML !== undefined) {
        TablaCitasHTML.columns.adjust();
    }
});

$('.right_col').resize(function () {
    alert('fsduhj');
});
// ---------------- ACCIONES ESPECIALES ----------------

// -------------------- MANEJO DE LA SESION --------------------
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
// -------------------- MANEJO DE LA SESION --------------------

// -------------------- INFORMACION DE USUARIO --------------------
// DOCUMENT - BOTON QUE LLAMA LA VISTA PARA CONFIGURAR LA INFO DEL USUARIOS
$(document).on('click', '#usuarioInfo', function () {
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Home/UsuarioInfo",
        beforeSend: function () {
            LoadingOn("Cargando Menu...");
        },
        success: function (data) {
            $('#vistaPrincipal').html(data);
            LoadingOff();
        },
        error: function (error) {
            errLog("E30001", error.responseText);
        }
    });
});
// -------------------- INFORMACION DE USUARIO --------------------

// ::::::::::::::: FUNCIONES GENERALES :::::::::::::::
// ---------------- ACCIONES ESPECIALES ----------------
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
                $('.right_col').css("min-height", "100vh");
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
// ---------------- ACCIONES ESPECIALES ----------------

// -------------------- INFORMACION DE USUARIO --------------------
// -------------------- INFORMACION DE USUARIO --------------------