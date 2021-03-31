using System;
using System.IO;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmLicense : Form
    {
        private static string dir = Directory.GetCurrentDirectory() + "/config.ini";
        public frmLicense()
        {
            InitializeComponent();
        }

        private void frmLicense_Load(object sender, EventArgs e)
        {

            txtMachine_code.Text = License.GetMachineCodeString();
            txtMachine_code2.Text = License.GetMachineCodeString();
            txtCode.Text = INIHelper.Read("License", "code", "", dir);
            txtOldMachine_code.Text = INIHelper.Read("License", "machine", "", dir);
            txtCode2.Text = INIHelper.Read("License", "code", "", dir);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            INIHelper.Write("License", "machine", txtMachine_code.Text, dir);
            INIHelper.Write("License", "code", txtCode.Text, dir);
            MessageBox.Show("保存成功!");
        }

        private void btnReg_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCode.Text.Trim()))
            {
                MessageBox.Show("授权码非法!");
                return;
            }

            if (License.Licensed)
            {
                MessageBox.Show("您已经是授权用户!到期时间:" + License.ExpireTime);
                this.Close();
                return;

            }

            try
            {
                string msg = License.Reg(txtMachine_code.Text, txtCode.Text);
                if (License.Licensed)
                {
                    INIHelper.Write("License", "machine", txtMachine_code.Text, dir);
                    INIHelper.Write("License", "code", txtCode.Text, dir);
                    FrmMain.str = "授权成功!到期时间:" + License.ExpireTime;
                    FrmMain.myDel();
                    MessageBox.Show(msg);
                    this.Close();
                    return;
                }
                MessageBox.Show(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCode2.Text.Trim()) || string.IsNullOrEmpty(txtOldMachine_code.Text.Trim()))
            {
                MessageBox.Show("授权码或旧机器码非法!");
                return;
            }
            if (txtMachine_code2.Text == txtOldMachine_code.Text)
            {
                MessageBox.Show("旧机器码和机器码相同,无法换绑!");
                return;
            }

            if (License.Licensed)
            {
                MessageBox.Show("您已经是授权用户!到期时间:" + License.ExpireTime);
                this.Close();
                return;

            }

            try
            {
                string msg = License.Change(txtMachine_code2.Text, txtCode2.Text, txtOldMachine_code.Text);
                if (License.Licensed)
                {
                    INIHelper.Write("License", "machine", txtMachine_code2.Text, dir);
                    INIHelper.Write("License", "code", txtCode2.Text, dir);
                    FrmMain.str = "授权成功!到期时间:" + License.ExpireTime;
                    FrmMain.myDel();
                    MessageBox.Show(msg);
                    this.Close();
                    return;

                }
                MessageBox.Show(msg);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            License.BuyCard();
        }
    }
}
