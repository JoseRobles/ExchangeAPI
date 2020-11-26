using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Repository;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class CurrencyService : ICurrencyService
    {
        private CurrencyConfiguration _currencyConfiguration;
        private ILogger _logger;
        public CurrencyService(IOptionsMonitor<CurrencyConfiguration> currencyOptions, ILogger<CurrencyService> logger)
        {
            _currencyConfiguration = currencyOptions.CurrentValue;
            _logger = logger;
        }

        public void SetCurrencyConfiguration(CurrencyConfiguration configuration)
        {
            _currencyConfiguration = configuration;
        }

        public CurrencyServiceResponse GetExchangeRate(string ISOCode)
        {
            var response = new CurrencyServiceResponse();
            try
            {
                var currency = GetCurrency(ISOCode);
                response.Currency = currency;
                response.ResponseCode = 200;
            }
            catch (CurrencyNotFoundException currency_exception)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = currency_exception.Message;
                _logger.LogError("Exception: Currency Not Found");
            }
            catch (Exception exception)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = "Internal Server Error";
                _logger.LogError("Exception: " + exception.ToString());
            }
            return response;
        }

        public AbstractCurrency GetCurrency(string ISOCode)
        {
            var parameters = new object[2] { ISOCode, this };
            Type type = Type.GetType(String.Format("Infrastructure.Currencies.{0}", ISOCode));
            if (type == null)
                throw new CurrencyNotFoundException(ISOCode);

            return (AbstractCurrency)Activator.CreateInstance(type, parameters);
        }

        public decimal ConvertToDecimal(string value)
        {
            decimal convertedValue = 0;

            string CultureName = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(CultureName);
            if (ci.NumberFormat.NumberDecimalSeparator != ".")
            {
                // Forcing use of decimal separator for numerical values
                ci.NumberFormat.NumberDecimalSeparator = ".";
                Thread.CurrentThread.CurrentCulture = ci;
            }

            decimal.TryParse(value, out convertedValue);

            return convertedValue;
        }

        public Tuple<decimal, decimal, decimal> GetURLRate(string ISOCode)
        {
            decimal buyRate = 0;
            decimal sellRate = 0;
            decimal currenyLimit = 0;

            var currency = _currencyConfiguration.URLSection.URLCurrencyList.Where(x => x.ISOCode == ISOCode).FirstOrDefault();
            var url = currency.URL;
            var limit = currency.PurchaseLimit;

            var serviceResponse = new List<string>();
            try
            {
                using (HttpClient apiRequest = new HttpClient())
                {
                    apiRequest.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    Task<HttpResponseMessage> response = apiRequest.GetAsync(url);
                    if (response.Result.IsSuccessStatusCode)
                    {
                        serviceResponse = JsonConvert.DeserializeObject<List<string>>(response.Result.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception exp)
            {
                //Logger
            }

            buyRate = ConvertToDecimal(serviceResponse[0]);
            sellRate = ConvertToDecimal(serviceResponse[1]);
            currenyLimit = ConvertToDecimal(limit);

            return new Tuple<decimal, decimal, decimal>(buyRate, sellRate, currenyLimit);
        }

        public Tuple<decimal, decimal, decimal> GetSetUpRate(string ISOCode)
        {
            decimal buyRate = 0;
            decimal sellRate = 0;
            decimal currencyLimit = 0;

            var currency = _currencyConfiguration.SetupSection.SetupCurrencyList.Where(x => x.ISOCode == ISOCode).FirstOrDefault();
            var currencyBase = currency.CurrencyBase;
            var convertionRate = ConvertToDecimal(currency.Rate);
            var limit = ConvertToDecimal(currency.PurchaseLimit);
            var rateList = GetURLRate(currencyBase);

            buyRate = rateList.Item1 * convertionRate;
            sellRate = rateList.Item2 * convertionRate;

            return new Tuple<decimal, decimal, decimal>(buyRate, sellRate, currencyLimit);
        }

        public async Task<PurchaseResponse> PurchaseCurrency(TransactionContext context, Transaction transaction)
        {
            var response = new PurchaseResponse();

            try
            {
                var checkTransaction = CheckTransactionLimit(context, transaction);
                if (!checkTransaction.Item1)
                {
                    transaction.TransactionDate = DateTime.Today;
                    transaction.Amount = checkTransaction.Item2;
                    context.Transactions.Add(transaction);
                    await context.SaveChangesAsync();
                    response.ExchangedAmount = transaction.Amount;
                    response.ResponseCode = 200;
                }
                else
                {
                    throw new PurchaseLimitException(transaction.IsoCurrency, transaction.UserId);
                }
            }
            catch (CurrencyNotFoundException currency_exception)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = currency_exception.Message;
            }
            catch (PurchaseLimitException purchase_limit_exception)
            {
                response.ResponseCode = 400;
                response.ErrorMessage = purchase_limit_exception.Message;
            }
            catch (Exception exception)
            {
                response.ResponseCode = 500;
                response.ErrorMessage = "Internal Server Error";
                _logger.LogError("Exception: " + exception.ToString());
            }

            return response;
        }

        public Tuple<bool, decimal> CheckTransactionLimit(TransactionContext context, Transaction transaction)
        {
            var currentTransactionSum = GetTransactionSum(context, transaction);
            var currency = GetCurrency(transaction.IsoCurrency);
            var currentTransaction = Math.Round(transaction.Amount / currency.SellRate, 2);
            bool excededLimit = (currentTransactionSum + currentTransaction) > currency.PurchaseLimit;

            return new Tuple<bool, decimal>(excededLimit, currentTransaction);
        }

        public decimal GetTransactionSum(TransactionContext context, Transaction transaction)
        {
            var firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1).AddTicks(-1);

            var currentTransactionSum = context.Transactions.Where(x => x.UserId == transaction.UserId && x.IsoCurrency == transaction.IsoCurrency && x.TransactionDate >= firstDayOfMonth && x.TransactionDate <= lastDayOfMonth).Sum(y => y.Amount);

            return currentTransactionSum;
        }
    }
}