﻿using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI.ChainCanvas;

namespace JistBridge.Behaviors
{
    public class ManageChainsBehavior : Behavior<StackPanel>
    {
        protected override void OnAttached()
        {
            ChainAddedMessage.Register(this,msg => HandleChainAdded(msg.Chain));
        }

        private void HandleChainAdded(Chain chain)
        {
            var chainView = new ChainView();
            AssociatedObject.Children.Add(chainView);
        }

        protected override void OnDetaching()
        {
            RichTextBoxLoadedMessage.Unregister(this);
        }
    }
}
