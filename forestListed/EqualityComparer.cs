using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace forestListed
{
    public class EqualityComparer
    {
        public bool Equals(int[] x, int[] y)
        {
            string stringx = string.Join(",", x);
            string stringy = string.Join(",", y);
            Console.WriteLine("x: " + stringx + "y: " + stringy);
            return stringx == stringy;
        }

        public int GetHashCode(int[] codeh)
        {
            return codeh.GetHashCode();
        }
    }
}