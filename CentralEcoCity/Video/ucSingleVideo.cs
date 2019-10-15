using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IcomClient.Common;
using CentralEcoCity.Properties;
using IcomClient.API;
using BethVideo;
using PreviewDemo;
using CentralEcoCity.API;

namespace CentralEcoCity.Video
{
    public delegate void pSetCapPath(string _sFilePath, bool _bIsTure);
    public partial class ucSingleVideo : UserControl
    {
        public ucSingleVideo()
        {
            InitializeComponent();
            Init();
        }

        public string m_strName;                   //摄像机名称
        public string m_strCamId;                  //是否显示了视频(用于直播)
        public string m_strSvrIp;                  //流媒体IP地址
        public Int32 m_lHandle;                    //连接句柄
        public bool m_bConnected;                  //是否连接成功
        public bool m_bRecording;                  //是否正在录像
        private string m_CapPath;                  //抓拍图片的路径
        public int m_HikLoginHandle;               //海康摄像机的登录句柄
        public int m_iType;                        //调用api类型#30859A
        public int m_iChannel;                     //海康摄像机播放通道号
        public bool m_bIsFirstFloder;              //是否是第一次选择文件夹
        public event pSetCapPath m_evntSetCapPath; //用来设置所有摄像机的抓拍路径
        public string m_strHikCamIp;               //海康摄像机ip 

        #region 初始化 成员变量等
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            m_lHandle = -1;
            m_strCamId = "";
            m_strSvrIp = "";
            m_bConnected = false;
            m_bRecording = false;
            pnlCaption.Visible = false;
            m_iType = -1;
            m_iChannel = -1;
            m_bIsFirstFloder = false;
            m_HikLoginHandle = -1;
            m_strHikCamIp = "";
        }
        #endregion

        #region [过程] [get set方法]
        /// <summary>
        /// 设置海康摄像机ip
        /// </summary>
        /// <param name="_strCamIp">设置海康摄像机ip</param>
        public void SetHikCamIp(string _strCamIp)
        {
            m_strHikCamIp = _strCamIp;
        }
        #endregion

