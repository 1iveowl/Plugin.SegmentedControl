using System.Collections.ObjectModel;
using Plugin.Segmented.Control;

namespace SegCtlr.Tests;

// Review findings 1.4 (null / non-string ItemsSource) and 1.5 (collection changes).
public class ItemsSourceTests
{
    [Fact]
    public void Setting_items_source_to_null_removes_the_segments()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B" } };

        control.ItemsSource = null;

        Assert.Empty(control.Children);
    }

    [Fact]
    public void Text_property_name_change_without_items_source_keeps_xaml_children()
    {
        var control = new SegmentedControl();
        control.Children.Add(new SegmentedControlOption { Text = "A" });

        control.TextPropertyName = "Name";

        Assert.Equal("A", Assert.Single(control.Children).Text);
    }

    [Fact]
    public void Non_string_items_without_text_property_name_use_to_string()
    {
        var control = new SegmentedControl { ItemsSource = new object[] { 1, 2.5m } };

        Assert.Equal(["1", "2.5"], control.Children.Select(c => c.Text));
        Assert.Equal(1, control.Children[0].Item);
    }

    [Fact]
    public void Empty_items_source_produces_no_segments()
    {
        var control = new SegmentedControl { ItemsSource = Array.Empty<object>() };

        Assert.Empty(control.Children);
    }

    [Fact]
    public void Lazy_sequence_is_enumerated()
    {
        var control = new SegmentedControl { ItemsSource = Enumerable.Range(1, 3).Select(i => $"#{i}") };

        Assert.Equal(["#1", "#2", "#3"], control.Children.Select(c => c.Text));
    }

    [Fact]
    public void Observable_items_source_additions_and_removals_update_the_segments()
    {
        var items = new ObservableCollection<string> { "A", "B" };
        var control = new SegmentedControl { ItemsSource = items };

        items.Add("C");
        items.Remove("A");

        Assert.Equal(["B", "C"], control.Children.Select(c => c.Text));
    }

    [Fact]
    public void Observable_items_source_change_keeps_the_selected_item()
    {
        var items = new ObservableCollection<string> { "A", "B" };
        var control = new SegmentedControl { ItemsSource = items, SelectedItem = "B" };

        items.Insert(0, "Z");

        Assert.Equal(2, control.SelectedSegment);
        Assert.Equal("B", control.SelectedItem);
    }

    [Fact]
    public void Replaced_observable_items_source_is_no_longer_followed()
    {
        var first = new ObservableCollection<string> { "A" };
        var control = new SegmentedControl { ItemsSource = first };
        control.ItemsSource = new[] { "X" };

        first.Add("B");

        Assert.Equal("X", Assert.Single(control.Children).Text);
    }

    [Fact]
    public void Control_can_be_collected_while_its_observable_items_source_lives_on()
    {
        var items = new ObservableCollection<string> { "A" };

        var control = CreateControlBoundTo(items);
        GcCollect();

        Assert.False(control.TryGetTarget(out _));
        items.Add("B"); // Raising after collection drops the dead subscription without throwing.
    }

    private static WeakReference<SegmentedControl> CreateControlBoundTo(ObservableCollection<string> items) =>
        new(new SegmentedControl { ItemsSource = items });

    internal static void GcCollect()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
