namespace CacaSudoku
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string input = Console.ReadLine();
            Sudoku sud = Sudoku.FromString(input);

            sud.Print();
            sud.Generate();
            //sud.Print();

            sud.IteraredLocalSearch(100);

            sud.Print();
        }
    }
}