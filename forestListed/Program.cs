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
            if (args != null) //we only want to display this the first time
            {
                Console.WriteLine("######################################");
                Console.WriteLine("BURNING FOREST SIMULATOR by Alan Nardo");
                Console.WriteLine("######################################");
            }

            Cell[,] forestMap;
            Random randy = new Random();
            List<int[]> burnspots = new List<int[]>() 
            { 
                new int[] { 10, 10 } 
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
        public static void Display(List<int[]> fires, Cell[,] forest)
        {
            PaintMap();
            for (int i = 0; i < fires.Count; i++)
            {
                Console.Write("(" + fires[i][0] + ", " + fires[i][1] + "), ");
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
                    forest[fires[i][0], fires[i][1]].SetState('x');
                }
            }
        }//end display

        public static void Handler(List<int[]> fires, Cell[,] forest, Random rand)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Please press 'Enter' to continue...");
            ConsoleKeyInfo keyPress = Console.ReadKey();

            if (keyPress.Key == ConsoleKey.Enter)
            {
                List<int[]> updatedList = BurnSpread(fires, forest, rand);
                Display(updatedList, forest);

                if (updatedList.Count == 0) //if they are the same, the game ends
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("The fire cannot spread anymore the game is over. Press R to restart, or any other key to quit.");
                    ConsoleKeyInfo option = Console.ReadKey();
                    if (option.Key == ConsoleKey.R)
                    {
                        Console.Clear(); //start a new game!
                        Main(null);
                    }
                    else 
                    {
                        Environment.Exit(0); // kind of clunky
                    }
                }
                Handler(updatedList, forest, rand);
            }
        } // end Runner
        public static List<int[]> BurnSpread(List<int[]> burnzones, Cell[,] forest, Random rand)
        {
            List<int[]> newburns = new List<int[]>();
            
            if (burnzones.Count > 0) 
            {
                for (int i = 0; i < burnzones.Count; i++)
                {
                    IDictionary<int[], char> returns = new Dictionary<int[], char>();
                    
                    
                    int x = burnzones[i][0];
                    int y = burnzones[i][1];

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
                        int[] south = { s, y };
                        int[] east = { x, e };

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                        }

                        returns.Add(south, forest[s, y].GetState());
                        returns.Add(east, forest[x, e].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x == 20 && y == 20) //SE corner only north and west
                    {
                        int[] north = { n, y };
                        int[] west = { x, w };

                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                        }

                        returns.Add(north, forest[n, y].GetState());
                        returns.Add(west, forest[x, w].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x == 20 && y == 0) //SW corner only north and east
                    {
                        int[] north = { n, y };
                        int[] east = { x, e };

                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                        }

                        returns.Add(north, forest[n, y].GetState());
                        returns.Add(east, forest[x, e].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x == 0 && y == 20) //NE corner only south and west
                    {
                        int[] west = { x, w };
                        int[] south = { s, y };

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                        }

                        returns.Add(west, forest[x, w].GetState());
                        returns.Add(south, forest[s, y].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x == 0 && y < 20 && y > 0) //along W edge ignore west spread
                    {
                        int[] north = { n, y };
                        int[] south = { s, y };
                        int[] east = { x, e };

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                        }

                        returns.Add(east, forest[x, e].GetState());
                        returns.Add(south, forest[s, y].GetState());
                        returns.Add(north, forest[n, y].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x > 0 && x < 20 && y == 0) // along N edge ignore north spread
                    {
                        int[] west = { x, w };
                        int[] south = { s, y };
                        int[] east = { x, e };

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                        }

                        returns.Add(east, forest[x, e].GetState());
                        returns.Add(south, forest[s, y].GetState());
                        returns.Add(west, forest[x, w].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x == 20 && y > 0 && y < 20) // along E edge ignore east spread
                    {
                        int[] north = { n, y };
                        int[] west = { x, w };
                        int[] south = { s, y };

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                        }

                        returns.Add(south, forest[s, y].GetState());
                        returns.Add(west, forest[x, w].GetState());
                        returns.Add(north, forest[n, y].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x > 0 && x < 20 && y == 20) // along S edge ignore south spread
                    {
                        int[] north = { n, y };
                        int[] west = { x, w };
                        int[] east = { x, e };

                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                        }

                        returns.Add(east, forest[x, e].GetState());
                        returns.Add(west, forest[x, w].GetState());
                        returns.Add(north, forest[n, y].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    else if (x > 0 && x < 20 && y > 0 && y < 20) // anywhere not along an edge
                    {
                        int[] north = { n, y };
                        int[] west = { x, w };
                        int[] south = { s, y };
                        int[] east = { x, e };

                        if (forest[s, y].GetState() == '&')
                        {
                            forest[s, y].SetState(Utilities.StateChangeCheck(forest[s, y], rand));
                        }
                        if (forest[n, y].GetState() == '&')
                        {
                            forest[n, y].SetState(Utilities.StateChangeCheck(forest[n, y], rand));
                        }
                        if (forest[x, e].GetState() == '&')
                        {
                            forest[x, e].SetState(Utilities.StateChangeCheck(forest[x, e], rand));
                        }
                        if (forest[x, w].GetState() == '&')
                        {
                            forest[x, w].SetState(Utilities.StateChangeCheck(forest[x, w], rand));
                        }

                        returns.Add(east, forest[x, e].GetState());
                        returns.Add(south, forest[s, y].GetState());
                        returns.Add(west, forest[x, w].GetState());
                        returns.Add(north, forest[n, y].GetState());

                        foreach (KeyValuePair<int[], char> entry in returns)
                        {
                            if (entry.Value == 'x' && !newburns.Contains(entry.Key))
                            {
                                newburns.Add(entry.Key);
                            }
                        }
                    }
                    forest[x, y].SetState('_');
                    burnzones.RemoveAt(i);
                }
            }
            burnzones.Clear();
            burnzones = newburns.ToList();
            return burnzones;
        }
    }
}
