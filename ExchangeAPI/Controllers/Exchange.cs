using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using Repository;
using Domain;

namespace ExchangeAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Exchange : ControllerBase
    {
        private readonly ICurrencyService _service;
        private TransactionContext _dbContext = null;

        public Exchange(ICurrencyService service, TransactionContext context)
        {
            _service = service;
            _dbContext = context;
        }

        [HttpGet("rate/{ISOCode}")]
        public IActionResult Rate(string ISOCode)
        {
            var response = _service.GetExchangeRate(ISOCode);
            return StatusCode(response.ResponseCode, response);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> Purchase(Transaction transaction)
        {
            var response = await _service.PurchaseCurrency(_dbContext, transaction);
            return StatusCode(response.ResponseCode, response);
        }
    }
}