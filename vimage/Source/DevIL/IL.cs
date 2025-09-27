using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace DevIL
{
    public static class IL
    {
        [DllImport("DevIL", EntryPoint = "ilInit")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void Init();

        [DllImport("DevIL", EntryPoint = "ilDeleteImage")]
        [SuppressUnmanagedCodeSecurity]
        public static extern void DeleteImage(int Num);

        [DllImport("DevIL", EntryPoint = "ilConvertImage")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ConvertImage(ChannelFormat DestFormat, ChannelType DestType);

        [DllImport("DevIL", EntryPoint = "ilCopyPixels")]
        [SuppressUnmanagedCodeSecurity]
        public static extern int CopyPixels(
            int XOff,
            int YOff,
            int ZOff,
            int Width,
            int Height,
            int Depth,
            ChannelFormat Format,
            ChannelType Type,
            IntPtr Data
        );

        [DllImport("DevIL", EntryPoint = "ilBindImage")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern void BindImage(int Image);

        [DllImport("DevIL", EntryPoint = "ilGenImage")]
        [SuppressUnmanagedCodeSecurity]
        internal static extern int GenImage();

        [DllImport("DevIL", EntryPoint = "ilEnable")]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool Enable(EnableCap Mode);

        [DllImport("DevIL", EntryPoint = "ilRegisterOrigin")]
        [SuppressUnmanagedCodeSecurity]
        public static extern void RegisterOrigin(OriginMode Origin);

        [DllImport("DevIL", EntryPoint = "ilLoadL")]
        [SuppressUnmanagedCodeSecurity]
        public static extern bool LoadL(ImageType Type, IntPtr Lump, int Size);

        public static bool LoadStreamWithType(ImageType imageType, Stream s)
        {
            byte[] array = new byte[s.Length];
            int num;
            for (int i = 0; i < array.Length; i += num)
                num = s.Read(array, i, array.Length - i);

            var gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            bool result = LoadL(imageType, gCHandle.AddrOfPinnedObject(), array.Length);
            gCHandle.Free();
            return result;
        }

        public static bool LoadStream(Stream s)
        {
            return LoadStreamWithType(ImageType.Unkwown, s);
        }

        [DllImport("DevIL", EntryPoint = "ilGetInteger")]
        [SuppressUnmanagedCodeSecurity]
        public static extern int GetInteger(IntName Mode);

        [DllImport("DevIL", EntryPoint = "ilGetData")]
        [SuppressUnmanagedCodeSecurity]
        public static extern IntPtr GetData();
    }
}
