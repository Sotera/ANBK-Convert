using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using JistBridge.Data.Model;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI.ReportView.States;
using JistBridge.Utilities.StateMachine;
using JistBridge.Utilities.StateMachine.States;

namespace JistBridge.UI.ReportView {
	[Export(typeof (IReportViewModel))]
	public class ReportViewModel : IReportViewModel {
		public IFSMSystem StateMachine { get; private set; }
		public Report Report { get; private set; }

		//This is not bound, the RichTextBox wont let you.  I set the flow document in the
		//ApplyMarkupBehavior after the Report View is fully loaded via the event that gets sent.
		public FlowDocument ReportDocument {
			get { return ConvertReportResponseToFlowDocument(Report.ReportResponse); }
		}

		public Markup ReportMarkup {
			get { return Report.ReportMarkup; }
		}

		public GetReportResponse GetReportResponse {
			get {
				return Report.ReportResponse;
			}
			set {
				Report.ReportResponse = value;
			}
		}

		[ImportingConstructor]
		public ReportViewModel(IFSMSystem fsmSystem, Report report) {
			Report = report;
			var waitingForLeftFragmentState = new WaitForLeftFragmentState(ReportMarkup);
			waitingForLeftFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForCenterFragment);
			waitingForLeftFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForLeftFragment);

			var waitingForCenterFragmentState = new WaitForCenterFragmentState(ReportMarkup);
			waitingForCenterFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForRightFragment);
			waitingForCenterFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForLeftFragment);

			var waitingForRightFragmentState = new WaitForRightFragmentState(ReportMarkup);
			waitingForRightFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForLeftFragment);
			waitingForRightFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForCenterFragment);

			StateMachine = fsmSystem;
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
			if (state.Markup != ReportMarkup) {
				return;
			}
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
	}
}