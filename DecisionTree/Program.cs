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
            treeNode root = new treeNode();
            //create a class for each list
            List<String> AttributeList = new List<string>();
            Dictionary<String, int> ClassBreakDown = new Dictionary<string, int>();
            double totalInstances = 0;
            using (StreamReader Sr = new StreamReader(inputFilePath))
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
                curNode.createChildren(AttributeList, 0, 10);
                if(!curNode.IsLeaf)
                {
                    Tree.Push(curNode.PositiveChild);
                    Tree.Push(curNode.NegativeChild);
                }
            }
            //print rules
            PrintTree(root, AttributesUsed);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        public static void PrintTree (treeNode root, Stack<String> AttributesUsed)
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
                Console.WriteLine(String.Join("&",arr.Reverse()) + " "+ Sb.ToString());
                return;
            }
            else
            {
                if (String.IsNullOrWhiteSpace(root.AttributeToSplitOn))
                    Console.WriteLine("Empty"); 
                AttributesUsed.Push(root.AttributeToSplitOn);
                PrintTree(root.PositiveChild, AttributesUsed);
                AttributesUsed.Pop();

                AttributesUsed.Push("!" + root.AttributeToSplitOn);
                PrintTree(root.NegativeChild, AttributesUsed);
                AttributesUsed.Pop();
            }
            
            
        }
    }
}

