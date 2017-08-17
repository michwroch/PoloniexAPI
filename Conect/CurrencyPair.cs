using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MWPoloniexAPI.Conect
{
    public class CurrencyPair
    {
        string _Base = "BTC";
        string _Quote = "";

        public CurrencyPair(string Quote, string Base = "BTC")
        {
            _Base = Base;
            _Quote = Quote;
        }

        public override string ToString()
        {
            return (_Base + "_" + _Quote).ToUpper();
        }
    }
}
