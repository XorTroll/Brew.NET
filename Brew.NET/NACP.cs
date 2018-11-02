using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Brew
{

    public class NACPEntry
    {
        public string Name { get; set; }
        public string Author { get; set; }
    }

    public class NACP
    {
        public string ApplicationId { get; set; }
        public string Version { get; set; }
        public string ProductCode { get; set; }
        public List<NACPEntry> Entries { get; set; }
        public byte StartupUserAccount { get; set; }
        public bool Screenshot { get; set; }
        public bool VideoCapture { get; set; }
        public ulong UserAccountSaveDataSize { get; set; }
        public ulong UserAccountSaveDataJournalSize { get; set; }

        public NACP()
        {
            Entries = new List<NACPEntry>();
            StartupUserAccount = 0;
            Screenshot = true;
            VideoCapture = true;
            UserAccountSaveDataSize = 0x6000000;
            UserAccountSaveDataJournalSize = 0xa00000;
        }

        public NACP(string Path)
        {
            Entries = new List<NACPEntry>();
            BinaryReader r = new BinaryReader(new FileStream(Path, FileMode.OpenOrCreate));
            r.BaseStream.Seek(0x3025, SeekOrigin.Begin);
            StartupUserAccount = r.ReadByte();
            r.BaseStream.Seek(0x3034, SeekOrigin.Begin);
            Screenshot = Convert.ToBoolean(r.ReadByte());
            r.BaseStream.Seek(0x3035, SeekOrigin.Begin);
            VideoCapture = Convert.ToBoolean(r.ReadByte());
            r.BaseStream.Seek(0x3038, SeekOrigin.Begin);
            ulong appid = r.ReadUInt64();
            r.BaseStream.Seek(0x3080, SeekOrigin.Begin);
            UserAccountSaveDataSize = r.ReadUInt64();
            r.BaseStream.Seek(0x3088, SeekOrigin.Begin);
            UserAccountSaveDataJournalSize = r.ReadUInt64();
            ApplicationId = string.Format("{0:X16}", appid);
            System.Windows.Forms.MessageBox.Show(ApplicationId);
            for(int i = 0; i < 15; i++)
            {
                long titleoff = (i * 0x300);
                r.BaseStream.Seek(titleoff, SeekOrigin.Begin);
                string name = Encoding.ASCII.GetString(r.ReadBytes(0x200));
                r.BaseStream.Seek(titleoff + 0x200, SeekOrigin.Begin);
                string author = Encoding.ASCII.GetString(r.ReadBytes(0x100));
                Entries.Add(new NACPEntry()
                {
                    Name = name,
                    Author = author,
                });
                System.Windows.Forms.MessageBox.Show(name + "\n" + author);
            }
            r.Close();
        }

        public void generate(string Out)
        {
            List<byte> all = new List<byte>();
            List<byte> titleid = Enumerable.Range(0, ApplicationId.Length).Where(x => x % 2 is 0).Select(x => Convert.ToByte(ApplicationId.Substring(x, 2), 16)).ToList();
            titleid.Reverse();
            StringBuilder sb = new StringBuilder(ApplicationId);
            sb[13] = '8';
            string sdlctid = sb.ToString();
            Console.WriteLine(sdlctid);
            List<byte> dlctitleid = Enumerable.Range(0, sdlctid.Length).Where(x => x % 2 is 0).Select(x => Convert.ToByte(sdlctid.Substring(x, 2), 16)).ToList();
            dlctitleid.Reverse();
            List<byte> version = new List<byte>(Encoding.ASCII.GetBytes(Version));
            int vsize = version.Count;
            List<byte> pcode = new List<byte>(Encoding.ASCII.GetBytes(ProductCode));
            int psize = pcode.Count;
            foreach(NACPEntry ent in Entries)
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
            all[0x3025] = StartupUserAccount;
            all[0x3034] = (byte)(Screenshot ? 1 : 0);
            all[0x3035] = (byte)(VideoCapture ? 1 : 0);
            byte[] sdata = BitConverter.GetBytes(UserAccountSaveDataSize);
            byte[] sjournal = BitConverter.GetBytes(UserAccountSaveDataJournalSize);
            int baseoff = 0x3080;
            foreach(byte b in sdata)
            {
                all[baseoff] = b;
                baseoff++;
            }
            baseoff = 0x3088;
            foreach(byte b in sjournal)
            {
                all[baseoff] = b;
                baseoff++;
            }
            if(Directory.Exists(Out)) File.WriteAllBytes(Out + "\\control.nacp", all.ToArray());
            else File.WriteAllBytes(Out, all.ToArray());
        }
    }
}
