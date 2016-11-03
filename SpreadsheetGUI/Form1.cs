using SS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSGui;
using System.IO;
using Formulas;

namespace SpreadsheetGUI
{

    public partial class Form1 : Form, ISpreadsheetView
    {

        /// <summary>
        /// Property definition for the ISpreadsheetView
        /// </summary>
        public string name
        {
            get
            {
                
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        /// <summary>
        ///  Property definition for the ISpreadsheetView
        /// </summary>
        public object content
        {
            set
            {
                textBox2.Text = (String) value;
            }
        }

        /// <summary>
        /// Property definition for the ISpreadsheetview
        /// </summary>
        public object value
        {
            set
            {
                
                textBox3.Text = (String) value;
            }
        }

             private int col;
             private int row;

     //   public event Action<int, int> RowCol;
        public event Action<int, int> CellChanged;
        public event Action<String> contentChanged;
        public event Action<TextReader> openFile;
        public event Action<TextWriter> saveFile;
        public event Action clear;

        public Form1()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// Creates a new instance of a Spreadsheet
        /// </summary>
        public void OpenNew()
        {
            Context.GetContext().RunNew();
        }

        /// <summary>
        /// Gets the current cell of the spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SpreadsheetPanel sender)
        {
            sender.GetSelection(out col, out row);

            if (CellChanged != null)
                CellChanged(col, row);

        }

        /// <summary>
        /// called when enter is pressed to change contents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar.Equals((char)13))
                {
                    contentChanged(textBox2.Text);

                }

            }
            catch (Exception)
            {
                MessageBox.Show("not a valid formula");

            }
        }


        /// <summary>
        /// Sets the value of a cell
        /// </summary>
        /// <param name="a"></param>
        /// <param name="ab"></param>
        /// <param name="b"></param>
        public void setContents(int a, int ab, string b)
        {
            spreadsheetPanel1.SetValue(a, ab, b);
            textBox3.Text = b;
            
        }

        /// <summary>
        /// Sets the display in the textboxes
        /// </summary>
        /// <param name="content"></param>
        /// <param name="value"></param>
        public void setDisplay(String content, string value)
        {
            textBox2.Text = content;
            textBox3.Text = value;
        }

        /// <summary>
        /// opens a spreadsheet file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            DialogResult result = openFileDialog1.ShowDialog();
            

            if (result == DialogResult.OK || result == DialogResult.Yes) // Test result.
            {
                string file = openFileDialog1.FileName;
                TextReader reader = File.OpenText(file);

                try
                {
                    openFile(reader);

                }
                catch(Exception m)
                {
                    MessageBox.Show("Unable to open file\n" + m.Message);
                }


            }
                
               
        }

        /// <summary>
        /// Saves a spreadsheet file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = "untitled spreadsheet";
            saveFileDialog1.Filter = "Spreadsheet Files (*.ss) |*.ss | All File (*.*) | *.*";

            saveFileDialog1.ShowDialog();

            TextWriter writer = File.CreateText(saveFileDialog1.FileName);

            try
            {
                saveFile(writer);

            }

            catch(Exception m)
            {
                MessageBox.Show("Something went wrong with saving!" + m.Message);
            }
        }

        /// <summary>
        /// Creates a new spreadsheet instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenNew();
        }

        /// <summary>
        /// Closes the current spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Do u want to save the current project? \n Warning: If no, all data will be lost.", "Warning", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                saveFileDialog1.FileName = "untitled spreadsheet";
                saveFileDialog1.Filter = "Spreadsheet Files (*.ss) |*.ss | All File (*.*) | *.*";
                TextWriter writer = File.CreateText(saveFileDialog1.FileName);
                saveFile(writer);

            }
            else
            {
                spreadsheetPanel1.Clear();
                clear();
                
                
            }
        }

        /// <summary>
        /// Explains the functionality of the spreadsheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is a basic Spreadsheet that allows users to input numbers, contents and values."
                          + " Users can enter text, numbers, or do arthimetic calculations by typing in the contents box and hitting enter. Only '+', '-', '*', or '/' operators are available \nv"
                          + " \n Users are also allowed to store variables and give them values. The user can then the the '=' operator in order to do calculations."
                          + " For example, '=A1+B1' would add the variables A1 and B1, this arthimetic on variables is called a formula \n"
                          + "\n This spreadsheet also has features that allow you to create new spreadsheets, open saved spreadsheets or to save the current spreadsheet."
                          + "Please note, when saving a spreadsheet, the .ss will be appended.");
        }
    }
}