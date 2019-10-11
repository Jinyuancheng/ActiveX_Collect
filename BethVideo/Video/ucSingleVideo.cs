using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BethVideo.Properties;
using IcomClient.API;
using PreviewDemo;

namespace BethVideo
{
    public partial class ucSingleVideo : UserControl
    {
        private Int32  m_iChannel;       //摄像机通道号
        private Int32  m_lPlayHandle;    //视频预览句柄
        public bool    m_bConnected;     //是否连接成功

        ///自己api使用
        public string m_strName;        //摄像机名称
        public string m_strSvrIp;       //摄像机ip
        public string m_strCamId;       //摄像机id
        public int     m_iType;          //摄像机调用api类型

        public ucSingleVideo()
        {
            InitializeComponent();
            m_lPlayHandle = -1;
            m_iChannel = 1;
            m_bConnected = false;
            pnlCaption.Visible = false;
            m_iType = -1;
        }
        /// <summary>
        /// 设置摄像机加载api类型
        /// </summary>
        /// <param name="iType">1.表示自己api 2.表示海康api</param>
        public void SetCamType(int iType)
        {
            m_iType = iType;
        }
        //获得连接摄像机通道
        public int GetLinkChannel()
        {
            return m_iChannel;
        }
        public bool GetIsConnect()
        {
            return m_bConnected;
        }
        #region [过程] [窗口关闭、单击、双击事件]
        //窗口关闭事件
        private void pbClose_MouseUp(object sender, MouseEventArgs e)
        {
            pbClose.Image = Resources.VideoClose_0;
        }
        private void pbClose_MouseDown(object sender, MouseEventArgs e)
        {
            pbClose.Image = Resources.VideoClose_1;
        }
        /// <summary>
        /// 关闭视频按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbClose_Click(object sender, EventArgs e)
        {
            switch (m_iType)
            {
                case 1://自己api
                    DisConnectVideoSelf();
                    ShowVideoCamNameSelf(false);
                    break;
                case 2://海康api
                    DisConnectVideo();
                    ShowVideoCaption(false);
                    break;
            }
        }
        #endregion

        #region [过程] [连接/关闭视频事件]
        public void ShowVideoCaption(bool bShow)
        {
            pnlCaption.Visible = bShow;
            if (bShow == true)
            {
                lblCamName.Text = "通道-" + Convert.ToString(m_iChannel);
            }
            else
            {
                lblCamName.Text = "";
            }
        }
        //连接视频
        public void ConnectVideo(Int32 lLoginHandle, int iChNo)
        {
            m_iChannel = iChNo;
            IntPtr pUser = new IntPtr();
            CHCNetSDK.NET_DVR_PREVIEWINFO struPlayInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            struPlayInfo.hPlayWnd = pnlShowVideo.Handle;         //需要SDK解码时句柄设为有效值，仅取流不解码时可设为空
            struPlayInfo.lChannel = iChNo;       //预览通道号
            struPlayInfo.dwStreamType = 0;       //0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            struPlayInfo.dwLinkMode = 0;       //0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP
            struPlayInfo.bBlocked = true;       //0- 非阻塞取流，1- 阻塞取流
            m_lPlayHandle = CHCNetSDK.NET_DVR_RealPlay_V40(lLoginHandle, ref struPlayInfo, null, pUser);
            if (m_lPlayHandle < 0)
            {
                return;
            }
            m_bConnected = true;
        }
        //断开视频
        public void DisConnectVideo()
        {
            if (m_lPlayHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lPlayHandle);
                m_lPlayHandle = -1;
            }
            m_bConnected = false;
        }
        #endregion

        #region 自己api的 连接/关闭 视频事件
        /// <summary>
        /// 是否显示摄像机信息
        /// </summary>
        /// <param name="bShow">true 显示 false 不显示</param>
        public void ShowVideoCamNameSelf(bool bShow)
        {
            pnlCaption.Visible = bShow;
            if (bShow == true)
            {
                lblCamName.Text = m_strName;
            }
            else
            {
                lblCamName.Text = "";
            }
        }

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
            sChConInfor.m_hVideo = pnlShowVideo.Handle;
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

    }
}
