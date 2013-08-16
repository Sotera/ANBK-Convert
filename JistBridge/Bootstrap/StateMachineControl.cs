using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Utilities.StateMachine;
using JistBridge.Utilities.StateMachine.States;

namespace JistBridge.Bootstrap
{
    [Export(typeof (IBootstrapTask))]
    internal class StateMachineControl : IBootstrapTask
    {
        public FSMSystem StateMachine { get; private set; }
        internal StateMachineControl()
        {
            var waitingForLeftFragmentState = new WaitForLeftFragmentState();
            waitingForLeftFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForCenterFragment);

            var waitingForCenterFragmentState = new WaitForCenterFragmentState();
            waitingForCenterFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForRightFragment);

            var waitingForRightFragmentState = new WaitForRightFragmentState();
            waitingForRightFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForLeftFragment);

            StateMachine = new FSMSystem();
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
