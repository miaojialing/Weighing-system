using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.ADSioReader
{

    public class ProtocolPacket
    {
        public byte Preamble { get; set; } = 124;


        public byte[] Address { get; set; } = new byte[2] { 255, 255 };


        public byte Code { get; set; } = 0;


        public byte Type { get; set; } = 0;


        public byte Length { get; set; } = 0;


        public byte[] Payload { get; set; } = new byte[255];


        public byte Checksum { get; set; } = 0;


        public ProtocolPacket()
        {
        }

        public ProtocolPacket(byte[] iData)
        {
            Append(iData);
        }

        public ProtocolPacket(byte nCode, byte nType)
        {
            BuildCmdPacketByte(124, 65535, nCode, nType, null);
        }

        public ProtocolPacket(byte nCode, byte nType, byte[] ArgByte)
        {
            BuildCmdPacketByte(124, 65535, nCode, nType, ArgByte);
        }

        public ProtocolPacket(int nAddr, byte nCode, byte nType)
        {
            BuildCmdPacketByte(124, nAddr, nCode, nType, null);
        }

        public ProtocolPacket(int nAddr, byte nCode, byte nType, byte[] ArgByte)
        {
            BuildCmdPacketByte(124, nAddr, nCode, nType, ArgByte);
        }

        public ProtocolPacket(byte nHead, int nAddr, byte nCode, byte nType, byte[] ArgByte)
        {
            BuildCmdPacketByte(nHead, nAddr, nCode, nType, ArgByte);
        }

        public byte[] BuildCmdPacketByte(byte nHead, int nAddr, byte nCode, byte nType, byte[] ArgByte)
        {
            Preamble = nHead;
            Address[0] = (byte)(nAddr >> 8);
            Address[1] = (byte)nAddr;
            Code = nCode;
            Type = nType;
            if (ArgByte != null)
            {
                Length = (byte)ArgByte.Length;
            }
            else
            {
                Length = 0;
            }

            for (int i = 0; i < Length; i++)
            {
                Payload[i] = ArgByte[i];
            }

            Checksum = CheckSum();
            return ToArray();
        }

        private byte CheckSum()
        {
            byte b = 0;
            b = (byte)(b + Preamble);
            b = (byte)(b + Address[0]);
            b = (byte)(b + Address[1]);
            b = (byte)(b + Code);
            b = (byte)(b + Type);
            b = (byte)(b + Length);
            for (int i = 0; i < Length; i++)
            {
                b = (byte)(b + Payload[i]);
            }

            return Convert.ToByte((~b + 1) & 0xFF);
        }

        public byte CheckSum(byte[] btAryBuffer, int nStartPos, int nLen)
        {
            byte b = 0;
            for (int i = nStartPos; i < nStartPos + nLen; i++)
            {
                b = (byte)(b + btAryBuffer[i]);
            }

            return Convert.ToByte((~b + 1) & 0xFF);
        }

        public void Append(byte[] iDataArray)
        {
            int i = 0;
            int num = iDataArray.Length;
            try
            {
                Preamble = iDataArray[i++];
                Address[0] = iDataArray[i++];
                Address[1] = iDataArray[i++];
                Code = iDataArray[i++];
                Type = iDataArray[i++];
                Length = iDataArray[i++];
                Checksum = iDataArray[i + Length];
                for (; i < ((num > 255) ? 255 : num); i++)
                {
                    Payload[i - 6] = iDataArray[i];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ToArray()
        {
            List<byte> list = new List<byte>();
            list.Add(Preamble);
            list.AddRange(Address);
            list.Add(Code);
            list.Add(Type);
            list.Add(Length);
            if (Code == 17 && Payload[0] == 14)
            {
                for (int i = 0; i < Length * 14 + 1; i++)
                {
                    try
                    {
                        list.Add(Payload[i]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            else
            {
                for (int j = 0; j < Length; j++)
                {
                    try
                    {
                        list.Add(Payload[j]);
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2.Message);
                    }
                }

                list.Add(Checksum);
            }

            return list.ToArray();
        }
    }

    public class ProtocolEventArgs : EventArgs
    {
        public byte[] Data { get; }

        public ProtocolPacket Protocol => new ProtocolPacket(Data);

        public ProtocolEventArgs(byte[] iData)
        {
            Data = iData;
        }
    }

    public delegate void ProtocolEventHandler(object sender, ProtocolEventArgs e);

    [Guid("714D7956-577A-4BA4-A922-59B44EDCE443")]
    [ProgId("ADActiveXADRcp")]
    [ComVisible(true)]
    public class RcpBase : ActiveXControl, IDisposable
    {
        public const byte RCP_AP_ISO6B_IDEN = 1;

        public const byte RCP_AP_ISO6B_RW = 2;

        public const byte RCP_AP_ISO6B_LOCK = 4;

        public const byte RCP_AP_ISO6B_UNLOCK = 5;

        public const byte RCP_AP_ISO6B_KILL = 6;

        public const byte RCP_AP_EPC_IDEN = 16;

        public const byte RCP_AP_EPC_MULT = 17;

        public const byte RCP_AP_EPC_RW = 18;

        public const byte RCP_AP_EPC_LOCK = 20;

        public const byte RCP_AP_EPC_UNLOCK = 21;

        public const byte RCP_AP_EPC_KILL = 22;

        public const byte RCP_AP_EPC_APPOINT = 34;

        public const byte RCP_AP_PARA = 129;

        public const byte RCP_AP_CANBUS = 184;

        public const byte RCP_AP_TCPIP = 185;

        public const byte RCP_AP_SYRIS = 186;

        public const byte RCP_AP_REMOTE = 187;

        public const byte RCP_AP_TIME = 131;

        public const byte RCP_AP_SECRET = 48;

        public const byte RCP_AP_OUTCARD = 49;

        public const byte RCP_AP_INITSYRIS = 141;

        public const byte RCP_AP_SOFTRESET = 143;

        public const byte RCP_AP_USBPOWER = 188;

        public const byte RCP_AP_USBPARA = 189;

        public const byte RCP_AP_WIFI_DOWNLOAD = 231;

        public const byte RCP_AP_WIFI_PUB = 232;

        public const byte RCP_AP_WIFI_STA = 233;

        public const byte RCP_AP_WIFI_AP = 234;

        public const byte PREAMBLE = 124;

        public const byte PREAMBLE_UP = 204;

        public const byte INDEX_PREAMBLE = 0;

        public const byte INDEX_ADDRESS = 1;

        public const byte INDEX_CODE = 3;

        public const byte INDEX_TYPE = 4;

        public const byte INDEX_RTN = 4;

        public const byte INDEX_LENGTH = 5;

        public const byte INDEX_PAYLOAD = 6;

        public const byte LEN_ENDMARK = 0;

        public const byte LEN_CHECKSUM = 1;

        public const byte LEN_PROTOCOL_MIN = 7;

        public const byte RCP_MSG_OK = 0;

        public const byte RCP_MSG_ERR = 1;

        public const byte RCP_MSG_CMD = 0;

        public const byte RCP_MSG_RSP = 1;

        public const byte RCP_MSG_NOTI = 2;

        public const byte RCP_MSG_AUTO = 5;

        public const byte RCP_MSG_AUTO_AP = 50;

        public const byte RCP_MSG_AUTO_AA = 33;

        public const byte RCP_MSG_SET = 49;

        public const byte RCP_MSG_GET = 50;

        public const byte RCP_MSG_SENIOR_SET = 33;

        public const byte RCP_MSG_SENIOR_GET = 34;

        public const byte RCP_CMD_INFO = 130;

        public const byte RCP_IR_PARA = 129;

        public const byte RCP_IR_ANT = 131;

        public const byte RCP_IR_EPT = 132;

        public const byte RCP_IR_ADDR = 133;

        public const byte RCP_IR_UART = 134;

        public const byte RCP_IR_OUTCARD = 135;

        public const byte RCP_IR_GET_TEMPERATURE = 136;

        public const byte RCP_IR_POWERDOWN_MODE = 137;

        public const byte RCP_IR_RESET = 208;

        public const byte RCP_IR_RELOAD = 209;

        public const byte RCP_IR_UPDATE_FLASH = 210;

        public const byte RCP_IR_ERASE_FLASH = 211;

        public const byte RCP_IR_GET_REGISTRY_ITEM = 212;

        public const byte RCP_IR_SET_REGISTRY_ITEM = 213;

        public const byte RCP_IR_GET_GPIO = 214;

        public const byte RCP_IR_SET_GPIO = 215;

        public const byte RCP_IR_GET_TX_PWR = 80;

        public const byte RCP_IR_SET_TX_PWR = 81;

        public const byte RCP_IR_GET_REGION = 82;

        public const byte RCP_IR_SET_REGION = 83;

        public const byte RCP_IR_GET_CH = 84;

        public const byte RCP_IR_SET_CH = 85;

        public const byte RCP_IR_GET_HOPPING_TBL = 86;

        public const byte RCP_IR_SET_HOPPING_TBL = 87;

        public const byte RCP_IR_GET_MODULATION = 88;

        public const byte RCP_IR_SET_MODULATION = 89;

        public const byte RCP_IR_GET_FH_LBT = 90;

        public const byte RCP_IR_SET_FH_LBT = 91;

        public const byte RCP_IR_READ_B_UII = 16;

        public const byte RCP_IR_READ_B_DT = 17;

        public const byte RCP_IR_WRITE_B_DT = 18;

        public const byte RCP_IR_LOCK_B = 22;

        public const byte RCP_IR_QRY_LOCK_B = 23;

        public const byte RCP_IR_READ_C_UII = 32;

        public const byte RCP_IR_READ_C_DT = 33;

        public const byte RCP_IR_WRITE_C_DT = 34;

        public const byte RCP_IR_LOCK_C = 38;

        public const byte RCP_IR_KILL_RECOM_C = 40;

        public const byte RCP_IR_SECRET_C_DT = 42;

        public const byte RCP_IR_GET_ACCESS_EPC_MATCH = 44;

        public const byte RCP_IR_SET_ACCESS_EPC_MATCH = 45;

        public const byte RCP_IR_GET_C_SEL_PARM = 64;

        public const byte RCP_IR_SET_C_SEL_PARM = 65;

        public const byte RCP_IR_GET_C_QRY_PARM = 66;

        public const byte RCP_IR_SET_C_QRY_PARM = 67;

        public const byte RCP_IR_STRT_AUTO_READ_EX = 50;

        public const byte RCP_IR_STOP_AUTO_READ_EX = 51;

        public const byte RCP_IR_CTRL_AUTO_READ = 52;

        public const byte RCP_IR_GET_IMPINJ_FAST_TID = 120;

        public const byte RCP_IR_SET_IMPINJ_FAST_TID = 121;

        public const byte RCP_MM_PARA = 129;

        public const byte RCP_MM_ANT = 131;

        public const byte RCP_MM_EPT = 132;

        public const byte RCP_MM_ADDR = 133;

        public const byte RCP_MM_UART = 134;

        public const byte RCP_MM_OUTCARD = 135;

        public const byte RCP_MM_GET_TEMPERATURE = 136;

        public const byte RCP_MM_POWERDOWN_MODE = 137;

        public const byte RCP_MM_RESET = 208;

        public const byte RCP_MM_RELOAD = 209;

        public const byte RCP_MM_UPDATE_FLASH = 210;

        public const byte RCP_MM_ERASE_FLASH = 211;

        public const byte RCP_MM_GET_REGISTRY_ITEM = 212;

        public const byte RCP_MM_SET_REGISTRY_ITEM = 213;

        public const byte RCP_MM_GET_GPIO = 214;

        public const byte RCP_MM_SET_GPIO = 215;

        public const byte RCP_MM_GET_TX_PWR = 80;

        public const byte RCP_MM_SET_TX_PWR = 81;

        public const byte RCP_MM_GET_REGION = 82;

        public const byte RCP_MM_SET_REGION = 83;

        public const byte RCP_MM_GET_CH = 84;

        public const byte RCP_MM_SET_CH = 85;

        public const byte RCP_MM_GET_HOPPING_TBL = 86;

        public const byte RCP_MM_SET_HOPPING_TBL = 87;

        public const byte RCP_MM_GET_MODULATION = 88;

        public const byte RCP_MM_SET_MODULATION = 89;

        public const byte RCP_MM_GET_FH_LBT = 90;

        public const byte RCP_MM_SET_FH_LBT = 91;

        public const byte RCP_MM_READ_C_UII = 32;

        public const byte RCP_MM_READ_C_DT = 33;

        public const byte RCP_MM_WRITE_C_DT = 34;

        public const byte RCP_MM_LOCK_C = 38;

        public const byte RCP_MM_KILL_RECOM_C = 40;

        public const byte RCP_MM_SECRET_C_DT = 42;

        public const byte RCP_MM_GET_ACCESS_EPC_MATCH = 44;

        public const byte RCP_MM_SET_ACCESS_EPC_MATCH = 45;

        public const byte RCP_MM_GET_C_SEL_PARM = 64;

        public const byte RCP_MM_SET_C_SEL_PARM = 65;

        public const byte RCP_MM_GET_C_QRY_PARM = 66;

        public const byte RCP_MM_SET_C_QRY_PARM = 67;

        public const byte RCP_MM_STRT_AUTO_READ_EX = 50;

        public const byte RCP_MM_STOP_AUTO_READ_EX = 51;

        public const byte RCP_MM_CTRL_AUTO_READ = 52;

        protected List<byte> m_oByteRxPkt = new List<byte>();

        public int Address = 65535;

        public string Mode = "X";

        public string Type = "X";

        public string Version = "V0.00";

        private bool disposedValue = false;

        public int VersionNumber => Convert.ToInt32(Version.Replace("V", "").Replace(".", ""));

        public event ProtocolEventHandler RxRspParsed;

        public event ProtocolEventHandler TxRspParsed;

        public byte CheckSum(byte[] btAryBuffer, int nStartPos, int nLen)
        {
            byte b = 0;
            for (int i = nStartPos; i < nStartPos + nLen; i++)
            {
                b = (byte)(b + btAryBuffer[i]);
            }

            return Convert.ToByte((~b + 1) & 0xFF);
        }

        public int GetVersion()
        {
            string version = Version;
            version = version.Replace("V", "");
            version = version.Replace(".", "");
            return Convert.ToInt16(version);
        }

        public void ReciveBytePkt(byte[] iData)
        {
            if (iData.Length == 0)
            {
                return;
            }

            m_oByteRxPkt.AddRange(iData);
            int num = 0;
            if (m_oByteRxPkt.Count < 7)
            {
                return;
            }

            for (num = 0; num < m_oByteRxPkt.Count; num++)
            {
                if (m_oByteRxPkt[num] == 204)
                {
                    if (m_oByteRxPkt.Count - num < 7)
                    {
                        return;
                    }

                    int num2 = m_oByteRxPkt[num + 1] | (m_oByteRxPkt[num + 1 + 1] << 8);
                    if (Address == 65535)
                    {
                        Address = num2;
                    }

                    byte b = (byte)(m_oByteRxPkt[num + 4] & 0x7Fu);
                    if ((num2 == 65535 || num2 == Address) && (b == 0 || b == 1 || b == 2 || b == 5 || b == 50 || b == 33 || m_oByteRxPkt[num + 3] == 231))
                    {
                        break;
                    }
                }
            }

            if (num != 0)
            {
                m_oByteRxPkt.RemoveRange(0, num);
            }

            while (m_oByteRxPkt.Count >= 7)
            {
                int num3 = m_oByteRxPkt[5];
                if (m_oByteRxPkt.Count >= num3 + 7)
                {
                    ProtocolPacket pst = new ProtocolPacket(m_oByteRxPkt.GetRange(0, num3 + 7).ToArray());
                    ParseMsgRspType(pst);
                    m_oByteRxPkt.RemoveRange(0, num3 + 7);
                    continue;
                }

                int num4 = m_oByteRxPkt[1] | (m_oByteRxPkt[2] << 8);
                if (num4 != 65535 && num4 != Address)
                {
                    m_oByteRxPkt.Clear();
                }

                break;
            }
        }

        public void ShowBytePack(ProtocolPacket pst)
        {
            this.TxRspParsed?.Invoke(this, new ProtocolEventArgs(pst.ToArray()));
        }

        protected virtual void ParseMsgRspType(ProtocolPacket pst)
        {
            if (pst.Code == 130 && pst.Length >= 34)
            {
                ParseMsgRspInfo(pst);
            }

            this.RxRspParsed?.Invoke(this, new ProtocolEventArgs(pst.ToArray()));
        }

        protected void ParseMsgRspInfo(ProtocolPacket pst)
        {
            if (pst.Code == 130 && pst.Preamble == 204)
            {
                string @string = Encoding.ASCII.GetString(pst.Payload, 0, pst.Length);
                Type = @string.Substring(17, 1);
                if (pst.Payload[17] < 48)
                {
                    Type = "C";
                }

                Mode = @string.Substring(18, 1);
                try
                {
                    Version = @string.Substring(19, 5);
                    Address = Convert.ToInt32(@string.Substring(29, 5));
                }
                catch
                {
                    Version = "V1.00";
                    Address = 65535;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
