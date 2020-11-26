using System;
using System.Collections.Generic;

#nullable disable

namespace Domain
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string IsoCurrency { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}