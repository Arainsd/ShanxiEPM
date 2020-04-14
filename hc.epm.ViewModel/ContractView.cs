using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ContractView
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public int? ContractType { get; set; }

        public long Id { get; set; }

        public List<string> FileName { get; set; }

        public long FileId { get; set; }

        public string FileNameStr { get; set; }
    }
}
