using System.Diagnostics;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.Utilities.StateMachine;

namespace JistBridge.UI.ReportView.States
{

    public class WaitForCenterFragmentState : FragmentStateBase
    {
        public WaitForCenterFragmentState(Markup markup)
        {
            Markup = markup;
            stateID = StateID.WaitingForCenterFragment;
        }

        protected override void HandleFragmentSelected(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup != Markup)
                return;

            base.HandleFragmentSelected(markup, fragment, status);

            if (markup.CurrentChain.Center != null)
            {
                Debug.WriteLine("ERROR: WaitForCenterFragmentState Got a chain with a non null center :(");
                return;
            }

            markup.CurrentChain.Add(fragment);
            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.CenterFragmentAdded).Send();
            new PerformStateTransitionMessage(this, null, Transition.RecievedFragment).Send();
        }

        protected override void HandleCancelFragment(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup != Markup)
                return;

            base.HandleCancelFragment(markup, fragment, status);

            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.CenterFragmentCanceled).Send();
            markup.CurrentChain = null;

            new PerformStateTransitionMessage(this, null, Transition.Cancel).Send();
        }

    }
}
