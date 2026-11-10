using System;
using System.Collections.Generic;
using Plugin.Segmented.Control;

namespace Plugin.Segmented.Event;

/// <summary>
/// Data for <see cref="SegmentedControl.OnElementChildrenChanging"/>, raised just before
/// <see cref="SegmentedControl.Children"/> is replaced.
/// </summary>
[Preserve(AllMembers = true)]
public class ElementChildrenChanging : EventArgs
{
    /// <summary>Creates the event data.</summary>
    /// <param name="oldValues">The segments being replaced.</param>
    /// <param name="newValues">The segments replacing them; may be <c>null</c>.</param>
    public ElementChildrenChanging(IList<SegmentedControlOption> oldValues, IList<SegmentedControlOption> newValues)
    {
        OldValues = oldValues;
        NewValues = newValues;
    }
    /// <summary>The segments being replaced. The control does not modify this list.</summary>
    public IList<SegmentedControlOption> OldValues { get; }

    /// <summary>The segments replacing them; may be <c>null</c>.</summary>
    public IList<SegmentedControlOption> NewValues { get; }
}
