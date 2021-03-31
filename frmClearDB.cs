using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace gmtoolNew
{
    public partial class frmClearDB : Form
    {
        private BackgroundWorker bgWorker = new BackgroundWorker();
        private System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        public frmClearDB()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }
        private void InitializeBackgroundWorker()
        {
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            bgWorker.DoWork += new DoWorkEventHandler(bgWorker_ReamkeId);
            bgWorker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgessChanged);
            bgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_WorkerCompleted);
            CheckForIllegalCrossThreadCalls = false;

        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string db1 = txtDB1.Text;
            string db2 = txtDB2.Text;
            bool isNewDB = Convert.ToBoolean(cbxIsNewDB.Checked);
            string userTable = "cq_user";
            if (isNewDB) userTable = "cq_user_new";
            //删除两个库的垃圾密保信息,防止其他工具删除账号没有删除密保，而用户合区前又没有使用垃圾清理功能造成工具出错。
            string delPwd = String.Format("use{0};delete account_pwd from account_pwd as a left join account as b on a.name=b.name where b.name is null;", db1);
            string delPwd2 = String.Format("use{0};delete account_pwd from account_pwd as a left join account as b on a.name=b.name where b.name is null;", db2);
            MySqlHelper.Query(delPwd);
            MySqlHelper.Query(delPwd2);
            //比较两个库的ID差异值(合区 step1)
            bgWorker.ReportProgress(0, "正在更新源库账号信息");
            int accountIdGap = Moyu.GetIdGap(db1, db2, "account");
            int playerIdGap = Moyu.GetIdGap(db1, db2, userTable);

            //更新账号ID(合区 step2)
            Moyu.UpdateAccountId(db2, accountIdGap, isNewDB);
            //处理重复账号(合区 step3)
            bgWorker.ReportProgress(1, "正在处理重复账号,数据量大过程较慢请耐心等待!");
            List<MyList> listAccount = Moyu.UpdateExistAccountName(db1, db2);
            txtLogs.AppendText("账号信息处理完成!\n");
            //更新人物ID(合区 step4)
            bgWorker.ReportProgress(2, "正在更新源库人物ID!");
            Moyu.UpdatePlayerId(db2, playerIdGap, isNewDB);
            //处理重复人物(合区 step5)
            bgWorker.ReportProgress(3, "正在处理重复人物,此过程可能较慢请耐心等待!");
            List<MyList> listPlayer = Moyu.UpdateExistPlayerName(db1, db2, isNewDB);
            txtLogs.AppendText("人物信息处理完成!\n");

            //密保
            bgWorker.ReportProgress(5, "正在更新源库密保信息!");
            int accPwdGap = Moyu.GetIdGap(db1, db2, "account_pwd");
            if (Moyu.UpdateTableId(db2, "account_pwd", accPwdGap))
            {
                txtLogs.AppendText("account_pwd表处理完成!\n");
            }

            //点卡
            bgWorker.ReportProgress(6, "正在更新源库点卡信息!");
            int cardIdGap = Moyu.GetIdGap(db1, db2, "cq_card");
            if (Moyu.UpdateTableId(db2, "cq_card", cardIdGap))
            {
                txtLogs.AppendText("cq_card表处理完成!\n");
            }
            int card2IdGap = Moyu.GetIdGap(db1, db2, "cq_card2");

            if (Moyu.UpdateTableId(db2, "cq_card2", card2IdGap))
            {
                txtLogs.AppendText("cq_card2表处理完成!\n");
            }

            //物品
            bgWorker.ReportProgress(8, "正在更新源库物品信息!");
            int itemIdGap = Moyu.GetIdGap(db1, db2, "cq_item");
            if (Moyu.UpdateTableId(db2, "cq_item", itemIdGap))
            {
                txtLogs.AppendText("cq_item表处理完成!\n");
            }

            //幻兽
            bgWorker.ReportProgress(10, "正在更新源库幻兽信息!");
            int eudemonIdGap = Moyu.GetIdGap(db1, db2, "cq_eudemon");
            if (Moyu.UpdateTableId(db2, "cq_eudemon", eudemonIdGap))
            {
                txtLogs.AppendText("cq_item表处理完成!\n");
            }

            //军团
            bgWorker.ReportProgress(12, "正在更新源库军团信息!");
            int synIdGap = Moyu.GetIdGap(db1, db2, "cq_syndicate");
            Moyu.UpdateTableId(db2, "cq_syndicate", synIdGap);
            List<MyList> listSyn = Moyu.UpdateExistSynName(db1, db2);
            txtLogs.AppendText("cq_syndicate表处理完成!\n");



            //家族
            bgWorker.ReportProgress(14, "正在更新源库家族信息!");
            int famIdGap = Moyu.GetIdGap(db1, db2, "cq_family");
            Moyu.UpdateTableId(db2, "cq_family", famIdGap);
            List<MyList> listFam = Moyu.UpdateExistFamilyName(db1, db2);
            txtLogs.AppendText("cq_family表处理完成!\n");

            //cq_magic 技能
            bgWorker.ReportProgress(16, "正在更新源库技能信息!");
            int magicIdGap = Moyu.GetIdGap(db1, db2, "cq_magic");
            if (Moyu.UpdateTableId(db2, "cq_magic", magicIdGap))
            {
                txtLogs.AppendText("cq_magic表处理完成!\n");
            }

            //cq_skill 图鉴
            bgWorker.ReportProgress(18, "正在更新源库图鉴信息!");
            int skillIdGap = Moyu.GetIdGap(db1, db2, "cq_skill");
            if (Moyu.UpdateTableId(db2, "cq_skill", skillIdGap))
            {
                txtLogs.AppendText("cq_skill表处理完成!\n");
            }

            //cq_pk_bonus cq_pk_item 悬赏
            bgWorker.ReportProgress(20, "正在更新源库悬赏信息!");
            int pk_bonusIdGap = Moyu.GetIdGap(db1, db2, "cq_pk_bonus");
            if (Moyu.UpdateTableId(db2, "cq_pk_bonus", pk_bonusIdGap))
            {
                txtLogs.AppendText("cq_pk_bonus表处理完成!\n");
            }
            int pk_itemIdGap = Moyu.GetIdGap(db1, db2, "cq_pk_item");
            if (Moyu.UpdateTableId(db2, "cq_pk_item", pk_itemIdGap))
            {
                txtLogs.AppendText("cq_pk_item表处理完成!\n");
            }

            if (!isNewDB)
            {
                //cq_donation_dynasort_rec 爵位
                bgWorker.ReportProgress(22, "正在更新源库爵位信息!");
                int donation_dynasort_recIdGap = Moyu.GetIdGap(db1, db2, "cq_donation_dynasort_rec");
                if (Moyu.UpdateTableId(db2, "cq_donation_dynasort_rec", donation_dynasort_recIdGap))
                {
                    txtLogs.AppendText("cq_donation_dynasort_rec表处理完成!\n");
                }
            }
            //cq_enemy 仇人
            bgWorker.ReportProgress(23, "正在更新源库仇人信息!");
            int enemyIdGap = Moyu.GetIdGap(db1, db2, "cq_enemy");
            if (Moyu.UpdateTableId(db2, "cq_enemy", enemyIdGap))
            {
                txtLogs.AppendText("cq_enemy表处理完成!\n");
            }
            //cq_friend 好友
            bgWorker.ReportProgress(24, "正在更新源库好友信息!");
            int friendIdGap = Moyu.GetIdGap(db1, db2, "cq_friend");
            if (Moyu.UpdateTableId(db2, "cq_friend", friendIdGap))
            {
                txtLogs.AppendText("cq_friend表处理完成!\n");
            }
            //cq_tutor 师徒
            bgWorker.ReportProgress(25, "正在更新源库人物师徒信息!");
            int tutorIdGap = Moyu.GetIdGap(db1, db2, "cq_tutor");
            if (Moyu.UpdateTableId(db2, "cq_tutor", tutorIdGap))
            {
                txtLogs.AppendText("cq_tutor表处理完成!\n");
            }
            //cq_partner 商业
            bgWorker.ReportProgress(26, "正在更新源库商业伙伴信息!");
            int partnerIdGap = Moyu.GetIdGap(db1, db2, "cq_partner");
            if (Moyu.UpdateTableId(db2, "cq_partner", partnerIdGap))
            {
                txtLogs.AppendText("cq_partner表处理完成!\n");
            }

            //cq_announce 导师公告
            bgWorker.ReportProgress(27, "正在更新源库导师公告信息!");
            int announceIdGap = Moyu.GetIdGap(db1, db2, "cq_announce");
            if (Moyu.UpdateTableId(db2, "cq_announce", announceIdGap))
            {
                txtLogs.AppendText("cq_announce表处理完成!\n");
            }
            //cq_special_status 特殊状态
            bgWorker.ReportProgress(28, "正在更新源库特殊状态信息!");
            int special_statusIdGap = Moyu.GetIdGap(db1, db2, "cq_special_status");
            if (Moyu.UpdateTableId(db2, "cq_special_status", special_statusIdGap))
            {
                txtLogs.AppendText("cq_special_status表处理完成!\n");
            }
            //cq_statistic 资料统计
            bgWorker.ReportProgress(29, "正在更新源库资料统计信息!");
            int statisticIdGap = Moyu.GetIdGap(db1, db2, "cq_statistic");
            if (Moyu.UpdateTableId(db2, "cq_statistic", statisticIdGap))
            {
                txtLogs.AppendText("cq_statistic表处理完成!\n");
            }

            //cq_taskdetail 任务
            bgWorker.ReportProgress(30, "正在更新源库任务信息!");
            int taskdetailIdGap = Moyu.GetIdGap(db1, db2, "cq_taskdetail");
            if (Moyu.UpdateTableId(db2, "cq_taskdetail", taskdetailIdGap))
            {
                txtLogs.AppendText("cq_taskdetail表处理完成!\n");
            }
            //cq_flower 玫瑰
            bgWorker.ReportProgress(31, "正在更新源库玫瑰花信息!");
            int flowerIdGap = Moyu.GetIdGap(db1, db2, "cq_flower");
            if (Moyu.UpdateTableId(db2, "cq_flower", flowerIdGap))
            {
                txtLogs.AppendText("cq_flower表处理完成!\n");
            }

            //摊位地皮占领按照市场价值返还金币到仓库
            if (!isNewDB && cbxBackMoney.Checked == true)
            {
                DataTable dtMoney = Moyu.backMoneyByCap(db1, isNewDB);
                DataTable dtMoney2 = Moyu.backMoneyByCap(db2, isNewDB);
                txtLogs.AppendText("已返还摊位占领金币到玩家仓库:\n");
                for (int i = 0; i < dtMoney.Rows.Count; i++)
                {
                    txtLogs.AppendText("摊位ID:" + dtMoney.Rows[i]["id"].ToString() + " 玩家名称:" + dtMoney.Rows[i]["name"].ToString() + " 金钱数量:" + dtMoney.Rows[i]["money"].ToString() + "\n");
                }
                for (int i = 0; i < dtMoney2.Rows.Count; i++)
                {
                    txtLogs.AppendText("摊位ID:" + dtMoney2.Rows[i]["id"].ToString() + " 玩家名称:" + dtMoney2.Rows[i]["name"].ToString() + " 金钱数量:" + dtMoney2.Rows[i]["money"].ToString() + "\n");
                }
            }
            bgWorker.ReportProgress(40, "源库信息更新完成,开始合并数据!");
            //新端
            if (isNewDB)
            {
                bgWorker.ReportProgress(33, "正在更新源库人物头像信息!");
                int faceinfoIdGap = Moyu.GetIdGap(db1, db2, "cq_faceinfo");
                if (Moyu.UpdateTableId(db2, "cq_faceinfo", faceinfoIdGap))
                {
                    txtLogs.AppendText("cq_faceinfo表处理完成!\n");
                }

                bgWorker.ReportProgress(34, "正在更新源库人物发型信息!");
                int hairinfoIdGap = Moyu.GetIdGap(db1, db2, "cq_hairinfo");
                if (Moyu.UpdateTableId(db2, "cq_hairinfo", hairinfoIdGap))
                {
                    txtLogs.AppendText("cq_hairinfo表处理完成!\n");
                }

                bgWorker.ReportProgress(35, "正在更新源库人物称号信息!");
                int titleidIdGap = Moyu.GetIdGap(db1, db2, "cq_titleid");
                if (Moyu.UpdateTableId(db2, "cq_titleid", titleidIdGap))
                {
                    txtLogs.AppendText("cq_titleid表处理完成!\n");
                }

                bgWorker.ReportProgress(36, "正在更新源库人物跟宠信息!");
                int packpetinfoIdGap = Moyu.GetIdGap(db1, db2, "cq_packpetinfo");
                if (Moyu.UpdateTableId(db2, "cq_packpetinfo", packpetinfoIdGap))
                {
                    txtLogs.AppendText("cq_packpetinfo表处理完成!\n");
                }

                bgWorker.ReportProgress(37, "正在更新源库幻兽皮肤信息!");
                int eudlookinfoIdGap = Moyu.GetIdGap(db1, db2, "cq_eudlookinfo");
                if (Moyu.UpdateTableId(db2, "cq_eudlookinfo", eudlookinfoIdGap))
                {
                    txtLogs.AppendText("cq_eudlookinfo表处理完成!\n");
                }

                bgWorker.ReportProgress(38, "正在更新源库邮件信息!");
                int mailinfoIdGap = Moyu.GetIdGap(db1, db2, "cq_mailinfo");
                if (Moyu.UpdateTableId(db2, "cq_mailinfo", mailinfoIdGap))
                {
                    txtLogs.AppendText("cq_mailinfo表处理完成!\n");
                }

                bgWorker.ReportProgress(38, "正在更新源库女神信息!");
                int goddessIdGap = Moyu.GetIdGap(db1, db2, "cq_goddess");
                if (Moyu.UpdateTableId(db2, "cq_goddess", goddessIdGap))
                {
                    txtLogs.AppendText("cq_goddess表处理完成!\n");
                }

                int goddessservantIdGap = Moyu.GetIdGap(db1, db2, "cq_goddessservant");
                if (Moyu.UpdateTableId(db2, "cq_goddessservant", goddessservantIdGap))
                {
                    txtLogs.AppendText("cq_goddessservant表处理完成!\n");
                }

                bgWorker.ReportProgress(39, "正在更新源库人物新任务信息!");
                int newtaskdetailIdGap = Moyu.GetIdGap(db1, db2, "cq_newtaskdetail");
                if (Moyu.UpdateTableId(db2, "cq_newtaskdetail", newtaskdetailIdGap))
                {
                    txtLogs.AppendText("cq_newtaskdetail表处理完成!\n");
                }

            }

            bgWorker.ReportProgress(50, "源库信息更新完成,开始合并数据!");

            //合并数据
            txtLogs.AppendText("数据处理完成,开始合并数据!\n");

            bgWorker.ReportProgress(51, "正在合并账号数据");
            Moyu.CombineTable(db1, db2, "account");
            txtLogs.AppendText("account表数据合并完成!\n");

            bgWorker.ReportProgress(58, "正在合并密保数据");
            Moyu.CombineTable(db1, db2, "account_pwd");
            txtLogs.AppendText("account_pwd表数据合并完成!\n");

            bgWorker.ReportProgress(60, "正在合并角色数据");
            Moyu.CombineTable(db1, db2, userTable);
            txtLogs.AppendText(userTable + "表数据合并完成!\n");

            bgWorker.ReportProgress(70, "正在合并幻兽数据");
            Moyu.CombineTable(db1, db2, "cq_eudemon");
            txtLogs.AppendText("cq_eudemon表数据合并完成!\n");

            bgWorker.ReportProgress(80, "正在合并物品数据");
            Moyu.CombineTable(db1, db2, "cq_item");
            txtLogs.AppendText("cq_item表数据合并完成!\n");

            bgWorker.ReportProgress(83, "正在合并点卡数据");
            Moyu.CombineTable(db1, db2, "cq_card");
            txtLogs.AppendText("cq_card表数据合并完成!\n");
            Moyu.CombineTable(db1, db2, "cq_card2");
            txtLogs.AppendText("cq_card2表数据合并完成!\n");

            bgWorker.ReportProgress(90, "正在合并军团数据");
            Moyu.CombineTable(db1, db2, "cq_syndicate");
            Moyu.CombineTable(db1, db2, "cq_synattr");
            txtLogs.AppendText("cq_syndicate表数据合并完成!\n");

            bgWorker.ReportProgress(92, "正在合并家族数据");
            Moyu.CombineTable(db1, db2, "cq_family");
            Moyu.CombineTable(db1, db2, "cq_family_attr");
            txtLogs.AppendText("cq_family表数据合并完成!\n");

            bgWorker.ReportProgress(93, "正在合并技能数据");
            Moyu.CombineTable(db1, db2, "cq_magic");
            txtLogs.AppendText("cq_magic表数据合并完成!\n");

            bgWorker.ReportProgress(93, "正在合并杂项数据");
            Moyu.CombineTable(db1, db2, "cq_donation_dynasort_rec");
            txtLogs.AppendText("cq_donation_dynasort_rec表数据合并完成!\n");


            Moyu.CombineTable(db1, db2, "cq_enemy");
            txtLogs.AppendText("cq_enemy表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_friend");
            txtLogs.AppendText("cq_friend表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_partner");
            txtLogs.AppendText("cq_partner表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_tutor");
            txtLogs.AppendText("cq_tutor表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_announce");
            txtLogs.AppendText("cq_announce表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_statistic");
            txtLogs.AppendText("cq_statistic表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_special_status");
            txtLogs.AppendText("cq_special_status表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_taskdetail");
            txtLogs.AppendText("cq_announce表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_skill");
            txtLogs.AppendText("cq_skill表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_pk_item");
            txtLogs.AppendText("cq_pk_item表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_pk_bonus");
            txtLogs.AppendText("cq_pk_bonus表数据合并完成!\n");

            Moyu.CombineTable(db1, db2, "cq_flower");
            txtLogs.AppendText("cq_flower表数据合并完成!\n");
            bgWorker.ReportProgress(100, "数据合并完毕!");

            //导出重复名
            string path = Directory.GetCurrentDirectory() + "/合区日志" + DateTime.Now.ToString("MM月dd日HH点mm分") + ".txt";//
            StreamWriter sw = new StreamWriter(path, true);
            sw.WriteLine("-----重复账号-----");
            foreach (var var in listAccount)
                sw.WriteLine("重复账号: " + var.Key + "   新账号:" + var.Value);

            sw.WriteLine("\n\n\n-----重复人物-----");
            foreach (var var in listPlayer)
                sw.WriteLine("重复人物名: " + var.Key + "   新人物名:" + var.Value);

            sw.WriteLine("\n\n\n-----重复军团-----");
            foreach (var var in listSyn)
                sw.WriteLine("重复军团名: " + var.Key + "   新军团名:" + var.Value);

            sw.WriteLine("\n\n\n-----重复家族-----");
            foreach (var var in listFam)
                sw.WriteLine("重复家族名: " + var.Key + "   新家族名:" + var.Value);
            sw.Close();
        }

        public void reMakeId()
        {
            //子线程
            string dbName = txtDB1.Text;
            bool isNewDB = Convert.ToBoolean(cbxIsNewDB.Checked);
            txtLogs.AppendText("已开启多线程对ID不重要的表重新编号!\n");

            int card = Moyu.RemakeTableId(dbName, "cq_card");
            txtLogs.AppendText("cq_card表重新编号完成,共处理数据:" + card + "条\n");

            int card2 = Moyu.RemakeTableId(dbName, "cq_card2");
            txtLogs.AppendText("cq_card2表重新编号完成,共处理数据:" + card2 + "条\n");

            int goods = Moyu.RemakeTableId(dbName, "cq_goods");
            txtLogs.AppendText("cq_goods表重新编号完成,共处理数据:" + goods + "条\n");

            int friend = Moyu.RemakeTableId(dbName, "cq_friend");
            txtLogs.AppendText("cq_friend表重新编号完成,共处理数据:" + friend + "条\n");

            int enemy = Moyu.RemakeTableId(dbName, "cq_enemy");
            txtLogs.AppendText("cq_enemy表重新编号完成,共处理数据:" + enemy + "条\n");

            int partner = Moyu.RemakeTableId(dbName, "cq_partner");
            txtLogs.AppendText("cq_partner表重新编号完成,共处理数据:" + partner + "条\n");

            int tutor = Moyu.RemakeTableId(dbName, "cq_tutor");
            txtLogs.AppendText("cq_tutor表重新编号完成,共处理数据:" + tutor + "条\n");

            int taskdetail = Moyu.RemakeTableId(dbName, "cq_taskdetail");
            txtLogs.AppendText("cq_taskdetail表重新编号完成,共处理数据:" + taskdetail + "条\n");

            //影响脚本刷的怪物需要刷怪表ID，否则不会攻击人物
            /*int gen = Moyu.RemakeTableId(dbName, "cq_generator");
            txtLogs.AppendText("cq_generator表重新编号完成,共处理数据:"+gen + "条\n");*/

            int drop = Moyu.RemakeTableId(dbName, "cq_dropitemrule");
            txtLogs.AppendText("cq_dropitemrule表重新编号完成,共处理数据:" + drop + "条\n");

            int lottery = Moyu.RemakeTableId(dbName, "cq_lottery");
            txtLogs.AppendText("cq_lottery表重新编号完成,共处理数据:" + drop + "条\n");

            int magic = Moyu.RemakeTableId(dbName, "cq_magic");
            txtLogs.AppendText("cq_magic表重新编号完成,共处理数据:" + magic + "条\n");

            int skill = Moyu.RemakeTableId(dbName, "cq_skill");
            txtLogs.AppendText("cq_skill表重新编号完成,共处理数据:" + skill + "条\n");

            if (isNewDB)
            {
                int cq_faceinfo = Moyu.RemakeTableId(dbName, "cq_faceinfo");
                txtLogs.AppendText("cq_faceinfo表重新编号完成,共处理数据:" + cq_faceinfo + "条\n");

                int cq_hairinfo = Moyu.RemakeTableId(dbName, "cq_hairinfo");
                txtLogs.AppendText("cq_hairinfo表重新编号完成,共处理数据:" + cq_hairinfo + "条\n");

                int cq_eudlookinfo = Moyu.RemakeTableId(dbName, "cq_eudlookinfo");
                txtLogs.AppendText("cq_eudlookinfo表重新编号完成,共处理数据:" + cq_eudlookinfo + "条\n");

                int cq_packpetinfo = Moyu.RemakeTableId(dbName, "cq_packpetinfo");
                txtLogs.AppendText("cq_packpetinfo表重新编号完成,共处理数据:" + cq_packpetinfo + "条\n");

                int cq_titleid = Moyu.RemakeTableId(dbName, "cq_titleid");
                txtLogs.AppendText("cq_titleid表重新编号完成,共处理数据:" + cq_titleid + "条\n");

                int cq_mailinfo = Moyu.RemakeTableId(dbName, "cq_mailinfo");
                txtLogs.AppendText("cq_mailinfo表重新编号完成,共处理数据:" + cq_mailinfo + "条\n");

                int cq_goddess = Moyu.RemakeTableId(dbName, "cq_goddess");
                txtLogs.AppendText("cq_goddess表重新编号完成,共处理数据:" + cq_goddess + "条\n");

                int cq_newtaskdetail = Moyu.RemakeTableId(dbName, "cq_newtaskdetail");
                txtLogs.AppendText("cq_newtaskdetail表重新编号完成,共处理数据:" + cq_newtaskdetail + "条\n");

                int cq_goddessservant = Moyu.RemakeTableId(dbName, "cq_goddessservant");
                txtLogs.AppendText("cq_goddessservant表重新编号完成,共处理数据:" + cq_goddessservant + "条\n");

            }
            txtLogs.AppendText("所有常用表都已经重新编号完毕!\n");
        }

        private void bgWorker_ReamkeId(object sender, DoWorkEventArgs e)
        {
            //开起不重要表的ID编号线程
            ThreadStart refMakeId = new ThreadStart(reMakeId);
            Thread td = new Thread(refMakeId);
            td.Start();

            bool isNewDB = Convert.ToBoolean(cbxIsNewDB.Checked);
            string userTable = "cq_user";
            if (cbxIsNewDB.Checked)
            {
                userTable = "cq_user_new";
            }
            string dbName = txtDB1.Text;

            bgWorker.ReportProgress(0, "开始对数据表重新编号!");
            //账号重新编号
            string sqlAccount = String.Format("select id from {0}.account order by id desc;", dbName);
            DataTable dtAccount = MySqlHelper.GetDataTable(sqlAccount);
            prcBar1.Maximum = dtAccount.Rows.Count;
            //防止ID冲突
            string sqlAccountTmp = String.Format("use {0};" +
                "update account set id=2147483647-id;" +
                "update {1} set account_id =2147483647-account_id;" +
                "update cq_card set account_id =2147483647-account_id;" +
                "update cq_card2 set account_id =2147483647-account_id;"
                , dbName, userTable);
            MySqlHelper.Query(sqlAccountTmp);
            for (int i = 0; i < dtAccount.Rows.Count; i++)
            {
                string status = String.Format("正在对账号ID重新编号![{0}/{1}]", i + 1, dtAccount.Rows.Count);
                bgWorker.ReportProgress(i, status);
                int oldId = 2147483647 - Convert.ToInt32(dtAccount.Rows[i]["id"]);
                int newId = 1000001 + i;
                string sqlNewId = String.Format("use {0};" +
                "update account set id={1} where id={2};" +
                "update {3} set account_id ={4} where account_id={5};" +
                "update cq_card set account_id ={6},chk_sum = {7}^780000 where account_id={8};" +
                "update cq_card2 set account_id ={9},chk_sum = {10}^780001 where account_id={11};"
                , dbName, newId, oldId, userTable, newId, oldId, newId, newId, oldId, newId, newId, oldId);
                MySqlHelper.Query(sqlNewId);
            }
            int numAccountIncId = 1000001 + dtAccount.Rows.Count;
            string sqlAccountIncId = string.Format("ALTER TABLE {0}.account AUTO_INCREMENT = {1};", dbName, numAccountIncId);
            MySqlHelper.Query(sqlAccountIncId);

            //人物重新编号
            string sqlPlayer = String.Format("select id,emoney from {0}.{1};", dbName, userTable);
            DataTable dtPlayer = MySqlHelper.GetDataTable(sqlPlayer);
            prcBar1.Maximum = dtPlayer.Rows.Count;
            //防止ID冲突
            string sqlPlayerTmp = String.Format("use {0};" +
                "update {1} set id=2147483647-id;" +
                "update cq_synattr set id=2147483647-id;" +
                "update cq_family_attr set id=2147483647-id;"
                , dbName, userTable);
            MySqlHelper.Query(sqlPlayerTmp);
            for (int i = 0; i < dtPlayer.Rows.Count; i++)
            {
                string status = String.Format("正在对人物ID重新编号![{0}/{1}]", i + 1, dtPlayer.Rows.Count);
                bgWorker.ReportProgress(i, status);
                int oldId = Convert.ToInt32(dtPlayer.Rows[i]["id"]);
                int emoney = Convert.ToInt32(dtPlayer.Rows[i]["emoney"]);
                int newId = 10000001 + i;
                //其他表ID还是用老id替换
                Moyu.RemakeUserId(dbName, oldId, newId, isNewDB);
                //角色表ID单独处理
                int chk_sum = Moyu.GetEmoneyChkSum(newId, emoney, isNewDB);
                string reMakeId = String.Format("update {0}.{1} set id={2},chk_sum={3} where id=2147483647-{4};", dbName, userTable, newId, chk_sum, oldId);
                MySqlHelper.Query(reMakeId);
            }
            int numPlayerIncId = 10000001 + dtAccount.Rows.Count;
            string sqlPlayerIncId = string.Format("ALTER TABLE {0}.{1} AUTO_INCREMENT = {2};", dbName, userTable, numPlayerIncId);
            MySqlHelper.Query(sqlPlayerIncId);

            //幻兽重新编号
            string sqlEudemon = String.Format("SELECT id,(life_grow_rate + phyatk_grow_rate_max + phyatk_grow_rate + phydef_grow_rate + magicatk_grow_rate_max + magicatk_grow_rate + magicdef_grow_rate + luck + damage_type) AS t1," +
                "(initial_life + initial_phy + initial_magic + initial_def) AS t2," +
                "(mete_lev + talent1 + talent2 + talent3 + talent4 + talent5) AS t3," +
                "reborn_times AS zs FROM {0}.cq_eudemon;", dbName);
            DataTable dtEudemon = MySqlHelper.GetDataTable(sqlEudemon);
            prcBar1.Maximum = dtEudemon.Rows.Count;
            //防止ID冲突
            string sqlEudemonTmp = String.Format("update {0}.cq_eudemon set id=2147483647-id;", dbName);
            MySqlHelper.Query(sqlEudemonTmp);
            for (int i = 0; i < dtEudemon.Rows.Count; i++)
            {
                string status = String.Format("正在对幻兽ID重新编号![{0}/{1}]", i + 1, dtEudemon.Rows.Count);
                bgWorker.ReportProgress(i, status);
                int oldId = 2147483647 - Convert.ToInt32(dtEudemon.Rows[i]["id"]);
                int newId = 2012000001 + i;
                int t1 = Convert.ToInt32(dtEudemon.Rows[i]["t1"]);
                int t2 = Convert.ToInt32(dtEudemon.Rows[i]["t2"]);
                int t3 = Convert.ToInt32(dtEudemon.Rows[i]["t3"]);
                int zs = Convert.ToInt32(dtEudemon.Rows[i]["zs"]);
                int chksum = Moyu.GetEudChkSum(t1, t2, t3, newId, zs);
                string sqlNewId = String.Format("update {0}.cq_eudemon set id={1},chksum={2} where id={3};", dbName, newId, chksum, oldId);
                MySqlHelper.Query(sqlNewId);
            }
            int numEudemonIncId = 2012000001 + dtEudemon.Rows.Count;
            string sqlEudemonIncId = string.Format("ALTER TABLE {0}.cq_eudemon AUTO_INCREMENT = {1};", dbName, numEudemonIncId);
            MySqlHelper.Query(sqlEudemonIncId);

            //物品重新编号
            string sqlItem = String.Format("select id,gem1,gem2,type,magic3 from {0}.cq_item;", dbName);
            DataTable dtItem = MySqlHelper.GetDataTable(sqlItem);
            prcBar1.Maximum = dtItem.Rows.Count;
            //防止ID冲突
            string sqlItemTmp = String.Format("update {0}.cq_item set id=2147483647-id;", dbName);
            MySqlHelper.Query(sqlItemTmp);
            for (int i = 0; i < dtItem.Rows.Count; i++)
            {
                string status = String.Format("正在对物品ID重新编号![{0}/{1}]", i + 1, dtItem.Rows.Count);
                bgWorker.ReportProgress(i, status);
                int oldId = 2147483647 - Convert.ToInt32(dtItem.Rows[i]["id"]);
                int newId = 1 + i;
                int gem1 = Convert.ToInt32(dtItem.Rows[i]["gem1"]);
                int gem2 = Convert.ToInt32(dtItem.Rows[i]["gem2"]);
                int type = Convert.ToInt32(dtItem.Rows[i]["type"]);
                int magic3 = Convert.ToInt32(dtItem.Rows[i]["magic3"]);
                int chksum = Moyu.GetItemChkSum(gem1, gem2, newId, type, magic3);
                string sqlNewId = String.Format("update {0}.cq_item set id={1},chksum={2} where id={3};", dbName, newId, chksum, oldId);
                MySqlHelper.Query(sqlNewId);
            }
            int numItemIncId = 1 + dtItem.Rows.Count;
            string sqlItemIncId = string.Format("ALTER TABLE {0}.cq_item AUTO_INCREMENT = {1};", dbName, numItemIncId);
            MySqlHelper.Query(sqlItemIncId);

            //军团重新编号
            string sqlSyn = String.Format("select id from {0}.cq_syndicate;", dbName);
            DataTable dtSyn = MySqlHelper.GetDataTable(sqlSyn);
            prcBar1.Maximum = dtSyn.Rows.Count;
            //防止ID冲突
            string sqlSynTmp = String.Format("use {0};update cq_syndicate set id=2147483647-id;update cq_synattr set syn_id=2147483647-syn_id;", dbName);
            MySqlHelper.Query(sqlSynTmp);
            for (int i = 0; i < dtSyn.Rows.Count; i++)
            {
                string status = String.Format("正在对军团ID重新编号![{0}/{1}]", i + 1, dtSyn.Rows.Count);
                bgWorker.ReportProgress(i, status);
                int oldId = 2147483647 - Convert.ToInt32(dtSyn.Rows[i]["id"]);
                int newId = 1 + i;
                string sqlNewId = String.Format("use {0};update cq_syndicate set id={1} where id={2};update cq_synattr set syn_id={3} where syn_id={4};", dbName, newId, oldId, newId, oldId);
                MySqlHelper.Query(sqlNewId);
            }
            int numSynIncId = 1 + dtSyn.Rows.Count;
            string sqlSynIncId = string.Format("ALTER TABLE {0}.cq_syndicate AUTO_INCREMENT = {1};", dbName, numSynIncId);
            MySqlHelper.Query(sqlSynIncId);

            //家族重新编号
            string sqlFam = String.Format("select id from {0}.cq_family;", dbName);
            DataTable dtFam = MySqlHelper.GetDataTable(sqlFam);
            prcBar1.Maximum = dtFam.Rows.Count;
            //防止ID冲突
            string sqlFamTmp = String.Format("use {0};update cq_family set id=2147483647-id;update cq_family_attr set family_id=2147483647-family_id;", dbName);
            MySqlHelper.Query(sqlFamTmp);
            for (int i = 0; i < dtFam.Rows.Count; i++)
            {
                string status = String.Format("正在对家族ID重新编号![{0}/{1}]", i + 1, dtFam.Rows.Count);
                bgWorker.ReportProgress(i, status);
                int oldId = 2147483647 - Convert.ToInt32(dtFam.Rows[i]["id"]);
                int newId = 1 + i;
                string sqlNewId = String.Format("use {0};update cq_family set id={1} where id={2};update cq_family_attr set family_id={3} where family_id={4};", dbName, newId, oldId, newId, oldId);
                MySqlHelper.Query(sqlNewId);
            }
            int numFamIncId = 1 + dtFam.Rows.Count;
            string sqlFamIncId = string.Format("ALTER TABLE {0}.cq_family AUTO_INCREMENT = {1};", dbName, numFamIncId);
            MySqlHelper.Query(sqlFamIncId);


            //重置军团，家族，PK,摊位占领
            prcBar1.Maximum = 100;
            string sqlReset = String.Format("use {0};", dbName);
            bgWorker.ReportProgress(10, "正在重置军团战!");
            if (cbxResetSyn.Checked)
            {
                sqlReset += "UPDATE cq_dynanpc SET ownerid = 0, type = 10, life = 66000000, maxlife = 66000000 WHERE id = 21150;" +
                            "UPDATE cq_dynanpc SET ownerid=0, type=124,life=207000000, maxlife=207000000 WHERE id=21151;";
            }
            bgWorker.ReportProgress(30, "正在重置家族占领!");
            if (cbxResetFam.Checked)
            {
                sqlReset += "UPDATE cq_dynanpc SET ownerid=0,ownertype=0,task7=0,owner_name='',harvest_date=0 where name ='兰德尔';";
            }
            bgWorker.ReportProgress(40, "正在重置PK赛!");
            if (cbxResetPk.Checked)
            {
                sqlReset += "UPDATE `cq_dynanpc` SET `data0`='0' WHERE (`id` >= '10000' AND `id` <= '10019');";
            }
            bgWorker.ReportProgress(50, "正在重置摊位占领!");
            if (cbxResetStand.Checked)
            {
                sqlReset += "UPDATE cq_dynanpc SET ownerid=0,ownertype=0,owner_name='',price=0,deposit=0 where name ='摊位旗';" +
                            "UPDATE cq_dynanpc SET ownerid=0,ownertype=0,owner_name='',price=0,deposit=0 where name like'%城堡地皮';";
            }
            bgWorker.ReportProgress(60, "正在清理合出库数据!");
            if (cbxClearDB.Checked)
            {
                Moyu.ClearDB(txtDB2.Text, isNewDB);
                txtLogs.AppendText("合出库数据清理完成,可作为新区数据库使用!");
            }
            bgWorker.ReportProgress(100, "合区完毕!重复数据可在工具目录下查看！");
            btnCombine.Enabled = true;
        }

        public void bgWorker_ProgessChanged(object sender, ProgressChangedEventArgs e)
        {
            prcBar1.Value = e.ProgressPercentage;
            //labPercent.Text = "处理进度:" + Convert.ToString(e.ProgressPercentage) + "%";
            labStatus.Text = (string)e.UserState;
        }

        public void bgWorker_WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString());
                return;
            }
            if (!e.Cancelled)
            {
                watch.Stop();
                txtLogs.AppendText("\n合区完成,共用时(毫秒):" + watch.ElapsedMilliseconds);
                labStatus.Text = "合区完毕!重复数据可在工具目录下查看！";
                btnCombine.Enabled = true;
            }
            else
            {
                labStatus.Text = "合区终止!";
            }
        }


        private void btnUpdateCode_Click(object sender, EventArgs e)
        {
            if (Moyu.UpdateCode() > 0)
            {
                MessageBox.Show("更新成功!");
            }

        }
        private void btnClearDB_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认清理吗?数据一旦清理将无法恢复！", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            dr = MessageBox.Show("再次提醒，确定要清空数据库吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            if (Moyu.ClearDB(Moyu.DataBaseName, Moyu.IsNewDB) > 0)
            {
                MessageBox.Show("数据清理完毕!");
            }

        }

        //按等级清理
        private void btnLevelSearch_Click(object sender, EventArgs e)
        {
            dgvLevel.Rows.Clear();
            DataTable dt = Moyu.GetUserWithLevel(numLevel.Value.ToString());
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvLevel.Rows.Add();
                dgvLevel.Rows[index].Cells["level_id"].Value = dt.Rows[i]["id"];
                dgvLevel.Rows[index].Cells["level_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvLevel.Rows[index].Cells["level_last_login"].Value = dt.Rows[i]["last_login"];
                dgvLevel.Rows[index].Cells["level_level"].Value = dt.Rows[i]["level"];
                dgvLevel.Rows[index].Cells["level_money"].Value = dt.Rows[i]["money"];
                dgvLevel.Rows[index].Cells["level_emoney"].Value = dt.Rows[i]["emoney"];
                dgvLevel.Rows[index].Cells["level_donation"].Value = dt.Rows[i]["donation"];
                dgvLevel.Rows[index].Cells["level_medal_select"].Value = dt.Rows[i]["medal_select"];
            }
        }

        private void btnLevelSelectALL_Click(object sender, EventArgs e)
        {
            Moyu.SelectAll(dgvLevel);
        }

        private void btnLevelClear_Click(object sender, EventArgs e)
        {
            if (dgvLevel.SelectedRows.Count < 1) return;
            for (int i = dgvLevel.SelectedRows.Count - 1; i >= 0; i--)
            {
                Moyu.DelUser(dgvLevel.SelectedRows[i].Cells["level_id"].Value.ToString());
                dgvLevel.Rows.Remove(dgvLevel.SelectedRows[i]);
            }

        }

        //按登录清理
        private void btnLoginSearch_Click(object sender, EventArgs e)
        {
            dgvLogin.Rows.Clear();
            DataTable dt = Moyu.GetUserWithLastLogin(Convert.ToInt32(numLogin.Value));
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvLogin.Rows.Add();
                dgvLogin.Rows[index].Cells["login_id"].Value = dt.Rows[i]["id"];
                dgvLogin.Rows[index].Cells["login_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvLogin.Rows[index].Cells["login_last_login"].Value = dt.Rows[i]["last_login"];
                dgvLogin.Rows[index].Cells["login_level"].Value = dt.Rows[i]["level"];
                dgvLogin.Rows[index].Cells["login_money"].Value = dt.Rows[i]["money"];
                dgvLogin.Rows[index].Cells["login_emoney"].Value = dt.Rows[i]["emoney"];
                dgvLogin.Rows[index].Cells["login_donation"].Value = dt.Rows[i]["donation"];
                dgvLogin.Rows[index].Cells["login_medal_select"].Value = dt.Rows[i]["medal_select"];
            }
        }

        private void btnLoginSelectALL_Click(object sender, EventArgs e)
        {
            Moyu.SelectAll(dgvLogin);
        }

        private void btnLoginClear_Click(object sender, EventArgs e)
        {
            if (dgvLogin.SelectedRows.Count < 1) return;
            for (int i = dgvLogin.SelectedRows.Count - 1; i >= 0; i--)
            {
                Moyu.DelUser(dgvLogin.SelectedRows[i].Cells["login_id"].Value.ToString());
                dgvLogin.Rows.Remove(dgvLogin.SelectedRows[i]);
            }
        }

        private void btnAccountSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = Moyu.GetNoRoleAccount();
            if (dt.Rows.Count < 1) return;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int index = dgvAccount.Rows.Add();
                dgvAccount.Rows[index].Cells["account_id"].Value = dt.Rows[i]["id"];
                dgvAccount.Rows[index].Cells["account_name"].Value = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                dgvAccount.Rows[index].Cells["account_reg_date"].Value = dt.Rows[i]["reg_date"];
                dgvAccount.Rows[index].Cells["account_netbar_ip"].Value = dt.Rows[i]["netbar_ip"];
                dgvAccount.Rows[index].Cells["account_vip"].Value = dt.Rows[i]["vip"];
                dgvAccount.Rows[index].Cells["account_superpass"].Value = dt.Rows[i]["superpass"];

            }
        }

        private void btnAccountSelectAll_Click(object sender, EventArgs e)
        {
            Moyu.SelectAll(dgvAccount);
        }

        private void btnAccountClear_Click(object sender, EventArgs e)
        {
            if (dgvAccount.SelectedRows.Count < 1) return;
            for (int i = dgvAccount.SelectedRows.Count - 1; i >= 0; i--)
            {
                Moyu.DelAccount(dgvAccount.SelectedRows[i].Cells["account_id"].Value.ToString());
                dgvAccount.Rows.Remove(dgvAccount.SelectedRows[i]);
            }
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(txtDB1.Text.Trim()) || string.IsNullOrEmpty(txtDB2.Text.Trim()) || txtDB1.Text == txtDB2.Text)
            {
                MessageBox.Show("数据库填写有误！");
                return;
            }
            DialogResult dr = MessageBox.Show("确认开始合区吗?", "提示", MessageBoxButtons.OKCancel);
            if (dr != DialogResult.OK) return;
            if (bgWorker.IsBusy) return;
            prcBar1.Maximum = 100;
            txtLogs.Text = "";
            btnCombine.Enabled = false;
            bgWorker.RunWorkerAsync("hello");
            watch.Start();//开始计时
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            frmCombineInfo frm = new frmCombineInfo();
            frm.MdiParent = this.MdiParent;
            frm.StartPosition = FormStartPosition.Manual;
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        private void btnRubbish_Click(object sender, EventArgs e)
        {
            txtRub.Text = "";
            btnRubbish.Enabled = false;
            if (cbxUser.Checked)
            {
                DataTable dtUser = Moyu.GetNoAccountRole();
                if (dtUser.Rows.Count > 0)
                {
                    prcBar.Maximum = dtUser.Rows.Count;
                    for (int i = 0; i < dtUser.Rows.Count; i++)
                    {
                        prcBar.Value = i + 1;
                        Moyu.DelUser(dtUser.Rows[i]["id"].ToString());
                    }
                    //Moyu.DelUser)
                }
                int pwd = MySqlHelper.Query("delete account_pwd from account_pwd as a left join account as b on a.name=b.name where b.name is null;");
                txtRub.AppendText("已清理无账号人物数据:" + dtUser.Rows.Count + "条\n");
                txtRub.AppendText("已清理无账号密保数据:" + pwd + "条\n");
            }
            if (cbxCard.Checked)
            {
                int card = MySqlHelper.Query("delete from cq_card where used=1;delete from cq_card2 where used=1;");
                txtRub.AppendText("已清理已领取点卡数据:" + card + "条\n");
            }
            if (cbxItem.Checked)
            {
                int itemNull = MySqlHelper.Query("delete cq_item from cq_item as a left join cq_itemtype as b on a.type=b.id where b.id is null;");
                txtRub.AppendText("已清理不存在的物品数据:" + itemNull + "条\n");

                int itemNoPlayer = MySqlHelper.Query(String.Format("delete cq_item from cq_item as a left join {0} as b on a.player_id=b.id where b.id is null;", Moyu.tName));
                txtRub.AppendText("已清理垃圾物品数据:" + itemNoPlayer + "条\n");
            }
            if (cbxEud.Checked)
            {
                int eud = MySqlHelper.Query(String.Format("delete cq_eudemon from cq_eudemon as a left join {0} as b on a.player_id=b.id where b.id is null;", Moyu.tName));
                txtRub.AppendText("已清理垃圾幻兽数据:" + eud + "条\n");
            }

            if (cbxGen.Checked)
            {
                int genNoMap = MySqlHelper.Query("delete cq_generator from cq_generator as a left join cq_map as b on a.mapid=b.id where b.id is null;");
                txtRub.AppendText("已清理不存在的刷怪地图数据:" + genNoMap + "条\n");

                int genNoMon = MySqlHelper.Query("delete cq_generator from cq_generator as a left join cq_monstertype as b on a.npctype=b.id where b.id is null;");
                txtRub.AppendText("已清理不存在的刷怪怪物数据:" + genNoMon + "条\n");
            }

            if (cbxMagic.Checked)
            {
                int magic = MySqlHelper.Query(String.Format("delete cq_magic from cq_magic as a LEFT JOIN cq_eudemon as b on a.ownerid=b.id LEFT JOIN {0} as c on a.ownerid=c.id where b.id is null and c.id is null;",Moyu.tName));
                txtRub.AppendText("已清理垃圾技能数据:" + magic + "条\n");
            }
            if (cbxSkill.Checked)
            {

                int skill = MySqlHelper.Query(String.Format("delete cq_skill from cq_skill as a left join {0} as b on a.owner_id = b.id where b.id is null;", Moyu.tName));
                txtRub.AppendText("已清理垃圾图鉴数据:" + skill + "条\n");
            }
            if (cbxFriend.Checked)
            {
                int friend = MySqlHelper.Query(String.Format("delete cq_friend from cq_friend as a left join {0} as b on a.userid=b.id or a.friend=b.id where b.id is null;" +
                    "delete cq_enemy from cq_enemy as a left join {1} as b on a.userid=b.id or a.enemy=b.id where b.id is null;" +
                    "delete cq_partner from cq_partner as a left join {2} as b on a.user_id=b.id or a.partner_id=b.id where b.id is null;" +
                    "delete cq_tutor from cq_tutor as a left join {3} as b on a.user_id=b.id or a.tutor_id=b.id where b.id is null;", Moyu.tName, Moyu.tName, Moyu.tName, Moyu.tName));
                txtRub.AppendText("已清理垃圾社交数据:" + friend + "条\n");
            }
            if (cbxSyn.Checked)
            {
                int syn = MySqlHelper.Query(String.Format("delete cq_syndicate from cq_syndicate as a left join {0} as b on a.leader_id=b.id where b.id is null;" +
                    "delete cq_synattr from cq_synattr as a left join {1} as b on a.id=b.id where b.id is null", Moyu.tName, Moyu.tName));
                txtRub.AppendText("已清理垃圾军团数据:" + syn + "条\n");
            }
            if (cbxFam.Checked)
            {
                int fam = MySqlHelper.Query(String.Format("delete cq_family from cq_family as a left join {0} as b on a.leader_id=b.id where b.id is null;" +
                "delete cq_family_attr from cq_family_attr as a left join {1} as b on a.id=b.id where b.id is null", Moyu.tName, Moyu.tName));
                txtRub.AppendText("已清理垃圾家族数据:" + fam + "条\n");
            }
            txtRub.AppendText("所有垃圾数据清理完成!");
            btnRubbish.Enabled = true;

            //select a.id,a.ownerid from cq_magic as a LEFT JOIN cq_eudemon as b on a.ownerid=b.id LEFT JOIN cq_user as c on a.ownerid=c.id where b.id is null and c.id is null
        }

    }
}
