using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Plugin.Segmented.Control.UWP
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

        public static readonly DependencyProperty TextColorProperty = DependencyProperty.Register(
            "TextColor",
            typeof(SolidColorBrush),
            typeof(SegmentRadioButton),
            new PropertyMetadata(default(SolidColorBrush), new PropertyChangedCallback(OnTextColorChanged)));


        public SolidColorBrush TextColor
        {
            get => (SolidColorBrush)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly DependencyProperty TintColorProperty = DependencyProperty.Register(
            "TintColor", 
            typeof(SolidColorBrush), 
            typeof(SegmentRadioButton), new PropertyMetadata(default(SolidColorBrush), new PropertyChangedCallback(OnTintChanged)));

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
                Refresh(segment);
            }
        }

        private static void OnTintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentRadioButton segment)
            {
                segment.BorderBrush = (SolidColorBrush) e.NewValue;
                Refresh(segment);
            }
        }


        private static void OnDisabledColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentRadioButton segment)
            {
                segment.BorderBrush = (SolidColorBrush)e.NewValue;
                Refresh(segment);
            }
        }

        private static void OnSelectedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SegmentRadioButton segment)
            {
                segment.SelectedTextColor = (SolidColorBrush)e.NewValue;
                Refresh(segment);
            }
        }

        private static void OnTextColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (d is SegmentRadioButton segment)
            {
                segment.TextColor = (SolidColorBrush)e.NewValue;
                Refresh(segment);
            }
        }

        private static void Refresh(SegmentRadioButton segment)
        {
            // Go to "Indeterminate" State to ensure that the GotoState is refreshed even if the state is the same. 
            // Necessary because properties might have changed even when the state have not.

            VisualStateManager.GoToState(segment, "Indeterminate", false);

            if (segment.IsChecked ?? false)
            {
                VisualStateManager.GoToState(segment, segment.IsEnabled ? "Checked" : "DisabledAndChecked", false);
            }
            else
            {
                VisualStateManager.GoToState(segment, segment.IsEnabled ? "Unchecked" : "DisabledAndUnchecked", false);
            }
        }
    }
}
