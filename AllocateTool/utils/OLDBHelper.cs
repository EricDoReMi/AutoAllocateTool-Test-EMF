using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Threading;

namespace AllocateTool.utils
{
    public partial class OLDBHelper
    {

        private static string driverNameStr;//Driver
        private static string persistSecurityStr;//PersistSecurity
        private static string dataSourceStr;//DataSource
        private static string connStr;//connectStr
        private static ThreadLocal<OleDbConnection> tlConn = new ThreadLocal<OleDbConnection>();
   
        //初始化
        static OLDBHelper() {
            driverNameStr = ConfigurationManager.AppSettings["DriverNameStr"];
            
            persistSecurityStr = ConfigurationManager.AppSettings["PersistSecurityStr"];

            dataSourceStr = ConfigurationManager.AppSettings["DataSourceStr"];

            connStr = driverNameStr + persistSecurityStr + dataSourceStr;
        }

        public static OleDbConnection GetConnection()  {
           OleDbConnection conn = tlConn.Value;//数据库连接

            if (conn == null)
            {

                conn = new OleDbConnection(connStr);

                tlConn.Value = conn;

            }
            
            return conn;
        }

        public static void CloseConnection() {
            OleDbConnection conn = tlConn.Value;//数据库连接
            if (conn!=null && conn.State.Equals(ConnectionState.Open))
            {
                conn.Close();
            }
        }


        //释放数据库连接的资源
        private void DisposeDB()
        {
            OleDbConnection conn = tlConn.Value;//数据库连接
            if (conn != null)
            {
                conn.Dispose();
                conn = null;
            }
        }

        ~OLDBHelper()
        {
            
            DisposeDB();
        }


    }
}
