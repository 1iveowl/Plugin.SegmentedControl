param([string]$betaver)

if ([string]::IsNullOrEmpty($betaver)) {
	$version = [Reflection.AssemblyName]::GetAssemblyName((resolve-path '..\main\Behavior.Forms.Xamarin.Netstandard20\bin\Release\netstandard2.0\Behavior.Forms.Xamarin.Netstandard20.dll')).Version.ToString(3)
	}
else {
		$version = [Reflection.AssemblyName]::GetAssemblyName((resolve-path '..\main\Behavior.Forms.Xamarin.Netstandard20\bin\Release\netstandard2.0\Behavior.Forms.Xamarin.Netstandard20.dll')).Version.ToString(3) + "-" + $betaver
}

.\build.ps1 $version

c:\tools\nuget\Nuget.exe push ".\NuGet\Behaviors.XamarinForms.$version.symbols.nupkg" -Source https://www.nuget.org