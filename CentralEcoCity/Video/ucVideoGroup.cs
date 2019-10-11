using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IcomClient.Video;
using BethVideo;

namespace CentralEcoCity.Video
{
    public partial class ucVideoGroup : UserControl
    {
        private int m_iMaxScreen = 16;        //最大屏幕数
        private int m_iCurScreens = 0;        //当前屏幕数
        private int m_iActiveWin = -1;        //当前活动窗口
        private int m_iMaxShowWin = -1;       //当前最大化显示窗口
        private ucSingleVideo[] m_arrVideos = null;       //屏幕数组
        private List<ToolStripMenuItem> m_lstScreenMenu;          //分屏菜单
        private fm_FullScreen m_fmFullScreen;           //全屏显示窗口号

        //构造函数
        public ucVideoGroup()
        {
            InitializeComponent();
            m_arrVideos = new ucSingleVideo[m_iMaxScreen];
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                m_arrVideos[i] = new ucSingleVideo();
                m_arrVideos[i].Parent = this;
                m_arrVideos[i].Visible = false;
                m_arrVideos[i].Tag = i;
                m_arrVideos[i].pnlShowVideo.Tag = i;
                m_arrVideos[i].pnlShowVideo.DoubleClick += new EventHandler(ucSingleVideo_DbClick);
                m_arrVideos[i].pnlShowVideo.MouseDown += new MouseEventHandler(pnlShowVideo_MouseDown);

                m_arrVideos[i].ContextMenuStrip = cmsVideo;
            }
            ChangeScreens(16);
            m_lstScreenMenu = new List<ToolStripMenuItem>();
            m_lstScreenMenu.Add(tmsiSingleScreen);
            m_lstScreenMenu.Add(tmsiFourScreen);
            m_lstScreenMenu.Add(tmsiNineScreen);
            m_lstScreenMenu.Add(tmsiSixtheenScreen);
            m_fmFullScreen = new fm_FullScreen();
            m_fmFullScreen.Visible = false;
            Init();
        }
        public void Init()
        {
            tmsiFourScreen = new ToolStripMenuItem();
            for(int i = 0;i < m_arrVideos.Length;i++)
            {
                m_arrVideos[i].m_evntSetCapPath += new pSetCapPath(SetCapPathAll);
            }
        }
        /// <summary>
        /// 设置所有的摄像机抓拍路径
        /// </summary>
        /// <param name="_sCapPath">抓拍路径</param>
        /// <param name="bIsPath">是否拥有路径</param>
        public void SetCapPathAll(string _sCapPath, bool bIsPath)
        {
            for (int i = 0; i < m_arrVideos.Length; i++)
            {
                m_arrVideos[i].SetCapturePath(_sCapPath, bIsPath);
            }
        }

        #region [过程] [分屏切换处理]
        //设置屏幕数
        public void ChangeScreens(int iScreen)
        {
            m_iCurScreens = iScreen;
            for (int i = iScreen; i < m_iMaxScreen; i++)
            {
                m_arrVideos[i].Visible = false;
                //将切屏后的数据全部释放掉
                // m_arrVideos[i].DisConnectVideo();
            }
            int iRows = Convert.ToInt32(Math.Sqrt((double)iScreen));
            int iWidth = (this.Width - iRows) / iRows;
            int iHeight = (this.Height - iRows) / iRows;
            for (int i = 0; i < iRows; i++)
            {
                for (int j = 0; j < iRows; j++)
                {
                    m_arrVideos[i * iRows + j].Width = iWidth;
                    m_arrVideos[i * iRows + j].Height = iHeight;
                    m_arrVideos[i * iRows + j].Left = j * iWidth + j + 1;
                    m_arrVideos[i * iRows + j].Top = i * iHeight + i + 1;
                    m_arrVideos[i * iRows + j].Visible = true;
                }
            }
            pnlShape.Visible = false;
        }
        //重新设置窗体大小
        private void ucVideoGroup_Resize(object sender, EventArgs e)
        {
            if (m_iCurScreens != 0)
            {
                ChangeScreens(m_iCurScreens);
            }
        }
        //窗口双击事件
        private void ucSingleVideo_DbClick(object sender, EventArgs e)
        {
            int iTag = Convert.ToInt32(((Panel)sender).Tag);
            pnlShape.Visible = false;
            m_fmFullScreen.Left = 0;
            m_fmFullScreen.Top = 0;
            m_fmFullScreen.Width = Screen.PrimaryScreen.Bounds.Width;
            m_fmFullScreen.Height = Screen.PrimaryScreen.Bounds.Height;
            m_fmFullScreen.Visible = true;
            m_fmFullScreen.ConnectVideo(m_arrVideos[iTag].m_strCamId, m_arrVideos[iTag].m_strSvrIp,
                m_arrVideos[iTag].m_strName, m_arrVideos[iTag].m_iType, m_arrVideos[iTag].m_iChannel,
                m_arrVideos[iTag].m_HikLoginHandle);
            //m_fmFullScreen.LocationHandle(m_arrVideos[iTag].m_lHandle, m_fmFullScreen.Handle, m_arrVideos[iTag].m_lShowWndHandle);
            setActiveWin(iTag);
        }
        //分屏切换
        private void tmsiScreen_Click(object sender, EventArgs e)
        {
            int iScreen = Convert.ToInt32(((ToolStripMenuItem)sender).Tag);
            foreach (ToolStripMenuItem item in m_lstScreenMenu)
            {
                if (item != sender)
                {
                    item.Checked = false;
                }
                else
                {
                    item.Checked = true;
                }
            }
            ChangeScreens(iScreen);
            m_iMaxShowWin = -1;
        }
        //设置窗口为活动窗口
        public void setActiveWin(int win)
        {
            m_iActiveWin = win;//设置当前活动窗口是第几个
            pnlShape.Left = m_arrVideos[m_iActiveWin].Left - 1;
            pnlShape.Top = m_arrVideos[m_iActiveWin].Top - 1;
            pnlShape.Width = m_arrVideos[m_iActiveWin].Width + 2;
            pnlShape.Height = m_arrVideos[m_iActiveWin].Height + 2;
            pnlShape.SendToBack();
            pnlShape.Visible = true;
        }
        #endregion

