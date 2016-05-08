using System;
using System.Data.SqlClient;

namespace UIShell.WpfShellPlugin
{
    //连接数据库
    public class dbinsert
    {
        private static string connectionString =
            "Server =;" +
            "Database =cabinet;" +
            "User ID =sa;" +
            "Password =;";

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <returns></returns>
        private SqlConnection ConnectionOpen()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 向表(Table)中插入一条数据
        /// </summary>
        public void Insert(int value1, string value2, string value3, int num)
        {
            SqlConnection conn = ConnectionOpen();
            string sql =
                "insert into Table(row1, row2, row3, DateTime) values ('" +
                value1 + "', '" + value2 + "', '" + value3 + "', '" + num + "')";
            SqlCommand comm = new SqlCommand(sql, conn);
            comm.ExecuteReader();
　　
            conn.Close();
        }

        /// <summary>
        /// 从数据库中获取当前时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTimeFromSQL()
        {
            SqlConnection conn = ConnectionOpen();
            string sql = "select getdate()";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            DateTime dt;
            if (reader.Read())
            {
                dt = (DateTime)reader[0];
                conn.Close();
                return dt;
            }
            conn.Close();
            return DateTime.MinValue;
        }
    }
}
