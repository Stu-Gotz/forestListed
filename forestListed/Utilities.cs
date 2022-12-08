using System;

namespace forestListed
{
    internal class Utilities
    {
        /* THE TERNINATORS */
        public static char CoinFlip(Random rnd, char currentState)
        {
            return rnd.Next() % 2 == 0 ? 'x' : currentState;
        }
        public static char TerrainChecker(int terrainType)
        {
            return terrainType == 1 ? 'x' : '&';
        }
        public static char StateChangeCheck(Cell cell, Random rand)
        {
            return cell.GetTerrain() == 0 
                ? CoinFlip(rand, cell.GetState()) 
                : TerrainChecker(cell.GetTerrain());
        }
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}