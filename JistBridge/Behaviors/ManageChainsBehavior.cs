using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Messages.ANBK;
using JistBridge.UI.ChainCanvas;

namespace JistBridge.Behaviors {
	//public class ManageChainsBehavior : Behavior<StackPanel>
	public class ManageChainsBehavior : Behavior<FrameworkElement> {
		private ChainView _currentChain;

		protected override void OnAttached() {
			ChainStatusMessage.Register(this, msg => HandleChainMessage(msg.Chain, msg.Status, msg.Markup));
		}

		private void HandleChainMessage(Chain chain, ChainStatus status, Markup markup) {
			var reportViewModel = AssociatedObject.DataContext as IReportViewModel;
			if (reportViewModel != null && reportViewModel.ReportMarkup != markup)
				return;

			switch (status) {
				case ChainStatus.LeftFragmentAdded: {
					LeftFragmentAdded(chain);
					break;
				}
				case ChainStatus.CenterFragmentAdded: {
					CenterFragmentAdded(chain);
					break;
				}
				case ChainStatus.RightFragmentAdded: {
					RightFragmentAdded(chain);
					break;
				}
				case ChainStatus.CenterFragmentCanceled:
				case ChainStatus.LeftFragmentCanceled:
				case ChainStatus.RightFragmentCanceled: {
					FragmentCanceled(chain, markup, status);
					break;
				}
			}
		}

		private void FragmentCanceled(Chain chain, Markup markup, ChainStatus status) {
			if (markup == null || chain == null || _currentChain == null)
				return;

			switch (status) {
				case ChainStatus.LeftFragmentCanceled: {
					DestroyCurrentChain();
					break;
				}
				case ChainStatus.CenterFragmentCanceled: {
					DestroyCurrentChain();
					break;
				}
				case ChainStatus.RightFragmentCanceled: {
					var chainCanvasViewModel = _currentChain.DataContext as IChainCanvasViewModel;
					if (chainCanvasViewModel != null)
						chainCanvasViewModel.CenterFragment = null;
					break;
				}
			}
		}

		private void RightFragmentAdded(Chain chain) {
			var chainCanvasViewModel = _currentChain.DataContext as IChainCanvasViewModel;
			if (chainCanvasViewModel != null)
				chainCanvasViewModel.RightFragment = chain.Right;
			SetChartElementText(SendChainInfoToANBKMessage.ActionType.ModifyRightNodeText, chain.Right.DisplayText);
		}

		private void CenterFragmentAdded(Chain chain) {
			var chainCanvasViewModel = _currentChain.DataContext as IChainCanvasViewModel;
			if (chainCanvasViewModel != null)
				chainCanvasViewModel.CenterFragment = chain.Center;

			SetChartElementText(SendChainInfoToANBKMessage.ActionType.ModifyLinkText, chain.Center.DisplayText);
		}

		private void LeftFragmentAdded(Chain chain) {
			StartNewChain();
			var chainCanvasViewModel = _currentChain.DataContext as IChainCanvasViewModel;
			if (chainCanvasViewModel != null) {
				chainCanvasViewModel.LeftFragment = chain.Left;
				var reportViewModel = AssociatedObject.DataContext as IReportViewModel;
				if (reportViewModel != null)
					chainCanvasViewModel.ParentMarkup = reportViewModel.ReportMarkup;
			}

			SetChartElementText(SendChainInfoToANBKMessage.ActionType.ModifyLeftNodeText, chain.Left.DisplayText);
		}

		private void SetChartElementText(SendChainInfoToANBKMessage.ActionType action, string text) {
			var reportViewModel = AssociatedObject.DataContext as IReportViewModel;
			if (reportViewModel == null) return;

			if (reportViewModel.TmpMsg == null) return;
			reportViewModel.TmpMsg.Action = action;
			reportViewModel.TmpMsg.Text = text;
			reportViewModel.TmpMsg.Send(reportViewModel);
		}

		private void StartNewChain() {
			_currentChain = new ChainView();
			var panel = AssociatedObject as Panel;
			if (panel != null) {
				panel.Children.Add(_currentChain);
				return;
			}
			var userControl = AssociatedObject as UserControl;
			if (userControl != null) {
				var reportViewModel = userControl.DataContext as IReportViewModel;
				if (reportViewModel == null) return;
				new SendChainInfoToANBKMessage(this, null, cbMsg => { reportViewModel.TmpMsg = cbMsg; }) {
					Action = SendChainInfoToANBKMessage.ActionType.CreateNewChain
				}.Send(reportViewModel);
			}
		}

		private void DestroyCurrentChain() {
			var panel = AssociatedObject as Panel;
			if (panel != null)
				panel.Children.Remove(_currentChain);
			_currentChain = null;
		}


		protected override void OnDetaching() {}
	}
}