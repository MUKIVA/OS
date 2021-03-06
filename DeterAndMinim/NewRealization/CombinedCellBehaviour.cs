using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NewRealization
{
    public class CombinedCellBehaviour : IParseCellBehavior
    {
        private Regex _rx = new Regex($"^.*/.*$");

        public void Print(TextWriter writer, Cell cell)
        {
            writer.Write($"{cell.State}/{cell.Enter}\t");
        }

        public bool Validate(string value) => _rx.IsMatch(value);
    }
}
