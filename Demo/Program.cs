using DFBSharp;
using DFBSharp.OpenCv4;
using OpenCvSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            FreeRollingCamera vcap = new FreeRollingCamera("/dev/video0", true);
            vcap.BeginRoll();
            FrameBuffer fb = new FrameBuffer("/dev/fb0");
            fb.Clear();
            Console.WriteLine($"{fb.Width}, {fb.Height}");
            var start = DateTime.Now;
            long frames = 0;
            while (true)
            {
                frames++;
                var fps = frames / (DateTime.Now - start).TotalSeconds;
                using (var aa = vcap.GetFrame())
                {
                    Cv2.PutText(aa, $"FPS:{fps.ToString("0.0")},{vcap.RollingRate.ToString("0.0")}", new Point(0, aa.Height / 2),
                        HersheyFonts.HersheyPlain, 2, Scalar.Green, 2, bottomLeftOrigin: false);
                    fb.DrawMat(aa);
                }
            }
        }
    }

}