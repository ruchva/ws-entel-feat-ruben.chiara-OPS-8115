'################################################################################################################
' Módulo           : Proxy Topaz 
' Clase            : Estado de Proceso
' Archivo          : clsEstadoTZ.vb
' Propósito        : Manejador de Resultado del Proceso 
' Autor            : María Guipzi Heredia S.
' Fecha de Creación: 10/11/2011
' Versión          : 1.0.0
'################################################################################################################

Option Explicit On
Option Compare Text

Imports BG.Topaz.Enlace.clsEnumProxy

<Serializable()> Public Class clsEstadoTZ

    '### Variables del Estado
    Public CodMsjeProceso As enumEstadoEjecucion       '### Indica si el proceso terminó con éxito o con error 

    '### Variables de Respuesta de Topaz
    Public NombreServicio As String                    '### Nombre del Servicio que fue invocado
    Public CodMensaje As String                        '### Código del Mensaje
    Public Mensaje As String                           '### Mensaje
    Public Estado As Boolean                           '### Estado 

    '### Otras Variables
    Public Resultado As Object                         '### Resultado: Datos genérico, datarow, datatable, dataset

End Class

