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

            if (Control is null)
            {
                CreateNativeSegmentedControl();
            }

            if (!(e.OldElement is null))
            {
                RemoveElementHandlers();
            }

            AddElementHandlers(e.NewElement);
        }

        private void CreateNativeSegmentedControl()
        {
            var titles = Element.Children.Select(s => s.Text);
            _nativeControl = NSSegmentedControl.FromLabels(titles.ToArray(), NSSegmentSwitchTracking.SelectOne, OnNativeSegmentChanged);
            _nativeControl.Enabled = Element.IsEnabled;
            _nativeControl.SetSelected(true, Element.SelectedSegment);
            _nativeControl.FocusRingType = NSFocusRingType.None;

            SetNativeControl(_nativeControl);
        }

        private void OnNativeSegmentChanged()
        {
            if (!(_nativeControl is null))
            {
                Element.SelectedSegment = (int)_nativeControl.SelectedSegment;
            }
        }

        private void ResetNativeControl()
        {
            if (!(_nativeControl is null) && !(Element is null))
            {
                if (_nativeControl.SegmentCount > 0)
                {
                    _nativeControl.RemoveFromSuperview();
                    _nativeControl.Dispose();
                    _nativeControl = null;
                    CreateNativeSegmentedControl();
                }
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

        private void RemoveElementHandlers(bool removeChildrenHandlersOnly = false)
        {
            if (!(Element is null))
            {
                if (!removeChildrenHandlersOnly)
                {
                    Element.OnElementChildrenChanging -= OnElementChildrenChanging;
                }

                if (!(Element.Children is null))
                {
                    foreach (var child in Element.Children)
                    {
                        child.PropertyChanged -= SegmentPropertyChanged;
                    }
                }
            }
        }

        private void OnElementChildrenChanging(object sender, EventArgs e)
        {
            RemoveElementHandlers(true);
        }

        private void SegmentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(_nativeControl is null) && !(Element is null) && sender is SegmentedControlOption option)
            {
                var index = Element.Children.IndexOf(option);

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

            if (e.PropertyName == "Renderer")
            {
                Element?.RaiseSelectionChanged();
                return;
            }

            if (_nativeControl is null || Element is null)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(NSSegmentedControl.SelectedSegment):
                    _nativeControl.SelectedSegment = Element.SelectedSegment;
                    Element.RaiseSelectionChanged();
                    break;
                case nameof(NSSegmentedControl.IsEnabled):
                    _nativeControl.Enabled = Element.IsEnabled;
                    break;
                case nameof(SegmentedControl.Children):
                    ResetNativeControl();
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
