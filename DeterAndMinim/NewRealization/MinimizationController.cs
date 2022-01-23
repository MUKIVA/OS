using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRealization
{
    public enum TableType
    {
        MI,
        MO
    }

    public class MinimizationController
    {
        private readonly TextReader _reader;
        private readonly TextWriter _writer;
        private readonly IParseCellBehavior _header;
        private readonly IParseCellBehavior _data;
        public MinimizationController(TextReader reader, TextWriter writer)
        {
            _reader = reader;
            _writer = writer;
            string tableType = _reader.ReadLine() ?? string.Empty;
            if (tableType.ToUpper() == TableType.MI.ToString().ToUpper())
            {
                _header = new SingleCellBehaviour();
                _data = new CombinedCellBehaviour();
            }
            if (tableType.ToUpper() == TableType.MO.ToString().ToUpper())
            {
                _header = new CombinedCellBehaviour();
                _data = new SingleCellBehaviour();
            }
        }

        public void Start()
        {
            new UniversalProcess().StartProcess(_reader, _writer, _header, _data);
        }
    }
}
