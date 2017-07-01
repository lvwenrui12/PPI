using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace testPPI.PPI
{
    public class PPIReadWritePara
    {
    //   public Socket TcpClient { get; set; }
        public TcpClient TcpClient { get; set; }

        public int ComNum { get; set; }

        public int ByteAddress { get; set; }


        public int Bitnumber { get; set; }

        public Enums.StorageType StorageType { get; set; }

        public int WriteValue { get; set; }

        public int PlcAddress { get; set; } = 2;

        public byte[] ReadValue { get; set; }

        public int ReadCount { get; set; } = 1;

        public
            Enums.VarType dataType { get; set; }

    

        public bool IsSuceess { get; set; } = false;

    }

}
