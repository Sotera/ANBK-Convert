using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using GalaSoft.MvvmLight.Threading;
using NLog;

namespace JistBridge.Utilities.MefHelpers {
	internal static class ExecutingAssemblyMefHelper {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private static AggregateCatalog _staticAggregateCatalog;

		private static AggregateCatalog AggregateCatalog {
			get {
				if (_staticAggregateCatalog != null) {
					return _staticAggregateCatalog;
				}
				_staticAggregateCatalog = new AggregateCatalog();
				_staticAggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
				return _staticAggregateCatalog;
			}
		}

		public static void DoMefCompose(object target) {
			try {
				DispatcherHelper.UIDispatcher.Invoke(
					() => {
						try {
							new CompositionContainer(AggregateCatalog).ComposeParts(target);
						}
						catch (Exception e) {
							Console.WriteLine(e);
						}
					});
			}
			catch (Exception ex) {}
		}
	}
}