/*

 */
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace MK.MobileDevice
{
    public enum PlistType
    {
        PLIST_BOOLEAN,	/**< Boolean, scalar type */
        PLIST_UINT,	/**< Unsigned integer, scalar type */
        PLIST_REAL,	/**< Real, scalar type */
        PLIST_STRING,	/**< ASCII string, scalar type */
        PLIST_ARRAY,	/**< Ordered array, structured type */
        PLIST_DICT,	/**< Unordered dictionary (key/value pair), structured type */
        PLIST_DATE,	/**< Date, scalar type */
        PLIST_DATA,	/**< Binary data, scalar type */
        PLIST_KEY,	/**< Key in dictionaries (ASCII String), scalar type */
        PLIST_UID,      /**< Special type used for 'keyed encoding' */
        PLIST_NONE	/**< No type */
    }

    public abstract class PlistNative
    {
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_dict();

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_array();

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_string(
            [MarshalAs(UnmanagedType.LPStr)] string val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_bool(
            [MarshalAs(UnmanagedType.U1)] bool val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_uint(
            ulong val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_real(
            double val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_data(
            byte[] val,
            ulong length
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_date(
            int sec,
            int usec
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_new_uid(
            ulong val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_free(
            IntPtr plist
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_copy(
            IntPtr node
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int plist_array_get_size(
            IntPtr node
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_array_get_item(
            IntPtr node,
            int n
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int plist_array_get_item_index(
            IntPtr node
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_array_set_item(
            IntPtr node,
            IntPtr item,
            int n
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_array_append_item(
            IntPtr node,
            IntPtr item
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_array_insert_item(
            IntPtr node,
            IntPtr item,
            int n
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_array_remove_item(
            IntPtr node,
            int n
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint plist_dict_get_size(
            IntPtr node
        );

        // TODO - Caller Frees iter
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_new_iter(
            IntPtr node,
            out IntPtr iter
        );

        // TODO - Caller Frees key
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_next_item(
            IntPtr node,
            IntPtr iter,
            out IntPtr key,
            out IntPtr val
        );

        // TODO - Caller frees key
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_get_item_key(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] ref string key
        );

        // TODO - Caller frees key
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_dict_get_item(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] string key
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_set_item(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] string key,
            IntPtr item
        );

        [Obsolete("use plist_dict_set_item instead")]
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_insert_item(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] string key,
            IntPtr item
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_remove_item(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] string key
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_dict_merge(
            ref IntPtr target,
            IntPtr source
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_get_parent(
            IntPtr node
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern PlistType plist_get_node_type(
            IntPtr node
        );

        // TODO - Caller frees val
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_key_val(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] ref string val
        );

        // TODO - caller frees val
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_string_val(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] ref string val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_bool_val(
            IntPtr node,
            [MarshalAs(UnmanagedType.U1)] ref bool val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_uint_val(
            IntPtr node,
            ref ulong val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_real_val(
            IntPtr node,
            ref double val
        );

        // TODO - Caller frees val
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_data_val(
            IntPtr node,
            ref byte[] val,
            ref int length
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_date_val(
            IntPtr node,
            ref int sec,
            ref int usec
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_get_uid_val(
            IntPtr node,
            ref ulong val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_key_val(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] string val
        );

        // TODO - function frees val
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_string_val(
            IntPtr node,
            [MarshalAs(UnmanagedType.LPStr)] string val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_bool_val(
            IntPtr node,
            [MarshalAs(UnmanagedType.U1)] bool val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_uint_val(
            IntPtr node,
            ulong val
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_real_val(
            IntPtr node,
            double val
        );

        // TODO: Warning - Library frees memory, what to do here?
        // TODO: In/Out 
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_data_val(
            IntPtr node,
            byte[] val,
            int length
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_date_val(
            IntPtr node,
            int sec,
            int usec
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_set_uid_val(
            IntPtr node,
            ulong val
        );

        // TODO - Caller frees memory
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_to_xml(
            IntPtr plist,
            out IntPtr plist_xml,
            out int length
        );

        // TODO - length can be out
        // TODO - Caller frees memory
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_to_bin(
            IntPtr plist,
            ref byte[] plist_bin,
            ref int length
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_from_xml(
            [MarshalAs(UnmanagedType.LPStr)] string plist_xml,
            int length,
            out IntPtr plist
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_from_xml(
            byte[] plist_xml,
            int length,
            out IntPtr plist
        );

        // TODO - plist_bin needs [In,Out] I think
        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void plist_from_bin(
            byte[] plist_bin,
            int length,
            out IntPtr plist
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr plist_access_path(
            IntPtr plist,
            uint length,
            __arglist
        );

        [DllImport("libplist.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool plist_compare_node_value(
            IntPtr node_l,
            IntPtr node_r
        );

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int memcmp(
            byte[] arr1,
            byte[] arr2,
            int length
        );

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void malloc(
            IntPtr buff
        );

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void free(
            IntPtr buff
        );

        internal IntPtr CreatePlistObject(object o)
        {
            if (o is string)
            {
                return plist_new_string((string)o);
            }
            else if (o is bool)
            {
                return plist_new_bool((bool)o);
            }
            else if (o is double || o is float)
            {
                return plist_new_real((double)o);
            }
            else if (o is DateTime)
            {
                var diff = (DateTime)o - new DateTime(2001, 1, 1);
                return plist_new_date((int)diff.TotalSeconds, (int)diff.TotalMilliseconds * 1000);
            }
            else if (o is byte[])
            {
                return plist_new_data((byte[])o, (uint)((byte[])o).Length);
            }
            return default(IntPtr);
        }
    }

    public sealed class PlistArray : PlistNative, IDisposable
    {
        private IntPtr m_Node;

        public PlistArray()
        {
            this.m_Node = plist_new_array();
        }

        internal PlistArray(IntPtr node)
        {
            this.m_Node = node;
        }

        public object this[int i]
        {
            get
            {
                return plist_array_get_item(
                    this.m_Node,
                    i);
            }
            set
            {
                plist_array_set_item(
                    this.m_Node,
                    CreatePlistObject(value),
                    i);
            }
        }

        public void Dispose()
        {
            if (this.m_Node != IntPtr.Zero)
            {
                plist_free(this.m_Node);
                this.m_Node = IntPtr.Zero;
            }
        }

        ~PlistArray()
        {
            Dispose();
        }
    }

    public class PlistEnumerator : PlistNative, IEnumerator
    {
        private IntPtr m_Node;
        private IntPtr m_Iter;
        private string m_CurrentKey;
        private IntPtr m_CurrentVal;

        public PlistEnumerator(IntPtr node)
        {
            m_Node = node;
            plist_dict_new_iter(node, out m_Iter);
        }

        public object Current { get { return m_CurrentKey; } }

        public bool MoveNext()
        {
            IntPtr valPtr;
            m_CurrentVal = IntPtr.Zero;
            
            plist_dict_next_item(this.m_Node, m_Iter, out valPtr, out m_CurrentVal);

            if (valPtr == IntPtr.Zero)
                return false;

            m_CurrentKey = Marshal.PtrToStringAnsi(valPtr);
            free(valPtr);

            if (m_CurrentKey == null || m_CurrentVal == IntPtr.Zero)
                return false;

            return true;
        }

        public void Reset()
        {
            if (m_Iter != IntPtr.Zero)
                free(m_Iter);
            m_Iter = IntPtr.Zero;
        }

        ~PlistEnumerator()
        {
            if (m_Iter != IntPtr.Zero)
                free(m_Iter);
            m_Iter = IntPtr.Zero;
        }
    }

    public sealed partial class LibPlist : PlistNative, IEnumerable, IDisposable
    {
        static byte[] BINARY_PLIST = new[] { (byte)'b', (byte)'p', (byte)'l', (byte)'i', (byte)'s', (byte)'t', (byte)'0', (byte)'0' };

        private IntPtr m_Node;

        public LibPlist()
        {
            this.m_Node = plist_new_dict();
        }

        private LibPlist(IntPtr node)
        {
            this.m_Node = node;
        }

        public static LibPlist FromFile(Stream strm)
        {
            byte[] data;
            using (var memStream = new MemoryStream())
            {
                strm.CopyTo(memStream);
                data = memStream.ToArray();
            }

            if (memcmp(data, BINARY_PLIST, 8) == 0)
            {
                IntPtr plistPtr;
                plist_from_bin(data, data.Length, out plistPtr);
                return new LibPlist(plistPtr);
            }
            else
            {
                IntPtr plistPtr;
                plist_from_xml(data, data.Length, out plistPtr);
                return new LibPlist(plistPtr);
            }
        }
        
        public static IntPtr GetPtrPlistFromData(byte[] data)
        {
            /*
            byte[] data;
            using (var memStream = new MemoryStream())
            {
                strm.CopyTo(memStream);
                data = memStream.ToArray();
                string sdata = System.Text.Encoding.UTF8.GetString(data);
            }
            */
            string sdata = System.Text.Encoding.UTF8.GetString(data);
            if (memcmp(data, BINARY_PLIST, 8) == 0)
            {
                IntPtr plistPtr;
                plist_from_bin(data, data.Length, out plistPtr);
                return plistPtr;
            }
            else
            {
                IntPtr plistPtr;
                plist_from_xml(data, data.Length, out plistPtr);
                return plistPtr;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new PlistEnumerator(this.m_Node);
        }

        public void Dispose()
        {
            if (this.m_Node != IntPtr.Zero)
            {
                plist_free(this.m_Node);
                this.m_Node = IntPtr.Zero;
            }
        }

        ~LibPlist()
        {
            Dispose();
        }
    }
}