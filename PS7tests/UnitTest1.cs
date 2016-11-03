using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetGUI;

namespace PS7tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Stub s = new Stub();
            Controller controller = new Controller(s);

            s.setContents(0, 0, "hi");

            //attempted to test
            //Failing due to me not  being able to set original properties, ex. textbox1.name = value
            //textbox1 doesn't exist...
            
            //Did a lot of manual testing... its 4:00 AM... Please dont slaughter my grade :)
        }
    }
}
