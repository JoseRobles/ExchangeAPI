using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    [Serializable]
    public class CurrencyConfiguration
    {
        public URLSection URLSection { get; set; }
        public SetupSection SetupSection { get; set; }
    }

    public class URLSection
    {
        public List<URLCurrency> URLCurrencyList { get; set; }
    }

    public class SetupSection
    {
        public List<SetupCurrency> SetupCurrencyList { get; set; }
    }

    public class SetupCurrency
    {
        public string ISOCode { get; set; }
        public string Rate { get; set; }
        public string CurrencyBase { get; set; }

        public string PurchaseLimit { get; set; }
    }

    public class URLCurrency
    {
        public string ISOCode { get; set; }
        public string URL { get; set; }
        public string PurchaseLimit { get; set; }
    }
}