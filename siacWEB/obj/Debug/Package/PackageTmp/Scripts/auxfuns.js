// :::::::::::: FUNCIONES AUXILIARES DE MULTIPLE USO ::::::::::::

// FUNCION QUE DEVUELVE LA FECHA DE HOY (LO MAS CERCANO AL DIA DE  HOY)
function FechaHoy() {
    var d = new Date(Date.UTC(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), new Date().getHours(), new Date().getMinutes(), new Date().getSeconds()));
    var dArr = d.toString().split(" ");
    var dN = new Date(dArr[0] + " " + dArr[1] + " " + dArr[2] + " " + dArr[3] + " " + dArr[4] + " GMT-1000 (CDT)");
    alert(dN);
    return dN;
}

// FUNCION QUE GENERA UNA FECHA PARA ASIGANR A LOS INPUTS TIPO  FECHA
function FechaInput() {
    var hoy = new Date();
    var dd = hoy.getDate();
    var mm = hoy.getMonth() + 1;
    var yyyy = hoy.getFullYear();
    if (dd < 10) {
        dd = '0' + dd
    }
    if (mm < 10) {
        mm = '0' + mm
    }
    return yyyy + '-' + mm + '-' + dd;
}

// FECHA QUE DEVUELVE LA INFO DE UNA FECHA (1 - INPUT DATE)
function fechaInfo(fecha, tipo) {
    var d, mes = 0, dInfo = {
        AnoNum: 0,
        MesNum: 0,
        DiaNum: 0,
        MesTxt: "",
        DiaTxt: "",
    };
    var dias = {
        Mon: "Lunes",
        Tue: "Martes",
        Wed: "Miercoles",
        Thu: "Jueves",
        Fri: "Viernes",
        Sat: "Sabado",
        Sun: "Domingo",
    };
    var meses = {
        Jan: "Enero",
        Feb: "Febrero",
        Mar: "Marzo",
        Apr: "Abril",
        May: "Mayo",
        Jun: "Junio",
        Jul: "Julio",
        Aug: "Agosto",
        Sep: "Septiembre",
        Oct: "Octubre",
        Nov: "Noviembre",
        Dec: "Diciembre",
    }
    if (tipo === 1) {
        var f = fecha.split("-");
        d = new Date(parseInt(f[0]), parseInt(f[1]) - 1, parseInt(f[2]), 0, 0, 0, 0);
        mes = parseInt(f);
    }
    var dArr = d.toString().split(" ");
    dInfo.AnoNum = dArr[3];
    dInfo.MesNum = mes;
    dInfo.DiaNum = dArr[2];
    dInfo.MesTxt = meses[dArr[1]];
    dInfo.DiaTxt = dias[dArr[0]];
    return dInfo;
}

// FUNCION QUE DEVUELVE UNA FECHA DE 24 HRS EN 12 HRS (CON AM O PM)
function reloj12hrs(hr) {
    var hrs = hr.split(":"), hrReturn = '';
    if (parseInt(hrs[0]) === 0) {
        hrReturn = "12:" + hrs[1] + " a.m.";
    } else if (parseInt(hrs[0]) > 12) {
        hrReturn = (((parseInt(hrs[0]) - 12) < 10) ? "0" + (parseInt(hrs[0]) - 12).toString() : (parseInt(hrs[0]) - 12).toString()) + ":" + hrs[1] + " p.m.";
    } else {
        hrReturn = ((parseInt(hrs[0]) < 10) ? "0" + parseInt(hrs[0]).toString() : hrs[0]) + ":" + hrs[1] + " a.m.";
    }
    return hrReturn;
}

// FUNCION QUE VALIDA SI UNA CADENA DE MAIL ES VALIDA
function esEmail(email) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

// FUNCION QUE CREA CADENA ALEATORIA (LONGITUD QUE SE NECESITE)
function cadAleatoria(lng) {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz123456789";
    for (var r = 0; r < lng; r++) {
        text += possible.charAt(Math.floor(Math.random() * possible.length));
    }
    return text;
}

// FUNCION QUE DEVUELVE UN NUMERO ALEATORIO DE ACUERDO A LA LONGITUD
function numAleatorio(lng) {
    var num = "";
    for (i = 0; i < lng; i++) {
        num += "9";
    }
    return Math.floor(Math.random() * parseInt(num)) + 1;
}

// FUNCION QUE GENERA UN ID DE USUARIO
function generarUsuarioID(array, prefij) {
    var numAl = ""
    do {
        numAl = prefij + numAleatorio(4);
    } while (array.includes(numAl));
    return numAl;
}

// FUNCION PROTOTIPICA DE [SPLICE]
if (!String.prototype.splice) {
    String.prototype.splice = function (start, delCount, newSubStr) {
        return this.slice(0, start) + newSubStr + this.slice(start + Math.abs(delCount));
    };
}