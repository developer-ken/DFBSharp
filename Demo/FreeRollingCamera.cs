using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class FreeRollingCamera : IDisposable
    {
        VideoCapture vcap;
        ManualResetEvent onCap = new ManualResetEvent(false);
        Mat latestframe;
        bool run = true;
        bool count_fps;
        public double RollingRate { get; private set; }
        private int frames;

        public FreeRollingCamera(string dev, bool fpscounter = false)
        {
            count_fps = fpscounter;
            vcap = new VideoCapture(dev);
            //vcap.FrameWidth = 640;
            //vcap.FrameHeight = 480;
            //vcap.Fps = 60;
            latestframe = new Mat();
        }

        public void BeginRoll()
        {
            Task.Run(() =>
            {
                if (count_fps)
                {
                    frames = 0;
                    while (run)
                    {
                        vcap.Read(latestframe);
                        if (latestframe.Empty())
                        {
                            continue;
                        }
                        onCap.Set();
                        frames++;
                    }
                }
                else
                    while (run)
                    {
                        vcap.Read(latestframe);
                        if (latestframe.Empty())
                        {
                            continue;
                        }
                        onCap.Set();
                    }
            });
            if (count_fps)
            {
                Task.Run(() =>
                {
                    DateTime time = DateTime.Now;
                    while (run)
                    {
                        Thread.Sleep(500);
                        RollingRate = frames / (DateTime.Now - time).TotalSeconds;
                        time = DateTime.Now;
                        frames = 0;
                    }
                });
            }
        }

        public void Dispose()
        {
            run = false;
            latestframe.Dispose();
        }

        public MatInPool GetFrame(double exposure = -5)
        {
            onCap.Reset();
            //vcap.Exposure = exposure;
            onCap.WaitOne();
            return MatPool.Default.EnPool(latestframe);
        }
    }
}
