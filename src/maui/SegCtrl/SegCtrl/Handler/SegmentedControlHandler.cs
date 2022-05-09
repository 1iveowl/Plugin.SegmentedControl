#nullable enable
using Microsoft.Maui.Handlers;

namespace SegCtrl.Handlers
{
    public partial class SegmentedControlHandler
    {
        public static IPropertyMapper<ISegmentedControl, SegmentedControlHandler> SegmentControlMapper =
            new PropertyMapper<ISegmentedControl, SegmentedControlHandler>(ViewHandler.ViewMapper)
            {
                [nameof(ISegmentedControl.TextColor)] = TextColor,
            };

        public SegmentedControlHandler() : base(SegmentControlMapper)
        {

        }

        public SegmentedControlHandler(IPropertyMapper? mapper = null) : base(mapper ?? SegmentControlMapper)
        {

        }
    }
}
