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
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using BethVideo;
using System.Threading;
using PreviewDemo;
using static PreviewDemo.CHCNetSDK;
using CentralEcoCity.API;

namespace CentralEcoCity.Video
{
    //对外消息委托代理
    public delegate void RecvTempMsgHandler(string ip, string value);

    [GuidAttribute("1B505CFC-01A1-4562-A7BB-FD8EE3C6FF29")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ControlEvents
    {
        [DispIdAttribute(0x001)]
        void OnRecvTempMsg(string ip, string value);
    }

    [Guid("5E19338B-2079-4375-8CB8-5C9A6D9FDD37")]
    [ComSourceInterfacesAttribute(typeof(ControlEvents))]
    public partial class ucVideo : UserControl, IObjectSafety
    {
        #region 自定义属性
        private VsClientMsgCB m_MsgCallBack;        //自己api的回调函数
        private MSGCallBack_V31 m_MsgCallbackHik;   //海康api的回调函数
        private pReadIniCallBack m_funcReadIniMsg;  //读取配置文件的回调函数
        private List<CLoginInfo> m_lstLoginInfo;    //存储设备登录信息
        //private Thread m_oThreadLogin;//该线程用来登录主机
        private object m_oSingleLock = null;//线程锁

        private string m_sCapPicPath;   //抓拍图片的存储路径
        private string m_sVsClientPath; //VsClient.dll的路径
        private string m_sHCNetSDKPath; //HCNetSDK.dll的路径

        //获取流媒体服务器的通道号
        public List<CHCNetSDK.NET_DVR_IPPARACFG_V40> m_lstStruIpParaCfgV40;
        #endregion


        public ucVideo()
        {
            InitializeComponent();
            m_MsgCallBack = new VsClientMsgCB(MsgCallback);
            m_MsgCallbackHik = new MSGCallBack_V31(HikMsgCallBack);
            m_funcReadIniMsg = new pReadIniCallBack(ReadIniCallBack);
            m_lstLoginInfo = new List<CLoginInfo>();
            m_oSingleLock = new object();
            //m_oThreadLogin = new Thread(ThreadLogin);
            //m_oThreadLogin.Start();

            m_sCapPicPath = "";
            m_sVsClientPath = "";
            m_sHCNetSDKPath = "";

            m_lstStruIpParaCfgV40 = new List<NET_DVR_IPPARACFG_V40>();
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
                            ucVGSHow.SetCapturePath(m_sCapPicPath, true);
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
                        System.Environment.SetEnvironmentVariable("HDCNETSDK", m_sHCNetSDKPath);
                    }
                }
            }
        }

        public void MsgCallback(Int32 _hHandle, Int32 _iwParam, Int32 _ilParam)
        {
            Console.WriteLine("_hHandle : " + _hHandle + "                _iwParam : "
                + _iwParam + "                 ilParam : " + _ilParam);
        }
        /// <summary>
        /// 海康api的回调函数
        /// </summary>
        /// <param name="lCommand"></param>
        /// <param name="pAlarmer"></param>
        /// <param name="pAlarmInfo"></param>
        /// <param name="dwBufLen"></param>
        /// <param name="pUser"></param>
        /// <returns></returns>
        public bool HikMsgCallBack(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            return true;
        }
        /// <summary>
        /// 使用线程登录
        /// </summary>
        public void ThreadLogin()
        {
            //string file = "G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\CentralEcoCity\\bin\\HCNetSDK.dll";
            //InitHikVideoSDK(m_iHCNetDllPath);
            Thread.Sleep(2000);
            while (true)
            {
                lock (m_oSingleLock)
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
                                struLoginInfo.sDeviceAddress = m_lstLoginInfo[i].sIp;
                                struLoginInfo.wPort = Convert.ToUInt16(m_lstLoginInfo[i].sPort); //设备服务端口
                                struLoginInfo.sUserName = m_lstLoginInfo[i].sUser; //设备登录用户名
                                struLoginInfo.sPassword = m_lstLoginInfo[i].sPass; //设备登录密码
                                struLoginInfo.bUseAsynLogin = false; //同步登录方式（异步现在设备不在线时会报错，不知道啥原因）
                                struLoginInfo.byLoginMode = 0;
                                //struLoginInfo.byHttps = 2;
                                //m_lstLoginInfo[i].iHandle = HikVideoAPI.NET_HIK_Login_V40(ref struLoginInfo, ref devInfor);
                                m_lstLoginInfo[i].iHandle = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref devInfor);
                                //将通道信息和对应的ip存储到list集合中
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
            }
        }
        /// <summary>
        /// 用来轮询的时候使用的方法
        /// </summary>
        /// <param name="EpollArray">前端发送过来的数据</param>
        private void EpollFunc(JArray EpollArray)
        {
            int iChannel = -1;
            for (int j = 0; j < EpollArray.Count; j++)
            {
                int iType = Convert.ToInt32(EpollArray[j]["iType"].ToString());
                switch (iType)
                {
                    case 1: //自己api
                        iChannel = Convert.ToInt32(EpollArray[j]["lId"].ToString().Substring(EpollArray[j]["lId"].ToString().Length - 1, 1));
                        ucVGSHow.ConnectVideo(EpollArray[j]["lId"].ToString(), EpollArray[j]["sStreamIp"].ToString(),
                            EpollArray[j]["sCamName"].ToString(), iType, iChannel);
                        break;
                    case 2://海康api
                        iChannel = RetChannelWithIp(EpollArray[j]["sIp"].ToString());
                        for (int i = 0; i < m_lstLoginInfo.Count; i++)
                        {
                            if (m_lstLoginInfo[i].iHandle != -1 &&
                                m_lstLoginInfo[i].sIp == EpollArray[j]["sStreamIp"].ToString())
                            {
                                ucVGSHow.ConnectVideo(EpollArray[j]["lId"].ToString(), m_lstLoginInfo[i].sIp,
                                    EpollArray[j]["sCamName"].ToString(), iType, iChannel, m_lstLoginInfo[i].iHandle);
                                break;
                            }
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 根据ip返回通道号
        /// </summary>
        /// <param name="_sIp">ip</param>
        private int RetChannelWithIp(string _sIp)
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

        /// <summary>
        /// 海康主机登录
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
                        struLoginInfo.sDeviceAddress = m_lstLoginInfo[i].sIp;
                        struLoginInfo.wPort = Convert.ToUInt16(m_lstLoginInfo[i].sPort); //设备服务端口
                        struLoginInfo.sUserName = m_lstLoginInfo[i].sUser; //设备登录用户名
                        struLoginInfo.sPassword = m_lstLoginInfo[i].sPass; //设备登录密码
                        struLoginInfo.bUseAsynLogin = false; //同步登录方式（异步现在设备不在线时会报错，不知道啥原因）
                        struLoginInfo.byLoginMode = 0;
                        //struLoginInfo.byHttps = 2;
                        //m_lstLoginInfo[i].iHandle = HikVideoAPI.NET_HIK_Login_V40(ref struLoginInfo, ref devInfor);
                        m_lstLoginInfo[i].iHandle = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref devInfor);
                        //将通道信息和对应的ip存储到list集合中
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

        #region 添加对外的接口
        /// <summary>
        /// 添加主机信息
        /// </summary>
        /// <param name="_sIp">登录ip地址</param>
        /// <param name="_sPort">登录端口</param>
        /// <param name="_sPass">登录密码</param>
        public void AddHost(string _sIp, string _sPort, string _sUser, string _sPass)
        {
            if (m_lstLoginInfo.Count == 0)
            {
                CLoginInfo oLoginInfo = new CLoginInfo();
                oLoginInfo.iHandle = -1;
                oLoginInfo.sIp = _sIp;
                oLoginInfo.sPort = _sPort;
                oLoginInfo.sUser = _sUser;
                oLoginInfo.sPass = _sPass;
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
                m_lstLoginInfo.Add(oLoginInfo);
            }
            /*\ 执行登录 /*/
            HikHostLogin();
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_sReadIniPath">ReadIni.dll文件路径</param>
        /// <param name="_sReadIniDllPath">要读取的配置文件路径,包含文件名</param>
        public void InitVideoAx(string _sReadIniDllPath, string _sReadIniFilePath)
        {
            this.ucVGSHow.Dock = DockStyle.Fill;
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
        /// 连接视频
        /// </summary>
        /// <param name="sIp">连接视频的ip</param>
        /// <param name="sId">连接视频的id</param>
        /// <param name="sCamName">摄像机的名称</param>
        /// <param name="sUser">用户名</param>
        /// <param name="sPass">密码</param>
        /// <param name="sType">连接类型 1.自己 2.海康</param>
        public void ConnectVideo(string sIp, string sId, string sCamName, string sUser,
            string sPass, string sType)
        {
            int iType = Convert.ToInt32(sType);
            int iChannel = -1;
            switch (iType)
            {
                case 1://自己的api
                    iChannel = Convert.ToInt32(sId.Substring(sId.Length - 1, 1));
                    ucVGSHow.ConnectVideo(sId, sIp, sCamName, iType, iChannel);
                    break;
                case 2://海康的api
                    if (m_lstLoginInfo.Count > 0)
                    {
                        iChannel = RetChannelWithIp(sIp); ;
                        for (int i = 0; i < m_lstLoginInfo.Count; i++)
                        {
                            if (m_lstLoginInfo[i].iHandle != -1)
                            {
                                ucVGSHow.ConnectVideo(sId, m_lstLoginInfo[i].sIp, sCamName, iType, iChannel, m_lstLoginInfo[i].iHandle);
                                break;
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 断开当前连接
        /// </summary>
        public void DisConnectVideo()
        {
            ucVGSHow.DisConnectVideo();
        }
        /// <summary>
        /// 断开所有连接视频
        /// </summary>
        public void DisConnectVideoAll()
        {
            ucVGSHow.DisConnectVideoAll();
        }
        /// <summary>
        /// 切屏，1,2,4,9,16
        /// </summary>
        /// <param name="iScreen"></param>
        public void ChangeScreen(int iScreen)
        {
            ucVGSHow.ChangeScreens(iScreen);
        }
        /// <summary>
        /// 轮询
        /// </summary>
        public void SwitchConnect(string CamInfo)
        {
            ucVGSHow.DisConnectVideoAll();
            //休眠两秒
            //System.Threading.Thread.Sleep(5000);
            ucVGSHow.setActiveWin(0);
            int iChannel = -1;
            var EpollArray = JArray.Parse(CamInfo);
            if (EpollArray != null)
            {
                ucVGSHow.ChangeScreens(1);
                if (EpollArray.Count == 1)
                {
                    int iType = Convert.ToInt32(EpollArray[0]["iType"].ToString());
                    switch (iType)
                    {
                        case 1: //自己api
                            iChannel = Convert.ToInt32(EpollArray[0]["lId"].ToString().Substring(EpollArray[0]["lId"].ToString().Length - 1, 1));
                            ucVGSHow.ConnectVideo(EpollArray[0]["lId"].ToString(), EpollArray[0]["sStreamIp"].ToString(),
                                EpollArray[0]["sCamName"].ToString(), iType, iChannel);
                            break;
                        case 2://海康api
                            iChannel = RetChannelWithIp(EpollArray[0]["sIp"].ToString());
                            for (int i = 0; i < m_lstLoginInfo.Count; i++)
                            {
                                if (m_lstLoginInfo[i].iHandle != -1 &&
                                    m_lstLoginInfo[i].sIp == EpollArray[0]["sStreamIp"].ToString())
                                {
                                    ucVGSHow.ConnectVideo(EpollArray[0]["lId"].ToString(), m_lstLoginInfo[i].sIp,
                                        EpollArray[0]["sCamName"].ToString(), iType, iChannel, m_lstLoginInfo[0].iHandle);
                                }
                            }
                            break;
                    }
                }
                else if (2 <= EpollArray.Count && EpollArray.Count <= 4)
                {
                    ucVGSHow.ChangeScreens(4);
                    EpollFunc(EpollArray);
                }
                else if (4 < EpollArray.Count && EpollArray.Count <= 9)
                {
                    ucVGSHow.ChangeScreens(9);
                    EpollFunc(EpollArray);
                }
                else if (9 < EpollArray.Count && EpollArray.Count <= 16)
                {
                    ucVGSHow.ChangeScreens(16);
                    EpollFunc(EpollArray);
                }
            }

        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void DelVideoAx()
        {
            VsClientAPI.VSSP_ClientCleanup();
        }

        #endregion




    }
}
