using System.Windows;
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
                ApplyFormatToRange(offset, richTextBox, Brushes.White);
            }
	    }

	    public static void DrawFragment(Fragment fragment, System.Windows.Controls.RichTextBox richTextBox)
	    {
            foreach (var offset in fragment.Offsets)
	        {
	            ApplyFormatToRange(offset,richTextBox, Brushes.CornflowerBlue);
	        }
	    }

	    public static void ApplyFormatToRange(Range<int> offset, System.Windows.Controls.RichTextBox richTextBox, SolidColorBrush background)
	    {
	        var contentStart = richTextBox.Document.ContentStart;

            var range = new TextRange(contentStart.GetPositionAtOffset(offset.Minimum,LogicalDirection.Forward),
                                            contentStart.GetPositionAtOffset(offset.Maximum, LogicalDirection.Forward));
	        range.ApplyPropertyValue(TextElement.BackgroundProperty, background);
	    }
	}
}