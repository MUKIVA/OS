using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinization
{
    using Matrix = Dictionary<char, Dictionary<char, HashSet<char>>>;
    public class LeftProcess : IProcessingBehavior
    {

        private readonly int _nonterminalIndex;
        private readonly int _terminalIndex;
        public LeftProcess(int termId, int nontermId)
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
                if (!matrix.ContainsKey(nonterminal))
                    matrix.Add(nonterminal, new());
                string links = data[1];
                links.Split('|').ToList().ForEach(link =>
                {
                    if (link.Length != 2) link = $"H{link}";

                    header.Add(link[_terminalIndex]);

                    if (!matrix.ContainsKey(link[_nonterminalIndex]))
                        matrix.Add(link[_nonterminalIndex], new());

                    if (matrix[link[_nonterminalIndex]].ContainsKey(nonterminal))
                        matrix[link[_nonterminalIndex]][nonterminal].Add(link[_terminalIndex]);
                    else
                        matrix[link[_nonterminalIndex]].Add(nonterminal, new() { link[_terminalIndex] });
                });
            }
            DeetermineTable table = new DeetermineTable(matrix, 'H', header);
            table.StartProcess();
            table.PrintDetTable(writer);
        }
    }
}
