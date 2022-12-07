using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace forestListed
{
    public class EqualityComparer : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            return Enumerable.SequenceEqual(x, y);
        }

        public int GetHashCode(int[] codeh)
        {
            return codeh.GetHashCode();
        }
    }
}