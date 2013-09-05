using System.Linq;
using System.Windows.Forms;
using Interop.i2NotebookConnector;
using Interop.i2NotebookData;
using JistBridge.Data.Model;
using NLog;

namespace JistBridge.UI.ANBKChart {
	public partial class ANBKContainer : UserControl {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private LNConnector _connector;
		private readonly LNChart _chart;
		private readonly LNGraphicViewport2 _view;

		public ANBKContainer() {
			InitializeComponent();
			_connector = (LNConnector) axi2LinkConnector1.GetConnectorInterface();
			_chart = (LNChart) axi2LinkData1.GetChartInterface();
			_view = (LNGraphicViewport2) axi2LinkView1.GetViewInterface();

			_connector.AddChart(_chart);
			_connector.AddView(_view);
			_connector.Connect(_chart, _view);

			_chart.Initialise();

			_chart.LoadChart(_connector.Options.StandardTemplateFile);
			_chart.ReleaseFile();

			_chart.BackColour = 0xf0fff0;

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


		public void AddInitializedChain(Chain chain) {
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

			var rightEnd = string.IsNullOrEmpty(chain.Right.AnalystNotebookIdentity)
				? _chart.CreateIcon(style, xRight, y, unknownText, _chart.GenerateUniqueIdentity())
				: GetEnd(chain.Right.AnalystNotebookIdentity);
			chain.Right.AnalystNotebookIdentity = GetEndIdentity(rightEnd);
			rightEnd.Label = chain.Right.DisplayText;

			var linkStyle = _chart.CreateLinkStyle();
			linkStyle.LineWidth = 4;
			var link = _chart.CreateLink(linkStyle, leftEnd, rightEnd, unknownText);
			chain.Center.AnalystNotebookIdentity = link.GuidId;
			link.Label = chain.Center.DisplayText;
		}

		private static string GetEndIdentity(LNEnd end) {
			var entity = (end as LNEntity);
			if (entity != null)
				return entity.Identity;
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
	}
}