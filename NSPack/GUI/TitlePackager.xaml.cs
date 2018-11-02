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
    public partial class TitlePackager : Page
    {
        public readonly string logopath = HacPack.Utils.TemporaryDirectory + "\\logo.jpg";

        public TitlePackager()
        {
            InitializeComponent();
            if(File.Exists(logopath)) File.Delete(logopath);
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
            if(res is DialogResult.OK)
            {
                if(!File.Exists(exe.SelectedPath + "\\main"))
                {
                    GUI.Resources.log("ExeFS directory doesn't have a main source file (main)");
                    return;
                }
                if(!File.Exists(exe.SelectedPath + "\\main.npdm"))
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
            if(res is DialogResult.OK) Box_RomFS.Text = rom.SelectedPath;
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
            if(res is DialogResult.OK) Box_CustomLogo.Text = clogo.SelectedPath;
        }

        private void Button_ImportantHTMLBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog imp = new FolderBrowserDialog()
            {
                Description = "Select legal information's important HTML directory",
                ShowNewFolderButton = true,
            };
            DialogResult res = imp.ShowDialog();
            if(res is DialogResult.OK)
            {
                if(!File.Exists(imp.SelectedPath + "\\index.html"))
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
            if(res is DialogResult.OK)
            {
                if(!File.Exists(ipn.SelectedPath + "\\index.html"))
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
            if(res is DialogResult.OK)
            {
                if(!File.Exists(sup.SelectedPath + "\\index.html"))
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
            if(res is DialogResult.OK)
            {
                if(!File.Exists(off.SelectedPath + "\\index.html"))
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
            if(res is DialogResult.OK)
            {
                Bitmap logo = new Bitmap(icon.FileName);
                if(File.Exists(logopath)) File.Delete(logopath);
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
            if(res is DialogResult.OK) Box_KeySet.Text = kset.FileName;
        }

        private void Button_BuildNSP_Click(object sender, RoutedEventArgs e)
        {
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
                    if(Directory.GetFileSystemEntries(sup).Length is 0)
                    {
                        GUI.Resources.log("Support HTML directory is empty.");
                        return;
                    }
                }
            }
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
            string ccwd = Utils.Cwd;
            string exefs = null;
            string romfs = null;
            string logo = null;
            string off = null;
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
            bool hasoff = !string.IsNullOrEmpty(Box_OfflineHTML.Text);
            if(hasoff)
            {
                if(!Directory.Exists(Box_OfflineHTML.Text))
                {
                    GUI.Resources.log("Offline HTML directory does not exist.");
                    return;
                }
                off = Box_OfflineHTML.Text;
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
            bool screen = Utils.nullableBoolValue(Check_AllowScreenshots.IsChecked);
            bool video = Utils.nullableBoolValue(Check_AllowVideo.IsChecked);
            byte user = Convert.ToByte(Utils.nullableBoolValue(Check_UserAccount.IsChecked));
            GUI.Resources.CurrentApp = new NSP(kset);
            GUI.Resources.CurrentApp.TitleID = tid;
            NCA program = new NCA(kset);
            program.TitleID = tid;
            program.KeyGeneration = keygen;
            program.Type = NCAType.Program;
            program.ExeFS = exefs;
            program.RomFS = romfs;
            program.Logo = logo;
            GUI.Resources.CurrentApp.Contents.Add(program);
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
            GUI.Resources.CurrentApp.Contents.Add(control);
            if(haslinfo)
            {
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
                GUI.Resources.CurrentApp.Contents.Add(linfo);
            }
            if(hasoff)
            {
                NCA noff = new NCA(kset);
                noff.TitleID = tid;
                noff.KeyGeneration = keygen;
                noff.Type = NCAType.OfflineHTML;
                if(Directory.Exists(HacPack.Utils.TemporaryDirectory + "\\offline")) Directory.Delete(HacPack.Utils.TemporaryDirectory + "\\offline", true);
                Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\offline");
                Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\offline\\html-document");
                Directory.CreateDirectory(HacPack.Utils.TemporaryDirectory + "\\offline\\html-document\\main.htdocs");
                FileSystem.CopyDirectory(off, HacPack.Utils.TemporaryDirectory + "\\offline\\html-document\\main.htdocs");
                noff.RomFS = HacPack.Utils.TemporaryDirectory + "\\offline";
                GUI.Resources.CurrentApp.Contents.Add(noff);
            }
            SaveFileDialog nsp = new SaveFileDialog()
            {
                Title = "Build an installable NSP package",
                Filter = "Nintendo Submission Package (*.nsp)|*.nsp",
                AddExtension = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            DialogResult res = nsp.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outnsp = nsp.FileName;
                GUI.Resources.CurrentApp.generate(outnsp);
                if(File.Exists(outnsp))
                {
                    long nspsize = new FileInfo(outnsp).Length;
                    if(nspsize is 0) GUI.Resources.log("The build failed. The built NSP seems to be empty.");
                    else System.Windows.MessageBox.Show("The NSP was successfully built:\n" + outnsp + " (" + Utils.formattedSize(nspsize) + ")", GUI.Resources.Program + " - Build succeeded!");
                }
                else GUI.Resources.log("The build failed. The built NSP does not exist.");
            }
        }

        private void Button_LoadAssets_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog nsxmls = new OpenFileDialog()
            {
                Title = "Load assets from a NSPack XML assets document",
                Filter = "NSPack XML assets document (*.nsxml)|*.nsxml",
                AddExtension = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            DialogResult res = nsxmls.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outxml = nsxmls.FileName;
                try
                {
                    XDocument nsxml = XDocument.Load(outxml);
                    Box_TitleID.Text = new List<XElement>(nsxml.Descendants("TitleID"))[0].Value;
                    Combo_KeyGen.SelectedIndex = int.Parse(new List<XElement>(nsxml.Descendants("KeyGeneration"))[0].Value) - 1;
                    Box_Name.Text = new List<XElement>(nsxml.Descendants("Name"))[0].Value;
                    Box_Author.Text = new List<XElement>(nsxml.Descendants("Author"))[0].Value;
                    Box_Version.Text = new List<XElement>(nsxml.Descendants("Version"))[0].Value;
                    Box_ProductCode.Text = new List<XElement>(nsxml.Descendants("ProductCode"))[0].Value;
                    GUI.Resources.log("Assets sucessfully loaded from the NSPack XML assets document.", LogType.Information);
                }
                catch
                {
                    GUI.Resources.log("An error happened parsing the assets XML document. Are you sure it's a valid document?");
                }
            }
        }

        private void Button_SaveAssets_Click(object sender, RoutedEventArgs e)
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
            if(!File.Exists(HacPack.Utils.TemporaryDirectory + "\\logo.jpg"))
            {
                GUI.Resources.log("The default icon cannot be used.\nYou need to use your own bitmap icon.");
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
            string nsxml = "<?xml version= \"1.0\" encoding=\"utf-8\"?>";
            nsxml += "<NSPAssets>";
            nsxml += "<TitleID>" + Box_TitleID.Text + "</TitleID>";
            nsxml += "<KeyGeneration>" + keygen.ToString() + "</KeyGeneration>";
            nsxml += "<Name>" + Box_Name.Text + "</Name>";
            nsxml += "<Author>" + Box_Author.Text + "</Author>";
            nsxml += "<Version>" + Box_Version.Text + "</Version>";
            nsxml += "<ProductCode>" + Box_ProductCode.Text + "</ProductCode>";
            nsxml += "</NSPAssets>";
            SaveFileDialog nsxmls = new SaveFileDialog()
            {
                Title = "Save the assets as a NSPack XML assets document",
                Filter = "NSPack XML assets document (*.nsxml)|*.nsxml",
                AddExtension = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            };
            DialogResult res = nsxmls.ShowDialog();
            if(res is DialogResult.OK)
            {
                string outxml = nsxmls.FileName;
                File.WriteAllText(outxml, nsxml);
                if(File.Exists(outxml)) GUI.Resources.log("The assets were successfully saved into a NSPack XML assets document.", LogType.Information);
                else GUI.Resources.log("The assets were not correctly saved.");
            }
        }
    }
}
