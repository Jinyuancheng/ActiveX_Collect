using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralEcoCity
{
    #region 自定义数据结构
    /// <summary>
    /// 设备登录信息
    /// </summary>
    public class CDevCamInfo
    {
        public string sCamName;   //摄像机名称
        public string sIp;        //登录摄像机的ip
        public string iPort;       //登录摄像机的端口
        public string lId;          //登录id
    }
    #endregion

}
