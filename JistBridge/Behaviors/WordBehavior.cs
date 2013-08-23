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
            AssociatedObject.PreviewMouseMove += UpdateFragment;
            AssociatedObject.PreviewMouseDown += BeginFragment;
            AssociatedObject.PreviewMouseUp += FinishFragment;

            FragmentStatusMessage.Register(this,msg=>HandleFragmentStatus(msg.Markup, msg.Fragment, msg.Status));
        }

        private void HandleFragmentStatus(Markup markup, Fragment fragment, FragmentStatus status)
        {
            var viewModel = AssociatedObject.DataContext as IReportViewModel;
            if (viewModel == null || viewModel.ReportMarkup != markup)
                return;

            switch (status)
            {
                case FragmentStatus.Highlighted:
                {
                    UIHelper.ToggleFragmentHilighted(fragment, _richTextBox, true);
        
                    break;
                }
                case FragmentStatus.UnHighlighted:
                {
                    UIHelper.ToggleFragmentHilighted(fragment, _richTextBox, false);
                    break;
                }
            }

        }

        private void CancelFragment()
        {
            var viewModel = AssociatedObject.DataContext as IReportViewModel;

            if (viewModel == null)
                return;

            new FragmentStatusMessage(_richTextBox, null, viewModel.ReportMarkup, null, FragmentStatus.Canceled).Send();
        }

        private TextRangeWithOrigin _currentFragmentRange;
        
        private void BeginFragment(object sender, RoutedEventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (_richTextBox == null || mouseEventArgs == null)
                return;
            var range = GetWordRange(mouseEventArgs.GetPosition(_richTextBox), _richTextBox);
            if (range == null)
                return;
            _currentFragmentRange = new TextRangeWithOrigin(range);
        }

        private void UpdateFragment(object sender, RoutedEventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (_richTextBox == null || mouseEventArgs == null) return;

            var range = GetWordRange(mouseEventArgs.GetPosition(_richTextBox), _richTextBox);
            
            if (_currentFragmentRange == null)
            {
                MouseoverText(range,mouseEventArgs);
                return;
            }

            UIHelper.ClearFragmentHighlight(_currentFragmentRange.Union, _richTextBox);

            _currentFragmentRange.Update(range);

            UIHelper.HighlightRange(_currentFragmentRange.Union, Brushes.White, Brushes.Black, _richTextBox.FontWeight, _richTextBox);
        }

        private void FinishFragment(object sender, RoutedEventArgs e)
        {
            var mouseEventArgs = e as MouseEventArgs;
            if (_richTextBox == null || mouseEventArgs == null)return;

            if (_currentFragmentRange == null)
            {
                CancelFragment();
                return;
            }

            var startString = new TextRange(_richTextBox.Document.ContentStart, _currentFragmentRange.Union.Start).Text;
            startString = startString.Replace(Environment.NewLine, "");
            var startToStart = startString.Length;

            var endString = new TextRange(_richTextBox.Document.ContentStart, _currentFragmentRange.Union.End).Text;
            endString = endString.Replace(Environment.NewLine, "");
            var startToEnd = endString.Length;

            var offsets = new Range<int>
            {
                Minimum = startToStart,
                Maximum = startToEnd
            };
            var sourceOffset = GetSourceOffset(_currentFragmentRange.Union);
            var fragment = new Fragment(new List<Range<int>> { offsets }, FragmentType.Node, _currentFragmentRange.Union.Text, sourceOffset);

            UIHelper.ClearHighlight(_currentFragmentRange.Union, _richTextBox);
        
            UIHelper.DrawFragment(fragment, _richTextBox);

            var viewModel = AssociatedObject.DataContext as IReportViewModel;
            if (viewModel == null)
                return;

            new FragmentStatusMessage(_richTextBox, null, viewModel.ReportMarkup, fragment, FragmentStatus.Selected).Send();
            _currentFragmentRange = null;
        }

        private void MouseoverText(TextRange range, MouseEventArgs mouseEventArgs)
        {
            if (_richTextBox == null || mouseEventArgs == null)return;

            if (!UpdateCursor(mouseEventArgs.GetPosition(_richTextBox), _richTextBox, _canvas))
            {
                UIHelper.ClearHighlight(_currentTextRange, _richTextBox);
                return;
            }

            if (range == null || _currentTextRange == range) return;

            UIHelper.ClearHighlight(_currentTextRange, _richTextBox);
            UIHelper.HighlightRange(range, Brushes.DarkRed, null , _richTextBox.FontWeight, _richTextBox);

            _currentTextRange = range;

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
            AssociatedObject.PreviewMouseMove -= UpdateFragment;
            AssociatedObject.PreviewMouseDown -= BeginFragment;
            AssociatedObject.PreviewMouseUp -= FinishFragment;

            FragmentStatusMessage.Unregister(this);
        }

        protected class TextRangeWithOrigin
        {
            public TextRange Origin { get; private set; }

            public TextRange Union { get; private set; }

            public TextRangeWithOrigin(TextRange origin)
            {
                Origin = origin;
                Union = origin;
            }

            public void Update(TextRange range)
            {
                if (range == null)
                    return;
                if (Origin.Start.CompareTo(range.Start) == 0 &&
                    Origin.End.CompareTo(range.End) == 0)
                {
                    Union = Origin;
                    return;
                }
                if (range.Start.CompareTo(Origin.End) > 0)
                {
                    Union = new TextRange(Origin.Start, range.End);
                    return;
                }

                if (range.Start.CompareTo(Origin.Start) < 0)
                    Union = new TextRange(range.Start, Origin.End);

            }
        }
    }
}