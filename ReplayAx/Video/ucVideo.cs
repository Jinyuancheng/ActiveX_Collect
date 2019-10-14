using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IcomClient.API;
using IcomClient.Common;
using CentralEcoCity.Video;
using CentralEcoCity;
using System.Runtime.InteropServices;
using PreviewDemo;
using ReplayAx.HikSDK;
using System.Threading;
using BethVideo;
using CentralEcoCity.API;
using Newtonsoft.Json.Linq;
using static PreviewDemo.CHCNetSDK;

namespace ReplayAx.Video
{
    //对外消息委托代理
    public delegate void RecvTempMsgHandler(string value);

    [GuidAttribute("5CE117F8-2396-4993-855A-6430F02C0574")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ControlEvents
    {
        [DispIdAttribute(0x001)]
        void OnRecvTempMsg(string value);
    }

    [Guid("6F9B18E4-57E4-47D6-875E-8E4BEF4A40CE")]
    [ComSourceInterfacesAttribute(typeof(ControlEvents))]
    public partial class ucVideo : UserControl, IObjectSafety
    {
        private string m_sStartTime; //开始时间
        private string m_sEndTime;   //结束时间
        private string m_sPalyTime;  //播放时间
        private string m_sUrl;       //服务器ip
        private string m_sCamId;     //摄像机id
        private int m_iChannel;   //海康摄像机的通道号
        private VsClientMsgCB m_MsgCallBack;//回调函数
        private pReadIniCallBack m_funcReadIniMsg;  //读取配置文件的回调函数

        //读取配置文件
        private string m_sCapPicPath;   //抓拍图片的存储路径
        private string m_sVsClientPath; //VsClient.dll的路径
        private string m_sHCNetSDKPath; //HCNetSDK.dll的路径

        //摄像机类型
        private int m_iCamType; //摄像机类型 1.自己api 2.海康api
        private string m_sCapPath;//摄像机抓拍图片的路径

        //操作海康摄像机回放使用的属性
        private CHCNetSDK.MSGCallBack_V31 m_MsgCallbackHik; //回调函数
        private List<CLoginInfo> m_lstLoginInfo;   //用来存储登录信息
        private object m_oSingleLock;    //线程锁
        private Thread m_oThread;        //开启子线程用来做海康摄像机登录操作
        private bool m_bTerminated = false;/*\ 判断是否结束 /*/

        private bool m_bIsFirstFloder;  //是否是第一次文件夹（配置文件中没有抓拍文件的路径）
        private List<NET_DVR_IPPARACFG_V40> m_lstStruIpParaCfgV40;//用来存储计算通道号的信息数据
        //事件
        public event RecvTempMsgHandler OnRecvTempMsg;//用来给js回调数据
        public ucVideo()
        {
            InitializeComponent();
            Init();
        }
        /*******************************************************/
        #region IObjectSafety 接口成员实现ActiveX（直接拷贝即可）

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
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            m_MsgCallBack = new VsClientMsgCB(MsgCallback);
            m_sUrl = "";
            m_sStartTime = "";
            m_sEndTime = "";
            m_sPalyTime = "";
            m_sCamId = "";
            ///海康摄像机操作 属性初始化
            m_MsgCallbackHik = new CHCNetSDK.MSGCallBack_V31(MsgCallbackHik);
            m_lstLoginInfo = new List<CLoginInfo>();
            m_oSingleLock = new object();
            m_oThread = new Thread(ThreadLogin);
            m_oThread.Start();

            //初始化读取配置文件的信息
            m_sCapPicPath = "";
            m_sVsClientPath = "";
            m_sHCNetSDKPath = "";

            //读取配置文件的回调函数
            m_funcReadIniMsg = new pReadIniCallBack(ReadIniCallBack);

            //设备类型初始化
            m_iCamType = -1;
            m_sCapPath = "";
            m_bIsFirstFloder = false;
            m_lstStruIpParaCfgV40 = new List<NET_DVR_IPPARACFG_V40>();
        }

        #region [回调函数] 回调函数
        //回调函数
        public void MsgCallback(Int32 _hHandle, Int32 _iwParam, Int32 _ilParam)
        {
            Console.WriteLine("_hHandle : " + _hHandle + "        " +
                "_iwParam : " + _iwParam + "          " +
                "_ilParam : " + _ilParam);
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
                    //读取的抓拍路径
                    if (Obj.Key == "CapPicPath")
                    {
                        JToken jToken = Obj.Value;
                        m_sCapPicPath = jToken.Value<string>();
                        if (m_sCapPicPath != "")
                        {
                            m_sCapPath = m_sCapPicPath;
                            ucSVideo.SetCapturePath(m_sCapPicPath);
                            m_bIsFirstFloder = true;
                        }
                    }
                    //读取的VsClient.dll的路径,并初始化动态库
                    else if (Obj.Key == "VsClientPath")
                    {
                        JToken jToken = Obj.Value;
                        m_sVsClientPath = jToken.Value<string>();
                        if (m_sVsClientPath != "")
                        {
                            //加载自己的api库
                            if (VsClientAPI.LoadVsClientAPI(m_sVsClientPath))
                            {
                                VsClientAPI.VSSP_ClientStartup(DefIcomClient.WM_VSCLIENT_DLL_MSG, this.Handle, m_MsgCallBack);
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
                        System.Environment.SetEnvironmentVariable("HDCNETSDK_REPLAY", m_sHCNetSDKPath);
                    }
                }
            }
        }

        //海康回调函数
        public bool MsgCallbackHik(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer,
            IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            return true;
        }
        #endregion

        /// <summary>
        /// 使用线程登录（使用）
        /// </summary>
        public void ThreadLogin()
        {
            //string file = "G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\CentralEcoCity\\bin\\HCNetSDK.dll";
            //InitHikVideoSDK(m_iHCNetDllPath);
            int iDelay = 0;/*\ 延迟 /*/
            while(!m_bTerminated)
            {
                if(m_lstLoginInfo.Count == 0)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    iDelay = 0;
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
                                if (m_lstLoginInfo[i].iHandle >= 0)
                                {
                                    //存储数据用来计算通道号
                                    NET_DVR_IPPARACFG_V40 oIpParaCfgV40 = new NET_DVR_IPPARACFG_V40();
                                    uint dwSize = (uint)Marshal.SizeOf(oIpParaCfgV40);
                                    IntPtr ptrIpParaCfgV40 = Marshal.AllocHGlobal((Int32)dwSize);
                                    Marshal.StructureToPtr(oIpParaCfgV40, ptrIpParaCfgV40, false);
                                    uint dwReturn = 0;
                                    //int iGroupNo = 0; //该Demo仅获取第一组64个通道，如果设备IP通道大于64路，需要按组号0~i多次调用NET_DVR_GET_IPPARACFG_V40获取
                                    /*\ 一共16组，每组64个，共1024个 /*/
                                    for (int iGroupNo = 0; iGroupNo < 15; iGroupNo++)
                                    {
                                        if (CHCNetSDK.NET_DVR_GetDVRConfig(m_lstLoginInfo[i].iHandle, CHCNetSDK.NET_DVR_GET_IPPARACFG_V40, iGroupNo, ptrIpParaCfgV40, dwSize, ref dwReturn))
                                        {
                                            lock(m_oSingleLock)
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
        /// 海康主机登录（未使用）
        /// </summary>
        public void HikHostLogin()
        {
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
                            return;
                        }
                        else
                        {
                            //存储数据用来计算通道号
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
                        //第二种登录
                        //CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();
                        //m_lstLoginInfo[i].iHandle = CHCNetSDK.NET_DVR_Login_V30(m_lstLoginInfo[i].sIp, Convert.ToInt32(m_lstLoginInfo[i].sPort),
                        //    m_lstLoginInfo[i].sUser, m_lstLoginInfo[i].sPass, ref DeviceInfo);
                    }
                }
            }
        }

        /// <summary>
        /// 根据ip返回通道号(海康摄像机)
        /// </summary>
        /// <param name="_sIp">ip</param>
        private int RetChannelWithIpHik(string _sIp)
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

        #region [对外接口] ActiveX对外接口
        /// <summary>
        /// 添加主机信息(用于海康登录)
        /// </summary>
        /// <param name="_sIp">登录ip地址</param>
        /// <param name="_sStreamIp">流媒体的ip地址</param>
        /// <param name="_sPort">登录端口</param>
        /// <param name="_sUser">登录用户名</param>
        /// <param name="_sPass">登录密码</param>
        public void AddHost(string _sIp, string _sStreamIp, string _sPort, string _sUser, string _sPass)
        {
            if (m_lstLoginInfo.Count == 0)
            {
                CLoginInfo oLoginInfo = new CLoginInfo();
                oLoginInfo.iHandle = -1;
                oLoginInfo.sIp = _sIp;
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
                    if (m_lstLoginInfo[i].sIp == _sIp)
                    {
                        return;
                    }
                }
                CLoginInfo oLoginInfo = new CLoginInfo();
                oLoginInfo.iHandle = -1;
                oLoginInfo.sIp = _sIp;
                oLoginInfo.sPort = _sPort;
                oLoginInfo.sUser = _sUser;
                oLoginInfo.sPass = _sPass;
                oLoginInfo.sStreamIp = _sStreamIp;
                m_lstLoginInfo.Add(oLoginInfo);
            }
            //HikHostLogin();
        }
        /// <summary>
        /// 初始化ActiveX
        /// </summary>
        /// <param name="_sReadIniDllPath">ReadIni.dll的路径</param>
        /// <param name="_sReadIniFilePath">要读取的配置文件路径,包含文件名</param>
        public void InitActiveX(string _sReadIniDllPath, string _sReadIniFilePath)
        {
            //加载读取配置文件的api
            if (ReadIniAPI.LoadReadIniAPI(_sReadIniDllPath))
            {
                ReadIniAPI.StartUp(_sReadIniFilePath, m_funcReadIniMsg);
                //读取配置文件 并存储起来
                ReadIniAPI.GetValueWithTitleAndKey("PATH", "CapPicPath");
                ReadIniAPI.GetValueWithTitleAndKey("PATH", "VsClientPath");
                ReadIniAPI.GetValueWithTitleAndKey("PATH", "HikSDKPath");
            }
            else
            {
                MessageBox.Show("加载ReadIni未成功");
            }
            //加载海康api库
            if (CHCNetSDK.NET_DVR_Init())
            {
                CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V31(m_MsgCallbackHik, IntPtr.Zero);
            }
        }
        /// <summary>
        /// 返回一个时间结构体
        /// </summary>
        /// <param name="Time">要转换为结构体的时间</param>
        /// <returns></returns>
        public DefVsClient.PLAY_NETTIME GetTimeObject(string Time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime oStartTime = startTime.AddSeconds(Convert.ToDouble(Time));
            DefVsClient.PLAY_NETTIME StartTime = new DefVsClient.PLAY_NETTIME();
            StartTime.m_iYear = oStartTime.Year;
            StartTime.m_uchMonth = (byte)oStartTime.Month;
            StartTime.m_uchDay = (byte)oStartTime.Day;
            StartTime.m_uchHour = (byte)oStartTime.Hour;
            StartTime.m_uchMinute = (byte)oStartTime.Minute;
            StartTime.m_uchSecond = (byte)oStartTime.Second;
            return StartTime;
        }
        /// <summary>
        /// 播放录像
        /// </summary>
        /// <param name="_pchUrl">自己api,流媒体服务器Ip(海康api,摄像机ip)</param>
        /// <param name="_startTime">开始时间</param>
        /// <param name="_endTime">结束时间</param>
        /// <param name="_playTime">播放时间</param>
        /// <param name="_pchCamId">摄像机id</param>
        /// <param name="_sType">摄像机类型 1,自己的api 2.海康的api</param>
        public void StartPlay(string _pchUrl, string _startTime, string _endTime,
            string _playTime, string _pchCamId, string _sType)
        {
            m_sCamId = _pchCamId;
            m_sStartTime = _startTime;
            m_sEndTime = _endTime;
            m_sPalyTime = _playTime;
            m_sUrl = _pchUrl;
            m_iChannel = RetChannelWithIpHik(_pchUrl);
            m_iCamType = Convert.ToInt32(_sType);
            //调用api的类型
            switch (m_iCamType)
            {
                case 1: //自己api
                    byte[] sIp = Encoding.Default.GetBytes(_pchUrl);
                    DefVsClient.PLAY_NETTIME StartTime = GetTimeObject(_startTime);
                    DefVsClient.PLAY_NETTIME EndTime = GetTimeObject(_endTime);
                    DefVsClient.PLAY_NETTIME PlayTime = GetTimeObject(_playTime);
                    ucSVideo.StartPlay(sIp, StartTime, EndTime, PlayTime, _pchCamId);
                    //设置设备类型
                    ucSVideo.SetCamType(m_iCamType);
                    //设置播放时间
                    ucSVideo.SetPalyTime(_startTime, _endTime, _playTime);
                    break;
                case 2: //海康api
                    if (m_lstLoginInfo.Count > 0)
                    {
                        for (int i = 0; i < m_lstLoginInfo.Count; i++)
                        {
                            if (m_lstLoginInfo[i].iHandle != -1 && m_lstLoginInfo[i].sIp == _pchUrl)
                            {
                                //设置设备类型
                                ucSVideo.SetCamType(m_iCamType);
                                //播放海康的录像回放
                                ucSVideo.StartPlayHik(m_lstLoginInfo[i].iHandle, _startTime, _endTime, m_iChannel);
                                //设置播放时间
                                ucSVideo.SetPalyTime(_startTime, _endTime, _playTime);
                            }
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="uiIsPaly">1暂停播放,0继续播放</param>
        public void PausePlay(uint uiIsPaly)
        {
            bool bIsPaly = false;
            if (m_iCamType != -1)
            {
                switch (m_iCamType)
                {
                    case 1://自己的api
                        ucSVideo.PausePlay(uiIsPaly);
                        break;
                    case 2://海康的api
                        if (uiIsPaly == 1)
                        {
                            bIsPaly = true;
                        }
                        ucSVideo.PasePlayHik(bIsPaly);
                        break;
                }
            }
        }
        /// <summary>
        /// 停止播放录像
        /// </summary>
        public void StopPlay()
        {
            if (m_iCamType != -1)
            {
                switch (m_iCamType)
                {
                    case 1://自己api
                        ucSVideo.StopPlay();
                        break;
                    case 2://海康api
                        ucSVideo.StopPlayHik();
                        break;
                }
            }
        }
        /// <summary>
        /// 快进播放
        /// </summary>
        public void FastPlay()
        {
            if (m_iCamType != -1)
            {
                switch (m_iCamType)
                {
                    case 1://自己api
                        ucSVideo.FastPlay();
                        break;
                    case 2://海康api
                        ucSVideo.FastPlayHik();
                        break;
                }
            }
        }
        /// <summary>
        /// 慢进播放
        /// </summary>
        public void SlowPlay()
        {
            if (m_iCamType != -1)
            {
                switch (m_iCamType)
                {
                    case 1://自己api
                        ucSVideo.SlowPlay();
                        break;
                    case 2://海康api
                        ucSVideo.SlowPlayHik();
                        break;
                }
            }
        }
        /// <summary>
        /// 停止快进慢进状态
        /// </summary>
        public void RecoverNormalPlay()
        {
            switch (m_iCamType)
            {
                case 1://自己api
                    PausePlay(1);
                    PausePlay(0);
                    break;
                case 2://海康api
                    ucSVideo.RecoverNormalPlayHik();
                    break;
            }
        }
        /// <summary>
        /// 抓拍图片 
        /// </summary>
        public void CaptureBmpEx()
        {
            if (m_iCamType != -1)
            {
                //说明配置文件中已经拥有文件的抓拍路径了
                if (m_bIsFirstFloder)
                {
                    switch (m_iCamType)
                    {
                        case 1: //自己api
                            string CapPathPath = m_sCapPath + "\\";
                            ucSVideo.SetCapturePath(CapPathPath);
                            ucSVideo.CaptureBmpEx();
                            break;
                        case 2: //海康api
                            string CapPathHikPath = m_sCapPath + "\\";
                            ucSVideo.SetCapPath(CapPathHikPath);
                            ucSVideo.CaptureJpegHik();
                            break;
                    }

                }
                //用户第一次抓拍，还没有抓拍路径
                else
                {
                    FolderBrowserDialog dialog = new FolderBrowserDialog();
                    dialog.Description = "请选择抓拍图片文件存储路径";
                    switch (m_iCamType)
                    {
                        case 1: //自己api
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                m_sCapPath = dialog.SelectedPath;
                                m_sCapPath = m_sCapPath.Replace(@"\", @"\\");
                                //将路径写入到配置文件中
                                if (ReadIniAPI.SetValueWithTitleAndKey("PATH", "PATH.CapPicPath", m_sCapPath))
                                {
                                    MessageBox.Show("配置抓拍文件存储路径成功", "提示");
                                }
                            }
                            string CapPathPath = m_sCapPath + "\\";
                            ucSVideo.SetCapturePath(CapPathPath);
                            ucSVideo.CaptureBmpEx();
                            m_bIsFirstFloder = true;
                            break;
                        case 2: //海康api
                            CHCNetSDK.NET_DVR_JPEGPARA JpegInfo = new CHCNetSDK.NET_DVR_JPEGPARA();
                            JpegInfo.wPicSize = 3; //3-UXGA(1600*1200)， 
                            JpegInfo.wPicQuality = 2;//0-最好，1-较好，2-一般 
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                m_sCapPath = dialog.SelectedPath;
                                m_sCapPath = m_sCapPath.Replace(@"\", @"\\");
                                //将路径写入到配置文件中
                                if (ReadIniAPI.SetValueWithTitleAndKey("PATH", "PATH.CapPicPath", m_sCapPath))
                                {
                                    MessageBox.Show("配置抓拍文件存储路径成功", "提示");
                                }
                            }
                            string CapPathHikPath = m_sCapPath + "\\";
                            ucSVideo.SetCapPath(CapPathHikPath);
                            ucSVideo.CaptureJpegHik();
                            m_bIsFirstFloder = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 返回文件的播放时间 
        /// </summary>
        /// <returns>文件播放时间，返回的是播放的多少秒</returns>
        public void GetPlayTime()
        {
            switch (m_iCamType)
            {
                case 1://自己api;
                    string Time = "{\"tag\":\"PlayTime\",\"time\":\"" + Convert.ToString(ucSVideo.GetFilePlayTime() / 1000) + "\"}";
                    if (OnRecvTempMsg != null)
                    {
                        OnRecvTempMsg(Time);
                    }
                    break;
                case 2://海康api
                    Time = "{\"tag\":\"PlayTime\",\"time\":\"" + Convert.ToString(ucSVideo.GetCurrentPlayTime()) + "\"}";
                    if (OnRecvTempMsg != null)
                    {
                        OnRecvTempMsg(Time);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获取总的播放时间
        /// </summary>
        public void GetTotalTime()
        {
            string Time;
            switch (m_iCamType)
            {
                case 1://自己api
                    Time = "{\"tag\":\"StartTime\",\"time\":\"" + Convert.ToString(ucSVideo.GetFilePlayTimeByTime()) + "\"}";
                    if (OnRecvTempMsg != null)
                    {
                        OnRecvTempMsg(Time);
                    }
                    break;
                case 2://海康api
                    string sTime = ucSVideo.GetFilePlayTimeHik();
                    if (sTime == null)
                    {
                        MessageBox.Show("该时间内没有视频信息", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    Time = "{\"tag\":\"StartTime\",\"time\":\"" + sTime + "\"}";
                    if (OnRecvTempMsg != null)
                    {
                        OnRecvTempMsg(Time);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 根据时间播放，获取文件开始播放的时间
        /// </summary>
        //public void GetFilePlayTimeByTime()
        //{
        //    string Time;
        //    switch (m_iCamType)
        //    {
        //        case 1:/*\ 自己api /*/
        //            uint uiTime = ucSVideo.GetFilePlayTimeByTime();
        //            Time = "{\"tag\":\"StartTime\",\"time\":\"" + uiTime + "\"}";
        //            if (OnRecvTempMsg != null)
        //            {
        //                OnRecvTempMsg(Time);
        //            }
        //            break;
        //        case 2:/*\ 海康api /*/
        //            break;
        //    }
        //}

        /// <summary>
        /// 设置播放位置
        /// </summary>
        /// <param name="iPos">播放的位置时间</param>
        public void SetPlayPos(string oPosTime)
        {
            if ((Convert.ToDouble(m_sStartTime) < Convert.ToDouble(oPosTime)) &&
                (Convert.ToDouble(m_sEndTime) > Convert.ToDouble(oPosTime)))
            {
                switch (m_iCamType)
                {
                    case 1://自己api
                        DefVsClient.PLAY_NETTIME StartTime = GetTimeObject(oPosTime);
                        ucSVideo.SetPlayPos(StartTime);
                        break;
                    case 2://海康api
                        ucSVideo.SetPlayPlanHik(oPosTime);
                        break;
                }
            }
            else
            {
                MessageBox.Show("播放时间不正确", "提示");
            }
        }
        #endregion
    }
}
