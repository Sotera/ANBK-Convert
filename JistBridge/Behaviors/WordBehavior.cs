using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using JistBridge.UI;

namespace JistBridge.Behaviors
{
    class WordBehavior:Behavior<RichTextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.PreviewMouseUp += AssociatedObject_Click;
            AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
        }

        static void AssociatedObject_MouseMove(object sender, RoutedEventArgs e)
        {
            var richTextBox = sender as RichTextBox;
            var mouseEventArgs = e as MouseEventArgs;
            if (richTextBox == null || mouseEventArgs == null)
                return;
            
           // ClearFontWeight(richTextBox);

            if (!UpdateCursor(mouseEventArgs.GetPosition(richTextBox), richTextBox))
                return;

            /*var range = GetWordRange(mouseEventArgs.GetPosition(richTextBox), richTextBox);
            if(range != null)
                SetFontWeight(range);*/
        }

        static void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var richTextBox = sender as RichTextBox;
            var mouseEventArgs = e as MouseButtonEventArgs;
            if (richTextBox == null || mouseEventArgs == null)
                return;
            var range = GetWordRange(mouseEventArgs.GetPosition(richTextBox), richTextBox);
            if (range == null)
                return;
            range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
            range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.CornflowerBlue);
        }

        private static TextRange GetWordRange(Point point, RichTextBox richTextBox)
        {
            var position = richTextBox.GetPositionFromPoint(point, false);
            return position == null ? null : WordBreaker.GetWordRange(position);
        }

        private static void ClearFontWeight(RichTextBox richTextBox)
        {
            var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Regular);
        }

        private static void SetFontWeight(TextRange range)
        {
            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private static bool UpdateCursor(Point mousePosition, RichTextBox richTextBox)
        {
            var position = richTextBox.GetPositionFromPoint(mousePosition, false);
            var mouseIsOverCharacter = position != null;
            if (position == null)
            {
                if (richTextBox.Cursor == Cursors.Arrow)
                    return mouseIsOverCharacter;
                
                richTextBox.Cursor = Cursors.Arrow;
                return mouseIsOverCharacter;
            }

            if (richTextBox.Cursor == Cursors.Hand)
                return mouseIsOverCharacter;

            richTextBox.Cursor = Cursors.Hand;
            return mouseIsOverCharacter;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseUp -= AssociatedObject_Click;
            AssociatedObject.PreviewMouseMove -= AssociatedObject_MouseMove;
        }
    }
}
