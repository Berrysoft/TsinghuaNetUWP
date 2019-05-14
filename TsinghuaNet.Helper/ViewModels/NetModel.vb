﻿Imports System.Windows.Input

Public Class NetModel
    Inherits NetObservableBase

    Private _Status As INetStatus
    Public Property Status As INetStatus
        Get
            Return _Status
        End Get
        Set(value As INetStatus)
            SetProperty(_Status, value, onChanged:=AddressOf RefreshStatus)
        End Set
    End Property

    Private _SuggestState As NetState
    Public Property SuggestState As NetState
        Get
            Return _SuggestState
        End Get
        Set(value As NetState)
            SetProperty(_SuggestState, value)
        End Set
    End Property

    Public ReadOnly Property RefreshStatusCommand As ICommand = New Command(Me, AddressOf RefreshStatus)
    Public Async Sub RefreshStatus()
        Await RefreshStatusAsync()
    End Sub
    Public Async Function RefreshStatusAsync() As Task
        Try
            IsBusy = True
            Await Status.RefreshAsync()
            SuggestState = Await Status.SuggestAsync()
        Finally
            IsBusy = False
        End Try
    End Function

    Private _OnlineUser As FluxUser
    Public Property OnlineUser As FluxUser
        Get
            Return _OnlineUser
        End Get
        Set(value As FluxUser)
            SetProperty(_OnlineUser, value)
        End Set
    End Property

    Public Event ReceivedResponse As EventHandler(Of LogResponse)

    Protected Overridable Sub OnReceivedResponse(res As LogResponse)
        RaiseEvent ReceivedResponse(Me, res)
    End Sub

    Friend Async Function NetCommandExecuteAsync(executor As Func(Of IConnect, Task(Of LogResponse))) As Task
        If Not IsBusy Then
            Try
                IsBusy = True
                Dim helper = Credential.GetHelper()
                OnReceivedResponse(Await executor(helper))
            Catch ex As Exception
                OnReceivedResponse(New LogResponse(False, ex.Message))
            Finally
                IsBusy = False
            End Try
        End If
    End Function

    Public ReadOnly Property LoginCommand As ICommand = New NetCommand(Me, AddressOf LoginAsync)
    Protected Overridable Async Function LoginAsync(helper As IConnect) As Task(Of LogResponse)
        Dim res As New LogResponse(True, "登录成功")
        If helper IsNot Nothing Then
            Dim r = Await helper.LoginAsync()
            If Not r.Succeed Then
                res = r
            End If
        End If
        Await RefreshAsync(helper)
        Return res
    End Function
    Public Function LoginAsync() As Task
        Return NetCommandExecuteAsync(AddressOf LoginAsync)
    End Function
    Public Async Sub Login()
        Await LoginAsync()
    End Sub

    Public ReadOnly Property LogoutCommand As ICommand = New NetCommand(Me, AddressOf LogoutAsync)
    Protected Overridable Async Function LogoutAsync(helper As IConnect) As Task(Of LogResponse)
        Dim res As New LogResponse(True, "注销成功")
        If helper IsNot Nothing Then
            Dim r = Await helper.LogoutAsync()
            If Not r.Succeed Then
                res = r
            End If
        End If
        Await RefreshAsync(helper)
        Return res
    End Function
    Public Function LogoutAsync() As Task
        Return NetCommandExecuteAsync(AddressOf LogoutAsync)
    End Function
    Public Async Sub Logout()
        Await LogoutAsync()
    End Sub

    Public ReadOnly Property RefreshCommand As ICommand = New NetCommand(Me, AddressOf RefreshAsync)
    Protected Overridable Async Function RefreshAsync(helper As IConnect) As Task(Of LogResponse)
        Dim user As FluxUser = Nothing
        If helper IsNot Nothing Then
            user = Await helper.GetFluxAsync()
        End If
        OnlineUser = user
        Return New LogResponse(True, String.Empty)
    End Function
    Public Function RefreshAsync() As Task
        Return NetCommandExecuteAsync(AddressOf RefreshAsync)
    End Function
    Public Async Sub Refresh()
        Await RefreshAsync()
    End Sub
End Class
