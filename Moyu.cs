using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace gmtoolNew
{
    class Moyu
    {
        public static bool IsNewDB = false; //是否新端
        public static string DataBaseName = null;
        public static string tName = "cq_user";
        public static string accountName = null;
        public static string playerName = null;
        public static string strNewName = null;
        public static int account_id = 0;
        public static int player_id = 0;
        public static List<string> strList = new List<string>();

        #region 内存读写相关
        [DllImport("kernel32.dll", EntryPoint = "OpenFileMappingA")]
        private static extern int OpenFileMappingA(int dwDesiredAccess, bool bInheritHandle, string lpName);

        [DllImport("kernel32.dll", EntryPoint = "MapViewOfFile")]
        private static extern int MapViewOfFile(int hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", EntryPoint = "UnmapViewOfFile")]
        private static extern int UnmapViewOfFile(int lpBaseAddress);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        private static extern int CloseHandle(int hObject);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        private static extern int MemoryReadByteSet(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        private static extern int MemoryReadInt32(int hProcess, int lpBaseAddress, ref int lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        private static extern int MemoryWriteByteSet(int hProcess, int lpBaseAddress, byte[] lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", EntryPoint = "WriteProcessMemory")]
        private static extern int MemoryWriteInt32(int hProcess, int lpBaseAddress, ref int lpBuffer, int nSize, int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", EntryPoint = "GetCurrentProcess")]
        private static extern int GetCurrentProcess();

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        private static extern int OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern int CopyMemory_ByteSet_Float(ref float item, ref byte source, int length);


        /// <summary>
        /// 读内存整数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <returns>0失败</returns>
        public static int ReadMemoryInt32(int pID, int bAddress)
        {
            int num = 0;
            int handle = GetProcessHandle(pID);
            int num3 = MemoryReadInt32(handle, bAddress, ref num, 4, 0);
            CloseHandle(handle);
            if (num3 == 0)
            {
                return 0;
            }
            else
            {
                return num;
            }
        }

        /// <summary>
        /// 写内存整数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <param name="value">写入值</param>
        /// <returns>false失败 true成功</returns>
        public static bool WriteMemoryInt32(int pID, int bAddress, int value)
        {
            int handle = GetProcessHandle(pID);
            int num = MemoryWriteInt32(handle, bAddress, ref value, 4, 0);
            CloseHandle(handle);
            return num != 0;
        }

        /// <summary>
        /// 取进程句柄
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <returns>进程句柄</returns>
        public static int GetProcessHandle(int pID)
        {
            if (pID == -1)
            {
                return GetCurrentProcess();
            }
            else
            {
                return OpenProcess(2035711, 0, pID);
            }
        }
        #endregion




        //输入判断，只允许输入正整数
        public static void InputCheck(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar) || e.KeyChar == '\b'))
            {
                e.Handled = true;
            }
        }

        //清空控件
        public static void ClearInput(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                if (c is TextBox)
                {
                    c.Text = "";
                }
                if (c is ListBox)
                {
                    ((ListBox)(c)).Items.Clear();
                }
                if (c is ComboBox)
                {
                    ((ComboBox)(c)).SelectedIndex = -1;
                }
                ClearInput(c);
            }
        }

        //巧用控件遍历生成Update语句
        //文本框tag属性标记数据库字段,0 老端 1新端
        //文本框name属性命名规则 "edit_数据库字段名"
        public static string UpdateSqlBuild(Control controlName)
        {
            string sql = "";
            foreach (Control c in controlName.Controls)
            {
                if (c is TextBox && c.Tag != null)
                {
                    if (c.Tag.ToString() == "0")
                    {
                        string field = c.Name.Remove(0, 5);
                        sql += String.Format("{0}='{1}',", field, c.Text);
                    }
                    if (IsNewDB && c.Tag.ToString() == "1")
                    {
                        string field = c.Name.Remove(0, 5);
                        sql += String.Format("{0}='{1}',", field, c.Text);
                    }
                }


            }
            sql = sql.Substring(0, sql.Length - 1);//处理最后一个逗号
            return sql; //处理最后一个逗号
        }

        //改变子控件状态
        public static void ChangeContorlStatus(Control controlName, bool status)
        {
            foreach (Control c in controlName.Controls)
            {
                if (c is TextBox || c is ComboBox)
                {
                    c.Enabled = status;
                }
            }
        }

        //根据value获取combobox索引值
        public static int GetCbbIndex(ComboBox c, string value)
        {
            int index = 0;
            for (int i = 0; i < c.Items.Count; i++)
            {
                string str = ((ComboxItem)c.Items[i]).Values;
                if (str == value) index = i;
            }
            return index;
        }

        //根据value获取combobox内容
        public static string GetCbbItem(ComboBox c, string value)
        {
            string item = "";
            for (int i = 0; i < c.Items.Count; i++)
            {
                string str = ((ComboxItem)c.Items[i]).Values;
                if (str == value) item = ((ComboxItem)c.Items[i]).Text;
            }
            return item;
        }

        //DGV全选
        public static void SelectAll(DataGridView dgv)
        {
            //结束列表的编辑状态,否则可能无法改变CheckBox的状态
            dgv.EndEdit();
            for (var i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Selected = true;
                //dgv.Rows[i].Cells[0].Value = true;//设置为选中状态
            }
        }

        //修复数据库二进制字段
        public static bool FixDataBase()
        {
            string sql = String.Format("ALTER TABLE `cq_action` MODIFY COLUMN `param`  char(255) BINARY NULL DEFAULT NULL AFTER `data`;" +
                "ALTER TABLE `account` MODIFY COLUMN `name`  varchar(32) BINARY NOT NULL DEFAULT '' AFTER `id`,MODIFY COLUMN `password`  varchar(32) BINARY NOT NULL DEFAULT '' AFTER `name`;" +
                "ALTER TABLE `cq_itemtype` MODIFY COLUMN `name`  varchar(15) BINARY NOT NULL DEFAULT '' AFTER `id`;" +
                "ALTER TABLE `cq_monstertype` MODIFY COLUMN `name`  char(15) BINARY NOT NULL DEFAULT '无' AFTER `id`;" +
                "ALTER TABLE `cq_map` MODIFY COLUMN `name`  char(15) BINARY NULL DEFAULT '未命名' AFTER `id`;" +
                "ALTER TABLE `{0}` MODIFY COLUMN `name`  varchar(15) BINARY NOT NULL DEFAULT '' FIRST;" +
                "ALTER TABLE `cq_npc` MODIFY COLUMN `name`  varchar(24) BINARY NULL DEFAULT '未命名' AFTER `playerid`;" +
                "ALTER TABLE `account_pwd` MODIFY COLUMN `reg_pwd1`  varchar(50) BINARY NULL DEFAULT '' AFTER `reg_wen1`;" +
                "ALTER TABLE `cq_family` MODIFY COLUMN `family_name`  varchar(15) BINARY NOT NULL DEFAULT '' AFTER `id`;" +
                "ALTER TABLE `cq_syndicate` MODIFY COLUMN `NAME`  varchar(15) BINARY NOT NULL DEFAULT '' AFTER `id`;", tName);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //新区数据清理
        public static int ClearDB(string dbName, bool isNewDB = false)
        {
            string userTable = "cq_user";
            if (isNewDB) userTable = "cq_user_new";
            string sql = String.Format("use {0};TRUNCATE TABLE {1};ALTER TABLE {2} AUTO_INCREMENT=10000001;", dbName, userTable, userTable);
            sql += "TRUNCATE TABLE account;" +
                "TRUNCATE TABLE account_pwd;" +
                "TRUNCATE TABLE cq_user_timeout;" +
                "TRUNCATE TABLE cq_deluser;" +
                "TRUNCATE TABLE cq_item;" +
                "TRUNCATE TABLE cq_eudemon;" +
                "TRUNCATE TABLE cq_eudemon_timeout;" +
                "TRUNCATE TABLE cq_skill;" +
                "TRUNCATE TABLE cq_pk_bonus;" +
                "TRUNCATE TABLE cq_pk_item;" +
                "TRUNCATE TABLE cq_castle;" +
                "TRUNCATE TABLE cq_castleitem;" +
                "UPDATE cq_dynanpc SET ownerid=0,ownertype=0,owner_name='',price=0,deposit=0 where name ='摊位旗';" +
                "UPDATE cq_dynanpc SET ownerid=0,ownertype=0,owner_name='',price=0,deposit=0 where name like'%城堡地皮';" +
                "UPDATE `cq_dynanpc` SET `data0`='0' WHERE (`id` >= '10000' AND `id` <= '10019');" +
                "TRUNCATE TABLE cq_family;" +
                "TRUNCATE TABLE cq_family_attr;" +
                "UPDATE cq_dynanpc SET ownerid=0,ownertype=0,task7=0,owner_name='',harvest_date=0 where name ='兰德尔';" +
                "TRUNCATE TABLE cq_synattr;" +
                "TRUNCATE TABLE cq_syndicate;" +
                "TRUNCATE TABLE cq_totem_add;" +
                "UPDATE cq_dynanpc SET ownerid=0, type=10,life=66000000, maxlife=66000000 WHERE id=21150;" +
                "UPDATE cq_dynanpc SET ownerid=0, type=124,life=207000000, maxlife=207000000 WHERE id=21151;" +
                "TRUNCATE TABLE cq_magic;" +
                "TRUNCATE TABLE cq_donation_dynasort_rec;" +
                "TRUNCATE TABLE cq_enemy;" +
                "TRUNCATE TABLE cq_friend;" +
                "TRUNCATE TABLE cq_tutor;" +
                "TRUNCATE TABLE cq_tutorEXP;" +
                "TRUNCATE TABLE cq_announce;" +
                "TRUNCATE TABLE cq_partner;" +
                "TRUNCATE TABLE cq_special_status;" +
                "TRUNCATE TABLE cq_ad_log;" +
                "TRUNCATE TABLE cq_ad_queue;" +
                "TRUNCATE TABLE cq_card;" +
                "TRUNCATE TABLE cq_card2;" +
                "TRUNCATE TABLE cq_card3;" +
                "TRUNCATE TABLE cq_taskcomplete;" +
                "TRUNCATE TABLE cq_taskdetail;" +
                "TRUNCATE TABLE cq_taskdetail_timeout;" +
                "TRUNCATE TABLE cq_statistic;" +
                "TRUNCATE TABLE cq_addresslist;" +
                "TRUNCATE TABLE cq_ip_log;" +
                "TRUNCATE TABLE cq_npc_income;" +
                "TRUNCATE TABLE cq_shortcut_key;" +
                "TRUNCATE TABLE cq_suspicion_event;" +
                "TRUNCATE TABLE e_money;" +
                "TRUNCATE TABLE cq_pet;" +
                "TRUNCATE TABLE cq_flower;" +
                "TRUNCATE TABLE cq_advert_log;" +
                "TRUNCATE TABLE cq_advert_queue;" +
                "TRUNCATE TABLE cq_dyna_rank_rec;" +
                "TRUNCATE TABLE fee;" +
                "TRUNCATE TABLE cq_trade;" +
                "TRUNCATE TABLE emoneysum;" +
                "TRUNCATE TABLE cq_token;" +
                "ALTER TABLE account AUTO_INCREMENT=1000001;" +
                "ALTER TABLE cq_eudemon AUTO_INCREMENT=2012000001;" +
                "update cq_config set data1=1 where type=0;";//身份牌复位
            if (isNewDB)
            {
                sql += "TRUNCATE TABLE `cq_faceinfo`;" +
                    "TRUNCATE TABLE `cq_hairinfo`;" +
                    "TRUNCATE TABLE `cq_mailinfo`;" +
                    "TRUNCATE TABLE `cq_packpetinfo`;" +
                    "TRUNCATE TABLE `cq_goddess`;" +
                    "TRUNCATE TABLE `cq_newtaskdetail`;" +
                    "TRUNCATE TABLE `cq_titleid`;" +
                    "TRUNCATE TABLE `cq_eudlookinfo`;" +
                    "TRUNCATE TABLE `account_phone_mail`;" +
                    "TRUNCATE TABLE `cq_goddessservant`;";
            }
            return MySqlHelper.Query(sql);
        }

        //添加账号
        public static bool AddAccount(string name, string password, string superpass = "", string vip = "0")
        {
            if (AccountName2Id(name) != -1) return false;
            string pass = String.Format("md5('{0}，。fdjf,jkgfkl')", password);
            string sql = String.Format("INSERT INTO `account` (`name`, `password`, `vip`, `superpass`) VALUES ('{0}', {1}, '{2}', '{3}');", name, pass, vip, superpass);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //更新防挂机验证码
        public static int UpdateCode()
        {
            string sql = "";
            for (int i = 1; i < 9; i++)
            {
                var k = new Random(Guid.NewGuid().GetHashCode());//随机种子，确保每次随机数不同。
                int code = k.Next(1000, 9999);
                string sql1 = String.Format("UPDATE `cq_action` SET `param`='15 20021{0}2 请输入验证码[{1}]' WHERE (`id`='20021{2}1');", i, code, i);
                string sql2 = String.Format("UPDATE `cq_action` SET `param`='{0}' WHERE (`id`='20021{1}2');", code, i);
                sql += sql1 + sql2;
            }
            return MySqlHelper.Query(sql);
        }

        //获取所有账号和主号角色
        public static DataTable GetAccountList()
        {
            string sql = String.Format("select a.name as account_name,b.name as player_name,a.id as account_id,b.id as player_id from account as a left join {0} as b on a.id=b.account_id and b.type=0 order by a.id;", tName);
            return MySqlHelper.GetDataTable(sql);
        }

        //查找账号角色列表
        public static DataTable FindUserByAccountOrName(string name, int type = 0)
        {
            string sql = String.Format("select a.name as account_name,b.id,b.name,b.type,b.account_id from account as a left join {0} as b on a.id=b.account_id where a.name='{1}';", tName, name);
            if (type == 1)
            {   //以人物名为查询条件时，也要获得所有角色列表。
                sql = String.Format("select account_id from {0} where name='{1}';", tName, name);
                object o = MySqlHelper.Find(sql);
                if (o == null) throw new Exception("没有这个角色!");
                sql = String.Format("select a.name as account_name,b.id,b.name,b.type,b.account_id from account as a left join {0} as b on a.id=b.account_id where b.account_id='{1}';", tName, o.ToString());
            }
            DataTable dt = MySqlHelper.GetDataTable(sql);
            if (dt.Rows.Count < 1) throw new Exception("没有这个账号!");
            if (DBNull.Value == dt.Rows[0]["name"]) throw new Exception("账号存在，但未创建角色！");
            return dt;

        }

        //获取角色属性
        public static MySqlDataReader GetUserInfo(string name)
        {
            string sql = String.Format("select a.name as account_name,a.superpass,a.online,a.vip,b.* from account as a left join {0} as b on a.id=b.account_id where b.name='{1}';", tName, name);
            MySqlDataReader r = MySqlHelper.Select(sql);
            return r;
        }

        //获取玩家硬件码
        public static string GetUserHardWare(string accountName)
        {
            string str = "";
            string sql = String.Format("select reg_pwd1 from account_pwd where name='{0}';", accountName);
            object o = MySqlHelper.Find(sql);
            if (o != null) str = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])o);
            return str;
        }

        //查账号ID。
        public static int AccountName2Id(string accountName)
        {
            string sql = String.Format("select id from account where name='{0}';", accountName);
            object o = MySqlHelper.Find(sql);
            if (o != null) return Convert.ToInt32(o);
            return -1;
        }

        //查角色ID
        public static int PlayName2Id(string playerName)
        {
            string sql = String.Format("select id from {0} where name='{1}';", tName, playerName);
            object o = MySqlHelper.Find(sql);
            if (o != null) return Convert.ToInt32(o);
            return -1;
        }

        //获取角色物品

        //账号过户
        public static bool ChangeAccountOwner(string oldAccount, string oldPlayerName, string newAccount, string newPlayerName, string newSuperPass, string newElock, string newVIP, string newPass = null)
        {
            string sql = "";
            //密码
            if (newPass != null)
            {
                sql += String.Format("update account set password=md5('{0}，。fdjf,jkgfkl') where name='{1}';", newPass, oldAccount);
            }

            //账号，超级密码
            sql += String.Format("update account set name='{0}',superpass='{1}',vip='{2}' where name='{3}';", newAccount, newSuperPass, newVIP, oldAccount);
            //人物名字、仓库密码
            sql += String.Format("update {0} set name='{1}',elock='{2}' where name='{3}';", tName, newPlayerName, newElock, oldPlayerName);
            sql += String.Format("update {0} set mate='{1}' where mate='{2}';", tName, newPlayerName, oldPlayerName); //配偶
            sql += String.Format("update cq_donation_dynasort_rec set user_name='{0}' where user_name='{1}';", newPlayerName, oldPlayerName);//爵位排序
            sql += String.Format("update cq_eudemon set ori_owner_name='{0}' where ori_owner_name='{1}';", newPlayerName, oldPlayerName);//宝宝归属
            sql += String.Format("update cq_enemy set enemyname='{0}' where enemyname='{1}';", newPlayerName, oldPlayerName);//仇人
            sql += String.Format("update cq_friend set friendname='{0}' where friendname='{1}';", newPlayerName, oldPlayerName);//好友
            sql += String.Format("update cq_family set leader_name='{0}' where leader_name='{1}';", newPlayerName, oldPlayerName);//家族
            sql += String.Format("update cq_syndicate set leader_name='{0}' where leader_name='{1}';", newPlayerName, oldPlayerName);//军团
            sql += String.Format("update cq_partner set partner_name='{0}' where partner_name='{1}';", newPlayerName, oldPlayerName);//商业伙伴
            sql += String.Format("update cq_tutor set user_name='{0}' where user_name='{1}';", newPlayerName, oldPlayerName);//师徒
            sql += String.Format("update cq_tutor set tutor_name='{0}' where tutor_name='{1}';", newPlayerName, oldPlayerName);
            sql += String.Format("update cq_pk_item set target_name='{0}' where target_name='{1}';", newPlayerName, oldPlayerName);//扣押装备
            sql += String.Format("update cq_pk_item set hunter_name='{0}' where hunter_name='{1}';", newPlayerName, oldPlayerName);
            sql += String.Format("update cq_pk_bonus set target_name='{0}' where target_name='{1}';", newPlayerName, oldPlayerName);
            sql += String.Format("update cq_pk_bonus set hunter_name='{0}' where hunter_name='{1}';", newPlayerName, oldPlayerName);
            MySqlHelper.Query(sql);
            return true;
        }

        //幻兽chksum算法
        /*
         * SELECT
	        id,
	        chksum,
	        (
		        life_grow_rate + phyatk_grow_rate_max + phyatk_grow_rate + phydef_grow_rate + magicatk_grow_rate_max + magicatk_grow_rate + magicdef_grow_rate + luck + damage_type
	        ) AS t1,
	        (
		        initial_life + initial_phy + initial_magic + initial_def
	        ) AS t2,
	        (mete_lev + talent1 + talent2 + talent3 + talent4 + talent5) AS t3,
	        reborn_times as zs
            FROM
	        cq_eudemon;
        */
        public static int GetEudChkSum(int temp1, int temp2, int temp3, int id, int zs)
        {
            int[] arr = { 70471, 978795, 498781, 628719, 4935, 65387, 60509, 2725871, 801, 8500, 14114, 65805 };
            int a1, a2, a3, a4, a5, a6;
            string a3st;
            a1 = id;
            a2 = zs;
            a3 = a1;
            a6 = a2;
            a3 = a3 & 7;
            a6 = a6 & 3;
            a4 = arr[a6];
            a6 = temp2;
            a5 = arr[a3 + 4];
            a3 = temp3;
            a5 = a5 + a4;
            a4 = a3;
            a4 = a4 ^ a6;
            a4 = a4 + a1;
            a3 = a3 >> 1;
            a4 = a4 + a5;
            a5 = temp1;
            a3 = a3 + a4;
            a3 = a3 + a5;
            a3 = a3 + a6;
            a3st = (a3 + a2 + 2294967296).ToString("X");
            a3st = a3st.Substring(a3st.Length - 8, 8);
            a3 = Int32.Parse(a3st, System.Globalization.NumberStyles.HexNumber);
            return a3;
        }

        //物品chksum算法
        public static int GetItemChkSum(int gem1, int gem2, int id, int type, int magic3)
        {
            int[] arr1 = { 41, 46, 67, 201, 162, 216, 124, 1, 61, 54, 84, 161, 236, 240, 6, 19, 98, 167, 5, 243, 192, 199, 115, 140, 152, 147, 43, 217, 188, 76, 130, 202, 30, 155, 87, 60,
                253, 212, 224, 22, 103, 66, 111, 24, 138, 23, 229,18, 190, 78, 196, 214, 218, 158, 222, 73, 160, 251, 245, 142, 187, 47, 238, 122, 169, 104, 121, 145, 21, 178, 7, 63, 148, 194,
                16, 137, 11, 34, 95, 33, 128, 127, 93, 154, 90, 144, 50, 39, 53, 62, 204, 231, 191, 247,151, 3, 255, 25, 48, 179, 72, 165, 181, 209, 215, 94, 146, 42, 172, 86, 170, 198, 79, 184,
                56, 210, 150, 164, 125, 182, 118, 252, 107, 226, 156, 116, 4, 241, 69, 157, 112, 89, 100, 113, 135, 32, 134, 91, 207, 101, 230,45, 168, 2, 27, 96, 37, 173, 174, 176, 185, 246, 28,
                70, 97, 105, 52, 64, 126, 15, 85, 71, 163, 35, 221, 81, 175, 58, 195, 92, 249, 206, 186, 197, 234, 38, 44, 83, 13, 110, 133, 40, 132, 9, 211, 223, 205, 244, 65, 129,77, 82, 106, 220,
                55, 200, 108, 193, 171, 250, 36, 225, 123, 8, 12, 189, 177, 74, 120, 136, 149, 139, 227, 99, 232, 109, 233, 203, 213, 254, 59, 0, 29, 57, 242, 239, 183, 14, 102, 88, 208, 228, 166,
                119, 114, 248, 235,117, 75, 10, 49, 68, 80, 180, 143, 237, 31, 26, 219, 153, 141, 51, 159, 17, 131, 20};
            int[] arr2 = { 70471, 978795, 498781, 628719, 801, 8500, 14114, 65805, 5378887, 1240939, 2333789, 54586956 };
            int temp1, temp2;
            temp1 = gem1;
            temp1 = temp1 & 255;
            temp1 = arr1[temp1];
            temp2 = 7 & id;
            temp2 = arr2[temp2];
            temp1 = temp1 + temp2;
            temp2 = magic3;
            temp2 = temp2 & 3;
            temp2 = arr2[temp2 + 8];
            temp1 = temp1 + temp2;
            temp2 = type;
            temp2 = temp2 ^ gem1;
            temp2 = temp2 + id;
            temp1 = temp1 + temp2;
            temp2 = gem2;
            temp1 = temp1 + temp2 * 8;
            temp1 = temp1 + type;
            temp1 = temp1 + gem1;
            temp1 = temp1 + magic3;
            return temp1;
        }

        //魔石chk_sum算法
        public static int GetEmoneyChkSum(int player_id, int emoney, bool isNewDB = false)
        {
            int[,] key = new int[2, 3] { { 29565076, 3118018, 9581781 }, { 5581781, 19565076, 2118018 } };
            int i = emoney % 3;
            int v = key[0, i];
            if (isNewDB)
            {
                v = key[1, i];
            }
            int sum = (player_id + v) ^ emoney;
            return sum;
        }

        //魔石卡充值
        public static int AddCard(int num, int type)
        {
            int chk_sum = Moyu.account_id ^ 780000;
            string table = "cq_card";
            int count = 0;
            if (type == 1)
            {
                chk_sum = Moyu.account_id ^ 780001;
                table = "cq_card2";
            }
            string sql = String.Format("INSERT INTO {0} (`type`, `account_id`, `ref_id`, `chk_sum`, `time_stamp`, `used`) VALUES ('1', '{1}', '1', '{2}', '0', '0');", table, Moyu.account_id, chk_sum);
            for (int i = 0; i < num; i++)
            {
                count += MySqlHelper.Query(sql);
            }
            return count;
        }

        //装备显示名称
        public static string GetItemDisplayName(int itemtype_id, string itemtype_name, int item_magic3 = 0)
        {
            string name = "";
            Int32 lastNum = itemtype_id % 10;
            switch (lastNum)
            {
                case 1:
                    name += "良品";
                    break;
                case 2:
                    name += "上品";
                    break;
                case 3:
                    name += "精品";
                    break;
                case 4:
                    name += "极品";
                    break;
                case 5:
                    name += "上品神器";
                    break;
                case 6:
                    name += "精品神器";
                    break;
                case 7:
                    name += "极品神器";
                    break;
            }
            name += itemtype_name;
            if (item_magic3 > 0)
            {
                name += String.Format("(+{0})", item_magic3.ToString());
            }
            return name;
        }

        //装备显示颜色
        public static Color GetItemDisplayColor(int itemtype_id)
        {
            Color color = Color.Black;
            Int32 lastNum = itemtype_id % 10;
            switch (lastNum)
            {
                case 0:
                    color = Color.Black;
                    break;
                case 1:
                    color = Color.Green;
                    break;
                case 2:
                    color = Color.SkyBlue;
                    break;
                case 3:
                    color = Color.Red;
                    break;
                case 4:
                    color = Color.SandyBrown;
                    break;
                case 5:
                    color = Color.Gold;
                    break;
                case 6:
                    color = Color.Gold;
                    break;
                case 7:
                    color = Color.Gold;
                    break;
            }
            return color;
        }

        //物品详细信息
        public static DataTable GetItemInfo(int item_id)
        {
            string sql = String.Format("select * from cq_item where id='{0}';", item_id);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return dt;
        }

        //删除物品
        public static int DelItem(int item_id)
        {
            string sql = String.Format("delete from cq_item where id='{0}';", item_id);
            return MySqlHelper.Query(sql);
        }

        //获取物品列表,type=1只获取装备,typ=2模糊查找
        public static DataTable GetItemList(int type = 0, string keyword = "")
        {
            string sql = "select * from cq_itemtype where req_profession=0 order by id";
            if (type == 1)
            {
                sql = "select * from cq_itemtype where req_profession>0 and amount>1 order by id;";
            }
            else if (type == 2)
            {
                sql = String.Format("select * from cq_itemtype where name like'%{0}%' or id like'%{1}%';", keyword, keyword);
            }
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return dt;
        }

        //刷物品(宝石类物品需要gem1参数，否则无法使用)
        public static int GiveItem(int player_id, int itemtype_id, int amount = 1, int gem1 = 0)
        {
            string sql = String.Format("INSERT INTO `cq_item` (`TYPE`, `owner_id`, `player_id`, `amount`, `amount_limit`, `POSITION`, `availabletime`, `chksum`, `monopoly`,`gem1`) " +
                "values('{0}', '{1}', '{2}', '{3}','{4}', '50', '0', '0', '0','{5}');", itemtype_id.ToString(), player_id.ToString(), player_id.ToString(), amount.ToString(), amount.ToString(), gem1.ToString());
            int id = MySqlHelper.Query(sql);
            string sql2 = String.Format("select id,gem1,gem2,type,magic3 from cq_item where id ={0};", id);
            DataTable dt = MySqlHelper.GetDataTable(sql2);
            int chksum = GetItemChkSum(Convert.ToInt32(dt.Rows[0]["gem1"]), Convert.ToInt32(dt.Rows[0]["gem2"]), id, Convert.ToInt32(dt.Rows[0]["type"]), Convert.ToInt32(dt.Rows[0]["magic3"]));
            string sql3 = String.Format("update cq_item set chksum={0} where id={1};", chksum, id);
            return MySqlHelper.Query(sql3);
        }

        //获取各等级同部位装备信息
        public static DataTable GetItemLevelList(int itemtype_id)
        {
            string id = itemtype_id.ToString().Substring(0, 3);
            string sql = String.Format("select id,name,req_level from cq_itemtype where req_profession> 0 and id like'{0}%' GROUP BY name order by req_level;", id);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return dt;
        }

        //获取女神开启状态
        public static bool CheckGoddessStatus(int userid, int goddesstype)
        {
            string sql = String.Format("select id from cq_goddess where userid='{0}' and goddesstype='{1}';", userid, goddesstype);
            object o = MySqlHelper.Find(sql);
            if (o != null) return true;
            return false;
        }

        //获取角色女神数据
        public static DataTable GetUserGoddessInfo(int userid)
        {
            string sql = String.Format("select * from cq_goddess where userid='{0}';", userid);
            return MySqlHelper.GetDataTable(sql);
        }

        //更新角色女神数据
        public static int UpdateUserGoddessInfo(int userid, int goddesstype, string level, string type1, string value1, string type2, string value2, string type3, string value3, string type4, string value4)
        {
            string sql = String.Format("insert into cq_goddess set userid='{0}',goddesstype='{1}';", userid, goddesstype);
            if (!CheckGoddessStatus(userid, goddesstype)) MySqlHelper.Query(sql);//更新前先判断数据库有没有数据。以免首次使用工具开启并保存时出错。

            string sql2 = string.Format("UPDATE `cq_goddess` SET `level`='{0}', `exp`='0', " +
                "`attribtype1`='{1}', `attribvalue1`='{2}', `attribtype2`='{3}', `attribvalue2`='{4}', `attribtype3`='{5}', `attribvalue3`='{6}', `attribtype4`='{7}', `attribvalue4`='{8}' " +
                "WHERE (`userid`='{9}' and `goddesstype`='{10}');", level, type1, value1, type2, value2, type3, value3, type4, value4, userid.ToString(), goddesstype);
            return MySqlHelper.Query(sql2);

        }

        //发送邮件
        public static bool SendMail(int player_id)
        {
            int hGameShareMapFile = OpenFileMappingA(983071, true, DataBaseName.ToUpper());
            if (hGameShareMapFile == 0) return false;
            int p = MapViewOfFile(hGameShareMapFile, 983071, 0, 0, 0) + 4;
            WriteMemoryInt32(GetCurrentProcess(), p, player_id);
            UnmapViewOfFile(p);
            CloseHandle(hGameShareMapFile);
            return true;
        }

        //踢人下线
        public static bool KickPlayer(int player_id)
        {
            int hGameShareMapFile = OpenFileMappingA(983071, true, DataBaseName.ToUpper());
            if (hGameShareMapFile == 0) return false;
            int p = MapViewOfFile(hGameShareMapFile, 983071, 0, 0, 0)+8;
            int a = ReadMemoryInt32(GetCurrentProcess(), p);
            int key = a ^ 2018;
            int v = key ^ player_id;
            WriteMemoryInt32(GetCurrentProcess(), p+4, v);
            UnmapViewOfFile(p);
            CloseHandle(hGameShareMapFile);
            return true;
        }

        //获取技能数据
        public static DataTable GetMagicType(string name = "")
        {
            string sql = String.Format("select id,type,name from cq_magictype where name like'%{0}%' group by type;", name);
            return MySqlHelper.GetDataTable(sql);
        }

        //获取角色技能数据
        public static DataTable GetUserMagic(int player_id)
        {
            string sql = String.Format("select a.*,b.name from cq_magic as a left join cq_magictype as b on a.type=b.type where a.ownerid='{0}' group by type;", player_id);
            return MySqlHelper.GetDataTable(sql);
        }


        //角色删除技能
        public static int UserDelMagic(int player_id, int type)
        {
            string sql = String.Format("delete from cq_magic where ownerid='{0}' and type='{1}';", player_id, type);
            return MySqlHelper.Query(sql);
        }

        //角色添加技能
        public static int UserLearnMagic(int player_id, int type)
        {
            string sql = String.Format("INSERT INTO cq_magic(`ownerid`, `type`) VALUES('{0}', '{1}');", player_id, type);
            return MySqlHelper.Query(sql);
        }

        //封硬件码
        public static bool BanHardcode(string hardcode)
        {
            try
            {
                string path = @"c:\禁止硬件码.txt";//文件存放路径，保证文件存在。
                StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(hardcode);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        //取物品位置
        public static string GetItemPosition(int p)
        {
            string position = "背包";
            switch (p)
            {
                case 1:
                    position = "头盔";
                    break;
                case 2:
                    position = "项链";
                    break;
                case 3:
                    position = "衣服";
                    break;
                case 4:
                    position = "武器";
                    break;
                case 7:
                    position = "手镯";
                    break;
                case 8:
                    position = "靴子";
                    break;
                case 9:
                    position = "婚戒";
                    break;
                case 12:
                    position = "外套";
                    break;
                case 13:
                    position = "加伤害法宝";
                    break;
                case 14:
                    position = "减伤害法宝";
                    break;
                case 15:
                    position = "战斗力法宝";
                    break;
                case 26:
                    position = "幻魂";
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
                case 52:
                    position = "幻兽蛋背包";
                    break;
                case 53:
                    position = "幻兽背包";
                    break;
                case 201:
                    position = "仓库";
                    break;
            }
            return position;
        }

        //去除字符串中的数字(可用于查询相似账号名)
        public static string RemoveNumber(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"\d", "");
        }

        //去除字符串中的非数字(可用于查询相似账号名)
        public static string RemoveNotNumber(string key)
        {
            return System.Text.RegularExpressions.Regex.Replace(key, @"[^\d]*", "");
        }

        //查询相似账号
        public static DataTable GetSimialarAccount(string account_name)
        {
            string sql = String.Format("select a.*,b.reg_pwd1 from account as a LEFT JOIN account_pwd as b on a.name=b.name where a.name='{0}';", account_name);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[0]["name"]);
            string password = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[0]["password"]);
            string ip = dt.Rows[0]["netbar_ip"].ToString();
            string superpass = dt.Rows[0]["superpass"].ToString();
            string hardcode = GetUserHardWare(name);
            string sql2 = String.Format("select a.*,b.reg_pwd1 from account as a LEFT JOIN account_pwd as b on a.name=b.name where `password`='{0}' or `netbar_ip`='{1}' or `superpass`='{2}' or `reg_pwd1`='{3}';", password, ip, superpass, hardcode);
            return MySqlHelper.GetDataTable(sql2);
        }

        //获取所有神火物品
        public static DataTable GetGodFireItemList()
        {
            string sql = "select * from cq_itemtype where id BETWEEN 1600000 and 1699999";
            return MySqlHelper.GetDataTable(sql);
        }

        //获取玩家神火物品
        public static DataTable GetUserGodfireItem(int player_id)
        {
            string sql = String.Format("select a.*,b.name,b.firepostion,b.firetype from cq_item as a left join cq_itemtype as b on a.type=b.id where a.player_id='{0}' and b.id between 1600000 and 1699999 order by a.position;", player_id);
            return MySqlHelper.GetDataTable(sql);
        }

        //以itemtype firetype属性取神火套装信息
        public static DataTable GetItemFireSuitGroup(int itemtype_id)
        {
            string sql = String.Format("select b.* from cq_itemtype as a left join cq_firesuitgroup as b on a.firetype=b.firetype where a.id='{0}';", itemtype_id);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return MySqlHelper.GetDataTable(sql);
        }
        //神火套装效果列表
        public static DataTable GodFireSuitList()
        {
            DataTable dt = new DataTable("firesuit");
            dt.Columns.Add("id", typeof(String));
            dt.Columns.Add("name", typeof(String));
            dt.Rows.Add(new String[] { "0", "无" });
            dt.Rows.Add(new String[] { "15", "八荒火海之怒" });
            dt.Rows.Add(new String[] { "16", "幽冥霜龙之魂" });
            dt.Rows.Add(new String[] { "17", "创世雷神之力" });
            dt.Rows.Add(new String[] { "651", "荒魂之哀嚎" });
            dt.Rows.Add(new String[] { "652", "业火之拥抱" });
            dt.Rows.Add(new String[] { "653", "死亡之凝视" });
            dt.Rows.Add(new String[] { "654", "启明之星光" });
            dt.Rows.Add(new String[] { "655", "混沌之黯影" });
            dt.Rows.Add(new String[] { "656", "不灭之圣辉" });
            return dt;
        }


        //取神火套装效果名称
        public static string GetFireSuitName(string firesuitgroupid)
        {
            string sql = String.Format("select firesuittype,starlev from cq_firesuitgroup where id='{0}';", firesuitgroupid);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            if (dt.Rows.Count < 1) return "无";

            DataTable dt2 = Moyu.GodFireSuitList();
            string sql2 = String.Format("id='{0}'", dt.Rows[0]["firesuittype"].ToString());
            DataRow[] dr = dt2.Select(sql2);
            if (dr.Length < 1) return "无";
            string name = dr[0]["name"].ToString() + " " + dt.Rows[0]["starlev"] + "星";
            return name;
        }
        //超凡效果列表
        public static DataTable GodexpList()
        {
            DataTable dt = new DataTable("god_exp");
            dt.Columns.Add("id", typeof(String));
            dt.Columns.Add("name", typeof(String));
            dt.Columns.Add("info", typeof(String));
            dt.Rows.Add(new String[] { "0", "无", "无" });
            dt.Rows.Add(new String[] { "2", "炎斩Ⅰ", "炎斩Ⅰ 战士的爆炎斩系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "3", "炎斩Ⅱ", "炎斩Ⅱ 战士的爆炎斩系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "4", "炎斩Ⅲ", "炎斩Ⅲ 战士的爆炎斩系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "5", "炎斩Ⅳ", "炎斩Ⅳ 战士的爆炎斩系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "6", "炎斩Ⅴ", "炎斩Ⅴ 战士的爆炎斩系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "8", "风珠Ⅰ", "风珠Ⅰ 法师的风珠系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "9", "风珠Ⅱ", "风珠Ⅱ 法师的风珠系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "10", "风珠Ⅲ", "风珠Ⅲ 法师的风珠系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "11", "风珠Ⅳ", "风珠Ⅳ 法师的风珠系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "12", "风珠Ⅴ", "风珠Ⅴ 法师的风珠系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "14", "神罚Ⅰ", "神罚Ⅰ 异能者的神罚系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "15", "神罚Ⅱ", "神罚Ⅱ 异能者的神罚系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "16", "神罚Ⅲ", "神罚Ⅲ 异能者的神罚系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "17", "神罚Ⅳ", "神罚Ⅳ 异能者的神罚系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "18", "神罚Ⅴ", "神罚Ⅴ 异能者的神罚系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "20", "血暴Ⅰ", "血暴Ⅰ 血族的血域风暴系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "21", "血暴Ⅱ", "血暴Ⅱ 血族的血域风暴系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "22", "血暴Ⅲ", "血暴Ⅲ 血族的血域风暴系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "23", "血暴Ⅳ", "血暴Ⅳ 血族的血域风暴系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "24", "血暴Ⅴ", "血暴Ⅴ 血族的血域风暴系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "26", "咒雨Ⅰ", "咒雨Ⅰ 亡灵巫师的降灵咒雨系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "27", "咒雨Ⅱ", "咒雨Ⅱ 亡灵巫师的降灵咒雨系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "28", "咒雨Ⅲ", "咒雨Ⅲ 亡灵巫师的降灵咒雨系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "29", "咒雨Ⅳ", "咒雨Ⅳ 亡灵巫师的降灵咒雨系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "30", "咒雨Ⅴ", "咒雨Ⅴ 亡灵巫师的降灵咒雨系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "32", "炽链Ⅰ", "炽链Ⅰ 暗黑龙骑的炽链陨灭系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "33", "炽链Ⅱ", "炽链Ⅱ 暗黑龙骑的炽链陨灭系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "34", "炽链Ⅲ", "炽链Ⅲ 暗黑龙骑的炽链陨灭系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "35", "炽链Ⅳ", "炽链Ⅳ 暗黑龙骑的炽链陨灭系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "36", "炽链Ⅴ", "炽链Ⅴ 暗黑龙骑的炽链陨灭系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "74", "旋刃Ⅰ", "旋刃Ⅰ 战士的旋风利刃系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "75", "旋刃Ⅱ", "旋刃Ⅱ 战士的旋风利刃系列技能伤害系数增加36%" });
            dt.Rows.Add(new String[] { "76", "旋刃Ⅲ", "旋刃Ⅲ 战士的旋风利刃系列技能伤害系数增加54%" });
            dt.Rows.Add(new String[] { "77", "旋刃Ⅳ", "旋刃Ⅳ 战士的旋风利刃系列技能伤害系数增加72%" });
            dt.Rows.Add(new String[] { "78", "旋刃Ⅴ", "旋刃Ⅴ 战士的旋风利刃系列技能伤害系数增加90%" });
            dt.Rows.Add(new String[] { "80", "飓风Ⅰ", "飓风Ⅰ 法师的飓风漩涡系列技能伤害系数增加4%" });
            dt.Rows.Add(new String[] { "81", "飓风Ⅱ", "飓风Ⅱ 法师的飓风漩涡系列技能伤害系数增加8%" });
            dt.Rows.Add(new String[] { "82", "飓风Ⅲ", "飓风Ⅲ 法师的飓风漩涡系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "83", "飓风Ⅳ", "飓风Ⅳ 法师的飓风漩涡系列技能伤害系数增加16%" });
            dt.Rows.Add(new String[] { "84", "飓风Ⅴ", "飓风Ⅴ 法师的飓风漩涡系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "86", "星陨Ⅰ", "星陨Ⅰ 异能者的星陨系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "87", "星陨Ⅱ", "星陨Ⅱ 异能者的星陨系列技能伤害系数增加30%" });
            dt.Rows.Add(new String[] { "88", "星陨Ⅲ", "星陨Ⅲ 异能者的星陨系列技能伤害系数增加45%" });
            dt.Rows.Add(new String[] { "89", "星陨Ⅳ", "星陨Ⅳ 异能者的星陨系列技能伤害系数增加60%" });
            dt.Rows.Add(new String[] { "90", "星陨Ⅴ", "星陨Ⅴ 异能者的星陨系列技能伤害系数增加75%" });
            dt.Rows.Add(new String[] { "92", "群蝠Ⅰ", "群蝠Ⅰ 血族的暗夜群蝠系列技能伤害系数增加22%" });
            dt.Rows.Add(new String[] { "93", "群蝠Ⅱ", "群蝠Ⅱ 血族的暗夜群蝠系列技能伤害系数增加44%" });
            dt.Rows.Add(new String[] { "94", "群蝠Ⅲ", "群蝠Ⅲ 血族的暗夜群蝠系列技能伤害系数增加66%" });
            dt.Rows.Add(new String[] { "95", "群蝠Ⅳ", "群蝠Ⅳ 血族的暗夜群蝠系列技能伤害系数增加88%" });
            dt.Rows.Add(new String[] { "96", "群蝠Ⅴ", "群蝠Ⅴ 血族的暗夜群蝠系列技能伤害系数增加110%" });
            dt.Rows.Add(new String[] { "98", "冥女Ⅰ", "冥女Ⅰ 亡灵巫师的冥国圣女系列技能额外继承2%人物攻击属性" });
            dt.Rows.Add(new String[] { "99", "冥女Ⅱ", "冥女Ⅱ 亡灵巫师的冥国圣女系列技能额外继承4%人物攻击属性" });
            dt.Rows.Add(new String[] { "100", "冥女Ⅲ", "冥女Ⅲ 亡灵巫师的冥国圣女系列技能额外继承6%人物攻击属性" });
            dt.Rows.Add(new String[] { "101", "冥女Ⅳ", "冥女Ⅳ 亡灵巫师的冥国圣女系列技能额外继承8%人物攻击属性" });
            dt.Rows.Add(new String[] { "102", "冥女Ⅴ", "冥女Ⅴ 亡灵巫师的冥国圣女系列技能额外继承10%人物攻击属性" });
            dt.Rows.Add(new String[] { "104", "焚世Ⅰ", "焚世Ⅰ 暗黑龙骑的魔龙焚世系列技能伤害系数增加5%" });
            dt.Rows.Add(new String[] { "105", "焚世Ⅱ", "焚世Ⅱ 暗黑龙骑的魔龙焚世系列技能伤害系数增加10%" });
            dt.Rows.Add(new String[] { "106", "焚世Ⅲ", "焚世Ⅲ 暗黑龙骑的魔龙焚世系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "107", "焚世Ⅳ", "焚世Ⅳ 暗黑龙骑的魔龙焚世系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "108", "焚世Ⅴ", "焚世Ⅴ 暗黑龙骑的魔龙焚世系列技能伤害系数增加25%" });
            dt.Rows.Add(new String[] { "146", "飞斩Ⅰ", "飞斩Ⅰ 战士的飞天连斩系列技能伤害系数增加7%" });
            dt.Rows.Add(new String[] { "147", "飞斩Ⅱ", "飞斩Ⅱ 战士的飞天连斩系列技能伤害系数增加14%" });
            dt.Rows.Add(new String[] { "148", "飞斩Ⅲ", "飞斩Ⅲ 战士的飞天连斩系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "149", "飞斩Ⅳ", "飞斩Ⅳ 战士的飞天连斩系列技能伤害系数增加28%" });
            dt.Rows.Add(new String[] { "150", "飞斩Ⅴ", "飞斩Ⅴ 战士的飞天连斩系列技能伤害系数增加35%" });
            dt.Rows.Add(new String[] { "152", "爆雷Ⅰ", "爆雷Ⅰ 法师的爆雷术系列技能伤害系数增加4%" });
            dt.Rows.Add(new String[] { "153", "爆雷Ⅱ", "爆雷Ⅱ 法师的爆雷术系列技能伤害系数增加8%" });
            dt.Rows.Add(new String[] { "154", "爆雷Ⅲ", "爆雷Ⅲ 法师的爆雷术系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "155", "爆雷Ⅳ", "爆雷Ⅳ 法师的爆雷术系列技能伤害系数增加16%" });
            dt.Rows.Add(new String[] { "156", "爆雷Ⅴ", "爆雷Ⅴ 法师的爆雷术系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "158", "裁决Ⅰ", "裁决Ⅰ 异能者的死神裁决系列技能伤害系数增加11%" });
            dt.Rows.Add(new String[] { "159", "裁决Ⅱ", "裁决Ⅱ 异能者的死神裁决系列技能伤害系数增加22%" });
            dt.Rows.Add(new String[] { "160", "裁决Ⅲ", "裁决Ⅲ 异能者的死神裁决系列技能伤害系数增加33%" });
            dt.Rows.Add(new String[] { "161", "裁决Ⅳ", "裁决Ⅳ 异能者的死神裁决系列技能伤害系数增加44%" });
            dt.Rows.Add(new String[] { "162", "裁决Ⅴ", "裁决Ⅴ 异能者的死神裁决系列技能伤害系数增加55%" });
            dt.Rows.Add(new String[] { "164", "血影Ⅰ", "血影Ⅰ 血族的血影星芒系列技能伤害系数增加5%" });
            dt.Rows.Add(new String[] { "165", "血影Ⅱ", "血影Ⅱ 血族的血影星芒系列技能伤害系数增加10%" });
            dt.Rows.Add(new String[] { "166", "血影Ⅲ", "血影Ⅲ 血族的血影星芒系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "167", "血影Ⅳ", "血影Ⅳ 血族的血影星芒系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "168", "血影Ⅴ", "血影Ⅴ 血族的血影星芒系列技能伤害系数增加25%" });
            dt.Rows.Add(new String[] { "170", "裂魂Ⅰ", "裂魂Ⅰ 亡灵巫师的裂魂闪系列技能伤害系数增加8%" });
            dt.Rows.Add(new String[] { "171", "裂魂Ⅱ", "裂魂Ⅱ 亡灵巫师的裂魂闪系列技能伤害系数增加16%" });
            dt.Rows.Add(new String[] { "172", "裂魂Ⅲ", "裂魂Ⅲ 亡灵巫师的裂魂闪系列技能伤害系数增加24%" });
            dt.Rows.Add(new String[] { "173", "裂魂Ⅳ", "裂魂Ⅳ 亡灵巫师的裂魂闪系列技能伤害系数增加32%" });
            dt.Rows.Add(new String[] { "174", "裂魂Ⅴ", "裂魂Ⅴ 亡灵巫师的裂魂闪系列技能伤害系数增加40%" });
            dt.Rows.Add(new String[] { "176", "龙枪Ⅰ", "龙枪Ⅰ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加5%" });
            dt.Rows.Add(new String[] { "177", "龙枪Ⅱ", "龙枪Ⅱ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加10%" });
            dt.Rows.Add(new String[] { "178", "龙枪Ⅲ", "龙枪Ⅲ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "179", "龙枪Ⅳ", "龙枪Ⅳ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "180", "龙枪Ⅴ", "龙枪Ⅴ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加25%" });
            dt.Rows.Add(new String[] { "218", "狂跃Ⅰ", "狂跃Ⅰ 战士的狂怒之跃系列技能伤害系数增加10%" });
            dt.Rows.Add(new String[] { "219", "狂跃Ⅱ", "狂跃Ⅱ 战士的狂怒之跃系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "220", "狂跃Ⅲ", "狂跃Ⅲ 战士的狂怒之跃系列技能伤害系数增加30%" });
            dt.Rows.Add(new String[] { "221", "狂跃Ⅳ", "狂跃Ⅳ 战士的狂怒之跃系列技能伤害系数增加40%" });
            dt.Rows.Add(new String[] { "222", "狂跃Ⅴ", "狂跃Ⅴ 战士的狂怒之跃系列技能伤害系数增加50%" });
            dt.Rows.Add(new String[] { "224", "陨火Ⅰ", "陨火Ⅰ 法师的流星陨火系列技能伤害系数增加5%" });
            dt.Rows.Add(new String[] { "225", "陨火Ⅱ", "陨火Ⅱ 法师的流星陨火系列技能伤害系数增加10%" });
            dt.Rows.Add(new String[] { "226", "陨火Ⅲ", "陨火Ⅲ 法师的流星陨火系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "227", "陨火Ⅳ", "陨火Ⅳ 法师的流星陨火系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "228", "陨火Ⅴ", "陨火Ⅴ 法师的流星陨火系列技能伤害系数增加25%" });
            dt.Rows.Add(new String[] { "230", "影魂Ⅰ", "影魂Ⅰ 异能者的影魂召唤系列技能召唤物额外继承1%人物攻击属性" });
            dt.Rows.Add(new String[] { "231", "影魂Ⅱ", "影魂Ⅱ 异能者的影魂召唤系列技能召唤物额外继承2%人物攻击属性" });
            dt.Rows.Add(new String[] { "232", "影魂Ⅲ", "影魂Ⅲ 异能者的影魂召唤系列技能召唤物额外继承3%人物攻击属性" });
            dt.Rows.Add(new String[] { "233", "影魂Ⅳ", "影魂Ⅳ 异能者的影魂召唤系列技能召唤物额外继承4%人物攻击属性" });
            dt.Rows.Add(new String[] { "234", "影魂Ⅴ", "影魂Ⅴ 异能者的影魂召唤系列技能召唤物额外继承5%人物攻击属性" });
            dt.Rows.Add(new String[] { "236", "真视Ⅰ", "真视Ⅰ 血族的真视打击系列技能概率提升1%" });
            dt.Rows.Add(new String[] { "237", "真视Ⅱ", "真视Ⅱ 血族的真视打击系列技能概率提升2%" });
            dt.Rows.Add(new String[] { "238", "真视Ⅲ", "真视Ⅲ 血族的真视打击系列技能概率提升3%" });
            dt.Rows.Add(new String[] { "239", "真视Ⅳ", "真视Ⅳ 血族的真视打击系列技能概率提升4%" });
            dt.Rows.Add(new String[] { "240", "真视Ⅴ", "真视Ⅴ 血族的真视打击系列技能概率提升5%" });
            dt.Rows.Add(new String[] { "242", "渊灵Ⅰ", "渊灵Ⅰ 亡灵巫师的深渊恶灵系列技能召唤物额外继承6%人物攻击属性" });
            dt.Rows.Add(new String[] { "243", "渊灵Ⅱ", "渊灵Ⅱ 亡灵巫师的深渊恶灵系列技能召唤物额外继承12%人物攻击属性" });
            dt.Rows.Add(new String[] { "244", "渊灵Ⅲ", "渊灵Ⅲ 亡灵巫师的深渊恶灵系列技能召唤物额外继承18%人物攻击属性" });
            dt.Rows.Add(new String[] { "245", "渊灵Ⅳ", "渊灵Ⅳ 亡灵巫师的深渊恶灵系列技能召唤物额外继承24%人物攻击属性" });
            dt.Rows.Add(new String[] { "246", "渊灵Ⅴ", "渊灵Ⅴ 亡灵巫师的深渊恶灵系列技能召唤物额外继承30%人物攻击属性" });
            dt.Rows.Add(new String[] { "248", "护卫团Ⅰ", "护卫团Ⅰ 暗黑龙骑的龙裔守护系列技能召唤物额外继承2%人物攻击属性" });
            dt.Rows.Add(new String[] { "249", "护卫团Ⅱ", "护卫团Ⅱ 暗黑龙骑的龙裔守护系列技能召唤物额外继承4%人物攻击属性" });
            dt.Rows.Add(new String[] { "250", "护卫团Ⅲ", "护卫团Ⅲ 暗黑龙骑的龙裔守护系列技能召唤物额外继承6%人物攻击属性" });
            dt.Rows.Add(new String[] { "251", "护卫团Ⅳ", "护卫团Ⅳ 暗黑龙骑的龙裔守护系列技能召唤物额外继承8%人物攻击属性" });
            dt.Rows.Add(new String[] { "252", "护卫团Ⅴ", "护卫团Ⅴ 暗黑龙骑的龙裔守护系列技能召唤物额外继承10%人物攻击属性" });
            dt.Rows.Add(new String[] { "254", "无畏Ⅰ", "无畏Ⅰ 战士的英勇无畏系列技能时间延长1秒" });
            dt.Rows.Add(new String[] { "255", "无畏Ⅱ", "无畏Ⅱ 战士的英勇无畏系列技能时间延长2秒" });
            dt.Rows.Add(new String[] { "256", "无畏Ⅲ", "无畏Ⅲ 战士的英勇无畏系列技能时间延长3秒" });
            dt.Rows.Add(new String[] { "257", "无畏Ⅳ", "无畏Ⅳ 战士的英勇无畏系列技能时间延长4秒" });
            dt.Rows.Add(new String[] { "258", "无畏Ⅴ", "无畏Ⅴ 战士的英勇无畏系列技能时间延长5秒" });
            dt.Rows.Add(new String[] { "260", "秘法Ⅰ", "秘法Ⅰ 法师的秘法觉醒系列技能时间延长1秒" });
            dt.Rows.Add(new String[] { "261", "秘法Ⅱ", "秘法Ⅱ 法师的秘法觉醒系列技能时间延长2秒" });
            dt.Rows.Add(new String[] { "262", "秘法Ⅲ", "秘法Ⅲ 法师的秘法觉醒系列技能时间延长3秒" });
            dt.Rows.Add(new String[] { "263", "秘法Ⅳ", "秘法Ⅳ 法师的秘法觉醒系列技能时间延长4秒" });
            dt.Rows.Add(new String[] { "264", "秘法Ⅴ", "秘法Ⅴ 法师的秘法觉醒系列技能时间延长5秒" });
            dt.Rows.Add(new String[] { "266", "虚无Ⅰ", "虚无Ⅰ 异能者的虚无空间系列技能冷却缩短1秒" });
            dt.Rows.Add(new String[] { "267", "虚无Ⅱ", "虚无Ⅱ 异能者的虚无空间系列技能冷却缩短2秒" });
            dt.Rows.Add(new String[] { "268", "虚无Ⅲ", "虚无Ⅲ 异能者的虚无空间系列技能冷却缩短3秒" });
            dt.Rows.Add(new String[] { "269", "虚无Ⅳ", "虚无Ⅳ 异能者的虚无空间系列技能冷却缩短4秒" });
            dt.Rows.Add(new String[] { "270", "虚无Ⅴ", "虚无Ⅴ 异能者的虚无空间系列技能冷却缩短5秒" });
            dt.Rows.Add(new String[] { "272", "幻影Ⅰ", "幻影Ⅰ 血族的幻影神术系列技能时间延长1秒" });
            dt.Rows.Add(new String[] { "273", "幻影Ⅱ", "幻影Ⅱ 血族的幻影神术系列技能时间延长2秒" });
            dt.Rows.Add(new String[] { "274", "幻影Ⅲ", "幻影Ⅲ 血族的幻影神术系列技能时间延长3秒" });
            dt.Rows.Add(new String[] { "275", "幻影Ⅳ", "幻影Ⅳ 血族的幻影神术系列技能时间延长4秒" });
            dt.Rows.Add(new String[] { "276", "幻影Ⅴ", "幻影Ⅴ 血族的幻影神术系列技能时间延长5秒" });
            dt.Rows.Add(new String[] { "278", "死契Ⅰ", "死契Ⅰ 亡灵巫师的冥河死契系列技能时间延长1秒" });
            dt.Rows.Add(new String[] { "279", "死契Ⅱ", "死契Ⅱ 亡灵巫师的冥河死契系列技能时间延长2秒" });
            dt.Rows.Add(new String[] { "280", "死契Ⅲ", "死契Ⅲ 亡灵巫师的冥河死契系列技能时间延长3秒" });
            dt.Rows.Add(new String[] { "281", "死契Ⅳ", "死契Ⅳ 亡灵巫师的冥河死契系列技能时间延长4秒" });
            dt.Rows.Add(new String[] { "282", "死契Ⅴ", "死契Ⅴ 亡灵巫师的冥河死契系列技能时间延长5秒" });
            dt.Rows.Add(new String[] { "284", "战吼Ⅰ", "战吼Ⅰ 暗黑龙骑的狂热战吼的战意效果时间增加0.5秒" });
            dt.Rows.Add(new String[] { "285", "战吼Ⅱ", "战吼Ⅱ 暗黑龙骑的狂热战吼的战意效果时间增加1秒" });
            dt.Rows.Add(new String[] { "286", "战吼Ⅲ", "战吼Ⅲ 暗黑龙骑的狂热战吼的战意效果时间增加1.5秒" });
            dt.Rows.Add(new String[] { "287", "战吼Ⅳ", "战吼Ⅳ 暗黑龙骑的狂热战吼的战意效果时间增加2秒" });
            dt.Rows.Add(new String[] { "288", "战吼Ⅴ", "战吼Ⅴ 暗黑龙骑的狂热战吼的战意效果时间增加2.5秒" });
            dt.Rows.Add(new String[] { "1585", "炎斩Ⅵ", "炎斩Ⅵ 战士的爆炎斩系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "1586", "炎斩Ⅶ", "炎斩Ⅶ 战士的爆炎斩系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "1587", "风珠Ⅵ", "风珠Ⅵ 法师的风珠系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "1588", "风珠Ⅶ", "风珠Ⅶ 法师的风珠系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "1589", "神罚Ⅵ", "神罚Ⅵ 异能者的神罚系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "1590", "神罚Ⅶ", "神罚Ⅶ 异能者的神罚系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "1591", "血暴Ⅵ", "血暴Ⅵ 血族的血域风暴系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "1592", "血暴Ⅶ", "血暴Ⅶ 血族的血域风暴系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "1593", "咒雨Ⅵ", "咒雨Ⅵ 亡灵巫师的降灵咒雨系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "1594", "咒雨Ⅶ", "咒雨Ⅶ 亡灵巫师的降灵咒雨系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "1595", "炽链Ⅵ", "炽链Ⅵ 暗黑龙骑的炽链陨灭系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "1596", "炽链Ⅶ", "炽链Ⅶ 暗黑龙骑的炽链陨灭系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "1609", "旋刃Ⅵ", "旋刃Ⅵ 战士的旋风利刃系列技能伤害系数增加108%" });
            dt.Rows.Add(new String[] { "1610", "旋刃Ⅶ", "旋刃Ⅶ 战士的旋风利刃系列技能伤害系数增加126%" });
            dt.Rows.Add(new String[] { "1611", "飓风Ⅵ", "飓风Ⅵ 法师的飓风漩涡系列技能伤害系数增加24%" });
            dt.Rows.Add(new String[] { "1612", "飓风Ⅶ", "飓风Ⅶ 法师的飓风漩涡系列技能伤害系数增加28%" });
            dt.Rows.Add(new String[] { "1613", "星陨Ⅵ", "星陨Ⅵ 异能者的星陨系列技能伤害系数增加90%" });
            dt.Rows.Add(new String[] { "1614", "星陨Ⅶ", "星陨Ⅶ 异能者的星陨系列技能伤害系数增加105%" });
            dt.Rows.Add(new String[] { "1615", "群蝠Ⅵ", "群蝠Ⅵ 血族的暗夜群蝠系列技能伤害系数增加132%" });
            dt.Rows.Add(new String[] { "1616", "群蝠Ⅶ", "群蝠Ⅶ 血族的暗夜群蝠系列技能伤害系数增加154%" });
            dt.Rows.Add(new String[] { "1617", "冥女Ⅵ", "冥女Ⅵ 亡灵巫师的冥国圣女系列技能额外继承12%人物攻击属性" });
            dt.Rows.Add(new String[] { "1618", "冥女Ⅶ", "冥女Ⅶ 亡灵巫师的冥国圣女系列技能额外继承14%人物攻击属性" });
            dt.Rows.Add(new String[] { "1619", "焚世Ⅵ", "焚世Ⅵ 暗黑龙骑的魔龙焚世系列技能伤害系数增加30%" });
            dt.Rows.Add(new String[] { "1620", "焚世Ⅶ", "焚世Ⅶ 暗黑龙骑的魔龙焚世系列技能伤害系数增加35%" });
            dt.Rows.Add(new String[] { "1633", "飞斩Ⅵ", "飞斩Ⅵ 战士的飞天连斩系列技能伤害系数增加42%" });
            dt.Rows.Add(new String[] { "1634", "飞斩Ⅶ", "飞斩Ⅶ 战士的飞天连斩系列技能伤害系数增加49%" });
            dt.Rows.Add(new String[] { "1635", "爆雷Ⅵ", "爆雷Ⅵ 法师的爆雷术系列技能伤害系数增加24%" });
            dt.Rows.Add(new String[] { "1636", "爆雷Ⅶ", "爆雷Ⅶ 法师的爆雷术系列技能伤害系数增加28%" });
            dt.Rows.Add(new String[] { "1637", "裁决Ⅵ", "裁决Ⅵ 异能者的死神裁决系列技能伤害系数增加66%" });
            dt.Rows.Add(new String[] { "1638", "裁决Ⅶ", "裁决Ⅶ 异能者的死神裁决系列技能伤害系数增加77%" });
            dt.Rows.Add(new String[] { "1639", "血影Ⅵ", "血影Ⅵ 血族的血影星芒系列技能伤害系数增加30%" });
            dt.Rows.Add(new String[] { "1640", "血影Ⅶ", "血影Ⅶ 血族的血影星芒系列技能伤害系数增加35%" });
            dt.Rows.Add(new String[] { "1641", "裂魂Ⅵ", "裂魂Ⅵ 亡灵巫师的裂魂闪系列技能伤害系数增加48%" });
            dt.Rows.Add(new String[] { "1642", "裂魂Ⅶ", "裂魂Ⅶ 亡灵巫师的裂魂闪系列技能伤害系数增加56%" });
            dt.Rows.Add(new String[] { "1643", "龙枪Ⅵ", "龙枪Ⅵ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加30%" });
            dt.Rows.Add(new String[] { "1644", "龙枪Ⅶ", "龙枪Ⅶ 暗黑龙骑的龙枪碎魂系列技能伤害系数增加35%" });
            dt.Rows.Add(new String[] { "1657", "狂跃Ⅵ", "狂跃Ⅵ 战士的狂怒之跃系列技能伤害系数增加60%" });
            dt.Rows.Add(new String[] { "1658", "狂跃Ⅶ", "狂跃Ⅶ 战士的狂怒之跃系列技能伤害系数增加70%" });
            dt.Rows.Add(new String[] { "1659", "陨火Ⅵ", "陨火Ⅵ 法师的流星陨火系列技能伤害系数增加30%" });
            dt.Rows.Add(new String[] { "1660", "陨火Ⅶ", "陨火Ⅶ 法师的流星陨火系列技能伤害系数增加35%" });
            dt.Rows.Add(new String[] { "1661", "影魂Ⅵ", "影魂Ⅵ 异能者的影魂召唤系列技能召唤物属性提升6%" });
            dt.Rows.Add(new String[] { "1662", "影魂Ⅶ", "影魂Ⅶ 异能者的影魂召唤系列技能召唤物属性提升7%" });
            dt.Rows.Add(new String[] { "1663", "真视Ⅵ", "真视Ⅵ 血族的真视打击系列技能概率提升6%" });
            dt.Rows.Add(new String[] { "1664", "真视Ⅶ", "真视Ⅶ 血族的真视打击系列技能概率提升7%" });
            dt.Rows.Add(new String[] { "1665", "渊灵Ⅵ", "渊灵Ⅵ 亡灵巫师的深渊恶灵系列技能召唤物额外继承72%人物攻击属性" });
            dt.Rows.Add(new String[] { "1666", "渊灵Ⅶ", "渊灵Ⅶ 亡灵巫师的深渊恶灵系列技能召唤物额外继承84%人物攻击属性" });
            dt.Rows.Add(new String[] { "1667", "护卫团Ⅵ", "护卫团Ⅵ 暗黑龙骑的龙裔守护系列技能召唤物额外继承12%人物攻击属性" });
            dt.Rows.Add(new String[] { "1668", "护卫团Ⅶ", "护卫团Ⅶ 暗黑龙骑的龙裔守护系列技能召唤物额外继承14%人物攻击属性" });
            dt.Rows.Add(new String[] { "1669", "无畏Ⅵ", "无畏Ⅵ 战士的英勇无畏系列技能时间延长6秒" });
            dt.Rows.Add(new String[] { "1670", "无畏Ⅶ", "无畏Ⅶ 战士的英勇无畏系列技能时间延长7秒" });
            dt.Rows.Add(new String[] { "1671", "秘法Ⅵ", "秘法Ⅵ 法师的秘法觉醒系列技能时间延长6秒" });
            dt.Rows.Add(new String[] { "1672", "秘法Ⅶ", "秘法Ⅶ 法师的秘法觉醒系列技能时间延长7秒" });
            dt.Rows.Add(new String[] { "1673", "虚无Ⅵ", "虚无Ⅵ 异能者的虚无空间系列技能冷却缩短6秒" });
            dt.Rows.Add(new String[] { "1674", "虚无Ⅶ", "虚无Ⅶ 异能者的虚无空间系列技能冷却缩短7秒" });
            dt.Rows.Add(new String[] { "1675", "幻影Ⅵ", "幻影Ⅵ 血族的幻影神术系列技能时间延长6秒" });
            dt.Rows.Add(new String[] { "1676", "幻影Ⅶ", "幻影Ⅶ 血族的幻影神术系列技能时间延长7秒" });
            dt.Rows.Add(new String[] { "1677", "死契Ⅵ", "死契Ⅵ 亡灵巫师的冥河死契系列技能时间延长6秒" });
            dt.Rows.Add(new String[] { "1678", "死契Ⅶ", "死契Ⅶ 亡灵巫师的冥河死契系列技能时间延长7秒" });
            dt.Rows.Add(new String[] { "1679", "战吼Ⅵ", "战吼Ⅵ 暗黑龙骑的狂热战吼的战意效果时间增加4秒" });
            dt.Rows.Add(new String[] { "1680", "战吼Ⅶ", "战吼Ⅶ 暗黑龙骑的狂热战吼的战意效果时间增加6秒" });
            dt.Rows.Add(new String[] { "1682", "追击Ⅰ", "追击Ⅰ 精灵游侠的迅捷追击充能时间缩短1秒" });
            dt.Rows.Add(new String[] { "1683", "追击Ⅱ", "追击Ⅱ 精灵游侠的迅捷追击充能时间缩短2秒" });
            dt.Rows.Add(new String[] { "1684", "追击Ⅲ", "追击Ⅲ 精灵游侠的迅捷追击充能时间缩短3秒" });
            dt.Rows.Add(new String[] { "1685", "追击Ⅳ", "追击Ⅳ 精灵游侠的迅捷追击充能时间缩短4秒" });
            dt.Rows.Add(new String[] { "1686", "追击Ⅴ", "追击Ⅴ 精灵游侠的迅捷追击充能时间缩短5秒" });
            dt.Rows.Add(new String[] { "1694", "制裁Ⅰ", "制裁Ⅰ 精灵游侠的裁决之箭伤害系数增加10%" });
            dt.Rows.Add(new String[] { "1695", "制裁Ⅱ", "制裁Ⅱ 精灵游侠的裁决之箭伤害系数增加20%" });
            dt.Rows.Add(new String[] { "1696", "制裁Ⅲ", "制裁Ⅲ 精灵游侠的裁决之箭伤害系数增加30%" });
            dt.Rows.Add(new String[] { "1697", "制裁Ⅳ", "制裁Ⅳ 精灵游侠的裁决之箭伤害系数增加40%" });
            dt.Rows.Add(new String[] { "1698", "制裁Ⅴ", "制裁Ⅴ 精灵游侠的裁决之箭伤害系数增加50%" });
            dt.Rows.Add(new String[] { "1706", "聚能Ⅰ", "聚能Ⅰ 精灵游侠的聚能射击满力射击系数增加32%" });
            dt.Rows.Add(new String[] { "1707", "聚能Ⅱ", "聚能Ⅱ 精灵游侠的聚能射击满力射击系数增加64%" });
            dt.Rows.Add(new String[] { "1708", "聚能Ⅲ", "聚能Ⅲ 精灵游侠的聚能射击满力射击系数增加96%" });
            dt.Rows.Add(new String[] { "1709", "聚能Ⅳ", "聚能Ⅳ 精灵游侠的聚能射击满力射击系数增加128%" });
            dt.Rows.Add(new String[] { "1710", "聚能Ⅴ", "聚能Ⅴ 精灵游侠的聚能射击满力射击系数增加160%" });
            dt.Rows.Add(new String[] { "1718", "箭雨Ⅰ", "箭雨Ⅰ 精灵游侠的群星箭雨伤害系数增加12%" });
            dt.Rows.Add(new String[] { "1719", "箭雨Ⅱ", "箭雨Ⅱ 精灵游侠的群星箭雨伤害系数增加24%" });
            dt.Rows.Add(new String[] { "1720", "箭雨Ⅲ", "箭雨Ⅲ 精灵游侠的群星箭雨伤害系数增加36%" });
            dt.Rows.Add(new String[] { "1721", "箭雨Ⅳ", "箭雨Ⅳ 精灵游侠的群星箭雨伤害系数增加48%" });
            dt.Rows.Add(new String[] { "1722", "箭雨Ⅴ", "箭雨Ⅴ 精灵游侠的群星箭雨伤害系数增加60%" });
            dt.Rows.Add(new String[] { "1724", "御风Ⅰ", "御风Ⅰ 精灵游侠的御风者冷却时间减少1秒" });
            dt.Rows.Add(new String[] { "1725", "御风Ⅱ", "御风Ⅱ 精灵游侠的御风者冷却时间减少2秒" });
            dt.Rows.Add(new String[] { "1726", "御风Ⅲ", "御风Ⅲ 精灵游侠的御风者冷却时间减少3秒" });
            dt.Rows.Add(new String[] { "1727", "御风Ⅳ", "御风Ⅳ 精灵游侠的御风者冷却时间减少4秒" });
            dt.Rows.Add(new String[] { "1728", "御风Ⅴ", "御风Ⅴ 精灵游侠的御风者冷却时间减少5秒" });
            dt.Rows.Add(new String[] { "1945", "追击Ⅵ", "追击Ⅵ 精灵游侠的迅捷追击充能时间缩短6秒" });
            dt.Rows.Add(new String[] { "1946", "追击Ⅶ", "追击Ⅶ 精灵游侠的迅捷追击充能时间缩短7秒" });
            dt.Rows.Add(new String[] { "1949", "制裁Ⅵ", "制裁Ⅵ 精灵游侠的裁决之箭伤害系数增加60%" });
            dt.Rows.Add(new String[] { "1950", "制裁Ⅶ", "制裁Ⅶ 精灵游侠的裁决之箭伤害系数增加70%" });
            dt.Rows.Add(new String[] { "1953", "聚能Ⅵ", "聚能Ⅵ 精灵游侠的聚能射击满力射击系数增加192%" });
            dt.Rows.Add(new String[] { "1954", "聚能Ⅶ", "聚能Ⅶ 精灵游侠的聚能射击满力射击系数增加224%" });
            dt.Rows.Add(new String[] { "1957", "箭雨Ⅵ", "箭雨Ⅵ 精灵游侠的群星箭雨伤害系数增加72%" });
            dt.Rows.Add(new String[] { "1958", "箭雨Ⅶ", "箭雨Ⅶ 精灵游侠的群星箭雨伤害系数增加84%" });
            dt.Rows.Add(new String[] { "1959", "御风Ⅵ", "御风Ⅵ 精灵游侠的御风者冷却时间减少6秒" });
            dt.Rows.Add(new String[] { "1960", "御风Ⅶ", "御风Ⅶ 精灵游侠的御风者冷却时间减少7秒" });
            dt.Rows.Add(new String[] { "2977", "乱刃Ⅰ", "乱刃Ⅰ 御剑师的剑御九天系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "2978", "乱刃Ⅱ", "乱刃Ⅱ 御剑师的剑御九天系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "2983", "剑虹Ⅰ", "剑虹Ⅰ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加10%" });
            dt.Rows.Add(new String[] { "2984", "剑虹Ⅱ", "剑虹Ⅱ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加20%" });
            dt.Rows.Add(new String[] { "2989", "剑舞Ⅰ", "剑舞Ⅰ 御剑师的无双剑舞系列技能连击伤害系数增加12%" });
            dt.Rows.Add(new String[] { "2990", "剑舞Ⅱ", "剑舞Ⅱ 御剑师的无双剑舞系列技能连击伤害系数增加24%" });
            dt.Rows.Add(new String[] { "2995", "掠影Ⅰ", "掠影Ⅰ 御剑师的疾风掠影系列技能充能时间减少1秒" });
            dt.Rows.Add(new String[] { "2996", "掠影Ⅱ", "掠影Ⅱ 御剑师的疾风掠影系列技能充能时间减少2秒" });
            dt.Rows.Add(new String[] { "2998", "剑意Ⅰ", "剑意Ⅰ 御剑师的独孤求败系列技能冷却时间减少1秒" });
            dt.Rows.Add(new String[] { "2999", "剑意Ⅱ", "剑意Ⅱ 御剑师的独孤求败系列技能冷却时间减少2秒" });
            dt.Rows.Add(new String[] { "3362", "乱刃Ⅲ", "乱刃Ⅲ 御剑师的剑御九天系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "3363", "乱刃Ⅳ", "乱刃Ⅳ 御剑师的剑御九天系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "3364", "乱刃Ⅴ", "乱刃Ⅴ 御剑师的剑御九天系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "3365", "乱刃Ⅵ", "乱刃Ⅵ 御剑师的剑御九天系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "3366", "乱刃Ⅶ", "乱刃Ⅶ 御剑师的剑御九天系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "3376", "剑虹Ⅲ", "剑虹Ⅲ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加30%" });
            dt.Rows.Add(new String[] { "3377", "剑虹Ⅳ", "剑虹Ⅳ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加40%" });
            dt.Rows.Add(new String[] { "3378", "剑虹Ⅴ", "剑虹Ⅴ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加50%" });
            dt.Rows.Add(new String[] { "3379", "剑虹Ⅵ", "剑虹Ⅵ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加60%" });
            dt.Rows.Add(new String[] { "3380", "剑虹Ⅶ", "剑虹Ⅶ 御剑师的白虹贯日系列技能中每把飞剑伤害系数增加70%" });
            dt.Rows.Add(new String[] { "3390", "剑舞Ⅲ", "剑舞Ⅲ 御剑师的无双剑舞系列技能连击伤害系数增加36%" });
            dt.Rows.Add(new String[] { "3391", "剑舞Ⅳ", "剑舞Ⅳ 御剑师的无双剑舞系列技能连击伤害系数增加48%" });
            dt.Rows.Add(new String[] { "3392", "剑舞Ⅴ", "剑舞Ⅴ 御剑师的无双剑舞系列技能连击伤害系数增加60%" });
            dt.Rows.Add(new String[] { "3393", "剑舞Ⅵ", "剑舞Ⅵ 御剑师的无双剑舞系列技能连击伤害系数增加72%" });
            dt.Rows.Add(new String[] { "3394", "剑舞Ⅶ", "剑舞Ⅶ 御剑师的无双剑舞系列技能连击伤害系数增加84%" });
            dt.Rows.Add(new String[] { "3404", "掠影Ⅲ", "掠影Ⅲ 御剑师的疾风掠影系列技能充能时间减少3秒" });
            dt.Rows.Add(new String[] { "3405", "掠影Ⅳ", "掠影Ⅳ 御剑师的疾风掠影系列技能充能时间减少4秒" });
            dt.Rows.Add(new String[] { "3406", "掠影Ⅴ", "掠影Ⅴ 御剑师的疾风掠影系列技能充能时间减少5秒" });
            dt.Rows.Add(new String[] { "3407", "掠影Ⅵ", "掠影Ⅵ 御剑师的疾风掠影系列技能充能时间减少6秒" });
            dt.Rows.Add(new String[] { "3408", "掠影Ⅶ", "掠影Ⅶ 御剑师的疾风掠影系列技能充能时间减少7秒" });
            dt.Rows.Add(new String[] { "3411", "剑意Ⅲ", "剑意Ⅲ 御剑师的独孤求败系列技能冷却时间减少3秒" });
            dt.Rows.Add(new String[] { "3412", "剑意Ⅳ", "剑意Ⅳ 御剑师的独孤求败系列技能冷却时间减少4秒" });
            dt.Rows.Add(new String[] { "3413", "剑意Ⅴ", "剑意Ⅴ 御剑师的独孤求败系列技能冷却时间减少5秒" });
            dt.Rows.Add(new String[] { "3414", "剑意Ⅵ", "剑意Ⅵ 御剑师的独孤求败系列技能冷却时间减少6秒" });
            dt.Rows.Add(new String[] { "3415", "剑意Ⅶ", "剑意Ⅶ 御剑师的独孤求败系列技能冷却时间减少7秒" });
            dt.Rows.Add(new String[] { "4425", "裂变Ⅰ", "裂变Ⅰ 星辰神子的星能裂变系列技能伤害系数增加3%" });
            dt.Rows.Add(new String[] { "4426", "裂变Ⅱ", "裂变Ⅱ 星辰神子的星能裂变系列技能伤害系数增加6%" });
            dt.Rows.Add(new String[] { "4431", "御星诀Ⅰ", "御星诀Ⅰ 星辰神子的御星诀系列技能连招伤害系数增加5%" });
            dt.Rows.Add(new String[] { "4432", "御星诀Ⅱ", "御星诀Ⅱ 星辰神子的御星诀系列技能连招伤害系数增加10%" });
            dt.Rows.Add(new String[] { "4437", "穿梭Ⅰ", "穿梭Ⅰ 星辰神子的星流穿梭系列技能冷却时间减少1秒" });
            dt.Rows.Add(new String[] { "4438", "穿梭Ⅱ", "穿梭Ⅱ 星辰神子的星流穿梭系列技能冷却时间减少2秒" });
            dt.Rows.Add(new String[] { "4443", "星移Ⅰ", "星移Ⅰ 星辰神子的斗转星移系列技能伤害系数增加20%" });
            dt.Rows.Add(new String[] { "4444", "星移Ⅱ", "星移Ⅱ 星辰神子的斗转星移系列技能伤害系数增加40%" });
            dt.Rows.Add(new String[] { "4446", "星域Ⅰ", "星域Ⅰ 星辰神子的神之星域系列技能冷却时间减少1秒" });
            dt.Rows.Add(new String[] { "4447", "星域Ⅱ", "星域Ⅱ 星辰神子的神之星域系列技能冷却时间减少2秒" });
            dt.Rows.Add(new String[] { "4810", "裂变Ⅲ", "裂变Ⅲ 星辰神子的星能裂变系列技能伤害系数增加9%" });
            dt.Rows.Add(new String[] { "4811", "裂变Ⅳ", "裂变Ⅳ 星辰神子的星能裂变系列技能伤害系数增加12%" });
            dt.Rows.Add(new String[] { "4812", "裂变Ⅴ", "裂变Ⅴ 星辰神子的星能裂变系列技能伤害系数增加15%" });
            dt.Rows.Add(new String[] { "4813", "裂变Ⅵ", "裂变Ⅵ 星辰神子的星能裂变系列技能伤害系数增加18%" });
            dt.Rows.Add(new String[] { "4814", "裂变Ⅶ", "裂变Ⅶ 星辰神子的星能裂变系列技能伤害系数增加21%" });
            dt.Rows.Add(new String[] { "4824", "御星诀Ⅲ", "御星诀Ⅲ 星辰神子的御星诀系列技能连招伤害系数增加15%" });
            dt.Rows.Add(new String[] { "4825", "御星诀Ⅳ", "御星诀Ⅳ 星辰神子的御星诀系列技能连招伤害系数增加20%" });
            dt.Rows.Add(new String[] { "4826", "御星诀Ⅴ", "御星诀Ⅴ 星辰神子的御星诀系列技能连招伤害系数增加25%" });
            dt.Rows.Add(new String[] { "4827", "御星诀Ⅵ", "御星诀Ⅵ 星辰神子的御星诀系列技能连招伤害系数增加30%" });
            dt.Rows.Add(new String[] { "4828", "御星诀Ⅶ", "御星诀Ⅶ 星辰神子的御星诀系列技能连招伤害系数增加35%" });
            dt.Rows.Add(new String[] { "4838", "穿梭Ⅲ", "穿梭Ⅲ 星辰神子的星流穿梭系列技能冷却时间减少3秒" });
            dt.Rows.Add(new String[] { "4839", "穿梭Ⅳ", "穿梭Ⅳ 星辰神子的星流穿梭系列技能冷却时间减少4秒" });
            dt.Rows.Add(new String[] { "4840", "穿梭Ⅴ", "穿梭Ⅴ 星辰神子的星流穿梭系列技能冷却时间减少5秒" });
            dt.Rows.Add(new String[] { "4841", "穿梭Ⅵ", "穿梭Ⅵ 星辰神子的星流穿梭系列技能冷却时间减少6秒" });
            dt.Rows.Add(new String[] { "4842", "穿梭Ⅶ", "穿梭Ⅶ 星辰神子的星流穿梭系列技能冷却时间减少7秒" });
            dt.Rows.Add(new String[] { "4852", "星移Ⅲ", "星移Ⅲ 星辰神子的斗转星移系列技能伤害系数增加60%" });
            dt.Rows.Add(new String[] { "4853", "星移Ⅳ", "星移Ⅳ 星辰神子的斗转星移系列技能伤害系数增加80%" });
            dt.Rows.Add(new String[] { "4854", "星移Ⅴ", "星移Ⅴ 星辰神子的斗转星移系列技能伤害系数增加100%" });
            dt.Rows.Add(new String[] { "4855", "星移Ⅵ", "星移Ⅵ 星辰神子的斗转星移系列技能伤害系数增加120%" });
            dt.Rows.Add(new String[] { "4856", "星移Ⅶ", "星移Ⅶ 星辰神子的斗转星移系列技能伤害系数增加140%" });
            dt.Rows.Add(new String[] { "4859", "星域Ⅲ", "星域Ⅲ 星辰神子的神之星域系列技能冷却时间减少3秒" });
            dt.Rows.Add(new String[] { "4860", "星域Ⅳ", "星域Ⅳ 星辰神子的神之星域系列技能冷却时间减少4秒" });
            dt.Rows.Add(new String[] { "4861", "星域Ⅴ", "星域Ⅴ 星辰神子的神之星域系列技能冷却时间减少5秒" });
            dt.Rows.Add(new String[] { "4862", "星域Ⅵ", "星域Ⅵ 星辰神子的神之星域系列技能冷却时间减少6秒" });
            dt.Rows.Add(new String[] { "4863", "星域Ⅶ", "星域Ⅶ 星辰神子的神之星域系列技能冷却时间减少7秒" });
            return dt;
        }

        //取神火超凡效果
        public static string GetGodexpName(string god_exp)
        {
            DataTable dt = Moyu.GodexpList();
            string sql = String.Format("id='{0}'", god_exp);
            DataRow[] dr = dt.Select(sql);
            if (dr.Length < 1) return "无";
            return dr[0]["name"].ToString();
        }

        //获取神火副属性
        public static DataTable GetFireAuxAttri(int itemtype)
        {
            string sql = String.Format("select * from cq_fireauxattri where stage=0 and itemtype='{0}' order by color;", itemtype);
            return MySqlHelper.GetDataTable(sql);
        }

        //获取角色幻兽信息
        public static DataTable GetUserEudemon(int player_id)
        {
            string sql = String.Format("select a.*,b.name as type from cq_eudemon as a left join cq_itemtype as b on a.item_type=b.id  where a.player_id='{0}' order by star_lev desc;", player_id);
            return MySqlHelper.GetDataTable(sql);
        }

        //根据ID取幻兽信息
        public static DataTable GetEudemonInfo(int item_id)
        {
            string sql = String.Format("select * from cq_eudemon where id='{0}';", item_id);
            return MySqlHelper.GetDataTable(sql);
        }

        //幻兽初始属性值分割
        public static string GetEudemonInitValue(string value, int index)
        {
            string str1 = "";
            string str2 = "";
            switch (value.Length)
            {
                case 2:
                    str1 = "0";
                    str2 = value;
                    break;
                case 3:
                    str1 = value.Substring(0, 1);
                    str2 = value.Substring(1, 2);
                    break;
                case 4:
                    str1 = value.Substring(0, 2);
                    str2 = value.Substring(2, 2);
                    break;
                default:
                    str1 = "0";
                    str2 = "0";
                    break;
            }
            if (index == 2) return str2;
            return str1;
        }
        //幻兽初始属性值生成 
        public static string SetEudemonInitValue(string value)
        {
            string str = "";
            switch (value.Length)
            {
                case 1:
                    str = "0" + value;
                    break;
                case 2:
                    str = value;
                    break;
                default:
                    str = value.Substring(0, 2);
                    break;
            }
            return str;
        }

        //抽奖数据
        public static DataTable GetLottery()
        {
            string sql = "select * from cq_lottery  order by prize_item;";
            return MySqlHelper.GetDataTable(sql);
        }

        //模糊查询怪物数据
        public static DataTable GetMonsterType(string keyword)
        {
            string sql = String.Format("select * from cq_monstertype where id like '%{0}%' or name like '%{1}%'", keyword, keyword);
            return MySqlHelper.GetDataTable(sql);
        }

        //模糊查询刷怪数据
        public static DataTable GetMonsterGen(string keyword)
        {
            string sql = String.Format("select b.id as mon_id,b.name,a.* from cq_generator as a LEFT JOIN cq_monstertype as b on a.npctype=b.id where b.name like'%{1}%' or b.id like'%{1}%';", keyword, keyword);
            return MySqlHelper.GetDataTable(sql);
        }

        //获取所有地图刷怪数量
        public static DataTable GetAllMapMonster()
        {
            string sql = "select b.id,b.name,count(a.max_per_gen) as num from cq_generator as a LEFT JOIN cq_map as b on a.mapid=b.id GROUP BY b.id order by b.id;";
            return MySqlHelper.GetDataTable(sql);
        }

        //根据地图获取刷怪信息(爆率用)
        public static DataTable GetMapMonster(long map_id) //新端地图ID很大
        {
            string sql = String.Format("select b.rest_secs,count(b.max_per_gen) as num,a.* from cq_monstertype as a LEFT JOIN cq_generator as b on b.npctype = a.id where b.mapid = '{0}' GROUP BY b.npctype order by a.name;", map_id);
            return MySqlHelper.GetDataTable(sql);
        }

        //根据地图获取刷怪信息息(刷怪用)
        public static DataTable GetMapMonsterGen(long map_id) //新端地图ID很大
        {
            string sql = String.Format("select b.*,a.id as mon_id,a.name from cq_monstertype as a LEFT JOIN cq_generator as b on b.npctype = a.id where b.mapid = '{0}' order by b.npctype;", map_id);
            return MySqlHelper.GetDataTable(sql);
        }

        //获取掉落规则基本信息
        public static DataTable GetDropitemGroup(int group_id)
        {
            string sql = String.Format("select  id,ruleid,chance,CASE WHEN item0 > 0 THEN 1 ELSE 0 END+ CASE WHEN item1 > 0 THEN 1 ELSE 0 END+ CASE WHEN item2 > 0 THEN 1 ELSE 0 END" +
                "+ CASE WHEN item3 > 0 THEN 1 ELSE 0 END+ CASE WHEN item4 > 0 THEN 1 ELSE 0 END + CASE WHEN item5 > 0 THEN 1 ELSE 0 END+ CASE WHEN item6 > 0 THEN 1 ELSE 0 END" +
                "+ CASE WHEN item7 > 0 THEN 1 ELSE 0 END+ CASE WHEN item8 > 0 THEN 1 ELSE 0 END+ CASE WHEN item9 > 0 THEN 1 ELSE 0 END+ CASE WHEN item10 > 0 THEN 1 ELSE 0 END" +
                "+ CASE WHEN item11 > 0 THEN 1 ELSE 0 END+ CASE WHEN item12 > 0 THEN 1 ELSE 0 END+ CASE WHEN item13 > 0 THEN 1 ELSE 0 END+ CASE WHEN item14 > 0 THEN 1 ELSE 0 END as itemnum " +
                "from cq_dropitemrule where group_id= '{0}';", group_id);
            return MySqlHelper.GetDataTable(sql);

        }

        //获取掉落物品信息
        public static DataTable GetDropitemInfo(int dropitem_id)
        {
            string sql = String.Format("select * from cq_dropitemrule where id='{0}';", dropitem_id);
            return MySqlHelper.GetDataTable(sql);
        }

        //物品ID取物品名称
        public static string ItemId2Name(int item_id)
        {
            string sql = String.Format("select name from cq_itemtype where id='{0}';", item_id);
            object o = MySqlHelper.Find(sql);
            if (o != null) return System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])o);
            return "无";
        }

        //脚本追踪
        public static DataTable GetAction(int action_id)
        {
            string sql = String.Format("select * from cq_action where id='{0}';", action_id);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return dt;

        }


        //怪物脚本掉落物品
        public static List<string> GetMonsterActionDropItem(int action_id)
        {
            string sql = String.Format("select * from cq_action where id='{0}';", action_id);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            if (dt.Rows.Count < 1) throw new Exception("找不到这个脚本ID");

            int id_next = Convert.ToInt32(dt.Rows[0]["id_next"]);
            int id_nextfail = Convert.ToInt32(dt.Rows[0]["id_nextfail"]);
            int type = Convert.ToInt32(dt.Rows[0]["type"]);
            string param = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[0]["param"]);

            // strList.Add(param);
            if (type == 801)
            {
                //String[] strs = param.Split(' ');//有些人脚本写法"dropitem 815012 25星XO礼包" 不这样做会导致出错
                String[] strs = System.Text.RegularExpressions.Regex.Split(param, @"\s{1,}");
                for (int i = 0; i < strs.Length; i++)
                {
                    string str = RemoveNotNumber(strs[i]);
                    if (str.Length > 5)//避免空字符串，假物品ID
                    {
                        if (!strList.Exists(x => x == str)) strList.Add(str);//排除重复项
                    }
                }
            }
            if (id_next != 0)
            {
                GetMonsterActionDropItem(id_next);
            }
            if (id_nextfail != 0)
            {
                GetMonsterActionDropItem(id_nextfail);
            }

            return strList;
        }

        //删除账号
        public static bool DelAccount(string id)
        {
            string sql = String.Format("DELETE FROM account WHERE id ='{0}';" +
                "DELETE FROM cq_card where account_id ='{1}';" +
                "DELETE FROM cq_card2 where account_id ='{2}';" +
                "delete account_pwd from account as a inner join account_pwd as b on a.name=b.name where a.id='{3}';", id, id, id, id);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //删除角色信息
        public static bool DelUser(string id)
        {
            string donaId = "user_id";
            if (IsNewDB) donaId = "id";
            string sql = String.Format("delete from cq_donation_dynasort_rec where {0}={1};", donaId, id);
            sql += String.Format("DELETE FROM {0} WHERE id ='{1}';" +
                "DELETE FROM cq_item WHERE player_id ='{2}';" +
                "DELETE FROM cq_eudemon WHERE player_id ='{3}';" +
                "DELETE FROM cq_dyna_rank_rec WHERE user_id ='{4}';" +
                "DELETE FROM `cq_friend` WHERE (`userid` = '{5}' OR `friend` = '{6}');" +
                "DELETE FROM `cq_partner` WHERE (`user_id` = '{7}' OR `partner_id` = '{8}');" +
                "DELETE FROM `cq_enemy` WHERE (`userid` = '{9}' OR `enemy` = '{10}');" +
                "DELETE FROM `cq_magic` WHERE (`ownerid`='{11}');" +
                "DELETE FROM `cq_skill` WHERE (`owner_id`='{12}');" +
                "DELETE FROM `cq_synattr` WHERE (`id`='{13}');" +
                "DELETE FROM `cq_taskdetail` WHERE (`userid`='{14}');" +
                "DELETE FROM `cq_special_status` WHERE (`user_id`='{15}');" +
                "DELETE FROM `cq_syndicate` WHERE `leader_id` ='{16}';" +
                "DELETE FROM `cq_family` WHERE `leader_id` ='{17}';" +
                "DELETE FROM `cq_family_attr` WHERE id ='{18}';" +
                "delete from cq_token where id_source='{19}' or id_target='{20}';" +
                "delete from e_money where id_source='{21}' or id_target='{22}';"
                , tName, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id, id);
            if (IsNewDB)
            {
                sql += String.Format("DELETE FROM `cq_eudlookinfo` WHERE (`ownerid`='{0}');" +
                    "DELETE FROM `cq_faceinfo` WHERE (`ownerid`='{1}');" +
                    "DELETE FROM `cq_hairinfo` WHERE (`ownerid`='{2}');" +
                    "DELETE FROM `cq_mailinfo` WHERE (`userid`='{3}');" +
                    "DELETE FROM `cq_packpetinfo` WHERE (`ownerid`='{4}');" +
                    "DELETE FROM `cq_goddess` WHERE (`userid`='{5}');" +
                    "DELETE FROM `cq_goddessservant` WHERE (`userid`='{6}');" +
                    "DELETE FROM `cq_titleid` WHERE (`ownerid`='{7}');" +
                    "DELETE FROM `cq_newtaskdetail` WHERE (`userid`='{8}')", id, id, id, id, id, id, id, id, id);
            }
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //按等级查询角色
        public static DataTable GetUserWithLevel(string level)
        {
            string sql = String.Format("select * from {0} where level<'{1}';", tName, level);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return dt;
        }
        //按登录时间查询角色
        public static DataTable GetUserWithLastLogin(int days)
        {
            //老端格式 2102241800 新端格式20190514
            DateTime time = DateTime.Now.AddDays(-days);
            string login = "";
            if (Moyu.IsNewDB)
            {
                login = time.ToString("yyyyMMdd");
            }
            else
            {
                login = time.ToString("yyMMddHHmm");
            }
            string sql = String.Format("select * from {0} where last_login<'{1}';", tName, login);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            return dt;
        }

        //查询无角色账号
        public static DataTable GetNoRoleAccount()
        {
            string sql = String.Format("select * from account as a left join {0} as b on a.id=b.account_id where b.id is null;", tName);
            return MySqlHelper.GetDataTable(sql);
        }

        //查询无账号角色
        public static DataTable GetNoAccountRole()
        {
            string sql = String.Format("select a.id from {0} as a left join account as b on a.account_id = b.id where b.id is null;", tName);
            return MySqlHelper.GetDataTable(sql);
        }

        //比较两个库的ID差异值(合区 step1)
        public static int GetIdGap(string dbName1, string dbName2, string tableName, string field = "id")
        {
            int maxid = 0;
            int minid = 0;
            string sql = String.Format("select max({0}) from {1}.{2};", field, dbName1, tableName);
            string sql2 = String.Format("select min({0}) from {1}.{2};", field, dbName2, tableName);
            object max = MySqlHelper.Find(sql);
            object min = MySqlHelper.Find(sql2);
            if (max != DBNull.Value) maxid = Convert.ToInt32(max);
            if (min != DBNull.Value) minid = Convert.ToInt32(min);
            int gap = (maxid - minid) + 1;
            return gap;
        }

        //查账号是否存在
        public static bool isAccountExist(string dbName, string accountName)
        {
            string sql = String.Format("select id from {0}.account where name='{1}';", dbName, accountName);
            object o = MySqlHelper.Find(sql);
            if (o != null) return true;
            return false;
        }

        //查人物是否存在
        public static bool isPlayerExist(string dbName, string accountName, bool isNewDB = false)
        {
            string tableName = "cq_user";
            if (isNewDB) tableName = "cq_user_new";
            string sql = String.Format("select id from {0}.{1} where name='{2}';", dbName, tableName, accountName);
            object o = MySqlHelper.Find(sql);
            if (o != null) return true;
            return false;
        }

        //查军团是否存在
        public static bool isSynExist(string dbName, string synName)
        {

            string sql = String.Format("select id from {0}.cq_syndicate where name='{1}';", dbName, synName);
            object o = MySqlHelper.Find(sql);
            if (o != null) return true;
            return false;
        }

        //查家族是否存在
        public static bool isFamilyExist(string dbName, string familyName)
        {
            string sql = String.Format("select id from {0}.cq_family where family_name='{1}';", dbName, familyName);
            object o = MySqlHelper.Find(sql);
            if (o != null) return true;
            return false;
        }
        //生成新名称
        public static string GetNewName(string sourceDb, string targetDb, string name, int mode = 0, bool isNewDB = false)
        {
            string newName = name + "a";
            if (mode == 0)
            {
                if (!isAccountExist(sourceDb, newName) && !isAccountExist(targetDb, newName))
                {
                    strNewName = newName;
                    if (strNewName.Length > 32)//数据库允许最大字符32
                    {
                        Random r = new Random(Guid.NewGuid().GetHashCode());
                        int i = r.Next(999999);
                        strNewName = "account" + i;
                    }
                }
                else
                {
                    GetNewName(sourceDb, targetDb, newName, 0);
                }

            }
            if (mode == 1)
            {
                if (!isPlayerExist(sourceDb, newName, isNewDB) && !isPlayerExist(targetDb, newName, isNewDB))
                {
                    strNewName = newName;
                    if (strNewName.Length > 8)//数据库允许最大字符15
                    {
                        Random r = new Random(Guid.NewGuid().GetHashCode());
                        int i = r.Next(99999);
                        strNewName = "rerole[1" + i + "]";
                    }
                }
                else
                {
                    GetNewName(sourceDb, targetDb, newName, 1, isNewDB);
                }

            }
            if (mode == 2)
            {
                if (!isSynExist(sourceDb, newName) && !isSynExist(targetDb, newName))
                {
                    strNewName = newName;
                    if (strNewName.Length > 8)//数据库允许最大字符15
                    {
                        Random r = new Random(Guid.NewGuid().GetHashCode());
                        int i = r.Next(99999);
                        strNewName = "resyn[" + i + "]";
                    }
                }
                else
                {
                    GetNewName(sourceDb, targetDb, newName, 2);
                }

            }
            if (mode == 3)
            {
                if (!isFamilyExist(sourceDb, newName) && !isFamilyExist(targetDb, newName))
                {
                    strNewName = newName;
                    if (strNewName.Length > 8)//数据库允许最大字符15
                    {
                        Random r = new Random(Guid.NewGuid().GetHashCode());
                        int i = r.Next(99999);
                        strNewName = "refam[" + i + "]";
                    }
                }
                else
                {
                    GetNewName(sourceDb, targetDb, newName, 3);
                }

            }

            return strNewName;
        }

        //更新账号ID(合区 step2)
        public static bool UpdateAccountId(string dbName, int idGap, bool isNewDB = false)
        {
            string tableName = "cq_user";
            if (isNewDB) tableName = "cq_user_new";
            string sql = String.Format("use {0};" +
                "update account set id = 2147483647-id;" +
                "update {1} set account_id = account_id + {2};" +
                "update cq_card set account_id = account_id + {3},chk_sum = account_id^780000;" +
                "update cq_card2 set account_id = account_id + {4},chk_sum = account_id^780001;" +
                "update account set id= (2147483647-id)+{5};"
                , dbName, tableName, idGap, idGap, idGap, idGap);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }


        //处理重复账号(合区 step3)
        public static List<MyList> UpdateExistAccountName(string targetDb, string sourceDb)
        {

            string sql = String.Format("select a.name from {0}.account as a inner join {1}.account as b on a.name=b.name;", sourceDb, targetDb);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            List<MyList> List = new List<MyList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                string newName = GetNewName(sourceDb, targetDb, name);
                string sql2 = String.Format("update {0}.account set name='{1}' where name='{2}';" +
                    "update {3}.account_pwd set name='{4}' where name='{5}';", sourceDb, newName, name, sourceDb, newName, name);
                MySqlHelper.Query(sql2);
                List.Add(new MyList(name, newName));
            }
            return List;
        }
        //更新人物ID(合区 step4)
        public static bool UpdatePlayerId(string dbName, int idGap, bool isNewDB = false)
        {
            string tableName = "cq_user";
            if (isNewDB) tableName = "cq_user_new";
            //爵位排序
            string sql = String.Format("use {0};update cq_donation_dynasort_rec set user_id = user_id+{1};", dbName, idGap);
            if (isNewDB)
            {
                sql = String.Format("use {0};update cq_donation_dynasort_rec set id = 2147483647-id;", dbName);
                sql += String.Format("update cq_donation_dynasort_rec set id = 2147483647-id;" +//爵位排序
                    "update cq_faceinfo set ownerid =ownerid+{0};" +//头像
                    "update cq_hairinfo set ownerid = ownerid +{1};" +//发型
                    "update cq_mailinfo set userid = userid +{2};" +//邮件
                    "update cq_packpetinfo set ownerid = ownerid +{3};" +//跟宠
                    "update cq_goddess set userid = userid +{4};" + //女神
                    "update cq_newtaskdetail set userid = userid +{5};" +//任务
                    "update cq_titleid set ownerid = ownerid +{6};" +//称号
                    "update cq_eudlookinfo set ownerid = ownerid +{7};" +//宝宝皮肤
                    "update cq_goddessservant set userid = userid +{8};" +//女神天赋师
                    "update cq_donation_dynasort_rec set id = (2147483647-id)+{9};"
                , idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap);
            }
            sql += String.Format(
                "update {0} set id=2147483647-id;" +
                "update cq_synattr set id = 2147483647-id;" +
                "update cq_family_attr set id = 2147483647-id;" +
                "update cq_item set availabletime='12345678',player_id =player_id+{1};" +//物品
                "update cq_eudemon set player_id = player_id +{2};" +//幻兽
                "update cq_skill set owner_id = owner_id +{3};" +//图鉴
                "update cq_pk_bonus set target_id = target_id +{4};" +//悬赏
                "update cq_pk_item set hunter_id = hunter_id +{5};" +
                "update cq_pk_item set target_id = target_id +{6};" +
                "update cq_syndicate set leader_id = leader_id +{7};" +//军团
                "update cq_family set leader_id = leader_id +{8}; " +//家族
                "update cq_magic set ownerid = ownerid +{9} where ownerid<2000000000;" +//技能 但是要排除幻兽
                "update cq_enemy set enemy = enemy +{10};" +
                "update cq_enemy set userid = userid +{11};" +//仇人
                "update cq_friend set friend = friend +{12};" +
                "update cq_friend set userid = userid +{13};" +//好友
                "update cq_tutor set tutor_id = tutor_id +{14};" +
                "update cq_tutor set user_id = user_id +{15}; " +//师徒
                "update cq_announce set user_id = user_id +{16};" +//导师发布的公告
                "update cq_partner set partner_id = partner_id +{17};" +
                "update cq_partner set user_id = user_id +{18};" +//商业
                "update cq_special_status set user_id = user_id +{19};" +//特殊状态 
                "update cq_statistic set player_id = player_id +{20};" +//资料统计
                "update cq_taskdetail set userid = userid +{21};" +//任务
                "update cq_flower set player_id = player_id +{22};" +//玫瑰
                "update cq_synattr set id = (2147483647-id)+{23};" +//军团成员
                "update cq_family_attr set id = (2147483647-id)+{24};" +//家族成员
                "update cq_dynanpc set ownerid = ownerid +{25} where name in('摊位旗','市场城堡地皮') and ownerid !=0;" +//摊位地皮
                "update {26} set id=(2147483647-id)+{27};" +//摊位
                "update cq_item set owner_id=owner_id+{28} where owner_id>99999;" + //物品幻兽还要更新owner_id
                "update cq_eudemon set owner_id=owner_id+{29} where owner_id>99999;"
                , tableName, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap, idGap,
                idGap, idGap, idGap, idGap, idGap, tableName, idGap, idGap, idGap);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //处理重复人物(合区 step5)
        public static List<MyList> UpdateExistPlayerName(string targetDb, string sourceDb, bool isNewDB = false)
        {
            string tableName = "cq_user";
            if (isNewDB) tableName = "cq_user_new";
            string sql = String.Format("select a.name from {0}.{1} as a inner join {2}.{3} as b on a.name=b.name;", sourceDb, tableName, targetDb, tableName);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            List<MyList> List = new List<MyList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                string newName = GetNewName(sourceDb, targetDb, name, 1, isNewDB);
                UpdatePlayerName(sourceDb, name, newName, isNewDB);
                List.Add(new MyList(name, newName));
            }
            return List;
        }

        //更改人物名字
        public static bool UpdatePlayerName(string dbName, string playerName, string newPlayerName, bool isNewDB = false)
        {

            string tableName = "cq_user";
            if (isNewDB) tableName = "cq_user_new";
            string sql = String.Format("use {0};" +
                "update {1} set name='{2}' where name='{3}';", dbName, tableName, newPlayerName, playerName);
            sql += String.Format("update {0} set mate='{1}' where mate='{2}';", tableName, newPlayerName, playerName); //配偶
            sql += String.Format("update cq_donation_dynasort_rec set user_name='{0}' where user_name='{1}';", newPlayerName, playerName);//爵位排序
            sql += String.Format("update cq_eudemon set ori_owner_name='{0}' where ori_owner_name='{1}';", newPlayerName, playerName);//宝宝归属
            sql += String.Format("update cq_enemy set enemyname='{0}' where enemyname='{1}';", newPlayerName, playerName);//仇人
            sql += String.Format("update cq_friend set friendname='{0}' where friendname='{1}';", newPlayerName, playerName);//好友
            sql += String.Format("update cq_family set leader_name='{0}' where leader_name='{1}';", newPlayerName, playerName);//家族
            sql += String.Format("update cq_syndicate set leader_name='{0}' where leader_name='{1}';", newPlayerName, playerName);//军团
            sql += String.Format("update cq_partner set partner_name='{0}' where partner_name='{1}';", newPlayerName, playerName);//商业伙伴
            sql += String.Format("update cq_tutor set user_name='{0}' where user_name='{1}';", newPlayerName, playerName);//师徒
            sql += String.Format("update cq_tutor set tutor_name='{0}' where tutor_name='{1}';", newPlayerName, playerName);
            sql += String.Format("update cq_pk_item set target_name='{0}' where target_name='{1}';", newPlayerName, playerName);//扣押装备
            sql += String.Format("update cq_pk_item set hunter_name='{0}' where hunter_name='{1}';", newPlayerName, playerName);
            sql += String.Format("update cq_pk_bonus set target_name='{0}' where target_name='{1}';", newPlayerName, playerName);
            sql += String.Format("update cq_pk_bonus set hunter_name='{0}' where hunter_name='{1}';", newPlayerName, playerName);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //处理重复军团名
        public static List<MyList> UpdateExistSynName(string targetDb, string sourceDb)
        {

            string sql = String.Format("select id,name from {0}.cq_syndicate;", sourceDb);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            List<MyList> List = new List<MyList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["name"]);
                if (isSynExist(targetDb, name))
                {
                    string newName = GetNewName(sourceDb, targetDb, name, 2);
                    string sql2 = String.Format("update {0}.cq_syndicate set name='{1}' where name='{2}';", sourceDb, newName, name);
                    MySqlHelper.Query(sql2);
                    List.Add(new MyList(name, newName));
                }
            }
            return List;
        }

        //处理重复家族
        public static List<MyList> UpdateExistFamilyName(string targetDb, string sourceDb)
        {

            string sql = String.Format("select id,family_name from {0}.cq_family;", sourceDb);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            List<MyList> List = new List<MyList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string name = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt.Rows[i]["family_name"]);
                if (isFamilyExist(targetDb, name))
                {
                    string newName = GetNewName(sourceDb, targetDb, name, 3);
                    string sql2 = String.Format("update {0}.cq_family set family_name='{1}' where family_name='{2}';", sourceDb, newName, name);
                    MySqlHelper.Query(sql2);
                    List.Add(new MyList(name, newName));
                }
            }
            return List;
        }

        //更新其他表ID(合区 step6)
        public static bool UpdateTableId(string dbName, string tableName, int idGap)
        {
            string sql = String.Format("update {0}.{1} set id = 2147483647-id;", dbName, tableName);

            if (tableName == "cq_syndicate")//军团成员
            {
                sql += String.Format("update cq_synattr set syn_id = syn_id + {0};", idGap);
            }
            if (tableName == "cq_family")//家族成员
            {
                sql += String.Format("update cq_family_attr set family_id = family_id + {0};", idGap);
            }
            if (tableName == "cq_eudemon")//幻兽
            {
                sql += String.Format("update cq_magic set ownerid = ownerid +{0} where ownerid>2000000000;", idGap);
            }
            if (MySqlHelper.Query(sql) > 0)
            {
                string sql2 = String.Format("update {0}.{1} set id = (2147483647-id)+{2};", dbName, tableName, idGap);
                if (MySqlHelper.Query(sql2) > 0) return true;
            }
            return false;
        }

        //返还占领摊位地皮金币到仓库
        public static DataTable backMoneyByCap(string dbName, bool isNewDB = false)
        {
            string tableName = "cq_user";
            if (isNewDB) tableName = "cq_user_new";
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(String));
            dt.Columns.Add("name", typeof(String));
            dt.Columns.Add("money", typeof(String));
            string sql = String.Format("use {0};select a.id,ownerid,price,b.name from cq_dynanpc as a inner JOIN {1} as b on a.ownerid=b.id where a.name in('摊位旗','市场城堡地皮') and ownerid !=0;", dbName, tableName);
            DataTable dt2 = MySqlHelper.GetDataTable(sql);
            if (dt2.Rows.Count < 1) return dt;
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                string sql2 = String.Format("update {0}.{1} set money_saved=money_saved+{2} where id='{3}';", dbName, tableName, dt2.Rows[i]["price"].ToString(), dt2.Rows[i]["ownerid"].ToString());
                if (MySqlHelper.Query(sql2) > 0)
                {
                    DataRow dr = dt.NewRow();
                    dr["id"] = dt2.Rows[i]["id"];
                    dr["name"] = System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])dt2.Rows[i]["name"]);
                    dr["money"] = dt2.Rows[i]["price"];
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }

        //合并表数据
        public static bool CombineTable(string dbName1, string dbName2, string tableName)
        {
            string sql = String.Format("insert into {0}.{1} (select * from {2}.{3});", dbName1, tableName, dbName2, tableName);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }


        //人物ID重新编号
        public static bool RemakeUserId(string dbName, int oldId, int newId, bool isNewDB = false)
        {
            string sql = String.Format("use {0};update cq_donation_dynasort_rec set user_id={1} where user_id={2};", dbName, newId, oldId);
            if (isNewDB)
            {
                sql = String.Format("use {0};update cq_donation_dynasort_rec set id={1} where id={2};" +//爵位排序
                    "update cq_faceinfo set ownerid ={3} where ownerid={4};" +//头像
                    "update cq_hairinfo set ownerid ={5} where ownerid={6};" +//发型
                    "update cq_mailinfo set userid ={7} where userid={8};" +//邮件
                    "update cq_packpetinfo set ownerid ={9} where ownerid={10};" +//跟宠
                    "update cq_goddess set userid ={11} where userid={12};" + //女神
                    "update cq_newtaskdetail set userid ={13} where userid={14};" +//任务
                    "update cq_titleid set ownerid ={15} where ownerid={16};" +//称号
                    "update cq_eudlookinfo set ownerid ={17} where ownerid={18};" +//宝宝皮肤
                    "update cq_goddessservant set userid ={19} where userid={20};"//天赋师
                    , dbName, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId);
            }
            sql += String.Format(
                "update cq_item set availabletime='12345678',player_id ={0} where player_id={1};" +//物品
                "update cq_eudemon set player_id ={2} where player_id ={3};" +//幻兽
                "update cq_skill set owner_id ={4} where owner_id ={5};" +//图鉴
                "update cq_pk_bonus set target_id ={6} where target_id ={7};" +//悬赏
                "update cq_pk_item set hunter_id ={8} where hunter_id ={9};" +
                "update cq_pk_item set target_id ={10} where target_id ={11};" +
                "update cq_syndicate set leader_id ={12} where leader_id ={13};" +//军团
                "update cq_family set leader_id ={14} where leader_id ={15}; " +//家族
                "update cq_magic set ownerid ={16} where ownerid ={17};" +//技能
                "update cq_enemy set enemy ={18} where enemy ={19};" +
                "update cq_enemy set userid ={20} where userid ={21};" +//仇人
                "update cq_friend set friend ={22} where friend ={23};" +
                "update cq_friend set userid ={24} where userid ={25};" +//好友
                "update cq_tutor set tutor_id ={26} where tutor_id ={27};" +
                "update cq_tutor set user_id ={28} where user_id ={29}; " +//师徒
                "update cq_announce set user_id ={30} where user_id ={31};" +//导师发布的公告
                "update cq_partner set partner_id ={32} where partner_id ={33};" +
                "update cq_partner set user_id ={34} where user_id ={35};" +//商业
                "update cq_special_status set user_id ={36} where user_id ={37};" +//特殊状态 
                "update cq_statistic set player_id ={38} where player_id ={39};" +//资料统计
                "update cq_taskdetail set userid ={40} where userid ={41};" +//任务
                "update cq_flower set player_id ={42} where player_id ={43};" +//玫瑰
                "update cq_synattr set id ={44} where id={45};" +//军团成员
                "update cq_family_attr set id ={46} where id={47};" +//家族成员
                "update cq_eudemon set owner_id ={48} where owner_id ={49};" +
                "update cq_item set owner_id ={50} where owner_id ={51};"
                , newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId,
                newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId, newId, oldId,
                newId, oldId, newId, oldId, newId, oldId, newId, oldId);
            if (MySqlHelper.Query(sql) > 0) return true;
            return false;
        }

        //对于ID不重要的表重新编号
        public static int RemakeTableId(string dbName, String tableName)
        {
            string sql = String.Format("ALTER TABLE {0}.`{1}` DROP COLUMN `id`,DROP PRIMARY KEY;" +
                "ALTER TABLE {2}.`{3}` ADD COLUMN `id`  int(4) UNSIGNED ZEROFILL NOT NULL AUTO_INCREMENT FIRST, ADD PRIMARY KEY(`id`);"
                , dbName, tableName, dbName, tableName);
            return MySqlHelper.Query(sql) / 2;
        }

        //查装备魔魂属性(突破魔魂上限用)

        public static DataTable GetItemAddition()
        {
            string sql = "select * from cq_itemaddition where level=12;";
            return MySqlHelper.GetDataTable(sql);
        }

        //读取cq_config常用设置
        public static DataTable GetConfig()
        {
            string sql = "select id,type,data1,data2,str,`desc` from cq_config where type in(1100,2000,2001,2005,8100,10300,10310,80000) or type BETWEEN 3101 and 3120 order by id;";
            return MySqlHelper.GetDataTable(sql);
        }

        //魔石交易类型
        public static string GetEmoneyTradeType(int type)
        {
            string str;
            switch (type)
            {
                case 1:
                    str = "脚本给予";
                    break;
                case 2:
                    str = "玩家交易";
                    break;
                case 3:
                    str = "玩家交易";
                    break;
                case 6:
                    str = "充值领取[270魔石卡]";
                    break;
                case 8:
                    str = "充值领取[1380魔石卡]";
                    break;
                case 10:
                    str = "商城购物";
                    break;
                case 11:
                    str = "魔法飞鸽";
                    break;
                case 16:
                    str = "脚本扣除";
                    break;
                case 23:
                    str = "宝宝喂食";
                    break;
                case 30:
                    str = "地皮并购";
                    break;
                case 12:
                    str = "摊位并购";
                    break;
                case 41:
                    str = "小号交易";
                    break;
                case 19:
                    str = "求购幻兽";
                    break;
                case 20:
                    str = "幻兽换蛋";
                    break;
                case 28:
                    str = "幻兽幻生";
                    break;
                case 33:
                    str = "爵位捐献";
                    break;
                default:
                    str = type.ToString();
                    break;
            }
            return str;
        }

        //查魔石记录
        public static DataTable GetEmoneyTradeLogs(string fromTime, string toTime, int type = 0, int isIn = 0, int userId = 0)
        {
            string tableName = type == 1 ? "cq_token" : "e_money";
            string field = isIn == 1 ? "id_target" : "id_source";
            string sql = String.Format("select * from {0} where {1} !=0 and number!=0 and time_stamp between {2} and {3} order by time_stamp;", tableName, field, fromTime, toTime);//全区
            if (isIn == 2)
            {
                sql = String.Format("select * from {0} where number!=0 and time_stamp between {1} and {2} order by time_stamp;", tableName, fromTime, toTime);//全区
            }

            if (userId != 0)
            {
                sql = String.Format("select * from {0} where {1}={2} and number!=0 and time_stamp between {3} and {4} order by time_stamp;", tableName, field, userId, fromTime, toTime);
                if (isIn == 2)
                {
                    sql = String.Format("select * from {0} where id_source={1} or id_target={2} and number!=0 and time_stamp between {3} and {4} order by time_stamp;", tableName, userId, userId, fromTime, toTime);//全区
                }
            }
            return MySqlHelper.GetDataTable(sql);
        }

        //获取魔石记录交易者名称
        public static string GetEmoneyTradeUser(int id)
        {
            if (id == 0) return "系统";
            string sql = String.Format("select name from {0} where id={1};", tName, id);
            object o = MySqlHelper.Find(sql);
            if (o != null) return System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])o);
            sql = String.Format("select name from cq_npc where id={0};", id);
            o = MySqlHelper.Find(sql);
            if (o != null) return System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])o);
            sql = String.Format("select name from cq_dynanpc where id={0};", id);
            o = MySqlHelper.Find(sql);
            if (o != null) return System.Text.Encoding.GetEncoding("gb2312").GetString((byte[])o);
            sql = String.Format("select name from cq_eudemon where id={0};", id);
            o = MySqlHelper.Find(sql);
            if (o != null) return o.ToString();
            return id.ToString();
        }

        //计算宝宝属性
        public static int GetEudAttr(int initAttr, int growAttr, int level, bool isLife = false)
        {
            float growRate = isLife == true ? (float)growAttr / 100 : (float)growAttr / 1000;
            int attr = (int)(growRate * (level - 1)) + initAttr;
            return attr;
        }

        //获取宝宝初始属性评分
        /*
         * type 0 生命
         * type 1 物攻最小
         * type 2 物攻最大
         * type 3 物防
         * type 4 魔攻最小
         * type 5 魔攻最大
         * type 6 魔防
         * type 7 生命成长
         */
        public static int GetEudInitAttrStarLev(int eud_type, int type, int value)
        {
            string sql = String.Format("select * from cq_grade where id='{0}';", eud_type);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            int attrA = 0, attrB = 0, attrC = 0, attrAIndex = 0, attrCIndex = 0;
            switch (type)
            {
                case 0:
                    attrA = Convert.ToInt32(dt.Rows[0]["life_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["life_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["life_c"]);
                    attrAIndex = 1;
                    attrCIndex = 3;
                    break;
                case 1:
                    attrA = Convert.ToInt32(dt.Rows[0]["phy_min_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["phy_min_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["phy_min_c"]);
                    attrAIndex = 7;
                    attrCIndex = 9;
                    break;
                case 2:
                    attrA = Convert.ToInt32(dt.Rows[0]["phy_max_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["phy_max_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["phy_max_c"]);
                    attrAIndex = 13;
                    attrCIndex = 15;
                    break;
                case 3:
                    attrA = Convert.ToInt32(dt.Rows[0]["phy_def_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["phy_def_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["phy_def_c"]);
                    attrAIndex = 19;
                    attrCIndex = 21;
                    break;
                case 4:
                    attrA = Convert.ToInt32(dt.Rows[0]["mgc_min_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["mgc_min_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["mgc_min_c"]);
                    attrAIndex = 25;
                    attrCIndex = 27;
                    break;
                case 5:
                    attrA = Convert.ToInt32(dt.Rows[0]["mgc_max_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["mgc_max_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["mgc_max_c"]);
                    attrAIndex = 31;
                    attrCIndex = 33;
                    break;
                case 6:
                    attrA = Convert.ToInt32(dt.Rows[0]["mgc_def_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["mgc_def_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["mgc_def_c"]);
                    attrAIndex = 37;
                    attrCIndex = 39;
                    break;
                case 7:
                    attrA = Convert.ToInt32(dt.Rows[0]["life_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["life_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["life_grow_c"]);
                    attrAIndex = 4;
                    attrCIndex = 6;
                    break;
            }
            value = type == 7 ? value : value * 100;
            attrA = type == 7 ? attrA : attrA * 100;
            attrB = type == 7 ? attrB : attrB * 100;
            return GetEudInitStarLev(value, attrA, attrC, attrB, attrAIndex, attrCIndex);

        }

        //获取宝宝成长属性评分
        /*
         * type 1 物攻最小
         * type 2 物攻最大
         * type 3 物防
         * type 4 魔攻最小
         * type 5 魔攻最大
         * type 6 魔防
         * 
         */
        public static int GetEudGrowAttrStarLev(int eud_type, int type, int value)
        {
            string sql = String.Format("select * from cq_grade where id='{0}';", eud_type);
            DataTable dt = MySqlHelper.GetDataTable(sql);
            int attrA = 0, attrB = 0, attrC = 0, attrAIndex = 0, attrCIndex = 0;
            switch (type)
            {
                case 1:
                    attrA = Convert.ToInt32(dt.Rows[0]["phy_min_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["phy_min_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["phy_min_grow_c"]);
                    attrAIndex = 10;
                    attrCIndex = 12;
                    break;
                case 2:
                    attrA = Convert.ToInt32(dt.Rows[0]["phy_max_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["phy_max_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["phy_max_grow_c"]);
                    attrAIndex = 16;
                    attrCIndex = 18;
                    break;
                case 3:
                    attrA = Convert.ToInt32(dt.Rows[0]["phy_def_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["phy_def_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["phy_def_grow_c"]);
                    attrAIndex = 22;
                    attrCIndex = 24;
                    break;
                case 4:
                    attrA = Convert.ToInt32(dt.Rows[0]["mgc_min_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["mgc_min_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["mgc_min_grow_c"]);
                    attrAIndex = 28;
                    attrCIndex = 30;
                    break;
                case 5:
                    attrA = Convert.ToInt32(dt.Rows[0]["mgc_max_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["mgc_max_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["mgc_max_grow_c"]);
                    attrAIndex = 34;
                    attrCIndex = 36;
                    break;
                case 6:
                    attrA = Convert.ToInt32(dt.Rows[0]["mgc_def_grow_a"]);
                    attrB = Convert.ToInt32(dt.Rows[0]["mgc_def_grow_b"]);
                    attrC = Convert.ToInt32(dt.Rows[0]["mgc_def_grow_c"]);
                    attrAIndex = 40;
                    attrCIndex = 42;
                    break;
            }
            return GetEudGrowStarLev(value, attrA, attrC, attrB, attrAIndex, attrCIndex);
        }


        //宝宝初始属性算评分
        /*
         * 生命成长也是使用这个函数,但是参数不用乘100
         * a1 宝宝初始属性*100
         * a2 grade表对应属性a*100
         * a3 grade表对应属性c
         * a4 grade表对应属性b*100
         * a5 grade表对应属性a字段排序
         * a6 grade表对应属性c字段排序
         */
        public static int GetEudInitStarLev(int a1, int a2, int a3, int a4, int a5, int a6)
        {
            double v6; // st7@1
            double v7; // st7@1
            double v8; // st7@1
            Int64 v9; // qax@9
            double v10; // st7@10
            float v12; // [sp+0h] [bp-14h]@1
            float v13; // [sp+4h] [bp-10h]@1
            float v14; // [sp+8h] [bp-Ch]@1
            float v15; // [sp+Ch] [bp-8h]@1
            float v16; // [sp+10h] [bp-4h]@1
            float v17; // [sp+28h] [bp+14h]@1
            int v18; // [sp+30h] [bp+1Ch]@3

            v16 = (float)(a2 * 0.009999999776482582);
            v13 = (float)(a4 * 0.009999999776482582);
            v7 = a1 * 0.009999999776482582;
            v14 = (float)v7;
            v17 = (float)(v7 - v16);
            v8 = v13 - v16;
            v12 = (float)v8;
            v15 = (float)(v8 * 1.19);
            v6 = (double)a3 * 0.001000000047497451;
            if (a6 > (double)v15 && v17 > (double)v15)
            {
                v18 = 2100;
                if (a5 == 42 || a5 == 52)
                {
                    v18 = 3000;
                }
                else
                {
                    if (a5 == 8 || a5 == 9)
                        v18 = 2500;
                }
                v9 = (Int64)((v14 - v13 * 1.19 + v16 * 0.19) * v12 * (double)a3 * 15.0 / (double)v18 + v6 * v15 * v15 + 0.5);
            }
            else
            {
                v10 = v6 * v17 * v17 + 0.5;
                if (a1 <= a2)
                {
                    (v9) = ((int)(((Int64)(1431655765 * (int)(Int64)v10) >> 32)
                                  - (Int64)v10) >> 31)
                  + ((int)(((Int64)(1431655765 * (int)(Int64)v10) >> 32)
                                - (Int64)v10) >> 1);
                    if (v9 < -20) v9 = -20;
                }
                else
                {
                    v9 = (Int64)v10;
                }
            }
            return (int)v9;
        }

        //宝宝成长属性算评分
        /*
        //a1 成长属性值
        //a2 cq_grade表的属性字段a值
        //a3 cq_grade表的属性字段c值
        //a4 cq_grade表的属性字段b值
        //a5 成长属性的属性字段a值字段排序（0算起）
        //a6 成长属性的属性字段c值字段排序(0算起)
        */
        public static int GetEudGrowStarLev(int a1, int a2, int a3, int a4, int a5, int a6)
        {
            double v6; // st7@1
            double v7; // st7@1
            double v8; // st7@1
            Int64 v9; // qax@9
            double v10; // st7@10
            float v12; // [sp+0h] [bp-14h]@1
            float v13; // [sp+4h] [bp-10h]@1
            float v14; // [sp+8h] [bp-Ch]@1
            float v15; // [sp+Ch] [bp-8h]@1
            float v16; // [sp+10h] [bp-4h]@1
            float v17; // [sp+28h] [bp+14h]@1
            int v18; // [sp+30h] [bp+1Ch]@3

            v15 = (float)(a2 * 0.001);
            v12 = (float)(a4 * 0.001);
            v7 = a1 * 0.001;
            v13 = (float)v7;
            v17 = (float)(v7 - v15);
            v8 = v12 - v15;
            v14 = (float)v8;
            v16 = (float)(v8 * 1.19);
            v6 = a3 * 0.001;

            if (a6 > (double)v16 && v17 > (double)v16)
            {
                v18 = 2100;
                if (a5 == 42 || a5 == 52)
                {
                    v18 = 3000;
                }
                else
                {
                    if (a5 == 8 || a5 == 9)
                        v18 = 2500;
                }
                v9 = (Int64)((v13 - v12 * 1.19 + v15 * 0.19) * v14 * v14 * (double)a3 * 15.0 / (double)v18
                                    + v6 * v16 * v16 * v16
                                    + 0.5);
            }
            else
            {
                v10 = v6 * v17 * v17 * v17 + 0.5;
                if (a1 <= a2)
                {
                    (v9) = (int)(Int64)v10 / 3;
                    if (v9 < -20) (v9) = -20;
                }
                else
                {
                    v9 = (Int64)v10;
                }
            }
            return (int)v9;
        }
    }
}
