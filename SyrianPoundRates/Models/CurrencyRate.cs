using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyrianPoundRates.Gateway;

namespace SyrianPoundRates.Models
{
    public class CurrencyRate
    {
        [Mappable]
        public string RateId { get; set; }

        [Mappable("RateChangeId")]
        public string ChangeId { get; set; }

        [Mappable("NewRate")]
        public double ExchangePrice { get; set; }

        public TradeType Type { get; set; }

        [Mappable]
        public ChangeType ChangeType { get; set;  }

        [Mappable]
        public double ChangeAmount { get; set;  }
        public string CurrencyName { get; set;  }

        [Mappable]
        public string UpdatedBy { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public enum TradeType
    {
        Selling = 0,
        Buying = 1
    }

    public enum ChangeType
    {
        Increase = 0,
        Decrease = 1
    }

}
