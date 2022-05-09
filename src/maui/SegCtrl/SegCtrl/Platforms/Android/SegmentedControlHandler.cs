#nullable enable
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;
using static Android.Views.ViewGroup;

namespace SegCtrl.Handlers
{
    public partial class SegmentedControlHandler : ViewHandler<ISegmentedControl, RadioGroup>
    {

        protected override RadioGroup CreatePlatformView()
        {
            return new RadioGroup(Context);
        }

        public static void OnElementChildrenChanging(SegmentedControlHandler handler, ISegmentedControl segmentedControl)
        {

        }

        protected override void ConnectHandler(RadioGroup radioGroup)
        {            
            if (radioGroup is not null)
            {
                // radioGroup.ViewAttachedToWindow += RadioGroup_ViewAttachedToWindow;
                //radioGroup.LayoutChange += PlatformView_LayoutChange;
                radioGroup.ChildViewAdded += PlatformView_ChildViewAdded;
                radioGroup.ChildViewRemoved += PlatformView_ChildViewRemoved;

                var layoutInflater = LayoutInflater.From(Context);

                    //_radioGroup = layoutInflater?.Inflate(Resource.Layout.RadioGroup, default) as RadioGroup;

                foreach(var (child, i) in VirtualView.Children.Select((child, i) => (child, i)))
                {
                    var radioButton = layoutInflater?.Inflate(Resource.Layout.RadioButton, null) as Android.Widget.RadioButton;

                    if (radioButton is not null)
                    {
                        if (child.WidthRequest > 0)
                        {
                            radioButton.LayoutParameters = new RadioGroup.LayoutParams(
                                Convert.ToInt32(Math.Round(child.WidthRequest)),
                                LayoutParams.WrapContent, 0);
                        }
                        else
                        {
                            radioButton.LayoutParameters = new RadioGroup.LayoutParams(0, LayoutParams.WrapContent, 1f);
                        }

                        radioButton.Text = child.Text;

                        if (i == 0)
                        {
                            radioButton.SetBackgroundResource(Resource.Drawable.segmented_control_first_background);
                        }
                        else
                        {
                            radioButton.SetBackgroundResource(Resource.Drawable.segmented_control_last_background);
                        }

                        ConfigureRadioButton(i, radioButton, VirtualView);

                        if (i == VirtualView.SelectedSegment)
                        {
                            radioButton.Checked = true;
                        }

                        radioGroup.AddView(radioButton);
                    }                    
                }

                base.ConnectHandler(radioGroup);
            }           
        }

        protected override void DisconnectHandler(RadioGroup radioGroup)
        {
            if (radioGroup is not null)
            {
                radioGroup.LayoutChange -= PlatformView_LayoutChange;
                radioGroup.ChildViewAdded -= PlatformView_ChildViewAdded;
                radioGroup.ChildViewRemoved -= PlatformView_ChildViewRemoved;

                base.DisconnectHandler(radioGroup);
            }
        }

        private void RadioGroup_ViewAttachedToWindow(object? sender, Android.Views.View.ViewAttachedToWindowEventArgs e)
        {
            if (sender is not null)
            {
                var segmentCtrl = (ISegmentedControl)sender;

                var layoutInflater = LayoutInflater.From(Context);



                var radioGroup = layoutInflater?.Inflate(Resource.Layout.RadioGroup, default) as RadioGroup;

                var segmentOption = radioGroup?.GetChildAt(segmentCtrl.SelectedSegment);
            }
        }

        internal static void TextColor(SegmentedControlHandler handler, ISegmentedControl entry)
        {
            var layoutInflator = LayoutInflater.From(handler.Context);                        
        }

        private void PlatformView_LayoutChange(object? sender, Android.Views.View.LayoutChangeEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PlatformView_ChildViewRemoved(object? sender, Android.Views.ViewGroup.ChildViewRemovedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void PlatformView_ChildViewAdded(object? sender, Android.Views.ViewGroup.ChildViewAddedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ConfigureRadioButton(
            int index, 
            Android.Widget.RadioButton radioButton,
            ISegmentedControl segmentControl)
        {
            if (index == segmentControl.SelectedSegment)
            {
                radioButton.SetTextColor(segmentControl.SelectedTextColor.ToAndroid());

                //_nativeRadioButtonControl = radioButton;
            }
            else
            {
                var textColor = segmentControl.IsEnabled
                    ? segmentControl.TextColor.ToAndroid()
                    : segmentControl.DisabledColor.ToAndroid();

                radioButton.SetTextColor(textColor);
            }

            radioButton.TextSize = Convert.ToSingle(segmentControl.FontSize);

            var font = Microsoft.Maui.Font.OfSize(segmentControl.FontFamily, segmentControl.FontSize); //.ToTypeface();

            var typeface = Typeface.Create(font.Family?.ToString(), TypefaceStyle.Normal);

            radioButton.SetTypeface(typeface, TypefaceStyle.Normal);

            //radioButton.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

            var gradientDrawable = radioButton.Background as StateListDrawable;

            var drawableContainerState = (DrawableContainer.DrawableContainerState)gradientDrawable?.GetConstantState();

            var children = drawableContainerState?.GetChildren();

            if (children is not null 
                && children[0] is not null 
                && children[1] is not null)
            {
                var selectedShape = children[0] is GradientDrawable drawable
                    ? drawable
                    : (GradientDrawable)((InsetDrawable)children[0]).Drawable;

                var unselectedShape = children[1] is GradientDrawable drawable1
                    ? drawable1
                    : (GradientDrawable)((InsetDrawable)children[1]).Drawable;

                var backgroundColor = segmentControl.IsEnabled ? segmentControl.TintColor.ToAndroid() : segmentControl.DisabledColor.ToAndroid();

                var borderColor = segmentControl.IsEnabled ? segmentControl.BorderColor.ToAndroid() : segmentControl.DisabledColor.ToAndroid();
                var borderWidthInPixel = ConvertDipToPixel(segmentControl.BorderWidth, radioButton);

                if (selectedShape is not null)
                {
                    selectedShape.SetStroke(borderWidthInPixel, borderColor);

                    selectedShape.SetColor(backgroundColor);
                }

                if (unselectedShape is not null)
                {

                    unselectedShape.SetStroke(borderWidthInPixel, borderColor);
                    //unselectedShape.SetColor(_unselectedItemBackgroundColor);
                }
            }

            radioButton.Enabled = segmentControl.Children[index].IsEnabled;
        }

        private int ConvertDipToPixel(double dip, Android.Widget.RadioButton radioButton)
        {
            return (int)Android.Util.TypedValue.ApplyDimension(Android.Util.ComplexUnitType.Dip, (float)dip, radioButton.Resources?.DisplayMetrics);
        }
    }
}
