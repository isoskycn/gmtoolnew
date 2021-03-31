using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmDropItem : Form
    {
        public frmDropItem()
        {
            InitializeComponent();
        }

        private void frmDropItem_Load(object sender, EventArgs e)
        {
            FillMapMonster();
        }

        private void FillMapMonster()
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
            //if (dgvMap.CurrentRow.Cells["id"] == null) return;
            dgvMonster.Rows.Clear();
            DataTable dt = Moyu.GetMapMonster(Convert.ToInt64(dgvMap.CurrentRow.Cells["id"].Value));
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvMonster.Rows.Add();
                dgvMonster.Rows[index].Cells["monster_id"].Value = dt.Rows[i]["id"];
                dgvMonster.Rows[index].Cells["monster_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvMonster.Rows[index].Cells["rest_secs"].Value = dt.Rows[i]["rest_secs"];
                dgvMonster.Rows[index].Cells["max_per_gen"].Value = dt.Rows[i]["num"];
                dgvMonster.Rows[index].Cells["drop_money_chance"].Value = dt.Rows[i]["drop_money_chance"];
                dgvMonster.Rows[index].Cells["drop_money_min"].Value = dt.Rows[i]["drop_money_min"];
                dgvMonster.Rows[index].Cells["drop_money_max"].Value = dt.Rows[i]["drop_money_max"];
                dgvMonster.Rows[index].Cells["drop_item_chance"].Value = dt.Rows[i]["drop_item_chance"];
                dgvMonster.Rows[index].Cells["explode_item_chance1"].Value = dt.Rows[i]["explode_item_chance1"];
                dgvMonster.Rows[index].Cells["explode_item_chance2"].Value = dt.Rows[i]["explode_item_chance2"];
                dgvMonster.Rows[index].Cells["explode_item_chance3"].Value = dt.Rows[i]["explode_item_chance3"];
                dgvMonster.Rows[index].Cells["drop_item_rule"].Value = dt.Rows[i]["drop_item_rule"];
                dgvMonster.Rows[index].Cells["action"].Value = dt.Rows[i]["action"];

            }
        }

        private void dgvMonster_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvDropItemGroup.Rows.Clear();
            dgvItem.Rows.Clear();
            dgvActionItem.Rows.Clear();
            if (dgvMonster.SelectedRows.Count < 1) return;
            //if (dgvMonster.CurrentRow.Cells["monster_id"] == null || Convert.ToInt32(dgvMonster.CurrentRow.Cells["drop_item_rule"].Value) == 0) return;
            //脚本物品
            btnAction.Tag = dgvMonster.CurrentRow.Cells["action"].Value;
            if (Convert.ToInt32(dgvMonster.CurrentRow.Cells["action"].Value) != 0)
            {
                try
                {//List<string> strList = Moyu.GetMonsterActionDropItem(20172317);
                    List<string> strList = Moyu.GetMonsterActionDropItem(Convert.ToInt32(dgvMonster.CurrentRow.Cells["action"].Value));
                    foreach (string s in strList)
                    {
                        int index = dgvActionItem.Rows.Add();
                        dgvActionItem.Rows[index].Cells["action_itemid"].Value = s;
                        dgvActionItem.Rows[index].Cells["action_itemname"].Value = Moyu.ItemId2Name(Convert.ToInt32(s));
                    }
                    Moyu.strList.Clear();//用完清空,否则会导致重复叠加
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //掉落规则
            DataTable dt = Moyu.GetDropitemGroup(Convert.ToInt32(dgvMonster.CurrentRow.Cells["drop_item_rule"].Value));
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvDropItemGroup.Rows.Add();
                dgvDropItemGroup.Rows[index].Cells["dropitem_id"].Value = dt.Rows[i]["id"];
                dgvDropItemGroup.Rows[index].Cells["ruleid"].Value = dt.Rows[i]["ruleid"];
                dgvDropItemGroup.Rows[index].Cells["dropitem_num"].Value = dt.Rows[i]["itemnum"];
                dgvDropItemGroup.Rows[index].Cells["dropitem_chance"].Value = dt.Rows[i]["chance"];

            }
        }



        private void dgvDropItemGroup_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvItem.Rows.Clear();
            if (dgvDropItemGroup.SelectedRows.Count < 1) return;
            //if (dgvDropItemGroup.CurrentRow.Cells["dropitem_id"] == null) return;
            dgvItem.Tag = dgvDropItemGroup.CurrentRow.Cells["dropitem_id"].Value;
            DataTable dt = Moyu.GetDropitemInfo(Convert.ToInt32(dgvDropItemGroup.CurrentRow.Cells["dropitem_id"].Value));
            dgvItem.Rows.Add(15);
            dgvItem.Rows[0].Cells["item_id"].Value = dt.Rows[0]["item0"];
            dgvItem.Rows[0].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item0"]));
            dgvItem.Rows[1].Cells["item_id"].Value = dt.Rows[0]["item1"];
            dgvItem.Rows[1].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item1"]));
            dgvItem.Rows[2].Cells["item_id"].Value = dt.Rows[0]["item2"];
            dgvItem.Rows[2].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item2"]));
            dgvItem.Rows[3].Cells["item_id"].Value = dt.Rows[0]["item3"];
            dgvItem.Rows[3].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item3"]));
            dgvItem.Rows[4].Cells["item_id"].Value = dt.Rows[0]["item4"];
            dgvItem.Rows[4].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item4"]));
            dgvItem.Rows[5].Cells["item_id"].Value = dt.Rows[0]["item5"];
            dgvItem.Rows[5].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item5"]));
            dgvItem.Rows[6].Cells["item_id"].Value = dt.Rows[0]["item6"];
            dgvItem.Rows[6].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item6"]));
            dgvItem.Rows[7].Cells["item_id"].Value = dt.Rows[0]["item7"];
            dgvItem.Rows[7].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item7"]));
            dgvItem.Rows[8].Cells["item_id"].Value = dt.Rows[0]["item8"];
            dgvItem.Rows[8].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item8"]));
            dgvItem.Rows[9].Cells["item_id"].Value = dt.Rows[0]["item9"];
            dgvItem.Rows[9].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item9"]));
            dgvItem.Rows[10].Cells["item_id"].Value = dt.Rows[0]["item10"];
            dgvItem.Rows[10].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item10"]));
            dgvItem.Rows[11].Cells["item_id"].Value = dt.Rows[0]["item11"];
            dgvItem.Rows[11].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item11"]));
            dgvItem.Rows[12].Cells["item_id"].Value = dt.Rows[0]["item12"];
            dgvItem.Rows[12].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item12"]));
            dgvItem.Rows[13].Cells["item_id"].Value = dt.Rows[0]["item13"];
            dgvItem.Rows[13].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item13"]));
            dgvItem.Rows[14].Cells["item_id"].Value = dt.Rows[0]["item14"];
            dgvItem.Rows[14].Cells["item_name"].Value = Moyu.ItemId2Name(Convert.ToInt32(dt.Rows[0]["item14"]));


        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(btnAction.Tag) == 0) return;

            if (Application.OpenForms["frmAction"] == null)
            {
                frmAction frm = new frmAction(btnAction.Tag.ToString());
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                if (Application.OpenForms["frmAction"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmActionc"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmAction"].Activate();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvMonster.Rows.Clear();
            DataTable dt = Moyu.GetMonsterType(txtSearch.Text);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvMonster.Rows.Add();
                dgvMonster.Rows[index].Cells["monster_id"].Value = dt.Rows[i]["id"];
                dgvMonster.Rows[index].Cells["monster_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvMonster.Rows[index].Cells["drop_money_chance"].Value = dt.Rows[i]["drop_money_chance"];
                dgvMonster.Rows[index].Cells["drop_money_min"].Value = dt.Rows[i]["drop_money_min"];
                dgvMonster.Rows[index].Cells["drop_money_max"].Value = dt.Rows[i]["drop_money_max"];
                dgvMonster.Rows[index].Cells["drop_item_chance"].Value = dt.Rows[i]["drop_item_chance"];
                dgvMonster.Rows[index].Cells["explode_item_chance1"].Value = dt.Rows[i]["explode_item_chance1"];
                dgvMonster.Rows[index].Cells["explode_item_chance2"].Value = dt.Rows[i]["explode_item_chance2"];
                dgvMonster.Rows[index].Cells["explode_item_chance3"].Value = dt.Rows[i]["explode_item_chance3"];
                dgvMonster.Rows[index].Cells["drop_item_rule"].Value = dt.Rows[i]["drop_item_rule"];
                dgvMonster.Rows[index].Cells["action"].Value = dt.Rows[i]["action"];

            }

        }

        //物品菜单
        private void miSelectItem_Click(object sender, EventArgs e)
        {
            if (dgvItem.SelectedRows.Count < 1) return;
            frmSelectItem frm = new frmSelectItem();
            frm.StartPosition = FormStartPosition.Manual;
            frm.StartPosition = FormStartPosition.CenterScreen;
            DialogResult result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                dgvItem.CurrentRow.Cells["item_id"].Value = frm.itemId;
                dgvItem.CurrentRow.Cells["item_name"].Value = frm.itemName;
                int index = dgvItem.CurrentRow.Index;
                string sql = String.Format("update cq_dropitemrule set item{0}='{1}' where id='{2}';", index, frm.itemId, dgvItem.Tag.ToString());
                MySqlHelper.Query(sql);
            }
        }

        private void miDelItem_Click(object sender, EventArgs e)
        {
            if (dgvItem.SelectedRows.Count < 1) return;
            dgvItem.CurrentRow.Cells["item_id"].Value = "0";
            dgvItem.CurrentRow.Cells["item_name"].Value = "无";
            int index = dgvItem.CurrentRow.Index;
            string sql = String.Format("update cq_dropitemrule set item{0}='0' where id='{1}';", index, dgvItem.Tag.ToString());
            MySqlHelper.Query(sql);
        }

        //规则菜单
        private void miAddRule_Click(object sender, EventArgs e)
        {
            if (dgvMonster.SelectedRows.Count < 1 || Convert.ToInt32(dgvMonster.CurrentRow.Cells["drop_item_rule"].Value) < 1)
            {
                MessageBox.Show("规则编号不能为0!如果需要添加怪物爆率,可先在怪物的掉落规则自定义一个数字,然后再添加规则!");
                return;
            }

            int ruleid = dgvDropItemGroup.Rows.Count + 1;//根据行数生成ruleid
            string sql = String.Format("INSERT INTO `cq_dropitemrule` (`group_id`, `ruleid`, `chance`) VALUES ('{0}','{1}','1000000');", dgvMonster.CurrentRow.Cells["drop_item_rule"].Value, ruleid);
            string sql2 = String.Format("update cq_monstertype set drop_item_rule='{0}' where id='{1}';", dgvMonster.CurrentRow.Cells["drop_item_rule"].Value.ToString(), dgvMonster.CurrentRow.Cells["monster_id"].Value.ToString());
            if (MySqlHelper.Query(sql) > 0 && MySqlHelper.Query(sql2) > 0)
            {
                dgvMonster_CellMouseClick(null, null);
            }
        }

        private void miDelRule_Click(object sender, EventArgs e)
        {
            if (dgvDropItemGroup.SelectedRows.Count < 1) return;
            string sql = String.Format("delete from cq_dropitemrule where id='{0}';", dgvDropItemGroup.CurrentRow.Cells["dropitem_id"].Value.ToString());
            if (MySqlHelper.Query(sql) > 0)
            {
                dgvMonster_CellMouseClick(null, null);
            }
        }

        private void miUpdateRule_Click(object sender, EventArgs e)
        {
            if (dgvDropItemGroup.SelectedRows.Count < 1) return;
            string sql = String.Format("update cq_dropitemrule set chance='{0}',ruleid='{1}' where id='{2}';", dgvDropItemGroup.CurrentRow.Cells["dropitem_chance"].Value.ToString(), dgvDropItemGroup.CurrentRow.Cells["ruleid"].Value.ToString(), dgvDropItemGroup.CurrentRow.Cells["dropitem_id"].Value.ToString());
            if (MySqlHelper.Query(sql) > 0)
            {
                dgvMonster_CellMouseClick(null, null);
            }
        }

        //怪物菜单
        private void miUpdateMonster_Click(object sender, EventArgs e)
        {
            if (dgvMonster.SelectedRows.Count < 1) return;
            string sql = String.Format("update cq_monstertype set drop_money_chance='{0}',drop_money_min='{1}',drop_money_max='{2}',drop_item_chance='{3}',explode_item_chance1='{4}'," +
                "explode_item_chance2='{5}',explode_item_chance3='{6}',drop_item_rule='{7}' where id='{8}';", dgvMonster.CurrentRow.Cells[4].Value.ToString(), dgvMonster.CurrentRow.Cells[5].Value.ToString(), dgvMonster.CurrentRow.Cells[6].Value.ToString(), dgvMonster.CurrentRow.Cells[7].Value.ToString(), dgvMonster.CurrentRow.Cells[8].Value.ToString(), dgvMonster.CurrentRow.Cells[9].Value.ToString(), dgvMonster.CurrentRow.Cells[10].Value.ToString(), dgvMonster.CurrentRow.Cells[11].Value.ToString(), dgvMonster.CurrentRow.Cells[0].Value.ToString());
            if (MySqlHelper.Query(sql) > 0)
            {
                MessageBox.Show("修改成功!");
            }
        }
    }
}
