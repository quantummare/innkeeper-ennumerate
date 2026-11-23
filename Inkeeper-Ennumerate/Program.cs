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
    
    void ZeroCover()
    {
        //turn matrix into a bipartite graph
        //While an augmenting path exists, add not-matching edges of path to matching and remove matching edges
    }

    int[,] ToGraph(int[,] array)
    {
        int[,] graph = new int[CUSTOMER_COUNT + 1, CUSTOMER_COUNT + 1];
        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                if (array[row, col] == 0)
                {
                    graph[row, col] = 1;
                    graph[row, CUSTOMER_COUNT] = -1;
                    graph[CUSTOMER_COUNT, col] = -1;
                }
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
        
        var graph2 = new int[CUSTOMER_COUNT, CUSTOMER_COUNT];
        graph.CopyTo(graph2, 0);

        while (true)
        {
            (bool, List<int>) result = DFS(graph, false, CUSTOMER_COUNT, new List<int>());

            if (result.Item1)
            {
                List<int> augmenting = result.Item2;

                bool isRow = true;
                int loc = CUSTOMER_COUNT;
                
                //add augmenting edges to or remove them from matching
                for (int i = 0; i < result.Item2.Count; i++)
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
            }

            else
            {
                return graph;
            }
        }
        
    }

    (bool, List<int>) DFS(int[,] graph, bool isRow, int loc, List<int> path)
    {
        if (isRow)
        {
            for (int col = 0; col< CUSTOMER_COUNT; col++)
            {
                if (graph[loc,col] == 1)
                {
                    path.Add(col);
                    graph[loc, col] = 2;
                    (bool, List<int>) result = DFS(graph, false, col, path);
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
                path.Add(CUSTOMER_COUNT);
                return (true, path);
            }

            for (int row = 0; row < CUSTOMER_COUNT; row++)
            {
                if (graph[row, loc] == -1)
                {
                    path.Add(row);
                    graph[row, loc] = -2; 
                    (bool, List<int>) result = DFS(graph, true, row, path);
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