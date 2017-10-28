using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Segmented.Event;
using Xamarin.Forms;

namespace Test.SegmentedControl
{
    public partial class MainPage : ContentPage
    {
        public static readonly BindableProperty SegmentSelectProperty = BindableProperty.Create(
            propertyName: "SegmentSelect",
            returnType: typeof(int),
            declaringType: typeof(MainPage),
            defaultValue: default(int));

        public int SegmentSelect
        {
            get => (int) GetValue(SegmentSelectProperty);
            set => SetValue(SegmentSelectProperty, value);
        }

        public int SegmentSelection => 2;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;
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
            SegmentedControl.IsEnabled = false;

        }

        private void Enable_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.IsEnabled = true;

        }

        private void ChangeDisabledColor_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.DisabledColor = Color.Brown;
        }

        private void SelectSegment3(object sender, EventArgs e)
        {
            SegmentSelect = 2;
        }
    }
}
