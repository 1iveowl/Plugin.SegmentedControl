# Plugin Segmented Control for Xamarin Forms and .NET Standard

[![NuGet Badge](https://buildstats.info/nuget/Plugin.SegmentedControl.Netstandard)](https://www.nuget.org/packages/Plugin.SegmentedControl.Netstandard/)

*Please star this project if you find it useful. Thank you!*

## Why
There are other Segmented Control libraries out there. This library adds two important capabilities:
- It works across all three key platforms: iOS, Android and UWP - all other libraries I've encounted lack UWP.
- It's based on .NET Standard

## Supported platforms
|Platform|Supported|Version|Renderer|
| ------------------- | :-----------: | :-----------: | :------------------: |
|Xamarin.iOS Unified|Yes|iOS 8.1+|UISegmentedControl|
|Xamarin.Android|Yes|API 18+|RadioGroup|
|Xamarin.UWP|Yes|Win10 16299+|User Control/RadioButton|

For previous versions of UWP please use version 1.1.5.

## How to used
Using this plugin is easy. 

### iOS
Add initializer to `AppDelegate`

```csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    global::Xamarin.Forms.Forms.Init();

    SegmentedControlRenderer.Initialize();
    ...
}
```

### UWP

You need to add the assembly to App.xaml.cs in you project. For more details see the Xamarin documentation [here](https://developer.xamarin.com/guides/xamarin-forms/platform-features/windows/installation/universal/#Troubleshooting).

```csharp
var assembliesToInclude = new List<Assembly> {typeof(SegmentedControlRenderer).GetTypeInfo().Assembly};

Xamarin.Forms.Forms.Init(e, assembliesToInclude);
```

### Android
No special needs.

#### .NET Standard
The Xamarin Forms must use .NET Standard. I suggest using .NET Standard 1.4. 

Here is a great blog post about how to move your PCL to .NET Standard: [Building Xamarin.Forms Apps with .NET Standard](https://blog.xamarin.com/building-xamarin-forms-apps-net-standard/)

#### XAML
![Plugin Segmented Control Picture](https://github.com/1iveowl/Plugin.SegmentedControl/blob/master/src/asset/SegmentedRadioButtonControl-1.png "Plugin Segmented Control")

`xmlns:control="clr-namespace:Plugin.Segmented.Control;assembly=Plugin.Segmented"`

```xml
<control:SegmentedControl x:Name="SegmentedControl" 
                            SelectedSegment="{Binding SegmentSelection}" 
                            OnSegmentSelected="SegmentedControl_OnValueChanged" 
                            TintColor="BlueViolet"
                            SelectedTextColor="White"
                            DisabledColor="Gray"
                            Margin="8,8,8,8">
    <control:SegmentedControl.Children>
        <control:SegmentedControlOption Text="Item 1"/>
        <control:SegmentedControlOption Text="Item 2"/>
        <control:SegmentedControlOption Text="Item 3"/>
        <control:SegmentedControlOption Text="Item 4"/>
    </control:SegmentedControl.Children>
</control:SegmentedControl>

```


## Credits
For inspiration and for the Android and iOS part I'd like to thank Alex Rainman for his great work on [SegmentedControl.FormsPlugin](https://www.nuget.org/packages/SegmentedControl.FormsPlugin/).
