using System.Threading.Tasks;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Test.SegCtrl.netstandard
{
    public partial class ThirdPage : ContentPage
    {
        public ThirdPage()
        {
            InitializeComponent();
            BindingContext = new ThirdPageViewModel();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var newPage = new NavigationPage(new SegmentedControl.MainPage());
            Application.Current.MainPage = newPage;
        }
    }

    public class ThirdPageViewModel
    {
        public List<Item> Items { get; }
        public Item TheSelectedItem {get; set;}

        public ThirdPageViewModel()
        {
            //Test TextPropertyName
            Items = new List<Item>
            {
                new Item {TheValue = "Item 1"}, 
                new Item {TheValue = "Item 2"}, 
                new Item {TheValue = "Item 3"},
            };

            TheSelectedItem = Items[1];
            
            //Test property changed
            Task.Delay(2000).ContinueWith(t =>
            {
                Items[0].TheValue = "New Item 1!!";
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }


        public class Item : INotifyPropertyChanged
        {
            private string theValue;

            public event PropertyChangedEventHandler PropertyChanged;

            public string TheValue
            {
                get => theValue;
                set
                {
                    theValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TheValue)));
                }
            }
        }
    }
}