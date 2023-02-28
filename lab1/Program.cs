using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Sproboj ponownie podajac sciezke folderu");
                return;
            }
            string path = args[0];
            DisplayDirectoryContents(path, 0);
            Console.WriteLine("Najstarszy plik to: {0}", (new DirectoryInfo(path)).GetOldestFile());
            CreateCollection(path);
        }

        static void DisplayDirectoryContents(string directoryPath, int indentLevel)
        {
            string indent = new string(' ', indentLevel * 2);
            string[] dirComps = directoryPath.Split('\\');
            string dirName = dirComps[dirComps.Length - 1]; 
            Console.WriteLine("{0}{1} ({2})",indent, dirName, CountComponents(directoryPath) );

            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                FileInfo file = new FileInfo(filePath);
                Console.WriteLine(indent + "  " + file.Name + " " + file.Length + " bajtow " + file.GetDosAttributes());
            }

            foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath))
            {
                DisplayDirectoryContents(subdirectoryPath, indentLevel + 1);
            }
        }

        public static DateTime GetOldestFile(this DirectoryInfo directory)// this jest tu użyte żeby rozszerzyć System.IO.DirectoryInfo
        {
            DateTime oldest = DateTime.MaxValue;

            foreach(var file in directory.GetFiles())
            {
                if(file.CreationTime < oldest)
                    oldest = file.CreationTime;

                if(file.LastWriteTime < oldest)
                    oldest = file.LastWriteTime;
            }
            foreach(var subDir in directory.GetDirectories())
            {
                DateTime subDirOldest = subDir.GetOldestFile();
                if (subDirOldest < oldest)
                    oldest = subDirOldest;
            }
            return oldest;
        }
        public static int CountComponents(string path)
        {
            int count = 0;
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach(var file in directory.GetFiles())
                count++;
            foreach (var file in directory.GetDirectories())
                count++;

            return count;
        }
        public static string GetDosAttributes(this FileSystemInfo fileInfo)
        {
            string result = "";
            if ((fileInfo.Attributes & FileAttributes.ReadOnly) != 0)
                result += "r";
            else
                result += "-";

            if ((fileInfo.Attributes & FileAttributes.Archive) != 0)
                result += "a";
            else
                result += "-";

            if ((fileInfo.Attributes & FileAttributes.Hidden) != 0)
                result += "h";
            else
                result += "-";
            if ((fileInfo.Attributes & FileAttributes.System) != 0)
                result += "s";
            else
                result += "-";

            return result;
        }
        public static void CreateCollection(string path)
        {
            IComparer<string> comparer = new StringComparer();
            SortedList<string, long> fileList = new SortedList<string, long>(comparer);
            DirectoryInfo directory = new DirectoryInfo(path);
            foreach (FileInfo file in directory.GetFiles())
            {
                fileList.Add(file.Name, file.Length);
            }
            foreach(DirectoryInfo dir in directory.GetDirectories())
            {
                fileList.Add(dir.Name, dir.GetDirectories().Length + dir.GetFiles().Length);
            }
            FileStream fileStream = new FileStream("Serialized.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, fileList);
            fileStream.Close();
            SortedList<string, long> deserializedList = Deserialization();
            foreach (KeyValuePair<string, long> keyVal in deserializedList)
            {
                Console.WriteLine($"{keyVal.Key} -> {keyVal.Value}");
            }
        }
        public static SortedList<string, long> Deserialization()
        {
            IComparer<string> comparer = new StringComparer();
            SortedList<string, long> fileList = new SortedList<string, long>(comparer);
            FileStream fileStream = new FileStream("Serialized.dat", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            fileList = (SortedList<string, long>)formatter.Deserialize(fileStream);
            return fileList;    
        }
    }
    [Serializable]
    public class StringComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a.Length > b.Length)
                return 1;
            else if (a.Length < b.Length)
                return -1;
            else
                return a.CompareTo(b);
        }
    }
}
