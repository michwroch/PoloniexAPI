using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MWPoloniexAPI.Conect
{
    public static class Helper
    {
        public static int PeriodToInteger(this Period Peroid)
        {
            int _peroid = 300;
            switch (Peroid)
            {
                case Period.M5:
                    _peroid = 300;
                    break;
                case Period.M15:
                    _peroid = 900;
                    break;
                case Period.M30:
                    _peroid = 1800;
                    break;
                case Period.H2:
                    _peroid = 7200;
                    break;
                case Period.H4:
                    _peroid = 14400;
                    break;
                case Period.H24:
                    _peroid = 86400;
                    break;
            }

            return _peroid;
        }

        public static BigInteger ToUnix(this DateTime dt)
        {
            TimeSpan span = dt - UnixEpoch;
            return (BigInteger)span.TotalSeconds;
        }

        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public static BigInteger microtime()
        {
            DateTime Now = DateTime.UtcNow;
            TimeSpan span = Now - UnixEpoch;
            return (BigInteger)span.TotalSeconds * 1000000 + int.Parse(Now.ToString("ffffff"));
        }

        static internal readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

        public static string ToHexString(this byte[] hex)
        {
            if (hex == null) return null;
            if (hex.Length == 0) return string.Empty;

            var s = new StringBuilder();
            foreach (byte b in hex)
            {
                s.Append(b.ToString("x2"));
            }
            return s.ToString();
        }

        internal static string GetResponseString(this HttpWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null) throw new NullReferenceException("The HttpWebRequest's response stream cannot be empty.");

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }

    public enum OrderType
    {
        sell,
        buy
    }

    public enum Period
    {
        M5,
        M15,
        M30,
        H2,
        H4,
        H24
    }
}
