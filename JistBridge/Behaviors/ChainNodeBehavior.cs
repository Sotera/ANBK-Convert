using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.Behaviors {
	public class ChainNodeBehavior : Behavior<Shape> {
		protected override void OnAttached() {
			AssociatedObject.PreviewMouseUp += AssociatedObject_Click;
			AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
			AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
		}

		private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e) {
			var shape = AssociatedObject;
			shape.Stroke = Brushes.Black;
			shape.StrokeThickness = 1;
			shape.Cursor = Cursors.Arrow;

			SendFragmentStatusMessage(FragmentStatus.UnHighlighted);
		}

		private void SendFragmentStatusMessage(FragmentStatus status) {
			var node = GetAssociatedFragment();
			if (node == null)
				return;

			var viewModel = (AssociatedObject.DataContext as IChainCanvasViewModel);
			if (viewModel == null)
				return;

			new FragmentStatusMessage(this, null, viewModel.ParentMarkup, node, status).Send();
		}

		private void AssociatedObject_MouseEnter(object sender, MouseEventArgs e) {
			var shape = AssociatedObject;
			shape.Stroke = Brushes.Red;
			shape.StrokeThickness = 2;
			shape.Cursor = Cursors.Hand;

			SendFragmentStatusMessage(FragmentStatus.Highlighted);
		}

		private Fragment GetAssociatedFragment() {
			var viewModel = (AssociatedObject.DataContext as IChainCanvasViewModel);
			if (viewModel == null)
				return null;
			Fragment node = null;

			switch (AssociatedObject.Name) {
				case "Left": {
					node = viewModel.LeftFragment;
					break;
				}
				case "Center": {
					node = viewModel.CenterFragment;
					break;
				}
				case "Right": {
					node = viewModel.RightFragment;
					break;
				}
			}
			return node;
		}

		private void AssociatedObject_Click(object sender, RoutedEventArgs e) {
			var node = GetAssociatedFragment();
			if (node == null)
				return;

			//SendFragmentStatusMessage(FragmentStatus.SendToANBK);
			new SetPropertyEditorTargetMessage(this, null) {PropertiesObject = node}.Send();
		}

		protected override void OnDetaching() {
			AssociatedObject.PreviewMouseUp -= AssociatedObject_Click;
		}
	}
}