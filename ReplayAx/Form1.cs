using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReplayAx
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 连接视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ucVideoMain.InitActiveX("G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\ReplayAx\\bin\\photos",
                "G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\ReplayAx\\bin\\dll\\VsClient.dll");
            //string StartTime = "1564113600";
            //string PlayTime = "1564114200";
            ////TimeSpan cha = (DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)));
            ////long EndTime = (long)cha.TotalSeconds;
            //string EndTime = "1564115400";
            //string sId = "100000053000";
           // ucVideoMain.StartPlay("192.168.1.154", StartTime, Convert.ToString(EndTime), PlayTime, sId);
        }
        /// <summary>
        /// 得到播放位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
           // MessageBox.Show("当前播放时间 ： " + Convert.ToString(ucVideoMain.GetPlayTime()));
        }
        /// <summary>
        /// 暂停播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //uint i = 1;
        private void button3_Click(object sender, EventArgs e)
        {
            //if(i == 1)
            //{
            //    ucVideoMain.PausePlay(i);
            //    i = 0;
            //}
            //else
            //{
            //    ucVideoMain.PausePlay(i);
            //    i = 1;
            //}
        }
        /// <summary>
        /// 停止播放按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            //ucVideoMain.StopPlay();
        }
        /// <summary>
        /// 快进播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //ucVideoMain.FastPlay();
        }
        /// <summary>
        /// 慢进播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            //ucVideoMain.SlowPlay();
        }
        /// <summary>
        /// 定位播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            //string PlayTime = "1564114800";
            //ucVideoMain.SetPlayPos(PlayTime);
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            //ucVideoMain.CaptureBmpEx();
        }
    }
}
