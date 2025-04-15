using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class OptionsRequest
    {
        public string Optionsymbol { get; set; }  // Required for 'options' table
        public int type { get; set; } // 1 or 2
        public string StockName { get; set; } // Required for 'stock_list' table
    }
}
