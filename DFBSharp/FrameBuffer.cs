using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace DFBSharp
{
    public class FrameBuffer
    {
        IntPtr fb;

        public UInt32 BitsPerPixel => Native.GetBitsPerPixel(fb);

        public UInt32 Width => Native.GetWidth(fb);

        public UInt32 Height => Native.GetHeight(fb);

        public FrameBuffer(string fbdev)
        {
            fb = Native.Open(fbdev);
            if (fb == IntPtr.Zero)
            {
                throw new IOException("Failed to open framebuffer device");
            }
        }

        public void Clear()
        {
            Native.Clear(fb);
        }

        public void DrawPixel(int x, int y, Native.Color color)
        {
            Native.DrawPixel(fb, x, y, color);
        }

        public void DrawLine(int x1, int y1, int x2, int y2, Native.Color color)
        {
            Native.DrawLine(fb, x1, y1, x2, y2, color);
        }

        public void DrawRectangle(int x1, int y1, int x2, int y2, Native.Color color, int fill)
        {
            Native.DrawRectangle(fb, x1, y1, x2, y2, color, fill);
        }

        public void DrawBitmap(int x1, int y1, int x2, int y2, IntPtr bitmap)
        {
            Native.DrawBitmap(fb, x1, y1, x2, y2, bitmap);
        }
    }
}
