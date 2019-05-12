using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using AllocateTool.Entity;
using AllocateTool.utils;

namespace AllocateTool.dao
{
    public partial class EmpDAO : BaseDAO<Emp>
    {
        /**//// <summary>   
            /// 根据id查找对应员工
            /// </summary>   
            /// <param name="conn">数据库连接</param> 
            /// <param name="id">Emp的ID</param>   
            /// <returns>OleDbDataAdapter</returns>   
        public Emp FindEmpByIdDAO(OleDbConnection conn,int id) {

            string sqlStr = @"SELECT e1.m_id as m_id,e1.m_name as m_name,e1.m_title as m_title,e1.m_login as m_login,e1.m_statue as m_statue,e1.m_mgid as m_mgid,e2.m_name as m_leaderName,e1.m_station as m_station,e1.m_keyword as m_keyword FROM emps e1 left join emps e2 on e1.m_mgid=e2.m_id WHERE e1.m_id=@M_id;";
            OleDbParameter[] paras = { new OleDbParameter("@M_id", id)};
            List<Emp> list = queryEntity(conn, sqlStr, paras);

            if (list.Count() > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        
        public void AddEmpItemDAO(OleDbConnection conn, Emp emp)
        {

            string sqlStr = @"INSERT INTO emps(m_name,m_title,m_login,m_statue,m_mgid,m_station,m_keyword) VALUES(@M_Name,@M_title,@M_login,@M_statue,@M_mgid,@M_station,@M_keyword);";
            OleDbParameter[] paras = emp.ToInsertByParamArray();
            ExecuteSQLNonquery(conn, sqlStr, paras);

        }

        public void UpdateEmpItemDAO(OleDbConnection conn, Emp emp) {
            string sqlStr = @"UPDATE emps SET m_name=@M_name,m_title=@M_title,m_login=@M_login,m_statue=@M_statue,m_mgid=@M_mgid,m_station=@M_station,m_keyword=@M_keyword WHERE m_id=@M_id";
            OleDbParameter[] paras = emp.ToUpdateByParamArray();
            ExecuteSQLNonquery(conn, sqlStr, paras);
        }

       

        public List<Emp> FindAllEmpDAO(OleDbConnection conn) {
            string sqlStr = @"SELECT e1.m_id as m_id,e1.m_name as m_name,e1.m_title as m_title,e1.m_login as m_login,e1.m_statue as m_statue,e1.m_mgid as m_mgid,e2.m_name as m_leaderName,e1.m_station as m_station,e1.m_keyword as m_keyword FROM emps e1 left join emps e2 on e1.m_mgid=e2.m_id;";
            List<Emp> list = queryEntity(conn, sqlStr);
            return list;
        }

        /// <summary>
        /// 寻找可以被自动分配case的员工
        /// </summary>
        /// <param name="conn"></param>
        /// <returns>符合条件的员工List</returns>
        public List<Emp> FindEmpsToAllocateDAO(OleDbConnection conn)
        {
            string sqlStr = @"SELECT * FROM emps WHERE m_login='1' AND m_statue='1' ORDER BY m_id;";
            List<Emp> list = queryEntity(conn, sqlStr);
            return list;
        }

        public void DeleteEmpByIdDAO(OleDbConnection conn, int id) {
            string sqlStr = @"DELETE * FROM emps WHERE m_id=@M_id";
            OleDbParameter[] paras = new OleDbParameter[] {new OleDbParameter("@M_id", id) };
            ExecuteSQLNonquery(conn, sqlStr, paras);
        }



        public override Emp ToEntity(OleDbDataReader reader)
        {
                Emp emp = new Emp();
                EntityUtils.FillEntity<Emp>(reader,emp);
                return emp;
            }
        }
}
