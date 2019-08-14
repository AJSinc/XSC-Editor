using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

namespace XSCEditor
{

    class XSC
    {
        private string XSCLocation;
        private uint parameterVarCount;
        private int globalVarCount;
        private int staticVarCount;
        private string assmebly;

        //private long scriptSize;

        private byte[] XSCBytes;
        private List<string> stringTable;
        private List<string> nativesTable;
        private List<int> globalVariablesTable;
        private List<int> staticVariables;

        //saving XSC
        private List<byte> newXSCBytes;
        private List<byte> newCodeTable;
        private List<byte> newStaticVariablesTable;
        private List<string> newNativesTableNativeNames;
        private List<byte> newNativesTable;
        private List<string> newStringTableStrings;
        private List<byte> newStringTable;

        private int newCodeTableSize;
        private int newStaticVariablesTableSize;
        private int newNativesTableSize;
        private int newStringTableSize;
        private string newXSCName;
        private uint newParameterVarCount;

        private List<uint> newCodeTableStartBytePos = new List<uint>();
        private List<uint> newStringTableStartBytePos = new List<uint>();
        private int newStringTableTableCount;

        //code
        public XSC(string path)
        {
            globalVarCount = 0;
            staticVarCount = 0;
            XSCLocation = path;
            XSCBytes = File.ReadAllBytes(path);
            stringTable = new List<string>();
            nativesTable = new List<string>();
            staticVariables = new List<int>();
            if(XSCBytes[0] != 82 && XSCBytes[1] != 53 && XSCBytes[2] != 67 && XSCBytes[3] != 55)
            {
                List<byte> tmpXSCBytes = new List<byte>(XSCBytes);
                for (int i = 0; i < 16; i++)
                    tmpXSCBytes.Insert(0, 0);
                XSCBytes = tmpXSCBytes.ToArray();
            }
        }

        public XSC()
        {


        }

        private uint XSCParameterCount()
        {
            byte[] block = { XSCBytes[36], XSCBytes[37], XSCBytes[38], XSCBytes[39] };
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            return BitConverter.ToUInt32(block, 0);
        }

        public uint GetParameterCount()
        {
            return this.XSCParameterCount();
        }

        private List<string> XSCStringTable()
        {
            string currentString = "";
            int byteToConvert;
            uint pointer = XSCBytesToUInt32(XSCBytes, 85) + 16;
            uint stringTableStartByte = XSCBytesToUInt32(XSCBytes, (int)(pointer) + 1) + 16;
            uint stringTableSize = stringTableStartByte + XSCBytesToUInt32(XSCBytes, 89);
            if (stringTableSize == 0)
            {
                return stringTable;
            }

            for (uint i = stringTableStartByte; i < stringTableSize; i++)
            {
                if (XSCBytes[i] == 0)
                {
                    stringTable.Add(currentString);
                    currentString = "";
                    continue;
                }
                byteToConvert = XSCBytes[i];
                currentString += (char)byteToConvert;
            }
            return stringTable;
        }

        public List<string> GetStringTable()
        {
            return this.XSCStringTable();
        }

