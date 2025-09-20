using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace vimage
{
    internal partial class WindowsFileSorting
    {
        [LibraryImport("shlwapi.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int StrCmpLogicalW(string psz1, string psz2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        internal static partial class SafeNativeMethods
        {
            [LibraryImport("shlwapi.dll", StringMarshalling = StringMarshalling.Utf16)]
            public static partial int StrCmpLogicalW(string psz1, string psz2);
        }

        public sealed class NaturalStringComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a, b);
            }
        }

        public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo a, FileInfo b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a.Name, b.Name);
            }
        }
    }
}
