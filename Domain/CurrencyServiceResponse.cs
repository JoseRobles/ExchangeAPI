using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    [Serializable]
    public class CurrencyServiceResponse
    {
        public int ResponseCode { get; set; }
        public string ErrorMessage { get; set; }

        public AbstractCurrency Currency { get; set; }
    }

}