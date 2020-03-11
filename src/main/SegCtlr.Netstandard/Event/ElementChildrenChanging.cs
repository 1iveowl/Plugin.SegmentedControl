using System;
using System.Collections.Generic;
using Plugin.Segmented.Control;

namespace Plugin.Segmented.Event
{

    [Preserve(AllMembers = true)]
    public class ElementChildrenChanging : EventArgs
    {
        public ElementChildrenChanging(IList<SegmentedControlOption> oldValues, IList<SegmentedControlOption> newValues)
        {
            OldValues = oldValues;
            NewValues = newValues;
        }
        public IList<SegmentedControlOption> OldValues { get; }
        public IList<SegmentedControlOption> NewValues { get; }
    }
}
