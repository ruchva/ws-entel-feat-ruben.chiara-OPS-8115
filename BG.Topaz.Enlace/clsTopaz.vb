'################################################################################################################
' Módulo           : Proxy Topaz 
' Clase            : Clase XML
' Archivo          : clsXML.vb
' Propósito        : Ejecutar los Servicios Web de Topaz y Retornar Determinadas Estructuras.
' Autor            : María Guipzi Heredia S.
' Fecha de Creación: 10/11/2011
' Versión          : 1.0.0
'################################################################################################################

Option Explicit On
Option Compare Text

Imports System.Xml
Imports System.Text
Imports BG.Topaz.Enlace.clsEnumProxy
Imports System.Configuration
Imports System.IO

<Serializable()> Public Class clsTopaz

#Region " Métodos Privados "
    Public Property CodUsuario As String
    ''' <summary>
    ''' Genera el elemento ExecutionInfo para la invocación del Servicio Web de Topaz 
    ''' </summary>
    ''' <param name="pStrUsuario">Usuario</param>
    ''' <param name="pStrPassword">Password</param>
    ''' <param name="pIntNroSession">Nro. de Session</param>
    ''' <param name="pLngSequence">Nro de secuencia único</param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerarExecutionInfo(ByVal pStrUsuario As String, ByVal pStrPassword As String, _
                                         Optional ByVal pIntNroSession As Integer = 0, _
                                         Optional ByVal pLngSequence As Long = 0) As String

        Dim lStbExecution As New StringBuilder

        lStbExecution.Append("<?xml version='1.0' encoding='UTF-8'?>")
        lStbExecution.Append("<xmlJBankExecutionParameters>")

        lStbExecution.Append("<authentication><type/>")
        lStbExecution.Append("<userName>" & pStrUsuario & "</userName>")
        lStbExecution.Append("<password>" & pStrPassword & "</password>")
        lStbExecution.Append("<sessionID>" & pIntNroSession & "</sessionID>")
        lStbExecution.Append("</authentication>")

        If pLngSequence > 0 Then
            lStbExecution.Append("<duplicatedControl><type/>")
            lStbExecution.Append("<sequenceID>" & pLngSequence & "</sequenceID>")
            lStbExecution.Append("</duplicatedControl>")
        End If

        lStbExecution.Append("</xmlJBankExecutionParameters>")

        Return lStbExecution.ToString
    End Function

    ''' <summary>
    ''' Genera el elemento Request para la invocación del Servicio Web de Topaz 
    ''' </summary>
    ''' <param name="pStrNombreServicio">Nombre del Servicio a Invocar</param>
    ''' <param name="pLstParametros">Lista de Parámetros</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GenerarRequest(ByVal pStrNombreServicio As String,
                                    Optional ByVal pLstParametros As List(Of clsParametrosProxy) = Nothing) As String

        Dim lStbRequest As New StringBuilder

        lStbRequest.Append("<?xml version='1.0' encoding='UTF-8'?>")
        lStbRequest.Append("<xmlJBankRequest><xmlJBankService>")
        lStbRequest.Append("<serviceName>" & pStrNombreServicio & "</serviceName>")

        lStbRequest.Append("<xmlJBankFields>")

        '### Adicionar los Parámetros si Existen
        If Not pLstParametros Is Nothing Then
            For Each clsParametros In pLstParametros
                lStbRequest.Append("<xmlJBankField>")
                lStbRequest.Append("<fieldName>" & clsParametros.Nombre & "</fieldName>")
                lStbRequest.Append("<fieldValue>" & clsParametros.Valor & "</fieldValue>")
                lStbRequest.Append("</xmlJBankField>")
            Next
        End If

        lStbRequest.Append("</xmlJBankFields>")

        lStbRequest.Append("</xmlJBankService></xmlJBankRequest>")

        Return lStbRequest.ToString
    End Function

    ''' <summary>
    ''' Carga los elementos JBankMetadata y JBankData a partir del XML recibido
    ''' </summary>
    ''' <param name="pStrXML"></param>
    ''' <param name="pStrJBankMetadata"></param>
    ''' <param name="pStrJBankData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CargarDatosXML(ByVal pStrXML As String, ByRef pStrJBankMetadata As String, _
                                    ByRef pStrJBankData As String) As clsEstadoTZ

        Dim lClsEstadoTZ As New clsEstadoTZ

        '### Inicializa con estado "enError"
        lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError

        If Trim(pStrXML) = "" Then
            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
            lClsEstadoTZ.Mensaje = "XML mal formado."

            Return lClsEstadoTZ
        End If

        Dim lXndNodoPrincipal As XmlNode
        Dim lXndNodoRespuesta As XmlNode
        Dim lXmlDocumento As New XmlDocument

        '### Cargar el Documento
        lXmlDocumento.LoadXml(pStrXML)

        lXndNodoPrincipal = lXmlDocumento.SelectSingleNode("xmlJBankResponse/xmlJBankService")

        If lXndNodoPrincipal Is Nothing Then
            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
            lClsEstadoTZ.Mensaje = "Nodo de respuesta no encontrado."

            Return lClsEstadoTZ
        End If

        Dim lStrValor As Object = Nothing

        For Each lXndNodoRespuesta In lXndNodoPrincipal.ChildNodes

            Select Case UCase(lXndNodoRespuesta.Name)
                Case "SERVICENAME", "ERRORCODE", "ERROR", "STATUS"
                    For Each lXelElemento In lXndNodoRespuesta
                        lStrValor = lXelElemento.Value
                    Next

                Case "XMLJBANKMETADATA" : pStrJBankMetadata = lXndNodoRespuesta.InnerXml

                Case "XMLJBANKDATA" : pStrJBankData = lXndNodoRespuesta.InnerXml

            End Select

            Select Case UCase(lXndNodoRespuesta.Name)
                Case "SERVICENAME" : lClsEstadoTZ.NombreServicio = lStrValor
                Case "ERRORCODE" : lClsEstadoTZ.CodMensaje = lStrValor
                Case "ERROR" : lClsEstadoTZ.Mensaje = lStrValor
                Case "STATUS" : lClsEstadoTZ.Estado = lStrValor
            End Select
        Next

        If CBool(lClsEstadoTZ.Estado) = True Then
            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enOk
        End If

        Return lClsEstadoTZ
    End Function

    ''' <summary>
    ''' Obtiene el valor resultante que se encuentra en JBankData
    ''' </summary>
    ''' <param name="pStrJBankDataXML">Contenido de JBankData en formato XML</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ObtenerValor(ByVal pStrJBankDataXML As String) As clsEstadoTZ
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lXmlDocumento As New XmlDocument
        Dim lXndNodoDato, lXndNodoDatos As XmlNode

        lXmlDocumento.LoadXml(pStrJBankDataXML)

        lXndNodoDatos = lXmlDocumento.SelectSingleNode("xmlJBankResultSet/xmlJBankRows/xmlJBankRow/xmlJBankFieldValues")

        If lXndNodoDatos Is Nothing Then
            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
            lClsEstadoTZ.Mensaje = "Valor no encontrado."

            Return lClsEstadoTZ
        End If

        For Each lXndNodoDato In lXndNodoDatos.ChildNodes
            For Each lXelElemento In lXndNodoDato
                lClsEstadoTZ.Resultado = lXelElemento.Value

                Return lClsEstadoTZ
            Next
        Next

        lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
        lClsEstadoTZ.Mensaje = "Valor no encontrado."

        '### Liberar Recursos
        lXmlDocumento = Nothing
        lXndNodoDato = Nothing
        lXndNodoDatos = Nothing

        Return lClsEstadoTZ
    End Function

    ''' <summary>
    ''' Obtiene una registro (Datarow) con los datos resultantes 
    ''' </summary>
    ''' <param name="pStrJBankMetadataXML">Metadata (Estructura)</param>
    ''' <param name="pStrJBankDataXML">Datos</param>
    ''' <param name="pShrNroTabla">Número de Tabla de la que se va a Obtener el Registro</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ObtenerDataRow(ByVal pStrJBankMetadataXML As String, _
                                    ByVal pStrJBankDataXML As String, _
                                    Optional ByVal pShrNroTabla As Short = 1) As clsEstadoTZ
        Dim lXndNodo As XmlNode
        Dim lDrwFila As DataRow
        Dim lXndCampo As XmlNode
        Dim lShrNroCampo As Short
        Dim lStrNombreTabla As String
        Dim lDtbTabla As New DataTable
        Dim lXnlListaNodos As XmlNodeList
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lXmlJBankData As New XmlDocument
        Dim lXmlJBankMetadata As New XmlDocument

        lXmlJBankData.LoadXml(pStrJBankDataXML)
        lXmlJBankMetadata.LoadXml(pStrJBankMetadataXML)

        '### Crear Estructura de Tablas
        For Each lXndNodo In lXmlJBankMetadata.ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            If IsNumeric(lXndNodo.FirstChild.InnerText) AndAlso pShrNroTabla = CShort(lXndNodo.FirstChild.InnerText) Then
                '### Crear la Tabla
                lDtbTabla.TableName = lStrNombreTabla

                '### Crear los Campos
                lXnlListaNodos = lXndNodo.ChildNodes(1).ChildNodes

                For Each lXndCampo In lXnlListaNodos
                    lDtbTabla.Columns.Add(lXndCampo.InnerText, Type.GetType("System.String"))
                Next

                Exit For
            End If
        Next

        '### Cargar los Resultados
        For Each lXndNodo In lXmlJBankData.ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            If IsNumeric(lXndNodo.FirstChild.InnerText) AndAlso pShrNroTabla = CShort(lXndNodo.FirstChild.InnerText) Then
                lDtbTabla.TableName = lStrNombreTabla
                lDrwFila = lDtbTabla.NewRow()

                lXnlListaNodos = lXndNodo.SelectNodes("xmlJBankRows/xmlJBankRow/xmlJBankFieldValues/fieldValue")

                For Each lXndCampo In lXnlListaNodos
                    lDrwFila.Item(lShrNroCampo) = lXndCampo.InnerText
                    lShrNroCampo += 1
                Next

                If lXnlListaNodos.Count > 0 Then
                    lDtbTabla.Rows.Add(lDrwFila)
                End If

                Exit For
            End If
        Next

        If lDtbTabla.Rows.Count > 0 Then
            lClsEstadoTZ.Resultado = lDtbTabla.Rows(0)
        Else
            lClsEstadoTZ.Resultado = Nothing
        End If

        Return lClsEstadoTZ
    End Function

    ''' <summary>
    ''' Obtiene una tabla con los datos resultantes 
    ''' </summary>
    ''' <param name="pStrJBankMetadataXML">Metadata (Estructura)</param>
    ''' <param name="pStrJBankDataXML">Datos</param>
    ''' <param name="pShrNroTabla">Número de Tabla a Obtener</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ObtenerDataTable(ByVal pStrJBankMetadataXML As String, _
                                      ByVal pStrJBankDataXML As String, _
                                      Optional ByVal pShrNroTabla As Short = 1) As clsEstadoTZ
        Dim lXndNodo As XmlNode
        Dim lDrwFila As DataRow
        Dim lXndCampo As XmlNode
        Dim lShrNroCampo As Short
        Dim lStrNombreTabla As String
        Dim lDtbTabla As New DataTable
        Dim lXnlListaNodos As XmlNodeList
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lXmlJBankData As New XmlDocument
        Dim lXmlJBankMetadata As New XmlDocument

        lXmlJBankData.LoadXml(pStrJBankDataXML)
        lXmlJBankMetadata.LoadXml(pStrJBankMetadataXML)

        '### Crear Estructura de Tablas
        For Each lXndNodo In lXmlJBankMetadata.ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            If IsNumeric(lXndNodo.FirstChild.InnerText) AndAlso pShrNroTabla = CShort(lXndNodo.FirstChild.InnerText) Then
                '### Crear la Tabla
                lDtbTabla.TableName = lStrNombreTabla

                '### Crear los Campos
                lXnlListaNodos = lXndNodo.ChildNodes(1).ChildNodes

                For Each lXndCampo In lXnlListaNodos
                    lDtbTabla.Columns.Add(lXndCampo.InnerText, Type.GetType("System.String"))
                Next

                Exit For
            End If
        Next

        '### Cargar los Resultados
        For Each lXndNodo In lXmlJBankData.ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            If IsNumeric(lXndNodo.FirstChild.InnerText) AndAlso pShrNroTabla = CShort(lXndNodo.FirstChild.InnerText) Then
                lDtbTabla.TableName = lStrNombreTabla
                lDrwFila = lDtbTabla.NewRow()

                lXnlListaNodos = lXndNodo.SelectNodes("xmlJBankRows/xmlJBankRow/xmlJBankFieldValues/fieldValue")

                For Each lXndCampo In lXnlListaNodos
                    lDrwFila.Item(lShrNroCampo) = lXndCampo.InnerText
                    lShrNroCampo += 1
                Next

                If lXnlListaNodos.Count > 0 Then
                    lDtbTabla.Rows.Add(lDrwFila)
                End If

                Exit For
            End If
        Next

        lClsEstadoTZ.Resultado = lDtbTabla

        Return lClsEstadoTZ
    End Function

    ''' <summary>
    ''' Obtiene un Dataset con los datos resultantes
    ''' </summary>
    ''' <param name="pStrJBankMetadataXML">Metadata (Estructura)</param>
    ''' <param name="pStrJBankDataXML">Datos</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ObtenerDataSet(ByVal pStrJBankMetadataXML As String, ByVal pStrJBankDataXML As String) As clsEstadoTZ
        Dim lXndNodo As XmlNode
        Dim lDrwFila As DataRow
        Dim lXndCampo As XmlNode
        Dim lShrNroCampo As Short
        Dim lStrNombreTabla As String
        Dim lDstTablas As New DataSet
        Dim lXnlListaNodos As XmlNodeList
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lXmlJBankData As New XmlDocument
        Dim lXmlJBankMetadata As New XmlDocument

        lXmlJBankData.LoadXml(pStrJBankDataXML)
        lXmlJBankMetadata.LoadXml(pStrJBankMetadataXML)

        '### Crear Estructura de Tablas
        For Each lXndNodo In lXmlJBankMetadata.ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            '### Crear la Tabla
            lDstTablas.Tables.Add(lStrNombreTabla)

            '### Crear los Campos
            lXnlListaNodos = lXndNodo.ChildNodes(1).ChildNodes

            For Each lXndCampo In lXnlListaNodos
                lDstTablas.Tables(lStrNombreTabla).Columns.Add(lXndCampo.InnerText, Type.GetType("System.String"))
            Next
        Next

        '### Cargar los Resultados
        For Each lXndNodo In lXmlJBankData.ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            lDrwFila = lDstTablas.Tables(lStrNombreTabla).NewRow()

            lXnlListaNodos = lXndNodo.SelectNodes("xmlJBankRows/xmlJBankRow/xmlJBankFieldValues/fieldValue")

            For Each lXndCampo In lXnlListaNodos
                lDrwFila.Item(lShrNroCampo) = lXndCampo.InnerText
                lShrNroCampo += 1

                '### Adicionado en Pruebas
                If lShrNroCampo = lDrwFila.ItemArray.Length Then
                    lShrNroCampo = 0
                    lDstTablas.Tables(lStrNombreTabla).Rows.Add(lDrwFila)
                    lDrwFila = lDstTablas.Tables(lStrNombreTabla).NewRow()
                End If
            Next
        Next

        lClsEstadoTZ.Resultado = lDstTablas

        Return lClsEstadoTZ
    End Function

    ''' <summary>
    ''' Obtiene un Dataset con los datos resultantes
    ''' </summary>
    ''' <param name="pStrJBankMetadataXML">Metadata (Estructura)</param>
    ''' <param name="pStrJBankDataXML">Datos</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ObtenerDataSetAnidado(ByVal pStrJBankMetadataXML As String, ByVal pStrJBankDataXML As String) As clsEstadoTZ
        Dim lXndNodo As XmlNode
        Dim lDrwFila As DataRow
        Dim lXndCampo As XmlNode
        Dim lShrNroCampo As Short
        Dim lDstTablas As New DataSet
        Dim lXnlListaNodos As XmlNodeList
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lStrNombreTabla As String = Nothing
        Dim lXmlJBankMetadata As New XmlDocument
        Dim lXmlJBankDataXML As New XmlDocument

        '### Colocar los Tags Iniciales Debido a que es Anidado
        pStrJBankMetadataXML = "<xmlJBankMetadata>" & pStrJBankMetadataXML & "</xmlJBankMetadata>"
        pStrJBankDataXML = "<xmlJBankData>" & pStrJBankDataXML & "</xmlJBankData>"

        lXmlJBankMetadata.LoadXml(pStrJBankMetadataXML)

        '### Crear Estructura de Tablas
        For Each lXndNodo In lXmlJBankMetadata.ChildNodes(0).ChildNodes
            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

            '### Crear la Tabla
            lDstTablas.Tables.Add(lStrNombreTabla)

            '### Crear los Campos
            lXnlListaNodos = lXndNodo.ChildNodes(1).ChildNodes

            For Each lXndCampo In lXnlListaNodos
                lDstTablas.Tables(lStrNombreTabla).Columns.Add(lXndCampo.InnerText, Type.GetType("System.String"))
            Next
        Next

        '### Cargar los Resultados
        Dim lStrJBankFieldValues As New StringBuilder
        Dim lXmlNodoJBankFieldValues As XmlNode = Nothing

        While ObtenerJBankResultSet(pStrJBankDataXML, lStrNombreTabla, lStrJBankFieldValues) ', pStrJBankDataXML)
            lStrNombreTabla = "Table" & lStrNombreTabla
            lXmlJBankDataXML.LoadXml(lStrJBankFieldValues.ToString)

            '### Obtiene la lista de XMLJBANKFIELDVALUES
            For Each lXndCampo In lXmlJBankDataXML.ChildNodes(0).ChildNodes
                '### Crear los Campos
                lDrwFila = lDstTablas.Tables(lStrNombreTabla).NewRow()

                For Each lXndNodo In lXndCampo.ChildNodes
                    lDrwFila.Item(lShrNroCampo) = lXndNodo.InnerText
                    lShrNroCampo += 1

                    '### Adicionado en Pruebas
                    If lShrNroCampo = lDrwFila.ItemArray.Length Then
                        lShrNroCampo = 0
                        lDstTablas.Tables(lStrNombreTabla).Rows.Add(lDrwFila)
                        lDrwFila = lDstTablas.Tables(lStrNombreTabla).NewRow()
                    End If

                Next
            Next

            lStrJBankFieldValues.Clear()
        End While

        lClsEstadoTZ.Resultado = lDstTablas

        Return lClsEstadoTZ
    End Function

    ''' <summary>
    ''' Obtiene los Datos Para el Dataset Anidado.
    ''' </summary>
    ''' <param name="pStrJBankDataXML"></param>
    ''' <param name="pStrID"></param>
    ''' <param name="lStrJBankFieldValues"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ObtenerJBankResultSet(ByRef pStrJBankDataXML As String, ByRef pStrID As String, _
                                           ByRef lStrJBankFieldValues As StringBuilder) As Boolean

        If Trim(pStrJBankDataXML) = "" Then Return False
        lStrJBankFieldValues.Clear()
        pStrJBankDataXML = UCase(pStrJBankDataXML)

        Dim lXmlNodo As XmlNode
        Dim lXmlListaNodos As XmlNodeList = Nothing
        Dim lXmlJBankData As New XmlDocument

        lXmlJBankData.LoadXml(pStrJBankDataXML)

        If UCase(lXmlJBankData.ChildNodes(0).Name) = "XMLJBANKDATA" OrElse _
           UCase(lXmlJBankData.ChildNodes(0).Name) = "XMLJBANKRESULTSET" Then
            lXmlListaNodos = lXmlJBankData.ChildNodes(0).ChildNodes
        End If

        If lXmlListaNodos Is Nothing Then
            Return False
        End If

        For Each lXmlNodo In lXmlListaNodos
            If UCase(lXmlNodo.Name) = "XMLJBANKRESULTSET" Then

                lXmlListaNodos = lXmlNodo.ChildNodes
                Exit For
            End If
        Next

        Dim lIntCont As Integer
        Dim lBlnExisteRegistro As Boolean = False
        Dim lBlnExisteJBankResultSet As Boolean = False

        While Not lBlnExisteJBankResultSet AndAlso (lIntCont < lXmlListaNodos.Count)
            lXmlNodo = lXmlListaNodos.Item(lIntCont)

            Select Case UCase(lXmlNodo.Name)
                Case "ID"
                    pStrID = lXmlNodo.InnerXml

                Case "XMLJBANKROWS"
                    '### Verificar si Contiene XMLJBANKRESULTSET
                    lBlnExisteJBankResultSet = lXmlNodo.InnerXml.Contains("XMLJBANKRESULTSET")

                    If lBlnExisteJBankResultSet Then
                        Dim lXmlNodoRow As XmlNode
                        Dim lXmlListaNodosXMLBankRow As XmlNodeList

                        '### Obtener la lista que está en el Tag <XMLJBANKROW>
                        lXmlListaNodosXMLBankRow = lXmlNodo.ChildNodes(0).ChildNodes

                        If lXmlListaNodosXMLBankRow.Count > 0 Then

                            For Each lXmlNodoRow In lXmlListaNodosXMLBankRow

                                If lXmlNodoRow.Name = "XMLJBANKFIELDVALUES" Then
                                    If lStrJBankFieldValues.Length = 0 Then
                                        lBlnExisteRegistro = True
                                        lStrJBankFieldValues.Append("<XMLDATOS>")
                                    End If
                                    'lStrJBankFieldValues.Append(lXmlNodoRow.InnerXml)
                                    lStrJBankFieldValues.Append("<XMLJBANKFIELDVALUES>" & lXmlNodoRow.InnerXml & "</XMLJBANKFIELDVALUES>")

                                ElseIf lXmlNodoRow.Name = "XMLJBANKRESULTSET" Then
                                    lBlnExisteJBankResultSet = True
                                    'pStrJBankResultSetXML()
                                    pStrJBankDataXML = "<xmlJBankResultSet>" & lXmlNodoRow.InnerXml & "</xmlJBankResultSet>"
                                End If
                            Next

                            If lBlnExisteRegistro Then
                                lStrJBankFieldValues.Append("</XMLDATOS>")
                            End If
                        End If

                    Else
                        Dim lXmlNodoRow As XmlNode
                        Dim lXmlListaNodosXMLBankRow As XmlNodeList

                        lXmlListaNodosXMLBankRow = lXmlNodo.ChildNodes

                        If lXmlListaNodosXMLBankRow.Count > 0 Then
                            lBlnExisteRegistro = True

                            lStrJBankFieldValues.Append("<XMLDATOS>")

                            For Each lXmlNodoRow In lXmlListaNodosXMLBankRow
                                lStrJBankFieldValues.Append(lXmlNodoRow.InnerXml)
                            Next

                            lStrJBankFieldValues.Append("</XMLDATOS>")
                        End If
                    End If

            End Select

            lIntCont += 1
        End While

        If lBlnExisteJBankResultSet = False Then pStrJBankDataXML = String.Empty ' pStrJBankResultSetXML = String.Empty

        '### Liberar Objetos
        lXmlJBankData = Nothing
        lXmlNodo = Nothing
        lXmlListaNodos = Nothing

        Return lBlnExisteRegistro
    End Function

