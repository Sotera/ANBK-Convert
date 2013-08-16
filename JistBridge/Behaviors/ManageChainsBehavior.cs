using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI.ChainCanvas;

namespace JistBridge.Behaviors
{
    public class ManageChainsBehavior : Behavior<StackPanel>
    {
        private ChainView _currentChain;

        protected override void OnAttached()
        {
            ChainStatusMessage.Register(this,msg => HandleChainMessage(msg.Chain, msg.Status, msg.MarkupId));
        }

        private void HandleChainMessage(Chain chain, ChainStatus status, Guid markupId)
        {
            switch (status)
            {
                case ChainStatus.LeftFragmentAdded:
                {
                    LeftFragmentAdded(chain);
                    break;
                }
                case ChainStatus.CenterFragmentAdded:
                {
                    CenterFragmentAdded(chain);
                    break;
                }
                case ChainStatus.RightFragmentAdded:
                {
                    RightFragmentAdded(chain, markupId);
                    break;
                }
                case ChainStatus.FragmentCanceled:
                {
                    
                    break;
                }
            }
        }

        private void RightFragmentAdded(Chain chain, Guid markupId)
        {
            _currentChain.RightLabel.Content = chain.Right.DisplayText;
        }

        private void CenterFragmentAdded(Chain chain)
        {
            _currentChain.CenterLabel.Content = chain.Center.DisplayText;
        }

        private void LeftFragmentAdded(Chain chain)
        {
            StartNewChain();
            _currentChain.LeftLabel.Content = chain.Left.DisplayText;
        }

        private void StartNewChain()
        {
            _currentChain = new ChainView();
            AssociatedObject.Children.Add(_currentChain);
        }


        protected override void OnDetaching()
        {
            RichTextBoxLoadedMessage.Unregister(this);
        }
    }
}
