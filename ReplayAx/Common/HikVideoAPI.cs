using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using IcomClient.Common;
using IcomClient.API;
using System.Windows;

namespace BethVideo
{
    #region 定义委托类型结构体
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct NET_DVR_DEVICEINFO_V30
    {
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
        public byte[] sSerialNumber;    //serial number
        public byte byAlarmInPortNum;   //Number of Alarm input
        public byte byAlarmOutPortNum;  //Number of Alarm Output
        public byte byDiskNum;  //Number of Hard Disk
        public byte byDVRType;  //DVR Type, 1: DVR 2: ATM DVR 3: DVS ......
        public byte byChanNum;  //Number of Analog Channel
        public byte byStartChan;    //The first Channel No. E.g. DVS- 1, DVR- 1
        public byte byAudioChanNum; //Number of Audio Channel
        public byte byIPChanNum;    //Maximum number of IP Channel  low
        public byte byZeroChanNum;  //Zero channel encoding number//2010- 01- 16
        public byte byMainProto;    //Main stream transmission protocol 0- private,  1- rtsp,2-both private and rtsp
        public byte bySubProto; //Sub stream transmission protocol 0- private,  1- rtsp,2-both private and rtsp
        public byte bySupport;  //Ability, the 'AND' result by bit: 0- not support;  1- support
        //bySupport & 0x1,  smart search
        //bySupport & 0x2,  backup
        //bySupport & 0x4,  get compression configuration ability
        //bySupport & 0x8,  multi network adapter
        //bySupport & 0x10, support remote SADP
        //bySupport & 0x20  support Raid card
        //bySupport & 0x40 support IPSAN directory search
        public byte bySupport1; //Ability expand, the 'AND' result by bit: 0- not support;  1- support
        //bySupport1 & 0x1, support snmp v30
        //bySupport1& 0x2,support distinguish download and playback
        //bySupport1 & 0x4, support deployment level
        //bySupport1 & 0x8, support vca alarm time extension 
        //bySupport1 & 0x10, support muti disks(more than 33)
        //bySupport1 & 0x20, support rtsp over http
        //bySupport1 & 0x40, support delay preview
        //bySuppory1 & 0x80 support NET_DVR_IPPARACFG_V40, in addition  support  License plate of the new alarm information
        public byte bySupport2; //Ability expand, the 'AND' result by bit: 0- not support;  1- support
        //bySupport & 0x1, decoder support get stream by URL
        //bySupport2 & 0x2,  support FTPV40
        //bySupport2 & 0x4,  support ANR
        //bySupport2 & 0x20, support get single item of device status
        //bySupport2 & 0x40,  support stream encryt
        public ushort wDevType; //device type
        public byte bySupport3; //Support  epresent by bit, 0 - not support 1 - support 
        //bySupport3 & 0x1-muti stream support 
        //bySupport3 & 0x8  support use delay preview parameter when delay preview
        //bySupport3 & 0x10 support the interface of getting alarmhost main status V40
        public byte byMultiStreamProto; //support multi stream, represent by bit, 0-not support ;1- support; bit1-stream 3 ;bit2-stream 4, bit7-main stream, bit8-sub stream
        public byte byStartDChan;   //Start digital channel
        public byte byStartDTalkChan;   //Start digital talk channel
        public byte byHighDChanNum; //Digital channel number high
        public byte bySupport4; //Support  epresent by bit, 0 - not support 1 - support
        //bySupport4 & 0x4 whether support video wall unified interface
        // bySupport4 & 0x80 Support device upload center alarm enable
        public byte byLanguageType; //support language type by bit,0-support,1-not support  
        //byLanguageType 0 -old device
        //byLanguageType & 0x1 support chinese
        //byLanguageType & 0x2 support english
        public byte byVoiceInChanNum;   //voice in chan num
        public byte byStartVoiceInChanNo;   //start voice in chan num
        public byte bySupport5;  //0-no support,1-support,bit0-muti stream
        //bySupport5 &0x01support wEventTypeEx 
        //bySupport5 &0x04support sence expend
        public byte bySupport6;
        public byte byMirrorChanNum;    //mirror channel num,<it represents direct channel in the recording host
        public ushort wStartMirrorChanNo;   //start mirror chan
        public byte bySupport7;        //Support  epresent by bit, 0 - not support 1 - support 
        //bySupport7 & 0x1- supports INTER_VCA_RULECFG_V42 extension    
        // bySupport7 & 0x2  Supports HVT IPC mode expansion
        // bySupport7 & 0x04  Back lock time
        // bySupport7 & 0x08  Set the pan PTZ position, whether to support the band channel
        // bySupport7 & 0x10  Support for dual system upgrade backup
        // bySupport7 & 0x20  Support OSD character overlay V50
        // bySupport7 & 0x40  Support master slave tracking (slave camera)
        // bySupport7 & 0x80  Support message encryption 
        public byte byRes2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NET_DVR_DEVICEINFO_V40
    {
        public NET_DVR_DEVICEINFO_V30 struDeviceV30;
        public byte bySupportLock;        //the device support lock function,this byte assigned by SDK.when bySupportLock is 1,dwSurplusLockTime and byRetryLoginTime is valid 
        public byte byRetryLoginTime;        //retry login times
        public byte byPasswordLevel;      //PasswordLevel,0-invalid,1-default password,2-valid password,3-risk password       
        public byte byProxyType;  //Proxy Type,0-not use proxy, 1-use socks5 proxy, 2-use EHome proxy
        public uint dwSurplusLockTime;    //surplus locked time
        public byte byCharEncodeType;     //character encode type
        public byte byRes1;
        public byte bySupport;  //能力集扩展，位与结果：0- 不支持，1- 支持
        // bySupport & 0x1:  0-保留
        // bySupport & 0x2:  0-不支持变化上报 1-支持变化上报
        public byte byRes;
        public uint dwOEMCode;
        public byte bySupportDev5;//Support v50 version of the device parameters, device name and device type name length is extended to 64 bytes 
        public byte byLoginMode; //登录模式 0-Private登录 1-ISAPI登录
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 246)]
        public byte[] byRes2;
    }
    public delegate void LoginResultCallBack(int lUserID, uint dwResult, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo, IntPtr pUser);

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_USER_LOGIN_INFO
    {
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 129)]
        public string sDeviceAddress;
        public byte byUseTransport;
        public ushort wPort;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string sUserName;
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string sPassword;
        public LoginResultCallBack cbLoginResult;
        public IntPtr pUser;
        public bool bUseAsynLogin;
        public byte byProxyType;
        public byte byUseUTCTime;
        public byte byLoginMode; //登录模式 0-Private 1-ISAPI 2-自适应（默认不采用自适应是因为自适应登录时，会对性能有较大影响，自适应时要同时发起ISAPI和Private登录）
        public byte byHttps;    //ISAPI登录时，是否使用HTTPS，0-不使用HTTPS，1-使用HTTPS 2-自适应（默认不采用自适应是因为自适应登录时，会对性能有较大影响，自适应时要同时发起HTTP和HTTPS）
        public int iProxyID;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes3;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct NET_DVR_PREVIEWINFO
    {
        public int lChannel;    //Channel no.
        public uint dwStreamType;   //Stream type 0-main stream,1-sub stream,2-third stream,3-forth stream, and so on
        public uint dwLinkMode; //Protocol type: 0-TCP, 1-UDP, 2-Muticast, 3-RTP,4-RTP/RTSP, 5-RSTP/HTTP
        public IntPtr hPlayWnd; //Play window's handle;  set NULL to disable preview
        public uint bBlocked;   //If data stream requesting process is blocked or not: 0-no, 1-yes
        //if true, the SDK Connect failure return until 5s timeout  , not suitable for polling to preview.
        public uint bPassbackRecord;    //0- not enable  ,1 enable
        public byte byPreviewMode;  //Preview mode 0-normal preview,2-delay preview
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] byStreamID;   //Stream ID
        public byte byProtoType;    //0-private,1-RTSP
        public byte byRes1;
        public byte byVideoCodingType;
        public uint dwDisplayBufNum;    //soft player display buffer size(number of frames), range:1-50, default:1
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 216, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_ALARMER
    {
        public byte byUserIDValid; /* Whether userID is valid,  0- invalid 1- valid. */
        public byte bySerialValid; /* Whether serial number is valid,  0- invalid 1- valid.  */
        public byte byVersionValid; /* Whether version number is valid,  0- invalid 1- valid. */
        public byte byDeviceNameValid; /* Whether device name is valid,  0- invalid 1- valid. */
        public byte byMacAddrValid; /* Whether MAC address is valid,  0- invalid 1- valid. */
        public byte byLinkPortValid; /* Whether login port number is valid,  0- invalid 1- valid. */
        public byte byDeviceIPValid; /* Whether device IP is valid,  0- invalid 1- valid.*/
        public byte bySocketIPValid; /* Whether socket IP is valid,  0- invalid 1- valid. */
        public int lUserID; /* NET_DVR_Login () effective when establishing alarm upload channel*/
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
        public byte[] sSerialNumber; /* Serial number. */
        public uint dwDeviceVersion; /* Version number,  2 high byte means the major version,  2 low byte means the minor version*/
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string sDeviceName; /* Device name. */
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
        public byte[] byMacAddr; /* MAC address */
        public ushort wLinkPort; /* link port */
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string sDeviceIP; /* IP address */
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string sSocketIP; /* alarm push- mode socket IP address. */
        public byte byIpProtocol;  /* IP protocol:  0- IPV4;  1- IPV6. */
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes2;
    }
    //public delegate bool MSGCallBack_V31(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser);

    [StructLayout(LayoutKind.Sequential)]
    public struct NET_DVR_REALTIME_THERMOMETRY_COND
    {
        public uint dwSize;
        public uint dwChan;
        public byte byRuleID;
        public byte byMode;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 62, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_VCA_POINT
    {
        public Single fX;
        public Single fY;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_VCA_POLYGON
    {
        public uint dwPointNum;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public NET_VCA_POINT[] struPos;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_LINEPOLYGON_THERM_CFG
    {
        public Single fMaxTemperature;
        public Single fMinTemperature;
        public Single fAverageTemperature;
        public Single fTemperatureDiff;
        public NET_VCA_POLYGON struRegion;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_POINT_THERM_CFG
    {
        public Single fTemperature;
        public NET_VCA_POINT struPoint;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 120, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct NET_DVR_THERMOMETRY_UPLOAD
    {
        public uint dwSize;
        public uint dwRelativeTime;
        public uint dwAbsTime;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] szRuleName;//char类型的一个数组
        public byte byRuleID;
        public byte byRuleCalibType;
        public ushort wPresetNo;
        public NET_DVR_POINT_THERM_CFG struPointThermCfg;
        public NET_DVR_LINEPOLYGON_THERM_CFG struLinePolygonThermCfg;
        public byte byThermometryUnit;
        public byte byDataType;
        public byte byRes1;
        public byte bySpecialPointThermType;
        public Single fCenterPointTemperature;
        public Single fHighestPointTemperature;
        public Single fLowestPointTemperature;
        public NET_VCA_POINT struHighestPoint;
        public NET_VCA_POINT struLowestPoint;
        public byte byIsFreezedata;
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 95, ArraySubType = UnmanagedType.I1)]
        public byte[] byRes;
    }
    public delegate void RemoteConfigCallback(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData);
    #endregion

    #region 定义委托类型函数
    public delegate void RealDataCallBack(int lPlayHandle, uint dwDataType, IntPtr pBuffer, uint dwBufSize, IntPtr pUser);
    public delegate void EXCEPYIONCALLBACK(uint dwType, int lUserID, int lHandle, IntPtr pUser);

    public delegate bool NET_DVR_Init();
    public delegate bool NET_DVR_Cleanup();
    public delegate uint NET_DVR_GetLastError();
    public delegate int  NET_DVR_Login_V40(ref NET_DVR_USER_LOGIN_INFO pLoginInfo, ref NET_DVR_DEVICEINFO_V40 lpDeviceInfo);
    public delegate int  NET_DVR_RealPlay_V40(int lUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, RealDataCallBack fRealDataCallBack_V30, IntPtr pUser);
    public delegate bool NET_DVR_StopRealPlay(int iRealHandle);
    public delegate bool NET_DVR_Logout_V30(Int32 lUserID);
    //public delegate bool NET_DVR_SetDVRMessageCallBack_V31(MSGCallBack_V31 fMessageCallBack, IntPtr pUser);
    public delegate int  NET_DVR_StartRemoteConfig(int lUserID, uint dwCommand, IntPtr lpInBuffer, Int32 dwInBufferLen, RemoteConfigCallback cbStateCallback, IntPtr pUserData);
    public delegate bool NET_DVR_StopRemoteConfig(int lHandle);
    public delegate bool NET_DVR_SetExceptionCallBack_V30(uint nMessage, IntPtr hWnd, EXCEPYIONCALLBACK fExceptionCallBack, IntPtr pUser);
    public delegate int NET_DVR_SetupAlarmChan_V30(int lUserID);
    public delegate int NET_DVR_SetupAlarmChan_V41(int lUserID, LPNET_DVR_SETUPALARM_PARAM lpSetupParam);
    //public delegate int NET_DVR_StartListen_V30(string sLocalIP,int wLocalPort, MSGCallBack_V31 DataCallback,IntPtr pUserData);
    #endregion

    public class HikVideoAPI
    {
        #region 成员变量

        #region 声明委托并初始化
        public static NET_DVR_Init NET_HIK_Init = null;
        public static NET_DVR_Cleanup NET_HIK_Cleanup = null;
        public static NET_DVR_GetLastError NET_HIK_GetLastError = null;
        public static NET_DVR_Login_V40 NET_HIK_Login_V40 = null;
        public static NET_DVR_RealPlay_V40 NET_HIK_RealPlay_V40 = null;
        public static NET_DVR_StopRealPlay NET_HIK_StopRealPlay = null;
        public static NET_DVR_Logout_V30 NET_HIK_Logout_V30 = null;
        //public static NET_DVR_SetDVRMessageCallBack_V31 NET_HIK_SetDVRMessageCallBack_V31 = null;
        public static NET_DVR_StartRemoteConfig NET_HIK_StartRemoteConfig = null;
        public static NET_DVR_StopRemoteConfig NET_HIK_StopRemoteConfig = null;
        public static NET_DVR_SetExceptionCallBack_V30 NET_HIK_SetExceptionCallBack_V30 = null;
        public static NET_DVR_SetupAlarmChan_V30 NET_HIK_SetupAlarmChan_V30 = null;
        public static NET_DVR_SetupAlarmChan_V41 NET_HIK_SetupAlarmChan_V41 = null;
        //public static NET_DVR_StartListen_V30 NET_HIK_StartListen_V30 = null;
        #endregion
        /// <summary>
        /// 加载动态库的返回句柄
        /// </summary>
        public static Int32 m_hHikVedeo;
        #endregion

        #region 加载动态库
        /// <summary>
        /// 加载动态库
        /// </summary>
        /// <param name="_strFilePath">动态库的完整路径</param>
        /// <returns>成功返回 true 失败返回 false</returns>
        public static bool LoadHikVideoAPI(string _strFilePath)
        {
            if (m_hHikVedeo != 0)
            {
                return true;
            }
            if (File.Exists(_strFilePath) != true)
            {
                return false;
            }
            m_hHikVedeo = Win32API.LoadLibrary(_strFilePath);
            if (m_hHikVedeo <= 0)
            {
                return false;
            }
            #region 实例化委托并判null
            NET_HIK_Login_V40 = (NET_DVR_Login_V40)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_Login_V40",
                typeof(NET_DVR_Login_V40));
            if (NET_HIK_Login_V40 == null) { return false; }
            NET_HIK_Init = (NET_DVR_Init)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_Init",
                typeof(NET_DVR_Init));
            if (NET_HIK_Init == null) { return false; }
            NET_HIK_Cleanup = (NET_DVR_Cleanup)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_Cleanup",
                typeof(NET_DVR_Cleanup));
            if (NET_HIK_Cleanup == null) { return false; }
            NET_HIK_GetLastError = (NET_DVR_GetLastError)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_GetLastError",
                typeof(NET_DVR_GetLastError));
            if (NET_HIK_GetLastError == null) { return false; }
            NET_HIK_RealPlay_V40 = (NET_DVR_RealPlay_V40)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_RealPlay_V40",
                typeof(NET_DVR_RealPlay_V40));
            if (NET_HIK_RealPlay_V40 == null) { return false; }
            NET_HIK_StopRealPlay = (NET_DVR_StopRealPlay)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_StopRealPlay",
                typeof(NET_DVR_StopRealPlay));
            if (NET_HIK_StopRealPlay == null) { return false; }
            NET_HIK_Logout_V30 = (NET_DVR_Logout_V30)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_Logout_V30",
                typeof(NET_DVR_Logout_V30));
            //if (NET_HIK_Logout_V30 == null) { return false; }
            //NET_HIK_SetDVRMessageCallBack_V31 = (NET_DVR_SetDVRMessageCallBack_V31)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_SetDVRMessageCallBack_V31",
            //    typeof(NET_DVR_SetDVRMessageCallBack_V31));
            //if (NET_HIK_SetDVRMessageCallBack_V31 == null) { return false; }
            NET_HIK_StartRemoteConfig = (NET_DVR_StartRemoteConfig)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_StartRemoteConfig",
                typeof(NET_DVR_StartRemoteConfig));
            if (NET_HIK_StartRemoteConfig == null) { return false; }
            NET_HIK_StopRemoteConfig = (NET_DVR_StopRemoteConfig)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_StopRemoteConfig",
                typeof(NET_DVR_StopRemoteConfig));
            if (NET_HIK_StopRemoteConfig == null) { return false; }
            NET_HIK_SetExceptionCallBack_V30 = (NET_DVR_SetExceptionCallBack_V30)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_SetExceptionCallBack_V30",
                typeof(NET_DVR_SetExceptionCallBack_V30));
            if (NET_HIK_SetExceptionCallBack_V30 == null) { return false; }
            NET_HIK_SetupAlarmChan_V30 = (NET_DVR_SetupAlarmChan_V30)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_SetupAlarmChan_V30",
                typeof(NET_DVR_SetupAlarmChan_V30));
            if (NET_HIK_SetupAlarmChan_V30 == null) { return false; }
            NET_HIK_SetupAlarmChan_V41 = (NET_DVR_SetupAlarmChan_V41)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_SetupAlarmChan_V41",
                typeof(NET_DVR_SetupAlarmChan_V41));
            if (NET_HIK_SetupAlarmChan_V41 == null) { return false; }
            //NET_HIK_StartListen_V30 = (NET_DVR_StartListen_V30)FuncCommon.GetFunctionAddress(m_hHikVedeo, "NET_DVR_StartListen_V30",
            //    typeof(NET_DVR_StartListen_V30));
            //if (NET_HIK_StartListen_V30 == null) { return false; }
                #endregion

                return true;
        }
        #endregion

        #region 释放动态库
        /// <summary>
        /// 释放动态库
        /// </summary>
        /// <returns>成功返回 true 失败返回 false</returns>
        public static bool FreeHikVideoAPI()
        {
            if (m_hHikVedeo != 0)
            {
                Win32API.FreeLibrary(m_hHikVedeo);
                m_hHikVedeo = 0;
            }
            return true;
        }
        #endregion
    }
}