        #region [过程] [连接/关闭视频事件]
        /// <summary>
        /// 设置摄像机类型
        /// </summary>
        /// <param name="iType">1.自己 2.海康</param>
        public void SetCamType(int iType, int iChannel)
        {
            m_iType = iType;
            m_iChannel = iChannel;
        }
        public void ShowVideoCaption(bool bShow)
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
        //获得当前视频ID
        public string GetLinkCamId()
        {
            return m_strCamId;
        }
        //获得当前连接句柄
        public Int32 GetLinkHandle()
        {
            return m_lHandle;
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
        //连接视频
        public void ConnectVideo(string strId, string strIp, string strName)
        {
            m_strName = strName;
            m_strCamId = strId;
            m_strSvrIp = strIp;
            DefVsClient.CHANNEL_CONINFOR sChConInfor = GetChConInfor();
            m_lHandle = VsClientAPI.VSSP_ClientStart(ref sChConInfor);
            if (m_lHandle >= 0)
            {
                m_bConnected = true;
            }
        }
        //断开视频
        public void DisConnectVideo()
        {
            if (m_lHandle != -1)
            {
                //StopRecFile();
                VsClientAPI.VSSP_ClientStop(m_lHandle);
                //pnlShowVideo.Refresh();
                //pnlShowVideo.BackgroundImage = Resources.VideoBack;
                m_lHandle = -1;
                m_strCamId = "";
                m_strSvrIp = "";
                m_bConnected = false;
                m_bRecording = false;
                pnlCaption.Visible = false;
                m_iType = -1;
                m_iChannel = -1;
                m_bIsFirstFloder = false;
                m_HikLoginHandle = -1;
                m_strHikCamIp = "";
            }
        }
        //开始录像
        public Int32 StartRecFile(string _strFilePath)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientStartRecord(m_lHandle, _strFilePath);
                if (iRet == 0)
                {
                    m_bRecording = true;
                }
            }
            return iRet;
        }
        //停止录像
        public Int32 StopRecFile()
        {
            Int32 iRet = -1;
            if (m_lHandle != -1 && m_bRecording == true)
            {
                iRet = VsClientAPI.VSSP_ClientStopRecord(m_lHandle);
                if (iRet == 0)
                {
                    m_bRecording = false;
                }
            }
            return iRet;
        }
        //抓拍图片
        public Int32 CaptureBmp(string _strFilePath)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientBmpCap(m_lHandle, _strFilePath);
            }
            return iRet;
        }

        #endregion

        #region 未使用的函数
        //获取视频参数
        public Int32 GetVideoParams(ref Int32 _iBri, ref Int32 _iCon, ref Int32 _iSat, ref Int32 _iHue)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                DefVsClient.PARAM_SET_DATA ParamSetData = new DefVsClient.PARAM_SET_DATA();
                DefVsClient.CHANNEL_CONINFOR sChConInfor = GetChConInfor();
                ParamSetData.m_sNetLinkInfo = sChConInfor.m_sNetLinkInfo;
                ParamSetData.m_iChNo = sChConInfor.m_iChNo;
                ParamSetData.m_iCommand = (Int32)ParamCmdType.cmdViParam;
                ParamSetData.m_iPriority = 1;
                iRet = VsClientAPI.VSSP_ClientGetDevParam(ref ParamSetData);
                if (iRet == 0)
                {
                    byte[] byteArray = ParamSetData.m_pchParams;
                    _iBri = byteArray[0];
                    _iCon = byteArray[1];
                    _iSat = byteArray[2];
                    _iHue = byteArray[3];
                }
                else
                {
                    _iBri = 0;
                    _iCon = 0;
                    _iSat = 0;
                    _iHue = 0;
                }
            }
            return iRet;
        }
        //设置视频参数
        public Int32 SetVideoParams(Int32 _iBri, Int32 _iCon, Int32 _iSat, Int32 _iHue)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                DefVsClient.PARAM_SET_DATA ParamSetData = new DefVsClient.PARAM_SET_DATA();
                DefVsClient.CHANNEL_CONINFOR sChConInfor = GetChConInfor();
                ParamSetData.m_sNetLinkInfo = sChConInfor.m_sNetLinkInfo;
                ParamSetData.m_iChNo = sChConInfor.m_iChNo;
                ParamSetData.m_iCommand = (Int32)ParamCmdType.cmdViParam;
                ParamSetData.m_iPriority = 1;
                byte[] byteArray = new byte[4096];
                byteArray[0] = (byte)_iBri;
                byteArray[1] = (byte)_iCon;
                byteArray[2] = (byte)_iSat;
                byteArray[3] = (byte)_iHue;
                ParamSetData.m_pchParams = byteArray;
                iRet = VsClientAPI.VSSP_ClientSetDevParam(ref ParamSetData);
            }
            return iRet;
        }
        // 控制前端设备
        public Int32 PTZCtrl(Int32 _iCommond, Int32 _iValue, Int32 _iPriority)
        {
            Int32 iRet = -1;
            DefVsClient.CHANNEL_CONINFOR sChConInfor = GetChConInfor();
            iRet = VsClientAPI.VSSP_ClientPTZCtrl(ref sChConInfor.m_sNetLinkInfo, sChConInfor.m_iChNo, _iCommond, _iValue, _iPriority, /*m_iExclusivedTime*/10);
            return iRet;
        }
        #endregion

        #region 自己api的操作
        /// <summary>
        ///设置图像的抓拍路径
        /// </summary>
        /// <param name="_Path">要设置的文件路径</param>
        /// <param name="_bIsPath">是否拥有路径</param>
        public void SetCapturePath(string _Path, bool _bIsPath)
        {
            m_CapPath = _Path;
            m_bIsFirstFloder = _bIsPath;
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
        //设置所有摄像机的抓拍路径
        public void SetCapPathAll()
        {
            if (m_evntSetCapPath != null)
            {
                m_evntSetCapPath(m_CapPath, true);
            }
        }

        /// <summary>
        /// 抓拍按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbCapture_Click(object sender, EventArgs e)
        {
            //不是第一次选择文件夹
            if (m_bIsFirstFloder)
            {
                switch (m_iType)
                {
                    case 1: //自己api
                        string CapPathPath = m_CapPath + "\\" + m_strName + "\\";
                        CreateDir(CapPathPath);
                        string CapPathSelf = CapPathPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";
                        if (CaptureBmp(CapPathSelf) >= 0)
                        {
                            MessageBox.Show("图片已保存", "提示");
                        }
                        else
                        {
                            MessageBox.Show("图片保存失败", "提示");
                        }
                        break;
                    case 2: //海康api
                        CHCNetSDK.NET_DVR_JPEGPARA JpegInfo = new CHCNetSDK.NET_DVR_JPEGPARA();
                        JpegInfo.wPicSize = 3; //3-UXGA(1600*1200)， 
                        JpegInfo.wPicQuality = 2;//0-最好，1-较好，2-一般 
                        string CapPathHikPath = m_CapPath + "\\" + m_strName + "\\";
                        CreateDir(CapPathHikPath);
                        string CapPathHik = CapPathHikPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpeg";
                        if (CHCNetSDK.NET_DVR_CapturePicture(m_lHandle, CapPathHik))
                        {
                            CHCNetSDK.NET_DVR_GetLastError();
                            m_bIsFirstFloder = true;
                            MessageBox.Show("图片已保存", "提示");
                        }
                        else
                        {
                            MessageBox.Show("图片保存失败", "提示");
                        }
                        break;
                }

            }
            //第一次选择文件夹
            else
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择抓拍图片文件存储路径";
                switch (m_iType)
                {
                    case 1: //自己api
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            m_CapPath = dialog.SelectedPath;
                            //将路径写入到配置文件中
                            if (ReadIniAPI.SetValueWithTitleAndKey("PATH", "PATH.CapPicPath", m_CapPath))
                            {
                                SetCapPathAll();
                                MessageBox.Show("配置抓拍文件存储路径成功", "提示");
                            }
                        }
                        string CapPathPath = m_CapPath + "\\" + m_strName + "\\";
                        CreateDir(CapPathPath);
                        string CapPathSelf = CapPathPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bmp";
                        if (CaptureBmp(CapPathSelf) >= 0)
                        {
                            MessageBox.Show("图片已保存", "提示");
                        }
                        else
                        {
                            MessageBox.Show("图片保存失败", "提示");
                        }
                        break;
                    case 2: //海康api
                        CHCNetSDK.NET_DVR_JPEGPARA JpegInfo = new CHCNetSDK.NET_DVR_JPEGPARA();
                        JpegInfo.wPicSize = 3; //3-UXGA(1600*1200)， 
                        JpegInfo.wPicQuality = 2;//0-最好，1-较好，2-一般 
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            m_CapPath = dialog.SelectedPath;
                            SetCapPathAll();
                            string CapPath = m_CapPath.Replace(@"\", @"\\");
                            //将路径写入到配置文件中
                            if (ReadIniAPI.SetValueWithTitleAndKey("PATH", "PATH.CapPicPath", CapPath))
                            {
                                MessageBox.Show("配置抓拍文件存储路径成功", "提示");
                            }
                        }
                        string CapPathHikPath = m_CapPath + "\\" + m_strName + "\\";
                        CreateDir(CapPathHikPath);
                        string CapPathHik = CapPathHikPath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpeg";
                        if (CHCNetSDK.NET_DVR_CapturePicture(m_lHandle, CapPathHik))
                        {
                            CHCNetSDK.NET_DVR_GetLastError();
                            m_bIsFirstFloder = true;
                            MessageBox.Show("图片已保存", "提示");
                        }
                        else
                        {
                            MessageBox.Show("图片保存失败", "提示");
                        }
                        break;
                }
            }



        }
        //关闭视频按钮
        private void pbClose_Click(object sender, EventArgs e)
        {
            switch (m_iType)
            {
                case 1://自己的api
                    DisConnectVideo();
                    ShowVideoCaption(false);
                    break;
                case 2://海康api
                    DisConnectVideoHik();
                    ShowVideoCaption(false);
                    break;
            }

        }

        #endregion

        #region 外观按钮操作
        private void pbCapture_MouseDown(object sender, MouseEventArgs e)
        {
            this.pbCapture.Image = Resources.VideoCap_1;
        }

        private void pbCapture_MouseUp(object sender, MouseEventArgs e)
        {
            this.pbCapture.Image = Resources.VideoCap_0;
        }

        private void pbClose_MouseDown(object sender, MouseEventArgs e)
        {
            this.pbClose.Image = Resources.VideoClose_1;
        }

        private void pbClose_MouseUp(object sender, MouseEventArgs e)
        {
            this.pbClose.Image = Resources.VideoClose_0;
        }

        #endregion

        #region 海康视频的操作
        /// <summary>
        /// 连接海康摄像机视频直播
        /// </summary>
        /// <param name="_iHikLoginHandle">海康摄像机登录句柄</param>
        public void ConnectVideoHik(int _iHikLoginHandle, int iChannel, string strName)
        {
            m_strName = strName;
            m_HikLoginHandle = _iHikLoginHandle;
            IntPtr pUser = new IntPtr();
            CHCNetSDK.NET_DVR_PREVIEWINFO struPlayInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
            struPlayInfo.hPlayWnd = pnlShowVideo.Handle; //需要SDK解码时句柄设为有效值，仅取流不解码时可设为空
            struPlayInfo.lChannel = iChannel;       //预览通道号
            struPlayInfo.dwStreamType = 0;       //0-主码流，1-子码流，2-码流3，3-码流4，以此类推
            struPlayInfo.dwLinkMode = 0;       //0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP
            struPlayInfo.bBlocked = true;       //0- 非阻塞取流，1- 阻塞取流
            m_lHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_HikLoginHandle, ref struPlayInfo, null, pUser);
            if (m_lHandle >= 0)
            {
                m_bConnected = true;
            }
            uint a = CHCNetSDK.NET_DVR_GetLastError();
        }

        /// <summary>
        /// 断开海康摄像机视频直播
        /// </summary>
        public void DisConnectVideoHik()
        {
            if (m_lHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lHandle);
                m_lHandle = -1;
                m_strCamId = "";
                m_strSvrIp = "";
                m_bConnected = false;
                m_bRecording = false;
                pnlCaption.Visible = false;
                m_iType = -1;
                m_iChannel = -1;
                m_bIsFirstFloder = false;
                m_HikLoginHandle = -1;
                m_strHikCamIp = "";
            }
        }
        #endregion
    }
}
