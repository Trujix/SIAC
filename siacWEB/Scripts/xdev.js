// ::::::::::::::::::::: VARIABLES GLOBALES :::::::::::::::::::::
var TablaEsquemaSELEC = "";
var accesosEspecialesJSON = {};

// ::::::::::::::::::::: DOCUMENTS E INPUTS :::::::::::::::::::::
// DOCUMENT - CONTROLA EL ENTER EN LOS  CAMPOS DEL FOMULARIO DE INICIAR SESION
$(document).on('keyup', '#xdevClaveInput', function (e) {
    if (e.keyCode === 13) {
        $('#xdevBtnIdentificar').click();
    }
});

// DOCUMENT - BOTON QUE ENVIA LOS DATOS PARA IDENTIFICAR USUARIO [ LOGIN ]
$(document).on('click', '#xdevBtnIdentificar', function () {
    if ($('#xdevClaveInput').val() !== "") {
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/XDev/ValidarKey",
            data: { Clave: $('#xdevClaveInput').val() },
            beforeSend: function () {
                LoadingOn("Cargando...");
            },
            success: function (data) {
                LoadingOff();
                if (data === "true") {
                    $('.container').html(XDevCuerpoHTML);
                    $('#xdevactdiv').html(XDevActHTML);
                    $('#xdevsqldiv').html(XDevSQLHTML);
                } else {
                    MsgAlerta("Atencion!", "Clave Incorrecta", 2500, "default");
                    console.log(data);
                }
            },
            error: function (error) {
                LoadingOff();
                MsgAlerta("Error!", "Ocurrió un problema - revise la consola", 2500, "error");
                console.log(error);
            }
        });
    } else {
        $('#xdevClaveInput').focus();
    }
});

// BOTON QUE ENVIA UNA QUERY
$(document).on('click', '#queryEjecutarBtn', function () {
    if ($('#queryEjecutar').val() !== "") {
        var query = $('#queryEjecutar').val();
        LoadingOn(" Ejecutando Query...");
        $('#loadingDiv').attr("data-text", "Actualizando Web Service...");
        $('#queryEjecutar').attr("num", "0");

        $('#queryConsultaResultadoDiv').css("overflow", "");
        $('#queryConsultaResultadoDiv').css("max-height", "");
        var ejecutarQuery = $.ajax({
            type: "POST",
            timeout: 900000,
            contentType: "application/x-www-form-urlencoded",
            url: "/XDev/QueryBD",
            data: { Query: query, Consulta: $('#queryConsultaBtn').is(":checked") },
        });
        ejecutarQuery.done(function (data) {
            if ($('#queryConsultaBtn').is(":checked")) {
                try {
                    $('#queryConsultaResultadoDiv').css("overflow", "scroll");
                    $('#queryConsultaResultadoDiv').css("height", "52vh");
                    var tablaConsulta = JSON.parse(data);
                    if (tablaConsulta.length > 0) {
                        var cont = 0, tabla = '<div class="col-md-12"><div class="table-responsive" style="height: 52vh; overflow: scroll;"><table class="table table-bordered"><thead>*HEADER*</thead><tbody>*CUERPO*</tbody></table></div></div>', headers = '', cuerpo = '';
                        $(tablaConsulta).each(function (key, value) {
                            if (cont === 0) {
                                headers += '<tr>';
                            }
                            cuerpo += '<tr>';
                            var elem = tablaConsulta[key];
                            Object.keys(elem).forEach(function (k) {
                                if (cont === 0) {
                                    headers += '<th class="tablaActualizador">' + k + '</th>';
                                }
                                cuerpo += '<td>' + elem[k] + '</td>';
                            });
                            if (cont === 0) {
                                headers += '</tr>';
                            }
                            cuerpo += '</tr>';
                            cont++;
                        });
                        $('#queryConsultaResultadoDiv').html(tabla.replace("*HEADER*", headers).replace("*CUERPO*", cuerpo));
                        LoadingOff();
                    } else {
                        $('#queryConsultaResultadoDiv').html('<div class="col-md-12" align="center"><span class="badge badge-pill badge-danger">Sin resultados</span></div>');
                        LoadingOff();
                    }
                } catch (e) {
                    LoadingOff();
                    if (data === "securErr") {
                        MsgAlerta('Atención!', 'Ha  caducado la <b>sesión...</b>, la  página se reiniciará en breve...', 2000, 'default');
                        setTimeout(function () {
                            location.reload();
                        }, 2500);
                    } else {
                        if (data === "error") {
                            MsgAlerta('Error!', '<b>Codigo de seguridad equivocado</b>', 2000, 'error');
                        } else {
                            MsgAlerta('Error!', 'Ocurrió un problema al <b>ejecutar query</b>', 2000, 'error');
                            console.log(data);
                        }
                    }
                }
            } else {
                LoadingOff();
                if (data === "true") {
                    MsgAlerta('Éxito!', '<b>Query ejecutada</b>', 2000, 'success');
                } else if (data === "securErr") {
                    MsgAlerta('Atención!', 'Ha  caducado la <b>sesión...</b>, la  página se reiniciará en breve...', 2000, 'default');
                    setTimeout(function () {
                        location.reload();
                    }, 2500);
                } else {
                    if (data === "error") {
                        MsgAlerta('Error!', '<b>Codigo de seguridad equivocado</b>', 2000, 'error');
                    } else {
                        MsgAlerta('Error!', 'Ocurrió un problema al <b>ejecutar query</b>', 2000, 'error');
                        console.log(data);
                    }
                }
            }
        });
        ejecutarQuery.fail(function (error) {
            LoadingOff();
            MsgAlerta('Error!', 'Ocurrió un problema al <b>ejecutar query</b>', 2000, 'error');
            console.log(error);
        });
    } else {
        MsgAlerta('Atención!', 'Coloque <b>la query</b>', 2000, 'default');
        $('#queryEjecutar').focus();
    }
});

