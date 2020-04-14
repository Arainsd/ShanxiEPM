using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.FileServer.Data
{
    public class FileConfig
    {
        public string Number { get; set; }

        public string App { get; set; }

        public string FileTypeName { get; set; }

        public string FileTypeDirectory { get; set; }

        public string FileTypeExtension { get; set; }

        public string SaveFilePath { get; set; }

        public string SaveTempPath { get; set; }

        public string ParentPath { get; set; }

        public string DirectoryFormat { get; set; }
    }
}
