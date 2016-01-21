using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadAndConvertToBinary
{
    class ConvertToBinary
    {
        static void Main (string[] args)
        {
            string TrainingFilePath = args[0];
            string OutputFileBinary = args[1];
            string line;
            StreamWriter Sw = new StreamWriter(OutputFileBinary);
            using (StreamReader Sr = new StreamReader(TrainingFilePath))
            {
                while ((line = Sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    line = Regex.Replace(line, "[1-9]+", "1");
                    line = Regex.Replace(line, "1[0]*", "1");
                    Sw.WriteLine(line);
                }
            }
            Sw.Close();
        }
    }
}
