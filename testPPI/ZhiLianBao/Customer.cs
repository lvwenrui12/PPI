using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace testPPI.ZhiLianBao
{
   public class Customer
    {

        #region 属性
        //消费者名
        private string _Name;

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        //产品缓存队列
        private WorkQueue<string> _queue;

        //消费线程
        public Thread _thread;

        //消费者等待时间
        private int Spead = 1000;//消费者等待时间 

        #endregion

        public Customer(WorkQueue<string> queue)
        {
            this._queue = queue;
            //_thread = new Thread(new ThreadStart(Start));
            //_thread.Name = "消费者";
        }


        //public void Start()
        //{
        //    while (true)
        //    {
        //        if (!IsWait())
        //        {
        //            Cusum();
        //        }
        //        else
        //        {
        //            Thread.Sleep(Spead);
                   
        //        }
        //    }
        //}

        public bool IsWait()
        {
            return _queue.IsEmpty;
        }

        //public void Cusum()
        //{


        //   string str = this._queue.TryDequeueBox();

        //    if (str != null)
        //    {
               
        //    }
        //}

    }
}
