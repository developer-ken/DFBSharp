using OpenCvSharp;
using System;

namespace DFBSharp.OpenCv4
{
    public static class OpenCv4Extention
    {
        public static void DrawMat(this FrameBuffer fb, Mat mat)
        {
            var bpx = fb.BitsPerPixel;
            using (var matt = new Mat())
            {
                Cv2.Resize(mat, matt, new Size((int)fb.Width, (int)fb.Height));
                switch (bpx)
                {
                    case 32: //bgra
                        using (var mat32 = matt.CvtColor(ColorConversionCodes.BGR2BGRA))
                        {
                            fb.DrawFullBitmap(mat32.DataStart);
                        }
                        break;
                    case 24: //bgr
                        using (var mat24 = matt.CvtColor(ColorConversionCodes.BGR2BGR565))
                        {
                            fb.DrawFullBitmap(mat24.DataStart);
                        }
                        break;
                    case 16: //bgr565
                        using (var mat16 = matt.CvtColor(ColorConversionCodes.BGR2BGR565))
                        {
                            fb.DrawFullBitmap(mat16.DataStart);
                        }
                        break;
                }
            }
        }
    }
}
