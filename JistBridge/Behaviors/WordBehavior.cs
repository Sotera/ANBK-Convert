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
using JistBridge.UI.RichTextBox;

namespace JistBridge.Behaviors
{
	internal class WordBehavior : Behavior<RichTextBoxView>
	{
	    private TextRange _currentTextRange;
	    private RichTextBox _richTextBox;

		protected override void OnAttached()
		{
		    _richTextBox = AssociatedObject.RichTextBoxInstance;
            AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
            AssociatedObject.PreviewMouseUp += AssociatedObject_Click;
		}

		private void AssociatedObject_MouseMove(object sender, RoutedEventArgs e)
		{
            var mouseEventArgs = e as MouseEventArgs;
            if (_richTextBox == null || mouseEventArgs == null)
				return;

            if (!UpdateCursor(mouseEventArgs.GetPosition(_richTextBox), _richTextBox))
		    {
                SetFont(_currentTextRange, Brushes.Black, FontWeights.Normal);
		        return;
		    }
            var range = GetWordRange(mouseEventArgs.GetPosition(_richTextBox), _richTextBox);
		   if (_currentTextRange == range)
		        return;

            SetFont(_currentTextRange, Brushes.Black, FontWeights.Normal);

			if(range != null)
                SetFont(range, Brushes.DarkRed, FontWeights.UltraBlack);
            
		    _currentTextRange = range;

		}

		private void AssociatedObject_Click(object sender, RoutedEventArgs e)
		{
			var mouseEventArgs = e as MouseButtonEventArgs;
            if (_richTextBox == null || mouseEventArgs == null)
				return;
            var range = GetWordRange(mouseEventArgs.GetPosition(_richTextBox), _richTextBox);
			if (range == null)
				return;
			range.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.CornflowerBlue);

		    var offsets = new Range<int>()
		    {
                Minimum = range.Start.GetOffsetToPosition(_richTextBox.Document.ContentStart),
                Maximum = range.End.GetOffsetToPosition(_richTextBox.Document.ContentStart)
		    };

            var fragment = new Fragment(new List<Range<int>>{offsets},FragmentType.Node,range.Text);
		    var viewModel = AssociatedObject.DataContext as RichTextBoxViewModel;

		    if (viewModel == null)
		        return;

            new FragmentSelectedMessage(_richTextBox, null, viewModel.ReportMarkup, fragment).Send();
        }

		private static TextRange GetWordRange(Point point, RichTextBox richTextBox)
		{
			var position = richTextBox.GetPositionFromPoint(point, false);
			return position == null ? null : WordBreaker.GetWordRange(position);
		}

		private static void SetFont(TextRange range, Brush brush, FontWeight weight)
		{
		    if (range == null)
		        return;
            range.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            range.ApplyPropertyValue(TextElement.FontWeightProperty, weight);
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