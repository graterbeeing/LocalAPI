using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    interface YahooInterface
    {
        Task<List<double>> GetHistoricalClosingPricesAsync(string symbol, DateTime startDate);
    }
}
