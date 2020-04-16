using Microsoft.Win32;
using System;
using System.Collections;
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

        public const int BLOCK_SIZE = 8; // 8 bytes = 64 

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog() == true)
            {
                binaryMessage = new BinaryMessage(openFileDialog.FileName);
                filepath.Text = binaryMessage.BM_Filepath;
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

        private void Cypher_Click(object sender, RoutedEventArgs e)
        {
            if(key.Text.Length<=0)
            {
                MessageBox.Show("Provided key lenght must be bigger than 0", "Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            byte[] Key = Encoding.Default.GetBytes(key.Text);
             
            if (Key.Length != 8)
            {
                MessageBox.Show("Provided key lenght must be equal to 8 bytes", "Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            BitArray[] keys = DESMethods.GenerateKeys(new BitArray(Key));

            FileStream stream = new FileStream(binaryMessage.BM_Filepath, FileMode.Open, FileAccess.Read);
            byte[] block = new byte[BLOCK_SIZE];
            while (stream.Read(block, 0, BLOCK_SIZE) > 0)
            {  //as long as this does not return 0, the data in the file hasn't been completely read          
                System.Diagnostics.Debug.WriteLine(BitConverter.ToString(block));
                //convert bytes to bits
                BitArray inputBits = new BitArray(block);
                System.Diagnostics.Debug.WriteLine("INPUT: " + DESMethods.printBinary(inputBits));
                BitArray tempBits = DESMethods.Permute(DESMethods.IP, inputBits, 64);
                System.Diagnostics.Debug.WriteLine("IP: " + DESMethods.printBinary(tempBits));
                //Divide into halfs
                BitArray leftPart = DESMethods.CopySlice(tempBits, 0, 32);
                BitArray rightPart = DESMethods.CopySlice(tempBits, 32, 32);
                System.Diagnostics.Debug.WriteLine("LEFT: " + DESMethods.printBinary(leftPart));
                System.Diagnostics.Debug.WriteLine("RIGHT: " + DESMethods.printBinary(rightPart));
                BitArray leftPartPrim = new BitArray(32);
                BitArray rightPartPrim = new BitArray(32);

                for (int i = 0; i<16; i++)
                {
                    leftPartPrim = rightPart;
                    rightPartPrim = leftPart.Xor(DESMethods.fFunction(rightPart, keys[i]));
                    leftPart = leftPartPrim;
                    rightPart = rightPartPrim;
                }
                BitArray preOutput = DESMethods.Merge(rightPart, leftPart);
                BitArray outputBits = DESMethods.Permute(DESMethods.IP_INVERSE, preOutput, 64);
                if (binaryMessage.OutputBytes == null)
                {
                    binaryMessage.OutputBytes = DESMethods.BitArrayToByteConverter(outputBits);
                }
                else
                {
                    binaryMessage.OutputBytes.Concat(DESMethods.BitArrayToByteConverter(outputBits)).ToArray();
                }
            }
        }

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            if(binaryMessage!=null)
            {
                binaryMessage.ReadFile();
                inputFileText.Text = binaryMessage.ToString();
            }
        }
    }
}
