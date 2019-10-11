/**********************************************************************
* Copyright (C) 2010-2012 天津市天祥世联网络科技有限公司
* 文件名: VsClient.cs
* 创建者: 肖元
* 创建时间: 2011-08-30
* 描述: 天祥世联新协议SDK引出类
**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
/*********************************************************************/
using System.IO;//判断文件是否存在
using System.Runtime.InteropServices;
using IcomClient.Common; //使用Marshal
using BethVideo;

namespace IcomClient.API
{
    #region 定义委托类型
    //-----定义委托类型-----//
    /********************************************************/
    public delegate Int32 VSSP_CLIENTStartup(Int32 _iMsg, IntPtr _hMsg, VsClientMsgCB _pMsgCB);
    public delegate Int32 VSSP_CLIENTCleanup();
    public delegate Int32 VSSP_CLIENTWaitTime(Int32 _iWait, Int32 _iTry);
    public delegate bool VSSP_CLIENTReadMessage(ref Int32 _iHandle, StringBuilder _pchToId, StringBuilder _pchUrl, ref Int32 _iPort, ref Int32 _iChNo, ref UInt32 _wParam, ref UInt32 _lParam);
    public delegate Int32 VSSP_CLIENTStart(ref DefVsClient.CHANNEL_CONINFOR _pCHANNEL_CONINFOR);
    public delegate Int32 VSSP_CLIENTStop(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTSetWnd(Int32 _iHandle, IntPtr _hwnd, bool _bMain);
    public delegate Int32 VSSP_CLIENTGetVideoSize(Int32 _iHandle, ref Int32 _piWidth, ref Int32 _piHeight);
    public delegate Int32 VSSP_CLIENTJpegCap(ref DefVsClient.CAP_JPEG_DATA _sCAP_JPEG_DATA);
    public delegate Int32 VSSP_CLIENTBmpCap(Int32 _iHandle, string _pchFilePath);
    public delegate Int32 VSSP_CLIENTStartRecord(Int32 _iHandle, string _pchFilePath);
    public delegate Int32 VSSP_CLIENTStopRecord(Int32 _iHandle);
    /********************************************************/
    public delegate Int32 VSSP_CLIENTPTZCtrl(ref DefVsClient.NETLINK_INFO _sNETLINK_INFO, Int32 _iChNo, Int32 _iType, Int32 _iValue, Int32 _iPriority, Int32 _iDelayTime);
    public delegate Int32 VSSP_CLIENTPlayAudio(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTStopAudio(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTAudioVolum(Int32 _iHandle, UInt32 _iVolume);
    /********************************************************/
    public delegate Int32 VSSP_CLIENTPlayByName(string _pchUrl, string _pchUser, string _pchPsw, string _pchFile, IntPtr _hWnd, Int32 _iPort);   
    public delegate Int32 VSSP_CLIENTDownByName(string _pchUrl, string _pchUser, string _pchPsw, string _pchRemote, string _pchSave, Int32 _iPort);
    public delegate bool VSSP_CLIENTStopDownFile(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTGetDownFilePos(Int32 _iHandle);
    /********************************************************/
    public delegate Int32 VSSP_CLIENTOpenFile(string _pchFilePath, IntPtr _hWnd);
    public delegate Int32 VSSP_CLIENTPausePlay(Int32 _iHandle, UInt32 _uPause);
    public delegate Int32 VSSP_CLIENTStopPlay(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTFastPlay(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTSlowPlay(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTGetPlaySpeed(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTOneByOnePlay(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTFastBackPlay(Int32 _iHandle);
    public delegate UInt32 VSSP_CLIENTGetFileTime(Int32 _iHandle);
    public delegate UInt32 VSSP_CLIENTGetPlayedTime(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTSetPlayedTime(Int32 _iHandle, UInt32 _uTime);    
    public delegate Int32 VSSP_CLIENTGetPlayPos(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTSetPlayPos(Int32 _iHandle, UInt32 _uPos);   
    public delegate Int32 VSSP_CLIENTSetFileCutStart(Int32 _iHandle, string _pchFilePath);
    public delegate Int32 VSSP_CLIENTSetFileCutEnd(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTSetPlayHwnd(Int32 _iHandle, IntPtr _hWnd);
    public delegate Int32 VSSP_CLIENTGetPlayVideoSize(Int32 _iHandle, ref UInt32 _uWidth, ref UInt32 _uHeight);
    public delegate Int32 VSSP_CLIENTRegPlayMsg(Int32 _iHandle, IntPtr _hWnd, Int32 _iMsg);
    /********************************************************/
    public delegate Int32 VSSP_CLIENTSetDevParam(ref DefVsClient.PARAM_SET_DATA _sPARAM_SET_DATA);
    public delegate Int32 VSSP_CLIENTGetDevParam(ref DefVsClient.PARAM_SET_DATA _sPARAM_SET_DATA);
    /********************************************************/
    public delegate Int32 VSSP_CLIENTFreshWnd(Int32 _iHandle);
    public delegate Int32 VSSP_CLIENTSetId(string _pchCLIENTId);
    public delegate Int32 VSSP_CLIENTViewPlay(Int32 _iHandle, bool _bView);
    public delegate Int32 VSSP_CLIENTViewVideo(Int32 _iHandle, bool _bView);
    public delegate bool VSSP_CLIENTGetIfNeedSetWnd(Int32 _iHandle);
    public delegate bool VSSP_FREETempPlayer(Int32 _iHandle);
    /********************************************************/
    public delegate Int32 VSSP_STARTPresence(ref DefVsClient.NETLINK_INFO _sNETLINK_INFO, string _pchGroupId, VsServicesCB _pMsgCB);
    public delegate Int32 VSSP_STOPPresence(Int32 _iHandle);
    public delegate Int32 VSSP_GETHttpResponse(string _pchUrl, StringBuilder _pchBody, Int32 _iSize);   
    public delegate bool VSSP_DOWNloadHttpFile(string _pchUrl, string _pchLocal);
    public delegate bool VSSP_UPloadHttpFile(string _pchUrl, string _pchLocal);
    public delegate bool VSSP_SENDPresenceData(Int32 _lHandle, byte[] _pchData, Int32 _iSize);//参数二：StringBuilder _pchData
    /********************************************************/
    public delegate Int32 VSSP_CLIENTDownByTime(string _pchUrl, string _pchUser, string _pchPsw, DefVsClient.PLAY_NETTIME _startTime, 
        DefVsClient.PLAY_NETTIME _endTime, string _pchCamId, string _pchSave, Int32 _iPort);
    public delegate Int32 VSSP_CLIENTPlayByTime(byte[] _pchUrl, byte[] _pchUser, byte[] _pchPsw, DefVsClient.PLAY_NETTIME _startTime, 
        DefVsClient.PLAY_NETTIME _endTime, DefVsClient.PLAY_NETTIME _playTime, string _pchCamId, IntPtr _hWnd, Int32 _iPort);
    public delegate Int32 VSSP_CLIENTSetPlayPosByTime(Int32 _lHandle, DefVsClient.PLAY_NETTIME _startTime);
    public delegate Int32 VSSP_OPERRemoteRecFile(DefVsClient.OPER_REC_FILE _pOperFile);
    public delegate UInt32 VSSP_GETDownPlayTime(Int32 _lHandle);
    public delegate Int32 VSSP_CLIENTPlayByNameEx(string _pchUrl, string _pchUser, string _pchPsw, string _pchFile, IntPtr _hWnd, Int32 _iPort);
    public delegate Int32 VSSP_CLIENTDownByNameEx(string _pchUrl, string _pchUser, string _pchPsw, string _pchRemote, string _pchSave, Int32 _iPort);
    //*****陈文玮加(电子放大接口)*****//
    public delegate Int32 VSSP_CLIENTElectronAmplifier(IntPtr _hWnd, float _left, float _top, float _right, float _bottom, Int32 _flag);
    //陈文玮 20120608 add avi转换 begin
    //(启动avi转换)
    public delegate Int32 VSSP_STRATTransformAvi(string _pchFilePath,string _pchDestFilePath);
    //(停止avi转换)
    public delegate Int32 VSSP_STOPTransformAvi(Int32 _handle);
    public delegate void pGetPicCallBack(Int32 _lHandle, string _pchData, UInt32 _lGmtTime);

    public delegate Int32 DEL_VSSP_StartNotify(ref DefVsClient.NETLINK_INFO _pConInfor, string _pchGroupId, pNotifyCallBack _pCB);
    public delegate Int32 DEL_VSSP_GETVodSnap(string _strUrl, string _strUser, string _strPsw, DefVsClient.PLAY_NETTIME _startTime
    , DefVsClient.PLAY_NETTIME _endTime, string _strCamId, Int32 _iPort, Int32 _iVodSnapCount, string _pchVodSnapPath, pGetPicCallBack _pCB);
    public delegate Int32 DEL_VSSP_GetLocalSnap(string _pchLocalFile, string _pchSnapPath, DefVsClient.PLAY_NETTIME _startTime
              , DefVsClient.PLAY_NETTIME _endTime, Int32 _iSnapCount, pGetPicCallBack _pCb);
    public delegate Int32 DEL_VSSP_MultiVodOpen(string _pchUser, string _pchPsw, DefVsClient.PLAY_NETTIME _startTime, DefVsClient.PLAY_NETTIME _endTime
                                 , DefVsClient.PLAY_NETTIME _playTime, IntPtr _pParam, Int32 _iCount);
    public delegate Int32 DEL_VSSP_MultiVodClose(Int32 _lHandle);
    public delegate Int32 DEL_VSSP_MultiPause(Int32 _lHandle, bool _bPause);
    public delegate Int32 DEL_VSSP_MultiFast(Int32 _lHandle);
    public delegate Int32 DEL_VSSP_MultiSlow(Int32 _lHandle);
    public delegate Int32 DEL_VSSP_MultiOneByOne(Int32 _lHandle);
    public delegate Int32 DEL_VSSP_MultiSetPlayPosByTime(Int32 _lHandle, ref DefVsClient.PLAY_NETTIME _tStartTime);
    public delegate Int32 DEL_VSSP_MultiCloseSingle(Int32 _lHandle, Int32 _iID);
    public delegate UInt32 DEL_VSSP_MultiGetGmtTime(Int32 _lHandle);
    public delegate Int32 DEL_VSSP_MultiOpenAudio(Int32 _lHandle, Int32 _iID);
    public delegate Int32 DEL_VSSP_MultiCloseAudio(Int32 _lHandle, Int32 _iID);
    public delegate Int32 DEL_VSSP_MultiSetVolume(Int32 _lHandle, UInt32 _iVolume);
    public delegate Int32 DEL_VSSP_MultiShowVideo(Int32 _lHandle, bool _bShow);

    //end
    #endregion

    /// <summary>
    /// 天祥世联新协议SDK引出类
    /// </summary>
    public class VsClientAPI
    {
        #region 成员变量

        #region 声明委托并初始化
        //-----声明委托并初始化-----//
        /********************************************************/
        public static VSSP_CLIENTStartup VSSP_ClientStartup = null;
        public static VSSP_CLIENTCleanup VSSP_ClientCleanup = null;
        public static VSSP_CLIENTWaitTime VSSP_ClientWaitTime = null;
        public static VSSP_CLIENTReadMessage VSSP_ClientReadMessage = null;
        public static VSSP_CLIENTStart VSSP_ClientStart = null;
        public static VSSP_CLIENTStop VSSP_ClientStop = null;
        public static VSSP_CLIENTSetWnd VSSP_ClientSetWnd = null;
        public static VSSP_CLIENTGetVideoSize VSSP_ClientGetVideoSize = null;
        public static VSSP_CLIENTJpegCap VSSP_ClientJpegCap = null;
        public static VSSP_CLIENTBmpCap VSSP_ClientBmpCap = null;
        public static VSSP_CLIENTStartRecord VSSP_ClientStartRecord = null;
        public static VSSP_CLIENTStopRecord VSSP_ClientStopRecord = null;
        /********************************************************/
        public static VSSP_CLIENTPTZCtrl VSSP_ClientPTZCtrl = null;
        public static VSSP_CLIENTPlayAudio VSSP_ClientPlayAudio = null;
        public static VSSP_CLIENTStopAudio VSSP_ClientStopAudio = null;
        public static VSSP_CLIENTAudioVolum VSSP_ClientAudioVolum = null;
        /********************************************************/
        public static VSSP_CLIENTPlayByName VSSP_ClientPlayByName = null;
        public static VSSP_CLIENTDownByName VSSP_ClientDownByName = null;
        public static VSSP_CLIENTStopDownFile VSSP_ClientStopDownFile = null;
        public static VSSP_CLIENTGetDownFilePos VSSP_ClientGetDownFilePos = null;
        public static VSSP_CLIENTOpenFile VSSP_ClientOpenFile = null;
        public static VSSP_CLIENTPausePlay VSSP_ClientPausePlay = null;
        public static VSSP_CLIENTFastPlay VSSP_ClientFastPlay = null;
        public static VSSP_CLIENTSlowPlay VSSP_ClientSlowPlay = null;
        public static VSSP_CLIENTOneByOnePlay VSSP_ClientOneByOnePlay = null;
        public static VSSP_CLIENTFastBackPlay VSSP_ClientFastBackPlay = null;
        public static VSSP_CLIENTGetPlayPos VSSP_ClientGetPlayPos = null;
        public static VSSP_CLIENTSetPlayPos VSSP_ClientSetPlayPos = null;
        public static VSSP_CLIENTGetFileTime VSSP_ClientGetFileTime = null;
        public static VSSP_CLIENTGetPlayedTime VSSP_ClientGetPlayedTime = null;
        public static VSSP_CLIENTSetPlayedTime VSSP_ClientSetPlayedTime = null;
        public static VSSP_CLIENTSetFileCutStart VSSP_ClientSetFileCutStart = null;
        public static VSSP_CLIENTSetFileCutEnd VSSP_ClientSetFileCutEnd = null;
        /********************************************************/
        public static VSSP_CLIENTSetPlayHwnd VSSP_ClientSetPlayHwnd = null;
        public static VSSP_CLIENTGetPlayVideoSize VSSP_ClientGetPlayVideoSize = null;
        public static VSSP_CLIENTStopPlay VSSP_ClientStopPlay = null;
        public static VSSP_STARTPresence VSSP_StartPresence = null;
        public static VSSP_STOPPresence VSSP_StopPresence = null;
        public static VSSP_GETHttpResponse VSSP_GetHttpResponse = null;
        public static VSSP_CLIENTSetId VSSP_ClientSetId = null;
        public static VSSP_CLIENTRegPlayMsg VSSP_ClientRegPlayMsg = null;
        public static VSSP_CLIENTViewPlay VSSP_ClientViewPlay = null;
        public static VSSP_CLIENTViewVideo VSSP_ClientViewVideo = null;
        public static VSSP_CLIENTGetPlaySpeed VSSP_ClientGetPlaySpeed = null;
        public static VSSP_CLIENTGetIfNeedSetWnd VSSP_ClientGetIfNeedSetWnd = null;
        public static VSSP_FREETempPlayer VSSP_FreeTempPlayer = null;
        public static VSSP_DOWNloadHttpFile VSSP_DownloadHttpFile = null;
        public static VSSP_UPloadHttpFile VSSP_UploadHttpFile = null;
        public static VSSP_CLIENTSetDevParam VSSP_ClientSetDevParam = null;
        public static VSSP_CLIENTGetDevParam VSSP_ClientGetDevParam = null;
        public static VSSP_CLIENTFreshWnd VSSP_ClientFreshWnd = null;
        /********************************************************/
        public static VSSP_CLIENTDownByTime VSSP_ClientDownByTime = null;
        public static VSSP_CLIENTPlayByTime VSSP_ClientPlayByTime = null;
        public static VSSP_CLIENTSetPlayPosByTime VSSP_ClientSetPlayPosByTime = null;
        public static VSSP_OPERRemoteRecFile VSSP_OperRemoteRecFile = null;
        public static VSSP_GETDownPlayTime VSSP_GetDownPlayTime = null;
        public static VSSP_SENDPresenceData VSSP_SendPresenceData = null;
        public static VSSP_CLIENTPlayByNameEx VSSP_ClientPlayByNameEx = null;
        public static VSSP_CLIENTDownByNameEx VSSP_ClientDownByNameEx = null;
        //*****陈文玮加*****//
        public static VSSP_CLIENTElectronAmplifier VSSP_ClientElectronAmplifier = null;
        //陈文玮 add 20120608 avi转换库 begin
        //(启动avi转换)
        public static VSSP_STRATTransformAvi VSSP_StartTransformAvi = null;
        //(停止avi转换)
        public static VSSP_STOPTransformAvi VSSP_StopTransformAvi = null;
        //******************//
        public static DEL_VSSP_StartNotify VSSP_StartNotify = null;
        public static DEL_VSSP_GetLocalSnap VSSP_GetLocalSnap = null;
        public static DEL_VSSP_GETVodSnap VSSP_GetVodSnap = null;
        public static DEL_VSSP_MultiVodOpen VSSP_MultiVodOpen = null;
        public static DEL_VSSP_MultiVodClose VSSP_MultiVodClose = null;
        public static DEL_VSSP_MultiPause VSSP_MultiPause = null;
        public static DEL_VSSP_MultiFast VSSP_MultiFast = null;
        public static DEL_VSSP_MultiSlow VSSP_MultiSlow = null;
        public static DEL_VSSP_MultiOneByOne VSSP_MultiOneByOne = null;
        public static DEL_VSSP_MultiSetPlayPosByTime VSSP_MultiSetPlayPosByTime = null;
        public static DEL_VSSP_MultiCloseSingle VSSP_MultiCloseSingle = null;
        public static DEL_VSSP_MultiGetGmtTime VSSP_MultiGetGmtTime = null;

        public static DEL_VSSP_MultiOpenAudio VSSP_MultiOpenAudio = null;
        public static DEL_VSSP_MultiCloseAudio VSSP_MultiCloseAudio = null;
        public static DEL_VSSP_MultiSetVolume VSSP_MultiSetVolume = null;
        public static DEL_VSSP_MultiShowVideo VSSP_MultiShowVideo = null;
        #endregion

        /// <summary>
        /// 加载动态库的返回句柄
        /// </summary>
        public static Int32 m_hVsClient;
        #endregion

        #region 成员函数

        #region 加载动态库
        /// <summary>
        /// 加载动态库
        /// </summary>
        /// <param name="_strFilePath">动态库的完整路径</param>
        /// <returns>成功返回 true 失败返回 false</returns>
        public static bool LoadVsClientAPI(string _strFilePath)
        {
            if (m_hVsClient != 0)
            {
                return true;
            }
            if (File.Exists(_strFilePath) != true)
            {
                return false;
            }
            m_hVsClient = Win32API.LoadLibrary(_strFilePath);
            if (m_hVsClient <= 0)
            {
                return false;
            }

            #region 实例化委托并判null
            //-----实例化委托并判null-----//
            /********************************************************/
            VSSP_ClientStartup = (VSSP_CLIENTStartup)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStartup", 
                typeof(VSSP_CLIENTStartup));
            if (VSSP_ClientStartup == null) { return false; }
            VSSP_ClientCleanup = (VSSP_CLIENTCleanup)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientCleanup", 
                typeof(VSSP_CLIENTCleanup));
            if (VSSP_ClientCleanup == null) { return false; }
            VSSP_ClientWaitTime = (VSSP_CLIENTWaitTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientWaitTime", 
                typeof(VSSP_CLIENTWaitTime));
            if (VSSP_ClientWaitTime == null) { return false; }
            VSSP_ClientReadMessage = (VSSP_CLIENTReadMessage)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientReadMessage", 
                typeof(VSSP_CLIENTReadMessage));
            if (VSSP_ClientReadMessage == null) { return false; }
            VSSP_ClientStart = (VSSP_CLIENTStart)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStart", 
                typeof(VSSP_CLIENTStart));
            if (VSSP_ClientStart == null) { return false; }
            VSSP_ClientStop = (VSSP_CLIENTStop)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStop", 
                typeof(VSSP_CLIENTStop));
            if (VSSP_ClientStop == null) { return false; }           
            VSSP_ClientSetWnd = (VSSP_CLIENTSetWnd)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetWnd",
                typeof(VSSP_CLIENTSetWnd));
            if (VSSP_ClientSetWnd == null) { return false; }  
            VSSP_ClientGetVideoSize = (VSSP_CLIENTGetVideoSize)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetVideoSize",
                typeof(VSSP_CLIENTGetVideoSize));
            if (VSSP_ClientGetVideoSize == null) { return false; }  
            VSSP_ClientJpegCap = (VSSP_CLIENTJpegCap)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientJpegCap",
                typeof(VSSP_CLIENTJpegCap));
            if (VSSP_ClientJpegCap == null) { return false; }  
            VSSP_ClientBmpCap = (VSSP_CLIENTBmpCap)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientBmpCap",
                typeof(VSSP_CLIENTBmpCap));
            if (VSSP_ClientBmpCap == null) { return false; } 
            VSSP_ClientStartRecord = (VSSP_CLIENTStartRecord)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStartRecord",
                typeof(VSSP_CLIENTStartRecord));
            if (VSSP_ClientStartRecord == null) { return false; } 
            VSSP_ClientStopRecord = (VSSP_CLIENTStopRecord)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStopRecord",
                typeof(VSSP_CLIENTStopRecord));
            if (VSSP_ClientStopRecord == null) { return false; } 
            /********************************************************/                      
            VSSP_ClientPTZCtrl = (VSSP_CLIENTPTZCtrl)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientPTZCtrl", 
                typeof(VSSP_CLIENTPTZCtrl));
            if (VSSP_ClientPTZCtrl == null) { return false; }           
            VSSP_ClientPlayAudio = (VSSP_CLIENTPlayAudio)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientPlayAudio", 
                typeof(VSSP_CLIENTPlayAudio));
            if (VSSP_ClientPlayAudio == null) { return false; } 
            VSSP_ClientStopAudio = (VSSP_CLIENTStopAudio)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStopAudio",
                typeof(VSSP_CLIENTStopAudio));
            if (VSSP_ClientStopAudio == null) { return false; } 
            VSSP_ClientAudioVolum = (VSSP_CLIENTAudioVolum)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientAudioVolum", 
                typeof(VSSP_CLIENTAudioVolum));
            if (VSSP_ClientStopAudio == null) { return false; } 
            /********************************************************/
            VSSP_ClientFreshWnd = (VSSP_CLIENTFreshWnd)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientFreshWnd",
                    typeof(VSSP_CLIENTFreshWnd));
            if (VSSP_ClientFreshWnd == null) { return false; } 
            /********************************************************/
            VSSP_ClientPlayByName = (VSSP_CLIENTPlayByName)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientPlayByName", 
                typeof(VSSP_CLIENTPlayByName));
            if (VSSP_ClientPlayByName == null) { return false; } 
            VSSP_ClientDownByName = (VSSP_CLIENTDownByName)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientDownByName", 
                typeof(VSSP_CLIENTDownByName));
            if (VSSP_ClientDownByName == null) { return false; } 
            VSSP_ClientStopDownFile = (VSSP_CLIENTStopDownFile)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStopDownFile", 
                typeof(VSSP_CLIENTStopDownFile));
            if (VSSP_ClientStopDownFile == null) { return false; } 
            VSSP_ClientGetDownFilePos = (VSSP_CLIENTGetDownFilePos)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetDownFilePos",
                typeof(VSSP_CLIENTGetDownFilePos));
            if (VSSP_ClientGetDownFilePos == null) { return false; }
            VSSP_ClientOpenFile = (VSSP_CLIENTOpenFile)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientOpenFile", 
                typeof(VSSP_CLIENTOpenFile));
            if (VSSP_ClientOpenFile == null) { return false; }
            VSSP_ClientPausePlay = (VSSP_CLIENTPausePlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientPausePlay", 
                typeof(VSSP_CLIENTPausePlay));
            if (VSSP_ClientPausePlay == null) { return false; }
            VSSP_ClientFastPlay = (VSSP_CLIENTFastPlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientFastPlay", 
                typeof(VSSP_CLIENTFastPlay));
            if (VSSP_ClientFastPlay == null) { return false; }
            VSSP_ClientSlowPlay = (VSSP_CLIENTSlowPlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSlowPlay", 
                typeof(VSSP_CLIENTSlowPlay));
            if (VSSP_ClientSlowPlay == null) { return false; }
            VSSP_ClientOneByOnePlay = (VSSP_CLIENTOneByOnePlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientOneByOnePlay", 
                typeof(VSSP_CLIENTOneByOnePlay));
            if (VSSP_ClientOneByOnePlay == null) { return false; }
            VSSP_ClientFastBackPlay = (VSSP_CLIENTFastBackPlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientFastBackPlay", 
                typeof(VSSP_CLIENTFastBackPlay));
            if (VSSP_ClientFastBackPlay == null) { return false; }
            VSSP_ClientGetPlayPos = (VSSP_CLIENTGetPlayPos)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetPlayPos",
                typeof(VSSP_CLIENTGetPlayPos));
            if (VSSP_ClientGetPlayPos == null) { return false; }
            VSSP_ClientSetPlayPos = (VSSP_CLIENTSetPlayPos)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetPlayPos", 
                typeof(VSSP_CLIENTSetPlayPos));
            if (VSSP_ClientSetPlayPos == null) { return false; }
            VSSP_ClientGetFileTime = (VSSP_CLIENTGetFileTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetFileTime", 
                typeof(VSSP_CLIENTGetFileTime));
            if (VSSP_ClientGetFileTime == null) { return false; }
            VSSP_ClientGetPlayedTime = (VSSP_CLIENTGetPlayedTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetPlayedTime",
                typeof(VSSP_CLIENTGetPlayedTime));
            if (VSSP_ClientGetPlayedTime == null) { return false; }
            VSSP_ClientSetPlayedTime = (VSSP_CLIENTSetPlayedTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetPlayedTime",
                typeof(VSSP_CLIENTSetPlayedTime));
            if (VSSP_ClientSetPlayedTime == null) { return false; }
            VSSP_ClientSetFileCutStart = (VSSP_CLIENTSetFileCutStart)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetFileCutStart",
                typeof(VSSP_CLIENTSetFileCutStart));
            if (VSSP_ClientSetFileCutStart == null) { return false; }
            VSSP_ClientSetFileCutEnd = (VSSP_CLIENTSetFileCutEnd)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetFileCutEnd",
                typeof(VSSP_CLIENTSetFileCutEnd));
            if (VSSP_ClientSetFileCutEnd == null) { return false; }
            /********************************************************/
            VSSP_ClientSetPlayHwnd = (VSSP_CLIENTSetPlayHwnd)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetPlayHwnd",
                typeof(VSSP_CLIENTSetPlayHwnd));
            if (VSSP_ClientSetPlayHwnd == null) { return false; }
            VSSP_ClientGetPlayVideoSize = (VSSP_CLIENTGetPlayVideoSize)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetPlayVideoSize",
                typeof(VSSP_CLIENTGetPlayVideoSize));
            if (VSSP_ClientGetPlayVideoSize == null) { return false; }
            VSSP_ClientStopPlay = (VSSP_CLIENTStopPlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientStopPlay", 
                typeof(VSSP_CLIENTStopPlay));
            if (VSSP_ClientStopPlay == null) { return false; }
            VSSP_StartPresence = (VSSP_STARTPresence)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_StartPresece", 
                typeof(VSSP_STARTPresence));
            if (VSSP_StartPresence == null) { return false; }
            VSSP_StopPresence = (VSSP_STOPPresence)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_StopPresece", 
                typeof(VSSP_STOPPresence));
            if (VSSP_StopPresence == null) { return false; }
            VSSP_GetHttpResponse = (VSSP_GETHttpResponse)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_GetHttpResponse", 
                typeof(VSSP_GETHttpResponse));
            if (VSSP_GetHttpResponse == null) { return false; }
            VSSP_ClientSetId = (VSSP_CLIENTSetId)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetId", 
                typeof(VSSP_CLIENTSetId));
            if (VSSP_ClientSetId == null) { return false; }
            VSSP_ClientRegPlayMsg = (VSSP_CLIENTRegPlayMsg)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientRegPlayMsg", 
                typeof(VSSP_CLIENTRegPlayMsg));
            if (VSSP_ClientRegPlayMsg == null) { return false; }
            VSSP_ClientViewPlay = (VSSP_CLIENTViewPlay)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientViewPlay", 
                typeof(VSSP_CLIENTViewPlay));
            if (VSSP_ClientViewPlay == null) { return false; }
            VSSP_ClientViewVideo = (VSSP_CLIENTViewVideo)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientViewVideo", 
                typeof(VSSP_CLIENTViewVideo));
            if (VSSP_ClientViewVideo == null) { return false; }
            VSSP_ClientGetPlaySpeed = (VSSP_CLIENTGetPlaySpeed)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetPlaySpeed", 
                typeof(VSSP_CLIENTGetPlaySpeed));
            if (VSSP_ClientGetPlaySpeed == null) { return false; }
            VSSP_ClientGetIfNeedSetWnd = (VSSP_CLIENTGetIfNeedSetWnd)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetIfNeedSetWnd",
                typeof(VSSP_CLIENTGetIfNeedSetWnd));
            if (VSSP_ClientGetIfNeedSetWnd == null) { return false; }
            VSSP_FreeTempPlayer = (VSSP_FREETempPlayer)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_FreeTempPlayer", 
                typeof(VSSP_FREETempPlayer));
            if (VSSP_FreeTempPlayer == null) { return false; }
            VSSP_DownloadHttpFile = (VSSP_DOWNloadHttpFile)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_DownloadHttpFile", 
                typeof(VSSP_DOWNloadHttpFile));
            if (VSSP_DownloadHttpFile == null) { return false; }
            VSSP_UploadHttpFile = (VSSP_UPloadHttpFile)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_UploadHttpFile", 
                typeof(VSSP_UPloadHttpFile));
            if (VSSP_UploadHttpFile == null) { return false; }
            VSSP_ClientSetDevParam = (VSSP_CLIENTSetDevParam)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetDevParam", 
                typeof(VSSP_CLIENTSetDevParam));
            if (VSSP_ClientSetDevParam == null) { return false; }
            VSSP_ClientGetDevParam = (VSSP_CLIENTGetDevParam)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientGetDevParam",
                typeof(VSSP_CLIENTGetDevParam));
            if (VSSP_ClientGetDevParam == null) { return false; }
            VSSP_ClientDownByTime = (VSSP_CLIENTDownByTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientDownByTime",
                typeof(VSSP_CLIENTDownByTime));
            if (VSSP_ClientDownByTime == null){ return false; }
            VSSP_ClientPlayByTime = (VSSP_CLIENTPlayByTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientPlayByTime",
                typeof(VSSP_CLIENTPlayByTime));
            if (VSSP_ClientPlayByTime == null){ return false; }
            VSSP_ClientSetPlayPosByTime = (VSSP_CLIENTSetPlayPosByTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientSetPlayPosByTime",
                typeof(VSSP_CLIENTSetPlayPosByTime));
            if (VSSP_ClientSetPlayPosByTime == null){ return false; }
            VSSP_OperRemoteRecFile = (VSSP_OPERRemoteRecFile)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_OperRemoteRecFile",
                typeof(VSSP_OPERRemoteRecFile));
            if (VSSP_OperRemoteRecFile == null){ return false; }
            VSSP_GetDownPlayTime = (VSSP_GETDownPlayTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_GetDownPlayTime",
                typeof(VSSP_GETDownPlayTime));
            if (VSSP_OperRemoteRecFile == null){ return false; }
            VSSP_SendPresenceData = (VSSP_SENDPresenceData)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_SendPresenceData",
                typeof(VSSP_SENDPresenceData));
            if (VSSP_SendPresenceData == null) { return false; }
            VSSP_ClientPlayByNameEx = (VSSP_CLIENTPlayByNameEx)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientPlayByNameEx",
                typeof(VSSP_CLIENTPlayByNameEx));
            if (VSSP_ClientPlayByNameEx == null) { return false; }
            VSSP_ClientDownByNameEx = (VSSP_CLIENTDownByNameEx)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientDownByNameEx",
                typeof(VSSP_CLIENTDownByNameEx));
            if (VSSP_ClientDownByNameEx == null) { return false; }
            //*****陈文玮加（电子放大动态库）*****//
            VSSP_ClientElectronAmplifier = (VSSP_CLIENTElectronAmplifier)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_ClientElectronAmplifier",
                typeof(VSSP_CLIENTElectronAmplifier));
            if (VSSP_ClientElectronAmplifier == null)
            {
                return false;
            }
            //陈文玮 add avi转换 20120608 begin
            //(启动avi转换)
            VSSP_StartTransformAvi = (VSSP_STRATTransformAvi)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_StartTransformAvi", typeof(VSSP_STRATTransformAvi));
            if (VSSP_StartTransformAvi==null)
            {
                return false;
            }
            //(停止avi转换)
            VSSP_StopTransformAvi = (VSSP_STOPTransformAvi)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_StopTransformAvi", typeof(VSSP_STOPTransformAvi));
            if (VSSP_StopTransformAvi==null)
            {
                return false;
            }
            VSSP_StartNotify = (DEL_VSSP_StartNotify)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_StartNotify", typeof(DEL_VSSP_StartNotify));
            if (VSSP_StartNotify == null)
            {
                return false;
            }
            VSSP_GetVodSnap = (DEL_VSSP_GETVodSnap)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_GetVodSnap", typeof(DEL_VSSP_GETVodSnap));
            if (VSSP_GetVodSnap == null)
            {
                return false;
            }

            VSSP_GetLocalSnap = (DEL_VSSP_GetLocalSnap)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_GetLocalSnap", typeof(DEL_VSSP_GetLocalSnap));
            if (VSSP_GetLocalSnap == null)
            {
                return false;
            }
            VSSP_MultiVodOpen = (DEL_VSSP_MultiVodOpen)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiVodOpen", typeof(DEL_VSSP_MultiVodOpen));
            if (VSSP_MultiVodOpen == null)
            {
                return false;
            }
            VSSP_MultiVodClose = (DEL_VSSP_MultiVodClose)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiVodClose", typeof(DEL_VSSP_MultiVodClose));
            if (VSSP_MultiVodClose == null)
            {
                return false;
            }
            VSSP_MultiPause = (DEL_VSSP_MultiPause)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiPause", typeof(DEL_VSSP_MultiPause));
            if (VSSP_MultiPause == null)
            {
                return false;
            }
            VSSP_MultiFast = (DEL_VSSP_MultiFast)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiFast", typeof(DEL_VSSP_MultiFast));
            if (VSSP_MultiFast == null)
            {
                return false;
            }
            VSSP_MultiSlow = (DEL_VSSP_MultiSlow)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiSlow", typeof(DEL_VSSP_MultiSlow));
            if (VSSP_MultiSlow == null)
            {
                return false;
            }
            VSSP_MultiOneByOne = (DEL_VSSP_MultiOneByOne)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiOneByOne", typeof(DEL_VSSP_MultiOneByOne));
            if (VSSP_MultiOneByOne == null)
            {
                return false;
            }
            VSSP_MultiSetPlayPosByTime = (DEL_VSSP_MultiSetPlayPosByTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiSetPlayPosByTime", typeof(DEL_VSSP_MultiSetPlayPosByTime));
            if (VSSP_MultiSetPlayPosByTime == null)
            {
                return false;
            }
            VSSP_MultiCloseSingle = (DEL_VSSP_MultiCloseSingle)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiCloseSingle", typeof(DEL_VSSP_MultiCloseSingle));
            if (VSSP_MultiCloseSingle == null)
            {
                return false;
            }
            VSSP_MultiGetGmtTime = (DEL_VSSP_MultiGetGmtTime)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiGetGmtTime", typeof(DEL_VSSP_MultiGetGmtTime));
            if (VSSP_MultiGetGmtTime == null)
            {
                return false;
            }

            VSSP_MultiOpenAudio = (DEL_VSSP_MultiOpenAudio)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiOpenAudio", typeof(DEL_VSSP_MultiOpenAudio));
            if (VSSP_MultiOpenAudio == null)
            {
                return false;
            }
            VSSP_MultiCloseAudio = (DEL_VSSP_MultiCloseAudio)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiCloseAudio", typeof(DEL_VSSP_MultiCloseAudio));
            if (VSSP_MultiCloseAudio == null)
            {
                return false;
            }
            VSSP_MultiSetVolume = (DEL_VSSP_MultiSetVolume)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiSetVolume", typeof(DEL_VSSP_MultiSetVolume));
            if (VSSP_MultiSetVolume == null)
            {
                return false;
            }
            VSSP_MultiShowVideo = (DEL_VSSP_MultiShowVideo)FuncCommon.GetFunctionAddress(m_hVsClient, "VSSP_MultiShowVideo", typeof(DEL_VSSP_MultiShowVideo));
            if (VSSP_MultiShowVideo == null)
            {
                return false;
            } 
            //end
            #endregion
            return true;
        }
        #endregion

        #region 释放动态库
        /// <summary>
        /// 释放动态库
        /// </summary>
        /// <returns>成功返回 true 失败返回 false</returns>
        public static bool FreeVsClientAPI()
        {
            if (m_hVsClient != 0)
            {
                Win32API.FreeLibrary(m_hVsClient);
                m_hVsClient = 0;
            }
            return true;
        }
        #endregion

        #endregion
    }
}     
