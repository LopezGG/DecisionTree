using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class MaxGain
    {
        public double maxGain;
        public string attribute;
        public double EntropyPositive;
        public double EntropyNegative;
        public MaxGain ()
        {
            maxGain = EntropyPositive = EntropyNegative = 0;
            attribute = "";
        }
    }
}
