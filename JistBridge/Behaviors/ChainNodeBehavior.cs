using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Shapes;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.Behaviors
{
    public class ChainNodeBehavior : Behavior<Shape>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseUp += AssociatedObject_Click;
        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (AssociatedObject.DataContext as IChainCanvasViewModel);
            if (viewModel == null)
                return;
            Fragment node = null;
            switch (AssociatedObject.Name)
            {
                case "Left":
                {
                    node = viewModel.LeftFragment;
                    break;
                }
                case "Center":
                {
                    node = viewModel.CenterFragment;
                    break;
                }
                case "Right":
                {
                    node = viewModel.RightFragment;
                    break;
                }
            }
            if (node == null)
                return;

            new SetPropertyEditorTargetMessage(this, null){PropertiesObject = node}.Send();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseUp -= AssociatedObject_Click;
        }
    }
}
