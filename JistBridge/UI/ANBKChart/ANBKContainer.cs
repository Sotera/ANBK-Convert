using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using Interop.i2NotebookConnector;
using Interop.i2NotebookData;
using JistBridge.Data.Model;
using JistBridge.Data.ReST;
using NLog;
namespace JistBridge.UI.ANBKChart
{
    public partial class ANBKContainer : UserControl
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private LNConnector _connector;
        private readonly LNChart _chart;
        private readonly LNGraphicViewport2 _view;
        private const string SourceReference = "JIST";

        
        public ANBKContainer()
        {
            InitializeComponent();
            _connector = (LNConnector)axi2LinkConnector1.GetConnectorInterface();
            _chart = (LNChart)axi2LinkData1.GetChartInterface();
            _view = (LNGraphicViewport2)axi2LinkView1.GetViewInterface();

            _connector.AddChart(_chart);
            _connector.AddView(_view);
            _connector.Connect(_chart, _view);

            _chart.Initialise();

            _chart.LoadChart(_connector.Options.StandardTemplateFile);
            _chart.ReleaseFile();

            _connector.CurrentChart = _chart;

            _view.ScrollTo(0, 0);
            _view.Scaling = 1;

			//Need to hook up layouts to the chart (though this should happen automagically through
			//this component container)
			axi2LinkCircleLayout1.Chart = _chart;
			axi2LinkPeacockLayout1.Chart = _chart;
			axi2LinkGroupLayout1.Chart = _chart;
        }

		public void ChangePointerMode_Pointer() {
			_view.Commands.CallCommand((int)ViewCommands.i2SetEditMode);
		}

		public void ChangePointerMode_Drag() {
			_view.Commands.CallCommand((int)ViewCommands.i2SetDragMode);
		}

		public void FitActualSize() {
			_view.Commands.CallCommand((int)ViewCommands.i2SmoothActualSize);
		}

		public void FitHeight() {
			_view.Commands.CallCommand((int)ViewCommands.i2SmoothFitHeightInWindow);
		}

		public void FitWindow() {
			_view.Commands.CallCommand((int)ViewCommands.i2SmoothFitChartInWindow);
		}

		public void FitSeletionInWindow() {
			_view.Commands.CallCommand((int)ViewCommands.i2SmoothFitSelectionInWindow);
		}

		public void DoCircleLayout() {
			((Ii2SyncCommand) axi2LinkCircleLayout1.GetOcx()).DoCommand();
		}

		public void DoPeacockLayout() {
			((Ii2SyncCommand) axi2LinkPeacockLayout1.GetOcx()).DoCommand();
		}

		public void DoGroupLayout() {
			((Ii2SyncCommand) axi2LinkGroupLayout1.GetOcx()).DoCommand();
		}

		public void ModifyChartItemLabel(string id, string text) {
            var chartItem = GetEnd(id);
			if (chartItem == null) {
                Log.Error("Tried to find entity in chart and could not find it : " + id); 
                return;
            }
            chartItem.Label = text;
        }

		public void ModifyChartLinkLabel(string endIdentity, string linkGuidId, string displayText) {
            var chartItem = GetEnd(endIdentity);
			if (chartItem == null) {
                Log.Error("Tried to find entity in chart and could not find it : " + endIdentity);
                return;
            }

			foreach (var link in chartItem.Links.Cast<LNLink>().Where(link => link.GuidId == linkGuidId)) {
                link.Label = displayText;
                return;
            }

            Log.Error("Tried to find Link on end build could not find it : " + linkGuidId);
        }

