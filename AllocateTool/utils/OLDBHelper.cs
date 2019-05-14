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
        private static string dataSourceKeywordStr;//KeyWordDataSource
        private static string connStr;//connectStr
        private static string connKeywordStr;//connectKeywordStr
        private static ThreadLocal<OleDbConnection> tlConn = new ThreadLocal<OleDbConnection>();
        private static ThreadLocal<OleDbConnection> keywordConn = new ThreadLocal<OleDbConnection>();

        //初始化
        static OLDBHelper() {
            driverNameStr = ConfigurationManager.AppSettings["DriverNameStr"];
            
            persistSecurityStr = ConfigurationManager.AppSettings["PersistSecurityStr"];

            dataSourceStr = ConfigurationManager.AppSettings["DataSourceStr"];
            dataSourceKeywordStr = ConfigurationManager.AppSettings["DataSourceStrKeyword"];
            connStr = driverNameStr + persistSecurityStr + dataSourceStr;
            connKeywordStr= driverNameStr + persistSecurityStr + dataSourceKeywordStr;
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
            OleDbConnection conn = tlConn.Value;//库连接
            OleDbConnection conn2 = keywordConn.Value;
            
            if (conn!=null && conn.State.Equals(ConnectionState.Open))
            {
                conn.Close();
            }

            if (conn2 != null && conn2.State.Equals(ConnectionState.Open))
            {
                conn2.Close();
            }

        }

        public static OleDbConnection GetKeywordConnection()
        {
            OleDbConnection conn = keywordConn.Value;//数据库连接

            if (conn == null)
            {
                conn = new OleDbConnection(connKeywordStr);
                keywordConn.Value = conn;
            }

            return conn;
        }



        //释放数据库连接的资源
        private void DisposeDB()
        {
            OleDbConnection conn = tlConn.Value;//数据库连接
            OleDbConnection conn2 = keywordConn.Value;
            if (conn != null)
            {
                conn.Dispose();
                conn = null;
            }

            if (conn2 != null)
            {
                conn2.Dispose();
                conn2 = null;
            }
        }

        ~OLDBHelper()
        {
            
            DisposeDB();
        }


    }
}
