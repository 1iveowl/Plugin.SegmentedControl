using System;
using System.Linq;
using Xamarin.Forms;
using AppKit;
using Xamarin.Forms.Platform.MacOS;
using Plugin.Segmented.Control;
using System.ComponentModel;
using Plugin.Segmented;
using Plugin.Segmented.Event;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegCtrl.macOS.SegmentedControlRenderer))]
namespace SegCtrl.macOS
{
    [Preserve(AllMembers = true)]
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, NSSegmentedControl>
    {
        private NSSegmentedControl _nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (Control is null && Element != null)
            {
                CreateNativeSegmentedControl();
            }

            if (!(e.OldElement is null))
            {
                // Element is already e.NewElement here; detach from the element being replaced.
                RemoveElementHandlers(e.OldElement);
            }

            AddElementHandlers(e.NewElement);
        }

        private void CreateNativeSegmentedControl()
        {
            var titles = Element.Children.Select(s => s.Text);
            _nativeControl = NSSegmentedControl.FromLabels(titles.ToArray(), NSSegmentSwitchTracking.SelectOne, OnNativeSegmentChanged);
            _nativeControl.Enabled = Element.IsEnabled;

            if (Element.SelectedSegment >= 0 && Element.SelectedSegment < _nativeControl.SegmentCount)
            {
                _nativeControl.SetSelected(true, Element.SelectedSegment);
            }
            _nativeControl.FocusRingType = NSFocusRingType.None;

            foreach (var (segmentedCtrlOption, i) in Element.Children.Select((segmentCtrlOption, i) => (segmentCtrlOption, i)))
            {
                if (segmentedCtrlOption.WidthRequest > 0)
                    _nativeControl.SetWidth((nfloat)segmentedCtrlOption.WidthRequest, i);
            }

            SetFont();
            SetNativeControl(_nativeControl);
        }

        private void OnNativeSegmentChanged()
        {
            if (!(_nativeControl is null) && !(Element is null))
            {
                Element.SelectedSegment = (int)_nativeControl.SelectedSegment;
            }
        }

        private void ResetNativeControl()
        {
            // Rebuild whenever the segments change, including from an empty control.
            if (!(_nativeControl is null) && !(Element is null))
            {
                _nativeControl.RemoveFromSuperview();
                _nativeControl.Dispose();
                _nativeControl = null;
                CreateNativeSegmentedControl();
            }
        }

        private void AddElementHandlers(SegmentedControl element, bool addChildHandlersOnly = false)
        {
            if (!(element is null))
            {
                if (!addChildHandlersOnly)
                {
                    element.OnElementChildrenChanging += OnElementChildrenChanging;
                }
                if (!(element.Children is null))
                {
                    foreach (var child in element.Children)
                    {
                        child.PropertyChanged += SegmentPropertyChanged;
                    }
                }
            }
        }

        private void RemoveElementHandlers(SegmentedControl element, bool removeChildrenHandlersOnly = false)
        {
            if (!(element is null))
            {
                if (!removeChildrenHandlersOnly)
                {
                    element.OnElementChildrenChanging -= OnElementChildrenChanging;
                }

                if (!(element.Children is null))
                {
                    foreach (var child in element.Children)
                    {
                        child.PropertyChanged -= SegmentPropertyChanged;
                    }
                }
            }
        }

        private void OnElementChildrenChanging(object sender, ElementChildrenChanging e)
        {
            // Raised before Children is replaced or after an observable Children list was edited, so detach from
            // the segments the event reports as old, not from Element.Children.
            if (e?.OldValues is null)
            {
                return;
            }

            foreach (var child in e.OldValues)
            {
                child.PropertyChanged -= SegmentPropertyChanged;
            }
        }

        private void SegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(_nativeControl is null) && !(Element is null) && sender is SegmentedControlOption option)
            {
                var index = Element.Children.IndexOf(option);
                if (index < 0)
                {
                    return;
                }

                switch (e.PropertyName)
                {
                    case nameof(SegmentedControlOption.Text):
                        _nativeControl.SetLabel(option.Text, index);
                        break;

                    case nameof(SegmentedControlOption.IsEnabled):
                        _nativeControl.SetEnabled(option.IsEnabled, index);
                        break;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (_nativeControl is null || Element is null)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(SegmentedControl.SelectedSegment):
                    // SegmentedControl raises OnSegmentSelected and the command itself.
                    _nativeControl.SelectedSegment = Element.SelectedSegment;
                    break;

                case nameof(SegmentedControl.IsEnabled):
                    _nativeControl.Enabled = Element.IsEnabled;
                    break;

                case nameof(SegmentedControl.Children):
                    ResetNativeControl();
                    // The old segments were detached in OnElementChildrenChanging; attach to the new ones.
                    AddElementHandlers(Element, true);
                    break;

                case nameof(SegmentedControl.FontSize):
                case nameof(SegmentedControl.FontFamily):
                    SetFont();
                    break;
            }
        }

        private void SetFont()
        {
            var font = NSFont.FromFontName(string.IsNullOrEmpty(Element.FontFamily) 
                ? _nativeControl.Font.FontName 
                : Element.FontFamily, (nfloat)Element.FontSize);
            _nativeControl.Font = font;
        }

        protected override void Dispose(bool disposing)
        {
            RemoveElementHandlers(Element);

            base.Dispose(disposing);
            _nativeControl = null;
        }

        public static void Initialize()
        {

        }
    }
}
