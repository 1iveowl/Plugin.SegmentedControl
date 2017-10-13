using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Plugin.SegmentedControl.Netstandard.Control;
using Plugin.SegmentedControl.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using ColumnDefinition = Windows.UI.Xaml.Controls.ColumnDefinition;
using Grid = Windows.UI.Xaml.Controls.Grid;
using GridLength = Xamarin.Forms.GridLength;
using GridUnitType = Xamarin.Forms.GridUnitType;
using TextAlignment = Xamarin.Forms.TextAlignment;

[assembly: ExportRenderer(typeof(SegmentedControl), typeof(SegmentedControlRenderer))]
namespace Plugin.SegmentedControl.UWP
{
    public class SegmentedControlRenderer : ViewRenderer<Netstandard.Control.SegmentedControl, Grid>
    {
        private readonly IList<SegmentedControlOption> _segmentList;

        private Grid _grid;
        

        public SegmentedControlRenderer()
        {
            var segmentCollection = new ObservableCollection<SegmentedControlOption>();

            segmentCollection.CollectionChanged += (s, e) =>
            {
                RebuildButtons();
            };

            _segmentList = segmentCollection;


        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            _segmentList.Add(Element.Children[0]);

            //RebuildButtons();

            //switch (e.PropertyName)
            //{
                    
            //}
        }

        private void RebuildButtons()
        {
            //this.ColumnDefinitions.Clear();
            this.Children.Clear();

            _grid = new Grid();
            _grid.Children.Clear();
            _grid.ColumnDefinitions.Clear();

            var label = new TextBlock
            {
                Text = _segmentList[0].Text,
            };

            _grid.Children.Add(label);

            SetNativeControl(_grid);

            //for (var i = 0; i < _segmentList.Count; i++)
            //{
            //    var buttonSeg = _segmentList[i];

            //    var label = new Label
            //    {
            //        Text = buttonSeg.Text,
            //        HorizontalTextAlignment = TextAlignment.Center,
            //        VerticalTextAlignment = TextAlignment.Center
            //    };

            //    _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            //    var frame = new AdvancedFrame();

            //    if (i == 0)
            //        frame.Corners = RoundedCorners.left;
            //    else if (i + 1 == SegmentedButtons.Count)
            //        frame.Corners = RoundedCorners.right;
            //    else
            //        frame.Corners = RoundedCorners.none;

            //    frame.CornerRadius = CornerRadius;

            //    frame.OutlineColor = OnColor;
            //    frame.Content = label;
            //    frame.HorizontalOptions = LayoutOptions.FillAndExpand;
            //    frame.VerticalOptions = LayoutOptions.FillAndExpand;

            //    DrawBoxes(i, frame, label);

            //    var tapGesture = new TapGestureRecognizer
            //    {
            //        Command = ItemTapped,
            //        CommandParameter = i
            //    };

            //    frame.GestureRecognizers.Add(tapGesture);

            //    this.Children.Add(frame, i, 0);
            //}
        }

        //public Command ItemTapped
        //{
        //    get
        //    {
        //        return new Command((obj) =>
        //        {

        //            var index = (int)obj;

        //            SelectedIndex = index;

        //            Command?.Execute(this.SegmentedButtons[index].Title);
        //        });
        //    }
        //}

        //private void SetSelectedIndex()
        //{
        //    for (var i = 0; i < Children.Count; i++)
        //    {
        //        var frame = Children[i] as AdvancedFrame;
        //        var label = frame.Content as Label;

        //        DrawBoxes(i, frame, label);
        //    }
        //}

        //private void DrawBoxes(int i, AdvancedFrame frame, Label label)
        //{

        //    if (i == SelectedIndex)
        //    {

        //        frame.InnerBackground = OnBackgroundColor;
        //        label.TextColor = OffColor;
        //    }
        //    else
        //    {

        //        frame.InnerBackground = OffBackgroundColor;
        //        label.TextColor = OnColor;
        //    }
        //}

    }
}
