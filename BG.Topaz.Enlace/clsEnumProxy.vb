'################################################################################################################
' Módulo           : Proxy Topaz 
' Clase            : Enumerados
' Archivo          : clsEnum.vb
' Propósito        : Definición de enumerados del módulo
' Autor            : María Guipzi Heredia S.
' Fecha de Creación: 10/11/2011
' Versión          : 1.0.0
'################################################################################################################

Option Explicit On
Option Compare Text

<Serializable()> Public Class clsEnumProxy

    'Public Enum enumTipoDato As Short
    '    enString = 1
    '    enShort = 2
    '    enInteger = 3
    '    enLong = 4
    'End Enum

    Public Enum enumEstadoEjecucion As Short
        enOk = 0
        enError = 1
    End Enum

    Public Enum enumTipoADevolver
        enObtenerValor = 1
        enObtenerDataRow = 2
        enObtenerDataTable = 3
        enObtenerDataSet = 4
    End Enum

End Class

