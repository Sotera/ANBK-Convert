using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        [ImportingConstructor]
        public ReportViewModel(IFSMSystem fsmSystem )
		{
            var waitingForLeftFragmentState = new WaitForLeftFragmentState();
            waitingForLeftFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForCenterFragment);

            var waitingForCenterFragmentState = new WaitForCenterFragmentState();
            waitingForCenterFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForRightFragment);

            var waitingForRightFragmentState = new WaitForRightFragmentState();
            waitingForRightFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForLeftFragment);

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
