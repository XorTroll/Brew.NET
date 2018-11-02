using System;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using System.IO;
using Brew.HacPack;

namespace Brew.NSPack.GUI
{

    public partial class ProgramWindow : MetroWindow
    {
        public int PreSelect = 0;
        public Start start;
        public TitlePackager editor;
        public ContentCreator creator;

        public ProgramWindow()
        {
            InitializeComponent();
            HacPack.Utils.initTemporaryDirectory();
            GUI.Resources.resetTitle();
        }

        private void OptionToggle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(PageHolder is null) return;
            int tsel = OptionToggle.SelectedIndex;
            if(PreSelect == tsel) return;
            PreSelect = tsel;
            switch(PreSelect)
            {
                case 0:
                    if(start is null) start = new Start();
                    PageHolder.NavigationService.Navigate(start);
                    break;
                case 1:
                    if(editor is null) editor = new TitlePackager();
                    PageHolder.NavigationService.Navigate(editor);
                    break;
                case 2:
                    if(creator is null) creator = new ContentCreator();
                    PageHolder.NavigationService.Navigate(creator);
                    break;
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if(PreSelect > 3) return;
            OptionToggle.SelectedIndex++;
            switch(PreSelect)
            {
                case 0:
                    if(start is null) start = new Start();
                    PageHolder.NavigationService.Navigate(start);
                    break;
                case 1:
                    if(editor is null) editor = new TitlePackager();
                    PageHolder.NavigationService.Navigate(editor);
                    break;
                case 2:
                    if(creator is null) creator = new ContentCreator();
                    PageHolder.NavigationService.Navigate(creator);
                    break;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            HacPack.Utils.exitTemporaryDirectory();
            base.OnClosing(e);
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if(PreSelect <= 0) return;
            OptionToggle.SelectedIndex--;
            switch(PreSelect)
            {
                case 0:
                    if(start is null) start = new Start();
                    PageHolder.NavigationService.Navigate(start);
                    break;
                case 1:
                    if(editor is null) editor = new TitlePackager();
                    PageHolder.NavigationService.Navigate(editor);
                    break;
                case 2:
                    if(creator is null) creator = new ContentCreator();
                    PageHolder.NavigationService.Navigate(creator);
                    break;
            }
        }
    }
}
