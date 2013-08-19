using System.Windows;
using System.Windows.Input;
using JistBridge.Interfaces;
using JistBridge.Messages;
using System.ComponentModel.Composition;

namespace JistBridge.UI.RichTextBox
{
	/// <summary>
	/// Interaction logic for RichTextBoxView.xaml
	/// </summary>
	public partial class RichTextBoxView
	{
		[Import]
		public IRichTextBoxViewModel RichTextBoxViewModel
		{
			set
			{
				DataContext = value;
			}
		    get
		    {
		        if (!(DataContext is RichTextBoxViewModel))
		            return null;
		        return DataContext as RichTextBoxViewModel;
		    }
		}

	    private void RtbLoaded(object obj, DependencyPropertyChangedEventArgs args)
	    {
            new RichTextBoxLoadedMessage(this, null).Send();
            IsVisibleChanged -= RtbLoaded;
	    }
        
	    public RichTextBoxView()
		{
			InitializeComponent();
            IsVisibleChanged += RtbLoaded;
			new QueueMefComposeMessage(null, null, this,null).Send();
        }
	}
}