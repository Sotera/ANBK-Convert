using JistBridge.Data.Model;
using JistBridge.Messages.ANBK;

namespace JistBridge.UI.ANBKChart
{
    public partial class ANBKChart
    {
        private readonly ANBKContainer _anbkContainer;

        public ANBKChart()
        {
            DataContextChanged += ANBKChart_DataContextChanged;
            InitializeComponent();

            _anbkContainer = new ANBKContainer();
            WindowsFormsHost.Child = _anbkContainer;
        }

        void ANBKChart_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            SendChainInfoToANBKMessage.Register(this, DataContext, msg =>
            {
                switch (msg.Action)
                {
                    case (SendChainInfoToANBKMessage.ActionType.CreateNewChain):
                        {
                            _anbkContainer.AddInitializedChain(msg.Chain);
                            msg.Execute(msg);
                            break;
                        }
                    case (SendChainInfoToANBKMessage.ActionType.ModifyLeftNodeText):
                        {
                            _anbkContainer.ModifyChartItemLabel(msg.Chain.Left.AnalystNotebookIdentity, msg.Chain.Left.DisplayText);
                            break;
                        }
                    case (SendChainInfoToANBKMessage.ActionType.ModifyLinkText):
                        {
                            _anbkContainer.ModifyChartLinkLabel(msg.Chain.Left.AnalystNotebookIdentity,msg.Chain.Center.AnalystNotebookIdentity, msg.Chain.Center.DisplayText);
                            break;
                        }
                    case (SendChainInfoToANBKMessage.ActionType.ModifyRightNodeText):
                        {
                            _anbkContainer.ModifyChartItemLabel(msg.Chain.Right.AnalystNotebookIdentity, msg.Chain.Right.DisplayText);
                            break;
                        }
                }
            });
        }
    }
}