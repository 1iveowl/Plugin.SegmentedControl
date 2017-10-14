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
            new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush SelectedTextColor
        {
            get => (SolidColorBrush) GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            "TintColor", typeof(SolidColorBrush), typeof(SegmentRadioButton), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush TintColor
        {
            get => (SolidColorBrush) GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public SegmentRadioButton()
        {
            
        }
    }
}