// BOTON QUE RETORNA ESQUEMA DE BD
var esquemaJSONGLOBAL = {};
$(document).on('click', '#querySchemaBtn', function () {
    $('#queryConsultaResultadoDiv').css("overflow", "");
    $('#queryConsultaResultadoDiv').css("max-height", "");
    var query = $('#queryEjecutar').val();
    LoadingOn(" Verificando Estructura BD...");
    var traerEsquema = $.ajax({
        type: "POST",
        timeout: 900000,
        contentType: "application/x-www-form-urlencoded",
        url: "/XDev/EsquemaBD"
    });
    traerEsquema.done(function (data) {
        $('#queryConsultaResultadoDiv').css("overflow", "scroll");
        try {
            esquemaJSONGLOBAL = JSON.parse(data), tablaEsquema = '<div class="accordion" id="acordionEsquema"';
            Object.keys(esquemaJSONGLOBAL).forEach(function (k) {
                tablaEsquema += '<div class="col-md-2" style="margin-bottom: 5px;"><div class="card">' +
                    '<div class="card-header">' +
                    '<h6 class="mb-0">' +
                    '<a data-toggle="collapse" class="esquemaTitulo" aria-expanded="false" accion="0" tabla="' + k + '" elem="' + k + '" title="' + k + '" style="cursor: pointer;">' + k + '</a>' +
                    '</h6>' +
                    '</div>' +
                    '<div id="' + k + '" class="collapseEsquema collapse" data-parent="#acordionEsquema">' +
                    '<div class="panel-body"><div class="row" style="overflow: scroll;"><div class="col-md-12"><div class="table-responsive"><table class="table table-bordered"><thead><tr><th>Columna</th><th>Tipo</th></tr></thead><tbody id="tbody_' + k + '"></tbody></table></div></div></div></div>' +
                    '</div>' +
                    '</div></div>';
            });
            tablaEsquema += '</div>';
            $('#queryConsultaResultadoDiv').html(tablaEsquema);
            LoadingOff();

            $('.collapseEsquema').on('shown.bs.collapse', function () {
                var tablaInfo = '';
                $(esquemaJSONGLOBAL[TablaEsquemaSELEC]).each(function (key, value) {
                    tablaInfo += '<tr><td>' + value.NombreColumna + '</td><td>' + value.TipoDato + '</td></tr>';
                });
                $('#tbody_' + TablaEsquemaSELEC).html(tablaInfo);
                LoadingOff();
            });
            $('.collapseEsquema').on('hidden.bs.collapse', function () {
                $('#tbody_' + TablaEsquemaSELEC).html('');
                LoadingOff();
            });
        } catch (e) {
            LoadingOff();
            if (data === "error") {
                MsgAlerta('Error!', '<b>Codigo de seguridad equivocado</b>', 2000, 'error');
            } else if (data === "securErr") {
                MsgAlerta('Atención!', 'Ha  caducado la <b>sesión...</b>, la  página se reiniciará en breve...', 2000, 'default');
                setTimeout(function () {
                    location.reload();
                }, 2500);
            } else {
                MsgAlerta('Error!', 'Ocurrió un problema al <b>traer esquema</b>', 2000, 'error');
                console.log(data);
            }
        }
    });
    traerEsquema.fail(function (error) {
        LoadingOff();
        MsgAlerta('Error!', 'Ocurrió un problema al <b>traer esquema</b>', 2000, 'error');
        console.log(error);
    });
});

