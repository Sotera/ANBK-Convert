using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Utilities.MefHelpers;
using NLog;

namespace JistBridge.Bootstrap {
	internal class Bootstrapper {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private const int SplashScreenDelayMS = 3000;

		[ImportMany(typeof (IBootstrapTask))]
		private IEnumerable<IBootstrapTask> BootstrapTasks { get; set; }

		internal Bootstrapper(Action callback) {
			QueueMefComposeMessage.Register(this, msg => {
				ExecutingAssemblyMefHelper.DoMefCompose(msg.MefTarget);
				msg.Execute(msg);
			});

			new QueueMefComposeMessage(this, this, this, msg => {
				new HideSplashScreenMessage(null, null).SendAfterWaiting(SplashScreenDelayMS);
				callback();
			}).Send();
		}
	}
}