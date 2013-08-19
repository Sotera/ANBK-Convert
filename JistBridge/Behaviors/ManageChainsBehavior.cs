using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI;
using JistBridge.UI.ChainCanvas;
using JistBridge.UI.RichTextBox;

namespace JistBridge.Behaviors
{
    public class ManageChainsBehavior : Behavior<StackPanel>
    {
        private ChainView _currentChain;

        protected override void OnAttached()
        {
            ChainStatusMessage.Register(this,msg => HandleChainMessage(msg.Chain, msg.Status, msg.Markup));
        }

        private void HandleChainMessage(Chain chain, ChainStatus status, Markup markup)
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
                    RightFragmentAdded(chain, markup);
                    break;
                }
                case ChainStatus.CenterFragmentCanceled:
                case ChainStatus.LeftFragmentCanceled:
                case ChainStatus.RightFragmentCanceled:
                {
                    FragmentCanceled(chain, markup, status);
                    break;
                }
            }
        }

        private void FragmentCanceled(Chain chain, Markup markup, ChainStatus status)
        {
            if (markup == null || chain == null || _currentChain == null)
                return;

            switch(status)
            {
                            
                case ChainStatus.LeftFragmentCanceled:
                    {
                        DestroyCurrentChain();
                        break;
                    }
                case ChainStatus.CenterFragmentCanceled:
                    {
                        DestroyCurrentChain();
                        break;
                    }
                case ChainStatus.RightFragmentCanceled:
                    {
                        _currentChain.CenterLabel.Content = "???";
                        break;
                    }
            }
        }

        private void RightFragmentAdded(Chain chain, Markup markup)
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

        private void DestroyCurrentChain()
        {
            AssociatedObject.Children.Remove(_currentChain);
            _currentChain = null;
        }


        protected override void OnDetaching()
        {
            RichTextBoxLoadedMessage.Unregister(this);
        }
    }
}
