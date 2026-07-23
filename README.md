# Plugin Segmented Control for Xamarin Forms and .NET Standard

[![NuGet](https://img.shields.io/nuget/v/Plugin.SegmentedControl.Netstandard?logo=nuget&label=Plugin.SegmentedControl.Netstandard)](https://www.nuget.org/packages/Plugin.SegmentedControl.Netstandard)
[![Downloads](https://img.shields.io/nuget/dt/Plugin.SegmentedControl.Netstandard?logo=nuget&color=blue)](https://www.nuget.org/packages/Plugin.SegmentedControl.Netstandard)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENCE)

[![.NET Standard](https://img.shields.io/badge/.NET%20Standard-2.0-5C2D91?logo=dotnet&logoColor=white)](https://learn.microsoft.com/dotnet/standard/net-standard)

> **Warning**
> This library is no longer maintained. There are no plans to migrate it to .NET MAUI or extend it with new features. It remains available for existing Xamarin.Forms applications.

A legacy Xamarin.Forms segmented-control plugin for iOS, Android, UWP, and macOS. The shared library targets .NET Standard 2.0.

*Please star this project if you find it useful. Thank you!*

## Release notes

### Version 5.2+

Version 5.2 and later use AndroidX on Android.

### Version 5.4.5+ (PR #101)

`ItemsSource` is now an `IEnumerable` rather than an `IList<string>`, so it can contain full objects. When it does, the text displayed in each segment is read from the property named by `TextPropertyName`.

- `TextPropertyName`: the name of the property on each `ItemsSource` item to display.
- Without `TextPropertyName`, `ItemsSource` must contain strings.
- `SelectedItem`: the currently selected object from `ItemsSource` (`Items[SelectedSegment]`).

Fixed an Android layout issue that added extra padding below the control because `match_parent` was used incorrectly.

## Overview

The plugin provides a bindable segmented control for Xamarin.Forms applications. At the time of its last release, it supported iOS, Android, UWP, and partial macOS support, with a .NET Standard 2.0 shared library.

The platform support listed below reflects the final release and is not a statement of current compatibility.

## Supported platforms
|Platform|Supported|Version|Renderer|
| ------------------- | :-----------: | :-----------: | :------------------: |
|Xamarin.iOS Unified|Yes|iOS 8.1+|UISegmentedControl|
|Xamarin.Android|Yes|API 21+|RadioGroup|
|Xamarin.UWP|Yes|Win10 16299+|User Control/RadioButton|
|Xamarin.MacOS|Partial|10.0+|NSSegmentedControl|

## Features

- Bindable Tint color
- Bindable Select color
- Bindable Text color
- Bindable Disabled color
- Bindable Font size
- Bindable Font Family
- Bindable Item Text
- Bindable Selected Item
- Bindable ICommand
- Bindable IsEnabled Item
- Bindable ItemsSource
- Bindable Border color (Android & iOS only)
- Bindable Border width (Android & iOS only)

For additional examples, see the [test/demo app](https://github.com/1iveowl/Plugin.SegmentedControl/tree/master/src/test/Test.SegCtrl.netstandard).

## Usage

### iOS

Register the renderer in `AppDelegate`:

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    global::Xamarin.Forms.Forms.Init();

    Plugin.Segmented.Control.iOS.SegmentedControlRenderer.Initialize();
    ...
}
```

### UWP

Add the assembly in your project's `App.xaml.cs`. For more detail, see the Xamarin documentation [here](https://developer.xamarin.com/guides/xamarin-forms/platform-features/windows/installation/universal/#Troubleshooting).

```csharp
var assembliesToInclude = new List<Assembly> {typeof(Plugin.Segmented.Control.UWP.SegmentedControlRenderer).GetTypeInfo().Assembly};

Xamarin.Forms.Forms.Init(e, assembliesToInclude);
```

### Android

No additional setup is required.

For custom fonts on Android, see this blog post: [https://blog.verslu.is/xamarin/xamarin-forms-xamarin/custom-fonts-with-xamarin-forms-revisited/](https://blog.verslu.is/xamarin/xamarin-forms-xamarin/custom-fonts-with-xamarin-forms-revisited/)

#### .NET Standard

The Xamarin.Forms project must target .NET Standard 2.0 or later.

Here is a great blog post about how to move your PCL to .NET Standard: [Building Xamarin.Forms Apps with .NET Standard](https://blog.xamarin.com/building-xamarin-forms-apps-net-standard/)

#### XAML
![Plugin Segmented Control Picture](https://github.com/1iveowl/Plugin.SegmentedControl/blob/master/src/asset/SegmentedRadioButtonControl-1.png "Plugin Segmented Control")


```xml
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:Test.SegmentedControl"
             xmlns:control="clr-namespace:Plugin.Segmented.Control;assembly=Plugin.Segmented"
             x:Class="Test.SegmentedControl.MainPage">

    <ContentPage.Resources>
        <OnPlatform x:Key="PlatformFontName" x:TypeArguments="x:String">
            <On Platform="UWP" Value="Courier New"></On>
            <On Platform="Android" Value="Serif"></On>
            <On Platform="iOS" Value="Helvetica"></On>
            <On Platform="macOS" Value="Baskerville"></On>
        </OnPlatform>
    </ContentPage.Resources>
    
    <ContentPage.Content>
        <StackLayout BackgroundColor="White" x:Name="SegmentWithStack">
            <Label 
                Text="Welcome to Xamarin.Forms!"
                HorizontalOptions="CenterAndExpand" />
            <control:SegmentedControl 
                x:Name="SegmentedControl" 
                SelectedSegment="{Binding SelectedSegment, Mode=TwoWay}"
                TintColor="BlueViolet"
                SelectedTextColor="White"
                DisabledColor="Gray"
                BorderColor="Black"
                BorderWidth="2.0"
                FontSize="Small"
                FontFamily="{StaticResource PlatformFontName}"
                Margin="8,8,8,8"
                SegmentSelectedCommand="{Binding SegmentChangedCommand}"
                OnElementChildrenChanging="OnElementChildrenChanging"
                ItemsSource="{Binding SegmentStringSource}">
                <!--<control:SegmentedControl.Children>
                    <control:SegmentedControlOption Text="{Binding ChangeText}"/>
                    <control:SegmentedControlOption Text="Item 2"/>
                    <control:SegmentedControlOption Text="Item 3"/>
                    <control:SegmentedControlOption Text="Item 4"/>
                </control:SegmentedControl.Children>-->
            </control:SegmentedControl>
        </StackLayout>
    </ContentPage.Content>
</ContentPage>

```
Bind `SegmentSelectedCommand` to notify your view model when the selected segment changes.
```xml
<control:SegmentedControl
    SegmentSelectedCommand="{Binding SegmentChangedCommand}"
</control:SegmentedControl>   
```

## Credits
For inspiration and for the Android and iOS part I'd like to thank Alex Rainman for his great work on [SegmentedControl.FormsPlugin](https://www.nuget.org/packages/SegmentedControl.FormsPlugin/).

Thank you to [rjantz3](https://github.com/rjantz3) for adding much requested features and enhancements.
Thank you to [Thomas Kälin](https://github.com/thomaskaelin) for critical Android and iOS fixes and improvements.
