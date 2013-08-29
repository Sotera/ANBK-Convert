using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using JistBridge.Data.Model;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI.ReportView.States;
using JistBridge.Utilities.StateMachine;
using NLog;

namespace JistBridge.UI.ReportView
{
    [Export(typeof(IReportViewModel))]
    public class ReportViewModel : IReportViewModel
    {
        public IFSMSystem StateMachine { get; private set; }
        public ReportData ReportData { get; private set; }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        //This is not bound, the RichTextBox wont let you.  I set the flow document in the
        //ApplyMarkupBehavior after the Report View is fully loaded via the event that gets sent.
        public FlowDocument ReportDocument
        {
            get { return ConvertReportResponseToFlowDocument(ReportData.ReportResponse); }
        }

        public Markup ReportMarkup
        {
            get { return ReportData.ReportMarkup; }
        }

        public GetReportResponse GetReportResponse
        {
            get
            {
                return ReportData.ReportResponse;
            }
            set
            {
                if (GetReportResponse != null)
                    return;

                ReportData.ReportResponse = value;
                InitializeReportView();
            }
        }

        [ImportingConstructor]
        public ReportViewModel(IFSMSystem fsmSystem, ReportData reportData)
        {
            StateMachine = fsmSystem;
            ReportData = reportData;

        }

        private void InitializeReportView()
        {
            ReportData.ReportResponse = GetReportResponse;

            if(GetReportResponse.report.Markup != null)
                ReportData.ReportMarkup = GetReportResponse.report.Markup;

            var waitingForLeftFragmentState = new WaitForLeftFragmentState(ReportMarkup);
            waitingForLeftFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForCenterFragment);
            waitingForLeftFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForLeftFragment);

            var waitingForCenterFragmentState = new WaitForCenterFragmentState(ReportMarkup);
            waitingForCenterFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForRightFragment);
            waitingForCenterFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForLeftFragment);

            var waitingForRightFragmentState = new WaitForRightFragmentState(ReportMarkup);
            waitingForRightFragmentState.AddTransition(Transition.RecievedFragment, StateID.WaitingForLeftFragment);
            waitingForRightFragmentState.AddTransition(Transition.Cancel, StateID.WaitingForCenterFragment);


            StateMachine.Start(new List<FSMState>
            {
                waitingForLeftFragmentState,
                waitingForCenterFragmentState,
                waitingForRightFragmentState
            },
                waitingForLeftFragmentState);

            PerformStateTransitionMessage.Register(this,
                msg => PerformStateTransition(msg.Sender as FragmentStateBase, msg.Transition));
        }

        private void PerformStateTransition(FragmentStateBase state, Transition transition)
        {
            if (state.Markup != ReportMarkup)
            {
                return;
            }
            StateMachine.PerformTransition(transition);
        }

        private static FlowDocument ConvertReportResponseToFlowDocument(GetReportResponse reportResponse)
        {
            var blocks = new List<Block>();

            foreach (var textObj in reportResponse.report.texts)
            {
                var paragraph = new Paragraph();

                var run = new Run(textObj.text)
                {
                    Tag = textObj
                };

                paragraph.Inlines.Add(run);

                blocks.Add(paragraph);
            }
            var flowDoc = new FlowDocument();
            flowDoc.Blocks.AddRange(blocks);
            return flowDoc;
        }
    }
}