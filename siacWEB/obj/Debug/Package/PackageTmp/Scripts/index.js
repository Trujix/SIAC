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
    medicos: {
        Controller: "Administracion",
        Vista: "Medicos",
        FuncionInicial: true,
    },
}
var UsuarioInfoDataJSON = {};
var ImgUsuarioFILE;
var CropperImgUSER;
var UsuarioTiposJSON = {
    usuA: '<i class="fa fa-user-shield"></i> Administrador',
    usuM: '<i class="fa fa-stethoscope"></i> Medico',
    usuU: '<i class="fa fa-user"></i> Usuario',
}
var UsuarioInfoVALSJSON = {};
var UsuarioConfigMenuJSON = {
    usuA: {
        Vista: 'ConfigAdmin',
        Titulo: 'Configuración - Administrador',
    },
    usuM: {
        Vista: 'ConfigMedico',
        Titulo: 'Configuración - Médico',
    },
    usuU: {
        Vista: 'ConfigUsuario',
        Titulo: 'Configuración - Usuario',
    },
}
var MedicoHorarioDiaSELEC = '';
var MedicoConfigParamsJSON = {};
var MedicoConfigHorarioJSON = {};

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

// -------------------- CONFIGURACIÓN DE USUARIO [ MEDICO ] --------------------
// DOCUMENT - BOTON QUE GUARDA LA CONFIGURACION DE UNA CLINICA
$(document).on('click', '.configuracionClinica', function () {
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Home/ConUsuarioInfo",
        dataType: 'JSON',
        beforeSend: function () {
            LoadingOn("Espere...");
        },
        success: function (data) {
            var IdUsuario = data.IdUsuario;
            var Json = UsuarioConfigMenuJSON["usu" + data.Tipo];
            $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                url: "/Home/" + Json.Vista,
                beforeSend: function () {
                    LoadingOn("Cargando Parametros...");
                },
                success: function (data) {
                    $('#vistaPrincipal').html(data);
                    $.ajax({
                        type: "POST",
                        contentType: "application/x-www-form-urlencoded",
                        url: "/Home/ConfMedicoParams",
                        dataType: 'JSON',
                        data: { IdUsuario: IdUsuario },
                        beforeSend: function () {
                            LoadingOn("Cargando Parametros...");
                        },
                        success: function (data) {
                            if (data.HorariosMedico !== undefined) {
                                $('#modalConfigTitulo').html(Json.Titulo);
                                MedicoConfigHorarioJSON = data.HorariosMedico;
                                MedicoConfigHorarioJSON["IdUsuario"] = IdUsuario;
                                llenarTablaMedicoHorariosConfig();

                                $('#ModalConfiguracion').modal('show');
                                LoadingOff();
                            } else {
                                errLog("E20003", data.responseText);
                            }
                        },
                        error: function (error) {
                            errLog("E20003", error.responseText);
                        }
                    });
                },
                error: function (error) {
                    errLog("E20003", error);
                }
            });
        },
        error: function (error) {
            errLog("E20003", error);
        }
    });
});

// DOCUMENT - BOTON QUE ABRE UN MODAL PARA CONFIGURAR UN NUEVO HORARIO
$(document).on('click', '.modalconfhor', function () {
    MedicoHorarioDiaSELEC = $(this).parent().attr("id").replace("modalConfBHorario", "");
    $('#modalHorarioNuevoHrInicio').val('');
    $('#modalHorarioNuevoHrFin').val('');
    $('#modalHorarioNuevoHr').modal('show');
    setTimeout(function () {
        $('#modalHorarioNuevoHrInicio').focus();
    }, 500);
});

// DOCUMENT - BOTON QUE GUARDA UN HORARIO (RANGO DE HORAS)
$(document).on('click', '#modalHorarioNuevoHrGuardar', function () {
    if (veriFormMedicoDiaHorario()) {
        var hr = (MedicoConfigHorarioJSON[MedicoHorarioDiaSELEC] !== '--') ? MedicoConfigHorarioJSON[MedicoHorarioDiaSELEC] : "";
        if (hr !== "") {
            hr += ",";
        }
        hr += $('#modalHorarioNuevoHrInicio').val() + "-" + $('#modalHorarioNuevoHrFin').val();
        MedicoConfigHorarioJSON[MedicoHorarioDiaSELEC] = hr;
        llenarTablaMedicoHorariosConfig();
        $('#modalHorarioNuevoHr').modal('hide');
    }
});

