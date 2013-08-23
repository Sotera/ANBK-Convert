using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace JistBridge.Interfaces {
	public interface IMainWindowViewModel {
		void SetLayoutDocumentInfo(LayoutRoot layoutRoot, LayoutDocumentPaneGroup layoutDocumentPaneGroup);
	}
}