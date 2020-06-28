using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace TestApp
{
    /// <summary>
    /// Сlass with methods for comparing bitmaps.
    /// 1. CompareBitmapsFastUnsafe     - The fastest way to compare bitmaps.
    /// 2. CompareBitmapsFast           
    /// 3. CompareBitmapsMemoryStream
    /// 4. CompareBitmapsLazy           - The slowest way to compare bitmaps.
    /// </summary>
    public static class BitmapComprasion
    {
        /// <summary>
        /// Compare bitmaps using GetPixel method.
        /// The slowest way to compare bitmaps.
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static bool CompareBitmapsLazy(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;


            for (int col = 0; col < bmp1.Width; col++)
            {
                for (int row = 0; row < bmp1.Height; row++)
                {
                    if (!bmp1.GetPixel(col, row).Equals(bmp2.GetPixel(col, row)))
                        return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Compare bitmaps using LockBits and Marshal.Copy.
        /// Really fast, but still losing to unsafe method.
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static bool CompareBitmapsFast(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int numBytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] bytes1 = new byte[numBytes];
            byte[] bytes2 = new byte[numBytes];

            BitmapData bmpData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bmpData1.Scan0, bytes1, 0, numBytes);
            Marshal.Copy(bmpData2.Scan0, bytes2, 0, numBytes);

            for (int i = 0; i < numBytes; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            return result;
        }


        /// <summary>
        /// Compare bitmaps using unsafe code.
        /// The fastest way to compare bitmaps.
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public unsafe static bool CompareBitmapsFastUnsafe(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int numBytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            BitmapData bmpData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);
            byte* bytes1 = (byte*)bmpData1.Scan0;
            byte* bytes2 = (byte*)bmpData2.Scan0;

            bool result = true;
            for (int i = 0; i < numBytes; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    result = false;
                    break;
                }
            }

            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            return result;
        }


        /// <summary>
        /// Compare bitmaps using MemoryStream. 
        /// Сompares not the pixels, but the whole file.
        /// Slower than CompareBitmapsFast.
        /// </summary>
        /// <param name="bmp1"></param>
        /// <param name="bmp2"></param>
        /// <returns></returns>
        public static bool CompareBitmapsMemoryStream(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (object.Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;


            byte[] bytes1;
            byte[] bytes2;
            using (var mstream = new MemoryStream())
            {
                bmp1.Save(mstream, bmp1.RawFormat);
                bytes1 = mstream.ToArray();
            }

            using (var mstream = new MemoryStream())
            {
                bmp2.Save(mstream, bmp2.RawFormat);
                bytes2 = mstream.ToArray();
            }

            if (bytes1.Length != bytes2.Length)
                return false;

            bool result = true;
            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}
