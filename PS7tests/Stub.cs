using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SSGui;

namespace PS7tests
{
    class Stub : ISpreadsheetView
    {
        public string name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public event Action<int, int> CellChanged;
        public event Action clear;
        public event Action<string> contentChanged;
        public event Action<TextReader> openFile;
        public event Action<TextWriter> saveFile;

        public void OpenNew()
        {
            throw new NotImplementedException();
        }

        public void setContents(int a, int ab, string b)
        {
            CellChanged(0, 0);
        }

        public void setDisplay(string content, string value)
        {
            throw new NotImplementedException();
        }
    }
}
