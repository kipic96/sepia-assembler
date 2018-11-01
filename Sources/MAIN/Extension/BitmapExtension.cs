﻿using System.Windows.Media.Imaging;

namespace MAIN.Extension
{
    public static class BitmapSourceExtension
    {
        public static float[] ToBmpBGRArray(this BitmapSource bitmapSource)
        { 
            int stride = bitmapSource.PixelWidth * (bitmapSource.Format.BitsPerPixel / 8);
            byte[] bytePixels = new byte[bitmapSource.PixelHeight * stride];

            bitmapSource.CopyPixels(bytePixels, stride, 0);

            float[] floatPixels = bytePixels.ToFloatArray();

            return floatPixels;
        }
    }
}
