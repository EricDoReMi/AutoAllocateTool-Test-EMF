using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllocateTool.control;

namespace AllocateTool
{
    [TestFixture]
    public class TestMailControl
    {
        [Test]
        public void TestFindCouldNotAllocateEmpsMethod()
        {
            MailControl mailControl = new MailControl();

            mailControl.TransferMail();
            
        }
    }
}
