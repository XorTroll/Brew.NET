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
            if(Directory.Exists(outdir)) Directory.Delete(outdir, true);
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
            cnmt.TitleID = TitleID;
            cnmt.Options.TitleType = CNMTTitleType.Application;
            foreach(NCAType t in types)
            {
                string[] ncas = Directory.GetFiles(outdir + "\\" + t.ToString());
                if(ncas.Length <= 0) continue;
                string curnca = ncas[0];
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
            cnmt.generate(outdir);
            foreach(NCAType t in types)
            {
                string[] ncas = Directory.GetFiles(outdir + "\\" + t.ToString());
                if(ncas.Length <= 0) continue;
                string curnca = ncas[0];
                File.Move(curnca, outdir + "\\" + Path.GetFileName(curnca));
            }
            string hacpack = "--type nsp";
            hacpack += " -k \"\"" + KeyFile + "\"\"";
            hacpack += " --tempdir \"\"" + Utils.TemporaryDirectory + "\\bin\\temp\"\"";
            hacpack += " -o \"\"" + outdir + "\"\"";
            hacpack += " --keygeneration " + keygen.ToString();
            hacpack += " --titleid " + TitleID.ToString();
            hacpack += " --ncadir \"\"" + outdir + "\"\"";
            Console.WriteLine(hacpack);
            try
            {
                Utils.executeCommand(Utils.TemporaryDirectory + "\\bin\\hacpack.exe", hacpack, true);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error processing hacPack command", "Brew.NET - NSP build error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Directory.Delete(outdir, true);
                return;
            }
            File.Move(outdir + "\\" + TitleID + ".nsp", Out);
            // Directory.Delete(outdir, true);
        }
    }
}
