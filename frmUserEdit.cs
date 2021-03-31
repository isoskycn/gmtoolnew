using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmUserEdit : Form
    {
        public frmUserEdit()
        {
            InitializeComponent();
        }
        //输入检测
        private void editNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            Moyu.InputCheck(sender, e);
        }

        private void DGVAccountList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (DGVAccountList.SelectedRows.Count < 1) return;
            txtFindAccountName.Text = DGVAccountList.CurrentRow.Cells["account_name"].Value.ToString();
            txtFindPlayerName.Text = DGVAccountList.CurrentRow.Cells["player_name"].Value.ToString();
            btnFindUser.PerformClick();
        }

        private void editUserList_MouseClick(object sender, MouseEventArgs e)
        {
            if (editUserList.Items.Count < 1) return;
            FillUserInfo(editUserList.SelectedItem.ToString());
        }

        private void frmUserEdit_Load(object sender, EventArgs e)
        {
            DGVAccountOwner.Rows.Add(1);//用于添加账号
            //职业cbb
            ComboxItem[] p = {
                new ComboxItem("魔法师","10"),
                new ComboxItem("战士","20"),
                new ComboxItem("异能者","30"),
                new ComboxItem("血族","50"),
                new ComboxItem("亡灵巫师","60"),
                new ComboxItem("暗黑龙骑","70"),
                new ComboxItem("精灵游侠","80"),
                new ComboxItem("千面","90"),
                new ComboxItem("御剑师","100"),
                new ComboxItem("星辰神子","110")
            };
            cbbProfession.Items.AddRange(p);
            //性别cbb
            ComboxItem[] sex = {
                new ComboxItem("男","1"),
                new ComboxItem("女","2"),
                new ComboxItem("GM","3")
            };
            cbbLookface.Items.AddRange(sex);
        }

        private void btnGetAccountList_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
            DGVAccountList.Rows.Clear();
            DataTable dt = Moyu.GetAccountList();
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = DGVAccountList.Rows.Add();
                DGVAccountList.Rows[index].Cells["account_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["account_name"]);
                DGVAccountList.Rows[index].Cells["player_name"].Value = DBNull.Value == dt.Rows[i]["player_name"] ? "无" : System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["player_name"]);
                DGVAccountList.Rows[index].Cells["account_id"].Value = dt.Rows[i]["account_id"];
                DGVAccountList.Rows[index].Cells["player_id"].Value = dt.Rows[i]["player_id"];
            }
            this.Text = "人物修改 - 读取到[" + dt.Rows.Count + "]个账号 右键可弹出操作菜单";

        }



        private void btnAccountOwnerSearch_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
            string name = txtAccountOwnerSeach.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("请先输入账号！");
                return;
            }
            DGVAccountOwner.Rows.Clear();
            string sql = String.Format("select a.id,a.name as username,a.password,a.superpass,a.vip,b.name,b.elock " +
                "from account as a left join {0} as b on a.id=b.account_id " +
                "where b.type=0 and a.name='{1}';", Moyu.tName, name);
            MySqlDataReader reader = MySqlHelper.Select(sql);
            try
            {
                while (reader.Read())
                {
                    if (reader.HasRows)
                    {
                        int index = DGVAccountOwner.Rows.Add();
                        DGVAccountOwner.Rows[index].Cells["ID"].Value = reader.GetString("ID");
                        DGVAccountOwner.Rows[index].Cells["account"].Value = reader.GetString("username");
                        //DGV1.Rows[index].Cells["accountPassword"].Value = reader.GetString("password");
                        DGVAccountOwner.Rows[index].Cells["name"].Value = reader.GetString("name");
                        DGVAccountOwner.Rows[index].Cells["superPass"].Value = reader.GetString("superpass");
                        DGVAccountOwner.Rows[index].Cells["elock"].Value = reader.GetString("elock");
                        DGVAccountOwner.Rows[index].Cells["vip"].Value = reader.GetString("vip");
                        Moyu.accountName = reader.GetString("username");//赋值以便后面修改操作。
                        Moyu.playerName = reader.GetString("name");
                        DGVAccountOwner.Tag = "true";//改变tag,以允许修改。
                    }
                    else
                    {
                        MessageBox.Show("没有这个账号的数据!");
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

        private void btnFindUser_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
            string accountName = txtFindAccountName.Text.Trim();
            string playerName = txtFindPlayerName.Text.Trim();
            if (string.IsNullOrEmpty(accountName) && string.IsNullOrEmpty(playerName))
            {
                MessageBox.Show("请输入账号或者人物名!");
                return;
            }
            //清空控件
            Moyu.ClearInput(groupUserEdit);
            cbxIsGM.Checked = false;

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
                    editUserList.Items.Add(System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]));
                }
                editUserList.SelectedItem = editUserList.Items[0];

                //属性处理
                DataRow[] dr = dt.Select("type='0'");//过滤小号,默认显示主号角色属性信息
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dr[0]["name"]);
                FillUserInfo(name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //填充人物属性数据，考虑到双击角色名取得属性，所以单独封装一个方法。
        public void FillUserInfo(string name)
        {
            //Moyu.ClearInput(groupUserEdit);//清空控件
            MySqlDataReader reader = Moyu.GetUserInfo(name);
            try
            {
                while (reader.Read())
                {
                    edit_name.Text = reader.GetString("name");
                    edit_emoney.Text = reader.GetString("emoney");
                    edit_emoney2.Text = reader.GetString("emoney2");
                    edit_money.Text = reader.GetString("money");
                    edit_bonus_points.Text = reader.GetString("bonus_points");
                    edit_level.Text = reader.GetString("level");
                    edit_tutor_level.Text = reader.GetString("tutor_level");
                    edit_medal_select.Text = reader.GetString("medal_select");
                    edit_mate.Text = reader.GetString("mate");
                    edit_life.Text = reader.GetString("life");
                    edit_mana.Text = reader.GetString("mana");
                    edit_power.Text = reader.GetString("power");
                    edit_soul.Text = reader.GetString("soul");
                    edit_dexterity.Text = reader.GetString("dexterity");
                    edit_pk.Text = reader.GetString("pk");
                    edit_donation.Text = reader.GetString("donation");
                    edit_elock.Text = reader.GetString("elock");
                    edit_superpass.Text = reader.GetString("superpass");
                    cbbVip.SelectedItem = reader.GetString("vip");
                    cbbProfession.SelectedIndex = Moyu.GetCbbIndex(cbbProfession, reader.GetString("profession"));//职业
                    cbbLookface.SelectedIndex = (reader.GetInt32("lookface") % 10) - 1;//性别
                    //账号状态(兼容其他GM工具设置的值)
                    if (reader.GetInt32("online") > 3)
                    {
                        cbbOnline.SelectedIndex = 4;
                    }
                    else
                    {
                        cbbOnline.SelectedIndex = reader.GetInt32("online");
                    }

                    if (reader.GetString("name").IndexOf("[PM]") != -1) cbxIsGM.Checked = true;//gm权限

                    //新端字段处理
                    if (Moyu.IsNewDB)
                    {
                        edit_godfiremoonvalue.Text = reader.GetString("godfiremoonvalue");
                        edit_godfirestarvalue.Text = reader.GetString("godfirestarvalue");
                        edit_godlevel.Text = reader.GetString("godlevel");
                    }
                    //赋值以便后面修改需要。
                    Moyu.accountName = reader.GetString("account_name");
                    Moyu.playerName = reader.GetString("name");
                    Moyu.account_id = reader.GetInt32("account_id");
                    Moyu.player_id = reader.GetInt32("id");
                    cbbLookface.Tag = reader.GetString("lookface").Substring(0, reader.GetString("lookface").Length - 1);
                    btnBanHardware.Tag = Moyu.GetUserHardWare(reader.GetString("account_name"));
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

        //更新角色信息
        private void btnUpdateUser_Click(object sender, EventArgs e)
        {
            if (editUserList.Items.Count < 1)
            {
                MessageBox.Show("没有需要修改的角色信息!");
                return;
            }

            string fields = Moyu.UpdateSqlBuild(groupUserEdit);
            //魔石
            string emoney = String.Format(",emoney='{0}',chk_sum='{1}'", edit_emoney.Text, Moyu.GetEmoneyChkSum(Moyu.player_id, Convert.ToInt32(edit_emoney.Text), Moyu.IsNewDB).ToString());
            //职业
            string profession = ((ComboxItem)(cbbProfession.SelectedItem)).Values;
            //性别
            string sex = cbbLookface.Tag.ToString() + ((ComboxItem)cbbLookface.SelectedItem).Values;

            string sql = String.Format("update {0} set {1},profession='{2}',lookface='{3}' where id='{4}';", Moyu.tName, fields + emoney, profession, sex, Moyu.player_id);
            //账号相关
            int online = 0;
            if (cbbOnline.SelectedIndex == 4)
            {
                online = 12;
            }
            else
            {
                online = cbbOnline.SelectedIndex;
            }
            sql += String.Format("update account set superpass='{0}',vip='{1}',online='{2}' where id='{3}';", edit_superpass.Text, cbbVip.SelectedItem, online, Moyu.account_id);
            MySqlHelper.Query(sql);
            MessageBox.Show("角色数据修改成功!");
        }

        //修改账号资料
        private void btnAccountOwnerUpdate_Click(object sender, EventArgs e)
        {
            if (!MySqlHelper.TestConn())
            {
                MessageBox.Show("请先连接数据库!");
                return;
            }
            if (DGVAccountOwner.Rows.Count == 0 || DGVAccountOwner.Tag.ToString() == "false")//添加个tag,防止添加账号而输入的数据被误用
            {
                MessageBox.Show("请先查询账号数据!");
                return;
            }

            DialogResult dr = MessageBox.Show("确认修改吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;

            string newAccount = DGVAccountOwner.Rows[0].Cells["account"].Value.ToString().Trim();
            string newPlayerName = DGVAccountOwner.Rows[0].Cells["name"].Value.ToString().Trim();
            string newSuperPass = DGVAccountOwner.Rows[0].Cells["superpass"].Value.ToString().Trim();
            string newElock = DGVAccountOwner.Rows[0].Cells["elock"].Value.ToString().Trim();
            string newVip = DGVAccountOwner.Rows[0].Cells["vip"].Value.ToString().Trim();
            object newPass = DGVAccountOwner.Rows[0].Cells["password"].Value;

            if (string.IsNullOrEmpty(newAccount) || string.IsNullOrEmpty(newPlayerName))
            {
                MessageBox.Show("新账号或人物名非法！");
                return;
            }

            if (newAccount != Moyu.accountName && Moyu.AccountName2Id(newAccount) != -1)
            {
                MessageBox.Show("已存在相同账号!");
                return;
            }
            if (newPlayerName != Moyu.playerName && Moyu.PlayName2Id(newPlayerName) != -1)
            {
                MessageBox.Show("已存在相同角色名!");
                return;
            }
            if (newPass != null)
            {
                Moyu.ChangeAccountOwner(Moyu.accountName, Moyu.playerName, newAccount, newPlayerName, newSuperPass, newElock, newVip, newPass.ToString());
            }
            else
            {
                Moyu.ChangeAccountOwner(Moyu.accountName, Moyu.playerName, newAccount, newPlayerName, newSuperPass, newElock, newVip);
            }
            txtAccountOwnerSeach.Text = newAccount;
            Moyu.accountName = newAccount;
            Moyu.playerName = newPlayerName;

            MessageBox.Show("修改成功");
        }

        //设置gm权限
        private void cbxIsGM_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(edit_name.Text.Trim())) return;
            if (cbxIsGM.Checked == true)
            {
                edit_name.Text = edit_name.Text + "[PM]";
            }
            else
            {
                edit_name.Text = edit_name.Text.Replace("[PM]", "");
            }
        }


        //点卡充值
        private void btnAddCard_Click(object sender, EventArgs e)
        {
            if (Moyu.account_id == 0 || string.IsNullOrEmpty(txtCardNum.Text) || Convert.ToInt32(txtCardNum.Text) < 1 || cbbCard.SelectedIndex < 0)
            {
                MessageBox.Show("没有充值的账号信息或数量出错!");
                return;
            }
            string count = Moyu.AddCard(Convert.ToInt32(txtCardNum.Text), cbbCard.SelectedIndex).ToString();
            MessageBox.Show(String.Format("充值成功！成功充值{0}张{1}！", count, cbbCard.SelectedItem.ToString()));
        }

        //添加账号
        private void btnAddAccount_Click(object sender, EventArgs e)
        {

            object account = DGVAccountOwner.Rows[0].Cells["account"].Value;
            object password = DGVAccountOwner.Rows[0].Cells["password"].Value;
            object superpass = DGVAccountOwner.Rows[0].Cells["superpass"].Value;
            object vip = DGVAccountOwner.Rows[0].Cells["vip"].Value;
            if (account == null || password == null)
            {
                MessageBox.Show("请至少填入账号、密码!");
                return;
            }

            if (string.IsNullOrEmpty(account.ToString().Trim()) || string.IsNullOrEmpty(password.ToString().Trim()))
            {
                MessageBox.Show("账号或密码不合法!");
                return;
            }

            if (vip == null) vip = "0";
            if (superpass == null) superpass = "123456";

            if (Moyu.AddAccount(account.ToString().Trim(), password.ToString().Trim(), superpass.ToString().Trim(), vip.ToString().Trim()))
            {
                MessageBox.Show("账号添加成功!");
            }
            else
            {
                MessageBox.Show("账号添加失败!");
            }
        }

        //封硬件码
        private void btnBanHardware_Click(object sender, EventArgs e)
        {
            if (btnBanHardware.Tag == null)
            {
                MessageBox.Show("请先选择角色!");
                return;
            }

            try
            {
                Moyu.BanHardcode(btnBanHardware.Tag.ToString());
                MessageBox.Show("封禁成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnMail_Click(object sender, EventArgs e)
        {
            if (!Moyu.IsNewDB)
            {
                MessageBox.Show("该功能老端无法使用!");
                return;
            }

            if (Moyu.player_id == 0 || Moyu.playerName == null)
            {
                MessageBox.Show("请先选择角色!");
                return;
            }

            if (Application.OpenForms["frmMail"] == null)
            {
                frmMail frm = new frmMail();
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                if (Application.OpenForms["frmMail"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmMail"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmMail"].Activate();
            }

        }

        private void btnGoddess_Click(object sender, EventArgs e)
        {
            if (!Moyu.IsNewDB)
            {
                MessageBox.Show("该功能老端无法使用!");
                return;
            }

            if (Moyu.player_id == 0)
            {
                MessageBox.Show("请先选择角色!");
                return;
            }

            if (Application.OpenForms["frmGoddess"] == null)
            {
                frmGoddess frm = new frmGoddess();
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                if (Application.OpenForms["frmGoddess"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmGoddess"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmGoddess"].Activate();
            }
        }


        //神火
        private void btnGodFire_Click(object sender, EventArgs e)
        {
            if (!Moyu.IsNewDB)
            {
                MessageBox.Show("该功能老端无法使用!");
                return;
            }

            if (Moyu.player_id == 0)
            {
                MessageBox.Show("请先选择角色!");
                return;
            }

            if (Application.OpenForms["frmGodFire"] == null)
            {
                frmGodFire frm = new frmGodFire();
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                if (Application.OpenForms["frmGodFire"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmGodFire"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmGodFire"].Activate();
            }
        }

        //角色技能
        private void btnUserMagic_Click(object sender, EventArgs e)
        {
            if (Moyu.player_id == 0)
            {
                MessageBox.Show("请先选择角色!");
                return;
            }


            if (Application.OpenForms["frmUserMagic"] == null)
            {
                frmUserMagic frm = new frmUserMagic(Moyu.player_id, Moyu.playerName);
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

        //相似账号
        private void btnSimilarAccount_Click(object sender, EventArgs e)
        {
            if (Moyu.accountName == null)
            {
                MessageBox.Show("请先选择角色!");
                return;
            }

            if (Application.OpenForms["frmSimilarAccount"] == null)
            {
                frmSimilarAccount frm = new frmSimilarAccount();
                frm.MdiParent = this.MdiParent;
                frm.StartPosition = FormStartPosition.Manual;
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            else
            {
                if (Application.OpenForms["frmSimilarAccount"].WindowState == FormWindowState.Minimized)
                    Application.OpenForms["frmSimilarAccount"].WindowState = FormWindowState.Normal;
                Application.OpenForms["frmSimilarAccount"].Activate();
            }
        }

        private void miDelAccount_Click(object sender, EventArgs e)
        {
            if (DGVAccountList.SelectedRows.Count < 1) return;
            DialogResult dr = MessageBox.Show("确认删除吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            Moyu.DelAccount(DGVAccountList.CurrentRow.Cells["account_id"].Value.ToString());
            DGVAccountList.Rows.Remove(DGVAccountList.CurrentRow);
        }

        private void miDelPlayer_Click(object sender, EventArgs e)
        {
            if (DGVAccountList.SelectedRows.Count < 1) return;
            DialogResult dr = MessageBox.Show("确认删除吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            Moyu.DelUser(DGVAccountList.CurrentRow.Cells["player_id"].Value.ToString());
            DGVAccountList.Rows.Remove(DGVAccountList.CurrentRow);
        }

        private void btnKick_Click(object sender, EventArgs e)
        {
            if (Moyu.KickPlayer(Moyu.player_id))
            {
                MessageBox.Show("已踢出玩家！");
            }
            else {
                MessageBox.Show("服务端未启动,踢人失败！");
            }
        }
    }
}
