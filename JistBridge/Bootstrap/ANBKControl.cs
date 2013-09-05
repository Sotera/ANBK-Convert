using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Threading;
using Interop.i2NotebookData;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Messages.ANBK;
using NLog;
using LNApplication2 = Interop.i2NotebookApp.LNApplication2;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class StartANBKControl : IBootstrapTask {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private static readonly Random Random = new Random();
		private LNApplication2 ANBKApplication { get; set; }
		private LNChart ANBKChart { get; set; }

		internal StartANBKControl() {
			StartANBKMessage.Register(this, AsyncStartANBK);
			ANBKStartedMessage.Register(this, ANBKStarted);
			AddIconToChartMessage.Register(this, AddIconToChart);
/*
			FragmentStatusMessage.Register(this, msg => {
				Log.Trace(msg.Status);
				if (msg.Status == FragmentStatus.SendToANBK)
					new AddIconToChartMessage(this, this) {
						Label = msg.chain.DisplayText
					}.Send();
			});
*/
		}

		private bool CheckANBKApplication() {
			var retVal = (ANBKApplication != null);
			if (!retVal)
				Log.Warn("ANBK application disconnected!");
			return retVal;
		}

		private bool CheckANBK() {
			var retVal = (ANBKChart != null);
			if (!retVal)
				Log.Warn("No ANBK chart to interact with!");
			return CheckANBKApplication() && retVal;
		}

		private void AddIconToChart(AddIconToChartMessage msg) {
			if (!CheckANBK()) return;

			var x = 200 + Random.Next(100, 200);
			var y = 200 + Random.Next(100, 200);
			ANBKChart.CreateIcon(ANBKChart.CurrentIconStyle, x, y, msg.Label, ANBKChart.GenerateUniqueIdentity());
		}

		private void ANBKStarted(ANBKStartedMessage anbkStartedMessage) {
			if (!CheckANBKApplication()) return;
			ANBKApplication.Charts.Add(ANBKApplication.Options.StandardTemplateFile);
			ANBKChart = ANBKApplication.Charts.CurrentChart;
		}

		private void AsyncStartANBK(StartANBKMessage msg) {
			//It may be that ANBK is already running ...
			//ANBKApplication = (LNApplication2)Marshal.GetActiveObject("LinkNotebook.Application.7");

			var worker = new BackgroundWorker();
			worker.DoWork += (o, ea) => {
				Log.Info("Starting ANBK application.");
				var anbkType = Type.GetTypeFromProgID("LinkNotebook.Application.7");
				ANBKApplication = (LNApplication2) Activator.CreateInstance(anbkType);
				var visible = ANBKApplication.Visible = true;
				new ANBKStartedMessage(this, this).Send();
				//Need to 'ping' the ANBK app so we know when it closes
				while (visible) {
					Thread.Sleep(1000); //Once per second probably good enough
					try {
						visible = ANBKApplication.Visible;
					}
					catch (Exception ex) {
						Log.Warn("ANBK application interface has thrown an exception: " + ex.Message);
						visible = false;
					}
				}
				//Cleanup and reset for another ANBK connection
				Log.Info("ANBK application has terminated.");
				ANBKApplication = null;
				ANBKChart = null;
			};
			worker.RunWorkerAsync();
		}
	}
}