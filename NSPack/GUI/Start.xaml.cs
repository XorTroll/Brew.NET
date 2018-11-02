using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Brew.NSPack.GUI
{
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();
        }

        private void Button_StartNSP_Click(object sender, RoutedEventArgs e)
        {
            ProgramWindow main = System.Windows.Application.Current.MainWindow as ProgramWindow;
            if(main.editor is null) main.editor = new TitlePackager();
            main.OptionToggle.SelectedIndex = 1;
            main.PreSelect = 1;
            main.PageHolder.NavigationService.Navigate(main.editor);
        }

        private void Button_StartNCA_Click(object sender, RoutedEventArgs e)
        {
            ProgramWindow main = System.Windows.Application.Current.MainWindow as ProgramWindow;
            if (main.creator is null) main.creator = new ContentCreator();
            main.OptionToggle.SelectedIndex = 2;
            main.PreSelect = 2;
            main.PageHolder.NavigationService.Navigate(main.creator);
        }
    }
}
