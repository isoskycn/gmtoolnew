using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmLottery : Form
    {
        public frmLottery()
        {
            InitializeComponent();
        }

        private void frmLottery_Load(object sender, EventArgs e)
        {
            ComboxItem[] type = {
                new ComboxItem("圣天使宝箱","1"),
                new ComboxItem("8星O型宝箱","2"),
                new ComboxItem("12星XO型宝箱","3"),
                new ComboxItem("特制经验球宝箱","4"),
                new ComboxItem("芬芳玫瑰宝箱","5")

            };
            cbbType.Items.AddRange(type);

            //每日抽奖次数
            string sql = "select param from cq_action where id='3700982';";
            object o = MySqlHelper.Find(sql);
            string num = Moyu.RemoveNotNumber(System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])o));
            txtLotteryNum.Text = num;
            txtLotteryNum.Tag = num;
            FillLottery(Moyu.GetLottery());
        }


        private void FillLottery(DataTable dt)
        {
            dgvLottery.Rows.Clear();
            //DataTable dt = Moyu.GetLottery();
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvLottery.Rows.Add();
                dgvLottery.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvLottery.Rows[index].Cells["type"].Value = Moyu.GetCbbItem(cbbType, dt.Rows[i]["type"].ToString());
                dgvLottery.Rows[index].Cells["rank"].Value = dt.Rows[i]["rank"];
                dgvLottery.Rows[index].Cells["chance"].Value = dt.Rows[i]["chance"];
                dgvLottery.Rows[index].Cells["prize_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["prize_name"]);
                dgvLottery.Rows[index].Cells["prize_item"].Value = dt.Rows[i]["prize_item"];
                dgvLottery.Rows[index].Cells["addition_lev"].Value = dt.Rows[i]["addition_lev"];
                dgvLottery.Rows[index].Cells["hole_num"].Value = dt.Rows[i]["hole_num"];
                dgvLottery.Rows[index].Cells["fire_atk"].Value = dt.Rows[i]["fire_atk"];
                dgvLottery.Rows[index].Cells["wind_atk"].Value = dt.Rows[i]["wind_atk"];
                dgvLottery.Rows[index].Cells["water_atk"].Value = dt.Rows[i]["water_atk"];
                dgvLottery.Rows[index].Cells["earth_atk"].Value = dt.Rows[i]["earth_atk"];
            }
        }

        private void dgvLottery_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            groupLottery.Tag = dgvLottery.CurrentRow.Cells["id"].Value;
            cbbType.SelectedIndex = cbbType.FindString(dgvLottery.CurrentRow.Cells["type"].Value.ToString());
            edit_prize_name.Text = dgvLottery.CurrentRow.Cells["prize_name"].Value.ToString();
            edit_prize_item.Text = dgvLottery.CurrentRow.Cells["prize_item"].Value.ToString();
            edit_rank.Text = dgvLottery.CurrentRow.Cells["rank"].Value.ToString();
            edit_chance.Text = dgvLottery.CurrentRow.Cells["chance"].Value.ToString();
            edit_addition_lev.Text = dgvLottery.CurrentRow.Cells["addition_lev"].Value.ToString();
            edit_hole_num.Text = dgvLottery.CurrentRow.Cells["hole_num"].Value.ToString();
            edit_fire_atk.Text = dgvLottery.CurrentRow.Cells["fire_atk"].Value.ToString();
            edit_wind_atk.Text = dgvLottery.CurrentRow.Cells["wind_atk"].Value.ToString(); ;
            edit_water_atk.Text = dgvLottery.CurrentRow.Cells["water_atk"].Value.ToString(); ;
            edit_earth_atk.Text = dgvLottery.CurrentRow.Cells["earth_atk"].Value.ToString(); ;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string sql = String.Format("update cq_action set param=replace(param,'{0}','{1}') where id='3700982';", txtLotteryNum.Tag.ToString(), txtLotteryNum.Text);
            MySqlHelper.Query(sql);
            if (groupLottery.Tag == null) return;
            string sql2 = String.Format("update cq_lottery set type='{0}',{1} where id='{2}';", ((ComboxItem)cbbType.SelectedItem).Values, Moyu.UpdateSqlBuild(groupLottery), groupLottery.Tag.ToString());
            MySqlHelper.Query(sql2);
            MessageBox.Show("修改成功");
            FillLottery(Moyu.GetLottery());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(edit_prize_item.Text.Trim()) || string.IsNullOrEmpty(edit_prize_name.Text.Trim()) || cbbType.SelectedIndex < 0)
            {
                MessageBox.Show("奖品名称或奖品ID或宝箱错误！");
                return;
            }
            string sql = String.Format("insert into cq_lottery set type='{0}',{1};", ((ComboxItem)cbbType.SelectedItem).Values, Moyu.UpdateSqlBuild(groupLottery));

            MySqlHelper.Query(sql);
            MessageBox.Show("添加成功");
            FillLottery(Moyu.GetLottery());
        }
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (groupLottery.Tag == null) return;
            string sql = String.Format("delete from cq_lottery where id='{0}';", groupLottery.Tag.ToString());
            if (MySqlHelper.Query(sql) > 0) MessageBox.Show("删除成功");
            FillLottery(Moyu.GetLottery());

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = String.Format("select * from cq_lottery where prize_name like'%{0}%' or prize_item like'%{1}%';", txtSearch.Text, txtSearch.Text);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            FillLottery(dt);
        }


        private void btnSelectItem_Click(object sender, EventArgs e)
        {
            frmSelectItem frm = new frmSelectItem();
            frm.StartPosition = FormStartPosition.Manual;
            frm.StartPosition = FormStartPosition.CenterScreen;
            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                edit_prize_name.Text = frm.itemName;
                edit_prize_item.Text = frm.itemId;
            }
        }
        private void Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            Moyu.InputCheck(sender, e);
        }


    }
}
