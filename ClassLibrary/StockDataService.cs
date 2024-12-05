using BlazorBootstrap;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YahooQuotesApi;

namespace ClassLibrary
{
    public class StockDataService
    {
        private List<double> historical;
        public StockDataService() { this.historical = new List<double> { 0 }; }
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

        // Example usage within a synchronous method
        public void GetStockData()
        {
            string symbol = "AAPL";
            DateTime startDate = new DateTime(2023, 1, 1);

            // Call the asynchronous method and wait for the result
            List<double> closingPrices = GetHistoricalClosingPricesAsync(symbol, startDate).Result;

            // Process the retrieved data
            Console.WriteLine($"Historical closing prices for {symbol}:");
            foreach (var price in closingPrices)
            {
                Console.WriteLine(price);
            }
        }
    }
}
