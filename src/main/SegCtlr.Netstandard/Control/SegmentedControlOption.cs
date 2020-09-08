using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Plugin.Segmented.Control
{
    [Preserve(AllMembers = true)]
    public class SegmentedControlOption : View
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SegmentedControlOption), string.Empty);
        public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(object), typeof(SegmentedControlOption), propertyChanged: (bindable, value, newValue) => ((SegmentedControlOption)bindable).OnItemChanged(value, newValue));
        public static readonly BindableProperty TextPropertyNameProperty = BindableProperty.Create(nameof(TextPropertyName), typeof(string), typeof(SegmentedControlOption));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public object Item         
        {
            get => GetValue(ItemProperty);
            set => SetValue(ItemProperty, value);
        }

        public string TextPropertyName 
        {
            get => (string)GetValue(TextPropertyNameProperty);
            set => SetValue(TextPropertyNameProperty, value);
        }

        private void OnItemChanged(object value, object newValue)
        {
            if (value is INotifyPropertyChanged mutableItem)
                mutableItem.PropertyChanged -= OnItemPropertyChanged;
            if (newValue is INotifyPropertyChanged newMutableItem)
                newMutableItem.PropertyChanged += OnItemPropertyChanged;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Item) || propertyName == nameof(TextPropertyName))
                SetTextFromItemProperty();
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == TextPropertyName)
                SetTextFromItemProperty();
        }

        private void SetTextFromItemProperty()
        {
            if (Item != null && TextPropertyName != null)
                Text = Item.GetType().GetProperty(TextPropertyName)?.GetValue(Item)?.ToString();
        }
    }
}
