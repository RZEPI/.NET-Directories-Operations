using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Sproboj ponownie podajac sciezke folderu");
                return;
            }
            string path = args[0];
            if(!Directory.Exists(path))
            {
                Console.WriteLine("Zla nazwa katalogu");
                return;
            }

            string[] directoryContents = Directory.GetFiles(path);
            Console.WriteLine("Pliki w katalogu {0}: ", path);

            foreach (var content in directoryContents)
                Console.WriteLine(content);
        }
    }
}
