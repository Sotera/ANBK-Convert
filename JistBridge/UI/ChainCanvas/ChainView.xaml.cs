using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.ChainCanvas
{
    /// <summary>
    /// Interaction logic for ChainView.xaml
    /// </summary>
    public partial class ChainView
    {
        public ChainView()
        {
            InitializeComponent();
            new QueueMefComposeMessage(null, null, this, null).Send();
        }

        [Import]
        public IChainCanvasViewModel MainWindowViewModel
        {
            set
            {
                DataContext = value;
            }
        }
    }
}