#End Region


#Region " Métodos Privados de Decodificación "

    Public Function DecodificarPinTrans(ByVal pStrPina As String) As String
        Dim lStrPin As String = ""
        Dim lStrCad As String = ""
        Dim lShrCont As Short

        pStrPina = Trim(pStrPina)
        If pStrPina = String.Empty Then Return String.Empty

        pStrPina &= IIf(pStrPina.Chars(pStrPina.Length - 1) = "|", "", "|")

        For lShrCont = 1 To pStrPina.Length
            If pStrPina.Chars(lShrCont - 1) <> "|" Then
                lStrCad &= pStrPina.Chars(lShrCont - 1)
            Else
                lStrPin &= Chr(CInt(DecodificarPin(CInt(lStrCad))))
                lStrCad = ""
            End If
        Next

        Return lStrPin
    End Function

    Public Function DecodificarPin(ByVal pIntPina As Integer) As String
        Dim lStrAuxPin As String
        Dim lStrBase As String = Nothing
        Dim lIntContI As Integer
        Dim lIntContJ As Integer
        Dim lStrPin As String = Nothing

        lStrAuxPin = Format(pIntPina, "0000")

        For lIntContI = 1 To 4
            Select Case lIntContI
                Case "1"
                    lStrBase = "5761892304"
                Case "2"
                    lStrBase = "9376524180"
                Case "3"
                    lStrBase = "0493185267"
                Case "4"
                    lStrBase = "7319248056"
            End Select

            For lIntContJ = 1 To 10
                If Mid(lStrAuxPin, lIntContI, 1) = Mid(lStrBase, lIntContJ, 1) Then
                    Exit For
                End If
            Next lIntContJ

            If lIntContJ > 9 Then
                lIntContJ = 0
            End If

            lStrPin = lStrPin & Format(lIntContJ, "0")
        Next lIntContI

        Return lStrPin
    End Function

