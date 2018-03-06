using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Plugins;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.ExchangeRate.UkrNationalBankExchangeRate
{
    public class UkrainianNationalBankRateProvider : BasePlugin, IExchangeRateProvider
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;

        #endregion

        public UkrainianNationalBankRateProvider(ILocalizationService localizationService,
            ILogger logger, ICurrencyService currencyService, CurrencySettings currencySettings)
        {
            _localizationService = localizationService;
            _logger = logger;
            _currencyService = currencyService;
            _currencySettings = currencySettings;
        }

        public IList<Core.Domain.Directory.ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            var currencyRatesParsed = JsonConvert.DeserializeObject<NbuData[]>(
                new WebClient
                {
                    Encoding = Encoding.UTF8
                }.DownloadString(@"http://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json"));

            if (exchangeRateCurrencyCode == null)
                throw new ArgumentNullException(nameof(exchangeRateCurrencyCode));

            var updateDate = DateTime.UtcNow;
            
            var ratesToUah = new List<Core.Domain.Directory.ExchangeRate>
            {
                new Core.Domain.Directory.ExchangeRate
                {
                    CurrencyCode = "UAH",
                    Rate = 1,
                    UpdatedOn = updateDate
                }
            };

            foreach (var nbuData in currencyRatesParsed)
                ratesToUah.Add(new Core.Domain.Directory.ExchangeRate()
                {
                    CurrencyCode = nbuData.cc,
                    Rate = (decimal)nbuData.rate,
                    UpdatedOn = updateDate
                });

            var primaryExchangeCurrency = _currencyService.GetCurrencyById(_currencySettings.PrimaryExchangeRateCurrencyId, false);
          var rateDivider = ratesToUah.First(c => c.CurrencyCode == primaryExchangeCurrency.CurrencyCode).Rate;

            foreach (var exchangeRate in ratesToUah)
            {
                if (exchangeRate.CurrencyCode == primaryExchangeCurrency.CurrencyCode)
                {
                    exchangeRate.Rate = 1.00m;
                }
                else
                {
                    exchangeRate.Rate /= rateDivider;
                    exchangeRate.Rate = 1.00m / exchangeRate.Rate;
                }
                
            }

            return ratesToUah;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Nop.Plugin.ExchangeRate.UkrNationalBankExchangeRate.Error", "Main currency should be Ukrainian hryvnia");

            base.Install();
        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Plugins.ExchangeRate.EcbExchange.Error");

            base.Uninstall();
        }

    }
}
