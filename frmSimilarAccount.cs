using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmSimilarAccount : Form
    {
        public frmSimilarAccount()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Moyu.RemoveNumber("112dsfgdsf520"));
        }

        private void frmSimilarAccount_Load(object sender, EventArgs e)
        {

            FillAccountList();
        }
        private void FillAccountList()
        {
            dgvAccountList.Rows.Clear();
            DataTable dt = Moyu.GetSimialarAccount(Moyu.accountName);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvAccountList.Rows.Add();
                dgvAccountList.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvAccountList.Rows[index].Cells["name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvAccountList.Rows[index].Cells["password"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["password"]);
                dgvAccountList.Rows[index].Cells["netbar_ip"].Value = dt.Rows[i]["netbar_ip"];
                dgvAccountList.Rows[index].Cells["superpass"].Value = dt.Rows[i]["superpass"];
                dgvAccountList.Rows[index].Cells["hardcode"].Value = Moyu.GetUserHardWare(System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]));//手工添加的账号account_pwd没有数据,直接用查询的会出错

                if (Convert.ToInt32(dt.Rows[i]["online"]) == 0)
                {
                    dgvAccountList.Rows[index].Cells["online"].Value = "正常";
                }
                else
                {
                    dgvAccountList.Rows[index].Cells["online"].Value = "封号";
                }
            }
        }




        private void dgvAccountList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvAccountList.Rows.Remove(dgvAccountList.SelectedRows[0]);
        }

        private void btnBanAccount_Click(object sender, EventArgs e)
        {
            if (dgvAccountList.Rows.Count < 1) return;
            for (int i = 0; i < dgvAccountList.Rows.Count; i++)
            {
                string sql = String.Format("update account set online='1' where id='{0}';", Convert.ToInt32(dgvAccountList.Rows[i].Cells["id"].Value));
                MySqlHelper.Query(sql);
            }
            FillAccountList();
        }

        private void btnBanHardcode_Click(object sender, EventArgs e)
        {
            if (dgvAccountList.Rows.Count < 1) return;
            for (int i = 0; i < dgvAccountList.Rows.Count; i++)
            {
                Moyu.BanHardcode(dgvAccountList.Rows[i].Cells["hardcode"].Value.ToString());
            }
            FillAccountList();
        }
    }
}
