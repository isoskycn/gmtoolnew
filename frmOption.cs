using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmOption : Form
    {
        public frmOption()
        {
            InitializeComponent();
        }

        private void frmOption_Load(object sender, EventArgs e)
        {
            Fill();
        }

        private void Fill()
        {
            DataTable dt = Moyu.GetConfig();
            dgvConfig.DataSource = dt;
        }

        private void btnAddGmIP_Click(object sender, EventArgs e)
        {
            string sql = String.Format("delete from cq_config where type in(10300,80000);" +
                "INSERT INTO `cq_config` (`type`,`str`, `desc`) VALUES ('80000', '{0}', '绑定GM的IP'),('10300', '{1}', '绑定GM的IP');", txtIP.Text, txtIP.Text);
            if (MySqlHelper.Query(sql) > 0)
            {
                MessageBox.Show("添加成功");
                Fill();
            }
        }

        private void btnDelGmIP_Click(object sender, EventArgs e)
        {
            string sql = "delete from cq_config where type in(10300,80000);";
            if (MySqlHelper.Query(sql) > 0)
            {
                MessageBox.Show("删除成功");
                Fill();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dgvConfig.Rows.Count; i++)
            {
                string sql = String.Format("UPDATE `cq_config` SET `data1`='{0}', `data2`='{1}', `str`='{2}', `desc`='{3}' WHERE (`id`='{4}');",
                    dgvConfig.Rows[i].Cells["data1"].Value, dgvConfig.Rows[i].Cells["data2"].Value, dgvConfig.Rows[i].Cells["str"].Value, dgvConfig.Rows[i].Cells["desc"].Value, dgvConfig.Rows[i].Cells["id"].Value);
                MySqlHelper.Query(sql);
            }
            MessageBox.Show("保存成功");
        }
    }
}
