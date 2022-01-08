using System;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Plugin.Segmented.Control;
using Plugin.Segmented.Control.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using RadioButton = Android.Widget.RadioButton;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]

namespace Plugin.Segmented.Control.Droid
{
    [Preserve(AllMembers = true)]
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, RadioGroup>
    {
        private RadioGroup _nativeControl;
        private RadioButton _nativeRadioButtonControl;
        private Android.Graphics.Color _unselectedItemBackgroundColor = Android.Graphics.Color.Transparent;

        private readonly Context _context;

        public SegmentedControlRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (Control is null)
            {
                // Instantiate the native control and assign it to the Control property with
                // the SetNativeControl method
            }

            if (!(e.OldElement is null))
            {
                // Unsubscribe from event handlers and cleanup any resources

                if (_nativeControl != null)
                {
                    _nativeControl.CheckedChange -= NativeControl_ValueChanged;
                }

                RemoveElementHandlers();
            }

            if (!(e.NewElement is null))
            {
                // Configure the control and subscribe to event handlers
                AddElementHandlers();
            }
        }

        private void AddElementHandlers(bool addChildrenHandlersOnly = false)
        {
            if (!(Element is null))
            {
                if (!addChildrenHandlersOnly)
                {
                    Element.SizeChanged += Element_SizeChanged;
                    Element.OnElementChildrenChanging += OnElementChildrenChanging;
                }

                if (!(Element.Children is null))
                {
                    foreach (var child in Element.Children)
                    {
                        child.PropertyChanged += Segment_PropertyChanged;
                    }
                }
            }
        }


        private void RemoveElementHandlers(bool removeChildrenHandlersOnly = false)
        {
            if (!(Element is null))
            {
                if (!removeChildrenHandlersOnly)
                {
                    Element.SizeChanged -= Element_SizeChanged;
                    Element.OnElementChildrenChanging -= OnElementChildrenChanging;
                }

                if (!(Element.Children is null))
                {
                    foreach (var child in Element.Children)
                    {
                        child.PropertyChanged -= Segment_PropertyChanged;
                    }
                }
            }
        }

        private void Element_SizeChanged(object sender, EventArgs e)
        {
            if (Control is null && !(Element is null))
            {
                var layoutInflater = LayoutInflater.From(_context);

                _nativeControl = (RadioGroup)layoutInflater.Inflate(Resource.Layout.RadioGroup, null);

                SetNativeControlSegments(layoutInflater);

                var option = (RadioButton)_nativeControl.GetChildAt(Element.SelectedSegment);

                if (!(option is null))
                {
                    option.Checked = true;
                }

                _nativeControl.CheckedChange += NativeControl_ValueChanged;

                SetNativeControl(_nativeControl);
            }
        }

