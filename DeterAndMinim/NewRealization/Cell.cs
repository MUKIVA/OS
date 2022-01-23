using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRealization
{
    public class Cell
    {
        private string _value = "";
        private int _index;
        private string _enter = "";
        private string _state = "";
        private IParseCellBehavior _parser;
        public Cell(int index, IParseCellBehavior validator) => (_index, _parser) = (index, validator);
        public Cell(int index, string value, IParseCellBehavior validator) => (_index, _parser, Value) = (index, validator, value);
        public string Value
        {
            get => _value;
            set
            {
                if (_parser.Validate(value))
                    _value = value;
                else
                    throw new Exception("Invalid cell value");
                SetEnterAndState();
            }
        }
        public void Print(TextWriter writer) => _parser.Print(writer, this);
        private void SetEnterAndState()
        {
            var _ = Value.Split('/');
            _state = _[0];
            _enter = (_.Length > 1) ? _[1] : null;
        }
        public int Index
        {
            get => _index;
            set => _index = value;
        }
        public string Enter
        {
            get => _enter;
        }
        public string State
        {
            get => _state;
        }
    }
}
