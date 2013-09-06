using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Messages.ANBK;
using JistBridge.Properties;
using NLog;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace JistBridge.UI.MainWindow {
	[Export(typeof (IMainWindowViewModel))]
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private static readonly List<LayoutDocumentPaneGroup> PaneGroupList = new List<LayoutDocumentPaneGroup>();
		private static DockingManager DockingManager { get; set; }

		private string _title = "";

		private static LayoutRoot LayoutRoot {
			get { return DockingManager.Layout; }
		}

		private object _propertyEditorTarget;

		public MainWindowViewModel() {
			SetMainWindowTitleMessage.Register(this, msg => { Title = msg.Title; });

			SetPropertyEditorTargetMessage.Register(this, msg => {
				PropertyEditorTarget = msg.PropertiesObject;
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

		public RelayCommand StartANBKCommand {
			get { return new RelayCommand(() => new StartANBKMessage(null, null).Send()); }
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

		public RelayCommand SaveCommand {
			get {
				return new RelayCommand(() => {
					var content = LayoutRoot.LastFocusedDocument.Content as ReportView.ReportView;
					if (content == null) return;

					var reportViewModel = content.DataContext as IReportViewModel;
					if (reportViewModel == null) return;

				    if (!reportViewModel.Modified)
				        return;

					reportViewModel.GetReportResponse.report.Markup = reportViewModel.ReportMarkup;

                    new SaveANBKChartMessage(this, null, cbMsg => new SaveReportRestMessage(null, null) 
                        {ReportData = reportViewModel.GetReportResponse}.Send()
                        ).Send(reportViewModel);
				});
			}
		}

		public string Title {
			set {
				_title = value;
				RaisePropertyChanged("Title");
			}
			get { return Settings.Default.ApplicationName + " " + _title; }
		}

		void IMainWindowViewModel.SetLayoutDocumentInfo(DockingManager dockingManager) {
			DockingManager = dockingManager;
			//TODO: Patch up this mess
			LayoutRoot.Updated += (s, ea) => {
				foreach (var floatingWindow in LayoutRoot.FloatingWindows) {
					foreach (var layoutElement in floatingWindow.Children) {
						var ld = layoutElement as LayoutDocument;
						if (ld != null) {
							var reportView = ld.Content as ReportView.ReportView;
							if (reportView != null) //TODO:We won't worry about stacking up events for now
								reportView.IsVisibleChanged += reportView_IsVisibleChanged;
						}
					}
				}
			};
		}

		private void reportView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			var reportView = sender as ReportView.ReportView;
			if (reportView == null) return;
			Log.Trace("Setting ReportView: " + reportView.ShortName + " to " + reportView.IsVisible +
								" + reportView_IsVisibleChanged");
			reportView.ReportViewModel.GetReportResponse.ReportVisibleSetOnly = reportView.IsVisible;
			if (!reportView.IsVisible) reportView.IsVisibleChanged -= reportView_IsVisibleChanged;
		}

		private static void PaneGroupChildrenCollectionChanged(object s, NotifyCollectionChangedEventArgs ea) {
			switch (ea.Action) {
				case NotifyCollectionChangedAction.Add:
					var newDocumentPaneGroup = ea.NewItems[0] as LayoutDocumentPaneGroup;
					if (newDocumentPaneGroup != null)
						newDocumentPaneGroup.Children.CollectionChanged += PaneGroupChildrenCollectionChanged;
					else {
						var newDocumentPane = ea.NewItems[0] as LayoutDocumentPane;
						if (newDocumentPane == null) return;
						newDocumentPane.Children.CollectionChanged += LayoutDocumentPaneCollectionChanged;
						var ourControl = newDocumentPane.Children.FirstOrDefault();
						if (ourControl == null) return;
						var reportView = ourControl.Content as ReportView.ReportView;
						if (reportView == null) return;
						reportView.ReportViewModel.GetReportResponse.ReportVisibleSetOnly = true;
						Log.Trace("Setting ReportView: " + reportView.ShortName + " to True + PaneGroupChildrenCollectionChanged");
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					var oldDocumentPane = ea.OldItems[0] as LayoutDocumentPane;
					if (oldDocumentPane == null) return;
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
			if (item == null) return;
			var reportView = item.Content as ReportView.ReportView;
			if (reportView == null) return;
			Log.Trace("Setting ReportView: " + reportView.ShortName + " to " + visible + " + LayoutDocumentPaneCollectionChanged");
			reportView.ReportViewModel.GetReportResponse.ReportVisibleSetOnly = visible;
		}

		private static LayoutDocumentPaneGroup MainPaneGroup {
			get {
				var lp = (LayoutPanel) LayoutRoot.Children.Single(c => c is LayoutPanel);
				var paneGroups = lp.Children.Where(c => c is LayoutDocumentPaneGroup).ToArray();
				if (paneGroups.Length == 1) {
					var mainPaneGroup = (LayoutDocumentPaneGroup) paneGroups[0];
					if (!PaneGroupList.Contains(mainPaneGroup)) {
						mainPaneGroup.Children.CollectionChanged += PaneGroupChildrenCollectionChanged;
						PaneGroupList.Add(mainPaneGroup);
						if (PaneGroupList.Count > 1) throw new Exception();
					}
					return mainPaneGroup;
				}
				throw new Exception();
			}
		}

		private static LayoutDocumentPane ActiveDocumentPane {
			get {
				if (LayoutRoot.LastFocusedDocument != null) {
					var possibleDocumentPane = LayoutRoot.LastFocusedDocument.Parent as LayoutDocumentPane;
					if (possibleDocumentPane != null) return possibleDocumentPane;
				}
				var candidates = MainPaneGroup.Children.Where(c => c is LayoutDocumentPane).ToArray();
				if (candidates.Length > 0)
					return (LayoutDocumentPane) candidates[0];
				var newPane = new LayoutDocumentPane();
				MainPaneGroup.Children.Add(newPane);
				return newPane;
			}
		}

		//This only gets gets called by user interaction with 'Reports' listbox
		private void AddRemoveDocumentView(AddRemoveDocumentViewMessage msg) {
			switch (msg.Operation) {
				case Operation.Add:
					var doc = new LayoutDocument {
						Title = msg.TabText,
						Content = msg.ReportView,
						CanFloat = true
					};
					ActiveDocumentPane.Children.Add(doc);
					doc.IsActive = true;
					break;
				case Operation.Remove:
					//Look for it in a floating pane
					foreach (var floatingWindow in LayoutRoot.FloatingWindows) {
						foreach (var layoutElement in floatingWindow.Children) {
							var ld = layoutElement as LayoutDocument;
							if (ld != null) {
								var reportView = ld.Content as ReportView.ReportView;
								// ReSharper disable once PossibleUnintendedReferenceComparison
								if (reportView == msg.ReportView) {
									ld.Close();
									return;
								}
							}
						}
					}
					RemoveReportViewPane(MainPaneGroup.Children, msg.ReportView);
					break;
			}
			msg.Execute(msg);
		}

		private static void RemoveReportViewPane(IEnumerable<ILayoutDocumentPane> children, Control reportView) {
			foreach (var child in children) {
				var layoutDocumentPaneGroup = child as LayoutDocumentPaneGroup;
				if (layoutDocumentPaneGroup != null) RemoveReportViewPane(layoutDocumentPaneGroup.Children, reportView);
				var layoutDocumentPane = child as LayoutDocumentPane;
				if (layoutDocumentPane != null) {
					// ReSharper disable once PossibleUnintendedReferenceComparison
					var candidates = layoutDocumentPane.Children.Where(c => c.Content == reportView).ToArray();
					if (candidates.Length == 1) layoutDocumentPane.Children.Remove(candidates[0]);
				}
			}
		}
	}
}