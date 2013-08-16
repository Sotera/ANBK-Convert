using System.Windows.Controls;
using System.Windows.Interactivity;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI.RichTextBox;

namespace JistBridge.Behaviors
{
    internal class ApplyMarkupBehavior : Behavior<RichTextBox>
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

        }

        protected override void OnDetaching()
        {
            RichTextBoxLoadedMessage.Unregister(this);
        }
    }
}
