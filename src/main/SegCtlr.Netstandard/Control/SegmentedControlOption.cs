using System;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms;

namespace Plugin.Segmented.Control;

/// <summary>
/// One segment of a <see cref="SegmentedControl"/>. Declare it in XAML inside the control, or let the control
/// generate it from <see cref="SegmentedControl.ItemsSource"/>.
/// </summary>
/// <remarks>
/// The renderers use <see cref="Text"/>, <see cref="VisualElement.IsEnabled"/> and
/// <see cref="VisualElement.WidthRequest"/> (a value above <c>0</c> fixes the segment width).
/// </remarks>
[Preserve(AllMembers = true)]
public class SegmentedControlOption : View
{
    /// <summary>Identifies the <see cref="Text"/> bindable property.</summary>
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SegmentedControlOption), string.Empty);

    /// <summary>Identifies the <see cref="Item"/> bindable property.</summary>
    public static readonly BindableProperty ItemProperty = BindableProperty.Create(nameof(Item), typeof(object), typeof(SegmentedControlOption), propertyChanged: (bindable, _, newValue) => ((SegmentedControlOption)bindable).OnItemChanged(newValue));

    /// <summary>Identifies the <see cref="TextPropertyName"/> bindable property.</summary>
    public static readonly BindableProperty TextPropertyNameProperty = BindableProperty.Create(nameof(TextPropertyName), typeof(string), typeof(SegmentedControlOption));

    private IDisposable _itemSubscription;
    private PropertyInfo _textProperty;

    /// <summary>
    /// The text shown in the segment. When <see cref="TextPropertyName"/> is set, it is read from
    /// <see cref="Item"/> and overwritten whenever that property changes.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// The value this segment stands for. It is what <see cref="SegmentedControl.SelectedItem"/> returns
    /// while the segment is selected.
    /// </summary>
    public object Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    /// <summary>
    /// The name of a public property on <see cref="Item"/> to show as <see cref="Text"/>. If
    /// <see cref="Item"/> implements <see cref="INotifyPropertyChanged"/>, the text follows that property.
    /// </summary>
    /// <remarks>
    /// The item holds only a weak reference to this option, so an option that is no longer displayed can be
    /// collected even while the item lives on.
    /// </remarks>
    public string TextPropertyName
    {
        get => (string)GetValue(TextPropertyNameProperty);
        set => SetValue(TextPropertyNameProperty, value);
    }

    private void OnItemChanged(object newValue)
    {
        _itemSubscription?.Dispose();
        _itemSubscription = newValue is INotifyPropertyChanged observable
            ? WeakEvents.SubscribePropertyChanged(observable, this, static (option, e) => option.OnItemPropertyChanged(e))
            : null;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName is nameof(Item) or nameof(TextPropertyName))
        {
            SetTextFromItemProperty();
        }
    }

    private void OnItemPropertyChanged(PropertyChangedEventArgs e)
    {
        // An empty or null name means "all properties changed".
        if (string.IsNullOrEmpty(e.PropertyName) || e.PropertyName == TextPropertyName)
        {
            SetTextFromItemProperty();
        }
    }

    private void SetTextFromItemProperty()
    {
        var item = Item;
        var name = TextPropertyName;
        if (item is null || name is null)
        {
            return;
        }

        var type = item.GetType();
        if (_textProperty is null || _textProperty.Name != name || _textProperty.DeclaringType?.IsAssignableFrom(type) != true)
        {
            _textProperty = type.GetProperty(name);
        }

        Text = _textProperty?.GetValue(item)?.ToString();
    }
}
