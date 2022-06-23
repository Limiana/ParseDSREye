using System;
using System.IO;
using System.Text;

namespace TestProgram
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Path to log: ");
            var file = Console.ReadLine().Replace("\"", "");
            using var inputFile = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(inputFile);
            string l1 = null;
            string sig1 = null;
            long time1 = 0;
            string x = null;
            string y = null;
            bool seeking = false;
            while(true)
            {
                var line = reader.ReadLine();
                if(line == null)
                {
                    break;
                }
                var chunks = line.Split("|");
                if (chunks[0] == "00" && chunks[2] == "0017")
                {
                    //Console.WriteLine(line);
                    if (chunks[4].Contains("00020001,"))
                    {
                        sig1 = chunks[4][..10];
                        l1 = chunks[4];
                        time1 = DateTimeOffset.Parse(chunks[1]).ToUnixTimeSeconds();
                        seeking = true;
                    }
                    else if (chunks[4].StartsWith($"{sig1}00400020"))
                    {
                        StringBuilder sb = new();
                        var time = DateTimeOffset.Parse(chunks[1]).ToUnixTimeSeconds() - time1;
                        sb.Append($"Data: {l1}");
                        if (seeking)
                        {
                            sb.Append(" Warning: not found thordan pos for this data");
                        }
                        else
                        {
                            sb.Append($" Position: {x}, {y}");
                        }
                        Console.WriteLine(sb.ToString());
                        seeking = false;
                        sig1 = l1 = x = y = null;
                        time1 = 0;
                    }
                }
                else if (seeking && chunks[0] == "03" && chunks[3] == "King Thordan" && chunks[19] == "6.50")
                {
                    seeking = false;
                    x = chunks[17];
                    y = chunks[18];
                }
            }
            Console.WriteLine("end");
            Console.ReadLine();
        }
    }
}
