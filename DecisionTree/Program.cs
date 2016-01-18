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
            //stores id of the string corresponding to a particular class
            List<String> ClassList = new List<String>();
            List<Dictionary<string, int>> InstanceDictList = new List<Dictionary<string, int>>();
            using (StreamReader Sr = new StreamReader(inputFilePath))
            {
                while ((line = Sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    Dictionary<String, int> temp = new Dictionary<string, int>();
                    for (int i = 1; i < words.Length; i++)
                    {
                        string[] pair = words[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (pair.Length != 2)
                            throw new Exception("there is some error with input pairs");
                        if (temp.ContainsKey(pair[0]))
                            temp[pair[0]] += Convert.ToInt32(pair[1]);
                        else
                            temp.Add(pair[0], Convert.ToInt32(pair[1]));
                    }
                    ClassList.Add(words[0]);
                    InstanceDictList.Add(temp);
                }
            }

        }
    }
}

