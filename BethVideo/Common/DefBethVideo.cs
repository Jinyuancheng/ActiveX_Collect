using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BethVideo
{
    #region 自定义数据结构
    public class sLoginInfor
    {
        public Int32 iHandle;                   //登录句柄
        public Int32 iRemoteHanle;
        public Int32 iType;                     //1表示视频 2表示动环 3表示门禁
        //public int iCamType;                    //摄像机类型 （api类型）1.自己api 2.海康api
        public string strIp;                    //登录ip
        public string strUser;                  //登录用户名
        public string strPsw;                   //登录密码
        public Int32 iPort;                     //登录端口
        public NET_DVR_DEVICEINFO_V40 devInfor; //设备信息用于登录
    }
    public class sTempUpload
    {
        public Int32 iHandle;
        public Single fTempHighest;
    }
    public class sDoorNoEntry
    {
        public Int32 iHandle;           //句柄
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] bCardNumber;      //卡号
        public byte bCardType;          //卡的类型
        public NET_DVR_TIME sAlarmTime; //报警时间
        public uint iMajor;              //主报警类型
        public uint iMijor;              //次报警类型
        public byte byPicTransType;     //图片类型 0,二进制 1,url
        public IntPtr pPicData;         //图片数据
        public uint dwPicDataLen;       //图片长度
    }
    public class sSealHead
    {
        public Int32 iHandle;   //操作句柄
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] bName;    //传感器名称
        public byte bType;      //模拟量类型
        public float fValue;    //当前模拟量的值
        public float fIVValue;  //当前电流电压值
        public byte bAlarmType; //报警类型
        public byte bAlarmMode; //报警模式
    }
    public class sOnOffAlarm
    {
        public Int32 iHandle;               //操作句柄
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
        public byte[] byName;               //开关量名称
        public byte byAlarmType;            //报警类型
        public NET_DVR_TIME_EX sAlarmTime;  //报警上传时间
        public int iChannel;                //通道
    }
    public class sTempAlarm
    {
        public Int32 iHandle;               //操作句柄
        public uint uiChannel;              //通道号
        public byte byTempUnit;             //温度单位 0 oc, 1 of ,2 k
        public byte byAlarmLevel;           //报警等级
        public byte byAlarmType;            //报警类型
        public byte byAlarmRule;            //报警规则
        public float fCurrTemp;             //当前温度
    }
    public class sTempDiffAlarm
    {
        public Int32 iHandle;               //操作句柄
        public uint uiChannel;              //通道号
        public byte byAlarmLevel;           //报警等级
        public byte byAlarmType;            //报警类型
        public byte byAlarmRule;            //报警规则
    }
    public class sVideoMove
    {
        public Int32 iHandle;               //操作句柄
        public uint uiAlarmType;            //报警类型
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
        public byte[] uiChannel;              //发生报警的通道
    }
    public class sThermalImagery
    {
        public Int32 iHandle;               //操作句柄
        public int iMaxTemp;                //最高温度
        public byte byStrategyType;         //策略类型
        public byte byAlarmSubType;         //报警类型
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
        public Char[] chIpv4;                  //设备ip地址
    }
    #endregion
}
