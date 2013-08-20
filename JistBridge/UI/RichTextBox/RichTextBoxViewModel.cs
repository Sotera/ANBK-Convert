using GalaSoft.MvvmLight;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using System.ComponentModel.Composition;

namespace JistBridge.UI.RichTextBox
{
	[Export(typeof(IRichTextBoxViewModel))]
	public class RichTextBoxViewModel : ViewModelBase, IRichTextBoxViewModel
	{
	    

	}
}