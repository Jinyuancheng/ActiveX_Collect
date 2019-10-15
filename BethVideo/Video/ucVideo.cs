using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Security;
using System.Threading;
using IcomClient.API;
using IcomClient.Common;
using CentralEcoCity.API;
using Newtonsoft.Json.Linq;
using PreviewDemo;
using ReplayAx.HikSDK;
using static PreviewDemo.CHCNetSDK;

namespace BethVideo
{
    //对外消息委托代理
    public delegate void RecvTempMsgHandler(string ip, string value);

    [GuidAttribute("1A585C4D-3371-48dc-AF8A-AFFECC1B0968")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ControlEvents
    {
        [DispIdAttribute(0x001)]
        void OnRecvTempMsg(string ip, string value);
    }

    [Guid("984770F0-A5B6-441f-A01A-58B6F1E31E3F")]
    [ComSourceInterfacesAttribute(typeof(ControlEvents))]
    public partial class ucVideo : UserControl, IObjectSafety
    {
        /*******************************************************/
        #region IObjectSafety 接口成员实现（直接拷贝即可）

        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";
        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";
        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";
        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";
        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";

        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 0x00000001;
        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 0x00000002;
        private const int S_OK = 0;
        private const int E_FAIL = unchecked((int)0x80004005);
        private const int E_NOINTERFACE = unchecked((int)0x80004002);

        private bool _fSafeForScripting = true;
        private bool _fSafeForInitializing = true;

        public int GetInterfaceSafetyOptions(ref Guid riid, ref int pdwSupportedOptions, ref int pdwEnabledOptions)
        {
            int Rslt = E_FAIL;

            string strGUID = riid.ToString("B");
            pdwSupportedOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER | INTERFACESAFE_FOR_UNTRUSTED_DATA;
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForScripting == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_CALLER;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    Rslt = S_OK;
                    pdwEnabledOptions = 0;
                    if (_fSafeForInitializing == true)
                        pdwEnabledOptions = INTERFACESAFE_FOR_UNTRUSTED_DATA;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            int Rslt = E_FAIL;
            string strGUID = riid.ToString("B");
            switch (strGUID)
            {
                case _IID_IDispatch:
                case _IID_IDispatchEx:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_CALLER) && (_fSafeForScripting == true))
                        Rslt = S_OK;
                    break;
                case _IID_IPersistStorage:
                case _IID_IPersistStream:
                case _IID_IPersistPropertyBag:
                    if (((dwEnabledOptions & dwOptionSetMask) == INTERFACESAFE_FOR_UNTRUSTED_DATA) && (_fSafeForInitializing == true))
                        Rslt = S_OK;
                    break;
                default:
                    Rslt = E_NOINTERFACE;
                    break;
            }

            return Rslt;
        }
        #endregion
        /*******************************************************/
        #region 消息队列等信息[后加]

        private Queue<sDoorNoEntry> m_qDoorNoEntry;      //用来存储门禁消息的消息队列
        private Queue<sSealHead> m_qSealHead;            //模拟量数据实时信息和报警信息
        private Queue<sOnOffAlarm> m_qOnOffInfo;         //开关报警信息
        private Queue<sTempAlarm> m_qTempAlarm;          //温度报警信息
        private Queue<sTempDiffAlarm> m_qTempDiffAlarm;  //温差报警信息
        private Queue<sVideoMove> m_qVideoMoveAlarm;     //视频移动报警信息
        private Queue<sThermalImagery> m_qThermalImagery;//热成像报警信息
        #endregion


        //定义事件
        public event RecvTempMsgHandler OnRecvTempMsg;
        //private string m_strModelPath = "G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\WebCom\\bin\\";
        //private string m_strModelPath = "C:\\HIK\\WebCom\\bin\\";
        private List<sLoginInfor> m_lstLoginInfor;
        private Queue<sTempUpload> m_deqCamTemp;

        private RemoteConfigCallback m_ReMoteConfig;
        private CHCNetSDK.MSGCallBack_V31 m_MsgCallback;
        private EXCEPYIONCALLBACK m_Error;
        private Thread m_threadLoginHost;
        private object m_singleLock = null;
        private bool m_bTerminated = false;

        private VsClientMsgCB m_MsgCallBackSelf;        //自己api的回调函数
        private pReadIniCallBack m_funcReadIniMsg;  //读取配置文件的回调函数

        //读取配置文件
        private string m_sVsClientPath; //VsClient.dll的路径
        private string m_sHCNetSDKPath; //HCNetSDK.dll的路径

        //用于海康摄像机播放视频
        private List<CLoginInfo> m_lstLoginInfo;   //用来存储登录信息
        //获取流媒体服务器的通道号
        public List<CHCNetSDK.NET_DVR_IPPARACFG_V40> m_lstStruIpParaCfgV40;
        public ucVideo()
        {
            InitializeComponent();
            //InitHikVideoSDK();
            Init();
            m_lstLoginInfor = new List<sLoginInfor>();
            m_deqCamTemp = new Queue<sTempUpload>();
            //tmrLoginHost.Enabled = true;
            m_threadLoginHost = new Thread(ThreadLoginHost);
            m_threadLoginHost.Start();
            tmrDealMsg.Enabled = true;
        }