#End Region


#Region " Métodos Públicos "

    Public Function EjecutarServicio(ByVal pStrNombreServicio As String, _
                                     ByVal pEnmTipoADevolver As enumTipoADevolver, _
                                     Optional ByVal pLstParametros As List(Of clsParametrosProxy) = Nothing, _
                                     Optional ByVal pIntNroSession As Integer = 0, _
                                     Optional ByVal pShrNroTabla As Short = 1,
                                     Optional ByVal pBlnServicioAnidado As Boolean = False,
                                     Optional ByVal pLngSequence As Long = 0) As clsEstadoTZ

        Dim lStrUsuario As String
        Dim lStrPassword As String
        Dim lStrResultadoXML As String
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lExeService As New WSTopaz.executeService
        Dim lWSTopaz As WSTopaz.JBankBeanWSService
        Try
            lStrUsuario = DecodificarPinTrans(ConfigurationManager.AppSettings("UsuarioTz"))
            lStrPassword = DecodificarPinTrans(ConfigurationManager.AppSettings("PasswordTz"))

            lWSTopaz = New WSTopaz.JBankBeanWSService(ConfigurationManager.AppSettings("EnlaceTz").ToString())

            lExeService.executionInfo = GenerarExecutionInfo(lStrUsuario, lStrPassword, pIntNroSession, pLngSequence)

            Dim LogMensaje As New System.Text.StringBuilder
            LogMensaje.AppendLine(pStrNombreServicio)
            Dim lStrHora As String = Format(CDate(Now), "HH:mm:ss.fff")
            LogMensaje.AppendLine("Iniciar Solicitud WsTopaz: " & lStrHora)
            lExeService.request = GenerarRequest(pStrNombreServicio, pLstParametros)
            lStrResultadoXML = lWSTopaz.executeService(lExeService).executeServiceResult
            lStrHora = Format(CDate(Now), "HH:mm:ss.fff")
            LogMensaje.AppendLine("Finalizar Solicitud WsTopaz: " & lStrHora)
            LogMensaje.AppendLine(lStrResultadoXML)
            CrearArchivoLog(LogMensaje.ToString, Me.GetType.FullName, CodUsuario)
            Dim lStrJBankData As String = String.Empty
            Dim lStrJBankMetadata As String = String.Empty
            lClsEstadoTZ = CargarDatosXML(lStrResultadoXML, lStrJBankMetadata, lStrJBankData)
            If lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError Then
                Return lClsEstadoTZ
            End If

            Select Case pEnmTipoADevolver
                Case enumTipoADevolver.enObtenerValor
                    lClsEstadoTZ = ObtenerValor(lStrJBankData)

                Case enumTipoADevolver.enObtenerDataRow
                    lClsEstadoTZ = ObtenerDataRow(lStrJBankMetadata, lStrJBankData, pShrNroTabla)

                Case enumTipoADevolver.enObtenerDataTable
                    lClsEstadoTZ = ObtenerDataTable(lStrJBankMetadata, lStrJBankData, pShrNroTabla)

                Case enumTipoADevolver.enObtenerDataSet
                    If Not pBlnServicioAnidado Then
                        lClsEstadoTZ = ObtenerDataSet(lStrJBankMetadata, lStrJBankData)
                    Else
                        lClsEstadoTZ = ObtenerDataSetAnidado(lStrJBankMetadata, lStrJBankData)
                    End If
            End Select

        Catch ex As Exception
            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
            lClsEstadoTZ.Mensaje = ex.Message
        End Try

        Return lClsEstadoTZ

    End Function

    Public Function RevertirServicio(ByVal pLngSequence As Long) As String
        Dim lStrUsuario As String
        Dim lStrPassword As String
        Dim lStrResultadoXML As String
        Dim lExeUndo As New WSTopaz.executeUndo
        Dim lWSTopaz As WSTopaz.JBankBeanWSService

        lStrUsuario = DecodificarPinTrans(ConfigurationManager.AppSettings("UsuarioTz"))
        lStrPassword = DecodificarPinTrans(ConfigurationManager.AppSettings("PasswordTz"))

        lWSTopaz = New WSTopaz.JBankBeanWSService(ConfigurationManager.AppSettings("EnlaceTz").ToString())

        lExeUndo.executionInfo = GenerarExecutionInfo(lStrUsuario, lStrPassword, 0, pLngSequence)
        lStrResultadoXML = lWSTopaz.executeUndo(lExeUndo).undoServiceResult

        Return lStrResultadoXML
    End Function

    Public Function EjecutarServicio_PRUEBA_MARIA(ByVal pStrResultadoServicioXML As String, _
                                     ByVal pEnmTipoADevolver As enumTipoADevolver, _
                                     Optional ByVal pLstParametros As List(Of clsParametrosProxy) = Nothing, _
                                     Optional ByVal pIntNroSession As Integer = 0, _
                                     Optional ByVal pShrNroTabla As Short = 1,
                                     Optional ByVal pBlnServicioAnidado As Boolean = False) As clsEstadoTZ

        'Dim lStrUsuario As String
        'Dim lStrPassword As String
        Dim lStrResultadoXML As String
        Dim lClsEstadoTZ As New clsEstadoTZ
        Dim lExeService As New WSTopaz.executeService
        Dim lWSTopaz As New WSTopaz.JBankBeanWSService("")

        'lStrUsuario = DecodificarPinTrans(ConfigurationManager.AppSettings("UsuarioTz"))
        'lStrPassword = DecodificarPinTrans(ConfigurationManager.AppSettings("PasswordTz"))

        'lExeService.executionInfo = GenerarExecutionInfo(lStrUsuario, lStrPassword, pIntNroSession)

        'lExeService.request = GenerarRequest(pStrNombreServicio, pLstParametros)

        lStrResultadoXML = pStrResultadoServicioXML

        Dim lStrJBankData As String = String.Empty
        Dim lStrJBankMetadata As String = String.Empty

        lClsEstadoTZ = CargarDatosXML(lStrResultadoXML, lStrJBankMetadata, lStrJBankData)

        If lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError Then
            Return lClsEstadoTZ
        End If

        Select Case pEnmTipoADevolver
            Case enumTipoADevolver.enObtenerValor
                lClsEstadoTZ = ObtenerValor(lStrJBankData)

            Case enumTipoADevolver.enObtenerDataRow
                lClsEstadoTZ = ObtenerDataRow(lStrJBankMetadata, lStrJBankData, pShrNroTabla)

            Case enumTipoADevolver.enObtenerDataTable
                lClsEstadoTZ = ObtenerDataTable(lStrJBankMetadata, lStrJBankData, pShrNroTabla)

            Case enumTipoADevolver.enObtenerDataSet
                If Not pBlnServicioAnidado Then
                    lClsEstadoTZ = ObtenerDataSet(lStrJBankMetadata, lStrJBankData)
                Else
                    lClsEstadoTZ = ObtenerDataSetAnidado(lStrJBankMetadata, lStrJBankData)
                End If

        End Select

        Return lClsEstadoTZ
    End Function

