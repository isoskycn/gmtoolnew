using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmUpdate : Form
    {
        private BackgroundWorker bgWorker = new BackgroundWorker();
        public frmUpdate()
        {
            InitializeComponent();
        }
        public frmUpdate(string url, string fileName, string version)
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            this.Url = url;
            this.FileName = fileName;
            this.Text += " - 正在下载" + version + "版本";
        }
        private string _url;
        private string _fileName;

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
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

        private void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            prcBar.Value = e.ProgressPercentage;
            //labPercent.Text = "处理进度:" + Convert.ToString(e.ProgressPercentage) + "%";
            labTxt.Text = (string)e.UserState;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                DownloadFile(Url, FileName, prcBar, labTxt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.DialogResult = DialogResult.No;
            }
        }

        private void frmUpdate_Load(object sender, EventArgs e)
        {
            bgWorker.RunWorkerAsync("hello");
        }

        //下载
        public static void DownloadFile(string URL, string filename, System.Windows.Forms.ProgressBar prog, System.Windows.Forms.Label label1)
        {
            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Maximum = (int)totalBytes;
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                    {
                        prog.Value = (int)totalDownloadedByte;
                    }
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (float)totalDownloadedByte / (float)totalBytes * 100;
                    label1.Text = "下载进度:" + percent.ToString() + "%";
                    System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
