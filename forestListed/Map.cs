using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace forestListed
{
    class Map
    {
        static void Main(string[] args)
        {

            Console.WriteLine("######################################");
            Console.WriteLine("BURNING FOREST SIMULATOR by Alan Nardo");
            Console.WriteLine("######################################");
            Wind wind = new Wind();
            Cell[,] forestMap;
            Random randy = new Random();
            List<Tuple<int, int>> burnspots = new List<Tuple<int, int>>() 
            { 
                new Tuple<int,int> ( 10, 10 )
            };
            Terrain terrain = new Terrain(); //terrain generation. need to make this optional
            bool active = Terrain.ActivateTerrain();
            forestMap = ForestGenerator(randy, terrain, active);

            Console.WriteLine("Press W to activate terrain, any other key to continue without.");
            ConsoleKey response = Console.ReadKey().Key;
            Console.WriteLine();
            if (response == ConsoleKey.W)
            {
                wind = new Wind();
                wind.SetDirection(randy);
                wind.SetSpeed(randy);
            }

            Display(burnspots, forestMap, wind, active);
            Handler(burnspots, forestMap, randy, wind, active);
        } // end main

        public static Cell[,] ForestGenerator(Random rand, Terrain terr, bool isActive)
        {
            Cell[,] forestGrid = new Cell[21, 21];

            for (int i = 0; i < forestGrid.GetLength(0); i++)
            {
                for (int j = 0; j < forestGrid.GetLength(1); j++)
                {
                    forestGrid[i, j] = new Cell();
                    forestGrid[i, j].SetState('&');
                    if (isActive)
                    {
                        int terrain = terr.TerrainMaker(rand, i);
                        forestGrid[i, j].SetTerrain(terrain);
                    }
                }
            }

            //make sure centre point is on fire and not wet
            forestGrid[10, 10].SetState('x');
            forestGrid[10, 10].SetTerrain(0);

            return forestGrid;
        }
        public static void Display(
            List<Tuple<int, int>> fires, 
            Cell[,] forest, 
            Wind wind,
            bool active
            )
        {
            PaintMap();
            if (active)
            {
                Console.WriteLine("Terrain mods are on.");
            }
            else
            {
                Console.WriteLine("Terrain mods are off.");
            }
            if (wind.GetSpeed() > 0)
            {
                Console.WriteLine("Wind is blowing " + char.ToUpper(wind.GetDirection()) + " at speed " + wind.GetSpeed() + ".");
            }
            else
            {
                Console.WriteLine("There is no wind.");
            }
            Console.WriteLine(fires.Count + " fires burning.");
            for (int i = 0; i < forest.GetLength(0); i++) //loop through outer
            {
                for (int j = 0; j < forest.GetLength(1); j++) //again on inner
                {
                    // a e s t h e t i c s
                    if (forest[i, j].GetState() == 'x')
                    {
                        
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    }
                    else if (forest[i, j].GetState() == '&')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    if (forest[i, j].GetTerrain() == 1) //dry
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                    }
                    else if (forest[i, j].GetTerrain() == 2) //wet
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    Console.Write(string.Format("{0}", forest[i, j].GetState()));
                }
                
                Console.Write(Environment.NewLine);
            }

            void PaintMap()
            {
                for(int i = 0; i < fires.Count; i++)
                {
                    forest[fires[i].Item1, fires[i].Item2].SetState('x');
                }
            }
        }//end display

        public static void Handler(
            List<Tuple<int, int>> fires, 
            Cell[,] forest, 
            Random rand, 
            Wind wind,
            bool active
            )
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please press 'Enter' to continue...");
            ConsoleKeyInfo keyPress = Console.ReadKey();

            if (keyPress.Key == ConsoleKey.Enter)
            {
                Console.Clear();
                List<Tuple<int, int>> updatedList = BurnSpread(fires, forest, rand, wind);
                Display(updatedList, forest, wind, active);

                if (updatedList.Count == 0) //if they are the same, the game ends
                {
                    active = false;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("The fire cannot spread anymore the sim is over. Press R to restart, any other key to quit.");
                    ConsoleKeyInfo option = Console.ReadKey();
                    if (option.Key == ConsoleKey.R)
                    {
                        Console.Clear(); //start a new game!
                        active = true;
                        bool terrain = Terrain.ActivateTerrain();
                        Cell[,] forestGrid = ForestGenerator(rand, new Terrain(), terrain);
                        Console.WriteLine("######################################");
                        Console.WriteLine("BURNING FOREST SIMULATOR by Alan Nardo");
                        Console.WriteLine("######################################");
                        Display(updatedList, forest, wind, active);
                        Handler(updatedList, forest, rand, wind, active);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                Handler(updatedList, forest, rand, wind, active);
            }
        } // end Handler

        public static List<Tuple<int, int>> BurnSpread(
            List<Tuple<int, int>> burnzones,
            Cell[,] forest,
            Random rand,
            Wind wind
            )
        {
            List<Tuple<int, int>> newburns = new List<Tuple<int, int>>();

            if (burnzones.Count > 0)
            {
                for (int i = 0; i < burnzones.Count; i++)
                {
                    IDictionary<Tuple<int, int>, char> returns = new Dictionary<Tuple<int, int>, char>();


                    int x = burnzones[i].Item1;
                    int y = burnzones[i].Item2;

                    int s = Utilities.Clamp(x + 1, 0, 21);
                    int n = Utilities.Clamp(x - 1, 0, 21);
                    int w = Utilities.Clamp(y - 1, 0, 21);
                    int e = Utilities.Clamp(y + 1, 0, 21);

                    int smax = Utilities.Clamp(s + 1, 0, 21);
                    int nmax = Utilities.Clamp(n + 1, 0, 21);
                    int wmax = Utilities.Clamp(w + 1, 0, 21);
                    int emax = Utilities.Clamp(e + 1, 0, 21);

                    switch (wind.GetDirection())
                    {
                        case 's':
                            smax = Utilities.Clamp(smax + wind.GetSpeed(), 0, 21);
                            break;
                        case 'n':
                            nmax = Utilities.Clamp(nmax + wind.GetSpeed(), 0, 21);
                            break;
                        case 'e':
                            emax = Utilities.Clamp(emax + wind.GetSpeed(), 0, 21);
                            break;
                        case 'w':
                            wmax = Utilities.Clamp(wmax + wind.GetSpeed(), 0, 21);
                            break;
                    }
                    
                    //burns to the north
                    for (int d = n; d < nmax; d++)
                    {
                        if (forest[d, y].GetState() == '&')
                        {
                            Tuple<int, int> north = new Tuple<int, int>(d, y);
                            forest[d, y].SetState(Utilities.StateChangeCheck(forest[d, y], rand));
                            if (!returns.ContainsKey(north))
                            {
                                returns.Add(north, forest[d, y].GetState());
                            }
                        }
                    }
                    //burns to the west
                    for (int c = w; c < wmax; c++)
                    {
                        if (forest[x, c].GetState() == '&')
                        {
                            Tuple<int, int> west = new Tuple<int, int>(x, c);
                            forest[x, c].SetState(Utilities.StateChangeCheck(forest[x, c], rand));
                            if (!returns.ContainsKey(west))
                            {
                                returns.Add(west, forest[x, c].GetState());
                            }
                        }
                    }
                    //burns to the south
                    for (int a = s; a < smax; a++)
                    {
                        if (forest[a, y].GetState() == '&')
                        {
                            Tuple<int, int> south = new Tuple<int, int>(a, y);
                            forest[a, y].SetState(Utilities.StateChangeCheck(forest[a, y], rand));
                            if (!returns.ContainsKey(south))
                            {
                                returns.Add(south, forest[a, y].GetState());
                            }
                        }
                    }
                    //burns to the east
                    for (int b = e; b < emax; b++)
                    {
                        if (forest[x, b].GetState() == '&')
                        {
                            Tuple<int, int> east = new Tuple<int, int>(x, b);
                            forest[x, b].SetState(Utilities.StateChangeCheck(forest[x, b], rand));
                            if (!returns.ContainsKey(east))
                            {
                                returns.Add(east, forest[x, b].GetState());
                            }
                        }
                    }
                    foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                    {
                        if (entry.Value == 'x')
                        {
                            newburns.Add(entry.Key);
                        }
                    }
                    forest[x, y].SetState('_');
                    returns.Clear();
                }
            }
            var hashSet = new HashSet<Tuple<int, int>>(newburns);
            return hashSet.ToList();
        }
    }
}
