using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace CacaSudoku
{
    
    internal class Sudoku
    {

        private (int, Boolean)[,] puzzle = new (int, Boolean)[9, 9];
        private int[,] evalMatrix = new int[2, 9];
        private int timeTaken = 0;

        public Sudoku(int[,] puzzle)
        {
            puzzle = puzzle;
        }


        public (int, Boolean)[,] Puzzle
        {
            get { return puzzle; }
        }

        /// <summary>
        /// Method <c>Swap</c> swaps the two given indexes of puzzle p;
        /// If the boolean is False you can swap the indexes.
        /// It updates the evaluation matrix if the swap has a better/equal score. 
        /// </summary>
        /// <param name="p">Which block you want to swap in</param>
        /// <param name="i">First number you want to swap</param>
        /// <param name="j">Second number you want to swap</param>
        /// <param name="eval">Allows all swaps to take place regardless of score if set to false</param>
        public void Swap(int p, int i, int j, Boolean eval = true)

        {
            // Transform the locations of the numbers into locations in the specific block in the large 9x9 array.
            if (this.puzzle[p / 3 * 3 + j / 3, p % 3 * 3 + j % 3].Item2 == false)
            {
                (this.puzzle[p / 3 * 3 + i / 3, p % 3 * 3 + i % 3], this.puzzle[p / 3 * 3 + j / 3, p % 3 * 3 + j % 3]) = (this.puzzle[p / 3 * 3 + j / 3, p % 3 * 3 + j % 3], this.puzzle[p / 3 * 3 + i / 3, p % 3 * 3 + i % 3]);

                int[,] update = Update(p, i, j);

                if (GetScore(update) > GetScore(evalMatrix) && eval)
                {
                    (this.puzzle[p / 3 * 3 + i / 3, p % 3 * 3 + i % 3], this.puzzle[p / 3 * 3 + j / 3, p % 3 * 3 + j % 3]) = (this.puzzle[p / 3 * 3 + j / 3, p % 3 * 3 + j % 3], this.puzzle[p / 3 * 3 + i / 3, p % 3 * 3 + i % 3]);
                }
                else if (GetScore(update) <= GetScore(evalMatrix) && eval)
                {
                    evalMatrix = (int[,])update.Clone();
                }
                else
                {
                    evalMatrix = (int[,])update.Clone();
                }
            }
        }


        /// <summary>
        /// Method <c>Evaluate</c> evaluetes the score of the entire sudoku.
        /// It stores the data in a 2x9 array that has the values for each row and column.
        /// </summary>
        private void Evaluate()
        {
            // calculates the score for each row and stores it in the first row of the evaluation matrix.
            for (int row = 0; row < 9; row++)
            {
                int[] Row = Enumerable.Range(0, puzzle.GetUpperBound(1) + 1).Select(i => puzzle[row, i].Item1).ToArray();
                evalMatrix[0, row] = CountIncorrect(Row);
            }

            // calculates the score for each column and stores it in the second row of the evaluation matrix.
            for (int col = 0; col < 9; col++)
            {
                int[] column = new int[9];
                for (int row = 0; row < 9; row++)
                {
                    column[row] = puzzle[row, col].Item1;
                }

                evalMatrix[1, col] = CountIncorrect(column);
            }

        }
        /// <summary>
        /// Method <c>CountIncorrect</c> calculates the amount of missing numbers in a row or column.
        /// </summary>
        /// <param name="RowCol">Which row or column you want to calculate the score of</param>
        /// <returns></returns>
        public int CountIncorrect(int[] RowCol)
        { 

            HashSet<int> seen = new HashSet<int>();
            int incorrectCount = 0;

            foreach (var value in RowCol) 
            {
                if (!seen.Add(value)) // if you cannot add an item, meaning it is in the list, it increase the count by one. 
                {
                    incorrectCount++;
                }
            }

            return incorrectCount;
        }

        /// <summary>
        /// Method <c>Update</c>  updates the score for the affected rows and colums when a swap takes place.
        /// </summary>
        /// <param name="p">Which block was swapped swap in</param>
        /// <param name="i">First number that was swapped</param>
        /// <param name="j">Second number that was swapped</param>
        /// <returns></returns>
        public int[,] Update(int p, int i, int j)
        {
            int[,] copyEval = (int[,])evalMatrix.Clone();

            // transform the value of the puzzle and first swapped number into the corresponding row/col.
            int rowI = p / 3 * 3 + i / 3;
            int columnI = p % 3 * 3 + i % 3;

            int[] RowI = Enumerable.Range(0, puzzle.GetUpperBound(1) + 1).Select(q => puzzle[rowI, q].Item1).ToArray();
            int[] ColumnI = new int[9];
            for (int row = 0; row < 9; row++)
            {
                ColumnI[row] = puzzle[row, columnI].Item1;
            }
            copyEval[0, rowI] = CountIncorrect(RowI);
            copyEval[1, columnI] = CountIncorrect(ColumnI);

            // transform the value of the puzzle and second swapped number into the corresponding row/col.
            int rowJ = p / 3 * 3 + j / 3;
            int columnJ = p % 3 * 3 + j % 3;

            int[] RowJ = Enumerable.Range(0, puzzle.GetUpperBound(1) + 1).Select(q => puzzle[rowJ, q].Item1).ToArray();
            int[] ColumnJ = new int[9];
            for (int row = 0; row < 9; row++)
            {
                ColumnJ[row] = puzzle[row, columnJ].Item1;
            }
            copyEval[0, rowJ] = CountIncorrect(RowJ);
            copyEval[1, columnJ] = CountIncorrect(ColumnJ);
            return copyEval;
        }

        /// <summary>
        /// Method <c>Generate</c> randomizes the remaining zero's to random numbers between 1-9 that are not yet in the block.
        /// At the end it generates the first evaluation matrix.
        /// </summary>
        public void Generate()
        {
            Random rand = new Random();

            for (int block = 0; block < 9; block++)
            {
                List<int> availableNumbers = Enumerable.Range(1, 9).ToList();
                // check which numbers are fixed and remove them from the list.
                for (int k = 0; k < 9; k++)
                {
                    if (puzzle[block / 3 * 3 + k / 3, block % 3 * 3 + k % 3].Item2 == true)
                    {
                        availableNumbers.Remove(puzzle[block / 3 * 3 + k / 3, block % 3 * 3 + k % 3].Item1);
                    }

                }

                for (int i = 0; i < 9; i++)
                { // add the remaining numbers in empty spaces at random and remove from the list once added. 
                    if (puzzle[block / 3 * 3 + i / 3, block % 3 * 3 + i % 3].Item2 == false)
                    {
                        int randomIndex = rand.Next(availableNumbers.Count);
                        puzzle[block / 3 * 3 + i / 3, block % 3 * 3 + i % 3] = (availableNumbers[randomIndex], false);
                        availableNumbers.RemoveAt(randomIndex);
                    }
                }
            }


            Evaluate();
        }

/*
        // Helper method to check if a number is already in the block
        private bool IsNumberInBlock(int blockIndex, int number)
        {
            for (int i = 0; i < 9; i++)
            {
                if (puzzle[blockIndex, i].Item1 == number)
                {
                    return true;
                }
            }
            return false;
        }
*/

        //// <summary>
        /// Method <c>FromString</c> generates a sudoku object from a string.
        /// </summary>
        /// <param name="str">The string to generate sudoku from</param>
        /// <returns>Sudoku object</returns>
        public static Sudoku FromString(string str)
        {
            Sudoku sudoku = new Sudoku(new int[9, 9]);

            string[] values = str.Split(' ');
            int index = 0;

            // fill in the numbers in each row, and add a bool to show whether it is a fixed or swappable number.
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    int value = int.Parse(values[index]);
                    bool isFixed = (value != 0); // If the value is not 0, it's fixed
                    sudoku.Puzzle[i, j] = (value, isFixed);
                    index++;
                }
            }

            return sudoku;
        }

        // prints the entire sudoku with axis to help determine where potential issues were.
        public void Print()
        {
            Console.WriteLine("*|-A-|-B-|-C-|-D-|-E-|-F-|-G-|-H-|-I-|*");
            Console.WriteLine($"0| {puzzle[0, 0].Item1} | {puzzle[0, 1].Item1} | {puzzle[0, 2].Item1} | {puzzle[0, 3].Item1} | {puzzle[0, 4].Item1} | {puzzle[0, 5].Item1} | {puzzle[0, 6].Item1} | {puzzle[0, 7].Item1} | {puzzle[0, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"1| {puzzle[1, 0].Item1} | {puzzle[1, 1].Item1} | {puzzle[1, 2].Item1} | {puzzle[1, 3].Item1} | {puzzle[1, 4].Item1} | {puzzle[1, 5].Item1} | {puzzle[1, 6].Item1} | {puzzle[1, 7].Item1} | {puzzle[1, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"2| {puzzle[2, 0].Item1} | {puzzle[2, 1].Item1} | {puzzle[2, 2].Item1} | {puzzle[2, 3].Item1} | {puzzle[2, 4].Item1} | {puzzle[2, 5].Item1} | {puzzle[2, 6].Item1} | {puzzle[2, 7].Item1} | {puzzle[2, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"3| {puzzle[3, 0].Item1} | {puzzle[3, 1].Item1} | {puzzle[3, 2].Item1} | {puzzle[3, 3].Item1} | {puzzle[3, 4].Item1} | {puzzle[3, 5].Item1} | {puzzle[3, 6].Item1} | {puzzle[3, 7].Item1} | {puzzle[3, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"4| {puzzle[4, 0].Item1} | {puzzle[4, 1].Item1} | {puzzle[4, 2].Item1} | {puzzle[4, 3].Item1} | {puzzle[4, 4].Item1} | {puzzle[4, 5].Item1} | {puzzle[4, 6].Item1} | {puzzle[4, 7].Item1} | {puzzle[4, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"5| {puzzle[5, 0].Item1} | {puzzle[5, 1].Item1} | {puzzle[5, 2].Item1} | {puzzle[5, 3].Item1} | {puzzle[5, 4].Item1} | {puzzle[5, 5].Item1} | {puzzle[5, 6].Item1} | {puzzle[5, 7].Item1} | {puzzle[5, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"6| {puzzle[6, 0].Item1} | {puzzle[6, 1].Item1} | {puzzle[6, 2].Item1} | {puzzle[6, 3].Item1} | {puzzle[6, 4].Item1} | {puzzle[6, 5].Item1} | {puzzle[6, 6].Item1} | {puzzle[6, 7].Item1} | {puzzle[6, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"7| {puzzle[7, 0].Item1} | {puzzle[7, 1].Item1} | {puzzle[7, 2].Item1} | {puzzle[7, 3].Item1} | {puzzle[7, 4].Item1} | {puzzle[7, 5].Item1} | {puzzle[7, 6].Item1} | {puzzle[7, 7].Item1} | {puzzle[7, 8].Item1} |");
            Console.WriteLine(" |---|---|---|---|---|---|---|---|---|");
            Console.WriteLine($"8| {puzzle[8, 0].Item1} | {puzzle[8, 1].Item1} | {puzzle[8, 2].Item1} | {puzzle[8, 3].Item1} | {puzzle[8, 4].Item1} | {puzzle[8, 5].Item1} | {puzzle[8, 6].Item1} | {puzzle[8, 7].Item1} | {puzzle[8, 8].Item1} |");
            Console.WriteLine("*|---|---|---|---|---|---|---|---|---|");
        }
        /// <summary>
        /// Method <c>IteratedLocalSearch</c> is the algorithm that find a solution.
        /// It keeps track of the last score in order for it to start a random walk if repeated to often.
        /// </summary>
        /// <param name="s">The length of the random walk</param>
        /// <param name="repAllowed">The allowed amount of repetitions of a score</param>
        /// <param name="verbose">Prints the score if set to true</param>
        public void IteratedLocalSearch(int s, int repAllowed = 10, bool verbose = false)
        {
            int rep = 0;
            int sDone = 0;
            int lastScore = GetScore(evalMatrix);
            Random rd = new Random();
            // stops when the score is 0 since the sudoku is solved then.
            while (GetScore(evalMatrix) != 0)
            {
                lastScore = GetScore(evalMatrix);
                int blokIndex = rd.Next(0, 9);

                if (rep < repAllowed) // checks if rep exceeds repAllowed to start random walk if it does.
                {
                    for (int i = 0; i < 9; i++)
                    {
                        if (puzzle[blokIndex / 3 * 3 + i / 3, blokIndex % 3 * 3 + i % 3].Item2 == false) // check if the first number is not fixed.
                        {
                            for (int j = 0; j < 9; j++)
                            {
                                Swap(blokIndex, i, j); // tries out each swap combination and uses the best one
                            }
                        }
                    }

                }
                else
                {
                    if (sDone <= s) // continues as long as length of random walk is not reached.
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (puzzle[blokIndex / 3 * 3 + i / 3, blokIndex % 3 * 3 + i % 3].Item2 == false)
                            {
                                for (int j = 0; j < 9; j++)
                                {
                                    Swap(blokIndex, i, j, false); // allows all swaps regardless of score
                                }
                            }
                        }
                        sDone = (sDone == s) ? sDone = 0 : sDone + 1;
                    }
                }

                int score = GetScore(evalMatrix);
                rep = (score == lastScore) ? rep + 1 : rep = 0; // compares the current score to the last score and increases rep if they are equal.
                if (verbose) Console.WriteLine($"Score: {score}"); // prints the score if verbose = true
            }
            //Console.WriteLine($"Solution found!");
        }

        /// <summary>
        /// Method <c>GetScore</c> sums the score for the 2x9 evaluation array.
        /// </summary>
        /// <param name="arr">the evaluation array to be summed</param>
        /// <returns></returns>
        public int GetScore(int[,] arr)
        {
            int sum = 0;

            // Iterate through rows
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                // Iterate through columns
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    // Add the current element to the sum
                    sum += arr[i, j];
                }
            }

            return sum;
        }
    }
}
