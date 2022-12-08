using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace forestListed
{
    internal class Wind
    {
        private int speed = 0;
        private char direction;

        public Wind() { }
        public void SetSpeed(Random rand)
        {
            speed = rand.Next(1, 4);
        }

        public void SetDirection(Random rand)
        {
            IDictionary<int, char> directions = new Dictionary<int, char>()
            {
                {0, 'n' },
                {1, 's' },
                {2, 'e' },
                {3, 'w' },
            };

            int roll = rand.Next(4);
            direction = directions[roll];
        }

        public char GetDirection()
        {
            return direction;
        }

        public int GetSpeed()
        {
            return speed;
        }
    }
}