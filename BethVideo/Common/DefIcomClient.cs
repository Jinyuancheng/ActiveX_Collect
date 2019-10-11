using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IcomClient.Common
{
    public class DefIcomClient
    {
        public const Int32 WM_USER = 0x0400;    //windows系统定义的用户消息
        public const Int32 WM_VSCLIENT_DLL_MSG = WM_USER + 6002; //视频开发包连接消息
        public const Int32 WM_NETCLIENT_DLL_MSG = WM_USER + 7002; //运维开发包连接消息
        public const Int32 WM_DECMATRIX_DLL_MSG = WM_USER + 8002; //解码开发包连接消息
    }
    public enum IOStatus { Disabled, Normal, Alarm, earlyAlarm };   //IO的状态
    public enum MapStatus { SelMod, DragMod, RectMod };  //地图状态

    //网络操作回复消息
    public struct sMsgResInfo
    {
        public Int32 iCmd;
        public Int32 iHandle;
        public Int32 iResult;
    }
    public class sOneKeyAlarm
    {
        public string chDevId;   //设备ID
        public string chHelpId;  //一键求助地址
        public Int32 iTalking;  //是否对讲中
    }
    //对象信息结构体
    public struct sObjectInfo
    {
        public IntPtr m_pObject;                          //对象指针
        public Int32 m_iSize;                             //该对象所占内存空间大小
    }
    /// <summary>
    /// 对象结构体
    /// </summary>
    public struct sOBJECTSTRUCT
    {
        public object m_Object;
    }
    /// <summary>
    /// 天祥世联连接回调消息结构体
    /// </summary>
    //[StructLayout(LayoutKind.Sequential)]
    public struct sVsspConMsg
    {
        public Int32 m_lHandle;                           //连接句柄
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public string m_strCamId;                         //摄像头ID
        public string m_strUrl;                           //IP地址
        public Int32 m_iPort;                             //端口号
        public Int32 m_iChNo;                             //通道号
        public UInt32 m_iWParam;                          //消息高参数
        public UInt32 m_iLParam;                          //消息低参数 
    }
}
