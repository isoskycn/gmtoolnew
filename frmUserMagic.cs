using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmUserMagic : Form
    {
        public frmUserMagic()
        {
            InitializeComponent();
        }

        public frmUserMagic(int id, string name)
        {
            InitializeComponent();
            this.Text += String.Format(" - [{0}]", name);
            dgvMagicType.Tag = name;
            dgvMagic.Tag = id;
        }

        private void frmUserMagic_Load(object sender, EventArgs e)
        {
            //this.Text += String.Format(" - [{0}]", Moyu.playerName);

            DataTable dt = Moyu.GetMagicType();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvMagicType.Rows.Add();
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvMagicType.Rows[i].Cells["type"].Value = dt.Rows[i]["type"].ToString();
                dgvMagicType.Rows[i].Cells["name"].Value = name;

            }
            FillUserMagic();
        }

        private void FillUserMagic()
        {
            dgvMagic.Rows.Clear();
            DataTable dt2 = Moyu.GetUserMagic(Convert.ToInt32(dgvMagic.Tag));
            //DataTable dt2 = Moyu.GetUserMagic(Moyu.player_id);
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                int index = dgvMagic.Rows.Add();
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt2.Rows[i]["name"]);
                dgvMagic.Rows[i].Cells["magicType"].Value = dt2.Rows[i]["type"];
                dgvMagic.Rows[i].Cells["magicName"].Value = name;
                dgvMagic.Rows[i].Cells["magicLevel"].Value = dt2.Rows[i]["level"];
                dgvMagic.Rows[i].Cells["magicExp"].Value = dt2.Rows[i]["exp"];

            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dgvMagicType.Rows.Clear();
            DataTable dt = Moyu.GetMagicType(txtSearch.Text);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvMagicType.Rows.Add();
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvMagicType.Rows[i].Cells["type"].Value = dt.Rows[i]["type"].ToString();
                dgvMagicType.Rows[i].Cells["name"].Value = name;

            }

        }

        private void dgvMagicType_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Moyu.UserDelMagic(Convert.ToInt32(dgvMagic.Tag), Convert.ToInt32(dgvMagicType.CurrentRow.Cells["type"].Value));
            Moyu.UserLearnMagic(Convert.ToInt32(dgvMagic.Tag), Convert.ToInt32(dgvMagicType.CurrentRow.Cells["type"].Value));
            MessageBox.Show("技能添加成功!");
            FillUserMagic();
        }


        private void dgvMagic_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Moyu.UserDelMagic(Convert.ToInt32(dgvMagic.Tag), Convert.ToInt32(dgvMagic.CurrentRow.Cells["magicType"].Value));
            MessageBox.Show("技能删除成功!");
            FillUserMagic();
        }
    }
}
