using System;
using System.IO;

namespace NewRealization
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (StreamReader sr = new("in.txt"))
            {
                MinimizationController controller = new(sr, Console.Out);
                controller.Start();
            }
        }
    }
}
