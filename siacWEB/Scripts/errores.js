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
    E013: {
        Detalle: "Parametros Nuevo Medico",
        ErrorReciente: "",
    },
    E014: {
        Detalle: "Guardar Nuevo Medico",
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
    E20003: {
        Detalle: "Menu Configuracion Medico",
        ErrorReciente: "",
    },
    E20004: {
        Detalle: "Guardar Horario Medico",
        ErrorReciente: "",
    },

    E30001: {
        Detalle: "Menu Usuario Info",
        ErrorReciente: "",
    },
    E30002: {
        Detalle: "Usuario Params. Info",
        ErrorReciente: "",
    },
    E30003: {
        Detalle: "Guardar Img. Usuario",
        ErrorReciente: "",
    },
    E30004: {
        Detalle: "Eliminar Img. Usuario",
        ErrorReciente: "",
    },
    E30005: {
        Detalle: "Cambiar Usuario Password",
        ErrorReciente: "",
    },
    E30006: {
        Detalle: "Guardar Info Usuario",
        ErrorReciente: "",
    },
};