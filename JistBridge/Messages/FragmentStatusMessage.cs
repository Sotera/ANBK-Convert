using JistBridge.Data.Model;

namespace JistBridge.Messages
{
    public enum FragmentStatus{Selected,Canceled}
    
    class FragmentStatusMessage : BaseMessage<FragmentStatusMessage>
    {
        public Fragment Fragment;

        public Markup Markup { get; private set; }

        public FragmentStatus Status { get; private set; }
        
        public FragmentStatusMessage(object sender, object target,Markup markup, Fragment fragment, FragmentStatus status)
			: base(sender, target)
        {
            Markup = markup;
            Fragment = fragment;
            Status = status;
        }

        
    }
}
