using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Plugin.Segmented.Control;
using Xamarin.Forms;

namespace Test.SegCtrl;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly SegmentedControlOption[] _list1 = [
        new() {Text="Test0"},
        new() {Text="Test1"},
        new() {Text="Test2"}
    ];

    internal SegmentedControlOption[] List2 = [
        new() {Text="Item1"},
        new() {Text="Item2"},
        new() {Text="Item3"},
        new() {Text="Item4"},
        new() {Text="Item5"}
    ];

    private readonly string[] _stringSet1 = ["TestZ", "TestY"];
    private readonly string[] _stringSet2 = ["TestA", "TestB", "TestC", "TestD"];

    public MainViewModel()
    {
        ChoiceText = "Start";
        ChangeText = "Item 1B";
        SegmentItemsSource = [.. _list1];
        ChangeItemsSourceCommand = new Command(OnChangeItemsSource);
        SegmentStringSource = [.. _stringSet1];
        SegmentChangedCommand = new Command(OnSegmentChanged);
    }

    private int _changedCount;

    /// <summary>How many times SegmentChangedCommand has run; handy to watch in the debugger.</summary>
    public int SegmentChangedCount => _changedCount;

    private void OnSegmentChanged(object obj)
    {
        _changedCount++;
    }

    private void OnChangeItemsSource(object obj)
    {
        //SegmentItemsSource[0].RemoveBinding(SegmentedControlOption.TextProperty);
        //SegmentItemsSource = SegmentItemsSource.Count == list1.Length ? new List<SegmentedControlOption>(list2) : new List<SegmentedControlOption>(list1);
        //SegmentItemsSource[0].SetBinding(SegmentedControlOption.TextProperty, nameof(ChangeText));
        SegmentStringSource = SegmentStringSource.Count == _stringSet1.Length ? [.. _stringSet2] : [.. _stringSet1];
    }

    //private double _changeFontsize;

    //public double ChangeFontSize
    //{
    //    get => _changeFontsize;
    //    set { _changeFontsize = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChangeFontSize))); }
    //}

    private string _changeText;
    public string ChangeText
    {
        get => _changeText;
        set { _changeText = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChangeText))); }
    }

    private int _selectedSegment;
    public int SelectedSegment
    {
        get => _selectedSegment;
        set
        {
            _selectedSegment = value;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(SelectedSegment)));
            ChoiceText = value.ToString();
        }
    }
    private IList<SegmentedControlOption> _segmentItemsSource;
    public IList<SegmentedControlOption> SegmentItemsSource
    {
        get => _segmentItemsSource;
        set { _segmentItemsSource = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(SegmentItemsSource))); }
    }

    private IList<string> _segmentStringSource;
    public IList<string> SegmentStringSource
    {
        get => _segmentStringSource;
        set { _segmentStringSource = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(SegmentStringSource))); }
    }

    private string _choiceText;
    public string ChoiceText
    {
        get => _choiceText;
        set { _choiceText = value; OnPropertyChanged(new PropertyChangedEventArgs(nameof(ChoiceText))); }
    }

    public ICommand ChangeItemsSourceCommand { get; }

    public ICommand SegmentChangedCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}
