using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.ADSioReader
{
    [Guid("9BBB0C17-22F7-4806-90C4-1EE81512861D")]
    [ProgId("ADActiveXADNet")]
    [ComVisible(true)]
    public class SioNet : SioBase
    {
        public TcpClient mTcpClient;

        public NetworkStream serverStream;

        private bool mIslogout = false;

        public const int DefaultBufferSize = 16384;

        private byte[] _recvDataBuffer = new byte[16384];

        ~SioNet()
        {
            Dispose(disposing: true);
        }

        public override bool Connect(string hostname, int baudorport)
        {
            if (mTcpClient != null && mTcpClient.Connected)
            {
                return true;
            }

            tcConnectAsync(hostname, baudorport);
            Thread.Sleep(50);
            return true;
        }

        public override bool Send(byte[] byData)
        {
            if (mTcpClient == null || !mTcpClient.Connected)
            {
                bConnected = false;
                return false;
            }

            try
            {
                tcBeginSend(byData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                bConnected = false;
                return false;
            }

            return true;
        }

        public override bool Send(string strData)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(strData);
            return Send(bytes);
        }

        public override void DisConnect()
        {
            mIslogout = false;
            bConnected = false;
            if (mTcpClient != null)
            {
                if (serverStream != null)
                {
                    serverStream.Close();
                }

                mTcpClient.Close();
            }

            RaiseEventStatus(new StatusEventArgs(4, ""));
        }

        public override string ToString()
        {
            if (mTcpClient != null)
            {
                try
                {
                    return mTcpClient.Client.RemoteEndPoint.ToString();
                }
                catch
                {
                    return base.ToString();
                }
            }

            return base.ToString();
        }

        protected virtual void tcConnectAsync(string ip, int ipport)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.BeginConnect(ip, ipport, tcConnected, tcpClient);
        }

        protected virtual void tcConnected(IAsyncResult iar)
        {
            try
            {
                mTcpClient = (TcpClient)iar.AsyncState;
                mTcpClient.EndConnect(iar);
                serverStream = mTcpClient.GetStream();
                serverStream.BeginRead(_recvDataBuffer, 0, 16384, tcRecvData, serverStream);
                mIslogout = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                bConnected = false;
                RaiseEventStatus(new StatusEventArgs(3, ex.Message));
                return;
            }

            bConnected = true;
            RaiseEventStatus(new StatusEventArgs(2, ""));
        }

        protected virtual void tcBeginSend(byte[] datagram)
        {
            if (datagram.Length != 0)
            {
                if (!bConnected)
                {
                    throw new ApplicationException("没有连接服务器，不能发送数据");
                }

                if (serverStream.CanWrite)
                {
                    serverStream.BeginWrite(datagram, 0, datagram.Length, tcSendDataEnd, serverStream);
                }
            }
        }

        protected virtual void tcSendDataEnd(IAsyncResult iar)
        {
            NetworkStream networkStream = (NetworkStream)iar.AsyncState;
            if (networkStream.CanWrite)
            {
                networkStream.EndWrite(iar);
            }
        }

        protected virtual void tcRecvData(IAsyncResult iar)
        {
            try
            {
                NetworkStream networkStream = (NetworkStream)iar.AsyncState;
                if (!networkStream.CanRead)
                {
                    goto IL_00cb;
                }

                int num = networkStream.EndRead(iar);
                if (num == 0)
                {
                    if (mTcpClient != null)
                    {
                        if (serverStream != null)
                        {
                            serverStream.Close();
                        }

                        mTcpClient.Close();
                    }

                    bConnected = false;
                    if (mIslogout)
                    {
                        RaiseEventStatus(new StatusEventArgs(5, "The remote host compulsory closing connection!"));
                    }

                    return;
                }

                byte[] array = new byte[num];
                for (int i = 0; i < num; i++)
                {
                    array[i] = _recvDataBuffer[i];
                }

                RaiseEventReceived(array);
                goto IL_00cb;
            IL_00cb:
                if (serverStream.CanRead)
                {
                    serverStream.BeginRead(_recvDataBuffer, 0, 16384, tcRecvData, serverStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (mTcpClient != null)
                {
                    if (serverStream != null)
                    {
                        serverStream.Close();
                    }

                    mTcpClient.Close();
                }

                bConnected = false;
                if (mIslogout)
                {
                    RaiseEventStatus(new StatusEventArgs(5, ex.Message));
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisConnect();
            if (mTcpClient != null)
            {
                ((IDisposable)mTcpClient).Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
