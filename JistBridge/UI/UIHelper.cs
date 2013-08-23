using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using JistBridge.Data.Model;

namespace JistBridge.UI
{
    public static class UIHelper
    {
        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the queried item.</param>
        /// <returns>The first parent item that matches the submitted type parameter.
        /// If not matching item can be found, a null reference is being returned.</returns>
        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            var parent = parentObject as T;
            return parent ?? FindVisualParent<T>(parentObject);
        }

        public static void ClearFragment(Fragment fragment, System.Windows.Controls.RichTextBox richTextBox)
        {
            foreach (var offset in fragment.Offsets)
            {
                ApplyFormatToOffsetRange(offset, richTextBox, richTextBox.Background);
            }
        }

        public static void DrawFragment(Fragment fragment, System.Windows.Controls.RichTextBox richTextBox)
        {
            foreach (var offset in fragment.Offsets)
            {
                ApplyFormatToOffsetRange(offset, richTextBox, Brushes.CornflowerBlue);
            }
        }

        public static void HighlightRange(TextRange range, Brush foregroundBrush, Brush backgroundBrush,  FontWeight weight, Control control)
        {
            if (range != null)
                SetFont(range, foregroundBrush, backgroundBrush, weight);
        }

        public static void ClearHighlight(TextRange range, Control control)
        {
            SetFont(range, control.Foreground, null, control.FontWeight);
        }

        public static void ClearFragmentHighlight(TextRange range, Control control)
        {
            SetFont(range, control.Foreground,control.Background, control.FontWeight);
        }

        private static void SetFont(TextRange range, Brush foreground, Brush background, FontWeight weight)
        {
            if (range == null)
                return;
            if(foreground != null)
                range.ApplyPropertyValue(TextElement.ForegroundProperty, foreground);
            if(background != null)
                range.ApplyPropertyValue(TextElement.BackgroundProperty, background);
            
            range.ApplyPropertyValue(TextElement.FontWeightProperty, weight);
        }

        public static void ApplyFormatToTextRange(TextRange range, System.Windows.Controls.RichTextBox richTextBox, Brush background)
        {
            range.ApplyPropertyValue(TextElement.BackgroundProperty, background);
        }

        public static void ApplyFormatToOffsetRange(Range<int> offset, System.Windows.Controls.RichTextBox richTextBox, Brush background)
        {
            var contentStart = richTextBox.Document.ContentStart;
            var start = GetPointerFromCharOffset(offset.Minimum, contentStart, richTextBox.Document);
            var end = GetPointerFromCharOffset(offset.Maximum, contentStart, richTextBox.Document);

            var range = new TextRange(start, end);
            range.ApplyPropertyValue(TextElement.BackgroundProperty, background);
        }

        private static TextPointer GetPointerFromCharOffset(int charOffset, TextPointer startPointer, FlowDocument document)
        {
            var navigator = startPointer;
            if (charOffset == 0)
            {
                return navigator;
            }

            var nextPointer = navigator;
            var counter = 0;
            while (nextPointer != null && counter < charOffset)
            {
                if (nextPointer.CompareTo(document.ContentEnd) == 0)
                {
                    // If we reach to the end of document, return the EOF pointer.
                    return nextPointer;
                }
                if (nextPointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    nextPointer = nextPointer.GetPositionAtOffset(1, LogicalDirection.Forward);
                    counter++;
                }
                else
                {
                    // If the current pointer is not pointing at a character, we should move to next insertion point
                    // without incrementing the character counter.
                    nextPointer = nextPointer.GetPositionAtOffset(1, LogicalDirection.Forward);
                }
            }

            return nextPointer;
        }

        
    }
}