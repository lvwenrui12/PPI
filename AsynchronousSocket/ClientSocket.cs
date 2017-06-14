///////////////////////////////////////////////////////
//NSTCPFramework
//�汾��1.0.0.1
//////////////////////////////////////////////////////
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.IO;
using System.Reflection;

namespace AsynchronousSocket
{
    public delegate void SocketErrorEvent(object sender,NetEventArgs e,SocketError ErrorCode);
    /// <summary> 
    /// �ṩTcp�������ӷ���Ŀͻ����� 
    /// 
    /// ԭ��: 
    /// 1.ʹ���첽SocketͨѶ�����������һ����ͨѶ��ʽͨѶ,��ע�����������ͨ 
    /// Ѷ��ʽһ��Ҫһ��,���������ɷ������������,��������û�п˷�,��ô��byte[] 
    /// �ж����ı����ʽ 
    /// 2.֧�ִ���ǵ����ݱ��ĸ�ʽ��ʶ��,����ɴ����ݱ��ĵĴ������Ӧ���ӵ��� 
    /// �绷��. 
    /// </summary> 
    public class ClientSocket
    {
        #region �ֶ�

        /// <summary> 
        /// �ͻ����������֮��ĻỰ�� 
        /// </summary> 
        private Session _session;

        /// <summary> 
        /// �ͻ����Ƿ��Ѿ����ӷ����� 
        /// </summary> 
        private bool _isConnected = false;

        /// <summary> 
        /// �������ݻ�������С64K 
        /// </summary> 
        public const int DefaultBufferSize = 4 * 1024*1024;

        /// <summary> 
        /// ���Ľ����� 
        /// </summary> 
        private DatagramResolver _resolver;
 
        /// <summary> 
        /// �������ݻ����� 
        /// </summary> 
        private byte[] _recvDataBuffer = new byte[DefaultBufferSize];
  
        #endregion

        #region �¼�����

        //��Ҫ�����¼������յ��¼���֪ͨ������������˳�������ȡ������ 

        /// <summary> 
        /// �Ѿ����ӷ������¼� 
        /// </summary> 
        public event NetEvent ConnectedServer;

        /// <summary> 
        /// �����¼� 
        /// </summary> 
        public event SocketErrorEvent ErrorEvent;
        /// <summary> 
        /// �������ݱ�������¼� 
        /// </summary> 
        public event NetEvent DataSended;

        /// <summary> 
        /// ���յ����ݱ����¼� 
        /// </summary> 
        public event NetEvent ReceivedDatagram;

        /// <summary> 
        /// ���ӶϿ��¼� 
        /// </summary> 
        public event NetEvent DisConnectedServer;
        #endregion

        #region ����

        /// <summary> 
        /// ���ؿͻ����������֮��ĻỰ���� 
        /// </summary> 
        public Session ClientSession
        {
            get
            {
                return _session;
            }
        }

        /// <summary> 
        /// ���ؿͻ����������֮�������״̬ 
        /// </summary> 
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
        }

        /// <summary> 
        /// ���ݱ��ķ����� 
        /// </summary> 
        public DatagramResolver Resovlver
        {
            get
            {
                return _resolver;
            }
            set
            {
                _resolver = value;
            }
        }

        /// <summary> 
        /// ��������� 
        /// </summary> 
        private Coder _coder;
        public Coder ServerCoder
        {
            get
            {
                return _coder;
            }
        }
		/// <summary> 
		/// �ͻ����ļ�����·�� 
		/// </summary> 
        private string _filePath="";
		public string FilePath
		{
			get
			{
				return _filePath;
			}

		}
        #endregion

        #region ���з���

        /// <summary> 
        /// Ĭ�Ϲ��캯��,ʹ��Ĭ�ϵı����ʽ 
        /// </summary> 
        public ClientSocket()
        { 
        } 

