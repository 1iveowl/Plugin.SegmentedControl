using System;

namespace Plugin.Segmented;

/// <summary>
/// Tells the Xamarin linker not to remove the marked type or member. The linker recognises any attribute
/// with this name, so this copy avoids a dependency on a platform assembly.
/// </summary>
public sealed class PreserveAttribute : Attribute
{
    /// <summary>Also keep every member of the marked type.</summary>
    public bool AllMembers;

    /// <summary>Keep the marked member only if its declaring type is kept.</summary>
    public bool Conditional;
}
