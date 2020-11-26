using Microsoft.VisualStudio.TestTools.UnitTesting;
using Infrastructure;
using Repository;
using Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace ExchangeAPITest
{
    public class TestSettings
    {
        public TransactionContext _contextInitial;
        public ICurrencyService _service;

        public TestSettings()
        {
            /*Setting Up Context*/
            var options = new DbContextOptionsBuilder<TransactionContext>()
            .UseInMemoryDatabase(databaseName: "TransactionListDatabase")
            .Options;
            _contextInitial = new TransactionContext(options);
            _contextInitial.SaveChanges();
            /*End of Setting Up Context*/

            /*Setting Up Configuration*/
            CurrencyConfiguration currencyConfiguration = new CurrencyConfiguration();
            currencyConfiguration.SetupSection = new SetupSection();
            currencyConfiguration.URLSection = new URLSection();
            currencyConfiguration.SetupSection.SetupCurrencyList = new List<SetupCurrency>();
            currencyConfiguration.URLSection.URLCurrencyList = new List<URLCurrency>();

            var setupCurrency = new SetupCurrency() { ISOCode = "BRL", PurchaseLimit = "300", CurrencyBase = "USD", Rate = "0.25" };
            var urlCurrency = new URLCurrency() { ISOCode = "USD", PurchaseLimit = "200" };

            currencyConfiguration.SetupSection.SetupCurrencyList.Add(setupCurrency);
            currencyConfiguration.URLSection.URLCurrencyList.Add(urlCurrency);
            var monitorMock = Mock.Of<IOptionsMonitor<CurrencyConfiguration>>(_ => _.CurrentValue == currencyConfiguration);
            var loggerMock = Mock.Of<ILogger<CurrencyService>>();

            _service = new CurrencyService(monitorMock, loggerMock);
            /*End of Setting Up Configuration*/
        }
    }
}