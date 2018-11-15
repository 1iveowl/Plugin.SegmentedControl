using System;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Plugin.Segmented.Control;
using Plugin.Segmented.Control.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]

namespace Plugin.Segmented.Control.Droid
{
    public class SegmentedControlRenderer : ViewRenderer<SegmentedControl, RadioGroup>
    {
        private RadioGroup _nativeControl;
        private RadioButton _nativeRadioButtonControl;

        private readonly Context _context;

        public SegmentedControlRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<SegmentedControl> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                // Instantiate the native control and assign it to the Control property with
                // the SetNativeControl method
            }

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources

                if (_nativeControl != null)
                    _nativeControl.CheckedChange -= NativeControl_ValueChanged;
                RemoveElementHandlers();
            }

            if (e.NewElement != null)
            {
                // Configure the control and subscribe to event handlers
                AddElementHandlers();
            }
        }

        private void AddElementHandlers(bool addChildrenHandlersOnly = false)
        {
            if (Element != null)
            {
                if (!addChildrenHandlersOnly)
                {
                    Element.SizeChanged += Element_SizeChanged;
                    Element.OnElementChildrenChanging += OnElementChildrenChanging;
                }
                if (Element.Children != null)
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
            if (Element != null)
            {
                if (!removeChildrenHandlersOnly)
                {
                    Element.SizeChanged -= Element_SizeChanged;
                    Element.OnElementChildrenChanging -= OnElementChildrenChanging;
                }
                if (Element.Children != null)
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
            if (Control == null && Element != null)
            {
                var layoutInflater = LayoutInflater.From(_context);

                _nativeControl = (RadioGroup)layoutInflater.Inflate(Resource.Layout.RadioGroup, null);

                SetNativeControlSegments(layoutInflater);

                var option = (RadioButton)_nativeControl.GetChildAt(Element.SelectedSegment);

                if (option != null)
                    option.Checked = true;

                _nativeControl.CheckedChange += NativeControl_ValueChanged;

                SetNativeControl(_nativeControl);
            }
        }

        private void Segment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_nativeControl != null && Element != null && sender is SegmentedControlOption option)
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
                    if (_nativeControl != null && Element != null)
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
                    OnPropertyChanged();
                    break;
                case nameof(SegmentedControl.IsEnabled):
                    OnPropertyChanged();
                    break;
                case nameof(SegmentedControl.SelectedTextColor):
                    if (_nativeControl != null && Element != null)
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
            if (_nativeControl is null || Element is null || Element.Children is null) return;
            if (_nativeControl.ChildCount > 0)
            {
                _nativeControl.RemoveAllViews();
            }
            for (var i = 0; i < Element.Children.Count; i++)
            {
                var o = Element.Children[i];
                var v = (RadioButton)layoutInflater.Inflate(Resource.Layout.RadioButton, null);

                v.LayoutParameters = new RadioGroup.LayoutParams(0, LayoutParams.WrapContent, 1f);
                v.Text = o.Text;

                if (i == 0)
                    v.SetBackgroundResource(Resource.Drawable.segmented_control_first_background);
                else if (i == Element.Children.Count - 1)
                    v.SetBackgroundResource(Resource.Drawable.segmented_control_last_background);

                ConfigureRadioButton(i, v);

                _nativeControl.AddView(v);
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
            if (_nativeControl == null || Element == null) return;

            for (var i = 0; i < Element.Children.Count; i++)
            {
                var v = (RadioButton)_nativeControl.GetChildAt(i);

                ConfigureRadioButton(i, v);
            }
        }

        private void ConfigureRadioButton(int index, RadioButton v)
        {
            if (index == Element.SelectedSegment)
            {
                v.SetTextColor(Element.SelectedTextColor.ToAndroid());
                _nativeRadioButtonControl = v;
            }
            else
            {
                var textColor = Element.IsEnabled ? Element.TintColor.ToAndroid() : Element.DisabledColor.ToAndroid();
                v.SetTextColor(textColor);
            }

            var gradientDrawable = (StateListDrawable)v.Background;
            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable.GetConstantState();
            var children = drawableContainerState.GetChildren();

            // Doesnt works on API < 18
            var selectedShape = children[0] is GradientDrawable drawable 
                ? drawable 
                : (GradientDrawable)((InsetDrawable)children[0]).Drawable;

            var unselectedShape = children[1] is GradientDrawable drawable1 
                ? drawable1 
                : (GradientDrawable)((InsetDrawable)children[1]).Drawable;

            var color = Element.IsEnabled ? Element.TintColor.ToAndroid() : Element.DisabledColor.ToAndroid();

            selectedShape.SetStroke(3, color);
            selectedShape.SetColor(color);
            unselectedShape.SetStroke(3, color);

            v.Enabled = Element.IsEnabled;
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

                var color = Element.IsEnabled ? Element.TintColor.ToAndroid() : Element.DisabledColor.ToAndroid();
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

        protected override void Dispose(bool disposing)
        {
            if (_nativeControl != null)
                _nativeControl.CheckedChange -= NativeControl_ValueChanged;
            
            if (_nativeRadioButtonControl != null)
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