param([string]$betaver)

if ([string]::IsNullOrEmpty($betaver)) {
	$version = [Reflection.AssemblyName]::GetAssemblyName((resolve-path '..\main\SegCtlr.Netstandard\bin\Release\netstandard2.1\Plugin.Segmented.dll')).Version.ToString(3)
	}
else {
		$version = [Reflection.AssemblyName]::GetAssemblyName((resolve-path '..\main\SegCtlr.Netstandard\bin\Release\netstandard2.1\Plugin.Segmented.dll')).Version.ToString(3) + "-" + $betaver
}

.\build.ps1 $version

nuget.exe push -Source "1iveowlNuGetRepo" -ApiKey key ".\NuGet\Plugin.SegmentedControl.Netstandard.$version.symbols.snupkg"
