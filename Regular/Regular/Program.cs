using System;
using System.Diagnostics;
using System.IO;

namespace Regular
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string regular = ";|:|=|(|)|+|-|*|>|<|/|,|.";
            RegularGrapfBuilder graphBuilder = new(regular);
            RGrammerBuilder grammerBuilder = new(graphBuilder.GetGraph());
            var grammer = grammerBuilder.GenerateRGrammer();

            using (StreamWriter sw = new("in.txt"))
            {
                sw.Write(grammer);
            }
            try
            {
                using (Process d = new())
                {
                    d.StartInfo.UseShellExecute = false;
                    d.StartInfo.FileName = "Determinization.exe";
                    d.StartInfo.CreateNoWindow = true;
                    d.StartInfo.ArgumentList.Add("in.txt");
                    d.StartInfo.ArgumentList.Add("d.txt");
                    d.Start();
                    d.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
