using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Infrastructure.Currencies
{
    public class BRLTEST : AbstractCurrency
    {
        private readonly ICurrencyService _service;
        public BRLTEST(string code, ICurrencyService service)
        {
            _service = service;
            ISOCode = code;
            SetRatesAndPurchaseLimit();
        }

        public override void SetRatesAndPurchaseLimit()
        {
            BuyRate = 19.5M;
            SellRate = 21.5M;
            PurchaseLimit = 300;
        }

    }
}