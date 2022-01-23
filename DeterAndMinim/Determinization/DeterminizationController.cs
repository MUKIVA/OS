using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinization
{
    using Matrix = Dictionary<string, Dictionary<char, HashSet<char>>>;
    public class DeterminizationController
    {
        private readonly TextReader _reader;
        private readonly TextWriter _writer;
        private readonly IProcessingBehavior _processingBehavior;

        public DeterminizationController(TextReader reader, TextWriter writer)
        {
            _reader = reader;
            _writer = writer;
            string grammarType = _reader.ReadLine() ?? string.Empty;

            if (grammarType.ToUpper() == "L")
                _processingBehavior = new LeftProcess(1, 0);
            if (grammarType.ToUpper() == "R")
                _processingBehavior = new RightProcess(0, 1);   
        }

        public void Start()
        {
            _processingBehavior.StartProcess(_reader, _writer);
        }
    }
}
