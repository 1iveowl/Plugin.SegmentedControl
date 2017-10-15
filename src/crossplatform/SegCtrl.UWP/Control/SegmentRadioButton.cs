using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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

        public static readonly DependencyProperty DisabledColorProperty = DependencyProperty.Register(
            "DisabledColor", typeof(SolidColorBrush), typeof(SegmentRadioButton), new PropertyMetadata(default(SolidColorBrush), new PropertyChangedCallback(OnDisabledColorChanged)));

        public SolidColorBrush DisabledColor
        {
            get => (SolidColorBrush) GetValue(DisabledColorProperty);
            set => SetValue(DisabledColorProperty, value);
        }


        public SegmentRadioButton()
        {
            this.IsEnabledChanged += SegmentRadioButton_IsEnabledChanged;
        }

        private void SegmentRadioButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is SegmentRadioButton segment)
            {
                if (segment.IsChecked ?? false)
                {
                    if (!segment.IsEnabled)
                    {
                        VisualStateManager.GoToState(this, "DisabledAndChecked", false);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "Checked", false);
  
                        segment.IsChecked = false;
                        segment.IsChecked = true;
                    }
                }
            }
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


        private static void OnDisabledColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentRadioButton segment)
            {
                segment.DisabledColor = (SolidColorBrush) e.NewValue;

                if (segment.IsChecked ?? false)
                {
                    //segment.IsChecked = false;
                    //segment.IsChecked = true;
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