        private List<string> XSCNativesTable()
        {
            uint nativesTableStartByte = XSCBytesToUInt32(XSCBytes, 61) + 16;
            uint nativesTableSize = XSCBytesToUInt32(XSCBytes, 49);
            uint nativeHash = 0;
            string line;
            StreamReader file = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\data\\natives.dat");
            for (uint i = nativesTableStartByte; nativesTableSize > 0; i += 4)
            {
                if (BitConverter.IsLittleEndian)
                {
                    byte[] little = { XSCBytes[i + 3], XSCBytes[i + 2], XSCBytes[i + 1], XSCBytes[i] };
                    nativeHash = BitConverter.ToUInt32(little, 0);
                }
                else
                {
                    nativeHash = BitConverter.ToUInt32(XSCBytes, (int)i);
                }

                if (nativeHash == 0)
                {
                    nativesTable.Add("0x0");
                    nativesTableSize--;
                    continue;
                }

                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(nativeHash.ToString()))
                    {
                        //string j = nativeHash.ToString();
                        //string f = line.Substring(0, line.IndexOf("="));
                        if (nativeHash.ToString() == line.Substring(0, line.IndexOf("=")))
                        {
                            nativesTable.Add(line.Substring(line.IndexOf("=") + 1));
                            break;
                        }
                        
                    }
                }
                if (line == null)
                {
                    nativesTable.Add("0x" + nativeHash.ToString("X"));
                }
                file.DiscardBufferedData();
                file.BaseStream.Position = 0;
                nativeHash = 0;
                nativesTableSize--;
            }
            return nativesTable;
        }

        public List<string> GetNativesTable()
        {
            return this.XSCNativesTable();
        }

        private List<int> StaticVariablesTable()
        {
            uint staticTableStartByte = XSCBytesToUInt32(XSCBytes, 53) + 16;
            uint staticTableSize = staticTableStartByte + XSCBytesToUInt32(XSCBytes, 41) * 4;
            for (uint i = staticTableStartByte; i < staticTableSize; i += 4)
            {
                byte[] buffer = new byte[4];
                buffer[0] = XSCBytes[i];
                buffer[1] = XSCBytes[i + 1];
                buffer[2] = XSCBytes[i + 2];
                buffer[3] = XSCBytes[i + 3];
                if (BitConverter.IsLittleEndian)
                {
                    buffer = buffer.Reverse().ToArray();
                }
                int staticValue = BitConverter.ToInt32(buffer, 0);
                staticVariables.Add(staticValue);
            }
            return staticVariables;
        }

        public List<int> GetStaticVariablesTable()
        {
            return this.StaticVariablesTable();
        }

        private List<int> GlobalVariablesTable()
        {
            return this.globalVariablesTable;
        }

        public List<int> GetGlobalVariablesTable()
        {
            return this.GlobalVariablesTable();
        }

        private uint XSCBytesToUInt32(byte[] b, int pos)
        {
            byte[] buffer = new byte[4];
            buffer[0] = 0;
            buffer[1] = b[pos];
            buffer[2] = b[pos + 1];
            buffer[3] = b[pos + 2];
            if (BitConverter.IsLittleEndian)
            {
                buffer = buffer.Reverse().ToArray();
            }
            return BitConverter.ToUInt32(buffer, 0);
        }

        //all opcode added, need to figure out some parts of it though
        private string ConvertXSCtoASM()
        {
            string assembly = "";
            List<string> assemblyList = new List<string>();
            List<int> opCodeBytePostion = new List<int>();
            List<int> labelList = new List<int>();
            int labelByteIndex, label;
            byte[] buffer = { 0, 0 };
            uint pointer = XSCBytesToUInt32(XSCBytes, 25) + 16;
            uint codeTableStartByte = XSCBytesToUInt32(XSCBytes, (int)(pointer) + 1) + 16;
            uint codeTableSize = codeTableStartByte + XSCBytesToUInt32(XSCBytes, 33);
            int previousOpCode = 0, previousOpCodeByte;
            for (uint i = codeTableStartByte; i < codeTableSize; i++)
            {
                opCodeBytePostion.Add((int)i);
                if (labelList.Contains((int)i))
                {
                    label = labelList.IndexOf((int)i);
                    assemblyList.Add(Environment.NewLine + ":Label_" + (label + 1).ToString() + Environment.NewLine);
                    opCodeBytePostion.Add(-1);
                }
                previousOpCodeByte = (int)i;
                switch (XSCBytes[i])
                {
                    case 0:
                        assembly += "NOp";
                        break;
                    case 1:
                        assembly += "AddI";
                        break;
                    case 2:
                        assembly += "SubI";
                        break;
                    case 3:
                        assembly += "MultI";
                        break;
                    case 4:
                        assembly += "DivI";
                        break;
                    case 5:
                        assembly += "ModI"; //modulo
                        break;
                    case 6:
                        assembly += "Not";
                        break;
                    case 7:
                        assembly += "NegI";
                        break;
                    case 8:
                        assembly += "CmpEqI";
                        break;
                    case 9:
                        assembly += "CmpNeqI";
                        break;
                    case 10:
                        assembly += "CmpGtI";
                        break;
                    case 11:
                        assembly += "CmpGteI";
                        break;
                    case 12:
                        assembly += "CmpLtI";
                        break;
                    case 13:
                        assembly += "CmpLteI";
                        break;
                    case 14:
                        assembly += "AddF";
                        break;
                    case 15:
                        assembly += "SubF";
                        break;
                    case 16:
                        assembly += "MultF";
                        break;
                    case 17:
                        assembly += "DivF";
                        break;
                    case 18:
                        assembly += "ModF";
                        break;
                    case 19:
                        assembly += "NegF";
                        break;
                    case 20:
                        assembly += "CmpEqF";
                        break;
                    case 21:
                        assembly += "CmpNeF";
                        break;
                    case 22:
                        assembly += "CmpGtF";
                        break;
                    case 23:
                        assembly += "CmpGeF";
                        break;
                    case 24:
                        assembly += "CmpLtF";
                        break;
                    case 25:
                        assembly += "CmpLeF";
                        break;
                    case 26:
                        assembly += "AddV";
                        break;
                    case 27:
                        assembly += "SubV";
                        break;
                    case 28:
                        assembly += "MultV";
                        break;
                    case 29:
                        assembly += "DivV";
                        break;
                    case 30:
                        assembly += "NegV";
                        break;
                    case 31:
                        assembly += "And";
                        break;
                    case 32:
                        assembly += "Or";
                        break;
                    case 33:
                        assembly += "xor";
                        break;
                    case 34:
                        assembly += "itof";
                        break;
                    case 35:
                        assembly += "undef";
                        break;
                    case 36:
                        assembly += "dup2";
                        break;
                    case 37:
                        assembly += "PushB ";   //push byte?
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 38:
                        assembly += "PushBB ";
                        assembly += XSCBytes[i + 1].ToString() + " ";
                        assembly += XSCBytes[i + 2].ToString();
                        i += 2;
                        break;
                    case 39:
                        assembly += "PushBBB ";
                        assembly += XSCBytes[i + 1].ToString() + " ";
                        assembly += XSCBytes[i + 2].ToString() + " ";
                        assembly += XSCBytes[i + 3].ToString();
                        i += 3;
                        break;
                    case 40:
                        assembly += "PushI ";
                        if (BitConverter.IsLittleEndian)
                        {
                            byte[] bytesToConvert = { XSCBytes[i + 4], XSCBytes[i + 3], XSCBytes[i + 2], XSCBytes[i + 1] };
                            assembly += BitConverter.ToUInt32(bytesToConvert, 0).ToString();
                        }
                        else
                        {
                            assembly += BitConverter.ToUInt32(XSCBytes, (int)i + 1).ToString();
                        }
                        i += 4;
                        break;
                    case 41:
                        assembly += "PushF ";
                        if (BitConverter.IsLittleEndian)
                        {
                            byte[] bytesToConvert = { XSCBytes[i + 4], XSCBytes[i + 3], XSCBytes[i + 2], XSCBytes[i + 1] };
                            assembly += BitConverter.ToSingle(bytesToConvert, 0).ToString();
                        }
                        else
                        {
                            assembly += BitConverter.ToSingle(XSCBytes, (int)i + 1).ToString();
                        }
                        i += 4;
                        break;
                    case 42:
                        assembly += "Dup";
                        break;
                    case 43:
                        assembly += "Drop";
                        break;
                    case 44:
                        buffer[0] = XSCBytes[i + 2]; buffer[1] =  XSCBytes[i + 3];
                        if(BitConverter.IsLittleEndian)
                        {
                            buffer = buffer.Reverse().ToArray();
                        }
                        assembly += "CallNative ";
                        assembly += nativesTable[(int)XSCBytes[i + 3]] + " ";
                        //assembly += nativesTable[(int)(short)(int)BitConverter.ToInt16(buffer, 0)] + " "; //native index
                        assembly += (XSCBytes[i + 1] / 4).ToString() + " ";
                        assembly += (XSCBytes[i + 1] % 4).ToString();
                        i += 3;
                        break;
                    case 45:
                        assembly += "enter ";
                        assembly += XSCBytes[i + 1].ToString() + " ";
                        assembly += XSCBytes[i + 2].ToString() + " ";
                        assembly += XSCBytes[i + 3].ToString() + " ";
                        assembly += XSCBytes[i + 4].ToString();
                        i += 4;
                        break;
                    case 46:
                        assembly += "ret ";
                        assembly += XSCBytes[i + 1].ToString() + " ";
                        assembly += XSCBytes[i + 2].ToString();
                        i += 2;
                        break;
                    case 47:
                        assembly += "getp";
                        break;
                    case 48:
                        assembly += "setp";
                        break;
                    case 49:
                        assembly += "setpp";
                        break;
                    case 50:
                        assembly += "ArrayExplode";
                        break;
                    case 51:
                        assembly += "ArrayImplode";
                        break;
                    case 52:
                        assembly += "getarrayp ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 53:
                        assembly += "getarray ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 54:
                        assembly += "setarray ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 55:
                        assembly += "getlocalp ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 56:
                        assembly += "getlocal ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 57:
                        assembly += "setlocal ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 58:
                        assembly += "getstackp ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 59:
                        assembly += "getstack ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 60:
                        assembly += "setstack ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 61:
                        assembly += "addimb ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 62:
                        assembly += "mulimb ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 63:
                        assembly += "undef5";
                        break;
                    case 64:
                        assembly += "getarraypb ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 65:
                        assembly += "getarrayb ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 66:
                        assembly += "setarrayb ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 67:
                        assembly += "PushShort ";   //push short?
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 68:
                        assembly += "addims ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 69:
                        assembly += "mulims ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 70:
                        assembly += "shladds ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 71:
                        assembly += "nops ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 72:
                        assembly += "shladdpks ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 73:
                        assembly += "getarrayps ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 74:
                        assembly += "getarrays ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 75:
                        assembly += "setarrays ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 76:
                        assembly += "getlocalps ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 77:
                        assembly += "getlocals ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 78:
                        assembly += "setlocals ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 79:
                        assembly += "getstaticps ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 80:
                        assembly += "getstatics ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 81:
                        assembly += "setstatics ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 82:
                        assembly += "getglobalps ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 83:
                        assembly += "getglobals ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 84:
                        assembly += "setglobals ";
                        assembly += (XSCBytes[i + 1] * 256 + XSCBytes[i + 2]);
                        i += 2;
                        break;
                    case 85:
                        assembly += "Jump@Label_";
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }

                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }

                        i += 2;
                        break;
                    case 86:
                        assembly += "IsZero Jump@Label_";
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 87:
                        assembly += "IsNotEqual Jump@Label_";     //bne = IsNotEqual?
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 88:
                        assembly += "IsEqual Jump@Label_";      //IsEqual?
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 89:
                        assembly += "IsGreaterThan Jump@Label_";     //bgt
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 90:
                        assembly += "IsGreaterOrEqualTo Jump@Label_";     //bge
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 91:
                        assembly += "IsLessThan Jump@Label_";     //blt
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 92:
                        assembly += "IsLessThanOrEqualTo Jump@Label_";    //ble
                        buffer[0] = XSCBytes[i + 1];
                        buffer[1] = XSCBytes[i + 2];
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse((Array)buffer);
                        }
                        labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 3;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 2;
                        break;
                    case 93:
                        assembly += "Call@Label_";
                        labelByteIndex = XSCBytes[i + 1] * 4096 + XSCBytes[i + 2] * 256 + XSCBytes[i + 3] + (int) codeTableStartByte;
                        if (!labelList.Contains(labelByteIndex))
                        {
                            labelList.Add(labelByteIndex);
                            assembly += (labelList.Count()).ToString();
                            if (opCodeBytePostion.Contains(labelByteIndex))
                            {
                                assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))] = Environment.NewLine + ":Label_" + labelList.Count() + Environment.NewLine + assemblyList[(opCodeBytePostion.IndexOf(labelByteIndex))];
                            }
                        }
                        else
                        {
                            assembly += (labelList.IndexOf(labelByteIndex) + 1).ToString();
                        }
                        i += 3;
                        break;
                    case 94:
                        assembly += "getglobalpt ";
                        assembly += XSCBytesToUInt32(XSCBytes, (int)i + 1);
                        i += 3;
                        break;
                    case 95:
                        assembly += "getglobalt ";
                        assembly += XSCBytesToUInt32(XSCBytes, (int)i + 1);
                        i += 3;
                        break;
                    case 96:
                        assembly += "setglobalt ";
                        assembly += XSCBytesToUInt32(XSCBytes, (int)i + 1);
                        i += 3;
                        break;
                    case 97:
                        assembly += "Pusht ";
                        assembly += XSCBytesToUInt32(XSCBytes, (int)i + 1);
                        i += 3;
                        break;
                    case 98:
                        assembly += "Switch";
                        int switchSize = (int) i + 2 + XSCBytes[++i] * 6;
                        i += 1;
                        for (; i < switchSize; i += 6)
                        {
                            assembly += Environment.NewLine + "Case ";
                            assembly += (XSCBytes[i] * 65536 + XSCBytes[i + 1] * 4096 + 256 * XSCBytes[i + 2] + XSCBytes[i + 3]).ToString() + ":";
                            buffer[0] = XSCBytes[i + 4];
                            buffer[1] = XSCBytes[i + 5];
                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse((Array)buffer);
                            }
                            labelByteIndex = (int)(short)i + (int)(short)(int)BitConverter.ToInt16(buffer, 0) + 6;
                            if (!labelList.Contains(labelByteIndex))
                            {
                                labelList.Add(labelByteIndex);
                                assembly += "Jump@Label_" + (labelList.Count()).ToString();
                                if (opCodeBytePostion.Contains(labelByteIndex))
                                {
                                    assemblyList.Insert((opCodeBytePostion.IndexOf(labelByteIndex)), ":Label_" + labelList.Count() + Environment.NewLine);
                                    opCodeBytePostion.Insert(opCodeBytePostion.IndexOf(labelByteIndex), -1);
                                }
                            }
                            else
                            {
                                assembly += "Jump@Label_" + (labelList.IndexOf(labelByteIndex) + 1).ToString();
                            }
                        }
                        assembly += Environment.NewLine + "Default:";
                        i--;
                        break;
                    case 99:
                        int stringIndex = -1;
                        if (previousOpCode == 37)
                        {
                            stringIndex = XSCBytes[i - 1];
                        }
                        /*
                        else if (previousOpCode == 38)
                        {
                            ///NEED TO FIGURE OUT
                        }
                        
                        else if (previousOpCode == 39)
                        {
                            ///NEED TO FIGURE OUT
                        }
                        */
                        else if (previousOpCode == 67)
                        {
                            if (BitConverter.IsLittleEndian)
                            {
                                byte[] bytesToConvert = { XSCBytes[i - 1], XSCBytes[i - 2] };
                                stringIndex = BitConverter.ToInt16(bytesToConvert, 0);
                            }
                            else
                            {
                                stringIndex = BitConverter.ToInt16(XSCBytes, (int)i - 2);
                            }
                        }
                        else if (previousOpCode > 108 && previousOpCode < 118)
                        {
                            stringIndex = XSCBytes[i - 1] - 110;
                        }
                        
                        else
                        {
                            assemblyList[(assemblyList.Count - 1)] = "";
                            assembly += "PushString - Invalid previous Op for PushString - " + previousOpCode.ToString();
                        }

                        //actually get the string
                        if (stringIndex != -1)
                        {
                            int currentIndex = 0;
                            for (int k = 0; k < stringTable.Count; k++)
                            {
                                if (currentIndex != stringIndex)
                                {
                                    currentIndex += stringTable[k].Length + 1;
                                }
                                else
                                {
                                    assemblyList[(assemblyList.Count - 1)] = "";
                                    assembly += "PushString " + "\"" + stringTable[k] + "\"";
                                    break;
                                }
                            }
                            if (currentIndex != stringIndex)
                            {
                                assemblyList[(assemblyList.Count - 1)] = "";
                                assembly += "PushString " + "unfound string in string table at index: " + stringIndex.ToString();
                            }
                        }


                        break;
                    case 100:
                        assembly += "GetHash";
                        break;
                    case 101:
                        assembly += "StrCpy ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 102:
                        assembly += "itos ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 103:
                        assembly += "apps ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 104:
                        assembly += "appi ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 105:
                        assembly += "strcpy_arg ";
                        assembly += XSCBytes[i + 1].ToString();
                        i += 1;
                        break;
                    case 106:
                        assembly += "undef7";
                        break;
                    case 107:
                        assembly += "undef8";
                        break;
                    case 108:
                        assembly += "callp";
                        break;
                    case 255:
                        assembly += "bad";  //end script maybe?
                        break;
                    //default
                    default:
                        if (XSCBytes[i] > 108)
                        {
                            if (XSCBytes[i] < 118)
                            {
                                assembly += "iPush_" + (XSCBytes[i] - 110);
                            }
                            else if (XSCBytes[i] < 127)
                            {
                                assembly += "fPush_" + (XSCBytes[i] - 119);
                            }
                            else
                            {
                                assembly += "UNKNOWN OP-> " + XSCBytes[i];
                            }

                        }
                        break;
                }
                assembly += Environment.NewLine;
                assemblyList.Add(assembly);
                previousOpCode = XSCBytes[previousOpCodeByte];
                assembly = "";
            }
            assembly = string.Concat(assemblyList.ToArray());
            return assembly;
        }

        public string GetASM()
        {
            return ConvertXSCtoASM();
        }

        private string XSCName()
        {
            int XSCNameStartByte = XSCBytes[81] * 65536 + XSCBytes[82] * 256 + XSCBytes[83] + 16;
            int byteToConvert;
            string name = "";
            for (int i = XSCNameStartByte; i < XSCBytes.Length; i++)
            {
                if (XSCBytes[i] == 0) return name;
                byteToConvert = XSCBytes[i];
                name += (char)byteToConvert;
            }
            return name;
        }

        public string GetXSCName()
        {
            return XSCName();
        }

        private uint GetJenkinsHash(string inputString)
        {
            char[] p = inputString.ToCharArray();
            uint hash = 0;
            int i;
            for (i = 0; i < inputString.Length; i++)
            {
                hash += p[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

        static public int GetSizeFromFlag(uint flag, int baseSize)
        {
            baseSize <<= (int)(flag & 0xf);
            int size = (int)((((flag >> 17) & 0x7f) + (((flag >> 11) & 0x3f) << 1) + (((flag >> 7) & 0xf) << 2) + (((flag >> 5) & 0x3) << 3) + (((flag >> 4) & 0x1) << 4)) * baseSize);
            for (int i = 0; i < 4; ++i)
            {
                size += (((flag >> (24 + i)) & 1) == 1) ? (baseSize >> (1 + i)) : 0;
            }
            return size;
        }
        
        //saving
       //when writing be sure to make sure it is (num of bytes % 16 != 0) add 0's

        public string ConvertASMtoBytes(String XSCAsmString)
        {
            newCodeTable = new List<byte>();
            newNativesTableNativeNames = new List<string>();
            newStringTableStrings = new List<string>();
            List<int> labelReferenceBytePosition = new List<int>();
            List<int> labelNumberBytePosition = new List<int>();
            List<int> labelNumberList = new List<int>();
            StringReader XSCASM = new StringReader(XSCAsmString);
            //StreamReader nativesFile = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\data\\HashNatives.dat");
            int spaceCount = 0;
            int currentLineNumber = 0;
            int stringTableSize = 0;
            string currentLine = "";
            try
            {
                while ((currentLine = XSCASM.ReadLine()) != null)
                {
                    currentLineNumber++;
                    //may have to do PushString first because of Regex/Check for "
                    currentLine = currentLine.Trim();
                    if (!currentLine.Contains("\""))
                    {
                        currentLine = Regex.Replace(currentLine, @"\s+", " ");
                        spaceCount = currentLine.Count(x => x == ' ');
                    }
                    if (currentLine.StartsWith(":Label_"))
                    {
                        if (currentLine.Length > 7)
                        {
                            int sToi = Convert.ToInt32(currentLine.Substring(7, currentLine.Length - 7));
                            if (!labelNumberList.Contains(sToi))
                            {
                                labelNumberBytePosition.Add(newCodeTable.Count);
                                labelNumberList.Add(sToi);
                            }
                            else return "Error line " + currentLineNumber + " " + "Label " + sToi + " already exists";
                        }
                        else return "Error line " + currentLineNumber + " " + "Label needs a number";
                    }
                    else if (currentLine == "") continue;
                    //actual code
                    else if (currentLine.StartsWith("NOp"))
                    {
                        newCodeTable.Add(0);
                    }
                    else if (currentLine.StartsWith("AddI"))
                    {
                        newCodeTable.Add(1);
                    }
                    else if (currentLine.StartsWith("SubI"))
                    {
                        newCodeTable.Add(2);
                    }
                    else if (currentLine.StartsWith("MultI"))
                    {
                        newCodeTable.Add(3);
                    }
                    else if (currentLine.StartsWith("DivI"))
                    {
                        newCodeTable.Add(4);
                    }
                    else if (currentLine.StartsWith("ModI"))
                    {
                        newCodeTable.Add(5);
                    }
                    else if (currentLine.StartsWith("Not"))
                    {
                        newCodeTable.Add(6);
                    }
                    else if (currentLine.StartsWith("NegI"))
                    {
                        newCodeTable.Add(7);
                    }
                    else if (currentLine.StartsWith("CmpEqI"))
                    {
                        newCodeTable.Add(8);
                    }
                    else if (currentLine.StartsWith("CmpNeqI"))
                    {
                        newCodeTable.Add(9);
                    }
                    else if (currentLine.StartsWith("CmpGtI"))
                    {
                        newCodeTable.Add(10);
                    }
                    else if (currentLine.StartsWith("CmpGteI"))
                    {
                        newCodeTable.Add(11);
                    }
                    else if (currentLine.StartsWith("CmpLtI"))
                    {
                        newCodeTable.Add(12);
                    }
                    else if (currentLine.StartsWith("CmpLteI"))
                    {
                        newCodeTable.Add(13);
                    }
                    else if (currentLine.StartsWith("AddF"))
                    {
                        newCodeTable.Add(14);
                    }
                    else if (currentLine.StartsWith("SubF"))
                    {
                        newCodeTable.Add(15);
                    }
                    else if (currentLine.StartsWith("MultF"))
                    {
                        newCodeTable.Add(16);
                    }
                    else if (currentLine.StartsWith("DivF"))
                    {
                        newCodeTable.Add(17);
                    }
                    else if (currentLine.StartsWith("ModF"))
                    {
                        newCodeTable.Add(18);
                    }
                    else if (currentLine.StartsWith("NegF"))
                    {
                        newCodeTable.Add(19);
                    }
                    else if (currentLine.StartsWith("CmpEqF"))
                    {
                        newCodeTable.Add(20);
                    }
                    else if (currentLine.StartsWith("CmpNeF"))
                    {
                        newCodeTable.Add(21);
                    }
                    else if (currentLine.StartsWith("CmpGtF"))
                    {
                        newCodeTable.Add(22);
                    }
                    else if (currentLine.StartsWith("CmpGeF"))
                    {
                        newCodeTable.Add(23);
                    }
                    else if (currentLine.StartsWith("CmpLtF"))
                    {
                        newCodeTable.Add(24);
                    }
                    else if (currentLine.StartsWith("CmpLeF"))
                    {
                        newCodeTable.Add(25);
                    }
                    else if (currentLine.StartsWith("AddV"))
                    {
                        newCodeTable.Add(26);
                    }
                    else if (currentLine.StartsWith("SubV"))
                    {
                        newCodeTable.Add(27);
                    }
                    else if (currentLine.StartsWith("MultV"))
                    {
                        newCodeTable.Add(28);
                    }
                    else if (currentLine.StartsWith("DivV"))
                    {
                        newCodeTable.Add(29);
                    }
                    else if (currentLine.StartsWith("NegV"))
                    {
                        newCodeTable.Add(30);
                    }
                    else if (currentLine.StartsWith("And"))
                    {
                        newCodeTable.Add(31);
                    }
                    else if (currentLine.StartsWith("Or"))
                    {
                        newCodeTable.Add(32);
                    }
                    else if (currentLine.StartsWith("xor"))
                    {
                        newCodeTable.Add(33);
                    }
                    else if (currentLine.StartsWith("itof"))
                    {
                        newCodeTable.Add(34);
                    }
                    else if (currentLine.StartsWith("undef"))
                    {
                        newCodeTable.Add(35);
                    }
                    else if (currentLine.StartsWith("dup2"))
                    {
                        newCodeTable.Add(36);
                    }
                    else if (currentLine.StartsWith("PushB "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(37);
                            int sToi = Convert.ToInt32(currentLine.Substring(6, currentLine.Length - 6));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", PushB not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", PushB requires 1 value";
                    }
                    else if (currentLine.StartsWith("PushBB "))
                    {
                        if (spaceCount == 2)
                        {
                            newCodeTable.Add(38);
                            int lastSpaceIndex = 7;
                            currentLine += " ";
                            for (int i = 0; i < 2; i++)
                            {
                                int spaceIndex = currentLine.IndexOf(' ', lastSpaceIndex);
                                int sToi = Convert.ToInt32(currentLine.Substring(lastSpaceIndex, spaceIndex - lastSpaceIndex));
                                if (sToi < 256 && sToi >= 0)
                                {
                                    byte[] convert = BitConverter.GetBytes(sToi);
                                    newCodeTable.Add(convert[0]);
                                }
                                else
                                {
                                    return "Error at line " + currentLineNumber + ", PushBB not within range 0 - 255";
                                }
                                lastSpaceIndex = currentLine.IndexOf(' ', lastSpaceIndex) + 1;
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", PushBB requires 2 values";
                    }
                    else if (currentLine.StartsWith("PushBBB "))
                    {
                        if (spaceCount == 3)
                        {
                            newCodeTable.Add(39);
                            int lastSpaceIndex = 8;
                            currentLine += " ";
                            for (int i = 0; i < 3; i++)
                            {
                                int sToi = Convert.ToInt32(currentLine.Substring(lastSpaceIndex, currentLine.IndexOf(' ', lastSpaceIndex) - lastSpaceIndex));
                                if (sToi < 256 && sToi >= 0)
                                {
                                    byte[] convert = BitConverter.GetBytes(sToi);
                                    newCodeTable.Add(convert[0]);
                                }
                                else
                                {
                                    return "Error at line " + currentLineNumber + ", PushBBB not within range 0 - 255";
                                }
                                lastSpaceIndex = currentLine.IndexOf(' ', lastSpaceIndex) + 1;
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", PushBBB requires 3 values";
                    }
                    else if (currentLine.StartsWith("PushI "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(40);
                            uint sToi = Convert.ToUInt32(currentLine.Substring(6, currentLine.Length - 6));
                            if (sToi <= uint.MaxValue && 0 <= sToi)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(convert[0]);
                                newCodeTable.Add(convert[1]);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                            else return "Error at line " + currentLineNumber + ", PushI not within range 0 - " + uint.MaxValue;
                        }
                        else return "Error at line " + currentLineNumber + ", PushI requires 1 value";

                    }
                    else if (currentLine.StartsWith("PushF "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(41);
                            float sTof = Convert.ToSingle(currentLine.Substring(6, currentLine.Length - 6));
                            if (sTof <= float.MaxValue && sTof >= float.MinValue)
                            {
                                byte[] convert = BitConverter.GetBytes(sTof);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(convert[0]);
                                newCodeTable.Add(convert[1]);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", PushF requires 1 value";
                    }
                    else if (currentLine.StartsWith("Dup"))
                    {
                        newCodeTable.Add(42);
                    }
                    else if (currentLine.StartsWith("Drop"))
                    {
                        newCodeTable.Add(43);
                    }
                    else if (currentLine.StartsWith("CallNative ")) ///FIX
                    {
                        if (spaceCount == 3)
                        {
                            newCodeTable.Add(44);
                            currentLine += " ";
                            string nativeName = currentLine.Substring(11, currentLine.IndexOf(' ', 11) - 11);
                            int lastSpaceIndex = nativeName.Length + 11 + 1;
                            int saved = 0;
                            for (int i = 0; i < 2; i++)
                            {
                                string str = currentLine.Substring(lastSpaceIndex, currentLine.IndexOf(' ', lastSpaceIndex) - lastSpaceIndex);
                                int sToi = Convert.ToInt32(currentLine.Substring(lastSpaceIndex, currentLine.IndexOf(' ', lastSpaceIndex) - lastSpaceIndex));
                                if (sToi < 256 && sToi >= 0)
                                {
                                    if (i == 0)
                                    {
                                        saved = sToi * 4;
                                    }
                                    else
                                    {
                                        saved += sToi;
                                        byte[] convert = BitConverter.GetBytes(saved);
                                        newCodeTable.Add(convert[0]);
                                    }
                                }
                                else
                                {
                                    return "Error at line " + currentLineNumber + ", CallNative not within range 0 - 255";
                                }
                                lastSpaceIndex = currentLine.IndexOf(' ', lastSpaceIndex) + 1;
                            }
                            newCodeTable.Add(0);    //gonna guess this is if the native table > 255 natives?
                            if (!newNativesTableNativeNames.Contains(nativeName))
                            {
                                newNativesTableNativeNames.Add(nativeName);
                                int nativeIndex = newNativesTableNativeNames.Count - 1;
                                byte[] convert = BitConverter.GetBytes(nativeIndex);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                byte[] convert = BitConverter.GetBytes(newNativesTableNativeNames.IndexOf(nativeName));
                                newCodeTable.Add(convert[0]);
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", CallNative requires 3 values";
                    }
                    else if (currentLine.StartsWith("enter "))
                    {
                        if (spaceCount == 4)
                        {
                            newCodeTable.Add(45);
                            currentLine += " ";
                            int lastSpaceIndex = 6;
                            for (int i = 0; i < 4; i++)
                            {
                                int sToi = Convert.ToInt32(currentLine.Substring(lastSpaceIndex, currentLine.IndexOf(' ', lastSpaceIndex) - lastSpaceIndex));
                                if (sToi < 256 && sToi >= 0)
                                {
                                    byte[] convert = BitConverter.GetBytes(sToi);
                                    newCodeTable.Add(convert[0]);
                                }
                                else
                                {
                                    return "Error at line " + currentLineNumber + ", enter not within range 0 - 255";
                                }
                                lastSpaceIndex = currentLine.IndexOf(' ', lastSpaceIndex) + 1;
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", enter requires 4 values";
                    }
                    else if (currentLine.StartsWith("ret "))
                    {
                        if (spaceCount == 2)
                        {
                            newCodeTable.Add(46);
                            currentLine += " ";
                            int lastSpaceIndex = 4;
                            for (int i = 0; i < 2; i++)
                            {
                                int sToi = Convert.ToInt32(currentLine.Substring(lastSpaceIndex, currentLine.IndexOf(' ', lastSpaceIndex) - lastSpaceIndex));
                                if (sToi < 256 && sToi >= 0)
                                {
                                    byte[] convert = BitConverter.GetBytes(sToi);
                                    newCodeTable.Add(convert[0]);
                                }
                                else
                                {
                                    return "Error at line " + currentLineNumber + ", ret not within range 0 - 255";
                                }
                                lastSpaceIndex = currentLine.IndexOf(' ', lastSpaceIndex) + 1;
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", ret requires 2 values";
                    }
                    else if (currentLine.StartsWith("getp"))
                    {
                        newCodeTable.Add(47);
                    }
                    else if (currentLine.StartsWith("setpp"))   //have to do setpp before setp otherwise it will think setpp is setp
                    {
                        newCodeTable.Add(49);
                    }
                    else if (currentLine.StartsWith("setp"))
                    {
                        newCodeTable.Add(48);
                    }
                    else if (currentLine.StartsWith("ArrayExplode"))
                    {
                        newCodeTable.Add(50);
                    }
                    else if (currentLine.StartsWith("ArrayImplode"))
                    {
                        newCodeTable.Add(51);
                    }
                    else if (currentLine.StartsWith("getarrayp "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(52);
                            int sToi = Convert.ToInt32(currentLine.Substring(10, currentLine.Length - 10));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getarrayp not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getarrayp requires 1 value";
                    }
                    else if (currentLine.StartsWith("getarray "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(53);
                            int sToi = Convert.ToInt32(currentLine.Substring(9, currentLine.Length - 9));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getarray not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getarray requires 1 value";
                    }
                    else if (currentLine.StartsWith("setarray "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(54);
                            int sToi = Convert.ToInt32(currentLine.Substring(9, currentLine.Length - 9));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", setarray not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", setarray requires 1 value";
                    }
                    else if (currentLine.StartsWith("getlocalp "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(55);
                            int sToi = Convert.ToInt32(currentLine.Substring(10, currentLine.Length - 10));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getlocalp not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getlocalp requires 1 value";
                    }
                    else if (currentLine.StartsWith("getlocal "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(56);
                            int sToi = Convert.ToInt32(currentLine.Substring(9, currentLine.Length - 9));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getlocal not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getlocal requires 1 value";
                    }
                    else if (currentLine.StartsWith("setlocal "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(57);
                            int sToi = Convert.ToInt32(currentLine.Substring(9, currentLine.Length - 9));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", setlocal not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", setlocal requires 1 value";
                    }
                    else if (currentLine.StartsWith("getstackp "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(58);
                            int sToi = Convert.ToInt32(currentLine.Substring(10, currentLine.Length - 10));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getstackp not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getstackp requires 1 value";
                    }
                    else if (currentLine.StartsWith("getstack "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(59);
                            int sToi = Convert.ToInt32(currentLine.Substring(9, currentLine.Length - 9));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getstack not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getstack requires 1 value";
                    }
                    else if (currentLine.StartsWith("setstack "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(60);
                            int sToi = Convert.ToInt32(currentLine.Substring(9, currentLine.Length - 9));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", setstack not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", setstack requires 1 value";
                    }
                    else if (currentLine.StartsWith("addimb "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(61);
                            int sToi = Convert.ToInt32(currentLine.Substring(7, currentLine.Length - 7));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", addimb not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", addimb requires 1 value";
                    }
                    else if (currentLine.StartsWith("mulimb "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(62);
                            int sToi = Convert.ToInt32(currentLine.Substring(7, currentLine.Length - 7));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", mulimb not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", mulimb requires 1 value";
                    }
                    else if (currentLine.StartsWith("undef5"))
                    {
                        newCodeTable.Add(63);
                    }
                    else if (currentLine.StartsWith("getarraypb "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(64);
                            int sToi = Convert.ToInt32(currentLine.Substring(11, currentLine.Length - 11));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getarraypb not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getarraypb requires 1 value";
                    }
                    else if (currentLine.StartsWith("getarrayb "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(65);
                            int sToi = Convert.ToInt32(currentLine.Substring(10, currentLine.Length - 10));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", getarrayb not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", getarrayb requires 1 value";
                    }
                    else if (currentLine.StartsWith("setarrayb "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(66);
                            int sToi = Convert.ToInt32(currentLine.Substring(10, currentLine.Length - 10));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", setarrayb not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", setarrayb requires 1 value";
                    }
                    else if (currentLine.StartsWith("PushShort "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(67);
                            if (ConvertASMToInt16(currentLine, "PushShort ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "PushShort ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", PushShort requires 1 value";
                    }
                    else if (currentLine.StartsWith("addims "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(68);
                            if (ConvertASMToInt16(currentLine, "addims ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "addims ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", addims requires 1 value";
                    }
                    else if (currentLine.StartsWith("mulims "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(69);
                            if (ConvertASMToInt16(currentLine, "mulims ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "mulims ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", mulims requires 1 value";
                    }
                    else if (currentLine.StartsWith("shladds "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(70);
                            if (ConvertASMToInt16(currentLine, "shladds ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "shladds ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", shladds requires 1 value";
                    }
                    else if (currentLine.StartsWith("nops "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(71);
                            if (ConvertASMToInt16(currentLine, "nops ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "nops ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", nops requires 1 value";
                    }
                    else if (currentLine.StartsWith("shladdpks "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(72);
                            if (ConvertASMToInt16(currentLine, "shladdpks ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "shladdpks ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", shladdpks requires 1 value";
                    }
                    else if (currentLine.StartsWith("getarrayps "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(73);
                            if (ConvertASMToInt16(currentLine, "getarrayps ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getarrayps ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getarrayps requires 1 value";
                    }
                    else if (currentLine.StartsWith("getarrays "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(74);
                            if (ConvertASMToInt16(currentLine, "getarrays ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getarrays ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getarrays requires 1 value";
                    }
                    else if (currentLine.StartsWith("setarrays "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(75);
                            if (ConvertASMToInt16(currentLine, "setarrays ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "setarrays ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", setarrays requires 1 value";
                    }
                    else if (currentLine.StartsWith("getlocalps "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(76);
                            if (ConvertASMToInt16(currentLine, "getlocalps ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getlocalps ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getlocalps requires 1 value";
                    }
                    else if (currentLine.StartsWith("getlocals "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(77);
                            if (ConvertASMToInt16(currentLine, "getlocals ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getlocals ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getlocals requires 1 value";
                    }
                    else if (currentLine.StartsWith("setlocals "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(78);
                            if (ConvertASMToInt16(currentLine, "setlocals ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "setlocals ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", setlocals requires 1 value";
                    }
                    else if (currentLine.StartsWith("getstaticps "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(79);
                            if (ConvertASMToInt16(currentLine, "getstaticps ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getstaticps ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getstaticps requires 1 value";
                    }
                    else if (currentLine.StartsWith("getstatics "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(80);
                            if (ConvertASMToInt16(currentLine, "getstatics ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getstatics ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getstatics requires 1 value";
                    }
                    else if (currentLine.StartsWith("setstatics "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(81);
                            if (ConvertASMToInt16(currentLine, "setstatics ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "setstatics ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", setstatics requires 1 value";
                    }
                    else if (currentLine.StartsWith("getglobalps "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(82);
                            if (ConvertASMToInt16(currentLine, "getglobalps ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getglobalps ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getglobalps requires 1 value";
                    }
                    else if (currentLine.StartsWith("getglobals "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(83);
                            if (ConvertASMToInt16(currentLine, "getglobals ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "getglobals ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", getglobals requires 1 value";
                    }
                    else if (currentLine.StartsWith("setglobals "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(84);
                            if (ConvertASMToInt16(currentLine, "setglobals ", currentLineNumber) != "Success")
                            {
                                return ConvertASMToInt16(currentLine, "setglobals ", currentLineNumber);
                            }

                        }
                        else return "Error at line " + currentLineNumber + ", setglobals requires 1 value";
                    }
                    else if (currentLine.StartsWith("Jump@Label_"))
                    {
                        if (spaceCount == 0 && currentLine.Length > 11)
                        {
                            newCodeTable.Add(85);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsZero Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 18)
                        {
                            newCodeTable.Add(86);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsZero Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsNotEqual Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 22)
                        {
                            newCodeTable.Add(87);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsNotEqual Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsEqual Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 19)
                        {
                            newCodeTable.Add(88);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsEqual Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsGreaterThan Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 25)
                        {
                            newCodeTable.Add(89);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsGreaterThan Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsGreaterOrEqualTo Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 30)
                        {
                            newCodeTable.Add(90);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsGreaterOrEqualTo Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsLessThan Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 22)
                        {
                            newCodeTable.Add(91);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsLessThan Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("IsLessThanOrEqualTo Jump@Label_"))
                    {
                        if (spaceCount == 1 && currentLine.Length > 31)
                        {
                            newCodeTable.Add(92);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //2 filler spots for bytes
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " IsLessThanOrEqualTo Jump requires a Label_#";
                    }
                    else if (currentLine.StartsWith("Call@Label_"))
                    {
                        if (spaceCount == 0 && currentLine.Length > 11)
                        {
                            newCodeTable.Add(93);
                            labelReferenceBytePosition.Add(newCodeTable.Count);
                            newCodeTable.Add(0);   //3 filler spots for bytes
                            newCodeTable.Add(0);
                            newCodeTable.Add(0);
                        }
                        else return "Error at line " + currentLineNumber + " Call Label requires a Label_#";
                    }
                    else if (currentLine.StartsWith("getglobalpt "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(94);
                            uint sToi = Convert.ToUInt32(currentLine.Substring(12, currentLine.Length - 12));
                            if (sToi < uint.MaxValue && 0 <= sToi)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(convert[1]);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                            else return "Error at line " + currentLineNumber + ", getglobalpt not within range 0 - " + uint.MaxValue;
                        }
                        else return "Error at line " + currentLineNumber + " getglobalpt requires a value";
                    }
                    else if (currentLine.StartsWith("getglobalt "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(95);
                            uint sToi = Convert.ToUInt32(currentLine.Substring(10, currentLine.Length - 10));
                            if (sToi < uint.MaxValue && 0 <= sToi)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(convert[1]);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                            else return "Error at line " + currentLineNumber + ", getglobalt not within range 0 - " + uint.MaxValue;
                        }
                        else return "Error at line " + currentLineNumber + " getglobalt requires a value";
                    }
                    else if (currentLine.StartsWith("setglobalt ")) /////////FIX
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(96);
                            uint sToi = Convert.ToUInt32(currentLine.Substring(11, currentLine.Length - 11));
                            if (sToi < uint.MaxValue && 0 <= sToi)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(convert[1]);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                            else return "Error at line " + currentLineNumber + ", setglobalt not within range 0 - " + uint.MaxValue;
                        }
                        else return "Error at line " + currentLineNumber + " setglobalt requires a value";
                    }
                    else if (currentLine.StartsWith("Pusht "))  /////////FIX
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(97);
                            uint sToi = Convert.ToUInt32(currentLine.Substring(6, currentLine.Length - 6));
                            if (sToi < uint.MaxValue && 0 <= sToi)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                //newCodeTable.Add(convert[0]);
                                newCodeTable.Add(convert[1]);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                            else return "Error at line " + currentLineNumber + ", Pusht not within range 0 - " + uint.MaxValue;
                        }
                        else return "Error at line " + currentLineNumber + " Pusht requires a value";
                    }
                    else if (currentLine.StartsWith("Switch"))
                    {
                        if (spaceCount == 0)
                        {
                            int switchSizeIndex = 0;
                            int switchSize = 0;
                            newCodeTable.Add(98);
                            byte[] convert;
                            switchSizeIndex = newCodeTable.Count;
                            newCodeTable.Add(0); //switch size;
                            while ((currentLine = XSCASM.ReadLine()) != null) //&& currentLine.StartsWith("Case ")) //&& currentLine.Contains("Jump@Label_"))
                            {
                                currentLineNumber++;
                                currentLine = currentLine.Trim();
                                currentLine = Regex.Replace(currentLine, @"\s+", " ");
                                spaceCount = currentLine.Count(x => x == ' ');
                                if (spaceCount == 1 && currentLine.StartsWith("Case ")) //&& currentLine.Contains(":") && currentLine.Contains("Jump@Label_"))
                                {
                                    int sToi = Convert.ToInt32(currentLine.Substring(5, currentLine.IndexOf(":") - 5));
                                    convert = BitConverter.GetBytes(sToi);
                                    if (BitConverter.IsLittleEndian)
                                    {
                                        convert = convert.Reverse().ToArray();
                                    }
                                    newCodeTable.Add(convert[0]);
                                    newCodeTable.Add(convert[1]);
                                    newCodeTable.Add(convert[2]);
                                    newCodeTable.Add(convert[3]);
                                    //label to jump to
                                    labelReferenceBytePosition.Add(newCodeTable.Count);
                                    newCodeTable.Add(0);   //2 filler spots for bytes
                                    newCodeTable.Add(0);
                                }
                                else if (currentLine == "") 
                                {
                                    continue;
                                }
                                else break;
                                switchSize++;
                            }
                            if (switchSize == 0)
                            {
                                return "Error at line " + currentLineNumber + " at least one case expected in Switch";
                            }
                            convert = BitConverter.GetBytes(switchSize);
                            if (BitConverter.IsLittleEndian)
                            {
                                convert = convert.Reverse().ToArray();
                            }
                            newCodeTable[switchSizeIndex] = convert[3];
                        }
                        else return "Error at line " + currentLineNumber + " bad Switch";
                    }
                    else if (currentLine.StartsWith("PushString "))
                    {
                        string pushedString = "";
                        int stringIndex = 0;
                        int quoteCount = currentLine.Count(x => x == '\"');
                        if (quoteCount % 2 == 0 && quoteCount != 0)
                        {
                            pushedString = currentLine.Substring(currentLine.IndexOf("\"") + 1);
                            pushedString = pushedString.Substring(0, pushedString.Length - 1);
                            if (newStringTableStrings.Contains(pushedString))
                            {
                                int stringTableIndex = newStringTableStrings.IndexOf(pushedString);
                                for (int i = 0; i < stringTableIndex; i++)
                                {
                                    stringIndex += newStringTableStrings[i].Length + 1;
                                }
                            }
                            else
                            {
                                newStringTableStrings.Add(pushedString);
                                stringIndex = stringTableSize;
                                stringTableSize += pushedString.Length + 1;
                            }
                            if (stringIndex < 8)
                            {
                                stringIndex += 110;
                                byte[] convert = BitConverter.GetBytes(stringIndex);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(convert[3]);
                            }
                            else if (stringIndex < 256)
                            {
                                byte[] convert = BitConverter.GetBytes(stringIndex);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(37);
                                newCodeTable.Add(convert[3]);
                            }
                            else
                            {
                                byte[] convert = BitConverter.GetBytes(stringIndex);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable.Add(67);
                                newCodeTable.Add(convert[2]);
                                newCodeTable.Add(convert[3]);
                            }
                            newCodeTable.Add(99);
                        }
                        else return "Error at line " + currentLineNumber + " invalid string/missing ' \" ' (quotation mark)";
                    }
                    else if (currentLine.StartsWith("GetHash"))
                    {
                        newCodeTable.Add(100);
                    }
                    else if (currentLine.StartsWith("StrCpy "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(101);
                            int sToi = Convert.ToInt32(currentLine.Substring(7, currentLine.Length - 7));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", StrCpy not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", StrCpy requires 1 value";
                    }
                    else if (currentLine.StartsWith("itos "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(102);
                            int sToi = Convert.ToInt32(currentLine.Substring(5, currentLine.Length - 5));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", itos not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", itos requires 1 value";
                    }
                    else if (currentLine.StartsWith("apps "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(103);
                            int sToi = Convert.ToInt32(currentLine.Substring(5, currentLine.Length - 5));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", apps not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", apps requires 1 value";
                    }
                    else if (currentLine.StartsWith("appi "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(104);
                            int sToi = Convert.ToInt32(currentLine.Substring(5, currentLine.Length - 5));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", appi not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", appi requires 1 value";
                    }
                    else if (currentLine.StartsWith("strcpy_arg "))
                    {
                        if (spaceCount == 1)
                        {
                            newCodeTable.Add(105);
                            int sToi = Convert.ToInt32(currentLine.Substring(11, currentLine.Length - 11));
                            if (sToi < 256 && sToi >= 0)
                            {
                                byte[] convert = BitConverter.GetBytes(sToi);
                                newCodeTable.Add(convert[0]);
                            }
                            else
                            {
                                return "Error at line " + currentLineNumber + ", strcpy_arg not within range 0 - 255";
                            }
                        }
                        else return "Error at line " + currentLineNumber + ", strcpy_arg requires 1 value";
                    }
                    else if (currentLine.StartsWith("undef7"))
                    {
                        newCodeTable.Add(106);
                    }
                    else if (currentLine.StartsWith("undef8"))
                    {
                        newCodeTable.Add(107);
                    }
                    else if (currentLine.StartsWith("callp"))
                    {
                        newCodeTable.Add(108);
                    }
                    else if (currentLine.StartsWith("iPush_"))
                    {
                        int sToi = Convert.ToInt32(currentLine.Substring(6, currentLine.Length - 6));
                        if (sToi < 8 && sToi > -2)
                        {
                            sToi += 110;
                            byte[] convert = BitConverter.GetBytes(sToi);
                            if (BitConverter.IsLittleEndian)
                            {
                                convert = convert.Reverse().ToArray();
                            }
                            newCodeTable.Add(convert[3]);
                        }
                        else return "Error at line " + currentLineNumber + ", iPush_ can only be between (-1) -> 7";
                    }
                    else if (currentLine.StartsWith("fPush_"))
                    {
                        int sToi = Convert.ToInt32(currentLine.Substring(6, currentLine.Length - 6));
                        if (sToi < 8 && sToi > -2)
                        {
                            sToi += 119;
                            byte[] convert = BitConverter.GetBytes(sToi);
                            if (BitConverter.IsLittleEndian)
                            {
                                convert = convert.Reverse().ToArray();
                            }
                            newCodeTable.Add(convert[3]);
                        }
                        else return "Error at line " + currentLineNumber + ", fPush_ can only be between (-1) -> 7";
                    }
                    else if (currentLine.StartsWith("bad"))
                    {
                        newCodeTable.Add(255);
                    }
                    else
                    {
                        return "Error at line " + currentLineNumber + ", unknown OP";
                    }
                }
            }
            catch(Exception)
            {
                return "Error at line " + currentLineNumber;
            }
            XSCASM = new StringReader(XSCAsmString);
            currentLineNumber = 0;
            int currentLabel = 0;
            //do labels
            try
            {
                while ((currentLine = XSCASM.ReadLine()) != null)
                {
                    currentLineNumber++;
                    currentLine = currentLine.Trim();
                    currentLine = Regex.Replace(currentLine, @"\s+", " ");
                    if (!currentLine.StartsWith("PushString "))
                    {
                        if (currentLine.Contains("Jump@Label_"))
                        {
                            int labelIndex = currentLine.IndexOf("Jump@Label_") + 11;
                            labelIndex = Convert.ToInt32(currentLine.Substring(labelIndex, currentLine.Length - labelIndex));
                            if (labelNumberList.Contains(labelIndex))
                            {
                                int labelBytePos = labelNumberList.IndexOf(labelIndex);
                                labelBytePos = labelNumberBytePosition[labelBytePos];
                                int labelSkip = labelBytePos - labelReferenceBytePosition[currentLabel] - 2;
                                byte[] convert = BitConverter.GetBytes(labelSkip);
                                if (BitConverter.IsLittleEndian)
                                {
                                    convert = convert.Reverse().ToArray();
                                }
                                newCodeTable[(labelReferenceBytePosition[currentLabel])] = convert[2];
                                newCodeTable[(labelReferenceBytePosition[currentLabel] + 1)] = convert[3];
                                currentLabel++;
                            }
                            else return "Error at line " + currentLineNumber + ", no Label with index number " + labelIndex;
                        }
                        else if (currentLine.StartsWith("Call@Label_"))
                        {
                            if (currentLine.Length > 11)
                            {
                                int labelIndex = Convert.ToInt32(currentLine.Substring(11, currentLine.Length - 11));
                                if (labelNumberList.Contains(labelIndex))
                                {
                                    int labelBytePos = labelNumberList.IndexOf(labelIndex);
                                    labelBytePos = labelNumberBytePosition[labelBytePos];
                                    byte[] convert = BitConverter.GetBytes(labelBytePos);
                                    if (BitConverter.IsLittleEndian)
                                    {
                                        convert = convert.Reverse().ToArray();
                                    }
                                    newCodeTable[(labelReferenceBytePosition[currentLabel])] = convert[1];
                                    newCodeTable[(labelReferenceBytePosition[currentLabel] + 1)] = convert[2];
                                    newCodeTable[(labelReferenceBytePosition[currentLabel] + 2)] = convert[3];
                                    currentLabel++;
                                }
                                else return "Error at line " + currentLineNumber + ", no Label to Call with index number " + labelIndex;
                            }

                        }
                    }

                }
            }
            catch(Exception)
            {
                return "Error at line " + currentLineNumber;
            }


            newCodeTableSize = newCodeTable.Count;
            //add
            newCodeTable.Add(0);
            while (newCodeTable.Count % 16 != 0)
            {
                newCodeTable.Add(0);
            }

            return "ASM converted successfully";
        }

        private string ConvertASMToInt16(string strM, string str, int lineNumber)
        {
            ushort sToi = Convert.ToUInt16(strM.Substring(str.Length, strM.Length - str.Length));
            if (sToi < UInt16.MaxValue && 0 <= sToi)
            {
                byte[] convert = BitConverter.GetBytes(sToi);
                if (BitConverter.IsLittleEndian)
                {
                    convert = convert.Reverse().ToArray();
                }
                newCodeTable.Add(convert[0]);
                newCodeTable.Add(convert[1]);
                return "Success";
            }
            else return "Error at line " + lineNumber + ", " + str + "not within range 0 - " + UInt16.MaxValue;
        }

        public void ConvertNewStaticVariablesTable(List<int> staticVarList)
        {
            newStaticVariablesTable = new List<byte>();
            byte[] convert = new byte[4];
            newStaticVariablesTableSize = staticVarList.Count;
            for (int i = 0; i < staticVarList.Count; i++)
            {
                convert = BitConverter.GetBytes(staticVarList[i]);
                if (BitConverter.IsLittleEndian)
                {
                    convert = convert.Reverse().ToArray();
                }
                newStaticVariablesTable.Add(convert[0]);
                newStaticVariablesTable.Add(convert[1]);
                newStaticVariablesTable.Add(convert[2]);
                newStaticVariablesTable.Add(convert[3]);
            }

            //size already calculated
            newStaticVariablesTable.Add(0);
            while(newStaticVariablesTable.Count % 16 != 0)
            {
                newStaticVariablesTable.Add(0);
            }
        }

        public string ConvertNewNativeTableBytes()
        {
            StreamReader file = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\data\\natives.dat");
            string line = "";
            string lineNativeName = "";
            string nativeName = "";
            newNativesTable = new List<byte>();
            newNativesTableSize = newNativesTableNativeNames.Count;
            for (int i = 0; i < newNativesTableNativeNames.Count; i++)
            {
                nativeName = newNativesTableNativeNames[i];
                if (newNativesTableNativeNames[i].StartsWith("0x"))
                {
                    if (nativeName.Length > 2)
                    {
                        nativeName = newNativesTableNativeNames[i].Substring(2);
                        uint value = Convert.ToUInt32(nativeName, 16);
                        byte[] convert = BitConverter.GetBytes(value);
                        if (BitConverter.IsLittleEndian)
                        {
                            convert = convert.Reverse().ToArray();
                        }
                        newNativesTable.Add(convert[0]);
                        newNativesTable.Add(convert[1]);
                        newNativesTable.Add(convert[2]);
                        newNativesTable.Add(convert[3]);
                        continue;
                    }
                    else return "Invalid hex value for native: " + newNativesTableNativeNames[i];
                }
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(nativeName))
                    {
                        lineNativeName = line.Substring(line.IndexOf("=") + 1);
                        if (lineNativeName == nativeName)
                        {
                            uint value = Convert.ToUInt32(line.Substring(0, line.IndexOf("=")));
                            byte[] convert = BitConverter.GetBytes(value);
                            if (BitConverter.IsLittleEndian)
                            {
                                convert = convert.Reverse().ToArray();
                            }
                            newNativesTable.Add(convert[0]);
                            newNativesTable.Add(convert[1]);
                            newNativesTable.Add(convert[2]);
                            newNativesTable.Add(convert[3]);
                            file.DiscardBufferedData();
                            file.BaseStream.Position = 0;
                            break;
                        }
                        else continue;
                    }
                }
                if (line == null)
                {
                    return "Invalid Native: " + newNativesTableNativeNames[i];
                }
            }

            //add
            newNativesTable.Add(0);
            while (newNativesTable.Count % 16 != 0)
            {
                newNativesTable.Add(0);
            }
            return "Natives Table converted successfully";
        }

        public void ConvertNewStringTable()
        {
            newStringTableTableCount = 0;
            newStringTableStartBytePos.Add(0);
            string stringTable = "";
            for (int i = 0; i < newStringTableStrings.Count; i++)
            {
                if (stringTable.Length + newStringTableStrings[i].Length > 16384 * (newStringTableTableCount + 1))
                {
                    newStringTableTableCount++;
                    while (newStringTable.Count % 16384 != 0)
                    {
                        newStringTable.Add(0);
                    }
                    stringTable += newStringTableStrings[i];
                }
                else // it might be that there has to be a null character after the last string!
                {
                    stringTable += newStringTableStrings[i];
                    if (newStringTable == null || newStringTable.Count % 16384 != 0)
                    {
                        stringTable += "\0";
                    }
                }
            }
            byte[] StringTableBytes = Encoding.ASCII.GetBytes(stringTable);
            newStringTable = StringTableBytes.ToList();
            newStringTableSize = stringTable.Length;
            //add
            newStringTable.Add(0);
            while(newStringTable.Count % 16 != 0)
            {
                newStringTable.Add(0);
            }
        }

        //saving stuff - "Created using Buffeting's Editor"
        /*
         * saving XSC code [Header]
         * bytes 0 - 3      [82, 83, 67, 55]
         * bytes 4 - 7      [00, 00, 00, 09]
         * bytes 8 - 11     [Depends on XSC file size (unknown how calculated)]         !CALCULATE
         * bytes 12 - 15    [144, 00, 00, 00]
         * ----------------------------------------------
         * bytes 16 - 19    [52, 39, 69, 00]
         * bytes 20 - 23    [80, pointer to pointer ? script flags?]                    !CALCULATE
         * bytes 24 - 27    [80, pointer to pointer that shows start of code table - 16]!CALCULATE
         * bytes 28 - 31    [253, 246, 158, 54]
         * ----------------------------------------------
         * bytes 32 - 35    [depends on code size]                                      !CALCULATE
         * bytes 36 - 39    [00, 00, 00, 00]                                 PARAM COUNT!CALCULATE
         * bytes 40 - 43    [depends on static variables table size]                    !CALCULATE
         * bytes 44 - 47    [00, 00, 00, 00]
         * ----------------------------------------------
         * bytes 48 - 51    [depends on natives table size]                             !CALCULATE
         * bytes 52 - 55    [80, pointer to static variable table?]                     !CALCULATE
         * bytes 56 - 59    [00, 00, 00, 00]
         * bytes 60 - 63    [80, pointer to natives table]                              !CALCULATE
         * ----------------------------------------------
         * bytes 64 - 67    [00, 00, 00, 00]
         * bytes 68 - 71    [00, 00, 00, 00]
         * bytes 72 - 75    [XSC name (converted with jenkins string to hash)]          !CALCULATE
         * bytes 76 - 79    [00, 00, 00, 01]    (unknown)
         * ----------------------------------------------
         * bytes 80 - 83    [80, pointer to XSC name]
         * bytes 84 - 87    [80, pointer to pointer that points to string table start]
         * bytes 88 - 91    [depends on string table size]                              !CALCULATE
         * bytes 92 - 95    [00, 00, 00, 00]
         * ----------------------------------------------
         * rest = start of code table for the most part;
        */

        public void SetNewXSCName(string name)
        {
            name = name.Trim();
            name = Regex.Replace(name, @"\s+", "_");
            if (name.Length < 1) throw new InvalidDataException();
            char lastChar = name[name.Length - 1];
            if (Regex.IsMatch(name, @"^[a-zA-Z0-9_]+$") && lastChar != '_')
            {
                this.newXSCName = name;
            }
            else throw new InvalidDataException();
        }

        public void SetNewParamCount(string count)
        {
            newParameterVarCount = Convert.ToUInt32(count);
        }

        public void MergeXSCTables()
        {
            newXSCBytes = new List<byte>();
            int codeTableBytePos = 0;
            int codeTablePointer = 0;
            int staticsTableBytePos = 0;
            int nativesTableBytePos = 0;
            int stringTableBytePos = 0;
            int stringTablePointer = 0;
            int unknownTablePointer = 0;
            int xscNameBytePos = 0;
            byte[] block;
            //header        55, 67, 83, 82
            newXSCBytes.AddRange(new byte[] { 82, 83, 67, 55,  00, 00, 00, 09 });   //RSC7 header                       
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0 });                        //Calculate - XSC Size              8 - 11
            newXSCBytes.AddRange(new byte[] { 144, 00, 00, 00, 180, 58, 69, 00 });  //unknown/update if ever needed                           
            newXSCBytes.AddRange(new byte[] { 80, 0, 0, 0 });                       //pointer to ? - Script flags?      20 - 23
            newXSCBytes.AddRange(new byte[] { 80, 0, 0, 0 });                       //pointer to code table pointer-16  24 - 27
            newXSCBytes.AddRange(new byte[] { 160, 179, 86, 47 });                 //unknown    udpate if needed                       
            block = BitConverter.GetBytes(newCodeTableSize);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes.AddRange(block);                                            //code table size                   32 - 35
            block = BitConverter.GetBytes(newParameterVarCount);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes.AddRange(block);                        //misc                              
            block = BitConverter.GetBytes(newStaticVariablesTableSize);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes.AddRange(block);                                            //static variables table size       40 - 43
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0 });                        //misc
            block = BitConverter.GetBytes(newNativesTableSize);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes.AddRange(block);                                            //natives table size                48 - 51
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0 });                        //pointer to static variable table  52 - 55
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0 });                        //misc
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0 });                        //pointer to natives table           60 - 63
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });            //misc
            block = BitConverter.GetBytes(GetJenkinsHash(newXSCName));
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes.AddRange(block);                                            //script name     72 - 75
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 1 });                        //misc
            newXSCBytes.AddRange(new byte[] { 80, 0, 0, 0 });                       //pointer to script name            80 - 83
            newXSCBytes.AddRange(new byte[] { 80, 0, 0, 0 });                       //pointer to string table?          84 - 87
            block = BitConverter.GetBytes(newStringTableSize);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes.AddRange(block);                                            //string table size                 88 - 91
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0 });                        //misc
            int[] scriptTableSort = { newCodeTableSize, newStaticVariablesTableSize, newNativesTableSize, newStringTableSize };
            bool[] alreadyAdded = {false, false, false, false};
            Array.Sort(scriptTableSort);
            scriptTableSort = scriptTableSort.Reverse().ToArray();
            for (int i = 0; i < 4; i++)
            {
                if(newCodeTableSize == scriptTableSort[i] && !alreadyAdded[0])
                {
                    codeTableBytePos = newXSCBytes.Count - 16;
                    newXSCBytes = newXSCBytes.Concat(newCodeTable).ToList();
                    alreadyAdded[0] = true;
                }
                else if (newStringTableSize == scriptTableSort[i] && !alreadyAdded[3])
                {
                    if (newCodeTableSize + newStringTableSize > 16384)
                    {
                        while ((newXSCBytes.Count - 16) % 16384 != 0)
                            newXSCBytes.Add(0);

                    }
                    stringTableBytePos = newXSCBytes.Count - 16;
                    newXSCBytes = newXSCBytes.Concat(newStringTable).ToList();
                    alreadyAdded[3] = true;
                }
                else if (newNativesTableSize == scriptTableSort[i] && !alreadyAdded[2])
                {
                    nativesTableBytePos = newXSCBytes.Count - 16;
                    newXSCBytes = newXSCBytes.Concat(newNativesTable).ToList();
                    alreadyAdded[2] = true;
                }
                else if(newStaticVariablesTableSize == scriptTableSort[i] && !alreadyAdded[1])
                {
                    staticsTableBytePos = newXSCBytes.Count - 16;
                    newXSCBytes = newXSCBytes.Concat(newStaticVariablesTable).ToList();
                    alreadyAdded[1] = true;
                }
            }
            // code & string pointers
            codeTablePointer = newXSCBytes.Count - 16;
            if (newCodeTable.Count > 0)
            {
                newXSCBytes.Add(80);
                block = BitConverter.GetBytes(codeTableBytePos);
                if (BitConverter.IsLittleEndian)
                {
                    block = block.Reverse().ToArray();
                }
                newXSCBytes.AddRange(new byte[] { block[1], block[2], block[3] });
                for (int i = 0; i < 12; i++)
                    newXSCBytes.Add(0);
            }
            else
            {
                for (int i = 0; i < 16; i++ )
                    newXSCBytes.Add(0);
            }
            stringTablePointer = newXSCBytes.Count - 16;
            if (newStringTable.Count > 0)
            {
                newXSCBytes.Add(80);
                block = BitConverter.GetBytes(stringTableBytePos);
                if (BitConverter.IsLittleEndian)
                {
                    block = block.Reverse().ToArray();
                }
                newXSCBytes.AddRange(new byte[] { block[1], block[2], block[3] });
                for (int i = 0; i < 12; i++)
                    newXSCBytes.Add(0);
            }
            else
            {
                for (int i = 0; i < 16; i++)
                    newXSCBytes.Add(0);
            }
            unknownTablePointer = newXSCBytes.Count - 16;
            newXSCBytes.AddRange(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            xscNameBytePos = newXSCBytes.Count - 16;
            newXSCBytes.AddRange(Encoding.ASCII.GetBytes(newXSCName));
            //might need to add more bytes after name

            //padding
            int XSCSize = 8208 * ((newXSCBytes.Count + 48) / 8208 + 1) - (16 * ((newXSCBytes.Count + 48) / 8208)) - 48;
            for (int i = newXSCBytes.Count; i < XSCSize; i++) 
                newXSCBytes.Add(0);
            newXSCBytes.AddRange(Encoding.ASCII.GetBytes("XSC created/edited with Buffeting's XSC Editor. "));
            //POINTERS
            //Code Table Pointer Pointer
            block = BitConverter.GetBytes(codeTablePointer);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes[25] = block[1]; newXSCBytes[26] = block[2]; newXSCBytes[27] = block[3];
            //Static Table Pointer
            if (newStaticVariablesTable.Count > 0)
            {
                newXSCBytes[52] = 80;
                block = BitConverter.GetBytes(staticsTableBytePos);
                if (BitConverter.IsLittleEndian)
                {
                    block = block.Reverse().ToArray();
                }
                newXSCBytes[53] = block[1]; newXSCBytes[54] = block[2]; newXSCBytes[55] = block[3];
            }
            //Natives Table Pointer
            if (newNativesTable.Count > 0)
            {
                newXSCBytes[60] = 80;
                block = BitConverter.GetBytes(nativesTableBytePos);
                if (BitConverter.IsLittleEndian)
                {
                    block = block.Reverse().ToArray();
                }
                newXSCBytes[61] = block[1]; newXSCBytes[62] = block[2]; newXSCBytes[63] = block[3];
            }
            //String Table Pointer Pointer
            block = BitConverter.GetBytes(stringTablePointer);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes[85] = block[1]; newXSCBytes[86] = block[2]; newXSCBytes[87] = block[3];
            //Unknown table - Possibly script flags
            block = BitConverter.GetBytes(unknownTablePointer);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes[21] = block[1]; newXSCBytes[22] = block[2]; newXSCBytes[23] = block[3];
            //XSC Name
            block = BitConverter.GetBytes(xscNameBytePos);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes[81] = block[1]; newXSCBytes[82] = block[2]; newXSCBytes[83] = block[3];

            block = BitConverter.GetBytes(newXSCBytes.Count - 16);
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }

            block = BitConverter.GetBytes(GetSizeFromFlag((uint)newXSCBytes.Count - 16, 0x2000));
            if (BitConverter.IsLittleEndian)
            {
                block = block.Reverse().ToArray();
            }
            newXSCBytes[8] = block[0]; newXSCBytes[9] = block[1]; newXSCBytes[10] = block[2]; newXSCBytes[11] = block[3];
            newXSCBytes.RemoveRange(0, 16); //until i can figure out the flags for LibertyV
        }

        public byte[] GetNewXSCBytes()
        {
            return newXSCBytes.ToArray();
        }
        
    }
}
