// :::::::::::::::::::::: FUNCIONES GENERALES ::::::::::::::::::::::
// FUNCION QUE EMITE UN MENSAJE DE ERROR DE SISTEMA
function errLog(clave, error) {
    LoadingOff();
    MsgAlerta("Error!", "Ocurrió un error en el sistema: Clave - <b>" + clave + "</b>", 4000, "error");
    ErrorLogJSON[clave].ErrDescripcion = error;
    if (LogConsola) {
        console.log(error);
    }
}

// :::::::::::::::::::::: VARIABLES GLOBALES ::::::::::::::::::::::
var LogConsola = true;
var ErrDescripcion;
var ErrorLogJSON = {
    E001: {
        Detalle: "Iniciar Sesion",
        ErrorReciente: "",
    },
    E002: {
        Detalle: "Cerrar Sesion",
        ErrorReciente: "",
    },
};