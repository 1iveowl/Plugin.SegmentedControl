using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Test.SegCtrl.netstandard
{
    public partial class SecondPage : ContentPage
    {
        public SecondPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var newPage = new NavigationPage(new SegmentedControl.MainPage());
            Application.Current.MainPage = newPage;
        }

        private void Seg_OnSegmentSelected(object sender, Plugin.Segmented.Event.SegmentSelectEventArgs e)
        {
            selectedItem.Text = seg.Children[e.NewValue].Text;
        }
    }
}