        #region 初始化程序变量[后加]
        public void Init()
        {
            m_qDoorNoEntry = new Queue<sDoorNoEntry>();
            m_qOnOffInfo = new Queue<sOnOffAlarm>();
            m_qSealHead = new Queue<sSealHead>();
            m_qTempAlarm = new Queue<sTempAlarm>();
            m_qTempDiffAlarm = new Queue<sTempDiffAlarm>();
            m_qVideoMoveAlarm = new Queue<sVideoMove>();
            m_qThermalImagery = new Queue<sThermalImagery>();
            m_ReMoteConfig = new RemoteConfigCallback(GetThermInfoCallback);
            //m_MsgCallback = new CHCNetSDK.MSGCallBack_V31(MsgCallback_V31);
            m_singleLock = new object();
            //m_threadLoginHost = new Thread(ThreadLoginHost);
            m_Error = new EXCEPYIONCALLBACK(HikExceptionCallBack);
            //m_threadLoginHost.IsBackground = true;
            //m_threadLoginHost.Start();
            m_MsgCallBackSelf = new VsClientMsgCB(MsgCallback);
            m_MsgCallback = new CHCNetSDK.MSGCallBack_V31(MsgCallbackHik);
            //读取配置文件的回调函数
            m_funcReadIniMsg = new pReadIniCallBack(ReadIniCallBack);
            m_sVsClientPath = "";
            m_sHCNetSDKPath = "";
            m_lstLoginInfo = new List<CLoginInfo>();
            m_lstStruIpParaCfgV40 = new List<NET_DVR_IPPARACFG_V40>();
        }
        #endregion

