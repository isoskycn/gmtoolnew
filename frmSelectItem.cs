using System;
using System.Data;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmSelectItem : Form
    {
        public frmSelectItem()
        {
            InitializeComponent();

        }
        private string _itemName;
        private string _itemId;

        public string itemId
        {
            get { return _itemId; }
            set { _itemId = value; }
        }
        public string itemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = Moyu.GetItemList(2, txtSearch.Text);
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvItems.Rows.Add();
                dgvItems.Rows[index].Cells["id"].Value = dt.Rows[i]["id"];
                dgvItems.Rows[index].Cells["name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
            }
        }

        private void dgvItems_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            this.itemId = dgvItems.CurrentRow.Cells["id"].Value.ToString();
            this.itemName = dgvItems.CurrentRow.Cells["name"].Value.ToString();
            this.DialogResult = DialogResult.OK;

        }

    }
}
