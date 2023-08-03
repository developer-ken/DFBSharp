using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    public class MatPool : IDisposable
    {
        public static MatPool Default = new MatPool();

        List<MatInPool> pool = new List<MatInPool>();
        public MatPool()
        {
        }

        [Obsolete("Size and MatType is now taken into consideration. Use Borrow(Size,MatType) to get better performance.")]
        public MatInPool Mat
        {
            get => Borrow();
        }

        public MatInPool Borrow(Size matsize, MatType type, bool accept_wrong_type = false)
        {
            lock (pool)
            {
                //第一轮 查找完美匹配
                foreach (var item in pool)
                {
                    if (item.IsBusy) continue;
                    if (item.Type == type && item.Size == matsize)
                    {
                        item.Lend();
                        return item;
                    }
                }
                //第二轮 不再考虑条件
                if (accept_wrong_type)
                    foreach (var item in pool)
                    {
                        if (item.IsBusy) continue;
                        item.Lend();
                        return item;
                    }
                //池中没有可用元素，新建并交付
                var mat = new MatInPool(this, matsize, type);
                pool.Add(mat);
                mat.Lend();
                return mat;
            }
        }


        public MatInPool Borrow(MatInPool reference, bool accept_wrong_type = false)
        {
            return Borrow(reference.Size, reference.Type, accept_wrong_type);
        }

        [Obsolete("Size and MatType is now taken into consideration. Use Borrow(Size,MatType) to get better performance.")]
        public MatInPool Borrow()
        {
            lock (pool)
            {
                foreach (var item in pool)
                {
                    if (item.IsBusy) continue;
                    item.Lend();
                    return item;
                }
                var mat = new MatInPool(this, new Mat());
                pool.Add(mat);
                mat.Lend();
                return mat;
            }
        }

        public MatInPool EnPool(Mat mat)
        {
            var result = Borrow(mat.Size(), mat.Type());
            Cv2.CopyTo(mat, result.Mat);
            return result;
        }

        /// <summary>
        /// 强制收回池中的所有矩阵。
        /// 大部分视觉程序是周期性运行的。在完成一帧图像的处理后，使用本函数释放所有矩阵比较方便。
        /// </summary>
        public void ForceReleaseAllMats()
        {
            foreach (var item in pool)
            {
                if (!item.IsBusy) continue;
                item.Dispose();
            }
        }

        /// <summary>
        /// 强制收回池中的所有矩阵，除了指定的矩阵。
        /// 大部分视觉程序是周期性运行的。在完成一帧图像的处理后，使用本函数释放所有矩阵比较方便。
        /// 指定例外的功能是为了不清除结果矩阵。
        /// </summary>
        /// <param name="except">不被回收的矩阵</param>
        public void ForceReleaseAllMatsExcept(params MatInPool[] except)
        {
            foreach (var item in pool)
            {
                if (!item.IsBusy) continue;
                if (except.Contains(item)) continue;
                item.Dispose();
            }
        }

        public void PurgePool()
        {
            lock (pool)
            {
                foreach (var item in pool)
                {
                    if (item.IsBusy)
                        throw new ApplicationException("Some elements in pool is still busy.");
                    item.Destroy();
                }
                pool.Clear();
            }
        }

        public void Dispose()
        {
            PurgePool();
            pool = null;
        }
    }

    /// <summary>
    /// 池矩阵元素。
    /// Dispose释放本类只会让Mat返回池中，并不会清理对应内存空间。
    /// 要清理内存空间，请使用Destroy方法。
    /// 本类实现了析构函数，使垃圾回收器可以正确回收Mat占用的内存空间。
    /// </summary>
    public class MatInPool : Mat
    {
        public Mat Mat
        {
            get
            {
                if (!IsBusy) throw new FieldAccessException("Mat is not locked. Call Lend() is required before accessing.");
                if (base.IsDisposed) throw new ObjectDisposedException("Mat is disposed.");
                return this;
            }
        }

        public bool IsBusy { get; private set; }
        public new Size Size { get; private set; }
        public new MatType Type { get; private set; }

        private MatPool pool;

        public MatInPool(MatPool pool, Mat mat)
        {
            Cv2.CopyTo(mat, this);
            this.pool = pool;
            IsBusy = false;
        }

        public MatInPool(MatPool pool, Size size, MatType type) : base(size, type)
        {
            this.pool = pool;
            IsBusy = false;
        }

        public void Lend()
        {
            if (IsBusy) throw new FieldAccessException("Mat is locked. It is in use elsewhere.");
            if (base.IsDisposed) throw new ObjectDisposedException("Mat is disposed.");
            base.SetTo(0);//清空数据
            IsBusy = true;
            Size = base.Size();
            Type = base.Type();
        }

        ~MatInPool()
        {
            if (IsBusy)
            {
                throw new ApplicationException("GC collecting a busy Mat. It indicates you did not return the Mat once nolonger needed.");
            }
            Destroy();
        }

        public void Destroy()
        {
            base.Dispose();
        }

        public new void Dispose()
        {
            if (Size != base.Size() || Type != base.Type())
            {
                throw new ApplicationException("Size or Type changed! Keep Size and Type unchanged when using borrowed mat.");
            }
            IsBusy = false;
        }

        public new MatInPool Clone()
        {
            var mat = EmptyClone();
            Cv2.CopyTo(this, mat);
            return mat;
        }

        public new MatInPool EmptyClone()
        {
            return pool.Borrow(this);
        }
    }
}
