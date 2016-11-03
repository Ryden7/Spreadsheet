using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

namespace PS6tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hi");
            s.SetContentsOfCell("A1", "2+2");
            s.SetContentsOfCell("A1", "=2+2");

        }

        [TestMethod]
        public void TestMethod2()
        {

            Regex pattern = new Regex("^([a-zA-Z]+)[1-9]\\d*$");
            Spreadsheet s = new Spreadsheet(pattern);

        }

        [TestMethod]
        public void TestMethod3()
        {

            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hi");

            Assert.AreEqual("hi", s.GetCellContents("A1"));

        }

        [TestMethod]
        public void TestMethod4()
        {

            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hi");

            IEnumerable<String> a = s.GetNamesOfAllNonemptyCells();
            List<String> list = new List<String>();


            foreach (String item in a)
            {
                list.Add(item);
            }

            Assert.IsTrue(list.Contains("A1"));




        }

        [TestMethod]
        public void TestMethod5()
        {

            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hi");
            s.SetContentsOfCell("B1", "5");
            s.SetContentsOfCell("C1", "=2+2");
            TextWriter a = File.CreateText("C:\\Users\\Rizwan_Laptop\\Desktop\\tettx.txt");
            s.Save(a);


        }


        [TestMethod]
        public void TestMethod6()
        {

            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("B1", "=a1+2");
            Assert.ReferenceEquals(new FormulaError(), s.GetCellValue("B1"));

        }

        [TestMethod]
        public void TestMethod7()
        {
            TextReader a = File.OpenText("C:\\Users\\Rizwan_Laptop\\Desktop\\test.txt");
            Spreadsheet s = new Spreadsheet(a);
        }

        [TestMethod]
        public void TestMethod8()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hi");
            Assert.IsTrue(s.Changed);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod9()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "hi");
            s.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void nulltest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellContents("ayylmao");
        }

        [TestMethod]
        public void emptytest()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetNamesOfAllNonemptyCells();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]

        public void nulltest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]

        public void InvalidnameTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("uaweoirjoiew", "=2+2");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void CircularExceptionTest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "=B1+2");
            s.SetContentsOfCell("B1", "=C1+2");
            s.SetContentsOfCell("C1", "=A1+2");

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InvalidNameTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void InvalidNameTest3()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell(null, "textest");
        }


        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void numTest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("Thisisnotavalidname", "5.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void nulltest4()
        {
            Spreadsheet s = new Spreadsheet();
            s.GetCellValue(null);
        }

        [TestMethod]
        public void valuetest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "5.0");
            Assert.AreEqual(5.0, s.GetCellValue("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadException))]
        public void Filetest()
        {
            TextReader a = File.OpenText("C:\\Users\\Rizwan_Laptop\\Desktop\\tett.txt");
            Spreadsheet s = new Spreadsheet(a);

        }

        [TestMethod]
        public void formulatest()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "2");
            s.SetContentsOfCell("B1", "=A1+3");

        }

        [TestMethod]
        public void formulatest2()
        {
            Spreadsheet s = new Spreadsheet();
            s.SetContentsOfCell("A1", "sup");
            s.GetCellValue("C1");

        }


    }
}

