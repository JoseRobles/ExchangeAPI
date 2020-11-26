using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    class URLNotFoundException : Exception
    {
        public URLNotFoundException(string ISOCode) : base(String.Format("URL for Currency {0} was not found", ISOCode))
        {

        }
    }
}
