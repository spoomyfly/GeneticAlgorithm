using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Parser
{
    public static class Parser
    {
        public static string[][] Parse(string path) //
        {
            List<string[]> ts = new List<string[]>();
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    if (!sr.ReadLine().StartsWith("#") || !sr.ReadLine().StartsWith(" "))
                    {
                        string[] line = sr.ReadLine().Split(' ');
                        ts.Add(line);
                    }
                }
            }
            return ts.ToArray();
        }

    }

}
