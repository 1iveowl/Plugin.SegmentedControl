using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Plugin.Segmented.Control;
using Xamarin.Forms;

namespace Test.SegCtrl
{
    public class MainViewModel : INotifyPropertyChanged
    {
        SegmentedControlOption[] list1 = {
            new SegmentedControlOption{Text="Test0"},
            new SegmentedControlOption{Text="Test1"},
            new SegmentedControlOption{Text="Test2"}
        };

        SegmentedControlOption[] list2 = {
            new SegmentedControlOption{Text="Item1"},
            new SegmentedControlOption{Text="Item2"},
            new SegmentedControlOption{Text="Item3"},
            new SegmentedControlOption{Text="Item4"},
            new SegmentedControlOption{Text="Item5"}
        };

        string[] stringSet1 = { "TestZ", "TestY" };
        string[] stringSet2 = { "TestA", "TestB", "TestC", "TestD" };
        public MainViewModel()
        {
            ChangeText = "Item 1B";
            SegmentItemsSource = new List<SegmentedControlOption>(list1);
            ChangeItemsSourceCommand = new Command(OnChangeItemsSource);
            SegmentStringSource = new List<string>(stringSet1);
        }

        private void OnChangeItemsSource(object obj)
        {
            //SegmentItemsSource[0].RemoveBinding(SegmentedControlOption.TextProperty);
            //SegmentItemsSource = SegmentItemsSource.Count == list1.Length ? new List<SegmentedControlOption>(list2) : new List<SegmentedControlOption>(list1);
            //SegmentItemsSource[0].SetBinding(SegmentedControlOption.TextProperty, nameof(ChangeText));
            SegmentStringSource = SegmentStringSource.Count == stringSet1.Length ? new List<string>(stringSet2) : new List<string>(stringSet1);
        }

        private string _changeText;
        public string ChangeText
        {
            get { return _changeText; }
            set { _changeText = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChangeText))); }
        }

        private int _selectedSegment;
        public int SelectedSegment
        {
            get { return _selectedSegment; }
            set { _selectedSegment = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedSegment))); }
        }
        private IList<SegmentedControlOption> _segmentItemsSource;
        public IList<SegmentedControlOption> SegmentItemsSource
        {
            get { return _segmentItemsSource; }
            set { _segmentItemsSource = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(SegmentItemsSource))); }
        }

        private IList<string> _segmentStringSource;
        public IList<string> SegmentStringSource
        {
            get { return _segmentStringSource; }
            set { _segmentStringSource = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(SegmentStringSource))); }
        }

        public ICommand ChangeItemsSourceCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}