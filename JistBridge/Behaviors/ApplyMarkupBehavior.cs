using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI;
using JistBridge.UI.RichTextBox;

namespace JistBridge.Behaviors {
	internal class ApplyMarkupBehavior : Behavior<RichTextBoxView> {
		private Markup _targetMarkup;
		private RichTextBox _richTextBox;

		protected override void OnAttached() {
			RichTextBoxLoadedMessage.Register(this, AssociatedObject, msg => HandleRichTextBoxLoaded(msg.Sender));
			ChainStatusMessage.Register(this, msg => HandleChainMessage(msg.Chain, msg.Status, msg.Markup));
		}

		private void HandleChainMessage(Chain chain, ChainStatus status, Markup markup) {
			if (markup == null || markup != _targetMarkup || chain == null) {
				return;
			}

			switch (status) {
				case ChainStatus.LeftFragmentCanceled: {
					RemoveFragment(chain.Left);
					chain.Left = null;
					break;
				}
				case ChainStatus.CenterFragmentCanceled: {
					RemoveFragment(chain.Left);
					chain.Left = null;
					break;
				}
				case ChainStatus.RightFragmentCanceled: {
					RemoveFragment(chain.Center);
					chain.Center = null;
					break;
				}
			}
		}

		private void HandleRichTextBoxLoaded(object sender) {
			if (_targetMarkup != null) {
				return;
			}

			var richTextBoxView = sender as RichTextBoxView;
			if (richTextBoxView == null) {
				return;
			}

			var richTextBox = richTextBoxView.RichTextBoxInstance;
			if (richTextBox == null) {
				return;
			}
			_richTextBox = richTextBox;

			var viewModel = richTextBoxView.DataContext as IReportViewModel;
			if (viewModel == null) {
				return;
			}

			_richTextBox.Document = viewModel.ReportDocument;

			_targetMarkup = viewModel.ReportMarkup;

			ApplyMarkup(_targetMarkup);
		}

		private void RemoveFragment(Fragment fragment) {
			var viewModel = AssociatedObject.DataContext as IReportViewModel;

			if (viewModel == null) {
				return;
			}

			var markup = viewModel.ReportMarkup;
			if (markup == null) {
				return;
			}

			if (markup.AreFragmentBoundsInMarkup(fragment)) {
				return;
			}

			UIHelper.ClearFragment(fragment, AssociatedObject.RichTextBoxInstance);
		}

		private void ApplyMarkup(Markup markup) 
        {
		    foreach (var chain in markup.Chains)
		    {
		        UIHelper.DrawFragment(chain.Left,_richTextBox);
		        //new ChainStatusMessage(this, null, markup, chain, ChainStatus.LeftFragmentAdded).Send();
                
                UIHelper.DrawFragment(chain.Center, _richTextBox);
		        //new ChainStatusMessage(this, null, markup, chain, ChainStatus.CenterFragmentAdded).Send();
                
                UIHelper.DrawFragment(chain.Right, _richTextBox);
		        //new ChainStatusMessage(this, null, markup, chain, ChainStatus.RightFragmentAdded).Send();
		    }
        }

		protected override void OnDetaching() {
			RichTextBoxLoadedMessage.Unregister(this);
			ChainStatusMessage.Unregister(this);
		}
	}
}