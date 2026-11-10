using System;

namespace Plugin.Segmented.Event;

/// <summary>Data for <see cref="Control.SegmentedControl.OnSegmentSelected"/>.</summary>
[Preserve(AllMembers = true)]
public class SegmentSelectEventArgs : EventArgs
{
    /// <summary>The newly selected <see cref="Control.SegmentedControl.SelectedSegment"/>.</summary>
    public int NewValue { get; set; }
}
