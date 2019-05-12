using NUnit.Framework;
using System;
using System.Data.OleDb;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllocateTool.Entity;
using AllocateTool.dao;
using AllocateTool.utils;

namespace AllocateTool
{
    [TestFixture]
    public class TestEmpDAO
    {
        [Test]
        public void TestAddEmpItemDAO()
        {
            
            Emp emp = new Emp();
            emp.M_name = "emp\" )'01";
            emp.M_statue = "0";
            emp.M_title = "TM";
            
            EmpDAO empDao = new EmpDAO();
            OleDbConnection con = empDao.Begin();
            empDao.AddEmpItemDAO(con, emp);
            empDao.Commit();
            empDao.Close();
        }

        [Test]
        public void TestFindEmpByIDDAO()
        {
           
            EmpDAO empDao = new EmpDAO();
            OleDbConnection con = empDao.Begin();
            Emp emp=empDao.FindEmpByIdDAO(con, 5);
            Assert.AreEqual(emp.M_leaderName, "gray.piao");
            empDao.Commit();
            empDao.Close();
        }

        [Test]
        public void TestUpdateEmpItemDAO()
        {
            EmpDAO empDao = new EmpDAO();
            OleDbConnection con = empDao.Begin();
            Emp emp=empDao.FindEmpByIdDAO(con,1);
      
            empDao.UpdateEmpItemDAO(con, emp);
            empDao.Commit();
            empDao.Close();
        }

    }
}
