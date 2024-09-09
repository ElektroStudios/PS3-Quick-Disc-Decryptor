<!-- Common Project Tags: 
desktop-app 
desktop-application 
dotnet 
dotnet-core 
netcore 
tool 
tools 
vbnet 
visualstudio 
windows 
windows-app 
windows-application 
windows-applications 
windows-forms 
winforms 
playstation
playstation-3
ps3
playstation3
consolegames
games
videogame
emulators
 -->

# PS3 Quick Disc Decryptor 💿🔑

### User-Friendly GUI to decrypt Redump's PS3 disc images using PS3Dec.

![screenshot](/Images/REDUMP.png)

------------------

## 👋 Introduction

PS3 Quick Disc Decryptor is a - *yet another* - application that allows you to decrypt PS3 disc images (\*.iso files) in a friendly way.

The decrypted PS3 disc images will work with [RPCS3](https://rpcs3.net/) emulator (*if marked as playable in their compatibility list*).

## 👌 Features

 - Simple, user-friendly graphical user-interface.
 - Designed for batch processing.
 - Meticulous status report and error handling.
 - Allows to abort the decryption procedure on demand.
 - Allows to see PS3Dec.exe output and progress in a embeded window.
 - Allows to automatically delete sucessfully decrypted disc images to save disk space.

## 🖼️ Screenshots

![screenshot](/Images/Screenshot_06.png)

![screenshot](/Images/Screenshot_07.png)

## 📝 Requirements

- Microsoft Windows OS with [net 6.0 desktop runtime](https://dotnet.microsoft.com/download/dotnet/6.0).

## 🤖 Getting Started

Download the latest release by clicking [here](https://github.com/ElektroStudios/PS3-Quick-Disc-Decryptor/releases/latest).

Open the program, configure the auto-descriptive program settings, and finally press the button with name 'Start Decryption'.

## 🌐 External resources

### Encrypted PS3 ISOs

To use this program you will need encrypted PS3 disc images (\*.iso files) from the **Redump** group. It will not work with PS3 disc images from **NO-INTRO** or other groups.

1. You can download **Redump**'s encrypted PS3 disc images from one of these links:

    - [Myrient](https://myrient.erista.me/files/Redump/Sony%20-%20PlayStation%203/)
    - [Archive.org](https://archive.org/details/@cvlt_of_mirrors?query=%22Sony+Playstation+3%22+%22Redump.org%22&sort=title)

2. Once you have your encrypted PS3 disc images, put all the \*.iso files together in the same folder, like this:

    ![screenshot](/Images/Screenshot_02.png)

3. Finally, in the program's user interface you just need to select the directory containing the encrypted PS3 ISO files by doing click in the following button:
    
    ![screenshot](/Images/Screenshot_03.png)

    💡 Tip: You can put all the \*.iso files in a folder with name "Encrypted" inside the program directory to skip this step.

    ❗ Note that the program will **not** perform a recursive \*.iso file search.

### Decryption keys

To use this program you will need decryption keys for the **Redump**'s encrypted PS3 ISO files, which are distributed as plain text files that each contain a string of 32 characters long.

1. Download the desired PS3 decryption keys from one of these links:

    - [Archive.org](https://archive.org/download/video_game_keys_and_sbi) (*only need to download the "Disc Keys TXT" zipped archive from here*)
    - [Myrient](https://myrient.erista.me/files/Redump/Sony%20-%20PlayStation%203%20-%20Disc%20Keys%20TXT/)
    - [Aldo's Tools](https://ps3.aldostools.org/dkey.html)
    
2. Once you have your desired decryption keys, put all the \*.dkey files together in the same folder, like this:

    ![screenshot](/Images/Screenshot_01.png)

3. Finally, in the program's user interface you just need to select the directory containing the decryption keys by doing click in the following button: 

    ![screenshot](/Images/Screenshot_04.png)

    💡 Tip: You can put all the \*.dkey files in a folder with name "DKeys" inside the program directory to skip this step.

    ❗ Note that the program will **not** perform a recursive \*.dkey file search.

### PS3Dec.exe

To use this program you will need a copy of **PS3Dec.exe** file, which is actually included in this package, however if you want to use your own:

1. Download **PS3Dec.exe** from one of these links: 

    - [al3xtjames's modified version from RomHacking.net](https://www.romhacking.net/utilities/1456/)
           
      *(✅ This is the one tested and already included in the program package)*

    - [al3xtjames's Github repository](https://github.com/al3xtjames/PS3Dec) or one of its [forks](https://github.com/al3xtjames/PS3Dec/forks).

      (*❗ I have not tested any of those forks nor checked if they are virus free. Use them at your own risk.*)

       ❗  Do **not** try to use **PS3Dec.exe** from [Redrrx's Github repository](https://github.com/Redrrx/ps3dec), since it was rewrote using a different (incompatible) command-line syntax with my program.

2. Once you have your copy of **PS3Dec.exe**, in the program's user interface you just need to select the **PS3Dec.exe** file by doing click in the following button: 

    ![screenshot](/Images/Screenshot_05.png)

    💡 Tip: You can put the **PS3Dec.exe** inside the program directory - *overwriting the included one or making a backup* -  to skip this step.

## 🔄 Change Log

Explore the complete list of changes, bug fixes and improvements across different releases by clicking [here](/Docs/CHANGELOG.md).

## 🏆 Credits

This work relies on the following resources: 

 - [al3xtjames's PS3Dec](https://github.com/al3xtjames/PS3Dec)
 - [Redump Disc Preservation Project's PS3 resources](http://redump.org/discs/system/ps3/)

## ⚠️ Disclaimer:

This Work (the repository and the content provided in) is provided "as is", without warranty of any kind, express or implied, including but not limited to the warranties of merchantability, fitness for a particular purpose and noninfringement. In no event shall the authors or copyright holders be liable for any claim, damages or other liability, whether in an action of contract, tort or otherwise, arising from, out of or in connection with the Work or the use or other dealings in the Work.

This Work has no affiliation, approval or endorsement by the author(s) of the third-party libraries used by this Work.

## 💪 Contributing

Your contribution is highly appreciated!. If you have any ideas, suggestions, or encounter issues, feel free to open an issue by clicking [here](https://github.com/ElektroStudios/PS3-Quick-Disc-Decryptor/issues/new/choose). 

Your input helps make this Work better for everyone. Thank you for your support! 🚀

## 💰 Beyond Contribution 

This work is distributed for educational purposes and without any profit motive. However, if you find value in my efforts and wish to support and motivate my ongoing work, you may consider contributing financially through the following options:

<br></br>
<p align="center"><img src="/Images/github_circle.png" height=100></p>
<p align="center">__________________</p>
<h3 align="center">Becoming my sponsor on Github:</h3>
<p align="center">You can show me your support by clicking <a href="https://github.com/sponsors/ElektroStudios/">here</a>, <br align="center">contributing any amount you prefer, and unlocking rewards!</br></p>
<br></br>

<p align="center"><img src="/Images/paypal_circle.png" height=100></p>
<p align="center">__________________</p>
<h3 align="center">Making a Paypal Donation:</h3>
<p align="center">You can donate to me any amount you like via Paypal by clicking <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=E4RQEV6YF5NZY">here</a>.</p>
<br></br>

<p align="center"><img src="/Images/envato_circle.png" height=100></p>
<p align="center">__________________</p>
<h3 align="center">Purchasing software of mine at Envato's Codecanyon marketplace:</h3>
<p align="center">If you are a .NET developer, you may want to explore '<b>DevCase Class Library for .NET</b>', <br align="center">a huge set of APIs that I have on sale. Check out the product by clicking <a href="https://codecanyon.net/item/elektrokit-class-library-for-net/19260282">here</a></br><br align="center"><i>It also contains all piece of reusable code that you can find across the source code of my open source works.</i></p>
<br></br>

<h2 align="center"><u>Your support means the world to me! Thank you for considering it!</u> 👍</h2>
