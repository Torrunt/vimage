using System;
using System.Collections.Generic;
using System.IO;

namespace vimage
{
    public static class MimeTypes
    {
        private static readonly List<(
            byte[] Signature,
            string Mime,
            string Description
        )> _signatures =
        [
            // === Bitmaps ===
            (new byte[] { 0x42, 0x4D }, "image/bmp", "BMP image"),
            // === PNG ===
            (
                new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A },
                "image/png",
                "PNG image"
            ),
            // === GIF ===
            (new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif", "GIF image"),
            // === ICO ===
            (new byte[] { 0x00, 0x00, 0x01, 0x00 }, "image/x-icon", "ICO image"),
            // === JPEG/JPG/JPE/JFIF/JFI ===
            (new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg", "JPEG image"),
            // === WebP === (RIFF container with WEBP header)
            (new byte[] { 0x52, 0x49, 0x46, 0x46 }, "image/webp", "WebP image"), // need extra check for "WEBP"
            // === TIFF ===
            (new byte[] { 0x49, 0x49, 0x2A, 0x00 }, "image/tiff", "TIFF image (little-endian)"),
            (new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, "image/tiff", "TIFF image (big-endian)"),
            // === Photoshop PSD ===
            (new byte[] { 0x38, 0x42, 0x50, 0x53 }, "image/vnd.adobe.photoshop", "Photoshop PSD"),
            // === Radiance HDR ===
            (System.Text.Encoding.ASCII.GetBytes("#?RADIANCE"), "image/vnd.radiance", "HDR image"),
            // === Pixar PIC ===
            (System.Text.Encoding.ASCII.GetBytes("PICT"), "image/x-pic", "PIC image"),
            // === TGA (Truevision TARGA) ===
            // TGA doesn’t have a fixed signature, but can be guessed by footer
            (
                System.Text.Encoding.ASCII.GetBytes("TRUEVISION-XFILE."),
                "image/x-tga",
                "TGA image"
            ),
            // === EXR (OpenEXR) ===
            (new byte[] { 0x76, 0x2F, 0x31, 0x01 }, "image/exr", "OpenEXR image"),
            // === JPEG 2000 (JP2) ===
            (
                new byte[] { 0x00, 0x00, 0x00, 0x0C, 0x6A, 0x50, 0x20, 0x20 },
                "image/jp2",
                "JPEG 2000 image"
            ),
            // === PCX ===
            (new byte[] { 0x0A }, "image/x-pcx", "PCX image"),
            // === PBM/PGM/PPM (Netpbm formats) ===
            (System.Text.Encoding.ASCII.GetBytes("P1"), "image/x-portable-bitmap", "PBM ASCII"),
            (System.Text.Encoding.ASCII.GetBytes("P4"), "image/x-portable-bitmap", "PBM binary"),
            (System.Text.Encoding.ASCII.GetBytes("P2"), "image/x-portable-graymap", "PGM ASCII"),
            (System.Text.Encoding.ASCII.GetBytes("P5"), "image/x-portable-graymap", "PGM binary"),
            (System.Text.Encoding.ASCII.GetBytes("P3"), "image/x-portable-pixmap", "PPM ASCII"),
            (System.Text.Encoding.ASCII.GetBytes("P6"), "image/x-portable-pixmap", "PPM binary"),
            // === SGI ===
            (new byte[] { 0x01, 0xDA }, "image/sgi", "SGI image"),
            // === Kodak PhotoCD (PCD) ===
            (new byte[] { 0x45, 0x56, 0x46 }, "image/x-photo-cd", "Kodak Photo CD"), // "EVF" magic
            // === LBM (IFF ILBM / Amiga) ===
            (System.Text.Encoding.ASCII.GetBytes("FORM"), "image/x-ilbm", "IFF ILBM image"),
            // === DDS (DirectDraw Surface) ===
            (new byte[] { 0x44, 0x44, 0x53, 0x20 }, "image/vnd.ms-dds", "DDS image"),
            // === Doom picture lumps ===
            (System.Text.Encoding.ASCII.GetBytes("PWAD"), "image/x-doom", "Doom WAD lump"),
            (System.Text.Encoding.ASCII.GetBytes("IWAD"), "image/x-doom", "Doom IWAD lump"),
        ];

        public static (string? Mime, string? Description) GetFileType(string filePath)
        {
            byte[] header = new byte[560];
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                fs.ReadExactly(header);
            }

            foreach (var (signature, mime, description) in _signatures)
            {
                if (header.AsSpan().StartsWith(signature))
                    return (mime, description);
            }

            return (null, "Unknown");
        }
    }
}
