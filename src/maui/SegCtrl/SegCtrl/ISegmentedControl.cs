using System.Collections;
using System.Windows.Input;

namespace SegCtrl
{
    public interface ISegmentedControl : IView
    {

        public event EventHandler<ElementChildrenChanging> OnElementChildrenChanging;
        public event EventHandler<SegmentSelectEventArgs> OnSegmentSelected;

        public Color BorderColor { get; set; }
        public double BorderWidth { get; set; }
        public IList<SegmentedControlOption> Children { get; set; }
        public Color DisabledColor { get; set; }
        public string FontFamily { get; set; }
        public double FontSize { get; set; }
        public IEnumerable ItemsSource { get; set; }
        public ICommand SegmentSelectedCommand { get; set; }
        public object SegmentSelectedCommandParameter { get; set; }
        public int SelectedSegment { get; set; }
        public Color SelectedTextColor { get; set; }
        public Color TextColor { get; set; }
        public string TextPropertyName { get; set; }
        public Color TintColor { get; set; }

        public SegmentedControlOption SelectedItem { get; set; }

        void RaiseSelectionChanged();
        void Completed();
    }
}