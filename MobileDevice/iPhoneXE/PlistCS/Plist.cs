using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace PlistCS
{
    // Token: 0x0200001F RID: 31
    public static class Plist
    {
        // Token: 0x06000112 RID: 274 RVA: 0x00007244 File Offset: 0x00005444
        private static void compose(object value, XmlWriter writer)
        {
            if (value == null || value is string)
            {
                writer.WriteElementString("string", value as string);
            }
            else if (value is int || value is long)
            {
                writer.WriteElementString("integer", ((int)value).ToString(NumberFormatInfo.InvariantInfo));
            }
            else if (value is Dictionary<string, object> || value.GetType().ToString().StartsWith("System.Collections.Generic.Dictionary`2[System.String"))
            {
                Dictionary<string, object> dictionary = value as Dictionary<string, object>;
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, object>();
                    IDictionary dictionary2 = (IDictionary)value;
                    foreach (object current in dictionary2.Keys)
                    {
                        dictionary.Add(current.ToString(), dictionary2[current]);
                    }
                }
                Plist.writeDictionaryValues(dictionary, writer);
            }
            else if (value is List<object>)
            {
                Plist.composeArray((List<object>)value, writer);
            }
            else if (value is byte[])
            {
                writer.WriteElementString("data", Convert.ToBase64String((byte[])value));
            }
            else if (value is float || value is double)
            {
                writer.WriteElementString("real", ((double)value).ToString(NumberFormatInfo.InvariantInfo));
            }
            else if (value is DateTime)
            {
                DateTime value2 = (DateTime)value;
                string value3 = XmlConvert.ToString(value2, XmlDateTimeSerializationMode.Utc);
                writer.WriteElementString("date", value3);
            }
            else
            {
                if (!(value is bool))
                {
                    throw new Exception(string.Format("Value type '{0}' is unhandled", value.GetType().ToString()));
                }
                writer.WriteElementString(value.ToString().ToLower(), "");
            }
        }

        // Token: 0x06000110 RID: 272 RVA: 0x00007050 File Offset: 0x00005250
        private static void composeArray(List<object> value, XmlWriter writer)
        {
            writer.WriteStartElement("array");
            foreach (object current in value)
            {
                Plist.compose(current, writer);
            }
            writer.WriteEndElement();
        }

        // Token: 0x06000117 RID: 279 RVA: 0x000078C8 File Offset: 0x00005AC8
        private static byte[] composeBinary(object obj)
        {
            string text = obj.GetType().ToString();
            byte[] result;
            switch (text)
            {
                case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
                    {
                        byte[] array = Plist.writeBinaryDictionary((Dictionary<string, object>)obj);
                        result = array;
                        return result;
                    }
                case "System.Collections.Generic.List`1[System.Object]":
                    {
                        byte[] array = Plist.composeBinaryArray((List<object>)obj);
                        result = array;
                        return result;
                    }
                case "System.Byte[]":
                    {
                        byte[] array = Plist.writeBinaryByteArray((byte[])obj);
                        result = array;
                        return result;
                    }
                case "System.Double":
                    {
                        byte[] array = Plist.writeBinaryDouble((double)obj);
                        result = array;
                        return result;
                    }
                case "System.Int32":
                    {
                        byte[] array = Plist.writeBinaryInteger((int)obj, true);
                        result = array;
                        return result;
                    }
                case "System.String":
                    {
                        byte[] array = Plist.writeBinaryString((string)obj, true);
                        result = array;
                        return result;
                    }
                case "System.DateTime":
                    {
                        byte[] array = Plist.writeBinaryDate((DateTime)obj);
                        result = array;
                        return result;
                    }
                case "System.Boolean":
                    {
                        byte[] array = Plist.writeBinaryBool((bool)obj);
                        result = array;
                        return result;
                    }
            }
            result = new byte[0];
            return result;
        }

        // Token: 0x06000116 RID: 278 RVA: 0x00007790 File Offset: 0x00005990
        private static byte[] composeBinaryArray(List<object> objects)
        {
            List<byte> list = new List<byte>();
            List<byte> list2 = new List<byte>();
            List<int> list3 = new List<int>();
            for (int i = objects.Count - 1; i >= 0; i--)
            {
                Plist.composeBinary(objects[i]);
                Plist.offsetTable.Add(Plist.objectTable.Count);
                list3.Add(Plist.refCount);
                Plist.refCount--;
            }
            if (objects.Count < 15)
            {
                list2.Add(Convert.ToByte((int)(160 | Convert.ToByte(objects.Count))));
            }
            else
            {
                list2.Add(175);
                list2.AddRange(Plist.writeBinaryInteger(objects.Count, false));
            }
            foreach (int current in list3)
            {
                byte[] array = Plist.RegulateNullBytes(BitConverter.GetBytes(current), Plist.objRefSize);
                Array.Reverse(array);
                list.InsertRange(0, array);
            }
            list.InsertRange(0, list2);
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x06000114 RID: 276 RVA: 0x000074D0 File Offset: 0x000056D0
        private static int countObject(object value)
        {
            int num = 0;
            string text = value.GetType().ToString();
            if (text != null)
            {
                if (text == "System.Collections.Generic.Dictionary`2[System.String,System.Object]")
                {
                    Dictionary<string, object> dictionary = (Dictionary<string, object>)value;
                    foreach (string current in dictionary.Keys)
                    {
                        num += Plist.countObject(dictionary[current]);
                    }
                    num += dictionary.Keys.Count;
                    num++;
                    return num;
                }
                if (text == "System.Collections.Generic.List`1[System.Object]")
                {
                    List<object> list = (List<object>)value;
                    foreach (object current2 in list)
                    {
                        num += Plist.countObject(current2);
                    }
                    num++;
                    return num;
                }
            }
            num++;
            return num;
        }

        // Token: 0x06000124 RID: 292 RVA: 0x00008098 File Offset: 0x00006298
        private static int getCount(int bytePosition, out int newBytePosition)
        {
            byte b = Plist.objectTable[bytePosition];
            byte b2 = Convert.ToByte((int)(b & 15));
            int result;
            if (b2 < 15)
            {
                result = (int)b2;
                newBytePosition = bytePosition + 1;
            }
            else
            {
                result = (int)Plist.parseBinaryInt(bytePosition + 1, out newBytePosition);
            }
            return result;
        }

        // Token: 0x06000104 RID: 260 RVA: 0x000069D0 File Offset: 0x00004BD0
        public static plistType getPlistType(Stream stream)
        {
            byte[] array = new byte[8];
            stream.Read(array, 0, 8);
            plistType result;
            if (BitConverter.ToInt64(array, 0) == 3472403351741427810L)
            {
                result = plistType.Binary;
            }
            else
            {
                result = plistType.Xml;
            }
            return result;
        }

        // Token: 0x06000111 RID: 273 RVA: 0x000070B0 File Offset: 0x000052B0
        private static object parse(XmlNode node)
        {
            string name = node.Name;
            if (name != null)
            {
                Dictionary<string, int> xdict = null;
                if (xdict == null)
				{
                    xdict = new Dictionary<string, int>(10)
                    {
                        {
                            "dict",
                            0
                        },
                        {
                            "array",
                            1
                        },
                        {
                            "string",
                            2
                        },
                        {
                            "integer",
                            3
                        },
                        {
                            "real",
                            4
                        },
                        {
                            "false",
                            5
                        },
                        {
                            "true",
                            6
                        },
                        {
                            "null",
                            7
                        },
                        {
                            "date",
                            8
                        },
                        {
                            "data",
                            9
                        }
                    };
                }
                int num;
                if (xdict.TryGetValue(name, out num))
				{
                    object result;
                    switch (num)
                    {
                        case 0:
                            result = Plist.parseDictionary(node);
                            break;
                        case 1:
                            result = Plist.parseArray(node);
                            break;
                        case 2:
                            result = node.InnerText;
                            break;
                        case 3:
                            result = Convert.ToInt32(node.InnerText, NumberFormatInfo.InvariantInfo);
                            break;
                        case 4:
                            result = Convert.ToDouble(node.InnerText, NumberFormatInfo.InvariantInfo);
                            break;
                        case 5:
                            result = false;
                            break;
                        case 6:
                            result = true;
                            break;
                        case 7:
                            result = null;
                            break;
                        case 8:
                            result = XmlConvert.ToDateTime(node.InnerText, XmlDateTimeSerializationMode.Utc);
                            break;
                        case 9:
                            result = Convert.FromBase64String(node.InnerText);
                            break;
                        default:
                            goto IL_170;
                    }
                    return result;
                }
            }
            IL_170:
            throw new ApplicationException(string.Format("Plist Node `{0}' is not supported", node.Name));
        }

        // Token: 0x0600010F RID: 271 RVA: 0x00006FD4 File Offset: 0x000051D4
        private static List<object> parseArray(XmlNode node)
        {
            List<object> list = new List<object>();
            foreach (XmlNode node2 in node.ChildNodes)
            {
                object obj = Plist.parse(node2);
                if (obj != null)
                {
                    list.Add(obj);
                }
            }
            return list;
        }

        // Token: 0x06000125 RID: 293 RVA: 0x000080E0 File Offset: 0x000062E0
        private static object parseBinary(int objRef)
        {
            byte b = Plist.objectTable[Plist.offsetTable[objRef]];
            int num = (int)(b & 240);
            if (num <= 48)
            {
                if (num <= 16)
                {
                    if (num == 0)
                    {
                        object result = (objectTable[offsetTable[objRef]] == 0) ? null : (object)(objectTable[offsetTable[objRef]] == 9);
                        return result;
                    }
                    if (num == 16)
                    {
                        object result = Plist.parseBinaryInt(Plist.offsetTable[objRef]);
                        return result;
                    }
                }
                else
                {
                    if (num == 32)
                    {
                        object result = Plist.parseBinaryReal(Plist.offsetTable[objRef]);
                        return result;
                    }
                    if (num == 48)
                    {
                        object result = Plist.parseBinaryDate(Plist.offsetTable[objRef]);
                        return result;
                    }
                }
            }
            else if (num <= 80)
            {
                if (num == 64)
                {
                    object result = Plist.parseBinaryByteArray(Plist.offsetTable[objRef]);
                    return result;
                }
                if (num == 80)
                {
                    object result = Plist.parseBinaryAsciiString(Plist.offsetTable[objRef]);
                    return result;
                }
            }
            else
            {
                if (num == 96)
                {
                    object result = Plist.parseBinaryUnicodeString(Plist.offsetTable[objRef]);
                    return result;
                }
                if (num == 160)
                {
                    object result = Plist.parseBinaryArray(objRef);
                    return result;
                }
                if (num == 208)
                {
                    object result = Plist.parseBinaryDictionary(objRef);
                    return result;
                }
            }
            throw new Exception("This type is not supported");
        }

        // Token: 0x06000123 RID: 291 RVA: 0x00007FB8 File Offset: 0x000061B8
        private static object parseBinaryArray(int objRef)
        {
            List<object> list = new List<object>();
            List<int> list2 = new List<int>();
            int num;
            int count = Plist.getCount(Plist.offsetTable[objRef], out num);
            if (count < 15)
            {
                num = Plist.offsetTable[objRef] + 1;
            }
            else
            {
                num = Plist.offsetTable[objRef] + 2 + Plist.RegulateNullBytes(BitConverter.GetBytes(count), 1).Length;
            }
            for (int i = num; i < num + count * Plist.objRefSize; i += Plist.objRefSize)
            {
                byte[] array = Plist.objectTable.GetRange(i, Plist.objRefSize).ToArray();
                Array.Reverse(array);
                list2.Add(BitConverter.ToInt32(Plist.RegulateNullBytes(array, 4), 0));
            }
            for (int i = 0; i < count; i++)
            {
                list.Add(Plist.parseBinary(list2[i]));
            }
            return list;
        }

        // Token: 0x0600012A RID: 298 RVA: 0x00008348 File Offset: 0x00006548
        private static object parseBinaryAsciiString(int headerPosition)
        {
            int index;
            int count = Plist.getCount(headerPosition, out index);
            List<byte> range = Plist.objectTable.GetRange(index, count);
            return (range.Count > 0) ? Encoding.ASCII.GetString(range.ToArray()) : string.Empty;
        }

        // Token: 0x0600012C RID: 300 RVA: 0x00008428 File Offset: 0x00006628
        private static object parseBinaryByteArray(int headerPosition)
        {
            int index;
            int count = Plist.getCount(headerPosition, out index);
            return Plist.objectTable.GetRange(index, count).ToArray();
        }

        // Token: 0x06000126 RID: 294 RVA: 0x00008230 File Offset: 0x00006430
        public static object parseBinaryDate(int headerPosition)
        {
            byte[] array = Plist.objectTable.GetRange(headerPosition + 1, 8).ToArray();
            Array.Reverse(array);
            double timestamp = BitConverter.ToDouble(array, 0);
            DateTime dateTime = PlistDateConverter.ConvertFromAppleTimeStamp(timestamp);
            return dateTime;
        }

        // Token: 0x06000122 RID: 290 RVA: 0x00007EC0 File Offset: 0x000060C0
        private static object parseBinaryDictionary(int objRef)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            List<int> list = new List<int>();
            int num;
            int count = Plist.getCount(Plist.offsetTable[objRef], out num);
            if (count < 15)
            {
                num = Plist.offsetTable[objRef] + 1;
            }
            else
            {
                num = Plist.offsetTable[objRef] + 2 + Plist.RegulateNullBytes(BitConverter.GetBytes(count), 1).Length;
            }
            for (int i = num; i < num + count * 2 * Plist.objRefSize; i += Plist.objRefSize)
            {
                byte[] array = Plist.objectTable.GetRange(i, Plist.objRefSize).ToArray();
                Array.Reverse(array);
                list.Add(BitConverter.ToInt32(Plist.RegulateNullBytes(array, 4), 0));
            }
            for (int i = 0; i < count; i++)
            {
                dictionary.Add((string)Plist.parseBinary(list[i]), Plist.parseBinary(list[i + count]));
            }
            return dictionary;
        }

        // Token: 0x06000127 RID: 295 RVA: 0x00008270 File Offset: 0x00006470
        private static object parseBinaryInt(int headerPosition)
        {
            int num;
            return Plist.parseBinaryInt(headerPosition, out num);
        }

        // Token: 0x06000128 RID: 296 RVA: 0x00008288 File Offset: 0x00006488
        private static object parseBinaryInt(int headerPosition, out int newHeaderPosition)
        {
            byte b = Plist.objectTable[headerPosition];
            int num = (int)Math.Pow(2.0, (double)(b & 15));
            byte[] array = Plist.objectTable.GetRange(headerPosition + 1, num).ToArray();
            Array.Reverse(array);
            newHeaderPosition = headerPosition + num + 1;
            return BitConverter.ToInt32(Plist.RegulateNullBytes(array, 4), 0);
        }

        // Token: 0x06000129 RID: 297 RVA: 0x000082EC File Offset: 0x000064EC
        private static object parseBinaryReal(int headerPosition)
        {
            byte b = Plist.objectTable[headerPosition];
            int count = (int)Math.Pow(2.0, (double)(b & 15));
            byte[] array = Plist.objectTable.GetRange(headerPosition + 1, count).ToArray();
            Array.Reverse(array);
            return BitConverter.ToDouble(Plist.RegulateNullBytes(array, 8), 0);
        }

        // Token: 0x0600012B RID: 299 RVA: 0x00008390 File Offset: 0x00006590
        private static object parseBinaryUnicodeString(int headerPosition)
        {
            int num2;
            int num = Plist.getCount(headerPosition, out num2);
            num *= 2;
            byte[] array = new byte[num];
            for (int i = 0; i < num; i += 2)
            {
                byte b = Plist.objectTable.GetRange(num2 + i, 1)[0];
                byte b2 = Plist.objectTable.GetRange(num2 + i + 1, 1)[0];
                if (BitConverter.IsLittleEndian)
                {
                    array[i] = b2;
                    array[i + 1] = b;
                }
                else
                {
                    array[i] = b;
                    array[i + 1] = b2;
                }
            }
            return Encoding.Unicode.GetString(array);
        }

        // Token: 0x0600010E RID: 270 RVA: 0x00006F34 File Offset: 0x00005134
        private static Dictionary<string, object> parseDictionary(XmlNode node)
        {
            XmlNodeList childNodes = node.ChildNodes;
            if (childNodes.Count % 2 != 0)
            {
                throw new DataMisalignedException("Dictionary elements must have an even number of child nodes");
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            for (int i = 0; i < childNodes.Count; i += 2)
            {
                XmlNode xmlNode = childNodes[i];
                XmlNode node2 = childNodes[i + 1];
                if (xmlNode.Name != "key")
                {
                    throw new ApplicationException("expected a key node");
                }
                object obj = Plist.parse(node2);
                if (obj != null)
                {
                    dictionary.Add(xmlNode.InnerText, obj);
                }
            }
            return dictionary;
        }

        // Token: 0x06000121 RID: 289 RVA: 0x00007E6C File Offset: 0x0000606C
        private static void parseOffsetTable(List<byte> offsetTableBytes)
        {
            for (int i = 0; i < offsetTableBytes.Count; i += Plist.offsetByteSize)
            {
                byte[] array = offsetTableBytes.GetRange(i, Plist.offsetByteSize).ToArray();
                Array.Reverse(array);
                Plist.offsetTable.Add(BitConverter.ToInt32(Plist.RegulateNullBytes(array, 4), 0));
            }
        }

        // Token: 0x06000120 RID: 288 RVA: 0x00007DE0 File Offset: 0x00005FE0
        private static void parseTrailer(List<byte> trailer)
        {
            Plist.offsetByteSize = BitConverter.ToInt32(Plist.RegulateNullBytes(trailer.GetRange(6, 1).ToArray(), 4), 0);
            Plist.objRefSize = BitConverter.ToInt32(Plist.RegulateNullBytes(trailer.GetRange(7, 1).ToArray(), 4), 0);
            byte[] array = trailer.GetRange(12, 4).ToArray();
            Array.Reverse(array);
            Plist.refCount = BitConverter.ToInt32(array, 0);
            byte[] array2 = trailer.GetRange(24, 8).ToArray();
            Array.Reverse(array2);
            Plist.offsetTableOffset = BitConverter.ToInt64(array2, 0);
        }

        // Token: 0x0600010D RID: 269 RVA: 0x00006E90 File Offset: 0x00005090
        private static object readBinary(byte[] data)
        {
            Plist.offsetTable.Clear();
            List<byte> offsetTableBytes = new List<byte>();
            Plist.objectTable.Clear();
            Plist.refCount = 0;
            Plist.objRefSize = 0;
            Plist.offsetByteSize = 0;
            Plist.offsetTableOffset = 0L;
            List<byte> list = new List<byte>(data);
            List<byte> range = list.GetRange(list.Count - 32, 32);
            Plist.parseTrailer(range);
            Plist.objectTable = list.GetRange(0, (int)Plist.offsetTableOffset);
            offsetTableBytes = list.GetRange((int)Plist.offsetTableOffset, list.Count - (int)Plist.offsetTableOffset - 32);
            Plist.parseOffsetTable(offsetTableBytes);
            return Plist.parseBinary(0);
        }

        // Token: 0x06000101 RID: 257 RVA: 0x00006954 File Offset: 0x00004B54
        public static object readPlist(string path)
        {
            object result;
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                result = Plist.readPlist(fileStream, plistType.Auto);
            }
            return result;
        }

        // Token: 0x06000103 RID: 259 RVA: 0x000069B4 File Offset: 0x00004BB4
        public static object readPlist(byte[] data)
        {
            return Plist.readPlist(new MemoryStream(data), plistType.Auto);
        }

        // Token: 0x06000105 RID: 261 RVA: 0x00006A0C File Offset: 0x00004C0C
        public static object readPlist(Stream stream, plistType type)
        {
            if (type == plistType.Auto)
            {
                type = Plist.getPlistType(stream);
                stream.Seek(0L, SeekOrigin.Begin);
            }
            object result;
            if (type == plistType.Binary)
            {
                using (BinaryReader binaryReader = new BinaryReader(stream))
                {
                    byte[] data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                    result = Plist.readBinary(data);
                    return result;
                }
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = null;
            xmlDocument.Load(stream);
            result = Plist.readXml(xmlDocument);
            return result;
        }

        // Token: 0x06000102 RID: 258 RVA: 0x00006994 File Offset: 0x00004B94
        public static object readPlistSource(string source)
        {
            return Plist.readPlist(Encoding.UTF8.GetBytes(source));
        }

        // Token: 0x0600010C RID: 268 RVA: 0x00006E68 File Offset: 0x00005068
        private static object readXml(XmlDocument xml)
        {
            XmlNode node = xml.DocumentElement.ChildNodes[0];
            return Plist.parse(node);
        }

        // Token: 0x0600011E RID: 286 RVA: 0x00007D30 File Offset: 0x00005F30
        private static byte[] RegulateNullBytes(byte[] value)
        {
            return Plist.RegulateNullBytes(value, 1);
        }

        // Token: 0x0600011F RID: 287 RVA: 0x00007D48 File Offset: 0x00005F48
        private static byte[] RegulateNullBytes(byte[] value, int minBytes)
        {
            Array.Reverse(value);
            List<byte> list = new List<byte>(value);
            int i = 0;
            while (i < list.Count && list[i] == 0 && list.Count > minBytes)
            {
                list.Remove(list[i]);
                i--;
                i++;
            }
            if (list.Count < minBytes)
            {
                int num = minBytes - list.Count;
                for (i = 0; i < num; i++)
                {
                    list.Insert(0, 0);
                }
            }
            value = list.ToArray();
            Array.Reverse(value);
            return value;
        }

        // Token: 0x0600010B RID: 267 RVA: 0x00006C84 File Offset: 0x00004E84
        public static byte[] writeBinary(object value)
        {
            Plist.offsetTable.Clear();
            Plist.objectTable.Clear();
            Plist.refCount = 0;
            Plist.objRefSize = 0;
            Plist.offsetByteSize = 0;
            Plist.offsetTableOffset = 0L;
            int num = Plist.countObject(value) - 1;
            Plist.refCount = num;
            Plist.objRefSize = Plist.RegulateNullBytes(BitConverter.GetBytes(Plist.refCount)).Length;
            Plist.composeBinary(value);
            Plist.writeBinaryString("bplist00", false);
            Plist.offsetTableOffset = (long)Plist.objectTable.Count;
            Plist.offsetTable.Add(Plist.objectTable.Count - 8);
            Plist.offsetByteSize = Plist.RegulateNullBytes(BitConverter.GetBytes(Plist.offsetTable[Plist.offsetTable.Count - 1])).Length;
            List<byte> list = new List<byte>();
            Plist.offsetTable.Reverse();
            for (int i = 0; i < Plist.offsetTable.Count; i++)
            {
                Plist.offsetTable[i] = Plist.objectTable.Count - Plist.offsetTable[i];
                byte[] array = Plist.RegulateNullBytes(BitConverter.GetBytes(Plist.offsetTable[i]), Plist.offsetByteSize);
                Array.Reverse(array);
                list.AddRange(array);
            }
            Plist.objectTable.AddRange(list);
            Plist.objectTable.AddRange(new byte[6]);
            Plist.objectTable.Add(Convert.ToByte(Plist.offsetByteSize));
            Plist.objectTable.Add(Convert.ToByte(Plist.objRefSize));
            byte[] bytes = BitConverter.GetBytes((long)num + 1L);
            Array.Reverse(bytes);
            Plist.objectTable.AddRange(bytes);
            Plist.objectTable.AddRange(BitConverter.GetBytes(0L));
            bytes = BitConverter.GetBytes(Plist.offsetTableOffset);
            Array.Reverse(bytes);
            Plist.objectTable.AddRange(bytes);
            return Plist.objectTable.ToArray();
        }

        // Token: 0x06000109 RID: 265 RVA: 0x00006BFC File Offset: 0x00004DFC
        public static void writeBinary(object value, string path)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(path, FileMode.Create)))
            {
                binaryWriter.Write(Plist.writeBinary(value));
            }
        }

        // Token: 0x0600010A RID: 266 RVA: 0x00006C44 File Offset: 0x00004E44
        public static void writeBinary(object value, Stream stream)
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(stream))
            {
                binaryWriter.Write(Plist.writeBinary(value));
            }
        }

        // Token: 0x06000119 RID: 281 RVA: 0x00007A70 File Offset: 0x00005C70
        public static byte[] writeBinaryBool(bool obj)
        {
            List<byte> list = new List<byte>(new byte[]
            {
                obj ? (byte)9 : (byte)8
            });
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x0600011C RID: 284 RVA: 0x00007C0C File Offset: 0x00005E0C
        private static byte[] writeBinaryByteArray(byte[] value)
        {
            List<byte> list = new List<byte>(value);
            List<byte> list2 = new List<byte>();
            if (value.Length < 15)
            {
                list2.Add(Convert.ToByte((int)(64 | Convert.ToByte(value.Length))));
            }
            else
            {
                list2.Add(79);
                list2.AddRange(Plist.writeBinaryInteger(list.Count, false));
            }
            list.InsertRange(0, list2);
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x06000118 RID: 280 RVA: 0x00007A28 File Offset: 0x00005C28
        public static byte[] writeBinaryDate(DateTime obj)
        {
            List<byte> list = new List<byte>(Plist.RegulateNullBytes(BitConverter.GetBytes(PlistDateConverter.ConvertToAppleTimeStamp(obj)), 8));
            list.Reverse();
            list.Insert(0, 51);
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x06000115 RID: 277 RVA: 0x000075D8 File Offset: 0x000057D8
        private static byte[] writeBinaryDictionary(Dictionary<string, object> dictionary)
        {
            List<byte> list = new List<byte>();
            List<byte> list2 = new List<byte>();
            List<int> list3 = new List<int>();
            for (int i = dictionary.Count - 1; i >= 0; i--)
            {
                object[] array = new object[dictionary.Count];
                dictionary.Values.CopyTo(array, 0);
                Plist.composeBinary(array[i]);
                Plist.offsetTable.Add(Plist.objectTable.Count);
                list3.Add(Plist.refCount);
                Plist.refCount--;
            }
            for (int i = dictionary.Count - 1; i >= 0; i--)
            {
                string[] array2 = new string[dictionary.Count];
                dictionary.Keys.CopyTo(array2, 0);
                Plist.composeBinary(array2[i]);
                Plist.offsetTable.Add(Plist.objectTable.Count);
                list3.Add(Plist.refCount);
                Plist.refCount--;
            }
            if (dictionary.Count < 15)
            {
                list2.Add(Convert.ToByte((int)(208 | Convert.ToByte(dictionary.Count))));
            }
            else
            {
                list2.Add(223);
                list2.AddRange(Plist.writeBinaryInteger(dictionary.Count, false));
            }
            foreach (int current in list3)
            {
                byte[] array3 = Plist.RegulateNullBytes(BitConverter.GetBytes(current), Plist.objRefSize);
                Array.Reverse(array3);
                list.InsertRange(0, array3);
            }
            list.InsertRange(0, list2);
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x0600011B RID: 283 RVA: 0x00007B64 File Offset: 0x00005D64
        private static byte[] writeBinaryDouble(double value)
        {
            List<byte> list = new List<byte>(Plist.RegulateNullBytes(BitConverter.GetBytes(value), 4));
            while ((double)list.Count != Math.Pow(2.0, Math.Log((double)list.Count) / Math.Log(2.0)))
            {
                list.Add(0);
            }
            int value2 = 32 | (int)(Math.Log((double)list.Count) / Math.Log(2.0));
            list.Reverse();
            list.Insert(0, Convert.ToByte(value2));
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x0600011A RID: 282 RVA: 0x00007AAC File Offset: 0x00005CAC
        private static byte[] writeBinaryInteger(int value, bool write)
        {
            List<byte> list = new List<byte>(BitConverter.GetBytes((long)value));
            list = new List<byte>(Plist.RegulateNullBytes(list.ToArray()));
            while ((double)list.Count != Math.Pow(2.0, Math.Log((double)list.Count) / Math.Log(2.0)))
            {
                list.Add(0);
            }
            int value2 = 16 | (int)(Math.Log((double)list.Count) / Math.Log(2.0));
            list.Reverse();
            list.Insert(0, Convert.ToByte(value2));
            if (write)
            {
                Plist.objectTable.InsertRange(0, list);
            }
            return list.ToArray();
        }

        // Token: 0x0600011D RID: 285 RVA: 0x00007C80 File Offset: 0x00005E80
        private static byte[] writeBinaryString(string value, bool head)
        {
            List<byte> list = new List<byte>();
            List<byte> list2 = new List<byte>();
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                char value2 = array[i];
                list.Add(Convert.ToByte(value2));
            }
            if (head)
            {
                if (value.Length < 15)
                {
                    list2.Add(Convert.ToByte((int)(80 | Convert.ToByte(value.Length))));
                }
                else
                {
                    list2.Add(95);
                    list2.AddRange(Plist.writeBinaryInteger(list.Count, false));
                }
            }
            list.InsertRange(0, list2);
            Plist.objectTable.InsertRange(0, list);
            return list.ToArray();
        }

        // Token: 0x06000113 RID: 275 RVA: 0x00007454 File Offset: 0x00005654
        private static void writeDictionaryValues(Dictionary<string, object> dictionary, XmlWriter writer)
        {
            writer.WriteStartElement("dict");
            foreach (string current in dictionary.Keys)
            {
                object value = dictionary[current];
                writer.WriteElementString("key", current);
                Plist.compose(value, writer);
            }
            writer.WriteEndElement();
        }

        // Token: 0x06000108 RID: 264 RVA: 0x00006B20 File Offset: 0x00004D20
        public static string writeXml(object value)
        {
            string @string;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    ConformanceLevel = ConformanceLevel.Document,
                    Indent = true
                }))
                {
                    xmlWriter.WriteStartDocument();
                    xmlWriter.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
                    xmlWriter.WriteStartElement("plist");
                    xmlWriter.WriteAttributeString("version", "1.0");
                    Plist.compose(value, xmlWriter);
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndDocument();
                    xmlWriter.Flush();
                    xmlWriter.Close();
                    @string = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            return @string;
        }

        // Token: 0x06000106 RID: 262 RVA: 0x00006AA0 File Offset: 0x00004CA0
        public static void writeXml(object value, string path)
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.Write(Plist.writeXml(value));
            }
        }

        // Token: 0x06000107 RID: 263 RVA: 0x00006AE0 File Offset: 0x00004CE0
        public static void writeXml(object value, Stream stream)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
                streamWriter.Write(Plist.writeXml(value));
            }
        }

        // Token: 0x04000091 RID: 145
        private static List<byte> objectTable = new List<byte>();

        // Token: 0x04000093 RID: 147
        private static int objRefSize;

        // Token: 0x04000094 RID: 148
        private static int offsetByteSize;

        // Token: 0x04000090 RID: 144
        private static List<int> offsetTable = new List<int>();

        // Token: 0x04000095 RID: 149
        private static long offsetTableOffset;

        // Token: 0x04000092 RID: 146
        private static int refCount;
    }
}
