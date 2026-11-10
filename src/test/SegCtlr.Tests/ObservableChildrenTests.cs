using System.Collections.ObjectModel;
using System.ComponentModel;
using Plugin.Segmented.Control;
using Plugin.Segmented.Event;

namespace SegCtlr.Tests;

// Review findings 1.5 (Children edits reach the renderers) and 1.6 (segments are logical children).
public class ObservableChildrenTests
{
    [Fact]
    public void Default_children_list_is_observable()
    {
        Assert.IsType<ObservableCollection<SegmentedControlOption>>(new SegmentedControl().Children);
    }

    [Fact]
    public void Added_segment_gets_the_control_as_parent_and_its_binding_context()
    {
        var context = new object();
        var control = new SegmentedControl { BindingContext = context };
        var option = new SegmentedControlOption { Text = "A" };

        control.Children.Add(option);

        Assert.Same(control, option.Parent);
        Assert.Same(context, option.BindingContext);
    }

    [Fact]
    public void Removed_segment_loses_its_parent()
    {
        var option = new SegmentedControlOption { Text = "A" };
        var control = new SegmentedControl();
        control.Children.Add(option);

        control.Children.Remove(option);

        Assert.Null(option.Parent);
    }

    [Fact]
    public void Segments_of_a_replaced_list_lose_their_parent()
    {
        var old = new SegmentedControlOption { Text = "A" };
        var control = new SegmentedControl { Children = [old] };

        control.Children = [new SegmentedControlOption { Text = "X" }];

        Assert.Null(old.Parent);
        Assert.Same(control, control.Children[0].Parent);
    }

    [Fact]
    public void Editing_children_notifies_renderers_like_a_replaced_list()
    {
        var a = new SegmentedControlOption { Text = "A" };
        var control = new SegmentedControl();
        control.Children.Add(a);
        var events = new List<string>();
        ElementChildrenChanging? changing = null;
        control.OnElementChildrenChanging += (_, e) =>
        {
            changing = e;
            events.Add("changing");
        };
        ((INotifyPropertyChanged)control).PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SegmentedControl.Children))
            {
                events.Add("changed");
            }
        };

        control.Children.Add(new SegmentedControlOption { Text = "B" });

        Assert.Equal(["changing", "changed"], events);
        Assert.Equal([a], changing!.OldValues);
        Assert.Equal(2, changing.NewValues.Count);
    }

    [Fact]
    public void Segments_added_one_at_a_time_keep_the_selected_segment()
    {
        // XAML sets SelectedSegment before it adds the segments.
        var control = new SegmentedControl { SelectedSegment = 2 };

        control.Children.Add(new SegmentedControlOption { Text = "A", Item = "a" });
        control.Children.Add(new SegmentedControlOption { Text = "B", Item = "b" });
        control.Children.Add(new SegmentedControlOption { Text = "C", Item = "c" });

        Assert.Equal(2, control.SelectedSegment);
        Assert.Equal("c", control.SelectedItem);
    }

    [Fact]
    public void Selected_item_set_before_segments_are_added_selects_its_segment()
    {
        var control = new SegmentedControl { SelectedItem = "b" };

        control.Children.Add(new SegmentedControlOption { Text = "A", Item = "a" });
        control.Children.Add(new SegmentedControlOption { Text = "B", Item = "b" });

        Assert.Equal(1, control.SelectedSegment);
    }

    [Fact]
    public void Removing_the_selected_segment_takes_the_selected_item_from_the_new_selection()
    {
        var control = new SegmentedControl();
        control.Children.Add(new SegmentedControlOption { Text = "A", Item = "a" });
        control.Children.Add(new SegmentedControlOption { Text = "B", Item = "b" });
        control.SelectedItem = "a";

        control.Children.RemoveAt(0);

        Assert.Equal(0, control.SelectedSegment);
        Assert.Equal("b", control.SelectedItem);
    }
}
