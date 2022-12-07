using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace forestListed
{
    internal class Terrain
    {
        public int TerrainMaker(Random rand, int index)
        {
            int terrain = 0;
            int roll = rand.Next(21);

            if (roll == index)
            {
                switch (roll % 2)
                {
                    case 1:
                        terrain = 1; //dry
                        return terrain;
                    case 0:
                        terrain = 2; //wet
                        return terrain;
                }
            }
         return terrain;
        }
    }
}