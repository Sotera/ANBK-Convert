using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JistBridge.Data.Model;
using JistBridge.Messages;

namespace JistBridge.Utilities.StateMachine.States
{
    class WaitForLeftFragmentState: FSMState
    {
        public WaitForLeftFragmentState()
        {
            stateID = StateID.WaitingForLeftFragment;
        }

        private void HandleFragmentSelected(Markup markup, Fragment fragment)
        {
            if (markup == null || fragment == null)
                return;

            if (markup.CurrentChain != null)
            {
                Debug.WriteLine("Error: WaitForLeftFragmentState encountered a non Null CurrentChain.");
                return;
            }
            markup.CurrentChain = new Chain(fragment, null, null);
            new ChainStatusMessage(this, null, markup.MarkupId, markup.CurrentChain, ChainStatus.LeftFragmentAdded).Send();
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
