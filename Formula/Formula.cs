// Skeleton written by Joe Zachary for CS 3500, January 2015
// Revised by Joe Zachary, January 2016
// JLZ Repaired pair of mistakes, January 23, 2016

//Skeleton complted by Rizwan Mohammed for CS 3500, January 28th 2016
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        private int Lparencount;    //Left Parenthesis count
        private string lastToken;  //previous token
        private int Rparencount;    //Right parenthesis count
        private IEnumerable<string> stringTokens;   //container
        private bool currentvar;    //bool to see if the current item is a variable
        private string pattern;   //Variable pattern
        private bool parsed;    //Parse success bool
        private bool lastTokenVar;
        private string formulaHolder;




        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message..
        /// </summary>
        public Formula(String formula) : this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="norm"></param>
        /// <param name="val"></param>
        public Formula(String formula, Normalizer norm, Validator val)
        {

            this.Lparencount = 0;    //Left Parenthesis count
            this.lastToken = "";  //previous token
            this.Rparencount = 0;    //Right parenthesis count
            this.currentvar = false;    //bool to see if the current item is a variable
            this.pattern = @"[a-zA-Z][0-9a-zA-Z]*";   //Variable pattern
            this.lastTokenVar = false;
            this.parsed = false;
            this.formulaHolder = formula;
            String normalized = "";

            double result = 0;  //result of tryparse
            double first = 0;   //first token tryparse
            double last = 0;    //last token tryparse
            int flag = 0;   //rule flag



            stringTokens = GetTokens(formula);

            if (ReferenceEquals(formula, null))
                return;


            rule2(formula); //Rule helper method



            foreach (var item in stringTokens)
            {
                parsed = Double.TryParse(item.ToString(), out result);
                bool firstparse = Double.TryParse(stringTokens.First(), out first);
                Double.TryParse(stringTokens.Last(), out last);

                if (Regex.IsMatch(item, pattern) && result == 0)
                {
                    currentvar = true;
                    normalized = norm(item);

                    if (Regex.IsMatch(normalized, pattern) == true)
                    {
                        if (val(normalized) == true)
                        {
                            //  item = item.Replace(item, normalized);
                            item.Replace(item, normalized);
                            formulaHolder = formulaHolder.Replace(item, normalized);


                        }

                        else
                        {
                            throw new FormulaFormatException("not a valid formula");
                        }
                    }

                    else
                    {
                        throw new FormulaFormatException("not a valid formula");
                    }

                }

                //rule 5 enforcement
                if (flag == 0)
                {
                    if (currentvar == false && !(stringTokens.First().ToString().Equals("(")) && firstparse == false)
                    {
                        throw new FormulaFormatException("Not a valid first token");
                    }

                    flag++;

                }

                //Rule 1 enforcement
                rule1(item, result);



                //Negative number enforcement
                if (result < 0)
                    throw new FormulaFormatException("Numbers must be postive!");





                //Rule helper method
                rule7(item, result);

                //Rule helper method
                rule8(item);





                //rule 6 enforcement
                if (currentvar == false)
                {
                    if (!Regex.IsMatch(stringTokens.Last(), pattern))
                    {
                        if (stringTokens.Last().Equals("0.0"))
                        {
                            //do nothing
                        }
                        else if (!(stringTokens.Last().ToString().Equals(")") || last != 0))
                        {
                            throw new FormulaFormatException("Not a valid last token");
                        }
                    }

                }

                //Rule helper method
                rule3(item);

                lastToken = item;
                lastTokenVar = false;
                if (currentvar == true)
                    lastTokenVar = true;

                currentvar = false;
            }

            //Rule helper method
            rule4();

        }

        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            if (ReferenceEquals(formulaHolder, null))
                return 0;

            Stack<String> Opstack = new Stack<String>();    //Operator stack
            Stack<double> ValStack = new Stack<Double>();   //Value stack
            double val = 0;
            double finalresult = 0; //final end result
            bool parseSuccess;      //if the parse succeeded
            stringTokens = GetTokens(formulaHolder);





            foreach (String item in stringTokens)
            {
                parseSuccess = double.TryParse(item, out val);

                //We found a variable
                if (parseSuccess == false)
                    if (Regex.IsMatch(item, pattern))
                    {
                        try
                        {
                            //lookup its value
                            val = lookup(item);
                        }
                        catch
                        {
                            //Delegate not provided
                            throw new FormulaEvaluationException("Something went wrong with the delegate!");
                        }

                    }

                //If the token is a number
                if (parseSuccess == true || val != 0)
                {
                    if (Opstack.Count > 0)
                    {
                        if (Opstack.Peek().Equals("*") || Opstack.Peek().Equals("/"))
                        {

                            String op = Opstack.Pop();
                            double poppedNum = ValStack.Pop();
                            double result = 0;

                            //check for division by 0

                            if (op.Equals("/"))
                                if (val == 0)
                                    throw new FormulaEvaluationException("Cannot divide by 0");

                            //Do arithmetic depending on operator
                            switch (op)
                            {
                                case "*":
                                    result = poppedNum * val;
                                    break;
                                case "/":
                                    result = poppedNum / val;
                                    break;
                            }

                            ValStack.Push(result);
                            continue;

                        }

                        //If the top of the Opstack is not * or /
                        else
                        {
                            ValStack.Push(val);
                            continue;
                        }


                    }

                    //If theres nothing in the Opstack
                    else
                    {
                        ValStack.Push(val);
                        continue;
                    }


                }


                //if t is a + or -
                if (item.Equals("+") || item.Equals("-"))
                {
                    if (Opstack.Count > 0)
                        if (Opstack.Peek().Equals("+") || Opstack.Peek().Equals("-"))
                            if (ValStack.Count >= 2 && Opstack.Count >= 1)
                            {
                                double val1 = ValStack.Pop();
                                double val2 = ValStack.Pop();
                                String op = Opstack.Pop();
                                double result = 0;

                                switch (op)
                                {
                                    case "+":
                                        result = val2 + val1;
                                        break;
                                    case "-":
                                        result = val2 - val1;
                                        break;
                                }

                                ValStack.Push(result);

                            }

                    Opstack.Push(item);
                    continue;



                }

                //if we encounter a * or /
                if (item.Equals("*") || item.Equals("/"))
                {
                    Opstack.Push(item);
                    continue;
                }

                if (item.Equals("("))
                {
                    Opstack.Push("(");
                    continue;
                }

                if (item.Equals(")"))
                {
                    if (Opstack.Count > 0)
                        if (Opstack.Peek().Equals("+") || Opstack.Peek().Equals("-"))
                        {
                            double val1 = ValStack.Pop();
                            double val2 = ValStack.Pop();
                            String tempop = Opstack.Pop();
                            double result = 0;

                            switch (tempop)
                            {
                                case "+":
                                    result = val1 + val2;
                                    break;
                                case "-":
                                    result = val2 - val1;
                                    break;
                            }

                            ValStack.Push(result);

                        }

                    // ( must be on the op stack
                    String topOp = Opstack.Pop();
                    if (!topOp.Equals("("))
                        throw new FormulaFormatException("Expected '(' at top of Op stack");

                    if (Opstack.Count > 0)
                    {
                        if (Opstack.Peek().Equals("*") || Opstack.Peek().Equals("/"))
                        {
                            double somval = ValStack.Pop();
                            double somval2 = ValStack.Pop();
                            String someop = Opstack.Pop();
                            double result = 0;

                            if (someop.Equals("/"))
                                if (somval == 0)
                                    throw new FormulaEvaluationException("Cannot divide by 0!");

                            switch (someop)
                            {
                                case "*":
                                    result = somval2 * somval;
                                    break;
                                case "/":
                                    result = somval2 / somval;
                                    break;
                            }

                            ValStack.Push(result);


                        }
                    }


                }

            }


            //final computations for the result

            if (Opstack.Count == 0)
            {
                if (ValStack.Count == 1)
                    return ValStack.Pop();
            }

            else
            {
                String finalop = Opstack.Pop();
                double finalval = ValStack.Pop();
                double finalval2 = ValStack.Pop();

                switch (finalop)
                {
                    case "+":
                        finalresult = finalval2 + finalval;
                        break;
                    case "-":
                        finalresult = finalval2 - finalval;
                        break;
                }
            }


            return finalresult;
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }

        /// <summary>
        /// Helper method to enforce rule 1
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void rule1(String item, double result)
        {
            if (currentvar == false && parsed == false)
                if (!(item.Equals("(") || item.Equals(")") || item.Equals("+") || item.Equals("-") || item.Equals("*") || item.Equals("/")))
                {
                    throw new FormulaFormatException("Invalid token");
                }

        }

        /// <summary>
        /// Helper method to enforce rule 2
        /// </summary>
        /// <param name="formula"></param>
        private void rule2(String formula)
        {
            formula = formula.Trim();
            if (formula.Equals(""))
                throw new FormulaFormatException("Must contain at least one token");
        }

        /// <summary>
        /// Helper method to enforce rule 3
        /// </summary>
        /// <param name="item"></param>
        private void rule3(String item)
        {
            if (item.Equals("("))
                Lparencount++;

            if (item.Equals(")"))
                Rparencount++;

            if (Rparencount > Lparencount)
                throw new FormulaFormatException("Too many ')' Parenthesis");
        }

        /// <summary>
        /// Helper method to enforce rule 4
        /// </summary>
        private void rule4()
        {
            if (Lparencount != Rparencount)
                throw new FormulaFormatException("Missing Parenthesis");
        }

        /// <summary>
        /// Helper method to enforce rule 7
        /// </summary>
        /// <param name="item"></param>
        /// <param name="result"></param>
        private void rule7(String item, double result)
        {
            if (currentvar == false && parsed == false)
            {
                if (lastToken.Equals("(") || lastToken.Equals("+") || lastToken.Equals("-") ||
                    lastToken.Equals("*") || lastToken.Equals("/"))
                {
                    if (!(item.Equals("(")) && result == 0)
                        throw new FormulaFormatException("Operator must be a variable, (, or a number");
                }
            }
        }

        /// <summary>
        /// Helper method to enforce rule 8
        /// </summary>
        /// <param name="item"></param>
        private void rule8(String item)
        {
            //Any token that immediately follows a number, a variable, or a closing parenthesis must be either 
            //an operator or a closing parenthesis.
            double result;
            double.TryParse(lastToken, out result);
            if (result != 0 || lastToken.Equals(")") || lastTokenVar == true)
            {
                if (!(item.Equals("+") || item.Equals("-") || item.Equals("*") || item.Equals("/") || item.Equals(")")))
                {
                    throw new FormulaFormatException("a token following a number, variable, or closing parenthesis must be an operator or closing parenthesis");
                }
            }

        }

        //I dont know how to call this...
        /// <summary>
        /// Returns the variables of a string
        /// </summary>
        /// <returns></returns>
        public ISet<String> GetVariables()
        {
            HashSet<String> set = new HashSet<string>();

            if (ReferenceEquals(formulaHolder, null))
                return set;


            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            String exp = "[0-9]\\.[0-9][e][0-9]";


            IEnumerable<String> tokens = GetTokens(formulaHolder);


            foreach (string token in tokens)
            {
                if (Regex.IsMatch(token, varPattern))
                {
                    if (Regex.IsMatch(token, exp))
                        continue;
                    else
                        set.Add(token);
                }
            }




            return set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ReferenceEquals(formulaHolder, null))
                return null;

            string s = formulaHolder;
            return s;
        }

    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string s);

    /// <summary>
    /// Normalizer delegate
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate string Normalizer(string s);

    /// <summary>
    /// Validator delegate
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message)
            : base(message)
        {
        }
    }
}