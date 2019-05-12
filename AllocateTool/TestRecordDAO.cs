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
    public class TestRecordDAO
    {
        [Test]
        public void TestAddRecordItemDAO()
        {
            Record record = new Record();
            record.UserName = "ShiMao";
            RecordDAO  recordDao = new RecordDAO();
            OleDbConnection con = recordDao.Begin();
            recordDao.AddRecordItemDAO(con, record);
            
            recordDao.Commit();
            recordDao.Close();

        }

        [Test]
        public void TestUpdateRecordItemDAO()
        {
            Record record = new Record();
            record.UserName = "ShiMao";
            record.M_id = 2;
            RecordDAO recordDao = new RecordDAO();
            OleDbConnection con = recordDao.Begin();
            recordDao.UpdateRecordItemDAO(con, record);

            recordDao.Commit();
            recordDao.Close();
        }

        [Test]
        public void TestFindRecordsToAllocateDAO()
        {
           
            RecordDAO recordDao = new RecordDAO();
            OleDbConnection con = recordDao.Begin();
            List<Record> records=recordDao.FindRecordsToAllocateDAO(con);

            recordDao.Commit();
            recordDao.Close();
        }

        

         [Test]
        public void TestFindRecordByRequestIDDAO()
        {

            RecordDAO recordDao = new RecordDAO();
            OleDbConnection con = recordDao.Begin();
            int? id = recordDao.FindRecordByRequestIDDAO(con, "Consol - C1900007772");

            recordDao.Commit();
            recordDao.Close();
        }


    }
}
