# Plugin Segmented Control for Xamarin Forms and .NET Standard

[![NuGet Badge](https://buildstats.info/nuget/Plugin.SegmentedControl.Netstandard)](https://www.nuget.org/packages/Plugin.SegmentedControl.Netstandard/)

*Please star this project if you find it useful. Thank you!*

## Why this library?
There are other Segmented Control libraries out there. This library adds two important capabilities:
- It works across all four key platforms: iOS, Android and UWP - all other libraries I've encounted lack UWP and/or MacOS.
- It's based on .NET Standard 2.0

Furthermore, this library is has more flexibility and features than other libraries that I'm aware of. 

Enjoy! And please don't forget to star this project if you find it useful and/or provide feedback if you run into issues or shortcomings.

## Supported platforms
|Platform|Supported|Version|Renderer|
| ------------------- | :-----------: | :-----------: | :------------------: |
|Xamarin.iOS Unified|Yes|iOS 8.1+|UISegmentedControl|
|Xamarin.Android|Yes|API 26+|RadioGroup|
|Xamarin.UWP|Yes|Win10 16299+|User Control/RadioButton|
|Xamarin.MacOS|Yes|10.0+|NSSegmentedControl|

## Features

- Bindable Tint color
- Bindable Select color
- Bindable Disabled color
- Bindable Font size
- Bindable Font Family
- Bindable Item Text
- Bindable Selected Item
- Bindable ICommand
- Bindable IsEnabled Item
- Bindable ItemsSource

For more details please see below or for even more details see: [Test/Demo App](https://github.com/1iveowl/Plugin.SegmentedControl/tree/master/src/test/Test.SegCtrl.netstandard)

## How to use
Using this plugin is easy. 

### iOS
Add initializer to `AppDelegate`

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    global::Xamarin.Forms.Forms.Init();

    Plugin.Segmented.Control.iOS.SegmentedControlRenderer.Initialize();
    ...
}
```

### UWP

You need to add the assembly to App.xaml.cs in you project. For more details see the Xamarin documentation [here](https://developer.xamarin.com/guides/xamarin-forms/platform-features/windows/installation/universal/#Troubleshooting).

```csharp
var assembliesToInclude = new List<Assembly> {typeof(Plugin.Segmented.Control.UWP.SegmentedControlRenderer).GetTypeInfo().Assembly};

Xamarin.Forms.Forms.Init(e, assembliesToInclude);
```

### Android
No special needs.

For using custom fonts with Android see this blog post: [https://blog.verslu.is/xamarin/xamarin-forms-xamarin/custom-fonts-with-xamarin-forms-revisited/](https://blog.verslu.is/xamarin/xamarin-forms-xamarin/custom-fonts-with-xamarin-forms-revisited/)

#### .NET Standard
The Xamarin Forms must use .NET Standard. I suggest using .NET Standard 2.0+. 

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
You can bind to the SegmentSelectedCommand for notification in your view model when a segment change has occurred.
```xml
<control:SegmentedControl
    SegmentSelectedCommand="{Binding SegmentChangedCommand}"
</control:SegmentedControl>   
```

## Credits
For inspiration and for the Android and iOS part I'd like to thank Alex Rainman for his great work on [SegmentedControl.FormsPlugin](https://www.nuget.org/packages/SegmentedControl.FormsPlugin/).

Thank you to [rjantz3](https://github.com/rjantz3) for adding much requested features and enhancements to this control library.
