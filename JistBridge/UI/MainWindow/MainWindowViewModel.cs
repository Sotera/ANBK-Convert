using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Properties;
using NLog;
using Xceed.Wpf.AvalonDock.Layout;

namespace JistBridge.UI.MainWindow {
	[Export(typeof (IMainWindowViewModel))]
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private LayoutRoot LayoutRoot { get; set; }
		private LayoutDocumentPaneGroup LayoutDocumentPaneGroup { get; set; }
		private object _propertyEditorTarget;

		public MainWindowViewModel() {
			SetPropertyEditorTargetMessage.Register(this, msg => {
				PropertyEditorTarget = msg.PropertiesObject;
				//MainLayoutDocumentPane.Children.
			});
			AddRemoveDocumentViewMessage.Register(this, AddRemoveDocumentView);
		}

		public string StatusMessage {
			get { return "Ready"; }
		}

		public object PropertyEditorTarget {
			get { return _propertyEditorTarget; }
			set {
				_propertyEditorTarget = value;
				RaisePropertyChanged("PropertyEditorTarget");
			}
		}

		public RelayCommand ShowOptionsDialogCommand {
			get { return new RelayCommand(() => new ShowOptionsDialogMessage(null, null).Send()); }
		}

		public RelayCommand ShowNLogWindowCommand {
			get { return new RelayCommand(() => new ShowNLogWindowMessage(null, null).Send()); }
		}

		public RelayCommand ShowAboutBoxCommand {
			get {
				return new RelayCommand(() => {

					Log.Trace("How Trace?");
					Log.Debug("How Debug?");
					Log.Info("How Info?");
					Log.Warn("How Warn?");
					Log.Error("How Error?");
					Log.Fatal("How Fatal?");

                    new ShowAboutBoxMessage(null, null).Send();
				});
			}
		}

		public RelayCommand ExitCommand {
			get { return new RelayCommand(() => new ShutdownApplicationMessage(null, null).Send()); }
		}

		public string Title {
			get { return Settings.Default.ApplicationName; }
		}

		public void SetLayoutDocumentInfo(LayoutRoot layoutRoot, LayoutDocumentPaneGroup layoutDocumentPaneGroup) {
			LayoutRoot = layoutRoot;
			LayoutRoot.Updated += (s, ea) => {
				foreach (var floatingWindow in LayoutRoot.FloatingWindows) {
					foreach (var layoutElement in floatingWindow.Children) {
						var ld = layoutElement as LayoutDocument;
						if (ld != null) {
							var reportView = ld.Content as ReportView.ReportView;
							if (reportView != null) {
								//We won't worry about stacking up events for now
								reportView.Loaded += reportView_Loaded;
								reportView.Unloaded += reportView_Unloaded;
							}
						}
					}
				}
			};

			LayoutDocumentPaneGroup = layoutDocumentPaneGroup;
			LayoutDocumentPaneGroup.Children.CollectionChanged += PainGroupChildrenCollectionChanged;
		}

		private void reportView_Unloaded(object sender, RoutedEventArgs e) {
			reportView_Loaded(sender, e);
			var rv = sender as ReportView.ReportView;
			if (rv == null) {
				return;
			}
/*
			rv.Loaded -= reportView_Loaded;
			rv.Unloaded -= reportView_Unloaded;
*/
		}

		private void reportView_Loaded(object sender, RoutedEventArgs e) {
			var rv = sender as ReportView.ReportView;
			if (rv == null) {
				return;
			}
			//rv.Unloaded += reportView_Unloaded;
			rv.ReportViewModel.GetReportResponse.ReportVisibleSetOnly = rv.IsLoaded;
		}

		private void PainGroupChildrenCollectionChanged(object s, NotifyCollectionChangedEventArgs ea) {
			switch (ea.Action) {
				case NotifyCollectionChangedAction.Add:
					var newDocumentPaneGroup = ea.NewItems[0] as LayoutDocumentPaneGroup;
					if (newDocumentPaneGroup != null) {
						newDocumentPaneGroup.Children.CollectionChanged += PainGroupChildrenCollectionChanged;
					}
					else {
						var newDocumentPane = ea.NewItems[0] as LayoutDocumentPane;
						if (newDocumentPane == null) {
							return;
						}
						newDocumentPane.Children.CollectionChanged += LayoutDocumentPaneCollectionChanged;
						var ourControl = newDocumentPane.Children.FirstOrDefault();
						if (ourControl == null) {
							return;
						}
						var reportView = (ReportView.ReportView) ourControl.Content;
						reportView.ReportViewModel.GetReportResponse.ReportVisibleSetOnly = true;
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					var oldDocumentPane = ea.OldItems[0] as LayoutDocumentPane;
					if (oldDocumentPane == null) {
						return;
					}
					oldDocumentPane.Children.CollectionChanged -= LayoutDocumentPaneCollectionChanged;
					break;
			}
		}

		private static void LayoutDocumentPaneCollectionChanged(object s, NotifyCollectionChangedEventArgs ea) {
			//Deal with synchronizing ReportView visibility with CheckBox
			LayoutDocument item = null;
			var visible = false;
			switch (ea.Action) {
				case NotifyCollectionChangedAction.Add:
					item = (LayoutDocument) ea.NewItems[0];
					visible = true;
					break;
				case NotifyCollectionChangedAction.Remove:
					item = (LayoutDocument) ea.OldItems[0];
					break;
			}
			if (item == null) {
				return;
			}
			var reportView = item.Content as ReportView.ReportView;
			if (reportView == null) {
				return;
			}
			reportView.ReportViewModel.GetReportResponse.ReportVisibleSetOnly = visible;
		}

		private void AddRemoveDocumentView(AddRemoveDocumentViewMessage msg) {
			switch (msg.Operation) {
				case Operation.Add:
					var doc = new LayoutDocument {
						Title = msg.TabText,
						Content = msg.ReportView
					};

					if (LayoutDocumentPaneGroup.Children.Count == 0) {
						var newDocPane = new LayoutDocumentPane(doc);
						LayoutDocumentPaneGroup.Children.Add(newDocPane);
					}
					else {
						var paneToAddTo = (LayoutDocumentPane) LayoutDocumentPaneGroup.Children.FirstOrDefault();
						if (paneToAddTo != null) {
							paneToAddTo.Children.Add(doc);
						}
					}
					break;
				case Operation.Remove:
					// ReSharper disable once PossibleUnintendedReferenceComparison
					//First, we need to find the damn thing ...
					foreach (var child in LayoutDocumentPaneGroup.Children) {
						var layoutDocumentPane = (LayoutDocumentPane) child;
						var children = layoutDocumentPane.Children.Where(c => c.Content == msg.ReportView).ToArray();
						if (children.Length != 1) {
							continue;
						}
						layoutDocumentPane.Children.Remove(children[0]);
						break;
					}
					break;
			}
			msg.Execute(msg);
		}
	}
}