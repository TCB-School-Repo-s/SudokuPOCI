using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CacaSudoku
{

    internal static class Util
    {
        public static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < s.Length; i += partLength)
                yield return s.Substring(i, Math.Min(partLength, s.Length - i));
        }
    }

    internal class Sudoku
    {

        private int[][] puzzle = new int[9][];

        public Sudoku(int[] first, int[] second, int[] third, int[] fourth, int[] fifth, int[] sixth, int[] seventh, int[] eight, int[] ninth)
        {
            puzzle[0] = first;
            puzzle[1] = second;
            puzzle[2] = third;
            puzzle[3] = fourth;
            puzzle[4] = fifth;
            puzzle[5] = sixth;
            puzzle[6] = seventh;
            puzzle[7] = eight;
            puzzle[8] = ninth;
        }

        public Sudoku(int[][] puzzle)
        {
            this.puzzle = puzzle;
        } 

        public int[][] Puzzle
        {
            get { return puzzle; }
        }

        /// <summary>
        /// Method <c>Swap</c> swaps the two given indexes of puzzle p;
        /// </summary>
        /// <param name="p">Which block you want to swap in</param>
        /// <param name="i">First number you want to swap</param>
        /// <param name="j">Second number you want to swap</param>
        public void Swap(int p, int i, int j)
        {
            (this.puzzle[p][i], this.puzzle[p][j]) = (this.puzzle[p][j], this.puzzle[p][i]);
        }

        /// <summary>
        /// Method <c>Generate</c> randomizes the remaining zero's to random numbers between 1-9 that are not yet in the block.
        /// </summary>
        public void Generate() // fills the sudoku with the missing numbers for each block
        {
            Random rand = new Random();

            // check if number is in puzzle and if yes, remove from list
            for (int i = 0; i < puzzle.Count(); i++)
            {
                List<int> numberList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

                numberList = numberList.Except(puzzle[i]).ToList();

                // replace zeros with numbers from the randomized list, remove when placed.
                for (int j = 0; j < puzzle[i].Count(); j++)
                {
                    if (puzzle[i][j] == 0)
                    {
                        int t = rand.Next(numberList.Count);
                        puzzle[i][j] = numberList[t];
                        numberList.Remove(numberList[t]);

                    }
                }

            }
        }

        /// <summary>
        /// Method <c>ToString</c> converts the Sudoku to a string
        /// </summary>
        /// <returns>String of the sudoku</returns>
        public String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");

            Array.ForEach(puzzle, (int[] p) =>
            {
                Array.ForEach(p, (int i) =>
                {
                    sb.Append(i);
                    sb.Append(" ");
                });
            });

            sb.Append(" ");

            return sb.ToString();
        }


        /// <summary>
        /// Method <c>FromString</c> generates a sudoku object from a string.
        /// </summary>
        /// <param name="str">The string to generate sudoku from</param>
        /// <returns>Sudoku object</returns>
        public static Sudoku FromString(string str)
        {
            // Remove any whitespace from the input string
            str = str.Replace(" ", "");

            // Use the SplitInParts method to split the string into chunks of 9 characters
            var chunks = Util.SplitInParts(str, 9);

            // Initialize a 2D array to store the puzzle
            int[][] pussie = new int[9][];

            // Iterate through the chunks and convert them to arrays of integers
            for (int i = 0; i < 9; i++)
            {
                pussie[i] = chunks.ElementAt(i).Select(c => int.Parse(c.ToString())).ToArray();
            }

            // Create and return a new Sudoku object with the initialized puzzle
            return new Sudoku(pussie[0], pussie[1], pussie[2], pussie[3], pussie[4], pussie[5], pussie[6], pussie[7], pussie[8]);
        }

    }
}
