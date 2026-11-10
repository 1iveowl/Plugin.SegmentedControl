using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Plugin.Segmented.Event;
using Xamarin.Forms;

namespace Plugin.Segmented.Control;

/// <summary>
/// A row of mutually exclusive segments, rendered with the platform's native segmented control
/// (<c>UISegmentedControl</c> on iOS, a <c>RadioGroup</c> on Android, radio buttons on UWP and
/// <c>NSSegmentedControl</c> on macOS).
/// </summary>
/// <remarks>
/// <para>
/// Segments come either from <see cref="Children"/> (for example declared in XAML) or are generated from
/// <see cref="ItemsSource"/>. The selection is exposed both as an index, <see cref="SelectedSegment"/>, and
/// as the selected segment's <see cref="SegmentedControlOption.Item"/>, <see cref="SelectedItem"/>. The two
/// are kept in sync in both directions; items are compared with <see cref="object.Equals(object, object)"/>.
/// </para>
/// <para>
/// Every change of <see cref="SelectedSegment"/>, whether made by the user or in code, raises
/// <see cref="OnSegmentSelected"/> and executes <see cref="SegmentSelectedCommand"/>. Building the control
/// does not.
/// </para>
/// </remarks>
[DesignTimeVisible(true)]
[Preserve(AllMembers = true)]
[ContentProperty(nameof(Children))]
public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
{
    private IDisposable _childrenSubscription;
    private IDisposable _itemsSourceSubscription;

    // The segments as the renderers last saw them, so OnElementChildrenChanging can report the old segments
    // after an edit to an observable Children list.
    private List<SegmentedControlOption> _childrenSnapshot = [];

    /// <summary>Creates a segmented control with an empty, observable <see cref="Children"/> list.</summary>
    public SegmentedControl()
    {
        Children = new ObservableCollection<SegmentedControlOption>();
    }

    /// <summary>
    /// Raised just before the segments change: before <see cref="Children"/> is replaced with a different list, and
    /// before the renderers are told about an edit to an observable <see cref="Children"/> list.
    /// <see cref="ElementChildrenChanging.OldValues"/> holds the segments as they were; renderers use it to detach
    /// from them.
    /// </summary>
    public event EventHandler<ElementChildrenChanging> OnElementChildrenChanging;

    /// <summary>
    /// Raised after <see cref="SelectedSegment"/> changes, by user interaction or in code. Not raised when the
    /// value is set to what it already is.
    /// </summary>
    public event EventHandler<SegmentSelectEventArgs> OnSegmentSelected;

    #region Children

    /// <summary>Identifies the <see cref="Children"/> bindable property.</summary>
    public static readonly BindableProperty ChildrenProperty = BindableProperty.Create(
        nameof(Children),
        typeof(IList<SegmentedControlOption>),
        typeof(SegmentedControl),
        default(IList<SegmentedControlOption>),
        propertyChanging: OnChildrenChanging,
        propertyChanged: OnChildrenChanged);

    /// <summary>
    /// The segments, in display order. This is the XAML content property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The default list is an <see cref="ObservableCollection{T}"/>, so adding or removing segments at runtime
    /// updates the control. Any list that implements <see cref="INotifyCollectionChanged"/> behaves the same; with
    /// a plain list, assign a new list to change the segments. Setting <see cref="ItemsSource"/> replaces this list.
    /// </para>
    /// <para>
    /// Each segment gets this control as its <see cref="Element.Parent"/> and its
    /// <see cref="BindableObject.BindingContext"/>. The list you assign is never modified by the control.
    /// </para>
    /// </remarks>
    public IList<SegmentedControlOption> Children
    {
        get => (IList<SegmentedControlOption>)GetValue(ChildrenProperty);
        set => SetValue(ChildrenProperty, value);
    }

    private static void OnChildrenChanging(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SegmentedControl)bindable;

        if (oldValue is IList<SegmentedControlOption> oldList)
        {
            control.OnElementChildrenChanging?.Invoke(
                control,
                new ElementChildrenChanging(oldList, newValue as IList<SegmentedControlOption>));
        }
    }

    private static void OnChildrenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SegmentedControl)bindable;
        var newList = newValue as IList<SegmentedControlOption>;

        control._childrenSubscription?.Dispose();
        control._childrenSubscription = newList is INotifyCollectionChanged observable
            ? WeakEvents.SubscribeCollectionChanged(observable, control, static (c, _) => c.OnChildrenCollectionChanged())
            : null;

        control.UpdateLogicalChildren(newList);
        control.SyncSelection(SelectionSync.SegmentsReplaced);
    }

    private void OnChildrenCollectionChanged()
    {
        var children = Children;
        var selectedItemWasPresent = IndexOfItem(_childrenSnapshot, SelectedItem) >= 0;

        // Renderers rebuild their segments when Children is reported as changed, and detach from the old segments
        // in OnElementChildrenChanging. Give them the same sequence as for a replaced list.
        OnElementChildrenChanging?.Invoke(this, new ElementChildrenChanging(_childrenSnapshot, children));
        UpdateLogicalChildren(children);
        OnPropertyChanged(nameof(Children));
        SyncSelection(SelectionSync.SegmentsEdited, selectedItemWasPresent);
    }

    private void UpdateLogicalChildren(IList<SegmentedControlOption> children)
    {
        var current = children is null ? [] : new List<SegmentedControlOption>(children);

        foreach (var removed in _childrenSnapshot)
        {
            if (!current.Contains(removed) && ReferenceEquals(removed.Parent, this))
            {
                removed.Parent = null;
            }
        }

        foreach (var segment in current)
        {
            segment.Parent = this;
            segment.BindingContext = BindingContext;
        }

        _childrenSnapshot = current;
    }

    #endregion

    #region ItemsSource

    /// <summary>Identifies the <see cref="ItemsSource"/> bindable property.</summary>
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(SegmentedControl),
        propertyChanged: (bindable, _, newValue) => ((SegmentedControl)bindable).OnItemsSourceChanged((IEnumerable)newValue));

    /// <summary>Identifies the <see cref="TextPropertyName"/> bindable property.</summary>
    public static readonly BindableProperty TextPropertyNameProperty = BindableProperty.Create(
        nameof(TextPropertyName),
        typeof(string),
        typeof(SegmentedControl),
        propertyChanged: (bindable, _, _) => ((SegmentedControl)bindable).RebuildChildrenFromItemsSource());

    /// <summary>
    /// The items to generate segments from. Each item becomes one <see cref="SegmentedControlOption"/> whose
    /// <see cref="SegmentedControlOption.Item"/> is the item.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A string item is displayed as is. For other items the segment shows the property named by
    /// <see cref="TextPropertyName"/>, or the item's <see cref="object.ToString"/> when no name is set.
    /// </para>
    /// <para>
    /// If the sequence implements <see cref="INotifyCollectionChanged"/> (for example an
    /// <see cref="ObservableCollection{T}"/>), the segments follow its changes. Setting <see cref="ItemsSource"/>
    /// to <c>null</c> removes the generated segments.
    /// </para>
    /// </remarks>
    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// The name of the property on each <see cref="ItemsSource"/> item whose value is shown as the segment
    /// text. If the item implements <see cref="INotifyPropertyChanged"/>, the text follows changes to it.
    /// </summary>
    public string TextPropertyName
    {
        get => (string)GetValue(TextPropertyNameProperty);
        set => SetValue(TextPropertyNameProperty, value);
    }

    private void OnItemsSourceChanged(IEnumerable newValue)
    {
        _itemsSourceSubscription?.Dispose();
        _itemsSourceSubscription = newValue is INotifyCollectionChanged observable
            ? WeakEvents.SubscribeCollectionChanged(observable, this, static (c, _) => c.RebuildChildrenFromItemsSource())
            : null;

        if (newValue is null)
        {
            // ItemsSource was cleared: remove the segments it generated.
            Children = new ObservableCollection<SegmentedControlOption>();
            return;
        }

        RebuildChildrenFromItemsSource();
    }

    private void RebuildChildrenFromItemsSource()
    {
        // Without an ItemsSource the segments come from Children (e.g. XAML); a TextPropertyName change must not
        // touch them.
        var itemsSource = ItemsSource;
        if (itemsSource is null)
        {
            return;
        }

        var textPropertyName = TextPropertyName;
        Children = [.. itemsSource.Cast<object>().Select(item => CreateOption(item, textPropertyName))];
    }

    private static SegmentedControlOption CreateOption(object item, string textPropertyName) => item switch
    {
        string text => new SegmentedControlOption { Text = text, Item = text },
        _ when textPropertyName is not null => new SegmentedControlOption { Item = item, TextPropertyName = textPropertyName },
        _ => new SegmentedControlOption { Text = item?.ToString(), Item = item },
    };

    #endregion

    #region Selection

    /// <summary>Identifies the <see cref="SelectedSegment"/> bindable property.</summary>
    public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create(
        nameof(SelectedSegment),
        typeof(int),
        typeof(SegmentedControl),
        0,
        propertyChanged: (bindable, _, _) => ((SegmentedControl)bindable).OnSelectedSegmentChanged());

    /// <summary>Identifies the <see cref="SelectedItem"/> bindable property. Binds two-way by default.</summary>
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(SegmentedControl),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (bindable, _, _) => ((SegmentedControl)bindable).SyncSelection(SelectionSync.SelectedItemChanged));

    /// <summary>
    /// The zero-based index of the selected segment. Defaults to <c>0</c>. Changing it updates
    /// <see cref="SelectedItem"/>, raises <see cref="OnSegmentSelected"/> and executes
    /// <see cref="SegmentSelectedCommand"/>.
    /// </summary>
    public int SelectedSegment
    {
        get => (int)GetValue(SelectedSegmentProperty);
        set => SetValue(SelectedSegmentProperty, value);
    }

    /// <summary>
    /// The <see cref="SegmentedControlOption.Item"/> of the selected segment. Setting it to an item in
    /// <see cref="Children"/> (compared with <see cref="object.Equals(object, object)"/>) selects that segment;
    /// setting it to an item that is not present leaves the selection unchanged.
    /// </summary>
    /// <remarks>
    /// When the segments are replaced, the selection follows <see cref="SelectedItem"/> if it is still present;
    /// otherwise <see cref="SelectedItem"/> is taken from the segment at <see cref="SelectedSegment"/>, which is
    /// reset to <c>0</c> if it is out of range.
    /// </remarks>
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private void OnSelectedSegmentChanged()
    {
        var children = Children;
        var segmentIndex = SelectedSegment;

        if (children is not null && segmentIndex >= 0 && segmentIndex < children.Count)
        {
            var item = children[segmentIndex].Item;
            if (!Equals(SelectedItem, item))
            {
                SelectedItem = item;
            }
        }

        RaiseSegmentSelected();
    }

    private enum SelectionSync
    {
        /// <summary><see cref="SelectedItem"/> was set.</summary>
        SelectedItemChanged,

        /// <summary><see cref="Children"/> was replaced with a new list.</summary>
        SegmentsReplaced,

        /// <summary>Segments were added to or removed from an observable <see cref="Children"/> list.</summary>
        SegmentsEdited,
    }

    /// <summary>
    /// Keeps <see cref="SelectedSegment"/> and <see cref="SelectedItem"/> in step. A <see cref="SelectedItem"/> found
    /// among the segments always wins. Otherwise, after the segments changed, <see cref="SelectedItem"/> is taken
    /// from the selected segment; a replaced list also resets an out-of-range <see cref="SelectedSegment"/> to 0.
    /// An edited list does not, because XAML adds segments one at a time and the selected one may come later; for
    /// the same reason an edit keeps a <see cref="SelectedItem"/> that has not been added yet, unless it was present
    /// before the edit (it was removed).
    /// </summary>
    private void SyncSelection(SelectionSync reason, bool selectedItemWasPresent = false)
    {
        var children = Children;
        if (children is null || children.Count == 0)
        {
            return;
        }

        var index = IndexOfItem(children, SelectedItem);
        if (index >= 0)
        {
            if (index != SelectedSegment)
            {
                SelectedSegment = index;
            }

            return;
        }

        if (reason == SelectionSync.SelectedItemChanged
            || (reason == SelectionSync.SegmentsEdited && SelectedItem is not null && !selectedItemWasPresent))
        {
            return;
        }

        if (SelectedSegment >= 0 && SelectedSegment < children.Count)
        {
            var item = children[SelectedSegment].Item;
            if (!Equals(SelectedItem, item))
            {
                SelectedItem = item;
            }
        }
        else if (reason == SelectionSync.SegmentsReplaced)
        {
            // The property-changed callback updates SelectedItem from the new segment.
            SelectedSegment = 0;
        }
    }

    private static int IndexOfItem(IList<SegmentedControlOption> children, object item)
    {
        // A null item would match every option without an Item (e.g. declared in XAML); treat it as "no item".
        if (item is null)
        {
            return -1;
        }

        for (var i = 0; i < children.Count; i++)
        {
            if (Equals(children[i].Item, item))
            {
                return i;
            }
        }

        return -1;
    }

    #endregion

    #region Appearance

    /// <summary>Identifies the <see cref="TextColor"/> bindable property.</summary>
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(SegmentedControl),
        default(Color));

    /// <summary>The text color of the segments that are not selected.</summary>
    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    /// <summary>Identifies the <see cref="TintColor"/> bindable property.</summary>
    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SegmentedControl), Color.Blue);

    /// <summary>The background color of the selected segment. Defaults to <see cref="Color.Blue"/>.</summary>
    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    /// <summary>Identifies the <see cref="SelectedTextColor"/> bindable property.</summary>
    public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentedControl), Color.White);

    /// <summary>The text color of the selected segment. Defaults to <see cref="Color.White"/>.</summary>
    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    /// <summary>Identifies the <see cref="DisabledColor"/> bindable property.</summary>
    public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create(nameof(DisabledColor), typeof(Color), typeof(SegmentedControl), Color.Gray);

    /// <summary>
    /// The color used in place of <see cref="TintColor"/>, <see cref="TextColor"/> and
    /// <see cref="BorderColor"/> while the control is disabled. Defaults to <see cref="Color.Gray"/>.
    /// </summary>
    public Color DisabledColor
    {
        get => (Color)GetValue(DisabledColorProperty);
        set => SetValue(DisabledColorProperty, value);
    }

    /// <summary>Identifies the <see cref="BorderColor"/> bindable property.</summary>
    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentedControl), defaultValueCreator: bindable => ((SegmentedControl)bindable).TintColor);

    /// <summary>
    /// The border color (iOS and Android only). If not set, it takes the value of <see cref="TintColor"/> the
    /// first time it is read.
    /// </summary>
    public Color BorderColor
    {
        get => (Color)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    /// <summary>Identifies the <see cref="BorderWidth"/> bindable property.</summary>
    public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(SegmentedControl), defaultValueCreator: _ => Device.RuntimePlatform == Device.Android ? 1.0 : 0.0);

    /// <summary>
    /// The border width in device-independent units (iOS and Android only). Defaults to <c>1</c> on Android
    /// and <c>0</c> elsewhere.
    /// </summary>
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    /// <summary>Identifies the <see cref="FontSize"/> bindable property.</summary>
    // A defaultValueCreator rather than a static default: Device.GetNamedSize needs Forms.Init(), which may not
    // have run when this type is first loaded.
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SegmentedControl), defaultValueCreator: _ => Device.GetNamedSize(NamedSize.Medium, typeof(Label)));

    /// <summary>The segment font size. Defaults to the platform's medium <see cref="Label"/> size.</summary>
    [Xamarin.Forms.TypeConverter(typeof(FontSizeConverter))]
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    /// <summary>Identifies the <see cref="FontFamily"/> bindable property.</summary>
    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SegmentedControl));

    /// <summary>The segment font family. <c>null</c> or empty uses the platform default font.</summary>
    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    #endregion

    #region Command

    /// <summary>Identifies the <see cref="SegmentSelectedCommand"/> bindable property.</summary>
    public static readonly BindableProperty SegmentSelectedCommandProperty = BindableProperty.Create(nameof(SegmentSelectedCommand), typeof(ICommand), typeof(SegmentedControl));

    /// <summary>
    /// Executed with <see cref="SegmentSelectedCommandParameter"/> each time <see cref="SelectedSegment"/>
    /// changes, if <see cref="ICommand.CanExecute(object)"/> returns <c>true</c>.
    /// </summary>
    public ICommand SegmentSelectedCommand
    {
        get => (ICommand)GetValue(SegmentSelectedCommandProperty);
        set => SetValue(SegmentSelectedCommandProperty, value);
    }

    /// <summary>Identifies the <see cref="SegmentSelectedCommandParameter"/> bindable property.</summary>
    public static readonly BindableProperty SegmentSelectedCommandParameterProperty = BindableProperty.Create(nameof(SegmentSelectedCommandParameter), typeof(object), typeof(SegmentedControl));

    /// <summary>The parameter passed to <see cref="SegmentSelectedCommand"/>.</summary>
    public object SegmentSelectedCommandParameter
    {
        get => GetValue(SegmentSelectedCommandParameterProperty);
        set => SetValue(SegmentSelectedCommandParameterProperty, value);
    }

    #endregion

    /// <summary>
    /// Raises <see cref="OnSegmentSelected"/> and executes <see cref="SegmentSelectedCommand"/> for the current
    /// <see cref="SelectedSegment"/>.
    /// </summary>
    /// <remarks>
    /// The control now does this itself whenever <see cref="SelectedSegment"/> changes. Calling it as well
    /// raises the event twice.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("SegmentedControl raises OnSegmentSelected and executes SegmentSelectedCommand itself when SelectedSegment changes. Calling this raises them a second time.")]
    public void RaiseSelectionChanged() => RaiseSegmentSelected();

    private void RaiseSegmentSelected()
    {
        OnSegmentSelected?.Invoke(this, new SegmentSelectEventArgs { NewValue = SelectedSegment });

        var command = SegmentSelectedCommand;
        var parameter = SegmentSelectedCommandParameter;
        if (command is not null && command.CanExecute(parameter))
        {
            command.Execute(parameter);
        }
    }

    /// <summary>Passes the new <see cref="BindableObject.BindingContext"/> on to every segment.</summary>
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (Children is null)
        {
            return;
        }

        foreach (var segment in Children)
        {
            segment.BindingContext = BindingContext;
        }
    }
}
