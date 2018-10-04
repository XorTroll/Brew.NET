using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Brew.HacPack
{
    public class NSP
    {
        public List<NCA> Contents { get; set; }
        public string TitleID { get; set; }
        public string KeyFile { get; set; }

        public NSP(string KeyFile)
        {
            this.KeyFile = KeyFile;
            Contents = new List<NCA>();
        }

        public void generate(string Out)
        {
            if(Contents.Count <= 0) return;
            string outdir = Utils.TemporaryDirectory + "\\gen\\" + TitleID;
            if(Directory.Exists(outdir)) Directory.Delete(outdir);
            Directory.CreateDirectory(outdir);
            List<NCAType> types = new List<NCAType>();
            byte keygen = Contents[0].KeyGeneration;
            foreach(NCA cnt in Contents)
            {
                if(cnt.TitleID != TitleID) continue;
                types.Add(cnt.Type);
                cnt.generate(outdir + "\\" + cnt.Type.ToString());
            }
            NCA cnmt = new NCA(KeyFile);
            cnmt.TitleID = TitleID;
            cnmt.KeyGeneration = keygen;
            cnmt.Type = NCAType.CNMT;
            foreach(NCAType t in types)
            {
                string[] ncas = Directory.GetFiles(outdir + "\\" + t.ToString());
                if(ncas.Length <= 0) continue;
                string curnca = ncas[0];
                string ncaname = Path.GetFileName(curnca);
                File.Copy(curnca, outdir + "\\" + ncaname, true);
                switch(t)
                {
                    case NCAType.Control:
                        cnmt.Options.ControlNCA = curnca;
                        break;
                    case NCAType.Data:
                        cnmt.Options.DataNCA = curnca;
                        break;
                    case NCAType.LegalInformation:
                        cnmt.Options.LegalInformationNCA = curnca;
                        break;
                    case NCAType.OfflineHTML:
                        cnmt.Options.OfflineHTMLNCA = curnca;
                        break;
                    case NCAType.Program:
                        cnmt.Options.ProgramNCA = curnca;
                        break;
                }
            }
            cnmt.TitleID = TitleID;
            cnmt.Options.TitleType = CNMTTitleType.Application;
            cnmt.generate(outdir);
            string hacpack = "--type nsp";
            hacpack += " -k \"" + KeyFile + "\"";
            hacpack += " --tempdir \"" + Utils.TemporaryDirectory + "\\bin\\temp\"";
            hacpack += " -o \"" + outdir + "\"";
            hacpack += " --keygeneration " + keygen.ToString();
            hacpack += " --titleid " + TitleID.ToString();
            hacpack += " --ncadir \"" + outdir + "\"";
            Console.WriteLine(hacpack);
            Utils.executeCommand(Utils.TemporaryDirectory + "\\bin\\hacpack.exe", hacpack, true);
            File.Copy(outdir + "\\" + TitleID + ".nsp", Out);
        }
    }
}
