using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Brew.HacPack
{
    public class NACPEntry
    {
        public string Name { get; set; }
        public string Author { get; set; }
    }

    public class NACP
    {
        public string TitleID { get; set; }
        public string Version { get; set; }
        public string ProductCode { get; set; }
        public List<NACPEntry> Entries { get; set; }

        public NACP()
        {
            Entries = new List<NACPEntry>();
        }

        public void generate(string OutDir)
        {
            if(!Directory.Exists(OutDir)) return;
            List<byte> all = new List<byte>();
            List<byte> titleid = Enumerable.Range(0, TitleID.Length).Where(x => x % 2 is 0).Select(x => Convert.ToByte(TitleID.Substring(x, 2), 16)).ToList();
            titleid.Reverse();
            StringBuilder sb = new StringBuilder(TitleID);
            sb[13] = '8';
            string sdlctid = sb.ToString();
            Console.WriteLine(sdlctid);
            List<byte> dlctitleid = Enumerable.Range(0, sdlctid.Length).Where(x => x % 2 is 0).Select(x => Convert.ToByte(sdlctid.Substring(x, 2), 16)).ToList();
            dlctitleid.Reverse();
            List<byte> version = new List<byte>(Encoding.ASCII.GetBytes(Version));
            int vsize = version.Count;
            List<byte> pcode = new List<byte>(Encoding.ASCII.GetBytes(ProductCode));
            int psize = pcode.Count;
            foreach (NACPEntry ent in Entries)
            {
                List<byte> name = new List<byte>(Encoding.ASCII.GetBytes(ent.Name));
                int size = name.Count;
                while(size < 0x200)
                {
                    name.Add(0);
                    size = name.Count;
                }
                List<byte> author = new List<byte>(Encoding.ASCII.GetBytes(ent.Author));
                size = author.Count;
                while(size < 0x100)
                {
                    author.Add(0);
                    size = author.Count;
                }
                all.AddRange(name);
                all.AddRange(author);
            }
            for(int i = 0; i < 0x24; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            all.AddRange(titleid);
            while(vsize < 0x10)
            {
                version.Add(0);
                vsize = version.Count;
            }
            for(int i = 0; i < 0x20; i++) all.Add(0xff);
            all.AddRange(version);
            all.AddRange(dlctitleid);
            all.AddRange(titleid);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x1c; i++) all.Add(0);
            while(psize < 0x8)
            {
                pcode.Add(0);
                psize = pcode.Count;
            }
            all.AddRange(pcode);
            all.AddRange(titleid);
            all.AddRange(titleid);
            all.AddRange(titleid);
            all.AddRange(titleid);
            all.AddRange(titleid);
            all.AddRange(titleid);
            all.AddRange(titleid);
            all.AddRange(titleid);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            for(int i = 0; i < 0x4; i++) all.Add(0);
            all.AddRange(titleid);
            for(int i = 0; i < 0x40; i++) all.Add(0);
            for(int i = 0; i < 0xec0; i++) all.Add(0);
            File.WriteAllBytes(OutDir + "\\control.nacp", all.ToArray());
        }
    }
}
