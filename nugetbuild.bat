cd xpf.Testing
nuget.exe pack xpf.testing.csproj -Prop Configuration=Release 
xcopy *.nupkg "C:\NuGet" /F /Y
rem xcopy *.nupkg "D:\dev\NuGet" /F /Y
cd ..
