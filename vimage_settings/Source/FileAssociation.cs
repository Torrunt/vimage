using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Windows.Forms;
using Microsoft.Win32;
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
            // TODO: this should be private
            public string GetProgramAssociationName(bool userspaceProgID)
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

                        string pra = GetProgramAssociationName(false);
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
                            string pra = GetProgramAssociationName(false);
                            RegistryKey praKey = Registry.ClassesRoot.OpenSubKey(pra,
                                RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                            if (praKey != null)
                            {
                                string defValue = (string)praKey.GetValue(String.Empty);
                                if (!String.IsNullOrEmpty(defValue))
                                    desc = defValue;

                                praKey.Close(); praKey.Dispose();
                            }
                            //else
                                //throw new KeyNotFoundException("The extension has a program association (to: " + pra + "), but the referenced key does not exist.");

                            extKey.Close(); extKey.Dispose();
                        }
                        //else
                            //throw new KeyNotFoundException("The extension appears to be registered, but the key does not exist.\n" +
                                //"This indicates an error in this.Registered's logic, or that the key has been deleted in the meantime.");
                    }
                    //else
                        //throw new NullReferenceException("This extension is not registered!");

                    return desc;
                }
                // TODO: Description Set
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
                        string pra = GetProgramAssociationName(userspaceProgID);
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
                            //else
                            //throw new KeyNotFoundException("The default icon key does not exist.");

                            praKey.Close(); praKey.Dispose();
                        }
                        //else
                        //throw new KeyNotFoundException("The extension has a program association (to: " + pra + "), but the referenced key does not exist.");

                        extKey.Close(); extKey.Dispose();
                    }
                    //else
                    //throw new KeyNotFoundException("The extension appears to be registered, but the key does not exist.\n" +
                    //"This indicates an error in this.Registered's logic, or that the key has been deleted in the meantime.");
                }
                //else
                //throw new NullReferenceException("This extension is not registered!");

                return icon;
            }

            public string Icon
            {
                get
                {
                    return this.GetIcon(false);
                }
                // TODO: Icon Set
            }

            public string UserIcon
            {
                get
                {
                    return this.GetIcon(true);
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
                        string pra = GetProgramAssociationName(userspaceProgID);
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
                                //else
                                    //throw new KeyNotFoundException("The shell's open key does not exist.");

                                praShellKey.Close(); praShellKey.Dispose();
                            }
                            //else
                                //throw new KeyNotFoundException("The shell key does not exist.");

                            praKey.Close(); praKey.Dispose();
                        }
                        //else
                            //throw new KeyNotFoundException("The extension has a program association (to: " + pra + "), but the referenced key does not exist.");

                        extKey.Close(); extKey.Dispose();
                    }
                    //else
                        //throw new KeyNotFoundException("The extension appears to be registered, but the key does not exist.\n" +
                            //"This indicates an error in this.Registered's logic, or that the key has been deleted in the meantime.");
                }
                //else
                    //throw new NullReferenceException("This extension is not registered!");

                return exec;
            }

            public string Executable
            {
                get
                {
                    return this.GetExecutable(false);
                }
                // TODO: Executable Set
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
