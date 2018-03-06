using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nop.Plugin.ExchangeRate.UkrNationalBankExchangeRate
{
    public class NbuData
    {
        /// <summary>
        /// Internation currency code
        /// </summary>
        [JsonProperty(PropertyName = "r030")]
        public int r030;
        /// <summary>
        /// Name of currencty
        /// </summary>
        [JsonProperty(PropertyName = "txt")]
        public string txt; 
        /// <summary>
        /// curency rate in relation to Hryvnya
        /// </summary>
        [JsonProperty(PropertyName = "rate")]
        public double rate; 
        /// <summary>
        /// Internation currency code
        /// </summary>
        [JsonProperty(PropertyName = "cc")]
        public string cc;
        /// <summary>
        /// Currency date
        /// </summary>
        [JsonProperty(PropertyName = "exchangedate")]
        public string exchangedate; 
    }
}
