using MWPoloniexAPI.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MWPoloniexAPI.Conect
{
    public class Poloniex
    {
        private string api_key;
        private string api_secret;
        private string base_url = "https://poloniex.com/";
        private string trading_url = "tradingApi";
        private string public_url = "public";

        public Poloniex(string APIkey, string APIsecret)
        {
            api_key = APIkey;
            api_secret = APIsecret;
        }

        #region  Private commands
        [Description("Get account balances.")]
        public async Task<Dictionary<string, string>> GetBalances()
        {
            Dictionary<string, string> return_ = new Dictionary<string, string>();

            try
            {
                await Task.Factory.StartNew(() => {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "returnBalances");
                    return_ = JsonConvert.DeserializeObject<Dictionary<string, string>>(query(req).Result);
                });
            }
            catch (Exception ex)
            {
                //throw;
            }

            return return_;
        }

        [Description("Get open orders by currency pair.")]
        public async Task<List<OpenOrdersClass>> GetOpenOrders(CurrencyPair CurrencyPair_)
        {
            List<OpenOrdersClass> return_ = new List<OpenOrdersClass>();

            try
            {
                await Task.Factory.StartNew(() => {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "returnOpenOrders");
                    req.Add("currencyPair", CurrencyPair_.ToString());

                    List<OpenOrdersClass> myDeserializedObjList = (List<OpenOrdersClass>)Newtonsoft.Json.JsonConvert.DeserializeObject(query(req).Result, typeof(List<OpenOrdersClass>));
                    return_ = myDeserializedObjList;
                });
            }
            catch (Exception)
            {
                //throw;
            }

            return return_;
        }

        [Description("Open buy order by currency pair, rate and amount.")]
        public async Task<Order> Buy(CurrencyPair CurrencyPair_, double Rate, double Amount)
        {
            Order buy_ = new Order();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "buy");
                    req.Add("currencyPair", CurrencyPair_.ToString());
                    req.Add("rate", Rate.ToString().Replace(",", "."));
                    req.Add("amount", Amount.ToString().Replace(",", "."));

                    buy_ = JsonConvert.DeserializeObject<Order>(query(req).Result);
                });
            }
            catch
            {

            }

            return buy_;
        }

        [Description("Open sell order by currency pair, rate and amount.")]
        public async Task<Order> Sell(CurrencyPair CurrencyPair_, double Rate, double Amount)
        {
            Order sell_ = new Order();

            try
            {
                await Task.Factory.StartNew(() =>
                {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "sell");
                    req.Add("currencyPair", CurrencyPair_.ToString());
                    req.Add("rate", Rate.ToString().Replace(",", "."));
                    req.Add("amount", Amount.ToString().Replace(",", "."));

                    sell_ = JsonConvert.DeserializeObject<Order>(query(req).Result);
                });
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex);
            }
            return sell_;
        }

        [Description("Cancel open order by currency pair and order number.")]
        public async Task<bool> CancelOrder(CurrencyPair CurrencyPair_, BigInteger OrderNumber)
        {
            bool canceled = false;
            try
            {
                await Task.Factory.StartNew(() => {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "cancelOrder");
                    req.Add("currencyPair", CurrencyPair_.ToString());
                    req.Add("orderNumber", OrderNumber.ToString().Replace(",", "."));

                    if (query(req).Result.Contains(string.Format("Order #{0} canceled.", OrderNumber)))
                        canceled = true;
                    else
                        canceled = false;
                });
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex);
            }
            return canceled;
        }
        #endregion

        #region Public commands
        public async Task<OrderBook> GetOrderBook(CurrencyPair CurrencyPair_, int depth = 1)
        {
            OrderBook OrderBook_ = new OrderBook();

            try
            {
                await Task.Run(() =>
                {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "returnOrderBook");
                    req.Add("currencyPair", CurrencyPair_.ToString());
                    req.Add("depth", depth.ToString());

                    string request = querypublic(req).Result;
                    OrderBook_ = JsonConvert.DeserializeObject<OrderBook>(request);
                });
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex);
            }

            return OrderBook_;
        }

        public async Task<List<ChartClass>> GetChartData(CurrencyPair CurrencyPair_, DateTime Start, DateTime End, Period Peroid)
        {
            List<ChartClass> myDeserializedObjList = new List<ChartClass>();

            try
            {
                await Task.Run(() =>
                {
                    Dictionary<string, object> req = new Dictionary<string, object> { };
                    req.Add("command", "returnChartData");
                    req.Add("currencyPair", CurrencyPair_.ToString());
                    req.Add("start", Start.ToUnix());
                    req.Add("end", End.ToUnix());
                    req.Add("period", Peroid.PeriodToInteger());

                    string request = querypublic(req).Result;
                    myDeserializedObjList = (List<ChartClass>)Newtonsoft.Json.JsonConvert.DeserializeObject(request, typeof(List<ChartClass>));
                });
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex);
            }

            return myDeserializedObjList;
        }
        #endregion

        #region Function to connection
        private async Task<string> query(Dictionary<string, object> req)
        {
            string return_ = "";
            req.Add("nonce", Helper.microtime());

            string postdata = http_build_query(req);

            var postBytes = Encoding.GetBytes(postdata);
            string sign = hash(postBytes);

            var request = CreateHttpWebRequest("POST", trading_url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postdata.Length;

            request.Headers["Key"] = api_key;
            request.Headers["Sign"] = sign;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                return_ = await reader.ReadToEndAsync();
            }
            return return_;
        }

        private async Task<string> querypublic(Dictionary<string, object> req)
        {
            string postdata = http_build_query(req);
            string html = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(base_url + public_url + "?" + postdata);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = await reader.ReadToEndAsync();
                }
            }
            catch (Exception)
            {
            }

            return html;
        }

        private HttpWebRequest CreateHttpWebRequest(string method, string relativeUrl)
        {
            var request = WebRequest.CreateHttp(base_url + relativeUrl);
            request.Method = method;
            request.UserAgent = "MW_bot API";

            request.Timeout = System.Threading.Timeout.Infinite;

            request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip,deflate";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            return request;
        }

        private string http_build_query(Dictionary<string, object> parameters)
        {
            var builder = new UriBuilder("http://url.com");
            builder.Port = -1;

            var query = System.Web.HttpUtility.ParseQueryString(builder.Query);
            foreach (var v in parameters)
            {
                query[v.Key] = v.Value.ToString();
            }
            builder.Query = query.ToString();
            string url = builder.ToString();

            return builder.ToString().Replace("http://url.com/?", "");
        }

        private readonly Encoding Encoding = Encoding.ASCII;

        private string hash(byte[] postBytes)
        {
            HMACSHA512 Encryptor = new HMACSHA512(Encoding.GetBytes(api_secret));
            return Encryptor.ComputeHash(postBytes).ToHexString();
        }
        #endregion
    }
}
