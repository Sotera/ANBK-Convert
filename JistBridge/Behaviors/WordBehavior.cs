using System;
using System.Collections.Generic;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
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
        private Canvas _canvas;

        protected override void OnAttached()
        {
            _richTextBox = AssociatedObject.RichTextBoxInstance;
            _canvas = AssociatedObject.CanvasInstance;
            AssociatedObject.PreviewMouseMove += AssociatedObject_MouseMove;
            AssociatedObject.PreviewMouseUp += AssociatedObject_Click;
        }

        private void CancelFragment()
        {
            var viewModel = AssociatedObject.DataContext as IReportViewModel;

            if (viewModel == null)
                return;

            new FragmentStatusMessage(_richTextBox, null, viewModel.ReportMarkup, null, FragmentStatus.Canceled).Send();
        }

        private void AssociatedObject_MouseMove(object sender, RoutedEventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (_richTextBox == null || mouseEventArgs == null)
                return;

            if (!UpdateCursor(mouseEventArgs.GetPosition(_richTextBox), _richTextBox, _canvas))
            {
                UIHelper.ClearHighlight(_currentTextRange, _richTextBox);
                return;
            }
            var range = GetWordRange(mouseEventArgs.GetPosition(_richTextBox), _richTextBox);
            if (_currentTextRange == range)
                return;

            UIHelper.ClearHighlight(_currentTextRange, _richTextBox);
            UIHelper.HighlightRange(range);

            _currentTextRange = range;

        }

        private void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            var mouseEventArgs = e as MouseButtonEventArgs;
            if (_richTextBox == null || mouseEventArgs == null)
                return;
            var range = GetWordRange(mouseEventArgs.GetPosition(_richTextBox), _richTextBox);
            if (range == null)
            {
                CancelFragment();
                return;
            }
            var startString = new TextRange(_richTextBox.Document.ContentStart, range.Start).Text;
            startString = startString.Replace(Environment.NewLine, "");
            var startToStart = startString.Length;

            var endString = new TextRange(_richTextBox.Document.ContentStart, range.End).Text;
            endString = endString.Replace(Environment.NewLine, "");
            var startToEnd = endString.Length;
            
            var offsets = new Range<int>
            {
                Minimum = startToStart,
                Maximum = startToEnd
            };
            var sourceOffset = GetSourceOffset(range);
            var fragment = new Fragment(new List<Range<int>> { offsets }, FragmentType.Node, range.Text,sourceOffset);
            
            UIHelper.DrawFragment(fragment,_richTextBox);

            var viewModel = AssociatedObject.DataContext as IReportViewModel;
            if (viewModel == null)
                return;

            new FragmentStatusMessage(_richTextBox, null, viewModel.ReportMarkup, fragment, FragmentStatus.Selected).Send();
        }

        private static int GetSourceOffset(TextRange range)
        {
            if (range == null || range.Start == null || range.Start.Parent == null)
                return -1;
            
            var frameworkElement = range.Start.Parent as FrameworkElement;
            if (frameworkElement == null)
                return -1;

            return (int)frameworkElement.Tag;
        }

        private static TextRange GetWordRange(Point point, RichTextBox richTextBox)
        {
            var position = richTextBox.GetPositionFromPoint(point, false);
            return position == null ? null : WordBreaker.GetWordRange(position);
        }

        private static bool UpdateCursor(Point mousePosition, RichTextBox richTextBox, Canvas canvas)
        {
            var position = richTextBox.GetPositionFromPoint(mousePosition, false);
            
            if (position == null)
            {
                canvas.Cursor = Cursors.Arrow;
                return false;
            }

            canvas.Cursor = Cursors.Hand;
            return true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseUp -= AssociatedObject_Click;
            AssociatedObject.PreviewMouseMove -= AssociatedObject_MouseMove;
        }
    }
}