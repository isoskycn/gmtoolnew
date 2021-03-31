using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmGenerator : Form
    {
        public frmGenerator()
        {
            InitializeComponent();
        }

        private void frmGenerator_Load(object sender, EventArgs e)
        {
            dgvMap.Rows.Clear();
            DataTable dt = Moyu.GetAllMapMonster();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvMap.Rows.Add();
                dgvMap.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvMap.Rows[index].Cells["name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvMap.Rows[index].Cells["num"].Value = dt.Rows[i]["num"];
            }
        }

        private void dgvMap_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvMap.SelectedRows.Count < 1) return;
            dgvGen.Rows.Clear();
            DataTable dt = Moyu.GetMapMonsterGen(Convert.ToInt64(dgvMap.CurrentRow.Cells["id"].Value));
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvGen.Rows.Add();
                dgvGen.Rows[index].Cells["gen_id"].Value = dt.Rows[i]["id"];
                dgvGen.Rows[index].Cells["mon_id"].Value = dt.Rows[i]["mon_id"];
                dgvGen.Rows[index].Cells["mon_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvGen.Rows[index].Cells["rest_secs"].Value = dt.Rows[i]["rest_secs"];
                dgvGen.Rows[index].Cells["max_per_gen"].Value = dt.Rows[i]["max_per_gen"];
                dgvGen.Rows[index].Cells["bound_x"].Value = dt.Rows[i]["bound_x"];
                dgvGen.Rows[index].Cells["bound_y"].Value = dt.Rows[i]["bound_y"];
                dgvGen.Rows[index].Cells["bound_cx"].Value = dt.Rows[i]["bound_cx"];
                dgvGen.Rows[index].Cells["bound_cy"].Value = dt.Rows[i]["bound_cy"];
                dgvGen.Rows[index].Cells["grid"].Value = dt.Rows[i]["grid"];
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvGen.Rows.Clear();
            DataTable dt = Moyu.GetMonsterGen(txtSearch.Text);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvGen.Rows.Add();
                dgvGen.Rows[index].Cells["gen_id"].Value = dt.Rows[i]["id"];
                dgvGen.Rows[index].Cells["mon_id"].Value = dt.Rows[i]["mon_id"];
                dgvGen.Rows[index].Cells["mon_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvGen.Rows[index].Cells["rest_secs"].Value = dt.Rows[i]["rest_secs"];
                dgvGen.Rows[index].Cells["max_per_gen"].Value = dt.Rows[i]["max_per_gen"];
                dgvGen.Rows[index].Cells["bound_x"].Value = dt.Rows[i]["bound_x"];
                dgvGen.Rows[index].Cells["bound_y"].Value = dt.Rows[i]["bound_y"];
                dgvGen.Rows[index].Cells["bound_cx"].Value = dt.Rows[i]["bound_cx"];
                dgvGen.Rows[index].Cells["bound_cy"].Value = dt.Rows[i]["bound_cy"];
                dgvGen.Rows[index].Cells["grid"].Value = dt.Rows[i]["grid"];

            }
        }

        private void miUpdateGen_Click(object sender, EventArgs e)
        {
            if (dgvGen.SelectedRows.Count < 1) return;
            string sql = String.Format("update cq_generator set rest_secs='{0}',max_per_gen='{1}',bound_x='{2}',bound_y='{3}',bound_cx='{4}',bound_cy='{5}',grid='{6}',npctype='{7}' where id='{8}';", dgvGen.CurrentRow.Cells["rest_secs"].Value.ToString(), dgvGen.CurrentRow.Cells["max_per_gen"].Value.ToString(), dgvGen.CurrentRow.Cells["bound_x"].Value.ToString(), dgvGen.CurrentRow.Cells["bound_y"].Value.ToString(), dgvGen.CurrentRow.Cells["bound_cx"].Value.ToString(), dgvGen.CurrentRow.Cells["bound_cy"].Value.ToString(), dgvGen.CurrentRow.Cells["grid"].Value.ToString(), dgvGen.CurrentRow.Cells["mon_id"].Value.ToString(), dgvGen.CurrentRow.Cells["gen_id"].Value.ToString());
            if (MySqlHelper.Query(sql) > 0) MessageBox.Show("修改成功!");
        }
        private void miAddGen_Click(object sender, EventArgs e)
        {
            if (dgvMap.SelectedRows.Count < 1) return;//
            frmSelectMonster frm = new frmSelectMonster();
            frm.StartPosition = FormStartPosition.Manual;
            frm.StartPosition = FormStartPosition.CenterScreen;
            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                string sql = String.Format("INSERT INTO `cq_generator` (`mapid`, `npctype`) VALUES ('{0}', '{1}');", dgvMap.CurrentRow.Cells["id"].Value.ToString(), frm.monsterId);
                if (MySqlHelper.Query(sql) > 0)
                {
                    dgvMap_CellMouseClick(null, null);
                    dgvGen.Sort(dgvGen.Columns["gen_id"], ListSortDirection.Ascending);//跳转到新增行
                    dgvGen.Rows[dgvGen.Rows.Count - 1].Selected = true;
                    dgvGen.FirstDisplayedScrollingRowIndex = dgvGen.Rows.Count - 1;
                }
            }
        }

        private void miDelGen_Click(object sender, EventArgs e)
        {
            if (dgvGen.SelectedRows.Count < 1) return;
            int num = 0;
            for (int i = dgvGen.SelectedRows.Count - 1; i >= 0; i--)
            {
                string sql = String.Format("delete from cq_generator where id='{0}';", dgvGen.SelectedRows[i].Cells["gen_id"].Value.ToString());
                num += MySqlHelper.Query(sql);
            }
            if (num > 0)
            {
                string str = String.Format("成功删除了{0}条数据!", num);
                MessageBox.Show(str);
                dgvMap_CellMouseClick(null, null);
            }
        }

        //修改怪物
        private void dgvGen_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvGen.CurrentCell.ColumnIndex == 1 || dgvGen.CurrentCell.ColumnIndex == 2)
            {
                frmSelectMonster frm = new frmSelectMonster();
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                DialogResult result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    int index = dgvGen.CurrentCell.RowIndex;
                    dgvGen.Rows[index].Cells[1].Value = frm.monsterId;
                    dgvGen.Rows[index].Cells[2].Value = frm.monsterName;
                }
            }
        }

    }
}
