using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmSelectMonster : Form
    {
        public frmSelectMonster()
        {
            InitializeComponent();
        }

        private string _monsterName;
        private string _monsterId;

        public string monsterId
        {
            get { return _monsterId; }
            set { _monsterId = value; }
        }
        public string monsterName
        {
            get { return _monsterName; }
            set { _monsterName = value; }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = Moyu.GetMonsterType(txtSearch.Text);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvMonsters.Rows.Add();
                dgvMonsters.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvMonsters.Rows[index].Cells["name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
            }
        }

        private void dgvMonsters_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.monsterId = dgvMonsters.CurrentRow.Cells["id"].Value.ToString();
            this.monsterName = dgvMonsters.CurrentRow.Cells["name"].Value.ToString();
            this.DialogResult = DialogResult.OK;

        }

    }
}
