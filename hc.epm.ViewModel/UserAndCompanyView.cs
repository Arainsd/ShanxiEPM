using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class UserAndCompanyView
    {
        public long? PId { get; set; }
        public string PreName { get; set; }
       // public string time { get; set; }
        public long? CompanyId { get; set; }
        public long? UserID { get; set; }
        public string PostValue { get; set; }
        public string companyName { get; set; }

        public string time { get; set; }
    }
}
