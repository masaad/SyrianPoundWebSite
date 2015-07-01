using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using SyrianPoundRates.Models;
using SyrianPoundRates.Services;

namespace SyrianPoundRates.Controllers
{
    [Authorize(Roles = UserRoles.Admin + "," + UserRoles.DataEntry)]
    public class RatesController : Controller
    {
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.DataEntry)]
        [HttpGet]
        public ActionResult Index()
        {            
            var currencyService = new CurrencyRateService();
            var currentRates = currencyService.GetCurrentRates().ToList();
            Session[SessionKeys.CurrentRates] = currentRates; 
            return View(new CurrencyRateViewMode(currentRates));
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.DataEntry)]
        [HttpPost]
        public ActionResult Submit(CurrencyRateViewMode updatedRates)
        {
           
            if (ModelState.IsValid)
            {
                var currentRates = Session[SessionKeys.CurrentRates] as List<CurrencyRate>;
                var service = new CurrencyRateService();

                UpdateDollarSellingRate(updatedRates, currentRates, service);
                UpdateEuroSellingRate(updatedRates, currentRates, service);
                UpdateDollarBuyingRate(updatedRates, currentRates, service);
                UpdateEuroBuyingRate(updatedRates, currentRates, service);
                return RedirectToAction("Index", "Home"); 
            }
            
           
            return View("Index", updatedRates); 
        }

        private void UpdateDollarSellingRate(CurrencyRateViewMode updatedRates, List<CurrencyRate> currentRates, CurrencyRateService service)
        {
            var currentDollarSellingRate = currentRates.First(r => r.RateId == updatedRates.SellingUsDollar.RateId);
            if (currentDollarSellingRate.ExchangePrice != updatedRates.SellingUsDollar.ExchangePrice)
            {
                double change = currentDollarSellingRate.ExchangePrice - updatedRates.SellingUsDollar.ExchangePrice;
                currentDollarSellingRate.ChangeAmount = Math.Abs(change);
                currentDollarSellingRate.ChangeType = change > 0
                    ? ChangeType.Decrease
                    : ChangeType.Increase;
                currentDollarSellingRate.ExchangePrice = updatedRates.SellingUsDollar.ExchangePrice; 
                currentDollarSellingRate.UpdatedBy = User.Identity.Name;
                service.UpdateRate(currentDollarSellingRate);
            }
        }

        private void UpdateEuroSellingRate(CurrencyRateViewMode updatedRates, List<CurrencyRate> currentRates, CurrencyRateService service)
        {
            var currentEuroSellingRate = currentRates.First(r => r.RateId == updatedRates.SellingEuro.RateId);
            if (currentEuroSellingRate.ExchangePrice != updatedRates.SellingEuro.ExchangePrice)
            {
                double change = currentEuroSellingRate.ExchangePrice - updatedRates.SellingEuro.ExchangePrice;
                currentEuroSellingRate.ChangeAmount = Math.Abs(change);
                currentEuroSellingRate.ChangeType = change > 0
                    ? ChangeType.Decrease
                    : ChangeType.Increase;
                currentEuroSellingRate.ExchangePrice = updatedRates.SellingEuro.ExchangePrice;
                currentEuroSellingRate.UpdatedBy = User.Identity.GetUserId();
                service.UpdateRate(currentEuroSellingRate);
            }
        }

        private void UpdateDollarBuyingRate(CurrencyRateViewMode updatedRates, List<CurrencyRate> currentRates, CurrencyRateService service)
        {
            var currentDollarBuyingRate = currentRates.First(r => r.RateId == updatedRates.BuyingUsDollar.RateId);
            if (currentDollarBuyingRate.ExchangePrice != updatedRates.BuyingUsDollar.ExchangePrice)
            {
                double change = currentDollarBuyingRate.ExchangePrice - updatedRates.BuyingUsDollar.ExchangePrice;
                currentDollarBuyingRate.ChangeAmount = Math.Abs(change);
                currentDollarBuyingRate.ChangeType = change > 0
                    ? ChangeType.Decrease
                    : ChangeType.Increase;
                currentDollarBuyingRate.ExchangePrice = updatedRates.BuyingUsDollar.ExchangePrice;
                currentDollarBuyingRate.UpdatedBy = User.Identity.GetUserId();
                service.UpdateRate(currentDollarBuyingRate);
            }
        }

        private void UpdateEuroBuyingRate(CurrencyRateViewMode updatedRates, List<CurrencyRate> currentRates, CurrencyRateService service)
        {
            var currenEuroBuyingRate = currentRates.First(r => r.RateId == updatedRates.BuyingEuro.RateId);
            if (currenEuroBuyingRate.ExchangePrice != updatedRates.BuyingEuro.ExchangePrice)
            {
                double change = currenEuroBuyingRate.ExchangePrice - updatedRates.BuyingEuro.ExchangePrice;
                currenEuroBuyingRate.ChangeAmount = Math.Abs(change);
                currenEuroBuyingRate.ChangeType = change > 0
                    ? ChangeType.Decrease
                    : ChangeType.Increase;
                currenEuroBuyingRate.ExchangePrice = updatedRates.BuyingEuro.ExchangePrice; 
                currenEuroBuyingRate.UpdatedBy = User.Identity.GetUserId();
                service.UpdateRate(currenEuroBuyingRate);
            }
        }
      
    }
}