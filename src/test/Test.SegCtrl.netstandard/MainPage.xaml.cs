using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.SegmentedControl.Netstandard.Event;
using Xamarin.Forms;

namespace Test.SegmentedControl
{
    public partial class MainPage : ContentPage
    {

        public int SegmentSelection => 2;

        public MainPage()
        {
            InitializeComponent();
        }

        private void SegmentedControl_OnValueChanged(object sender, SegmentSelectEventArgs e)
        {
            ChoiceLabel.Text = SegmentedControl.SelectedSegment.ToString();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            SegmentWithStack.Children.Remove(SegmentedControl);
        }

        private void ButtonTintColor_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.TintColor = Color.Aqua;
        }

        private void ButtonSelectedTextColor_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.SelectedTextColor = Color.Red;
        }

        private void Disable_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.DisabledColor = Color.Gray;
        }
    }
}
