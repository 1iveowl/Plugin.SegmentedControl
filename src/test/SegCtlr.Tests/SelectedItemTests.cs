using Plugin.Segmented.Control;

namespace SegCtlr.Tests;

// Review finding 1.1 (SelectedItem ignored without TextPropertyName) and 1.3 (reference equality).
public class SelectedItemTests
{
    [Fact]
    public void String_items_source_sets_selected_item_from_selected_segment()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B", "C" } };

        Assert.Equal(0, control.SelectedSegment);
        Assert.Equal("A", control.SelectedItem);
    }

    [Fact]
    public void String_items_source_selected_item_moves_selected_segment()
    {
        var control = new SegmentedControl
        {
            ItemsSource = new[] { "A", "B", "C" },
            SelectedItem = "C"
        };

        Assert.Equal(2, control.SelectedSegment);
    }

    [Fact]
    public void Selected_item_set_before_items_source_is_applied_when_items_arrive()
    {
        var control = new SegmentedControl
        {
            SelectedItem = "B",
            ItemsSource = new[] { "A", "B", "C" }
        };

        Assert.Equal(1, control.SelectedSegment);
        Assert.Equal("B", control.SelectedItem);
    }

    [Fact]
    public void Selected_segment_change_updates_selected_item()
    {
        var control = new SegmentedControl
        {
            ItemsSource = new[] { "A", "B", "C" },
            SelectedSegment = 1
        };

        Assert.Equal("B", control.SelectedItem);
    }

    [Fact]
    public void Equal_string_that_is_a_different_instance_is_found()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B", "C" } };
        var differentInstance = new string(['B']);
        Assert.False(ReferenceEquals("B", differentInstance));

        control.SelectedItem = differentInstance;

        Assert.Equal(1, control.SelectedSegment);
    }

    [Fact]
    public void Boxed_value_items_are_matched_by_value()
    {
        var control = new SegmentedControl
        {
            Children =
            [
                new() { Text = "One", Item = 1 },
                new() { Text = "Two", Item = 2 },
                new() { Text = "Three", Item = 3 },
            ],
            SelectedItem = 3
        };

        Assert.Equal(2, control.SelectedSegment);
    }

    [Fact]
    public void Object_items_with_text_property_name_are_matched_with_equals()
    {
        var control = new SegmentedControl
        {
            TextPropertyName = nameof(Fruit.Name),
            ItemsSource = new[] { new Fruit("Apple"), new Fruit("Pear") },
            SelectedItem = new Fruit("Pear")
        };

        Assert.Equal(1, control.SelectedSegment);
        Assert.Equal("Pear", control.Children[1].Text);
    }

    [Fact]
    public void Selected_item_not_in_the_list_leaves_selected_segment_alone()
    {
        var control = new SegmentedControl
        {
            ItemsSource = new[] { "A", "B", "C" },
            SelectedSegment = 2,
            SelectedItem = "Z"
        };

        Assert.Equal(2, control.SelectedSegment);
    }

    [Fact]
    public void Children_without_items_keep_the_selected_segment()
    {
        // XAML-style options have no Item; a null SelectedItem must not match them and reset the segment to 0.
        var control = new SegmentedControl
        {
            SelectedSegment = 2,
            Children =
            [
                new() { Text = "A" },
                new() { Text = "B" },
                new() { Text = "C" },
            ]
        };

        Assert.Equal(2, control.SelectedSegment);
    }

    [Fact]
    public void Out_of_range_selected_segment_resets_to_first_item_when_items_shrink()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B", "C", "D" }, SelectedSegment = 3 };

        control.ItemsSource = new[] { "X", "Y" };

        Assert.Equal(0, control.SelectedSegment);
        Assert.Equal("X", control.SelectedItem);
    }

    [Fact]
    public void Replacing_items_source_keeps_selected_item_when_it_still_exists()
    {
        var control = new SegmentedControl { ItemsSource = new[] { "A", "B", "C" }, SelectedSegment = 1 };

        control.ItemsSource = new[] { "Z", "A", "B" };

        Assert.Equal("B", control.SelectedItem);
        Assert.Equal(2, control.SelectedSegment);
    }

    private sealed record Fruit(string Name);
}
