using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using Plugin.SegmentedControl.Netstandard.Event;

namespace Plugin.SegmentedControl.Netstandard.Control
{
    public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
    {
        public IList<SegmentedControlOption> Children { get; set; }

        public SegmentedControl()
        {
            Children = new List<SegmentedControlOption>();
        }

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create("TintColor", typeof(Color), typeof(SegmentedControl), Color.Blue);

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create("SelectedTextColor", typeof(Color), typeof(SegmentedControl), Color.White);

        public Color SelectedTextColor
        {
            get { return (Color)GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }

        public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create("SelectedSegment", typeof(int), typeof(SegmentedControl), 0);

        public int SelectedSegment
        {
            get
            {
                return (int)GetValue(SelectedSegmentProperty);
            }
            set
            {
                SetValue(SelectedSegmentProperty, value);
            }
        }

        public event EventHandler<SegmentSelectEventArgs> ValueChanged;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SendValueChanged()
        {
            ValueChanged?.Invoke(this, new SegmentSelectEventArgs { NewValue = this.SelectedSegment });
        }
    }
}
