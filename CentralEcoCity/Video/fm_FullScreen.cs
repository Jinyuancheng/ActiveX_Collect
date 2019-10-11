using IcomClient.API;
using PreviewDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CentralEcoCity.Video
{
    public partial class fm_FullScreen : Form
    {
        private int m_iPlayHandle;  //连接句柄
        private string m_sIp;       //ip
        private string m_sCamId;    //摄像机id
        private string m_sName;     //端口
        private IntPtr m_pPlayWnd;  //播放窗口（小的窗口）
        private int m_iType;        //播放类型 用来判断是哪个api
        public fm_FullScreen()
        {
            InitializeComponent();
            m_iPlayHandle = -1;
            m_sIp = "";
            m_sCamId = "";
            m_sName = "";
            m_iType = -1;
        }

        private DefVsClient.CHANNEL_CONINFOR GetChConInfor()
        {
            DefVsClient.CHANNEL_CONINFOR sChConInfor = new DefVsClient.CHANNEL_CONINFOR();
            sChConInfor.m_sNetLinkInfo.m_pchFromID = Encoding.Default.GetBytes("900000002001");
            sChConInfor.m_sNetLinkInfo.m_pchToID = Encoding.Default.GetBytes(m_sCamId);

            byte[] temp = null;
            byte[] bUser = new byte[12];
            Array.Clear(bUser, 0, 12);
            temp = Encoding.Default.GetBytes("888888");
            Array.Copy(temp, bUser, temp.Length);
            sChConInfor.m_sNetLinkInfo.m_pchUser = bUser;
            byte[] bPass = new byte[12];
            Array.Clear(bPass, 0, 12);
            temp = Encoding.Default.GetBytes("888888");
            Array.Copy(temp, bPass, temp.Length);
            sChConInfor.m_sNetLinkInfo.m_pchPsw = bPass;
            sChConInfor.m_sNetLinkInfo.m_iTranType = 1;
            byte[] bUrl = new byte[20];
            Array.Clear(bUrl, 0, 20);
            temp = Encoding.Default.GetBytes(m_sIp);
            Array.Copy(temp, bUrl, temp.Length);
            sChConInfor.m_sNetLinkInfo.m_pchUrl = bUrl;
            sChConInfor.m_sNetLinkInfo.m_iPort = 3000;
            sChConInfor.m_iType = 0x0201018;
            sChConInfor.m_hVideo = this.Handle;
            sChConInfor.m_iChNo = 0;
            sChConInfor.m_iStreamType = 0;
            sChConInfor.m_iBuffNum = 50;
            return sChConInfor;
        }

        /// <summary>
        /// 连接海康摄像机视频直播
        /// </summary>
        /// <param name="_iHikLoginHandle">海康摄像机登录句柄</param>
        public void ConnectVideoHik(int _iHikLoginHandle, int iChannel, string strName)
        {
            IntPtr pUser = new IntPtr();
            CHCNetSDK.NET_DVR_PREVIEWINFO struPlayInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            struPlayInfo.hPlayWnd = this.Handle; //需要SDK解码时句柄设为有效值，仅取流不解码时可设为空
            struPlayInfo.lChannel = iChannel;       //预览通道号
            struPlayInfo.dwStreamType = 0;       //0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            struPlayInfo.dwLinkMode = 0;       //0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP
            struPlayInfo.bBlocked = true;       //0- 非阻塞取流，1- 阻塞取流
            m_iPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(_iHikLoginHandle, ref struPlayInfo, null, pUser);
            uint a = CHCNetSDK.NET_DVR_GetLastError();
        }

        /// <summary>
        /// 断开海康摄像机视频直播
        /// </summary>
        public void DisConnectVideoHik()
        {
            if (m_iPlayHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_iPlayHandle);
                m_iPlayHandle = -1;
            }
        }


        //连接视频
        public void ConnectVideo(string strId, string strIp, string strName, int iType, int iChannel, int iLoginHandle = -1)
        {
            m_iType = iType;
            switch (iType)
            {
                case 1://自己api
                    m_sIp = strIp;
                    m_sCamId = strId;
                    m_sName = strName;
                    DefVsClient.CHANNEL_CONINFOR sChConInfor = GetChConInfor();
                    m_iPlayHandle = VsClientAPI.VSSP_ClientStart(ref sChConInfor);
                    break;
                case 2://海康api
                    ConnectVideoHik(iLoginHandle, iChannel, strName);
                    break;
            }
        }
        //断开视频
        public void DisConnectVideo()
        {
            if (m_iPlayHandle != -1)
            {
                VsClientAPI.VSSP_ClientStop(m_iPlayHandle);
            }
            m_iPlayHandle = -1;
        }
        /// <summary>
        ///重新定位播放窗口
        /// </summary>
        /// <param name="_lHandle">连接句柄</param>
        /// <param name="_Handle">显示窗口句柄</param>
        public void LocationHandle(int _lHandle, IntPtr _WndHandle, IntPtr _OldHandle)
        {
            m_iPlayHandle = _lHandle;
            this.m_pPlayWnd = _OldHandle;
            int ret = VsClientAPI.VSSP_ClientSetPlayHwnd(_lHandle, _WndHandle);
        }

        private void fm_FullScreen_DoubleClick(object sender, EventArgs e)
        {
            switch (m_iType)
            {
                case 1://自己的api
                    DisConnectVideo();
                    break;
                case 2://海康api
                    DisConnectVideoHik();
                    break;
            }
            //VsClientAPI.VSSP_ClientSetPlayHwnd(m_iPlayHandle, m_pPlayWnd);
            this.Visible = false;
        }
    }
}