        private void Segment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(_nativeControl is null) && !(Element is null) && sender is SegmentedControlOption option)
            {
                var index = Element.Children.IndexOf(option);

                if (_nativeControl.GetChildAt(index) is RadioButton segment)
                {
                    switch (e.PropertyName)
                    {
                        case nameof(SegmentedControlOption.Text):
                            segment.Text = option.Text;
                            break;

                        case nameof(SegmentedControlOption.IsEnabled):
                            segment.Enabled = option.IsEnabled;
                            break;
                    }
                }
            }
        }


        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Renderer":
                    Element_SizeChanged(null, null);
                    Element?.RaiseSelectionChanged();
                    break;

                case nameof(SegmentedControl.SelectedSegment):
                    if (!(_nativeControl is null) && !(Element is null))
                    {
                        if (Element.SelectedSegment < 0)
                        {
                            var layoutInflater = LayoutInflater.From(_context);

                            _nativeControl = (RadioGroup)layoutInflater.Inflate(Resource.Layout.RadioGroup, null);

                            SetNativeControlSegments(layoutInflater);

                            _nativeControl.CheckedChange += NativeControl_ValueChanged;

                            SetNativeControl(_nativeControl);
                        }

                        SetSelectedRadioButton(Element.SelectedSegment);

                        Element.RaiseSelectionChanged();
                    }
                    break;

                case nameof(SegmentedControl.TintColor):
                case nameof(SegmentedControl.IsEnabled):
                case nameof(SegmentedControl.FontSize):
                case nameof(SegmentedControl.FontFamily):
                case nameof(SegmentedControl.TextColor):
                case nameof(SegmentedControl.BorderColor):
                case nameof(SegmentedControl.BorderWidth):
                    OnPropertyChanged();
                    break;

                case nameof(SegmentedControl.SelectedTextColor):
                    if (!(_nativeControl is null) && !(Element is null))
                    {
                        var v = (RadioButton)_nativeControl.GetChildAt(Element.SelectedSegment);

                        v.SetTextColor(Element.SelectedTextColor.ToAndroid());
                    }
                    break;

                case nameof(SegmentedControl.Children):
                    SetNativeControlSegments(LayoutInflater.FromContext(_context));

                    AddElementHandlers(true);
                    break;
            }
        }

        private void SetNativeControlSegments(LayoutInflater layoutInflater)
        {
            if (_nativeControl is null || Element?.Children is null)
            {
                return;
            }

            if (_nativeControl.ChildCount > 0)
            {
                _nativeControl.RemoveAllViews();
            }

            foreach (var (child, i) in Element?.Children.Select((child, i) => (child, i)))
            {
                var radioButton = (RadioButton)layoutInflater.Inflate(Resource.Layout.RadioButton, null);

                if (radioButton is null)
                {
                    return;
                }

                if (child.WidthRequest > 0)
                    radioButton.LayoutParameters = new RadioGroup.LayoutParams(
                        Convert.ToInt32(Math.Round(child.WidthRequest)), 
                        LayoutParams.WrapContent, 0);
                else
                    radioButton.LayoutParameters = new RadioGroup.LayoutParams(0, LayoutParams.WrapContent, 1f);

                radioButton.Text = child.Text;

                if (i == 0)
                {
                    radioButton.SetBackgroundResource(Resource.Drawable.segmented_control_first_background);
                }
                else
                {
                    radioButton.SetBackgroundResource(Resource.Drawable.segmented_control_last_background);
                }

                ConfigureRadioButton(i, radioButton);
                _nativeControl.AddView(radioButton);
            }

            SetSelectedRadioButton(Element.SelectedSegment);
        }

        private void SetSelectedRadioButton(int index)
        {
            if (_nativeControl.GetChildAt(index) is RadioButton radioButton)
            {
                radioButton.Checked = true;
            }
        }


        private void OnPropertyChanged()
        {
            if (_nativeControl is null || Element is null)
            {
                return;
            }

            for (var i = 0; i < Element.Children.Count; i++)
            {
                var radioButton = (RadioButton)_nativeControl.GetChildAt(i);

                ConfigureRadioButton(i, radioButton);
            }
        }

        private void ConfigureRadioButton(int index, RadioButton radioButton)
        {
            if (index == Element.SelectedSegment)
            {
                radioButton.SetTextColor(Element.SelectedTextColor.ToAndroid());

                _nativeRadioButtonControl = radioButton;
            }
            else
            {
                var textColor = Element.IsEnabled 
                    ? Element.TextColor.ToAndroid() 
                    : Element.DisabledColor.ToAndroid();

                radioButton.SetTextColor(textColor);
            }

            radioButton.TextSize = Convert.ToSingle(Element.FontSize);

            var font = Font.OfSize(Element.FontFamily, Element.FontSize).ToTypeface();

            radioButton.SetTypeface(font, TypefaceStyle.Normal);

            var gradientDrawable = (StateListDrawable)radioButton.Background;

            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable?.GetConstantState();

            var children = drawableContainerState?.GetChildren();

            if (!(children is null))
            {
                var selectedShape = children[0] is GradientDrawable drawable
                    ? drawable
                    : (GradientDrawable)((InsetDrawable)children[0]).Drawable;

                var unselectedShape = children[1] is GradientDrawable drawable1
                    ? drawable1
                    : (GradientDrawable)((InsetDrawable)children[1]).Drawable;

                var backgroundColor = Element.IsEnabled ? Element.TintColor.ToAndroid() : Element.DisabledColor.ToAndroid();

                var borderColor = Element.IsEnabled ? Element.BorderColor.ToAndroid() : Element.DisabledColor.ToAndroid();
                var borderWidthInPixel = ConvertDipToPixel(Element.BorderWidth);

                if (!(selectedShape is null))
                {
                    selectedShape.SetStroke(borderWidthInPixel, borderColor);

                    selectedShape.SetColor(backgroundColor);
                }

                if (!(unselectedShape is null))
                {

                    unselectedShape.SetStroke(borderWidthInPixel, borderColor);
                    unselectedShape.SetColor(_unselectedItemBackgroundColor);
                }
            }
            
            radioButton.Enabled = Element.Children[index].IsEnabled;
        }

        private void NativeControl_ValueChanged(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            var rg = (RadioGroup)sender;

            if (rg.CheckedRadioButtonId != -1)
            {
                var id = rg.CheckedRadioButtonId;
                var radioButton = rg.FindViewById(id);
                var radioId = rg.IndexOfChild(radioButton);
                var v = (RadioButton)rg.GetChildAt(radioId);
                var color = Element.IsEnabled ? Element.TextColor.ToAndroid() : Element.DisabledColor.ToAndroid();

                _nativeRadioButtonControl?.SetTextColor(color);

                v.SetTextColor(Element.SelectedTextColor.ToAndroid());

                _nativeRadioButtonControl = v;

                Element.SelectedSegment = radioId;
            }
        }

        private void OnElementChildrenChanging(object sender, EventArgs e)
        {
            RemoveElementHandlers(true);
        }

        private int ConvertDipToPixel(double dip)
        {
            return (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, (float)dip, _context.Resources.DisplayMetrics);
        }

        public override void SetBackgroundColor(Android.Graphics.Color color)
        {
            _unselectedItemBackgroundColor = color;
            OnPropertyChanged();

            base.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

        protected override void Dispose(bool disposing)
        {
            if (!(_nativeControl is null))
            {
                _nativeControl.CheckedChange -= NativeControl_ValueChanged;
            }
                
            if (!(_nativeRadioButtonControl is null))
            {
                _nativeRadioButtonControl.Dispose();
                _nativeRadioButtonControl = null;
            }

            RemoveElementHandlers();

            try
            {
                base.Dispose(disposing);
                _nativeControl = null;
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init()
        {
            var temp = DateTime.Now;
        }
    }
}