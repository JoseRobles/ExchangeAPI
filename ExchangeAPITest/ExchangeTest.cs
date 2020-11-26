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
using System.Threading.Tasks;

namespace ExchangeAPITest
{
    [TestClass]
    public class ExchangeTest
    {
        private ICurrencyService _service;
        private TransactionContext _context;

        public ExchangeTest()
        {
            TestSettings testService = new TestSettings();
            _context = testService._contextInitial;
            _service = testService._service;
        }

        [TestMethod]
        public void TestConvertToDecimal()
        {
            Assert.AreEqual(_service.ConvertToDecimal("10.25"), 10.25M);
        }

        [TestMethod]
        [ExpectedException(typeof(CurrencyNotFoundException))]
        public void TestIncorrectISOCode()
        {
            _service.GetCurrency("PEN");
        }

        [TestMethod]
        public void TestGetTransactionSumByMonth()
        {
            /*Get Transactions for BRL*/
            var transactionBRL01 = new Transaction { UserId = 1, IsoCurrency = "BRL", Amount = 10.20M, TransactionDate = DateTime.Today };
            _context.Transactions.Add(transactionBRL01);
            _context.SaveChanges();
            var transaction = new Transaction { IsoCurrency = "BRL", UserId = 1 };
            var sum = _service.GetTransactionSum(_context, transaction);
            Assert.AreEqual(sum, 10.20M);
        }

        [TestMethod]
        public async Task TestPurchaseBRL()
        {
            /**/
            var optionBRL = new DbContextOptionsBuilder<TransactionContext>()
            .UseInMemoryDatabase(databaseName: "TransactionListDatabaseBRL")
            .Options;

            var transactionBRLTEST01 = new Transaction { UserId = 1, IsoCurrency = "BRLTEST", Amount = 10.20M, TransactionDate = DateTime.Today };
            var transactionBRLTEST02 = new Transaction { UserId = 1, IsoCurrency = "BRLTEST", Amount = 10.20M, TransactionDate = DateTime.Today.AddMonths(1) };
            var transactionBRLTEST03 = new Transaction { UserId = 1, IsoCurrency = "USD", Amount = 20.20M, TransactionDate = DateTime.Today };
            TransactionContext _contextBRL = new TransactionContext(optionBRL);
            _contextBRL.Transactions.Add(transactionBRLTEST01);
            _contextBRL.Transactions.Add(transactionBRLTEST02);
            _contextBRL.Transactions.Add(transactionBRLTEST03);
            _contextBRL.SaveChanges();
            /**/

            var transaction = new Transaction { IsoCurrency = "BRLTEST", Amount = 1500M, TransactionDate = DateTime.Today, UserId = 1 };
            var currrentTransaction = await _service.PurchaseCurrency(_contextBRL, transaction);

            Assert.AreEqual(Math.Round(currrentTransaction.ExchangedAmount, 2), 69.77M);
        }

        [TestMethod]
        public async Task TestPurchaseBRLLimit()
        {

            /**/
            var optionBRLTestLimit = new DbContextOptionsBuilder<TransactionContext>()
            .UseInMemoryDatabase(databaseName: "TransactionListDatabaseBRLTESTLIMIT")
            .Options;

            var transactionBRLTEST01 = new Transaction { UserId = 1, IsoCurrency = "BRLTEST", Amount = 10.20M, TransactionDate = DateTime.Today };
            var transactionBRLTEST02 = new Transaction { UserId = 1, IsoCurrency = "BRLTEST", Amount = 10.20M, TransactionDate = DateTime.Today.AddMonths(1) };
            var transactionBRLTEST03 = new Transaction { UserId = 1, IsoCurrency = "USD", Amount = 20.20M, TransactionDate = DateTime.Today };
            TransactionContext _contextBRLTestLimit = new TransactionContext(optionBRLTestLimit);
            _contextBRLTestLimit.Transactions.Add(transactionBRLTEST01);
            _contextBRLTestLimit.Transactions.Add(transactionBRLTEST02);
            _contextBRLTestLimit.Transactions.Add(transactionBRLTEST03);
            _contextBRLTestLimit.SaveChanges();
            /**/

            var transaction = new Transaction { IsoCurrency = "BRLTEST", Amount = 15000M, TransactionDate = DateTime.Today, UserId = 1 };
            var currrentTransaction = await _service.PurchaseCurrency(_contextBRLTestLimit, transaction);

            Assert.AreEqual(currrentTransaction.ExchangedAmount, 0);
        }

    }
}