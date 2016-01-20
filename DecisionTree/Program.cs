using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string TrainingFilePath = args[0];
            string TestFilePath = args[1];
            int maxDepth = Convert.ToInt32(args[2]);
            double minGain = Convert.ToDouble(args[3]);
            string modelFile = args[4];
            string sysOutput = args[5];
            string line;
            treeNode root = new treeNode();
            //create a class for each list
            List<String> AttributeList = new List<string>();
            Dictionary<String, int> ClassBreakDown = new Dictionary<string, int>();
            double totalInstances = 0;
            Stopwatch stopwatch = Stopwatch.StartNew(); //creates and start the instance of Stopwatch
            using (StreamReader Sr = new StreamReader(TrainingFilePath))
            {
                while ((line = Sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    Instance temp = new Instance(words[0]);
                    totalInstances++;
                    if (ClassBreakDown.ContainsKey(words[0]))
                        ClassBreakDown[words[0]]++;
                    else
                        ClassBreakDown.Add(words[0], 1);
                    for (int i = 1; i < words.Length; i++)
                    {
                        string[] pair = words[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (pair.Length != 2)
                            throw new Exception("there is some error with input pairs");
                        string key = pair[0];
                        int value = Convert.ToInt32(pair[1]);
                        if (temp.Features.ContainsKey(key))
                            temp.Features[key] += value ;
                        else
                            temp.Features.Add(key, Convert.ToInt32(pair[1]));
                            
                        AttributeList.Add(key);
                    }
                    root.InstancesList.Add(temp);
                }
            }

            Double entropy = 0;
            foreach (var label in ClassBreakDown)
            {
                double probtemp = label.Value / totalInstances;
                entropy += (-1 * probtemp * System.Math.Log(probtemp, 2));
            }
            root.curEntropy = entropy;
            //get unique values of Attributes;
            AttributeList = AttributeList.Distinct().ToList();
            Stack<treeNode> Tree = new Stack<treeNode>();
            Stack<String> AttributesUsed = new Stack<string>();
            Tree.Push(root);
            //create a tree here
            while(Tree.Count > 0)
            {
                treeNode curNode = Tree.Pop();
                curNode.createChildren(AttributeList, minGain, maxDepth);
                if(!curNode.IsLeaf)
                {
                    Tree.Push(curNode.PositiveChild);
                    Tree.Push(curNode.NegativeChild);
                }
            }

            //print model file
            StreamWriter Sw = new StreamWriter(modelFile);
            PrintTree(root, AttributesUsed,Sw);
            Sw.Close();

            //Read Test File
            List<Instance> TestInstancesList = new List<Instance>();
            using (StreamReader Sr = new StreamReader(TestFilePath))
            {
                while ((line = Sr.ReadLine()) != null)
                {
                    if (String.IsNullOrEmpty(line))
                        continue;
                    string[] words = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    Instance temp = new Instance(words[0]);

                    for (int i = 1; i < words.Length; i++)
                    {
                        string[] pair = words[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if (pair.Length != 2)
                            throw new Exception("there is some error with input pairs");
                        string key = pair[0];
                        int value = Convert.ToInt32(pair[1]);
                        if (temp.Features.ContainsKey(key))
                            temp.Features[key] += value;
                        else
                            temp.Features.Add(key, Convert.ToInt32(pair[1]));
                    }
                    TestInstancesList.Add(temp);
                }
            }
            StreamWriter Sw1 = new StreamWriter(sysOutput);
            String[][] ConfusionArray = new string[TestInstancesList.Count][];
            for (int i = 0; i < TestInstancesList.Count; i++)
            {
                ConfusionArray[i] = new string[2];
                ConfusionArray[i][0] = TestInstancesList[i].Label;
                classify(root, TestInstancesList[i], Sw1, i);
            }
            Sw1.Close();
            stopwatch.Stop();
            Console.WriteLine("Time Elapsed: " + Convert.ToString(stopwatch.ElapsedMilliseconds / 60000) + " minutes");
            Console.ReadLine();
        }

        public static string classify(treeNode root ,Instance curInstance,StreamWriter Sw,int index)
        {
            //base case
            if(root.IsLeaf)
            {
                StringBuilder Sb = new StringBuilder();
                //Sb.Append(curInstance.Label);
                //Sb.Append(" ");
                Sb.Append("array:" + index + " ");
                double totalInstances = root.InstancesList.Count;
                int maxValue = 0;
                string PredClass = "";
                foreach (var label in root.classBreakdown)
                {
                    Sb.Append(label.Key + " " + Convert.ToString(label.Value / totalInstances) + " ");
                    if (label.Value > maxValue)
                        PredClass = label.Key;
                }
                Sw.WriteLine(Sb.ToString());
                return PredClass;
            }
            else
            {
                if (curInstance.Features.ContainsKey(root.AttributeToSplitOn) && curInstance.Features[root.AttributeToSplitOn] > 0)
                    classify(root.PositiveChild, curInstance, Sw, index);
                else
                    classify(root.NegativeChild, curInstance, Sw, index);
            }
        }
        public static void PrintTree (treeNode root, Stack<String> AttributesUsed,StreamWriter Sw)
        {
            if (root.IsLeaf)
            {
                string[] arr = new string[AttributesUsed.Count];
                int i = 0;
                foreach (var item in AttributesUsed)
                {
                    arr[i++] = item;
                }
                double totalInst = root.InstancesList.Count;
                StringBuilder Sb = new StringBuilder();
                Sb.Append(totalInst);
                foreach (var  label in root.classBreakdown)
                {
                    Sb.Append(" ");
                    Sb.Append(label.Key +" "+ Convert.ToString(label.Value / totalInst));
                }
                Sw.WriteLine(String.Join("&", arr.Reverse()) + " " + Sb.ToString());
                return;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(root.AttributeToSplitOn))
                    Console.WriteLine("Empty"); 
                AttributesUsed.Push(root.AttributeToSplitOn);
                PrintTree(root.PositiveChild, AttributesUsed, Sw);
                AttributesUsed.Pop();

                AttributesUsed.Push("!" + root.AttributeToSplitOn);
                PrintTree(root.NegativeChild, AttributesUsed, Sw);
                AttributesUsed.Pop();
            }
            
            
        }
    }
}

