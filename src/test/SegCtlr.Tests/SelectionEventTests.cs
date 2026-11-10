using Plugin.Segmented.Control;
using Xamarin.Forms;

namespace SegCtlr.Tests;

// Review finding 1.8: SegmentedControl raises its own selection event and command, once per change.
public class SelectionEventTests
{
    [Fact]
    public void Changing_selected_segment_raises_event_once_with_the_new_value()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B", "C" } };
        var raised = new List<int>();
        control.OnSegmentSelected += (_, e) => raised.Add(e.NewValue);

        control.SelectedSegment = 2;

        Assert.Equal(new[] { 2 }, raised);
    }

    [Fact]
    public void Changing_selected_segment_executes_the_command_with_its_parameter()
    {
        var parameters = new List<object>();
        var control = new SegmentedControl
        {
            ItemsSource = new[] { "A", "B" },
            SegmentSelectedCommand = new Command(p => parameters.Add(p)),
            SegmentSelectedCommandParameter = "param",
            SelectedSegment = 1
        };

        Assert.Equal(new object[] { "param" }, parameters);
    }

    [Fact]
    public void Command_that_cannot_execute_is_not_executed()
    {
        var executed = false;
        var control = new SegmentedControl
        {
            ItemsSource = new[] { "A", "B" },
            SegmentSelectedCommand = new Command(() => executed = true, () => false),
            SelectedSegment = 1
        };

        Assert.False(executed);
    }

    [Fact]
    public void Setting_the_same_segment_again_does_not_raise()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B" }, SelectedSegment = 1 };
        var count = 0;
        control.OnSegmentSelected += (_, _) => count++;

        control.SelectedSegment = 1;

        Assert.Equal(0, count);
    }

    [Fact]
    public void Setting_selected_item_raises_once()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B", "C" } };
        var raised = new List<int>();
        control.OnSegmentSelected += (_, e) => raised.Add(e.NewValue);

        control.SelectedItem = "B";

        Assert.Equal(new[] { 1 }, raised);
    }

    [Fact]
    public void Building_the_control_does_not_raise()
    {
        var count = 0;
        var control = new SegmentedControl();
        control.OnSegmentSelected += (_, _) => count++;
        control.SegmentSelectedCommand = new Command(() => count++);

        control.ItemsSource = new[] { "A", "B" };
        control.TextColor = Color.Red;

        Assert.Equal(0, count);
    }
}
