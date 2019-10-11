using IcomClient.API;
using PreviewDemo;
using ReplayAx.HikSDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CentralEcoCity.Video
{
    public partial class fm_FullScreen : Form
    {
        private int m_iHandle;  //连接句柄
        private IntPtr m_pOldHandle;
        private int m_iLoginHandle;//登录句柄
        private int m_iChannel;    //通道号
        private int m_iType; //摄像机类型
        private CHCNetSDK.NET_DVR_TIME m_oPlayTime;//播放时间

        public fm_FullScreen()
        {
            InitializeComponent();
            m_pOldHandle = IntPtr.Zero;
            m_iLoginHandle = -1;
            m_iChannel = -1;
            m_iType = -1;
            m_iHandle = -1;
            m_oPlayTime = new CHCNetSDK.NET_DVR_TIME();
        }
        /// <summary>
        /// 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelFull_DoubleClick(object sender, EventArgs e)
        {
            if (m_iType != -1)
            {
                switch (m_iType)
                {
                    case 1://自己api
                        VsClientAPI.VSSP_ClientSetPlayHwnd(m_iHandle, m_pOldHandle);
                        this.Visible = false;
                        break;
                    case 2://海康api
                        StopPlayHik();
                        this.Visible = false;
                        break;
                }
            }
        }

        public void ChangeHandle(int iHandle, IntPtr Oldhandle, int iType)
        {
            m_iType = iType;
            this.m_iHandle = iHandle;
            this.m_pOldHandle = Oldhandle;
            VsClientAPI.VSSP_ClientSetPlayHwnd(m_iHandle, PanelFull.Handle);
        }

        #region 海康回放
        /// <summary>
        /// 返回海康时间结构体
        /// </summary>
        /// <param name="_sTime">要转换为结构体的时间字符串</param>
        /// <returns>返回一个CHCNetSDK.NET_DVR_TIME 对象</returns>
        public CHCNetSDK.NET_DVR_TIME GetTimeObjectHik(string _sTime)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            DateTime oStartTime = startTime.AddSeconds(Convert.ToDouble(_sTime));
            CHCNetSDK.NET_DVR_TIME oRetTime = new CHCNetSDK.NET_DVR_TIME();
            oRetTime.dwYear = (uint)oStartTime.Year;
            oRetTime.dwMonth = (uint)oStartTime.Month;
            oRetTime.dwDay = (uint)oStartTime.Day;
            oRetTime.dwHour = (uint)oStartTime.Hour;
            oRetTime.dwMinute = (uint)oStartTime.Minute;
            oRetTime.dwSecond = (uint)oStartTime.Second;
            return oRetTime;
        }
        /// <summary>
        /// 设置播放时间
        /// </summary>
        /// <param name="_sPlayTime">播放时间</param>
        public void SetPlayTime(CHCNetSDK.NET_DVR_TIME _oPlayTime)
        {
            m_oPlayTime = _oPlayTime;
        }
        /// <summary>
        /// 停止海康的录像回放
        /// </summary>
        public void StopPlayHik()
        {
            if (m_iHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopPlayBack(m_iHandle);
                m_pOldHandle = IntPtr.Zero;
                m_iLoginHandle = -1;
                m_iChannel = -1;
                m_iType = -1;
                m_iHandle = -1;
            }
        }
        /// <summary>
        /// 设置海康录像播放进度 
        /// </summary>
        /// <param name="_iPlan">播放进度</param>
        public void SetPlayPlanHik(CHCNetSDK.NET_DVR_TIME _oPlayTime)
        {
            IntPtr pIn = Marshal.AllocHGlobal(Marshal.SizeOf(_oPlayTime));
            Marshal.StructureToPtr(_oPlayTime, pIn, true);
            uint pInLeng = (uint)Marshal.SizeOf(pIn);
            uint pOutLeng = 0;
            if (m_iHandle >= 0)
            {
                CHCNetSDK.NET_DVR_PlayBackControl_V40(m_iHandle, (uint)EHikControlInfo.NET_DVR_PLAYSETTIME,
                    pIn, pInLeng, IntPtr.Zero, ref pOutLeng);
            }
        }

        /// <summary>
        /// 播放海康的回放
        /// </summary>
        /// <param name="_iLoginHandle">登录句柄</param>
        /// <param name="_sEndTime">结束时间</param>
        /// <param name="_sStartTime">开始时间</param>
        /// <param name="iType">摄像机类型1.自己 2.海康</param>
        /// <param name="_iChannel">通道号</param>
        public void StartPlayHik(int _iLoginHandle, string _sStartTime, string _sEndTime,
            int _iChannel, int iType)
        {
            m_iType = iType;
            m_iChannel = _iChannel;
            m_iLoginHandle = _iLoginHandle;
            //录像回放的参数
            CHCNetSDK.NET_DVR_VOD_PARA oHikLoginInfo = new CHCNetSDK.NET_DVR_VOD_PARA();
            oHikLoginInfo.struIDInfo = new CHCNetSDK.NET_DVR_STREAM_INFO();
            oHikLoginInfo.dwSize = (uint)Marshal.SizeOf(oHikLoginInfo);     //结构体大小
            oHikLoginInfo.struIDInfo.dwChannel = (uint)m_iChannel;
            oHikLoginInfo.struBeginTime = GetTimeObjectHik(_sStartTime);    //开始播放的时间
            oHikLoginInfo.struEndTime = GetTimeObjectHik(_sEndTime);        //结束播放的时间
            oHikLoginInfo.hWnd = PanelFull.Handle;                      //播放窗口句柄
            m_iHandle = CHCNetSDK.NET_DVR_PlayBackByTime_V40(m_iLoginHandle, ref oHikLoginInfo);
            if (m_iHandle != -1)
            {
                //播放录像
                PlayControlHik(EHikControlInfo.NET_DVR_PLAYSTART);
            }
        }
        /// <summary>
        /// 海康录像回放的播放控制
        /// </summary>
        public void PlayControlHik(EHikControlInfo _eHikControlInfo)
        {
            IntPtr pIn = new IntPtr();
            IntPtr pOut = new IntPtr();
            uint pInLeng = (uint)Marshal.SizeOf(pIn);
            uint pOutLeng = (uint)Marshal.SizeOf(pOut);
            CHCNetSDK.NET_DVR_PlayBackControl_V40(m_iHandle, (uint)_eHikControlInfo, pIn, pInLeng, pOut, ref pOutLeng);
        }
        #endregion
    }
}