// BOTON QUE MUESTRA ELEMENTOS DE LA TABLA AL TRAER ESQUEMA
$(document).on('click', '.esquemaTitulo', function () {
    TablaEsquemaSELEC = $(this).attr("tabla");
    LoadingOn(" Generando esquema...");
});

// BOTON QUE MANIPULA LOS COLLAPSE
$(document).on('click', 'a[data-toggle="collapse"]', function () {
    if (parseInt($(this).attr("accion")) > 0) {
        $(this).attr("accion", "0");
        $(this).attr("aria-expanded", "false");
        $('#' + $(this).attr("elem")).collapse('hide');
    } else {
        $(this).attr("accion", "1");
        $(this).attr("aria-expanded", "true");
        $('#' + $(this).attr("elem")).collapse('show');
    }
});

// BOTON QUE EJECUTA LA ACTUALIZACION DE LA APLICACION
$(document).on('click', '#urlAplicacionBtn', function () {
    if ($('#urlAplicacion').val() !== "") {
        var url = $('#urlAplicacion').val();
        if (validarURLActualizador(url)) {
            LoadingOn(" Actualizando APP...");
            var actualizar = $.ajax({
                type: "POST",
                timeout: 900000,
                contentType: "application/x-www-form-urlencoded",
                url: "/XDev/ActualizarApp",
                data: { URL: url, Credenciales: accesosEspecialesJSON },
            });
            actualizar.done(function (data) {
                LoadingOff();
                if (data === "true") {
                    MsgAlerta('Éxito!', '<b>Aplicación Web actualizada</b>', 2000, 'success');
                    $('#urlWebServiceBtn').val('');
                } else if (data === "securErr") {
                    MsgAlerta('Atención!', 'Ha  caducado la<b>sesión...</b>, la  página se reiniciará en breve...', 2000, 'default');
                    setTimeout(function () {
                        location.reload();
                    }, 2500);
                } else {
                    if (data === "error") {
                        MsgAlerta('Error!', '<b>Codigo de seguridad equivocado</b>', 2000, 'error');
                    } else {
                        MsgAlerta('Error!', 'Ocurrió un problema al <b>actualizar aplicación</b>', 2000, 'error');
                        console.log(data);
                    }
                }
            });
            actualizar.fail(function (error) {
                LoadingOff();
                MsgAlerta('Error!', 'Ocurrió un problema al <b>actualizar aplicación</b>', 2000, 'error');
                console.log(error);
            });
        } else {
            MsgAlerta('Atención!', 'La <b>la url de archivo de actualización es incorrecta</b>', 2000, 'default');
        }
    } else {
        MsgAlerta('Atención!', 'Coloque <b>la url de archivo de actualización</b>', 2000, 'default');
    }
});

// ::::::::::::::::::::: FUNCIONES GLOBALES :::::::::::::::::::::
// FUNCION INICIAL DEL CPANEL
$(function () {
    $('#xdevClaveInput').focus();
});

