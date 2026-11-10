using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Plugin.Segmented.Control;
using Plugin.Segmented.Control.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace Plugin.Segmented.Control.UWP
{
    [Preserve(AllMembers = true)]
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, SegmentedUserControl>
    {
        private SegmentedUserControl _segmentedUserControl;

        private readonly ColorConverter _colorConverter = new ColorConverter();

        // The options this renderer is subscribed to. Tracked separately because by the time Children or the
        // element changes, Element.Children already returns the new list.
        private readonly List<SegmentedControlOption> _subscribedOptions = new List<SegmentedControlOption>();

        public SegmentedControlRenderer() {}

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                DisposeEventHandlers();
            }

            if (_segmentedUserControl is null || e.NewElement != null)
            {
                CreateSegmentedRadioButtonControl();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (_segmentedUserControl is null || Element is null) return;

            switch (e.PropertyName)
            {
                case nameof(SegmentedControl.SelectedSegment):
                    SetSelectedSegment();
                    break;

                case nameof(SegmentedControl.TintColor):
                    SetTintColor();
                    break;

                case nameof(SegmentedControl.IsEnabled):
                    SetIsEnabled();
                    break;

                case nameof(SegmentedControl.DisabledColor):
                    SetDisabledColor();
                    break;

                case nameof(SegmentedControl.TextColor):
                    SetTextColor();
                    break;

                case nameof(SegmentedControl.SelectedTextColor):
                    SetSelectedTextColor();
                    break;

                case nameof(SegmentedControl.Children):
                    SetChildren();
                    break;

                case nameof(SegmentedControl.FontSize):
                    SetFontSize();
                    break;

                case nameof(SegmentedControl.FontFamily):
                    SetFontFamily();
                    break;
            }
        }

        private void SetFontSize()
        {
            if (Element.FontSize > 0)
            {
                foreach (SegmentRadioButton segment in _segmentedUserControl.SegmentedControlGrid.Children)
                {
                    segment.FontSize = Element.FontSize;
                };
            }
        }

        private void SetFontFamily()
        {
            if (!string.IsNullOrWhiteSpace(Element.FontFamily))
            {
                foreach (SegmentRadioButton segment in _segmentedUserControl.SegmentedControlGrid.Children)
                {
                    segment.FontFamily = new FontFamily(Element.FontFamily);
                };
            }
        }

        private void SetSelectedSegment()
        {
            if (_segmentedUserControl.SegmentedControlGrid.Children
                .Where(x =>
                {
                    var btn = (RadioButton)x;

                    int.TryParse(btn.Tag.ToString(), out var i);
                    return i == Element.SelectedSegment;
                })
                .FirstOrDefault() is RadioButton checkedButton)
            {
                checkedButton.IsChecked = true;
            }

        }

        private void SetTintColor()
        {
            _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");

            foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
            {
                ((SegmentRadioButton)segment).TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");
            }
        }

        private void SetIsEnabled()
        {
            if (Element.IsEnabled)
            {
                foreach (var uiElement in _segmentedUserControl.SegmentedControlGrid.Children)
                {
                    var segment = (SegmentRadioButton)uiElement;
                    segment.IsEnabled = true;
                }
                _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");
            }
            else
            {
                foreach (var uiElement in _segmentedUserControl.SegmentedControlGrid.Children)
                {
                    var segment = (SegmentRadioButton)uiElement;
                    segment.IsEnabled = false;
                }
                _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
            }
        }

        private void SetDisabledColor()
        {
            foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
            {
                ((SegmentRadioButton)segment).DisabledColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
            }

            if (!Element.IsEnabled)
            {
                _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
            }
        }

        private void SetTextColor()
        {
            foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
            {
                ((SegmentRadioButton)segment).TextColor = (SolidColorBrush)_colorConverter.Convert(
                    Element.TextColor, null, null, "");
            }
        }

        private void SetSelectedTextColor()
        {
            foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
            {
                ((SegmentRadioButton)segment).SelectedTextColor = (SolidColorBrush)_colorConverter.Convert(Element.SelectedTextColor, null, null, "");
            }
        }

        private void SetChildren()
        {
            if (Element.Children != null)
            {
                DisposeEventHandlers();
                CreateSegmentedRadioButtonControl();
            }
        }
        
        private void CreateSegmentedRadioButtonControl()
        {
            _segmentedUserControl = new SegmentedUserControl();

            var radioButtonGroupName = Guid.NewGuid().ToString();

            var grid = _segmentedUserControl.SegmentedControlGrid;
            grid.BorderBrush = (SolidColorBrush) _colorConverter.Convert(Element.TintColor, null, null, "");

            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();

            foreach (var (child, i) in Element.Children.Select((child, i) => (child, i)))
            {
                var segmentButton = new SegmentRadioButton
                {
                    GroupName = radioButtonGroupName,
                    Style = (Style)_segmentedUserControl.Resources["SegmentedRadioButtonStyle"],
                    Content = child.Text,
                    Tag = i,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, ""),
                    TextColor = (SolidColorBrush)_colorConverter.Convert(Element.TextColor, null, null, ""),
                    SelectedTextColor = (SolidColorBrush)_colorConverter.Convert(Element.SelectedTextColor, null, null, ""),
                    TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, ""),
                    DisabledColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, ""),
                    BorderThickness = i > 0 ? new Thickness(1, 0, 0, 0) : new Thickness(0, 0, 0, 0),
                    IsEnabled = Element.Children[i].IsEnabled, 
                    MinWidth = 0,
                };

                segmentButton.Checked += SegmentRadioButtonOnChecked;

                if (i == Element.SelectedSegment)
                {
                    segmentButton.IsChecked = true;
                }

                if (Element.FontSize > 0)
                {
                    segmentButton.FontSize = Element.FontSize;
                }

                if (!string.IsNullOrEmpty(Element.FontFamily))
                {
                    segmentButton.FontFamily = new FontFamily(Element.FontFamily);
                }

                var columnDefinition = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star),
                };

                if (child.WidthRequest > 0)
                    columnDefinition.Width = new GridLength(child.WidthRequest);

                grid.ColumnDefinitions.Add(columnDefinition);

                segmentButton.SetValue(Grid.ColumnProperty, i);

                grid.Children.Add(segmentButton);

                child.PropertyChanged += Segment_PropertyChanged;
                _subscribedOptions.Add(child);
            }

            SetNativeControl(_segmentedUserControl);
        }

        private void Segment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(_segmentedUserControl is null) && !(Element is null) && sender is SegmentedControlOption option)
            {
                var index = Element.Children.IndexOf(option);
                if (index < 0 || index >= _segmentedUserControl.SegmentedControlGrid.Children.Count)
                {
                    return;
                }

                switch (e.PropertyName)
                {
                    case nameof(SegmentedControlOption.Text):
                        _segmentedUserControl.SegmentedControlGrid.Children[index].SetValue(ContentControl.ContentProperty, option.Text);
                        break;
                    case nameof(SegmentedControlOption.IsEnabled):
                        _segmentedUserControl.SegmentedControlGrid.Children[index].SetValue(Windows.UI.Xaml.Controls.Control.IsEnabledProperty, option.IsEnabled);
                        break;
                }
            }
        }

        private void SegmentRadioButtonOnChecked(object sender, RoutedEventArgs e)
        {
            // SegmentedControl raises OnSegmentSelected and the command itself when SelectedSegment changes.
            if (sender is SegmentRadioButton button && !(Element is null))
            {
                Element.SelectedSegment = (int)button.Tag;
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeEventHandlers();
            base.Dispose(disposing);
        }

        private void DisposeEventHandlers()
        {

            if (_segmentedUserControl?.SegmentedControlGrid?.Children != null)
            {
                foreach (var element in _segmentedUserControl.SegmentedControlGrid.Children)
                {
                    if (element is SegmentRadioButton segment)
                    {
                        segment.Checked -= SegmentRadioButtonOnChecked;
                    }
                }
            }

            foreach (var option in _subscribedOptions)
            {
                option.PropertyChanged -= Segment_PropertyChanged;
            }

            _subscribedOptions.Clear();
        }

    }
}
