namespace JistBridge.Interfaces
{
	public interface IUserConfiguration
	{
		string ValidateUserUrl { get; set; }
		string GetReportUrl { get; set; }
		string SaveReportUrl{ get; set; }
	}
}