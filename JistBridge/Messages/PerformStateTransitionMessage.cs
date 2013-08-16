using JistBridge.Utilities.StateMachine;

namespace JistBridge.Messages
{
    public class PerformStateTransitionMessage : BaseMessage<PerformStateTransitionMessage>
    {
        public Transition Transition { get; private set; }
        public PerformStateTransitionMessage(object sender, object target, Transition transition)
			: base(sender, target)
        {
            Transition = transition;
        }
    }
}
