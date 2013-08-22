using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Properties;
using Xceed.Wpf.AvalonDock.Layout;

namespace JistBridge.UI.MainWindow {
	[Export(typeof (IMainWindowViewModel))]
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
		private LayoutDocumentPane LayoutDocumentPane { get; set; }
		private object _propertyEditorTarget;

		public MainWindowViewModel() {
			SetPropertyEditorTargetMessage.Register(this, msg => {
				PropertyEditorTarget = msg.PropertiesObject;

			});
			AddRemoveReportViewMessage.Register(this, AddRemoveReportView);
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

		public RelayCommand ShowAboutBoxCommand {
			get { return new RelayCommand(() => new ShowAboutBoxMessage(null, null).Send()); }
		}

		public RelayCommand ExitCommand {
			get { return new RelayCommand(() => new ShutdownApplicationMessage(null, null).Send()); }
		}

		public string Title {
			get { return Settings.Default.ApplicationName; }
		}

		public void SetLayoutDocumentPane(LayoutDocumentPane layoutDocumentPane) {
			LayoutDocumentPane = layoutDocumentPane;
			LayoutDocumentPane.Children.CollectionChanged += (s, ea) => {
				//The only reason we care about this event is that when the user closes
				//a tab by clicking on its 'x' the checkbox in the 'Reports' panel needs
				//to be cleared.
				if (ea.Action != NotifyCollectionChangedAction.Remove) {
					return;
				}
				var oldItem = (LayoutDocument) ea.OldItems[0];
				var reportView = (ReportView.ReportView) oldItem.Content;
				reportView.ReportViewModel.GetReportResponse.ReportVisible = false;
			};
		}

		private void AddRemoveReportView(AddRemoveReportViewMessage msg) {
			switch (msg.Operation) {
				case Operation.Add:
					LayoutDocumentPane.Children.Add(
						new LayoutDocument {
							Title = msg.TabText,
							Content = msg.ReportView
						});
					break;
				case Operation.Remove:
					// ReSharper disable once PossibleUnintendedReferenceComparison
					var children = LayoutDocumentPane.Children.Where(c => c.Content == msg.ReportView).ToArray();
					if (children.Length == 1) {
						LayoutDocumentPane.Children.Remove(children[0]);
					}
					break;
			}
		}
	}
}