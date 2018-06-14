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
        private RadioButton _v;

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

                if (Element != null) Element.SizeChanged += Element_SizeChanged;
                foreach (var child in Element.Children)
                {
                    child.PropertyChanged += Segment_PropertyChanged;
                }
            }
        }

        private void RemoveElementHandlers()
        {
            if (Element != null)
            {
                Element.SizeChanged -= Element_SizeChanged;
                foreach (var child in Element.Children)
                {
                    child.PropertyChanged -= Segment_PropertyChanged;
                }
            }
        }

        private void Element_SizeChanged(object sender, EventArgs e)
        {
            if (Control == null && Element != null)
            {
                var layoutInflater = LayoutInflater.From(_context);

                _nativeControl = (RadioGroup)layoutInflater.Inflate(Plugin.Segmented.Control.Droid.Resource.Layout.RadioGroup, null);

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

                var option = (RadioButton)_nativeControl.GetChildAt(Element.SelectedSegment);

                if (option != null)
                    option.Checked = true;

                _nativeControl.CheckedChange += NativeControl_ValueChanged;

                SetNativeControl(_nativeControl);
            }
        }

        void Segment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (_nativeControl != null && Element != null && sender is SegmentedControlOption option && e.PropertyName == nameof(option.Text))
            {
                var index = Element.Children.IndexOf(option);
                if (_nativeControl.GetChildAt(index) is RadioButton segment)
                    segment.Text = Element.Children[index].Text;
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
                case "SelectedSegment":
                    if (_nativeControl != null && Element != null)
                    {
                        var option = (RadioButton)_nativeControl.GetChildAt(Element.SelectedSegment);

                        if (option != null)
                            option.Checked = true;

                        if (Element.SelectedSegment < 0)
                        {
                            var layoutInflater = LayoutInflater.From(_context);

                            _nativeControl = (RadioGroup)layoutInflater.Inflate(Resource.Layout.RadioGroup, null);

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

                            _nativeControl.CheckedChange += NativeControl_ValueChanged;

                            SetNativeControl(_nativeControl);
                        }

                        Element.RaiseSelectionChanged();
                    }
                    break;
                case "TintColor":
                    OnPropertyChanged();
                    break;
                case "IsEnabled":
                    OnPropertyChanged();
                    break;
                case "SelectedTextColor":
                    if (_nativeControl != null && Element != null)
                    {
                        var v = (RadioButton)_nativeControl.GetChildAt(Element.SelectedSegment);
                        v.SetTextColor(Element.SelectedTextColor.ToAndroid());
                    }
                    break;
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

        private void ConfigureRadioButton(int i, RadioButton v)
        {
            if (i == Element.SelectedSegment)
            {
                v.SetTextColor(Element.SelectedTextColor.ToAndroid());
                _v = v;
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
                _v?.SetTextColor(color);
                v.SetTextColor(Element.SelectedTextColor.ToAndroid());
                _v = v;

                Element.SelectedSegment = radioId;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (_nativeControl != null)
            {
                _nativeControl.CheckedChange -= NativeControl_ValueChanged;
                _nativeControl?.Dispose();
                _nativeControl = null;
                _v = null;
            }

            RemoveElementHandlers();

            try
            {
                base.Dispose(disposing);
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