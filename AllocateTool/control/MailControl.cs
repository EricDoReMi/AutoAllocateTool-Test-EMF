using Outlook = Microsoft.Office.Interop.Outlook;
using System.Configuration;
using AllocateTool.dao;
using AllocateTool.Entity;
using AllocateTool.utils;
using System.Data.OleDb;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AllocateTool.AllocateToolException;

namespace AllocateTool.control
{
    public partial class  MailControl
    {
        private EmpDAO empDao = new EmpDAO();
        private RecordDAO recordDao = new RecordDAO();
        
        private static long mailNum = 0;//用于标记邮件个数,存储邮件时作为唯一标识
        /// <summary>
        /// 将邮件内容存放到数据库中
        /// </summary>
        public void TransferMail(){

             if (MailHelper.myFolderInbox.Items.Count > 0)
                {
                    
                    
                    foreach (Outlook.MailItem myItem in MailHelper.myFolderInbox.Items)
                    {
                       
                        string mailAddress = myItem.SenderEmailAddress.Trim();
                        string saveMailItemPath; //将邮件存到公共盘的路径  
                    //若是系统发来的邮件就屏蔽
                    if (!Regex.IsMatch(mailAddress.ToUpper(), @"G\.SERVICENOW|REQUESTIT\.NO-REPLY|NEW\.SERVICE|REPLY"))

                    {

                        //ForwardMailItem(myItem);//向服务器邮箱发送邮件
                        AddMailToDB(myItem,out saveMailItemPath);//将接收到的邮件信息添加到数据库
                        SaveMailItemToDisk(myItem,saveMailItemPath);//将邮件保存到公共盘
                        myItem.Move(MailHelper.mySourceFolder);//将邮件移动到了Source文件夹
                                                              
                    }

                    else
                    {
                        myItem.Move(MailHelper.myOutFolder);
                    }
  
                    }
                }
            
        }

        /// <summary>
        /// 将request邮件转发到服务器邮箱,仅针对KR Team
        /// </summary>
        /// <param name="myItem"></param>
        private void ForwardMailItem(Outlook.MailItem myItem) {
            
                int importance = (int)myItem.Importance;
                Outlook.MailItem myForward = myItem.Forward();
                string ag = "OFR.GSCKRteam";
                //string sentNameMail = "new.servicedesk@dhl.com";//接收邮件的公邮地址
                string sentNameMail = "shimaorun@163.com";
                string iFlag = "";

                switch (importance)
                {
                    case 2:
                        iFlag = "Country";
                        break;
                    case 1:
                        iFlag = "Location";
                        break;
                    case 0:
                        iFlag = "User(s)";
                        break;
                }

                string newLine = System.Environment.NewLine;
                StringBuilder forwardBody = new StringBuilder();
                forwardBody.Append("Sender: ");
                forwardBody.Append(myItem.SenderEmailAddress);
                forwardBody.Append(newLine);
                forwardBody.Append("Assignment group: ");
                forwardBody.Append(ag);
                forwardBody.Append(newLine);
                forwardBody.Append("Impact:");
                forwardBody.Append(iFlag);
                forwardBody.Append(newLine);
                forwardBody.Append("Urgency: Normal");
                forwardBody.Append(newLine);
                forwardBody.Append("Impacted BU: DGF");
                forwardBody.Append(newLine);
                forwardBody.Append(newLine);
                forwardBody.Append(newLine);
                forwardBody.Append(newLine);
                forwardBody.Append("Original Message Below");
                forwardBody.Append(newLine);
                forwardBody.Append("_____________________________________________");
                forwardBody.Append(newLine);
                forwardBody.Append(newLine);
                forwardBody.Append(myForward.Body);
                myForward.Body = forwardBody.ToString();

                myForward.Recipients.Add(sentNameMail);
                myForward.Send();
    
        }

