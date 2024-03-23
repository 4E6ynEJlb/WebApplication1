using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicsLib
{
    internal class ConstsOptions
    {
        public const string ConstsConfiguration = "ConstsConfiguration";

        public int AdsMaxCount { get; set; }
        public int AdLifeDays { get; set; }
        public int TicksCount { get; set; }
        public string LinkTemplate { get; set; }
    }
}
