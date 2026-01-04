using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using vimage.Common;

namespace vimage.Utils
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
            public int Compare(string? a, string? b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a ?? "", b ?? "");
            }
        }

        public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo>
        {
            public int Compare(FileInfo? a, FileInfo? b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a?.Name ?? "", b?.Name ?? "");
            }
        }

        public static string? GetWindowsSortOrder(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (directory is null)
                return null;
            var parentFolder = Path.GetFileName(directory);
            if (parentFolder is null)
                return null;

            var shellWindowsType = Type.GetTypeFromProgID("Shell.Application");
            if (shellWindowsType is null)
                return null;
            dynamic? shell = Activator.CreateInstance(shellWindowsType);
            if (shell is null)
                return null;
            foreach (var window in shell.Windows())
            {
                dynamic? view = window.Document;
                if (view is null)
                    continue;

                var folderPath = view.Folder?.Self?.Path;
                if (string.IsNullOrEmpty(folderPath))
                    continue;
                if (!string.Equals(folderPath, directory, StringComparison.OrdinalIgnoreCase))
                    continue;

                string sortColumns = view.SortColumns;

                // can be sorted by multiple columns (eg: date then name) - just return first one
                int firstSemi = sortColumns.IndexOf(';');
                string firstProp = sortColumns[5..firstSemi]; // strip off "prop:" prefix

                return firstProp;
            }
            return null;
        }

        public static (SortBy, SortDirection) GetSorting(
            SortBy sortBy,
            SortDirection sortDir,
            string file
        )
        {
            if (file == "" || (sortBy != SortBy.Folder && sortDir != SortDirection.Folder))
                return (SortBy.Name, SortDirection.Ascending);

            // Get sort column info from window with corresponding name
            var sort = GetWindowsSortOrder(file);
            if (sort is null)
                return (SortBy.Name, SortDirection.Ascending);

            // Direction
            if (sort[0] == '-')
            {
                sort = sort[1..];

                if (sortDir == SortDirection.Folder)
                    sortDir = SortDirection.Descending;
            }
            else if (sortDir == SortDirection.Folder)
                sortDir = SortDirection.Ascending;

            // By
            if (sortBy == SortBy.Folder)
            {
                sortBy = sort switch
                {
                    "System.ItemDate" => SortBy.Date,
                    "System.DateModified" => SortBy.DateModified,
                    "System.DateCreated" => SortBy.DateCreated,
                    "System.Size" => SortBy.Size,
                    _ => SortBy.Name,
                };
            }

            return (sortBy, sortDir);
        }
    }
}
