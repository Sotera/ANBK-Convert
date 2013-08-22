using JistBridge.Data.Model;

namespace JistBridge.Interfaces
{
    public interface IChainCanvasViewModel
    {
        Fragment LeftFragment
        { get; set; }

        Fragment CenterFragment
        { get; set; }

        Fragment RightFragment
        { get; set; }
    }
}