        #region [窗口操作函数]
        /// <summary>
        /// 获取下一个空白窗口
        /// </summary>
        private ucSingleVideo GetNextNullWin()
        {
            int iNullNo = 0;
            if (m_iActiveWin == -1)
            {
                iNullNo = 0;
            }
            else
            {
                iNullNo = m_iActiveWin;
                if (iNullNo == m_iCurScreens)
                {
                    iNullNo = 0;
                }
            }
            for (int i = iNullNo; i < m_iCurScreens; i++)
            {
                if (m_arrVideos[i].GetLinkHandle() == -1)
                {
                    iNullNo = i;
                    break;
                }
            }
            setActiveWin(iNullNo);
            return m_arrVideos[iNullNo];
        }
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
        /// 根据连接句柄查找窗口对象
        /// </summary>
        public ucSingleVideo GetSingleFormByHandle(Int32 _lHandle)
        {
            ucSingleVideo svideo = null;
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                if (m_arrVideos[i].GetLinkHandle() == _lHandle)
                {
                    svideo = m_arrVideos[i];
                    break;
                }
            }
            return svideo;
        }
        /// <summary>
        /// 根据摄像机ID查找窗口
        /// </summary>
        /// <returns>成功返回摄像机对应的控件，否则返回null</returns>
        public ucSingleVideo GetSingleFormByCamId(string strId)
        {
            ucSingleVideo svideo = null;
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                if (m_arrVideos[i].GetLinkCamId() == strId)
                {
                    svideo = m_arrVideos[i];
                    break;
                }
            }
            return svideo;
        }
        /// <summary>
        /// 设置图片抓拍路径
        /// </summary>
        /// <param name="filename">图片的抓拍路径</param>
        /// <param name="bIsPath">设置有路径，不用用户进行设置</param>
        public void SetCapturePath(string filename, bool bIsPath)
        {
            for (int i = 0; i < m_arrVideos.Length; i++)
            {
                m_arrVideos[i].SetCapturePath(filename, bIsPath);
            }
        }


        #endregion

        #region [过程] [连接断开视频]
        /// <summary>
        /// 连接视频
        /// </summary>
        /// <param name="strId">连接视频的id</param>
        /// <param name="strIp">连接视频的ip</param>
        /// <param name="strName">摄像机名称</param>
        /// <param name="itype">api类型 1.自己api 2.海康api</param>
        /// <param name="iLoginHandle">海康摄像机的登录句柄</param>
        /// <param name="iChannel">播放通道（海康摄像机）</param>
        public void ConnectVideo(string strId, string strIp, string strName, int iType,
            int iChannel, int iLoginHandle = -1)
        {
            //ucSingleVideo sOldVideo = GetSingleFormByCamId(strId);
            ////说明没有找到id对应的窗口
            //if (sOldVideo == null)
            //{
            //返回窗口的信息(空白的窗口)
            ucSingleVideo svideo = GetNextNullWin();
            if (svideo != null)
            {
                switch (iType)
                {
                    case 1://自己api
                        svideo.ConnectVideo(strId, strIp, strName);
                        svideo.SetCamType(iType, iChannel);
                        svideo.ShowVideoCaption(true);
                        break;
                    case 2://海康api
                        svideo.ConnectVideoHik(iLoginHandle, iChannel, strName);
                        svideo.SetCamType(iType, iChannel);
                        svideo.ShowVideoCaption(true);
                        break;
                }
            }
            //}
            //else
            //{
            //    setActiveWin(Convert.ToInt32(sOldVideo.Tag));
            //}
        }
        /// <summary>
        /// 断开所有的连接视频
        /// </summary>
        public void DisConnectVideoAll()
        {
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                //自己api
                if (m_arrVideos[i].m_iType == 1)
                {
                    if (m_arrVideos[i].m_bConnected)
                    {
                        m_arrVideos[i].DisConnectVideo();
                        m_arrVideos[i].ShowVideoCaption(false);
                    }
                }
                //海康api
                else if (m_arrVideos[i].m_iType == 2)
                {
                    m_arrVideos[i].DisConnectVideoHik();
                    m_arrVideos[i].ShowVideoCaption(false);
                }

            }
        }
        /// <summary>
        /// 断开当前窗口的连接视频
        /// </summary>
        public void DisConnectVideo()
        {
            if (m_iActiveWin != -1)
            {
                //自己api
                if (m_arrVideos[m_iActiveWin].m_iType == 1)
                {
                    if (m_arrVideos[m_iActiveWin].m_bConnected)
                    {
                        m_arrVideos[m_iActiveWin].DisConnectVideo();
                        m_arrVideos[m_iActiveWin].ShowVideoCaption(false);
                    }
                }
                //海康api
                else if (m_arrVideos[m_iActiveWin].m_iType == 2)
                {
                    m_arrVideos[m_iActiveWin].DisConnectVideoHik();
                    m_arrVideos[m_iActiveWin].ShowVideoCaption(false);
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
            else
            {
                if (m_arrVideos[iWinNo].m_bConnected)
                {
                    tsmiDisConect.Visible = true;
                }
                else
                {
                    tsmiDisConect.Visible = false;
                }
            }
        }
        #endregion

        #region 未使用的函数
        /// <summary>
        /// 断开所有视频
        /// </summary>
        private void tsmiDisconectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_iMaxScreen; i++)
            {
                if (m_arrVideos[i].m_bConnected)
                {
                    m_arrVideos[i].DisConnectVideo();
                    m_arrVideos[i].ShowVideoCaption(false);
                }
            }
        }
        /// <summary>
        /// 断开当前窗口视频
        /// </summary>
        private void tsmiDisConect_Click(object sender, EventArgs e)
        {
            if (m_iActiveWin != -1)
            {
                if (m_arrVideos[m_iActiveWin].m_bConnected)
                {
                    m_arrVideos[m_iActiveWin].DisConnectVideo();
                    m_arrVideos[m_iActiveWin].ShowVideoCaption(false);
                }
            }
        }
        //获取视频参数
        public Int32 GetVideoParams(ref Int32 _iBri, ref Int32 _iCon, ref Int32 _iSat, ref Int32 _iHue)
        {
            if (m_iActiveWin == -1 || !m_arrVideos[m_iActiveWin].m_bConnected)
            {
                return -1;
            }
            return m_arrVideos[m_iActiveWin].GetVideoParams(ref _iBri, ref _iCon, ref _iSat, ref _iHue);
        }
        //设置视频参数
        public Int32 SetVideoParams(Int32 _iBri, Int32 _iCon, Int32 _iSat, Int32 _iHue)
        {
            if (m_iActiveWin == -1 || !m_arrVideos[m_iActiveWin].m_bConnected)
            {
                return -1;
            }
            return m_arrVideos[m_iActiveWin].SetVideoParams(_iBri, _iCon, _iSat, _iHue);
        }
        //普通PTZ控制
        public void PtzNormalCtrl(int iCmd, int iSpeed)
        {
            if (m_iActiveWin == -1 || !m_arrVideos[m_iActiveWin].m_bConnected)
            {
                return;
            }
            m_arrVideos[m_iActiveWin].PTZCtrl(iCmd, iSpeed, 1);
        }
        #endregion
    }
}
