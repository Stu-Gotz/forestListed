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

        public static bool ActivateTerrain()
        {
            Console.WriteLine("Press Y to activate terrain, any other key to continue without.");
            ConsoleKey reply = Console.ReadKey().Key;
            Console.WriteLine();
            if (reply == ConsoleKey.Y)
            {
                return true;
            }
            return false;
        } // end ActivateTerrain
    }
}