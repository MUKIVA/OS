using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinization
{
    //                       //from             to   enter
    using Matrix = Dictionary<char, Dictionary<char, HashSet<char>>>;
    //                              row              enter  NextRowValue
    using Data = Dictionary<HashSet<char>, Dictionary<char, HashSet<char>>>;
    public class DeetermineTable
    {
        private Matrix _matrix;
        private Data _data = new();
        private Queue<HashSet<char>> _detQ = new();
        private HashSet<char> _header;

        public DeetermineTable(Matrix matrix, char startNonterm, HashSet<char> header)
        {
            _matrix = matrix;
            _detQ.Enqueue(new() { startNonterm });
            _header = header;
        }

        private bool SetEqual(HashSet<char> lSet, HashSet<char> rSet)
        {
            if (lSet.Count != rSet.Count) return false;
            return lSet.Union(rSet).Count() == lSet.Count;
        }

        private bool SetNotProcessed(HashSet<char> set)
        {
            return _data.Select(x => x.Key).FirstOrDefault(y => SetEqual(y, set)) == null;
        }

        private bool SetNotInProcess(HashSet<char> set)
        {
            return _detQ.FirstOrDefault(y => SetEqual(y, set)) == null;
        }

        public void Determ()
        {
            var set = _detQ.Dequeue();
            _data.Add(set, new());
            // Заполняем значениями
            _header.ToList().ForEach(term =>
                set.ToList().ForEach(nonterm => 
                {
                    var _ = _matrix[nonterm]
                    .Where(way => way.Value.Contains(term))
                    .Select(pair => pair.Key).ToHashSet();

                    if (_data[set].ContainsKey(term))
                        _data[set][term] = _data[set][term].Union(_).ToHashSet();
                    else
                        _data[set].Add(term, _);
                }));
            // Добавляем в очередь
            _data[set].ToList().ForEach(pair => 
            {
                if (pair.Value.Count != 0 && SetNotProcessed(pair.Value) && SetNotInProcess(pair.Value))
                    _detQ.Enqueue(pair.Value);
            }); 
        }

        public void StartProcess()
        {
            while (_detQ.Count != 0)
                Determ();
        }

        public void PrintDetTable(TextWriter writer)
        {
            // header
            _header.ToList().ForEach(term => writer.Write($"\t{term}"));
            writer.WriteLine();
            // Data
            _data.ToList().ForEach(row =>
            {
                string key = "";
                row.Key.ToList().ForEach(el => key += el);
                writer.Write($"{key}\t");

                var values = row.Value;
                values.ToList().ForEach(columnAndValue =>
                {
                    columnAndValue.Value.ToList().ForEach(value => writer.Write(value));
                    writer.Write('\t');
                });
                writer.WriteLine();
            });
        }
    }
}
