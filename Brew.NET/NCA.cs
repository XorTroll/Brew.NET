using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.IO.Compression;

namespace Brew.HacPack
{
    public enum NCAType
    {
        Program,
        Control,
        LegalInformation,
        OfflineHTML,
        Data,
        PublicData,
        CNMT,
    }

    public enum CNMTTitleType
    {
        Application,
        AddOn,
    }

    public class CNMTOptions
    {
        public CNMTTitleType TitleType { get; set; }
        public string ProgramNCA { get; set; }
        public string ControlNCA { get; set; }
        public string LegalInformationNCA { get; set; }
        public string OfflineHTMLNCA { get; set; }
        public string DataNCA { get; set; }
    }

    public class NCA
    {
        public NCAType Type { get; set; }
        public string TitleID { get; set; }
        public byte KeyGeneration { get; set; }
        public string ExeFS { get; set; }
        public string RomFS { get; set; }
        public string Logo { get; set; }
        public string KeyFile { get; set; }
        public CNMTOptions Options { get; set; }

        public NCA(string KeyFile)
        {
            this.KeyFile = KeyFile;
            Options = new CNMTOptions();
        }

        public void generate(string OutDir)
        {
            string hacpack = "--type nca";
            hacpack += " -k \"\"" + KeyFile + "\"\"";
            hacpack += " --tempdir \"\"" + Utils.TemporaryDirectory + "\\bin\\temp\"\"";
            hacpack += " -o \"\"" + OutDir + "\"\"";
            hacpack += " --keygeneration " + KeyGeneration.ToString();
            hacpack += " --titleid " + TitleID.ToString();
            switch(Type)
            {
                case NCAType.CNMT:
                    hacpack += " --ncatype meta";
                    switch(Options.TitleType)
                    {
                        case CNMTTitleType.AddOn:
                            hacpack += " --titletype addon";
                            break;
                        case CNMTTitleType.Application:
                            hacpack += " --titletype application";
                            break;
                    }
                    hacpack += " --titleversion 0";
                    if(!string.IsNullOrEmpty(Options.ProgramNCA)) hacpack += " --programnca \"\"" + Options.ProgramNCA + "\"\"";
                    if(!string.IsNullOrEmpty(Options.ControlNCA)) hacpack += " --controlnca \"\"" + Options.ControlNCA + "\"\"";
                    if(!string.IsNullOrEmpty(Options.LegalInformationNCA)) hacpack += " --legalnca \"\"" + Options.LegalInformationNCA + "\"\"";
                    if(!string.IsNullOrEmpty(Options.OfflineHTMLNCA)) hacpack += " --htmldocnca \"\"" + Options.OfflineHTMLNCA + "\"\"";
                    if(!string.IsNullOrEmpty(Options.DataNCA)) hacpack += " --datanca \"\"" + Options.DataNCA + "\"\"";
                    break;
                case NCAType.Control:
                    hacpack += " --ncatype control";
                    hacpack += " --romfsdir \"\"" + RomFS + "\"\"";
                    break;
                case NCAType.LegalInformation:
                case NCAType.OfflineHTML:
                    hacpack += " --ncatype manual";
                    hacpack += " --romfsdir \"\"" + RomFS + "\"\"";
                    break;
                case NCAType.Data:
                    hacpack += " --ncatype data";
                    hacpack += " --romfsdir \"\"" + RomFS + "\"\"";
                    break;
                case NCAType.PublicData:
                    hacpack += " --ncatype publicdata";
                    hacpack += " --romfsdir \"\"" + RomFS + "\"\"";
                    break;
                case NCAType.Program:
                    hacpack += " --ncatype program";
                    hacpack += " --exefsdir \"\"" + ExeFS + "\"\"";
                    if(!string.IsNullOrEmpty(RomFS)) hacpack += " --romfsdir \"\"" + RomFS + "\"\"";
                    if(!string.IsNullOrEmpty(Logo)) hacpack += " --logodir \"\"" + Logo + "\"\"";
                    break;
            }
            try
            {
                Utils.executeCommand(Utils.TemporaryDirectory + "\\bin\\hacpack.exe", hacpack, true);
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error processing hacPack command", "Brew.NET - NCA build error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }
        }
    }
}
