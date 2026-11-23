using System.Data;

namespace Innkeeper_Ennumerate;

class Inkeeper_Ennumerate
{
    private static int CUSTOMER_COUNT = 15;
    private static int FLOOR_COUNT = 5;
    private static int AMENITY_COUNT = 12;
    private static int MAX_CUSTOMER_AMENITY_SCORE = 4;
    
    private string[] CUSTOMERS =
    [
        "Native", "Redneck", "Bachelor", "Pseudoscientist", "Zoologist", "Pensioner", "Critic",
        "Blogger", "Socialite", "Politician", "Celebrity", "MIB", "Hipster", "Jetsetter", "Blank"
    ];

    private string[] AMENITIES =
    [
        "Buffet", "Laundry", "Gym", "Interior Garden", "Game Room", "Bar", "Computer Room", 
        "Clinic", "Spa", "Chapel", "Fine Dining", "Convention Center"
    ];
    
    private List<List<int>> CUSTOMER_AMENITY = 
    [   [2, 2, 0, 0, 1, 0, 0, 1, 1, 2, 0, 0],  //native
        [2, 0, 2, 1, 1, 1, 0, 2, 0, 0, 0, 0],  //redneck
        [0, 0, 2, 0, 2, 2, 1, 0, 1, 0, 0, 1],  //bachelor
        [2, 1, 0, 0, 0, 0, 2, 2, 1, 0, 0, 1],  //pseudoscientist
        [0, 0, 2, 2, 0, 2, 1, 0, 0, 1, 1, 0],  //zoologist
        [0, 2, 0, 1, 0, 0, 0, 2, 1, 2, 1, 0],  //pensioner
        [1, 0, 1, 0, 2, 0, 0, 0, 2, 1, 2, 0],  //critic
        [1, 1, 0, 0, 2, 0, 2, 1, 0, 0, 0, 1],  //blogger
        [0, 0, 0, 0, 0, 2, 1, 0, 2, 1, 2, 1],  //socialite
        [0, 0, 1, 1, 0, 0, 0, 2, 0, 2, 2, 1],  //politician
        [1, 0, 2, 1, 0, 2, 0, 0, 0, 0, 1, 2],  //celebrity
        [0, 1, 1, 1, 0, 0, 2, 2, 0, 0, 0, 2],  //MIB
        [2, 1, 0, 2, 0, 1, 0, 0, 0, 2, 0, 1],  //hipster
        [0, 0, 1, 2, 0, 1, 0, 0, 2, 0, 1, 2],  //jetsetter
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]]; //blank

    static void Main()
    {
        var c = new Inkeeper_Ennumerate();
        c.Run();
    }

    void Run()
    {
        
    }

    void Output()
    {
        
    }

    void Solve()
    {
        //make a list of all possible combinations of amenities
        //for each combination, run the hungarian algorithm
        
        
    }
    
    int[,] ToMinimization(int[,] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array.GetLength(1); j++)
            {
                array[i,j] = MAX_CUSTOMER_AMENITY_SCORE - array[i,j];
            }
        }
        
        return array;
    }
    
    int[,] ReduceRow(int[,] array)
    {
        for (int row = 0; row < array.Length; row++)
        {
            int rowmin = MAX_CUSTOMER_AMENITY_SCORE;
            for (int col = 0; col < array.GetLength(1); col++)
            {
                if (array[row,col] < rowmin)
                {
                    rowmin = array[row,col];
                }
            }

            for (int col = 0; col < array.GetLength(1); col++)
            {
                array[row,col] -= rowmin;
            }
        }
        
        return array;
    }

    int[,] ReduceColumn(int[,] array)
    {
        for (int col = 0; col < array.GetLength(1); col++)
        {
            int colmin = MAX_CUSTOMER_AMENITY_SCORE;
            for (int row = 0; row < array.Length; row++)
            {
                if (array[row,col] < colmin)
                {
                    colmin = array[row,col];
                }
            }

            for (int row = 0; row < array.Length; row++)
            {
                array[row,col] -= colmin;
            }
        }
        
        return array;
    }

    int[,] MakeOneToOne(int[,] array)
    {
        int nrows = array.Length;
        int ncols = array.GetLength(1);
        int colMult = nrows / ncols;

        int[,] newarray = new int[nrows, ncols * colMult];
        
        for (int i = 0; i < ncols; i++)
        {
            for (int k = 0; k < nrows; k++)
            {
                for (int j = 0; j < colMult; j++)
                {
                
                    newarray[k, i * colMult + j] = array[k, i];
                }
            }
        }
        
        return newarray;
    }

    void Hungarian(int[,] array)
    {
        //turn matrix into a bipartite graph
        //While an augmenting path exists, add not-matching edges of path to matching and remove matching edges
        
        int[,] graph = ToGraph(array);
        int[,] matchedgraph = FindAugmenting(graph);
        int[,] newarray = new int[array.GetLength(0), array.GetLength(1)];
        array.CopyTo(newarray, 0);

        while (!IsPerfectMatching(matchedgraph))
        {
            matchedgraph = FindAugmenting(matchedgraph);
            (List<int>, List<int>) Z = ZeroCover(matchedgraph);
            int min = findMinZ(Z.Item1, Z.Item2, newarray);
            adjustCells(min, newarray, Z.Item1, Z.Item2);
        }
    }

    bool IsPerfectMatching(int[,] graph)
    {
        for (int i = 0; i < CUSTOMER_COUNT; i++)
        {
            if (graph[i, CUSTOMER_COUNT] != 0)
            {
                return false;
            }
        }
        
        return true;
    }
    
    (List<int>, List<int>) ZeroCover(int[,] graph)
    {
        List<int> Zrows = new List<int>();
        List<int> Zcols = new List<int>();
        
        //add unmatched rows to Zrows
        for (int row = 0; row < graph.GetLength(0); row++)
        {
            if (graph[row, CUSTOMER_COUNT] == -1)
            {
                Zrows.Add(row);
            }
        }
        
        //then add new columns if a cell is 1 or new rows if a cell is -1

        bool isCol = false;
        bool addedsomething = true;

        while (addedsomething)
        {
            addedsomething = false;
            
            if (isCol)
            {
                for (int i = 0; i < Zcols.Count; i++)
                {
                    for (int row = 0; row < CUSTOMER_COUNT; row++)
                    {
                        if (graph[row, Zcols[i]] == -1 && Zrows.Contains(row))
                        {
                            Zrows.Add(row);
                            addedsomething = true;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Zrows.Count; i++)
                {
                    for (int col = 0; col < CUSTOMER_COUNT; col++)
                    {
                        if (graph[Zrows[i], col] == 1 && !Zcols.Contains(col))
                        {
                            Zcols.Add(col);
                            addedsomething = true;
                        }
                    }
                }
            }
            
            isCol = !isCol;
        }
        
        return (Zrows, Zcols);
    }

    int findMinZ(List<int> zrows, List<int> zcols, int[,] array)
    {
        int min = MAX_CUSTOMER_AMENITY_SCORE;
        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                if (zrows.Contains(row) && !zcols.Contains(col) && array[row, col] < min)
                {
                    min = array[row, col];
                }

                if (min == 1) return min;
            }
        }
        return min;
    }

    void adjustCells(int min, int[,] array, List<int> zrows, List<int> zcols)
    {
        for (int i = 0; i < zrows.Count; i++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                array[zrows[i], col] -= min;
            }
        }

        for (int i = 0; i < zcols.Count; i++)
        {
            for (int row = 0; row < CUSTOMER_COUNT; row++)
            {
                array[row, zcols[i]] += min;
            }
        }
    }

    int[,] ToGraph(int[,] array)
    {
        int[,] graph = new int[CUSTOMER_COUNT + 1, CUSTOMER_COUNT + 1];
        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                graph[row, CUSTOMER_COUNT] = -1;
                graph[CUSTOMER_COUNT, col] = -1;
                
                if (array[row, col] == 0)
                {
                    graph[row, col] = 1;
                }
                else graph[row, col] = 0;
            }
        }
        return graph;
    }

    int[,] FindAugmenting(int[,] graph)
    {
        //1 means row -> column (forwards)
        //-1 means column -> row (backwards)
        //2 means not in matching and row -> column
        //-2 means not in matching and column -> row

        while (true)
        {
            (bool, List<int>) result = DepthFirstSearch(graph, false, CUSTOMER_COUNT, new List<int>());

            if (result.Item1)
            {
                List<int> augmenting = result.Item2;

                bool isRow = true;
                int loc = CUSTOMER_COUNT;
                
                //add augmenting edges to or remove them from matching
                for (int i = 0; i < result.Item2.Count - 1; i++)
                {
                    if (isRow)
                    {
                        if (graph[i, loc] == -2)
                        {
                            graph[i, loc] = 1;
                            graph[i, CUSTOMER_COUNT] = -1;
                        }
                        else if (graph[i, loc] == 2)
                        {
                            graph[i, loc] = -1;
                            graph[i, CUSTOMER_COUNT] = 1;
                        }
                    }
                    else
                    {
                        if (graph[loc, i] == -2)
                        {
                            graph[loc, i] = 1;
                            graph[CUSTOMER_COUNT, i] = -1;
                        }
                        else if (graph[loc, i] == 2)
                        {
                            graph[loc, i] = -1;
                            graph[CUSTOMER_COUNT, i] = 1;
                        }
                    }
                }

                graph[result.Item2[0], CUSTOMER_COUNT] = 0;
                graph[CUSTOMER_COUNT, result.Item2[^1]] = 0;
            }

            else
            {
                return graph;
            }
        }
        
    }

    (bool, List<int>) DepthFirstSearch(int[,] graph, bool isRow, int loc, List<int> path)
    {
        if (isRow)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                if (graph[loc,col] == 1)
                {
                    path.Add(col);
                    graph[loc, col] = 2;
                    (bool, List<int>) result = DepthFirstSearch(graph, false, col, path);
                    if (result.Item1)
                    {
                        return result;
                    }
                }
            }
                
        }
        else
        {
            if (graph[CUSTOMER_COUNT, loc] == -1)
            {
                return (true, path);
            }

            for (int row = 0; row < CUSTOMER_COUNT; row++)
            {
                if (graph[row, loc] == -1)
                {
                    path.Add(row);
                    graph[row, loc] = -2; 
                    (bool, List<int>) result = DepthFirstSearch(graph, true, row, path);
                    if (result.Item1)
                    {
                        return result;
                    }
                }
            }
                
        }
        
        return (false, path);
    }
}