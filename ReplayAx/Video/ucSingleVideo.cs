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
using CentralEcoCity.Video;
using IcomClient.Common;
using PreviewDemo;
using System.Runtime.InteropServices;
using ReplayAx.HikSDK;
using System.Threading;

namespace ReplayAx.Video
{
    public partial class ucSingleVideo : UserControl
    {
        public string m_strId;      //摄像机播放id
        public string m_strSvrIp;   //流媒体IP地址
        public Int32 m_lHandle;     //连接句柄
        private string m_CapPath;   //文件抓拍存储路径
        private fm_FullScreen m_fmFullScreen;

        //操作海康摄像机的属性
        private int m_iLoginHandle; //海康摄像机登录句柄
        private int m_iCamType;     //摄像机类型 1.自己api 2.海康api
        private int m_iPlayHandle;  //回放播放句柄
        private int m_iChannel;     //通道号
        private int m_lFindHandle;

        //播放时间
        private string m_sStartTime;//开始播放的时间
        private string m_sEndTime;  //结束播放的时间
        private string m_sPlayTime; //播放时间

        private bool m_bIsConnect;  //视频是否连接 true 连接 false未连接


        #region [公共函数]
        public ucSingleVideo()
        {
            InitializeComponent();
            Init();
            m_fmFullScreen = new fm_FullScreen();
        }
        /// <summary>
        /// 初始化属性信息
        /// </summary>
        public void Init()
        {
            m_strId = "";
            m_strSvrIp = "";
            m_lHandle = -1;
            m_CapPath = "";

            //海康属性 初始化
            m_iLoginHandle = -1;
            m_iCamType = -1;
            m_iPlayHandle = -1;
            m_iChannel = -1;
            m_lFindHandle = -1;

            //初始化播放时间
            m_sStartTime = "";
            m_sEndTime = "";
            m_sPlayTime = "";
            m_bIsConnect = false;
        }
        /// <summary>
        /// 设置播放时间
        /// </summary>
        /// <param name="_sStartTime">开始时间</param>
        /// <param name="_sEndTime">结束时间</param>
        /// <param name="_sPlayTime">播放时间</param>
        public void SetPalyTime(string _sStartTime, string _sEndTime, string _sPlayTime)
        {
            m_sStartTime = _sStartTime;
            m_sEndTime = _sEndTime;
            m_sPlayTime = _sPlayTime;
        }
        /// <summary>
        /// 设置摄像机类型
        /// </summary>
        /// <param name="_iCamType">摄像机api类型 1.自己api 2.海康api</param>
        public void SetCamType(int _iCamType)
        {
            m_iCamType = _iCamType;
        }
        /// <summary>
        /// 设置抓拍路径
        /// </summary>
        /// <param name="_sCapPath">抓拍路径</param>
        public void SetCapPath(string _sCapPath)
        {
            m_CapPath = _sCapPath;
        }
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="filePath">要创建文件夹的路径包含该文件夹</param>
        public void CreateDir(string filePath)
        {
            if (false == System.IO.Directory.Exists(filePath))
            {
                //创建pic文件夹
                System.IO.Directory.CreateDirectory(filePath);
            }
        }
        #endregion

