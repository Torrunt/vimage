cd "C:\Program Files (x86)\Microsoft\ILMerge"

ILMerge.exe /target:winexe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:"%~dp0vimage\bin\vimage\vimage.exe" "%~dp0vimage\bin\Release\vimage.exe" "%~dp0vimage\sfmlnet-graphics-2.dll" "%~dp0vimage\sfmlnet-window-2.dll" "%~dp0vimage\sfmlnet-system-2.dll" "%~dp0vimage\OpenTK.dll" "%~dp0vimage\DevILNet.dll" "%~dp0vimage\CrashReporter.NET.dll" "%~dp0vimage\ExifLib.dll"

ILMerge.exe /target:winexe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:"%~dp0vimage\bin\vimage\vimage_settings.exe" "%~dp0vimage_settings\bin\Release\vimage_settings.exe" "%~dp0vimage\sfmlnet-window-2.dll"

copy "%~dp0vimage\csfml-graphics-2.dll" "%~dp0vimage\bin\vimage"

copy "%~dp0vimage\csfml-window-2.dll" "%~dp0vimage\bin\vimage"

copy "%~dp0vimage\csfml-system-2.dll" "%~dp0vimage\bin\vimage"

copy "%~dp0vimage\DevIL.dll" "%~dp0vimage\bin\vimage"

del "%~dp0vimage\bin\vimage\vimage.pdb"
del "%~dp0vimage\bin\vimage\vimage_settings.pdb"
