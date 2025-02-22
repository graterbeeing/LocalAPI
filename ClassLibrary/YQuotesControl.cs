using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooQuotesApi;

namespace ClassLibrary
{
    public class YQuotesControl : YahooInterface
    {
        private List<double> historical;
        public YQuotesControl() { this.historical = new List<double> { 0 }; }
        public List<double> GetHistoricl() { return this.historical; }

        public async Task<List<double>> GetHistoricalClosingPricesAsync(string symbol, DateTime startDate)
        {
            var yahooQuotes = new YahooQuotesBuilder()
                .WithHistoryStartDate(NodaTime.Instant.FromDateTimeUtc(startDate.ToUniversalTime()))
                .Build();

            // Fetch historical data asynchronously
            var result = await yahooQuotes.GetHistoryAsync(symbol);


            if (!result.HasValue)
            {
                throw new Exception($"No historical data found for symbol '{symbol}'.");
            }

            // Extract closing prices
            var history = result.Value;
            var historical = history.Ticks.Select(t => (double)t.Close).ToList();

            return historical;
        }
    }
}