        #region 回调函数
        //海康回调函数
        public bool MsgCallbackHik(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer,
            IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            return true;
        }
        /// <summary>
        /// 读取配合文件ini的回调函数
        /// </summary>
        /// <param name="_sCallBackData">读取的key值和value(json字符串)</param>
        public void ReadIniCallBack(string _sCallBackData)
        {
            JObject jObject = JObject.Parse(_sCallBackData);
            if (jObject.Count > 0)
            {
                foreach (var Obj in jObject)
                {
                    //读取的VsClient.dll的路径,并初始化动态库
                    if (Obj.Key == "VsClientPath")
                    {
                        JToken jToken = Obj.Value;
                        m_sVsClientPath = jToken.Value<string>();
                        if (m_sVsClientPath != "")
                        {
                            //加载自己的api库
                            if (VsClientAPI.LoadVsClientAPI(m_sVsClientPath))
                            {
                                VsClientAPI.VSSP_ClientStartup(DefIcomClient.WM_VSCLIENT_DLL_MSG, this.Handle, m_MsgCallBackSelf);
                                VsClientAPI.VSSP_ClientWaitTime(2, 2);
                            }
                        }
                    }
                    //读取HCNetSDK.dll的路径，并初始化动态库
                    else if (Obj.Key == "HikSDKPath")
                    {
                        JToken jToken = Obj.Value;
                        m_sHCNetSDKPath = jToken.Value<string>();
                        //设置海康的环境变量
                        System.Environment.SetEnvironmentVariable("HDCNETSDK_BETHVIDEO", m_sHCNetSDKPath);
                        //初始化海康sdk
                        if (CHCNetSDK.NET_DVR_Init())
                        {
                            CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(m_MsgCallback, IntPtr.Zero);
                        }
                    }
                }
            }
        }
        //自己api的回调函数
        public void MsgCallback(Int32 _hHandle, Int32 _iwParam, Int32 _ilParam)
        {
            Console.WriteLine("_hHandle : " + _hHandle + "                _iwParam : "
                + _iwParam + "                 ilParam : " + _ilParam);
        }
        //门禁数据回调函数
        private bool MsgCallback_V31(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            switch (lCommand)
            {
                case 0x5002://门禁消息
                    NET_DVR_ACS_ALARM_INFO strAlarmInfo = new NET_DVR_ACS_ALARM_INFO();
                    byte[] pByte = new byte[1024];
                    int iSize = Marshal.SizeOf(strAlarmInfo);
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(strAlarmInfo));
                    strAlarmInfo = (NET_DVR_ACS_ALARM_INFO)BytesToStruct(pByte, strAlarmInfo.GetType());
                    sDoorNoEntry sDoorAlarmInfo = new sDoorNoEntry();
                    sDoorAlarmInfo.iHandle = pAlarmer.lUserID;
                    sDoorAlarmInfo.bCardNumber = strAlarmInfo.struAcsEventInfo.byCardNo;
                    sDoorAlarmInfo.bCardType = strAlarmInfo.struAcsEventInfo.byCardType;
                    sDoorAlarmInfo.sAlarmTime = strAlarmInfo.struTime;
                    sDoorAlarmInfo.iMajor = strAlarmInfo.dwMajor;
                    sDoorAlarmInfo.iMijor = strAlarmInfo.dwMinor;
                    sDoorAlarmInfo.pPicData = strAlarmInfo.pPicData;
                    sDoorAlarmInfo.dwPicDataLen = strAlarmInfo.dwPicDataLen;
                    sDoorAlarmInfo.byPicTransType = strAlarmInfo.byPicTransType;
                    m_qDoorNoEntry.Enqueue(sDoorAlarmInfo);
                    //MessageBox.Show("dwMajor : " + strAlarmInfo.dwMajor + "dwMinor : " + strAlarmInfo.dwMinor);
                    break;
                case 0x1120://模拟量实时数据信息
                            //case 0x1121://模拟量报警信息
                    NET_DVR_SENSOR_ALARM resObj = new NET_DVR_SENSOR_ALARM();
                    pByte = new byte[1024];
                    iSize = Marshal.SizeOf(resObj);
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(resObj));
                    resObj = (NET_DVR_SENSOR_ALARM)BytesToStruct(pByte, resObj.GetType());
                    sSealHead sSealHeadInfo = new sSealHead();
                    sSealHeadInfo.iHandle = pAlarmer.lUserID;
                    sSealHeadInfo.bName = resObj.byName;
                    sSealHeadInfo.bType = resObj.byType;
                    sSealHeadInfo.fValue = resObj.fValue;
                    sSealHeadInfo.fIVValue = resObj.fOriginalValue;
                    sSealHeadInfo.bAlarmType = resObj.byAlarmType;
                    sSealHeadInfo.bAlarmMode = resObj.byAlarmMode;
                    m_qSealHead.Enqueue(sSealHeadInfo);
                    break;
                case 0x1122://开关量报警信息
                    NET_DVR_SWITCH_ALARM OnOffAlarmObj = new NET_DVR_SWITCH_ALARM();
                    pByte = new byte[1024];
                    iSize = Marshal.SizeOf(OnOffAlarmObj);
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(OnOffAlarmObj));
                    OnOffAlarmObj = (NET_DVR_SWITCH_ALARM)BytesToStruct(pByte, OnOffAlarmObj.GetType());
                    sOnOffAlarm sOnOffInfo = new sOnOffAlarm();
                    sOnOffInfo.iHandle = pAlarmer.lUserID;
                    sOnOffInfo.byName = OnOffAlarmObj.byName;
                    sOnOffInfo.byAlarmType = OnOffAlarmObj.byAlarmType;
                    sOnOffInfo.sAlarmTime = OnOffAlarmObj.struOperateTime;
                    sOnOffInfo.iChannel = OnOffAlarmObj.wSwitchChannel;
                    m_qOnOffInfo.Enqueue(sOnOffInfo);
                    break;
                case 0x5212://温度报警
                    NET_DVR_THERMOMETRY_ALARM TempAlarmObj = new NET_DVR_THERMOMETRY_ALARM();
                    pByte = new byte[1024];
                    iSize = Marshal.SizeOf(TempAlarmObj);
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(TempAlarmObj));
                    TempAlarmObj = (NET_DVR_THERMOMETRY_ALARM)BytesToStruct(pByte, TempAlarmObj.GetType());
                    sTempAlarm TempAlarm = new sTempAlarm();
                    TempAlarm.iHandle = pAlarmer.lUserID;
                    TempAlarm.uiChannel = TempAlarmObj.dwChannel;
                    TempAlarm.byTempUnit = TempAlarmObj.byThermometryUnit;
                    TempAlarm.byAlarmLevel = TempAlarmObj.byAlarmLevel;
                    TempAlarm.byAlarmRule = TempAlarmObj.byAlarmRule;
                    TempAlarm.byAlarmType = TempAlarmObj.byAlarmType;
                    TempAlarm.fCurrTemp = TempAlarmObj.fCurrTemperature;
                    m_qTempAlarm.Enqueue(TempAlarm);
                    break;
                case 0x5211://温差报警
                    NET_DVR_THERMOMETRY_DIFF_ALARM TempDiffAlarmObj = new NET_DVR_THERMOMETRY_DIFF_ALARM();
                    pByte = new byte[1024];
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(TempDiffAlarmObj));
                    TempDiffAlarmObj = (NET_DVR_THERMOMETRY_DIFF_ALARM)BytesToStruct(pByte, TempDiffAlarmObj.GetType());
                    sTempDiffAlarm TempDiffAlarm = new sTempDiffAlarm();
                    TempDiffAlarm.iHandle = pAlarmer.lUserID;
                    TempDiffAlarm.byAlarmLevel = TempDiffAlarmObj.byAlarmLevel;
                    TempDiffAlarm.byAlarmRule = TempDiffAlarmObj.byAlarmRule;
                    TempDiffAlarm.byAlarmType = TempDiffAlarmObj.byAlarmRule;
                    m_qTempDiffAlarm.Enqueue(TempDiffAlarm);
                    break;
                case 0x4007://视频移动报警
                    NET_DVR_ALARMINFO_V30 VideoMoveAlarmObj = new NET_DVR_ALARMINFO_V30();
                    pByte = new byte[1024];
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(VideoMoveAlarmObj));
                    VideoMoveAlarmObj = (NET_DVR_ALARMINFO_V30)BytesToStruct(pByte, VideoMoveAlarmObj.GetType());
                    sVideoMove VideoMoveAlarm = new sVideoMove();
                    VideoMoveAlarm.iHandle = pAlarmer.lUserID;
                    VideoMoveAlarm.uiAlarmType = VideoMoveAlarmObj.dwAlarmType;
                    VideoMoveAlarm.uiChannel = VideoMoveAlarmObj.byChannel;
                    m_qVideoMoveAlarm.Enqueue(VideoMoveAlarm);
                    break;
                case 0x4991://热成像报警
                    NET_DVR_FIREDETECTION_ALARM ThermalImageryObj = new NET_DVR_FIREDETECTION_ALARM();
                    pByte = new byte[1024];
                    iSize = Marshal.SizeOf(ThermalImageryObj);
                    Marshal.Copy(pAlarmInfo, pByte, 0, Marshal.SizeOf(ThermalImageryObj));
                    ThermalImageryObj = (NET_DVR_FIREDETECTION_ALARM)BytesToStruct(pByte, ThermalImageryObj.GetType());
                    sThermalImagery ThermalImagery = new sThermalImagery();
                    ThermalImagery.iHandle = pAlarmer.lUserID;
                    ThermalImagery.iMaxTemp = ThermalImageryObj.wFireMaxTemperature;
                    ThermalImagery.byAlarmSubType = ThermalImageryObj.byAlarmSubType;
                    ThermalImagery.byStrategyType = ThermalImageryObj.byStrategyType;
                    ThermalImagery.chIpv4 = ThermalImageryObj.struDevInfo.struDevIP.sIpV4;
                    m_qThermalImagery.Enqueue(ThermalImagery);
                    break;
                default:
                    break;
            }
            return true;
        }
        //实时测温回调
        private void GetThermInfoCallback(uint dwType, IntPtr lpBuffer, uint dwBufLen, IntPtr pUserData)
        {
            if (dwType == 2) //测温数据
            {
                NET_DVR_THERMOMETRY_UPLOAD resObj = new NET_DVR_THERMOMETRY_UPLOAD();
                byte[] pByte = new byte[1024];
                int iSize = Marshal.SizeOf(resObj);
                Marshal.Copy(lpBuffer, pByte, 0, Marshal.SizeOf(resObj));
                resObj = (NET_DVR_THERMOMETRY_UPLOAD)BytesToStruct(pByte, resObj.GetType());
                sTempUpload tempObj = new sTempUpload();
                tempObj.iHandle = (Int32)pUserData;
                tempObj.fTempHighest = resObj.struLinePolygonThermCfg.fMaxTemperature;
                m_deqCamTemp.Enqueue(tempObj);
            }
        }
        //异常消息回调
        private void HikExceptionCallBack(uint dwType, int lUserID, int lHandle, IntPtr pUser)
        {
            //MessageBox.Show("异常消息回调 : " + "dwType : " + dwType + "lUserId : " +
            //    lUserID + "lHandle : " + lHandle + "pUser : " + pUser);
            switch (dwType)
            {
                case 1:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 公用函数
        public static object BytesToStruct(byte[] bytes, Type strcutType)
        {
            int size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        private string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
        //添加0
        private string Insert0WithTime(uint time)
        {
            if (Convert.ToInt32(time) > 0 && Convert.ToInt32(time) < 10)
            {
                return "0" + time;
            }
            else
            {
                return Convert.ToString(time);
            }
        }
        #endregion

        #region 海康摄像机登录
        //使用线程登录（使用）
        private void ThreadLoginHost()
        {
            int iDelay = 0;/*\ 延迟 /*/
            while (!m_bTerminated)
            {
                if (m_lstLoginInfo.Count == 0)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    iDelay = 0;
                    if (m_lstLoginInfo.Count > 0)
                    {
                        for (int i = 0; i < m_lstLoginInfo.Count; i++)
                        {
                            if (m_lstLoginInfo[i].iHandle == -1)
                            {

                                NET_DVR_USER_LOGIN_INFO struLoginInfo = new NET_DVR_USER_LOGIN_INFO();
                                NET_DVR_DEVICEINFO_V40 devInfor = new NET_DVR_DEVICEINFO_V40();
                                devInfor.byRes2 = new byte[246];
                                devInfor.struDeviceV30.sSerialNumber = new byte[48];
                                devInfor.byRes2 = new byte[246];
                                devInfor.struDeviceV30.sSerialNumber = new byte[48];
                                struLoginInfo.sDeviceAddress = m_lstLoginInfo[i].sStreamIp;
                                struLoginInfo.wPort = Convert.ToUInt16(m_lstLoginInfo[i].sPort); //设备服务端口
                                struLoginInfo.sUserName = m_lstLoginInfo[i].sUser; //设备登录用户名
                                struLoginInfo.sPassword = m_lstLoginInfo[i].sPass; //设备登录密码
                                struLoginInfo.bUseAsynLogin = false; //同步登录方式（异步现在设备不在线时会报错，不知道啥原因）
                                struLoginInfo.byLoginMode = 0;
                                struLoginInfo.byHttps = 2;
                                //m_lstLoginInfo[i].iHandle = HikVideoAPI.NET_HIK_Login_V40(ref struLoginInfo, ref devInfor);
                                m_lstLoginInfo[i].iHandle = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref devInfor);
                                //失败
                                if (m_lstLoginInfo[i].iHandle < 0)
                                {
                                    CHCNetSDK.NET_DVR_Logout(m_lstLoginInfo[i].iHandle);
                                    CHCNetSDK.NET_DVR_Cleanup();
                                    return;
                                }
                                if (m_lstLoginInfo[i].iHandle >= 0)
                                {
                                    NET_DVR_IPPARACFG_V40 oIpParaCfgV40 = new NET_DVR_IPPARACFG_V40();
                                    uint dwSize = (uint)Marshal.SizeOf(oIpParaCfgV40);
                                    IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
                                    Marshal.StructureToPtr(oIpParaCfgV40, ptrIpParaCfgV40, false);
                                    uint dwReturn = 0;
                                    //int iGroupNo = 0; //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取
                                    /*\ 共16组每组64个 /*/
                                    for (int iGroupNo = 0; iGroupNo < 15; iGroupNo++)
                                    {
                                        if (CHCNetSDK.NET_DVR_GetDVRConfig(m_lstLoginInfo[i].iHandle, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
                                        {
                                            lock(m_singleLock)
                                            {
                                                oIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));
                                                m_lstStruIpParaCfgV40.Add(oIpParaCfgV40);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        while (!m_bTerminated)
                        {
                            iDelay++;
                            Thread.Sleep(1000);
                            if (iDelay > 10)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 海康主机登录(未使用)
        /// </summary>
        private void HikHostLogin()
        {
            if(m_lstLoginInfo.Count > 0)
            {
                for (int i = 0; i < m_lstLoginInfo.Count; i++)
                {
                    if (m_lstLoginInfo[i].iHandle == -1)
                    {
                        NET_DVR_USER_LOGIN_INFO struLoginInfo = new NET_DVR_USER_LOGIN_INFO();
                        NET_DVR_DEVICEINFO_V40 devInfor = new NET_DVR_DEVICEINFO_V40();
                        devInfor.byRes2 = new byte[246];
                        devInfor.struDeviceV30.sSerialNumber = new byte[48];
                        devInfor.byRes2 = new byte[246];
                        devInfor.struDeviceV30.sSerialNumber = new byte[48];
                        struLoginInfo.sDeviceAddress = m_lstLoginInfo[i].sStreamIp;
                        struLoginInfo.wPort = Convert.ToUInt16(m_lstLoginInfo[i].sPort); //设备服务端口
                        struLoginInfo.sUserName = m_lstLoginInfo[i].sUser; //设备登录用户名
                        struLoginInfo.sPassword = m_lstLoginInfo[i].sPass; //设备登录密码
                        struLoginInfo.bUseAsynLogin = false; //同步登录方式（异步现在设备不在线时会报错，不知道啥原因）
                        struLoginInfo.byLoginMode = 0;
                        struLoginInfo.byHttps = 2;
                        //m_lstLoginInfo[i].iHandle = HikVideoAPI.NET_HIK_Login_V40(ref struLoginInfo, ref devInfor);
                        m_lstLoginInfo[i].iHandle = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref devInfor);
                        //失败
                        if (m_lstLoginInfo[i].iHandle < 0)
                        {
                            CHCNetSDK.NET_DVR_Logout(m_lstLoginInfo[i].iHandle);
                            CHCNetSDK.NET_DVR_Cleanup();
                            return;
                        }
                        if (m_lstLoginInfo[i].iHandle >= 0)
                        {
                           
                            NET_DVR_IPPARACFG_V40 oIpParaCfgV40 = new NET_DVR_IPPARACFG_V40();
                            uint dwSize = (uint)Marshal.SizeOf(oIpParaCfgV40);
                            IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
                            Marshal.StructureToPtr(oIpParaCfgV40, ptrIpParaCfgV40, false);
                            uint dwReturn = 0;
                            //int iGroupNo = 0; //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取
                            for (int iGroupNo = 0; iGroupNo < 4; iGroupNo++)
                            {
                                if (CHCNetSDK.NET_DVR_GetDVRConfig(m_lstLoginInfo[i].iHandle, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
                                {
                                    oIpParaCfgV40 = (CHCNetSDK.NET_DVR_IPPARACFG_V40)Marshal.PtrToStructure(ptrIpParaCfgV40, typeof(CHCNetSDK.NET_DVR_IPPARACFG_V40));
                                    m_lstStruIpParaCfgV40.Add(oIpParaCfgV40);
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 根据ip计算通道号
        /// </summary>
        /// <param name="_sIp"></param>
        private int ComputeChannel(string _sIp)
        {
            int iChannel = -1;
            if (m_lstStruIpParaCfgV40.Count > 0)
            {
                for (int i = 0; i < m_lstStruIpParaCfgV40.Count; i++)
                {
                    for (int j = 0; j < m_lstStruIpParaCfgV40[i].struIPDevInfo.Length; j++)
                    {
                        if (_sIp == m_lstStruIpParaCfgV40[i].struIPDevInfo[j].struIP.sIpV4)
                        {
                            iChannel = j + 1;
                            break;
                        }
                    }
                }
            }
            return iChannel;
        }
        #endregion

        /// <summary>
        /// 消息处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrDealMsg_Tick(object sender, EventArgs e)
        {
            #region [温度处理回调数据]
            if (m_deqCamTemp.Count > 0)
            {
                sTempUpload tempObj = m_deqCamTemp.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj != null)
                    {
                        if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                        {
                            if (OnRecvTempMsg != null)
                            {
                                string Json = "{"
                                    + "\"fValue\":" + "\"" + tempObj.fTempHighest.ToString("F2") + "\""
                                    + "}";
                                OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                            }
                            break;
                        }
                    }
                }
            }//温度处理
            #endregion

            #region [门禁处理数据]
            if (m_qDoorNoEntry.Count > 0)
            {
                sDoorNoEntry tempObj = m_qDoorNoEntry.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        if (OnRecvTempMsg != null)
                        {
                            string time = Insert0WithTime(tempObj.sAlarmTime.dwYear) + "-"
                                + Insert0WithTime(tempObj.sAlarmTime.dwMonth) + "-"
                                + Insert0WithTime(tempObj.sAlarmTime.dwDay) + " "
                                + Insert0WithTime(tempObj.sAlarmTime.dwHour) + ":"
                                + Insert0WithTime(tempObj.sAlarmTime.dwMinute) + ":"
                                + Insert0WithTime(tempObj.sAlarmTime.dwSecond);
                            string CardNumber = System.Text.Encoding.Default.GetString(tempObj.bCardNumber).Replace("\0", "");
                            string MenJin = "{"
                                //+ "\"bCardNumber\":" + "\"" + BitConverter.ToString(tempObj.bCardNumber).Replace('-', ' ') + "\"" + ","
                                + "\"bCardNumber\":" + "\"" + CardNumber + "\"" + ","
                                + "\"bCardType\":" + "\"" + tempObj.bCardType + "\"" + ","
                                + "\"sAlarmTime\":" + "\"" + time + "\"" + ","
                                + "\"iMajor\":" + "\"" + Convert.ToString(tempObj.iMajor, 16) + "\"" + ","
                                + "\"iMijor\":" + "\"" + Convert.ToString(tempObj.iMijor, 16) + "\"" + ","
                                + "\"PicType\":" + "\"" + tempObj.byPicTransType + "\"" + ","
                                + "\"Pic\":" + "\"" + tempObj.pPicData + "\"" + ","
                                + "\"PicLen\":" + "\"" + tempObj.dwPicDataLen + "\""
                                + "}";
                            OnRecvTempMsg(m_lstLoginInfor[i].strIp, MenJin);
                        }
                    }
                }
            }//门禁
            #endregion

            #region [动环数据处理]
            if (m_qSealHead.Count > 0)//动环
            {
                sSealHead tempObj = m_qSealHead.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        if (OnRecvTempMsg != null)
                        {
                            //MessageBox.Show("btype = 1 : value: " + tempObj.fValue);
                            string Name = System.Text.Encoding.Default.GetString(tempObj.bName).Replace("\0", "");
                            Name = System.Text.RegularExpressions.Regex.Replace(Name, @"[^0-9]+", "");
                            //计算模拟量
                            float Temp = ((((tempObj.fValue - 4) / 16) * 200) - 50);
                            string sTemp = Temp.ToString("f2");
                            string Json = "{"
                                + "\"fValue\":" + "\"" + sTemp + "\"" + ","
                                + "\"byName\":" + "\"" + Name + "\"" + ","
                                + "\"byAlarmType\":" + "\"" + tempObj.bAlarmType + "\"" + ","
                                + "\"byType\":" + "\"" + tempObj.bType + "\"" + ","
                                + "\"byAlarmMode\":" + "\"" + tempObj.bAlarmMode + "\""
                                + "}";
                            OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                        }
                    }
                }
            }
            #endregion

            #region [开关量报警处理]
            if (m_qOnOffInfo.Count > 0)
            {
                sOnOffAlarm tempObj = m_qOnOffInfo.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        string time = Insert0WithTime(Convert.ToUInt32(tempObj.sAlarmTime.wYear)) + "-"
                              + Insert0WithTime(Convert.ToUInt32(tempObj.sAlarmTime.byMonth)) + "-"
                              + Insert0WithTime(Convert.ToUInt32(tempObj.sAlarmTime.byDay)) + " "
                              + Insert0WithTime(Convert.ToUInt32(tempObj.sAlarmTime.byHour)) + ":"
                              + Insert0WithTime(Convert.ToUInt32(tempObj.sAlarmTime.byMinute)) + ":"
                              + Insert0WithTime(Convert.ToUInt32(tempObj.sAlarmTime.bySecond));
                        string name = System.Text.Encoding.Default.GetString(tempObj.byName).Replace("\0", "");
                        string Json = "{"
                                + "\"tag\":" + "\"" + "OnOffAlarm" + "\"" + ","
                                + "\"byName\":" + "\"" + name + "\"" + ","
                                + "\"byAlarmType\":" + "\"" + tempObj.byAlarmType + "\"" + ","
                                + "\"sAlarmTime\":" + "\"" + time + "\"" + ","
                                + "\"iChannel\":" + "\"" + tempObj.iChannel + "\""
                                + "}";
                        OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                    }
                }
            }//开关量报警
            #endregion

            #region [温度报警处理]
            if (m_qTempAlarm.Count > 0)//温度报警
            {
                sTempAlarm tempObj = m_qTempAlarm.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        string Json = "{"
                                + "\"tag\":" + "\"" + "TempAlarm" + "\"" + ","
                                + "\"uiChannel\":" + "\"" + tempObj.uiChannel + "\"" + ","
                                + "\"byTempUnit\":" + "\"" + tempObj.byTempUnit + "\"" + ","
                                + "\"byAlarmLevel\":" + "\"" + tempObj.byAlarmLevel + "\"" + ","
                                + "\"byAlarmRule\":" + "\"" + tempObj.byAlarmRule + "\"" + ","
                                + "\"byAlarmType\":" + "\"" + tempObj.byAlarmType + "\"" + ","
                                + "\"fCurrTemp\":" + "\"" + tempObj.fCurrTemp + "\""
                                + "}";
                        OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                    }
                }
            }
            #endregion

            #region[温差报警处理]
            if (m_qTempDiffAlarm.Count > 0)//温度报警
            {
                sTempDiffAlarm tempObj = m_qTempDiffAlarm.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        string Json = "{"
                                + "\"tag\":" + "\"" + "TempDiffAlarm" + "\"" + ","
                                + "\"uiChannel\":" + "\"" + tempObj.uiChannel + "\"" + ","
                                + "\"byAlarmLevel\":" + "\"" + tempObj.byAlarmLevel + "\"" + ","
                                + "\"byAlarmRule\":" + "\"" + tempObj.byAlarmRule + "\"" + ","
                                + "\"byAlarmType\":" + "\"" + tempObj.byAlarmType + "\""
                                + "}";
                        OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                    }
                }
            }
            #endregion

            #region [视频移动报警处理]
            if (m_qVideoMoveAlarm.Count > 0)//温度报警
            {
                sVideoMove tempObj = m_qVideoMoveAlarm.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        string sChannel = System.Text.Encoding.Default.GetString(tempObj.uiChannel).Replace("\0", "");
                        string Json = "{"
                                + "\"tag\":" + "\"" + "VideoMove" + "\"" + ","
                                + "\"uiChannel\":" + "\"" + sChannel + "\"" + ","
                                + "\"uiAlarmType\":" + "\"" + tempObj.uiAlarmType + "\""
                                + "}";
                        OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                    }
                }
            }
            #endregion

            #region [热成像报警处理]
            if (m_qThermalImagery.Count > 0)//温度报警
            {
                sThermalImagery tempObj = m_qThermalImagery.Dequeue();
                for (int i = 0; i < m_lstLoginInfor.Count; i++)
                {
                    if (tempObj.iHandle == m_lstLoginInfor[i].iHandle)
                    {
                        string Ipv4 = new string(tempObj.chIpv4).Replace("\0", "");
                        string Json = "{"
                                + "\"tag\":" + "\"" + "Thermal" + "\"" + ","
                                + "\"sIp\":" + "\"" + Ipv4 + "\"" + ","
                                + "\"byStrategyType\":" + "\"" + tempObj.byStrategyType + "\""
                                + "\"byAlarmSubType\":" + "\"" + tempObj.byAlarmSubType + "\"" + ","
                                + "\"iMaxTemp\":" + "\"" + tempObj.iMaxTemp + "\"" + ","
                                + "}";
                        OnRecvTempMsg(m_lstLoginInfor[i].strIp, Json);
                    }
                }
            }
            #endregion
        }

        #region 对外接口事件
        /// <summary>
        /// 初始化active控件
        /// </summary>
        /// <param name="_sReadIniDllPath">ReadIni.dll的文件路径（包含文件路径）</param>
        /// <param name="_sReadIniFilePath">config.ini的文件路径（包含文件路径）</param>
        public void InitActiveX(string _sReadIniDllPath, string _sReadIniFilePath)
        {
            if (ReadIniAPI.LoadReadIniAPI(_sReadIniDllPath))
            {
                ReadIniAPI.StartUp(_sReadIniFilePath, m_funcReadIniMsg);
                //读取配置文件 并存储起来
                ReadIniAPI.GetValueWithTitleAndKey("PATH", "VsClientPath");
                ReadIniAPI.GetValueWithTitleAndKey("PATH", "HikSDKPath");
            }
        }

        /// <summary>
        /// 添加主机信息(用于海康登录)
        /// </summary>
        /// <param name="_sStreamIp">流媒体的ip地址</param>
        /// <param name="_sPort">登录端口</param>
        /// <param name="_sUser">登录用户名</param>
        /// <param name="_sPass">登录密码</param>
        public void AddStreamServer(string _sStreamIp, string _sPort, string _sUser, string _sPass)
        {
            if (m_lstLoginInfo.Count == 0)
            {
                CLoginInfo oLoginInfo = new CLoginInfo();
                oLoginInfo.iHandle = -1;
                oLoginInfo.sPort = _sPort;
                oLoginInfo.sUser = _sUser;
                oLoginInfo.sPass = _sPass;
                oLoginInfo.sStreamIp = _sStreamIp;
                m_lstLoginInfo.Add(oLoginInfo);
            }
            else if (m_lstLoginInfo.Count > 0)
            {
                for (int i = 0; i < m_lstLoginInfo.Count; i++)
                {
                    if (m_lstLoginInfo[i].sStreamIp == _sStreamIp)
                    {
                        return;
                    }
                }
                CLoginInfo oLoginInfo = new CLoginInfo();
                oLoginInfo.iHandle = -1;
                oLoginInfo.sPort = _sPort;
                oLoginInfo.sUser = _sUser;
                oLoginInfo.sPass = _sPass;
                oLoginInfo.sStreamIp = _sStreamIp;
                m_lstLoginInfo.Add(oLoginInfo);
            }
            //HikHostLogin();
        }

        /// <summary>
        /// 添加流媒体信息
        /// </summary>
        /// <param name="strIp">摄像机ip</param>
        /// <param name="strStreamIp">流媒体的ip</param>
        /// <param name="strUser">流媒体的登录用户名</param>
        /// <param name="strPsw">登录密码</param>
        /// <param name="iPort">登录端口</param>
        /// <param name="type">登录类型 1.表示自己api 2.表示海康api</param>
        //public void AddHost(string strIp, string strStreamIp, string strUser, string strPsw, int iPort, int type)
        //{
        //    sLoginInfor loginHost = new sLoginInfor();
        //    loginHost.strIp = strIp;
        //    loginHost.strUser = strUser;
        //    loginHost.strPsw = strPsw;
        //    loginHost.iPort = iPort;
        //    loginHost.iHandle = -1;
        //    loginHost.iRemoteHanle = -1;
        //    loginHost.iType = type;
        //    loginHost.devInfor = new NET_DVR_DEVICEINFO_V40();
        //    loginHost.devInfor.byRes2 = new byte[246];
        //    loginHost.devInfor.struDeviceV30.sSerialNumber = new byte[48];
        //    m_lstLoginInfor.Add(loginHost);
        //    //Login();
        //    //MessageBox.Show("Ip : " + strIp + " type : " + type + "devInfo" + loginHost.devInfor);
        //}
        /// <summary>
        ///连接视频
        /// </summary>
        /// <param name="strIp">摄像机Ip</param>
        /// <param name="strStream">流媒体Ip</param>
        /// <param name="strId">摄像机id</param>
        /// <param name="strType">摄像机类型 1.自己api 2.海康api</param>
        /// <param name="strName">摄像机名称</param>
        /// <param name="strCamType">摄像机类型</param>
        public void ConnectVideo(string strIp, string strStream, string strId,
            string strType, string strName, string strCamType)
        {
            int iType = Convert.ToInt32(strType);
            int iChannel = ComputeChannel(strIp);
            int iCamType = Convert.ToInt32(strCamType);
            switch (iType)
            {
                case 1://自己api
                    ucVideoMain.ConnectVideo(iType, strId, strStream, strName, iChannel, iCamType);
                    break;
                case 2://海康api
                    foreach (CLoginInfo item in m_lstLoginInfo)
                    {
                        if (item.sStreamIp == strStream && item.iHandle >= 0)
                        {
                            ucVideoMain.ConnectVideo(iType, strId, strIp, strName, iChannel, iCamType, item.iHandle);
                            break;
                        }
                    }
                    break;
            }

        }
        //断开视频
        public void DisAllConnectVideo()
        {
            ucVideoMain.DisAllConnectVideo();
        }
        //销毁相关资源
        public void UnInitBethVideo()
        {
            tmrDealMsg.Enabled = false;
            ucVideoMain.DisAllConnectVideo();
            for (int i = 0; i < m_lstLoginInfor.Count; i++)
            {
                if (m_lstLoginInfor[i].iHandle >= 0)
                {
                    CHCNetSDK.NET_DVR_Logout_V30(m_lstLoginInfor[i].iHandle);
                }
            }
            m_lstLoginInfo.Clear();
            VsClientAPI.VSSP_ClientCleanup();
            CHCNetSDK.NET_DVR_Cleanup();
        }
        #endregion


    }
}
