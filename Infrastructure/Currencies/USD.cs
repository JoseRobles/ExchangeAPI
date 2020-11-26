using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Domain;
using Infrastructure;

namespace Infrastructure.Currencies
{
    public class USD : AbstractCurrency
    {
        private readonly ICurrencyService _service;

        public USD(string code, ICurrencyService service)
        {
            _service = service;
            ISOCode = code;
            SetRatesAndPurchaseLimit();
        }

        public override void SetRatesAndPurchaseLimit()
        {
            var rateList = _service.GetURLRate(ISOCode);
            SellRate = rateList.Item1;
            BuyRate = rateList.Item2;
            PurchaseLimit = rateList.Item3;
        }
    }
}