using System.Collections.Generic;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace JistBridge.Behaviors
{
	internal class WordBehavior : Behavior<RichTextBox>
	{
		protected override void OnAttached()
		{
			AssociatedObject.PreviewMouseUp += AssociatedObject_Click;
			AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
		}

		private static void AssociatedObject_MouseMove(object sender, RoutedEventArgs e)
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

		private static void AssociatedObject_Click(object sender, RoutedEventArgs e)
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

		    var offsets = new Range<int>()
		    {
		        Minimum = range.Start.GetOffsetToPosition(richTextBox.Document.ContentStart),
		        Maximum = range.End.GetOffsetToPosition(richTextBox.Document.ContentStart)
		    };
            var fragment = new Fragment(new List<Range<int>>{offsets},FragmentType.Node,range.Text);
            new FragmentSelectedMessage(richTextBox, null, fragment).Send();
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