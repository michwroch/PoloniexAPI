using MWPoloniexAPI.Conect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MWPoloniexAPI.Commands
{
    public class OpenOrdersClass
    {
        public BigInteger orderNumber { get; set; }
        public OrderType type { get; set; }
        public double rate { get; set; }
        public double startingAmount { get; set; }
        public double amount { get; set; }
        public double total { get; set; }
        public DateTime date { get; set; }
        public double margin { get; set; }
    }

    public class Order
    {
        public BigInteger orderNumber { get; set; }
        [JsonProperty("resultingTrades")]
        public List<resultingTradesClass> resultingTrades { get; set; }
    }

    public class resultingTradesClass
    {
        public double amount { get; set; }
        public DateTime date { get; set; }
        public double total { get; set; }
        public BigInteger tradeID { get; set; }
        public OrderType type { get; set; }
    }

    public class OrderBook
    {
        public IList<double[]> asks { get; set; }
        public IList<double[]> bids { get; set; }
    }

    public class ChartClass
    {
        public BigInteger date { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double open { get; set; }
        public double close { get; set; }
        public double volume { get; set; }
        public double quoteVolume { get; set; }
        public double weightedAverage { get; set; }
    }

}
