using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DFBSharp.FakeFb
{
    public class FrameBuffer
    {
        public UInt32 BitsPerPixel;// => Native.GetBitsPerPixel(fb);

        public UInt32 Width;// => Native.GetWidth(fb);

        public UInt32 Height;// => Native.GetHeight(fb);

        public FrameBuffer(string nothing,int width=960,int height=480,int bpp=16)
        {
            BitsPerPixel = (uint)bpp;
            Width = (uint)width;
            Height = (uint)height;
        }

        public void Clear()
        {
        }

        public void DrawPixel(int x, int y, Native.Color color)
        {
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Native.Color color)
        {
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, Native.Color color, int fill)
        {
        }

        public void DrawBitmap(int x1, int y1, int x2, int y2, IntPtr bitmap)
        {
        }

        [Obsolete("Use DrawBitmap(IntPtr) instead.")]
        public void DrawFullBitmap(IntPtr bitmap)
        {
        }

        public void DrawBitmap(IntPtr bitmap)
        {
        }
    }
}
