/**********************************************************************
* Copyright (C) 2010-2012 天津市天祥世联网络科技有限公司
* 文件名: DefVsClient.cs
* 创建者: 肖元
* 创建时间: 2011-08-30
* 描述: 天祥世联新协议API数据结构定义类
**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
/*********************************************************************/
using System.Runtime.InteropServices; //使用Marshal

namespace IcomClient.API
{
    #region 枚举类型

    #region 录像文件操作类型
    /// <summary>
    /// 录像文件操作类型
    /// </summary>
    public enum eRecFileCommand
    {
        rfcSearch, 
        rfcDelete, 
        rfcLock, 
        rfcUnlock
    }
    #endregion

    #region 获取参数命令
    /// <summary>
    /// 获取参数命令
    /// </summary>
    public enum ParamCmdType
    {
        cmdViParam,
        cmdChParam,
        cmdPtzParam,
        cmdNetParam,
        cmdTimeSyncParam,
        cmdPlatParam
    }
    #endregion

    #region 视频通道类型
    /// <summary>
    /// 视频通道类型
    /// </summary>
    public enum eEPlayType : int
    {
        PLAYLC = 0, 
        PLAYAP = 1,
        PLAYTAS = 2, 
        PLAYHIK = 3, 
        PLAYTLK = 4
    }
    #endregion

    #endregion

    #region 委托类型

    #region 天祥世联新协议API消息回调函数 委托
    /// <summary>
    /// 天祥世联新协议API消息回调函数 委托
    /// </summary>
    /// <param name="_hHandle">播放句柄</param>
    /// <param name="_iwParam">高参数</param>
    /// <param name="_ilParam">低参数</param>
    public delegate void VsClientMsgCB(Int32 _hHandle, Int32 _iwParam, Int32 _ilParam);
    #endregion

    #region 报警服务器 Presence服务器 位置服务器 回调函数 委托
    /// <summary>
    /// 报警服务器 Presence服务器 位置服务器 回调函数 委托
    /// </summary>
    /// <param name="_hHandle">连接服务器句柄</param>
    /// <param name="_iwParam">高参数</param>
    /// <param name="_ilParam">低参数</param>
    /// <param name="_pchData">数据</param>
    public delegate void VsServicesCB(Int32 _hHandle, Int32 _iwParam, Int32 _ilParam, string _pchData);//byte[] _pchData);//string _pchData);
    #endregion

    /// 消息服务器回调函数
    /// </summary>
    /// <param name="_hHandle">句柄</param>
    /// <param name="_iwParam">高消息</param>
    /// <param name="_ilParam">低消息</param>
    /// <param name="_pchData">数据</param>
    /// <param name="_iType">类型</param>
    /// <param name="_iCount">结构体个数</param>
    /// <returns></returns>
    public delegate void pNotifyCallBack(Int32 _hHandle, Int32 _iwParam, Int32 _ilParam, IntPtr _strDate, Int32 _iType, Int32 _iCount);

    #region 查询录像文件回调函数 委托
    /// <summary>
    /// 查询录像文件回调函数 委托
    /// </summary>
    /// <param name="_hHandle">查询句柄</param>
    /// <param name="_iRecCount">记录数目</param>
    /// <param name="_pchRecData">记录缓冲</param>
    public delegate void VsSearchRecFileCB(Int32 _hHandle, Int32 _iRecCount, IntPtr _pchRecData);//StringBuilder _pchRecData);
    #endregion

    #endregion

    /// <summary>
    /// 天祥世联新协议API数据结构定义类
    /// </summary>
    public class DefVsClient
    {
        #region 常量定义

        #region 连接消息
        /// <summary>
        /// 连接消息
        /// </summary>
        public const Int32 VSSP_LINKMSG_TYPE = 1;
        #endregion

        #region 抓拍消息
        /// <summary>
        /// 抓拍消息
        /// </summary>
        public const Int32 VSSP_CAPMSG_TYPE = 2;
        #endregion

        #region 参数配置和透明通道消息
        /// <summary>
        /// 参数配置和透明通道消息
        /// </summary>
        public const Int32 VSSP_PARAMMSG_TYPE = 3;
        #endregion

