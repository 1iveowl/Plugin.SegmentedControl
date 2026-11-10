using Plugin.Segmented.Control;

namespace SegCtlr.Tests;

// Review finding 1.7: the type must load without Xamarin.Forms.Forms.Init(); this test host has no platform services.
public class DefaultsTests
{
    [Fact]
    public void Control_can_be_created_and_used_without_forms_init()
    {
        var exception = Record.Exception(() => new SegmentedControl { ItemsSource = new[] { "A" }, SelectedSegment = 0 });

        Assert.Null(exception);
    }
}
