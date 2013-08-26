using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.Utilities.StateMachine;
using NLog;

namespace JistBridge.UI.ReportView.States
{

    public class WaitForRightFragmentState : FragmentStateBase
    {
        
        public WaitForRightFragmentState(Markup markup)
        {
            Markup = markup;
            stateID = StateID.WaitingForRightFragment;
            Log = LogManager.GetCurrentClassLogger();
        }

        protected override void HandleFragmentSelected(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup != Markup)
                return;

            base.HandleFragmentSelected(markup, fragment, status);

            if (markup.CurrentChain.Center == null || markup.CurrentChain.Left == null)
            {
                Log.Error("ERROR: WaitForRightFragmentState somehow got a Chain with null parts :(");
                return;
            }

            markup.CurrentChain.Add(fragment);
            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.RightFragmentAdded).Send();
            markup.AddChain(markup.CurrentChain);
            markup.CurrentChain = null;
            new PerformStateTransitionMessage(this, null, Transition.RecievedFragment).Send();
        }

        protected override void HandleCancelFragment(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup != Markup)
                return;

            base.HandleCancelFragment(markup, fragment, status);
            new ChainStatusMessage(this, null, markup, markup.CurrentChain, ChainStatus.RightFragmentCanceled).Send();
            new PerformStateTransitionMessage(this, null, Transition.Cancel).Send();
        }

    }
}
