using System;
using System.Runtime.InteropServices;

namespace Monitor
{
    public class CHCNetSDK
    {
        #region 调用HCNetSDK.dll

        public const int SERIALNO_LEN = 48;//序列号长度
        public const int STREAM_ID_LEN = 32;
        public const int NAME_LEN = 32;//用户名长度
        public const int MAX_TIMESEGMENT_V30 = 8;//9000设备最大时间段数
        public const int MAX_SHELTERNUM = 4;//8000设备最大遮挡区域数
        public const int MAX_ANALOG_ALARMOUT = 32; //最大32路模拟报警输出 
        public const int MAX_IP_ALARMOUT_V40 = 4096;    //允许加入的最多报警输出数
        public const int MAX_ALARMOUT_V40 = MAX_IP_ALARMOUT_V40 + MAX_ANALOG_ALARMOUT;//4128
        public const int MAX_DAYS = 7;
        public const int MAX_CHANNUM_V40 = 512;
        public const int MAX_MULTI_AREA_NUM = 24;
        public const int NET_DVR_GET_PICCFG_V40 = 6179;//获取图象参数V40扩展
        public const int NET_DVR_SET_PICCFG_V40 = 6180;//设置图象参数V40扩展

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Init();

        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_Login_V30(string sDVRIP, Int32 wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWINFO
        {
            public Int32 lChannel;//通道号
            public uint dwStreamType;	// 码流类型，0-主码流，1-子码流，2-码流3，3-码流4 等以此类推
            public uint dwLinkMode;// 0：TCP方式,1：UDP方式,2：多播方式,3 - RTP方式，4-RTP/RTSP,5-RSTP/HTTP 
            public IntPtr hPlayWnd;//播放窗口的句柄,为NULL表示不播放图象
            public bool bBlocked;  //0-非阻塞取流, 1-阻塞取流, 如果阻塞SDK内部connect失败将会有5s的超时才能够返回,不适合于轮询取流操作.
            public bool bPassbackRecord; //0-不启用录像回传,1启用录像回传
            public byte byPreviewMode;//预览模式，0-正常预览，1-ccc延迟预览
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byStreamID;//流ID，lChannel为0xffffffff时启用此参数
            public byte byProtoType; //应用层取流协议，0-私有协议，1-RTSP协议
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public uint dwDisplayBufNum;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 216, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        public delegate void REALDATACALLBACK(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser);

        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_RealPlay_V40(int iUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, REALDATACALLBACK fRealDataCallBack_V30, IntPtr pUser);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;  //序列号
            public byte byAlarmInPortNum;		        //报警输入个数
            public byte byAlarmOutPortNum;		        //报警输出个数
            public byte byDiskNum;				    //硬盘个数
            public byte byDVRType;				    //设备类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;				    //模拟通道个数
            public byte byStartChan;			        //起始通道号,例如DVS-1,DVR - 1
            public byte byAudioChanNum;                //语音通道数
            public byte byIPChanNum;					//最大数字通道个数，低位  
            public byte byZeroChanNum;			//零通道编码个数 //2010-01-16
            public byte byMainProto;			//主码流传输协议类型 0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySubProto;				//子码流传输协议类型0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySupport;        //能力，位与结果为0表示不支持，1表示支持，
                                          //bySupport & 0x1, 表示是否支持智能搜索
                                          //bySupport & 0x2, 表示是否支持备份
                                          //bySupport & 0x4, 表示是否支持压缩参数能力获取
                                          //bySupport & 0x8, 表示是否支持多网卡
                                          //bySupport & 0x10, 表示支持远程SADP
                                          //bySupport & 0x20, 表示支持Raid卡功能
                                          //bySupport & 0x40, 表示支持IPSAN 目录查找
                                          //bySupport & 0x80, 表示支持rtp over rtsp
            public byte bySupport1;        // 能力集扩充，位与结果为0表示不支持，1表示支持
                                           //bySupport1 & 0x1, 表示是否支持snmp v30
                                           //bySupport1 & 0x2, 支持区分回放和下载
                                           //bySupport1 & 0x4, 是否支持布防优先级	
                                           //bySupport1 & 0x8, 智能设备是否支持布防时间段扩展
                                           //bySupport1 & 0x10, 表示是否支持多磁盘数（超过33个）
                                           //bySupport1 & 0x20, 表示是否支持rtsp over http	
                                           //bySupport1 & 0x80, 表示是否支持车牌新报警信息2012-9-28, 且还表示是否支持NET_DVR_IPPARACFG_V40结构体
            public byte bySupport2; /*能力，位与结果为0表示不支持，非0表示支持							
							bySupport2 & 0x1, 表示解码器是否支持通过URL取流解码
							bySupport2 & 0x2,  表示支持FTPV40
							bySupport2 & 0x4,  表示支持ANR
							bySupport2 & 0x8,  表示支持CCD的通道参数配置
							bySupport2 & 0x10,  表示支持布防报警回传信息（仅支持抓拍机报警 新老报警结构）
							bySupport2 & 0x20,  表示是否支持单独获取设备状态子项
							bySupport2 & 0x40,  表示是否是码流加密设备*/
            public ushort wDevType;              //设备型号
            public byte bySupport3; //能力集扩展，位与结果为0表示不支持，1表示支持
                                    //bySupport3 & 0x1, 表示是否多码流
                                    // bySupport3 & 0x4 表示支持按组配置， 具体包含 通道图像参数、报警输入参数、IP报警输入、输出接入参数、
                                    // 用户参数、设备工作状态、JPEG抓图、定时和时间抓图、硬盘盘组管理 
                                    //bySupport3 & 0x8为1 表示支持使用TCP预览、UDP预览、多播预览中的"延时预览"字段来请求延时预览（后续都将使用这种方式请求延时预览）。而当bySupport3 & 0x8为0时，将使用 "私有延时预览"协议。
                                    //bySupport3 & 0x10 表示支持"获取报警主机主要状态（V40）"。
                                    //bySupport3 & 0x20 表示是否支持通过DDNS域名解析取流

            public byte byMultiStreamProto;//是否支持多码流,按位表示,0-不支持,1-支持,bit1-码流3,bit2-码流4,bit7-主码流，bit-8子码流
            public byte byStartDChan;		//起始数字通道号,0表示无效
            public byte byStartDTalkChan;	//起始数字对讲通道号，区别于模拟对讲通道号，0表示无效
            public byte byHighDChanNum;		//数字通道个数，高位
            public byte bySupport4;
            public byte byLanguageType;// 支持语种能力,按位表示,每一位0-不支持,1-支持  
                                       //  byLanguageType 等于0 表示 老设备
                                       //  byLanguageType & 0x1表示支持中文
                                       //  byLanguageType & 0x2表示支持英文
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;		//保留
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CaptureJPEGPicture(int lUserID, int lChannel, ref NET_DVR_JPEGPARA lpJpegPara, string sPicFileName);

        //动态生成I帧
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MakeKeyFrame(Int32 lUserID, Int32 lChannel);//主码流

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_MakeKeyFrameSub(Int32 lUserID, Int32 lChannel);//子码流

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SaveRealData(Int32 lRealHandle, string sFileName);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopSaveRealData(Int32 lRealHandle);

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_JPEGPARA
        {
            /*注意：当图像压缩分辨率为VGA时，支持0=CIF, 1=QCIF, 2=D1抓图，
            当分辨率为3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA,7=XVGA, 8=HD900p
            仅支持当前分辨率的抓图*/
            public ushort wPicSize;/* 0=CIF, 1=QCIF, 2=D1 3=UXGA(1600x1200), 4=SVGA(800x600), 5=HD720p(1280x720),6=VGA*/
            public ushort wPicQuality;/* 图片质量系数 0-最好 1-较好 2-一般 */
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_COLOR
        {
            public byte byBrightness;/*亮度,0-255*/
            public byte byContrast;/*对比度,0-255*/
            public byte bySaturation;/*饱和度,0-255*/
            public byte byHue;/*色调,0-255*/
        }

        //时间段(子结构)
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SCHEDTIME
        {
            public byte byStartHour;//开始时间
            public byte byStartMin;//开始时间
            public byte byStopHour;//结束时间
            public byte byStopMin;//结束时间
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_VICOLOR
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_COLOR[] struColor;/*图象参数(第一个有效，其他三个保留)*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struHandleTime;/*处理时间段(保留)*/
        }

        //遮挡区域(子结构)
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct NET_DVR_SHELTER
        {
            public ushort wHideAreaTopLeftX;/* 遮挡区域的x坐标 */
            public ushort wHideAreaTopLeftY;/* 遮挡区域的y坐标 */
            public ushort wHideAreaWidth;/* 遮挡区域的宽 */
            public ushort wHideAreaHeight;/*遮挡区域的高*/
        }

        //信号丢失报警
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_VILOST_V40
        {
            public uint dwEnableVILostAlarm;                /* 是否启动信号丢失报警 ,0-否,1-是*/
                                                            /* 信号丢失触发报警输出 */
            public uint dwHandleType;        //异常处理,异常处理方式的"或"结果     
                                             /*0x00: 无响应*/
                                             /*0x01: 监视器上警告*/
                                             /*0x02: 声音警告*/
                                             /*0x04: 上传中心*/
                                             /*0x08: 触发报警输出*/
                                             /*0x10: 触发JPRG抓图并上传Email*/
                                             /*0x20: 无线声光报警器联动*/
                                             /*0x40: 联动电子地图(目前只有PCNVR支持)*/
                                             /*0x200: 抓图并上传FTP*/
            public uint dwMaxRelAlarmOutChanNum; //触发的报警输出通道数（只读）最大支持数量
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelAlarmOut; /*触发报警输出号，按值表示,采用紧凑型排列，从下标0 - dwRelAlarmOut -1有效，如果中间遇到0xffffffff,则后续无效*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime; /*布防时间*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //保留
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MOTION_SINGLE_AREA
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64 * 96, ArraySubType = UnmanagedType.I1)]
            public byte[] byMotionScope;		/*侦测区域,0-96位,表示64行,共有96*64个小宏块,目前有效的是22*18,为1表示是移动侦测区域,0-表示不是*/
            public byte byMotionSensitive;			/*移动侦测灵敏度, 0 - 5,越高越灵敏,0xff关闭*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_DAYTIME
        {
            public byte byHour;//0~24
            public byte byMinute;//0~60
            public byte bySecond;//0~60
            public byte byRes;
            public ushort wMilliSecond; //0~1000
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_SCHEDULE_DAYTIME
        {
            public NET_DVR_DAYTIME struStartTime; //开始时间
            public NET_DVR_DAYTIME struStopTime; //结束时间
        }

        //区域框结构
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_RECT
        {
            public float fX;//边界框左上角点的X轴坐标, 0.001~1
            public float fY;//边界框左上角点的Y轴坐标, 0.001~1
            public float fWidth;//边界框的宽度, 0.001~1
            public float fHeight;//边界框的高度, 0.001~1
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_DNMODE
        {
            public byte byObjectSize;//占比参数(0~100)
            public byte byMotionSensitive; /*移动侦测灵敏度, 0 - 5,越高越灵敏,0xff关闭*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MOTION_MULTI_AREAPARAM
        {
            public byte byAreaNo;//区域编号(IPC- 1~8)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_VCA_RECT struRect;//单个区域的坐标信息(矩形) size = 16;
            public NET_DVR_DNMODE struDayNightDisable;//关闭模式
            public NET_DVR_DNMODE struDayModeParam;//白天模式
            public NET_DVR_DNMODE struNightModeParam;//夜晚模式
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MOTION_MULTI_AREA
        {
            public byte byDayNightCtrl;//日夜控制 0~关闭,1~自动切换,2~定时切换(默认关闭)
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
            public NET_DVR_SCHEDULE_DAYTIME struScheduleTime;//切换时间  16
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_MULTI_AREA_NUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_MOTION_MULTI_AREAPARAM[] struMotionMultiAreaParam;//最大支持24个区域
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 60, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MOTION_MODE_PARAM
        {
            public NET_DVR_MOTION_SINGLE_AREA struMotionSingleArea; //普通模式下的单区域设
            public NET_DVR_MOTION_MULTI_AREA struMotionMultiArea; //专家模式下的多区域设置	
        }

        //移动侦测
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_MOTION_V40
        {
            public NET_DVR_MOTION_MODE_PARAM struMotionMode; //(5.1.0新增)
            public byte byEnableHandleMotion;       /* 是否处理移动侦测 0－否 1－是*/
            public byte byEnableDisplay;	/*启用移动侦测高亮显示，0-否，1-是*/
            public byte byConfigurationMode; //0~普通,1~专家(5.1.0新增)
            public byte byRes1; //保留字节
            /* 异常处理方式 */
            public uint dwHandleType;        //异常处理,异常处理方式的"或"结果  
                                             /*0x00: 无响应*/
                                             /*0x01: 监视器上警告*/
                                             /*0x02: 声音警告*/
                                             /*0x04: 上传中心*/
                                             /*0x08: 触发报警输出*/
                                             /*0x10: 触发JPRG抓图并上传Email*/
                                             /*0x20: 无线声光报警器联动*/
                                             /*0x40: 联动电子地图(目前只有PCNVR支持)*/
                                             /*0x200: 抓图并上传FTP*/
            public uint dwMaxRelAlarmOutChanNum; //触发的报警输出通道数（只读）最大支持数量
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelAlarmOut; //实际触发的报警输出号，按值表示,采用紧凑型排列，从下标0 - dwRelAlarmOut -1有效，如果中间遇到0xffffffff,则后续无效
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime; /*布防时间*/
            /*触发的录像通道*/
            public uint dwMaxRecordChanNum;   //设备支持的最大关联录像通道数-只读
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_CHANNUM_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelRecordChan;   /* 实际触发录像通道，按值表示,采用紧凑型排列，从下标0 - dwRelRecordChan -1有效，如果中间遇到0xffffffff,则后续无效*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //保留字节
        }

        //遮挡报警
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_HIDEALARM_V40
        {
            public uint dwEnableHideAlarm;  /* 是否启动遮挡报警，0-否，1-低灵敏度，2-中灵敏度，3-高灵敏度*/
            public ushort wHideAlarmAreaTopLeftX;			/* 遮挡区域的x坐标 */
            public ushort wHideAlarmAreaTopLeftY;			/* 遮挡区域的y坐标 */
            public ushort wHideAlarmAreaWidth;				/* 遮挡区域的宽 */
            public ushort wHideAlarmAreaHeight;             /*遮挡区域的高*/
                                                            /* 信号丢失触发报警输出 */
            public uint dwHandleType;        //异常处理,异常处理方式的"或"结果  
                                             /*0x00: 无响应*/
                                             /*0x01: 监视器上警告*/
                                             /*0x02: 声音警告*/
                                             /*0x04: 上传中心*/
                                             /*0x08: 触发报警输出*/
                                             /*0x10: 触发JPRG抓图并上传Email*/
                                             /*0x20: 无线声光报警器联动*/
                                             /*0x40: 联动电子地图(目前只有PCNVR支持)*/
                                             /*0x200: 抓图并上传FTP*/
            public uint dwMaxRelAlarmOutChanNum; //触发的报警输出通道数（只读）最大支持数量
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_ALARMOUT_V40, ArraySubType = UnmanagedType.U4)]
            public uint[] dwRelAlarmOut; /*触发报警输出号，按值表示,采用紧凑型排列，从下标0 - dwRelAlarmOut -1有效，如果中间遇到0xffffffff,则后续无效*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_DAYS * MAX_TIMESEGMENT_V30, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SCHEDTIME[] struAlarmTime; /*布防时间*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes; //保留
        }

        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_RGB_COLOR
        {
            public byte byRed;	 //RGB颜色三分量中的红色
            public byte byGreen; //RGB颜色三分量中的绿色
            public byte byBlue;	//RGB颜色三分量中的蓝色
            public byte byRes;	//保留
        }

        //通道图象结构(V40扩展)
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_PICCFG_V40
        {
            public uint dwSize;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NAME_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sChanName;
            public uint dwVideoFormat;	/* 只读 视频制式 1-NTSC 2-PAL  */
            public NET_DVR_VICOLOR struViColor;//	图像参数按时间段设置
                                               //显示通道名
            public uint dwShowChanName; // 预览的图象上是否显示通道名称,0-不显示,1-显示
            public ushort wShowNameTopLeftX;				/* 通道名称显示位置的x坐标 */
            public ushort wShowNameTopLeftY;                /* 通道名称显示位置的y坐标 */
                                                            //隐私遮挡
            public uint dwEnableHide;		/* 是否启动遮挡 ,0-否,1-是*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_SHELTERNUM, ArraySubType = UnmanagedType.Struct)]
            public NET_DVR_SHELTER[] struShelter;
            //OSD
            public uint dwShowOsd;// 预览的图象上是否显示OSD,0-不显示,1-显示
            public ushort wOSDTopLeftX;				/* OSD的x坐标 */
            public ushort wOSDTopLeftY;				/* OSD的y坐标 */
            public byte byOSDType;					/* OSD类型(主要是年月日格式) */
            /* 0: XXXX-XX-XX 年月日 */
            /* 1: XX-XX-XXXX 月日年 */
            /* 2: XXXX年XX月XX日 */
            /* 3: XX月XX日XXXX年 */
            /* 4: XX-XX-XXXX 日月年*/
            /* 5: XX日XX月XXXX年 */
            /*6: xx/xx/xxxx(月/日/年) */
            /*7: xxxx/xx/xx(年/月/日) */
            /*8: xx/xx/xxxx(日/月/年)*/
            public byte byDispWeek;				/* 是否显示星期 */
            public byte byOSDAttrib;                /* OSD属性:透明，闪烁 */
                                                    /* 0: 不显示OSD */
                                                    /* 1: 透明，闪烁 */
                                                    /* 2: 透明，不闪烁 */
                                                    /* 3: 不透明，闪烁 */
                                                    /* 4: 不透明，不闪烁 */
            public byte byHourOSDType;				/* OSD小时制:0-24小时制,1-12小时制 */
            public byte byFontSize;      //16*16(中)/8*16(英)，1-32*32(中)/16*32(英)，2-64*64(中)/32*64(英) FOR 91系列HD-SDI高清DVR
            public byte byOSDColorType;	 //0-默认（黑白）；1-自定义
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
            public NET_DVR_VILOST_V40 struVILost;  //视频信号丢失报警（支持组）
            public NET_DVR_VILOST_V40 struAULost;  /*音频信号丢失报警（支持组）*/
            public NET_DVR_MOTION_V40 struMotion;  //移动侦测报警（支持组）
            public NET_DVR_HIDEALARM_V40 struHideAlarm;  //遮挡报警（支持组）
            public NET_DVR_RGB_COLOR struOsdColor;//OSD颜色
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 124, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopRealPlay(int iRealHandle);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Logout(int iUserID);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Cleanup();

        [DllImport(@"HCNetSDK.dll")]
        public static extern uint NET_DVR_GetLastError();

        //参数配置 begin
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_GetDVRConfig(int lUserID, uint dwCommand, int lChannel, IntPtr lpOutBuffer, uint dwOutBufferSize, ref uint lpBytesReturned);

        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRConfig(int lUserID, uint dwCommand, int lChannel, System.IntPtr lpInBuffer, uint dwInBufferSize);

        #endregion

    }
}
