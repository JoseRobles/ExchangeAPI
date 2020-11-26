using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Domain
{
    public abstract class AbstractCurrency
    {
        public string ISOCode { get; set; }
        private decimal _purchaselimit { get; set; }
        private decimal _buyrate { get; set; }
        private decimal _sellrate { get; set; }
        public decimal BuyRate { get { return _buyrate; } set { _buyrate = value; } }
        public decimal SellRate { get { return _sellrate; } set { _sellrate = value; } }

        [JsonIgnore]
        public decimal PurchaseLimit { get { return _purchaselimit; } set { _purchaselimit = value; } }

        public abstract void SetRatesAndPurchaseLimit();


    }
}