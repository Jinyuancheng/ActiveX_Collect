using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BethVideo
{
    /// <summary>
    /// 存储海康摄像机登录信息
    /// </summary>
    public class CLoginInfo
    {
        public string   sIp;     //ip
        public string   sPort;   //端口
        public string   sUser;   //登录用户名
        public string   sPass;   //登录密码
        public int      iHandle; //对应的登录成功的句柄
    }
    /// <summary>
    /// 模拟量上传的结构体（得到实时模拟量数据的结构体）
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_SENSOR_ALARM
    {
        public uint dwSize;
        public uint dwAbsTime;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] byName;
        public byte bySensorChannel;
        public byte byType;
        public byte byAlarmType;
        public byte byAlarmMode;
        public float fValue;
        public float fOriginalValue;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes2;
    }
    /// <summary>
    /// 时间参数结构体
    /// </summary>
    public struct NET_DVR_TIME
    {
        public uint dwYear;
        public uint dwMonth;
        public uint dwDay;
        public uint dwHour;
        public uint dwMinute;
        public uint dwSecond;
    }
    /// <summary>
    /// 门禁主机事件信息
    /// </summary>
    public struct NET_DVR_ACS_EVENT_INFO
    {
        public uint dwSize;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] byCardNo;
        public byte byCardType;
        public byte byWhiteListNo;
        public byte byReportChannel;
        public byte byCardReaderKind;
        public uint dwCardReaderNo;
        public uint dwDoorNo;
        public uint dwVerifyNo;
        public uint dwAlarmInNo;
        public uint dwAlarmOutNo;
        public uint dwCaseSensorNo;
        public uint dwRs485No;
        public uint dwMultiCardGroupNo;
        public int wAccessChannel;
        public byte byDeviceNo;
        public byte byDistractControlNo;
        public uint dwEmployeeNo;
        public int wLocalControllerID;
        public byte byInternetAccess;
        public byte byType;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
        public byte[] byMACAddr;
        public byte bySwipeCardType;
        public byte byRes2;
        public uint dwSerialNo;
        public byte byChannelControllerID;
        public byte byChannelControllerLampID;
        public byte byChannelControllerIRAdaptorID;
        public byte byChannelControllerIREmitterID;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
    /// <summary>
    /// ip地址结构体
    /// </summary>
    public struct NET_DVR_IPADDR
    {
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
        public Char[] sIpV4;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]
        public byte[] sIpV6;
    }
    /// <summary>
    /// 门禁报警信息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_ACS_ALARM_INFO
    {
        public uint dwSize;
        public uint dwMajor;
        public uint dwMinor;
        public NET_DVR_TIME struTime;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
        public byte[] sNetUser;
        public NET_DVR_IPADDR struRemoteHostAddr;
        public NET_DVR_ACS_EVENT_INFO struAcsEventInfo;
        public uint dwPicDataLen;
        public IntPtr pPicData;
        public int wInductiveEventType;
        public byte byPicTransType;
        public byte byRes1;
        public uint dwIOTChannelNo;
        public string pAcsEventInfoExtend;
        public byte byAcsEventInfoExtend;
        public byte byTimeType;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
    /// <summary>
    /// 时间参数结构体
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_TIME_EX
    {
        public int wYear;
        public byte byMonth;
        public byte byDay;
        public byte byHour;
        public byte byMinute;
        public byte bySecond;
        public byte byRes;
    }
    /// <summary>
    /// 开关量报警信息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_SWITCH_ALARM
    {
        public uint dwSize;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] byName;
        public int wSwitchChannel;
        public byte byAlarmType;
        public byte byEventType;
        public uint dwRelativeTime;
        public NET_DVR_TIME_EX struOperateTime;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 28, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
    /// <summary>
    /// 布防结构体
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct LPNET_DVR_SETUPALARM_PARAM
    {
        public uint dwSize;
        public byte byLevel;
        public byte byAlarmInfoType;
        public byte byRetAlarmTypeV40;
        public byte byRetDevInfoVersion;
        public byte byRetVQDAlarmType;
        public byte byFaceAlarmDetection;
        public byte bySupport;
        public byte byBrokenNetHttp;
        public int wTaskNo;
        public byte byDeployType;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes1;
        public byte byAlarmTypeURL;
        public byte byCustomCtrl;
    }
    /// <summary>
    /// 热成像相机位置信息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_PTZ_INFO
    {
        public float fPan;
        public float fTilt;
        public float fZoom;
        public uint dwFocus;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }

    /// <summary>
    /// 温度报警
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_THERMOMETRY_ALARM
    {
        public uint dwSize;
        public uint dwChannel;
        public byte byRuleID;
        public byte byThermometryUnit;
        public int wPresetNo;
        public NET_PTZ_INFO struPtzInfo;
        public byte byAlarmLevel;
        public byte byAlarmType;
        public byte byAlarmRule;
        public byte byRuleCalibType;
        public NET_VCA_POINT struPoint;
        public NET_VCA_POLYGON struRegion;
        public float fRuleTemperature;
        public float fCurrTemperature;
        public uint dwPicLen;
        public uint dwThermalPicLen;
        public uint dwThermalInfoLen;
        public IntPtr pPicBuff;
        public IntPtr pThermalPicBuff;
        public IntPtr pThermalInfoBuff;
        public NET_VCA_POINT struHighestPoint;
        public char fToleranceTemperature;
        public char dwAlertFilteringTime;
        public char dwAlarmFilteringTime;
        public uint dwTemperatureSuddenChangeCycle;
        public uint fTemperatureSuddenChangeValue;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 40, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
    /// <summary>
    /// 温差报警
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_THERMOMETRY_DIFF_ALARM
    {
        public uint dwSize;
        public uint dwChannel;
        public byte byAlarmID1;
        public byte byAlarmID2;
        public int wPresetNo;
        public byte byAlarmLevel;
        public byte byAlarmType;
        public byte byAlarmRule;
        public byte byRuleCalibType;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
        public NET_VCA_POINT[] struPoint;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
        public NET_VCA_POLYGON[] struRegion;
        public float fRuleTemperatureDiff;
        public float fCurTemperatureDiff;
        public NET_PTZ_INFO struPtzInfo;
        public uint dwPicLen;
        public uint dwThermalPicLen;
        public uint dwThermalInfoLen;
        public IntPtr pPicBuff;
        public IntPtr pThermalPicBuff;
        public IntPtr pThermalInfoBuff;
        public byte byThermometryUnit;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes1;
        public float fToleranceTemperature;
        public uint dwAlarmFilteringTime;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 52, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
    /// <summary>
    /// 视频移动报警
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_ALARMINFO_V30
    {
        public uint dwAlarmType;
        public uint dwAlarmInputNumber;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
        public byte[] byAlarmOutputNumber;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
        public byte[] byAlarmRelateChannel;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
        public byte[] byChannel;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 33, ArraySubType = UnmanagedType.I1)]
        public byte[] byDiskNumber;
    }
    /// <summary>
    /// 前端设备信息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_VCA_DEV_INFO
    {
        public NET_DVR_IPADDR struDevIP;
        public int wPort;
        public byte byChannel;
        public byte byIvmsChannel;
    }
    /// <summary>
    /// 区域框
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_VCA_RECT
    {
        public float fX;
        public float fY;
        public float fWidth;
        public float fHeight;
    }

    /// <summary>
    /// 热成像报警
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_FIREDETECTION_ALARM
    {
        public uint dwSize;
        public uint dwRelativeTime;
        public uint dwAbsTime;
        public NET_VCA_DEV_INFO struDevInfo;
        public int wPanPos;
        public int wTiltPos;
        public int wZoomPos;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes1;
        public uint dwPicDataLen;
        public IntPtr pBuffer;
        public NET_VCA_RECT struRect;
        public NET_VCA_POINT struPoint;
        public int wFireMaxTemperature;
        public int wTargetDistance;
        public byte byStrategyType;
        public byte byAlarmSubType;
        public byte byPTZPosExEnable;
        public byte byRes2;
        public NET_PTZ_INFO struPtzPosEx;
        public uint dwVisiblePicLen;
        public IntPtr pVisiblePicBuf;
        public IntPtr pSmokeBuf;
        public int wDevInfoIvmsChannelEx;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 58, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
}
