using Formulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS
{

    class Cell
    {
        public Object name { get; set; }
        public Object content { get; set; }


        public Cell(String name, string content)
        {
            this.name = name;
            this.content = content;
        }

        public Cell(String name, double content)
        {
            this.name = name;
            this.content = content;
        }

        public Cell(String name, Formula content)
        {
            this.name = name;
            this.content = content;
        }

    }
}