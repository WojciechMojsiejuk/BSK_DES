using Microsoft.Win32;
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

namespace DES
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private BinaryMessage binaryMessage;

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == true)
            {
                binaryMessage = new BinaryMessage(openFileDialog.FileName);
                filepath.Text = binaryMessage.BM_Filepath;
                binaryMessage.ReadFile();
                inputFileText.Text = binaryMessage.ToString();
            }
                

        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "output"; 
            saveFileDialog.DefaultExt = ".bin"; 
            saveFileDialog.Filter = "Binary files (*.bin)|*.bin"; 

            // Show save file dialog box
            Nullable<bool> result = saveFileDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = saveFileDialog.FileName;
                binaryMessage.WriteFile(filename);
            }
        }
    }
}
