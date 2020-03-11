using System;

namespace Plugin.Segmented.Event
{
    [Preserve(AllMembers = true)]
    public class SegmentSelectEventArgs : EventArgs
    {
        public int NewValue { get; set; }
    }
}
