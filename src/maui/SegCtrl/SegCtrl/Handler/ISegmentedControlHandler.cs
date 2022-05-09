//#if __IOS__ || MACCATALYST
//using PlatformView = Microsoft.Maui.Platform.MauiLabel;
//#elif MONOANDROID
//using PlatformView = AndroidX.AppCompat.Widget.RadioGroup;
//#elif WINDOWS
//using PlatformView = Microsoft.UI.Xaml.Controls.RadioButton;
//#elif TIZEN
//using PlatformView = Tizen.UIExtensions.ElmSharp.Label;
//#elif NETSTANDARD || (NET6_0 && !IOS && !TIZEN)
//using PlatformView = System.Object;
//#endif

//namespace SegCtrl
//{
//    public interface ISegmentedControlHandler
//    {
//        new ISegmentedControl VirtualView { get; }
//        new PlatformView PlatformView { get; }
//    }
//}
