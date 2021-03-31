using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class groupEmoney : Form
    {
        private static int PlayerId =0;
        public groupEmoney()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            dgvLogs.Rows.Clear();
            string name = txtUser.Text.Trim();
            if (name != "")
            {
                int playerId = Moyu.PlayName2Id(name);
                if (playerId == -1)
                {
                    MessageBox.Show("没有这个角色！");
                    return;
                }
                else {
                    PlayerId = playerId;
                }
            }
            btnSearch.Enabled = false;

            int model = rbtnToken.Checked ? 1 : 0;
            int isIn = cbxIn.Checked ? 1 : 0;
            if (cbxIn.Checked && cbxOut.Checked) isIn = 2;
            string fromTime = dtpBegin.Value.ToString("yyMMddHHmm");
            string endTime = dtpEnd.Value.ToString("yyMMddHHmm");
            DataTable dt = Moyu.GetEmoneyTradeLogs(fromTime, endTime, model, isIn, PlayerId);
            prcBar.Maximum = dt.Rows.Count;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                prcBar.Value = i + 1;
                labStatus.Text = String.Format("已读取到数据条数[{0}/{1}]", i + 1, dt.Rows.Count);
                int index = dgvLogs.Rows.Add();
                int type = Convert.ToInt32(dt.Rows[i]["type"]);
                int id_source = Convert.ToInt32(dt.Rows[i]["id_source"]);
                int id_target = Convert.ToInt32(dt.Rows[i]["id_target"]);
                DateTime time = DateTime.ParseExact(dt.Rows[i]["time_stamp"].ToString(), "yyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                dgvLogs.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvLogs.Rows[index].Cells["type"].Value = Moyu.GetEmoneyTradeType(type);
                dgvLogs.Rows[index].Cells["number"].Value = dt.Rows[i]["number"];
                dgvLogs.Rows[index].Cells["id_source"].Value = Moyu.GetEmoneyTradeUser(id_source);
                dgvLogs.Rows[index].Cells["source_emoney"].Value = dt.Rows[i]["source_emoney"];
                dgvLogs.Rows[index].Cells["id_target"].Value = Moyu.GetEmoneyTradeUser(id_target);
                dgvLogs.Rows[index].Cells["target_emoney"].Value = dt.Rows[i]["target_emoney"];
                dgvLogs.Rows[index].Cells["time_stamp"].Value = time.ToString("yyyy/MM/dd HH:mm");
                Application.DoEvents();//防止循环过快导致界面假死
            }
            btnSearch.Enabled = true;
        }

        private void groupEmoney_Load(object sender, EventArgs e)
        {
            dtpBegin.Value = DateTime.Today.AddDays(-30);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string tip = String.Format("确认删除{0}之前的记录吗?", dtpBegin.Value.ToString("yyyy/MM/dd HH:mm"));
            DialogResult dr = MessageBox.Show(tip, "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            string tableName = rbtnEmoney.Checked ? "e_money" : "cq_token";
            string time = dtpBegin.Value.ToString("yyMMddHHmm");
            string sql = String.Format("delete from {0} where time_stamp<{1};", tableName, time);
            if (MySqlHelper.Query(sql) > 0)
                MessageBox.Show("删除成功!");
            
        }
    }
}
