﻿' Copyright 2016-2019 Hermann Mundprecht
' This file is licensed under the terms of the license 'CC BY 4.0'. 
' For a human readable version of the license, see https://creativecommons.org/licenses/by/4.0/

Imports SAP.Middleware.Connector
Imports System.Configuration
Imports System.Collections.Specialized

Public Class SapExcelDestinationConfiguration
    Private Shared ReadOnly log As log4net.ILog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType)

    Private Shared inMemoryDestinationConfiguration As New SapInMemoryDestinationConfiguration()

    Public Shared Sub SetUp()
        '' register the in-memory destination configuration -- called before executing any of the examples
        log.Debug("SetUp - " & "RegisterDestinationConfiguration")
        Try
            RfcDestinationManager.RegisterDestinationConfiguration(inMemoryDestinationConfiguration)
        Catch Exc As System.Exception
            MsgBox(Exc.ToString, MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, "SapExcelDestinationConfiguration;SetUp")
            log.Error("SetUp - Exception=" & Exc.ToString)
            Exit Sub
        End Try
    End Sub

    Public Shared Sub TearDown(Optional destinationName As String = Nothing)
        '' unregister the in-memory destination configuration -- called after we are done working with the examples 
        RfcDestinationManager.UnregisterDestinationConfiguration(inMemoryDestinationConfiguration)
        If destinationName IsNot Nothing Then
            inMemoryDestinationConfiguration.RemoveDestination(destinationName)
        End If
    End Sub

    Public Shared Sub ConfigAddOrChangeDestination()
        Dim conParameter As New ConParameter
        Dim parameters As New RfcConfigParameters()
        Dim sAll As NameValueCollection
        Dim iD As String
        Dim par As String
        sAll = ConfigurationSettings.AppSettings()
        Dim s As String
        For Each s In sAll.AllKeys
            iD = Right(s, 1)
            par = Left(s, Len(s) - 1)
            log.Debug("ConfigAddOrChangeDestination - conParameter.addConValue iD=" & CStr(iD) & " Field=" & par & " Value=" & CStr(sAll(s)))
            conParameter.addConValue(iD, par, CStr(sAll(s)))
            Console.WriteLine("Key: " & s & " Value: " & sAll(s))
        Next
        Dim conRec As ConParamterRec
        For Each conRec In conParameter.aConCol
            parameters = New RfcConfigParameters()
            parameters(RfcConfigParameters.Name) = conRec.aName.Value
            parameters(RfcConfigParameters.PeakConnectionsLimit) = "5"
            parameters(RfcConfigParameters.ConnectionIdleTimeout) = "600" '' 600 seconds, i.e. 10 minutes
            If conRec.aAppServerHost.Value IsNot Nothing Then
                parameters(RfcConfigParameters.AppServerHost) = conRec.aAppServerHost.Value
                parameters(RfcConfigParameters.SystemNumber) = CInt(conRec.aSystemNumber.Value)
            ElseIf conRec.aMessageServerHost.Value IsNot Nothing Then
                parameters(RfcConfigParameters.MessageServerHost) = conRec.aMessageServerHost.Value
                parameters(RfcConfigParameters.LogonGroup) = conRec.aLogonGroup.Value
            End If
            parameters(RfcConfigParameters.SystemID) = conRec.aSystemID.Value
            If conRec.aTrace.Value IsNot Nothing Then
                parameters(RfcConfigParameters.Trace) = conRec.aTrace.Value
            End If
            If conRec.aClient.Value IsNot Nothing Then
                parameters(RfcConfigParameters.Client) = conRec.aClient.Value
            End If
            If conRec.aLanguage.Value IsNot Nothing Then
                parameters(RfcConfigParameters.Language) = conRec.aLanguage.Value
            End If
            If conRec.aSncMode.Value IsNot Nothing Then
                parameters(RfcConfigParameters.SncMode) = conRec.aSncMode.Value
                parameters(RfcConfigParameters.SncPartnerName) = conRec.aSncPartnerName.Value
                If conRec.aSncMyName.Value IsNot Nothing Then
                    parameters(RfcConfigParameters.SncMyName) = conRec.aSncMyName.Value
                End If
            End If
            log.Debug("ConfigAddOrChangeDestination - inMemoryDestinationConfiguration.AddOrEditDestination Name=" & conRec.aName.Value)
            Try
                inMemoryDestinationConfiguration.AddOrEditDestination(parameters)
            Catch Exc As System.Exception
                log.Error("ConfigAddOrChangeDestination - Exception=" & Exc.ToString)
            End Try
        Next
    End Sub

    Public Shared Sub ExcelAddOrChangeDestination(pWSname As String)
        Dim conParameter As New ConParameter
        Dim parameters As New RfcConfigParameters()
        Dim i As Integer
        Dim j As Integer

        Dim aPws As Excel.Worksheet
        Dim aWB As Excel.Workbook
        aWB = Globals.SapAccAddIn.Application.ActiveWorkbook
        Try
            aPws = aWB.Worksheets(pWSname)
        Catch Exc As System.Exception
            MsgBox("No " & pWSname & " Sheet in current workbook", MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical, "SAP Acc")
            Exit Sub
        End Try
        i = 2
        Do Until CStr(aPws.Cells(2, i).value) = ""
            For j = 2 To 12
                If CStr(aPws.Cells(j, i).value) <> "" Then
                    log.Debug("ExcelAddOrChangeDestination - conParameter.addConValue iD=" & CStr(i - 2) & " Field=" & CStr(aPws.Cells(j, 1).value) & " Value=" & CStr(aPws.Cells(j, i).value))
                    conParameter.addConValue(CStr(i - 2), CStr(aPws.Cells(j, 1).value), CStr(aPws.Cells(j, i).value))
                End If
            Next
            i = i + 1
        Loop
        Dim conRec As ConParamterRec
        For Each conRec In conParameter.aConCol
            parameters = New RfcConfigParameters()
            parameters(RfcConfigParameters.Name) = conRec.aName.Value
            parameters(RfcConfigParameters.PeakConnectionsLimit) = "5"
            parameters(RfcConfigParameters.ConnectionIdleTimeout) = "600" '' 600 seconds, i.e. 10 minutes
            If conRec.aAppServerHost.Value IsNot Nothing Then
                parameters(RfcConfigParameters.AppServerHost) = conRec.aAppServerHost.Value
                parameters(RfcConfigParameters.SystemNumber) = CInt(conRec.aSystemNumber.Value)
            ElseIf conRec.aMessageServerHost.Value IsNot Nothing Then
                parameters(RfcConfigParameters.MessageServerHost) = conRec.aMessageServerHost.Value
                parameters(RfcConfigParameters.LogonGroup) = conRec.aLogonGroup.Value
            End If
            parameters(RfcConfigParameters.SystemID) = conRec.aSystemID.Value
            If conRec.aTrace.Value IsNot Nothing Then
                parameters(RfcConfigParameters.Trace) = conRec.aTrace.Value
            End If
            If conRec.aClient.Value IsNot Nothing Then
                parameters(RfcConfigParameters.Client) = conRec.aClient.Value
            End If
            If conRec.aLanguage.Value IsNot Nothing Then
                parameters(RfcConfigParameters.Language) = conRec.aLanguage.Value
            End If
            If conRec.aSncMode.Value IsNot Nothing Then
                parameters(RfcConfigParameters.SncMode) = conRec.aSncMode.Value
                parameters(RfcConfigParameters.SncPartnerName) = conRec.aSncPartnerName.Value
                If conRec.aSncMyName.Value IsNot Nothing Then
                    parameters(RfcConfigParameters.SncMyName) = conRec.aSncMyName.Value
                End If
            End If
            Try
                log.Debug("ExcelAddOrChangeDestination - inMemoryDestinationConfiguration.AddOrEditDestination Name=" & conRec.aName.Value)
                inMemoryDestinationConfiguration.AddOrEditDestination(parameters)
            Catch Exc As System.Exception
                log.Error("ExcelAddOrChangeDestination - Exception=" & Exc.ToString)
            End Try
        Next
    End Sub

    Public Function getDestinationList() As Collection
        Dim list As New Collection
        Dim availableDestinations As Dictionary(Of String, RfcConfigParameters)
        log.Debug("getDestinationList - getting availableDestinations")
        availableDestinations = inMemoryDestinationConfiguration.getAvailableDestinations()
        Dim key As String
        For Each key In availableDestinations.Keys
            list.Add(key)
        Next
        getDestinationList = list
        log.Debug("getDestinationList - getDestinationList.Count=" & CStr(getDestinationList.Count))
    End Function
End Class
