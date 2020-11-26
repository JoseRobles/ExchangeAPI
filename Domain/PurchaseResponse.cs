using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    [Serializable]
    public class PurchaseResponse
    {
        public int ResponseCode { get; set; }
        public string ErrorMessage { get; set; }

        public decimal ExchangedAmount { get; set; }
    }

}
