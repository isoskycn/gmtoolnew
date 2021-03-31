using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmUserItemEdit : Form
    {
        public frmUserItemEdit()
        {
            InitializeComponent();
        }

        //输入检测
        private void InPutNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            Moyu.InputCheck(sender, e);
        }

        private void frmUserItemEdit_Load(object sender, EventArgs e)
        {
            if (Moyu.accountName != null)
            {
                txtFindAccountItem.Text = Moyu.accountName;
                txtFindPlayerItem.Text = Moyu.playerName;
                btnFindUserItem.PerformClick();
            }
            //宝石combobox
            ComboxItem[] gem = {
                new ComboxItem("无","0"),
                new ComboxItem("已开洞无宝石","255"),
                new ComboxItem("战斗力+1","16"),
                new ComboxItem("战斗力+3","17"),
                new ComboxItem("战斗力+5","18"),
                new ComboxItem("伤害-5%","22"),
                new ComboxItem("伤害-10%","23"),
                new ComboxItem("伤害-15%","24"),
                new ComboxItem("升级经验+10%","28"),
                new ComboxItem("升级经验+15%","29"),
                new ComboxItem("升级经验+25%","30"),
                new ComboxItem("攻击+5%","19"),
                new ComboxItem("攻击+10%","20"),
                new ComboxItem("攻击+20%","21"),
                new ComboxItem("耐久+20%","25"),
                new ComboxItem("耐久+50%","26"),
                new ComboxItem("耐久+100%","27")
            };
            cbbGem1.Items.AddRange(gem);
            cbbGem2.Items.AddRange(gem);
            cbbGem3.Items.AddRange(gem);
            //器灵combobox
            ComboxItem[] newgem = {
                new ComboxItem("无","0"),
                new ComboxItem("阿拉玛之魂·1级","1701630"),
                new ComboxItem("阿拉玛之魂·2级","1702630"),
                new ComboxItem("阿拉玛之魂·3级","1703630"),
                new ComboxItem("阿拉玛之魂·4级","1704630"),
                new ComboxItem("阿拉玛之魂·5级","1705630"),
                new ComboxItem("阿拉玛之魂·6级","1706630"),
                new ComboxItem("阿拉玛之魂·7级","1707630"),
                new ComboxItem("阿拉玛之魂·8级","1708630"),
                new ComboxItem("阿拉玛之魂·9级","1709630"),
                new ComboxItem("阿拉玛之魂·10级","1710630"),
                new ComboxItem("阿拉玛之魂·11级","1711630"),
                new ComboxItem("阿拉玛之魂·12级","1712630"),
                new ComboxItem("阿拉玛之魂·13级","1713630"),
                new ComboxItem("阿拉玛之魂·14级","1714630"),
                new ComboxItem("阿拉玛之魂·15级","1715630"),
                new ComboxItem("阿拉玛之魂·16级","1716630"),
                new ComboxItem("阿拉玛之魂·17级","1717630"),
                new ComboxItem("阿拉玛之魂·18级","1718630"),
            };
            cbbNewgem.Items.AddRange(newgem);
        }

        //查找用户
        private void btnFindUserItem_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
            string accountName = txtFindAccountItem.Text.Trim();
            string playerName = txtFindPlayerItem.Text.Trim();
            if (string.IsNullOrEmpty(accountName) && string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("请输入账号或者人物名!");
                return;
            }
            //清空控件
            dgvUserItem.Rows.Clear();
            listSubPlayer.Items.Clear();
            Moyu.ClearInput(groupAmorEdit);
            Moyu.ClearInput(groupUserAmor);

            try
            {
                DataTable dt = new DataTable();
                if (string.IsNullOrEmpty(accountName))
                {
                    dt = Moyu.FindUserByAccountOrName(playerName, 1);
                }
                else
                {
                    dt = Moyu.FindUserByAccountOrName(accountName);
                }
                //小号列表处理
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    listSubPlayer.Items.Add(System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]));
                }
                listSubPlayer.SelectedItem = listSubPlayer.Items[0];

                //属性处理
                DataRow[] dr = dt.Select("type='0'");//过滤小号,默认显示主号角色属性信息
                int id = Convert.ToInt32(dr[0]["id"]);
                FillUserItem(id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //填充玩家物品
        public void FillUserItem(int player_id)
        {
            dgvUserItem.Rows.Clear();
            Moyu.ClearInput(groupUserAmor);

            string sql = String.Format("select a.id,a.magic3,a.position,a.type,b.name,b.req_profession,b.req_level from cq_item as a left join cq_itemtype as b on a.type=b.id where a.player_id='{0}' order by a.position;", player_id.ToString());
            MySqlDataReader reader = MySqlHelper.Select(sql);
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        int index = dgvUserItem.Rows.Add();
                        dgvUserItem.Rows[index].Cells["itemId"].Value = reader.GetString("id");
                        dgvUserItem.Rows[index].Cells["itemName"].Value = reader.GetString("name");
                        if (reader.GetInt32("req_profession") > 0 && reader.GetInt32("req_level") > 1) dgvUserItem.Rows[index].Cells["itemIsAmor"].Value = 1;//给装备物品标记，以便双击修改时筛选

                        //位置
                        int p = reader.GetInt32("position");
                        string position = p.ToString();
                        switch (p)
                        {
                            case 1:
                                position = "头盔";
                                txtToukui.Tag = reader.GetInt32("id");//给个tag，以便双击修改装备时传递。
                                txtToukui.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                txtToukui.ForeColor = Moyu.GetItemDisplayColor(reader.GetInt32("type"));
                                break;
                            case 2:
                                position = "项链";
                                txtXiangLian.Tag = reader.GetInt32("id");
                                txtXiangLian.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                txtXiangLian.ForeColor = Moyu.GetItemDisplayColor(reader.GetInt32("type"));
                                break;
                            case 3:
                                position = "衣服";
                                txtYiFu.Tag = reader.GetInt32("id");
                                txtYiFu.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                txtYiFu.ForeColor = Moyu.GetItemDisplayColor(reader.GetInt32("type"));
                                break;
                            case 4:
                                position = "武器";
                                txtWuQi.Tag = reader.GetInt32("id");
                                txtWuQi.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                txtWuQi.ForeColor = Moyu.GetItemDisplayColor(reader.GetInt32("type"));
                                break;
                            case 7:
                                position = "手镯";
                                txtShouZhuo.Tag = reader.GetInt32("id");
                                txtShouZhuo.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                txtShouZhuo.ForeColor = Moyu.GetItemDisplayColor(reader.GetInt32("type"));
                                break;
                            case 8:
                                position = "靴子";
                                txtXueZi.Tag = reader.GetInt32("id");
                                txtXueZi.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                txtXueZi.ForeColor = Moyu.GetItemDisplayColor(reader.GetInt32("type"));
                                break;
                            case 9:
                                position = "婚戒";
                                txtHunJie.Text = reader.GetString("name");
                                break;
                            case 12:
                                position = "外套";
                                txtWaiTao.Text = reader.GetString("name");
                                break;
                            case 13:
                                position = "法宝";
                                txtGongFabao.Tag = reader.GetInt32("id");
                                txtGongFabao.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                break;
                            case 14:
                                position = "法宝";
                                txtShangFabao.Tag = reader.GetInt32("id");
                                txtShangFabao.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                break;
                            case 15:
                                position = "法宝";
                                txtZhanFabao.Tag = reader.GetInt32("id");
                                txtZhanFabao.Text = Moyu.GetItemDisplayName(reader.GetInt32("type"), reader.GetString("name"), reader.GetInt32("magic3"));
                                break;
                            case 26:
                                position = "幻魂";
                                txtHuanHun.Text = reader.GetString("name");
                                break;
                            case 27:
                                position = "神火1号格子";
                                break;
                            case 28:
                                position = "神火2号格子";
                                break;
                            case 29:
                                position = "神火3号格子";
                                break;
                            case 30:
                                position = "神火4号格子";
                                break;
                            case 31:
                                position = "神火5号格子";
                                break;
                            case 32:
                                position = "神火6号格子";
                                break;
                            case 33:
                                position = "神火7号格子";
                                break;
                            case 34:
                                position = "神火8号格子";
                                break;
                            case 43:
                                position = "神火背包";
                                break;
                            case 44:
                                position = "衣柜";
                                break;
                            case 49:
                                position = "衣柜";
                                break;
                            case 50:
                                position = "背包";
                                break;
                            case 201:
                                position = "仓库";
                                break;

                        }
                        dgvUserItem.Rows[index].Cells["itemPosition"].Value = position;

                    }
                    else
                    {
                        MessageBox.Show("没有这个角色的物品数据!");
                    }
                }
            }
            catch (Exception es)
            {
                MessageBox.Show("查询失败了！" + es.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        public void SetComboBoxSelectItemByValue(ComboBox cb, string Value)
        {
            for (int i = 0; i < cb.Items.Count; ++i)
            {
                if (cb.ValueMember.ToString() == Value)
                {
                    cb.SelectedIndex = i;
                    break;
                }
                else
                    continue;
            }
        }
        //填充装备修改信息
        public void FillAmorEdit(int item_id)
        {
            cbxMonopoly.Checked = false;
            cbxIdent.Checked = false;
            dtpPrescription.Checked = false;
            cbbAmorLevel.Items.Clear();

            DataTable dt = Moyu.GetItemInfo(item_id);
            cbbAmorLevel.Tag = item_id;//给个tag，以便提交修改装备时传递。
            edit_forgename.Text = dt.Rows[0]["forgename"].ToString();
            edit_eudemon_attack1.Text = dt.Rows[0]["eudemon_attack1"].ToString();
            edit_eudemon_attack2.Text = dt.Rows[0]["eudemon_attack2"].ToString();
            edit_eudemon_attack3.Text = dt.Rows[0]["eudemon_attack3"].ToString();
            edit_eudemon_attack4.Text = dt.Rows[0]["eudemon_attack4"].ToString();
            edit_magic3.Text = dt.Rows[0]["magic3"].ToString();

            if (Convert.ToInt32(dt.Rows[0]["monopoly"]) != 0) cbxMonopoly.Checked = true;//是否赠品
            if (Convert.ToInt32(dt.Rows[0]["ident"]) != 0) cbxIdent.Checked = true;//永不磨损
            cbbAmorQuality.SelectedIndex = Convert.ToInt32(dt.Rows[0]["type"]) % 10;//装备品质
            cbbGem1.SelectedIndex = Moyu.GetCbbIndex(cbbGem1, dt.Rows[0]["gem1"].ToString());//宝石
            cbbGem2.SelectedIndex = Moyu.GetCbbIndex(cbbGem2, dt.Rows[0]["gem2"].ToString());
            //战魂
            double war = Convert.ToInt32(dt.Rows[0]["warghostexp"]);
            int x = (int)Math.Pow((float)war / (float)2, (float)1 / (float)3);
            cbbWarghostexp.SelectedIndex = x;
            //装备等级
            DataTable dt2 = Moyu.GetItemLevelList(Convert.ToInt32(dt.Rows[0]["type"]));
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                string levelName = String.Format("{0}[{1}级]", System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt2.Rows[i]["name"]), dt2.Rows[i]["req_level"].ToString());
                cbbAmorLevel.Items.Add(new ComboxItem(levelName, dt2.Rows[i]["id"].ToString()));
            }
            string idItem = dt.Rows[0]["type"].ToString();
            idItem = idItem.Remove(idItem.Length - 1, 1) + "0";//因为组合框里面的值都是白品（0),所以要修改item_id的最后一位来获得索引
            cbbAmorLevel.SelectedIndex = Moyu.GetCbbIndex(cbbAmorLevel, idItem);
            //新端处理
            if (Moyu.IsNewDB)
            {
                cbbGem3.SelectedIndex = Moyu.GetCbbIndex(cbbGem3, dt.Rows[0]["gem3"].ToString());//3洞
                cbbNewgem.SelectedIndex = Moyu.GetCbbIndex(cbbNewgem, dt.Rows[0]["newgem"].ToString());//器灵
                //时效
                int p = Convert.ToInt32(dt.Rows[0]["prescription"]);
                if (p > 0)
                {
                    dtpPrescription.Checked = true;
                    dtpPrescription.Value = DateTime.ParseExact(p.ToString(), "yyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
                }
                //神佑                                                              
                string exp = dt.Rows[0]["god_exp"].ToString();
                if (exp.Length > 4)
                {
                    edit_god_exp.Text = exp.Remove(exp.Length - 4, 4);
                    edit_god_exp2.Text = exp.Substring(exp.Length - 4, 4);
                }
                else
                {
                    edit_god_exp.Text = "0";
                    edit_god_exp2.Text = exp;
                }
            }
        }

        private void listSubPlayer_MouseClick(object sender, MouseEventArgs e)
        {
            if (listSubPlayer.Items.Count < 1) return;
            FillUserItem(Moyu.PlayName2Id(listSubPlayer.SelectedItem.ToString()));
        }

        private void txtToukui_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtToukui.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtToukui.Tag));
        }

        private void txtWuQi_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtWuQi.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtWuQi.Tag));
        }

        private void txtXiangLian_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtXiangLian.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtXiangLian.Tag));
        }

        private void txtShouZhuo_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtShouZhuo.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtShouZhuo.Tag));
        }

        private void txtYiFu_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtYiFu.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtYiFu.Tag));
        }

        private void txtXueZi_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtXueZi.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtXueZi.Tag));
        }

        private void txtZhanFabao_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtZhanFabao.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtZhanFabao.Tag));
        }

        private void txtGongFabao_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtGongFabao.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtGongFabao.Tag));
        }

        private void txtShangFabao_MouseClick(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(txtShangFabao.Text)) return;
            FillAmorEdit(Convert.ToInt32(txtShangFabao.Tag));
        }

        private void btnUpdateAmor_Click(object sender, EventArgs e)
        {
            if (cbbAmorLevel.Tag == null)
            {
                MessageBox.Show("没有需要修改的装备信息！");
                return;
            }
            string sql = String.Format("update cq_item set availabletime=12345678,{0}", Moyu.UpdateSqlBuild(groupAmorEdit));
            //战魂
            int war = (int)(Math.Pow(Convert.ToInt32(cbbWarghostexp.SelectedIndex), 3) * 2);
            sql += String.Format(",warghostexp='{0}'", war.ToString());
            //品质
            string str = ((ComboxItem)cbbAmorLevel.SelectedItem).Values;
            str = str.Remove(str.Length - 1, 1) + cbbAmorQuality.SelectedIndex.ToString();
            if (!Moyu.IsNewDB && cbbAmorQuality.SelectedIndex > 4) str = str.Remove(str.Length - 1, 1) + "4";//老端最高极品
            sql += String.Format(",type='{0}'", str);
            //宝石
            sql += String.Format(",gem1='{0}',gem2='{1}'", ((ComboxItem)cbbGem1.SelectedItem).Values, ((ComboxItem)cbbGem2.SelectedItem).Values);
            //赠品
            if (cbxMonopoly.Checked == true)
            {
                sql += ",monopoly='1'";
            }
            else
            {
                sql += ",monopoly='0'";
            }
            //耐久
            if (cbxIdent.Checked == true)
            {
                sql += ",ident='4'";
            }
            else
            {
                sql += ",ident='0'";
            }
            //新端
            if (Moyu.IsNewDB)
            {
                if (dtpPrescription.Checked)
                {
                    sql += String.Format(",prescription='{0}'", dtpPrescription.Value.ToString("yyMMddHHmm"));
                }
                else
                {
                    sql += ",prescription='0'";
                }
                sql += String.Format(",gem3='{0}'", ((ComboxItem)cbbGem3.SelectedItem).Values);
                //神佑
                int exp = Convert.ToInt32(edit_god_exp.Text) * 10000 + Convert.ToInt32(edit_god_exp2.Text);
                sql += String.Format(",god_exp='{0}'", exp.ToString());
                //器灵
                sql += String.Format(",newgem='{0}'", ((ComboxItem)cbbNewgem.SelectedItem).Values);
            }
            sql += String.Format(" where id='{0}';", Convert.ToInt32(cbbAmorLevel.Tag));
            MySqlHelper.Query(sql);
            MessageBox.Show("装备修改成功!");
            FillUserItem(Moyu.PlayName2Id(listSubPlayer.SelectedItem.ToString()));
        }

        private void dgvUserItem_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvUserItem.SelectedRows.Count < 1) return;
            if (dgvUserItem.CurrentRow.Cells["itemIsAmor"].Value != null)
            {
                FillAmorEdit(Convert.ToInt32(dgvUserItem.CurrentRow.Cells["itemID"].Value));
            }
        }

        private void miUserItemEdit_Click(object sender, EventArgs e)
        {
            if (dgvUserItem.SelectedRows.Count < 1) return;
            int index = dgvUserItem.CurrentRow.Index;
            if (dgvUserItem.CurrentRow.Cells["itemIsAmor"].Value != null)
            {
                FillAmorEdit(Convert.ToInt32(dgvUserItem.CurrentRow.Cells["itemID"].Value));
            }
            else
            {
                MessageBox.Show("这件物品不是装备，无法修改!");
            }
        }

        private void miUserItemDel_Click(object sender, EventArgs e)
        {
            if (dgvUserItem.SelectedRows.Count < 1) return;
            int id = Convert.ToInt32(dgvUserItem.CurrentRow.Cells["itemId"].Value);
            DialogResult dr = MessageBox.Show("确认删除吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            if (Moyu.DelItem(id) > 0) MessageBox.Show("删除成功");
            FillUserItem(Moyu.PlayName2Id(listSubPlayer.SelectedItem.ToString()));
        }

        private void btnAmorList_Click(object sender, EventArgs e)
        {
            dgvItemList.Rows.Clear();
            DataTable dt = Moyu.GetItemList(1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvItemList.Rows.Add();
                dgvItemList.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvItemList.Rows[index].Cells["gem1"].Value = dt.Rows[i]["gem1"];
                dgvItemList.Rows[index].Cells["amount"].Value = dt.Rows[i]["amount"];
                dgvItemList.Rows[index].Cells["amount_limit"].Value = dt.Rows[i]["amount_limit"];
                int id = Convert.ToInt32(dt.Rows[i]["id"]);
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvItemList.Rows[index].Cells["name"].Value = Moyu.GetItemDisplayName(id, name);
            }
        }

        private void btnItemList_Click(object sender, EventArgs e)
        {
            dgvItemList.Rows.Clear();
            DataTable dt = Moyu.GetItemList();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvItemList.Rows.Add();
                dgvItemList.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvItemList.Rows[index].Cells["gem1"].Value = dt.Rows[i]["gem1"];
                dgvItemList.Rows[index].Cells["amount"].Value = dt.Rows[i]["amount"];
                dgvItemList.Rows[index].Cells["amount_limit"].Value = dt.Rows[i]["amount_limit"];
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvItemList.Rows[index].Cells["name"].Value = name;
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
                dgvItemList.Rows[index].Cells["gem1"].Value = dt.Rows[i]["gem1"];
                dgvItemList.Rows[index].Cells["amount"].Value = dt.Rows[i]["amount"];
                dgvItemList.Rows[index].Cells["amount_limit"].Value = dt.Rows[i]["amount_limit"];
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvItemList.Rows[index].Cells["name"].Value = name;
            }
        }

        //刷物品
        private void dgvItemList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Moyu.GiveItem
            int index = dgvItemList.CurrentRow.Index;
            if (dgvItemList.Rows[index].Cells["id"].Value == null || listSubPlayer.Items.Count < 1)
            {
                return;
            }
            string str = Interaction.InputBox("请输入要刷物品的数量(装备类物品将固定为1):", "提示", "1", -1, -1);
            if (str == "") return;
            try
            {
                int num = int.Parse(str);
                int player_id = Moyu.PlayName2Id(listSubPlayer.SelectedItem.ToString());
                int itemtype_id = Convert.ToInt32(dgvItemList.Rows[index].Cells["id"].Value);
                int gem1 = Convert.ToInt32(dgvItemList.Rows[index].Cells["gem1"].Value);
                int amount = Convert.ToInt32(dgvItemList.Rows[index].Cells["amount"].Value);
                int amount_limit = Convert.ToInt32(dgvItemList.Rows[index].Cells["amount_limit"].Value);
                if (amount == 1 && amount == amount_limit)//不可叠加类物品
                {
                    for (int i = 0; i < num; i++)
                    {
                        Moyu.GiveItem(player_id, itemtype_id, amount, gem1);
                    }
                    FillUserItem(player_id);
                    MessageBox.Show("添加物品成功!");
                    return;
                }

                if (amount > 1 && amount == amount_limit)//对于装备类型物品，直接读取耐久进行设置。
                {
                    Moyu.GiveItem(player_id, itemtype_id, amount, gem1);
                    MessageBox.Show("添加物品成功!");
                }
                else
                {
                    Moyu.GiveItem(player_id, itemtype_id, num, gem1); //叠加类物品
                    MessageBox.Show("添加物品成功!");
                }
                FillUserItem(player_id);
            }
            catch
            {
                MessageBox.Show("输入不合法！");
                return;
            }


        }

    }
}
