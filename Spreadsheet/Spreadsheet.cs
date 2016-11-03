using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Formulas;
using System.Text.RegularExpressions;
using Dependencies;
using System.IO;
using System.Xml;

//modified by Rizwan Mohammed for CS3500 2/26/2016
//u0746436

namespace SS
{
    // MODIFIED PARAGRAPHS 1-3 AND ADDED PARAGRAPH 4 FOR PS6
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of a regular expression (called IsValid below) and an infinite 
    /// number of named cells.
    /// 
    /// A string is a valid cell name if and only if (1) s consists of one or more letters, 
    /// followed by a non-zero digit, followed by zero or more digits AND (2) the C#
    /// expression IsValid.IsMatch(s.ToUpper()) is true.
    /// 
    /// For example, "A15", "a15", "XY32", and "BC7" are valid cell names, so long as they also
    /// are accepted by IsValid.  On the other hand, "Z", "X07", and "hello" are not valid cell 
    /// names, regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized by converting all letters to upper case before it is used by this 
    /// this spreadsheet.  For example, the Formula "x3+a5" should be normalize to "X3+A5" before 
    /// use.  Similarly, all cell names and Formulas that are returned or written to a file must also
    /// be normalized.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important, and it is important that you understand the distinction and use
    /// the right term when writing code, writing comments, and asking questions.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In an empty spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError.
    /// The value of a Formula, of course, can depend on the values of variables.  The value 
    /// of a Formula variable is the value of the spreadsheet cell it names (if that cell's 
    /// value is a double) or is undefined (otherwise).  If a Formula depends on an undefined
    /// variable or on a division by zero, its value is a FormulaError.  Otherwise, its value
    /// is a double, as specified in Formula.Evaluate.
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
       private Dictionary<String, Cell> NametoCell;
       private DependencyGraph graph;
       private Regex Isvalid;

        // ADDED FOR PS6
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get;

