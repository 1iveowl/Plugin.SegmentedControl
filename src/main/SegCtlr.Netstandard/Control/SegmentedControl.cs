using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Plugin.SegmentedControl.Netstandard.Event;

namespace Plugin.SegmentedControl.Netstandard.Control
{
    public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
    {
        public event EventHandler<SegmentSelectEventArgs> SegmentSelected;
        public IList<SegmentedControlOption> Children { get; set; }

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create("TintColor", typeof(Color), typeof(SegmentedControl), Color.Blue);

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create("SelectedTextColor", typeof(Color), typeof(SegmentedControl), Color.White);

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create("SelectedSegment", typeof(int), typeof(SegmentedControl), 0);

        public int SelectedSegment
        {
            get => (int)GetValue(SelectedSegmentProperty);
            set => SetValue(SelectedSegmentProperty, value);
        }

        //TODO Implement across platforms
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(SegmentedControl), 12);

        public double FontSize
        {
            get => (double) GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public SegmentedControl()
        {
            Children = new List<SegmentedControlOption>();
        }
        

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseSelectionChanged()
        {
            SegmentSelected?.Invoke(this, new SegmentSelectEventArgs { NewValue = this.SelectedSegment });
        }
    }
}
