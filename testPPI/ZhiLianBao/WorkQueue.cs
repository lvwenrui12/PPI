using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testPPI
{
    public class WorkQueue<T> : ConcurrentQueue<T>

    {
        /// <summary>
        /// 队列工作形式
        /// </summary>
        public enum WorkType
        {
            Add,
            Get,
            NotAciton
        }

        public int MaxSize = 10;//队列最大值

        private static readonly object _Lockobj = new object();//锁对象



        /// <summary>
        /// 尝试移除并返回位于 ConcurrentQueue<T> 开始处的对象
        /// </summary>
        /// <returns></returns>
        public T TryDequeueBox()
        {
            T _obj = default(T);
            if (!this.IsEmpty)//如果队列不为空，也就是有产品
                this.TryDequeue(out _obj);

            return _obj;
        }

        /// <summary>
        /// 添加到队列末尾处
        /// </summary>
        /// <returns></returns>
        public void EnqueueBox(T box)
        {

            if (this.MaxSize > this.Count)
            {
                this.Enqueue(box);
            }
        }




    }
}
