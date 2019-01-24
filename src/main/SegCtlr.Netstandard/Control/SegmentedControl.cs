using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Plugin.Segmented.Event;
using Xamarin.Forms;

namespace Plugin.Segmented.Control
{
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
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IList<string>), typeof(SegmentedControl), default(IList<string>), propertyChanged: OnItemsSourceChanged);
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is SegmentedControl segmentedControl && newValue is IList<string> textValues)
            {
                var newChildren = new List<SegmentedControlOption>(textValues.Count);
                foreach (var child in textValues)
                {
                    newChildren.Add(new SegmentedControlOption { Text = child });
                }
                segmentedControl.Children = newChildren;
            }
        }
        public IList<string> ItemsSource
        {
            get => (IList<string>)GetValue(ItemsSourceProperty);
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

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


	    public static readonly BindableProperty SelectedSegmentProperty = BindableProperty.Create(nameof(SelectedSegment), typeof(int), typeof(SegmentedControl), 0, propertyChanged:OnSelectedSegmentChanged);

	    private static void OnSelectedSegmentChanged(BindableObject bindable, object oldValue, object newValue)
	    {
            if (bindable is SegmentedControl segmentedControl)
            {
                segmentedControl.RaiseSelectionChanged();
            }
	    }


        public int SelectedSegment
        {
            get => (int)GetValue(SelectedSegmentProperty);
            set => SetValue(SelectedSegmentProperty, value);
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

        //public static readonly BindableProperty FontWeightProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(SegmentedControl), default(FontAttributes));
        //public FontAttributes FontAttributes
        //{
        //    get => (FontAttributes)GetValue(FontWeightProperty);
        //    set => SetValue(FontWeightProperty, value);
        //}


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
