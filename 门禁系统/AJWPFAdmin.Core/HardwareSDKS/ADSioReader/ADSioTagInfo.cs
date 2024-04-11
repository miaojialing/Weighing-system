using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.ADSioReader
{
    public enum TagType
    {
        TYPE_6B = 1,
        TYPE_6C
    }

    public class ADSioTagInfo : ICloneable
    {
        private byte[] _PCData;

        private byte[] _EPCData;

        private byte[] _CRCData;

        private byte[] _DataBytes;

        public int ID { get; set; }

        public string Name { get; set; }

        public TagType TagType { get; set; }

        public DateTime DateTime { get; set; }

        public int Antenna { get; set; }

        public int Address { get; set; }

        public int CID { get; set; }

        public int Alarm { get; set; }

        public int Count { get; set; }

        public int Length { get; set; }

        public byte[] CRCData
        {
            get
            {
                return _CRCData;
            }
            set
            {
                _CRCData = value;
                try
                {
                    CRCString = ByteArrayToHexString(_CRCData);
                }
                catch
                {
                    CRCString = "/";
                }
            }
        }

        public string CRCString { get; private set; } = "/";


        public byte[] PCData
        {
            get
            {
                return _PCData;
            }
            set
            {
                _PCData = value;
                try
                {
                    PCString = ByteArrayToHexString(_PCData);
                }
                catch
                {
                    PCString = "/";
                }
            }
        }

        public string PCString { get; private set; } = "/";


        public byte[] EPCData
        {
            get
            {
                return _EPCData;
            }
            set
            {
                _EPCData = value;
                try
                {
                    EPCString = ByteArrayToHexString(_EPCData);
                }
                catch
                {
                    EPCString = "/";
                }
            }
        }

        public string EPCString { get; private set; } = "/";


        public byte[] DataBytes
        {
            get
            {
                return _DataBytes;
            }
            set
            {
                _DataBytes = value;
                try
                {
                    DataString = ByteArrayToHexString(_DataBytes);
                }
                catch
                {
                    DataString = "/";
                }
            }
        }

        public string DataString { get; private set; } = "/";


        public string Rssi { get; set; }

        public static bool IsHex(string input)
        {
            string pattern = "^[A-Fa-f0-9]+$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        public static string ByteArrayToHexString(byte[] bytArray)
        {
            StringBuilder stringBuilder = new StringBuilder(bytArray.Length * 3);
            foreach (byte b in bytArray)
            {
                try
                {
                    stringBuilder.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                }
                catch
                {
                }
            }

            return stringBuilder.ToString().ToUpper();
        }

        public static byte[] HexStringToByteArray(string hexStr)
        {
            hexStr = hexStr.Replace(" ", "");
            if (!IsHex(hexStr))
            {
                return null;
            }

            byte[] array = new byte[hexStr.Length / 2];
            for (int i = 0; i < hexStr.Length; i += 2)
            {
                try
                {
                    array[i / 2] = Convert.ToByte(hexStr.Substring(i, 2), 16);
                }
                catch
                {
                }
            }

            return array;
        }

        public static byte[] GetData(byte[] bytArray, int start, int length)
        {
            byte[] array = new byte[length];
            try
            {
                Array.Copy(bytArray, start, array, 0, length);
            }
            catch
            {
            }

            return array;
        }

        public override string ToString()
        {
            return Name;
        }

        public ADSioTagInfo()
        {
            ID = 0;
            Name = "";
            TagType = TagType.TYPE_6C;
            Antenna = 1;
            Address = 65535;
            CID = 0;
            Alarm = 0;
            Count = 1;
            Length = 0;
            _CRCData = null;
            _PCData = null;
            _EPCData = null;
            _DataBytes = null;
            Rssi = "/";
        }

        public ADSioTagInfo(string name)
        {
            ID = 0;
            Name = name;
            TagType = TagType.TYPE_6C;
            Antenna = 1;
            Address = 65535;
            CID = 0;
            Alarm = 0;
            Count = 1;
            Length = 0;
            _CRCData = null;
            _PCData = null;
            _EPCData = null;
            _DataBytes = null;
            Rssi = "/";
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
