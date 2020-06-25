// ::::::::::::::: VARIABLES GLOBALES :::::::::::::::

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
                        errLog("E002", error.responseText);
                    }
                },
                error: function (error) {
                    errLog("E002", error.responseText);
                }
            });
        }
    });
});