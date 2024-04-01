using System;
using System.Runtime.InteropServices;

namespace LPR
{
    class VzClientSDK
    {
        public VzClientSDK()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /**可过滤的车牌识别触发类型*/
        public const int VZ_LPRC_TRIG_ENABLE_STABLE = 0x1;     /**<允许触发稳定结果*/
        public const int VZ_LPRC_TRIG_ENABLE_VLOOP = 0x2;     /**<允许触发虚拟线圈结果*/
        public const int VZ_LPRC_TRIG_ENABLE_IO_IN1 = 0x10;    /**<允许外部IO_IN_1触发*/
        public const int VZ_LPRC_TRIG_ENABLE_IO_IN2 = 0x20;    /**<允许外部IO_IN_2触发*/
        public const int VZ_LPRC_TRIG_ENABLE_IO_IN3 = 0x40;    /**<允许外部IO_IN_3触发*/

        //车牌类型
        public const int LT_UNKNOWN = 0;   //未知车牌
        public const int LT_BLUE = 1;   //蓝牌小汽车
        public const int LT_BLACK = 2;   //黑牌小汽车
        public const int LT_YELLOW = 3;   //单排黄牌
        public const int LT_YELLOW2 = 4;   //双排黄牌（大车尾牌，农用车）
        public const int LT_POLICE = 5;   //警车车牌
        public const int LT_ARMPOL = 6;   //武警车牌
        public const int LT_INDIVI = 7;   //个性化车牌
        public const int LT_ARMY = 8;   //单排军车牌
        public const int LT_ARMY2 = 9;   //双排军车牌
        public const int LT_EMBASSY = 10;  //使馆车牌
        public const int LT_HONGKONG = 11;  //香港进出中国大陆车牌
        public const int LT_TRACTOR = 12;  //农用车牌
        public const int LT_COACH = 13;  //教练车牌
        public const int LT_MACAO = 14;  //澳门进出中国大陆车牌
        public const int LT_ARMPOL2 = 15; //双层武警车牌
        public const int LT_ARMPOL_ZONGDUI = 16;  // 武警总队车牌
        public const int LT_ARMPOL2_ZONGDUI = 17; // 双层武警总队车牌
        public const int LI_AVIATION = 18;		  //民航
        public const int LI_ENERGY = 19;       //新能源小型车
        public const int LI_NO_PLATE = 20;     //无车牌

        /**可配置的识别类型*/
        public const int VZ_LPRC_REC_BLUE = (1 << (LT_BLUE));						    /**<蓝牌车*/
        public const int VZ_LPRC_REC_YELLOW = (1 << (LT_YELLOW) | 1 << (LT_YELLOW2));	/**<黄牌车*/
        public const int VZ_LPRC_REC_BLACK = (1 << (LT_BLACK));						/**<黑牌车*/
        public const int VZ_LPRC_REC_COACH = (1 << (LT_COACH));						/**<教练车*/
        public const int VZ_LPRC_REC_POLICE = (1 << (LT_POLICE));					    /**<警车*/
        public const int VZ_LPRC_REC_AMPOL = (1 << (LT_ARMPOL));				        /**<武警车*/
        public const int VZ_LPRC_REC_ARMY = (1 << (LT_ARMY) | 1 << (LT_ARMY2));		/**<军车*/
        public const int VZ_LPRC_REC_GANGAO = (1 << (LT_HONGKONG) | 1 << (LT_MACAO));	/**<港澳进出大陆车*/
        public const int VZ_LPRC_REC_EMBASSY = (1 << (LT_EMBASSY));                      /**<使馆车*/
        public const int VZ_LPRC_REC_AVIATION = (1 << (LT_EMBASSY));	                    /**<民航*/
        public const int VZ_LPRC_REC_ENERGY = (1 << (LI_ENERGY));                       /**<新能源*/
        public const int VZ_LPRC_REC_NO_PLATE = (1 << (LI_NO_PLATE));                     /**<无车牌*/

        //触发输入的类型
        public enum VZ_InputType : uint
        {
            nWhiteList = 0, //通过
            nNotWhiteList,  //不通过
            nNoLicence,     //无车牌
            nBlackList,     //黑名单
            nExtIoctl1,     //开关量/电平输入 1
            nExtIoctl2,     //开关量/电平输入 2
            nExtIoctl3		//开关量/电平输入 3
        };

        //输出配置
        public struct VZ_LPRC_OutputConfig
        {
            public int switchout1;					//开关量输出 1
            public int switchout2;					//开关量输出 2
            public int switchout3;					//开关量输出 3
            public int switchout4;					//开关量输出 4
            public int levelout1;					//电平输出 1 
            public int levelout2;					//电平输出 2
            public int rs485out1;					//RS485-1
            public int rs485out2;                   //RS485-2
            VZ_InputType eInputType;		        //触发输入的类型
        };

