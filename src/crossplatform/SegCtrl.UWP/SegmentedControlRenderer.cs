using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Plugin.SegmentedControl.Netstandard.Control;
using Plugin.SegmentedControl.UWP;
using Plugin.SegmentedControl.UWP.Control;
using Xamarin.Forms.Platform.UWP;
using Grid = Windows.UI.Xaml.Controls.Grid;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace Plugin.SegmentedControl.UWP
{
    public class SegmentedControlRenderer : ViewRenderer<Netstandard.Control.SegmentedControl, Control.SegmentedUserControl>
    {
        private SegmentedUserControl _segmentedUserControl;

        private readonly ColorConverter _colorConverter = new ColorConverter();

        public SegmentedControlRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Netstandard.Control.SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (_segmentedUserControl == null)
            {
                CreateSegmentedRadioButtonControl();
            }

            if (e.OldElement != null)
            {
                DisposeEventHandlers();
            }

            if (e.NewElement != null)
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

            if (_segmentedUserControl == null || Element == null) return;

            switch (e.PropertyName)
            {
                case "TintColor":
                    _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");

                    foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
                    {
                        ((SegmentRadioButton)segment).TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");
                    }
                    break;
                case "IsEnabled":
                    if (Element.IsEnabled)
                    {
                        _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");
                    }
                    else
                    {
                        _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
                    }
                    break;
                case "DisabledColor":
                    _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");

                    foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
                    {
                        ((SegmentRadioButton)segment).DisabledColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
                    }
                    break;
                    
                case "SelectedTextColor":
                    SetSelectedTextColor();
                    break;
                default:
                    break;
            }
        }

        //private void SetTintColor()
        //{
        //    if (Element.IsEnabled)
        //    {
        //        _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");

        //        foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
        //        {
        //            ((SegmentRadioButton)segment).TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");
        //        }
        //    }
        //    else
        //    {
        //        _segmentedUserControl.SegmentedControlGrid.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Gray);
        //        foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
        //        {
        //            ((SegmentRadioButton)segment).TintColor = new SolidColorBrush(Windows.UI.Colors.Gray);
        //        }
        //    }
        //}

        //private void SetTintColor(SegmentRadioButton segment)
        //{
        //    if (Element.IsEnabled)
        //    {
        //        segment.TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, "");
        //    }
        //    else
        //    {
        //        segment.TintColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
        //    }
        //}

        private void SetSelectedTextColor()
        {
            foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
            {
                ((SegmentRadioButton)segment).SelectedTextColor = (SolidColorBrush)_colorConverter.Convert(Element.SelectedTextColor, null, null, "");
            }
        }

        private void CreateSegmentedRadioButtonControl()
        {
            _segmentedUserControl = new SegmentedUserControl();

            var grid = _segmentedUserControl.SegmentedControlGrid;
            grid.BorderBrush = (SolidColorBrush) _colorConverter.Convert(Element.TintColor, null, null, "");

            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();

            foreach (var child in Element.Children.Select((value, i) => new {i, value}))
            {
                var segmentButton = new SegmentRadioButton()
                {
                    Style = (Style)_segmentedUserControl.Resources["SegmentedRadioButtonStyle"],
                    Content = child.value.Text,
                    Tag = child.i,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, ""),
                    SelectedTextColor = (SolidColorBrush)_colorConverter.Convert(Element.SelectedTextColor, null, null, ""),
                    TintColor = (SolidColorBrush)_colorConverter.Convert(Element.TintColor, null, null, ""),
                    DisabledColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, ""),
                    BorderThickness = child.i > 0 ? new Thickness(1, 0, 0, 0) : new Thickness(0, 0, 0, 0),
                    IsEnabled = Element.IsEnabled
                };

                segmentButton.Checked += SegmentRadioButtonOnChecked;

                if (child.i == Element.SelectedSegment)
                {
                    segmentButton.IsChecked = true;
                }
                
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star),
                });

                segmentButton.SetValue(Grid.ColumnProperty, child.i);

                grid.Children.Add(segmentButton);
            }

            SetNativeControl(_segmentedUserControl);
        }

        private void SegmentRadioButtonOnChecked(object sender, RoutedEventArgs e)
        {
            var button = (SegmentRadioButton) sender;

            if (button != null)
            {
                Element.SelectedSegment = int.Parse(button.Tag.ToString());
                Element.RaiseSelectionChanged();
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeEventHandlers();

            base.Dispose(disposing);
        }

        private void DisposeEventHandlers()
        {
            if (_segmentedUserControl != null)
            {
                foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
                {
                    ((SegmentRadioButton)segment).Checked -= SegmentRadioButtonOnChecked;
                }
            }
        }
    }
}