// FUNCION QUE VERIFICA SI LA CADENA DE LAS URL DE ARCHIVOS DE ACTUALIZACION ES UNA DIRECCION VÁLIDA
function validarURLActualizador(url) {
    var pattern = new RegExp(/(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)/g);
    if (!pattern.test(url)) {
        return false;
    } else {
        return true;
    }
}

// -------------- VALORES HTML DINAMICOS ---------------------
var actualizadorHTML = '' +
    '<div class="row">' +
    '<div class="col-md-12">' +
    '<div class="accordion" id="acordionPadre">' +

    /*'<div class="card">' +
    '<div class="card-header" id="headerActualizador">' +
    '<h5 class="mb-0">' +
    '<a data-toggle="collapse" accion="0" elem="actualizadorCollapse" aria-expanded="false" style="cursor: pointer;"><span class="fa fa-cloud-upload"></span>&nbsp;Actualizadores</a>' +
    '</h5>' +
    '</div>' +
    '<div id="actualizadorCollapse" class="collapse" aria-labelledby="headerActualizador" data-parent="acordionPadre">' +
    '<div class="card-body">' +
    '<div class="col-md-12">' +
    '<div class="panel panel-default">' +
    '<div class="panel-heading">' +
    '<h6 class="panel-title">Actualizar Aplicación</h6>' +
    '</div>' +
    '<div class="panel-body">' +
    '<div class="row">' +
    '<div class="col-md-10">' +
    '<input placeholder="URL de archivo de actualización..." id="urlAplicacion" type="text" class="form-control" />' +
    '</div>' +
    '<div class="col-md-2">' +
    '<button id="urlAplicacionBtn" class="btn btn-sm btn-success"><span class="fa fa-rocket"></span>&nbsp;Actualizar</button>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div><p></p>' +
    '<div class="col-md-12">' +
    '<div class="panel panel-default">' +
    '<div class="panel-heading">' +
    '<h6 class="panel-title">Actualizar Web Service</h6>' +
    '</div>' +
    '<div class="panel-body">' +
    '<div class="row">' +
    '<div class="col-md-10">' +
    '<input placeholder="URL de archivo de actualización..." id="urlWebService" type="text" class="form-control" />' +
    '</div>' +
    '<div class="col-md-2">' +
    '<button id="urlWebServiceBtn" class="btn btn-sm btn-info"><span class="fa fa-rocket"></span>&nbsp;Actualizar</button>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div><p></p>' +
    '<div class="col-md-12">' +
    '<div class="row">' +
    '<div class="col-md-1"></div>' +
    '<div class="col-md-10" align="center">' +
    '<div class="alert alert-info" role="alert">' +
    '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>' +
    '<span class="sr-only">Error:</span>&nbsp;' +
    'Utilice la siguiente url para usar en <b>Google Drive</b>: https://docs.google.com/uc?export=download&id= + <b>ID</b>' +
    '</div>' +
    '</div>' +
    '<div class="col-md-1"></div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '<br>' +*/

    '<div class="card">' +
    '<div class="card-header" id="headerQuery">' +
    '<h5 class="mb-0">' +
    '<a data-toggle="collapse" accion="0" elem="queryCollapse" aria-expanded="false" style="cursor: pointer;"><span class="fa fa-hdd-o"></span>&nbsp;Ejecutar Query</a>' +
    '</h5>' +
    '</div>' +
    '<div id="queryCollapse" class="collapse" aria-labelledby="headerQuery" data-parent="#acordionPadre">' +
    '<div class="card-body">' +
    '<div class="col-md-12">' +
    '<div class="card border-dark">' +
    '<div class="card-body">' +
    '<div class="row">' +
    '<div class="col-md-8">' +
    '<textarea id="queryEjecutar" num="0" type="text" class="form-control"></textarea>' +
    '</div>' +
    '<div class="col-md-2" align="right">' +
    '<input id="queryConsultaBtn" type="checkbox">&nbsp;Consulta' +
    '</div>' +
    '<div class="col-md-1">' +
    '<button id="queryEjecutarBtn" class="btn btn-sm btn-primary"><span class="fa fa-send"></span>&nbsp;Ejecutar</button>' +
    '</div>' +
    '<div class="col-md-1">' +
    '<button id="querySchemaBtn" class="btn btn-sm btn-warning" title="Ver Esquema BD"><span class="fa fa-th-list"></span></button>' +
    '</div>' +
    '</div><p></p>' +
    '<div id="queryConsultaResultadoDiv" class="row"></div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +
    '</div>' +

    '</div>' +
    '</div>' +
    '</div>';


