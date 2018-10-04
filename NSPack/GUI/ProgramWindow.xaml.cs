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
        public AssetEditor editor;

        public ProgramWindow()
        {
            InitializeComponent();
            HacPack.Utils.initTemporaryDirectory();
            GUI.Resources.setTitle("Ready to build NSP packages!");
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
                    if(editor is null) editor = new AssetEditor();
                    PageHolder.NavigationService.Navigate(editor);
                    break;
            }
        }

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            if(PreSelect > 2) return;
            OptionToggle.SelectedIndex++;
            switch(PreSelect)
            {
                case 0:
                    if(start is null) start = new Start();
                    PageHolder.NavigationService.Navigate(start);
                    break;

                case 1:
                    if(editor is null) editor = new AssetEditor();
                    PageHolder.NavigationService.Navigate(editor);
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
                    if(editor is null) editor = new AssetEditor();
                    PageHolder.NavigationService.Navigate(editor);
                    break;
            }
        }
    }
}
