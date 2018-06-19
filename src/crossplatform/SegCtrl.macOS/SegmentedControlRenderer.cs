using System;
using System.Linq;
using Xamarin.Forms;
using AppKit;
using Xamarin.Forms.Platform.MacOS;
using Plugin.Segmented.Control;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegCtrl.macOS.SegmentedControlRenderer))]
namespace SegCtrl.macOS
{
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, NSSegmentedControl>
    {
        private NSSegmentedControl _nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var titles = Element.Children.Select(s => s.Text );
                _nativeControl = NSSegmentedControl.FromLabels(titles.ToArray(), NSSegmentSwitchTracking.SelectOne, OnNativeSegmentChanged);
                _nativeControl.Enabled = Element.IsEnabled;
                _nativeControl.SetSelected(true, Element.SelectedSegment);
                _nativeControl.FocusRingType = NSFocusRingType.None;

                SetNativeControl(_nativeControl);
            }

            if (e.OldElement != null)
                RemoveElementHandlers();
            
            if (e.NewElement != null && Element != null)
            {
                foreach (var child in Element.Children)
                {
                    child.PropertyChanged += SegmentPropertyChanged;
                }
            }
        }

        private void OnNativeSegmentChanged()
        {
            if (_nativeControl != null)
            {
                Element.SelectedSegment = (int)_nativeControl.SelectedSegment;
            }
        }

        private void RemoveElementHandlers()
        {
            if (Element != null)
            {
                foreach (var child in Element.Children)
                {
                    child.PropertyChanged -= SegmentPropertyChanged;
                }
            }
        }

        private void SegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_nativeControl != null && Element != null && sender is SegmentedControlOption option && e.PropertyName == nameof(option.Text))
            {
                
                var index = Element.Children.IndexOf(option);
                _nativeControl.SetLabel(Element.Children[index].Text, index);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Renderer")
            {
                Element?.RaiseSelectionChanged();
                return;
            }

            if (_nativeControl == null || Element == null) return;

            switch (e.PropertyName)
            {
                case nameof(NSSegmentedControl.SelectedSegment):
                    _nativeControl.SelectedSegment = Element.SelectedSegment;
                    Element.RaiseSelectionChanged();
                    break;
                case nameof(NSSegmentedControl.IsEnabled):
                    _nativeControl.Enabled = Element.IsEnabled;
                    break;
                default:
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            RemoveElementHandlers();

            base.Dispose(disposing);
            _nativeControl = null;
        }

        public static void Initialize()
        {

        }
    }
}
