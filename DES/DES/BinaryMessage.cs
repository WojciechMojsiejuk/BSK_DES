using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DES
{
    public class BinaryMessage
    {
        public string BM_Filepath { get; set; }
        public byte[] FileBytes { get; set; }
        public byte[] OutputBytes { get; set; }

        public BinaryMessage(string filepath)
        {
            BM_Filepath = filepath;
        }
        public void ReadFile()
        {
            try
            {
                FileBytes = File.ReadAllBytes(BM_Filepath);
            }
            catch(ArgumentException ae)
            {
                MessageBox.Show("Invalid path", ae.ToString(), MessageBoxButton.OK, MessageBoxImage.Error); 
            }
            catch(PathTooLongException ptle)
            {
                MessageBox.Show("Path too long", ptle.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DirectoryNotFoundException dnfe)
            {
                MessageBox.Show("Directory not found", dnfe.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FileNotFoundException fnfe)
            {
                MessageBox.Show("File not found", fnfe.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (NotSupportedException nse)
            {
                MessageBox.Show("Invalid file format", nse.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SecurityException se)
            {
                MessageBox.Show("No sufficient privilages to open file", se.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (IOException ioe)
            {
                MessageBox.Show("I/O Error", ioe.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void WriteFile(string filepath)
        {
            OutputBytes = FileBytes;
            File.WriteAllBytes(filepath, OutputBytes);
        }
         
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

                while(FileBytes.Length % 64 != 0)
            {
               // dodanie bitów
            }
            

            foreach (byte b in FileBytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            }
            return sb.ToString();
        }

        public static int[][] tablica = new int[9][];

        public void PermutacjeIP()
        {
            var allLines = File.ReadAllLines("permutacja.txt");

            for(int i=0; i<9;i++)
            {
                if(i>=allLines.Length)
                {
                    break;
                }
                var line = allLines[i];

                for(int j = 0; j<8;j++)
                {
                    if(j>=line.Length)
                    {
                        break;
                    }

                    tablica[i][j] = line[j];
                }
            }
        }

    }
}
