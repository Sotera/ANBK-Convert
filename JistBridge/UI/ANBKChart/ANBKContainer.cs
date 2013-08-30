using System.Linq;
using System.Windows.Forms;
using Interop.i2NotebookConnector;
using Interop.i2NotebookData;

namespace JistBridge.UI.ANBKChart {
	public partial class ANBKContainer : UserControl {
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
		}

		public void ModifyChartItemLabel(int id, string text) {
			var chartItem = _chart.FindChartItemById(id);
			chartItem.Label = text;
		}

		public void AddUninitializedChain(out int leftNodeId, out int linkId, out int rightNodeId) {
			const string unknownText = "???";
			var type = _chart.EntityTypes.Find("Query");
			var style = _chart.CreateIconStyle();
			style.Type = type;

			int xLeft;
			int xRight;
			int y;
			GetLocationsForNewChain(out xLeft, out xRight, out y);
			var leftEnd = (LNEnd) _chart.CreateIcon(style, xLeft, y, unknownText, _chart.GenerateUniqueIdentity());
			var rightEnd = (LNEnd) _chart.CreateIcon(style, xRight, y, unknownText, _chart.GenerateUniqueIdentity());

			var linkStyle = _chart.CreateLinkStyle();
			linkStyle.LineWidth = 4;
			var link = _chart.CreateLink(linkStyle, leftEnd, rightEnd, unknownText);
			leftNodeId = leftEnd.Id;
			rightNodeId = rightEnd.Id;
			linkId = link.Id;
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
				var top = int.MaxValue;
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
					if (t < top) top = t;
					if (l < left) left = l;
				}
				xLeft = 50;
				xRight = _view.ViewX + (_view.ViewWidth - 50);
				y = top + 100;
			}
		}
	}
}