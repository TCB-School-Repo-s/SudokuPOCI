namespace CacaSudoku
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            Sudoku sud = Sudoku.FromString(input);

            //sud.Swap(0, 0, 2);
            sud.Generate();


            Console.WriteLine(sud.ToString());

        }
    }
}