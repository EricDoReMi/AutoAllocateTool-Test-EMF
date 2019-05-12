using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllocateTool.utils;

namespace AllocateTool
{
    [TestFixture]
    public class TestRegexHelper
    {
        [Test]
        public void TestGetFirstStrByRegex()
        {
           
            string returnStr=RegexHelper.GetFirstStrByRegex(@"(shipment|consol){1}\s?[\-]+\s?[A-Z]{1}[0-9]+", "RE: Shipment - S1900442549");

            Assert.That(returnStr, Is.EqualTo("Shipment - S1900442549"), "GetFirstStrByRegex Failed");



        }
    }
}
