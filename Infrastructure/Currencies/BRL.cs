using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Infrastructure.Currencies
{
    public class BRL : AbstractCurrency
    {
        private readonly ICurrencyService _service;
        public BRL(string code, ICurrencyService service)
        {
            _service = service;
            ISOCode = code;
            SetRatesAndPurchaseLimit();
        }

        public override void SetRatesAndPurchaseLimit()
        {
            var rateList = _service.GetSetUpRate(ISOCode);
            BuyRate = rateList.Item1;
            SellRate = rateList.Item2;
            PurchaseLimit = rateList.Item3;
        }

    }
}