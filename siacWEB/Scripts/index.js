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

// DOCUMENT - CONTROLA EL CAMBIO DEL TAMAÑO DE DIV PRINCIPAL PARA QUE LO REASIGNE DE TAMAÑO
$('.right_col').attrchange({
    trackValues: true,
    callback: function (event) {
        $('.right_col').css("min-height", "100vh");
    }
});

$(document).on('click', '#menu_togglemobil', function () {
    $('#menu_toggle').click();
});
// ---------------- ACCIONES ESPECIALES ----------------

// -------------------- MANEJO DE LA SESION --------------------
// DOCUMENT - QUE CIERRA LA SESION
$(document).on('click', '.cerrarSesion', function () {
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
$(document).on('click', '.usuarioInfo', function () {
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
            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                $('.right_col').before(PanelCelularHTML);
            }
            LoadingOn("Favor Espere...");
        },
        success: function (data) {
            if (data.NombreUsuario !== undefined) {
                $('.menuUsuarioNombre').html(data.NombreUsuario);
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

// ::::::::::::::::: ELEMENTOS DOM HTML :::::::::::::::::
var PanelCelularHTML = '' +
    '<div class="top_nav">' +
    '<div class="nav_menu">' +
    '<div class="nav toggle">' +
    '<a id="menu_togglemobil" class="menu_abrirbtn"><i class="fa fa-bars"></i></a>' +
    '</div>' +
    '<nav class="nav navbar-nav">' +
    '<ul class=" navbar-right">' +
    '<li class="nav-item dropdown open" style="padding-left: 15px;">' +
    '<a class="user-profile dropdown-toggle" aria-haspopup="true" id="navbarDropdown" data-toggle="dropdown" aria-expanded="false">' +
    '<img src="../Media/usuariodefault.png" alt=""><i class="menuUsuarioNombre">--</i>' +
    '</a>' +
    '<div class="dropdown-menu dropdown-usermenu pull-right" aria-labelledby="navbarDropdown">' +
    '<a class="configuracionClinica dropdown-item"><i class="fa fa-cog"></i> Configuracion</a>' +
    '<a class="usuarioInfo dropdown-item"><i class="fa fa-user"></i> Usuario</a>' +
    '<a class="usuarioNotificaciones dropdown-item"><i class="fa fa-envelope"></i> Mensajes</a>' +
    '<a class="cerrarSesion dropdown-item"><i class="fa fa-power-off"></i> Cerrar Sesión</a>' +
    '</div>' +
    '</li>' +
    '</ul>' +
    '</nav>' +
    '</div>' +
    '</div>' ;