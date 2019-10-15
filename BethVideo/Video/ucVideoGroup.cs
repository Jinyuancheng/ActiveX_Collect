using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BethVideo
{
    public partial class ucVideoGroup : UserControl
    {
        private const int m_iMaxScreen = 2;         //最大屏幕数
        private int m_iActiveWin = -1;              //当前活动窗口
        //private int m_iMaxShowWin = -1;             //当前最大化显示窗口   
        private Int32 m_lLoginHandle;               //登录句柄
        private ucSingleVideo[] m_arrVideos = null;  //屏幕数组
        private fmFullscreen m_fmFullScreen = null;  //全屏显示窗口

        private int m_iType;                         //摄像机类型 1.自己api 2.海康api

        public ucVideoGroup()
        {
            InitializeComponent();
            m_lLoginHandle = -1;
            m_arrVideos = new ucSingleVideo[2];
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                m_arrVideos[i] = new ucSingleVideo();
                m_arrVideos[i].Parent = this;
                m_arrVideos[i].Visible = false;
                m_arrVideos[i].Tag = i;
                m_arrVideos[i].pnlShowVideo.Tag = i;
                m_arrVideos[i].pnlShowVideo.DoubleClick += new EventHandler(ucSingleVideo_DbClick);
                m_arrVideos[i].pnlShowVideo.MouseDown += new MouseEventHandler(pnlShowVideo_MouseDown);
            }
            ChangeScreens();
            m_fmFullScreen = new fmFullscreen();
            m_fmFullScreen.Visible = false;
        }

        #region [设置属性]
        /// <summary>
        /// 设置登录句柄（海康）
        /// </summary>
        /// <param name="_iHandle">登录句柄</param>
        public void SetLoginHandle(int _iHandle)
        {
            m_lLoginHandle = _iHandle;
        }
        #endregion

        #region [过程] [分屏切换处理]
        //设置屏幕数
        public void ChangeScreens()
        {
            int iWidth = (this.Width - 2) / 2;
            int iHeight = this.Height;
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                m_arrVideos[i].Width = iWidth;
                m_arrVideos[i].Height = iHeight - 1;
                m_arrVideos[i].Left = i * iWidth + i + 1;
                m_arrVideos[i].Top = 1;
                m_arrVideos[i].Visible = true;
            }
            pnlShape.Visible = false;

        }
        //重新设置窗体大小
        private void ucVideoGroup_Resize(object sender, EventArgs e)
        {
            ChangeScreens();
        }
        //窗口双击事件
        private void ucSingleVideo_DbClick(object sender, EventArgs e)
        {
            int iTag = Convert.ToInt32(((Panel)sender).Tag);
            if (!m_arrVideos[iTag].GetIsConnect())
            {
                return;
            }
            //pnlShape.Visible = false;
            m_fmFullScreen.Left = 0;
            m_fmFullScreen.Top = 0;
            m_fmFullScreen.Width = Screen.PrimaryScreen.Bounds.Width;
            m_fmFullScreen.Height = Screen.PrimaryScreen.Bounds.Height;
            m_fmFullScreen.Visible = true;
            m_fmFullScreen.ConnectVideo(m_lLoginHandle, m_arrVideos[iTag].GetLinkChannel(), m_iType,
                m_arrVideos[iTag].m_strCamId, m_arrVideos[iTag].m_strSvrIp, m_arrVideos[iTag].m_strName, iTag);
            setActiveWin(iTag);
        }
        //设置窗口为活动窗口
        private void setActiveWin(int win)
        {
            m_iActiveWin = win;
            pnlShape.Left = m_arrVideos[m_iActiveWin].Left - 1;
            pnlShape.Top = m_arrVideos[m_iActiveWin].Top - 1;
            pnlShape.Width = m_arrVideos[m_iActiveWin].Width + 2;
            pnlShape.Height = m_arrVideos[m_iActiveWin].Height + 2;
            pnlShape.SendToBack();
            pnlShape.Visible = true;
        }
        #endregion

        #region [过程] [连接/关闭视频事件]
        /// <summary>
        /// 根据编号获得窗口对象
        /// </summary>
        public ucSingleVideo GetSingleFormByNo(Int32 _iWinNo)
        {
            ucSingleVideo svideo = null;
            if (_iWinNo >= 0 && _iWinNo < m_iMaxScreen)
            {
                svideo = m_arrVideos[_iWinNo];
            }
            return svideo;
        }
        /// <summary>
        /// 连接视频
        /// </summary>
        /// <param name="iHandle">海康摄像机登录句柄</param>
        /// <param name="iType">摄像机类型 1.自己api 2.海康api</param>
        /// <param name="sCamName">摄像机名称</param>
        /// <param name="sId">登录摄像机的id</param>
        /// <param name="sIp">登陆摄像机的ip</param>
        /// <param name="iCamType">摄像机类型</param>
        /// <param name="iChannel">通道号</param>
        public void ConnectVideo(int iType, string sId, string sIp, string sCamName, int iChannel, int iCamType, int iHandle = -1)
        {
            m_iType = iType;
            switch (iType)
            {
                case 1://自己api
                    switch (iCamType)
                    {
                        case 1://红外摄像机 两个窗口
                            m_arrVideos[0].ConnectVideoSelf(sId, sIp, sCamName, 0);
                            m_arrVideos[0].ShowVideoCamNameSelf(true);
                            m_arrVideos[0].SetCamType(iType);
                            m_arrVideos[1].ConnectVideoSelf(sId, sIp, sCamName, 1);
                            m_arrVideos[1].ShowVideoCamNameSelf(true);
                            m_arrVideos[1].SetCamType(iType);
                            setActiveWin(0);
                            break;
                        default:
                            m_arrVideos[0].ConnectVideoSelf(sId, sIp, sCamName, 0);
                            m_arrVideos[0].ShowVideoCamNameSelf(true);
                            m_arrVideos[0].SetCamType(iType);
                            setActiveWin(0);
                            break;
                    }
                    break;
                case 2://海康api
                    if (iHandle < 0)
                    {
                        return;
                    }
                    switch (iCamType)
                    {
                        case 1://红外摄像机
                            m_lLoginHandle = iHandle;
                            m_arrVideos[0].ConnectVideo(m_lLoginHandle, iChannel);
                            m_arrVideos[0].ShowVideoCaption(true);
                            m_arrVideos[0].SetCamType(iType);
                            m_arrVideos[1].ConnectVideo(m_lLoginHandle, (iChannel + 1));
                            m_arrVideos[1].ShowVideoCaption(true);
                            m_arrVideos[1].SetCamType(iType);
                            setActiveWin(0);
                            break;
                        default:
                            m_lLoginHandle = iHandle;
                            m_arrVideos[0].ConnectVideo(m_lLoginHandle, iChannel);
                            m_arrVideos[0].ShowVideoCaption(true);
                            m_arrVideos[0].SetCamType(iType);
                            setActiveWin(0);
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 断开所有视频
        /// </summary>
        public void DisAllConnectVideo()
        {
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                if (m_arrVideos[i].m_bConnected)
                {
                    switch (m_arrVideos[i].m_iType)
                    {
                        case 1://自己api
                            m_arrVideos[i].DisConnectVideoSelf();
                            m_arrVideos[i].ShowVideoCamNameSelf(false);
                            break;
                        case 2://海康api
                            m_arrVideos[i].DisConnectVideo();
                            m_arrVideos[i].ShowVideoCaption(false);
                            break;
                    }
                }
            }
        }
        #endregion

        #region [过程] [窗口右键菜单事件]
        private void pnlShowVideo_MouseDown(object sender, MouseEventArgs e)
        {
            int iWinNo = Convert.ToInt32(((Panel)sender).Tag);
            if (e.Button == MouseButtons.Left)
            {
                setActiveWin(iWinNo);
            }
        }
        #endregion
    }
}
