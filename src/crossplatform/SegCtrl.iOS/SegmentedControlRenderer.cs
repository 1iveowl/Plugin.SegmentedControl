using System;
using System.Collections.Generic;
using System.ComponentModel;
using Plugin.Segmented.Control;
using Plugin.Segmented.Control.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace Plugin.Segmented.Control.iOS
{
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, UISegmentedControl>
    {
        private UISegmentedControl _nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                _nativeControl = new UISegmentedControl();
                SetNativeControlSegments(Element.Children);
                _nativeControl.Enabled = Element.IsEnabled;
                _nativeControl.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.DisabledColor.ToUIColor();
                SetSelectedTextColor();
                SetNativeControl(_nativeControl);
            }

            if (e.OldElement != null)
            {
                if (_nativeControl != null) _nativeControl.ValueChanged -= NativeControl_SelectionChanged;
                RemoveElementHandlers();
            }

            if (e.NewElement != null)
            {
                if (_nativeControl != null) _nativeControl.ValueChanged += NativeControl_SelectionChanged;
                AddElementHandlers(e.NewElement);
            }
        }

        private void SetNativeControlSegments(IList<SegmentedControlOption> children)
        {
            if (_nativeControl != null)
            {
                if (_nativeControl.NumberOfSegments > 0)
                {
                    _nativeControl.RemoveAllSegments();
                }
                for (int i = 0; i < children.Count; i++)
                {
                    _nativeControl.InsertSegment(children[i].Text, i, false);
                }
                if (Element != null)
                {
                    _nativeControl.SelectedSegment = Element.SelectedSegment;
                }
            }
        }

        private void AddElementHandlers(SegmentedControl element, bool addChildHandlersOnly = false)
        {
            if (element != null)
            {
                if (!addChildHandlersOnly)
                {
                    element.OnElementChildrenChanging += OnElementChildrenChanging;
                }
                if (element.Children != null)
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
            if (Element != null)
            {
                if (!removeChildrenHandlersOnly)
                {
                    Element.OnElementChildrenChanging -= OnElementChildrenChanging;
                }
                if (Element.Children != null)
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
            if (_nativeControl != null && Element != null && sender is SegmentedControlOption option)
            {
                var index = Element.Children.IndexOf(option);
                switch (e.PropertyName)
                {
                    case nameof(SegmentedControlOption.Text):
                        _nativeControl.SetTitle(option.Text, index);
                        break;
                    case nameof(SegmentedControlOption.IsEnabled):
                        _nativeControl.SetEnabled(option.IsEnabled, index);
                        break;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
                case nameof(SegmentedControl.SelectedSegment):
                    _nativeControl.SelectedSegment = Element.SelectedSegment;
                    Element.RaiseSelectionChanged();
                    break;
                case nameof(SegmentedControl.TintColor):
                    _nativeControl.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.DisabledColor.ToUIColor();
                    break;
                case nameof(SegmentedControl.IsEnabled):
                    _nativeControl.Enabled = Element.IsEnabled;
                    _nativeControl.TintColor = Element.IsEnabled ? Element.TintColor.ToUIColor() : Element.DisabledColor.ToUIColor();
                    break;
                case nameof(SegmentedControl.SelectedTextColor):
                    SetSelectedTextColor();
                    break;
                case nameof(SegmentedControl.Children):
                    if (Element.Children != null)
                    {
                        SetNativeControlSegments(Element.Children);
                        AddElementHandlers(Element, true);
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetSelectedTextColor()
        {
            var attr = new UITextAttributes {TextColor = Element.SelectedTextColor.ToUIColor()};
            _nativeControl.SetTitleTextAttributes(attr, UIControlState.Selected);
        }

        private void NativeControl_SelectionChanged(object sender, EventArgs e)
        {
            Element.SelectedSegment = (int)_nativeControl.SelectedSegment;
        }

        protected override void Dispose(bool disposing)
        {
            if (_nativeControl != null)
            {
                _nativeControl.ValueChanged -= NativeControl_SelectionChanged;
                _nativeControl?.Dispose();
                _nativeControl = null;
            }
            RemoveElementHandlers();

            base.Dispose(disposing);
        }

        public static void Initialize()
        {

        }
    }
}