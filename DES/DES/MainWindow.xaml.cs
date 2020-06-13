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
        public bool AllBlocksAreFull { get; set; } = false;
        public double NumberOfBlocks { get; set; }
        public int LastBlockBytesNumber { get; set; }

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
            saveFileDialog.DefaultExt = ".bin"; 
            saveFileDialog.Filter = "Binary files (*.bin)|*.bin"; 

            // Show save file dialog box
            Nullable<bool> result = saveFileDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = saveFileDialog.FileName;
                System.Diagnostics.Debug.WriteLine(filename);
                binaryMessage.WriteFile(filename);
            }
        }
        public static BitArray StringToBitArray(string hex)
        {
            
            StringBuilder sb = new StringBuilder();
            foreach (char hexSymbol in hex)
                sb.Append(DESMethods.HexCharacters[hexSymbol]);

            string bitString = sb.ToString();
            bool[] result = bitString.Select(s => s == '0' ? false : true).ToArray();
            return new BitArray(result);
            
        }

        private void Cypher_Click(object sender, RoutedEventArgs e)
        {
            outputFileText.Text = "Inicjalizacja\n";
            if (key.Text.Length<=0)
            {
                MessageBox.Show("Provided key lenght must be bigger than 0", "Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            BitArray Key = StringToBitArray(key.Text);
            if (Key.Length != 64)
            {
                MessageBox.Show("Provided key lenght must be equal to 8 bytes", "Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            outputFileText.Text += "Generowanie klucza...\n";
            BitArray[] keys = DESMethods.GenerateKeys(Key);
            outputFileText.Text += "Wygenerowano klucz\n";
            outputFileText.Text += "Szyfrowanie wiadomości...\n";
            FileStream stream = new FileStream(binaryMessage.BM_Filepath, FileMode.Open, FileAccess.Read);
            byte[] block = new byte[BLOCK_SIZE];
            System.Diagnostics.Debug.WriteLine("FILE SIZE : " + stream.Length.ToString());
            NumberOfBlocks = Math.Ceiling((double)stream.Length / BLOCK_SIZE);
            LastBlockBytesNumber = Convert.ToInt32(stream.Length % BLOCK_SIZE);
            if(LastBlockBytesNumber == 0)
            {
                AllBlocksAreFull = true;
                NumberOfBlocks++;
            }
            else
            {
                AllBlocksAreFull = false;
            }
            for(int indexOfBlock = 0; indexOfBlock<NumberOfBlocks; indexOfBlock++)
            {
                BitArray inputBits;
                if (indexOfBlock==NumberOfBlocks-1)
                {
                    if(AllBlocksAreFull)
                    {
                        //We need to generete additional block to add at the end of the message
                        inputBits = DESMethods.Padding(64);
                    }
                    else
                    {
                        stream.Read(block, 0, LastBlockBytesNumber);
                        BitArray additionalBlock = DESMethods.Padding(64 - (8 * LastBlockBytesNumber));
                        inputBits = new BitArray(block);
                        inputBits = DESMethods.revertBitArray(inputBits);
                        inputBits = DESMethods.Merge(DESMethods.CopySlice(inputBits, 0, 8 * LastBlockBytesNumber), additionalBlock);
                    }
                }
                else
                {
                    stream.Read(block, 0, BLOCK_SIZE);
                    //convert bytes to bits
                    inputBits = new BitArray(block);
                    inputBits = DESMethods.revertBitArray(inputBits);
                }
                //as long as this does not return 0, the data in the file hasn't been completely read          
                System.Diagnostics.Debug.WriteLine(BitConverter.ToString(block));
                System.Diagnostics.Debug.WriteLine("INPUT: " + DESMethods.printBinary(inputBits));

                BitArray tempBits = DESMethods.Permute(DESMethods.IP, inputBits, 64);
                System.Diagnostics.Debug.WriteLine("IP: " + DESMethods.printBinary(tempBits));
                //Divide into halfs
                BitArray leftPart = DESMethods.CopySlice(tempBits, 0, 32);
                BitArray rightPart = DESMethods.CopySlice(tempBits, 32, 32);
                //System.Diagnostics.Debug.WriteLine("LEFT: " + DESMethods.printBinary(leftPart));
                //System.Diagnostics.Debug.WriteLine("RIGHT: " + DESMethods.printBinary(rightPart));
                BitArray leftPartPrim = new BitArray(32);
                BitArray rightPartPrim = new BitArray(32);

                for (int i = 0; i < 16; i++)
                {
                    leftPartPrim = rightPart;
                    rightPartPrim = leftPart.Xor(DESMethods.fFunction(rightPart, keys[i]));
                    //System.Diagnostics.Debug.WriteLine("RIGHT after XOR: " + DESMethods.printBinary(rightPartPrim));
                    leftPart = leftPartPrim;
                    rightPart = rightPartPrim;
                }
                BitArray preOutput = DESMethods.Merge(rightPart, leftPart);
                System.Diagnostics.Debug.WriteLine("PRE OUT: " + DESMethods.printBinary(preOutput));
                BitArray outputBits = DESMethods.Permute(DESMethods.IP_INVERSE, preOutput, 64);
                System.Diagnostics.Debug.WriteLine("OUT: " + DESMethods.printBinary(outputBits));
                outputBits = DESMethods.revertBitArray(outputBits);
                if (binaryMessage.OutputBytes == null)
                {
                    binaryMessage.OutputBytes = DESMethods.BitArrayToByteConverter(outputBits);
                }
                else
                {
                    binaryMessage.OutputBytes = binaryMessage.OutputBytes.Concat(DESMethods.BitArrayToByteConverter(outputBits)).ToArray();
                }
            }
            outputFileText.Text += "Zaszyfrowano wiadomość\n";
        }

        private void LoadFile(object sender, RoutedEventArgs e)
        {
            if(binaryMessage!=null)
            {
                binaryMessage.ReadFile();
                inputFileText.Text = binaryMessage.ToString();
            }
        }

        private void Decypher_Click(object sender, RoutedEventArgs e)
        {
            outputFileText.Text = "Inicjalizacja\n";
            if (key.Text.Length <= 0)
            {
                MessageBox.Show("Provided key lenght must be bigger than 0", "Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            BitArray Key = StringToBitArray(key.Text);

            if (Key.Length != 64)
            {
                MessageBox.Show("Provided key lenght must be equal to 8 bytes", "Key Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            outputFileText.Text += "Generowanie klucza...\n";
            BitArray[] keys = DESMethods.GenerateKeys(Key);
            outputFileText.Text += "Wygenerowano klucz\n";
            outputFileText.Text += "Rozszyfrowanie wiadomości...\n";

            FileStream stream = new FileStream(binaryMessage.BM_Filepath, FileMode.Open, FileAccess.Read);
            byte[] block = new byte[BLOCK_SIZE];
            System.Diagnostics.Debug.WriteLine("FILE SIZE : " + stream.Length.ToString());
            NumberOfBlocks = Math.Ceiling((double)stream.Length / BLOCK_SIZE);
            
            for (int indexOfBlock = 0; indexOfBlock < NumberOfBlocks; indexOfBlock++)
            {
                BitArray inputBits;
                stream.Read(block, 0, BLOCK_SIZE);
                //convert bytes to bits
                inputBits = new BitArray(block);
                
                //as long as this does not return 0, the data in the file hasn't been completely read          
                System.Diagnostics.Debug.WriteLine(BitConverter.ToString(block));
                //System.Diagnostics.Debug.WriteLine("INPUT: " + DESMethods.printBinary(inputBits));

                BitArray tempBits = DESMethods.Permute(DESMethods.IP, inputBits, 64);
                //System.Diagnostics.Debug.WriteLine("IP: " + DESMethods.printBinary(tempBits));
                //Divide into halfs
                BitArray leftPart = DESMethods.CopySlice(tempBits, 0, 32);
                BitArray rightPart = DESMethods.CopySlice(tempBits, 32, 32);
                //System.Diagnostics.Debug.WriteLine("LEFT: " + DESMethods.printBinary(leftPart));
                //System.Diagnostics.Debug.WriteLine("RIGHT: " + DESMethods.printBinary(rightPart));
                BitArray leftPartPrim = new BitArray(32);
                BitArray rightPartPrim = new BitArray(32);

                for (int i = 0; i < 16; i++)
                {
                    leftPartPrim = rightPart;
                    rightPartPrim = leftPart.Xor(DESMethods.fFunction(rightPart, keys[15-i]));
                    leftPart = leftPartPrim;
                    rightPart = rightPartPrim;
                }
                BitArray preOutput = DESMethods.Merge(rightPart, leftPart);
                BitArray outputBits = DESMethods.Permute(DESMethods.IP_INVERSE, preOutput, 64);
                if (indexOfBlock == NumberOfBlocks - 1)
                {
                    //delete padding
                    outputBits = DESMethods.DeletePadding(outputBits);
                    if (outputBits == null)
                        continue;
                }
                if (binaryMessage.OutputBytes == null)
                {
                    binaryMessage.OutputBytes = DESMethods.BitArrayToByteConverter(outputBits);
                }
                else
                {
                    binaryMessage.OutputBytes = binaryMessage.OutputBytes.Concat(DESMethods.BitArrayToByteConverter(outputBits)).ToArray();
                }
            }
            outputFileText.Text += "Rozszyfrowano wiadomość\n";

        }
    }
}
