namespace JistBridge.Utilities.DialogManagement.Interfaces
{
	public interface IProgressDialog : IWaitDialog
	{
		int Progress { get; set; }
	}
}