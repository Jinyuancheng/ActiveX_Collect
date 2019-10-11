using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IcomClient.API;

namespace IcomClient.Video
{
    public class ChannelTvlink
    {
        private Int32 m_lHandle;   //连接句柄
        private DefVsClient.CHANNEL_CONINFOR m_sChConInfor;

        public string m_strDevId = ""; //设备ID
        public string m_strMMIp = ""; //流媒体IP地址
        public bool m_bConnected;  //是否连接成功
        public bool m_bRecording;  //是否正在录像
        public bool m_bPtzAuto = false;  //云台是否自转
        public bool m_bPtzLight = false; //云台灯光是否开启
        public uint m_iVolume = 100;///视频音量

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChannelTvlink(string strId,string strIp)
        {
            m_lHandle = -1;
            m_bRecording = false;
            m_bConnected = false;
            m_strDevId = strId;
            m_strMMIp = strIp;
        }
        /// <summary>
        /// 连接视频
        /// </summary>
        public Int32 ConnectVideo(IntPtr _hWnd)
        {
            m_lHandle = -1;
            m_bRecording = false;
            m_bConnected = false;
            m_sChConInfor = new DefVsClient.CHANNEL_CONINFOR();
            m_sChConInfor.m_sNetLinkInfo.m_pchFromID = Encoding.Default.GetBytes("900000000001");
            m_sChConInfor.m_sNetLinkInfo.m_pchToID = Encoding.Default.GetBytes(m_strDevId + "000");

            byte[] temp = null;
            byte[] bUser = new byte[12];
            Array.Clear(bUser, 0, 12);
            temp = Encoding.Default.GetBytes("888888");
            Array.Copy(temp, bUser, temp.Length);
            m_sChConInfor.m_sNetLinkInfo.m_pchUser = bUser;
            byte[] bPass = new byte[12];
            Array.Clear(bPass, 0, 12);
            temp = Encoding.Default.GetBytes("888888");
            Array.Copy(temp, bPass, temp.Length);
            m_sChConInfor.m_sNetLinkInfo.m_pchPsw = bPass;
            m_sChConInfor.m_sNetLinkInfo.m_iTranType = 1;
            byte[] bUrl = new byte[20];
            Array.Clear(bUrl, 0, 20);
            temp = Encoding.Default.GetBytes(m_strMMIp);
            Array.Copy(temp, bUrl, temp.Length);
            m_sChConInfor.m_sNetLinkInfo.m_pchUrl = bUrl;
            m_sChConInfor.m_sNetLinkInfo.m_iPort = 3000;
            m_sChConInfor.m_iType = 0x0201018;
            m_sChConInfor.m_hVideo = _hWnd;
            m_sChConInfor.m_iChNo = 0;
            m_sChConInfor.m_iBuffNum = 50;
            m_lHandle = VsClientAPI.VSSP_ClientStart(ref m_sChConInfor);
            CloseAudio();
            return m_lHandle;
        }
        /// <summary>
        /// 断开视频
        /// </summary>
        public Int32 DisConnectVideo()
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                if (m_bRecording == true)
                {
                    StopRecFile();
                }
                iRet = VsClientAPI.VSSP_ClientStop(m_lHandle);
                m_bConnected = false;
                m_lHandle = -1;
                m_strDevId = "";
            }
            return iRet;
        }
        /// <summary>
        /// 更改视频主显示区
        /// </summary>
        public Int32 ShowVideo(IntPtr _hWnd)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientSetWnd(m_lHandle, _hWnd, true);
            }
            return iRet;
        }
        /// <summary>
        /// 在子显示区显示视频（支持两个显示区）
        /// </summary>
        public Int32 ShowVideoEx(IntPtr _hWnd)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientSetWnd(m_lHandle, _hWnd, false);
            }
            return iRet;
        }
        /// <summary>
        /// 获取视频参数
        /// </summary>
        public Int32 GetVideoParams(ref Int32 _iBri, ref Int32 _iCon, ref Int32 _iSat, ref Int32 _iHue, bool _bRead)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                DefVsClient.PARAM_SET_DATA ParamSetData = new DefVsClient.PARAM_SET_DATA();
                ParamSetData.m_sNetLinkInfo = m_sChConInfor.m_sNetLinkInfo;
                ParamSetData.m_iChNo = m_sChConInfor.m_iChNo;
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
        /// <summary>
        /// 设置视频参数
        /// </summary>
        public Int32 SetVideoParams(Int32 _iBri, Int32 _iCon, Int32 _iSat, Int32 _iHue)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                DefVsClient.PARAM_SET_DATA ParamSetData = new DefVsClient.PARAM_SET_DATA();
                ParamSetData.m_sNetLinkInfo = m_sChConInfor.m_sNetLinkInfo;
                ParamSetData.m_iChNo = m_sChConInfor.m_iChNo;
                ParamSetData.m_iCommand = (Int32)ParamCmdType.cmdViParam;
                ParamSetData.m_iPriority = 1;
                //DefVsClient.CHANNVIPARAM ChannViParam = new DefVsClient.CHANNVIPARAM();
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

        /// <summary>
        /// 打开音频
        /// </summary>
        public Int32 OpenAudio()
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                if (m_bConnected == true)
                {
                    iRet = VsClientAPI.VSSP_ClientPlayAudio(m_lHandle);
                    if (m_iVolume != 0)
                    {
                        iRet = VsClientAPI.VSSP_ClientAudioVolum(m_lHandle, m_iVolume);
                    }
                }
            }
            return iRet;
        }

        /// <summary>
        /// 关闭音频
        /// </summary>
        public Int32 CloseAudio()
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientStopAudio(m_lHandle);
            }
            return iRet;
        }
        /// <summary>
        /// 设置音量
        /// </summary>
        public Int32 SetAudioVolume(UInt32 _uVolume)
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientAudioVolum(m_lHandle, _uVolume);
                m_iVolume = _uVolume;
            }
            return iRet;
        }
        /// <summary>
        /// 抓拍Bmp图片
        /// </summary>
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
        /// 开始录像
        /// </summary>
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

        /// <summary>
        /// 停止录像
        /// </summary>
        public Int32 StopRecFile()
        {
            Int32 iRet = -1;
            if (m_lHandle != -1)
            {
                iRet = VsClientAPI.VSSP_ClientStopRecord(m_lHandle);
                if (iRet == 0)
                {
                    m_bRecording = false;
                }
            }
            return iRet;
        }
        /// <summary>
        /// 控制前端设备
        /// </summary>
        /// <param name="_iCommond">控制命令</param>
        /// <param name="_iValue">速度值</param>
        /// <param name="_iPriority">控制优先级</param>
        /// <returns>成功返回 0 失败返回 -1</returns>
        public Int32 PTZCtrl(Int32 _iCommond, Int32 _iValue, Int32 _iPriority)
        {
            Int32 iRet = -1;
            DefVsClient.CHANNEL_CONINFOR sChConInfor = new DefVsClient.CHANNEL_CONINFOR();
            sChConInfor.m_sNetLinkInfo.m_pchFromID = Encoding.Default.GetBytes("900000000001");
            sChConInfor.m_sNetLinkInfo.m_pchToID = Encoding.Default.GetBytes(m_strDevId + "000");
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
            temp = Encoding.Default.GetBytes(m_strMMIp);
            Array.Copy(temp, bUrl, temp.Length);
            sChConInfor.m_sNetLinkInfo.m_pchUrl = bUrl;
            sChConInfor.m_sNetLinkInfo.m_iPort = 3004;
            sChConInfor.m_iType = 0x0201018;
            sChConInfor.m_iChNo = 0;
            sChConInfor.m_iBuffNum = 50;

            iRet = VsClientAPI.VSSP_ClientPTZCtrl(ref sChConInfor.m_sNetLinkInfo, sChConInfor.m_iChNo, _iCommond, _iValue, _iPriority, /*m_iExclusivedTime*/10);
            return iRet;
        }
        /// <summary>
        /// 获取连接句柄
        /// </summary>
        public Int32 GetConHandle()
        {
            return m_lHandle;
        }


    }
}
