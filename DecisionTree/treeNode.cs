using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTree
{
    class treeNode
    {
        public List<Instance> InstancesList;//has a list of instances passed to this node
        public string AttributeToSplitOn;
        public treeNode PositiveChild;
        public treeNode NegativeChild;
        public double curEntropy;
        public int depth;
        public bool IsLeaf;
        public Dictionary<String, int> classBreakdown;

        public treeNode ()
        {
            InstancesList = new List<Instance>();
            IsLeaf = false;
            curEntropy = 0;
            depth = 0;
            classBreakdown = new Dictionary<string, int>();
        }
        public treeNode (double Entropy,int level)
        {
            InstancesList = new List<Instance>();
            IsLeaf = false;
            curEntropy = Entropy;
            depth = level;
            classBreakdown = new Dictionary<string, int>();
        }
        public void createChildren(List<string> AttributeList, double threshold,int MaxDepth)
        {
            MaxGain mg = FindAttribute(AttributeList);
            if (mg.maxGain < threshold || this.depth >= MaxDepth || String.IsNullOrWhiteSpace(mg.attribute))
            {
                this.IsLeaf = true;
                foreach (var inst in this.InstancesList)
                {
                    if (classBreakdown.ContainsKey(inst.Label))
                        classBreakdown[inst.Label]++;
                    else
                        classBreakdown.Add(inst.Label, 1);
                }
                return;
            }
            this.AttributeToSplitOn = mg.attribute; 
            this.PositiveChild = new treeNode(mg.EntropyPositive,this.depth+1);
            this.NegativeChild = new treeNode(mg.EntropyNegative,this.depth+1);
            foreach (var inst in this.InstancesList)
            {
                if (inst.Features.ContainsKey(mg.attribute) && inst.Features[mg.attribute] > 0)
                    this.PositiveChild.InstancesList.Add(inst);
                else
                    this.NegativeChild.InstancesList.Add(inst);
            }
        }
        public MaxGain FindAttribute (List<string> AttributeList)
        {
            
            MaxGain mg = new MaxGain();
            int countPositive, countNegative;
            foreach (var item in AttributeList)
            {
                countPositive = countNegative = 0;
                // keeps track of the number of examples per class per option of the attribute
                Dictionary<String,int> ClassCountPositive = new Dictionary<String,int>();
                Dictionary<String,int> ClassCountNegative = new Dictionary<String,int>();
                foreach (var inst in this.InstancesList)
                {
                    if (inst.Features.ContainsKey(item) && inst.Features[item] > 0)
                    {
                        countPositive++;
                        if (ClassCountPositive.ContainsKey(inst.Label))
                            ClassCountPositive[inst.Label]++;
                        else
                            ClassCountPositive.Add(inst.Label, 1);
                    }   
                    else
                    {
                        countNegative++;
                        if (ClassCountNegative.ContainsKey(inst.Label))
                            ClassCountNegative[inst.Label]++;
                        else
                            ClassCountNegative.Add(inst.Label, 1);
                    }
                        
                }
                //calculate  entropy for instances where the attribute is positive
                double EntropyPositive = 0;
                double probtemp=0;
                foreach (var label in ClassCountPositive)
                {
                    probtemp = label.Value/((double)countPositive);
                    EntropyPositive += (-1* probtemp * System.Math.Log(probtemp,2));
                }
                //calculate  entropy for instances where the attribute is  negative
                double EntropyNegative = 0;
                foreach (var label in ClassCountNegative)
                {
                    probtemp = label.Value / (( double )countNegative);
                    EntropyNegative += (-1 * probtemp * System.Math.Log(probtemp, 2));
                }
                double totalInstances = countPositive+countNegative;
                double averageChildEntropy = ((countPositive / totalInstances) * EntropyPositive) + ((countNegative / totalInstances) * EntropyNegative);
                double curGain = this.curEntropy - averageChildEntropy;
                if(curGain >  mg.maxGain)
                {
                    mg.maxGain = curGain;
                    mg.attribute = item;
                    mg.EntropyNegative = EntropyNegative;
                    mg.EntropyPositive = EntropyPositive;
                }
            }
            return mg;

        }

    }
}
