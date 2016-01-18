using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class Program
    {
        static void Main (string[] args)
        {
            string inputFilePath = args[0];
            string line;
            List<int> PositiveClass = new List<int>();
            List<int> NegativeClass = new List<int>();
            List<Dictionary<string, bool>> InstanceDictList = new List<Dictionary<string, bool>>();
            int positive = 0;
            int negative = 0;
            using (StreamReader Sr = new StreamReader(inputFilePath))
            {
                while ((line = Sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 1; i < words.Length; i++)
                    {
                        string[] pair = words[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (pair.Length != 2)
                            throw new Exception("there is some error with input pairs");

                    }
                }
            }

        }
    }
}