        #region [自己API]  操作回放数据
        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="iHandle">操作句柄</param>
        /// <param name="bIsPlay">1表示暂停播放，0表示继续播放</param>
        public void PausePlay(uint bIsPlay)
        {
            VsClientAPI.VSSP_ClientPausePlay(m_lHandle, bIsPlay);
        }
        /// <summary>
        /// 快进播放
        /// </summary>
        public void FastPlay()
        {
            VsClientAPI.VSSP_ClientFastPlay(m_lHandle);
        }
        /// <summary>
        /// 慢进播放
        /// </summary>
        public void SlowPlay()
        {
            VsClientAPI.VSSP_ClientSlowPlay(m_lHandle);
        }
        /// <summary>
        /// 设置播放位置
        /// </summary>
        /// <param name="iPos">时间，定位时间</param>
        public void SetPlayPos(DefVsClient.PLAY_NETTIME oPosTime)
        {
            VsClientAPI.VSSP_ClientSetPlayPosByTime(m_lHandle, oPosTime);
        }
        /// <summary>
        /// 播放视频
        /// </summary>
        /// <param name="_pchUrl">摄像机ip</param>
        /// <param name="_startTime">开始时间</param>
        /// <param name="_endTime">结束时间</param>
        /// <param name="_playTime">播放时间</param>
        /// <param name="_pchCamId">摄像机id</param>
        public void StartPlay(byte[] _pchUrl, DefVsClient.PLAY_NETTIME _startTime,
            DefVsClient.PLAY_NETTIME _endTime, DefVsClient.PLAY_NETTIME _playTime, string _pchCamId)
        {
            m_strId = _pchCamId;
            m_strSvrIp = System.Text.Encoding.Default.GetString(_pchUrl);
            string sUser = "user";
            string sPass = "pass";
            byte[] byUser = System.Text.Encoding.Default.GetBytes(sUser);
            byte[] byPass = System.Text.Encoding.Default.GetBytes(sPass);
            m_lHandle = VsClientAPI.VSSP_ClientPlayByTime(_pchUrl, byUser, byPass, _startTime, _endTime, _playTime, _pchCamId, ucSinglePanle.Handle, 4002);
            m_bIsConnect = true;
        }
        /// <summary>
        /// 得到播放位置
        /// </summary>
        public int GetPlayPos()
        {
            return VsClientAPI.VSSP_ClientGetPlayPos(m_lHandle);
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
        /// 返回文件播放的时间
        /// </summary>
        /// <returns>返回文件播放时间</returns>
        public long GetFilePlayTime()
        {
            return VsClientAPI.VSSP_ClientGetPlayedTime(m_lHandle);
        }
        /// <summary>
        /// 停止播放
        /// </summary>
        public void StopPlay()
        {
            VsClientAPI.VSSP_ClientStopPlay(m_lHandle);
            m_strId = "";
            m_strSvrIp = "";
            m_lHandle = -1;
            m_CapPath = "";
        }
        /// <summary>
        /// 抓拍图片 
        /// </summary>
        /// <param name="_strFilePath">保存路径</param>
        /// <returns></returns>
        public Int32 CaptureBmp(string _strFilePath)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientBmpCap(m_lHandle, _strFilePath);
            }
            return iRet;
        }
        /// <summary>
        /// 设置图像的抓拍路径
        /// </summary>
        public void SetCapturePath(string _Path)
        {
            m_CapPath = _Path;
        }
        /// <summary>
        /// 返回文件播放时间(绝对时间,从开始播放当前时间开始)
        /// </summary>
        public uint GetFilePlayTimeByTime()
        {
            return VsClientAPI.VSSP_GetDownPlayTime(m_lHandle);
        }
        /// <summary>
        /// 抓拍图片
        /// </summary>
        public void CaptureBmpEx()
        {
            string sSelfCapPath = m_CapPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            FuncCommon.CreateMultiDir(sSelfCapPath);
            if (m_lHandle != -1)
            {
                CaptureBmp(sSelfCapPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp");
                MessageBox.Show("图片以保存", "提示");
            }
        }
        //设置是否显示窗口
        public void SetVisible(bool bIsShow)
        {
            this.Visible = bIsShow;
        }
        #endregion

        #region [海康API] 操作回放数据
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
        /// 播放海康的回放
        /// </summary>
        /// <param name="_iLoginHandle">登录句柄</param>
        /// <param name="_sEndTime">结束时间</param>
        /// <param name="_sStartTime">开始时间</param>
        public void StartPlayHik(int _iLoginHandle, string _sStartTime, string _sEndTime, int _iChannel)
        {
            if (!m_bIsConnect)
            {
                m_iChannel = _iChannel;
                m_iLoginHandle = _iLoginHandle;
                //录像回放的参数
                CHCNetSDK.NET_DVR_VOD_PARA oHikLoginInfo = new CHCNetSDK.NET_DVR_VOD_PARA();
                oHikLoginInfo.struIDInfo = new CHCNetSDK.NET_DVR_STREAM_INFO();
                oHikLoginInfo.dwSize = (uint)Marshal.SizeOf(oHikLoginInfo);     //结构体大小
                oHikLoginInfo.struIDInfo.dwChannel = (uint)m_iChannel;
                oHikLoginInfo.struBeginTime = GetTimeObjectHik(_sStartTime);    //开始播放的时间
                oHikLoginInfo.struEndTime = GetTimeObjectHik(_sEndTime);        //结束播放的时间
                oHikLoginInfo.hWnd = ucSinglePanle.Handle;                      //播放窗口句柄
                m_iPlayHandle = CHCNetSDK.NET_DVR_PlayBackByTime_V40(m_iLoginHandle, ref oHikLoginInfo);
                //CHCNetSDK.NET_DVR_TIME oStartTime = GetTimeObjectHik(_sStartTime);
                //CHCNetSDK.NET_DVR_TIME oEndTime = GetTimeObjectHik(_sEndTime);
                //CHCNetSDK.NET_DVR_TIME oStartTime = GetTimeObjectHik(_sStartTime);
                //CHCNetSDK.NET_DVR_TIME oEndTime = GetTimeObjectHik(_sEndTime);
                //m_iPlayHandle = CHCNetSDK.NET_DVR_PlayBackByTime(m_iLoginHandle, m_iChannel, ref oStartTime, ref oEndTime, ucSinglePanle.Handle);
                uint a = CHCNetSDK.NET_DVR_GetLastError();
                if (m_iPlayHandle != -1)
                {
                    //播放录像
                    PlayControlHik(EHikControlInfo.NET_DVR_PLAYSTART);
                    m_bIsConnect = true;
                }
            }
        }
        /// <summary>
        /// 停止海康的录像回放
        /// </summary>
        public void StopPlayHik()
        {
            if (m_iPlayHandle != -1)
            {
                CHCNetSDK.NET_DVR_StopPlayBack(m_iPlayHandle);
                m_bIsConnect = false;
            }
            m_iPlayHandle = -1;
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
            CHCNetSDK.NET_DVR_PlayBackControl_V40(m_iPlayHandle, (uint)_eHikControlInfo, pIn, pInLeng, pOut, ref pOutLeng);
        }
        /// <summary>
        /// 海康录像暂停播放
        /// </summary>
        /// <param name="_bIsPlay">true 表示暂停 false 表示继续播放</param>
        public void PasePlayHik(bool _bIsPlay)
        {
            if (_bIsPlay)
            {
                PlayControlHik(EHikControlInfo.NET_DVR_PLAYPAUSE);
            }
            else
            {
                PlayControlHik(EHikControlInfo.NET_DVR_PLAYRESTART);
            }
        }
        /// <summary>
        /// 海康录像快进播放
        /// </summary>
        public void FastPlayHik()
        {
            PlayControlHik(EHikControlInfo.NET_DVR_PLAYFAST);
        }
        /// <summary>
        /// 海康录像慢进播放
        /// </summary>
        public void SlowPlayHik()
        {
            PlayControlHik(EHikControlInfo.NET_DVR_PLAYSLOW);
        }
        /// <summary>
        /// 海康录像回复正常播放
        /// </summary>
        public void RecoverNormalPlayHik()
        {
            PlayControlHik(EHikControlInfo.NET_DVR_PLAYNORMAL);
        }
        /// <summary>
        /// 海康录像抓拍图片
        /// </summary>
        /// <param name="_sCapPath">抓拍图片存储路径</param>
        public void CaptureJpegHik()
        {
            string sHikCapPath = m_CapPath + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            FuncCommon.CreateMultiDir(sHikCapPath);
            if (m_iPlayHandle != -1)
            {
                CHCNetSDK.NET_DVR_PlayBackCaptureFile(m_iLoginHandle, sHikCapPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpeg");
                MessageBox.Show("图片以保存", "提示");
            }
        }
        /// <summary>
        /// 得到当前播放时间 
        /// </summary>
        /// <returns></returns>
        public uint GetCurrentPlayTime()
        {
            uint pOutLeng = 0;
            CHCNetSDK.NET_DVR_PlayBackControl(m_iPlayHandle, (uint)EHikControlInfo.NET_DVR_PLAYGETTIME, 0, ref pOutLeng);
            return pOutLeng;
        }
        /// <summary>
        /// 得到门禁的播放时间
        /// </summary>
        /// <returns>返回播放时间</returns>
        public string GetFilePlayTimeHik()
        {
            string sRetStr = null;
            CHCNetSDK.NET_DVR_FILECOND_V40 struFileCond_V40 = new CHCNetSDK.NET_DVR_FILECOND_V40();
            CHCNetSDK.NET_DVR_FINDDATA_V30 struFileData = new CHCNetSDK.NET_DVR_FINDDATA_V30();
            struFileCond_V40.lChannel = m_iChannel; //通道号 Channel number
            struFileCond_V40.dwFileType = 0xff; //0xff-全部，0-定时录像，1-移动侦测，2-报警触发，...
            struFileCond_V40.dwIsLocked = 0xff; //0-未锁定文件，1-锁定文件，0xff表示所有文件（包括锁定和未锁定）
            struFileCond_V40.struStartTime = GetTimeObjectHik(m_sStartTime);
            struFileCond_V40.struStopTime = GetTimeObjectHik(m_sEndTime);
            //开始录像文件查找 Start to search video files 
            m_lFindHandle = CHCNetSDK.NET_DVR_FindFile_V40(m_iLoginHandle, ref struFileCond_V40);
            /*\ 失败 /*/
            if(m_lFindHandle < 0)
            {
                return sRetStr;
            }
            while (true)
            {
                //逐个获取查找到的文件信息 Get file information one by one.
                int result = CHCNetSDK.NET_DVR_FindNextFile_V30(m_lFindHandle, ref struFileData);
                if (result == CHCNetSDK.NET_DVR_FILE_SUCCESS)
                {
                    DateTime dateTime = new DateTime((int)struFileData.struStartTime.dwYear,
                        (int)struFileData.struStartTime.dwMonth,
                        (int)struFileData.struStartTime.dwDay,
                        (int)struFileData.struStartTime.dwHour,
                        (int)struFileData.struStartTime.dwMinute,
                        (int)struFileData.struStartTime.dwSecond);
                    DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
                    return Convert.ToString((dateTime - DateStart).TotalSeconds);
                }
                /*\ 未找到 /*/
                else if (result == CHCNetSDK.NET_DVR_FILE_NOFIND || result == CHCNetSDK.NET_DVR_NOMOREFILE)
                {
                    return sRetStr;
                }
            }
        }
        /// <summary>
        /// 得到播放的总时间
        /// </summary>
        /// <returns>返回时间戳</returns>
        //public string GetTotalPlayTimeHik()
        //{
        //    string oTime = GetFilePlayTimeHik();
        //    return oTime;
        //}

        /// <summary>
        /// 设置海康录像播放进度 
        /// </summary>
        /// <param name="_iPlan">播放进度</param>
        public void SetPlayPlanHik(string _sPlayTime)
        {
            CHCNetSDK.NET_DVR_TIME _oPlanTime = GetTimeObjectHik(_sPlayTime);
            IntPtr pIn = Marshal.AllocHGlobal(Marshal.SizeOf(_oPlanTime));
            Marshal.StructureToPtr(_oPlanTime, pIn, true);
            uint pInLeng = (uint)Marshal.SizeOf(pIn);
            uint pOutLeng = 0;
            CHCNetSDK.NET_DVR_PlayBackControl_V40(m_iPlayHandle, (uint)EHikControlInfo.NET_DVR_PLAYSETTIME,
                pIn, pInLeng, IntPtr.Zero, ref pOutLeng);
        }
        /// <summary>
        /// 得到回放录像osd时间
        /// </summary>
        public CHCNetSDK.NET_DVR_TIME GetReplayOSDTime()
        {
            CHCNetSDK.NET_DVR_TIME oOSDTime = new CHCNetSDK.NET_DVR_TIME();
            CHCNetSDK.NET_DVR_GetPlayBackOsdTime(m_iLoginHandle, ref oOSDTime);
            return oOSDTime;
        }
        #endregion
        /// <summary>
        /// 双击放大视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ucSinglePanle_DoubleClick(object sender, EventArgs e)
        {
            int iTag = Convert.ToInt32(((Panel)sender).Tag);
            //this.Visible = false;
            m_fmFullScreen.Left = 0;
            m_fmFullScreen.Top = 0;
            m_fmFullScreen.Width = Screen.PrimaryScreen.Bounds.Width;
            m_fmFullScreen.Height = Screen.PrimaryScreen.Bounds.Height;
            if (m_bIsConnect)
            {
                switch (m_iCamType)
                {
                    case 1://自己api
                        m_fmFullScreen.Visible = true;
                        m_fmFullScreen.ChangeHandle(m_lHandle, ucSinglePanle.Handle, m_iCamType);
                        break;
                    case 2://海康api
                        m_fmFullScreen.Visible = true;
                        m_fmFullScreen.StartPlayHik(m_iLoginHandle, m_sStartTime, m_sEndTime,
                            m_iChannel, m_iCamType);
                        Thread.Sleep(500);
                        m_fmFullScreen.SetPlayPlanHik(GetReplayOSDTime());
                        break;
                }
            }
        }
    }
}
