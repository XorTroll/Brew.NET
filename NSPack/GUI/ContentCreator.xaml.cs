using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using System.Windows.Interop;
using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using Brew.HacPack;

namespace Brew.NSPack.GUI
{
    public partial class ContentCreator : Page
    {
        public readonly string logopath = HacPack.Utils.TemporaryDirectory + "\\logo.jpg";

        public ContentCreator()
        {
            InitializeComponent();
            if (File.Exists(logopath)) File.Delete(logopath);
            Properties.Resources.SampleLogo.Save(logopath, ImageFormat.Jpeg);
            Image_Icon.Source = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.SampleLogo.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        private void Button_ExeFSBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog exe = new FolderBrowserDialog()
            {
                Description = "Select ExeFS directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = exe.ShowDialog();
            if (res is DialogResult.OK)
            {
                if (!File.Exists(exe.SelectedPath + "\\main"))
                {
                    GUI.Resources.log("ExeFS directory doesn't have a main source file (main)");
                    return;
                }
                if (!File.Exists(exe.SelectedPath + "\\main.npdm"))
                {
                    GUI.Resources.log("ExeFS directory doesn't have a NPDM metadata file (main.npdm)");
                    return;
                }
                Box_ExeFS.Text = exe.SelectedPath;
            }
        }

        private void Button_RomFSBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog rom = new FolderBrowserDialog()
            {
                Description = "Select RomFS directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = rom.ShowDialog();
            if (res is DialogResult.OK) Box_RomFS.Text = rom.SelectedPath;
        }

        private void Button_CustomLogoBrowse_Click(object sender, RoutedEventArgs e)
        {
            GUI.Resources.log("The logo directory must contain just two files: a GIF file and a PNG file.\nThe GIF and the PNG need to be made using Photoshop.\nHorizon reads their metadata, otherwise they won't work.", LogType.Warning);
            FolderBrowserDialog clogo = new FolderBrowserDialog()
            {
                Description = "Select custom logo directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = clogo.ShowDialog();
            if (res is DialogResult.OK) Box_CustomLogo.Text = clogo.SelectedPath;
        }

        private void Button_ImportantHTMLBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog imp = new FolderBrowserDialog()
            {
                Description = "Select legal information's important HTML directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = imp.ShowDialog();
            if (res is DialogResult.OK)
            {
                if (!File.Exists(imp.SelectedPath + "\\index.html"))
                {
                    GUI.Resources.log("Selected HTML documents folder does not have a HTML index file (index.html)");
                    return;
                }
                Box_ImportantHTML.Text = imp.SelectedPath;
            }
        }

        private void Button_IPNoticesHTMLBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog ipn = new FolderBrowserDialog()
            {
                Description = "Select legal information's IPNotices HTML directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = ipn.ShowDialog();
            if (res is DialogResult.OK)
            {
                if (!File.Exists(ipn.SelectedPath + "\\index.html"))
                {
                    GUI.Resources.log("Selected HTML documents folder does not have a HTML index file (index.html)");
                    return;
                }
                Box_IPNoticesHTML.Text = ipn.SelectedPath;
            }
        }

        private void Button_SupportHTMLBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog sup = new FolderBrowserDialog()
            {
                Description = "Select legal information's support HTML directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = sup.ShowDialog();
            if (res is DialogResult.OK)
            {
                if (!File.Exists(sup.SelectedPath + "\\index.html"))
                {
                    GUI.Resources.log("Selected HTML documents folder does not have a HTML index file (index.html)");
                    return;
                }
                Box_SupportHTML.Text = sup.SelectedPath;
            }
        }

        private void Button_OfflineHTMLBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog off = new FolderBrowserDialog()
            {
                Description = "Select offline HTML directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = off.ShowDialog();
            if (res is DialogResult.OK)
            {
                if (!File.Exists(off.SelectedPath + "\\index.html"))
                {
                    GUI.Resources.log("Selected HTML documents folder does not have a HTML index file (index.html)");
                    return;
                }
                Box_OfflineHTML.Text = off.SelectedPath;
            }
        }

        private void Button_IconBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog icon = new OpenFileDialog()
            {
                Title = "Select icon for the NSP package",
                Filter = "Common image types (*.bmp, *.png, *.jpg, *.jpeg, *.dds, *.tga)|*.bmp;*.png;*.jpg;*.jpeg;*.dds;*.tga",

                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            DialogResult res = icon.ShowDialog();
            if (res is DialogResult.OK)
            {
                Bitmap logo = new Bitmap(icon.FileName);
                if (File.Exists(logopath)) File.Delete(logopath);
                logo.Save(logopath, ImageFormat.Jpeg);
                Image_Icon.Source = Imaging.CreateBitmapSourceFromHBitmap(logo.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
        }

        private void Button_KeySetBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog kset = new OpenFileDialog()
            {
                Title = "Select keyset file",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            DialogResult res = kset.ShowDialog();
            if (res is DialogResult.OK) Box_KeySet.Text = kset.FileName;
        }

        private void Button_BuildProgram_Click(object sender, RoutedEventArgs e)
        {
            string tid = Box_TitleID.Text;
            if(tid.Length != 16)
            {
                GUI.Resources.log("Title ID doesn't have 16 characters.");
                return;
            }
            if(!Regex.IsMatch(tid, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                GUI.Resources.log("Title ID is not a valid hex string.");
                return;
            }
            byte keygen = 5;
            string kgen = Combo_KeyGen.Text;
            if(kgen is "1 (1.0.0 - 2.3.0)") keygen = 1;
            else if(kgen is "2 (3.0.0)") keygen = 2;
            else if(kgen is "3 (3.0.1 - 3.0.2)") keygen = 3;
            else if(kgen is "4 (4.0.0 - 4.1.0)") keygen = 4;
            else if(kgen is "5 (5.0.0 - 5.1.0)") keygen = 5;
            else if(kgen is "6 (6.0.0 - Latest)") keygen = 6;
            string ccwd = Utils.Cwd;
            string exefs = null;
            string romfs = null;
            string logo = null;
            if(string.IsNullOrEmpty(Box_ExeFS.Text))
            {
                GUI.Resources.log("ExeFS directory was not selected.");
                return;
            }
            if(!Directory.Exists(Box_ExeFS.Text))
            {
                GUI.Resources.log("ExeFS directory does not exist.");
                return;
            }
            exefs = Box_ExeFS.Text;
            if(!string.IsNullOrEmpty(Box_RomFS.Text))
            {
                if(!Directory.Exists(Box_RomFS.Text))
                {
                    GUI.Resources.log("RomFS directory does not exist.");
                    return;
                }
                romfs = Box_RomFS.Text;
            }
            if(!string.IsNullOrEmpty(Box_CustomLogo.Text))
            {
                if(!Directory.Exists(Box_CustomLogo.Text))
                {
                    GUI.Resources.log("Custom logo directory does not exist.");
                    return;
                }
                if(!File.Exists(Box_CustomLogo.Text + "\\NintendoLogo.png"))
                {
                    GUI.Resources.log("NintendoLogo.png logo file does not exist in custom logo's directory.");
                    return;
                }
                if(!File.Exists(Box_CustomLogo.Text + "\\StartupMovie.gif"))
                {
                    GUI.Resources.log("StartupMovie.giflogo file does not exist in custom logo's directory.");
                    return;
                }
                if(Directory.GetFileSystemEntries(Box_CustomLogo.Text).Length > 2)
                {
                    GUI.Resources.log("There are too many files or directories in custom logo directory.");
                    return;
                }
                logo = Box_CustomLogo.Text;
            }
            if(string.IsNullOrEmpty(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file was not selected. This file is required to build the NSP package.");
                return;
            }
            if(!File.Exists(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file does not exist. This file is required to build the NSP package.");
                return;
            }
            string kset = Box_KeySet.Text;
            NCA program = new NCA(kset);
            program.TitleID = tid;
            program.KeyGeneration = keygen;
            program.Type = NCAType.Program;
            program.ExeFS = exefs;
            program.RomFS = romfs;
            program.Logo = logo;
            FolderBrowserDialog cnca = new FolderBrowserDialog()
            {
                Description = "Select directory to save program NCA content (will create a 'Program' directory)",
                ShowNewFolderButton = true,
            };
            DialogResult res = cnca.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outnca = cnca.SelectedPath + "\\Program";
                if(!Directory.Exists(outnca)) Directory.CreateDirectory(outnca);
                program.generate(outnca);
                if(Directory.Exists(outnca))
                {
                    string[] files = Directory.GetFiles(outnca);
                    if(files.Length <= 0) GUI.Resources.log("The build failed. No NCA was generated.");
                    bool ncafound = false;
                    foreach(string file in files)
                    {
                        if(file.ToLower().EndsWith("nca"))
                        {
                            long size = new FileInfo(file).Length;
                            GUI.Resources.log("The build succeeded: \n" + file + "\nFile size: " + Utils.formattedSize(size), LogType.Information);
                            ncafound = true;
                            break;
                        }
                    }
                    if(!ncafound) GUI.Resources.log("The build failed. No NCA was generated.");
                }
                else GUI.Resources.log("The build failed. The NCA's directory does not exist.");
            }
        }

        private void Button_BuildControl_Click(object sender, RoutedEventArgs e)
        {
            string tid = Box_TitleID.Text;
            if(tid.Length != 16)
            {
                GUI.Resources.log("Title ID doesn't have 16 characters.");
                return;
            }
            if(!Regex.IsMatch(tid, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                GUI.Resources.log("Title ID is not a valid hex string.");
                return;
            }
            byte keygen = 5;
            string kgen = Combo_KeyGen.Text;
            if(kgen is "1 (1.0.0 - 2.3.0)") keygen = 1;
            else if(kgen is "2 (3.0.0)") keygen = 2;
            else if(kgen is "3 (3.0.1 - 3.0.2)") keygen = 3;
            else if(kgen is "4 (4.0.0 - 4.1.0)") keygen = 4;
            else if(kgen is "5 (5.0.0 - 5.1.0)") keygen = 5;
            else if(kgen is "6 (6.0.0 - Latest)") keygen = 6;
            string ccwd = Utils.Cwd;
            if(string.IsNullOrEmpty(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file was not selected. This file is required to build the NSP package.");
                return;
            }
            if(!File.Exists(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file does not exist. This file is required to build the NSP package.");
                return;
            }
            if(string.IsNullOrEmpty(Box_Name.Text))
            {
                GUI.Resources.log("No application name was set.");
                return;
            }
            if(string.IsNullOrEmpty(Box_Version.Text))
            {
                GUI.Resources.log("No application version string was set.");
                return;
            }
            if(string.IsNullOrEmpty(Box_ProductCode.Text))
            {
                GUI.Resources.log("No application product code string was set.\nRetail games use 'LA-H-XXXXX' (SMO uses LA-H-AAACA)");
                return;
            }
            string kset = Box_KeySet.Text;
            bool screen = Utils.nullableBoolValue(Check_AllowScreenshots.IsChecked);
            bool video = Utils.nullableBoolValue(Check_AllowVideo.IsChecked);
            byte user = Convert.ToByte(Utils.nullableBoolValue(Check_UserAccount.IsChecked));
            NCA control = new NCA(kset);
            control.TitleID = tid;
            control.KeyGeneration = keygen;
            control.Type = NCAType.Control;
            NACP controlnacp = new NACP();
            NACPEntry ent = new NACPEntry();
            ent.Name = Box_Name.Text;
            ent.Author = Box_Author.Text;
            controlnacp.Screenshot = screen;
            controlnacp.VideoCapture = video;
            controlnacp.StartupUserAccount = user;
            controlnacp.ProductCode = Box_ProductCode.Text;
            controlnacp.ApplicationId = tid;
            controlnacp.Version = Box_Version.Text;
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            controlnacp.Entries.Add(ent);
            Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\control");
            controlnacp.generate(HacPack.Utils.TemporaryDirectory + "\\control");
            ControlIcons.generate(logopath, HacPack.Utils.TemporaryDirectory + "\\control");
            control.RomFS = HacPack.Utils.TemporaryDirectory + "\\control";
            FolderBrowserDialog cnca = new FolderBrowserDialog()
            {
                Description = "Select directory to save control NCA content (will create a 'Control' directory)",
                ShowNewFolderButton = true,
            };
            DialogResult res = cnca.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outnca = cnca.SelectedPath + "\\Control";
                if(!Directory.Exists(outnca)) Directory.CreateDirectory(outnca);
                control.generate(outnca);
                if(Directory.Exists(outnca))
                {
                    string[] files = Directory.GetFiles(outnca);
                    if(files.Length <= 0) GUI.Resources.log("The build failed. No NCA was generated.");
                    bool ncafound = false;
                    foreach(string file in files)
                    {
                        if(file.ToLower().EndsWith("nca"))
                        {
                            long size = new FileInfo(file).Length;
                            GUI.Resources.log("The build succeeded: \n" + file + "\nFile size: " + Utils.formattedSize(size), LogType.Information);
                            ncafound = true;
                            Directory.Delete(HacPack.Utils.TemporaryDirectory + "\\control", true);
                            break;
                        }
                    }
                    if(!ncafound) GUI.Resources.log("The build failed. No NCA was generated.");
                }
                else GUI.Resources.log("The build failed. The NCA's directory does not exist.");
            }
        }

        private void Button_BuildLegalInfo_Click(object sender, RoutedEventArgs e)
        {
            string tid = Box_TitleID.Text;
            if(tid.Length != 16)
            {
                GUI.Resources.log("Title ID doesn't have 16 characters.");
                return;
            }
            if(!Regex.IsMatch(tid, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                GUI.Resources.log("Title ID is not a valid hex string.");
                return;
            }
            byte keygen = 5;
            string kgen = Combo_KeyGen.Text;
            if (kgen is "1 (1.0.0 - 2.3.0)") keygen = 1;
            else if (kgen is "2 (3.0.0)") keygen = 2;
            else if (kgen is "3 (3.0.1 - 3.0.2)") keygen = 3;
            else if (kgen is "4 (4.0.0 - 4.1.0)") keygen = 4;
            else if (kgen is "5 (5.0.0 - 5.1.0)") keygen = 5;
            else if (kgen is "6 (6.0.0 - Latest)") keygen = 6;
            string ccwd = Utils.Cwd;
            string imp = Box_ImportantHTML.Text;
            string ipn = Box_IPNoticesHTML.Text;
            string sup = Box_SupportHTML.Text;
            bool hasimp = !string.IsNullOrEmpty(imp);
            bool hasipn = !string.IsNullOrEmpty(ipn);
            bool hassup = !string.IsNullOrEmpty(sup);
            bool haslinfo = (hasimp || hasipn || hassup);
            if(haslinfo)
            {
                if(hasimp)
                {
                    if(!Directory.Exists(imp))
                    {
                        GUI.Resources.log("Important HTML directory does not exist.");
                        return;
                    }
                    if(!File.Exists(imp + "\\index.html"))
                    {
                        GUI.Resources.log("Important HTML directory does not have a 'index.html' entry file.");
                        return;
                    }
                    if(Directory.GetFileSystemEntries(imp).Length is 0)
                    {
                        GUI.Resources.log("Important HTML directory is empty.");
                        return;
                    }
                }
                if(hasipn)
                {
                    if(!Directory.Exists(ipn))
                    {
                        GUI.Resources.log("IPNotices HTML directory does not exist.");
                        return;
                    }
                    if(!File.Exists(ipn + "\\index.html"))
                    {
                        GUI.Resources.log("IPNotices HTML directory does not have a 'index.html' entry file.");
                        return;
                    }
                    if(Directory.GetFileSystemEntries(ipn).Length is 0)
                    {
                        GUI.Resources.log("IPNotices HTML directory is empty.");
                        return;
                    }
                }
                if(hassup)
                {
                    if(!Directory.Exists(sup))
                    {
                        GUI.Resources.log("Support HTML directory does not exist.");
                        return;
                    }
                    if(!File.Exists(sup + "\\index.html"))
                    {
                        GUI.Resources.log("Support HTML directory does not have a 'index.html' entry file.");
                        return;
                    }
                    if(Directory.GetFileSystemEntries(sup).Length is 0)
                    {
                        GUI.Resources.log("Support HTML directory is empty.");
                        return;
                    }
                }
            }
            else
            {
                GUI.Resources.log("At least one of the legalinfo HTML directories needs to be set.");
                return;
            }
            if(string.IsNullOrEmpty(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file was not selected. This file is required to build the NSP package.");
                return;
            }
            if(!File.Exists(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file does not exist. This file is required to build the NSP package.");
                return;
            }
            string kset = Box_KeySet.Text;
            NCA linfo = new NCA(kset);
            linfo.TitleID = tid;
            linfo.KeyGeneration = keygen;
            linfo.Type = NCAType.LegalInformation;
            if(Directory.Exists(HacPack.Utils.TemporaryDirectory + "\\legalinfo")) Directory.Delete(HacPack.Utils.TemporaryDirectory + "\\legalinfo", true);
            Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\legalinfo");
            if(hasimp) FileSystem.CopyDirectory(imp, HacPack.Utils.TemporaryDirectory + "\\legalinfo\\important.htdocs");
            if(hasipn) FileSystem.CopyDirectory(ipn, HacPack.Utils.TemporaryDirectory + "\\legalinfo\\ipnotices.htdocs");
            if(hassup) FileSystem.CopyDirectory(sup, HacPack.Utils.TemporaryDirectory + "\\legalinfo\\support.htdocs");
            linfo.RomFS = HacPack.Utils.TemporaryDirectory + "\\legalinfo";
            FolderBrowserDialog cnca = new FolderBrowserDialog()
            {
                Description = "Select directory to save legalinfo NCA content (will create a 'LegalInfo' directory)",
                ShowNewFolderButton = true,
            };
            DialogResult res = cnca.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outnca = cnca.SelectedPath + "\\LegalInfo";
                if(!Directory.Exists(outnca)) Directory.CreateDirectory(outnca);
                linfo.generate(outnca);
                if (Directory.Exists(outnca))
                {
                    string[] files = Directory.GetFiles(outnca);
                    if (files.Length <= 0) GUI.Resources.log("The build failed. No NCA was generated.");
                    bool ncafound = false;
                    foreach (string file in files)
                    {
                        if (file.ToLower().EndsWith("nca"))
                        {
                            long size = new FileInfo(file).Length;
                            GUI.Resources.log("The build succeeded: \n" + file + "\nFile size: " + Utils.formattedSize(size), LogType.Information);
                            ncafound = true;
                            break;
                        }
                    }
                    if (!ncafound) GUI.Resources.log("The build failed. No NCA was generated.");
                }
                else GUI.Resources.log("The build failed. The NCA's directory does not exist.");
            }
        }

        private void Button_BuildOffline_Click(object sender, RoutedEventArgs e)
        {
            string tid = Box_TitleID.Text;
            if(tid.Length != 16)
            {
                GUI.Resources.log("Title ID doesn't have 16 characters.");
                return;
            }
            if(!Regex.IsMatch(tid, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                GUI.Resources.log("Title ID is not a valid hex string.");
                return;
            }
            byte keygen = 5;
            string kgen = Combo_KeyGen.Text;
            if (kgen is "1 (1.0.0 - 2.3.0)") keygen = 1;
            else if (kgen is "2 (3.0.0)") keygen = 2;
            else if (kgen is "3 (3.0.1 - 3.0.2)") keygen = 3;
            else if (kgen is "4 (4.0.0 - 4.1.0)") keygen = 4;
            else if (kgen is "5 (5.0.0 - 5.1.0)") keygen = 5;
            else if (kgen is "6 (6.0.0 - Latest)") keygen = 6;
            string ccwd = Utils.Cwd;
            string off = Box_OfflineHTML.Text;
            if(string.IsNullOrEmpty(off))
            {
                GUI.Resources.log("No offline HTML directory was set.");
                return;
            }
            if (!Directory.Exists(off))
            {
                GUI.Resources.log("Offline HTML directory does not exist.");
                return;
            }
            if(Directory.GetFileSystemEntries(off).Length is 0)
            {
                GUI.Resources.log("Offline HTML directory is empty.");
                return;
            }
            if(!File.Exists(off + "\\index.html"))
            {
                GUI.Resources.log("Offline HTML directory does not have a 'index.html' entry file.");
                return;
            }
            if(string.IsNullOrEmpty(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file was not selected. This file is required to build the NSP package.");
                return;
            }
            if(!File.Exists(Box_KeySet.Text))
            {
                GUI.Resources.log("Keyset file does not exist. This file is required to build the NSP package.");
                return;
            }
            string kset = Box_KeySet.Text;
            NCA offh = new NCA(kset);
            offh.TitleID = tid;
            offh.KeyGeneration = keygen;
            offh.Type = NCAType.OfflineHTML;
            if(Directory.Exists(HacPack.Utils.TemporaryDirectory + "\\offline")) Directory.Delete(HacPack.Utils.TemporaryDirectory + "\\offline", true);
            Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\offline");
            Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\offline\\html-document");
            Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\offline\\html-document\\main.htdocs");
            FileSystem.CopyDirectory(off, HacPack.Utils.TemporaryDirectory + "\\offline\\html-document\\main.htdocs");
            offh.RomFS = HacPack.Utils.TemporaryDirectory + "\\offline";
            FolderBrowserDialog cnca = new FolderBrowserDialog()
            {
                Description = "Select directory to save offline HTML NCA content (will create a 'OfflineHTML' directory)",
                ShowNewFolderButton = true,
            };
            DialogResult res = cnca.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outnca = cnca.SelectedPath + "\\OfflineHTML";
                if(!Directory.Exists(outnca)) Directory.CreateDirectory(outnca);
                offh.generate(outnca);
                if(Directory.Exists(outnca))
                {
                    string[] files = Directory.GetFiles(outnca);
                    if(files.Length <= 0) GUI.Resources.log("The build failed. No NCA was generated.");
                    bool ncafound = false;
                    foreach(string file in files)
                    {
                        if(file.ToLower().EndsWith("nca"))
                        {
                            long size = new FileInfo(file).Length;
                            GUI.Resources.log("The build succeeded: \n" + file + "\nFile size: " + Utils.formattedSize(size), LogType.Information);
                            ncafound = true;
                            break;
                        }
                    }
                    if(!ncafound) GUI.Resources.log("The build failed. No NCA was generated.");
                }
                else GUI.Resources.log("The build failed. The NCA's directory does not exist.");
            }
        }
    }
}
