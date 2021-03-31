using System;
using System.IO;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmDbConf : Form
    {
        public frmDbConf()
        {
            InitializeComponent();
        }

        private void btnConnDB_Click(object sender, EventArgs e)
        {
            if (!License.Licensed)
            {
                MessageBox.Show("未授权!");
                frmLicense frm = new frmLicense();
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
                return;
            }

            if (btnConnDB.Text == "断开数据库")
            {
                MySqlHelper.connStr = null;
                btnConnDB.Text = "连接数据库";
                return;
            }
            try
            {
                MySqlHelper.connStr = String.Format("server={0};user id={1};password={2};database={3};port={4};Charset=gb2312",
                txtIP.Text, txtDBUser.Text, txtDBPwd.Text, txtDBName.Text, txtPort.Text);
                Moyu.DataBaseName = txtDBName.Text;
                if (cbxIsNewDB.Checked)//新端
                {
                    Moyu.IsNewDB = true;
                    Moyu.tName = "cq_user_new";
                }
                else
                {
                    Moyu.IsNewDB = false;
                    Moyu.tName = "cq_user";
                }
                MySqlHelper.TestConn();
                btnConnDB.Text = "断开数据库";
                string dir = Directory.GetCurrentDirectory() + "/config.ini";
                bool isFixed = Convert.ToBoolean(INIHelper.Read("DataBase", "Fixed", "False", dir));
                string confDbName = INIHelper.Read("DataBase", "DBName", "False", dir);
                if (!isFixed || txtDBName.Text != confDbName) FixDataBase();
                INIHelper.Write("DataBase", "IP", txtIP.Text, dir);
                INIHelper.Write("DataBase", "Port", txtPort.Text, dir);
                INIHelper.Write("DataBase", "DBUser", txtDBUser.Text, dir);
                INIHelper.Write("DataBase", "DBPwd", txtDBPwd.Text, dir);
                INIHelper.Write("DataBase", "DBName", txtDBName.Text, dir);
                INIHelper.Write("DataBase", "isNewDB", Convert.ToString(cbxIsNewDB.Checked), dir);
                this.Close();
            }
            catch (Exception ex)
            {
                MySqlHelper.connStr = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void frmDbConf_Load(object sender, EventArgs e)
        {
            if (MySqlHelper.connStr != null)
            {
                btnConnDB.Text = "断开数据库";
            }
            string dir = Directory.GetCurrentDirectory() + "/config.ini";
            txtIP.Text = INIHelper.Read("DataBase", "IP", "127.0.0.1", dir);
            txtPort.Text = INIHelper.Read("DataBase", "Port", "3306", dir);
            txtDBUser.Text = INIHelper.Read("DataBase", "DBUser", "test", dir);
            txtDBPwd.Text = INIHelper.Read("DataBase", "DBPwd", "test", dir);
            txtDBName.Text = INIHelper.Read("DataBase", "DBName", "top1", dir);
            cbxIsNewDB.Checked = Convert.ToBoolean(INIHelper.Read("DataBase", "isNewDB", "False", dir));
        }

        //修复数据库二进制字段
        private void FixDataBase()
        {
            if (Moyu.FixDataBase())
            {
                string dir = Directory.GetCurrentDirectory() + "/config.ini";
                INIHelper.Write("DataBase", "Fixed", "True", dir);
            }
        }

    }
}
