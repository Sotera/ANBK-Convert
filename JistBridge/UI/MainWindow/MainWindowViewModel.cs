using System.Collections.Specialized;
using System.ComponentModel.Composition;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JistBridge.Interfaces;
using JistBridge.Messages;
using Xceed.Wpf.AvalonDock.Layout;

namespace JistBridge.UI.MainWindow {
	[Export(typeof (IMainWindowViewModel))]
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
		private LayoutDocumentPane _layoutDocumentPane;
		private object _propertyEditorTarget;
		public static IAppConfiguration StaticAppConfiguration { get; private set; }
		public static IUserConfiguration StaticUserConfiguration { get; private set; }

		public MainWindowViewModel() {
			SetPropertyEditorTargetMessage.Register(this, msg => { PropertyEditorTarget = msg.PropertiesObject; });
			AddRemoveReportViewMessage.Register(this, AddRemoveReportView);
		}


		[Import(typeof (IAppConfiguration))]
		public IAppConfiguration AppConfiguration {
			get { return StaticAppConfiguration; }
			set { StaticAppConfiguration = value; }
		}

		[Import(typeof (IUserConfiguration))]
		public IUserConfiguration UserConfiguration {
			get { return StaticUserConfiguration; }
			set { StaticUserConfiguration = value; }
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

		public RelayCommand GetReportRestTest {
			get { return new RelayCommand(() => new GetReportRestMessage(null, null).Send()); }
		}

		public RelayCommand ValidateUserRestTest {
			get { return new RelayCommand(() => new ValidateUserRestMessage(null, null).Send()); }
		}

		public RelayCommand ShowOptionsDialogCommand {
			get {
				return new RelayCommand(() => new ShowOptionsDialogMessage(null, null) {
					PropertiesObject = UserConfiguration
				}.Send());
			}
		}

		public RelayCommand ShowAboutBoxCommand {
			get { return new RelayCommand(() => new ShowAboutBoxMessage(null, null).Send()); }
		}

		public RelayCommand ExitCommand {
			get { return new RelayCommand(() => new ShutdownApplicationMessage(null, null).Send()); }
		}

		public string Title {
			get { return AppConfiguration.ApplicationName; }
		}

		public void SetLayoutDocumentPane(LayoutDocumentPane layoutDocumentPane) {
			_layoutDocumentPane = layoutDocumentPane;
			_layoutDocumentPane.Children.CollectionChanged += Children_CollectionChanged;
		}

		void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Remove) {
				var oldItem = e.OldItems[0] as LayoutDocument;
				if (oldItem == null) {
					return;
				}
				var reportView = oldItem.Content as ReportView.ReportView;
				if (reportView == null) {
					return;
				}
				reportView.ReportViewModel.GetReportResponse.ReportVisible = false;
			}
		}

		private void AddRemoveReportView(AddRemoveReportViewMessage msg) {
			if (msg.Operation == Operation.Add) {
				var layoutDocument = new LayoutDocument {
					Title = msg.TabText,
					Content = msg.ReportView
				};
				_layoutDocumentPane.Children.Add(layoutDocument);
			}
			else if (msg.Operation == Operation.Remove) {
				foreach (var child in _layoutDocumentPane.Children) {
					// ReSharper disable once PossibleUnintendedReferenceComparison
					if (child.Content == msg.ReportView) {
						child.Content = null;
						_layoutDocumentPane.Children.Remove(child);
						return;
					}
				}
			}
		}
	}
}