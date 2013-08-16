using System.Collections.Generic;

namespace JistBridge.Data.Model
{
    public class Markup
    {
        public List<Chain> Chains { get; set; }

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
