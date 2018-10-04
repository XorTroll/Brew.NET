using System;
using System.IO;
using System.Reflection;

namespace Brew.NSPack
{
    public static class Utils
    {
        public static string Cwd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string formattedSize(long Bytes)
        {
            string[] suf = { " bytes", " KB", " MB", " GB", " TB", " PB", " EB" };
            if (Bytes is 0) return "0" + suf[0];
            long bytes = Math.Abs(Bytes);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(Bytes) * num).ToString() + suf[place];
        }

        public static bool nullableBoolValue(bool? Nullable)
        {
            return (Nullable ?? false);
        }
    }
}
