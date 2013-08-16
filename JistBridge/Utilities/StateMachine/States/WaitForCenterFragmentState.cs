using System.Diagnostics;
using JistBridge.Data.Model;
using JistBridge.Messages;

namespace JistBridge.Utilities.StateMachine.States
{
    
    public class WaitForCenterFragmentState : FSMState
    {
        public WaitForCenterFragmentState()
        {
            stateID = StateID.WaitingForCenterFragment;
        }

        private void HandleFragmentSelected(Markup markup, Fragment fragment)
        {
            if (markup == null || fragment == null)
                return;

            if (markup.CurrentChain.Center != null)
            {
                Debug.WriteLine("ERROR: WaitForCenterFragmentState Got a chain with a non null center :(");
                return;
            }
            
            markup.CurrentChain.Add(fragment);
            new ChainStatusMessage(this, null, markup.MarkupId, markup.CurrentChain, ChainStatus.CenterFragmentAdded).Send();
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
