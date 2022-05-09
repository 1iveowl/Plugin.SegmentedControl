//#nullable enable
//using Microsoft.Maui.Handlers;
//using RadioButton = Microsoft.UI.Xaml.Controls.RadioButton;

//namespace SegCtrl
//{
//    public partial class SegmentedControlHandler // : ViewHandler<ISegmentedControl, RadioButton>
//    {

//        public static PropertyMapper<ISegmentedControl, SegmentedControlHandler> SegmentControlMapper =
//            new PropertyMapper<ISegmentedControl, SegmentedControlHandler>(ViewMapper)
//            {
//                [nameof(ISegmentedControl.OnElementChildrenChanging)] = OnElementChildrenChanging,
//            };

//        public SegmentedControlHandler(
//            IPropertyMapper mapper, 
//            CommandMapper? commandMapper = null) : base(mapper, commandMapper)
//        {
//        }

//        //protected override RadioButton CreatePlatformView()
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //protected override void ConnectHandler(RadioButton platformView)
//        //{
//        //    base.ConnectHandler(platformView);
//        //}

//        //protected override void DisconnectHandler(RadioButton platformView)
//        //{
//        //    base.DisconnectHandler(platformView);
//        //}
//    }
//}
