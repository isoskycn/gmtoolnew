using System;
using System.Windows.Forms;

namespace gmtoolNew
{
    public delegate void MyDel();
    public partial class FrmMain : Form
    {
        public static MyDel myDel;//定义一个委托类型的变量
        public static string str;//用于修改状态栏授权信息
        public FrmMain()
        {
            InitializeComponent();
            myDel = new MyDel(statusLabText);
            this.Text += " - 版本号:V" + License.Version;
        }


        //防止打开多个窗体
        public bool CheckFrmExist(string frmName)
        {
            int i;
            //依次检测当前窗体的子窗体
            for (i = 0; i < this.MdiChildren.Length; i++)
            {
                //判断当前子窗体的Text属性值是否与传入的字符串值相同
                if (this.MdiChildren[i].Name == frmName)
                {
                    //如果值相同则表示此子窗体为想要调用的子窗体，激活此子窗体并返回true值
                    if (this.MdiChildren[i].WindowState == FormWindowState.Minimized)//如果窗体被最小化了，则正常显示窗体
                        this.MdiChildren[i].WindowState = FormWindowState.Normal;
                    this.MdiChildren[i].Activate();//窗体获得焦点
                    return true;
                }
            }
            return false;
        }
        public void statusLabText()
        {
            statusLab1.Text = str;
            if (License.Licensed)
            {
                timerHertbeat.Interval = 3600000;//每1小时检测一次心跳,判断授权是否正常!
                timerHertbeat.Enabled = true;
            }
        }
        private void timerHertbeat_Tick(object sender, EventArgs e)
        {
            License.HeartBeat();
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            try
            {   //检查更新
                if (License.CheckUpdate())
                {
                    statusLab2.Text = "服务器连接成功!";
                    string msg = License.Init();
                    if (License.Licensed)
                    {
                        FrmMain.str = msg;
                        FrmMain.myDel();
                    }
                    else
                    {
                        statusLab1.Text = "未授权," + msg + "机器码:" + License.GetMachineCodeString();
                    }
                }
                else
                {
                    statusLab1.Text = "未授权,机器码:" + License.GetMachineCodeString();
                    statusLab2.Text = "服务器连接失败!";
                }

                //删除旧版本
                String[] args = Environment.GetCommandLineArgs();
                if (args.Length > 1 && args[1].ToString() == "-del")
                {
                    System.IO.File.Delete(args[2].ToString());
                }

                //加载数据库配置窗体
                if (!CheckFrmExist("frmDbConf"))
                {
                    frmDbConf frm = new frmDbConf();
                    frm.MdiParent = this;
                    frm.StartPosition = FormStartPosition.Manual;//居中
                    frm.StartPosition = FormStartPosition.CenterScreen;
                    frm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void subUserEdit_Click(object sender, EventArgs e)
        {
            if (!CheckFrmExist("frmUserEdit"))
            {
                frmUserEdit frm = new frmUserEdit();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();

            }
        }

        private void subUserItemEdit_Click(object sender, EventArgs e)
        {
            if (!CheckFrmExist("frmUserItemEdit"))
            {
                frmUserItemEdit frm = new frmUserItemEdit();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void miDbConf_Click(object sender, EventArgs e)
        {
            if (!CheckFrmExist("frmDbConf"))
            {
                frmDbConf frm = new frmDbConf();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }



        private void miMail_Click(object sender, EventArgs e)
        {
            if (!CheckFrmExist("frmMail"))
            {
                frmMail frm = new frmMail();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void subUserEduemon_Click(object sender, EventArgs e)
        {
            if (Moyu.player_id == 0)
            {
                MessageBox.Show("请先在人物管理选择玩家！");
                return;
            }
            if (!CheckFrmExist("frmEudemon"))
            {
                frmEudemon frm = new frmEudemon();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void miLottery_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmLottery"))
            {
                frmLottery frm = new frmLottery();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void subDropitem_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmDropItem"))
            {
                frmDropItem frm = new frmDropItem();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void subGenerator_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmGenerator"))
            {
                frmGenerator frm = new frmGenerator();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void miAction_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmAction"))
            {
                frmAction frm = new frmAction();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }


        private void miLicense_Click(object sender, EventArgs e)
        {
            if (!CheckFrmExist("frmLicense"))
            {
                frmLicense frm = new frmLicense();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void subSync_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
        }

        private void subOldNewBB_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
        }

        private void subOldNewAmor_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
        }

        private void subOldNewItem_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
        }

        private void miPrimaryOption_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
            if (!CheckFrmExist("frmOption"))
            {
                frmOption frm = new frmOption();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void miData_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmClearDB"))
            {
                frmClearDB frm = new frmClearDB();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void miOld_Click(object sender, EventArgs e)
        {
            /*if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }*/

            if (!CheckFrmExist("frmShop"))
            {
                frmShop frm = new frmShop();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }
        private void subAddition_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmAddition"))
            {
                frmAddition frm = new frmAddition();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

        private void subEmoney_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }

            if (!CheckFrmExist("frmToken"))
            {
                groupEmoney frm = new groupEmoney();
                frm.MdiParent = this;
                frm.StartPosition = FormStartPosition.Manual;//居中
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
        }

    }
}
