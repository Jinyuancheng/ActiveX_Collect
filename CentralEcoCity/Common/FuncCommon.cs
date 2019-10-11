using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;
using IcomClient.API;

namespace BethVideo
{
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
            IntPtr procAddr = Win32API.GetProcAddress(_hDllHandle, _strFuncName);
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

        public static void SetRunPath(string pathValue)
        {
            string pathNew = Environment.GetEnvironmentVariable("PATH");
            pathNew += (pathValue + ";");
            Environment.SetEnvironmentVariable("PATH", pathNew);
            pathNew = Environment.GetEnvironmentVariable("PATH");
        }
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
        public static Int32 SplitStringToList(string _strSource, string _strSplit, ref ArrayList _strlst)
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
    }

}
