using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class PurchaseLimitException : Exception
    {
        public PurchaseLimitException(string ISOCode, int IdUser) : base(String.Format("Purchase Limit Exception for ISOCode: {0} and User: {1}", ISOCode, IdUser))
        {

        }
    }
}