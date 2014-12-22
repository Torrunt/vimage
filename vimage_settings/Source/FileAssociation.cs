using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using Microsoft.Win32;

/* Written by Whisperity
 * Inspired by Aidan Follestad's C# FileAssociation Class
 * (http://www.codeproject.com/Articles/43675/C-FileAssociation-Class)
 * [Nice job, sir! Though you could've gone a little less harsh on permission requirements.]
 */

namespace vimage_settings
{
    /// <summary>
    /// A Windows file association
    /// </summary>
    internal class FileAssociation
    {
        /// <summary>
        /// The extension to handle.
        /// </summary>
        public readonly string Extension;

        /// <summary>
        /// Creates an instance of a file association
        /// </summary>
        /// <param name="extension">The extension the object should handle (include the prefix . character!)</param>
        public FileAssociation(string extension)
        {
            this.Extension = extension;
        }

        /// <summary>
        /// The default application executed for unknown files
        /// </summary>
        private const string Default_OpenAs = @"""C:\Windows\system32\rundll32.exe " +
            @"C:\Windows\system32\shell32.dll,OpenAs_RunDLL"" ""%1""";

        /// <summary>
        /// Create a skeleton of the association registry keys with the provided ProgID (file type name)
        /// </summary>
        /// <param name="progID">The ProgID (file type) to use (for example: Image.Bitmap for .bmp files)</param>
        public void Create(string progID)
        {
            if (!this.Registered)
            {
                try
                {
                    RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(Extension,
                        RegistryKeyPermissionCheck.ReadWriteSubTree);

                    MakeProgID(progID);

                    // The link to the ProgID key
                    extKey.SetValue(String.Empty, progID, RegistryValueKind.String);
                    extKey.Close();
                }
                catch (SecurityException e)
                {
                    throw new FieldAccessException("Failed to open the registry for writing.", e);
                }
            }
            else
                throw new InvalidOperationException("This extension is already registered, there is no need to Create() it.");
        }

        #region ProgID (file type) of the association
        /// <summary>
        /// Gets or sets the extension's system-wide ProgID (file type)
        /// </summary>
        public string ProgID
        {
            get
            {
                return this.GetAssociatedProgID(false);
            }
            set
            {
                this.SetProgID(value, false);
            }
        }

        /// <summary>
        /// Gets or sets the extension's ProgID (file type) for the current user
        /// </summary>
        public string UserProgID
        {
            get
            {
                return this.GetAssociatedProgID(true);
            }
            set
            {
                this.SetProgID(value, true);
            }
        }

        /// <summary>
        /// Create a skeleton of a ProgID (file type) key tree
        /// </summary>
        /// <param name="progID">The ProgID (file type) to create the keys for</param>
        public static void MakeProgID(string progID)
        {
            try
            {
                RegistryKey progIDKey = Registry.ClassesRoot.CreateSubKey(progID,
                        RegistryKeyPermissionCheck.ReadWriteSubTree);

                progIDKey.SetValue(String.Empty, progID + " file", RegistryValueKind.String);
                progIDKey.CreateSubKey(@"DefaultIcon");

                RegistryKey shellKey = progIDKey.CreateSubKey(@"shell");
                RegistryKey openKey = shellKey.CreateSubKey(@"open");
                RegistryKey comKey = openKey.CreateSubKey(@"command");

                comKey.SetValue(String.Empty, Default_OpenAs, RegistryValueKind.String);
                comKey.Close();

                openKey.SetValue(String.Empty, @"&Open", RegistryValueKind.String);
                openKey.SetValue(@"FriendlyAppName", "Open self");
                openKey.Close();

                shellKey.SetValue(String.Empty, @"open", RegistryValueKind.String);
                shellKey.Close();

                progIDKey.Close();
            }
            catch (SecurityException e)
            {
                throw new FieldAccessException("Failed to open the registry for writing.", e);
            }
        }

