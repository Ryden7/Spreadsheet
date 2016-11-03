using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadsheetGUI
{
    public interface ISpreadsheetView
    {
        event Action<int, int> CellChanged;
        event Action<String> contentChanged;
        event Action<TextReader> openFile;
        event Action<TextWriter> saveFile;
        event Action clear;

        String name { get; set; }

        void setContents(int a, int ab, String b);

        void setDisplay(String content, string value);

        
        

        

        void OpenNew();
    }
}
