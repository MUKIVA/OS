using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public class GraphArc
    {
        public Node Start { get; set; }
        public Node End { get; set; }
        public string Value { get; set; }

        public static int globalIdentiti = 0;
        public GraphArc(Node startName, Node endName, string value)
        {
            Start = startName;
            End = endName;
            Value = value;
        }

        //public GraphArc(string nodeName, string value)
        //{
        //    Start = $"{nodeName}{globalIdentiti++}";
        //    End = $"{nodeName}{globalIdentiti++}"; ;
        //    Value = value;
        //}
    }
}
