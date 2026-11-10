using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Plugin.Segmented.Control;
using Plugin.Segmented.Control.iOS;
using Plugin.Segmented.Event;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace Plugin.Segmented.Control.iOS
{
    [Preserve(AllMembers = true)]
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, UISegmentedControl>
    {
        private UISegmentedControl _nativeControl;

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (Control is null && Element != null)
            {
                _nativeControl = new UISegmentedControl
                {
                    Enabled = Element.IsEnabled
                };

                SetNativeControlSegments(Element.Children);
                SetNativeControl(_nativeControl);
                UpdateAppearance();
            }
            else if (!(_nativeControl is null) && !(e.NewElement is null))
            {
                // The renderer is being reused for another element: show that element's segments and settings.
                _nativeControl.Enabled = Element.IsEnabled;
                SetNativeControlSegments(Element.Children);
                UpdateAppearance();
            }

            if (!(e.OldElement is null))
            {
                if (!(_nativeControl is null))
                {
                    _nativeControl.ValueChanged -= NativeControl_SelectionChanged;
                }

                // Element is already e.NewElement here; detach from the element being replaced.
                RemoveElementHandlers(e.OldElement);
            }

            if (!(e.NewElement is null))
            {
                if (!(_nativeControl is null))
                {
                    _nativeControl.ValueChanged += NativeControl_SelectionChanged;
                }

                AddElementHandlers(e.NewElement);
            }
        }

        private void SetNativeControlSegments(IList<SegmentedControlOption> children)
        {
            if (!(_nativeControl is null))
            {
                if (_nativeControl.NumberOfSegments > 0)
                {
                    _nativeControl.RemoveAllSegments();
                }

                foreach (var (child, i) in children.Select((child, i) => ( child, i)))
                {
                    _nativeControl.InsertSegment(child.Text, i, false);
                    _nativeControl.SetEnabled(children[i].IsEnabled, i);

                    if (children[i].WidthRequest > 0)
                        _nativeControl.SetWidth((nfloat)children[i].WidthRequest, i);
                }

                if (!(Element is null))
                {
                    _nativeControl.SelectedSegment = Element.SelectedSegment;
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

                case nameof(SegmentedControl.TintColor):
                    SetEnabledStateColor();
                    break;

                case nameof(SegmentedControl.IsEnabled):
                    _nativeControl.Enabled = Element.IsEnabled;
                    SetEnabledStateColor();
                    SetBorder();
                    break;

                case nameof(SegmentedControl.DisabledColor):
                    SetEnabledStateColor();
                    SetBorder();
                    break;

                case nameof(SegmentedControl.SelectedTextColor):
                case nameof(SegmentedControl.TextColor):
                case nameof(SegmentedControl.FontSize):
                case nameof(SegmentedControl.FontFamily):
                    UpdateTextAttributes();
                    break;

                case nameof(SegmentedControl.Children):
                    if (!(Element.Children is null))
                    {
                        SetNativeControlSegments(Element.Children);
                        AddElementHandlers(Element, true);
                    }
                    break;

                case nameof(SegmentedControl.BorderWidth):
                case nameof(SegmentedControl.BorderColor):
                    SetBorder();
                    break;
            }
        }

        private void SetEnabledStateColor()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                _nativeControl.SelectedSegmentTintColor = Element.IsEnabled 
                    ? Element.TintColor.ToUIColor() 
                    : Element.DisabledColor.ToUIColor();
            }
            else
            {
                _nativeControl.TintColor = Element.IsEnabled 
                    ? Element.TintColor.ToUIColor() 
                    : Element.DisabledColor.ToUIColor();
            }
        }

        private void UpdateAppearance()
        {
            SetEnabledStateColor();
            UpdateTextAttributes();
            SetBorder();
        }

        // Font and color for both states in one place: the selected state used to be copied from the normal state
        // once, so later font changes never reached the selected segment.
        private void UpdateTextAttributes()
        {
            var size = (nfloat)Element.FontSize;
            var font = (string.IsNullOrEmpty(Element.FontFamily) ? null : UIFont.FromName(Element.FontFamily, size))
                ?? UIFont.SystemFontOfSize(size);

            var normal = _nativeControl.GetTitleTextAttributes(UIControlState.Normal);
            normal.Font = font;
            normal.TextColor = Element.TextColor.ToUIColor();
            _nativeControl.SetTitleTextAttributes(normal, UIControlState.Normal);

            var selected = _nativeControl.GetTitleTextAttributes(UIControlState.Selected);
            selected.Font = font;
            selected.TextColor = Element.SelectedTextColor.ToUIColor();
            _nativeControl.SetTitleTextAttributes(selected, UIControlState.Selected);
        }

        private void SetBorder()
        {
            _nativeControl.Layer.BorderWidth = (nfloat)Element.BorderWidth;

            _nativeControl.Layer.BorderColor = Element.IsEnabled ?
                Element.BorderColor.ToCGColor() :
                Element.DisabledColor.ToCGColor();
        }

        private void NativeControl_SelectionChanged(object sender, EventArgs e)
        {
            if (Element is null || _nativeControl is null)
            {
                return;
            }

            Element.SelectedSegment = (int)_nativeControl.SelectedSegment;
        }

        protected override void Dispose(bool disposing)
        {
            if (!(_nativeControl is null))
            {
                _nativeControl.ValueChanged -= NativeControl_SelectionChanged;
                _nativeControl?.Dispose();
                _nativeControl = null;
            }
            RemoveElementHandlers(Element);

            base.Dispose(disposing);
        }

        public static void Initialize()
        {

        }
    }
}