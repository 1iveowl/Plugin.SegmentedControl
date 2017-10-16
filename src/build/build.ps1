#################################################
## IMPORTANT: Using MS Build Tool for VS 2017! ##
## This project include C# 7.0 features        ##
#################################################

param([string]$version)

if ([string]::IsNullOrEmpty($version)) {$version = "0.0.1"}

$msbuild = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
&$msbuild ..\main\SegCtlr.Netstandard\SegCtlr.Netstandard.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.Droid\SegCtrl.Droid.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.UWP\SegCtrl.UWP.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.iOS\SegCtrl.iOS.csproj /t:Build /p:Configuration="Release"


Remove-Item .\NuGet -Force -Recurse
New-Item -ItemType Directory -Force -Path .\NuGet
NuGet.exe pack Plugin.SegmentedControl.Netstandard.nuspec -Verbosity detailed -Symbols -OutputDir "NuGet" -Version $version