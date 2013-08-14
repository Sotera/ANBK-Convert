namespace JistBridge.Messages{
	public class QueueMefComposeMessage : BaseMessage<QueueMefComposeMessage>{
		public object MefTarget { get; set; }

		public QueueMefComposeMessage(object sender, object target, object mefTarget)
			: base(sender, target){
			MefTarget = mefTarget;
		}
	}
}