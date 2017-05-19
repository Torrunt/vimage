using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace vimage
{
    class WindowsFileSorting
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);

        [System.Security.SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern int StrCmpLogicalW(string psz1, string psz2);
        }

        public sealed class NaturalStringComparer : IComparer<string>
        {
            public int Compare(string a, string b) { return SafeNativeMethods.StrCmpLogicalW(a, b); }
        }

        public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo a, FileInfo b) { return SafeNativeMethods.StrCmpLogicalW(a.Name, b.Name); }
        }
    }
}
