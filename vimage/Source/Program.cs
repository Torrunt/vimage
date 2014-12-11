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
                return;
            else
                file = args[0];

            if (System.IO.File.Exists(file))
            {
                ImageViewer imageViewer = new ImageViewer(file);
            }
        }
    }
}
