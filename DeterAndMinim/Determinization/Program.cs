using System;
using System.IO;
using System.Text;

namespace Determinization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter sw = new("out.txt"))
            using (StreamReader sr = new("in2.txt"))
            {
                DeterminizationController controller = new(sr, sw);
                controller.Start();
            }
        }
    }
}