        /// <summary>
        /// Get the extension's associated ProgID (file type)
        /// </summary>
        /// <param name="userspaceProgID">True if the user's choice (per-account) ProgID should be accessed instead of the system-wide settings</param>
        private string GetAssociatedProgID(bool userspaceProgID)
        {
            string retVal = String.Empty;

            // Get the "kernel space" (HKCR) associated program ID.
            if (!userspaceProgID)
            {
                RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                    RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
                if (extKey != null)
                {
                    string defValue = (string)extKey.GetValue(String.Empty);
                    if (!String.IsNullOrEmpty(defValue))
                        retVal = defValue;

                    extKey.Close();
                }
            }
            else
            {
                // Get userspace associated prog ID
                RegistryKey hkcu_ExtsKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + Extension,
                    RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                if (hkcu_ExtsKey != null)
                {
                    RegistryKey praKey = hkcu_ExtsKey.OpenSubKey(@"UserChoice",
                        RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                    if (praKey != null)
                    {
                        string defValue = (string)praKey.GetValue("Progid");
                        if (!String.IsNullOrEmpty(defValue))
                            retVal = defValue;

                        praKey.Close();
                    }

                    hkcu_ExtsKey.Close();
                }

                // If there is no user-choice association set, fallback to the system-level setting
                // (Just as how Windows does)
                retVal = GetAssociatedProgID(false);
            }

            return retVal;
        }

        /// <summary>
        /// Sets the extension's associated ProgID (file type)
        /// </summary>
        /// <param name="progID">The new ProgID (file type) to set</param>
        /// <param name="userspaceProgID">True if the user's choice (per-account) ProgID should be accessed instead of the system-wide settings</param>
        private void SetProgID(string progID, bool userspaceProgID)
        {
            if (this.Registered)
            {
                try
                {
                    if (!userspaceProgID)
                    {
                        RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(Extension,
                                RegistryKeyPermissionCheck.ReadWriteSubTree);

                        // The link to the ProgID key
                        extKey.SetValue(String.Empty, progID, RegistryValueKind.String);
                        extKey.Close();
                    }
                    else
                    {
                        RegistryKey hkcu_ExtsKey = Registry.CurrentUser.OpenSubKey(
                            @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + Extension,
                            RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                        if (hkcu_ExtsKey != null)
                        {
                            RegistryKey praKey = hkcu_ExtsKey.CreateSubKey(@"UserChoice",
                                RegistryKeyPermissionCheck.ReadWriteSubTree);

                            if (praKey != null)
                            {
                                praKey.SetValue(String.Empty, progID, RegistryValueKind.String);

                                praKey.Close();
                            }

                            hkcu_ExtsKey.Close();
                        }
                    }
                }
                catch (SecurityException e)
                {
                    throw new FieldAccessException("Failed to open the registry for writing.", e);
                }
            }
            else
                throw new NullReferenceException("The extension is not registered.");
        }

        /// <summary>
        /// Copies the default (system-wide) ProgID (file type) to the user's personal preferences
        /// </summary>
        public void MakeUserChoice()
        {
            if (this.Registered)
            {
                try
                {
                    RegistryKey hkcu_ExtsRoot = Registry.CurrentUser.OpenSubKey(
                           @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts",
                           RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                    if (hkcu_ExtsRoot != null)
                    {
                        // Creates (or write-access opens) the extension's key.
                        RegistryKey extKey = hkcu_ExtsRoot.CreateSubKey(Extension,
                            RegistryKeyPermissionCheck.ReadWriteSubTree);

                        if (extKey != null)
                        {
                            RegistryKey userChoiceKey = extKey.CreateSubKey(@"UserChoice",
                                RegistryKeyPermissionCheck.ReadWriteSubTree);

                            if (userChoiceKey != null)
                            {
                                userChoiceKey.SetValue("Progid", this.GetAssociatedProgID(false), RegistryValueKind.String);

                                userChoiceKey.Close();
                            }

                            extKey.Close();
                        }

                        hkcu_ExtsRoot.Close();
                    }
                }
                catch (SecurityException e)
                {
                    throw new FieldAccessException("Failed to open the registry for writing.", e);
                }
            }
            else
                throw new NullReferenceException("The extension is not registered.");
        }

        /// <summary>
        /// Deletes the user's personal preference about the extension's associated ProgID (file type)
        /// This will usually make the system fall back to the default (system-wide) setting
        /// </summary>
        public void ResetUserChoice()
        {
            if (this.Registered)
            {
                try
                {
                    RegistryKey hkcu_ExtsRoot = Registry.CurrentUser.OpenSubKey(
                           @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts",
                           RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                    if (hkcu_ExtsRoot != null)
                    {
                        // Creates (or write-access opens) the extension's key.
                        RegistryKey extKey = hkcu_ExtsRoot.CreateSubKey(Extension,
                            RegistryKeyPermissionCheck.ReadWriteSubTree);

                        if (extKey != null)
                        {
                            extKey.DeleteSubKeyTree(@"UserChoice", false);

                            extKey.Close();
                        }

                        hkcu_ExtsRoot.Close();
                    }
                }
                catch (SecurityException e)
                {
                    throw new FieldAccessException("Failed to open the registry for writing.", e);
                }
            }
            else
                throw new NullReferenceException("The extension is not registered.");
        }
        #endregion

        /// <summary>
        /// Deletes the association's file extension or the associated ProgID (file type) base on the specified parameters
        /// </summary>
        /// <param name="ext"></param>
        /// <param name="progID"></param>
        public void Delete(bool ext, bool progID)
        {
            if (this.Registered)
            {
                try
                {
                    string pra = GetAssociatedProgID(false);

                    if (ext)
                        Registry.ClassesRoot.DeleteSubKeyTree(Extension, false);

                    if (progID)
                        Registry.ClassesRoot.DeleteSubKeyTree(pra, false);
                }
                catch (Exception e)
                {
                    throw new FieldAccessException("Failed to delete the extension.", e);
                }
            }
            else
                throw new NullReferenceException("The extension is not registered.");
        }

        /// <summary>
        /// Gets whether the current extension is registered in the system (as in: the extension keys and whatnot exists)
        /// </summary>
        public bool Registered
        {
            get
            {
                bool extensionKeyExists = false;
                bool programAssociationKeyExists = false;

                // Attempt to find the extension itself in the Registry
                RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                    RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
                if (extKey != null)
                {
                    extensionKeyExists = true;

                    string pra = GetAssociatedProgID(false);
                    if (!String.IsNullOrEmpty(pra))
                    {
                        RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                            RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                        if (praKey != null)
                        {
                            programAssociationKeyExists = true;

                            praKey.Close();
                        }
                    }

                    extKey.Close();
                }

                return (extensionKeyExists && programAssociationKeyExists);
            }
        }

        /// <summary>
        /// Gets or sets the extension's file type name (like "Text Document" for .txt files)
        /// </summary>
        public string Description
        {
            get
            {
                string desc = String.Empty;

                if (this.Registered)
                {
                    RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                            RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                    if (extKey != null)
                    {
                        string pra = GetAssociatedProgID(false);
                        RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                            RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                        if (praKey != null)
                        {
                            string defValue = (string)praKey.GetValue(String.Empty);
                            if (!String.IsNullOrEmpty(defValue))
                                desc = defValue;

                            praKey.Close();
                        }

                        extKey.Close();
                    }
                }

                return desc;
            }
            set
            {
                if (this.Registered)
                {
                    RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                            RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                    if (extKey != null)
                    {
                        string pra = GetAssociatedProgID(false);
                        RegistryKey praKey;
                        try
                        {
                            praKey = Registry.ClassesRoot.OpenSubKey(pra,
                                RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.WriteKey);
                        }
                        catch (SecurityException e)
                        {
                            throw new FieldAccessException("Failed to open the registry for writing.", e);
                        }

                        if (praKey != null)
                        {
                            praKey.SetValue(String.Empty, value, RegistryValueKind.String);
                            praKey.SetValue(@"FriendlyTypeName", value, RegistryValueKind.String);

                            praKey.Close();
                        }

                        extKey.Close();
                    }
                }
                else
                    throw new NullReferenceException("The extension is not registered.");
            }
        }

        #region Icon
        /// <summary>
        /// Gets or sets the file association's system-wide icon
        /// </summary>
        public string Icon
        {
            get
            {
                return this.GetIcon(false);
            }
            set
            {
                this.SetIcon(value, false);
            }
        }

        /// <summary>
        /// Gets or sets the file assoication's icon for the current user
        /// </summary>
        public string UserIcon
        {
            get
            {
                return this.GetIcon(true);
            }
            set
            {
                this.SetIcon(value, true);
            }
        }

        /// <summary>
        /// Gets whether the known icon is a valid file
        /// </summary>
        public bool IconValid
        {
            get
            {
                // Usually, executable-resource links are in this format: C:\Path_to_exe\exe.exe,123.
                string iconPath = this.Icon;
                string[] iconPathParts = iconPath.Split(',');
                if (iconPathParts.Length > 1)
                    iconPath = iconPathParts[0];

                return ValidChecker(iconPath);
            }
        }

        /// <summary>
        /// Gets whether the known icon (for the current account) is a valid file
        /// </summary>
        public bool UserIconValid
        {
            get
            {
                string uiconPath = this.UserIcon;
                string[] uiconPathParts = uiconPath.Split(',');
                if (uiconPathParts.Length > 1)
                    uiconPath = uiconPathParts[0];

                return ValidChecker(uiconPath);
            }
        }

        /// <summary>
        /// Gets the icon path for the file association
        /// </summary>
        /// <param name="userspaceProgID">True if the user's choice (per-account) ProgID should be accessed instead of the system-wide settings</param>
        private string GetIcon(bool userspaceProgID)
        {
            string icon = String.Empty;

            if (this.Registered)
            {
                RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                        RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                if (extKey != null)
                {
                    string pra = GetAssociatedProgID(userspaceProgID);
                    RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                        RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                    if (praKey != null)
                    {
                        RegistryKey iconKey = praKey.OpenSubKey(@"DefaultIcon",
                            RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                        if (iconKey != null)
                        {
                            string iconPath = (string)iconKey.GetValue(String.Empty);
                            if (!String.IsNullOrEmpty(iconPath))
                                icon = iconPath;

                            iconKey.Close();
                        }

                        praKey.Close();
                    }
                    extKey.Close();
                }
            }

            return icon;
        }

        /// <summary>
        /// Sets the icon path for the file association
        /// </summary>
        /// <param name="iconPath">The full icon path (this includes the resource-link format) to be set</param>
        /// <param name="userspaceProgID">True if the user's choice (per-account) ProgID should be accessed instead of the system-wide settings</param>
        private void SetIcon(string iconPath, bool userspaceProgID)
        {
            if (this.Registered)
            {
                RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                        RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (extKey != null)
                {
                    string pra = GetAssociatedProgID(userspaceProgID);
                    RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                                RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                    if (praKey != null)
                    {
                        RegistryKey iconKey;

                        try
                        {
                            iconKey = praKey.CreateSubKey(@"DefaultIcon",
                                RegistryKeyPermissionCheck.ReadWriteSubTree);
                        }
                        catch (SecurityException e)
                        {
                            throw new FieldAccessException("Failed to open the registry for writing.", e);
                        }

                        if (iconKey != null)
                        {
                            iconKey.SetValue(String.Empty, iconPath, RegistryValueKind.String);

                            iconKey.Close();
                        }

                        praKey.Close();
                    }

                    extKey.Close();
                }
            }
            else
                throw new NullReferenceException("The extension is not registered.");
        }
        #endregion

        #region Opening executable
        /// <summary>
        /// Gets the file association's default execute command
        /// </summary>
        public string ExecuteCommand
        {
            get
            {
                return this.GetExecutable(false);
            }
        }

        /// <summary>
        /// Gets the file association's default execute command (for the current user)
        /// </summary>
        public string UserExecuteCommand
        {
            get
            {
                return this.GetExecutable(true);
            }
        }

        /// <summary>
        /// Gets the file association's executing application's path
        /// </summary>
        public string ExecutablePath
        {
            get
            {
                return this.ExecuteCommand.Replace(@"""", String.Empty).Replace(@"%1", String.Empty);
            }
        }

        /// <summary>
        /// Gets the file association's executing application's path (for the current user)
        /// </summary>
        public string UserExecutablePath
        {
            get
            {
                return this.UserExecuteCommand.Replace(@"""", String.Empty).Replace(@"%1", String.Empty);
            }
        }

        /// <summary>
        /// Gets whether the known executable is a valid file
        /// </summary>
        public bool ExecutableValid
        {
            get
            {
                return ValidChecker(ExecutablePath);
            }
        }

        /// <summary>
        /// Gets whether the known executable (for the current account) is a valid file
        /// </summary>
        public bool UserExecutableValid
        {
            get
            {
                return ValidChecker(UserExecutablePath);
            }
        }

        /// <summary>
        /// Gets the default execute command for the file association
        /// </summary>
        /// <param name="userspaceProgID">True if the user's choice (per-account) ProgID should be accessed instead of the system-wide settings</param>
        private string GetExecutable(bool userspaceProgID)
        {
            string exec = String.Empty;

            if (this.Registered)
            {
                RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                        RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                if (extKey != null)
                {
                    string pra = GetAssociatedProgID(userspaceProgID);
                    RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                        RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                    if (praKey != null)
                    {
                        RegistryKey praShellKey = praKey.OpenSubKey(@"shell",
                            RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                        if (praShellKey != null)
                        {
                            RegistryKey praOpenCommandKey = praShellKey.OpenSubKey(@"open\command",
                                RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                            if (praOpenCommandKey != null)
                            {
                                string exeCommand = (string)praOpenCommandKey.GetValue(String.Empty);
                                if (!String.IsNullOrEmpty(exeCommand))
                                    exec = exeCommand;

                                praOpenCommandKey.Close();
                            }
                            else
                            {
                                // If there is no open subcommand, we load the first subcommand of "shell"
                                if (praShellKey.GetSubKeyNames().Length > 0)
                                {
                                    RegistryKey firstSubCommandKey = praShellKey.OpenSubKey(
                                        praShellKey.GetSubKeyNames()[0] + @"\command",
                                        RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                                    if (firstSubCommandKey != null)
                                    {
                                        string exeCommand = (string)firstSubCommandKey.GetValue(String.Empty);
                                        if (!String.IsNullOrEmpty(exeCommand))
                                            exec = exeCommand;

                                        firstSubCommandKey.Close();
                                    }
                                }
                            }

                            praShellKey.Close();
                        }

                        praKey.Close();
                    }

                    extKey.Close();
                }
            }

            return exec;
        }

        /// <summary>
        /// Sets the file association's default execute command
        /// </summary>
        /// <param name="exeCmd">The full command to execute (include a "%1" (with quotes) in the string literal to have the executing application get the opened file's path)</param>
        /// <param name="openName">The name of the open command that should be shown</param>
        /// <param name="friendlyAppName">The name of the opening executable that should be shown</param>
        /// <param name="userspaceProgID">True if the user's choice (per-account) ProgID should be accessed instead of the system-wide settings</param>
        public void SetExecutable(string exeCmd, string openName, string friendlyAppName, bool userspaceProgID)
        {
            if (this.Registered)
            {
                RegistryKey extKey = Registry.ClassesRoot.OpenSubKey(Extension,
                        RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (extKey != null)
                {
                    string pra = GetAssociatedProgID(userspaceProgID);
                    RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                                RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                    if (praKey != null)
                    {
                        try
                        {
                            RegistryKey shellKey = praKey.CreateSubKey(@"shell",
                                RegistryKeyPermissionCheck.ReadWriteSubTree);

                            if (shellKey != null)
                            {
                                RegistryKey openKey = shellKey.CreateSubKey(@"open",
                                    RegistryKeyPermissionCheck.ReadWriteSubTree);

                                if (openKey != null)
                                {
                                    // Set the command
                                    RegistryKey commandKey = openKey.CreateSubKey(@"command",
                                        RegistryKeyPermissionCheck.ReadWriteSubTree);

                                    commandKey.SetValue(String.Empty, exeCmd, RegistryValueKind.String);

                                    commandKey.Close();

                                    // Set values for the "Open" option
                                    openKey.SetValue(String.Empty, openName, RegistryValueKind.String);
                                    openKey.SetValue(@"FriendlyAppName", friendlyAppName, RegistryValueKind.String);

                                    openKey.Close();
                                }

                                shellKey.SetValue(String.Empty, @"open", RegistryValueKind.String);

                                shellKey.Close();
                            }
                        }
                        catch (SecurityException e)
                        {
                            throw new FieldAccessException("Failed to open the registry for writing.", e);
                        }

                        praKey.Close();
                    }

                    extKey.Close();
                }
            }
            else
                throw new NullReferenceException("The extension is not registered.");
        }
        #endregion

        /// <summary>
        /// Checks whether the given path is a valid file
        /// </summary>
        /// <param name="path">The file path to check</param>
        private bool ValidChecker(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return (fileInfo != null && fileInfo.Exists);
        }
    }
}

/* Written by Whisperity
 * Taken directly from my other project, SharpGMad
 * A cut down version of
 * https://github.com/whisperity/SharpGMad/blob/edeb1b7ad134f1af6c350db27bc19f20a665a89c/SharpGMad/Extensions.cs@116:289
 * 
 * A handy way with WinAPI to get the icon of a file extension via the Windows shell.
 */
namespace SharpGMad
{
    /// <summary>
    /// Provides methods to retrieve information for file associations
    /// </summary>
    static class FileAssocation
    {
        /// <summary>
        /// Gets the icon (using a Windows API call) of the file specified.
        /// </summary>
        /// <param name="path">The path of the file we need the icon of</param>
        /// <returns>An System.Drawing.Icon object if the icon exists or null.</returns>
        public static Icon GetIcon(string path)
        {
            Icon retIcon = null;

            // Small icon
            ShFileInfo small = new ShFileInfo();
            ShGetFileInfoAttributes callSmall = ShGetFileInfoAttributes.Icon | ShGetFileInfoAttributes.UseFileAttributes |
                ShGetFileInfoAttributes.SmallIcon | ShGetFileInfoAttributes.SysIconIndex;

            ShGetFileInfo(path, (uint)System.IO.FileAttributes.Normal, ref small,
                (uint)Marshal.SizeOf(small), (uint)callSmall);

            // Copy the retrieved icon into a local resource to disconnect it from the external one.
            if (small.hIcon != IntPtr.Zero && small.iIcon != 0)
                retIcon = (Icon)Icon.FromHandle(small.hIcon).Clone();

            DestroyIcon(small.hIcon);
            return retIcon;
        }

        /// <summary>
        /// Releases the resources of the system icon pointed by the hIcon value.
        /// </summary>
        /// <param name="hIcon">The icon's handle pointer</param>
        /// <returns>Zero if failed, non-zero if successful.</returns>
        [DllImport("user32.dll", EntryPoint = "DestroyIcon", SetLastError = true)]
        private static extern int DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// The implementation of an SHFILEINFO struct containing information about a file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct ShFileInfo
        {
            /// <summary>
            /// The pointer to the handle of the file's icon.
            /// </summary>
            public IntPtr hIcon;

            /// <summary>
            /// The index of the file's icon within the system image list.
            /// (This seems to be 0 if an item could not be generated for the file.)
            /// </summary>
            public int iIcon;

            /// <summary>
            /// An array of values that indicate attributes of the file.
            /// Unused.
            /// </summary>
            public uint dwAttributes;

            /// <summary>
            /// A string that contains the name of the file as it appears in the Windows Shell,
            /// or the path and file name of the file that contains the icon representing the file.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            /// <summary>
            /// A string that describes the type of file. (For example: "Text Document" for .txt files.)
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        /// <summary>
        /// A list of attributes that can be used to fine tune ShGetFileInfo().
        /// </summary>
        [Flags]
        private enum ShGetFileInfoAttributes : uint
        {
            /// <summary>Populate ShFileInfo.hIcon with the file's icon's handle and ShFileInfo.iIcon with its index.</summary>
            Icon = 0x000000100,
            /// <summary>Retrieves the index of a system image list icon. If successful, ShFileInfo.iIcon is populated with the value.</summary>
            SysIconIndex = 0x000004000,
            /// <summary>Adds the little file link arrow overlaid on the icon if Icon is set.</summary>
            /// <summary>If Icon is set, the retrieved icon will be a small (16x16) icon.
            /// If SysIconIndex is set, the call will return a handle to the system image list of small icons. (?)</summary>
            SmallIcon = 0x000000001,
            /// <summary>If set, the call will not attempt to access the file at the specified path.
            /// Instead, it will act like the file path given exists with the attributes given in dwFileAttributes.</summary>
            UseFileAttributes = 0x000000010,
        }

        /// <summary>
        /// Calls the external SHGetFileInfo method of Shell32.dll to retrieve information about a file.
        /// </summary>
        /// <param name="pszPath">The file to retrieve information about.</param>
        /// <param name="dwFileAttributes">File attribute flags (?, unused)</param>
        /// <param name="psfi">The structure where certain values will be populated accordingly.</param>
        /// <param name="cbFileInfo">The size of the structure passed at psfi.</param>
        /// <param name="uFlags">Flags from SHGetFileInfoAttributes fine-tuning the behaviour of the call.</param>
        /// <returns>Some sort of a pointer... (?)</returns>
        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr ShGetFileInfo(string pszPath, uint dwFileAttributes, ref ShFileInfo psfi,
            uint cbFileInfo, uint uFlags);
    }
}