#End Region

    'TODO Borrar más adelante
    Public Function EjemploTopaz(ByVal sXml As String) As DataSet

        'TOMAR EN CUENTA XML DE ERROR 
        '<?xml version="1.0" encoding="UTF-8"?>
        '<xmlJBankResponse><xmlJBankService><serviceName>[NOMBRE DE SERVICIO INACCESIBLE]</serviceName><errorCode>1</errorCode><error>Se produjo un error al intentar interpretar el XML de la solicitud.</error><status>false</status><xmlJBankMetadata/><xmlJBankData/></xmlJBankService></xmlJBankResponse>

        'Respuesta buena
        '<?xml version="1.0" encoding="UTF-8"?>
        '<xmlJBankResponse><xmlJBankService><serviceName>BuscarPersonaFisicaGNT</serviceName><errorCode>0</errorCode><error></error><status>true</status><xmlJBankMetadata><xmlJBankResultSetMetadata><id>1</id><xmlJBankFieldNames><fieldName>NOMBRE</fieldName></xmlJBankFieldNames></xmlJBankResultSetMetadata></xmlJBankMetadata><xmlJBankData><xmlJBankResultSet><id>1</id><xmlJBankRows><xmlJBankRow><xmlJBankFieldValues><fieldValue>RICARDO LEONEL TABORA SAUCEDO - TOPAZ BD</fieldValue></xmlJBankFieldValues></xmlJBankRow></xmlJBankRows></xmlJBankResultSet></xmlJBankData></xmlJBankService></xmlJBankResponse>


        Dim Xml As XmlDocument
        Dim NodeRows1 As XmlNodeList, NodeRow As XmlNode, Tabla As Integer
        Dim NodeResulset1 As XmlNode, NodeResulset2 As XmlNode

        Dim NodeList As XmlNodeList
        Dim NodeListF As XmlNodeList
        Dim myDS As DataSet

        Dim Node As XmlNode, Node1 As XmlNode, Texto As String
        Dim Campo As XmlNode, R1 As DataRow, FCampo As Integer
        'Me.OpenFileDialog1.ShowDialog()

        Xml = New XmlDocument()
        Xml.LoadXml(sXml)

        'Creo dataset
        Node = Xml.SelectSingleNode("xmlJBankResponse/xmlJBankService/serviceName")
        myDS = New Data.DataSet(Node.InnerText)
        NodeList = Xml.SelectNodes("xmlJBankResponse/xmlJBankService/xmlJBankMetadata/xmlJBankResultSetMetadata")

        For Each Node In NodeList
            Texto = "Table" & Node.FirstChild.InnerText
            'Creo Tablas
            myDS.Tables.Add(Texto)
            'Creo Campos
            NodeListF = Node.ChildNodes(1).ChildNodes

            For Each Campo In NodeListF
                myDS.Tables(Texto).Columns.Add(Campo.InnerText, Type.GetType("System.String"))
            Next
        Next

        'Cargo Resultset
        NodeResulset1 = Xml.SelectSingleNode("xmlJBankResponse/xmlJBankService/xmlJBankData/xmlJBankResultSet")
        Tabla = NodeResulset1.FirstChild.InnerText 'Recupero numero de tabla
        Texto = "Table" & Tabla
        NodeRows1 = NodeResulset1.SelectNodes("xmlJBankRows") 'Recupero rows

        For Each NodeRow In NodeRows1
            R1 = myDS.Tables(Texto).NewRow()
            'Recupero Campos
            NodeListF = NodeRow.SelectNodes("xmlJBankRow/xmlJBankFieldValues/fieldValue")

            For Each Node1 In NodeListF
                R1.Item(FCampo) = Node1.InnerText
            Next

            myDS.Tables(Texto).Rows.Add(R1)
            NodeResulset2 = NodeRow.SelectSingleNode("xmlJBankRow/xmlJBankResultSet")

            If Not NodeResulset2 Is Nothing Then
                Beep()
                'Trato segundo nivel de profundidad tabla 2
            End If
        Next

        Return myDS
    End Function

    Public Sub CrearArchivoLog(ByVal pStrMessage As String, _
                              ByVal pStrSource As String, ByVal pStrUser As String)
        Dim lStrPath As String
        Dim lStrHora As String
        Dim lStrNombreArchivo As String = String.Empty
        Dim lSwtArchivo As StreamWriter = Nothing

        Try
            lStrPath = ConfigurationManager.AppSettings("LogExecTopaz")
            lStrNombreArchivo = pStrUser & "-" & Format(Now.Date, "yyyy-MM-dd")

            '### Verificar si existe una carpeta creada para la fecha actual y carpeta de los parámetros
            If Not Directory.Exists(lStrPath) Then
                Directory.CreateDirectory(lStrPath)
            End If

            lStrHora = Format(CDate(Now), "HH:mm:ss")
            lStrPath &= lStrNombreArchivo & ".txt"

            If File.Exists(lStrPath) Then
                lSwtArchivo = File.AppendText(lStrPath)
            Else
                lSwtArchivo = File.CreateText(lStrPath)
            End If

            '### Contenido del Archivo
            lSwtArchivo.WriteLine("")
            'lSwtArchivo.WriteLine("        : " & lStrHora)
            lSwtArchivo.WriteLine(lStrHora & ": Usuario " & pStrUser)
            lSwtArchivo.WriteLine("        : " & pStrSource)
            lSwtArchivo.WriteLine("        : " & pStrMessage)
            lSwtArchivo.WriteLine("")

            '### Cierra el archivo
            If Not lSwtArchivo Is Nothing Then lSwtArchivo.Close()

        Catch ex As Exception
            If Not lSwtArchivo Is Nothing Then lSwtArchivo.Close()
        End Try
    End Sub