        private void AddAttributesToItem(LNChartItem item, Dictionary<string, string> fields,
            GetReportResponse.CReport.CMetadata metadata)
        {
            if (item == null)
                return;
            
            foreach (var key in fields.Keys)
            {
                AddAttribute(item,key,fields[key]);
            }

            AddAttribute(item, GetPropertyName(() => metadata.offsetField), metadata.offsetField);
            AddAttribute(item, GetPropertyName(() => metadata.resourceField), metadata.resourceField);
            AddAttribute(item, GetPropertyName(() => metadata.resourceId),  metadata.resourceId);
            AddAttribute(item, GetPropertyName(() => metadata.textField),  metadata.textField);
            
        }

        
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            return memberExpression != null ? memberExpression.Member.Name : null;
        }

        private void AddAttribute(LNChartItem item, string className, string value)
        {
            var attribute = GetJistAttributeClass(className);
            item.AttributeValue[attribute] = value;
        }

        private LNAttributeClass GetJistAttributeClass(string className)
        {
            LNAttributeClass objAttributeClass = null;
            try
            {
                objAttributeClass = _chart.AttributeClasses.Find(className);
                if (objAttributeClass != null)
                    return objAttributeClass;
                objAttributeClass = _chart.CreateAttributeClass(className, AttributeType.AttText, "Blackx", false, false, false);
                objAttributeClass.Visible = false;
            }
            catch (Exception e)
            {
                Log.ErrorException("Error adding attribute", e);
                throw;
            }
            
            return objAttributeClass;
        }

        


        public void AddInitializedChain(Chain chain, Dictionary<string,string> fields, GetReportResponse.CReport.CMetadata metadata )
        {
            const string unknownText = "???";
            var type = _chart.EntityTypes.Find("Query");
            var style = _chart.CreateIconStyle();
            style.Type = type;

            int xLeft;
            int xRight;
            int y;
            GetLocationsForNewChain(out xLeft, out xRight, out y);

			var leftEnd = string.IsNullOrEmpty(chain.Left.AnalystNotebookIdentity)
				? _chart.CreateIcon(style, xLeft, y, unknownText, _chart.GenerateUniqueIdentity())
				: GetEnd(chain.Left.AnalystNotebookIdentity);
            chain.Left.AnalystNotebookIdentity = GetEndIdentity(leftEnd);
            leftEnd.Label = chain.Left.DisplayText;
            AddAttributesToItem(leftEnd,fields,metadata);

			var rightEnd = string.IsNullOrEmpty(chain.Right.AnalystNotebookIdentity)
				? _chart.CreateIcon(style, xRight, y, unknownText, _chart.GenerateUniqueIdentity())
				: GetEnd(chain.Right.AnalystNotebookIdentity);
            chain.Right.AnalystNotebookIdentity = GetEndIdentity(rightEnd);
            rightEnd.Label = chain.Right.DisplayText;
            AddAttributesToItem(rightEnd, fields, metadata);

            var linkStyle = _chart.CreateLinkStyle();
            linkStyle.LineWidth = 4;
            var link = _chart.CreateLink(linkStyle, leftEnd, rightEnd, unknownText);
            chain.Center.AnalystNotebookIdentity = link.GuidId;
            link.Label = chain.Center.DisplayText;
            AddAttributesToItem(link, fields, metadata);
            
        }

		private static string GetEndIdentity(LNEnd end) {
            var entity = (end as LNEntity);
            if (entity != null)
                return  entity.Identity;
                Log.Error("Left end node could not be converted to an LNEntity.");
            return null;
        }

		private LNEnd GetEnd(string endId) {
            return string.IsNullOrEmpty(endId) ? null : _chart.FindEntityByIdentity(endId);
        }

		private void GetLocationsForNewChain(out int xLeft, out int xRight, out int y) {
            xLeft = 50;
            xRight = 350;
            y = 50;

            //Go through all entities and make a bounding box
            var list = _chart.Ends[ChartItemSelectionMask.ChartItemsAll, 0]
                .Cast<object>()
                .Cast<LNEnd>()
                .ToArray();

			if (list.Length == 0) {
                xLeft = 50;
                xRight = _view.ViewX + (_view.ViewWidth - 50);
            }
			else {
                var top = int.MinValue;
                var bottom = int.MinValue;
                var left = int.MaxValue;
                var right = int.MinValue;
				foreach (var item in list) {
                    var l = 0;
                    var t = 0;
                    var width = 0;
                    var height = 0;
                    item.GetExtent(ref l, ref t, ref width, ref height);
                    var r = l + width;
                    var b = t + height;
                    //Quick and dirty for now using only left,top
                    if (t > top) top = t;
                    if (l < left) left = l;
                }
                xLeft = 50;
                xRight = _view.ViewX + (_view.ViewWidth - 50);
                y = top + 100;
            }
        }

        public void SaveChart()
        {
            var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                            "\\JISTBridge";
            const string filename = "\\ANBK.anb";
            try
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                _chart.SaveChart(directory + filename);
                _chart.ReleaseFile();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            
        }
    }
}