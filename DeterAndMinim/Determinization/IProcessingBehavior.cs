using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Determinization
{
    public interface IProcessingBehavior
    {
        public void StartProcess(TextReader reader, TextWriter writer);
    }
}
