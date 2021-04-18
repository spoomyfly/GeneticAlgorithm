using System;
using System.Collections.Generic;
using System.IO;

namespace Parser
{
    public class Parser
    {
        public static string[][] Parse(string path) //
        {
            List<string[]> ts = new List<string[]>();
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (char.IsDigit(line[0]))
                    {
                        string[] divided = line.Split(' '); // index, x, y
                        ts.Add(divided);
                    }
                }
            }
            return ts.ToArray();
        }
    }
}
