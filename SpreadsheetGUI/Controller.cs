using SS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    public class Controller
    {
        private ISpreadsheetView window;
        private Spreadsheet spreadsheet;
        private int grow;
        private int gcol;

        public Controller(ISpreadsheetView window)
        {
            this.window = window;
            this.spreadsheet = new Spreadsheet();

            window.CellChanged += Window_CellChanged;
            window.contentChanged += window_contentChanged;
            window.openFile += Window_openFile;
            window.saveFile += Window_saveFile;
            window.clear += Window_clear;


        }

        private void Window_clear()
        {
            this.spreadsheet = new Spreadsheet();
        }

        /// <summary>
        /// Saves the file
        /// </summary>
        /// <param name="obj"></param>
        private void Window_saveFile(TextWriter obj)
        {
            
            spreadsheet.Save(obj);
        }

        /// <summary>
        /// Opens a Spreadsheet file
        /// </summary>
        /// <param name="obj"></param>
        private void Window_openFile(TextReader obj)
        {
            
                spreadsheet = new Spreadsheet(obj);

                foreach (string cellName in spreadsheet.GetNamesOfAllNonemptyCells())
                {
                    celltocolrow(cellName);
                    window.setContents(gcol, grow, spreadsheet.GetCellValue(cellName).ToString());
                    
                }
            
        }

    

        /// <summary>
        /// Sets the display and is called whenever a cell gets selected
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
    private void Window_CellChanged(int col, int row)
        {
            row = row + 1;
            char ch = Convert.ToChar(col + 65);

            string name = ch.ToString() + row.ToString();

            window.name = name;

            if (spreadsheet.GetNamesOfAllNonemptyCells().Contains(name))
            {
                
                window.setDisplay(spreadsheet.GetCellContents(name).ToString(), spreadsheet.GetCellValue(name).ToString());
            }

            else
            {
                window.setDisplay("", "");
            }

        }

        /// <summary>
        /// Called when content is changed
        /// </summary>
        /// <param name="txt2"></param>
        private void window_contentChanged(String txt2)
        {

            String content = txt2;
            ISet<String> set;


            set = spreadsheet.SetContentsOfCell(window.name, content);
            celltocolrow(window.name);


            window.setContents(gcol, grow, spreadsheet.GetCellValue(window.name).ToString());


            foreach (var item in set)
            {
                celltocolrow(item);
                window.setContents(gcol, grow, spreadsheet.GetCellValue(item).ToString());
                
            }

        }

        /// <summary>
        /// Private method that transforms a name into its spreadsheet column and row equivalents.
        /// </summary>
        /// <param name="name"></param>
        private void celltocolrow(String name)
        {
            char[] arr = name.ToArray();
            char a = '\0';
            int result = 0;
            int temp = 0;

           for(int i = 0; i < arr.Length; i++)
            {
                if (i == 0)
                    a = arr[0];
                else
                {
                    //result += (int)arr[i];
                    int.TryParse(arr[i].ToString(), out temp);
                    if (i == 1)
                        temp = temp * 10;
                    result = result + temp;
                }
            }

            gcol = (int) a - 65;
            grow = (result / 10) - 1;

        }

  

    }
    
}
