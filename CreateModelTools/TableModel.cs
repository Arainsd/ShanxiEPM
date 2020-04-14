using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModelTools
{
    public class TableModel
    {
        public bool IsMap { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string NoPrefixName
        {

            get
            {
                int start = Name.IndexOf("_");
                return Name.Substring(start + 1);
            }
        }

        public string NoLineName
        {
            get
            {
                return Name.Replace("_", "");
            }
        }
    }
}
