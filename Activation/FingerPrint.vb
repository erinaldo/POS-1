﻿Imports System.Management
Imports System.Security.Cryptography
Imports System.Security
Imports System.Collections
Imports System.Text
''' <summary>
''' Generates a 16 byte Unique Identification code of a computer
''' Example: 4876-8DB5-EE85-69D3-FE52-8CF7-395D-2EA9
''' </summary>
Public Class FingerPrint
    Private Shared fingerPrint As String = String.Empty
    Public Shared Function Value(ByVal KEY As String) As String
        If String.IsNullOrEmpty(fingerPrint) Then
            '+"\nDISK >> "+ diskId() + "\nVIDEO >> " + videoId() +"\nMAC >> "+ macId()
            fingerPrint = GetHash(KEY & "CPU >> " & cpuId() & vbLf & "BIOS >> " & biosId() & vbLf & "BASE >> " & baseId())
        End If
        Return fingerPrint
    End Function
    Private Shared Function GetHash(ByVal s As String) As String
        Dim sec As MD5 = New MD5CryptoServiceProvider()
        Dim enc As New ASCIIEncoding()
        Dim bt As Byte() = enc.GetBytes(s)
        Return GetHexString(sec.ComputeHash(bt))
    End Function
    Private Shared Function GetHexString(ByVal bt As Byte()) As String
        Dim s As String = String.Empty
        For i As Integer = 0 To bt.Length - 1
            Dim b As Byte = bt(i)
            Dim n As Integer, n1 As Integer, n2 As Integer
            n = CInt(b)
            n1 = n And 15
            n2 = (n >> 4) And 15
            If n2 > 9 Then
                s += (ChrW(n2 - 10 + AscW("A"c))).ToString()
            Else
                s += n2.ToString()
            End If
            If n1 > 9 Then
                s += (ChrW(n1 - 10 + AscW("A"c))).ToString()
            Else
                s += n1.ToString()
            End If
            If (i + 1) <> bt.Length AndAlso (i + 1) Mod 2 = 0 Then
                s += "-"
            End If
        Next
        Return s
    End Function
#Region "Original Device ID Getting Code"
    'Return a hardware identifier
    Private Shared Function identifier(ByVal wmiClass As String, ByVal wmiProperty As String, ByVal wmiMustBeTrue As String) As String
        Dim result As String = ""
        Dim mc As New System.Management.ManagementClass(wmiClass)
        Dim moc As System.Management.ManagementObjectCollection = mc.GetInstances()
        For Each mo As System.Management.ManagementObject In moc
            If mo(wmiMustBeTrue).ToString() = "True" Then
                'Only get the first one
                If result = "" Then
                    Try
                        result = mo(wmiProperty).ToString()
                        Exit Try
                    Catch
                    End Try
                End If
            End If
        Next
        Return result
    End Function
    'Return a hardware identifier
    Private Shared Function identifier(ByVal wmiClass As String, ByVal wmiProperty As String) As String
        Dim result As String = ""
        Dim mc As New System.Management.ManagementClass(wmiClass)
        Dim moc As System.Management.ManagementObjectCollection = mc.GetInstances()
        For Each mo As System.Management.ManagementObject In moc
            'Only get the first one
            If result = "" Then
                Try
                    result = mo(wmiProperty).ToString()
                    Exit Try
                Catch
                End Try
            End If
        Next
        Return result
    End Function
    Private Shared Function cpuId() As String
        'Uses first CPU identifier available in order of preference
        'Don't get all identifiers, as very time consuming
        Dim retVal As String = identifier("Win32_Processor", "UniqueId")
        If retVal = "" Then
            'If no UniqueID, use ProcessorID
            retVal = identifier("Win32_Processor", "ProcessorId")
            If retVal = "" Then
                'If no ProcessorId, use Name
                retVal = identifier("Win32_Processor", "Name")
                If retVal = "" Then
                    'If no Name, use Manufacturer
                    retVal = identifier("Win32_Processor", "Manufacturer")
                End If
                'Add clock speed for extra security
                retVal += identifier("Win32_Processor", "MaxClockSpeed")
            End If
        End If
        Return retVal
    End Function
    'BIOS Identifier
    Private Shared Function biosId() As String
        Return identifier("Win32_BIOS", "Manufacturer") & identifier("Win32_BIOS", "SMBIOSBIOSVersion") & identifier("Win32_BIOS", "IdentificationCode") & identifier("Win32_BIOS", "SerialNumber") & identifier("Win32_BIOS", "ReleaseDate") & identifier("Win32_BIOS", "Version")
    End Function
    'Main physical hard drive ID
    Private Shared Function diskId() As String
        Return identifier("Win32_DiskDrive", "Model") & identifier("Win32_DiskDrive", "Manufacturer") & identifier("Win32_DiskDrive", "Signature") & identifier("Win32_DiskDrive", "TotalHeads")
    End Function
    'Motherboard ID
    Private Shared Function baseId() As String
        Return identifier("Win32_BaseBoard", "Model") & identifier("Win32_BaseBoard", "Manufacturer") & identifier("Win32_BaseBoard", "Name") & identifier("Win32_BaseBoard", "SerialNumber")
    End Function
    'Primary video controller ID
    Private Shared Function videoId() As String
        Return identifier("Win32_VideoController", "DriverVersion") & identifier("Win32_VideoController", "Name")
    End Function
    'First enabled network card ID
    Private Shared Function macId() As String
        Return identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled")
    End Function
#End Region
End Class
