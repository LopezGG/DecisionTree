﻿using System;
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
            string sysTraining = args[6];


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
            ClassifyandWrite(sysTraining, root, ClassBreakDown, root.InstancesList, "Train");
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
            ClassifyandWrite(sysOutput, root, ClassBreakDown, TestInstancesList,"Test");
            stopwatch.Stop();
            //Console.WriteLine("Time Elapsed: " + Convert.ToString(stopwatch.ElapsedMilliseconds / 60000) + " minutes");
            Console.ReadLine();
        }
        public static void ClassifyandWrite (string sysOutput,treeNode root, Dictionary<String, int> ClassBreakDown, List<Instance> TestInstancesList, string testOrTrain)
        {
            StreamWriter Sw1 = new StreamWriter(sysOutput);
            Dictionary<String, int> ConfusionDictTest = new Dictionary<string, int>();
            string st1, st2;
            for (int i = 0; i < TestInstancesList.Count; i++)
            {
                st1 = TestInstancesList[i].Label;
                st2 = classify(root, TestInstancesList[i], Sw1, i);
                st1 = st1 + "_" + st2;
                if (ConfusionDictTest.ContainsKey(st1))
                    ConfusionDictTest[st1]++;
                else
                    ConfusionDictTest.Add(st1, 1);
            }
            int correctPred = 0;
            Console.WriteLine("Confusion matrix for the"+ testOrTrain + "data:\n row is the truth, column is the system output");
            Console.Write("\t\t\t");
            foreach (var actClass in ClassBreakDown)
            {
                Console.Write(actClass.Key + "\t");
            }
            Console.WriteLine();
            foreach (var actClass in ClassBreakDown)
            {
                st1 = actClass.Key;
                Console.Write(st1 + "\t");
                foreach (var predClass in ClassBreakDown)
                {
                    st2 = predClass.Key;
                    if (ConfusionDictTest.ContainsKey(st1 + "_" + st2))
                    {
                        Console.Write(ConfusionDictTest[st1 + "_" + st2] + "\t");
                        if (st1 == st2)
                            correctPred += ConfusionDictTest[st1 + "_" + st2];
                    }
                    else
                        Console.Write("0" + "\t");

                }
                Console.WriteLine();
            }
            Console.WriteLine(testOrTrain + " accuracy=" + Convert.ToString(correctPred / ( double )TestInstancesList.Count));
            Console.WriteLine();
            Sw1.Close();
        }
        public static string classify(treeNode root ,Instance curInstance,StreamWriter Sw,int index)
        {
            string PredClass = "";
            //base case
            if(root.IsLeaf)
            {
                StringBuilder Sb = new StringBuilder();
                //Sb.Append(curInstance.Label);
                //Sb.Append(" ");
                Sb.Append("array:" + index + " ");
                double totalInstances = root.InstancesList.Count;
                int maxValue = 0;
                
                foreach (var label in root.classBreakdown)
                {
                    Sb.Append(label.Key + " " + Convert.ToString(label.Value / totalInstances) + " ");
                    if (label.Value > maxValue)
                        PredClass = label.Key;
                }
                Sw.WriteLine(Sb.ToString());
                
            }
            else
            {
                if (curInstance.Features.ContainsKey(root.AttributeToSplitOn) && curInstance.Features[root.AttributeToSplitOn] > 0)
                    PredClass=classify(root.PositiveChild, curInstance, Sw, index);
                else
                    PredClass=classify(root.NegativeChild, curInstance, Sw, index);
            }
            return PredClass;
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

