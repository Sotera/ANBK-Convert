using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI;
using JistBridge.UI.RichTextBox;

namespace JistBridge.Behaviors
{
    internal class ApplyMarkupBehavior : Behavior<RichTextBoxView>
    {
        protected override void OnAttached()
        {
            RichTextBoxLoadedMessage.Register(this, msg => HandleRichTextBoxLoaded(msg.Sender));
            ChainStatusMessage.Register(this, msg => HandleChainMessage(msg.Chain, msg.Status, msg.Markup));
            //AssociatedObject.
        }

        private void HandleChainMessage(Chain chain, ChainStatus status, Markup markup)
        {
            if (markup == null || chain == null )
                return;

            switch (status)
            {

                case ChainStatus.LeftFragmentCanceled:
                    {
                        RemoveFragment(chain.Left);
                        break;
                    }
                case ChainStatus.CenterFragmentCanceled:
                    {
                        RemoveFragment(chain.Left);
                        break;
                    }
                case ChainStatus.RightFragmentCanceled:
                    {
                        RemoveFragment(chain.Center);
                        break;
                    }
            }
            

        }

        private void HandleRichTextBoxLoaded(object sender)
        {
            var richTextBoxView = sender as RichTextBoxView;
            if (richTextBoxView == null)
                return;

            ApplyMarkup(richTextBoxView.RichTextBoxViewModel.ReportMarkup);
        }

        private void RemoveFragment(Fragment fragment)
        {
            var viewModel = AssociatedObject.DataContext as RichTextBoxViewModel;

            if (viewModel == null)
                return;
            var markup = viewModel.ReportMarkup;

            if (markup.AreFragmentBoundsInMarkup(fragment))
                return;

            UIHelper.ClearFragment(fragment, AssociatedObject.RichTextBoxInstance);
        }
        private void ApplyMarkup(Markup markup)
        {
            
            //TODO:Apply all of the markup chains to the Rich Text Box
        }

        protected override void OnDetaching()
        {
            RichTextBoxLoadedMessage.Unregister(this);
            ChainStatusMessage.Unregister(this);
        }
    }
}
