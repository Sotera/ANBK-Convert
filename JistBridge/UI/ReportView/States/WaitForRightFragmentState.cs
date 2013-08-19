using System.Diagnostics;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI.ReportView.States;

namespace JistBridge.Utilities.StateMachine.States
{

    public class WaitForRightFragmentState : FragmentStateBase
    {
        public WaitForRightFragmentState()
        {
            stateID = StateID.WaitingForRightFragment;
        }

        protected override void HandleFragmentSelected(Markup markup, Fragment fragment, FragmentStatus status)
        {
            base.HandleFragmentSelected(markup, fragment, status);

            if (markup.CurrentChain.Center == null || markup.CurrentChain.Left == null)
            {
                Debug.WriteLine("ERROR: WaitForRightFragmentState somehow got a Chain with null parts :(");
                return;
            }

            markup.CurrentChain.Add(fragment);
            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.RightFragmentAdded).Send();

            markup.CurrentChain = null;
            new PerformStateTransitionMessage(this, null, Transition.RecievedFragment).Send();
        }

        protected override void HandleCancelFragment(Markup markup, Fragment fragment, FragmentStatus status)
        {
            base.HandleCancelFragment(markup, fragment, status);
            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.RightFragmentCanceled).Send();
            new PerformStateTransitionMessage(this, null, Transition.Cancel).Send();
        }

    }
}
