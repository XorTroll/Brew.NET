using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Brew.HacPack
{
    public static class Utils
    {
        public static string Cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string TemporaryDirectory = Cwd + "\\.Build";

        public static void initTemporaryDirectory()
        {
            if(Directory.Exists(TemporaryDirectory)) Directory.Delete(TemporaryDirectory, true);
            Directory.CreateDirectory(TemporaryDirectory).Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            Directory.CreateDirectory(TemporaryDirectory + "\\bin");
            File.WriteAllBytes(TemporaryDirectory + "\\bin\\hacpack.exe", Properties.Resources.hacpack);
            Directory.CreateDirectory(TemporaryDirectory + "\\bin\\temp");
            Directory.CreateDirectory(TemporaryDirectory + "\\gen");
        }

        public static void exitTemporaryDirectory()
        {
            if(Directory.Exists(TemporaryDirectory)) Directory.Delete(TemporaryDirectory, true);
        }

        public static void executeCommand(string File, string Args, bool Wait)
        {
            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = File,
                Arguments = Args,
                CreateNoWindow = true,
                UseShellExecute = false,
            };
            Process p = Process.Start(info);
            if(Wait) p.WaitForExit();
        }

        public static string formattedSize(long Bytes)
        {
            string[] suf = { " bytes", " KB", " MB", " GB", " TB", " PB", " EB" };
            if(Bytes is 0) return "0" + suf[0];
            long bytes = Math.Abs(Bytes);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(Bytes) * num).ToString() + suf[place];
        }
    }
}
