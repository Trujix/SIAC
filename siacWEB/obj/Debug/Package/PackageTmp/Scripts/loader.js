﻿// ::::::::: ECHANDO A ANDAR TODO :::::::::

// FUNCION QUE MUESTRA EL MENSAJE DE 'CARGANDO'
function LoadingOn(texto) {
    LoadingOff();
    var dataText = (texto !== undefined || texto === "") ? texto : "Espere...";
    $('body').append('<div id="loadingDiv" class="loader loader-default is-active" data-text="' + dataText + '" data-blink data-half></div>');
}
// FUNCION QUE REMUEVE EL MENSAJE DE 'CARGANDO'
function LoadingOff() {
    $("#loadingDiv").remove();
}