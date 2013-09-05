using System.Windows;
using JistBridge.Messages;
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

		private void ANBKChart_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			ChangePointerModeMessage.Register(this,DataContext,
				msg => {
					switch (msg.FitType) {
						case PointerType.Pointer:
							_anbkContainer.ChangePointerMode_Pointer();
							break;
						case PointerType.Drag:
							_anbkContainer.ChangePointerMode_Drag();
							break;
					}
				});
			FitChartMessage.Register(this, DataContext,
				msg => {
					switch (msg.FitType) {
						case FitType.ActualSize:
							_anbkContainer.FitActualSize();
							break;
						case FitType.Height:
							_anbkContainer.FitHeight();
							break;
						case FitType.SelectionInWindow:
							_anbkContainer.FitSeletionInWindow();
							break;
						case FitType.Window:
							_anbkContainer.FitWindow();
							break;
					}
				});
			ChangeLayoutMessage.Register(this, DataContext,
				msg => {
					switch (msg.LayoutType) {
						case LayoutType.Circle:
							_anbkContainer.DoCircleLayout();
							break;
						case LayoutType.Peacock:
							_anbkContainer.DoPeacockLayout();
							break;
						case LayoutType.Group:
							_anbkContainer.DoGroupLayout();
							break;
					}
				});
			SendChainInfoToANBKMessage.Register(this, DataContext, msg => {
				switch (msg.Action) {
					case (SendChainInfoToANBKMessage.ActionType.CreateNewChain): {
                            _anbkContainer.AddInitializedChain(msg.Chain, msg.Fields,msg.MetaData);
                            msg.Execute(msg);
                            break;
                        }
					case (SendChainInfoToANBKMessage.ActionType.ModifyLeftNodeText): {
                            _anbkContainer.ModifyChartItemLabel(msg.Chain.Left.AnalystNotebookIdentity, msg.Chain.Left.DisplayText);
                            break;
                        }
					case (SendChainInfoToANBKMessage.ActionType.ModifyLinkText): {
						_anbkContainer.ModifyChartLinkLabel(msg.Chain.Left.AnalystNotebookIdentity,
							msg.Chain.Center.AnalystNotebookIdentity, msg.Chain.Center.DisplayText);
                            break;
                        }
					case (SendChainInfoToANBKMessage.ActionType.ModifyRightNodeText): {
                            _anbkContainer.ModifyChartItemLabel(msg.Chain.Right.AnalystNotebookIdentity, msg.Chain.Right.DisplayText);
                            break;
                        }
                }
            });
        }
    }
}