using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace JistBridge.Data.Model
{
	[Export(typeof(Markup))]
    public class Markup
    {
        private Guid _markupId = Guid.Empty;
        public Guid MarkupId
        {
            get
            {
                if (_markupId == Guid.Empty)
                    _markupId = Guid.NewGuid();
                return _markupId;
            }
            private set { _markupId = value; }
        }

        public List<Chain> Chains { get; set; }

        public Chain CurrentChain { get; set; }


        public Markup()
        {
            Chains = new List<Chain>();
        }

	    public Fragment GetOrCreateFragment(Range<int> offsetRange, FragmentType fragmentType, string displayText, int sourceOffset)
	    {
	        Fragment fragment = null;
	        if (fragmentType == FragmentType.Link)
	        {
	            fragment = new Fragment(new List<Range<int>> {offsetRange}, fragmentType, displayText, sourceOffset);
	            return fragment;
	        }

            fragment = GetFragmentWithExactRange(offsetRange,true);
	        return fragment ?? new Fragment(new List<Range<int>> { offsetRange }, fragmentType, displayText, sourceOffset);
	    }

	    public void MergeFragments(Fragment winner, Fragment loser)
        {
            winner.Consume(loser);
            ReLinkChains(winner,loser);
        }

	    public Fragment GetFragmentWithExactRange(Range<int> offsetRange, bool nodesOnly)
	    {
	        Fragment fragment = null;
            foreach (var chain in Chains)
            {
                fragment = chain.GetFragmentWithExactRange(offsetRange, nodesOnly);
                if (fragment != null)
                    return fragment;
            }
	        return fragment;

	    }

	    public bool AreFragmentBoundsInMarkup(Fragment fragment)
        {
            foreach (var chain in Chains)
            {
                if (chain.AreFragmentBoundsInChain(fragment))
                    return true;
            }
            return false;
        }

        private void ReLinkChains(Fragment winner, Fragment loser)
        {
            foreach (var chain in Chains)
            {
                chain.ConvertFragments(loser, winner);
            }
        }

        public void AddChain(Chain chain)
        {
            Chains.Add(chain);
        }
    }
}
