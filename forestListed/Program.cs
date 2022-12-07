using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace forestListed
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("######################################");
            Console.WriteLine("BURNING FOREST SIMULATOR by Alan Nardo");
            Console.WriteLine("######################################");

            Cell[,] forestMap;
            Random randy = new Random();
            List<Tuple<int, int>> burnspots = new List<Tuple<int, int>>() 
            { 
                new Tuple<int,int> ( 10, 10 )
            };
            Terrain terrain = new Terrain(); //terrain generation. need to make this optional
            
            Console.WriteLine("Press Y to activate terrain, any other key to continue without.");
            ConsoleKey reply = Console.ReadKey().Key;
            if (reply == ConsoleKey.Y)
            {
                forestMap = ForestGenerator(randy, terrain, true);
            }
            else
            {
                forestMap = ForestGenerator(randy, terrain, false);
            }
            
            Display(burnspots, forestMap);
            Handler(burnspots, forestMap, randy);
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
        public static void Display(List<Tuple<int, int>> fires, Cell[,] forest)
        {
            PaintMap();
            for (int i = 0; i < fires.Count; i++)
            {
                Console.Write("(" + fires[i].Item1 + ", " + fires[i].Item2 + "), ");
            }
            Console.WriteLine(fires.Count);
            for (int i = 0; i < forest.GetLength(0); i++) //loop through outer
            {
                for (int j = 0; j < forest.GetLength(1); j++) //again on inner
                {
                    // a e s t h e t i c s
                    if (forest[i, j].GetState() == 'x')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
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

        public static void Handler(List<Tuple<int, int>> fires, Cell[,] forest, Random rand)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please press 'Enter' to continue...");
            ConsoleKeyInfo keyPress = Console.ReadKey();

            if (keyPress.Key == ConsoleKey.Enter)
            {
                List<Tuple<int, int>> updatedList = BurnSpread(fires, forest, rand);
                Display(updatedList, forest);

                if (updatedList.Count == 0) //if they are the same, the game ends
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("The fire cannot spread anymore the game is over. Press any key to quit.");
                    Environment.Exit(0); // kind of clunky

                }
                Handler(updatedList, forest, rand);
            }
        } // end Runner
        public static List<Tuple<int, int>> BurnSpread(List<Tuple<int, int>> burnzones, Cell[,] forest, Random rand)
        {
            List<Tuple<int, int>> newburns = new List<Tuple<int, int>>();
            
            if (burnzones.Count > 0) 
            {
                for (int i = 0; i < burnzones.Count; i++)
                {
                    IDictionary<Tuple<int, int>, char> returns = new Dictionary<Tuple<int, int>, char>();
                    
                    
                    int x = burnzones[i].Item1;
                    int y = burnzones[i].Item2;

                    int s = Utilities.Clamp(x + 1, 0, 20);
                    int n = Utilities.Clamp(x - 1, 0, 20);
                    int w = Utilities.Clamp(y - 1, 0, 20);
                    int e = Utilities.Clamp(y + 1, 0, 20);

                    //southern  forest[s, y]
                    //eastern   forest[x, e]
                    //western   forest[x, w]
                    //northern  forest[n, y]

                    if (x == 0 && y == 0) //NW corner only south and east
                    {
                        Tuple<int, int> south = new Tuple<int, int>( s, y );
                        Tuple<int, int> east = new Tuple<int, int>( x, e );

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                            returns.Add(south, forest[s, y].GetState());
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                            returns.Add(east, forest[x, e].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn.Equals(entry.Key))
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x == 20 && y == 20) //SE corner only north and west
                    {

                        Tuple<int, int> north = new Tuple<int, int>(n, y);
                        Tuple<int, int> west = new Tuple<int, int>(x, w);

                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                            returns.Add(north, forest[n, y].GetState());
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                            returns.Add(west, forest[x, w].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x == 20 && y == 0) //SW corner only north and east
                    {
                        Tuple<int, int> north = new Tuple<int, int>(n, y);
                        Tuple<int, int> east = new Tuple<int, int>(x, e);

                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                            returns.Add(north, forest[n, y].GetState());
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                            returns.Add(east, forest[x, e].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x == 0 && y == 20) //NE corner only south and west
                    {
                        Tuple<int, int> west = new Tuple<int, int>(x, w);
                        Tuple<int, int> south = new Tuple<int, int>(s, y);

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                            returns.Add(south, forest[s, y].GetState());
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                            returns.Add(west, forest[x, w].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x == 0 && y < 20 && y > 0) //along W edge ignore west spread
                    {
                        Tuple<int, int> north = new Tuple<int, int>(n, y);
                        Tuple<int, int> south = new Tuple<int, int>(s, y);
                        Tuple<int, int> east = new Tuple<int, int>(x, e);

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                            returns.Add(south, forest[s, y].GetState());
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                            returns.Add(north, forest[n, y].GetState());
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                            returns.Add(east, forest[x, e].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x > 0 && x < 20 && y == 0) // along N edge ignore north spread
                    {
                        Tuple<int, int> west = new Tuple<int, int>(x, w);
                        Tuple<int, int> south = new Tuple<int, int>(s, y);
                        Tuple<int, int> east = new Tuple<int, int>(x, e);

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                            returns.Add(south, forest[s, y].GetState());
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                            returns.Add(east, forest[x, e].GetState());
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                            returns.Add(west, forest[x, w].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x == 20 && y > 0 && y < 20) // along E edge ignore east spread
                    {
                        Tuple<int, int> north = new Tuple<int, int>(n, y);
                        Tuple<int, int> west = new Tuple<int, int>(x, w);
                        Tuple<int, int> south = new Tuple<int, int>(s, y);

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                            returns.Add(south, forest[s, y].GetState());
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                            returns.Add(north, forest[n, y].GetState());
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                            returns.Add(west, forest[x, w].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x > 0 && x < 20 && y == 20) // along S edge ignore south spread
                    {
                        Tuple<int, int> north = new Tuple<int, int>(n, y);
                        Tuple<int, int> west = new Tuple<int, int>(x, w);
                        Tuple<int, int> east = new Tuple<int, int>(x, e);

                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                            returns.Add(north, forest[n, y].GetState());
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                            returns.Add(east, forest[x, e].GetState());
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                            returns.Add(west, forest[x, w].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (burn != entry.Key)
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    else if (x > 0 && x < 20 && y > 0 && y < 20) // anywhere not along an edge
                    {
                        Tuple<int, int> north = new Tuple<int, int>(n, y);
                        Tuple<int, int> west = new Tuple<int, int>(x, w);
                        Tuple<int, int> south = new Tuple<int, int>(s, y);
                        Tuple<int, int> east = new Tuple<int, int>(x, e);

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                            returns.Add(south, forest[s, y].GetState());
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                            returns.Add(north, forest[n, y].GetState());
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                            returns.Add(east, forest[x, e].GetState());
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                            returns.Add(west, forest[x, w].GetState());
                        }

                        foreach (KeyValuePair<Tuple<int, int>, char> entry in returns)
                        {
                            if (entry.Value == 'x')
                            {
                                foreach (Tuple<int, int> burn in burnzones)
                                {
                                    if (!burn.Equals(entry.Key))
                                    {
                                        newburns.Add(entry.Key);
                                    }
                                }
                            }
                        }
                    }
                    forest[x, y].SetState('_');
                }
            }
            burnzones.Clear();
            burnzones = newburns.ToList();
            return burnzones;
        }
    }
}
