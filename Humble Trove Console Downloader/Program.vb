Imports System
Imports System.Net
Imports System.IO
Imports System.Threading
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Runtime.InteropServices

Module Program

    Public baseuri = "https://www.humblebundle.com/api/v1/trove/chunk?index="
    Public counter = 0
    Public download = ""

    Public win_downloads = New List(Of String)
    Public mac_downloads = New List(Of String)
    Public lin_downloads = New List(Of String)
    Public win_links = New Integer
    Public mac_links = New Integer
    Public lin_links = New Integer
    Public all_links = New Integer
    Public downloaded = New Integer
    Public downloads_to_complete = New Integer
    Public verbose = False
    Public platform = "Windows"
    Public download_type = "Direct"
    Public session_key = ""
    Public save_to = Environment.CurrentDirectory
    Public time = New Integer
    Public timethread As New Thread(AddressOf ThreadedTimer)
    Public abort_thread = False
    Public seperator = ""

    Sub Main(args As String())
        If args.Length = 0 Then
            HelpFile()
            Environment.Exit(1)
        End If

        verbose = GetArg("v", "verbose")
        If TypeOf GetArg("t", "token") Is Boolean Then
            HelpFile()
            Console.WriteLine("")
            Console.WriteLine("Missing token!")
            Environment.Exit(1)
        Else
            session_key = GetArg("t", "token")
        End If
        If TypeOf GetArg("d", "download") Is Boolean Then
            ' Don't do anything
        Else
            download_type = SentenceCase(GetArg("d", "download"))
        End If
        If TypeOf GetArg("p", "platform") Is Boolean Then
            ' Don't do anything
        Else
            platform = SentenceCase(GetArg("p", "platform"))
        End If
        If TypeOf GetArg("s", "save") Is Boolean Then
            ' Don't do anything
        Else
            save_to = GetArg("s", "save")
        End If

        If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            seperator = "\"
        Else
            seperator = "/"
        End If

        Console.WriteLine($"Download: {download_type}")
        Console.WriteLine($"Platform: {platform}")
        Console.WriteLine($"Saving To: {save_to}")
        Console.WriteLine($"Slash: {seperator}")

        Console.WriteLine("Fetching chunks, please wait...")
        timethread.Start()
        Do
            Dim result = GetStringURL(baseuri + counter.ToString)
            If result = "[]" Then
                Exit Do
            End If
            If verbose = True Then
                Console.WriteLine($"Fetched Chunk #{counter}")
                Console.WriteLine($"Processing JSON for chunk #{counter}")
            End If
            Dim trove = JArray.Parse(result)
            For Each Row In trove
                If Row("downloads")("windows") IsNot Nothing Then
                    win_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?filename={Row("downloads")("windows")("url")("web")}&machine_name={Row("downloads")("windows")("machine_name")}")
                End If
                If Row("downloads")("mac") IsNot Nothing Then
                    mac_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?filename={Row("downloads")("mac")("url")("web")}&machine_name={Row("downloads")("mac")("machine_name")}")
                End If
                If Row("downloads")("linux") IsNot Nothing Then
                    lin_downloads.Add($"https://www.humblebundle.com/api/v1/user/download/sign?filename={Row("downloads")("linux")("url")("web")}&machine_name={Row("downloads")("linux")("machine_name")}")
                End If
            Next
            counter += 1
        Loop

        win_links = win_downloads.Count
        mac_links = mac_downloads.Count
        lin_links = lin_downloads.Count
        all_links = win_links + mac_links + lin_links
        If verbose = True Then
            Console.WriteLine($"Windows: {win_links} available downloads")
            Console.WriteLine($"Mac: {mac_links} available downloads")
            Console.WriteLine($"Linux: {lin_links} available downloads")
            Console.WriteLine($"Total: {all_links} available downloads")
            Console.WriteLine("")
        End If

        Select Case platform
            Case "Windows"
                downloads_to_complete = win_links
                If verbose = True Then
                    Console.WriteLine($"There are {win_links} files for me to download")
                    Console.WriteLine($"Creating directory {save_to}{seperator}Windows")
                End If
                Directory.CreateDirectory(save_to & "\Windows")
                For Each link In win_downloads
                    downloaded += 1
                    Dim qs = New Uri(link).Query
                    Dim qd = Web.HttpUtility.ParseQueryString(qs)
                    Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                    Dim downloads = JObject.Parse(download_urls)
                    Dim this_dl = ""
                    If download_type = "Direct" Then
                        this_dl = downloads("signed_url")
                    Else
                        this_dl = downloads("signed_torrent_url")
                    End If
                    If this_dl Is Nothing Then
                        Continue For
                    End If
                    Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                    Dim this_filepath = $"{save_to}{seperator}Windows\{this_filename}"
                    GetFileURL(this_dl, this_filepath).Wait()
                    Console.SetCursorPosition(0, Console.CursorTop - 1)
                    Console.WriteLine($"I've downloaded {downloaded} / {win_links} files, {win_links - downloaded} files left")
                Next
                abort_thread = True
                Dim iSpan = TimeSpan.FromSeconds(time)
                Dim human_time = iSpan.Minutes.ToString.PadLeft(1, "0"c) & ":" &
                                 iSpan.Seconds.ToString.PadLeft(2, "0"c)
                Console.WriteLine($"Downloads completed in {human_time}")
            Case "Mac"
                downloads_to_complete = mac_links
                If verbose = True Then
                    Console.WriteLine($"There are {mac_links} files for me to download")
                    Console.WriteLine($"Creating directory {save_to}{seperator}Mac")
                End If
                Directory.CreateDirectory(save_to & "\Mac")
                For Each link In mac_downloads
                    downloaded += 1
                    Dim qs = New Uri(link).Query
                    Dim qd = Web.HttpUtility.ParseQueryString(qs)
                    Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                    Dim downloads = JObject.Parse(download_urls)
                    Dim this_dl = ""
                    If download_type = "Direct" Then
                        this_dl = downloads("signed_url")
                    Else
                        this_dl = downloads("signed_torrent_url")
                    End If
                    If this_dl Is Nothing Then
                        Continue For
                    End If
                    Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                    Dim this_filepath = $"{save_to}{seperator}Mac\{this_filename}"
                    GetFileURL(this_dl, this_filepath).Wait()
                    Console.SetCursorPosition(0, Console.CursorTop - 1)
                    Console.WriteLine($"I've downloaded {downloaded} / {mac_links} files, {mac_links - downloaded} files left")
                Next
                abort_thread = True
                Dim iSpan = TimeSpan.FromSeconds(time)
                Dim human_time = iSpan.Minutes.ToString.PadLeft(1, "0"c) & ":" &
                                 iSpan.Seconds.ToString.PadLeft(2, "0"c)
                Console.WriteLine($"Downloads completed in {human_time}")
            Case "Linux"
                downloads_to_complete = lin_links
                If verbose = True Then
                    Console.WriteLine($"There are {lin_links} files for me to download")
                    Console.WriteLine($"Creating directory {save_to}{seperator}Linux")
                End If
                Directory.CreateDirectory(save_to & "\Linux")
                For Each link In lin_downloads
                    downloaded += 1
                    Dim qs = New Uri(link).Query
                    Dim qd = Web.HttpUtility.ParseQueryString(qs)
                    Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                    Dim downloads = JObject.Parse(download_urls)
                    Dim this_dl = ""
                    If download_type = "Direct" Then
                        this_dl = downloads("signed_url")
                    Else
                        this_dl = downloads("signed_torrent_url")
                    End If
                    If this_dl Is Nothing Then
                        Continue For
                    End If
                    Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                    Dim this_filepath = $"{save_to}{seperator}Linux\{this_filename}"
                    GetFileURL(this_dl, this_filepath).Wait()
                    Console.SetCursorPosition(0, Console.CursorTop - 1)
                    Console.WriteLine($"I've downloaded {downloaded} / {lin_links} files, {lin_links - downloaded} files left")
                Next
                abort_thread = True
                Dim iSpan = TimeSpan.FromSeconds(time)
                Dim human_time = iSpan.Minutes.ToString.PadLeft(1, "0"c) & ":" &
                                 iSpan.Seconds.ToString.PadLeft(2, "0"c)
                Console.WriteLine($"Downloads completed in {human_time}")
            Case "All"
                downloads_to_complete = all_links
                If verbose = True Then
                    Console.WriteLine($"There are {all_links} files for me to download")
                    Console.WriteLine($"Creating directory {save_to}{seperator}Windows")
                    Console.WriteLine($"Creating directory {save_to}{seperator}Mac")
                    Console.WriteLine($"Creating directory {save_to}{seperator}Linux")
                End If
                Directory.CreateDirectory(save_to & "\Windows")
                Directory.CreateDirectory(save_to & "\Mac")
                Directory.CreateDirectory(save_to & "\Linux")
                For Each link In win_downloads
                    downloaded += 1
                    Dim qs = New Uri(link).Query
                    Dim qd = Web.HttpUtility.ParseQueryString(qs)
                    Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                    Dim downloads = JObject.Parse(download_urls)
                    Dim this_dl = ""
                    If download_type = "Direct" Then
                        this_dl = downloads("signed_url")
                    Else
                        this_dl = downloads("signed_torrent_url")
                    End If
                    If this_dl Is Nothing Then
                        Continue For
                    End If
                    Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                    Dim this_filepath = $"{save_to}{seperator}Windows\{this_filename}"
                    GetFileURL(this_dl, this_filepath).Wait()
                    Console.SetCursorPosition(0, Console.CursorTop - 1)
                    Console.WriteLine($"I've downloaded {downloaded} / {all_links} files, {all_links - downloaded} files left")
                Next
                For Each link In mac_downloads
                    downloaded += 1
                    Dim qs = New Uri(link).Query
                    Dim qd = Web.HttpUtility.ParseQueryString(qs)
                    Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                    Dim downloads = JObject.Parse(download_urls)
                    Dim this_dl = ""
                    If download_type = "Direct" Then
                        this_dl = downloads("signed_url")
                    Else
                        this_dl = downloads("signed_torrent_url")
                    End If
                    If this_dl Is Nothing Then
                        Continue For
                    End If
                    Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                    Dim this_filepath = $"{save_to}{seperator}Mac\{this_filename}"
                    GetFileURL(this_dl, this_filepath).Wait()
                    Console.SetCursorPosition(0, Console.CursorTop - 1)
                    Console.WriteLine($"I've downloaded {downloaded} / {all_links} files, {all_links - downloaded} files left")
                Next
                For Each link In lin_downloads
                    downloaded += 1
                    Dim qs = New Uri(link).Query
                    Dim qd = Web.HttpUtility.ParseQueryString(qs)
                    Dim download_urls = PostURL(link, qd("machine_name"), qd("filename"))
                    Dim downloads = JObject.Parse(download_urls)
                    Dim this_dl = ""
                    If download_type = "Direct" Then
                        this_dl = downloads("signed_url")
                    Else
                        this_dl = downloads("signed_torrent_url")
                    End If
                    If this_dl Is Nothing Then
                        Continue For
                    End If
                    Dim this_filename = Path.GetFileName(New Uri(this_dl).LocalPath)
                    Dim this_filepath = $"{save_to}{seperator}Linux\{this_filename}"
                    GetFileURL(this_dl, this_filepath).Wait()
                    Console.SetCursorPosition(0, Console.CursorTop - 1)
                    Console.WriteLine($"I've downloaded {downloaded} / {all_links} files, {all_links - downloaded} files left")
                Next
                abort_thread = True
                Dim iSpan = TimeSpan.FromSeconds(time)
                Dim human_time = iSpan.Minutes.ToString.PadLeft(1, "0"c) & ":" &
                                 iSpan.Seconds.ToString.PadLeft(2, "0"c)
                Console.WriteLine($"Downloads completed in {human_time}")
        End Select

    End Sub

    Sub ThreadedTimer()
        Do
            If abort_thread = True Then
                Exit Do
            End If

            time += 1
            Thread.Sleep(1000)
        Loop
    End Sub

    Function HelpFile()
        Console.WriteLine("Humble Bundle Trove Downloader - Help File")
        Console.WriteLine("")
        Console.WriteLine("Arguments:")
        Console.WriteLine("/v | -v | --verbose   - Verbose output")
        Console.WriteLine("/t | -t | --token     - Humble Bundle Session Key")
        Console.WriteLine("/d | -d | --download  - What to download from Humble's servers - [Direct|Torrent], default Direct")
        Console.WriteLine("/p | -p | --platform  - Platform to download for - [Windows|Mac|Linux|All], default Windows")
        Console.WriteLine("/s | -s | --save      - Where to save downlads. Defaults to current directory")
    End Function
    Function SentenceCase(ByVal val As String) As String
        ' Test for nothing or empty.
        If String.IsNullOrEmpty(val) Then
            Return val
        End If

        ' Convert to character array.
        Dim array() As Char = val.ToLower.ToCharArray

        ' Uppercase first character.
        array(0) = Char.ToUpper(array(0))

        ' Return new string.
        Return New String(array)
    End Function

    Function GetArg(slashParam As String, doubleDash As String)
        Dim args = Environment.GetCommandLineArgs
        Dim arg
        For i As Integer = 0 To args.Length - 1
            If args(i) = $"/{slashParam}" OrElse args(i) = $"-{slashParam}" OrElse args(i) = $"--{doubleDash}" Then
                Try
                    If args(i + 1).Contains("/") OrElse args(i + 1).Contains("-") OrElse args(i + 1).Contains("--") Then
                        arg = True
                    Else
                        arg = args(i + 1)
                    End If
                Catch ex As Exception
                    arg = True
                End Try
            End If
        Next
        If String.IsNullOrEmpty(arg) Then
            arg = False
        End If
        Return arg
    End Function
    Function DeserialiseJson(json As String)
        ' This is specific to Humble Bundle's Trove API
        Return JArray.Parse(json)
    End Function

    Function GetStringURL(url As String)
        Dim client = New WebClient
        Dim result = client.DownloadString(url)
        client.Dispose()
        Return result
    End Function

    Async Function GetFileURL(url As String, filename As String) As Task
        Dim client = New WebClient
        AddHandler client.DownloadProgressChanged, AddressOf DownloadProgressChanged
        Await client.DownloadFileTaskAsync(New Uri(url), filename)
        client.Dispose()
        Console.Write(Environment.NewLine)
        File.SetLastWriteTime(filename, client.ResponseHeaders("Last-Modified"))
    End Function

    Function PostURL(url As String, filename As String, machine_name As String)
        Dim client As New WebClient
        Dim reqparm As New Specialized.NameValueCollection
        reqparm.Add("machine_name", machine_name)
        reqparm.Add("filename", filename)
        client.Headers.Set("cookie", $"_simpleauth_sess={session_key}")
        Dim responsebytes = client.UploadValues(url, "POST", reqparm)
        Dim responsebody = (New Text.UTF8Encoding).GetString(responsebytes)
        client.Dispose()
        Return responsebody
    End Function

    Function DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
        Console.SetCursorPosition(0, Console.CursorTop)
        Console.Write(vbCr & $"{e.ProgressPercentage}%")
    End Function

End Module
