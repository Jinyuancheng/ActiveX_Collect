/**********************************************************************
* Copyright (C) 2010-2012 天津市天祥世联网络科技有限公司
* 文件名: FuncCommon.cs
* 创建者: 肖元
* 创建时间: 2011-08-30
* 描述: 公共函数类
**********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
/*********************************************************************/
using System.Runtime.InteropServices;//Marshal 类在此命名空间下
using System.Collections;//使用ArrayList
using System.Runtime.Serialization.Formatters.Binary;//序列化 BinaryFormatter类
using System.IO;//序列化 MemoryStream类
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Reflection;
using Microsoft.Win32;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace IcomClient.Common
{
    #region 公共函数类
    /// <summary>
    /// 公共函数类
    /// </summary>
    public class FuncCommon
    {
        #region 通过非托管函数名获得与之对应的委托
        /// <summary>
        /// 通过非托管函数名获得与之对应委托
        /// </summary>
        /// <param name="_hDllHandle">通过LoadLibrary获得的DLL句柄</param>
        /// <param name="_strFuncName">非托管函数名</param>
        /// <param name="_tDelegateType">对应的委托类型</param>
        /// <returns>委托实例，可强制转换为适当的委托类型</returns>
        public static Delegate GetFunctionAddress(Int32 _hDllHandle, string _strFuncName, Type _tDelegateType)
        {
            IntPtr procAddr = IcomClient.API.Win32API.GetProcAddress(_hDllHandle, _strFuncName);
            if (procAddr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return Marshal.GetDelegateForFunctionPointer(procAddr, _tDelegateType);
            }
        }
        #endregion

        #region 将表示函数地址的 IntPtr 实例转换成对应的委托
        /// <summary>
        /// 将表示函数地址的 IntPtr 实例转换成对应的委托
        /// </summary>
        /// <param name="address"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Delegate GetDelegateFromIntPtr(IntPtr address, Type t)
        {
            if (address == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return Marshal.GetDelegateForFunctionPointer(address, t);
            }
        }
        #endregion

        #region 将表示函数地址的 int 转换成对应的委托
        /// <summary>
        /// 将表示函数地址的 int 转换成对应的委托
        /// </summary>
        /// <param name="address"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Delegate GetDelegateFromIntPtr(Int32 address, Type t)
        {
            if (address == 0)
            {
                return null;
            }
            else
            {
                return Marshal.GetDelegateForFunctionPointer(new IntPtr(address), t);
            }
        }
        #endregion

        #region 以某一子串为间隔，将目标字符串截取，将结果存放到字符串列表中
        /// <summary>
        /// 以某一子串为间隔，将目标字符串截取，将结果存放到字符串列表中
        /// </summary>
        /// <param name="_strSource">目标字符串</param>
        /// <param name="_strSplit">间隔字符串</param>
        /// <param name="_strlst">存放截取结果的字符串列表</param>
        /// <returns>所截得的字符串的个数</returns>
        public static Int32 SplitStringToList(string _strSource, string _strSplit,ref ArrayList _strlst)
        {
            Int32 iRet = -1;
            if (_strSource == null)
            {
                return iRet;
            }
            if (_strlst == null)
            {
                return iRet;
            }
            while (_strSource.IndexOf(_strSplit, 0) >= 0)
            {
                Int32 iPos = _strSource.IndexOf(_strSplit, 0);
                _strlst.Add(_strSource.Substring(0, iPos));
                _strSource = _strSource.Substring(iPos + _strSplit.Length);
            }
            if (_strSource != null)
            {
                _strlst.Add(_strSource);
            }
            iRet = _strlst.Count;
            return iRet;
        }
        #endregion

        #region 将一个对象序列化为byte数组
        /// <summary>
        /// 将一个对象序列化为byte数组
        /// </summary>
        /// <param name="_Obj"></param>
        /// <param name="_iSize"></param>
        /// <returns></returns>
        public static byte[] Serialize(object _Obj, out Int32 _iSize)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream();
            formatter.Serialize(rems, _Obj);
            _iSize = Int32.Parse(rems.Length.ToString());
            return rems.GetBuffer();
        }
        #endregion

        #region 把一个byte数组反序列化为对象
        /// <summary>
        /// 把一个byte数组反序列化为对象
        /// </summary>
        /// <param name="_Obj"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] _Obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream(_Obj);
            _Obj = null;
            return formatter.Deserialize(rems);
        }
        #endregion

        #region 把自定义类型的对象转换为IntPtr类型
        /// <summary>
        /// 把自定义类型的对象转换为IntPtr类型
        /// </summary>
        /// <param name="_Obj"></param>
        /// <param name="_iSize"></param>
        /// <returns></returns>
        public static IntPtr ClassToIntPtr(object _Obj, out Int32 _iSize)
        {
            byte[] temp = Serialize(_Obj, out _iSize);
            _iSize = temp.Length;
            IntPtr ptr = Marshal.AllocHGlobal(_iSize);
            Marshal.Copy(temp, 0, ptr, _iSize);
            return ptr;
        }
        #endregion

        #region 把IntPtr类型转换为自定义类型
        /// <summary>
        /// 把IntPtr类型转换为自定义类型
        /// </summary>
        /// <param name="_ptr"></param>
        /// <param name="_iSize"></param>
        /// <returns></returns>
        public static object IntPtrToClass(IntPtr _ptr, Int32 _iSize)
        {
            byte[] temp = new byte[_iSize];
            Marshal.Copy(_ptr, temp, 0, _iSize);
            return Deserialize(temp);
        }
        #endregion

        #region 把一个自定义类型的对象转化为对象信息结构体
        /// <summary>
        /// 把一个自定义类型的对象转化为对象信息结构体
        /// </summary>
        /// <param name="_Obj"></param>
        /// <returns></returns>
        public static sObjectInfo GetObjInfoFromObject(object _Obj)
        {
            Int32 iSize = 0;
            IntPtr ptrObj = IntPtr.Zero;
            ptrObj = ClassToIntPtr(_Obj, out iSize);
                sObjectInfo sObjInfo = new sObjectInfo();
                sObjInfo.m_pObject = ptrObj;
                sObjInfo.m_iSize = iSize;
                return sObjInfo;
        }
        #endregion

        #region 根据对象信息结构体获取对象
        /// <summary>
        /// 根据对象信息结构体获取对象
        /// </summary>
        /// <param name="_sObjInfo"></param>
        /// <returns></returns>
        public static object GetObjectFromObjInfo(sObjectInfo _sObjInfo)
        {
            return IntPtrToClass(_sObjInfo.m_pObject, _sObjInfo.m_iSize);
        }
        #endregion

        #region 将结构体转化为指针
        /// <summary>
        /// 将结构体转化为指针
        /// </summary>
        /// <param name="_struct"></param>
        /// <returns></returns>
        public static IntPtr StructToIntPtr(object _Obj)
        {
            int marsize = Marshal.SizeOf(_Obj);
            IntPtr ptr = Marshal.AllocHGlobal(marsize);
            Marshal.StructureToPtr(_Obj, ptr, true);
            return ptr;
        }
        #endregion

        #region 将指针转化为结构体
        /// <summary>
        /// 将指针转化为结构体
        /// </summary>
        /// <param name="_ptr"></param>
        /// <param name="_type"></param>
        /// <param name="_struct"></param>
        /// <returns></returns>
        public static sObjectInfo IntPtrToStruct(IntPtr _ptr)
        {
            sObjectInfo sObjInfo = new sObjectInfo();
            sObjInfo = (sObjectInfo)Marshal.PtrToStructure(_ptr, typeof(sObjectInfo));
            Marshal.FreeHGlobal(_ptr);
            return sObjInfo;
        }
        #endregion

        #region 把一个任意自定义类型的对象转化为可发送的IntPtr类型变量
        /// <summary>
        /// 把一个任意自定义类型的对象转化为可发送的IntPtr类型变量
        /// </summary>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static IntPtr ClassToSendIntPtr(object _object)
        {
            return StructToIntPtr(GetObjInfoFromObject(_object));
        }
        #endregion

        #region 把接收到的IntPtr类型的变量转化为一个对象
        /// <summary>
        /// 把接收到的IntPtr类型的变量转化为一个对象
        /// </summary>
        /// <param name="_ptr"></param>
        /// <returns></returns>
        public static object ResIntPtrToClass(IntPtr _ptr)
        {
            return GetObjectFromObjInfo(IntPtrToStruct(_ptr));
        }
        #endregion

        #region 将int时间转化为字符串00:00:00格式
        private static string strTime = "";
        /// <summary>
        ///  将int时间转化为字符串00:00:00格式
        /// </summary>
        /// <param name="_lFileTime">int时间</param>
        /// <returns>字符串00:00:00格式</returns>
        public static string FileTimeToStr(Int64 _lFileTime)
        {
            Int64 iSec, itmHour, itmMin, itmSec;

            iSec = _lFileTime / 1000;
            itmHour = iSec / 3600;
            string Hour = itmHour.ToString();
            if (Hour.Length == 1)
            {
                strTime = "0" + Hour + ":";
            }
            else
            {
                if (Hour.Length == 2)
                {
                    strTime = Hour + ":";
                }
            }
            itmMin = (iSec % 3600) / 60;
            string Min = itmMin.ToString();
            if (Min.Length == 1)
            {
                strTime += "0" + Min + ":";
            }
            else
            {
                if (Min.Length == 2)
                {
                    strTime += Min + ":";
                }
            }
            itmSec = (iSec % 3600) % 60;
            string Sec = itmSec.ToString();
            if (Sec.Length == 1)
            {
                strTime += "0" + Sec;
            }
            else
            {
                if (Sec.Length == 2)
                {
                    strTime += Sec;
                }
            }
            return strTime;
        }
        #endregion

        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="_strDir">文件或者文件夹的路径</param>
        /// <returns>成功返回 0 失败返回 -1</returns>
        public static Int32 CreateMultiDir(string _strDir)
        {
            Int32 iRet = -1;
            string strDir = System.IO.Path.GetDirectoryName(_strDir);
            ArrayList lstStrDirName = new ArrayList();
            SplitStringToList(strDir, "\\", ref lstStrDirName);
            if (lstStrDirName.Count > 0)
            {
                string strTemp = lstStrDirName[0].ToString();
                for (Int32 i = 1; i < lstStrDirName.Count; ++i)
                {
                    strTemp = strTemp + '\\' + lstStrDirName[i].ToString();
                    if (!System.IO.Directory.Exists(strTemp))
                    {
                        System.IO.Directory.CreateDirectory(strTemp);
                        if (i == (lstStrDirName.Count - 1))
                        {
                            iRet = 0;
                        }
                    }
                }
            }
            return iRet;
        }
        #endregion
        /// <summary>
        /// 修改设备ID
        /// </summary>
        public static void ConvertDevId(byte[] devid, int count)
        {
            bool bFind = false;
            for (int i = 0; i < count; i++)
            {
                if (bFind == true)
                {
                    devid[i] = 0;
                }
                if (devid[i] == 0)
                {
                    bFind = true;
                }
            }
        }
        /// <summary>
        /// 添加操作日志
        /// </summary>
        //public static void InsertOperLog(string url, string user, string content)
        //{
        //    IcapWeb.IcapWSFacadeClient client = new IcapWeb.IcapWSFacadeClient("IcapWSFacadePort", url);
        //    IcapWeb.operLog log = new IcapWeb.operLog();
        //    log.i_TYPE = 1;
        //    log.CH_TIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    log.CH_NAME = user;
        //    log.CH_CONTENT = content;
        //    try
        //    {
        //        client.AddOperLog(log);
        //    }
        //    catch (Exception)
        //    {
        //        ;
        //    }
        //}

    }
    #endregion
}
