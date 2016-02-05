cd "C:\Program Files (x86)\Microsoft\ILMerge"

ILMerge.exe /target:winexe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:"C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage\vimage.exe" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\Release\vimage.exe" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\sfmlnet-graphics-2.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\sfmlnet-window-2.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\Tao.OpenGl.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\gma.Drawing.ImageInfo.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\DevILNet.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\CrashReporter.NET.dll"

ILMerge.exe /target:winexe /targetplatform:"v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5" /out:"C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage\vimage_settings.exe" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage_settings\bin\Release\vimage_settings.exe" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\sfmlnet-window-2.dll"

copy "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\csfml-graphics-2.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage"

copy "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\csfml-window-2.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage"

copy "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\DevIL.dll" "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage"

del "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage\vimage.pdb"
del "C:\Users\Corey\Documents\Projects\Visual Studio\vimage\vimage\bin\vimage\vimage_settings.pdb"

pause
