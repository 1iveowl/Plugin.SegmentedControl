using System.ComponentModel;
using System.Runtime.CompilerServices;
using Plugin.Segmented.Control;

namespace SegCtlr.Tests;

public class SegmentedControlOptionTests
{
    [Fact]
    public void Text_is_read_from_the_item_property()
    {
        var option = new SegmentedControlOption { TextPropertyName = nameof(Person.Name), Item = new Person { Name = "Ada" } };

        Assert.Equal("Ada", option.Text);
    }

    [Fact]
    public void Text_follows_changes_to_the_item_property()
    {
        var person = new Person { Name = "Ada" };
        var option = new SegmentedControlOption { TextPropertyName = nameof(Person.Name), Item = person };

        person.Name = "Grace";

        Assert.Equal("Grace", option.Text);
    }

    [Fact]
    public void Changes_to_other_item_properties_are_ignored()
    {
        var person = new Person { Name = "Ada" };
        // Initializers run in order, so Text overrides the value just read from the item.
        var option = new SegmentedControlOption { TextPropertyName = nameof(Person.Name), Item = person, Text = "Custom" };

        person.Age = 36;

        Assert.Equal("Custom", option.Text);
    }

    [Fact]
    public void Replaced_item_is_no_longer_followed()
    {
        var first = new Person { Name = "Ada" };
        var option = new SegmentedControlOption { TextPropertyName = nameof(Person.Name), Item = first };
        option.Item = new Person { Name = "Grace" };

        first.Name = "Changed";

        Assert.Equal("Grace", option.Text);
    }

    [Fact]
    public void Text_is_left_alone_without_a_text_property_name()
    {
        var option = new SegmentedControlOption { Text = "Fixed", Item = new Person { Name = "Ada" } };

        Assert.Equal("Fixed", option.Text);
    }

    [Fact]
    public void Items_source_with_text_property_name_uses_the_item_text()
    {
        var control = new SegmentedControl
        {
            TextPropertyName = nameof(Person.Name),
            ItemsSource = new List<Person> { new() { Name = "Ada" }, new() { Name = "Grace" } },
        };

        Assert.Equal(["Ada", "Grace"], control.Children.Select(c => c.Text));
    }

    [Fact]
    public void Option_can_be_collected_while_its_item_lives_on()
    {
        // Review finding 2.1: the item's PropertyChanged must not keep discarded options alive.
        var person = new Person { Name = "Ada" };

        var option = CreateOptionFor(person);
        ItemsSourceTests.GcCollect();

        Assert.False(option.TryGetTarget(out _));
        person.Name = "Grace"; // Raising after collection drops the dead subscription without throwing.
    }

    [Fact]
    public void Text_refreshes_when_the_item_reports_all_properties_changed()
    {
        var person = new Person { Name = "Ada" };
        var option = new SegmentedControlOption { TextPropertyName = nameof(Person.Name), Item = person };

        person.SetNameSilently("Grace");
        person.RaiseAllChanged();

        Assert.Equal("Grace", option.Text);
    }

    private static WeakReference<SegmentedControlOption> CreateOptionFor(Person person) =>
        new(new SegmentedControlOption { TextPropertyName = nameof(Person.Name), Item = person });

    private sealed class Person : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string? Name
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int Age
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        private bool _muted;

        public void SetNameSilently(string name)
        {
            _muted = true;
            Name = name;
            _muted = false;
        }

        public void RaiseAllChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            if (!_muted)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
