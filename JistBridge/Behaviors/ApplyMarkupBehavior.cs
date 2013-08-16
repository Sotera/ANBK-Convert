using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI.RichTextBox;

namespace JistBridge.Behaviors
{
    internal class ApplyMarkupBehavior : Behavior<RichTextBoxView>
    {
        protected override void OnAttached()
        {
            RichTextBoxLoadedMessage.Register(this, msg => HandleRichTextBoxLoaded(msg.Sender));
		    
            //AssociatedObject.
        }

        private void HandleRichTextBoxLoaded(object sender)
        {
            var richTextBoxView = sender as RichTextBoxView;
            if (richTextBoxView == null)
                return;

            ApplyMarkup(richTextBoxView.RichTextBoxViewModel.ReportMarkup);
        }

        private void ApplyMarkup(Markup markup)
        {
            if (markup.Chains == null || markup.Chains.Count == 0)
                return;
            //TODO:Apply all of the markup chains to the Rich Text Box
        }

        protected override void OnDetaching()
        {
            RichTextBoxLoadedMessage.Unregister(this);
        }
    }
}
