using System;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Plugin.Segmented.Control;
using Plugin.Segmented.Control.UWP;
using Xamarin.Forms.Platform.UWP;
using Grid = Windows.UI.Xaml.Controls.Grid;
using SegmentedUserControl = Plugin.Segmented.Control.UWP.SegmentedUserControl;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace Plugin.Segmented.Control.UWP
{
    public class SegmentedControlRenderer : ViewRenderer<Segmented.Control.SegmentedControl, SegmentedUserControl>
    {
        private SegmentedUserControl _segmentedUserControl;

        private readonly ColorConverter _colorConverter = new ColorConverter();

        public SegmentedControlRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Segmented.Control.SegmentedControl> e)
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
                //Element?.RaiseSelectionChanged();
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
                        foreach (var uiElement in _segmentedUserControl.SegmentedControlGrid.Children)
                        {
                            var segment = (SegmentRadioButton) uiElement;
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
                    break;
                case "DisabledColor":
                    foreach (var segment in _segmentedUserControl.SegmentedControlGrid.Children)
                    {
                        ((SegmentRadioButton)segment).DisabledColor = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
                    }

                    if (!Element.IsEnabled)
                    {
                        _segmentedUserControl.SegmentedControlGrid.BorderBrush = (SolidColorBrush)_colorConverter.Convert(Element.DisabledColor, null, null, "");
                    }

                    break;
                    
                case "SelectedTextColor":
                    SetSelectedTextColor();
                    break;
                default:
                    break;
            }
        }

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

            var radioButtonGroupName = Guid.NewGuid().ToString();

            var grid = _segmentedUserControl.SegmentedControlGrid;
            grid.BorderBrush = (SolidColorBrush) _colorConverter.Convert(Element.TintColor, null, null, "");

            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();

            foreach (var child in Element.Children.Select((value, i) => new {i, value}))
            {
                var segmentButton = new SegmentRadioButton()
                {
                    GroupName = radioButtonGroupName,
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
