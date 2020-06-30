// ::::::::::::::: VARIABLES GLOBALES :::::::::::::::
var loginFormJSON = {};

// ::::::::::::::: DOCUMENTS - INPUTS GENERALES :::::::::::::::

// DOCUMENT - BOTON QUE INICIA SESION
$(document).on('click', '#iniciarSesionBtn', function () {
    if (veriFormLogin()) {
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/Home/IniciarSesion",
            data: { LoginData: loginFormJSON },
            dataType: 'JSON',
            beforeSend: function () {
                LoadingOn("Verificando Usuario...");
            },
            success: function (data) {
                if (data.Respuesta !== undefined) {
                    if (data.Respuesta) {
                        location.reload();
                    } else {
                        if (data.Error === "errLogin") {
                            LoadingOff();
                            MsgAlerta("Error!", "Usuario y/o contraseña <b>incorrectos</b>", 2000, "error");
                        } else {
                            errLog("E001", data.Error);
                        }
                    }
                } else {
                    errLog("E001", data.responseText);
                }
            },
            error: function (error) {
                errLog("E001", error.responseText);
            }
        });
    }
});

// DOCUMENT - CONTROLA EL ENTER EN LOS  CAMPOS DEL FOMULARIO DE INICIAR SESION
$(document).on('keyup', '#container, #passTxt, #clinicaClaveTxt', function (e) {
    if (e.keyCode === 13) {
        $('#iniciarSesionBtn').click();
    }
});

// ::::::::::::::: FUNCIONES GENERALES :::::::::::::::

// -------------- [ FUNCION DE INICIO ] --------------
// TRANSICIÓN INICIAL DEL INICIO DE SESIÓN
$(function () {
    setTimeout(function () {
        $('.login-button').fadeOut("slow", function () {
            $("#container").fadeIn();
            $('#usuarioTxt').val('').focus();
            $('#passTxt').val('');
        });
    }, 1500);
});
// -------------- [ FUNCION DE INICIO ] --------------

// FUNCION QUE VALIDA EL FORMULARIO DE LOGIN
function veriFormLogin() {
    var correcto = true, msg = "";
    if ($('#usuarioTxt').val() === "") {
        correcto = false;
        msg = "Coloque el <b>Usuario</b>";
        $('#usuarioTxt').focus();
    } else if ($('#passTxt').val() === "") {
        correcto = false;
        msg = "Coloque la <b>Contraseña</b>";
        $('#passTxt').focus();
    } else {
        loginFormJSON = {
            Usuario: $('#usuarioTxt').val(),
            Pass: $('#passTxt').val(),
            ClaveClinica: "9999"/*$('#claveClinicaTxt').val()*/,
        }
    }
    if (!correcto) {
        MsgAlerta("Atención!", msg, 2900, "default");
    }
    return correcto;
}