End Class

'<Serializable()> Public Class clsTopaz_PRUEBA

'#Region " Métodos Privados "

'    ''' <summary>
'    ''' Genera el elemento ExecutionInfo para la invocación del Servicio Web de Topaz 
'    ''' </summary>
'    ''' <param name="pStrUsuario">Usuario</param>
'    ''' <param name="pStrPassword">Password</param>
'    ''' <param name="pIntNroSession">Nro. de Session</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function GenerarExecutionInfo(ByVal pStrUsuario As String, ByVal pStrPassword As String, _
'                                         Optional ByVal pIntNroSession As Integer = 0) As String

'        Dim lStbExecution As New StringBuilder

'        lStbExecution.Append("<?xml version='1.0' encoding='UTF-8'?>")
'        lStbExecution.Append("<xmlJBankExecutionParameters><authentication><type/>")
'        lStbExecution.Append("<userName>" & pStrUsuario & "</userName>")
'        lStbExecution.Append("<password>" & pStrPassword & "</password>")
'        lStbExecution.Append("<sessionID>" & pIntNroSession & "</sessionID>")
'        lStbExecution.Append("</authentication></xmlJBankExecutionParameters>")

'        Return lStbExecution.ToString
'    End Function

'    ''' <summary>
'    ''' Genera el elemento Request para la invocación del Servicio Web de Topaz 
'    ''' </summary>
'    ''' <param name="pStrNombreServicio">Nombre del Servicio a Invocar</param>
'    ''' <param name="pLstParametros">Lista de Parámetros</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function GenerarRequest(ByVal pStrNombreServicio As String,
'                                    Optional ByVal pLstParametros As List(Of clsParametrosProxy) = Nothing) As String

'        Dim lStbRequest As New StringBuilder

'        lStbRequest.Append("<?xml version='1.0' encoding='UTF-8'?>")
'        lStbRequest.Append("<xmlJBankRequest><xmlJBankService>")
'        lStbRequest.Append("<serviceName>" & pStrNombreServicio & "</serviceName>")

'        '### Adicionar los Parámetros si Existen
'        If Not pLstParametros Is Nothing Then

'            lStbRequest.Append("<xmlJBankFields>")

'            For Each clsParametros In pLstParametros
'                lStbRequest.Append("<xmlJBankField>")
'                lStbRequest.Append("<fieldName>" & clsParametros.Nombre & "</fieldName>")
'                lStbRequest.Append("<fieldValue>" & clsParametros.Valor & "</fieldValue>")
'                lStbRequest.Append("</xmlJBankField>")
'            Next

'            lStbRequest.Append("</xmlJBankFields>")
'        End If

'        lStbRequest.Append("</xmlJBankService></xmlJBankRequest>")

'        Return lStbRequest.ToString
'    End Function

'    ''' <summary>
'    ''' Carga los elementos JBankMetadata y JBankData a partir del XML recibido
'    ''' </summary>
'    ''' <param name="pStrXML"></param>
'    ''' <param name="pStrJBankMetadata"></param>
'    ''' <param name="pStrJBankData"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function CargarDatosXML(ByVal pStrXML As String, ByRef pStrJBankMetadata As String, _
'                                    ByRef pStrJBankData As String) As clsEstadoTZ

'        Dim lClsEstadoTZ As New clsEstadoTZ

'        '### Inicializa con estado "enError"
'        lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError

'        If Trim(pStrXML) = "" Then
'            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
'            lClsEstadoTZ.Mensaje = "XML mal formado."

'            Return lClsEstadoTZ
'        End If

'        Dim lXndNodoPrincipal As XmlNode
'        Dim lXndNodoRespuesta As XmlNode
'        Dim lXmlDocumento As New XmlDocument

'        '### Cargar el Documento
'        lXmlDocumento.LoadXml(pStrXML)

'        lXndNodoPrincipal = lXmlDocumento.SelectSingleNode("xmlJBankResponse/xmlJBankService")

'        If lXndNodoPrincipal Is Nothing Then
'            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError
'            lClsEstadoTZ.Mensaje = "Nodo de respuesta no encontrado."

'            Return lClsEstadoTZ
'        End If

'        Dim lStrValor As Object = Nothing

'        For Each lXndNodoRespuesta In lXndNodoPrincipal.ChildNodes

'            Select Case UCase(lXndNodoRespuesta.Name)
'                Case "SERVICENAME", "ERRORCODE", "ERROR", "STATUS"
'                    For Each lXelElemento In lXndNodoRespuesta
'                        lStrValor = lXelElemento.Value
'                    Next

'                    'Case "XMLJBANKMETADATA" : pStrJBankMetadata = lXndNodoRespuesta.InnerXml
'                Case "XMLJBANKMETADATA" : pStrJBankMetadata = "<xmlJBankMetadata>" & lXndNodoRespuesta.InnerXml & "</xmlJBankMetadata>"

'                    'Case "XMLJBANKDATA" : pStrJBankData = lXndNodoRespuesta.InnerXml
'                Case "XMLJBANKDATA" : pStrJBankData = "<xmlJBankData>" & lXndNodoRespuesta.InnerXml & "</xmlJBankData>"

'            End Select

'            Select Case UCase(lXndNodoRespuesta.Name)
'                Case "SERVICENAME" : lClsEstadoTZ.NombreServicio = lStrValor
'                Case "ERRORCODE" : lClsEstadoTZ.CodMensaje = lStrValor
'                Case "ERROR" : lClsEstadoTZ.Mensaje = lStrValor
'                Case "STATUS" : lClsEstadoTZ.Estado = lStrValor
'            End Select
'        Next

'        If CBool(lClsEstadoTZ.Estado) = True Then
'            lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enOk
'        End If

'        Return lClsEstadoTZ
'    End Function


'    ''' <summary>
'    ''' Obtiene un Dataset con los datos resultantes
'    ''' </summary>
'    ''' <param name="pStrJBankMetadataXML">Metadata (Estructura)</param>
'    ''' <param name="pStrJBankDataXML">Datos</param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Private Function ObtenerDataSetAnidado(ByVal pStrJBankMetadataXML As String, ByVal pStrJBankDataXML As String) As clsEstadoTZ
'        Dim lXndNodo As XmlNode
'        Dim lDrwFila As DataRow
'        Dim lXndCampo As XmlNode
'        Dim lShrNroCampo As Short
'        Dim lDstTablas As New DataSet
'        Dim lXnlListaNodos As XmlNodeList
'        Dim lClsEstadoTZ As New clsEstadoTZ
'        Dim lStrNombreTabla As String = Nothing
'        Dim lXmlJBankMetadata As New XmlDocument
'        Dim lXmlJBankDataXML As New XmlDocument

'        lXmlJBankMetadata.LoadXml(pStrJBankMetadataXML)

'        '### Crear Estructura de Tablas
'        For Each lXndNodo In lXmlJBankMetadata.ChildNodes(0).ChildNodes
'            lStrNombreTabla = "Table" & lXndNodo.FirstChild.InnerText

'            '### Crear la Tabla
'            lDstTablas.Tables.Add(lStrNombreTabla)

'            '### Crear los Campos
'            lXnlListaNodos = lXndNodo.ChildNodes(1).ChildNodes

'            For Each lXndCampo In lXnlListaNodos
'                lDstTablas.Tables(lStrNombreTabla).Columns.Add(lXndCampo.InnerText, Type.GetType("System.String"))
'            Next
'        Next

'        '### Cargar los Resultados
'        Dim lStrJBankFieldValues As New StringBuilder
'        Dim lXmlNodoJBankFieldValues As XmlNode = Nothing

'        While ObtenerJBankResultSet(pStrJBankDataXML, lStrNombreTabla, lStrJBankFieldValues) ', pStrJBankDataXML)
'            lStrNombreTabla = "Table" & lStrNombreTabla
'            lXmlJBankDataXML.LoadXml(lStrJBankFieldValues.ToString)

'            '### Obtiene la lista de XMLJBANKFIELDVALUES
'            For Each lXndCampo In lXmlJBankDataXML.ChildNodes(0).ChildNodes
'                '### Crear los Campos
'                lDrwFila = lDstTablas.Tables(lStrNombreTabla).NewRow()

'                For Each lXndNodo In lXndCampo.ChildNodes
'                    lDrwFila.Item(lShrNroCampo) = lXndNodo.InnerText
'                    lShrNroCampo += 1

'                    '### Adicionado en Pruebas
'                    If lShrNroCampo = lDrwFila.ItemArray.Length Then
'                        lShrNroCampo = 0
'                        lDstTablas.Tables(lStrNombreTabla).Rows.Add(lDrwFila)
'                        lDrwFila = lDstTablas.Tables(lStrNombreTabla).NewRow()
'                    End If

'                Next
'            Next

'            lStrJBankFieldValues.Clear()
'        End While

'        lClsEstadoTZ.Resultado = lDstTablas

'        Return lClsEstadoTZ
'    End Function


'    'Private Function ObtenerJBankResultSet(ByVal pStrJBankDataXML As String, ByRef pStrID As String, _
'    '                                       ByRef lStrJBankFieldValues As StringBuilder, ByRef pStrJBankResultSetXML As String) As Boolean

'    Private Function ObtenerJBankResultSet(ByVal pStrJBankDataXML As String, ByRef pStrID As String, _
'                                        ByRef lStrJBankFieldValues As StringBuilder) As Boolean

'        If Trim(pStrJBankDataXML) = "" Then Return False
'        lStrJBankFieldValues.Clear()
'        pStrJBankDataXML = UCase(pStrJBankDataXML)

'        Dim lXmlNodo As XmlNode
'        Dim lXmlListaNodos As XmlNodeList = Nothing
'        Dim lXmlJBankData As New XmlDocument

'        lXmlJBankData.LoadXml(pStrJBankDataXML)

'        If UCase(lXmlJBankData.ChildNodes(0).Name) = "XMLJBANKDATA" OrElse _
'           UCase(lXmlJBankData.ChildNodes(0).Name) = "XMLJBANKRESULTSET" Then
'            lXmlListaNodos = lXmlJBankData.ChildNodes(0).ChildNodes
'        End If

'        If lXmlListaNodos Is Nothing Then
'            Return False
'        End If

'        For Each lXmlNodo In lXmlListaNodos
'            If UCase(lXmlNodo.Name) = "XMLJBANKRESULTSET" Then

'                lXmlListaNodos = lXmlNodo.ChildNodes
'                Exit For
'            End If
'        Next

'        Dim lIntCont As Integer
'        Dim lBlnExisteRegistro As Boolean = False
'        Dim lBlnExisteJBankResultSet As Boolean = False

'        While Not lBlnExisteJBankResultSet AndAlso (lIntCont < lXmlListaNodos.Count)
'            lXmlNodo = lXmlListaNodos.Item(lIntCont)

'            Select Case UCase(lXmlNodo.Name)
'                Case "ID"
'                    pStrID = lXmlNodo.InnerXml

'                Case "XMLJBANKROWS"
'                    '### Verificar si Contiene XMLJBANKRESULTSET
'                    lBlnExisteJBankResultSet = lXmlNodo.InnerXml.Contains("XMLJBANKRESULTSET")

'                    If lBlnExisteJBankResultSet Then
'                        Dim lXmlNodoRow As XmlNode
'                        Dim lXmlListaNodosXMLBankRow As XmlNodeList

'                        '### Obtener la lista que está en el Tag <XMLJBANKROW>
'                        lXmlListaNodosXMLBankRow = lXmlNodo.ChildNodes(0).ChildNodes

'                        If lXmlListaNodosXMLBankRow.Count > 0 Then

'                            For Each lXmlNodoRow In lXmlListaNodosXMLBankRow

'                                If lXmlNodoRow.Name = "XMLJBANKFIELDVALUES" Then
'                                    If lStrJBankFieldValues.Length = 0 Then
'                                        lBlnExisteRegistro = True
'                                        lStrJBankFieldValues.Append("<XMLDATOS>")
'                                    End If
'                                    lStrJBankFieldValues.Append(lXmlNodoRow.InnerXml)

'                                ElseIf lXmlNodoRow.Name = "XMLJBANKRESULTSET" Then
'                                    lBlnExisteJBankResultSet = True
'                                    'pStrJBankResultSetXML()
'                                    pStrJBankDataXML = "<xmlJBankResultSet>" & lXmlNodoRow.InnerXml & "</xmlJBankResultSet>"
'                                End If
'                            Next

'                            If lBlnExisteRegistro Then
'                                lStrJBankFieldValues.Append("</XMLDATOS>")
'                            End If
'                        End If

'                    Else
'                        Dim lXmlNodoRow As XmlNode
'                        Dim lXmlListaNodosXMLBankRow As XmlNodeList

'                        lXmlListaNodosXMLBankRow = lXmlNodo.ChildNodes

'                        If lXmlListaNodosXMLBankRow.Count > 0 Then
'                            lBlnExisteRegistro = True

'                            lStrJBankFieldValues.Append("<XMLDATOS>")

'                            For Each lXmlNodoRow In lXmlListaNodosXMLBankRow
'                                lStrJBankFieldValues.Append(lXmlNodoRow.InnerXml)
'                            Next

'                            lStrJBankFieldValues.Append("</XMLDATOS>")
'                        End If
'                    End If

'            End Select

'            lIntCont += 1
'        End While

'        If lBlnExisteJBankResultSet = False Then pStrJBankDataXML = String.Empty ' pStrJBankResultSetXML = String.Empty

'        '### Liberar Objetos
'        lXmlJBankData = Nothing
'        lXmlNodo = Nothing
'        lXmlListaNodos = Nothing

'        Return lBlnExisteRegistro
'    End Function

'    'Private Function ObtenerJBankResultSet(ByVal pStrJBankDataXML As String, ByRef pStrID As String, _
'    '                                       ByRef pXmlNodoJBankFieldValues As XmlNode, ByRef pStrJBankResultSetXML As String) As Boolean

'    '    If Trim(pStrJBankDataXML) = "" Then Return False

'    '    Dim lXmlNodo As XmlNode
'    '    Dim lXmlListaNodos As XmlNodeList = Nothing
'    '    Dim lXmlListaNodosHijos As XmlNodeList = Nothing
'    '    Dim lXmlJBankData As New XmlDocument

'    '    lXmlJBankData.LoadXml(pStrJBankDataXML)

'    '    If UCase(lXmlJBankData.ChildNodes(0).Name) = "XMLJBANKDATA" OrElse _
'    '       UCase(lXmlJBankData.ChildNodes(0).Name) = "XMLJBANKRESULTSET" Then
'    '        lXmlListaNodos = lXmlJBankData.ChildNodes(0).ChildNodes
'    '    End If

'    '    If lXmlListaNodos Is Nothing Then
'    '        Return False
'    '    End If

'    '    For Each lXmlNodo In lXmlListaNodos
'    '        If UCase(lXmlNodo.Name) = "XMLJBANKRESULTSET" Then

'    '            lXmlListaNodos = lXmlNodo.ChildNodes
'    '            Exit For
'    '        End If
'    '    Next

'    '    Dim lIntCont As Integer
'    '    Dim lBlnExisteRegistro As Boolean = False
'    '    Dim lBlnExisteJBankResultSet As Boolean = False

'    '    While Not lBlnExisteJBankResultSet AndAlso (lIntCont < lXmlListaNodos.Count)
'    '        lXmlNodo = lXmlListaNodos.Item(lIntCont)

'    '        Select Case UCase(lXmlNodo.Name)
'    '            Case "ID"
'    '                pStrID = lXmlNodo.InnerXml

'    '                'Case "XMLJBANKROWS", "XMLJBANKROW"
'    '                '    lIntCont = -1
'    '                '    lXmlListaNodos = lXmlNodo.ChildNodes

'    '            Case "XMLJBANKROWS" ', "XMLJBANKROW"
'    '                'lIntCont = -1
'    '                lXmlListaNodosHijos = lXmlNodo.ChildNodes

'    '                'For lIntNodoHijo = 0 To lXmlListaNodosHijos.Count - 1
'    '                '    lXmlNodo = lXmlListaNodosHijos.Item(lIntNodoHijo)
'    '                '    pStrJBankResultSetXML &= lXmlNodo.InnerXml
'    '                'Next
'    '                'pStrJBankResultSetXML = "<xmlJBankResultSet>" & pStrJBankResultSetXML & "</xmlJBankResultSet>"

'    '                For lIntNodoHijo = 0 To lXmlListaNodosHijos.Count - 1
'    '                    lXmlNodo = lXmlListaNodosHijos.Item(lIntNodoHijo)
'    '                    'pXmlNodoJBankFieldValues &= lXmlNodo.InnerXml
'    '                    If pXmlNodoJBankFieldValues Is Nothing Then
'    '                        pXmlNodoJBankFieldValues = lXmlNodo
'    '                    Else
'    '                        pXmlNodoJBankFieldValues.AppendChild(lXmlNodo)
'    '                    End If

'    '                    'pXmlNodoJBankFieldValues.AppendChild(lXmlNodo)
'    '                Next
'    '                If lXmlListaNodosHijos.Count > 0 Then
'    '                    lBlnExisteRegistro = True
'    '                End If

'    '                'pXmlNodoJBankFieldValues = "<xmlJBankResultSet>" & pXmlNodoJBankFieldValues & "</xmlJBankResultSet>"

'    '                'Case "XMLJBANKROW"
'    '                '    lIntCont = -1
'    '                '    lXmlListaNodos = lXmlNodo.ChildNodes

'    '            Case "XMLJBANKFIELDVALUES"
'    '                lBlnExisteRegistro = True
'    '                pXmlNodoJBankFieldValues = lXmlNodo

'    '            Case "XMLJBANKRESULTSET"
'    '                lBlnExisteJBankResultSet = True
'    '                pStrJBankResultSetXML = "<xmlJBankResultSet>" & lXmlNodo.InnerXml & "</xmlJBankResultSet>"
'    '        End Select

'    '        lIntCont += 1
'    '    End While

'    '    If lBlnExisteJBankResultSet = False Then pStrJBankResultSetXML = String.Empty

'    '    '### Liberar Objetos
'    '    lXmlJBankData = Nothing
'    '    lXmlNodo = Nothing
'    '    lXmlListaNodos = Nothing

'    '    Return lBlnExisteRegistro
'    'End Function


'#End Region


'#Region " Métodos Privados de Decodificación "

'    Private Function DecodificarPinTrans(ByVal pStrPina As String) As String
'        Dim lStrPin As String = ""
'        Dim lStrCad As String = ""
'        Dim lShrCont As Short

'        pStrPina = Trim(pStrPina)
'        If pStrPina = String.Empty Then Return String.Empty

'        pStrPina &= IIf(pStrPina.Chars(pStrPina.Length - 1) = "|", "", "|")

'        For lShrCont = 1 To pStrPina.Length
'            If pStrPina.Chars(lShrCont - 1) <> "|" Then
'                lStrCad &= pStrPina.Chars(lShrCont - 1)
'            Else
'                lStrPin &= Chr(CInt(DecodificarPin(CInt(lStrCad))))
'                lStrCad = ""
'            End If
'        Next

'        Return lStrPin
'    End Function

'    Private Function DecodificarPin(ByVal pIntPina As Integer) As String
'        Dim lStrAuxPin As String
'        Dim lStrBase As String = Nothing
'        Dim lIntContI As Integer
'        Dim lIntContJ As Integer
'        Dim lStrPin As String = Nothing

'        lStrAuxPin = Format(pIntPina, "0000")

'        For lIntContI = 1 To 4
'            Select Case lIntContI
'                Case "1"
'                    lStrBase = "5761892304"
'                Case "2"
'                    lStrBase = "9376524180"
'                Case "3"
'                    lStrBase = "0493185267"
'                Case "4"
'                    lStrBase = "7319248056"
'            End Select

'            For lIntContJ = 1 To 10
'                If Mid(lStrAuxPin, lIntContI, 1) = Mid(lStrBase, lIntContJ, 1) Then
'                    Exit For
'                End If
'            Next lIntContJ

'            If lIntContJ > 9 Then
'                lIntContJ = 0
'            End If

'            lStrPin = lStrPin & Format(lIntContJ, "0")
'        Next lIntContI

'        Return lStrPin
'    End Function

'#End Region


'#Region " Métodos Públicos "

'    Public Function EjecutarServicio(ByVal pStrNombreServicio As String, _
'                                     ByVal pEnmTipoADevolver As enumTipoADevolver, _
'                                     ByVal pLstParametros As List(Of clsParametrosProxy), _
'                                     Optional ByVal pIntNroSession As Integer = 0, _
'                                     Optional ByVal pShrNroTabla As Short = 1) As clsEstadoTZ

'        Dim lStrUsuario As String
'        Dim lStrPassword As String
'        Dim lStrResultadoXML As String
'        Dim lClsEstadoTZ As New clsEstadoTZ
'        Dim lExeService As New WSTopaz.executeService
'        Dim lWSTopaz As New WSTopaz.JBankBeanWSService

'        lStrUsuario = DecodificarPinTrans(ConfigurationManager.AppSettings("UsuarioTz"))
'        lStrPassword = DecodificarPinTrans(ConfigurationManager.AppSettings("PasswordTz"))

'        lExeService.executionInfo = GenerarExecutionInfo(lStrUsuario, lStrPassword, pIntNroSession)

'        lExeService.request = GenerarRequest(pStrNombreServicio, pLstParametros)

'        lStrResultadoXML = lWSTopaz.executeService(lExeService).executeServiceResult

'        Dim lStrJBankData As String = String.Empty
'        Dim lStrJBankMetadata As String = String.Empty

'        lClsEstadoTZ = CargarDatosXML(lStrResultadoXML, lStrJBankMetadata, lStrJBankData)

'        If lClsEstadoTZ.CodMsjeProceso = enumEstadoEjecucion.enError Then
'            Return lClsEstadoTZ
'        End If

'        Select Case pEnmTipoADevolver
'            Case enumTipoADevolver.enObtenerDataSet
'                lClsEstadoTZ = ObtenerDataSetAnidado(lStrJBankMetadata, lStrJBankData)
'        End Select

'        Return lClsEstadoTZ
'    End Function

'#End Region

'    'TODO Borrar más adelante
'    Public Function EjemploTopaz(ByVal sXml As String) As DataSet

'        'TOMAR EN CUENTA XML DE ERROR 
'        '<?xml version="1.0" encoding="UTF-8"?>
'        '<xmlJBankResponse><xmlJBankService><serviceName>[NOMBRE DE SERVICIO INACCESIBLE]</serviceName><errorCode>1</errorCode><error>Se produjo un error al intentar interpretar el XML de la solicitud.</error><status>false</status><xmlJBankMetadata/><xmlJBankData/></xmlJBankService></xmlJBankResponse>

'        'Respuesta buena
'        '<?xml version="1.0" encoding="UTF-8"?>
'        '<xmlJBankResponse><xmlJBankService><serviceName>BuscarPersonaFisicaGNT</serviceName><errorCode>0</errorCode><error></error><status>true</status><xmlJBankMetadata><xmlJBankResultSetMetadata><id>1</id><xmlJBankFieldNames><fieldName>NOMBRE</fieldName></xmlJBankFieldNames></xmlJBankResultSetMetadata></xmlJBankMetadata><xmlJBankData><xmlJBankResultSet><id>1</id><xmlJBankRows><xmlJBankRow><xmlJBankFieldValues><fieldValue>RICARDO LEONEL TABORA SAUCEDO - TOPAZ BD</fieldValue></xmlJBankFieldValues></xmlJBankRow></xmlJBankRows></xmlJBankResultSet></xmlJBankData></xmlJBankService></xmlJBankResponse>


'        Dim Xml As XmlDocument
'        Dim NodeRows1 As XmlNodeList, NodeRow As XmlNode, Tabla As Integer
'        Dim NodeResulset1 As XmlNode, NodeResulset2 As XmlNode

'        Dim NodeList As XmlNodeList
'        Dim NodeListF As XmlNodeList
'        Dim myDS As DataSet

'        Dim Node As XmlNode, Node1 As XmlNode, Texto As String
'        Dim Campo As XmlNode, R1 As DataRow, FCampo As Integer
'        'Me.OpenFileDialog1.ShowDialog()

'        Xml = New XmlDocument()
'        Xml.LoadXml(sXml)

'        'Creo dataset
'        Node = Xml.SelectSingleNode("xmlJBankResponse/xmlJBankService/serviceName")
'        myDS = New Data.DataSet(Node.InnerText)
'        NodeList = Xml.SelectNodes("xmlJBankResponse/xmlJBankService/xmlJBankMetadata/xmlJBankResultSetMetadata")

'        For Each Node In NodeList
'            Texto = "Table" & Node.FirstChild.InnerText
'            'Creo Tablas
'            myDS.Tables.Add(Texto)
'            'Creo Campos
'            NodeListF = Node.ChildNodes(1).ChildNodes

'            For Each Campo In NodeListF
'                myDS.Tables(Texto).Columns.Add(Campo.InnerText, Type.GetType("System.String"))
'            Next
'        Next

'        'Cargo Resultset
'        NodeResulset1 = Xml.SelectSingleNode("xmlJBankResponse/xmlJBankService/xmlJBankData/xmlJBankResultSet")
'        Tabla = NodeResulset1.FirstChild.InnerText 'Recupero numero de tabla
'        Texto = "Table" & Tabla
'        NodeRows1 = NodeResulset1.SelectNodes("xmlJBankRows") 'Recupero rows

'        For Each NodeRow In NodeRows1
'            R1 = myDS.Tables(Texto).NewRow()
'            'Recupero Campos
'            NodeListF = NodeRow.SelectNodes("xmlJBankRow/xmlJBankFieldValues/fieldValue")

'            For Each Node1 In NodeListF
'                R1.Item(FCampo) = Node1.InnerText
'            Next

'            myDS.Tables(Texto).Rows.Add(R1)
'            NodeResulset2 = NodeRow.SelectSingleNode("xmlJBankRow/xmlJBankResultSet")

'            If Not NodeResulset2 Is Nothing Then
'                Beep()
'                'Trato segundo nivel de profundidad tabla 2
'            End If
'        Next

'        Return myDS
'    End Function

'End Class
