using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmAddition : Form
    {
        private BackgroundWorker bgWorker1 = new BackgroundWorker();
        public frmAddition()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }
        private void InitializeBackgroundWorker()
        {
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);
            CheckForIllegalCrossThreadCalls = false;

        }
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DataTable dt = Moyu.GetItemAddition();
            int needAddNum = (int)numMaxLev.Value - 12;
            prcBar.Maximum = dt.Rows.Count;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string status = String.Format("正在写入装备数据[{0}/{1}]", i + 1, dt.Rows.Count);
                bgWorker.ReportProgress(i + 1, status);
                for (int lev = 0; lev < needAddNum; lev++)
                {
                    int level = lev + 13;
                    int v = (lev + 1) * (int)numValue.Value;
                    int typeid = Convert.ToInt32(dt.Rows[i]["typeid"]);
                    int life = Convert.ToInt32(dt.Rows[i]["life"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["life"]) + v;
                    int aMin = Convert.ToInt32(dt.Rows[i]["attack_min"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["attack_min"]) + v;
                    int aMax = Convert.ToInt32(dt.Rows[i]["attack_max"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["attack_max"]) + v;
                    int mMin = Convert.ToInt32(dt.Rows[i]["mgcatk_min"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["mgcatk_min"]) + v;
                    int mMax = Convert.ToInt32(dt.Rows[i]["mgcatk_max"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["mgcatk_max"]) + v;
                    int def = Convert.ToInt32(dt.Rows[i]["defense"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["defense"]) + v;
                    int mDef = Convert.ToInt32(dt.Rows[i]["magic_def"]) == 0 ? 0 : Convert.ToInt32(dt.Rows[i]["magic_def"]) + v;
                    if (cbxAll.Checked)
                    {
                        life = life == 0 ? v : life;
                        aMin = aMin == 0 ? v : aMin;
                        aMax = aMax == 0 ? v : aMax;
                        mMin = mMin == 0 ? v : mMin;
                        mMax = mMax == 0 ? v : mMax;
                        def = def == 0 ? v : def;
                        mDef = mDef == 0 ? v : mDef;
                    }
                    string sql = String.Format("INSERT INTO `cq_itemaddition` (`typeid`, `level`, `life`, `attack_max`, `attack_min`, `defense`, `mgcatk_max`, `mgcatk_min`, `magic_def`, `dexterity`, `dodge`) " +
                    "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '0', '0');", typeid, level, life, aMax, aMin, def, mMax, mMin, mDef);
                    MySqlHelper.Query(sql);
                }
            }
        }

        public void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            prcBar.Value = e.ProgressPercentage;
            labStatus.Text = (string)e.UserState;
        }
        public void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = MySqlHelper.GetDataTable("select * from cq_itemaddition order by typeid,level;");
            foreach (DataRow dr in dt.Rows)
            {
                string str = dr["typeid"].ToString() + " " + dr["level"].ToString() + " " + dr["life"].ToString() + " " + dr["attack_max"].ToString() + " " + dr["attack_min"].ToString() + " "
                    + dr["defense"].ToString() + " " + dr["mgcatk_max"].ToString() + " " + dr["mgcatk_min"].ToString() + " " + dr["magic_def"].ToString() + " 0 0";
                sb.AppendLine(str);
            }
            string path = Directory.GetCurrentDirectory() + "\\ItemAddition.ini";
            if (File.Exists(path)) File.Delete(path);
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine(sb);
            sw.Close();
            btnSave.Enabled = true;
            labStatus.Text = "数据写入完成，请使用工具目录下的ItemAddition.ini替换你的补丁。";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (bgWorker1.IsBusy) return;
            DialogResult dr = MessageBox.Show("确认写入吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            btnSave.Enabled = false;
            MySqlHelper.Query("delete from cq_itemaddition where level>12;");
            bgWorker.RunWorkerAsync("hello");
        }

    }
}
