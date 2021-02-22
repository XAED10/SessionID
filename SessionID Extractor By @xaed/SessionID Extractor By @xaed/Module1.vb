Imports System.Collections.Specialized
Imports System.IO, System.Net, System.Text, System.Text.RegularExpressions, System.Console
Imports System.Threading
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices
Imports RestSharp
Module Module1
    Dim banner As String = "   _____              _             _____ _____    ____         __   __         ______ _____  
  / ____|            (_)           |_   _|  __ \  |  _ \        \ \ / /   /\   |  ____|  __ \ 
 | (___   ___ ___ ___ _  ___  _ __   | | | |  | | | |_) |_   _   \ V /   /  \  | |__  | |  | |
  \___ \ / _ \ __/ __| |/ _ \| '_ \  | | | |  | | |  _ <| | | |   > <   / /\ \ |  __| | |  | |
  ____) |  __\__ \__ \ | (_) | | | |_| |_| |__| | | |_) | |_| |  / . \ / ____ \| |____| |__| |
 |_____/ \___|___/___/_|\___/|_| |_|_____|_____/  |____/ \__, | /_/ \_\_/    \_\______|_____/ 
                                                          __/ |                               
                                                         |___/                                "
    Dim username, password, sessid As String
    Dim logged_in As Boolean
    Public CookiesContainer2 As CookieContainer = New CookieContainer

    Sub Main()
        Console.ForegroundColor = ConsoleColor.Magenta
        Write(banner)
        Console.ForegroundColor = ConsoleColor.White
        Write(vbNewLine & "[#] Username : ")
        username = ReadLine()
        Write("[*] Password : ")
        password = ReadLine()
        Console.ForegroundColor = ConsoleColor.Blue
        Write("STATUS : ")
        Console.ForegroundColor = ConsoleColor.Blue
        Write("Logging In ")
        API_Login(username, password)
        Console.Clear()
        Console.ForegroundColor = ConsoleColor.Magenta
        Write(banner)
        Console.ForegroundColor = ConsoleColor.Green
        Write(vbNewLine & "Session ID For @" + username + " : " & sessid)
        Try
            My.Computer.FileSystem.WriteAllText("@" + username + " SessionID.txt", "Session ID For @" + username + " : " & sessid + vbNewLine + "Extracted By XAED SESSION ID EXTRACTOR", False)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Cant Save Session ID In .txt File")
        End Try
        Write(vbNewLine & "Press Enter To Exit ...")
        ReadLine()
        Environment.Exit(0)
    End Sub
    Sub API_Login(username As String, password As String)
        Dim guidString As String = Guid.NewGuid().ToString()
        Try
            Dim data As Byte() = Encoding.UTF8.GetBytes($"username={username}&password={password}&device_id={Guid.NewGuid}&login_attempt_count=0")
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create("https://i.instagram.com/api/v1/accounts/login/"), HttpWebRequest)
            With req
                .Method = "POST"
                .CookieContainer = CookiesContainer2
                .Proxy = Nothing
                .UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
                .ContentType = ("application/x-www-form-urlencoded")
            End With
            Dim stream As Stream = req.GetRequestStream : stream.Write(data, 0, data.Length) : stream.Dispose() : stream.Close()
            Dim httpresponse As HttpWebResponse = req.GetResponse : Dim reader As New StreamReader(httpresponse.GetResponseStream) : Dim response As String = DirectCast(reader.ReadToEnd, String) : reader.Dispose() : reader.Close()
            If response.Contains("logged_in_user") Then
                Dim getdata As String = httpresponse.GetResponseHeader("set-cookie")
                sessid = Regex.Match(getdata, "sessionid=(.*?);").Groups(1).Value
            ElseIf response.Contains("challenge_required") Then
                Dim url As String = Regex.Match(response, """api_path"":""(.*?)""").Groups(1).Value
                SendEmail(url)
            Else
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
                ProjectData.EndApp()
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
            End If
        Catch ex As WebException
            Dim exx As String = New IO.StreamReader(ex.Response.GetResponseStream()).ReadToEnd()
            If exx.Contains("Incorrect") Or exx.Contains("bad") Or exx.Contains("Bad") Then
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
                ProjectData.EndApp()
                MsgBox("Bad Username or Password !", MsgBoxStyle.Critical)
            Else
                MsgBox(exx)
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
                ProjectData.EndApp()
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
            End If
        End Try
    End Sub
    Sub SendEmail(url As String)
        Try
            Dim data As Byte() = Encoding.UTF8.GetBytes("choice=1")
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create("https://i.instagram.com/api/v1" & url), HttpWebRequest)
            With req
                .Method = "POST"
                .CookieContainer = CookiesContainer2
                .Proxy = Nothing
                .UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
                .ContentType = ("application/x-www-form-urlencoded")
            End With
            Dim stream As Stream = req.GetRequestStream : stream.Write(data, 0, data.Length) : stream.Dispose() : stream.Close()
            Dim httpresponse As HttpWebResponse = DirectCast(req.GetResponse, HttpWebResponse) : Dim reader As New StreamReader(httpresponse.GetResponseStream) : Dim response As String = reader.ReadToEnd
            If response.Contains("security_code") Then
                Dim code As String
                Write("[+] Enter Code")
                code = ReadLine()
                Security_code(url, code)
            Else
                MsgBox("error on sending the code", MsgBoxStyle.Critical)
                ProjectData.EndApp()
                MsgBox("error on sending the code", MsgBoxStyle.Critical)
            End If
        Catch ex As WebException
            MsgBox("error on sending the code", MsgBoxStyle.Critical)
            ProjectData.EndApp()
            MsgBox("error on sending the code", MsgBoxStyle.Critical)
        End Try
    End Sub

    Sub Security_code(url As String, Codetext As String)
        Try
            Dim data As Byte() = Encoding.UTF8.GetBytes("security_code=" & Codetext)
            Dim req As HttpWebRequest = DirectCast(WebRequest.Create("https://i.instagram.com/api/v1" & url), HttpWebRequest)
            With req
                .Method = "POST"
                .CookieContainer = CookiesContainer2
                .Proxy = Nothing
                .UserAgent = "Instagram 100.1.0.29.135 Android (25/7.1.2; 192dpi; 720x1280; google; G011A; G011A; qcom; en_US; 262886984)"
                .ContentType = ("application/x-www-form-urlencoded")
            End With
            Dim stream As Stream = req.GetRequestStream : stream.Write(data, 0, data.Length) : stream.Dispose() : stream.Close()
            Dim httpresponse As HttpWebResponse = DirectCast(req.GetResponse, HttpWebResponse) : Dim reader As New StreamReader(httpresponse.GetResponseStream) : Dim response As String = reader.ReadToEnd
            If response.Contains("logged_in_user") Then
                Dim getdata As String = httpresponse.GetResponseHeader("set-cookie")
                sessid = Regex.Match(getdata, "sessionid=(.*?);").Groups(1).Value
            Else
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
                ProjectData.EndApp()
                Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
            End If
        Catch ex As WebException
            Dim exresponse As String = New StreamReader(ex.Response.GetResponseStream).ReadToEnd
            Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
            ProjectData.EndApp()
            Interaction.MsgBox("Error on login !", MsgBoxStyle.Critical, Nothing)
        End Try
    End Sub
End Module
