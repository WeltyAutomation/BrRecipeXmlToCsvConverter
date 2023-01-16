using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BrRecipeXmlToCsvConverter;
using Microsoft.Win32;

namespace BrRecipeConverterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenXml_OnClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Title = "Open B&R Recipe XML File for Conversion to CSV",
                DefaultExt = ".xml",
                Filter = "Xml documents (.xml)|*.xml"

            };

            dlg.FileOk += (o, args) =>
            {
                if (!File.Exists(dlg.FileName)) throw new FileNotFoundException(dlg.FileName);
                var reader = File.OpenText(dlg.FileName);
                var results = BrRecipeXmlToCsvTool.ConvertXmlToCsv(reader.ReadToEnd());
                Results.Text = results;
            };

            dlg.ShowDialog();
        }
    }
}
