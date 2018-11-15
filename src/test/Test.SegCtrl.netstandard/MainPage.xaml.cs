using System;
using System.Collections.Generic;
using Plugin.Segmented.Control;
using Plugin.Segmented.Event;
using Test.SegCtrl;
using Xamarin.Forms;

namespace Test.SegmentedControl
{
    public partial class MainPage : ContentPage
    {
        //public static readonly BindableProperty SegmentSelectProperty = BindableProperty.Create(
            //propertyName: "SegmentSelect",
            //returnType: typeof(int),
            //declaringType: typeof(MainPage),
            //defaultValue: default(int));

        //public static readonly BindableProperty ChangeTextProperty = BindableProperty.Create(nameof(ChangeText), typeof(string), typeof(MainPage), "Item1", propertyChanged: OnTextChanged);

        //private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
        //{
            
        //}

        //public string ChangeText
        //{
        //    get => (string)GetValue(ChangeTextProperty);
        //    set => SetValue(ChangeTextProperty, value);
        //}

        //public int SegmentSelect
        //{
        //    get => (int) GetValue(SegmentSelectProperty);
        //    set => SetValue(SegmentSelectProperty, value);
        //}

        //public int SegmentSelection => 2;

        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MainViewModel();
        }

        //private void SegmentedControl_OnValueChanged(object sender, SegmentSelectEventArgs e)
        //{
        //    ChoiceLabel.Text = SegmentedControl.SelectedSegment.ToString();
        //}

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
            _viewModel.SelectedSegment = 2;
        }

        private void ChangeFirstText(object sender, EventArgs e)
        {
            const string boundText = "Item 1B";
            _viewModel.ChangeText = _viewModel.ChangeText == boundText ? "Item1" : boundText;
        }

        public void DisableFirstSegment_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.Children[0].IsEnabled = false;
        }

        public void EnableFirstSegment_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.Children[0].IsEnabled = true;
        }

        public void OnElementChildrenChanging(object sender, ElementChildrenChanging e)
        {
            if (e.OldValues != null && e.OldValues.Count > 0)
            {
                e.OldValues[0].RemoveBinding(SegmentedControlOption.TextProperty);
            }
            if (e.NewValues != null && e.NewValues.Count > 0)
            {
                e.NewValues[0].SetBinding(SegmentedControlOption.TextProperty, nameof(_viewModel.ChangeText));
            }
        }
    }
}
