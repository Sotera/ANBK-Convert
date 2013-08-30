using JistBridge.Messages.ANBK;

namespace JistBridge.UI.ANBKChart {
	public partial class ANBKChart {
		private readonly ANBKContainer _anbkContainer;

		public ANBKChart() {
			DataContextChanged += ANBKChart_DataContextChanged;
			InitializeComponent();

			_anbkContainer = new ANBKContainer();
			WindowsFormsHost.Child = _anbkContainer;
		}

		void ANBKChart_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			SendChainInfoToANBKMessage.Register(this, DataContext, msg => {
				switch (msg.Action) {
					case (SendChainInfoToANBKMessage.ActionType.CreateNewChain): {
						_anbkContainer.AddUninitializedChain(out msg.LeftNodeId, out msg.LinkId, out msg.RightNodeId);
						msg.Execute(msg);
						break;
					}
					case (SendChainInfoToANBKMessage.ActionType.ModifyLeftNodeText): {
						_anbkContainer.ModifyChartItemLabel(msg.LeftNodeId, msg.Text);
						break;
					}
					case (SendChainInfoToANBKMessage.ActionType.ModifyLinkText): {
						_anbkContainer.ModifyChartItemLabel(msg.LinkId, msg.Text);
						break;
					}
					case (SendChainInfoToANBKMessage.ActionType.ModifyRightNodeText): {
						_anbkContainer.ModifyChartItemLabel(msg.RightNodeId, msg.Text);
						break;
					}
				}
			});
		}
	}
}