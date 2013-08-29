namespace JistBridge.Data.Model
{
    public class Chain
    {
        public Fragment Left { get; set; }
        public Fragment Center { get; set; }
        public Fragment Right { get; set; }

        public bool IsComplete 
        {
            get { return Right != null && Center != null && Left != null; }
        }

        public Chain()
        {
        }

        public Chain(Fragment leftFragment, Fragment rightFragment, Fragment centerFragment)
        {
            Left = leftFragment;
            Right = rightFragment;
            Center = centerFragment;
        }

        public void ConvertFragments(Fragment from, Fragment to)
        {
            if (Left == from)
                Left = to;
            if (Right == from)
                Right = to;
            if (Center == from)
                Center = to;

        }

        public bool Contains(Fragment fragment)
        {
            if (Left == fragment)
                return true;
            if (Right == fragment)
                return true;
            return Center == fragment;
        }

        public void Add(Fragment fragment)
        {
            if (Left == null)
            {
                Left = fragment;
                return;
            }

            if (Center == null)
            {
                Center = fragment;
                return;
            }

            if (Right == null)
                Right = fragment;

        }

        public void Reset()
        {
            Left = Right = Center = null;
        }
        
        public bool AreFragmentBoundsInChain(Fragment fragment)
        {
            if (Left.ContainsFragment(fragment))
                return true;
            if (Right.ContainsFragment(fragment))
                return true;
            if (Center.ContainsFragment(fragment))
                return true;

            return false;
        }

        
    }
}
