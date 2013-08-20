using System.Collections.Generic;
using System.ComponentModel.Composition;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI.ReportView.States;
using JistBridge.Utilities.StateMachine;
using JistBridge.Utilities.StateMachine.States;

namespace JistBridge.UI.ReportView
{

    [Export(typeof(IReportViewModel))]
    public class ReportViewModel: IReportViewModel
    {
        public IFSMSystem StateMachine { get; private set; }
        private Report _report;

        public const string ReportContentsPropertyName = "ReportContents";

        public string ReportContents
        {
            get { return _report.ReportText; }
        }

        public Markup ReportMarkup
        {
            get { return _report.ReportMarkup; }
        }

        [ImportingConstructor]
        public ReportViewModel(IFSMSystem fsmSystem, IReportService reportService)
		{
            reportService.GetReport(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    _report = item;
                });

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
            StateMachine.Start(new List<FSMState>()
                                {
                                    waitingForLeftFragmentState,
                                    waitingForCenterFragmentState,
                                    waitingForRightFragmentState
                                },
                                waitingForLeftFragmentState);

            PerformStateTransitionMessage.Register(this, msg => PerformStateTransition(msg.Transition));
		}

        private void PerformStateTransition(Transition transition)
        {
            StateMachine.PerformTransition(transition);
        }
    }
}
