using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmGodFire : Form
    {
        public frmGodFire()
        {
            InitializeComponent();
        }

        private void frmGodFire_Load(object sender, EventArgs e)
        {
            this.Text += String.Format(" - [{0}]", Moyu.playerName);
            FillUserGodFireInfo();
            FillUserGodFireItem();
            FillGodFireType();


            //超凡效果cbb
            DataTable dt = Moyu.GodexpList();
            cbbGod_exp.DataSource = Moyu.GodexpList();
            cbbGod_exp.DisplayMember = "info";
            cbbGod_exp.ValueMember = "id";
        }


        //填充神火类型列表
        private void FillGodFireType()
        {
            DataTable dt = Moyu.GetGodFireItemList();
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = String.Format("{0} [{1}号位]", System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]), dt.Rows[i]["firepostion"]);
                cbbFireType.Items.Add(new ComboxItem(name, dt.Rows[i]["id"].ToString()));
            }
        }


        //获取神火副属性
        private void GetFireAuxAttri(int itemtype)
        {
            cbbGemtype.Items.Clear();
            cbbAvailabletime.Items.Clear();
            cbbGod_strong.Items.Clear();
            DataTable dt = Moyu.GetFireAuxAttri(itemtype);
            cbbGemtype.Items.Add(new ComboxItem("无", "0"));
            cbbAvailabletime.Items.Add(new ComboxItem("无", "0"));
            cbbGod_strong.Items.Add(new ComboxItem("无", "0"));
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string type = "";
                switch (Convert.ToInt32(dt.Rows[i]["attribtype"]))
                {
                    case 1:
                        type = "暴击";
                        break;
                    case 2:
                        type = "爆伤";
                        break;
                    case 3:
                        type = "生命追加";
                        break;
                    case 4:
                        type = "韧性";
                        break;
                    case 5:
                        type = "穿透";
                        break;
                    case 6:
                        type = "神圣伤害";
                        break;
                    case 7:
                        type = "幻兽生命";
                        break;
                    case 8:
                        type = "幻兽韧性";
                        break;
                    case 9:
                        type = "幻兽穿透";
                        break;
                    case 10:
                        type = "幻兽神圣伤害";
                        break;
                    case 11:
                        type = "暴击率";
                        break;
                    case 12:
                        type = "暴伤率";
                        break;

                }
                string name = String.Format("{0}+{1} 评分:{2}", type, dt.Rows[i]["attribvalue"], dt.Rows[i]["point"]);
                cbbGemtype.Items.Add(new ComboxItem(name, dt.Rows[i]["id"].ToString()));
                cbbAvailabletime.Items.Add(new ComboxItem(name, dt.Rows[i]["id"].ToString()));
                cbbGod_strong.Items.Add(new ComboxItem(name, dt.Rows[i]["id"].ToString()));
            }
        }

        //填充套装效果
        private void FillFireSuitGroup(int itemtype_id)
        {
            cbbFiresuitgroup.Items.Clear();
            cbbFiresuitgroup.Items.Add(new ComboxItem("无", "0"));
            DataTable dt = Moyu.GetItemFireSuitGroup(itemtype_id);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = Moyu.GetFireSuitName(dt.Rows[i]["id"].ToString());
                string str = String.Format("{0} 评分:{1}", name, dt.Rows[i]["point"].ToString());
                cbbFiresuitgroup.Items.Add(new ComboxItem(str, dt.Rows[i]["id"].ToString()));
            }
        }

        //填充玩家神火物品
        private void FillUserGodFireItem()
        {
            dgvUserGodFire.Rows.Clear();
            DataTable dt = Moyu.GetUserGodfireItem(Moyu.player_id);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvUserGodFire.Rows.Add();
                dgvUserGodFire.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvUserGodFire.Rows[index].Cells["type"].Value = dt.Rows[i]["type"];
                dgvUserGodFire.Rows[index].Cells["gemtype"].Value = dt.Rows[i]["gemtype"];
                dgvUserGodFire.Rows[index].Cells["availabetime"].Value = dt.Rows[i]["availabletime"];
                dgvUserGodFire.Rows[index].Cells["god_strong"].Value = dt.Rows[i]["god_strong"];
                dgvUserGodFire.Rows[index].Cells["position"].Value = Moyu.GetItemPosition(Convert.ToInt32(dt.Rows[i]["position"]));
                dgvUserGodFire.Rows[index].Cells["firepostion"].Value = dt.Rows[i]["firepostion"];
                dgvUserGodFire.Rows[index].Cells["name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvUserGodFire.Rows[index].Cells["magic3"].Value = dt.Rows[i]["magic3"];
                dgvUserGodFire.Rows[index].Cells["god_exp"].Value = Moyu.GetGodexpName(dt.Rows[i]["god_exp"].ToString());
                dgvUserGodFire.Rows[index].Cells["firesuitgroupid"].Value = Moyu.GetFireSuitName(dt.Rows[i]["firesuitgroupid"].ToString());
                dgvUserGodFire.Rows[index].Cells["firesuitgroupid1"].Value = dt.Rows[i]["firesuitgroupid"];//真正的groupid,用于后面显示和修改
                dgvUserGodFire.Rows[index].Cells["god_exp1"].Value = dt.Rows[i]["god_exp"];
            }
        }

        //填充玩家神火信息
        private void FillUserGodFireInfo()
        {
            MySqlDataReader reader = Moyu.GetUserInfo(Moyu.playerName);
            try
            {
                while (reader.Read())
                {
                    cbbGodFireLev.SelectedIndex = reader.GetInt32("godfirelev");
                    switch (reader.GetInt32("godfire"))
                    {
                        case 0:
                            cbx1.Checked = false;
                            cbx2.Checked = false;
                            cbx3.Checked = false;
                            cbx4.Checked = false;
                            break;
                        case 1:
                            cbx1.Checked = true;
                            cbx2.Checked = false;
                            cbx3.Checked = false;
                            cbx4.Checked = false;
                            break;
                        case 2:
                            cbx1.Checked = false;
                            cbx2.Checked = true;
                            cbx3.Checked = false;
                            cbx4.Checked = false;
                            break;
                        case 3:
                            cbx1.Checked = true;
                            cbx2.Checked = true;
                            cbx3.Checked = false;
                            cbx4.Checked = false;
                            break;
                        case 4:
                            cbx1.Checked = false;
                            cbx2.Checked = false;
                            cbx3.Checked = true;
                            cbx4.Checked = false;
                            break;
                        case 5:
                            cbx1.Checked = true;
                            cbx2.Checked = false;
                            cbx3.Checked = true;
                            cbx4.Checked = false;
                            break;
                        case 6:
                            cbx1.Checked = false;
                            cbx2.Checked = true;
                            cbx3.Checked = true;
                            cbx4.Checked = false;
                            break;
                        case 7:
                            cbx1.Checked = true;
                            cbx2.Checked = true;
                            cbx3.Checked = true;
                            cbx4.Checked = false;
                            break;
                        case 8:
                            cbx1.Checked = false;
                            cbx2.Checked = false;
                            cbx3.Checked = false;
                            cbx4.Checked = true;
                            break;
                        case 9:
                            cbx1.Checked = true;
                            cbx2.Checked = false;
                            cbx3.Checked = false;
                            cbx4.Checked = true;
                            break;
                        case 10:
                            cbx1.Checked = false;
                            cbx2.Checked = true;
                            cbx3.Checked = false;
                            cbx4.Checked = true;
                            break;
                        case 11:
                            cbx1.Checked = true;
                            cbx2.Checked = true;
                            cbx3.Checked = false;
                            cbx4.Checked = true;
                            break;
                        case 12:
                            cbx1.Checked = false;
                            cbx2.Checked = false;
                            cbx3.Checked = true;
                            cbx4.Checked = true;
                            break;
                        case 13:
                            cbx1.Checked = true;
                            cbx2.Checked = false;
                            cbx3.Checked = true;
                            cbx4.Checked = true;
                            break;
                        case 14:
                            cbx1.Checked = false;
                            cbx2.Checked = true;
                            cbx3.Checked = true;
                            cbx4.Checked = true;
                            break;
                        case 15:
                            cbx1.Checked = true;
                            cbx2.Checked = true;
                            cbx3.Checked = true;
                            cbx4.Checked = true;
                            break;
                        default:
                            cbx1.Checked = false;
                            cbx2.Checked = false;
                            cbx3.Checked = false;
                            cbx4.Checked = false;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("查询数据出错：" + ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }


        private void dgvUserGodFire_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvUserGodFire.Rows.Count < 1) return;
            GetFireAuxAttri(Convert.ToInt32(dgvUserGodFire.CurrentRow.Cells["type"].Value));
            FillFireSuitGroup(Convert.ToInt32(dgvUserGodFire.CurrentRow.Cells["type"].Value));
            cbbFireType.SelectedIndex = Moyu.GetCbbIndex(cbbFireType, dgvUserGodFire.CurrentRow.Cells["type"].Value.ToString());
            edit_magic3.Value = Convert.ToInt32(dgvUserGodFire.CurrentRow.Cells["magic3"].Value);
            cbbFiresuitgroup.SelectedIndex = Moyu.GetCbbIndex(cbbFiresuitgroup, dgvUserGodFire.CurrentRow.Cells["firesuitgroupid1"].Value.ToString());
            cbbGemtype.SelectedIndex = Moyu.GetCbbIndex(cbbGemtype, dgvUserGodFire.CurrentRow.Cells["gemtype"].Value.ToString());
            cbbAvailabletime.SelectedIndex = Moyu.GetCbbIndex(cbbAvailabletime, dgvUserGodFire.CurrentRow.Cells["availabetime"].Value.ToString());
            cbbGod_strong.SelectedIndex = Moyu.GetCbbIndex(cbbGod_strong, dgvUserGodFire.CurrentRow.Cells["god_strong"].Value.ToString());
            cbbGod_exp.SelectedValue = dgvUserGodFire.CurrentRow.Cells["god_exp1"].Value;
            groupGodFireEdit.Tag = dgvUserGodFire.CurrentRow.Cells["id"].Value;//给个标签，用于判断是否修改神火属性
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //神火格子
            int godfire = 0;
            if (cbx1.Checked) godfire += Convert.ToInt32(cbx1.Tag);
            if (cbx2.Checked) godfire += Convert.ToInt32(cbx2.Tag);
            if (cbx3.Checked) godfire += Convert.ToInt32(cbx3.Tag);
            if (cbx4.Checked) godfire += Convert.ToInt32(cbx4.Tag);

            //神境界
            int godfirelev = cbbGodFireLev.SelectedIndex;
            string sql = String.Format("update cq_user_new set godfire='{0}',godfirelev='{1}' where id='{2}';", godfire, godfirelev, Moyu.player_id);
            MySqlHelper.Query(sql);
            //神火属性
            if (groupGodFireEdit.Tag == null) return;
            string type = ((ComboxItem)cbbFireType.SelectedItem).Values;
            string magic3 = edit_magic3.Value.ToString();
            string gemtype = ((ComboxItem)cbbGemtype.SelectedItem).Values;
            string availabletime = ((ComboxItem)cbbAvailabletime.SelectedItem).Values;
            string god_strong = ((ComboxItem)cbbGod_strong.SelectedItem).Values;
            string firesuitgroupid = ((ComboxItem)cbbFiresuitgroup.SelectedItem).Values;
            string god_exp = cbbGod_exp.SelectedValue.ToString();
            string sql2 = String.Format("update cq_item set type='{0}',magic3='{1}',gemtype='{2}',availabletime='{3}',god_strong='{4}',god_exp='{5}',firesuitgroupid='{6}' where id='{7}';", type, magic3, gemtype, availabletime, god_strong, god_exp, firesuitgroupid, groupGodFireEdit.Tag.ToString());
            MySqlHelper.Query(sql2);
            MessageBox.Show("修改成功!");
            FillUserGodFireItem();
        }

        private void cbbFireType_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetFireAuxAttri(Convert.ToInt32(((ComboxItem)cbbFireType.SelectedItem).Values));
            FillFireSuitGroup(Convert.ToInt32(((ComboxItem)cbbFireType.SelectedItem).Values));
            cbbGemtype.SelectedIndex = 0;
            cbbAvailabletime.SelectedIndex = 0;
            cbbGod_strong.SelectedIndex = 0;
            cbbFiresuitgroup.SelectedIndex = 0;
            cbbGod_exp.SelectedIndex = 0;
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvUserGodFire.CurrentRow.Cells[0].Value == null) return;
            DialogResult dr = MessageBox.Show("确认删除吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            if (Moyu.DelItem(Convert.ToInt32(dgvUserGodFire.CurrentRow.Cells["id"].Value)) > 0)
                MessageBox.Show("删除成功!");
            FillUserGodFireItem();
        }
    }
}
