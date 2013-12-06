// vimage - http://torrunt.net/vimage
// Corey Zeke Womack (Torrunt) - me@torrunt.net

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
                //file = @"C:\Users\Corey\Pictures\Test Images\Beaver_Transparent.png";
                //file = @"C:\Users\Corey\Pictures\Test Images\AdventureTime_TransparentAnimation.gif";
                //file = @"C:\Users\Corey\Pictures\Test Images\AnimatedGif.gif";
                file = @"G:\Misc\Desktop Backgrounds\0diHF.jpg";
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
