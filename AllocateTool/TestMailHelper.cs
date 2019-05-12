using NUnit.Framework;
using Outlook = Microsoft.Office.Interop.Outlook;
using AllocateTool.utils;

namespace AllocateTool
{
    [TestFixture]
    public class TestMailHelper
    {
        [Test]
        public void TestMailHelperInit()
        {
            foreach (Outlook.MailItem myItem in MailHelper.myFolderInbox.Items)
            {

                myItem.SaveAs(MailHelper.MailFolder + myItem.SenderName + ".msg", Microsoft.Office.Interop.Outlook.OlInspectorClose.olPromptForSave);
            }
        }
    }
}
