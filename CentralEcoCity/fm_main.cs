using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace CentralEcoCity
{
    public partial class fm_main : Form
    {
        public fm_main()
        {
            InitializeComponent();
        }

        private void conn_video_Click(object sender, EventArgs e)
        {
            //ucVideoMain.InitVideoAx("G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\CentralEcoCity\\bin\\photos");
            for (int i = 0; i < 3; i++)
            {
                //ucVideoMain.ConnectVideo("192.168.1.154", 3000, "100000053000", "视频" + i);
            }
            //ucVideoMain.ConnectVideo("192.168.1.154", 3000, "b7a79d98-2b7a-11e1-8cc1-d027884f4b4c");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ucVideoMain.InitVideoAx("G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\CentralEcoCity\\bin\\photos");
            ucVideoMain.ChangeScreen(9);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ucVideoMain.InitVideoAx("G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\CentralEcoCity\\bin\\photos");
            ucVideoMain.DisConnectVideo();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ucVideoMain.InitVideoAx("G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\CentralEcoCity\\bin\\photos");
            ucVideoMain.DisConnectVideoAll();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //ucVideoMain.InitVideoAx("G:\\天津白泽技术有限公司项目\\天津白泽技术项目源文件等\\2019-07-18\\CentralEcoCity\\bin\\photos");
            Random rd = new Random();
            int num = rd.Next() % 10;
            string[] Strs = new string[num];
            for (int i = 0; i < num; i++)
            {
                CDevCamInfo oCamInfo = new CDevCamInfo();
                oCamInfo.sCamName = "相机_" + i;
                oCamInfo.sIp = "192.168.1.154";
                oCamInfo.iPort = "3000";
                oCamInfo.lId = "100000053000";
                Strs[i] = JsonConvert.SerializeObject(oCamInfo);
                //Strs[i] = "{\"sCamName\":\"相机_\"" + i + 1 + ",\"sIp\":\"192.168.1.154\"," +
                //    "\"iPort\":\"3000\",\"lId\":\"100000053000\"}";
            }
            //ucVideoMain.SwitchConnect(Strs);
        }
    }
}
