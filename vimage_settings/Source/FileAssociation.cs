using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Security;
using System.Security.AccessControl;
using System.IO;

/* Written by Whisperity
 * Inspired by Aidan Follestad's C# FileAssociation Class
 * (http://www.codeproject.com/Articles/43675/C-FileAssociation-Class)
 * [Nice job, sir! Though you could've gone a little less harsh on permission requirements.]
 */

namespace vimage_settings
{
    namespace Association
    {
        internal class FileAssocation
        {
            public readonly string Extension;

            public FileAssocation(string extension)
            {
                this.Extension = extension;
            }

            private const string Default_OpenAs = @"""C:\Windows\system32\rundll32.exe " +
                @"C:\Windows\system32\shell32.dll,OpenAs_RunDLL"" ""%1""";

            public void Create(string progID)
            {
                // Creates the skeleton of the registry keys for an extension
                if (!this.Registered)
                {
                    try
                    {
                        RegistryKey extKey = Registry.ClassesRoot.CreateSubKey(Extension,
                            RegistryKeyPermissionCheck.ReadWriteSubTree);

                        MakeProgID(progID);

                        // The link to the ProgID key
                        extKey.SetValue(String.Empty, progID, RegistryValueKind.String);
                        extKey.Close(); extKey.Dispose();
                    }
                    catch (SecurityException e)
                    {
                        throw new FieldAccessException("Failed to open the registry for writing.", e);
                    }
                }
                else
                    throw new InvalidOperationException("This extension is already registered, there is no need to Create() it.");
            }

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
                    comKey.Close(); comKey.Dispose();

                    openKey.SetValue(String.Empty, @"&Open", RegistryValueKind.String);
                    openKey.SetValue(@"FriendlyAppName", "Open self");
                    openKey.Close(); openKey.Dispose();

                    shellKey.SetValue(String.Empty, @"open", RegistryValueKind.String);
                    shellKey.Close(); shellKey.Dispose();

                    progIDKey.Close(); progIDKey.Dispose();
                }
                catch (SecurityException e)
                {
                    throw new FieldAccessException("Failed to open the registry for writing.", e);
                }
            }
            
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

                        extKey.Close(); extKey.Dispose();
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

                            praKey.Close(); praKey.Dispose();
                        }

                        hkcu_ExtsKey.Close(); hkcu_ExtsKey.Dispose();
                    }
                }

                return retVal;
            }

            public void MakeUserChoice()
            {
                // Set the extension's user choice (in the HKCU) to the HKCR (system-wide) value.
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

                                    userChoiceKey.Close(); userChoiceKey.Dispose();
                                }

                                extKey.Close(); extKey.Dispose();
                            }

                            hkcu_ExtsRoot.Close(); hkcu_ExtsRoot.Dispose();
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

            public void ResetUserChoice()
            {
                // Remove the user's HKCU choice to let the system fall-back to the default one.
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

                                extKey.Close(); extKey.Dispose();
                            }

                            hkcu_ExtsRoot.Close(); hkcu_ExtsRoot.Dispose();
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

                                praKey.Close(); praKey.Dispose();
                            }
                        }

                        extKey.Close(); extKey.Dispose();
                    }

                    return (extensionKeyExists && programAssociationKeyExists);
                }
            }

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

                                praKey.Close(); praKey.Dispose();
                            }

                            extKey.Close(); extKey.Dispose();
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

                                praKey.Close(); praKey.Dispose();
                            }

                            extKey.Close(); extKey.Dispose();
                        }
                    }
                    else
                        throw new NullReferenceException("The extension is not registered.");
                }
            }

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

                                iconKey.Close(); iconKey.Dispose();
                            }

                            praKey.Close(); praKey.Dispose();
                        }
                        extKey.Close(); extKey.Dispose();
                    }
                }

                return icon;
            }

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

                                iconKey.Close(); iconKey.Dispose();
                            }

                            praKey.Close(); praKey.Dispose();
                        }

                        extKey.Close(); extKey.Dispose();
                    }
                }
                else
                    throw new NullReferenceException("The extension is not registered.");
            }

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

            public bool IconValid
            {
                get
                {
                    // Usually, executable-resource links are in this format: C:\Path_to_exe\exe.exe,123.
                    string iconPath = this.Icon;
                    string[] iconPathParts = iconPath.Split(',');
                    if (iconPathParts.Length > 1)
                        iconPath = iconPathParts[0];

                    FileInfo iconInfo = new FileInfo(iconPath);

                    if (iconInfo != null && iconInfo.Exists)
                        return true;
                    else
                        return false;
                }
            }

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

                                    praOpenCommandKey.Close(); praOpenCommandKey.Dispose();
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

                                            firstSubCommandKey.Close(); firstSubCommandKey.Dispose();
                                        }
                                    }
                                }

                                praShellKey.Close(); praShellKey.Dispose();
                            }

                            praKey.Close(); praKey.Dispose();
                        }

                        extKey.Close(); extKey.Dispose();
                    }
                }

                return exec;
            }

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

                                        commandKey.Close(); commandKey.Dispose();

                                        // Set values for the "Open" option
                                        openKey.SetValue(String.Empty, openName, RegistryValueKind.String);
                                        openKey.SetValue(@"FriendlyAppName", friendlyAppName, RegistryValueKind.String);

                                        openKey.Close(); openKey.Dispose();
                                    }

                                    shellKey.SetValue(String.Empty, @"open", RegistryValueKind.String);

                                    shellKey.Close(); shellKey.Dispose();
                                }
                            }
                            catch (SecurityException e)
                            {
                                throw new FieldAccessException("Failed to open the registry for writing.", e);
                            }

                            praKey.Close(); praKey.Dispose();
                        }

                        extKey.Close(); extKey.Dispose();
                    }
                }
                else
                    throw new NullReferenceException("The extension is not registered.");
            }

            public string Executable
            {
                get
                {
                    return this.GetExecutable(false);
                }
            }

            // This separate property must exist, because users CAN specify their own "open with" executable
            // outside HKCR, at HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts
            public string UserExecutable
            {
                get
                {
                    return this.GetExecutable(true);
                }
            }

            public bool ExecutableValid
            {
                get
                {
                    // Usually, executable comamnds are like this: "C:\Path\Executable.exe" "%1"
                    string exeCmd = this.Executable;
                    string[] cmdParts = exeCmd.Split('"');
                    if (cmdParts.Length > 2)
                        exeCmd = cmdParts[1];

                    FileInfo exeInfo = new FileInfo(exeCmd);

                    if (exeInfo != null && exeInfo.Exists)
                        return true;
                    else
                        return false;
                }
            }
        }
    }
}
