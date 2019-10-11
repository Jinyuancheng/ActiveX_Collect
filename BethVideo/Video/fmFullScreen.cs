using IcomClient.API;
using PreviewDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BethVideo
{
    public partial class fmFullscreen : Form
    {
        private Int32 m_lPlayHandle;   //视频预览句柄

        private int m_iType;    //摄像机调用api类型 1.自己api 2.海康api
        private string m_strName; //摄像机名称
        private string m_strCamId;//摄像机id
        private string m_strSvrIp;//摄像机ip
        private bool m_bConnected;//是否连接视频 true连接

        public fmFullscreen()
        {
            InitializeComponent();
            m_iType = -1;
            m_strName = "";
            m_strCamId = "";
            m_strSvrIp = "";
            m_bConnected = false;
        }

        //连接视频
        public void ConnectVideo(Int32 lLoginHandle, int iChNo, int iType, string sId, string sIp, string sName)
        {
            m_iType = iType;
            switch (m_iType)
            {
                case 1://自己api
                    ConnectVideoSelf(sId, sIp, sName);
                    break;
                case 2://海康api
                    IntPtr pUser = new IntPtr();
                    CHCNetSDK.NET_DVR_PREVIEWINFO struPlayInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                    struPlayInfo.hPlayWnd = panelFull.Handle;         //需要SDK解码时句柄设为有效值，仅取流不解码时可设为空
                    struPlayInfo.lChannel = iChNo;       //预览通道号
                    struPlayInfo.dwStreamType = 0;       //0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    struPlayInfo.dwLinkMode = 0;       //0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP
                    struPlayInfo.bBlocked = true;       //0- 非阻塞取流，1- 阻塞取流
                    m_lPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(lLoginHandle, ref struPlayInfo, null, pUser);
                    if (m_lPlayHandle < 0)
                    {
                        return;
                    }
                    break;
            }
        }
        //断开视频
        public void DisConnectVideo()
        {
            if (m_lPlayHandle >= 0)
            {
                switch (m_iType)
                {
                    case 1://自己api
                        DisConnectVideoSelf();
                        break;
                    case 2://海康api
                        CHCNetSDK.NET_DVR_StopRealPlay(m_lPlayHandle);
                        m_lPlayHandle = -1;
                        break;
                }

            }
        }

        #region 自己api的 连接/关闭 视频事件
        private DefVsClient.CHANNEL_CONINFOR GetChConInfor()
        {
            DefVsClient.CHANNEL_CONINFOR sChConInfor = new DefVsClient.CHANNEL_CONINFOR();
            sChConInfor.m_sNetLinkInfo.m_pchFromID = Encoding.Default.GetBytes("900000002001");
            sChConInfor.m_sNetLinkInfo.m_pchToID = Encoding.Default.GetBytes(m_strCamId);

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
            temp = Encoding.Default.GetBytes(m_strSvrIp);
            Array.Copy(temp, bUrl, temp.Length);
            sChConInfor.m_sNetLinkInfo.m_pchUrl = bUrl;
            sChConInfor.m_sNetLinkInfo.m_iPort = 3000;
            sChConInfor.m_iType = 0x0201018;
            sChConInfor.m_hVideo = panelFull.Handle;
            sChConInfor.m_iChNo = 0;
            sChConInfor.m_iStreamType = 0;
            sChConInfor.m_iBuffNum = 50;
            return sChConInfor;
        }
        /// <summary>
        /// 自己api连接视频
        /// </summary>
        /// <param name="strId">登录摄像机id</param>
        /// <param name="strIp">登录摄像机ip</param>
        /// <param name="strName">登录摄像机名称</param>
        public void ConnectVideoSelf(string strId, string strIp, string strName)
        {
            m_strName = strName;
            m_strCamId = strId;
            m_strSvrIp = strIp;
            DefVsClient.CHANNEL_CONINFOR sChConInfor = GetChConInfor();
            m_lPlayHandle = VsClientAPI.VSSP_ClientStart(ref sChConInfor);
            if (m_lPlayHandle >= 0)
            {
                m_bConnected = true;
            }
        }
        //断开视频
        public void DisConnectVideoSelf()
        {
            if (m_lPlayHandle != -1)
            {
                //StopRecFile();
                VsClientAPI.VSSP_ClientStop(m_lPlayHandle);
                //pnlShowVideo.Refresh();
                //pnlShowVideo.BackgroundImage = Resources.VideoBack;
                m_bConnected = false;
                m_lPlayHandle = -1;
                m_strCamId = "";
                m_strName = "";
                m_strSvrIp = "";
            }
        }
        #endregion

        private void panelFull_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = false;
            DisConnectVideo();
        }
    }
}