// DOCUMENT - BOTON QUE GUARDA EL HORARIO
$(document).on('click', '#modalConfGuardar', function () {
    console.log(MedicoConfigHorarioJSON);
    $.ajax({
        type: "POST",
        contentType: "application/x-www-form-urlencoded",
        url: "/Home/EditHorarioMedico",
        data: { HorarioData: MedicoConfigHorarioJSON },
        beforeSend: function () {
            LoadingOn("Guardando Horario...");
        },
        success: function (data) {
            LoadingOff();
            if (data !== "true") {
                errLog("E20004", data);
            }
        },
        error: function (error) {
            errLog("E20004", error);
        }
    });
});
// -------------------- CONFIGURACIÓN DE USUARIO [ MEDICO ] --------------------

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
            $('#modalAddImgUsuario').on('shown.bs.modal', function (e) {
                CropperImgUSER = new Cropper(document.getElementById('modalAddImgUsuarioPic'), {
                    minCropBoxWidth: 200,
                    minCropBoxHeight: 200,
                    cropBoxResizable: false,
                    dragMode: ["move"],
                    data: {
                        width: 200,
                        height: 200,
                    },
                });
                setTimeout(function () {
                    CropperImgUSER.setDragMode('move');
                    LoadingOff();
                }, 2000);
            });
            $('#modalAddImgUsuario').on('hidden.bs.modal', function (e) {
                $('#modalAddImgUsuarioDivPic').html('');
            });
            LoadingOff();
            $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                url: "/Home/ConUsuarioInfo",
                dataType: 'JSON',
                beforeSend: function () {
                    LoadingOn("Cargando Parametros...");
                },
                success: function (data) {
                    UsuarioInfoDataJSON = data;
                    iniParamsUsuarioInfo(data, function () {
                        LoadingOff();
                        setTimeout(function () {
                            $('.form-control').removeAttr('readonly');
                        }, 1500);
                    });
                },
                error: function (error) {
                    errLog("E30002", error.responseText);
                }
            });
        },
        error: function (error) {
            errLog("E30001", error.responseText);
        }
    });
});

// DOCUMENT - BOTON QUE PERMITE AGREGAR - ELIMINAR UNA FOTO DE USUARIO
$(document).on('click', '#imagenUsuarioBoton', function () {
    if ($(this).attr("accion") === "nimg") {
        $('#imagenUsuarioBotonDoc').click();
    } else if ($(this).attr("accion") === "dimg") {
        MsgPregunta("Borrar Img. Usuario", "¿Desea continuar?", function (si) {
            if (si) {
                var ImgData = {
                    IdUsuario: UsuarioInfoDataJSON.IdUsuario,
                    ImgNombre: UsuarioInfoDataJSON.ImgNombre,
                    EstatusImg: false,
                };
                $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    url: "/Home/ElimImgUsuario",
                    data: { ImgData: ImgData },
                    beforeSend: function () {
                        LoadingOn("Guardando Cambios...");
                    },
                    success: function (data) {
                        if (data === "true") {
                            UsuarioInfoDataJSON.ImagenUsuario = false;
                            $('.menuUsuarioPic').attr("src", "../Media/usuariodefault.png?" + new Date().getTime());
                            iniParamsUsuarioInfo(UsuarioInfoDataJSON, function () {
                                MsgAlerta("Ok!", "Cambios <b>guardados correctamente</b>", 2000, "success");
                                LoadingOff();
                            });
                        } else {
                            errLog("E30004", data);
                        }
                    },
                    error: function (error) {
                        errLog("E30004", error);
                    }
                });
            }
        });
    }
});

