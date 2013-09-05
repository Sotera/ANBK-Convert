using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using GalaSoft.MvvmLight.Command;
using JistBridge.Annotations;
using JistBridge.Data.Model;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Messages.ANBK;
using JistBridge.UI.ReportView.States;
using JistBridge.Utilities.StateMachine;
using NLog;

namespace JistBridge.UI.ReportView {
	[Export(typeof (IReportViewModel))]
	public class ReportViewModel : IReportViewModel, INotifyPropertyChanged {
		public IFSMSystem StateMachine { get; private set; }
		public ReportData ReportData { get; private set; }

		private const string ModifiedIndicatior = " *";

		private bool _modified;

		public bool Modified {
			get { return _modified; }
			set {
				if (value.Equals(_modified)) return;
				_modified = value;
				if (_modified)
					Title = ReportData.ReportResponse.ShortName + ModifiedIndicatior;
				else
					Title = ReportData.ReportResponse.ShortName;
				OnPropertyChanged();
			}
		}

		private string _title = "";

		public string Title {
			get { return _title; }
			set {
				if (value == _title) return;
				_title = value;
				OnPropertyChanged();
			}
		}

		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		public RelayCommand FitActualSizeCommand {
			get { return new RelayCommand(() =>
				new FitChartMessage(this, null){FitType = FitType.ActualSize}.Send(this)); }
		}

		public RelayCommand FitHeightCommand {
			get { return new RelayCommand(() =>
				new FitChartMessage(this, null){FitType = FitType.Height}.Send(this)); }
		}

		public RelayCommand FitSelectionInWindowCommand {
			get { return new RelayCommand(() =>
				new FitChartMessage(this, null){FitType = FitType.SelectionInWindow}.Send(this)); }
		}

		public RelayCommand FitWindowCommand {
			get { return new RelayCommand(() =>
				new FitChartMessage(this, null){FitType = FitType.Window}.Send(this)); }
		}

		public RelayCommand CircularLayoutCommand {
			get { return new RelayCommand(() =>
				new ChangeLayoutMessage(this, null){LayoutType = LayoutType.Circle}.Send(this)); }
		}

		public RelayCommand PeacockLayoutCommand {
			get { return new RelayCommand(() =>
				new ChangeLayoutMessage(this, null){LayoutType = LayoutType.Peacock}.Send(this)); }
		}

		public RelayCommand GroupLayoutCommand {
			get { return new RelayCommand(() =>
				new ChangeLayoutMessage(this, null){LayoutType = LayoutType.Group}.Send(this)); }
		}

		//This is not bound, the RichTextBox wont let you.  I set the flow document in the
		//ApplyMarkupBehavior after the Report View is fully loaded via the event that gets sent.
		public FlowDocument ReportDocument {
			get { return ConvertReportResponseToFlowDocument(ReportData.ReportResponse); }
		}

		public Markup ReportMarkup {
			get { return ReportData.ReportMarkup; }
		}

		public GetReportResponse GetReportResponse {
			get { return ReportData.ReportResponse; }
			set {
				if (GetReportResponse != null)
					return;

				ReportData.ReportResponse = value;
				Title = ReportData.ReportResponse.ShortName;
				InitializeReportView();
			}
		}

		public SendChainInfoToANBKMessage TmpMsg { get; set; }

		[ImportingConstructor]
		public ReportViewModel(IFSMSystem fsmSystem, ReportData reportData) {
			StateMachine = fsmSystem;
			ReportData = reportData;
			ReportSavedMessage.Register(this, msg => ReportSaved(msg.SaveReportResponse));
			ReportModifiedMessage.Register(this, msg => ReportModified(msg.ReportMarkup));
		}

		private void ReportModified(Markup reportMarkup) {
			if (reportMarkup != ReportData.ReportMarkup)
				return;
			Modified = true;
		}

		private void ReportSaved(SaveReportResponse saveReportResponse) {
			if (saveReportResponse.resultCode != 1 ||
					saveReportResponse.resourceId != ReportData.ReportResponse.report.metadata.resourceId)
				return;
			Modified = false;
		}

		private void InitializeReportView() {
			ReportData.ReportResponse = GetReportResponse;

			if (GetReportResponse.report.Markup != null)
				ReportData.ReportMarkup = GetReportResponse.report.Markup;

			var waitingForLeftFragmentState = new WaitForLeftFragmentState(ReportMarkup);
			waitingForLeftFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForCenterFragment);
			waitingForLeftFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForLeftFragment);

			var waitingForCenterFragmentState = new WaitForCenterFragmentState(ReportMarkup);
			waitingForCenterFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForRightFragment);
			waitingForCenterFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForLeftFragment);

			var waitingForRightFragmentState = new WaitForRightFragmentState(ReportMarkup);
			waitingForRightFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForLeftFragment);
			waitingForRightFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForCenterFragment);


			StateMachine.Start(new List<FSMState> {
				waitingForLeftFragmentState,
				waitingForCenterFragmentState,
				waitingForRightFragmentState
			},
				waitingForLeftFragmentState);

			PerformStateTransitionMessage.Register(this,
				msg => PerformStateTransition(msg.Sender as FragmentStateBase, msg.Transition));
		}

		private void PerformStateTransition(FragmentStateBase state, Transition transition) {
			if (state.Markup != ReportMarkup)
				return;
			StateMachine.PerformTransition(transition);
		}

		private static FlowDocument ConvertReportResponseToFlowDocument(GetReportResponse reportResponse) {
			var blocks = new List<Block>();

			foreach (var textObj in reportResponse.report.texts) {
				var paragraph = new Paragraph();

				var run = new Run(textObj.text) {
					Tag = textObj
				};

				paragraph.Inlines.Add(run);

				blocks.Add(paragraph);
			}
			var flowDoc = new FlowDocument();
			flowDoc.Blocks.AddRange(blocks);
			return flowDoc;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}