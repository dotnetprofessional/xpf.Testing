cd xpf.Testing
nuget.exe pack
xcopy *.nupkg "D:\dev\NuGet" /F /Y
cd ..
