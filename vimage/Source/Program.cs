using System;

namespace vimage
{
    class Program
    {
        static void Main(string[] args)
        {
            string file;
            if (args.Length == 0)
            {
#if DEBUG
                file = "G:\\Misc\\Desktop Backgrounds\\0diHF.jpg";
                //file = "C:\\Users\\Corey\\Pictures\\Screenshots and Gifs\\Firefight\\crossbow.gif"; // Animated Gif
                //file = "C:\\Users\\Corey\\Pictures\\Avatars\\finn_and_jake_by_abysswolf-d6bgo83.gif"; // Transparent Animated Gif
#else
                return;
#endif
            }
            else
                file = args[0];

            ImageViewer ImageViewer = new ImageViewer(file);
        }
    }
}
