using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmEudemon : Form
    {
        public frmEudemon()
        {
            InitializeComponent();
        }

        private void Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            Moyu.InputCheck(sender, e);
        }

        private void frmEudemon_Load(object sender, EventArgs e)
        {
            this.Text += String.Format(" - [{0}]", Moyu.playerName);

            //天赋
            ComboxItem[] t = {
                new ComboxItem("无","0"),
                new ComboxItem("瞬移 [1级天赋]","51"),
                new ComboxItem("小瞬移 [1级天赋]","52"),
                new ComboxItem("回城 [1级天赋]","53"),
                new ComboxItem("鉴定 [1级天赋]","54"),
                new ComboxItem("修理 [1级天赋]","55"),
                new ComboxItem("减伤0.5% [1级天赋]","100"),
                new ComboxItem("减伤1% [2级天赋]","101"),
                new ComboxItem("减伤1.5% [3级天赋]","102"),
                new ComboxItem("减伤2.5% [4级天赋]","103"),
                new ComboxItem("减伤2.5% [4级天赋]","104"),
                new ComboxItem("生命+100 [1级天赋]","110"),
                new ComboxItem("生命+200 [2级天赋]","111"),
                new ComboxItem("生命+300 [3级天赋]","112"),
                new ComboxItem("生命+500 [4级天赋]","113"),
                new ComboxItem("生命+1000 [5级天赋]","114"),
                new ComboxItem("物防+50 [1级天赋]","120"),
                new ComboxItem("物防+100 [2级天赋]","121"),
                new ComboxItem("物防+150 [3级天赋]","122"),
                new ComboxItem("物防+200 [4级天赋]","123"),
                new ComboxItem("物防+300 [5级天赋]","124"),
                new ComboxItem("魔防+30 [1级天赋]","130"),
                new ComboxItem("魔防+50 [2级天赋]","131"),
                new ComboxItem("魔防+100 [3级天赋]","132"),
                new ComboxItem("魔防+150 [4级天赋]","133"),
                new ComboxItem("魔防+250 [5级天赋]","134"),
                new ComboxItem("攻击+50 [1级天赋]","140"),
                new ComboxItem("攻击+100 [2级天赋]","141"),
                new ComboxItem("攻击+150 [3级天赋]","142"),
                new ComboxItem("攻击+250 [4级天赋]","143"),
                new ComboxItem("攻击+350 [5级天赋]","144"),
                new ComboxItem("战斗力+1 [1级天赋]","190"),
                new ComboxItem("战斗力+2 [2级天赋]","191"),
                new ComboxItem("战斗力+3 [3级天赋]","192"),
                new ComboxItem("战斗力+4 [4级天赋]","194"),
                new ComboxItem("战斗力+5 [5级天赋]","195"),
                new ComboxItem("军团战魂 [5级天赋]","219"),
                new ComboxItem("王族战魂 [5级天赋]","229"),
                new ComboxItem("小队战魂 [5级天赋]","239"),
                new ComboxItem("寰宇战魂-5 [5级天赋]","200"),
                new ComboxItem("寰宇战魂+5 [5级天赋]","209"),
                new ComboxItem("怒火战魂-5 [5级天赋]","240"),
                new ComboxItem("怒火战魂+5 [5级天赋]","249")
            };
            cbbTalent1.Items.AddRange(t);
            cbbTalent2.Items.AddRange(t);
            cbbTalent3.Items.AddRange(t);
            cbbTalent4.Items.AddRange(t);
            cbbTalent5.Items.AddRange(t);
            FillUserEudemon();
        }

        private void FillUserEudemon()
        {
            dgvEudemon.Rows.Clear();
            DataTable dt = Moyu.GetUserEudemon(Moyu.player_id);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvEudemon.Rows.Add();
                dgvEudemon.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvEudemon.Rows[index].Cells["item_type"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["type"]);
                dgvEudemon.Rows[index].Cells["name"].Value = dt.Rows[i]["name"];
                dgvEudemon.Rows[index].Cells["star_lev"].Value = DBNull.Value != dt.Rows[i]["star_lev"] ? Convert.ToDecimal(dt.Rows[i]["star_lev"]) / 100 + "星" : "";
                dgvEudemon.Rows[index].Cells["level"].Value = dt.Rows[i]["level"];
                dgvEudemon.Rows[index].Cells["reborn_times"].Value = dt.Rows[i]["reborn_times"];
                dgvEudemon.Rows[index].Cells["cinnabar"].Value = dt.Rows[i]["cinnabar"];
                dgvEudemon.Rows[index].Cells["card_id"].Value = dt.Rows[i]["card_id"];
                dgvEudemon.Rows[index].Cells["type"].Value = dt.Rows[i]["item_type"];
                string atktype = "土";
                switch (Convert.ToInt32(dt.Rows[i]["damage_type"]))
                {
                    case 1:
                        atktype = "土";
                        break;
                    case 2:
                        atktype = "水";
                        break;
                    case 3:
                        atktype = "火";
                        break;
                    case 4:
                        atktype = "风";
                        break;
                    case 5:
                        atktype = "雷";
                        break;
                    default:
                        atktype = "土";
                        break;
                }
                dgvEudemon.Rows[index].Cells["damage_type"].Value = atktype;

                //新端处理
                if (Moyu.IsNewDB)
                {
                    dgvEudemon.Rows[index].Cells["god_lev"].Value = dt.Rows[i]["god_lev"];
                    dgvEudemon.Rows[index].Cells["cinnabar1"].Value = dt.Rows[i]["cinnabar1"];
                }
            }
        }

        private void FillEudemonInfo(int item_id)
        {
            Moyu.ClearInput(groupEudemonEdit);//防止新端查询后连接老端数据库，造成显示问题。
            DataTable dt = Moyu.GetEudemonInfo(item_id);
            if (dt.Rows.Count < 1) return;
            edit_name.Text = dt.Rows[0]["name"].ToString();
            edit_level.Text = dt.Rows[0]["level"].ToString();
            edit_luck.Text = dt.Rows[0]["luck"].ToString();
            cbbTalent1.SelectedIndex = Moyu.GetCbbIndex(cbbTalent1, dt.Rows[0]["talent1"].ToString());
            cbbTalent2.SelectedIndex = Moyu.GetCbbIndex(cbbTalent2, dt.Rows[0]["talent2"].ToString());
            cbbTalent3.SelectedIndex = Moyu.GetCbbIndex(cbbTalent3, dt.Rows[0]["talent3"].ToString());
            cbbTalent4.SelectedIndex = Moyu.GetCbbIndex(cbbTalent4, dt.Rows[0]["talent4"].ToString());
            cbbTalent5.SelectedIndex = Moyu.GetCbbIndex(cbbTalent5, dt.Rows[0]["talent5"].ToString());
            edit_initial_life.Text = dt.Rows[0]["initial_life"].ToString();
            edit_reborn_times.Text = dt.Rows[0]["reborn_times"].ToString();
            edit_reborn_limit_add.Text = dt.Rows[0]["reborn_limit_add"].ToString();
            edit_phyatk_grow_rate.Text = dt.Rows[0]["phyatk_grow_rate"].ToString();
            edit_phyatk_grow_rate_max.Text = dt.Rows[0]["phyatk_grow_rate_max"].ToString();
            edit_magicatk_grow_rate.Text = dt.Rows[0]["magicatk_grow_rate"].ToString();
            edit_magicatk_grow_rate_max.Text = dt.Rows[0]["magicatk_grow_rate_max"].ToString();
            edit_life_grow_rate.Text = dt.Rows[0]["life_grow_rate"].ToString();
            edit_phydef_grow_rate.Text = dt.Rows[0]["phydef_grow_rate"].ToString();
            edit_magicdef_grow_rate.Text = dt.Rows[0]["magicdef_grow_rate"].ToString();
            edit_cinnabar.Text = dt.Rows[0]["cinnabar"].ToString(); ;
            cbbDamage_type.SelectedIndex = Convert.ToInt32(dt.Rows[0]["damage_type"]) - 1;
            edit_star_lev.Text = dt.Rows[0]["star_lev"].ToString();
            //cbbPresent.SelectedIndex = Convert.ToInt32(dt.Rows[0]["relationship"]);


            //初始物攻
            string initPhy = dt.Rows[0]["initial_phy"].ToString();
            edit_initial_phy.Text = Moyu.GetEudemonInitValue(initPhy, 1);
            edit_initial_phy2.Text = Moyu.GetEudemonInitValue(initPhy, 2);
            //初始魔攻
            string initMagic = dt.Rows[0]["initial_magic"].ToString();
            edit_initial_magic.Text = Moyu.GetEudemonInitValue(initMagic, 1);
            edit_initial_magic2.Text = Moyu.GetEudemonInitValue(initMagic, 2);
            //初始防御
            string initDef = dt.Rows[0]["initial_def"].ToString();
            edit_initial_def.Text = Moyu.GetEudemonInitValue(initDef, 1);
            edit_initial_def2.Text = Moyu.GetEudemonInitValue(initDef, 2);

            //新端
            if (Moyu.IsNewDB)
            {
                edit_god_lev.Text = dt.Rows[0]["god_lev"].ToString();
                edit_cinnabar1.Text = dt.Rows[0]["cinnabar1"].ToString();
                edit_god_attrib1.Text = dt.Rows[0]["god_attrib1"].ToString();
                edit_god_attrib2.Text = dt.Rows[0]["god_attrib2"].ToString();
                edit_god_attrib3.Text = dt.Rows[0]["god_attrib3"].ToString();
                edit_god_attrib5.Text = dt.Rows[0]["god_attrib5"].ToString();
                cbbGod_attrib4.SelectedIndex = Convert.ToInt32(dt.Rows[0]["god_attrib4"]);
            }
        }

        private void dgvEudemon_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            groupEudemonEdit.Tag = dgvEudemon.CurrentRow.Cells["id"].Value;
            btnPreview.Tag = dgvEudemon.CurrentRow.Cells["type"].Value;
            if (!cbx1.Checked)
            {
                FillEudemonInfo(Convert.ToInt32(dgvEudemon.CurrentRow.Cells["id"].Value));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (groupEudemonEdit.Tag == null)
            {
                MessageBox.Show("请先选择幻兽!");
                return;
            }
            if (Convert.ToInt32(edit_initial_magic2.Text) < Convert.ToInt32(edit_initial_magic.Text) || Convert.ToInt32(edit_initial_phy2.Text) < Convert.ToInt32(edit_initial_phy.Text) || Convert.ToInt32(edit_magicatk_grow_rate_max.Text) < Convert.ToInt32(edit_magicatk_grow_rate.Text) || Convert.ToInt32(edit_phyatk_grow_rate_max.Text) < Convert.ToInt32(edit_phyatk_grow_rate.Text))
            {
                MessageBox.Show("属性设置有误,最大值不能小于最小值.请检查!");
                return;
            }
            string t1 = ((ComboxItem)cbbTalent1.SelectedItem).Values;
            string t2 = ((ComboxItem)cbbTalent2.SelectedItem).Values;
            string t3 = ((ComboxItem)cbbTalent3.SelectedItem).Values;
            string t4 = ((ComboxItem)cbbTalent4.SelectedItem).Values;
            string t5 = ((ComboxItem)cbbTalent5.SelectedItem).Values;
            string initPhy = Moyu.SetEudemonInitValue(edit_initial_phy.Text) + Moyu.SetEudemonInitValue(edit_initial_phy2.Text);
            string initMagic = Moyu.SetEudemonInitValue(edit_initial_magic.Text) + Moyu.SetEudemonInitValue(edit_initial_magic2.Text);
            string initDef = Moyu.SetEudemonInitValue(edit_initial_def.Text) + Moyu.SetEudemonInitValue(edit_initial_def2.Text);
            string atktype = (cbbDamage_type.SelectedIndex + 1).ToString();

            string sql = String.Format("update cq_eudemon set name='{0}',level='{1}',luck='{2}',talent1='{3}',talent2='{4}',talent3='{5}',talent4='{6}',talent5='{7}'," +
                "initial_phy='{8}',initial_magic='{9}',initial_def='{10}',initial_life='{11}'," +
                "reborn_times='{12}',phyatk_grow_rate='{13}',phyatk_grow_rate_max='{14}',magicatk_grow_rate='{15}',magicatk_grow_rate_max='{16}',phydef_grow_rate='{17}'," +
                "magicdef_grow_rate='{18}',life_grow_rate='{19}',reborn_limit_add='{20}',cinnabar='{21}',damage_type='{22}' where id='{23}';",
                edit_name.Text, edit_level.Text, edit_luck.Text, t1, t2, t3, t4, t5, initPhy, initMagic, initDef, edit_initial_life.Text, edit_reborn_times.Text,
                edit_phyatk_grow_rate.Text, edit_phyatk_grow_rate_max.Text, edit_magicatk_grow_rate.Text, edit_magicatk_grow_rate_max.Text, edit_phydef_grow_rate.Text, edit_magicdef_grow_rate.Text,
                edit_life_grow_rate.Text, edit_reborn_limit_add.Text, edit_cinnabar.Text, atktype, groupEudemonEdit.Tag.ToString());
            MySqlHelper.Query(sql);

            if (Moyu.IsNewDB)
            {
                string sql2 = String.Format("update cq_eudemon set god_lev='{0}',cinnabar1='{1}',star_lev='{2}',god_attrib1='{3}',god_attrib2='{4}',god_attrib3='{5}',god_attrib4='{6}',god_attrib5='{7}' where id='{8}';",
                    edit_god_lev.Text, edit_cinnabar1.Text, edit_star_lev.Text, edit_god_attrib1.Text, edit_god_attrib2.Text, edit_god_attrib3.Text, cbbGod_attrib4.SelectedIndex.ToString(), edit_god_attrib5.Text, groupEudemonEdit.Tag.ToString());
                MySqlHelper.Query(sql2);
            }
            MessageBox.Show("修改成功！");
            FillUserEudemon();
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (dgvEudemon.CurrentRow.Cells[0].Value == null) return;
            string tips = String.Format("确认删除[{0}]吗?", dgvEudemon.CurrentRow.Cells["name"].Value.ToString());
            DialogResult dr = MessageBox.Show(tips, "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            string sql = String.Format("delete from cq_eudemon where id='{0}';", dgvEudemon.CurrentRow.Cells["id"].Value.ToString());
            if (MySqlHelper.Query(sql) > 0)
            {
                MessageBox.Show("删除成功！");
                FillUserEudemon();
            }

        }

        private void btnMagic_Click(object sender, EventArgs e)
        {
            if (dgvEudemon.CurrentRow.Cells[0].Value == null) return;
            if (Application.OpenForms["frmUserMagic"] == null)
            {
                frmUserMagic frm = new frmUserMagic(Convert.ToInt32(dgvEudemon.CurrentRow.Cells["id"].Value), dgvEudemon.CurrentRow.Cells["name"].Value.ToString());
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                if (Application.OpenForms["frmUserMagic"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmUserMagic"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmUserMagic"].Activate();
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (btnPreview.Tag == null) return;
            frmEudPreview frm = new frmEudPreview(btnPreview.Tag.ToString(),edit_name.Text, dgvEudemon.CurrentRow.Cells["card_id"].Value.ToString(),
                edit_level.Text, edit_luck.Text, edit_initial_life.Text, edit_initial_phy.Text, edit_initial_phy2.Text, edit_initial_def.Text, edit_initial_magic.Text,
                edit_initial_magic2.Text, edit_initial_def2.Text,edit_life_grow_rate.Text,edit_phyatk_grow_rate.Text,edit_phyatk_grow_rate_max.Text,edit_phydef_grow_rate.Text,
                edit_magicatk_grow_rate.Text,edit_magicatk_grow_rate_max.Text,edit_magicdef_grow_rate.Text,edit_reborn_times.Text,edit_reborn_limit_add.Text);
            frm.StartPosition = FormStartPosition.Manual;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.ShowDialog();
        }
    }
}