        public const Int32 VSSP_PTZ_CTRL_TYPE = 7;

        public const Int32 VSSP_PTZ_CTRL_TIME_OUT = 0;

        #region Presence消息
        /// <summary>
        /// Presence消息
        /// </summary>
        public const Int32 VSSP_PRESENCE_TYPE = 4;
        #endregion

        #region 报警消息
        /// <summary>
        /// 报警消息
        /// </summary>
        public const Int32 VSSP_ALARM_TYPE = 5;
        #endregion

        #region GPS位置信息
        /// <summary>
        /// GPS位置信息
        /// </summary>
        public const Int32 VSSP_LOCATION_TYPE = 6;
        #endregion

        /// <summary>
        /// 直播消息：连接成功
        /// </summary>
        public const Int32 VSSP_CONNECT_OK = 0;
        /// <summary>
        /// 直播消息：连接失败
        /// </summary>
        public const UInt32 VSSP_CONNECT_ERR = 0xFFFFFFFF;
        /// <summary>
        /// 直播消息：本地录像磁盘满
        /// </summary>
        public const Int32 VSSP_RECODERDISKFILL = 65;
        /****************************************/
        //VSSP_PLAYBACK_TYPE 对应的lParam
        /// <summary>
        /// 回放消息
        /// </summary>
        public const Int32 VSSP_PLAYBACK_TYPE = 1;
        /// <summary>
        /// 回放结束
        /// </summary>
        public const Int32 VSSP_PLAYBACK_END = 2;
        /// <summary>
        /// 回放录像磁盘空间满
        /// </summary>
        public const Int32 VSSP_PLAYBACK_DISK_FULL = 3;
        /// <summary>
        /// 回放定位操作结束
        /// </summary>
        public const Int32 VSSP_PLAYBACK_POS_END = 4;
        /****************************************/
        //VSSP_DOWNLOAD_TYPE 对应的lParam
        /// <summary>
        /// vod录像：下载开始
        /// </summary>
        public const Int32 VSSP_DOWNLOAD_START = 1;
        /// <summary>
        ///  vod录像：下载结束
        /// </summary>
        public const Int32 VSSP_DOWNLOAD_END = 2;
        /// <summary>
        ///  vod录像：下载录像磁盘空间满
        /// </summary>
        public const Int32 VSSP_DOWNLOAD_DISK_FULL = 3;
        /// <summary>
        ///  vod录像：下载进度
        /// </summary>
        public const Int32 VSSP_DOWNLOAD_PROCESS = 4;
        /****************************************/
        //VSSP_MULTI_VOD_TYPE 对应的lParam
        /// <summary>
        /// 多路回放开始
        /// </summary>
        public const Int32 VSSP_MULTI_VOD_START = 1;	 //多路回放开始
        /// <summary>
        /// 多路回放结束
        /// </summary>
        public const Int32 VSSP_MULTI_VOD_END = 2;	 //多路回放结束
        /****************************************/
        /*********avi 消息****************************************/
        /// <summary>
        /// avi次消息 成功 31
        /// </summary>
        public const Int32 VSSP_AVI_OK = 31;
        /// <summary>
        /// avi次消息 失败 32
        /// </summary>
        public const Int32 VSSP_AVI_NO = 32;
        /// <summary>
        /// avi主消息  13
        /// </summary>
        public const Int32 VSSP_AVI = 13;
        /*********avi 消息****************************************/

        #region 接收到Precece数据
        /// <summary>
        /// 接收到Precece数据
        /// </summary>
        public const Int32 VSSP_PRESENCE_DATA = 1;
        #endregion

        #region Precese连接成功
        /// <summary>
        /// Precese连接成功
        /// </summary>
        public const Int32 VSSP_PRESENCE_CON = 2;
        #endregion

        #region 断开Presence连接
        /// <summary>
        /// 断开Presence连接
        /// </summary>
        public const Int32 VSSP_PRESENCE_DISCON = 3;



        public const Int32 VSSP_RECRSDATARES_ERR = 9;     //接收RS Data回复失败
        #endregion

        #endregion

