using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public class RGrammerBuilder
    {
        private HashSet<(string name, Node node)> _nodes = new();
        private Node _startNode;
        private Node _endNode;
        private List<GraphArc> _graph;
        public RGrammerBuilder(List<GraphArc> graph)
        {
            _graph = graph;
            HashSet<(string name, Node node)> VWithInArc = new();
            HashSet<(string name, Node node)> VWithOutArc = new();

            foreach (var arc in graph)
            {
                VWithInArc.Add((arc.Start.Name, arc.Start));
                VWithOutArc.Add((arc.End.Name, arc.End));
            }
            _nodes = VWithInArc.Union(VWithOutArc).ToHashSet();

            _startNode = VWithInArc.Except(VWithOutArc).ToList().First().node;
            try
            {
                _endNode = VWithOutArc.Except(VWithInArc).ToList().First().node;
            }
            catch
            { 
            }

            if (_startNode.Name == "S") return;

            _nodes.Select(x => x).Where(node => node.name == "S")
                .ToList()
                .ForEach(y => (y.name, y.node.Name) = (_startNode.Name, _startNode.Name) );
            _startNode.Name = "S";

        }

        public string GenerateRGrammer()
        {
            string grammer = "R\n";
            foreach (var node in _nodes)
            {
                //Берем информацию о ноде
                var info = _graph.Select(x => x).Where(y => y.Start == node.node).ToList();
                if (info.Count == 0) continue;
                grammer += $"{node.node.Name}->";
                    info.ForEach(to => grammer += (to.End == _endNode) ? $"{to.Value}|" : $"{to.Value}{to.End.Name}|");

                grammer += "\n";
            }
            return grammer;
        }
    }
}
