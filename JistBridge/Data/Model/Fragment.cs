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

        /// <summary>
        /// Used for serialization
        /// </summary>
        public Fragment()
        {
        }

        /// <summary>
        /// Should only be called from within the Markup class...The Markup class acts as a Fragment Factory
        /// </summary>
        /// <param name="offsets"></param>
        /// <param name="fragmentType"></param>
        /// <param name="displayText"></param>
        /// <param name="sourceOffset"></param>
        public Fragment(List<Range<int>> offsets, FragmentType fragmentType, string displayText, int sourceOffset)
        {
            Offsets = offsets;
            FragmentType = fragmentType;
            DisplayText = displayText;
            SourceOffset = sourceOffset;
        }

        /// <summary>
        ///Straight up consume loser without comparing...
        ///This should be ok but we may want to check each range for some reason.
        /// </summary>
        /// <param name="loser"></param>
        public void Consume(Fragment loser)
        {
            Offsets.AddRange(loser.Offsets);
        }

        /// <summary>
        /// Will check to see if  --ALL-- of the offsets int fragment are contained inside of this fragment.
        /// This can be an exact match or inside of a particular offset
        /// </summary>
        /// <param name="fragment"></param>
        /// <returns></returns>
        public bool ContainsFragment(Fragment fragment)
        {
            foreach (var offset in fragment.Offsets)
            {
                if (!ContainsOffset(offset))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks to see if offset is an exact match to or contained within this fragments offsets
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool ContainsOffset(Range<int> offset)
        {
            foreach (var localOffset in Offsets)
            {
                if (localOffset.ContainsRange(offset))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks to see if this fragment contains an offset that is exactly like offsetRange
        /// </summary>
        /// <param name="offsetRange"></param>
        /// <returns></returns>
        public bool ContainsExactRange(Range<int> offsetRange)
        {
            foreach (var localOffset in Offsets)
            {
                if (localOffset.Equals(offsetRange))
                    return true;
            }
            return false;
        }
    }
}
