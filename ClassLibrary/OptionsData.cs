using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class OptionsData
    {
        public int? Id { get; set; }
        public string? Symbol { get; set; }
        public int? type { get; set; }

        // used in a specific page (only way i found it to work)
        public double? CurrentPrice { get; set; }
    }
}
