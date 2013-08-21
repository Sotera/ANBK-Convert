using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.Utilities.StateMachine;

namespace JistBridge.UI.ReportView.States
{
    public class FragmentStateBase : FSMState
    {
        public Markup Markup { get; protected set; }
        protected virtual void HandleFragmentSelected(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup == null || markup != Markup || fragment == null)
                return;
        }

        protected virtual void HandleCancelFragment(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup == null || markup != Markup)
                return;
        }

        private void HandleFragmentStatus(Markup markup, Fragment fragment, FragmentStatus status)
        {
            if (markup == null || markup != Markup)
                return;

            switch (status)
            {
                case FragmentStatus.Canceled:
                    {
                        HandleCancelFragment(markup, fragment, status);
                        break;
                    }
                case FragmentStatus.Selected:
                    {
                        HandleFragmentSelected(markup, fragment, status);
                        break;
                    }
            }
        }

        public override void DoBeforeEntering()
        {
            base.DoBeforeEntering();
            FragmentStatusMessage.Register(this, msg => HandleFragmentStatus(msg.Markup, msg.Fragment, msg.Status));
        }


        public override void DoBeforeLeaving()
        {
            base.DoBeforeLeaving();
            FragmentStatusMessage.Unregister(this);
        }
    }
}
