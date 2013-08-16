using System;
using System.Collections.Generic;

namespace JistBridge.Data.Model
{
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

        public Markup()
        {
            Chains = new List<Chain>();
        }

        public void MergeFragments(Fragment winner, Fragment loser)
        {
            winner.Consume(loser);
            ReLinkChains(winner,loser);
        }

        private void ReLinkChains(Fragment winner, Fragment loser)
        {
            foreach (var chain in Chains)
            {
                chain.ConvertFragments(loser, winner);
            }
        }
    }
}