var XDevCuerpoHTML = '' +
    '<ul class="nav nav-tabs" id="myTab" role="tablist">' +
        '<li class="nav-item" role = "presentation" > ' +
            '<a class="nav-link active" id="actualizador-tab" data-toggle="tab" href="#actualizador" role="tab" aria-controls="actualizador" aria-selected="true"><i class="fa fa-wrench"></i> Actualizador</a>' +
        '</li> ' +
        '<li class="nav-item" role="presentation">' +
            '<a class="nav-link" id="sql-tab" data-toggle="tab" href="#sql" role="tab" aria-controls="sql" aria-selected="false"><i class="fa fa-database"></i> Gestor SQL</a>'+
        '</li>'+
    '</ul> '+
    '<div class="tab-content" id="myTabContent">'+
        '<div class="tab-pane fade show active" id="actualizador" role="tabpanel" aria-labelledby="actualizador-tab">'+
            '<div class="row">'+
                '<div class="col-sm-12">'+
                    '<div class="card">'+
                        '<div id="xdevactdiv" class="card-body"></div>'+
                    '</div>'+
                '</div>'+
            '</div>'+
        '</div>'+
        '<div class="tab-pane fade" id="sql" role="tabpanel" aria-labelledby="sql-tab">'+
            '<div class="row">'+
                '<div class="col-sm-12">'+
                    '<div class="card">'+
                        '<div id="xdevsqldiv" class="card-body"></div>'+
                    '</div>'+
                '</div>'+
            '</div>'+
        '</div>'+
    '</div>';

var XDevActHTML = '' +
    '<div class="row">' +
        '<div class="col-md-12">' +
            '<div class="panel panel-default">' +
                '<div class="panel-heading">' +
                    '<h6 class="panel-title">Actualizar Sistema</h6>' +
                '</div>' +
                '<div class="panel-body">' +
                    '<div class="row">' +
                        '<div class="col-md-10">' +
                            '<input placeholder="URL de archivo de actualización..." id="urlAplicacion" type="text" class="form-control form-control-sm" />' +
                        '</div>' +
                        '<div class="col-md-2">' +
                            '<button id="urlAplicacionBtn" class="btn btn-block btn-sm btn-success"><span class="fa fa-rocket"></span>&nbsp;Actualizar</button>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</div>' +
        '</div>' +
    '</div>' +
    '<div class="row" style="padding-top: 10px;">' +
        '<div class="col-md-1"></div>' +
        '<div class="col-md-10" align="center">' +
            '<div class="alert alert-info" role="alert">' +
                '<span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>' +
                '<span class="sr-only">Error:</span>&nbsp;' +
                'Utilice la siguiente url para usar en <b>Google Drive</b>: https://docs.google.com/uc?export=download&id= + <b>ID</b>' +
            '</div>' +
        '</div>' +
        '<div class="col-md-1"></div>' +
    '</div>' +
'';

var XDevSQLHTML = '' +
    '<div class="row">' +
        '<div class="col-md-7">' +
            '<textarea id="queryEjecutar" num="0" type="text" class="form-control"></textarea>' +
        '</div>' +
        '<div class="col-md-2" align="right">' +
            '<input id="queryConsultaBtn" type="checkbox">&nbsp;Consulta' +
        '</div>' +
        '<div class="col-md-2">' +
            '<button id="queryEjecutarBtn" class="btn btn-sm btn-block btn-primary"><span class="fa fa-paper-plane"></span>&nbsp;Ejecutar</button>' +
        '</div>' +
        '<div class="col-md-1">' +
            '<button id="querySchemaBtn" class="btn btn-sm btn-block btn-warning" title="Ver Esquema BD"><span class="fa fa-th-list"></span></button>' +
        '</div>' +
    '</div>' +
    '<div id="queryConsultaResultadoDiv" class="row" style="padding-top: 10px;"></div>';