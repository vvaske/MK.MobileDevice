using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MK.MobileDevice.XEDevice
{
	// Token: 0x02000002 RID: 2
	public class CFDictParser
	{
		// Token: 0x06000002 RID: 2
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern int CFArrayGetCount(void* array);

		// Token: 0x06000006 RID: 6
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void* CFArrayGetValueAtIndex(void* array, int idx);

		// Token: 0x06000005 RID: 5
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void CFArrayGetValues(void* array, CFDictParser.__CFRange range, void*[] values);

		// Token: 0x0600000B RID: 11
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern bool CFBooleanGetValue(void* obj);

		// Token: 0x0600000F RID: 15
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void CFDataGetBytes(void* obj, CFDictParser.__CFRange range, IntPtr buffer);

		// Token: 0x0600000E RID: 14
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern int CFDataGetLength(void* obj);

		// Token: 0x06000001 RID: 1
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern int CFDictionaryGetCount(void* dict);

		// Token: 0x06000003 RID: 3
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void CFDictionaryGetKeysAndValues(void* dict, void*[] keys, void*[] values);

		// Token: 0x06000004 RID: 4
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void* CFDictionaryGetValue(void* dict, void* key);

		// Token: 0x06000007 RID: 7
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern CFDictParser.CFType CFGetTypeID(void* obj);

		// Token: 0x0600000C RID: 12
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern CFDictParser.CFNumberType CFNumberGetType(void* obj);

		// Token: 0x0600000D RID: 13
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern bool CFNumberGetValue(void* obj, CFDictParser.CFNumberType theType, IntPtr valuePtr);

		// Token: 0x0600000A RID: 10
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void CFStringGetCharacters(void* obj, CFDictParser.__CFRange range, IntPtr buffer);

		// Token: 0x06000009 RID: 9
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern char* CFStringGetCharactersPtr(void* obj);

		// Token: 0x06000008 RID: 8
		[DllImport("CoreFoundation.dll", CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern int CFStringGetLength(void* obj);

		// Token: 0x06000013 RID: 19 RVA: 0x000024D8 File Offset: 0x000006D8
		private unsafe static Array GetArrayFromObject(void* obj)
		{
			int num = CFDictParser.CFArrayGetCount(obj);
			object[] array = new object[num];
			void*[] array2 = new void*[num];
			CFDictParser.__CFRange _CFRange = default(CFDictParser.__CFRange);
			_CFRange.length = num;
			_CFRange.location = 0;
			for (int i = 0; i < num; i++)
			{
				array2[i] = CFDictParser.CFArrayGetValueAtIndex(obj, i);
			}
			for (int i = 0; i < num; i++)
			{
				array[i] = CFDictParser.ParseObject(array2[i], null);
			}
			return array;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002558 File Offset: 0x00000758
		private unsafe static byte[] GetDataFromObject(void* obj)
		{
			int num = CFDictParser.CFDataGetLength(obj);
			byte[] array = new byte[num];
			CFDictParser.CFDataGetBytes(obj, new CFDictParser.__CFRange
			{
				length = num,
				location = 0
			}, Marshal.UnsafeAddrOfPinnedArrayElement(array, 0));
			return array;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000259C File Offset: 0x0000079C
		private unsafe static object GetNumberFromObject(void* obj)
		{
			CFDictParser.CFNumberType cFNumberType = CFDictParser.CFNumberGetType(obj);
			IntPtr intPtr = Marshal.AllocHGlobal(8);
			CFDictParser.CFNumberGetValue(obj, cFNumberType, intPtr);
			byte[] array = new byte[8];
			Marshal.Copy(intPtr, array, 0, 8);
			Marshal.FreeHGlobal(intPtr);
			switch (cFNumberType)
			{
			case CFDictParser.CFNumberType.kCFNumberSInt16Type:
			{
				object result = BitConverter.ToInt16(array, 0);
				return result;
			}
			case CFDictParser.CFNumberType.kCFNumberSInt32Type:
			case CFDictParser.CFNumberType.kCFNumberIntType:
			{
				object result = BitConverter.ToInt32(array, 0);
				return result;
			}
			case CFDictParser.CFNumberType.kCFNumberSInt64Type:
			{
				object result = BitConverter.ToInt64(array, 0);
				return result;
			}
			case CFDictParser.CFNumberType.kCFNumberFloat32Type:
			case CFDictParser.CFNumberType.kCFNumberFloatType:
			{
				object result = BitConverter.ToSingle(array, 0);
				return result;
			}
			case CFDictParser.CFNumberType.kCFNumberFloat64Type:
			case CFDictParser.CFNumberType.kCFNumberDoubleType:
			{
				object result = BitConverter.ToDouble(array, 0);
				return result;
			}
			}
			throw new Exception("CFNumberType not implemented: " + cFNumberType.ToString());
		}

		// Token: 0x06000012 RID: 18 RVA: 0x0000248C File Offset: 0x0000068C
		private unsafe static string GetStringFromObject(void* obj)
		{
			int num = CFDictParser.CFStringGetLength(obj);
			CFDictParser.__CFRange range = default(CFDictParser.__CFRange);
			range.length = num;
			range.location = 0;
			char[] array = new char[num];
			CFDictParser.CFStringGetCharacters(obj, range, Marshal.UnsafeAddrOfPinnedArrayElement(array, 0));
			return new string(array);
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000023FC File Offset: 0x000005FC
		public unsafe static Dictionary<string, object> Parse(void* root_dict)
		{
			int num = CFDictParser.CFDictionaryGetCount(root_dict);
			Dictionary<string, object> dictionary = new Dictionary<string, object>(num);
			void*[] array = new void*[num];
			void*[] values = new void*[num];
			CFDictParser.CFDictionaryGetKeysAndValues(root_dict, array, values);
			for (int i = 0; i < num; i++)
			{
				CFDictParser.CFType cFType = CFDictParser.CFGetTypeID(array[i]);
				if (cFType != CFDictParser.CFType.CFString)
				{
					throw new Exception("Key not string type");
				}
				string stringFromObject = CFDictParser.GetStringFromObject(array[i]);
				void* obj = CFDictParser.CFDictionaryGetValue(root_dict, array[i]);
				dictionary.Add(stringFromObject, CFDictParser.ParseObject(obj, stringFromObject));
			}
			return dictionary;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002278 File Offset: 0x00000478
		public unsafe static Dictionary<string, object> ParseAppDictionaryData(void* root_dict)
		{
			int num = CFDictParser.CFDictionaryGetCount(root_dict);
			Dictionary<string, object> dictionary = new Dictionary<string, object>(num);
			void*[] array = new void*[num];
			void*[] values = new void*[num];
			CFDictParser.CFDictionaryGetKeysAndValues(root_dict, array, values);
			for (int i = 0; i < num; i++)
			{
				CFDictParser.CFType cFType = CFDictParser.CFGetTypeID(array[i]);
				if (cFType != CFDictParser.CFType.CFString)
				{
					throw new Exception("Key not string type");
				}
				string stringFromObject = CFDictParser.GetStringFromObject(array[i]);
				List<string> list = new List<string>();
				list.Add("CFBundleDisplayName");
				list.Add("CFBundleName");
				list.Add("CFBundleIconFiles");
				list.Add("UIPrerenderedIcon");
				list.Add("ApplicationType");
				list.Add("UIFileSharingEnabled");
				list.Add("SignerIdentity");
				void* dict = CFDictParser.CFDictionaryGetValue(root_dict, array[i]);
				int num2 = CFDictParser.CFDictionaryGetCount(dict);
				void*[] array2 = new void*[num2];
				void*[] values2 = new void*[num2];
				CFDictParser.CFDictionaryGetKeysAndValues(dict, array2, values2);
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>(num2);
				for (int j = 0; j < num2; j++)
				{
					CFDictParser.CFType cFType2 = CFDictParser.CFGetTypeID(array2[j]);
					if (cFType2 != CFDictParser.CFType.CFString)
					{
						throw new Exception("Key not string type");
					}
					string stringFromObject2 = CFDictParser.GetStringFromObject(array2[j]);
					if (list.Contains(stringFromObject2))
					{
						dictionary2.Add(stringFromObject2, CFDictParser.ParseObject(CFDictParser.CFDictionaryGetValue(dict, array2[j]), stringFromObject2));
					}
				}
				dictionary.Add(stringFromObject, dictionary2);
			}
			return dictionary;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002680 File Offset: 0x00000880
		private unsafe static object ParseObject(void* obj, string keyname)
		{
			CFDictParser.CFType cFType = CFDictParser.CFGetTypeID(obj);
			CFDictParser.CFType cFType2 = cFType;
			object result;
			if (cFType2 != CFDictParser.CFType.CFString)
			{
				switch (cFType2)
				{
				case CFDictParser.CFType.CFDictionary:
					result = CFDictParser.Parse(obj);
					return result;
				case CFDictParser.CFType.CFArray:
				case CFDictParser.CFType.CFBundleIconFiles:
					result = CFDictParser.GetArrayFromObject(obj);
					return result;
				case CFDictParser.CFType.CFBoolean:
					result = CFDictParser.CFBooleanGetValue(obj);
					return result;
				case CFDictParser.CFType.CFNumber:
					result = CFDictParser.GetNumberFromObject(obj);
					return result;
				case CFDictParser.CFType.CFData:
					result = CFDictParser.GetDataFromObject(obj);
					return result;
				}
				result = CFDictParser.GetDataFromObject(obj);
			}
			else
			{
				result = CFDictParser.GetStringFromObject(obj);
			}
			return result;
		}

		// Token: 0x02000004 RID: 4
		private enum CFNumberType
		{
			// Token: 0x0400000A RID: 10
			kCFNumberSInt8Type = 1,
			// Token: 0x0400000B RID: 11
			kCFNumberSInt16Type,
			// Token: 0x0400000C RID: 12
			kCFNumberSInt32Type,
			// Token: 0x0400000D RID: 13
			kCFNumberSInt64Type,
			// Token: 0x0400000E RID: 14
			kCFNumberFloat32Type,
			// Token: 0x0400000F RID: 15
			kCFNumberFloat64Type,
			// Token: 0x04000010 RID: 16
			kCFNumberCharType,
			// Token: 0x04000011 RID: 17
			kCFNumberShortType,
			// Token: 0x04000012 RID: 18
			kCFNumberIntType,
			// Token: 0x04000013 RID: 19
			kCFNumberLongType,
			// Token: 0x04000014 RID: 20
			kCFNumberLongLongType,
			// Token: 0x04000015 RID: 21
			kCFNumberFloatType,
			// Token: 0x04000016 RID: 22
			kCFNumberDoubleType,
			// Token: 0x04000017 RID: 23
			kCFNumberCFIndexType,
			// Token: 0x04000018 RID: 24
			kCFNumberMaxType = 14
		}

		// Token: 0x02000003 RID: 3
		private enum CFType
		{
			// Token: 0x04000002 RID: 2
			CFString = 7,
			// Token: 0x04000003 RID: 3
			CFDictionary = 17,
			// Token: 0x04000004 RID: 4
			CFArray,
			// Token: 0x04000005 RID: 5
			CFBundleIconFiles,
			// Token: 0x04000006 RID: 6
			CFBoolean = 21,
			// Token: 0x04000007 RID: 7
			CFNumber,
			// Token: 0x04000008 RID: 8
			CFData
		}

		// Token: 0x02000005 RID: 5
		private struct __CFRange
		{
			// Token: 0x04000019 RID: 25
			public int location;

			// Token: 0x0400001A RID: 26
			public int length;
		}
	}
}
