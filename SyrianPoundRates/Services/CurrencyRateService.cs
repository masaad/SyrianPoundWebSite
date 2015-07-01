using System;
using System.Collections.Generic;
using System.Data;
using SyrianPoundRates.Gateway;
using SyrianPoundRates.Models;

namespace SyrianPoundRates.Services
{
    public class CurrencyRateService
    {
        private readonly IDbGateway _gateway; 
        public CurrencyRateService()
        {
            _gateway = new DbGateway();
        }

        public IList<CurrencyRate> GetCurrentRates()
        {
            var results = new List<CurrencyRate>(); 
            DataTable dt = _gateway.ExecuteStoredProcedure("GetCurrentRates", new List<DbGatewayParameter>());
            foreach (DataRow row in dt.Rows)
            {
                results.Add(new CurrencyRate()
                {
                    RateId = row["RateId"].ToString(),
                    ChangeId = row["ChangeId"].ToString(),
                    ExchangePrice = double.Parse(row["ExchangePrice"].ToString()), 
                    Type = (TradeType)Enum.Parse(typeof(TradeType), row["TradeType"].ToString()), 
                    UpdatedAt = DateTime.Parse(row["UpdatedAt"].ToString()),
                    CurrencyName = row["Name"].ToString()
                });
            }

            return results;

        }

        public void UpdateRate(CurrencyRate currencyRate)
        {
            var parameters = SpParametersMapper.MapToSpParameters(currencyRate, SpActionType.InsertUpdate);
            _gateway.ExecuteUpdate("UpdateRate", parameters); 
        }
    }
}
