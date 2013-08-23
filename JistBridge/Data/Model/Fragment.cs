using System.Collections.Generic;
using System.ComponentModel;

namespace JistBridge.Data.Model
{
    public enum FragmentType
    {
        Node,
        Link
    }

    public class Fragment
    {
        [Browsable(false)]
        public List<Range<int>> Offsets { get; set; }
        [Browsable(false)]
        public FragmentType FragmentType { get; set; }
        [Browsable(false)]
        public string AnalystNotebookId { get; set; }
        public string DisplayText { get; set; }
        [Browsable(false)]
        public int SourceOffset { get; set; }

        public Fragment(List<Range<int>> offsets, FragmentType fragmentType, string displayText, int sourceOffset)
        {
            Offsets = offsets;
            FragmentType = fragmentType;
            DisplayText = displayText;
            SourceOffset = sourceOffset;
        }

        //Straight up consume without comparing...
        //This should be ok but we may want to check each range for some reason.
        public void Consume(Fragment loser)
        {
            Offsets.AddRange(loser.Offsets);
        }

        public bool ContainsFragment(Fragment fragment)
        {
            foreach (var offset in fragment.Offsets)
            {
                if (!ContainsOffset(offset))
                    return false;
            }

            return true;
        }

        public bool ContainsOffset(Range<int> offset)
        {
            foreach (var localOffset in Offsets)
            {
                if (localOffset.ContainsRange(offset))
                    return true;
            }
            return false;
        }


    }
}
