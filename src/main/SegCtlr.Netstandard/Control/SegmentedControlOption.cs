using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Plugin.SegmentedControl.Netstandard.Control
{
    public class SegmentedControlOption : View
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(SegmentedControlOption), string.Empty);

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
