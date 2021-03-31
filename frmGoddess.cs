using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmGoddess : Form
    {
        public frmGoddess()
        {
            InitializeComponent();
        }

        private void frmGoddess_Load(object sender, EventArgs e)
        {
            this.Text += String.Format(" - [{0}]", Moyu.playerName);
            //天赋下拉框
            ComboxItem[] goddess = {
                new ComboxItem("无","0"),
                new ComboxItem("战斗力","1"),
                new ComboxItem("物理攻击","6"),
                new ComboxItem("魔法攻击","11"),
                new ComboxItem("物理防御","16"),
                new ComboxItem("魔法防御","21"),
                new ComboxItem("地攻击","26"),
                new ComboxItem("水攻击","31"),
                new ComboxItem("火攻击","36"),
                new ComboxItem("风攻击","41"),
                new ComboxItem("生命上限","46"),
                new ComboxItem("体力上限","51"),
                new ComboxItem("杀怪经验增加","56")
            };
            cbbGoddess1Attribtype1.Items.AddRange(goddess);
            cbbGoddess1Attribtype2.Items.AddRange(goddess);
            cbbGoddess1Attribtype3.Items.AddRange(goddess);
            cbbGoddess1Attribtype4.Items.AddRange(goddess);
            cbbGoddess2Attribtype1.Items.AddRange(goddess);
            cbbGoddess2Attribtype2.Items.AddRange(goddess);
            cbbGoddess2Attribtype3.Items.AddRange(goddess);
            cbbGoddess2Attribtype4.Items.AddRange(goddess);
            cbbGoddess3Attribtype1.Items.AddRange(goddess);
            cbbGoddess3Attribtype2.Items.AddRange(goddess);
            cbbGoddess3Attribtype3.Items.AddRange(goddess);
            cbbGoddess3Attribtype4.Items.AddRange(goddess);
            cbbGoddess4Attribtype1.Items.AddRange(goddess);
            cbbGoddess4Attribtype2.Items.AddRange(goddess);
            cbbGoddess4Attribtype3.Items.AddRange(goddess);
            cbbGoddess4Attribtype4.Items.AddRange(goddess);

            //女神开启状态
            cbxGoddess1.Checked = Moyu.CheckGoddessStatus(Moyu.player_id, 1);
            cbxGoddess2.Checked = Moyu.CheckGoddessStatus(Moyu.player_id, 2);
            cbxGoddess3.Checked = Moyu.CheckGoddessStatus(Moyu.player_id, 3);
            cbxGoddess4.Checked = Moyu.CheckGoddessStatus(Moyu.player_id, 4);
            //根据女神开启状态判定是否允许操作
            Moyu.ChangeContorlStatus(groupGoddess1, cbxGoddess1.Checked);
            Moyu.ChangeContorlStatus(groupGoddess2, cbxGoddess2.Checked);
            Moyu.ChangeContorlStatus(groupGoddess3, cbxGoddess3.Checked);
            Moyu.ChangeContorlStatus(groupGoddess4, cbxGoddess4.Checked);
            FillGoddessInfo();
        }
        //填充角色女神信息
        private void FillGoddessInfo()
        {
            DataTable dt = Moyu.GetUserGoddessInfo(Moyu.player_id);
            if (dt.Rows.Count < 1) return;

            //女神1
            DataRow[] god1 = dt.Select("goddesstype = '1' ");
            if (god1.Length == 0) return;
            cbbGoddess1Level.SelectedIndex = Convert.ToInt32(god1[0]["level"]);
            cbbGoddess1Attribtype1.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess1Attribtype1, god1[0]["attribtype1"].ToString());
            cbbGoddess1Attribtype2.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess1Attribtype2, god1[0]["attribtype2"].ToString());
            cbbGoddess1Attribtype3.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess1Attribtype3, god1[0]["attribtype3"].ToString());
            cbbGoddess1Attribtype4.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess1Attribtype4, god1[0]["attribtype4"].ToString());
            edit_goddess1attribvalue1.Text = god1[0]["attribvalue1"].ToString();
            edit_goddess1attribvalue2.Text = god1[0]["attribvalue2"].ToString();
            edit_goddess1attribvalue3.Text = god1[0]["attribvalue3"].ToString();
            edit_goddess1attribvalue4.Text = god1[0]["attribvalue4"].ToString();

            //女神2
            DataRow[] god2 = dt.Select("goddesstype = '2' ");
            if (god2.Length == 0) return;
            cbbGoddess2Level.SelectedIndex = Convert.ToInt32(god2[0]["level"]);
            cbbGoddess2Attribtype1.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess2Attribtype1, god2[0]["attribtype1"].ToString());
            cbbGoddess2Attribtype2.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess2Attribtype2, god2[0]["attribtype2"].ToString());
            cbbGoddess2Attribtype3.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess2Attribtype3, god2[0]["attribtype3"].ToString());
            cbbGoddess2Attribtype4.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess2Attribtype4, god2[0]["attribtype4"].ToString());
            edit_goddess2attribvalue1.Text = god2[0]["attribvalue1"].ToString();
            edit_goddess2attribvalue2.Text = god2[0]["attribvalue2"].ToString();
            edit_goddess2attribvalue3.Text = god2[0]["attribvalue3"].ToString();
            edit_goddess2attribvalue4.Text = god2[0]["attribvalue4"].ToString();

            //女神3
            DataRow[] god3 = dt.Select("goddesstype = '3' ");
            if (god3.Length == 0) return;
            cbbGoddess3Level.SelectedIndex = Convert.ToInt32(god3[0]["level"]);
            cbbGoddess3Attribtype1.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess3Attribtype1, god3[0]["attribtype1"].ToString());
            cbbGoddess3Attribtype2.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess3Attribtype2, god3[0]["attribtype2"].ToString());
            cbbGoddess3Attribtype3.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess3Attribtype3, god3[0]["attribtype3"].ToString());
            cbbGoddess3Attribtype4.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess3Attribtype4, god3[0]["attribtype4"].ToString());
            edit_goddess3attribvalue1.Text = god3[0]["attribvalue1"].ToString();
            edit_goddess3attribvalue2.Text = god3[0]["attribvalue2"].ToString();
            edit_goddess3attribvalue3.Text = god3[0]["attribvalue3"].ToString();
            edit_goddess3attribvalue4.Text = god3[0]["attribvalue4"].ToString();

            //女神4
            DataRow[] god4 = dt.Select("goddesstype = '4' ");
            if (god4.Length == 0) return;
            cbbGoddess4Level.SelectedIndex = Convert.ToInt32(god4[0]["level"]);
            cbbGoddess4Attribtype1.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess4Attribtype1, god4[0]["attribtype1"].ToString());
            cbbGoddess4Attribtype2.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess4Attribtype2, god4[0]["attribtype2"].ToString());
            cbbGoddess4Attribtype3.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess4Attribtype3, god4[0]["attribtype3"].ToString());
            cbbGoddess4Attribtype4.SelectedIndex = Moyu.GetCbbIndex(cbbGoddess4Attribtype4, god4[0]["attribtype4"].ToString());
            edit_goddess4attribvalue1.Text = god4[0]["attribvalue1"].ToString();
            edit_goddess4attribvalue2.Text = god4[0]["attribvalue2"].ToString();
            edit_goddess4attribvalue3.Text = god4[0]["attribvalue3"].ToString();
            edit_goddess4attribvalue4.Text = god4[0]["attribvalue4"].ToString();

        }

        //更新角色女神数据
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!cbxGoddess1.Checked && !cbxGoddess2.Checked && !cbxGoddess3.Checked && !cbxGoddess4.Checked)//如果都没有开启，则直接删除所有数据并结束
            {
                string str = String.Format("delete from cq_goddess where userid='{0}';", Moyu.player_id);
                MySqlHelper.Query(str);
                MessageBox.Show("保存成功!");
                return;
            }
            string sql = "";
            //女神1
            if (cbxGoddess1.Checked)
            {
                string g1level = cbbGoddess1Level.SelectedItem.ToString();
                string g1t1 = ((ComboxItem)cbbGoddess1Attribtype1.SelectedItem).Values;
                string g1t2 = ((ComboxItem)cbbGoddess1Attribtype2.SelectedItem).Values;
                string g1t3 = ((ComboxItem)cbbGoddess1Attribtype3.SelectedItem).Values;
                string g1t4 = ((ComboxItem)cbbGoddess1Attribtype4.SelectedItem).Values;
                string g1v1 = edit_goddess1attribvalue1.Text;
                string g1v2 = edit_goddess1attribvalue2.Text;
                string g1v3 = edit_goddess1attribvalue3.Text;
                string g1v4 = edit_goddess1attribvalue4.Text;
                Moyu.UpdateUserGoddessInfo(Moyu.player_id, 1, g1level, g1t1, g1v1, g1t2, g1v2, g1t3, g1v3, g1t4, g1v4);
            }
            else
            {
                sql += String.Format("delete from cq_goddess where goddesstype='1' and userid='{0}';", Moyu.player_id);
            }
            //女神2
            if (cbxGoddess2.Checked)
            {
                string g2level = cbbGoddess2Level.SelectedItem.ToString();
                string g2t1 = ((ComboxItem)cbbGoddess2Attribtype1.SelectedItem).Values;
                string g2t2 = ((ComboxItem)cbbGoddess2Attribtype2.SelectedItem).Values;
                string g2t3 = ((ComboxItem)cbbGoddess2Attribtype3.SelectedItem).Values;
                string g2t4 = ((ComboxItem)cbbGoddess2Attribtype4.SelectedItem).Values;
                string g2v1 = edit_goddess2attribvalue1.Text;
                string g2v2 = edit_goddess2attribvalue2.Text;
                string g2v3 = edit_goddess2attribvalue3.Text;
                string g2v4 = edit_goddess2attribvalue4.Text;
                Moyu.UpdateUserGoddessInfo(Moyu.player_id, 2, g2level, g2t1, g2v1, g2t2, g2v2, g2t3, g2v3, g2t4, g2v4);
            }
            else
            {
                sql += String.Format("delete from cq_goddess where goddesstype='2' and userid='{0}';", Moyu.player_id);
            }
            //女神3
            if (cbxGoddess3.Checked)
            {
                string g3level = cbbGoddess3Level.SelectedItem.ToString();
                string g3t1 = ((ComboxItem)cbbGoddess3Attribtype1.SelectedItem).Values;
                string g3t2 = ((ComboxItem)cbbGoddess3Attribtype2.SelectedItem).Values;
                string g3t3 = ((ComboxItem)cbbGoddess3Attribtype3.SelectedItem).Values;
                string g3t4 = ((ComboxItem)cbbGoddess3Attribtype4.SelectedItem).Values;
                string g3v1 = edit_goddess3attribvalue1.Text;
                string g3v2 = edit_goddess3attribvalue2.Text;
                string g3v3 = edit_goddess3attribvalue3.Text;
                string g3v4 = edit_goddess3attribvalue4.Text;
                Moyu.UpdateUserGoddessInfo(Moyu.player_id, 3, g3level, g3t1, g3v1, g3t2, g3v2, g3t3, g3v3, g3t4, g3v4);
            }
            else
            {
                sql += String.Format("delete from cq_goddess where goddesstype='3' and userid='{0}';", Moyu.player_id);
            }
            //女神4
            if (cbxGoddess4.Checked)
            {
                string g4level = cbbGoddess4Level.SelectedItem.ToString();
                string g4t1 = ((ComboxItem)cbbGoddess4Attribtype1.SelectedItem).Values;
                string g4t2 = ((ComboxItem)cbbGoddess4Attribtype2.SelectedItem).Values;
                string g4t3 = ((ComboxItem)cbbGoddess4Attribtype3.SelectedItem).Values;
                string g4t4 = ((ComboxItem)cbbGoddess4Attribtype4.SelectedItem).Values;
                string g4v1 = edit_goddess4attribvalue1.Text;
                string g4v2 = edit_goddess4attribvalue2.Text;
                string g4v3 = edit_goddess4attribvalue3.Text;
                string g4v4 = edit_goddess4attribvalue4.Text;
                Moyu.UpdateUserGoddessInfo(Moyu.player_id, 4, g4level, g4t1, g4v1, g4t2, g4v2, g4t3, g4v3, g4t4, g4v4);
            }
            else
            {
                sql += String.Format("delete from cq_goddess where goddesstype='4' and userid='{0}';", Moyu.player_id);
            }

            if (sql != "") MySqlHelper.Query(sql);
            MessageBox.Show("保存成功!");
        }


        private void Input_KeyPress(object sender, KeyPressEventArgs e)
        {
            Moyu.InputCheck(sender, e);
        }

        private void cbxGoddess1_CheckedChanged(object sender, EventArgs e)
        {
            Moyu.ChangeContorlStatus(groupGoddess1, cbxGoddess1.Checked);
        }

        private void cbxGoddess2_CheckedChanged(object sender, EventArgs e)
        {
            Moyu.ChangeContorlStatus(groupGoddess2, cbxGoddess2.Checked);
        }

        private void cbxGoddess3_CheckedChanged(object sender, EventArgs e)
        {
            Moyu.ChangeContorlStatus(groupGoddess3, cbxGoddess3.Checked);
        }

        private void cbxGoddess4_CheckedChanged(object sender, EventArgs e)
        {
            Moyu.ChangeContorlStatus(groupGoddess4, cbxGoddess4.Checked);
        }


    }
}
