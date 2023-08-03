using DFBSharp;
using OpenCvSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Demo
{
    internal class Program
    {
        const bool IsRedLaser = true;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            FreeRollingCamera vcap = new FreeRollingCamera(1);
            vcap.BeginRoll();
            //FrameBuffer fb = new FrameBuffer("/dev/fb0");
            //fb.Clear();
            //Mat n = new Mat();
            //Console.WriteLine($"{fb.Width}, {fb.Height}");
            while (true)
            {
                if (IsRedLaser)
                {
                    using (var m = vcap.GetFrame())
                    //using (var rb = vcap.GetFrame(-11))
                    using (var n = RedMinusBG(m))
                    {
                        /*var points = FindBlackParallelogram(m);
                        foreach( var p in points )
                        {
                            Cv2.DrawMarker(m, p, Scalar.Green);
                        }
                        */
                        var aa = DetectSkewRect(m);

                        //Cv2.ArrowedLine(m, aa.ContourCW[0], aa.ContourCW[1], Scalar.Red, 2);
                        //Cv2.ArrowedLine(m, aa.ContourCW[1], aa.ContourCW[2], Scalar.Red, 2);
                        //Cv2.ArrowedLine(m, aa.ContourCW[2], aa.ContourCW[3], Scalar.Red, 2);
                        //Cv2.ArrowedLine(m, aa.ContourCW[3], aa.ContourCW[0], Scalar.Red, 2);

                        Cv2.ImShow("red", n);
                        Cv2.ImShow("img", m);
                        Cv2.WaitKey(1);
                        //Cv2.Resize(m, n, new Size(fb.Width, fb.Height));
                        //Cv2.CvtColor(n, n, ColorConversionCodes.BGR2BGRA);
                        //fb.DrawBitmap(0, 0, n.Width, n.Height, n.Data);
                    }
                }
            }
        }

        //static Point RedPointDetect(Mat image)
        //{

        //}

        static Rect4P DetectSkewRect(Mat image)
        {
            using (Mat gray = GrayIsh(image))
            using (Mat imgbin = new Mat())
            using (Mat edges = new Mat())
            {
                Cv2.Threshold(gray, imgbin, 80, 255, ThresholdTypes.BinaryInv); // 二值化
                //Cv2.Erode(imgbin, imgbin, new Mat());
                //Cv2.Erode(imgbin, imgbin, new Mat());
                Cv2.ImShow("bin", imgbin);
                Cv2.Canny(imgbin, edges, 50, 150); // 边缘检测算子
                Cv2.FindContours(edges, out Point[][] contours, out HierarchyIndex[] hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
                Cv2.DrawContours(image, contours, -1, Scalar.Yellow, 1);
                double maxScore = 0;
                double mastersize = 0;
                bool t = false;
                Rect4P bx = new Rect4P();
                int maxbxid = -1;
                int ijj = 0;
                foreach (var contour in contours)
                {
                    double area = Cv2.ContourArea(contour); // 面积
                    RotatedRect rect = Cv2.MinAreaRect(contour); // 最小外接矩形
                    double dist = Distance((Point)rect.Center - new Point(image.Width / 2, image.Height / 2));
                    double score = area + dist * 5;

                    //if (hierarchy[ijj].Child < 0) continue;
                    //if (hierarchy[hierarchy[ijj].Child].Next > 0) continue;
                    //if (hierarchy[hierarchy[ijj].Child].Previous > 0) continue;

                    var bbx = Cv2.BoxPoints(rect);
                    if (score > maxScore)
                    {
                        maxbxid = ijj;
                        mastersize = area;
                        bx.LeftUp = (Point)bbx[3];
                        bx.LeftDown = (Point)bbx[2];
                        bx.RightDown = (Point)bbx[1];
                        bx.RightUp = (Point)bbx[0];
                        t = true;
                        maxScore = score; // 过滤最高分矩形
                    }
                    ijj++;
                }
                if (t)
                {
                    return bx;
                }
                else
                {
                    throw new Exception("No Target Found");
                }
            }
        }

        static MatInPool GrayIsh(Mat input)
        {
            MatInPool result = MatPool.Default.Borrow(input.Size(), MatType.CV_8UC1);//new Mat(input.Size(), MatType.CV_8UC1);
            for (int y = 0; y < input.Rows; y++)
            {
                for (int x = 0; x < input.Cols; x++)
                {
                    var bgr = input.Get<Vec3b>(y, x);
                    byte max = Math.Max(bgr.Item0, Math.Max(bgr.Item1, bgr.Item2));
                    result.Set(y, x, new Vec3b(max, max, max));
                }
            }
            return result;
        }

        static MatInPool RedMinusBG(Mat input)
        {
            var mmap = input.Split();
            var r = mmap[2];
            var g = mmap[1];
            var b = mmap[0];
            Cv2.Multiply(b, 0.5, b);
            Cv2.Multiply(g, 0.5, g);
            Cv2.Subtract(r, b, r);
            Cv2.Subtract(r, g, r);
            var chr = MatPool.Default.EnPool(r);
            r.Dispose();
            g.Dispose();
            b.Dispose();
            return chr;
        }

        static float Distance(Point p)
        {
            return (float)Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        struct Rect4P
        {
            public Point LeftUp, RightUp, LeftDown, RightDown;
            public Point[] ContourCW => new Point[] { LeftUp, RightUp, RightDown, LeftDown };
            public Point[] ContourCCW => new Point[] { LeftUp, LeftDown, RightDown, RightUp };
        }
    }

}