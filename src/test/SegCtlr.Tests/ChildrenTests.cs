using Plugin.Segmented.Control;
using Plugin.Segmented.Event;

namespace SegCtlr.Tests;

// Review finding 1.2: replacing Children must not modify the lists the caller passed in.
public class ChildrenTests
{
    [Fact]
    public void Replacing_children_does_not_modify_the_previous_list()
    {
        var a = new SegmentedControlOption { Text = "A" };
        var b = new SegmentedControlOption { Text = "B" };
        var first = new List<SegmentedControlOption> { a, b };
        var second = new List<SegmentedControlOption> { new() { Text = "X" } };
        var control = new SegmentedControl { Children = first };

        control.Children = second;

        Assert.Equal(new[] { a, b }, first);
        Assert.Same(second, control.Children);
        Assert.Single(second);
    }

    [Fact]
    public void Children_changing_event_reports_the_old_and_new_lists_unchanged()
    {
        var old = new List<SegmentedControlOption> { new() { Text = "A" } };
        var replacement = new List<SegmentedControlOption> { new() { Text = "X" }, new() { Text = "Y" } };
        var control = new SegmentedControl { Children = old };
        ElementChildrenChanging? args = null;
        control.OnElementChildrenChanging += (_, e) => args = e;

        control.Children = replacement;

        Assert.NotNull(args);
        Assert.Same(old, args!.OldValues);
        Assert.Same(replacement, args.NewValues);
        Assert.Equal("A", Assert.Single(args.OldValues).Text);
    }

    [Fact]
    public void New_children_get_the_control_binding_context()
    {
        var context = new object();
        var control = new SegmentedControl { BindingContext = context };
        var option = new SegmentedControlOption { Text = "A" };

        control.Children = [option];

        Assert.Same(context, option.BindingContext);
    }

    [Fact]
    public void Binding_context_change_flows_to_children()
    {
        var option = new SegmentedControlOption { Text = "A" };
        var control = new SegmentedControl { Children = [option] };
        var context = new object();

        control.BindingContext = context;

        Assert.Same(context, option.BindingContext);
    }

    [Fact]
    public void Setting_children_to_null_does_not_throw()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A" } };

        var exception = Record.Exception(() => control.Children = null);

        Assert.Null(exception);
    }

    [Fact]
    public void Items_source_produces_a_fresh_children_list_each_time()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B" } };
        var firstChildren = control.Children;

        control.ItemsSource = new[] { "X" };

        Assert.NotSame(firstChildren, control.Children);
        Assert.Equal(["A", "B"], firstChildren.Select(c => c.Text));
        Assert.Equal("X", Assert.Single(control.Children).Text);
    }
}
