using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    [Serializable]
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException(string ISOCode)
        : base(String.Format("Invalid Curreny ISOCode: {0}", ISOCode))
        {

        }
    }
}