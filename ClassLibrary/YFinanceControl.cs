using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using YahooFinanceApi;
using ClassLibrary;

namespace ClassLibrary
{
    public class YFinanceControl : YahooInterface
    {
        DateTime endDate = DateTime.Today;
        DateTime startDate;
        List<double> history = new List<double>();

        public async Task<List<double>> GetHistoricalClosingPricesAsync(string symbol, DateTime startDate)
        {
            return null;
        }

        public async Task<double?> GetSingle(string symbol)
        {
            try
            {
                var result = await Yahoo
                    .Symbols(symbol)
                    .Fields(YahooFinanceApi.Field.RegularMarketPrice)
                    .QueryAsync();

                return result[symbol][Field.RegularMarketPrice];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stock data for symbol: {symbol}: {ex.Message}");
                return null;
            }
        }
    }
}
