using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyrianPoundRates.Models
{
    [Serializable]
    public class CurrencyRateViewMode
    {
        public CurrencyRateViewMode()
        {
            
        }
        public CurrencyRateViewMode(IList<CurrencyRate> rates)
        {
            var dollarSellingRate = rates.First(r => r.Type == TradeType.Selling && r.CurrencyName == "Dollar");
            var euroSellingRate = rates.First(r => r.Type == TradeType.Selling && r.CurrencyName == "Euro");
            var dollarBuyingRate = rates.First(r => r.Type == TradeType.Buying && r.CurrencyName == "Dollar");
            var euroBuyingRate = rates.First(r => r.Type == TradeType.Buying && r.CurrencyName == "Euro");

            SellingUsDollar = new RateKeyPair(dollarSellingRate.RateId){ExchangePrice =  dollarSellingRate.ExchangePrice};
            SellingEuro = new RateKeyPair(euroSellingRate.RateId) { ExchangePrice = euroSellingRate.ExchangePrice};
            BuyingUsDollar = new RateKeyPair(dollarBuyingRate.RateId) {ExchangePrice = dollarBuyingRate.ExchangePrice};
            BuyingEuro = new RateKeyPair(euroBuyingRate.RateId) { ExchangePrice = euroBuyingRate.ExchangePrice };

        }

        public RateKeyPair SellingUsDollar { get;  set; }
        public RateKeyPair SellingEuro { get;  set; }

        public RateKeyPair BuyingUsDollar { get;  set; }

        public RateKeyPair BuyingEuro { get;  set; }

    }

     [Serializable]
    public class RateKeyPair
    {
        public RateKeyPair() { }
        public RateKeyPair(string id)
        {
            RateId = id;      
        }
       
        public string RateId { get;  set; }

        [Required]
        [DataType(DataType.Currency)]
        public double ExchangePrice { get; set; }
    }
}
