using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExchangeSharp.API.Exchanges.TheRockTrading
{
    public  class ExchangeTheRockTradingAPI: ExchangeAPI
    {
        public override string BaseUrl { get; set; } = "https://api.therocktrading.com/v1";

        public override string Name => "The Rock Trading";
        
        public ExchangeTheRockTradingAPI()
        {
            NonceStyle = NonceStyle.UnixMilliseconds;
            NonceOffset = TimeSpan.FromSeconds(0.1);
            MarketSymbolIsUppercase = false;
            MarketSymbolSeparator = "";
            MarketSymbolIsReversed = true;
            WebSocketOrderBookType = WebSocketOrderBookType.DeltasOnly;
        }

        public override async Task<IReadOnlyDictionary<string, ExchangeCurrency>> GetCurrenciesAsync()
        {
            var response = await MakeJsonRequestAsync<Dictionary<string, TheRockCurrency>>("currencies");

            return response.ToDictionary(pair => pair.Key, pair => new ExchangeCurrency()
            {
                FullName = pair.Value.CommonName,
                Name = pair.Value.Symbol
            });
        }

        public override async Task<IEnumerable<MarketCandle>> GetCandlesAsync(string marketSymbol, int periodSeconds, DateTime? startDate = null, DateTime? endDate = null,
            int? limit = null)
        {
            var response = await MakeJsonRequestAsync<List<TheRockOhlcStatistic>>(
                $"funds/{marketSymbol.Replace(MarketSymbolSeparator, string.Empty)}/ohlc_statistics",null, new Dictionary<string, object>()
                {
                    {"before", endDate},
                    {"after", startDate},
                    {"period", periodSeconds* 60}
                });

            return response.Select(statistic => new MarketCandle()
            {
                Name = statistic.FundId,
                Timestamp = statistic.IntervalStartsAt,
                LowPrice = statistic.Low,
                HighPrice = statistic.High,
                OpenPrice = statistic.Open,
                ClosePrice = statistic.Close,
                ExchangeName = Name,
                PeriodSeconds = (int)(statistic.IntervalEndsAt - statistic.IntervalStartsAt).TotalSeconds,
                WeightedAverage =  statistic.WeightedAverage,
                BaseCurrencyVolume = statistic.BaseVolume,
            });
        }


        public override async Task<ExchangeTicker> GetTickerAsync(string marketSymbol)
        {
            var response = await MakeJsonRequestAsync<TheRockTicker>(
                $"funds/{marketSymbol.Replace(MarketSymbolSeparator, string.Empty)}/ticker");

            return new ExchangeTicker()
            {
                Ask = response.Ask,
                Bid = response.Bid,
                Last = response.Last,
                Volume = new ExchangeVolume()
                {
                    Timestamp = response.Date,
                    BaseCurrencyVolume = response.Volume
                },
                MarketSymbol =  marketSymbol
            };
        }
        
        public overr

    public class TheRockCurrency
    {
        public string Symbol { get; set; }
        [JsonProperty(PropertyName = "common_name")]
        public string CommonName { get; set; }
        public int Decimals { get; set; }
    }

    public class TheRockTicker
    {
        [JsonProperty(PropertyName = "fund_id")]
        public string FundId { get; set; }
        public DateTime Date { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal Last { get; set; }
        public decimal Volume { get; set; }
        [JsonProperty(PropertyName = "volume_traded")]
        public decimal VolumeTraded { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
       
    }

    public class TheRockOhlcStatistic
    {
        [JsonProperty(PropertyName = "fund_id")]
        public string FundId { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Open { get; set; }
        public decimal Close { get; set; }
        public decimal Average { get; set; }
        [JsonProperty(PropertyName = "weighted_average")]
        public decimal WeightedAverage { get; set; }
        public double BaseVolume { get; set; }
        public double TradeVolume { get; set; }
        [JsonProperty(PropertyName = "interval_starts_at")]
        public DateTime IntervalStartsAt { get; set; }
        [JsonProperty(PropertyName = "interval_ends_at")]
        public DateTime IntervalEndsAt { get; set; }
    }
}
}