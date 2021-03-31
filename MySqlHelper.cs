using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace gmtoolNew
{
    public class MySqlHelper
    {
        //private MySqlHelper() { }

        //连接字符串

        public static string connStr = null;

        //单实例
        /*private static MySqlHelper _instance = null;
        public static MySqlHelper Ins
        {
            get { if (_instance == null) { _instance = new MySqlHelper(); } return _instance; }
        }*/


        // 测试数据库连接
        public static bool TestConn()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                if (connStr == null) return false; //用于所有操作前检测数据库连接，防止不连接数据库直接进行操作。

                try
                {

                    string sql = String.Format("select count(id) from {0};", Moyu.tName);
                    Find(sql);
                    return true;
                }
                catch (MySqlException ex)
                {
                    throw new Exception("数据库连接出错：" + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 增删改，返回影响行数
        /// insert 返回ID
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static int Query(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddRange(paras);
                conn.Open();
                if (sql.Contains("insert") || sql.Contains("INSERT"))
                {
                    cmd.ExecuteNonQuery();
                    long id = cmd.LastInsertedId;
                    return Convert.ToInt32(id);
                }
                return cmd.ExecuteNonQuery();

            }
        }

        /// <summary>
        /// 查询一条
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static object Find(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddRange(paras);
                if (paras != null && paras.Length > 0)
                    cmd.Parameters.AddRange(paras);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// 查询，返回DataReader
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static MySqlDataReader Select(string sql, params MySqlParameter[] paras)
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            MySqlDataReader dr = null;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddRange(paras);
            try
            {
                conn.Open();
                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (MySqlException ex)
            {
                //conn.Close();
                throw new Exception("执行查询异常！" + ex.Message);
            }
            return dr;
        }

        /// <summary>
        /// 获得多个结果集，填充DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddRange(paras);
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// 获得一个结果集，填充DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, params MySqlParameter[] paras)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddRange(paras);
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
