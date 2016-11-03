// Skeleton implementation written by Joe Zachary for CS 3500, January 2015.
// Revised for CS 3500 by Joe Zachary, January 29, 2016

//Completed by Rizwan Mohammed, CS 3500
// 2/5/2016
using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if .
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    /// 
    ///    (t are the dependents, the second set are known as the dependents)
    ///     notation = dependents(s) returns t
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    /// 
    ///    (s is the dependees, the first set is known as the dependees)
    ///    notation = dependees(t) will return s?
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<String, HashSet<String>> Dependents; //Links Dependents to Dependees
        private Dictionary<String, HashSet<String>> Dependees;  //Links dependees to Depdents
        private int size; // keeps track of size of the DependencyGrap

        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// </summary>
        public DependencyGraph()
        {
            Dependents = new Dictionary<string, HashSet<string>>();
            Dependees = new Dictionary<string, HashSet<string>>();
            size = 0;
        }

        /// <summary>
        /// Creates a copy of a Dependency graph
        /// </summary>
        /// <param name="graph"></param>
        public DependencyGraph(DependencyGraph graph)
        {
            Dictionary<String, HashSet<String>> DependentCopy;
            Dictionary<String, HashSet<String>> DependeeCopy;
            int sizeCopy;

            DependencyGraph newDG = new DependencyGraph();
            this.Dependees = newDG.Dependees;
            this.Dependents = newDG.Dependents;
            this.size = newDG.size;


            HashSet<String> temp = new HashSet<string>();



            DependeeCopy = new Dictionary<string, HashSet<string>>(graph.Dependees);
            DependentCopy = new Dictionary<string, HashSet<string>>(graph.Dependents);
            sizeCopy = graph.size;

            foreach (var item in DependeeCopy.Keys)
            {
                DependeeCopy.TryGetValue(item, out temp);
                foreach (var item2 in temp)
                    this.AddDependency(item, item2);
            }

            foreach (var item in DependentCopy.Keys)
            {
                DependentCopy.TryGetValue(item, out temp);
                foreach (var item2 in temp)
                    this.AddDependency(item2, item);
            }




        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// Throws a null exception if the parameters are null
        /// </summary>
        public bool HasDependents(string s)
        {
            if (ReferenceEquals(s, null))
                throw new ArgumentNullException();

            HashSet<String> temp;
            Dependees.TryGetValue(s, out temp);

            //If the string does not link to a hashset, there are no dependents.
            if (ReferenceEquals(temp, null))
            {
                return false;
            }

            if (temp.Count > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// Throws a null exception if the parameters are null
        /// </summary>
        public bool HasDependees(string s)
        {
            if (ReferenceEquals(s, null))
                throw new ArgumentNullException();

            HashSet<String> temp;
            Dependents.TryGetValue(s, out temp);

            //If the String does ont link to a hashset, there are no dependees
            if (ReferenceEquals(temp, null))
            {
                return false;

            }

            if (temp.Count > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// Throws a null exception if the parameters are null
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (ReferenceEquals(s, null))
                throw new ArgumentNullException();

            HashSet<String> temp;
            Dependees.TryGetValue(s, out temp);

            //If there is no Hashset, then it links to an empty set
            if (ReferenceEquals(temp, null))
            {
                HashSet<String> EmptySet = new HashSet<string>();
                return EmptySet;

            }

            //protect our data by giving a copy, instead of the original data structure
            String[] arr = new String[temp.Count];
            temp.CopyTo(arr);




            return arr;
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// Throws a null exception if the parameter is null
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (ReferenceEquals(s, null))
                throw new ArgumentNullException();

            HashSet<String> temp;
            Dependents.TryGetValue(s, out temp);

            //If there is no hashset, then it links to an empty set
            if (ReferenceEquals(temp, null))
            {
                HashSet<String> EmptySet = new HashSet<string>();
                return EmptySet;

            }

            //protect data by giving a copy
            String[] arr = new String[temp.Count];
            temp.CopyTo(arr);

            return arr;
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Throws a null exception if the parameters are null
        /// </summary>
        public void AddDependency(string s, string t)
        {
            //throws a null exception if the paramters are null
            if (ReferenceEquals(s, null) || ReferenceEquals(t, null))
                throw new ArgumentNullException();

            //If it already exists in the dictionary
            if (Dependees.ContainsKey(s))
            {
                //pull ou the Hashset associated with it
                HashSet<String> temp;
                Dependees.TryGetValue(s, out temp);

                //If it already in it, do nothing
                if (temp.Contains(t))
                    return;

                //If somehow it didn't create the link both ways, safety measure here.
                else if (ReferenceEquals(temp, null))
                {
                    temp = new HashSet<String>();
                    temp.Add(t);
                    Dependees.Add(s, temp);
                    return;
                }

                //Add it to the existing set
                else
                {
                    temp.Add(t);
                    size++;

                    //checks to see if the other way is true as well. Add it if it doesn't exist
                    if (Dependents.ContainsKey(t))
                    {
                        HashSet<String> blah;
                        Dependents.TryGetValue(t, out blah);
                        blah.Add(s);
                        return;
                    }

                    //The set the opposite way doesn't exist, create it
                    else
                    {
                        HashSet<String> blah = new HashSet<string>();
                        blah.Add(s);
                        Dependents.Add(t, blah);
                        return;

                    }

                }
            }

            //Checks if the dependent parts contains t
            else if (Dependents.ContainsKey(t))
            {
                //pull out the hashset
                HashSet<String> temp;
                Dependents.TryGetValue(t, out temp);

                //if its in there, do nothing
                if (temp.Contains(s))
                    return;

                //If it doesn't exist, create it
                else if (ReferenceEquals(temp, null))
                {
                    temp = new HashSet<string>();
                    temp.Add(s);
                    Dependees.Add(t, temp);
                }

                //It exists but its not in there
                else
                {
                    temp.Add(s);
                    size++;


                    //Checks to see if the other way is true, adds it t
                    if (Dependees.ContainsKey(s))
                    {
                        HashSet<String> blah;
                        Dependents.TryGetValue(s, out blah);
                        blah.Add(t);
                        return;
                    }

                    //if no hashset associated with that value, create it
                    else
                    {
                        HashSet<String> blah = new HashSet<string>();
                        blah.Add(t);
                        Dependees.Add(s, blah);
                        return;

                    }
                }

            }

            //Both items, either way, dont exist, update both dictionaries
            else
            {
                HashSet<String> dependentList = new HashSet<String>();
                dependentList.Add(t);
                Dependees.Add(s, dependentList);

                HashSet<String> dependeesList = new HashSet<string>();
                dependeesList.Add(s);
                Dependents.Add(t, dependeesList);
                size++;
            }

            return;


        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// throws a null exception if the parameters are null.
        /// </summary>
        public void RemoveDependency(string s, string t)
        {

            //null check
            if (ReferenceEquals(s, null) || ReferenceEquals(t, null))
                throw new ArgumentNullException();

            //If it contains that dependency, remove it
            if (Dependees.ContainsKey(s))
            {
                HashSet<String> temp;
                Dependees.TryGetValue(s, out temp);

                if (temp.Contains(t))
                    temp.Remove(t);
            }

            //There is on such dependency, do nothing
            else
            {
                return;
            }

            //Remove it from both dictionaries
            if (Dependents.ContainsKey(t))
            {
                HashSet<String> temp;
                Dependents.TryGetValue(t, out temp);

                if (temp.Contains(s))
                    temp.Remove(s);

                else
                {
                    return;
                }
            }

            //update the size
            size--;
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Throws a null exception if the parameters are null
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (ReferenceEquals(s, null) || ReferenceEquals(newDependents, null))
                throw new ArgumentNullException();

            HashSet<String> temp;


            if (Dependees.TryGetValue(s, out temp))
            {
                //cant iterate a modified list
                String[] arr = new String[temp.Count];
                temp.CopyTo(arr);

                //remove as appropriate
                foreach (String item in arr)
                {
                    RemoveDependency(s, item);
                }
            }

            //add as appropriate
            foreach (String item in newDependents)
            {
                AddDependency(s, item);
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Throws a null exception if the parameters are null
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (ReferenceEquals(t, null) || ReferenceEquals(newDependees, null))
                throw new ArgumentNullException();

            HashSet<String> temp;
            if (Dependents.TryGetValue(t, out temp))
            {
                //Cant iterate a modified set
                String[] arr = new String[temp.Count];
                temp.CopyTo(arr);

                //remove as appropriate
                foreach (String item in arr)
                {
                    RemoveDependency(item, t);
                }
            }

            //add as appropriate
            foreach (String item in newDependees)
            {
                AddDependency(item, t);
            }
        }
    }
}