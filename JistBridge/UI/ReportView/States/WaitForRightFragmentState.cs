using System.Diagnostics;
using JistBridge.Data.Model;
using JistBridge.Messages;

namespace JistBridge.Utilities.StateMachine.States
{

    public class WaitForRightFragmentState : FSMState
    {
        public WaitForRightFragmentState()
        {
            stateID = StateID.WaitingForRightFragment;
        }

        private void HandleFragmentSelected(Markup markup, Fragment fragment)
        {
            if (markup == null || fragment == null)
                return;

            if (markup.CurrentChain.Center == null || markup.CurrentChain.Left == null)
            {
                Debug.WriteLine("ERROR: WaitForRightFragmentState somehow got a Chain with null parts :(");
                return;
            }

            markup.CurrentChain.Add(fragment);
            new ChainStatusMessage(this, null, markup.MarkupId, markup.CurrentChain, ChainStatus.RightFragmentAdded).Send();

            markup.CurrentChain = null;
            new PerformStateTransitionMessage(this, null, Transition.RecievedFragment).Send();
        }

        public override void DoBeforeEntering()
        {
            base.DoBeforeEntering();
            FragmentSelectedMessage.Register(this, msg => HandleFragmentSelected(msg.Markup, msg.Fragment));
        }


        public override void DoBeforeLeaving()
        {
            base.DoBeforeLeaving();
            FragmentSelectedMessage.Unregister(this);
        }
    }
}
