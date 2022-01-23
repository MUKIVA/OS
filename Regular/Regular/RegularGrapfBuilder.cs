using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular
{
    public struct NodeLink
    {
        public string Name  { get; set; }
        public string Value { get; set; }
        public NodeLink(string name = null, string value = null)
        {
            Name = name;
            Value = value;
        }
    }
    public class AdjacencyTable
    {
        private Dictionary<string, Dictionary<string, NodeLink?>> _table;

        public AdjacencyTable(List<GraphArc> graph)
        {
            _table = new();

            foreach (var arc in graph)
                if (!_table.ContainsKey(arc.Start.Name))
                    _table.Add(arc.Start.Name, new());

            foreach (var arc in graph)
                if (!_table.ContainsKey(arc.End.Name))
                    _table.Add(arc.End.Name, new());
            _table.ToList().ForEach(x => { _table.ToList().ForEach(y => { x.Value.Add(y.Key, null); }); });

            foreach (var arc in graph)
                _table[arc.Start.Name][arc.End.Name] = new NodeLink(arc.End.Name, arc.Value);
        }

        public Dictionary<string, Dictionary<string, NodeLink?>> Table
        {
            get => _table;
        }
    }
    public class RegularGrapfBuilder
    {
        public const char LOCKING_ACTION_CHAR = '*';
        public const char OR_ACTION_CHAR = '|';
        public const char OPEN_BRACKET_CHAR = '(';
        public const char CLOSE_BRACKET_CHAR = ')';
        public const string NODE_NAME = "S";

        private List<GraphArc> _graph;
        private Node _startNode;
        private Node _endNode;
        public RegularGrapfBuilder(string regular)
        {
            _graph = new();
            _startNode = CreateNode();
            _endNode = CreateNode();
            _graph.Add(new GraphArc(_startNode, _endNode, regular));

            Build();
        }
        private Node CreateNode() => new Node($"{NODE_NAME}{GraphArc.globalIdentiti++}");
        private void Build()
        {
            do
            { 
                while (BracketHandler()) continue;     // Убиваем последовательные скобки ")("
                while (OrDelimiterHandler()) continue; // Избавляемся от |
            } while (OpenBrackets()) ;                 // Делаем пока есть "()"
            while (ConcatenationHandler()) continue;   // Избавляемся от конкатенаций
            while (LockHandler()) continue;            // Избавляемся от *
            CleanEmpty();
            GC.Collect();
        }
        private void CleanEmpty()
        {
            var wayTable = new AdjacencyTable(_graph);
            var header = _graph.Select(arc => arc.Value).Where(value => value != null).ToHashSet();
            Dictionary<HashSet<Node>, Dictionary<string, HashSet<Node>>> eraseTable = new();
            HashSet<(string name, Node node)> nodes = new();
            //Получаем ссылки на все ноды
            foreach (var arc in _graph)
            {
                nodes.Add((arc.Start.Name, arc.Start));
                nodes.Add((arc.End.Name, arc.End));
            }
            Queue<string> q = new();
            //Заполнение ключевых множеств
            foreach (var node in nodes)
            {
                HashSet<Node> _ = new();
                q.Enqueue(node.name);
                while (q.Count != 0)
                {
                    var name = q.Dequeue();
                    _.Add(nodes.FirstOrDefault(n => n.name == name).node);
                    var emptyWay = wayTable.Table[name]
                        .Select(x => x.Value)
                        .Where(y => y.HasValue && y?.Value == null).ToList();
                    emptyWay.ForEach(way => q.Enqueue(way?.Name));
                }
                eraseTable.Add(_, new());
            }
            //Заполнение столбцов
            foreach (var enter in header)
            {
                foreach (var set in eraseTable)
                {
                    HashSet<Node> _ = new();
                    set.Value.Add(enter, _);
                    set.Key.ToList().ForEach(node =>
                    {
                        wayTable.Table[node.Name]
                        .Select(x => x.Value)
                        .Where(y => y.HasValue && y?.Value == enter).ToList()
                        .ForEach(findedNode => { _.Add(nodes.FirstOrDefault(n => n.name == findedNode.Value.Name ).node); });
                    });
                }
            }
            //Перестройка графа
            _graph.Clear(); char nodeName = 'A';
            foreach (var nodeInfo in eraseTable)
            {
                if (nodeName == 'H') ++nodeName;

                var from = nodeInfo.Key.First();
                from.Name = nodeName++.ToString();
                header.ToList().ForEach(enter => 
                {
                    nodeInfo.Value[enter].ToList().ForEach(to => 
                    {
                        _graph.Add(new(from, to, enter));
                    });
                });
            }
        }
        private bool LockHandler()
        {
            var arc = _graph.FirstOrDefault(x => x.Value != null && x.Value.Contains('*'));
            if (arc == null) return false;

            // Обновляем граф

            string lockValue = arc.Value[0].ToString();
            var _ = CreateNode();
            GraphArc @toEnd = new(_, arc.End, null);
            GraphArc @lock = new(_, _, lockValue);
            arc.End = _;
            arc.Value = null;
            _graph.Add(toEnd); _graph.Add(@lock);
            
            return true;
        }
        private bool OpenBrackets()
        {
            bool _ = false;
            foreach (GraphArc arc in _graph)
            {
                if (arc.Value[0] == OPEN_BRACKET_CHAR && arc.Value[^1] == CLOSE_BRACKET_CHAR)
                { 
                    arc.Value = arc.Value.Substring(1, arc.Value.Length - 2);
                    _ = true;
                }
            }
            return _;
        }
        private List<string> GetSubsByIndexes(string rx, Stack<int> ids)
        {
            List<string> _ = new();
            while (true)
            {
                int? endIndex = (ids.Count != 0) ? ids.Pop() : null;
                int? startIndex = (ids.Count != 0) ? ids.Pop() : null;
                if (startIndex == null) throw new Exception("Start index can't be null");
                _.Add(rx.Substring((int)startIndex, (int)endIndex - (int)startIndex));
                if (ids.Count == 0)
                    break;
                else
                    ids.Push((int)startIndex);
            }
            return _;
        }
        private Stack<int> GetSiresBracketIndex(string rx)
        {
            int bracketIndex = 0;
            Stack<int> subindexS = new();
            for (int i = 0; i < rx.Length; ++i)
            {
                if (bracketIndex == 0 && rx[i] == OPEN_BRACKET_CHAR) subindexS.Push(i);
                if (rx[i] == OPEN_BRACKET_CHAR) ++bracketIndex;
                if (rx[i] == CLOSE_BRACKET_CHAR) --bracketIndex;
            }
            subindexS.Push(rx.Length);
            return subindexS;
        }
        private Stack<int> GetSiresConcatIndex(string rx)
        {
            Stack<int> _ = new();
            for (int i = 0; i < rx.Length; ++i)
                if (rx[i] != LOCKING_ACTION_CHAR) _.Push(i);
            _.Push(rx.Length);
            return _;
        }
        private Stack<int> GetOrDelimiterIndex(string rx)
        {
            Stack<int> _ = new();
            _.Push(0);
            int deepIndex = 0;
            for (int i = 0; i < rx.Length; ++i)
            {
                if (rx[i] == OPEN_BRACKET_CHAR) ++deepIndex;
                if (rx[i] == CLOSE_BRACKET_CHAR) --deepIndex;
                if (deepIndex != 0) continue;
                if (rx[i] == OR_ACTION_CHAR) _.Push(i);
            }
            _.Push(rx.Length);
            return _;
        }
        private bool OrDelimiterHandler()
        {
            var arc = _graph.FirstOrDefault(x => GetOrDelimiterIndex(x.Value).Count > 2);
            if (arc == null) return false;

            // Получаем подстроки
            List<string> subs = GetSubsByIndexes(arc.Value, GetOrDelimiterIndex(arc.Value));
            subs = subs.Select(sub => (sub[0] == '|') ? sub.Substring(1) : sub).ToList();
            // Обновляем граф
            foreach (string sub in subs.SkipLast(1))
            {
                GraphArc @new = new(arc.Start, arc.End, sub);
                _graph.Add(@new);
            }
            arc.Value = subs[^1];
            return true;
        }
        private bool ConcatenationHandler()
        {
            var arc = _graph.FirstOrDefault(x => GetSiresConcatIndex(x.Value).Count > 2);
            if (arc == null) return false;
            
            // Получаем подстроки
            var subs = GetSubsByIndexes(arc.Value, GetSiresConcatIndex(arc.Value));
            // Обновляем граф
            foreach (var sub in subs.SkipLast(1))
            {
                var _ = CreateNode();
                GraphArc @new = new(_, arc.End, sub);
                arc.End = _;
                _graph.Add(@new);
            }
            arc.Value = subs[^1];
            return true;
        }
        private bool BracketHandler()
        {
            GraphArc arc = _graph.FirstOrDefault(x => GetSiresBracketIndex(x.Value).Count > 2);
            if (arc == null) return false;
            // Получаем индексы последовательных скобок
            var subindexS = GetSiresBracketIndex(arc.Value);
            var rx = arc.Value;
            // Записываем значения в скобках в массив
            List<string> subs = GetSubsByIndexes(rx, subindexS);
            // Обновляем дуги графа
            foreach (var sub in subs.SkipLast(1).ToList())
            {
                var _ = CreateNode();
                GraphArc @new = new(_, arc.End, sub.Substring(1, sub.Length - 2));
                arc.End = _;
                _graph.Add(@new);
            }
            arc.Value = subs[^1].Substring(1, subs[^1].Length - 2);
            return true;
        }
        public List<GraphArc> GetGraph()
        {
            return _graph;
        }
    }
}
