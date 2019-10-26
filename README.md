

# Trove Downloader

This program is to download all the items in the Humble Bundle Trove. The code is a mess, but it works, and is relatively efficient at what it does (using a total of 20MB of RAM during the entire process)

*****
# How it works

* Download (or compile) the files listed below.
* On first run you will have to provide a session key (Details on how to get it below)
* The program will download trove.json from the Humble Bundle API
* it will then ask you what you would like to download. Your choices are Windows, Mac, Linux, or Everything

****
# Installation.

If you have Visual Studio then you can compile it yourself.  
If you don't, the built installer is below.  
* [Windows](https://ltscdn.m6.nz/humble/trove-downloader.exe?version=1.6.0&utm_source=htd-github)

* If you built the application, in `bin\Release` will be the downloader. Run that, and continue on!

****

# Usage

### Compatibility

This application has been tested on the following platforms:

| System                     | Status   | Notes                          |
| -------------------------- | -------- | ------------------------------ |
| Windows 7                  | &#9989;  |                                |
| Windows 8.1                | &#9989;  |                                |
| Windows 10                 | &#9989;  |                                |
| macOS Sierra               | &#9989;  |                                |
| macOS High Sierra          | &#9989;  |                                |
| macOS Mojave               | &#9989;  |                                |
| macOS Catalina             | &#9989;  |                                |
| Debian 9                   | &#10060; | Errors with libcurl.so.4         |
| Debian 10                  | &#10060; | Segmentation fault             |
| Red Hat Enterprise Linux 6 | &#10060; | Errors with libcurl.so.4       |
| Red Hat Enterprise Linux 7 | &#9989;  |                                |
| Red Hat Enterprise Linux 8 | &#9989;  |                                |
| CentOS 6                   | &#10060; | Errors with libcurl.so.4         |
| CentOS 7                   | &#9989;  |                                |
| CentOS 8                   | &#9989;  |                                |
| openSUSE Leap 15           | &#9989;  |                                |
| openSUSE Leap 42           | &#9989;  | Might want to rebuild for this |
| Ubuntu 14.04               | &#9989;  | Must use 1604 build            |
| Ubuntu 16.04               | &#9989;  | Must use 1604 build            |
| Ubuntu 18.04               | &#9989;  |                                |
| Ubuntu 19.04               | &#9989;  |                                |
| Arch Linux                 | &#9989;  |                                |

### Session Key

You will need to get a session key, [Instructions here](https://github.com/talonius/hb-downloader/wiki/Using-Session-Information-From-Windows-For-hb-downloader)

### Arguments

This application takes up to 5 arguments:
* `/v`, `-v`, `--verbose`  - Print verbose status messages. Defaults to false
* `/t`, `-t`, `--token`    - Humble Bundle Session Key
* `/d`, `-d`, `--download` - What to download from Humble's servers. Accepts `Direct`, or `Torrent`
* `/p`, `-p`, `--platform` - What platform to downlod for. Accepts `Windows`, `Linux`, `Mac`, or `All`
* `/s`, `-s`, `--save`     - Where to save to. Defaults to the terminal's current directory

### Actually using it

1. In a terminal (be it PowerShell, iTerm2, or Termite), run `trove`. If you've missed anything, it'll let you know

### FAQ

* What's the difference between this and the GUI one?

> This one is cross platform, and can be used to automate things. 

* It crashed! Help!

> File an issue [here](https://github.com/td512/Humble-Trove-Console-Downloader/issues) and I'll take a look. If you've got the .NET stack trace, please include that in your report
 
****

# Improving this

If you have any ideas on how to improve this, submit a PR, PM me on reddit (/u/td512) or ping me on discord (TheoM#0331)

****

# Contributions

A big thank you to [Advanced Installer](https://www.advancedinstaller.com) for providing a license for their application. Without that, the installation wouldn't quite be so simple
<br><br>
<img src="https://github.com/td512/Humble-Trove-Downloader/blob/master/contrib/contrib_logo_ai.png" height="112" width="200">
