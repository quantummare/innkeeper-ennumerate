using System.Data;
using System.Diagnostics;

namespace Innkeeper_Ennumerate;

class Inkeeper_Ennumerate
{
    private static int CUSTOMER_COUNT = 15;
    private static int FLOOR_COUNT = 5;
    private static int AMENITY_COUNT = 12;
    private static int AMENITIES_PER_FLOOR = 2;
    private static int AMENITIES_USED = 10;
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
        [1, 1, 0, 0, 2, 0, 2, 1, 0, 0, 0, 2],  //blogger
        [0, 0, 0, 0, 0, 2, 1, 0, 2, 1, 2, 1],  //socialite
        [0, 0, 1, 1, 0, 0, 0, 2, 0, 2, 2, 1],  //politician
        [1, 0, 2, 1, 0, 2, 0, 0, 0, 0, 1, 2],  //celebrity
        [0, 1, 1, 1, 0, 0, 2, 2, 0, 0, 0, 2],  //MIB
        [2, 1, 0, 2, 0, 1, 0, 0, 0, 2, 0, 1],  //hipster
        [0, 0, 1, 2, 0, 1, 0, 0, 2, 0, 1, 2],  //jetsetter
        [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]]; //blank

    private int[] testAmenity = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

    static void Main()
    {
        var c = new Inkeeper_Ennumerate();
        c.Run();
    }

    void Run()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        int[] initAmenityfloor = new int[AMENITIES_USED];
        bool[] initUsedAmenities = new bool[AMENITY_COUNT];
        Array.Fill(initAmenityfloor, -1);
        (int, int[]) Optimal = MaxAmenityScore(initAmenityfloor, initUsedAmenities, 0, 0);
        stopwatch.Stop();
        Output(Optimal.Item1, Optimal.Item2);
        Console.WriteLine($"Calculation completed in {stopwatch.ElapsedMilliseconds} ms.");
    }

    void Output(int bestscore, int[] amenityfloor)
    {
        Console.Write($"The best score is: {bestscore}");

        int[,] matchedgraph = Score(amenityfloor).Item2;

        int customersperfloor = CUSTOMER_COUNT / FLOOR_COUNT;
        
        for (int col = 0; col < CUSTOMER_COUNT; col++)
        {

            if (col % customersperfloor == 0)
            {
                Console.WriteLine();
                Console.Write($"{AMENITIES[amenityfloor[col / 3 * AMENITIES_PER_FLOOR]]} and {AMENITIES[amenityfloor[col / 3 * AMENITIES_PER_FLOOR + 1]]}: ");
            }
            
            for (int row = 0; row < CUSTOMER_COUNT; row++)
            {
                if (matchedgraph[row, col] == -1)
                {
                    Console.Write($"{CUSTOMERS[row]} ");
                }
            }
        }
        
        Console.WriteLine();
        
        
    }

    (int, int[]) MaxAmenityScore(int[] amenityfloor, bool[] usedamenities, int floor, int minAmenity)
    {
        //if this is the first amenity on the floor, update minAmenity
        //All amenities must be after minAmenity

        (int, int[]) maxscore = (0, amenityfloor);
        
        for (int i = minAmenity; i < AMENITY_COUNT; i++)
        {
            if (!usedamenities[i])
            {
                for (int j = i + 1; j < AMENITY_COUNT; j++)
                {
                    if (!usedamenities[j])
                    {
                        int[] newAmenityFloor = new int[AMENITIES_USED];
                        for (int k = 0; k < AMENITIES_USED; k++)
                        {
                            newAmenityFloor[k] = amenityfloor[k];
                        }
                        
                        newAmenityFloor[floor * AMENITIES_PER_FLOOR] = i;
                        newAmenityFloor[floor * AMENITIES_PER_FLOOR + 1] = j;
                        usedamenities[i] = true;
                        usedamenities[j] = true;

                        (int, int[]) newscore = (0, newAmenityFloor);

                        if (floor < FLOOR_COUNT - 1)
                        {
                            newscore = MaxAmenityScore(newAmenityFloor, usedamenities, floor + 1, i + 1);
                        }
                        else newscore = (Score(newAmenityFloor).Item1, newAmenityFloor);
                    
                        //reverse changes for the next configuration
                        usedamenities[i] = false;
                        usedamenities[j] = false;

                        if (newscore.Item1 > maxscore.Item1)
                        {
                            maxscore = newscore;
                        }
                    }
                }
            }
        }
        
        return maxscore;
    }
    
    (int, int[,]) Score(int[] amenityconfig)
    {
        //make a list of all possible combinations of amenities
        //for each combination, run the hungarian algorithm
        
        int[,] matrix = AmenitiesToMatrix(amenityconfig);
        int[,] newmatrix = new int[CUSTOMER_COUNT, FLOOR_COUNT];
        for (int i = 0; i < CUSTOMER_COUNT; i++)
        {
            for (int j = 0; j < FLOOR_COUNT; j++)
            {
                newmatrix[i, j] = matrix[i, j];
            }
        }
        newmatrix = MakeOneToOne(ReduceColumn(ReduceRow(ToMinimization(newmatrix))));
        int[,] matchedgraph = Hungarian(newmatrix);
        return (MatchingScore(matchedgraph, matrix), matchedgraph);
    }
    
    int[,] AmenitiesToMatrix(int[] amenities)
    {
        int[,] matrix = new int[CUSTOMER_COUNT, FLOOR_COUNT];

        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < FLOOR_COUNT; col++)
            {
                matrix[row, col] = CUSTOMER_AMENITY[row][amenities[col * 2]] 
                                   + CUSTOMER_AMENITY[row][amenities[col * 2 + 1]];
            }
        }

        return matrix;
    }
    
    int[,] ToMinimization(int[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i,j] = MAX_CUSTOMER_AMENITY_SCORE - matrix[i,j];
            }
        }
        
        return matrix;
    }
    
    int[,] ReduceRow(int[,] matrix)
    {
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            int rowmin = MAX_CUSTOMER_AMENITY_SCORE;
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                if (matrix[row,col] < rowmin)
                {
                    rowmin = matrix[row,col];
                }
            }

            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                matrix[row,col] -= rowmin;
            }
        }
        
        return matrix;
    }

    int[,] ReduceColumn(int[,] matrix)
    {
        for (int col = 0; col < matrix.GetLength(1); col++)
        {
            int colmin = MAX_CUSTOMER_AMENITY_SCORE;
            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                if (matrix[row,col] < colmin)
                {
                    colmin = matrix[row,col];
                }
            }

            for (int row = 0; row < matrix.GetLength(0); row++)
            {
                matrix[row,col] -= colmin;
            }
        }
        
        return matrix;
    }

    int[,] MakeOneToOne(int[,] matrix)
    {
        int nrows = matrix.GetLength(0);
        int ncols = matrix.GetLength(1);
        int colMult = nrows / ncols;

        int[,] newmatrix = new int[nrows, ncols * colMult];
        
        for (int i = 0; i < ncols; i++)
        {
            for (int k = 0; k < nrows; k++)
            {
                for (int j = 0; j < colMult; j++)
                {
                
                    newmatrix[k, i * colMult + j] = matrix[k, i];
                }
            }
        }
        
        return newmatrix;
    }

    int[,] Hungarian(int[,] matrix)
    {
        //turn matrix into a bipartite graph
        //While an augmenting path exists, add not-matching edges of path to matching and remove matching edges
        
        int[,] graph = ToGraph(matrix);
        int[,] matchedgraph = FindAugmenting(graph);
        int[,] newmatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
        
        for (int row = 0; row < matrix.GetLength(0); row++)
        {
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                newmatrix[row, col] = matrix[row, col];
            }
        }
        
        //matrix.CopyTo(newmatrix, 0);

        while (!IsPerfectMatching(matchedgraph))
        {
            (List<int>, List<int>) Z = ZeroCover(matchedgraph);
            int min = findMinZ(Z.Item1, Z.Item2, newmatrix);
            adjustCells(min, newmatrix, Z.Item1, Z.Item2);
            matchedgraph = ToGraph(newmatrix);
            matchedgraph = FindAugmenting(matchedgraph);
        }
        
        return matchedgraph;
    }

    int MatchingScore(int[,] graph, int[,] matrix)
    {
        int score = 0;

        int nrows = matrix.GetLength(0);
        int ncols = matrix.GetLength(1);
        int colMult = nrows / ncols;

        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                if (graph[row, col] == -1)
                {
                    score += matrix[row, col / colMult];
                    break; //since each customer is only on one floor
                }
            }
        }
        
        return score;
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
        for (int row = 0; row < CUSTOMER_COUNT; row++)
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
                        if (graph[row, Zcols[i]] == -1 && !Zrows.Contains(row))
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

    int findMinZ(List<int> zrows, List<int> zcols, int[,] matrix)
    {
        int min = MAX_CUSTOMER_AMENITY_SCORE;
        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                if (zrows.Contains(row) && !zcols.Contains(col) && matrix[row, col] < min)
                {
                    min = matrix[row, col];
                }

                if (min == 1) return min;
            }
        }
        return min;
    }

    void adjustCells(int min, int[,] matrix, List<int> zrows, List<int> zcols)
    {
        for (int i = 0; i < zrows.Count; i++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                matrix[zrows[i], col] -= min;
            }
        }

        for (int i = 0; i < zcols.Count; i++)
        {
            for (int row = 0; row < CUSTOMER_COUNT; row++)
            {
                matrix[row, zcols[i]] += min;
            }
        }
    }

    int[,] ToGraph(int[,] matrix)
    {
        int[,] graph = new int[CUSTOMER_COUNT + 1, CUSTOMER_COUNT + 1];
        for (int row = 0; row < CUSTOMER_COUNT; row++)
        {
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                graph[row, CUSTOMER_COUNT] = -1;
                graph[CUSTOMER_COUNT, col] = -1;
                
                if (matrix[row, col] == 0)
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
            (bool, List<int>) result = DepthFirstSearch(graph, false, 0, CUSTOMER_COUNT, new List<int>());

            if (result.Item1)
            {
                List<int> augmenting = result.Item2;

                bool isRow = false;
                int loc = augmenting[^1];
                
                //add augmenting edges to or remove them from matching
                for (int i = result.Item2.Count - 2; i >= 0; i--)
                {
                    if (isRow)
                    {
                        if (graph[augmenting[i], loc] == -2)
                        {
                            graph[augmenting[i], loc] = 1;
                        }
                        else if (graph[augmenting[i], loc] == 2)
                        {
                            graph[augmenting[i], loc] = -1;
                        }
                    }
                    else
                    {
                        if (graph[loc, augmenting[i]] == -2)
                        {
                            graph[loc, augmenting[i]] = 1;
                        }
                        else if (graph[loc, augmenting[i]] == 2)
                        {
                            graph[loc, augmenting[i]] = -1;
                        }
                    }

                    loc = augmenting[i];
                    isRow = !isRow;
                }

                graph[result.Item2[^1], CUSTOMER_COUNT] = 0;
                graph[CUSTOMER_COUNT, result.Item2[0]] = 0;
            }

            else
            {
                return graph;
            }
        }
        
    }

    (bool, List<int>) DepthFirstSearch(int[,] graph, bool isRow, int oldrow, int oldcol, List<int> path)
    {
        if (isRow)
        {
            int startcol = oldcol == CUSTOMER_COUNT ? 0 : oldcol;
            for (int col = 0; col < CUSTOMER_COUNT; col++)
            {
                if (graph[oldrow,col] == 1)
                {
                    graph[oldrow, col] = 2;
                    (bool, List<int>) result = DepthFirstSearch(graph, false, oldrow, col, path);
                    if (result.Item1)
                    {
                        path.Add(col);
                        return result;
                    }
                    else graph[oldrow, col] = 1;
                }
            }
                
        }
        else
        {
            if (graph[CUSTOMER_COUNT, oldcol] == -1)
            {
                return (true, path);
            }

            for (int row = 0; row < CUSTOMER_COUNT; row++)
            {
                if (graph[row, oldcol] == -1)
                {
                    graph[row, oldcol] = -2; 
                    (bool, List<int>) result = DepthFirstSearch(graph, true, row, oldcol, path);
                    if (result.Item1)
                    {
                        path.Add(row);
                        return result;
                    }
                    else graph[row, oldcol] = -1;
                }
            }
                
        }
        
        return (false, path);
    }
}