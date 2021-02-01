using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Plugin.Segmented.Event;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Plugin.Segmented.Control
{
    [DesignTimeVisible(true)]
    [Preserve(AllMembers = true)]
    [ContentProperty(nameof(Children))]
    public class SegmentedControl : View, IViewContainer<SegmentedControlOption>
    {
        public SegmentedControl()
        {
            Children = new List<SegmentedControlOption>();
        }

        public event EventHandler<ElementChildrenChanging> OnElementChildrenChanging;

        public event EventHandler<SegmentSelectEventArgs> OnSegmentSelected;

        #region Children
        public static readonly BindableProperty ChildrenProperty = BindableProperty.Create(nameof(Children), typeof(IList<SegmentedControlOption>), typeof(SegmentedControl), default(IList<SegmentedControlOption>), propertyChanging: OnChildrenChanging);
        private static void OnChildrenChanging(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SegmentedControl segmentedControl 
                && newValue is IList<SegmentedControlOption> newItemsList
                && segmentedControl.Children != null)
            {
                segmentedControl.OnElementChildrenChanging?.Invoke(segmentedControl, new ElementChildrenChanging((IList<SegmentedControlOption>)oldValue, newItemsList));
                segmentedControl.Children.Clear();

                foreach (var newSegment in newItemsList)
                {
                    newSegment.BindingContext = segmentedControl.BindingContext;
                    segmentedControl.Children.Add(newSegment);
                }
            }
        }
        public IList<SegmentedControlOption> Children
        {
            get => (IList<SegmentedControlOption>)GetValue(ChildrenProperty);
            set => SetValue(ChildrenProperty, value);
        }
        #endregion

        #region ItemsSource
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(SegmentedControl));
        public static readonly BindableProperty TextPropertyNameProperty = BindableProperty.Create(nameof(TextPropertyName), typeof(string), typeof(SegmentedControl));
        
        private void OnItemsSourceChanged()
        {
            var itemsSource = ItemsSource;
            var items = itemsSource as IList;
            if (items == null && itemsSource is IEnumerable list)
                items = list.Cast<object>().ToList();

            if (items != null)
            {
                var textValues = items as IEnumerable<string>;
                if (textValues == null && items.Count > 0 && items[0] is string)
                    textValues = items.Cast<string>();

                if (textValues != null)
                {
                    Children = new List<SegmentedControlOption>(textValues.Select(child => new SegmentedControlOption {Text = child}));
                    OnSelectedItemChanged(true);
                }
                else
                {
                    var textPropertyName = TextPropertyName;
                    if (textPropertyName != null)
                    {
                        var newChildren = new List<SegmentedControlOption>();
                        foreach (var item in items)
                            newChildren.Add(new SegmentedControlOption { Item = item, TextPropertyName = textPropertyName });
                        Children = newChildren;
                        OnSelectedItemChanged(true);
                    }
                }
            }
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(ItemsSource) || propertyName == nameof(TextPropertyName))
                OnItemsSourceChanged();
            else if(propertyName == nameof(SelectedItem))
                OnSelectedItemChanged();
            else if(propertyName == nameof(SelectedSegment))
                OnSelectedSegmentChanged();
        }

        private void OnSelectedSegmentChanged()
        {
            var segmentIndex = SelectedSegment;
            if (segmentIndex >= 0 && segmentIndex < Children.Count && SelectedItem != Children[segmentIndex].Item)
                SelectedItem = Children[segmentIndex].Item;
        }

        private void OnSelectedItemChanged(bool forceUpdateSelectedSegment = false)
        {
            if (TextPropertyName != null)
            {
                var selectedItem = SelectedItem;
                var selectedIndex = Children.IndexOf(item => item.Item == selectedItem);
                if (selectedIndex == -1)
                {
                    selectedIndex = SelectedSegment;
                    if (selectedIndex < 0 || selectedIndex >= Children.Count)
                        SelectedSegment = 0;
                    else if(SelectedSegment != selectedIndex)
                        SelectedSegment = selectedIndex;
                    else if(forceUpdateSelectedSegment)
                        OnSelectedSegmentChanged();
                }
                else if (selectedIndex != SelectedSegment)
                    SelectedSegment = selectedIndex;
            }
        }

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public string TextPropertyName
        {
            get => (string)GetValue(TextPropertyNameProperty);
            set => SetValue(TextPropertyNameProperty, value);
        }
        #endregion

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: "TextColor",
            returnType: typeof(Color),
            declaringType: typeof(SegmentedControl),
            defaultValue: default(Color));

        public Color TextColor
        {
            get => (Color) GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty TintColorProperty = BindableProperty.Create(nameof(TintColor), typeof(Color), typeof(SegmentedControl), Color.Blue);

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }

        public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(nameof(SelectedTextColor), typeof(Color), typeof(SegmentedControl), Color.White);

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }

        public static readonly BindableProperty DisabledColorProperty = BindableProperty.Create(nameof(DisabledColor), typeof(Color), typeof(SegmentedControl), Color.Gray);

        public Color DisabledColor
        {
            get => (Color)GetValue(DisabledColorProperty);
            set => SetValue(DisabledColorProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(SegmentedControl), defaultValueCreator: bindable => ((SegmentedControl)bindable).TintColor);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(SegmentedControl), defaultValueCreator: _ => Device.RuntimePlatform == Device.Android ? 1.0 : 0.0);

        public double BorderWidth
        {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create(nameof(SelectedSegment), typeof(int), typeof(SegmentedControl), 0);
        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(SegmentedControl), defaultBindingMode: BindingMode.TwoWay);

        public int SelectedSegment
        {
            get => (int)GetValue(SelectedSegmentProperty);
            set => SetValue(SelectedSegmentProperty, value);
        }

        public object SelectedItem
        {
            get => (object)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
       

        public static readonly BindableProperty SegmentSelectedCommandProperty = BindableProperty.Create(nameof(SegmentSelectedCommand), typeof(ICommand), typeof(SegmentedControl));
        public ICommand SegmentSelectedCommand
        {
            get => (ICommand)GetValue(SegmentSelectedCommandProperty);
            set => SetValue(SegmentSelectedCommandProperty, value);
        }

        public static readonly BindableProperty SegmentSelectedCommandParameterProperty = BindableProperty.Create(nameof(SegmentSelectedCommandParameter), typeof(object), typeof(SegmentedControl));


        public object SegmentSelectedCommandParameter
        {
            get => GetValue(SegmentSelectedCommandParameterProperty);
            set => SetValue(SegmentSelectedCommandParameterProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SegmentedControl), Device.GetNamedSize(NamedSize.Medium, typeof(Label)));
        [Xamarin.Forms.TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(SegmentedControl));
        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }


        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseSelectionChanged()
        {
            OnSegmentSelected?.Invoke(this, new SegmentSelectEventArgs { NewValue = this.SelectedSegment });

            if (!(SegmentSelectedCommand is null) && SegmentSelectedCommand.CanExecute(SegmentSelectedCommandParameter))
            {
                SegmentSelectedCommand.Execute(SegmentSelectedCommandParameter);
            }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (!(Children is null))
            {
                foreach (var segment in Children)
                {
                    segment.BindingContext = BindingContext;
                }
            }
        }
    }
}
