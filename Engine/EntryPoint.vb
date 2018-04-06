﻿
Imports System.ComponentModel
Imports POS.CodeModule

Module EntryPoint

    'Dim Thread1 As New System.Threading.Thread(AddressOf BackgroundProcess)
    Private WithEvents MailChecker As New BackgroundWorker

    Public Sub main()
        Try
            Dim arr As New ArrayList
            arr = ReadConnection.GetFileContents("Connection.txt")

            ConnStr = arr(0)

         
            Dim STR As String = ConnStr

            Dim Index_From As Integer = STR.IndexOf("Dsn=") + 4
            STR = STR.Substring(Index_From)
            Dim Index_To As Integer = STR.IndexOf(";")
            DatabaseName = STR.Substring(0, Index_To)

            Index_From = STR.IndexOf("Uid=") + 4
            STR = STR.Substring(Index_From)
            Index_To = STR.IndexOf(";")
            DatabaseLogID = STR.Substring(0, Index_To)

            Index_From = STR.IndexOf("Pwd=") + 4
            DatabasePass = STR.Substring(Index_From)


            If DBO.Connect(DatabaseName, DatabaseLogID, DatabasePass) Then


                CLS_Config = New SysConfig
                'CurrentShift = DataF.GetCurrentShift

                'HeaderApperance.BackColor = System.Drawing.Color.FromArgb(CType(CType(234, Byte), Integer), CType(CType(240, Byte), Integer), CType(CType(248, Byte), Integer))
                'HeaderApperance.BackColor2 = System.Drawing.Color.FromArgb(CType(CType(209, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(240, Byte), Integer))


                'Thread1.IsBackground = True
                'Thread1.Start()

                If IsNothing(frmLoginIns) Then
                    frmLoginIns = New frmLogin
                    If frmLoginIns.ShowDialog() = DialogResult.OK Then
                        'Show Main
                        'Audit.LogIn()

                        If IsNothing(frmMainIns) Then
                            frmMainIns = New frmMain
                            frmMainIns.ShowDialog()
                            'Audit.LogOut()

                        End If
                    Else
                        'Close Application
                        End
                    End If
                End If

            Else
                MsgBox("لا يمكن الاتصال بالخادم")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    'Permission Refreshing
    Dim Bool As Boolean = True

    Private Sub MailChecker_DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles MailChecker.DoWork
        While Bool
            'GetUnReadMsg()
        End While
    End Sub

    'Private Sub BackgroundProcess()
    '    Do While True
    '        'Set Current Shift
    '        If DataF.GetCurrentShift <> CurrentShift Then
    '            MSG.InfoOk("Shift was changed")
    '        End If
    '    Loop
    'End Sub

End Module