        public const int MAX_OutputConfig_Len = 7;
        //输出配置信息
        public struct VZ_OutputConfigInfo
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_OutputConfig_Len, ArraySubType = UnmanagedType.I1)]
            public VZ_LPRC_OutputConfig[] oConfigInfo;	//多个输出配置输出的消息
        };

        ///**加密类型**/
        public const int ENCRYPT_NAME_LENGTH = 32;
        public struct VZ_EMS_INFO
        {
            public UInt32 uId;    //加密ID
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = ENCRYPT_NAME_LENGTH)]
            public string sName;  //加密名
        }

        public const int ENCRYPT_LENGTH = 16;
        public const int SIGNATURE_LENGTH = 32;
        /**当前识别结果加密类型**/
        public struct VZ_LPRC_ACTIVE_ENCRYPT
        {
            public UInt32 uActiveID;//当前加密类型ID
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = ENCRYPT_LENGTH)]

            public VZ_EMS_INFO[] oEncrpty;//系统加密类型

            public UInt32 uSize;//系统加密类型长度

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = SIGNATURE_LENGTH)]
            public string signature;//SIGNATURE
        }

        /**串口参数*/
        public struct VZ_SERIAL_PARAMETER
        {
            public UInt32 uBaudRate;		// <波特率 300,600,1200,2400,4800,9600,19200,34800,57600,115200
            public UInt32 uParity;		    // <校验位 其值为0-2=no,odd,even
            public UInt32 uDataBits;		// <数据位 其值为7,8 位数据位
            public UInt32 uStopBit;		    // <停止位 其值为1,2位停止位
        };

        //设置回调函数时需要制定的类型
        public enum VZ_LPRC_CALLBACK_TYPE : uint
        {
            VZ_LPRC_CALLBACK_COMMON_NOTIFY = 0,	    //SDK通用信息反馈
            VZ_LPRC_CALLBACK_PLATE_STR,		        //车牌号码字符
            VZ_LRPC_CALLBACK_FULL_IMAGE,	        //完整图像
            VZ_LPRC_CALLBACK_CLIP_IMAGE,	        //截取图像
            VZ_LPRC_CALLBACK_PLATE_RESULT,	        //实时识别结果
            VZ_LPRC_CALLBACK_PLATE_RESULT_STABLE,	//稳定识别结果
            VZ_LPRC_CALLBACK_PLATE_RESULT_TRIGGER,	//触发的识别结果，包括API（软件）和IO（硬件）方式的
            VZ_LPRC_CALLBACK_VIDEO,			        //视频帧回调
        }
        //通用信息反馈类型
        public enum VZ_LPRC_COMMON_NOTIFY : uint
        {
            VZ_LPRC_NO_ERR = 0,
            VZ_LPRC_ACCESS_DENIED,	//用户名密码错误
            VZ_LPRC_NETWORK_ERR,	//网络连接故障
        }

        //识别结果的类型
        public enum VZ_LPRC_RESULT_TYPE : uint
        {
            VZ_LPRC_RESULT_REALTIME,		/*<实时识别结果*/
            VZ_LPRC_RESULT_STABLE,			/*<稳定识别结果*/
            VZ_LPRC_RESULT_FORCE_TRIGGER,	/*<调用“VzLPRClient_ForceTrigger”触发的识别结果*/
            VZ_LPRC_RESULT_IO_TRIGGER,		/*<外部IO信号触发的识别结果*/
            VZ_LPRC_RESULT_VLOOP_TRIGGER,	/*<虚拟线圈触发的识别结果*/
            VZ_LPRC_RESULT_MULTI_TRIGGER,	/*<由_FORCE_\_IO_\_VLOOP_中的一种或多种同时触发，具体需要根据每个识别结果的TH_PlateResult::uBitsTrigType来判断*/
            VZ_LPRC_RESULT_TYPE_NUM			/*<结果种类个数*/
        }

        //顶点定义
        //X_1000和Y_1000的取值范围为[0, 1000]；
        //即位置信息为实际图像位置在整体图像位置的相对尺寸；
        //例如X_1000 = x*1000/win_width，其中x为点在图像中的水平像素位置，win_width为图像宽度
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_VERTEX
        {
            public uint X_1000;
            public uint Y_1000;
        }

        public const int VZ_LPRC_ROI_VERTEX_NUM_EX = 12;
        //识别区域信息定义
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_ROI_EX
        {
            public byte byRes1;     //预留
            public byte byEnable;	//是否有效
            public byte byDraw;		//是否绘制
            public byte byRes2;     //预留
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;	//预留
            uint uNumVertex;        //顶点实际个数
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = VZ_LPRC_ROI_VERTEX_NUM_EX)]
            public VZ_LPRC_VERTEX[] struVertex;	//顶点数组
        }

        public const int VZ_LPRC_VIRTUAL_LOOP_NAME_LEN = 32;
        public const int VZ_LPRC_VIRTUAL_LOOP_VERTEX_NUM = 4;
        //虚拟线圈信息定义
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_VIRTUAL_LOOP
        {
            public byte byID;       //序号
            public byte byEnable;   //是否有效
            public byte byDraw;		//是否绘制
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;	//预留
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = VZ_LPRC_VIRTUAL_LOOP_NAME_LEN)]
            public string strName;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = VZ_LPRC_VIRTUAL_LOOP_VERTEX_NUM)]
            public VZ_LPRC_VERTEX[] struVertex;	//顶点数组
            public uint eCrossDir;	            // 穿越方向限制
            public uint uTriggerTimeGap;	    // 对相同车牌的触发时间间隔的限制，单位为秒
            public uint uMaxLPWidth;		    // 最大车牌尺寸限制
            public uint uMinLPWidth;		    // 最小车牌尺寸限制
        }


        public const int VZ_LPRC_VIRTUAL_LOOP_MAX_NUM = 8;
        //虚拟线圈序列
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_VIRTUAL_LOOPS
        {
            public uint uNumVirtualLoop;	//实际个数
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = VZ_LPRC_VIRTUAL_LOOP_MAX_NUM)]
            public VZ_LPRC_VIRTUAL_LOOP[] struLoop;
        }

        public const int VZ_LPRC_PROVINCE_STR_LEN = 128;

        //预设省份信息
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_PROVINCE_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = VZ_LPRC_PROVINCE_STR_LEN, ArraySubType = UnmanagedType.I1)]
            public char[] strProvinces; //所有支持的省份简称构成的字符串
            public int nCurrIndex;	    //当前的预设省份的序号，在strProvinces中的，-1为未设置
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TH_RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        /**分解时间*/
        [StructLayout(LayoutKind.Sequential)]
        public struct VzBDTime
        {
            public byte bdt_sec;    /*<秒，取值范围[0,59]*/
            public byte bdt_min;    /*<分，取值范围[0,59]*/
            public byte bdt_hour;   /*<时，取值范围[0,23]*/
            public byte bdt_mday;   /*<一个月中的日期，取值范围[1,31]*/
            public byte bdt_mon;    /*<月份，取值范围[1,12]*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            public byte[] res1;     /*<预留*/
            public uint bdt_year;   /*<年份*/
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            public byte[] res2;     /*<预留*/
        }   //broken-down time

        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_TIMEVAL
        {
            public uint uTVSec;
            public uint uTVUSec;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TH_PlateResult
        {
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public char[] license;      // 车牌号码

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public char[] color;        // 车牌颜色

            public int nColor;			// 车牌颜色序号
            public int nType;			// 车牌类型
            public int nConfidence;	    // 车牌可信度
            public int nBright;		    // 亮度评价
            public int nDirection;		// 运动方向，0 unknown, 1 left, 2 right, 3 up , 4 down	
            public TH_RECT rcLocation;  //车牌位置
            public int nTime;           //识别所用时间
            public VZ_TIMEVAL tvPTS;    //识别时间点
            public uint uBitsTrigType;  //强制触发结果的类型，见TH_TRIGGER_TYPE_BIT
            public byte nCarBright;		//车的亮度
            public byte nCarColor;		//车的颜色

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public char[] reserved0;    //为了对齐

            public uint uId;            //记录的编号
            public VzBDTime struBDTime; //分解时间

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 84, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public char[] reserved;	    // 保留
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VzYUV420P
        {
            public IntPtr pY;
            public IntPtr pU;
            public IntPtr pV;
            int widthStepY;
            int widthStepU;
            int widthStepV;
            int width;
            int height;
        }

        //图像信息
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_IMAGE_INFO
        {
            public uint uWidth;
            public uint uHeight;
            public uint uPitch;
            public uint uPixFmt;
            public IntPtr pBuffer;
        }

        //智能视频        
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_LPRC_DRAWMODE
        {
            public byte byDspAddTarget;     //dsp叠加报警目标
            public byte byDspAddRule;           //dsp叠加设置规则
            public byte byDspAddTrajectory;	//dsp叠加轨迹	

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = System.Runtime.InteropServices.UnmanagedType.I1)]
            public byte[] dwRes;
        };

        //设备序列号
        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_DEV_SERIAL_NUM
        {
            public uint uHi;
            public uint uLo;
        }


        //********白名单********//

        [StructLayout(LayoutKind.Sequential)]
        public struct VZ_TM
        {

            /// short
            public short nYear;

            /// short
            public short nMonth;

            /// short
            public short nMDay;

            /// short
            public short nHour;

            /// short
            public short nMin;

            /// short
            public short nSec;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_TM_WEEK_DAY
        {

            /// char
            public byte bSun;

            /// char
            public byte bMon;

            /// char
            public byte bTue;

            /// char
            public byte bWed;

            /// char
            public byte bThur;

            /// char
            public byte bFri;

            /// char
            public byte bSat;

            /// char
            public byte reserved;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_OSD_Param
        {
            public byte dstampenable;                   // 0 off 1 on
            public int dateFormat;                      // 0:YYYY/MM/DD;1:MM/DD/YYYY;2:DD/MM/YYYY
            public int datePosX;
            public int datePosY;
            public byte tstampenable;                   // 0 off 1 on
            public int timeFormat;                      // 0:12Hrs;1:24Hrs
            public int timePosX;
            public int timePosY;
            public byte nLogoEnable;                    // 0 off 1 on
            public int nLogoPositionX;                  //<  logo position
            public int nLogoPositionY;                  //<  logo position
            public byte nTextEnable;                    //0 off 1 on
            public int nTextPositionX;                  //<  text position
            public int nTextPositionY;   				//<  text position
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 16)]
            public string overlaytext;              	//user define text           	//user define text
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_TM_DAY
        {

            /// short
            public short nHour;

            /// short
            public short nMin;

            /// short
            public short nSec;

            /// short
            public short reserved;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_TM_WEEK_SEGMENT
        {

            /// unsigned int
            public uint uEnable;

            /// VZ_TM_WEEK_DAY->Anonymous_a54d933b_d2e6_4eba_97b3_61ea9b47dd3b
            public VZ_TM_WEEK_DAY struDaySelect;

            /// VZ_TM_DAY->Anonymous_2bafa8b8_e11f_4cdc_a109_eb09791f91d6
            public VZ_TM_DAY struDayTimeStart;

            /// VZ_TM_DAY->Anonymous_2bafa8b8_e11f_4cdc_a109_eb09791f91d6
            public VZ_TM_DAY struDayTimeEnd;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_TM_RANGE
        {

            /// VZ_TM->Anonymous_40d76b6c_816a_4821_a5db_3480cee2a116
            public VZ_TM struTimeStart;

            /// VZ_TM->Anonymous_40d76b6c_816a_4821_a5db_3480cee2a116
            public VZ_TM struTimeEnd;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_TM_PERIOD_OR_RANGE
        {

            /// unsigned int
            public uint uEnable;

            /// VZ_TM_WEEK_SEGMENT[8]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public VZ_TM_WEEK_SEGMENT[] struWeekSeg;
        }

        /**设备日期时间参数*/
        public struct VZ_DATE_TIME_INFO
        {
            public uint uYear;		/*<年*/
            public uint uMonth;	    /*<月 [1, 12]*/
            public uint uMDay;		/*<月中的天数 [1, 31]*/
            public uint uHour;		/*<时*/
            public uint uMin;		/*<分*/
            public uint uSec;		/*<秒*/
        }

        public enum VZ_LPR_WLIST_ERROR
        {

            /// VZ_LPR_WLIST_ERROR_NO_ERROR -> 0
            VZ_LPR_WLIST_ERROR_NO_ERROR = 0,

            VZ_LPR_WLIST_ERROR_PLATEID_EXISTS,

            VZ_LPR_WLIST_ERROR_INSERT_CUSTOMERINFO_FAILED,

            VZ_LPR_WLIST_ERROR_INSERT_VEHICLEINFO_FAILED,

            VZ_LPR_WLIST_ERROR_UPDATE_CUSTOMERINFO_FAILED,

            VZ_LPR_WLIST_ERROR_UPDATE_VEHICLEINFO_FAILED,

            VZ_LPR_WLIST_ERROR_PLATEID_EMPTY,

            VZ_LPR_WLIST_ERROR_ROW_NOT_CHANGED,

            VZ_LPR_WLIST_ERROR_CUSTOMERINFO_NOT_CHANGED,

            VZ_LPR_WLIST_ERROR_VEHICLEINFO_NOT_CHANGED,

            VZ_LPR_WLIST_ERROR_CUSTOMER_VEHICLE_NOT_MATCH,

            VZ_LPR_WLIST_ERROR_SERVER_GONE,
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_WLIST_VEHICLE
        {

            /// unsigned int
            public uint uVehicleID;         //车辆在数据库的ID

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string strPlateID;      //车牌字符串

            /// unsigned int
            public uint uCustomerID;       //客户在数据库的ID

            /// unsigned int
            public uint bEnable;           //该记录有效标记(是否启用)

            /// unsigned int
            public uint bEnableTMEnable;   //是否开启生效时间

            /// unsigned int
            public uint bEnableTMOverdule; //是否开启过期时间

            /// VZ_TM*
            public VZ_TM struTMEnable;     //该记录生效时间

            public VZ_TM struTMOverdule;   //该记录过期时间

            /// unsigned int
            public uint bUsingTimeSeg;     //是否使用周期时间段

            /// VZ_TM_PERIOD_OR_RANGE->Anonymous_6f46bf7e_03f5_450b_84da_e56739a41561
            public VZ_TM_PERIOD_OR_RANGE struTimeSegOrRange;//周期时间段信息

            /// unsigned int
            public uint bAlarm;            //是否触发报警（黑名单记录）

            public uint iColor;            //车辆颜色

            public uint iPlateType;	       //车牌类型								
            // 车辆代码
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string strCode;         //车辆编码

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
            public string strComment;      //车辆编码
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_WLIST_CUSTOMER
        {

            /// unsigned int
            public uint uCustomerID;

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string strName;

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string strCode;

            /// char[256]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 256)]
            public string reserved;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPR_WLIST_ROW
        {

            /// VZ_LPR_WLIST_CUSTOMER*
            public System.IntPtr pCustomer;

            /// VZ_LPR_WLIST_VEHICLE*
            public System.IntPtr pVehicle;
        }

        //查找数据条件
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_WLIST_SEARCH_CONSTRAINT
        {

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string key;           //查找的字段

            /// char[128]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 128)]
            public string search_string; //查找的字符串
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_MSG_PLATE_INFO
        {

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string plate;

            /// char[128]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 260)]
            public string img_path;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_DEVICE_INFO
        {
            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string device_ip;

            /// char[64]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 64)]
            public string serial_no;
        }

        public enum VZ_LPR_WLIST_LIMIT_TYPE
        {
            VZ_LPR_WLIST_LIMIT_TYPE_ONE,   //查找一条
            VZ_LPR_WLIST_LIMIT_TYPE_ALL,   //查找所有
            VZ_LPR_WLIST_LIMIT_TYPE_RANGE, //查找一段
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPR_WLIST_RANGE_LIMIT
        {
            public int startIndex;  //查找起始位置
            public int count;       //查找条数
        }

        //查找条数限制
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPR_WLIST_LIMIT
        {
            /// VZ_LPR_WLIST_LIMIT_TYPE->Anonymous_988ed792_488c_49e0_9b97_4fef91401704
            public VZ_LPR_WLIST_LIMIT_TYPE limitType; //查找条数限制

            /// VZ_LPR_WLIST_RANGE_LIMIT*
            public System.IntPtr pRangeLimit;         //查找哪一段数据
        }

        public enum VZ_LPR_WLIST_SORT_DIRECTION
        {

            /// VZ_LPR_WLIST_SORT_DIRECTION_DESC -> 0
            VZ_LPR_WLIST_SORT_DIRECTION_DESC = 0,

            /// VZ_LPR_WLIST_SORT_DIRECTION_ASC -> 1
            VZ_LPR_WLIST_SORT_DIRECTION_ASC = 1,
        }

        //结果的排列方式
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_WLIST_SORT_TYPE
        {

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string key;                             //排序的字段

            /// VZ_LPR_WLIST_SORT_DIRECTION->Anonymous_dde74036_93c7_4601_966c_0439d47c4836
            public VZ_LPR_WLIST_SORT_DIRECTION direction; //排序的方式
        }

        //查找的方式
        public enum VZ_LPR_WLIST_SEARCH_TYPE
        {
            VZ_LPR_WLIST_SEARCH_TYPE_LIKE,  //包含字符
            VZ_LPR_WLIST_SEARCH_TYPE_EQUAL, //完全匹配
        }

        //查找条件
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPR_WLIST_SEARCH_WHERE
        {

            /// VZ_LPR_WLIST_SEARCH_TYPE->Anonymous_e3b38339_d7de_4d6d_998f_8f03f1a82e9c
            public VZ_LPR_WLIST_SEARCH_TYPE searchType;//查找的方式，如果是完全匹配，每个条件之间为与;是包含字符时，每个条件之间为或

            /// unsigned int
            public uint searchConstraintCount;         //查找条件个数，为0表示没有搜索条件

            /// VZ_LPR_WLIST_SEARCH_CONSTRAINT*
            public System.IntPtr pSearchConstraints;   //查找条件数组指针
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPR_WLIST_LOAD_CONDITIONS
        {

            /// VZ_LPR_WLIST_SEARCH_WHERE*
            public System.IntPtr pSearchWhere; //查找条件

            /// VZ_LPR_WLIST_LIMIT*
            public System.IntPtr pLimit;       //查找条数限制

            /// VZ_LPR_WLIST_SORT_TYPE*
            public System.IntPtr pSortType;    //结果的排序方式，为空表示按默认排序
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
        public struct VZ_LPR_WLIST_KEY_DEFINE
        {

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string key;

            /// char[32]
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string name;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPR_WLIST_IMPORT_RESULT
        {
            /// int
            public int ret;

            /// int
            public int error_code;
        }

        public enum VZLPRC_WLIST_CB_TYPE
        {
            /// VZLPRC_WLIST_CB_TYPE_ROW -> 0
            VZLPRC_WLIST_CB_TYPE_ROW = 0,

            VZLPRC_WLIST_CB_TYPE_CUSTOMER,

            VZLPRC_WLIST_CB_TYPE_VEHICLE,
        }

        /**LED补光灯命令*/
        public enum VZ_LED_CTRL
        {
            VZ_LED_AUTO,        /*<自动控制LED的开和关*/
            VZ_LED_MANUAL_ON,   /*<手动控制LED开启*/
            VZ_LED_MANUAL_OFF,	/*<手动控制LED关闭*/
        }

        /**************************************中心服务器***********************************************/

        //中心服务器网络
        public const int LPRC_CENTER_IPLEN = 200;
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_CENTER_SERVER_NET
        {
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = LPRC_CENTER_IPLEN)]
            public string centerServerIp;    //中心服务器地址

            public UInt16 port;              //中心服务器端口

            public Byte enableSsl;           //是否使用ssl协议

            public UInt16 sslPort;           //ssl协议端口 

            public UInt16 timeout;           //超时时间设置错误, 范围【1~30】
        }

        public const int URLLENGTH = 1000;

        //中心服务器网络设备注册
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_CENTER_SERVER_DEVICE_REG
        {
            public Byte type;                //中心服务器设备注册类型 0:取消心跳 1:普通心跳 2:comet轮询

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = URLLENGTH)]
            public string url;               //中心服务器设备注册地址
        }


        //中心服务器网络车牌
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_CENTER_SERVER_PLATE
        {
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = URLLENGTH)]
            public string url;               //中心服务器车牌地址

            public Byte enable;              //中心服务器车牌推送使能

            public Byte contentLevel;        //中心服务器车牌内容详细等级 0:全部 1:较详细 2:较简略 3:简略

            public Byte sendLargeImage;      //中心服务器车牌是否发送大图片

            public Byte sendSmallImage;      //中心服务器车牌是否发送小图片
        }

        //中心服务器网络设备端口触发
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_CENTER_SERVER_GIONIN
        {
            public Byte enable;              //中心服务器网络设备端口触发使能

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = URLLENGTH)]
            public string url;               //中心服务器网络设备端口触发地址
        }


        //中心服务器网络设备串口
        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_CENTER_SERVER_SERIAL
        {
            public Byte enable;              //中心服务器网络设备串口使能

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = URLLENGTH)]
            public string url;               //中心服务器网络设备串口地址
        }


        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_R_ENCODE_PARAM
        {
            public int default_stream;  /* 当前选择的码流 */
            public int stream_id;       /* 码流类型 */
            public int resolution;      /* 分辨率; */
            public int frame_rate;      /* [0, 25]，帧率; */
            public int encode_type;     /* h264; */
            public int rate_type;       /* 码流类型，对应码流控制; */
            public int data_rate;       /* 码流上限; */
            public int video_quality;   /* 视频质量; */
        }

        public int VZ_LPRC_MAX_RESOLUTION = 12;
        public int VZ_LPRC_MAX_RATE = 5;
        public int VZ_LPRC_MAX_VIDEO_QUALITY = 12;

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_R_RESOLUTION
        {
            public int resolution_type;			/* 码流类型 */

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string resolution_content;  //码流类型
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_R_RATE_TYPE
        {
            public int rate_type_value;

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string rate_type_content;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_R_VIDEO_QUALITY
        {
            int video_quality_type;

            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 32)]
            public string video_quality_content;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_R_ENCODE_PARAM_PROPERTY
        {
            public int encode_stream;               /* 当前选择的码流 */
            public int resolution_cur;				/* 分辨率   */
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public VZ_LPRC_R_RESOLUTION[] resolution;       /* 分辨率	*/
            public int frame_rate_cur;                      /* 当前帧率; */
            public int frame_rate_min;                      /* 最小帧率; */
            public int frame_rate_max;                      /* 最大帧率; */
            public int rate_type_cur;                       /* 码率控制 */
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public VZ_LPRC_R_RATE_TYPE[] rate_type;
            public int data_rate_cur;                       /* 码流上限; */
            public int data_rate_min;
            public int data_rate_max;
            public int video_quality_cur;                   /* 视频质量; */
            [System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 12, ArraySubType = System.Runtime.InteropServices.UnmanagedType.Struct)]
            public VZ_LPRC_R_VIDEO_QUALITY[] video_quality;
        }

        [System.Runtime.InteropServices.StructLayoutAttribute(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct VZ_LPRC_R_VIDEO_PARAM
        {
            public int brightness;
            public int contrast;
            public int saturation;
            public int sharpness;
            public int hue;
            public int exposure;
            public int max_exposure;
            public int gain;
            public int max_gain;
            public int denoise;
            public int flip;
            public int frquency;
            public int night_mode;
        }

        //API
        /**
        *  @brief 全局初始化，在所有接口调用之前调用
        *  @return 0表示成功，-1表示失败
        */


        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_Setup();

        /**
        *  @brief 全局释放
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern void VzLPRClient_Cleanup();

        public delegate void VZLPRC_COMMON_NOTIFY_CALLBACK(int handle, IntPtr pUserData,
                                                       VZ_LPRC_COMMON_NOTIFY eNotify, string pStrDetail);

        /**
        *  @brief 设置设备连接反馈结果相关的回调函数
        *  @param  [IN] func 设备连接结果和状态，通过该回调函数返回
        *  @param [IN] pUserData 回调函数中的上下文
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VZLPRClient_SetCommonNotifyCallBack(VZLPRC_COMMON_NOTIFY_CALLBACK func, IntPtr pUserData);

        /**
        *  @brief 打开一个设备
        *  @param  [IN] pStrIP 设备的IP地址
        *  @param [IN] wPort 设备的端口号
        *  @param  [IN] pStrUserName 访问设备所需用户名
        *  @param [IN] pStrPassword 访问设备所需密码
        *  @return 返回设备的操作句柄，当打开失败时，返回-1
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_Open(string pStrIP, ushort wPort, string pStrUserName, string pStrPassword);

        /**
        *  @brief 关闭一个设备
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_Close(int handle);

        /**
        *  @brief 通过IP地址关闭一个设备
        *  @param  [IN] pStrIP 设备的IP地址
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_CloseByIP(string pStrIP);

        /**
        *  @brief 获取连接状态
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param[IN/OUT] pStatus 输入获取状态的变量地址，输出内容为 1已连上，0未连上
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_IsConnected(int handle, ref byte pStatus);

        /**
        *  @brief 根据句柄获取设备的IP
        *  @param [IN]  handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] ip  相机IP
        *  @param [IN] max_count IP传入长度
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetDeviceIP(int handle, ref byte ip, int max_count);

        /**
        *  @brief 播放实时视频
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] hWnd 窗口的句柄
        *  @return 播放句柄，小于0表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_StartRealPlay(int handle, IntPtr hWnd);

        /**
        *  @brief 停止正在播放的窗口上的实时视频
        *  @param  [IN] hWnd 窗口的句柄
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_StopRealPlay(int hRealHandle);

        public delegate int VZLPRC_PLATE_INFO_CALLBACK(int handle, IntPtr pUserData,
                                                    IntPtr pResult, uint uNumPlates,
                                                    VZ_LPRC_RESULT_TYPE eResultType,
                                                    IntPtr pImgFull,
                                                    IntPtr pImgPlateClip);

        /**
        *  @brief 设置识别结果的回调函数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] func 识别结果回调函数
        *  @param  [IN] pUserData 回调函数中的上下文
        *  @param  [IN] bEnableImage 指定识别结果的回调是否需要包含截图信息：1为需要，0为不需要
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int VzLPRClient_SetPlateInfoCallBack(int handle, VZLPRC_PLATE_INFO_CALLBACK func, IntPtr pUserData, int bEnableImage);


        /**
        *  @brief  通过该回调函数获得实时图像数据
        *  @param  [IN] handle		由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pUserData	回调函数的上下文
        *  @param  [IN] pFrame		图像帧信息，详见结构体定义VzYUV420P
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_callback
        */
        public delegate void VZLPRC_VIDEO_FRAME_CALLBACK(int handle, IntPtr pUserData, ref VzYUV420P pFrame);

        /**
        *  @brief 设置实时图像数据的回调函数
        *  @param  [IN] handle		由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] func		实时图像数据函数
        *  @param  [IN] pUserData	回调函数中的上下文
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int VzLPRClient_SetVideoFrameCallBack(int handle, VZLPRC_VIDEO_FRAME_CALLBACK pFunc, IntPtr pUserData);

        /**
        *  @brief 发送软件触发信号，强制处理当前时刻的数据并输出结果
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_ForceTrigger(int handle);

        /**
        *  @brief 设置虚拟线圈
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pVirtualLoops 虚拟线圈的结构体指针
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVirtualLoop(int handle, ref VZ_LPRC_VIRTUAL_LOOPS pVirtualLoops);

        /**
        *  @brief 获取已设置的虚拟线圈
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pVirtualLoops 虚拟线圈的结构体指针
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVirtualLoop(int handle, ref VZ_LPRC_VIRTUAL_LOOPS pVirtualLoops);

        /**
        *  @brief 获取已设置的识别区域
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pROI 识别区域的结构体指针
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetRegionOfInterestEx(int handle, ref VZ_LPRC_ROI_EX pROI);

        /**
        *  @brief 获取已设置的预设省份
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pProvInfo 预设省份信息指针
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetSupportedProvinces(int handle, ref VZ_LPRC_PROVINCE_INFO pProvInfo);

        /**
        *  @brief 设置预设省份
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] nIndex 设置预设省份的序号，序号需要参考VZ_LPRC_PROVINCE_INFO::strProvinces中的顺序，从0开始，如果小于0，则表示不设置预设省份
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_PresetProvinceIndex(int handle, int nIndex);

        /**
        *  @brief 将图像保存为JPEG到指定路径
        *  @param  [IN] pImgInfo 图像结构体，目前只支持默认的格式，即ImageFormatRGB
        *  @param  [IN] pFullPathName 设带绝对路径和JPG后缀名的文件名字符串
        *  @param  [IN] nQuality JPEG压缩的质量，取值范围1~100；
        *  @return 0表示成功，-1表示失败
        *  @note   给定的文件名中的路径需要存在
        *  @ingroup group_global
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_ImageSaveToJpeg(IntPtr pImgInfo, string pFullPathName, int nQuality);


        /**
        *  @brief 读出设备序列号，可用于二次加密
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN/OUT] pSN 用于存放读到的设备序列号，详见定义 VZ_DEV_SERIAL_NUM
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetSerialNumber(int handle, ref VZ_DEV_SERIAL_NUM pSN);

        /**
        *  @brief 保存正在播放的视频的当前帧的截图到指定路径
        *  @param  [IN] nPlayHandle 播放的句柄
        *  @param  [IN] pFullPathName 设带绝对路径和JPG后缀名的文件名字符串
        *  @param  [IN] nQuality JPEG压缩的质量，取值范围1~100；
        *  @return 0表示成功，-1表示失败
        *  @note   使用的文件名中的路径需要存在
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetSnapShootToJpeg2(int nPlayHandle, string pFullPathName, int nQuality);

        /**
        *  @brief 通过该回调函数获得透明通道接收的数据
        *  @param  [IN] nSerialHandle VzLPRClient_SerialStart返回的句柄
        *  @param  [IN] pStrIPAddr	设备IP地址
        *  @param  [IN] usPort1		设备端口号
        *  @param  [IN] usPort2		预留
        *  @param  [IN] pUserData	回调函数上下文
        *  @ingroup group_global
        */
        public delegate int VZDEV_SERIAL_RECV_DATA_CALLBACK(int nSerialHandle, IntPtr pRecvData, int uRecvSize, IntPtr pUserData);

        /**
        *  @brief 开启透明通道
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] nSerialPort 指定使用设备的串口序号：0表示第一个串口，1表示第二个串口
        *  @param  [IN] func 接收数据的回调函数
        *  @param  [IN] pUserData 接收数据回调函数的上下文
        *  @return 返回透明通道句柄，0表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SerialStart(int handle, int nSerialPort, VZDEV_SERIAL_RECV_DATA_CALLBACK func, IntPtr pUserData);

        /**
        *  @brief 透明通道发送数据
        *  @param [IN] nSerialHandle 由VzLPRClient_SerialStart函数获得的句柄
        *  @param [IN] pData 将要传输的数据块的首地址
        *  @param [IN] uSizeData 将要传输的数据块的字节数
        *  @return 0表示成功，其他值表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SerialSend(int nSerialHandle, IntPtr pData, int uSizeData);

        /**
        *  @brief 透明通道停止发送数据
        *  @param [IN] nSerialHandle 由VzLPRClient_SerialStart函数获得的句柄
        *  @return 0表示成功，其他值表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SerialStop(int nSerialHandle);

        /**
        *  @brief 设置IO输出的状态
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] uChnId IO输出的通道号，从0开始
        *  @param  [OUT] nOutput 将要设置的IO输出的状态，0表示继电器开路，1表示继电器闭路
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetIOOutput(int handle, int uChnId, int nOutput);

        /**
        *  @brief 获取IO输出的状态
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] uChnId IO输出的通道号，从0开始
        *  @param  [OUT] pOutput IO输出的状态，0表示继电器开路，1表示继电器闭路
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetIOOutput(int handle, int uChnId, ref int pOutput);

        /**
        *  @brief 获取GPIO的状态
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] gpioIn 数据为0或1
        *  @param  [OUT] value 0代表短路，1代表开路
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetGPIOValue(int handle, int gpioIn, IntPtr value);

        /**
        *  @brief 根据ID获取车牌图片
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] id     车牌记录的ID
        *  @param  [IN] pdata  存储图片的内存
        *  @param  [IN][OUT] size 为传入传出值，传入为图片内存的大小，返回的是获取到jpg图片内存的大小
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_LoadImageById(int handle, int id, IntPtr pdata, IntPtr size);

        /**
        *  @brief 向白名单表导入客户和车辆记录
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] rowcount 记录的条数
        *  @param  [IN] pRowDatas 记录的内容数组的地址
        *  @param  [OUT] results 每条数据是否导入成功
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListImportRows(int handle,
                                                                  uint rowcount,
                                                                  ref VZ_LPR_WLIST_ROW pRowDatas,
                                                                  ref VZ_LPR_WLIST_IMPORT_RESULT pResults);

        /**
        *  @brief 从数据库删除车辆信息
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] strPlateID 车牌号码
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListDeleteVehicle(int handle, string strPlateID);

        /**
        *  @brief 清空数据库客户信息和车辆信息
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListClearCustomersAndVehicles(int handle);


        /**
        *  @brief 获取白名单表中所有车辆信息记录的条数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return >=0表示所有车辆信息记录的总数，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListGetVehicleCount(int handle, ref uint pCount,
                                                                 ref VZ_LPR_WLIST_SEARCH_WHERE pSearchWhere);


        /**
        *  @brief 查询白名单表车辆记录数据
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pLoadCondition 查询条件
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListLoadVehicle(int handle,
                                                            ref VZ_LPR_WLIST_LOAD_CONDITIONS pLoadCondition);

        public delegate void VZLPRC_WLIST_QUERY_CALLBACK(VZLPRC_WLIST_CB_TYPE type, IntPtr pLP,
                                                         IntPtr pCustomer,
                                                         IntPtr pUserData);
        /**
        *  @brief 设置白名单表和客户信息表的查询结果回调
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] func 查询结果回调函数
        *  @param  [IN] pUserData 回调函数中的上下文
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListSetQueryCallBack(int handle, VZLPRC_WLIST_QUERY_CALLBACK func, IntPtr pUserData);

        /**
        *  @brief 往白名单表中更新一个车辆信息
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pVehicle 将要更新的车辆信息，详见结构体定义VZ_LPR_WLIST_VEHICLE
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListUpdateVehicleByID(int handle, ref VZ_LPR_WLIST_VEHICLE pVehicle);


        /**
        *  @brief 查询白名单表客户和车辆记录条数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [OUT] pCount 记录的条数
        *  @param  [IN] search_constraints 搜索的条件
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_database
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WhiteListGetRowCount(int handle, ref int count, ref VZ_LPR_WLIST_SEARCH_WHERE pSearchWhere);

        /**
        *  @brief 设置LED控制模式
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] eCtrl 控制LED开关模式，详见定义 VZ_LED_CTRL
        *  @return 返回值为0表示成功，返回其他值表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetLEDLightControlMode(int handle, VZ_LED_CTRL eCtrl);
        /**
        *  @brief 获取LED当前亮度等级和最大亮度等级
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] pLevelNow 用于输出当前亮度等级的地址
        *  @param [OUT] pLevelMax 用于输出最高亮度等级的地址
        *  @return 0表示成功，其他值表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetLEDLightStatus(int handle, ref int pLevelNow, ref int pLevelMax);

        /**
        *  @brief 设置LED亮度等级
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] nLevel，LED亮度等级
        *  @return 0表示成功，其他值表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetLEDLightLevel(int handle, int nLevel);

        /**
        *  @brief 开始录像功能
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] sFileName 录像文件的路径
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SaveRealData(int handle, string sFileName);

        /**
        *  @brief 停止录像
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_StopSaveRealData(int handle);

        /**
        *  @brief 开启脱机功能
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pUserData 接收数据回调函数的上下文
        *   @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetOfflineCheck(int handle);

        /**
        *  @brief 关闭脱机功能
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pUserData 接收数据回调函数的上下文
        *   @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_CancelOfflineCheck(int handle);
        /**
        *  @brief 通过该回调函数获得透明通道接收的数据
        *  @param  [IN] nSerialHandle VzLPRClient_SerialStart返回的句柄
        *  @param  [IN] pRecvData	接收的数据的首地址
        *  @param  [IN] uRecvSize	接收的数据的尺寸
        *  @param  [IN] pUserData	回调函数上下文
        *  @ingroup group_callback
        */

        /**
        *  @brief 透明通道发送数据
        *  @param [IN] nSerialHandle 由VzLPRClient_SerialStart函数获得的句柄
        *  @param [IN] pData 将要传输的数据块的首地址
        *  @param [IN] uSizeData 将要传输的数据块的字节数
        *  @return 0表示成功，其他值表示失败
        *  @ingroup group_device
        */
        //[DllImport("VzLPRSDK.dll")]
        //public static extern int VzLPRClient_SerialSend(int nSerialHandle,string pData, uint uSizeData);

        /**
        *  @brief 通过该回调函数获得找到的设备基本信息
        *  @param  [IN] pStrDevName 设备名称
        *  @param  [IN] pStrIPAddr	设备IP地址
        *  @param  [IN] usPort1		设备端口号
        *  @param  [IN] usPort2		预留
        *  @param  [IN] pUserData	回调函数上下文
        *  @ingroup group_callback
        */
        public delegate void VZLPRC_FIND_DEVICE_CALLBACK_EX(string pStrDevName, string pStrIPAddr, ushort usPort1, ushort usPort2, uint SL, uint SH, string netmask, string gateway, IntPtr pUserData);

        /**
        *  @brief 开始查找设备
        *  @param  [IN] func 找到的设备通过该回调函数返回
        *  @param  [IN] pUserData 回调函数中的上下文
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_global
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VZLPRClient_StartFindDeviceEx(VZLPRC_FIND_DEVICE_CALLBACK_EX func, IntPtr pUserData);

        /**
        *  @brief 停止查找设备
        *  @ingroup group_global
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VZLPRClient_StopFindDevice();

        /**
        *  @brief 根据起始时间和车牌关键字查询记录
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pStartTime 起始时间，格式如"2015-01-02 12:20:30"
        *  @param  [IN] pEndTime   起始时间，格式如"2015-01-02 19:20:30"
        *  @param  [IN] keyword    车牌号关键字, 如"川"
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @说明   通过回调返回数据，最多返回100条数据，超过时请调用分页查询的接口
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_QueryRecordByTimeAndPlate(int handle, string pStartTime, string pEndTime, string keyword);


        /**
        *  @brief 根据时间和车牌号查询记录条数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pStartTime 起始时间，格式如"2015-01-02 12:20:30"
        *  @param  [IN] pEndTime   起始时间，格式如"2015-01-02 19:20:30"
        *  @param  [IN] keyword    车牌号关键字, 如"川"
        *  @return 返回值为0表示失败，大于0表示记录条数
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_QueryCountByTimeAndPlate(int handle, string pStartTime, string pEndTime, string keyword);

        /**
        *  @brief 根据时间和车牌号查询分页查询记录
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] pStartTime 起始时间，格式如"2015-01-02 12:20:30"
        *  @param  [IN] pEndTime   起始时间，格式如"2015-01-02 19:20:30"
        *  @param  [IN] keyword    车牌号关键字, 如"川"
        *  @param  [IN] start      起始位置大于0,小于结束位置
        *  @param  [IN] end        结束位置大于0,大于起始位置，获取记录条数不能大于100
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_QueryPageRecordByTimeAndPlate(int handle, string pStartTime, string pEndTime, string keyword, int start, int end);


        /**
        *  @brief 设置查询车牌记录的回调函数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] func 识别结果回调函数，如果为NULL，则表示关闭该回调函数的功能
        *  @param  [IN] pUserData 回调函数中的上下文
        *  @param  [IN] bEnableImage 指定识别结果的回调是否需要包含截图信息：1为需要，0为不需要
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetQueryPlateCallBack(int handle, VZLPRC_PLATE_INFO_CALLBACK func, IntPtr pUserData);

        /**
        *  @brief 获取视频OSD参数；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetOsdParam(int handle, IntPtr pParam);

        /**
        *  @brief 设置视频OSD参数；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetOsdParam(int handle, IntPtr pParam);

        /**
        *  @brief 设置设备的日期时间
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pDTInfo 将要设置的设备日期时间信息，详见定义 VZ_DATE_TIME_INFO
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetDateTime(int handle, IntPtr IntpDTInfo);

        /**
        *  @brief 读出用户私有数据，可用于二次加密
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN/OUT] pBuffer 用于存放读到的用户数据
        *  @param [IN] uSizeBuf 用户数据缓冲区的最小尺寸，不小于128字节
        *  @return 返回值为实际用户数据的字节数，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_ReadUserData(int handle, IntPtr pBuffer, uint uSizeBuf);

        /**
        *  @brief 写入用户私有数据，可用于二次加密
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pUserData 用户数据
        *  @param [IN] uSizeData 用户数据的长度，最大128字节
        *  @return 返回值为0表示成功，返回其他值表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_WriteUserData(int handle, IntPtr pUserData, uint uSizeData);

        /**
        *  @brief 将图像编码为JPEG，保存到指定内存
        *  @param  [IN] pImgInfo 图像结构体，目前只支持默认的格式，即ImageFormatRGB
        *  @param  [IN/OUT] pDstBuf JPEG数据的目的存储首地址
        *  @param  [IN] uSizeBuf JPEG数据地址的内存的最大尺寸；
        *  @param  [IN] nQuality JPEG压缩的质量，取值范围1~100；
        *  @return >0表示成功，即编码后的尺寸，-1表示失败，-2表示给定的压缩数据的内存尺寸不够大
        *  @ingroup group_global
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_ImageEncodeToJpeg(IntPtr pImgInfo, IntPtr pDstBuf, int uSizeBuf, int nQuality);

        /**
        *  @brief 设置IO输出，并自动复位
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] uChnId IO输出的通道号，从0开始
        *  @param  [IN] nDuration 延时时间，取值范围[500, 5000]毫秒
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetIOOutputAuto(int handle, int uChnId, int nDuration);

        public delegate void VZLPRC_VIDEO_FRAME_CALLBACK_EX(int handle, IntPtr pUserData, ref VZ_LPRC_IMAGE_INFO pFrame);

        /**
        *  @brief 获取实时视频帧，图像数据通过回调函数到用户层，用户可改动图像内容，并且显示到窗口
        *  @param  [IN] handle		由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] hWnd		窗口的句柄，如果为有效值，则视频图像会显示到该窗口上，如果为空，则不显示视频图像
        *  @param  [IN] func		实时图像数据函数
        *  @param  [IN] pUserData	回调函数中的上下文
        *  @return 播放的句柄，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int VzLPRClient_StartRealPlayFrameCallBack(int handle, IntPtr hWnd, VZLPRC_VIDEO_FRAME_CALLBACK_EX func, IntPtr pUserData);

        /**
        *  @brief 获取已设置的允许的车牌识别触发类型
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] pBitsTrigType 允许的车牌识别触发类型按位或的变量的地址，允许触发类型位详见定义VZ_LPRC_TRIG_ENABLE_XXX
        *  @return 返回值：返回值为0表示成功，返回其他值表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetPlateTrigType(int handle, ref int pBitsTrigType);

        /**
        *  @brief 设置允许的车牌识别触发类型
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] uBitsTrigType 允许的车牌识别触发类型按位或的值，允许触发类型位详见定义VZ_LPRC_TRIG_ENABLE_XXX
        *  @return 返回值：返回值为0表示成功，返回其他值表示失败
        *  @note  如果设置不允许某种类型的触发，那么该种类型的触发结果也不会保存在设备的SD卡中
        *  @note  默认输出稳定触发和虚拟线圈触发
        *  @note  不会影响手动触发和IO输入触发
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetPlateTrigType(int handle, UInt32 uBitsTrigType);

        /**
        *  @brief 获取智能视频显示模式
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] pDrawMode 显示模式，参考VZ_LPRC_DRAWMODE
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetDrawMode(int handle, ref VZ_LPRC_DRAWMODE pDrawMode);

        /**
        *  @brief 设置智能视频显示模式
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pDrawMode 显示模式，参考VZ_LPRC_DRAWMODE
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetDrawMode(int handle, ref VZ_LPRC_DRAWMODE pDrawMode);

        /**
        *  @brief 获取已设置的需要识别的车牌类型位
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] pBitsRecType 需要识别的车牌类型按位或的变量的地址，车牌类型位详见定义VZ_LPRC_REC_XXX
        *  @return 返回值：返回值为0表示成功，返回其他值表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetPlateRecType(int handle, ref int pBitsRecType);

        /**
        *  @brief 设置需要识别的车牌类型
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] uBitsRecType 需要识别的车牌类型按位或的值，车牌类型位详见定义VZ_LPRC_REC_XXX
        *  @return 返回值：返回值为0表示成功，返回其他值表示失败
        *  @note  在需要识别特定车牌时，调用该接口来设置，将不同类型的车牌位定义取或，得到的结果作为参数传入；
        *  @note  在不必要的情况下，使用最少的车牌识别类型，将最大限度提高识别率；
        *  @note  默认识别蓝牌和黄牌；
        *  @note  例如，需要识别蓝牌、黄牌、警牌，那么输入参数uBitsRecType = VZ_LPRC_REC_BLUE|VZ_LPRC_REC_YELLOW|VZ_LPRC_REC_POLICE
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetPlateRecType(int handle, UInt32 uBitsRecType);

        /**
        *  @brief 获取输出配置0
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pOutputConfig 输出配置
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetOutputConfig(int handle, ref VZ_OutputConfigInfo pOutputConfigInfo);

        /**
        *  @brief 设置输出配置
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pOutputConfig 输出配置
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetOutputConfig(int handle, ref VZ_OutputConfigInfo pOutputConfigInfo);

        /**
        *  @brief 设置车牌识别触发延迟时间
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] nDelay 触发延迟时间,时间范围[0, 10000)
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetTriggerDelay(int handle, int nDelay);

        /**
        *  @brief 获取车牌识别触发延迟时间
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [OUT] nDelay 触发延迟时间,时间范围[0, 10000)
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetTriggerDelay(int handle, ref int nDelay);

        /**
        *  @brief 设置白名单验证模式
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] nType 0 脱机自动启用;1 启用;2 不启用
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetWLCheckMethod(int handle, int nType);

        /**
        *  @brief 获取白名单验证模式
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT]	nType 0 脱机自动启用;1 启用;2 不启用
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetWLCheckMethod(int handle, ref int nType);

        /**
        *  @brief 设置白名单模糊匹配
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] nFuzzyType 0  精确匹配;1 相似字符匹配;2 普通字符模糊匹配
        *  @param [IN] nFuzzyLen  允许误识别长度
        *  @param [IN] nFuzzyType 忽略汉字
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetWLFuzzy(int handle, int nFuzzyType, int nFuzzyLen, bool bFuzzyCC);

        /**
        *  @brief 获取白名单模糊匹配
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] nFuzzyType 0  精确匹配;1 相似字符匹配;2 普通字符模糊匹配
        *  @param [IN] nFuzzyLen  允许误识别长度
        *  @param [IN] nFuzzyType 忽略汉字
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetWLFuzzy(int handle, ref int nFuzzyType, ref int nFuzzyLen, ref bool bFuzzyCC);

        /**
        *  @brief 设置串口参数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] nSerialPort 指定使用设备的串口序号：0表示第一个串口，1表示第二个串口
        *  @param  [IN] pParameter 将要设置的串口参数，详见定义 VZ_SERIAL_PARAMETER
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetSerialParameter(int handle, int nSerialPort,
                                                         ref VZ_SERIAL_PARAMETER pParameter);

        /**
        *  @brief 获取串口参数
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] nSerialPort 指定使用设备的串口序号：0表示第一个串口，1表示第二个串口
        *  @param  [OUT] pParameter 将要获取的串口参数，详见定义 VZ_SERIAL_PARAMETER
        *  @return 0表示成功，-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetSerialParameter(int handle, int nSerialPort,
                                                         ref VZ_SERIAL_PARAMETER pParameter);
        //        /**
        //        *  @brief 保存正在播放的视频的当前帧的截图到指定路径
        //        *  @param  [IN] nPlayHandle 播放的句柄
        //        *  @param  [IN] pFullPathName 设带绝对路径和JPG后缀名的文件名字符串
        //        *  @param  [IN] nQuality JPEG压缩的质量，取值范围1~100；
        //        *  @return 0表示成功，-1表示失败
        //        *  @note   使用的文件名中的路径需要存在
        //        *  @ingroup group_device
        //*/
        //        [DllImport("VzLPRSDK.dll")]
        //        public static extern int VzLPRClient_GetSnapShootToJpeg2(int nPlayHandle, string pFullPathName, int nQuality);
        /**
        *  @brief 获取主码流分辨率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] sizeval 详见VZDEV_FRAMESIZE_宏定义
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoFrameSizeIndex(int handle, ref int sizeval);

        /**
        *  @brief 设置主码流分辨率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] sizeval 详见VZDEV_FRAMESIZE_宏定义
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoFrameSizeIndex(int handle, int sizeval);

        /**
        *  @brief 获取主码流分辨率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] sizeval 
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoFrameSizeIndexEx(int handle, ref int sizeval);

        /**
        *  @brief 设置主码流分辨率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] sizeval 
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoFrameSizeIndexEx(int handle, int sizeval);

        /**
        *  @brief 获取主码流帧率
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] Rateval 帧率，范围1-25
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoFrameRate(int handle, ref int Rateval);//1-25

        /**
        *  @brief 设置主码流帧率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] Rateval 帧率，范围1-25
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoFrameRate(int handle, int Rateval);//1-25

        /**
        *  @brief 获取主码流压缩模式；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] modeval 详见VZDEV_VIDEO_COMPRESS_宏定义
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoCompressMode(int handle, ref int modeval);//VZDEV_VIDEO_COMPRESS_XXX

        /**
        *  @brief 设置主码流压缩模式；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] modeval 详见VZDEV_VIDEO_COMPRESS_宏定义
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoCompressMode(int handle, int modeval);//VZDEV_VIDEO_COMPRESS_XXX


        /**
        *  @brief 获取主码流比特率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] rateval 当前视频比特率
        *  @param [OUT] ratelist 暂时不用
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoCBR(int handle, ref int rateval/*Kbps*/, ref int ratelist);

        /**
        *  @brief 设置主码流比特率；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] rateval 当前视频比特率
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoCBR(int handle, int rateval/*Kbps*/);


        /**
        *  @brief 获取视频参数；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] brt 亮度
        *  @param [OUT] cst 对比度
        *  @param [OUT] sat 饱和度
        *  @param [OUT] hue 色度
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoPara(int handle, ref int brt, ref int cst, ref int sat, ref int hue);

        /**
        *  @brief 设置视频参数；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] brt 亮度
        *  @param [IN] cst 对比度
        *  @param [IN] sat 饱和度
        *  @param [IN] hue 色度
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoPara(int handle, int brt, int cst, int sat, int hue);

        /**
        *  @brief 设置通道主码流编码方式
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] cmd    返回的编码方式, 0->H264  1->MPEG4  2->JPEG  其他->错误
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoEncodeType(int handle, int cmd);

        /**
        *  @brief 获取视频的编码方式
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [OUT] pEncType	返回的编码方式, 0:H264  1:MPEG4  2:JPEG  其他:错误
        *  @return 返回值为0表示成功，返回-1表示失败
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoEncodeType(int handle, ref int pEncType);

        /**
        *  @brief 获取视频图像质量；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] levelval //0~6，6最好
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetVideoVBR(int handle, ref int levelval);

        /**
        *  @brief 设置视频图像质量；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] levelval //0~6，6最好
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetVideoVBR(int handle, int levelval);


        /**
        *  @brief 获取视频制式；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] frequency 0:MaxOrZero, 1: 50Hz, 2:60Hz
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetFrequency(int handle, ref int frequency);

        /**
        *  @brief 设置视频制式；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] frequency 0:MaxOrZero, 1: 50Hz, 2:60Hz
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetFrequency(int handle, int frequency);

        /**
        *  @brief 获取曝光时间；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] shutter 2:>0~8ms 停车场推荐, 3: 0~4ms, 4:0~2ms 卡口推荐
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetShutter(int handle, ref int shutter);

        /**
        *  @brief 设置曝光时间；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] shutter 2:>0~8ms 停车场推荐, 3: 0~4ms, 4:0~2ms 卡口推荐
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetShutter(int handle, int shutter);

        /**
        *  @brief 获取图像翻转；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] flip, 0: 原始图像, 1:上下翻转, 2:左右翻转, 3:中心翻转
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetFlip(int handle, ref int flip);

        /**
        *  @brief 设置图像翻转；
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] flip, 0: 原始图像, 1:上下翻转, 2:左右翻转, 3:中心翻转
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetFlip(int handle, int flip);

        /**
        *  @brief 修改网络参数
        *  @param  [IN] SL        设备序列号低位字节
        *  @param  [IN] SH		  设备序列号高位字节	
        *  @param [IN] strNewIP   新IP     格式如"192.168.3.109"
        *  @param [IN] strGateway 网关     格式如"192.168.3.1"
        *  @param [IN] strNetmask 子网掩码 格式如"255.255.255.0"
        *  @note 可以用来实现跨网段修改IP的功能
        *  @ingroup group_global
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_UpdateNetworkParam(uint SL, uint SH, string strNewIP, string strGateway, string strNetmask);


        /**
        *  @brief 获取设备序列号；
        *  @param [IN] ip ip统一使用字符串的形式传入
        *  @param [IN] port 使用和登录时相同的端口
        *  @param [OUT] SerHi 序列号高位
        *  @param [OUT] SerLo 序列号低位
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetSerialNo(string ip, short port, ref int SerHi, ref int SerLo);


        /**
        *  @brief 开始实时图像数据流，用于实时获取图像数据
        *  @param  [IN] handle		由VzLPRClient_Open函数获得的句柄
        *  @return 返回值为0表示成功，返回其他值表示失败。
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_StartRealPlayDecData(int handle);

        /**
        *  @brief 停止实时图像数据流
        *  @param  [IN] handle		由VzLPRClient_Open函数获得的句柄
        *  @return 返回值为0表示成功，返回其他值表示失败。
        *  @ingroup group_device
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_StopRealPlayDecData(int handle);

        /**
        *  @brief 从解码流中获取JPEG图像，保存到指定内存
        *  @param  [IN] handle		由VzLPRClient_Open函数获得的句柄
        *  @param  [IN/OUT] pDstBuf JPEG数据的目的存储首地址
        *  @param  [IN] uSizeBuf JPEG数据地址的内存的最大尺寸；
        *  @param  [IN] nQuality JPEG压缩的质量，取值范围1~100；
        *  @return >0表示成功，即编码后的尺寸，-1表示失败，-2表示给定的压缩数据的内存尺寸不够大
        *  @ingroup group_global
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetJpegStreamFromRealPlayDec(int handle, IntPtr pDstBuf, uint uSizeBuf, int nQuality);


        /**
        *  @brief 设置是否输出实时结果
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param  [IN] bOutput 是否输出
        *  @return 0表示成功，-1表示失败
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetIsOutputRealTimeResult(int handle, bool bOutput);

        /**
        *  @brief 获取设备加密类型和当前加密类型
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pData 加密信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetEMS(int handle, ref VZ_LPRC_ACTIVE_ENCRYPT pData);
        /**
        *  @brief 设置设备加密类型
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCurrentKey 当前识别密码
        *  @param [IN] nEncyptId	修改的加密类型ID 
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetEncrypt(int handle, IntPtr pCurrentKey, UInt32 nEncyptId);

        /**
        *  @brief 修改设备识别密码
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCurrentKey 当前识别密码
        *  @param [IN] pNewKey	新识别密码
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_ChangeEncryptKey(int handle, IntPtr pCurrentKey, IntPtr pNewKey);

        /**
        *  @brief 重置设备识别密码
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pPrimeKey 当前设备主密码
        *  @param [IN] pNewKey	新识别密码
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_ResetEncryptKey(int handle, IntPtr pPrimeKey, IntPtr pNewKey);


        /**
        *  @brief 语音播放功能
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] voice 播放的语音文字
        *  @param [IN] interval 语音文件的播放间隔(0-5000)
        *  @param [IN] volume 声音大小(0-100)
        *  @param [IN] male 声音类型(男声0，女生1)
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_PlayVoice(int handle, string voice, int interval, int volume, int male);

        //**************************************************************
        // 中心服务器配置
        /**
        *  @brief 设置中心服务器网络
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerNet  中心服务器信息结构
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetCenterServerNet(int handle, ref VZ_LPRC_CENTER_SERVER_NET pCenterServerNet);

        /**
        *  @brief 获取中心服务器网络
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerNet  中心服务器信息结构
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetCenterServerNet(int handle, ref VZ_LPRC_CENTER_SERVER_NET pCenterServerNet);

        /**
        *  @brief 设置中心服务器设备注册
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerDeviceReg  中心服务器注册结构
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetCenterServerDeviceReg(int handle, ref VZ_LPRC_CENTER_SERVER_DEVICE_REG pCenterServerDeviceReg);

        /**
        *  @brief 获取中心服务器设备注册
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerDeviceReg  中心服务器注册结构
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetCenterServerDeviceReg(int handle, ref VZ_LPRC_CENTER_SERVER_DEVICE_REG pCenterServerDeviceReg);

        /**
        *  @brief 设置中心服务器网络车牌推送信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerPlate  中心服务器车牌推送信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetCenterServerPlate(int handle, ref VZ_LPRC_CENTER_SERVER_PLATE pCenterServerPlate);

        /**
        *  @brief 获取中心服务器网络车牌推送信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerPlate  中心服务器车牌推送信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetCenterServerPlate(int handle, ref VZ_LPRC_CENTER_SERVER_PLATE pCenterServerPlate);

        /**
        *  @brief 设置中心服务器网络
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerNet  中心服务器信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetCenterServerGionin(int handle, ref VZ_LPRC_CENTER_SERVER_GIONIN pCenterServerGionin);

        /**
        *  @brief 获取中心服务器网络端口触发信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerGionin  中心服务器端口触发信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetCenterServerGionin(int handle, ref VZ_LPRC_CENTER_SERVER_GIONIN pCenterServerGionin);

        /**
        *  @brief 设置中心服务器网络串口信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerSerial  中心服务器串口信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetCenterServerSerial(int handle, ref VZ_LPRC_CENTER_SERVER_SERIAL pCenterServerSerial);

        /**
        *  @brief 获取中心服务器网络串口信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerSerial  中心服务器串口信息
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetCenterServerSerial(int handle, ref VZ_LPRC_CENTER_SERVER_SERIAL pCenterServerSerial);

        /**
        *  @brief 设置中心服务器网络主机备份信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerHostBak  中心服务器主机备份信息  例如:"192.168.3.5;192.168.3.6"
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetCenterServerHostBak(int handle, string pCenterServerHostBak);

        /**
        *  @brief 获取中心服务器网络主机备份信息
        *  @param [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] pCenterServerHostBak  中心服务器主机备份信息  例如:"192.168.3.5;192.168.3.6"
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetCenterServerHostBak(int handle, ref string pCenterServerHostBak);


        /**
        *  @brief 获取设备硬件信息
        *  @param [IN]  handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] board_version  设备类型
        *  @param [OUT] exdataSize 额外数据长度。
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetHwBoardVersion(int handle, ref int board_version, ref Int64 exdataSize);

        /**
        *  @brief 获取设备硬件类型
        *  @param [IN]  handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] board_type  设备类型(0:3730,1:6446,2:8127)
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetHwBoardType(int handle, ref int board_type);


        /**
        *  @brief 获取定焦版本相机安装距离
        *  @param [IN] iUserID VZC_Login函数返回的用户ID
        *  @param [OUT] reco_dis安装距离 0:2-4米, 2: 4-6米, 1: 6-8米
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetAlgResultParam(int handle, ref int reco_dis);


        /**
        *  @brief 获取定焦版本相机安装距离
        *  @param [IN] iUserID VZC_Login函数返回的用户ID
        *  @param [OUT] reco_dis安装距离 0:2-4米, 2: 4-6米, 1: 6-8米
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetAlgResultParam(int handle, int reco_dis);


        /**
        *  @brief 获取图像增强配置
        *  @param [IN]  handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] mode  设备类型
        *  @param [OUT] strength 额外数据长度。
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_GetDenoise(int handle, ref int mode, ref int strength);

        /**
        *  @brief 设置图像增强配置
        *  @param [IN]  handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] mode  设备类型
        *  @param [OUT] strength 额外数据长度。
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_SetDenoise(int handle, int mode, int strength);


        /**
        *  @brief 获取R相机的编码参数；
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] iChannel 通道号
        *  @param [IN] stream 0主码流 1子码流 
        *  @param [OUT] param 编码参数
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_RGet_Encode_Param(int handle, int stream, ref VZ_LPRC_R_ENCODE_PARAM param);

        /**
        *  @brief 设置R相机的编码参数；
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] iChannel 通道号
        *  @param [IN] stream 0主码流 1子码流
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_RSet_Encode_Param(int handle, int stream, ref VZ_LPRC_R_ENCODE_PARAM param);

        /**
        *  @brief 获取R相机支持的编码参数；
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] iChannel 通道号
        *  @param [IN] stream 0主码流 1子码流
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_RGet_Encode_Param_Property(int handle, ref VZ_LPRC_R_ENCODE_PARAM_PROPERTY param);


        /**
        *  @brief 获取R相机的视频参数；
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [OUT] param 视频参数
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_RGet_Video_Param(int handle, ref VZ_LPRC_R_VIDEO_PARAM param);

        /**
        *  @brief 获取R相机的视频参数；
        *  @param  [IN] handle 由VzLPRClient_Open函数获得的句柄
        *  @param [IN] iChannel 通道号
        *  @return 返回值为0表示成功，返回其他值表示失败。
        */
        [DllImport("VzLPRSDK.dll")]
        public static extern int VzLPRClient_RSet_Video_Param(int handle, ref VZ_LPRC_R_VIDEO_PARAM param);
    }
}
