using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace MazeGenerator
{
    public static class PathFind
    {
        public static Node[,] graph(bool[,] table)
        {
            int width = table.GetLength(0);
            int hight = table.GetLength(1);
            Node[,] graph = new Node[width, hight];

            for (int x = 0; x < width; x++)
            {

                for (int y = 0; y < hight; y++)
                {
                    graph[x, y] = new Node(x, y);

                }
            }
            for (int x = 0; x < width - 1; x++)
            {
                for (int y = 0; y < hight - 1; y++)
                {
                    if (table[x, y] == false)
                    {
                        if (table[x + 1, y] == false)
                        {
                            graph[x, y].neighbours.Add(graph[x + 1, y]);
                            graph[x + 1, y].neighbours.Add(graph[x, y]);
                        }
                        if (table[x, y + 1] == false)
                        {
                            graph[x, y].neighbours.Add(graph[x, y + 1]);
                            graph[x, y + 1].neighbours.Add(graph[x, y]);
                        }
                    }
                }
            }

            return graph;

        }
        public class Node
        {
            public List<Node> neighbours = new List<Node>();
            public int x;
            public int y;
            public override string ToString()
            {
                return $"{x},{y}";
            }
            public string longToString()
            {
                string neighboursOut = "";
                for (int x = 0; x < neighbours.Count; x++)
                {
                    neighboursOut += "-" + neighbours[x].ToString();
                }
                return $"{this.ToString()},{neighboursOut}";
            }
            public Node(int x, int y)
            {
                this.x = x;
                this.y = y;

            }
            //public static explicit operator MainWindow.Vector(Node n) => new MainWindow.Vector(n.x, n.y);


        }
        public static List<Node> A_Star(Node start, Node end)
        {
            List<Node> open= new List<Node>();
            open.Add(start);
            Dictionary<Node, double> gScore = new Dictionary<Node, double>();
            gScore.Add(start, 0);

            Dictionary<Node, double> fScore = new Dictionary<Node, double>();
            fScore.Add(start, H_Cost(start, end));
            Node current;
            Dictionary<Node, Node> from = new Dictionary<Node, Node>();
            double tentaive;
            while(open.Count!=0)
            {
                current = open.OrderBy(n => GetFScore(n, fScore)).First();
                if (current.x == end.x && current.y == end.y)
                {
                    return Reconstruct(from, current);
                }
                open.Remove(current);
                foreach (var neigbour in current.neighbours)
                {
                //Trace.WriteLine("almas");
                    
                    tentaive = gScore[current] + 1;
                    if (gScore.ContainsKey(neigbour) == false || tentaive<gScore[neigbour])
                    {
                        from[neigbour] =current;
                        gScore[neigbour]=tentaive;
                        fScore[neigbour]=gScore[neigbour]+ H_Cost(neigbour, end);
                        if (open.Contains(neigbour)==false)
                        {
                            open.Add(neigbour);
                        }
                    }
                }
            }

            return new List<Node>();
        }
        public static List<Node> Reconstruct(Dictionary<Node,Node> from, Node current)
        {
            List<Node> total = new List<Node>();
            total.Add(current);
            while(from.ContainsKey(current))
            {
                current = from[current];
                total = total.Prepend(current).ToList();
            }
            return total;
        }
        public static double GetFScore (Node node, Dictionary<Node, double> fScore)
        {
            if (fScore.ContainsKey(node))
            {
                return fScore[node];
            }
            else
            {
                return double.PositiveInfinity;
            }
        }
        public static double H_Cost(Node current,Node end)
        {
            double h=0;
            h =Math.Sqrt(Math.Pow(current.x - end.x, 2) + Math.Pow(current.y-end.y,2));
            return h;
        }
    }
}