        #region 网络连接信息结构体
        /// <summary>
        /// 网络连接信息结构体
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct NETLINK_INFO
        {
            /// <summary>
            /// 主叫号码
            /// </summary>
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            //public string m_pchFromID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] m_pchFromID;
            /// <summary>
            /// 被叫号码
            /// </summary>
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            //public string m_pchToID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] m_pchToID;
            /// <summary>
            /// 用户名
            /// </summary>
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            //public string m_pchUser;


            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] m_pchUser;
            /// <summary>
            /// 密码
            /// </summary>
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            //public string m_pchPsw;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] m_pchPsw;                              
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
            //public char[] m_pchFromID;                 //主叫号码
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            //public char[] m_pchToID;                   //被叫号码
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            //public char[] m_pchUser;                   //用户名
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            //public char[] m_pchPsw;                    //密码
            /// <summary>
            /// 连接方式 1 TCP
            /// </summary>
            public Int32 m_iTranType;
            /// <summary>
            /// IP地址
            /// </summary>
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            //public string m_pchUrl;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] m_pchUrl;                   
            /// <summary>
            /// 端口
            /// </summary>
            public Int32 m_iPort;
        }
        #endregion

        #region 被叫通道信息结构体
        /// <summary>
        /// 被叫通道信息结构体
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct CHANNEL_CONINFOR
        {
            public NETLINK_INFO m_sNetLinkInfo;        //网络连接信息
            public Int32 m_iChNo;                      //通道号从0开始
            public IntPtr m_hVideo;                    //视频显示句柄
            public Int32 m_iBuffNum;                   //缓冲个数10~50
            public Int32 m_iType;                      //被叫通道类型 0 朗驰 1 艾普
            public Int32 m_iStreamType;                //码流类型 0表示主码流 1表示子码流 
            public UInt32 m_uRecv;                     //预留
        }
        #endregion

        #region 抓拍图片信息结构体
        /// <summary>
        /// 抓拍图片
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CAP_JPEG_DATA
        {
            public NETLINK_INFO m_sNetLinkInfo;        //网络连接信息
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string m_pchFilePath;               //图片路径  
        }
        #endregion

