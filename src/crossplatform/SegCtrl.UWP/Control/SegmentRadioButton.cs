using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Plugin.SegmentedControl.UWP.Control
{
    public class SegmentRadioButton : RadioButton
    {
        public static readonly DependencyProperty SelectedTextColorProperty = DependencyProperty.Register(
            "SelectedTextColor", 
            typeof(SolidColorBrush), typeof(SegmentRadioButton), 
            new PropertyMetadata(default(SolidColorBrush), new PropertyChangedCallback(OnSelectedTextChanged)));

        public SolidColorBrush SelectedTextColor
        {
            get => (SolidColorBrush) GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            "TintColor", typeof(SolidColorBrush), typeof(SegmentRadioButton), new PropertyMetadata(default(SolidColorBrush), new PropertyChangedCallback(OnTintChanged)));

        public SolidColorBrush TintColor
        {
            get => (SolidColorBrush) GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public SegmentRadioButton()
        {
            
        }

        private static void OnTintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentRadioButton segment)
            {
                segment.BorderBrush = (SolidColorBrush) e.NewValue;

                if (segment.IsChecked ?? false)
                {
                    // Hack to make the selected segment re-draw.
                    segment.IsChecked = false;
                    segment.IsChecked = true;
                }
            }
        }

        private static void OnSelectedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentRadioButton segment)
            {
                if (segment.IsChecked ?? false)
                {
                    // Hack to make the selected segment re-draw.
                    segment.IsChecked = false;
                    segment.IsChecked = true;
                }
            }
        }

    }
}