        /// <summary>
        /// 将邮件保存到公共盘
        /// </summary>
        /// <param name="myItem">要保存的邮件</param>
        /// <param name="mailSaveNameFilePath">保存邮件的完整路径</param>
        private void SaveMailItemToDisk(Outlook.MailItem myItem,string mailSaveNameFilePath) {
 
                myItem.SaveAs(mailSaveNameFilePath, Microsoft.Office.Interop.Outlook.OlInspectorClose.olPromptForSave);
           
        }

        
        /// <summary>
        /// 将邮件信息存储到数据库中
        /// </summary>
        /// <param name="myItem">邮件Item</param>
        /// <param name="saveMailNamePath">传出参数，传出要存储的路径</param>
        private void AddMailToDB(Outlook.MailItem myItem,out string saveMailNamePath) {
           
                Record record = new Record();
                record.M_subject = myItem.Subject.Trim();//邮件主题
                record.M_importance = ((int)myItem.Importance).ToString();
                record.M_sender = myItem.SenderName.Trim();

                record.M_mailincometime = Convert.ToDateTime(myItem.ReceivedTime.ToString("G"));
                record.JOBID = record.M_subject + "-" + DateTime.Now.ToString();
                record.T1 = Convert.ToDateTime(DateTime.Now.ToString("G"));

            //寻找关键字
            record.M_requestID = FindRequestKeyWord(record.M_subject);
                

                string mailBody = myItem.Body.Trim();
 

                record.M_statu = "0";
                int? preAsignEmpId = 0;
                
                

            string subjectStrGet = RegexHelper.ReplaceStrByRegex(@"RE:|FW:|回复：", record.M_subject, "");


                preAsignEmpId = FindRecordByRequest("%"+ subjectStrGet.Substring(0,subjectStrGet.Length)+"%");
         

                if (preAsignEmpId != null)
                {
                    record.M_statu = "4";
                    record.M_asign = preAsignEmpId;
                }
                else
                {
                    record.M_statue = "0";
                    record.M_asign = 0;
                }

           
           
                string saveMailName = record.M_requestID + "-" + new Random().GetHashCode().ToString() + myItem.ReceivedTime.ToString("yyyyMMddHHmmss") + mailNum.ToString();//邮件保存在公共盘上的名字

                saveMailNamePath = MailHelper.MailFolder + saveMailName + ".msg";
                record.M_filePath = saveMailNamePath;

               

                //将邮件记录添加到DB中
                AddRecordToDB(record);

               
            

        }


        /// <summary>
        /// 从subjectStr中匹配出关键字
        /// </summary>
        /// <param name="subjectStr"></param>
        /// <returns></returns>
        private string FindRequestKeyWord(string subjectStr) {
            List<Emp> emps = FindAllEmpsForAllocate();
            foreach (Emp emp in emps)
            {

                if (emp.M_keyword.Length>0 && subjectStr.ToUpper().Contains(emp.M_keyword.ToUpper())) {
                    return emp.M_keyword;
                }
                
            }
            return "";
        }



        private List<Emp> FindAllEmpsForAllocate() {
            List<Emp> emps = null;
            try
            {
                OleDbConnection con = empDao.Begin();
                emps = empDao.FindAllEmpDAO(con);
                empDao.Commit();
                return emps;
            }
            catch (Exception)
            {
                empDao.RollBack();
                throw;
            }
            finally
            {
                empDao.Close();
            }
            
        }

  

        /// <summary>
        ///按subjectStr寻找最近一次complete或incomplete的case，并且做这一票的Emp要在职，在线
        /// </summary>
        /// <param name="subjectStr"></param>
        /// <returns></returns>
        private int? FindRecordByRequest(string subjectStr){
            try
            {
                OleDbConnection con = recordDao.Begin();
                int? asignId=recordDao.FindRecordByRequestIDDAO(con, subjectStr);
                recordDao.Commit();
                return asignId;

            }
            catch (Exception)
            {
                recordDao.RollBack();
                throw;
            }
            finally
            {
                recordDao.Close();
            }

        }

        private void UpdateRecordToDB(Record record) {
            try
            {
                OleDbConnection con = recordDao.Begin();
                recordDao.UpdateRecordItemDAO(con, record);
                recordDao.Commit();

            }
            catch (Exception)
            {
                recordDao.RollBack();
                throw;
            }
            finally
            {
                recordDao.Close();
            }
        }



        private void BathUpdateRecordsToDB(List<Record> records)
        {
            try
            {
                OleDbConnection con = recordDao.Begin();
                foreach (Record record in records)
                {
                    recordDao.UpdateRecordItemDAO(con, record);
                }

                recordDao.Commit();

            }
            catch (Exception)
            {
                recordDao.RollBack();
                throw;
            }
            finally
            {
                recordDao.Close();
            }
        }

        private void BathUpdateEmpsToDB(List<Emp> emps)
        {
            try
            {
                OleDbConnection con = empDao.Begin();
                foreach (Emp emp in emps)
                {
                    empDao.UpdateEmpItemDAO(con, emp);
                }

                empDao.Commit();

            }
            catch (Exception)
            {
                empDao.RollBack();
                throw;
            }
            finally
            {
                empDao.Close();
            }
        }



        /// <summary>
        /// 向数据库添加单条Record的记录 
        /// </summary>
        /// <param name="record"></param>
        private void AddRecordToDB(Record record) {
            try
            {
                OleDbConnection con = recordDao.Begin();
                recordDao.AddRecordItemDAO(con,record);
                recordDao.Commit();
                
            }
            catch (Exception)
            {
                recordDao.RollBack();
                throw;
            }
            finally
            {
                recordDao.Close();
            }
        }

        private void BathAddRecordsToDB(List<Record> records) {
            try
            {
                OleDbConnection con = recordDao.Begin();
                foreach (Record record in records)
                {
                    recordDao.AddRecordItemDAO(con, record);
                }
                
                recordDao.Commit();

            }
            catch (Exception)
            {
                recordDao.RollBack();
                throw;
            }
            finally
            {
                recordDao.Close();
            }
        }

       

    }
}
