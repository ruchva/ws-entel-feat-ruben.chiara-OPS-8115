'################################################################################################################
' Módulo           : Proxy Topaz 
' Clase            : Parámetros
' Archivo          : clsParametros.vb
' Propósito        : Definición de Parámetros de Entrada para la Invocación de Métodos.
' Autor            : María Guipzi Heredia S.
' Fecha de Creación: 10/11/2011
' Versión          : 1.0.0
'################################################################################################################

Option Explicit On
Option Compare Text

<Serializable()> Public Class clsParametrosProxy

#Region " Variables Privadas "

    Private NombreParm As String
    Private ValorParm As String

    'Private TipoDato As enumTipoDato

    'Private ValorShort As Short
    'Private ValorString As String
    'Private ValorInteger As Integer
    'Private ValorLong As Long

#End Region

#Region " Propiedades "

    Public ReadOnly Property Nombre As String
        Get
            Return NombreParm
        End Get
    End Property

    Public ReadOnly Property Valor As String
        Get
            Return ValorParm
        End Get
    End Property

    'Public Property Valor As Object
    '    'Get
    '    '    Select Case TipoDato
    '    '        Case enumTipoDato.enString : Return ValorString
    '    '        Case enumTipoDato.enShort : Return ValorShort
    '    '        Case enumTipoDato.enInteger : Return ValorInteger
    '    '        Case enumTipoDato.enLong : Return ValorLong
    '    '    End Select

    '    '    Return Nothing
    '    'End Get

    '    'Set(ByVal value As Object)
    '    '    Select Case TipoDato
    '    '        Case enumTipoDato.enString : ValorString = value
    '    '        Case enumTipoDato.enShort : ValorShort = value
    '    '        Case enumTipoDato.enInteger : ValorInteger = value
    '    '        Case enumTipoDato.enLong : ValorLong = value
    '    '    End Select
    '    'End Set
    'End Property

#End Region

#Region " Constructor "

    'Sub New(ByVal pStrNombre As String, ByVal pObjValor As Object, ByVal pEnmTipoDato As enumTipoDato)
    '    NombreParm = pStrNombre
    '    TipoDato = pEnmTipoDato
    '    Valor = pObjValor
    'End Sub

    Sub New(ByVal pStrNombre As String, ByVal pStrValor As String)
        NombreParm = pStrNombre
        'TipoDato = pEnmTipoDato
        ValorParm = pStrValor
    End Sub

#End Region

End Class