        /// <summary> 
        /// ���ӷ����� 
        /// </summary> 
        /// <param name="ip">������IP��ַ</param> 
        /// <param name="port">�������˿�</param> 
        public virtual void Connect(string ip, int port)
        {
            if (IsConnected)
            {
                //�������� 
                Debug.Assert(_session != null);
                Close();
            }
            Socket newsock = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ip), port);
            newsock.BeginConnect(iep, new AsyncCallback(Connected), newsock);

        }

        /// <summary> 
        /// �������ݱ��� 
        /// </summary> 
        /// <param name="datagram"></param> 
        public virtual void SendText(string datagram)
        {
            if (datagram.Length == 0)
            {
                return;
            }

            if (!_isConnected)
            {
                throw (new ApplicationException("û�����ӷ����������ܷ�������"));
            }
            try
            {
                //��ñ��ĵı����ֽ� 
                SocketError ErrorCode=new SocketError();
                byte[] data = System.Text.Encoding.Default.GetBytes(datagram);
                _session.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,out ErrorCode, new AsyncCallback(SendDataEnd), _session.ClientSocket);
                if (ErrorCode != SocketError.Success)
                {
                    //�������ʹ����¼�
                    this.ErrorSocket(ErrorCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual void SendBytes(byte[] data)
        {
            if (data.Length == 0)
            {
                return;
            }

            if (!_isConnected)
            {
                throw (new ApplicationException("û�����ӷ����������ܷ�������"));
            }
            try
            {
                _session.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                    new AsyncCallback(SendDataEnd), _session.ClientSocket);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual void SendFile(string FilePath)
        {
            if (FilePath.Length == 0)
            {
                return;
            }

            if (!_isConnected)
            {
                throw (new ApplicationException("û�����ӷ����������ܷ�������"));
            }

            try
            {
                if (File.Exists(FilePath))
                {
                    byte[] data = _coder.GetFileBytes(FilePath);

                    _session.ClientSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                        new AsyncCallback(SendDataEnd), _session.ClientSocket);
                }
                else
                {
                    throw new Exception("�ļ�������");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 
        /// �ر����� 
        /// </summary> 
        public virtual void Close()
        {
            if (!_isConnected)
            {
                return;
            }
            _session.Close();
            _session = null;
            _isConnected = false;
        }

        #endregion

        #region �ܱ�������
        /// <summary> 
        /// ���ݷ��ʹ������� 
        /// </summary> 
        /// <param name="iar"></param> 
        /// <summary> 
        protected virtual void ErrorSocket(SocketError ErrorCode)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(this, new NetEventArgs(_session), ErrorCode);
            }
        }
        /// ���ݷ�����ɴ����� 
        /// </summary> 
        /// <param name="iar"></param> 
        protected virtual void SendDataEnd(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            int sent = remote.EndSend(iar);
            Debug.Assert(sent != 0);
            if (DataSended != null)
            {
                DataSended(this, new NetEventArgs(_session));
            }
        }

        /// <summary> 
        /// ����Tcp���Ӻ������ 
        /// </summary> 
        /// <param name="iar">�첽Socket</param> 
        protected virtual void Connected(IAsyncResult iar)
        {
            try
            {
                Socket socket = (Socket)iar.AsyncState;
                socket.EndConnect(iar);
                //�����µĻỰ 
                _session = new Session(socket);
                _isConnected = true;
                //�������ӽ����¼� 
                if (ConnectedServer != null)
                {
                    ConnectedServer(this, new NetEventArgs(_session));
                }
                //�������Ӻ�Ӧ�������������� 
                _session.ClientSocket.BeginReceive(_recvDataBuffer, 0,
                    DefaultBufferSize, SocketFlags.None,
                    new AsyncCallback(RecvData), socket);
            }
            catch (SocketException ex)
            {
                //�������ʹ����¼� 
                this.ErrorSocket((SocketError)ex.ErrorCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 
        /// ���ݽ��մ����� 
        /// </summary> 
        /// <param name="iar">�첽Socket</param> 
        protected virtual void RecvData(IAsyncResult iar)
        {
            Socket remote = (Socket)iar.AsyncState;
            try
            {
                int recv = remote.EndReceive(iar);
                //�������˳� 
                if (recv == 0)
                {
                    _session.TypeOfExit = Session.ExitType.NormalExit;

                    if (DisConnectedServer != null)
                    {
                        DisConnectedServer(this, new NetEventArgs(_session));
                    }
                    return;
                }
                //ͨ���¼������յ��ı��� 
                if (ReceivedDatagram != null)
                {                    
                    ICloneable copySession = (ICloneable)_session;
                    Session clientSession = (Session)copySession.Clone();
                    clientSession.Datagram = new byte[recv];
                    Array.Copy(_recvDataBuffer,0,clientSession.Datagram,0,recv); 
                    ReceivedDatagram(this, new NetEventArgs(clientSession)); 
                }//end of if(ReceivedDatagram != null) 

                //������������ 
                _session.ClientSocket.BeginReceive(_recvDataBuffer, 0, DefaultBufferSize, SocketFlags.None,
                    new AsyncCallback(RecvData), _session.ClientSocket);
            }
            catch (SocketException ex)
            {
                //�ͻ����˳� 
                if (10054 == ex.ErrorCode)
                {
                    //������ǿ�ƵĹر����ӣ�ǿ���˳� 
                    _session.TypeOfExit = Session.ExitType.ExceptionExit;

                    if (DisConnectedServer != null)
                    {
                        DisConnectedServer(this, new NetEventArgs(_session));
                    }
                }
                else
                {
                    //�������ʹ����¼� 
                    this.ErrorSocket((SocketError)ex.ErrorCode);
                }
            }
            catch (ObjectDisposedException ex)
            {
                //�����ʵ�ֲ������� 
                //������CloseSession()ʱ,��������ݽ���,�������ݽ��� 
                //�����л����int recv = client.EndReceive(iar); 
                //�ͷ�����CloseSession()�Ѿ����õĶ��� 
                //����������ʵ�ַ���Ҳ�����˴��ŵ�. 
                if (ex != null)
                {
                    ex = null;
                    //DoNothing; 
                }
            }
        }
        #endregion
    } 
}
