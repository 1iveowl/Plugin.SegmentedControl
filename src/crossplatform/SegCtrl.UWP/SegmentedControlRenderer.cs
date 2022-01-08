using System;
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

            if (e.PropertyName == "Renderer")
            {
                Element?.RaiseSelectionChanged();
                return;
            }

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

            foreach (var child in Element.Children.Select((value, i) => new {i, value}))
            {
                var segmentButton = new SegmentRadioButton
                {
                    GroupName = radioButtonGroupName,
                    Style = (Style)_segmentedUserControl.Resources["SegmentedRadioButtonStyle"],
                    Content = child.value.Text,
                    Tag = child.i,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, ""),
                    TextColor = (SolidColorBrush)_colorConverter.Convert(Element.TextColor, null, null, ""),
                    SelectedTextColor = (SolidColorBrush)_colorConverter.Convert(Element.SelectedTextColor, null, null, ""),
                    TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, ""),
                    DisabledColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, ""),
                    BorderThickness = child.i > 0 ? new Thickness(1, 0, 0, 0) : new Thickness(0, 0, 0, 0),
                    IsEnabled = Element.Children[child.i].IsEnabled, 
                    MinWidth = 0,
                };

                segmentButton.Checked += SegmentRadioButtonOnChecked;

                if (child.i == Element.SelectedSegment)
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

                var cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star),
                };
                if (Element.WidthRequest > 0)
                    cd.Width = new GridLength(Element.WidthRequest);

                grid.ColumnDefinitions.Add(cd);

                segmentButton.SetValue(Grid.ColumnProperty, child.i);

                grid.Children.Add(segmentButton);

                child.value.PropertyChanged += Segment_PropertyChanged;
            }

            SetNativeControl(_segmentedUserControl);
        }

        private void Segment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(_segmentedUserControl is null) && !(Element is null) && sender is SegmentedControlOption option)
            {
                var index = Element.Children.IndexOf(option);
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
            var button = (SegmentRadioButton) sender;

            if (!(button is null))
            {
                Element.SelectedSegment = int.Parse(button.Tag.ToString());
                Element?.RaiseSelectionChanged();
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

            if (!(Element is null))
            {
                foreach (var child in Element.Children)
                {
                    child.PropertyChanged -= Segment_PropertyChanged;
                }
            }
        }

    }
}
