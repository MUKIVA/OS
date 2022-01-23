using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewRealization
{
    public interface IParseCellBehavior
    {
        public bool Validate(string value);
        public void Print(TextWriter writer, Cell cell);
    }
}
