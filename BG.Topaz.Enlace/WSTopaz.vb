﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18052
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.Xml.Serialization

'
'This source code was auto-generated by wsdl, Version=4.0.30319.1.
'

Namespace WSTopaz

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Web.Services.WebServiceBindingAttribute(Name:="JBankBeanWSBinding", [Namespace]:="http://jbankws.jbank.topsystems/")> _
    Partial Public Class JBankBeanWSService
        Inherits System.Web.Services.Protocols.SoapHttpClientProtocol

        Private executeServiceOperationCompleted As System.Threading.SendOrPostCallback

        Private executeUndoOperationCompleted As System.Threading.SendOrPostCallback

        '''<remarks/>
        Public Sub New(ByVal pStrURL As String)
            MyBase.New()
            Me.Url = pStrURL
        End Sub

        '''<remarks/>
        Public Event executeServiceCompleted As executeServiceCompletedEventHandler

        '''<remarks/>
        Public Event executeUndoCompleted As executeUndoCompletedEventHandler

        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)> _
        Public Function executeService(<System.Xml.Serialization.XmlElementAttribute("executeService", [Namespace]:="http://jbankws.jbank.topsystems/")> ByVal executeService1 As executeService) As <System.Xml.Serialization.XmlElementAttribute("executeServiceResponse", [Namespace]:="http://jbankws.jbank.topsystems/")> executeServiceResponse
            Dim results() As Object = Me.Invoke("executeService", New Object() {executeService1})
            Return CType(results(0), executeServiceResponse)
        End Function

        '''<remarks/>
        Public Function BeginexecuteService(ByVal executeService1 As executeService, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("executeService", New Object() {executeService1}, callback, asyncState)
        End Function

        '''<remarks/>
        Public Function EndexecuteService(ByVal asyncResult As System.IAsyncResult) As executeServiceResponse
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0), executeServiceResponse)
        End Function

        '''<remarks/>
        Public Overloads Sub executeServiceAsync(ByVal executeService1 As executeService)
            Me.executeServiceAsync(executeService1, Nothing)
        End Sub

        '''<remarks/>
        Public Overloads Sub executeServiceAsync(ByVal executeService1 As executeService, ByVal userState As Object)
            If (Me.executeServiceOperationCompleted Is Nothing) Then
                Me.executeServiceOperationCompleted = AddressOf Me.OnexecuteServiceOperationCompleted
            End If
            Me.InvokeAsync("executeService", New Object() {executeService1}, Me.executeServiceOperationCompleted, userState)
        End Sub

        Private Sub OnexecuteServiceOperationCompleted(ByVal arg As Object)
            If (Not (Me.executeServiceCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent executeServiceCompleted(Me, New executeServiceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub

        '''<remarks/>
        <System.Web.Services.Protocols.SoapDocumentMethodAttribute("", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Bare)> _
        Public Function executeUndo(<System.Xml.Serialization.XmlElementAttribute("executeUndo", [Namespace]:="http://jbankws.jbank.topsystems/")> ByVal executeUndo1 As executeUndo) As <System.Xml.Serialization.XmlElementAttribute("executeUndoResponse", [Namespace]:="http://jbankws.jbank.topsystems/")> executeUndoResponse
            Dim results() As Object = Me.Invoke("executeUndo", New Object() {executeUndo1})
            Return CType(results(0), executeUndoResponse)
        End Function

        '''<remarks/>
        Public Function BeginexecuteUndo(ByVal executeUndo1 As executeUndo, ByVal callback As System.AsyncCallback, ByVal asyncState As Object) As System.IAsyncResult
            Return Me.BeginInvoke("executeUndo", New Object() {executeUndo1}, callback, asyncState)
        End Function

        '''<remarks/>
        Public Function EndexecuteUndo(ByVal asyncResult As System.IAsyncResult) As executeUndoResponse
            Dim results() As Object = Me.EndInvoke(asyncResult)
            Return CType(results(0), executeUndoResponse)
        End Function

        '''<remarks/>
        Public Overloads Sub executeUndoAsync(ByVal executeUndo1 As executeUndo)
            Me.executeUndoAsync(executeUndo1, Nothing)
        End Sub

        '''<remarks/>
        Public Overloads Sub executeUndoAsync(ByVal executeUndo1 As executeUndo, ByVal userState As Object)
            If (Me.executeUndoOperationCompleted Is Nothing) Then
                Me.executeUndoOperationCompleted = AddressOf Me.OnexecuteUndoOperationCompleted
            End If
            Me.InvokeAsync("executeUndo", New Object() {executeUndo1}, Me.executeUndoOperationCompleted, userState)
        End Sub

        Private Sub OnexecuteUndoOperationCompleted(ByVal arg As Object)
            If (Not (Me.executeUndoCompletedEvent) Is Nothing) Then
                Dim invokeArgs As System.Web.Services.Protocols.InvokeCompletedEventArgs = CType(arg, System.Web.Services.Protocols.InvokeCompletedEventArgs)
                RaiseEvent executeUndoCompleted(Me, New executeUndoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState))
            End If
        End Sub

        '''<remarks/>
        Public Shadows Sub CancelAsync(ByVal userState As Object)
            MyBase.CancelAsync(userState)
        End Sub
    End Class

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.SerializableAttribute(), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://jbankws.jbank.topsystems/")> _
    Partial Public Class executeService

        Private executionInfoField As String

        Private requestField As String

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> _
        Public Property executionInfo() As String
            Get
                Return Me.executionInfoField
            End Get
            Set(value As String)
                Me.executionInfoField = value
            End Set
        End Property

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> _
        Public Property request() As String
            Get
                Return Me.requestField
            End Get
            Set(value As String)
                Me.requestField = value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.SerializableAttribute(), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://jbankws.jbank.topsystems/")> _
    Partial Public Class executeUndoResponse

        Private undoServiceResultField As String

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> _
        Public Property undoServiceResult() As String
            Get
                Return Me.undoServiceResultField
            End Get
            Set(value As String)
                Me.undoServiceResultField = value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.SerializableAttribute(), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://jbankws.jbank.topsystems/")> _
    Partial Public Class executeUndo

        Private executionInfoField As String

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> _
        Public Property executionInfo() As String
            Get
                Return Me.executionInfoField
            End Get
            Set(value As String)
                Me.executionInfoField = value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.SerializableAttribute(), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code"), _
     System.Xml.Serialization.XmlTypeAttribute([Namespace]:="http://jbankws.jbank.topsystems/")> _
    Partial Public Class executeServiceResponse

        Private executeServiceResultField As String

        '''<remarks/>
        <System.Xml.Serialization.XmlElementAttribute(Form:=System.Xml.Schema.XmlSchemaForm.Unqualified)> _
        Public Property executeServiceResult() As String
            Get
                Return Me.executeServiceResultField
            End Get
            Set(value As String)
                Me.executeServiceResultField = value
            End Set
        End Property
    End Class

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")> _
    Public Delegate Sub executeServiceCompletedEventHandler(ByVal sender As Object, ByVal e As executeServiceCompletedEventArgs)

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code")> _
    Partial Public Class executeServiceCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs

        Private results() As Object

        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub

        '''<remarks/>
        Public ReadOnly Property Result() As executeServiceResponse
            Get
                Me.RaiseExceptionIfNecessary()
                Return CType(Me.results(0), executeServiceResponse)
            End Get
        End Property
    End Class

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")> _
    Public Delegate Sub executeUndoCompletedEventHandler(ByVal sender As Object, ByVal e As executeUndoCompletedEventArgs)

    '''<remarks/>
    <System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1"), _
     System.Diagnostics.DebuggerStepThroughAttribute(), _
     System.ComponentModel.DesignerCategoryAttribute("code")> _
    Partial Public Class executeUndoCompletedEventArgs
        Inherits System.ComponentModel.AsyncCompletedEventArgs

        Private results() As Object

        Friend Sub New(ByVal results() As Object, ByVal exception As System.Exception, ByVal cancelled As Boolean, ByVal userState As Object)
            MyBase.New(exception, cancelled, userState)
            Me.results = results
        End Sub

        '''<remarks/>
        Public ReadOnly Property Result() As executeUndoResponse
            Get
                Me.RaiseExceptionIfNecessary()
                Return CType(Me.results(0), executeUndoResponse)
            End Get
        End Property
    End Class

End Namespace
