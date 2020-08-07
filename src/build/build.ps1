param([string]$version)

if ([string]::IsNullOrEmpty($version)) {$version = "0.0.1"}

if ($IsMacOS) {
    $msbuild = "msbuild"
} else {
    $vswhere = 'C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe'
    $msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    $msbuild = join-path $msbuild 'MSBuild\Current\Bin\MSBuild.exe'
}

&$msbuild ..\main\SegCtlr.Netstandard\SegCtlr.Netstandard.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.Droid\SegCtrl.Droid.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.UWP\SegCtrl.UWP.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.iOS\SegCtrl.iOS.csproj /t:Build /p:Configuration="Release"
&$msbuild ..\crossplatform\SegCtrl.macOS\SegCtrl.macOS.csproj /t:Build /p:Configuration="Release"

Remove-Item .\NuGet -Force -Recurse
New-Item -ItemType Directory -Force -Path .\NuGet
NuGet.exe pack Plugin.SegmentedControl.Netstandard.nuspec -Verbosity detailed -Symbols -OutputDir "NuGet" -Version $version
#NuGet.exe pack Plugin.SegmentedControl.Netstandard.nuspec -Verbosity detailed -Symbols -SymbolPackageFormat snupkg -OutputDir "NuGet" -Version $version