// DOCUMENT - FILE QUE CONTROLA EL ARCHIVO SELECCIONADO PARA LA FOTO  DE USUARIO
$(document).on('change', '#imagenUsuarioBotonDoc', function () {
    ImgUsuarioFILE = $(this).prop('files')[0];
    if (ImgUsuarioFILE !== undefined) {
        var nombre = ImgUsuarioFILE.name;
        var extension = nombre.substring(nombre.lastIndexOf('.') + 1);
        if (extension === "jpg" || extension === "jpeg" || extension === "png") {
            LoadingOn("Espere...");
            $('#modalAddImgUsuarioDivPic').html('<div class="col-sm-12"><div><img id="modalAddImgUsuarioPic" style="display: block; max-width: 100%; max-height: 70vh;" /></div></div>');
            $('#modalAddImgUsuario').modal('show');
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#modalAddImgUsuarioPic').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        } else {
            ImgUsuarioFILE = undefined;
            MsgAlerta("Atención!", "Formato de archivo para <b>Comprobante</b> NO <b>válido</b>\n\n<b>Imagenes tipo: </b> [ JPG, JPEG, PNG ]", 4500, "default");
        }
    }
});

// DOCUMENT - BOTONES QUE CONTROLAN EL ZOOM DE LA IMAGEN DEL USUARIO
$(document).on('click', '#modalAddImgUsuarioZMas', function () {
    CropperImgUSER.zoom(0.1);
});
$(document).on('click', '#modalAddImgUsuarioZMenos', function () {
    CropperImgUSER.zoom(-0.1);
});

// DOCUMENT - BOTON QUE GUARDA LA IMAGEN DE USUARIO PERSONALIZADA
$(document).on('click', '#modalAddImgUsuarioGuardarSelec', function () {
    MsgPregunta("Guardar Imagen", "¿Desea continuar?", function (si) {
        if (si) {
            var ImgData = {
                IdUsuario: UsuarioInfoDataJSON.IdUsuario,
                ImgNombre: UsuarioInfoDataJSON.ImgNombre,
                Base64Codigo: CropperImgUSER.getCroppedCanvas().toDataURL().replace("data:image/png;base64,", ""),
                EstatusImg: true,
            };
            $.ajax({
                type: "POST",
                contentType: "application/x-www-form-urlencoded",
                url: "/Home/AltaImgUsuario",
                data: { ImgData: ImgData },
                dataType: 'JSON',
                beforeSend: function () {
                    LoadingOn("Guardando Imagen...");
                },
                success: function (data) {
                    if (data.AltaImgEstatus === "true") {
                        $('#modalAddImgUsuario').modal('hide');
                        if (data.GuardarImg === "true") {
                            UsuarioInfoDataJSON.ImagenUsuario = true;
                            $('.menuUsuarioPic').attr("src", "../Docs/" + UsuarioInfoDataJSON.Folder + "/" + UsuarioInfoDataJSON.ImgNombre + "?" + new Date().getTime());
                            iniParamsUsuarioInfo(UsuarioInfoDataJSON, function () {
                                MsgAlerta("Ok!", "Imagen <b>guardada correctamente</b>", 2000, "success");
                                LoadingOff();
                            });
                        } else {
                            errLog("E30003", data.GuardarImg);
                        }
                    } else {
                        errLog("E30003", data.AltaImgEstatus);
                    }
                },
                error: function (error) {
                    errLog("E30003", error.responseText);
                }
            });
        }
    });
});

// DOCUMENT - BOTON QUE CAMBIA LA CONTRASEÑA DEL USUARIO
$(document).on('click', '#usuarioInfoPassGuardar', function () {
    if (veriFormNuevaPass()) {
        MsgPregunta("Cambiar Contraseña", "¿Desea continuar?", function (si) {
            if (si) {
                $.ajax({
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    url: "/Home/EditUsuarioPass",
                    data: { NuevaPassData: UsuarioInfoVALSJSON },
                    beforeSend: function () {
                        LoadingOn("Guardando Cambios...");
                    },
                    success: function (data) {
                        if (data === "true") {
                            $('.usuarioinfopass').val('');
                            LoadingOff();
                            MsgAlerta("Ok!", "La <b>contraseña ha sido cambiada</b>", 2500, "success");
                        } else if (data === "errLogin") {
                            LoadingOff();
                            MsgAlerta("Error!", "La <b>antigua contraseña es incorrecta</b>", 2900, "error");
                        } else {
                            errLog("E30005", data);
                        }
                    },
                    error: function (error) {
                        errLog("E30005", error);
                    }
                });
            }
        });
    }
});

