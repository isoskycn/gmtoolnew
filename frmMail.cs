using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmMail : Form
    {
        public frmMail()
        {
            InitializeComponent();
        }

        private void frmMail_Load(object sender, EventArgs e)
        {
            this.Text += String.Format(" - [{0}]", Moyu.playerName);

        }

       
        private void btnSend_Click(object sender, EventArgs e)
        {
            long unixTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            long endtime = (Convert.ToInt32(edit_saveday.Text) * 86400) + unixTime;
            string sql = String.Format("INSERT INTO cq_mailinfo set item1Lock='{0}',item2Lock='{1}',item3Lock='{2}',sendtime='{3}',endtime='{4}',userid='{5}',", Convert.ToInt32(cbx1.Checked), Convert.ToInt32(cbx2.Checked), Convert.ToInt32(cbx3.Checked), unixTime, endtime, Moyu.player_id);
            sql += Moyu.UpdateSqlBuild(groupSendMail);
            MySqlHelper.Query(sql);
            if (Moyu.SendMail(Moyu.player_id))
            {
                MessageBox.Show("邮件发送成功,已实时送达！");
            }
            else {
                MessageBox.Show("邮件发送成功,服务端未运行,未能实时送达！");
            }
            
        }

        private void btnSendServer_Click(object sender, EventArgs e)
        {
            long unixTime = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            long endtime = (Convert.ToInt32(edit_saveday.Text) * 86400) + unixTime;
            string sql = String.Format("INSERT INTO cq_mailinfo set item1Lock='{0}',item2Lock='{1}',item3Lock='{2}',sendtime='{3}',endtime='{4}',", Convert.ToInt32(cbx1.Checked), Convert.ToInt32(cbx2.Checked), Convert.ToInt32(cbx3.Checked), unixTime, endtime);
            sql += Moyu.UpdateSqlBuild(groupSendMail);
            string sql2 = "select id from cq_user_new where type=0;";
            int count = 0;
            DataTable dt = MySqlHelper.GetDataTable(sql2);
            string sql3 = "";
            try
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sql3 = sql + ",userid=" + dt.Rows[i]["id"].ToString();
                    MySqlHelper.Query(sql3);
                    Moyu.SendMail(Moyu.player_id);
                    count++;
                }

                MessageBox.Show(String.Format("邮件发送成功！本次共发送给{0}名玩家！", count.ToString()));
            }
            catch {
                MessageBox.Show("邮件发送失败！");
            }
        }

        private void btnSearchItem_Click(object sender, EventArgs e)
        {
            dgvItemList.Rows.Clear();
            DataTable dt = Moyu.GetItemList(2, txtSearchItem.Text.Trim());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvItemList.Rows.Add();

                dgvItemList.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvItemList.Rows[index].Cells["name"].Value = name;
            }
        }

        private void Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            Moyu.InputCheck(sender, e);
        }

        private void dgvItemList_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dgvItemList.Rows[e.RowIndex].Selected == false)
                    {
                        dgvItemList.ClearSelection();
                        dgvItemList.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dgvItemList.SelectedRows.Count == 1)
                    {
                        dgvItemList.CurrentCell = dgvItemList.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                    //弹出操作菜单
                    menuSendMail.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void miAddToItem1_Click(object sender, EventArgs e)
        {
            int index = dgvItemList.CurrentRow.Index;
            if (dgvItemList.Rows[index].Cells["id"].Value != null)
            {
                edit_item1.Text = dgvItemList.Rows[index].Cells["id"].Value.ToString();
            }
        }

        private void miAddToItem2_Click(object sender, EventArgs e)
        {
            int index = dgvItemList.CurrentRow.Index;
            if (dgvItemList.Rows[index].Cells["id"].Value != null)
            {
                edit_item2.Text = dgvItemList.Rows[index].Cells["id"].Value.ToString();
            }
        }

        private void miAddToItem3_Click(object sender, EventArgs e)
        {
            int index = dgvItemList.CurrentRow.Index;
            if (dgvItemList.Rows[index].Cells["id"].Value != null)
            {
                edit_item3.Text = dgvItemList.Rows[index].Cells["id"].Value.ToString();
            }
        }


    }
}
