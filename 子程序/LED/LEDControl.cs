using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LED
{
    public class LEDControl : ILED
    {
        private static readonly ILog log = LogManager.GetLogger("LEDControl");
        LedDll.COMMUNICATIONINFO CommunicationInfo;//定义一通讯参数结构体变量用于对设定的LED通讯
        int hProgram;//节目句柄
        LedDll.AREARECT AreaRect;//区域坐标属性结构体变量
        LedDll.FONTPROP FontProp;
        LedDll.PLAYPROP PlayProp;
        public int AreaWidth { get; set; }
        public int AreaHeight { get; set; }
        public string getplateFile { set; get; } = "./Data/LedText/getplate.txt";
        public string isweighingFile { set; get; } = "./Data/LedText/isweighing.txt";
        public string isDelayNZStableFile { set; get; } = "./Data/LedText/isDelayNZStable.txt";
        public string weighingcompleteFile { set; get; } = "./Data/LedText/weighingcomplete.txt";
        public string readytoweighFile { set; get; } = "./Data/LedText/readytoweigh.txt";
        public string Name { set; get; }

        public LEDControl(LEDConfig config)
        {
            Name = config.IP;
            AreaWidth= config.AreaWidth;
            AreaHeight = config.AreaHeight;
            //TCP通讯
            CommunicationInfo = new LedDll.COMMUNICATIONINFO();
            CommunicationInfo.SendType = 0;//设为固定IP通讯模式，即TCP通讯
            CommunicationInfo.IpStr = config.IP;//给IpStr赋值LED控制卡的IP
            CommunicationInfo.LedNumber = 1;//LED屏号为1
        }
        public void GET_PLATE()
        {
            log.Info("getplate");

            hProgram = LedDll.LV_CreateProgram(AreaWidth, AreaHeight, 1);//根据传的参数创建节目句柄
            LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目

            AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = AreaWidth;
            AreaRect.height = AreaHeight;

            LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

            FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 1;

            PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 0;
            PlayProp.DelayTime = 3;
            PlayProp.Speed = 4;

            LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, getplateFile,
                ref FontProp, ref PlayProp, 2, 1);

            int ret = LedDll.LV_Send(ref CommunicationInfo, hProgram);

            if (ret == 0)
            {
                log.Info("发送成功");
            }
            else
            {
                log.Info("发送失败：" + ret);
            }

            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象
        }
        public void IS_WEIGHING()
        {
            log.Info("isweighing");

            hProgram = LedDll.LV_CreateProgram(AreaWidth, AreaHeight, 1);//根据传的参数创建节目句柄
            LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目

            AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = AreaWidth;
            AreaRect.height = AreaHeight;

            LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

            FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 1;

            PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 0;
            PlayProp.DelayTime = 3;
            PlayProp.Speed = 4;

            LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, isweighingFile,
                ref FontProp, ref PlayProp, 2, 1);

            LedDll.LV_Send(ref CommunicationInfo, hProgram);

            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象
        }
        public void IS_STABLE()
        {
            log.Info("isstable");

            hProgram = LedDll.LV_CreateProgram(AreaWidth, AreaHeight, 1);//根据传的参数创建节目句柄
            LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目

            AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = AreaWidth;
            AreaRect.height = AreaHeight;

            LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

            FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 1;

            PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 0;
            PlayProp.DelayTime = 3;
            PlayProp.Speed = 4;

            LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, isDelayNZStableFile,
                ref FontProp, ref PlayProp, 2, 1);

            LedDll.LV_Send(ref CommunicationInfo, hProgram);

            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象
        }
        public void WEIGHING_COMPLETE()
        {
            int flag = 0;
            log.Info("weighingcomplete");

            hProgram = LedDll.LV_CreateProgram(AreaWidth, AreaHeight, 1);//根据传的参数创建节目句柄
            flag = LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目

            AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = AreaWidth;
            AreaRect.height = AreaHeight;

            flag = LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

            FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "Terminal";
            FontProp.FontSize = 10;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;

            PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 0;
            PlayProp.DelayTime = 3;
            PlayProp.Speed = 4;


            flag = LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, weighingcompleteFile,
                ref FontProp, ref PlayProp, 0, 0);

            flag = LedDll.LV_Send(ref CommunicationInfo, hProgram);

            var error= LedDll.LS_GetError(flag);

            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象

        }
        public void READY_TO_WEIGH()
        {
            log.Info("readytoweigh");

            hProgram = LedDll.LV_CreateProgram(AreaWidth, AreaHeight, 1);

            LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目

            LedDll.AREARECT AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = AreaWidth;
            AreaRect.height = 16;

            LedDll.FONTPROP FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "宋体";
            FontProp.FontSize = 12;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;

            LedDll.PLAYPROP PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 6;
            PlayProp.DelayTime = 1;
            PlayProp.Speed = 4;

            LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);
            LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, readytoweighFile, ref FontProp, ref PlayProp, 0, 0);//通过字符串添加一个多行文本到图文区，参数说明见声明注示

            AreaRect.left = 0;
            AreaRect.top = AreaHeight - 32;
            AreaRect.width = AreaWidth;
            AreaRect.height = 32;
            LedDll.DIGITALCLOCKAREAINFO DigitalClockAreaInfo = new LedDll.DIGITALCLOCKAREAINFO();
            DigitalClockAreaInfo.TimeColor = LedDll.COLOR_RED;

            DigitalClockAreaInfo.ShowStrFont.FontName = "宋体";
            DigitalClockAreaInfo.ShowStrFont.FontSize = 12;
            DigitalClockAreaInfo.IsShowYear = 1;
            DigitalClockAreaInfo.IsShowMonth = 1;
            DigitalClockAreaInfo.IsShowDay = 1;
            DigitalClockAreaInfo.IsShowHour = 1;
            DigitalClockAreaInfo.IsShowMinute = 1;
            DigitalClockAreaInfo.DateFormat = 4;
            DigitalClockAreaInfo.TimeFormat = 2;
            DigitalClockAreaInfo.IsMutleLineShow = 1;


            LedDll.LV_AddDigitalClockArea(hProgram, 1, 2, ref AreaRect, ref DigitalClockAreaInfo);//注意区域号不能一样，详见函数声明注示

            LedDll.LV_Send(ref CommunicationInfo, hProgram);//发送，见函数声明注示
            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象，详见函数声明注示
        }
        public void SendTxtToLed(string path)
        {
            hProgram = LedDll.LV_CreateProgram(AreaWidth, AreaHeight, 1);//根据传的参数创建节目句柄
            LedDll.LV_AddProgram(hProgram, 1, 0, 1);//添加一个节目

            AreaRect = new LedDll.AREARECT();//区域坐标属性结构体变量
            AreaRect.left = 0;
            AreaRect.top = 0;
            AreaRect.width = AreaWidth;
            AreaRect.height = AreaHeight;

            LedDll.LV_AddImageTextArea(hProgram, 1, 1, ref AreaRect, 0);

            FontProp = new LedDll.FONTPROP();//文字属性
            FontProp.FontName = "Terminal";
            FontProp.FontSize = 10;
            FontProp.FontColor = LedDll.COLOR_RED;
            FontProp.FontBold = 0;

            PlayProp = new LedDll.PLAYPROP();
            PlayProp.InStyle = 0;
            PlayProp.DelayTime = 3;
            PlayProp.Speed = 4;

            LedDll.LV_AddMultiLineTextToImageTextArea(hProgram, 1, 1, LedDll.ADDTYPE_FILE, path,
                ref FontProp, ref PlayProp, 0, 0);

            LedDll.LV_Send(ref CommunicationInfo, hProgram);

            LedDll.LV_DeleteProgram(hProgram);//删除节目内存对象
        }
    }
}
