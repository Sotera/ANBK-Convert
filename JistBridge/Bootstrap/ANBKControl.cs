using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Threading;
using Interop.i2NotebookData;
using JistBridge.Interfaces;
using JistBridge.Messages.ANBK;
using NLog;
using LNApplication2 = Interop.i2NotebookApp.LNApplication2;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class ANBKControl : IBootstrapTask {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private static readonly Random Random = new Random();
		private LNApplication2 ANBKApplication { get; set; }
		private LNChart ANBKChart { get; set; }

		internal ANBKControl() {
			StartANBKMessage.Register(this, AsyncStartANBK);
			ANBKStartedMessage.Register(this, ANBKStarted);
			//ExportANBKChartMessage.Register(this, ExportANBKChart);
			//AddIconToChartMessage.Register(this, AddIconToChart);
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

/*
		private void ExportANBKChart(ExportANBKChartMessage msg) {
			Log.Trace("Entering ExportANBKChart");
		}
*/

		private bool CheckANBKApplication() {
			var retVal = (ANBKApplication != null);
			if (!retVal)
				Log.Warn("ANBK application disconnected!");
			return retVal;
		}

/*
		private void AddIconToChart(AddIconToChartMessage msg) {
			var x = 200 + Random.Next(100, 200);
			var y = 200 + Random.Next(100, 200);
			ANBKChart.CreateIcon(ANBKChart.CurrentIconStyle, x, y, msg.Label, ANBKChart.GenerateUniqueIdentity());
		}
*/

		private void ANBKStarted(ANBKStartedMessage msg) {
			ANBKApplication.Visible = true;
			//if (!CheckANBKApplication()) return;
			var worker = new BackgroundWorker();
			worker.DoWork += (o, ea) => {
				var visible = ANBKApplication.Visible;
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
			ANBKApplication.Charts.Add(ANBKApplication.Options.StandardTemplateFile);
			ANBKChart = ANBKApplication.Charts.CurrentChart;
		}

		//Interacting with out of process COM servers is always a bit of touchy business.
		//We need to do our level headed best to make what we think is happening is what
		//actually is happening.
		private void AsyncStartANBK(StartANBKMessage msg) {
			//It may be that ANBK is already running ...
			try {
				ANBKApplication = (LNApplication2) Marshal.GetActiveObject("LinkNotebook.Application.7");
				Log.Info("ANBK application already running.");
				new ANBKStartedMessage(this, this).Send();
			}
			catch (Exception e) {
				var worker = new BackgroundWorker();
				worker.DoWork += (o, ea) => {
					Log.Info("Starting ANBK application.");
					var anbkType = Type.GetTypeFromProgID("LinkNotebook.Application.7");
					ANBKApplication = (LNApplication2) Activator.CreateInstance(anbkType);
					new ANBKStartedMessage(this, this).Send();
				};
				worker.RunWorkerAsync();
			}
		}
	}
}