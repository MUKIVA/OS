using System;
using System.IO;
using System.Text;

namespace Determinization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter sw = new(args[1]))
            using (StreamReader sr = new(args[0]))
            {
                DeterminizationController controller = new(sr, sw);
                controller.Start();
            }
        }
    }
}
