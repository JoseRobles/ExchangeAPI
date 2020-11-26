using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Repository;

namespace Infrastructure
{
    public interface ICurrencyService
    {
        void SetCurrencyConfiguration(CurrencyConfiguration configuration);
        CurrencyServiceResponse GetExchangeRate(string ISOCode);
        AbstractCurrency GetCurrency(string ISOCode);

        decimal ConvertToDecimal(string value);
        Tuple<decimal, decimal, decimal> GetURLRate(string url);

        Tuple<decimal, decimal, decimal> GetSetUpRate(string ISOCode);

        Tuple<bool, decimal> CheckTransactionLimit(TransactionContext context, Transaction transaction);

        Task<PurchaseResponse> PurchaseCurrency(TransactionContext context, Transaction transaction);

        decimal GetTransactionSum(TransactionContext context, Transaction transaction);
    }
}