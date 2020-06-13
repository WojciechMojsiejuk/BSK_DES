using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    public static class DESMethods
    {
        //ARRAYS DEFINITION

        public static int[] IP = new int[]
       {
            57, 49, 41, 33, 25, 17, 9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7,
            56, 48, 40, 32, 24, 16, 8, 0,
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6
       };

        public static int[] IP_INVERSE = new int[]
        {
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25,
            32, 0, 40, 8, 48, 16, 56, 24
        };

        public static int[] E = new int[]
        {
            31, 0, 1, 2, 3, 4,
            3, 4, 5, 6, 7, 8,
            7, 8, 9, 10, 11, 12,
            11, 12, 13, 14, 15, 16,
            15, 16, 17, 18, 19, 20,
            19, 20, 21, 22, 23, 24,
            23, 24, 25, 26, 27, 28,
            27, 28, 29, 30, 31, 0
        };

        public static int[] P = new int[]
        {
            15, 6, 19, 20, 28, 11, 27, 16,
            0, 14, 22, 25, 4, 17, 30, 9,
            1, 7, 23, 13, 31, 26, 2, 8,
            18, 12, 29, 5, 21, 10, 3, 24
        };

        //Drop parity bits 64b -> 56b
        public static int[] PC1 = new int[]
        {
            56, 48, 40, 32, 24, 16, 8,
            0, 57, 49, 41, 33, 25, 17,
            9, 1, 58, 50, 42, 34, 26,
            18, 10, 2, 59, 51, 43, 35,
            62, 54, 46, 38, 30, 22, 14,
            6, 61, 53, 45, 37, 29, 21,
            13, 5, 60, 52, 44, 36, 28,
            20, 12, 4, 27, 19, 11, 3 
        };

        //Compress key 56b -> 48b
        public static int[] PC2 = new int[]
        {
            13, 16, 10, 23, 0, 4,
            2, 27, 14, 5, 20, 9,
            22, 18, 11, 3, 25, 7,
            15, 6, 26, 19, 12, 1,
            40, 51, 30, 36, 46, 54,
            29, 39, 50, 44, 32, 47,
            43, 48, 38, 55, 33, 52,
            45, 41, 49, 35, 28, 31
        };

        public static int[] keySchedule = new int[] 
        {
            1, 1, 2, 2, 
            2, 2, 2, 2, 
            1, 2, 2, 2, 
            2, 2, 2, 1 
        };

        public static int[,,] S = new int[,,]
        { 
            {
                { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
            }, 
            {
                { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
            }, 
  
            {
                { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
            }, 
            {
                { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
                { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
                { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
            }, 
            {
                { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
            }, 
            {
                { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
            }, 
            {
                { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
            }, 
            {
                { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
            } 
        };
        public static Dictionary<char, string> HexCharacters = new Dictionary<char, string>
        {
            ['0'] = "0000",
            ['1'] = "0001",
            ['2'] = "0010",
            ['3'] = "0011",
            ['4'] = "0100",
            ['5'] = "0101",
            ['6'] = "0110",
            ['7'] = "0111",
            ['8'] = "1000",
            ['9'] = "1001",
            ['A'] = "1010",
            ['B'] = "1011",
            ['C'] = "1100",
            ['D'] = "1101",
            ['E'] = "1110",
            ['F'] = "1111",
        };

        public static int[] getSIndex(BitArray bitArray)
        {
            int[] coords = new int[2] { 0, 0 };
            if(bitArray.Length!=6)
            {
                throw new ArgumentException("BitArray must consists 6 bits");
            }
            if((bool)bitArray[0] == true)
            {
                coords[0] += 2;
            }
            if ((bool)bitArray[1] == true)
            {
                coords[1] += 8;
            }
            if ((bool)bitArray[2] == true)
            {
                coords[1] += 4;
            }
            if ((bool)bitArray[3] == true)
            {
                coords[1] += 2;
            }
            if ((bool)bitArray[4] == true)
            {
                coords[1] += 1;
            }
            if ((bool)bitArray[5] == true)
            {
                coords[0] += 1;
            }
            return coords;
        }


        public static BitArray Permute(int[] permutationTable, BitArray inputBlock, int size)
        {
            BitArray result = new BitArray(size);
            for (int i = 0; i < size; i++)
            {
                int index = permutationTable[i];
                result[i] = inputBlock[index];
            }
            return result;
        }
        public static BitArray PermuteKey(int[] permutationTable, BitArray inputBlock, int size)
        {
            if (inputBlock.Length < size)
            {
                System.Diagnostics.Debug.WriteLine("Niepelny blok, dlugosc {0}", inputBlock.Length);
            }
            BitArray result = new BitArray(inputBlock.Length);
            for (int i = 0; i < size; i++)
            {
                if(i % 7 == 6 )
                {

                }
                int index = permutationTable[i];
                result[i] = inputBlock[index];
            }
            return result;
        }

        public static BitArray Padding(int size)
        {
            bool[] array = new bool[size];
            array[0] = true;
            for(int i = 1; i<size; i++)
            {
                array[i] = false;
            }
            BitArray padding = new BitArray(array);
            System.Diagnostics.Debug.WriteLine("Padding: "+printBinary(padding));
            return padding;
        }
        public static BitArray DeletePadding(BitArray block)
        {
            for(int i = 0; i<block.Length; i++)
            {
                //search until first 1 in block
                if((bool)block[block.Length - i - 1]==true && i != block.Length - 1)
                {
                    return CopySlice(block, 0, block.Length - i - 1);
                }
            }
            //whole block needs to be deleted
            return null;
        }



        public static byte[] BitArrayToByteConverter(BitArray bits)
        {
            byte[] results = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(results, 0);
            return results;
        }

        public static BitArray revertBitArray(BitArray block)
        {
            BitArray revertedBlock = new BitArray(block.Length);
            for(int i = 0; i<8;i++)
            {
                for(int j = 0; j<8; j++)
                {
                    revertedBlock[(8 * i) + j] = block[(8 * i) + (7 - j)];
                }
            }
            return revertedBlock;
        }

        public static BitArray[] GenerateKeys(BitArray key)
        {
            System.Diagnostics.Debug.WriteLine("ENTERED KEY: " + DESMethods.printBinary(key));
            if (key.Length < 64)
            {
                throw new ArgumentException("Key needs to be 64bit array");
            }
            BitArray[] keys = new BitArray[16];
            BitArray pc1Output = Permute(PC1, key, 56);
            System.Diagnostics.Debug.WriteLine("PC1 output: "+ printBinary(pc1Output));
            BitArray c = new BitArray(28);
            BitArray d = new BitArray(28);
            //Devide output into halfs
            c = CopySlice(pc1Output, 0, 28);
            d = CopySlice(pc1Output, 28, 28);
            //System.Diagnostics.Debug.WriteLine("C0: " + printBinary(c));
            //System.Diagnostics.Debug.WriteLine("D0: " + printBinary(d));

            for(int i = 0; i<keySchedule.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine("Shift key: " + i);
                for (int j = 0; j<keySchedule[i]; j++)
                {
                    c = LeftShift(c);
                    d = LeftShift(d);
                    //System.Diagnostics.Debug.WriteLine("C: " + printBinary(c));
                    //System.Diagnostics.Debug.WriteLine("D: " + printBinary(d));
                }
                //System.Diagnostics.Debug.WriteLine("Merged: " + printBinary(Merge(c, d)));
                keys[i] = Permute(PC2, Merge(c, d), 48);
                System.Diagnostics.Debug.WriteLine("Key " + i +": " + printBinary(keys[i]));

            }
            return keys;
        }

        public static BitArray LeftShift(BitArray arr)
        {
            bool temp = arr[0];
            for(int i = 0; i<arr.Length-1; i++)
            {
                arr[i] = arr[i + 1];
            }
            arr[arr.Length - 1] = temp;
            return arr;
        }

        public static BitArray CopySlice(BitArray source, int offset, int length)
        {
            BitArray results = new BitArray(length);
            for (int i = 0; i < length; i++)
            {
                results[i] = source[offset + i];
            }
            return results;
        }

        public static BitArray Merge(BitArray first, BitArray second)
        {
            BitArray bitArray = new BitArray(first.Length + second.Length);
            for(int i=0; i<first.Length; i++)
            {
                bitArray[i] = first[i];
            }
            for(int j = 0; j<second.Length;j++)
            {
                bitArray[first.Length + j] = second[j];
            }
            return bitArray;
        }
        public static BitArray fFunction(BitArray R, BitArray K)
        {
            if(R.Length!=32 || K.Length!=48)
            {
                throw new ArgumentException("Key should be 48b, R should be 32b");
            }
            BitArray result = new BitArray(32);

            BitArray temp = Permute(E, R, 48);
            //System.Diagnostics.Debug.WriteLine("After extend: " + DESMethods.printBinary(temp));
            temp.Xor(K);
            //System.Diagnostics.Debug.WriteLine("After xor: " + DESMethods.printBinary(temp));
            for (int i = 0; i<8; i++)
            {
                BitArray inputS = CopySlice(temp, 6*i, 6);
                //System.Diagnostics.Debug.WriteLine("S" + (i + 1).ToString() + ": " + DESMethods.printBinary(inputS));

                int[] coords = getSIndex(inputS);
                //System.Diagnostics.Debug.WriteLine(coords[0].ToString()+" "+ coords[1].ToString());
                int decimalValue = S[i, coords[0], coords[1]];
                //System.Diagnostics.Debug.WriteLine("VALUE: " + decimalValue.ToString());
                bool[] boolOutput = ConvertDecimalToFourBits(decimalValue);
                BitArray outputS = new BitArray(boolOutput);
                if (i==0)
                {
                    result = outputS;
                }
                else
                {
                    result = Merge(result, outputS);
                }
            }
            //System.Diagnostics.Debug.WriteLine("RESULT: " + DESMethods.printBinary(result));
            //System.Diagnostics.Debug.WriteLine("fFunction: " + DESMethods.printBinary(Permute(P, result, 32)));
            return Permute(P, result, 32);
        }
        public static bool[] ConvertDecimalToFourBits(int decimalValue)
        {
            if (decimalValue > 15)
                throw new Exception("Paremeter must be less than 15");
            BitArray bitArray = new BitArray(new int[] { decimalValue });
            List<bool> result = new List<bool>();
            for (int i = 0; i < bitArray.Length; i++)
            {
                result.Add(bitArray[i]);
            }
            return result.Take(4).Reverse().ToArray();
        }

        public static string printBinary(BitArray value)
        {
            StringBuilder sb = new StringBuilder();

            foreach (bool b in value)
            {
                if(b)
                {
                    sb.Append("1");
                }
                else
                {
                    sb.Append("0");
                }
            }
            return sb.ToString();
        }

    }
}
