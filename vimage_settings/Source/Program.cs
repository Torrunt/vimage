using System;
using System.Security.Principal;
using System.Windows.Forms;

namespace vimage_settings
{
    static class Program
    {
        // Directly copied from vimage/Source/ImageViewer.cs@20:24
        static public readonly string[] EXTENSIONS =
        {
            "bmp", "cut", "dds", "doom", "exr", "hdr", "gif", "ico", "jp2", "jpg", "jpeg", "lbm", "mdl", "mng",
            "pal", "pbm", "pcd", "pcx", "pgm", "pic", "png", "ppm", "psd", "psp", "raw", "sgi", "tga", "tif"
        };

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Associations.AF_FileAssociator af = new Associations.AF_FileAssociator(".bmp");

            foreach (string extension in EXTENSIONS)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("." + extension);
                Console.ResetColor();
                Association.FileAssocation ext_assoc = new Association.FileAssocation("." + extension);

                Console.Write("Registered: ");
                if (ext_assoc.Registered)
                    Console.ForegroundColor = ConsoleColor.Green;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ext_assoc.Registered);
                Console.ResetColor();

                if (ext_assoc.Registered)
                {
                    Console.WriteLine("ProgramAssociationName: " + ext_assoc.GetAssociatedProgID(false));

                    if (!String.IsNullOrWhiteSpace(ext_assoc.GetAssociatedProgID(true)))
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("User ProgID: ");
                        Console.ResetColor();
                        Console.WriteLine(ext_assoc.GetAssociatedProgID(true));
                    }

                    Console.WriteLine("Description: " + ext_assoc.Description);

                    if (!String.IsNullOrWhiteSpace(ext_assoc.Icon))
                    {
                        Console.Write("Icon: ");
                        if (!ext_assoc.IconValid)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("(Invalid) ");
                            Console.ResetColor();
                        }
                        Console.WriteLine(ext_assoc.Icon);
                    }

                    if (!String.IsNullOrWhiteSpace(ext_assoc.GetAssociatedProgID(true)))
                    {
                        if (!String.IsNullOrWhiteSpace(ext_assoc.UserIcon))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("User Icon: ");
                            Console.ResetColor();
                            Console.WriteLine(ext_assoc.UserIcon);
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ext_assoc.Executable))
                    {
                        Console.Write("Default action: ");
                        if (!ext_assoc.ExecutableValid)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("(Invalid) ");
                            Console.ResetColor();
                        }
                        Console.WriteLine(ext_assoc.Executable);
                    }

                    if (!String.IsNullOrWhiteSpace(ext_assoc.GetAssociatedProgID(true)))
                    {
                        if (!String.IsNullOrWhiteSpace(ext_assoc.UserExecutable))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write("User default action: ");
                            Console.ResetColor();
                            Console.WriteLine(ext_assoc.UserExecutable);
                        }
                    }
                }
            }

            //Console.ReadLine();
            //Environment.Exit(0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConfigWindow());
        }

        public static bool IsAdministrator
        {
            get
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
    }
}