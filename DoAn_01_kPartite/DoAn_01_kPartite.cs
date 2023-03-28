/*******************************************************************************************************************************
 * 
 * Licensed under GPL version 3 or any later version
 * You can find the detail of the license at https://www.gnu.org/licenses/gpl-3.0.en.html
 * 
 * Copyright (c) 2023 Tran Trung Hieu
 * bsctranhieu@gmail.com
 * 
*******************************************************************************************************************************/

/*
  Build/compile/debug main.exe: 
  csc DoAn_01_kPartite.cs
  (Net5)
*/

/*
Sample

adjacency-list.txt
6           (số đỉnh 6)
5 1 2 3 4 5 (đỉnh 0 nối với 5 đỉnh là 1,2,3,4,5)
3 0 2 5     (đỉnh 1 nối với 3 đỉnh là 0,2,5)
3 0 1 3
3 0 2 4
3 0 3 5
3 0 1 4

Kết quả
Đồ thị k-phân: 4-partite {0} {1, 3} {2, 4} {5}
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;

namespace DoAn_01
{
    class ResultKpartiteGraph
    {
        public int kMin;
        public string kMinDS;
        public List<string> kDSubsets;
    }
    class AdjacencyList
    {
        public int n;
        public int[,] data;
        public bool ReadAdjacencyList(string filename)
        {
            if (!File.Exists(filename))
            {

                Console.WriteLine("This file does not exist.");

                return false;
            }
            string[] lines = File.ReadAllLines(filename);

            n = Int32.Parse(lines[0]);
            data = new int[n, n];
            for (int i = 0; i < n; ++i)
            {

                string[] tokens = lines[i + 1].Split(new char[]
                { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                data[i, 0] = Int32.Parse(tokens[0]);
                for (int j = 1; j < data[i, 0] + 1; ++j)

                    data[i, j] = Int32.Parse(tokens[j]);

            }
            return true;
        }
        public int ExpNum(int a, int b)
        {
            int result = 1;
            for (int i = 0; i < b; i++)
            {
                result = result * a;
            }
            return result;
        }
        public void D2Array(int dec, int[] result, int size, int oBase)
        {
            if (size == 0) return;
            result[size - 1] = dec % oBase;
            int num = result[size - 1];
            size--;
            D2Array(dec / oBase, result, size, oBase);
        }
        public string A2S(int[] a, int s)
        {
            string r = "";
            for (int j = 0; j < s; j++)
            {
                int c;
                if (a[j] + '0' <= '9') c = a[j] + '0';
                else c = a[j] + 'A' - 10;
                r += (char)c;
            }
            return r;
        }
        public bool CheckDistinct(int[] bArray, int size, int[,] data)
        {
            for (int i = 0; i < size; i++)
            {
                if (bArray[i] == 0) continue;
                for (int j = i + 1; j < size; j++)
                {
                    if (bArray[j] == 0) continue;
                    // check whether node i and node j has an edge
                    //Console.Write(data[i, 0] + ":");
                    for (int k = 1; k < data[i, 0] + 1; k++)
                    {
                        //Console.Write(data[i, k] + " ");
                        if (j == data[i, k])
                        {
                            //Console.Write("FalseD ");
                            return false;
                        }
                    }
                }
            }
            //Console.Write("TrueD ");
            return true;
        }
        public bool CheckFull(int[] bArrayC, List<int[]> allDSubsets, List<int[]> DSubsets)
        {
            int[] rArray = new int[this.n];
            for (int i = 0; i < bArrayC.Length; i++)
            {
                if (bArrayC[i] == 0) continue;
                for (int j = 0; j < allDSubsets[i].Length; j++)
                {
                    if (allDSubsets[i][j] == 1 && rArray[j] == 1) return false;
                    if (allDSubsets[i][j] == 1 && rArray[j] == 0) rArray[j] = 1;
                }
                DSubsets.Add(allDSubsets[i]);
            }
            for (int i = 0; i < rArray.Length; i++)
            {
                if (rArray[i] == 0) return false;
            }
            return true;
        }
        public ResultKpartiteGraph CheckKpartiteGraph()
        {
            int oBase = 2;
            List<int[]> allDSubsets = new List<int[]>();
            for (int i = 1; i < ExpNum(2, n); i++)
            {
                int[] bArray = new int[n];
                D2Array(i, bArray, n, oBase);
                if (CheckDistinct(bArray, n, data))
                {
                    allDSubsets.Add(bArray);
                }
            }
            int size = allDSubsets.Count;
            List<string> kDSubsets = new List<string>();
            string kMinDS = "";
            int kMin = n + 1;
            for (int k = 0; k < ExpNum(2, size); k++)
            {
                int[] bArrayC = new int[size];
                D2Array(k, bArrayC, size, oBase);
                List<int[]> DSubsets = new List<int[]>();
                if (CheckFull(bArrayC, allDSubsets, DSubsets))
                {
                    for (int i = 0; i < bArrayC.Length; i++)
                    {
                        if (bArrayC[i] == 0) continue;
                    }
                    string kDStr = "";
                    for (int i = 0; i < DSubsets.Count; i++)
                    {
                        kDStr += "{";
                        int count = 0;
                        for (int j = 0; j < DSubsets[i].Length; j++)
                        {
                            if (DSubsets[i][j] == 0) continue;
                            if (count != 0) kDStr += ",";
                            kDStr += j;
                            count++;
                        }
                        kDStr += "} ";
                    }
                    kDSubsets.Add(kDStr);

                    if (kMin > DSubsets.Count)
                    {
                        kMin = DSubsets.Count;
                        kMinDS = kDStr;
                    }
                }
            }
            ResultKpartiteGraph result = new ResultKpartiteGraph();
            result.kMin = kMin;
            result.kMinDS = kMinDS;
            result.kDSubsets = kDSubsets;
            return result;
        }
        public void CheckBiPartiteGraph()
        {
            var r = CheckKpartiteGraph();
            if (r.kMin > 2)
                Console.WriteLine($"Do thi luong phan: Khong");
            else
                Console.WriteLine($"Do thi luong phan: {r.kMinDS}");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string ALfile_path = "D:\\adjacency-list.txt";
            AdjacencyList gl = new AdjacencyList();
            gl.ReadAdjacencyList(ALfile_path);
            var r = gl.CheckKpartiteGraph();
            Console.WriteLine($"Đo thi k-phan (kMin = {r.kMin}): {r.kMin}-partite {r.kMinDS}");

            Console.WriteLine($"\nTat ca cac truong hop k-partite: ");
            for (int i = 0; i < r.kDSubsets.Count; i++)
            {
                Console.WriteLine($"subsets = {r.kDSubsets[i]}");
            }
            Console.ReadLine();

            gl.CheckBiPartiteGraph();

        }
    }
}
