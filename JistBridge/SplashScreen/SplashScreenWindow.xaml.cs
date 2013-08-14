using System.Diagnostics;
using System.Reflection;

namespace JistBridge.SplashScreen
{
	/// <summary>
	/// Interaction logic for SplashScreenWindow.xaml
	/// </summary>
	public partial class SplashScreenWindow
	{
		// MEF composition isn't available yet, so this can't be accessed from the IAppConfiguration object.

		public string FlippyVersion
		{
			get
			{
				var exePath = Assembly.GetExecutingAssembly().Location;
				var versionInfo = FileVersionInfo.GetVersionInfo(exePath);
				return versionInfo.FileVersion;
			}
		}

		public string ApplicationName { get { return "Jist Bridger"; } }

		public SplashScreenWindow()
		{
			InitializeComponent();
		}
	}
}