            protected set;

        }

        /// Creates an empty Spreadsheet whose IsValid regular expression accepts every string.
        public Spreadsheet()
        {
            graph = new DependencyGraph();
            NametoCell = new Dictionary<string, Cell>();
            Isvalid = new Regex(@".*");
                
            

        }

        /// Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        public Spreadsheet(Regex isValid)
        {
            graph = new DependencyGraph();
            NametoCell = new Dictionary<string, Cell>();
            Isvalid = isValid;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  If there's a problem reading source, throws an IOException
        /// If the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  If there is an invalid cell name, or a 
        /// duplicate cell name, or an invalid formula in the source, throws a SpreadsheetReadException.
        /// If there's a Formula that causes a circular dependency, throws a SpreadsheetReadException. 
        public Spreadsheet(TextReader source)
        {
            //Create necessary variables
            String name = "";
            String contents = "";
            graph = new DependencyGraph();
            NametoCell = new Dictionary<string, Cell>();

            //Use XMLreader to read the file
            try
            {
                using (XmlReader reader = XmlReader.Create(source))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "spreadsheet":
                                    //Gets the first attribute of the spreadsheet (regex)
                                    Isvalid = new Regex(reader.GetAttribute(0));
                                    break;

                                    //Get the first attribute of cell (name)
                                case "cell":
                                    name = reader.GetAttribute(0);
                                    if (!ValidName(name))
                                        throw new SpreadsheetReadException("Not a valid name");
                                    goto case "contents";

                                    //Get second attribute of cell (content)
                                case "contents":
                                    contents = reader.GetAttribute(1);
                                    this.SetContentsOfCell(name, contents);
                                    break;

                            }
                        }

                    }
                }
            }

            //Catches any problems reading a file
            catch (IOException e)
            {
                throw new IOException("Problem reading the Spreadsheet!");
            }



        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if (ReferenceEquals(null, name) || ValidName(name) == false)
                throw new InvalidNameException();

            name = name.ToUpper();
            Cell temp;
            //Check if the cell exists in our dictionary
            NametoCell.TryGetValue(name, out temp);

            //the contents of every cell is the empty string.
            if (ReferenceEquals(temp, null))
                return "";

            return temp.content;
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<String> list = new List<string>();
            Cell temp;

            foreach (String item in NametoCell.Keys)
            {
                NametoCell.TryGetValue(item, out temp);

                // "" is considered an empty cell
                if (temp.content.Equals(""))
                    continue;
                else
                    list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// If formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (ReferenceEquals(formula, null))
                throw new ArgumentNullException();

            if (ReferenceEquals(name, null) || ValidName(name) == false)
                throw new InvalidNameException();

            Cell temp;
            Object revertFormula;
            Cell cell;


            //Retreieve the cell if it exists
            NametoCell.TryGetValue(name, out temp);

            //if it doesn't, add it
            if (ReferenceEquals(temp, null))
            {
                cell = new Cell(name, formula);
                NametoCell.Add(name, cell);
                revertFormula = "";


            }
            // Else change the content
            else
            {
                revertFormula = temp.content;
                temp.content = formula;
            }

            //Get dependencies in the formula
            IEnumerable<String> vars = formula.GetVariables();

            foreach (String variable in vars)
            {
                graph.AddDependency(variable, name);
            }

            IEnumerable<String> templist = null;

            //find all of the cells we need to recalculate based off of our change
            try
            {
                templist = GetCellsToRecalculate(name);

            }
            catch (CircularException e)
            {
                if (ReferenceEquals(temp, null))
                    cell = null;
                else
                    temp.content = revertFormula;

                foreach (var item in vars)
                {
                    graph.RemoveDependency(item, name);
                }

                throw e;
            }

            HashSet<String> list = new HashSet<string>();

            foreach (var item in templist)
            {
                list.Add(item);
            }


            return list;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (ReferenceEquals(text, null))
                throw new ArgumentNullException();

            if (ReferenceEquals(name, null) || ValidName(name) == false)
                throw new InvalidNameException();

            Cell temp;

            //Retrieve cell
            NametoCell.TryGetValue(name, out temp);

            //Create the cell if it doesn't exist
            if (ReferenceEquals(temp, null))
            {
                Cell cell = new Cell(name, text);
                NametoCell.Add(name, cell);
            }
            //Else change the content
            else
            {
                temp.content = text;
            }


            //Get all the cells we need to change
            IEnumerable<String> templist = GetCellsToRecalculate(name);
            HashSet<String> list = new HashSet<string>();

            //Add it to the list
            foreach (var item in templist)
            {
                list.Add(item);
            }


            return list;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (ReferenceEquals(name, null) || ValidName(name) == false)
                throw new InvalidNameException();

            Cell temp;
            //Retrieve cell if it exists
            NametoCell.TryGetValue(name, out temp);

            //Create it if it doens't
            if (ReferenceEquals(temp, null))
            {
                Cell cell = new Cell(name, number);
                NametoCell.Add(name, cell);
            }
            //Else set its content
            else
            {
                temp.content = number;
            }

            //Get all of the cells that need to be recalculated
            IEnumerable<String> templist = GetCellsToRecalculate(name);
            HashSet<String> list = new HashSet<string>();

            foreach (var item in templist)
            {
                list.Add(item);
            }


            return list;
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if (ReferenceEquals(name, null))
                throw new ArgumentNullException();

            if (ValidName(name) == false)
                throw new InvalidNameException();

            //Returns the direct dependents of the cell
            return graph.GetDependents(name);
        }

        /// A string is a cell name if and only if it consists of one or more letters, 
        /// followed by a non-zero digit, followed by zero or more digits.  Cell names
        /// are not case sensitive.
        /// 
        /// For example, "A15", "a15", "XY32", and "BC7" are cell names.  (Note that 
        /// "A15" and "a15" name the same cell.)  On the other hand, "Z", "X07", and 
        /// "hello" are not cell names."
        private bool ValidName(String name)
        {
            String pattern = Isvalid.ToString(); //"^([a-zA-Z]+)[1-9]\\d*$";
            bool isValid = Regex.IsMatch(name, pattern);

            return isValid;
        }

        // Element, Attibute = AttributeString, Attribute = AttributeString
        // ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the isvalid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            Cell cell;
            //Uses an XML writer to write a file to a location
            try
            {
                using (XmlWriter writer = XmlWriter.Create(dest))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("IsValid", Isvalid.ToString());

                    foreach (String item in NametoCell.Keys)
                    {
                        writer.WriteStartElement("cell");

                        writer.WriteAttributeString("name", item);

                        NametoCell.TryGetValue(item, out cell);

                        if (cell.content is Formula)
                        {
                            writer.WriteAttributeString("contents", "=" + cell.content);

                        }
                        else
                        {
                            writer.WriteAttributeString("contents", cell.content.ToString());

                        }



                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                    writer.WriteEndDocument();

                }

                this.Changed = false;
            }

            catch(Exception e)
            {
                throw new IOException();
            }

        }

        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            if (ReferenceEquals(name, null) || !ValidName(name))
                throw new InvalidNameException();

            Cell cell;
            double d;
            name = name.ToUpper();
            NametoCell.TryGetValue(name, out cell);

            if (ReferenceEquals(cell, null))
                return "";

            if (cell.content is Formula)
            {
                try
                {
                     d = new Formula(cell.content.ToString()).Evaluate(lookup);
                }
                catch(Exception e)
                {
                    return new FormulaError("Variable doesn't exist");

                }

                if (d == -1)
                    return new FormulaError("Variable does not exist");

                return d; //just in case something weird happens
            }

            else
                return cell.content;
            
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {

            if (ReferenceEquals(content, null))
                throw new ArgumentNullException();

            if (ReferenceEquals(name, null) || !ValidName(name))
                throw new InvalidNameException();


            double ifDouble;
            char[] charsToTrim = { '=' };
            Formula f;

            //sets the name to the normalized version
            name = name.ToUpper();
            Changed = true;



            //if we have a double
            if (double.TryParse(content, out ifDouble) == true)
            {
                return SetCellContents(name, ifDouble);        
            }

            //if we have a formula
            if (content.StartsWith("="))
            {
               String temp = content.Trim(charsToTrim);
               f = new Formula(temp.ToString(), s => s.ToUpper(), ValidName);
                return SetCellContents(name, f);
            }

            //if we have text
            return SetCellContents(name, content);

        }

        /// <summary>
        /// Private method that looks up the values of variables in the Spreadsheet
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private double lookup(String s)
        {
            Formula f;
            
            Cell temp;
            
            double result = 0;
            NametoCell.TryGetValue(s, out temp);

            if (temp.content is double)
                return (double)temp.content;
            else
            {
                f = new Formula(temp.content.ToString());
                foreach (String item in f.GetVariables())
                {
                    
                    result = f.Evaluate(lookup);
                   // result += lookup(item);
                   

                }
            }

            return result;



        }

    }
}