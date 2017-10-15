using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Plugin.SegmentedControl.Netstandard.Event;

namespace Plugin.SegmentedControl.Netstandard.Control
{
    public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
    {
        public event EventHandler<SegmentSelectEventArgs> OnSegmentSelected;
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

        public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create("DisabledColor", typeof(Color), typeof(SegmentedControl), Color.Gray);

        public Color DisabledColor
        {
            get => (Color)GetValue(DisabledColorProperty);
            set => SetValue(DisabledColorProperty, value);
        }


        public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create("SelectedSegment", typeof(int), typeof(SegmentedControl), 0);

        public int SelectedSegment
        {
            get => (int)GetValue(SelectedSegmentProperty);
            set => SetValue(SelectedSegmentProperty, value);
        }


        //public static readonly BindableProperty FontSizeProperty = BindableProperty.Create("FontSize", typeof(double), typeof(SegmentedControl), Device.GetNamedSize(NamedSize.Medium, typeof(Label)));

        //public double FontSize
        //{
        //    get => (double)GetValue(FontSizeProperty);
        //    set => SetValue(FontSizeProperty, value);
        //}

        //public static readonly BindableProperty FontWeigthProperty = BindableProperty.Create("FontAttributes", typeof(FontAttributes), typeof(SegmentedControl), default(FontAttributes));

        //public FontAttributes FontAttributes
        //{
        //    get => (FontAttributes)GetValue(FontWeigthProperty);
        //    set => SetValue(FontWeigthProperty, value);
        //}

        public SegmentedControl()
        {
            Children = new List<SegmentedControlOption>();
        }
        

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseSelectionChanged()
        {
            OnSegmentSelected?.Invoke(this, new SegmentSelectEventArgs { NewValue = this.SelectedSegment });
        }
    }
}
