namespace CacaSudoku
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            Sudoku sud = Sudoku.fromString(input);

            sud.Swap(0, 0, 2);


            Console.WriteLine(sud.Puzzle[0][2]);

        }
    }
}