        #region 参数设置结构体
        /// <summary>
        /// 参数设置结构体
        /// </summary> 
        [StructLayout(LayoutKind.Sequential,  CharSet = CharSet.Ansi, Pack = 4)]
        public struct PARAM_SET_DATA
        {
            public NETLINK_INFO m_sNetLinkInfo;        //网络连接信息
            public Int32 m_iCommand;                   //命令
            public Int32 m_iChNo;                      //通道号
            public Int32 m_iPriority;                  //优先级别
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            //public char[] m_pchParams;                 //数据
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4096)]
            //public string m_pchParams;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
            public byte[] m_pchParams;

        }
        #endregion

        #region 视频参数结构体
        /// <summary>
        /// 视频参数结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CHANNVIPARAM
        {
            public byte m_bri;                         //图像亮度      
            public byte m_con;                         //图像对比度
            public byte m_sat;                         //图像饱和度
            public byte m_hue;                         //图像色度
        }
        #endregion

        #region 点播或下载的网络时间结构体
        /// <summary>
        /// 点播或下载的网络时间结构体
        /// </summary>
        public struct PLAY_NETTIME
        {
            public Int32 m_iYear;         //年
	        public byte m_uchMonth;       //月
	        public byte m_uchDay;         //日
	        public byte m_uchHour;        //时
	        public byte m_uchMinute;      //分
	        public byte m_uchSecond;      //秒
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
	        public byte[] pchReceived;    //保留
        }
        #endregion

        #region 多路VOD回放结构体
        /// <summary>
        /// 点播或下载的网络时间结构体
        /// </summary>
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct VOD_SPECIAL_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string pchUrl;
            public Int32 iPort;
            public IntPtr hWnd;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string pchChlID;
        }
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct ARRY_VOD_SPECIAL_INFO
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public VOD_SPECIAL_INFO[] arryVodSpecialInfo;
        }
        #endregion
        #region 操作录像文件结构体
        /// <summary>
        /// 操作录像文件结构体
        /// </summary>
        public struct OPER_REC_FILE
        {
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            //public string m_pchUrl;       //IP地址
            //public Int32 m_iPort;         //端口
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            //public string m_pchUser;      //用户名
            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            //public string m_pchPsw;       //密码
            public NETLINK_INFO m_sNetLinkInfo;        //网络连接信息
            public Int32 m_iType;         //方式 0文件方式 1 时间方式
	        public Int32 m_iCommand;      //命令
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
	        public string m_pchFile;      //文件路径/通道ID
	        public PLAY_NETTIME m_sStartTime;    //开始时间
	        public PLAY_NETTIME m_sEndTime;      //结束时间
	        public VsSearchRecFileCB m_pSearchCB;//查询回调
        }
        #endregion 

        public const Int32 STRSIZE = 256;

        #region 文件信息结构体
        /// <summary>
        /// 文件信息结构体
        /// </summary>
        public struct FILE_INFO_LIST
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] m_pchChannelID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = STRSIZE)]
            public string m_pchFilePath;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string m_pchStartTime;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string m_pchEndTime;
            public Int32 m_iLocfFlag;
            public Int32 m_iAlarmFlag;
        }
        #endregion

        public const Int32 FileInfoSize = 316;

        public struct ETI_FILE_HEAD
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string m_filemask;  //文件Mask "LAUNCHDIGITAL"
            public Int32 m_fileversion;//文件版本：0x01               
	        public Int32 videofoucc;   //视频fourcc : 0X34363248:H264(lc5000) //0X58564944 : DIVX(lc8000,lc9000,lc6000)
	        public Int32 videoid;      //此项忽略
	        public short m_width;      //视频宽度
	        public short m_height;     //视频高度
	        public short m_audiotag;   //音频fourcc:  0xFE:G722(lc5000,lc6000) //0x55: mp3(lc8000,lc9000)
	        public short m_haveaudio;  //音频有无标志
	        public short m_auchs;      //音频通道数
	        public short m_aurates;    //音频码流
	        public Int32 m_ausample;   //音频采样频率,(lc5000,lc6000)采样频率为8000
	        public UInt32 m_totalframes;//此项忽略
	        public UInt32 m_firsttick; //此项忽略
            public UInt32 m_losttick;  //此项忽略
	        public Int32 maskend;      //此项忽略
        }
        public struct ETI_DVR_FRMAME_HEAD
        {
            public UInt32 nID;		   //ETI_DVR_FRAME_HEAD_ID,0XB3010000
	        public UInt32 nTimeTick;   //时间戳,相对时间,毫秒为单位
	        public UInt32 nGMTTime;    //绝对时间,自1970年1月1日 00:00:00 开始的秒数,时区按GMT时区计算.
	        public Int32 nFrameType;   //低4位表示视频帧类型: 0->I-FRAME/1->P-FRAME/3->B-FRAME
                                       //第5位表示是否包含音频: 0x10:表示有音频帧
								       //第6位表示是否包含文件头信息: 0x20: 表示有文件头信息
            public Int32 nFrameCount;  //编码帧序号
	        public Int32 nVideoSize;   //视频数据长度
	        public UInt32 nAudioSize;  //音频数据长度
	        public UInt32 nMuxData;	   //复合数据
        }
        public struct ETI_FRMAME_HEAD
        {
	        public UInt32 nID;			     //0XB2010000
	        public UInt32 nTimeTick;         //时间戳,(ms),相对时间
	        public Int32 nVideoSize;         //视频数据长度
	        public UInt32 nAudioSize;        //音频数据长度
	        public UInt32 nMuxData;	         //复合数据：视频通道、帧类型、用户数据
        }

        /// <summary>
        /// 车载消息类型
        /// </summary>
        public enum emCarMsgType
        {
            /// <summary>
            /// 状态消息
            /// </summary>
            CARMSG_STATUS = 0x3001,                //状态消息
            /// <summary>
            /// 旧版本位置信息
            /// </summary>
            CARMSG_OLDVER_GPS,                   //旧版本位置信息
            /// <summary>
            /// 位置信息
            /// </summary>
            CARMSG_GPS,                         //位置信息
        }
        /// <summary>
        /// 设备上下线信息
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct VSSP_OnOffInfor
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string achId;           //设备id   
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string achTime;        //时间
            public Int32 iState;             //状态0;下线1上线
        }
    }
}
