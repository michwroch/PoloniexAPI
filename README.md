# PoloniexAPI
This is very simple C# class for Poloniex API

This project was maded for study. Author doesn't take a resposibitity for using it. To correct runing, you need Newtonsoft.Json liberary and System.Net.Http Unity

Main clas is Conect/Poloniex

You can use it like this:
Poloniex pol = new Poloniex(APIKEY, APISECRET);
and for Example:
var Balances = await pol.GetBalances();
var OpenOrders = await pol.GetOpenOrders(new CurrencyPair("ETH"));
etc...
