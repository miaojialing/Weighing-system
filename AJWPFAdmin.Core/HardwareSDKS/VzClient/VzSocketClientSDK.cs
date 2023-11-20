using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Utils;
using Azure;
using Masuit.Tools.Systems;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using SharpCompress.Common;
using SixLabors.ImageSharp.Memory;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace AJWPFAdmin.Core.HardwareSDKS.VzClient
{
    public enum SocketCmdType : byte
    {
        /// <summary>
        /// 车牌识别推送结果
        /// </summary>
        ivsresult = 0,
        /// <summary>
        /// 心跳包
        /// </summary>
        heartbeat = 1,
        /// <summary>
        /// 获取rtsp地址
        /// </summary>
        get_rtsp_uri,
        /// <summary>
        /// 开关闸
        /// </summary>
        ioctl,
        /// <summary>
        /// 设置推送方式
        /// </summary>
        set_ivsresult
    }

    public class VzSDKErrorEventArgs : EventArgs
    {
        public bool NotifyToUser { get; set; }
        public string ErrMessage { get; set; }
        public Exception ErrorData { get; set; }
    }

    public class VzSocketClientSDK
    {
        private Socket _socket;
        private int _port;
        private string _ip;

        private System.Timers.Timer _heartbeatTimer;

        public EventHandler<string> RtspUriResponseEvent;

        public EventHandler<VzCarPlateResultModel> IVSCarPlateResultEvent;

        public EventHandler<VzSDKErrorEventArgs> ErrorEvent;

        public VzSocketClientSDK(string ip, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _ip = ip;
            _port = port;
        }

        public async Task<bool> ConnectAsync()
        {
            try
            {
                await _socket.ConnectAsync(new IPEndPoint(IPAddress.Parse(_ip), _port));

                // 每30s发送心跳包
                _heartbeatTimer = new System.Timers.Timer(1000 * 30);
                _heartbeatTimer.Elapsed += (s, e) =>
                {
                    try
                    {
                        if (_socket.Connected)
                        {
                            _socket.Send(CreateCmd(SocketCmdType.heartbeat, string.Empty, true), SocketFlags.None);
                        }
                    }
                    catch (SocketException ex)
                    {
                        //await _socket.ConnectAsync(new IPEndPoint(IPAddress.Parse(_ip), _port));

                        _heartbeatTimer.Stop();

                        ErrorEvent?.Invoke(this, new VzSDKErrorEventArgs
                        {
                            NotifyToUser = true,
                            ErrMessage = ex.Message,
                            ErrorData = ex
                        });
                    }

                };

                _heartbeatTimer.Start();

                Task.Factory.StartNew(ReceiveSocketMessage, TaskCreationOptions.LongRunning);

                return true;
            }
            catch (Exception e)
            {
                ErrorEvent?.Invoke(this, new VzSDKErrorEventArgs
                {
                    ErrMessage = e.Message,
                    ErrorData = e,
                });
            }

            return false;
        }

        private void ReceiveSocketMessage()
        {
            var messagebyteArrayList = new List<byte[]>();
            var cleared = false;
            var pacakgeDataLen = 0;

            while (true)
            {
                if (_socket == null || !_socket.Connected)
                {
                    continue;
                }
                try
                {
                    var messgaeByteArray = new byte[8192];

                    var buffSize = _socket.Receive(messgaeByteArray, SocketFlags.None);

                    if (buffSize == 0)
                    {
                        cleared = false;
                        continue;
                    }

                    var header = messgaeByteArray.Take(8).ToArray();

                    if (header.Length < 8
                    || header[0] != (byte)'V'
                    || header[1] != (byte)'Z')
                    {
                        messagebyteArrayList.Add(messgaeByteArray.Take(buffSize).ToArray());

                        if (messagebyteArrayList.Sum(p => p.Length) < pacakgeDataLen)
                        {
                            cleared = false;
                            continue;
                        }

                        messgaeByteArray = MergeByteArrays(messagebyteArrayList).ToArray();

                        header = messgaeByteArray.Take(8).ToArray();
                    }

                    //int recvLen = RecvBlock(ms, header, 8);

                    if (header[2] == 1)
                    {
                        //心跳包
                        cleared = false;
                        continue;
                    }

                    var dataLen = ConvBytesToInt(header, 4);


                    var fullMsgBuff = messgaeByteArray.Skip(8).Take(dataLen).ToArray();

                    // 粘包
                    if (fullMsgBuff.Any(p => p == (byte)'V'))
                    {
                        var body = fullMsgBuff.ToList();
                        var hVIndex = body.FindIndex(p => p == (byte)'V');
                        var hZIndex = hVIndex + 1;
                        if (body.ElementAtOrDefault(hZIndex) == (byte)'Z')
                        {

                            var leftArray = new List<byte>();
                            var rightArray = new List<byte>();
                            for (int i = 0; i < body.Count; i++)
                            {
                                if (i < hVIndex)
                                {
                                    leftArray.Add(body[i]);
                                }
                                else
                                {
                                    rightArray.Add(body[i]);
                                }
                            }

                            messagebyteArrayList.Add(leftArray.ToArray());

                            fullMsgBuff = MergeByteArrays(messagebyteArrayList).Skip(8).ToArray();

                            messagebyteArrayList.Clear();
                            cleared = true;

                            messagebyteArrayList.Add(rightArray.ToArray());
                        }
                    }
                    else
                    {
                        messagebyteArrayList.Add(messgaeByteArray);
                        fullMsgBuff = MergeByteArrays(messagebyteArrayList).Skip(8).ToArray();
                    }

                    if (fullMsgBuff.Length < dataLen)
                    {
                        // 如果实际读取的长度比header里面定义的小, 说明是分包, 需要继续接收
                        System.Diagnostics.Trace.WriteLine($"分包:messagebyteArrayList.count/fullMsgBuff.Length < dataLen:{messagebyteArrayList.Count}/{fullMsgBuff.Length}<{dataLen}");
                        pacakgeDataLen = dataLen;
                        messagebyteArrayList.Add(messgaeByteArray);
                        Thread.Sleep(500);
                        cleared = false;
                        continue;
                    }

                    var cmdType = messgaeByteArray.Take(8).ElementAt(3);

                    // 普通的指令响应
                    var cmdTypeEnum = (SocketCmdType)cmdType;


                    //System.Diagnostics.Trace.WriteLine($"收到数据 newMsgResp:{newMsgResp}");

                    switch (cmdTypeEnum)
                    {
                        case SocketCmdType.get_rtsp_uri:
                            var response = CommonUtil
                            .TryGetJSONObject<JObject>(Encoding.UTF8.GetString(fullMsgBuff));
                            RtspUriResponseEvent?.Invoke(this, response["uri"].ToString());
                            break;
                        case SocketCmdType.set_ivsresult:

                            break;
                        case SocketCmdType.ivsresult:
                            OnIVSResultRecv(fullMsgBuff);
                            break;
                        default:
                            break;
                    }

                    if (!cleared)
                    {
                        messagebyteArrayList.Clear();
                    }

                    pacakgeDataLen = 0;

                    Thread.Sleep(500);
                }
                catch (Exception ex)
                {
                    messagebyteArrayList.Clear();
                    pacakgeDataLen = 0;
                    System.Diagnostics.Trace.WriteLine($"Socket发生异常:{ex.Message}");
                    ErrorEvent?.Invoke(this, new VzSDKErrorEventArgs
                    {
                        ErrMessage = ex.Message,
                        ErrorData = ex
                    });
                    //break;
                }
            }
        }

        public Task<int> ExecuteRTSPUriCmdAsync()
        {
            var msg = new JObject
            {
                { "cmd" , $"{SocketCmdType.get_rtsp_uri}"},
                { "id" , DateTime.Now.Ticks.ToString()},

            }.ToString(Newtonsoft.Json.Formatting.None);

            if (_socket.Connected)
            {
                return _socket.SendAsync(CreateCmd(SocketCmdType.get_rtsp_uri, msg), SocketFlags.None);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">0 断, 1 通, 2 先通后断</param>
        /// <param name="io"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public Task<int> ExecuteIOCtlCmdAsync(int value, int io = 0, int delay = 500)
        {
            var msg = new JObject
            {
                { "cmd" , $"{SocketCmdType.ioctl}"},
                { "id" , DateTime.Now.Ticks.ToString()},
                { nameof(delay) , delay},
                { nameof(io) , io},
                { nameof(value) , value}

            }.ToString(Newtonsoft.Json.Formatting.None);

            if (_socket.Connected)
            {
                return _socket.SendAsync(CreateCmd(SocketCmdType.ioctl, msg), SocketFlags.None);
            }

            return Task.FromResult(0);

        }

        /// <summary>
        /// 配置推送数据方式
        /// </summary>
        /// <param name="enable">是否允许推送识别结果</param>
        /// <param name="image">识别结果是否包含图片</param>
        /// <param name="image_type">识别图片类型, 0 全图, 1 车牌区域小图, 2 两者都有</param>
        /// <param name="format">识别结果数据格式, json 或 bin</param>
        /// <returns></returns>
        public Task<int> ExecuteIVSResultCmdAsync(bool enable = true,
            bool image = true,
            int image_type = 2,
            string format = "json")
        {
            var msg = new JObject
            {
                { "cmd" , $"ivsresult"},
                { "id" , DateTime.Now.Ticks.ToString()},
                { nameof(enable) , enable},
                { nameof(format) , format},
                { nameof(image) , image},
                { nameof(image_type) , image_type}

            }.ToString(Newtonsoft.Json.Formatting.None);

            if (_socket.Connected)
            {
                return _socket.SendAsync(CreateCmd(SocketCmdType.set_ivsresult, msg), SocketFlags.None);
            }
            return Task.FromResult(0);
        }

        private byte[] CreateCmd(SocketCmdType type, string cmd, bool heartbeat = false)
        {
            using var ms = new MemoryStream();

            byte[] header = new byte[8] { (byte)'V', (byte)'Z', 0, (byte)type, 0, 0, 0, 0 };

            if (heartbeat)
            {
                header[2] = 1;
                ms.Write(header, 0, header.Length);
                return ms.ToArray();
            }

            byte[] cmdBytes = Encoding.UTF8.GetBytes(cmd);
            int len = cmdBytes.Length;

            header[4] += (byte)(len >> 24 & 0xFF);
            header[5] += (byte)(len >> 16 & 0xFF);
            header[6] += (byte)(len >> 8 & 0xFF);
            header[7] += (byte)(len & 0xFF);

            ms.Write(header, 0, header.Length);
            ms.Write(cmdBytes, 0, cmdBytes.Length);

            return ms.ToArray();
        }

        public void Shutdown()
        {
            try
            {
                _heartbeatTimer?.Stop();
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            catch
            {
            }
        }



        private void OnIVSResultRecv(byte[] data)
        {
            //接收到识别结果的处理
            if (data[0] == 'I' && data[1] == 'R')
            {
                //二进制的结构体处理
                //ParseBinIVSResult(data, len);
            }
            else
            {
                //json处理
                int pos = 0;
                while (true)
                {
                    if (data[pos] == 0)
                    {
                        break;
                    }
                    pos++;
                }

                var jsonStr = Encoding
                    .GetEncoding("GB2312").GetString(data, 0, pos);
                var obj = CommonUtil
                    .TryGetJSONObject<VzCarPlateResultModel>(jsonStr);

                if (obj == null)
                {
                    ErrorEvent?.Invoke(this, new VzSDKErrorEventArgs
                    {
                        ErrMessage = $"序列化车牌识别数据失败:{jsonStr}",
                    });
                    return;
                }

                if (!CommonRegex.CARNO.IsMatch(obj.PlateResult.license))
                {
                    // 识别失败,或者无车牌, 直接忽略
                    ErrorEvent?.Invoke(this, new VzSDKErrorEventArgs
                    {
                        ErrMessage = $"车牌号有误:{obj.PlateResult.license}",
                    });
                    return;
                }

                try
                {
                    obj.FullImgFile = Path.Combine(Path.GetTempPath(), $"full_{obj.PlateResult.license}_{SnowFlake.GetInstance().GetUniqueShortId()}.jpg");

                    obj.ClipImgFile = Path.Combine(Path.GetTempPath(), $"clip_{obj.PlateResult.license}_{SnowFlake.GetInstance().GetUniqueShortId()}.jpg");

                    using var fullImageWriter = new FileStream(obj.FullImgFile, FileMode.OpenOrCreate);
                    fullImageWriter
                        .Write(data.Skip(pos + 1).Take(obj.fullImgSize).ToArray(), 0, obj.fullImgSize);

                    using var clipImageWriter = new FileStream(obj.ClipImgFile, FileMode.OpenOrCreate);
                    clipImageWriter
                        .Write(data.Skip(pos + 1 + obj.fullImgSize).Take(obj.clipImgSize).ToArray(),
                        0, obj.clipImgSize);
                }
                catch (Exception e)
                {
                    ErrorEvent?.Invoke(this, new VzSDKErrorEventArgs
                    {
                        ErrMessage = $"保存临时车牌识别图片失败:{e.Message}",
                    });
                }

                IVSCarPlateResultEvent?.Invoke(this, obj);

            }
        }

        private static byte[] MergeByteArrays(List<byte[]> byteArrays)
        {
            int totalLength = byteArrays.Sum(array => array.Length);
            byte[] mergedByteArray = new byte[totalLength];

            int currentIndex = 0;
            foreach (byte[] byteArray in byteArrays)
            {
                Buffer.BlockCopy(byteArray, 0, mergedByteArray, currentIndex, byteArray.Length);
                currentIndex += byteArray.Length;
            }

            return mergedByteArray;
        }

        private int RecvPacketSize(ref byte[] msgBuff, out short cmdType)
        {
            //byte[] header = new byte[8];

            var header = msgBuff.Take(8).ToArray();

            //int recvLen = RecvBlock(ms, header, 8);

            cmdType = 0;

            if (header.Length <= 0 || header.Length < 8)
            {
                return -1;
            }

            if (header[0] != (byte)'V' || header[1] != (byte)'Z')
            {
                //格式不对
                return -1;
            }

            if (header[2] == 1)
            {
                //心跳包
                return 0;
            }

            cmdType = header[3];

            var dataLen = ConvBytesToInt(header, 4);

            if (dataLen > 0)
            {
                var body = msgBuff.Skip(8).ToList();
                var hdIndex = body.FindIndex(p => p == (byte)'V');
                // 粘包
                if (hdIndex != -1)
                {

                }

                var realCount = msgBuff.Skip(8).Take(dataLen).Count();

                // 如果实际读取的长度比header里面定义的小, 说明是分包, 需要继续接收, 返回 -1
                if (realCount < dataLen)
                {
                    dataLen = -1;
                }
            }

            return dataLen;
        }

        private static int ConvBytesToInt(byte[] buff, int offset)
        {
            //4bytes 转为int，要考虑机器的大小端问题
            int len = 0;
            byte byteValue;

            byteValue = (byte)(0x000000FF & buff[offset]);
            len += byteValue << 24;
            byteValue = (byte)(0x000000FF & buff[offset + 1]);
            len += byteValue << 16;
            byteValue = (byte)(0x000000FF & buff[offset + 2]);
            len += byteValue << 8;
            byteValue = (byte)(0x000000FF & buff[offset + 3]);
            len += byteValue;
            return len;
        }

        private int RecvBlock(MemoryStream sm, byte[] buff, int len)
        {
            try
            {
                int totLeRecvLen = 0;
                int recvLen;
                while (totLeRecvLen < len)
                {
                    recvLen = sm.Read(buff, totLeRecvLen, len - totLeRecvLen);
                    totLeRecvLen += recvLen;
                }
                return len;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine($"RecvBlock 异常:{e.Message}");
                return -1;
            }
        }
    }
}