// DOCUMENT - BOTON QUE GUARDA LA  INFORMACION DEL USUARIO
$(document).on('click', '#usuarioInfoGuardar', function () {
    if (veriFormInfoUsuario()) {
        $.ajax({
            type: "POST",
            contentType: "application/x-www-form-urlencoded",
            url: "/Home/GuardarInfoUsuario",
            data: { UsuarioInfo: UsuarioInfoVALSJSON },
            beforeSend: function () {
                LoadingOn("Guardando Cambios...");
            },
            success: function (data) {
                if (data === "true") {
                    $('.menuUsuarioNombre').html(UsuarioInfoVALSJSON.Nombre + " " + UsuarioInfoVALSJSON.Apellido);
                    UsuarioInfoDataJSON.Nombre = UsuarioInfoVALSJSON.Nombre;
                    UsuarioInfoDataJSON.Apellido = UsuarioInfoVALSJSON.Apellido;
                    UsuarioInfoDataJSON.Correo = UsuarioInfoVALSJSON.Correo;
                    UsuarioInfoDataJSON.Direccion = UsuarioInfoVALSJSON.Direccion;
                    UsuarioInfoDataJSON.Telefono = UsuarioInfoVALSJSON.Telefono;
                    UsuarioInfoDataJSON.Celular = UsuarioInfoVALSJSON.Celular;
                    
                    LoadingOff();
                } else {
                    errLog("E30006", data);
                }
            },
            error: function (error) {
                errLog("E30006", error);
            }
        });
    }
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
                $('.menuUsuarioPic').attr("src", data.ImgUsuario + "?" + new Date().getTime());
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

// -------------------- CONFIGURACIÓN DE USUARIO [ MEDICO ] --------------------
// FUNCION RELLENAR TABLA DE HORARIOS
function llenarTablaMedicoHorariosConfig() {
    var dias = ["Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"];
    for (i = 0; i < dias.length; i++) {
        if (MedicoConfigHorarioJSON[dias[i]] !== "--") {
            var diaHr = MedicoConfigHorarioJSON[dias[i]].split(",");

            var arrAux = [], diaOrd = '';
            for (o = 0; o < diaHr.length; o++) {
                var hrOrd = diaHr[o].split("-");
                arrAux.push(hrOrd[0].replace(":", ""));
                arrAux.push(hrOrd[1].replace(":", ""));
            }
            arrAux.sort();
            for (o = 0; o < arrAux.length; o++) {
                if (diaOrd !== "") {
                    diaOrd += ",";
                }
                diaOrd += arrAux[o].splice(2, 0, ":") + "-" + arrAux[o + 1].splice(2, 0, ":");
                o++;
            }
            MedicoConfigHorarioJSON[dias[i]] = diaOrd;

            diaHr = MedicoConfigHorarioJSON[dias[i]].split(",");
            var hrs = '';
            for (j = 0; j < diaHr.length; j++) {
                var hr = diaHr[j].split("-");
                if (hrs !== "") {
                    hrs += "<br />";
                }
                hrs += '<b>' + reloj12hrs(hr[0]) + ' - ' + reloj12hrs(hr[1]) + '</b>';
            }
            $('#modalConfTHorario' + dias[i]).html('<div class="alert2 alert2-info" role="alert" align="center">' + hrs + '<br /><button class="badge badge-pill badge-danger" onclick="borrarHorarioDiaMedico(' + i + ');"><i class="fa fa-times"></i></button></div>');
        } else {
            $('#modalConfTHorario' + dias[i]).html('<h5><span class="badge badge-secondary">Sin Horario</span></h5>');
        }
    }
}

// FUNCION QUE BORRA UNA  CONFIGURACION DE HORARIO DE UN DIA
function borrarHorarioDiaMedico(id) {
    var dias = ["Lunes", "Martes", "Miercoles", "Jueves", "Viernes", "Sabado", "Domingo"];
    MedicoConfigHorarioJSON[dias[id]] = '--';
    llenarTablaMedicoHorariosConfig();
}
// --------- VALIDACIONES
// FUNCION QUE VALIDA EL FORMULARIO DE AGREGAR NUEVO HORARIO A UN DIA HORARIO MEDICO
function veriFormMedicoDiaHorario() {
    var correcto = true, msg = '', t = 2000;
    var hrInpIni = $('#modalHorarioNuevoHrInicio').val().split(":"), hrInpFin = $('#modalHorarioNuevoHrFin').val().split(":");
    var ini = new Date(new Date().getFullYear(), 1, 1, parseInt(hrInpIni[0]), parseInt(hrInpIni[1]), 0, 0),
        fin = new Date(new Date().getFullYear(), 1, 1, parseInt(hrInpFin[0]), parseInt(hrInpFin[1]), 0, 0);
    if ($('#modalHorarioNuevoHrInicio').val() === "") {
        correcto = false;
        msg = 'Coloque la <b>Hora Inicio</b>';
        $('#modalHorarioNuevoHrInicio').focus();
    } else if ($('#modalHorarioNuevoHrFin').val() === "") {
        correcto = false;
        msg = 'Coloque la <b>Hora Término</b>';
        $('#modalHorarioNuevoHrFin').focus();
    } else if (ini.toString() === fin.toString()) {
        correcto = false;
        msg = 'Las Horas <b>NO pueden ser iguales</b>';
        $('#modalHorarioNuevoHrInicio').focus();
    } else if (ini > fin) {
        correcto = false;
        msg = 'El rango de Horas <b>NO es congruente</b>';
        $('#modalHorarioNuevoHrInicio').focus();
    } else {
        var horario = MedicoConfigHorarioJSON[MedicoHorarioDiaSELEC].split(",");
        for (i = 0; i < horario.length; i++) {
            var hrsV = horario[i].split("-");
            var iniV = new Date(new Date().getFullYear(), 1, 1, parseInt(hrsV[0].split(":")[0]), parseInt(hrsV[0].split(":")[1]), 0, 0),
                finV = new Date(new Date().getFullYear(), 1, 1, parseInt(hrsV[1].split(":")[0]), parseInt(hrsV[1].split(":")[1]), 0, 0);
            if ((ini >= iniV && ini <= finV) || (fin >= iniV && fin <= finV)) {
                correcto = false;
                msg = 'El Horario que intenta agregar <b>se interpone otro horario configurado</b><br><br><b>' + MedicoHorarioDiaSELEC + "<br>" + reloj12hrs(hrsV[0]) + '-' + reloj12hrs(hrsV[1]) + '</b>';
                t = 5000;
            }
        }
    }
    if (!correcto) {
        MsgAlerta("Atención!", msg, t, "default");
    }
    return correcto;
}
// -------------------- CONFIGURACIÓN DE USUARIO [ MEDICO ] --------------------

// -------------------- INFORMACION DE USUARIO --------------------
// FUNCION QUE CARGA LOS PARAMETROS INICIALES DEL MENU DE USUARIO
function iniParamsUsuarioInfo(usuario, callback) {
    $('#imagenUsuarioTitulo').html((usuario.ImagenUsuario) ? "Imagen Actual" : "Sin Imagen de Usuario");
    $('#imagenUsuarioPic').attr("src", ((usuario.ImagenUsuario) ? "../Docs/" + usuario.Folder + "/" + usuario.ImgNombre : "../Media/usuariodefault.png") + "?" + new Date().getTime())
    $('#imagenUsuarioBoton').html(
        (usuario.ImagenUsuario) ? '<i class="fa fa-trash"></i> Eliminar Imagen Usuario' : '<i class="fa fa-image"></i> Agregar Imagen Usuario'
    ).removeClass(
        (usuario.ImagenUsuario) ? 'btn-success' : 'btn-danger'
    ).addClass(
        (usuario.ImagenUsuario) ? 'btn-danger' : 'btn-success'
    ).attr(
        "accion", (usuario.ImagenUsuario) ? 'dimg' : 'nimg'
    );

    $('#usuarioInfoNombre').val(usuario.Nombre);
    $('#usuarioInfoApellido').val(usuario.Apellido);
    $('#usuarioInfoCorreo').val(usuario.Correo);
    $('#usuarioInfoDireccion').val(usuario.Direccion);
    $('#usuarioInfoTelefono').val(usuario.Telefono);
    $('#usuarioInfoCelular').val(usuario.Celular);
    $('#usuarioInfoDivAdicional').html(
        '<div class="col-sm-3"><h6><span class="badge badge-primary">' + UsuarioTiposJSON["usu" + usuario.Tipo] + '</span></h6></div>'
    );

    callback(true);
}

// ------ VALIDAR
// FUNCION QUE VALIDA EL FORMULARIO DE INFRO GENERAL DEL USUARIO
function veriFormInfoUsuario() {
    var correcto = true, msg = '';
    if ($('#usuarioInfoNombre').val() === "") {
        correcto = false;
        msg = 'Coloque <b>Nombre Usuario</b>';
        $('#usuarioInfoNombre').focus();
    } else if ($('#usuarioInfoApellido').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Apellido</b>';
        $('#usuarioInfoApellido').focus();
    } else if ($('#usuarioInfoCorreo').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>correo</b>';
        $('#usuarioInfoCorreo').focus();
    } else if (!esEmail($('#usuarioInfoCorreo').val())) {
        correcto = false;
        msg = 'El formato de <b>correo es incorrecto</b>';
        $('#usuarioInfoCorreo').focus();
    } else if ($('#usuarioInfoDireccion').val() === "") {
        correcto = false;
        msg = 'Coloque la <b>Dirección</b>';
        $('#usuarioInfoDireccion').focus();
    } else if ($('#usuarioInfoTelefono').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Num. Teléfono</b>';
        $('#usuarioInfoTelefono').focus();
    } else if ($('#usuarioInfoCelular').val() === "") {
        correcto = false;
        msg = 'Coloque el <b>Num. Celular</b>';
        $('#usuarioInfoCelular').focus();
    } else {
        UsuarioInfoVALSJSON = {
            Nombre: $('#usuarioInfoNombre').val().trim(),
            Apellido: $('#usuarioInfoApellido').val().trim(),
            Correo: $('#usuarioInfoCorreo').val().trim(),
            Direccion: $('#usuarioInfoDireccion').val().trim(),
            Telefono: $('#usuarioInfoTelefono').val(),
            Celular: $('#usuarioInfoCelular').val(),
        };
    }
    if (!correcto) {
        MsgAlerta("Atención!", msg, 2000, "default");
    }
    return correcto;
}

// FUNCION QUE VALIDA EL FORMULARIO DE CAMBIO DE CONTRASEÑA
function veriFormNuevaPass() {
    var correcto = true, msg = '';
    if ($('#usuarioInfoAntPass').val() === "") {
        correcto = false;
        msg = 'Coloque la <b>antigua contraseña</b>';
        $('#usuarioInfoAntPass').focus();
    } else if ($('#usuarioInfoNuevaPass').val() === "") {
        correcto = false;
        msg = 'Coloque la <b>nueva contraseña</b>';
        $('#usuarioInfoNuevaPass').focus();
    } else if ($('#usuarioInfoNuevaPassO').val() === "") {
        correcto = false;
        msg = 'Repita la <b>nueva contraseña</b>';
        $('#usuarioInfoNuevaPassO').focus();
    }
    else if ($('#usuarioInfoNuevaPassO').val() !== $('#usuarioInfoNuevaPass').val()) {
        correcto = false;
        msg = 'Las <b>contraseñas NO coinciden</b>';
        $('#usuarioInfoNuevaPass').focus();
    } else {
        UsuarioInfoVALSJSON = {
            NuevaPass: $('#usuarioInfoNuevaPass').val(),
            AntiguaPass: $('#usuarioInfoAntPass').val(),
        };
    }
    if (!correcto) {
        MsgAlerta("Atención!", msg, 2000, "default");
    }
    return correcto;
}
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
    '<img class="menuUsuarioPic" alt="" /><i class="menuUsuarioNombre">--</i>' +
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