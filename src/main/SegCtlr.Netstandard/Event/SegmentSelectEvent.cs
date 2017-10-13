using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.SegmentedControl.Netstandard.Event
{
    public class SegmentSelectEventArgs : EventArgs
    {
        public int NewValue { get; set; }
    }
}
