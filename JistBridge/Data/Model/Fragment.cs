using System.Collections.Generic;

namespace JistBridge.Data.Model
{
    public enum FragmentType
    {
        Node,
        Link
    }

    public class Fragment
    {
        public List<Range<int>> Offsets { get; set; }
        public FragmentType FragmentType { get; set; }
        public string AnalystNotebookId { get; set; }
        public string DisplayText { get; set; }

        public Fragment(List<Range<int>> offsets, FragmentType fragmentType, string displayText)
        {
            Offsets = offsets;
            FragmentType = fragmentType;
            DisplayText = displayText;
        }

        //Straight up consume without comparing...
        //This should be ok but we may want to check each range for some reason.
        public void Consume(Fragment loser)
        {
            Offsets.AddRange(loser.Offsets);
        }
    }
}
