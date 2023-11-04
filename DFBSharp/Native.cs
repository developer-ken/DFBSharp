using System;
using System.Runtime.InteropServices;

namespace DFBSharp
{
    public static class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Color
        {
            public int a, r, g, b;
        }

        [DllImport("libdfb")]
        public static extern IntPtr Open(string device);

        [DllImport("libdfb")]
        public static extern void Clear(IntPtr ptr);

        [DllImport("libdfb")]
        public static extern void DrawPixel(IntPtr ptr, int x, int y, Color color);

        [DllImport("libdfb")]
        public static extern void DrawLine(IntPtr ptr, int x1, int y1, int x2, int y2, Color color);

        [DllImport("libdfb")]
        public static extern void DrawRectangle(IntPtr ptr, int x1, int y1, int x2, int y2, Color color, int fill);

        [DllImport("libdfb")]
        public static extern void DrawBitmap(IntPtr ptr, int x1, int y1, int x2, int y2, IntPtr bitmap);
        [DllImport("libdfb")]
        public static extern void DrawFullBitmap(IntPtr handle, IntPtr bitmap);

        [DllImport("libdfb")]
        public static extern UInt32 GetBitsPerPixel(IntPtr ptr);

        [DllImport("libdfb")]
        public static extern UInt32 GetWidth(IntPtr ptr);

        [DllImport("libdfb")]
        public static extern UInt32 GetHeight(IntPtr ptr);
    }
}
