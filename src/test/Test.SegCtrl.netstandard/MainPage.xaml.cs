﻿using System;
using Plugin.Segmented.Control;
using Plugin.Segmented.Event;
using Test.SegCtrl;
using Xamarin.Forms;

namespace Test.SegmentedControl
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MainViewModel();
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

        private void ButtonBackgroundColor_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.BackgroundColor = Color.HotPink;
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

        public void ChangeTextSize_OnClicked(object sender, EventArgs e)
        {
            SegmentedControl.FontSize = SegmentedControl.FontSize < 20 ? 20 : 12;
        }

        public void ChangeFontFamily_OnClicked(object sender, EventArgs e)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    SegmentedControl.FontFamily = SegmentedControl.FontFamily == "monospace" ? "serif" : "monospace";
                    break;

                case Device.iOS:
                case Device.macOS:
                    SegmentedControl.FontFamily = SegmentedControl.FontFamily == "Baskerville" ? "HelveticaNeue" : "Baskerville";
                    break;

                case Device.UWP:
                    SegmentedControl.FontFamily = SegmentedControl.FontFamily == "Courier New" ? "Microsoft Sans Serif" : "Courier New";
                    break;

            }
        }

        public void SecondPage_OnClicked(object sender, EventArgs e)
        {
            var newPage = new NavigationPage(new SegCtrl.netstandard.SecondPage());
            Application.Current.MainPage = newPage;
        }

        private bool _isTextColorChanged;
        private Color _defaultTextColor;

        private void Button_TextColor(object sender, EventArgs e)
        {
            if (!_isTextColorChanged)
            {
                _defaultTextColor = SegmentedControl.TextColor;
                SegmentedControl.TextColor = Color.Chocolate;
                _isTextColorChanged = true;
            }
            else
            {
                _isTextColorChanged = false;
                SegmentedControl.TextColor = _defaultTextColor;
            }
        }
    }
}
