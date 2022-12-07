using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace forestListed
{
    internal class Cell
    {
        private char state = '&';
        private int terrain = 0;

        public void SetState(char state)
        {
            this.state = state;
        }

        public char GetState()
        {
            return state;
        }
        public void SetTerrain(int terrain)
        {
            this.terrain = terrain;
        }
        public int GetTerrain()
        {
            return terrain;
        }
    }
}