using AppKit;
using Foundation;
using SegCtrl.macOS;
using Test.SegmentedControl;

namespace Test.SegCtrl.macOS
{
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.MacOS.FormsApplicationDelegate
    {
        NSWindow _window;
        public AppDelegate()
        {
            var style = NSWindowStyle.Closable | NSWindowStyle.Resizable | NSWindowStyle.Titled;

            var rect = new CoreGraphics.CGRect(200, 200, 800, 600);
            _window = new NSWindow(rect, style, NSBackingStore.Buffered, false);
            _window.Title = "SegmentedControl Test on macOS";
        }

        public override NSWindow MainWindow => _window;

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application

            global::Xamarin.Forms.Forms.Init();

            SegmentedControlRenderer.Initialize();

            LoadApplication(new App());
            base.DidFinishLaunching(notification);
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }
    }
}
