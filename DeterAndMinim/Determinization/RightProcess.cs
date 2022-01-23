using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinization
{
    using Matrix = Dictionary<char, Dictionary<char, HashSet<char>>>;
    public class RightProcess : IProcessingBehavior
    {
        private readonly int _nonterminalIndex;
        private readonly int _terminalIndex;

        public RightProcess(int termId, int nontermId)
        {
            _nonterminalIndex = nontermId;
            _terminalIndex = termId;
        }

        public void StartProcess(TextReader reader, TextWriter writer)
        {
            //Построить граф
            HashSet<char> header = new();
            Matrix matrix = new() 
            {
                { 'H', new() }
            };
            while (reader.Peek() != -1)
            {
                var row = reader.ReadLine();
                var data = row.Split("->");
                char nonterminal = data[0][0];
                matrix.Add(nonterminal, new());
                string links = data[1];
                links.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(link =>
                {
                    if (link.Length != 2) link = $"{link}H";

                    header.Add(link[_terminalIndex]);

                    if (matrix[nonterminal].ContainsKey(link[_nonterminalIndex]))
                        matrix[nonterminal][link[_nonterminalIndex]].Add(link[_terminalIndex]);
                    else
                        matrix[nonterminal].Add(link[_nonterminalIndex], new() { link[_terminalIndex] });
                });
            }
            //Детерминирование
            DeetermineTable table = new DeetermineTable(matrix, 'S', header);
            table.StartProcess();
            table.PrintDetTable(writer);
        }
    }
}
