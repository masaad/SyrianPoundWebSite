using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SyrianPoundRates.Models;
using SyrianPoundRates.Services;

namespace SyrianPoundRates.Controllers
{
    public class HomeController : Controller
    {

        private List<CurrencyRate> CurrentRates
        {
            get { return Session[SessionKeys.CurrentRates] as List<CurrencyRate>; }
            set { Session[SessionKeys.CurrentRates] = value; }
        }
        public ActionResult Index()
        {     
            var currencyService = new CurrencyRateService();
            var currentRates = currencyService.GetCurrentRates().ToList();

            return View(currentRates);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}