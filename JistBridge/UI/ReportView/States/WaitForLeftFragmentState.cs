using System.Diagnostics;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.Utilities.StateMachine;

namespace JistBridge.UI.ReportView.States
{
    class WaitForLeftFragmentState: FragmentStateBase
    {
        public WaitForLeftFragmentState(Markup markup)
        {
            Markup = markup;
            stateID = StateID.WaitingForLeftFragment;
        }

        protected override void HandleFragmentSelected(Markup markup, Fragment fragment, FragmentStatus status)
        {
            base.HandleFragmentSelected(markup, fragment, status);

            if (markup.CurrentChain != null)
            {
                Debug.WriteLine("Error: WaitForLeftFragmentState encountered a non Null CurrentChain.");
                return;
            }
            markup.CurrentChain = new Chain(fragment, null, null);
            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.LeftFragmentAdded).Send();
            new PerformStateTransitionMessage(this, null, Transition.RecievedFragment).Send();
        }

        protected override void HandleCancelFragment(Markup markup, Fragment fragment, FragmentStatus status)
        {
            base.HandleCancelFragment(markup, fragment, status);

            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.LeftFragmentCanceled).Send();
            markup.CurrentChain = null;
                        
            new PerformStateTransitionMessage(this, null, Transition.Cancel).Send();
        }
    }
}
