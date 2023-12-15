//using Microsoft.Data.Analysis;
using System.Diagnostics;

namespace CacaSudoku
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Following code was used for the experiment
            /*var sList = new List<Single>() { 0 };
            var repList = new List<Single>() { 0 };
            var timeList = new List<Single>() { 0 };

            

            // To run the code below you need to install the Microsoft.Data.Analysis; package.
            var sColumn = new SingleDataFrameColumn("S", sList);
            var repColumn = new SingleDataFrameColumn("repAllowed", repList);
            var timeColumn = new SingleDataFrameColumn("timeTaken", timeList);
            var dataframe = new DataFrame(sColumn, repColumn, timeColumn); 


            /* We testen de snelheid en efficientie van het algoritme voor verschillende waarden van s, repAllowed 
             * en we voeren het voor elke sudoku 10 keer uit om tot een goed gemiddelde te komen van hoelang hij er over doet om het op te lossen.
             * */
            /*for (int s = 32; s <= 32; s = s * 2) // s waardes [10, 20, 30, 40, 50, 60, 70, 80, 90, 100]
            {
                for (int repAllowed = 15; repAllowed <= 20; repAllowed += 5) // rep allowed [5, 10, 15, 20, 25, 30]
                {
                    List<int> times = new List<int>();
                    for (int n = 0; n < 5; n++) // Hoe vaak per parameter, voor average time? 
                    {
                        Sudoku sud = Sudoku.FromString("0 3 0 0 5 0 0 4 0 0 0 8 0 1 0 5 0 0 4 6 0 0 0 0 0 1 2 0 7 0 5 0 2 0 8 0 0 0 0 6 0 3 0 0 0 0 4 0 1 0 9 0 3 0 2 5 0 0 0 0 0 9 8 0 0 1 0 2 0 6 0 0 0 8 0 0 6 0 0 2 0\r\n");

                        //sud.Generate();
                        sud.Print();

                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        sud.IteratedLocalSearch(s, repAllowed, false);
                        stopwatch.Stop();
                        times.Add((int)stopwatch.ElapsedMilliseconds);
                    }
                    List<KeyValuePair<string, object>> newRowData = new()
                    {
                        new KeyValuePair<string, object>("S", s),
                        new KeyValuePair<string, object>("repAllowed", repAllowed),
                        new KeyValuePair<string, object>("timeTaken", times.Average()),
                    };

                   // dataframe.Append(newRowData, inPlace: true);
                    Console.WriteLine($"S: {s}; repAllowed: {repAllowed}; average time taken: {times.Average()}");
                }
            }

            DataFrame.SaveCsv(dataframe, $"result-{DateTime.Now.Millisecond}.csv", ';');
            */

            Sudoku sud =
                Sudoku.FromString(
                    "0 0 3 0 2 0 6 0 0 9 0 0 3 0 5 0 0 1 0 0 1 8 0 6 4 0 0 0 0 8 1 0 2 9 0 0 7 0 0 0 0 0 0 0 8 0 0 6 7 0 8 2 0 0 0 0 2 6 0 9 5 0 0 8 0 0 2 0 3 0 0 9 0 0 5 0 1 0 3 0 0");
            
            sud.Print();
            sud.Generate();
            sud.IteratedLocalSearch(10, 10, false);

            Console.WriteLine("Solution found!");
            sud.Print();
            
        }
    }
}
