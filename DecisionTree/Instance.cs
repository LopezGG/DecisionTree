using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class Instance
    {
        public string Label;
        public Dictionary<String, int> Features;
        public Instance ()
        {
            Label = "";
            Features = new Dictionary<string, int>();
        }
        public Instance (string label)
        {
            Label = label;
            Features = new Dictionary<string, int>();
        }
    }
}
