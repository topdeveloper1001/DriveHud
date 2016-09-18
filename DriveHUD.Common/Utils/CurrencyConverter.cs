using DriveHUD.Common.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DriveHUD.Common.Utils
{
    public static class CurrencyConverter
    {
        /// <summary>
        /// Converts currency using  Google API
        /// </summary>
        /// <param name="amount">amount to convert</param>
        /// <param name="fromCurrency">initial currency</param>
        /// <param name="toCurrency">target currency</param>
        /// <returns>Converted amount</returns>
        public static decimal ConvertCurrency(decimal amount, string fromCurrency, string toCurrency)
        {
            decimal result = 0m;

            try
            {
                //Grab values and build Web Request to the API
                string apiURL = String.Format("https://www.google.com/finance/converter?a={0}&from={1}&to={2}&meta={3}", amount, fromCurrency, toCurrency, Guid.NewGuid().ToString());

                //Make Web Request and grab the results
                var request = WebRequest.Create(apiURL);

                //Get the Response
                var streamReader = new StreamReader(request.GetResponse().GetResponseStream(), System.Text.Encoding.ASCII);

                //Grab converted value (ie 2.45 USD)
                var resultString = Regex.Matches(streamReader.ReadToEnd(), "<span class=\"?bld\"?>([^<]+)</span>")[0].Groups[1].Value;

                if (string.IsNullOrEmpty(resultString))
                {
                    return result;
                }

                //Get the Result
                Decimal.TryParse(resultString.Split(' ').First(), out result);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            return result;
        }
    }
}
