using System;
using System.Windows;
using Brew.HacPack;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace Brew.NSPack.GUI
{
    public enum LogType
    {
        Warning,
        Information,
        Error
    }

    public static class Resources
    {
        public static string Program = "NSPack tool";
        public static NSP CurrentApp = null;

        public static void log(string Text, LogType Type = LogType.Error)
        {
            string title = Program + " - ";
            MessageBoxImage img = MessageBoxImage.Information;
            switch(Type)
            {
                case LogType.Information:
                    title += "Information";
                    img = MessageBoxImage.Information;
                    break;

                case LogType.Warning:
                    title += "Warning";
                    img = MessageBoxImage.Warning;
                    break;

                case LogType.Error:
                    title += "Error";
                    img = MessageBoxImage.Error;
                    break;
            }
            MessageBox.Show(Text, title, MessageBoxButton.OK, img);
        }

        public static void setTitle(string Title)
        {
            ProgramWindow main = Application.Current.MainWindow as ProgramWindow;
            main.Title = Program + " → " + Title;
        }

        public static void resetTitle() => setTitle("Ready to build NSP packages!");

        public static void actionAsync(string Description, Action Action)
        {
            setTitle(Description);
            new Task(Action).Start();
        }
    }
}