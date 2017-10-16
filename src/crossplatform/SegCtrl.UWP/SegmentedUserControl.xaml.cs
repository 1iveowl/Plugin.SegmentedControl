using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Plugin.Segmented.Control.UWP
{
    public sealed partial class SegmentedUserControl : UserControl
    {
        public Grid SegmentedControlGrid => mainGrid;
        public SegmentedUserControl()
        {
            this.InitializeComponent();
        }
    }
}
