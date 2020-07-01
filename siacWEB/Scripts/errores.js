// :::::::::::::::::::::: FUNCIONES GENERALES ::::::::::::::::::::::
// FUNCION QUE EMITE UN MENSAJE DE ERROR DE SISTEMA
function errLog(clave, error) {
    LoadingOff();
    if (document.cookie !== "") {
        MsgAlerta("Error!", "Ocurrió un error en el sistema: Clave - <b>" + clave + "</b>", 4000, "error");
    } else {
        MsgAlerta("Atención!", "La sesión ha caducado, <b>por favor espere...</b>", 2000, "default");
        setTimeout(function () {
            location.reload();
        }, 2300);
    }
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
    E003: {
        Detalle: "Parametros de Usuario",
        ErrorReciente: "",
    },
    E004: {
        Detalle: "Cargar Vista de Menu",
        ErrorReciente: "",
    },
    E005: {
        Detalle: "Parametros de Registro Consulta",
        ErrorReciente: "",
    },
    E006: {
        Detalle: "Cargando Citas Medico - Nueva Cita",
        ErrorReciente: "",
    },
    E007: {
        Detalle: "Guardando Nueva Cita",
        ErrorReciente: "",
    },
    E008: {
        Detalle: "Cargando Lista Citas",
        ErrorReciente: "",
    },
    E009: {
        Detalle: "Cancelando Citas",
        ErrorReciente: "",
    },
    E010: {
        Detalle: "Reenviar Mail Cita",
        ErrorReciente: "",
    },
    E011: {
        Detalle: "Cargar Lista Citas Pagar",
        ErrorReciente: "",
    },
    E012: {
        Detalle: "Pago Consulta",
        ErrorReciente: "",
    },

    E20001: {
        Detalle: "Consulta Dinamica Pacientes",
        ErrorReciente: "",
    },
    E20002: {
        Detalle: "Consulta Dinamica Pacientes - Elecc",
        ErrorReciente: "",
    },
};