using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dependencies;
using System.Collections.Generic;

namespace DependencyGraphTestCases
{
    [TestClass]
    public class DGUnittests
    {
        /// <summary>
        /// checks size
        /// </summary>
        [TestMethod]
       
        public void Size()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            Assert.AreEqual(1, dg.Size);
        }

        //remove and check the size
        [TestMethod]
        public void Dependees()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.RemoveDependency("a", "b");

            Assert.AreEqual(0, dg.Size);
            
        }

        /// <summary>
        /// Checks HasDependents
        /// </summary>
        [TestMethod]
        public void HasDependentsTest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.HasDependees("b");

            Assert.IsTrue(dg.HasDependents("a"));
        }

        /// <summary>
        /// Checks hasdependees
        /// </summary>
        [TestMethod]
        public void HasDependeestest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.HasDependees("b");

            Assert.IsTrue(dg.HasDependees("b"));
        }

        /// <summary>
        /// Second dependee null test
        /// </summary>
        [TestMethod]
        public void HasDependeestest2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, null);
           

            Assert.IsFalse(dg.HasDependees("b"));
        }

        /// <summary>
        /// HasDependent test
        /// </summary>
        [TestMethod]
        public void HasDependentsTest2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.HasDependees("b");

            Assert.IsFalse(dg.HasDependents("c"));
        }

        /// <summary>
        /// null test
        /// </summary>
        [TestMethod]
        public void HasDependentsTest3()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, null);

            Assert.IsFalse(dg.HasDependents("c"));
        }

        /// <summary>
        /// Checks to see if GetDependees is working correctly
        /// </summary>
        [TestMethod]
        public void contains()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            IEnumerable<String> a = dg.GetDependees("b");

            a.GetEnumerator().MoveNext().Equals("a");

            
        }

        /// <summary>
        /// Returns empty set as approrpiate for an item that does not exist
        /// </summary>

        [TestMethod]
        public void contains3()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            IEnumerable<String> a = dg.GetDependees("c");

            a.GetEnumerator().MoveNext().Equals("");


        }

        /// <summary>
        /// Contains test
        /// </summary>
        [TestMethod]
        public void contains2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            IEnumerable<String> a = dg.GetDependents("a");

            a.GetEnumerator().MoveNext().Equals("b");


        }

        /// <summary>
        /// Dependent empty test
        /// </summary>
        [TestMethod]
        public void contains4()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            IEnumerable<String> a = dg.GetDependents("c");

            a.GetEnumerator().MoveNext().Equals("");


        }

        /// <summary>
        /// Replace test
        /// </summary>
        [TestMethod]
        public void add()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "d");

            dg.ReplaceDependees("b", new HashSet<string>());
            dg.ReplaceDependents("a", new HashSet<string>());


        }

        /// <summary>
        /// Replace and empty tests
        /// </summary>
        [TestMethod]
        public void add2()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency("", "");
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("b", "d");

            dg.ReplaceDependees("", new HashSet<string>() { "a", "b", "c" });
            dg.ReplaceDependents("a", new HashSet<string>() { "d", "e", "f" });


        }

        /// <summary>
        /// Null test
        /// </summary>
        [TestMethod]
        public void nulltest()
        {
            DependencyGraph dg = new DependencyGraph();
            dg.AddDependency(null, null);
            dg.RemoveDependency(null, null);


      //      dg.ReplaceDependees("", new HashSet<string>() { "a", "b", "c" });
        //    dg.ReplaceDependents("a", new HashSet<string>() { "d", "e", "f" });


        }


    }
}
