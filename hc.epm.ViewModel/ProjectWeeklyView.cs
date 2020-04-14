using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ProjectWeeklyView
    {
        public int sort { get; set; }
        public string name { get; set; }
        public string time { get; set; }
        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }
    }
}
