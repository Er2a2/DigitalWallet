using DigitalWallet.Application.Commands;
using DigitalWallet.Application.CQRS.Command;
using DigitalWallet.Application.CQRS.Query;
using DigitalWallet.Application.Interfaces;
using DigitalWallet.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DigitalWallet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletCQRSController : ControllerBase
    { 
     private readonly IMediator _mediator;
     private readonly ICacheService _cacheService;

        public WalletCQRSController(IMediator mediator, ICacheService cacheService)
    {
            _cacheService = cacheService;
            _mediator = mediator;
    }

    [HttpPost("deposit")]
        [SwaggerOperation(
    Summary = "انتقال وجه",
    Description = "شماره حساب را وارد کنید",
    OperationId = "Wallets.Deposit",
    Tags = new[] { "WalletCQRSController" })]
    public async Task<IActionResult> Deposit(int walletId, decimal amount)
    {
        await _mediator.Send(new DepositCommand(walletId, amount));
        return Ok(new { Message = "واریز با موفقیت انجام شد" });
    }

    [HttpPost("withdraw")]
        [SwaggerOperation(
    Summary = "برداشت وجه",
    Description = "شماره حساب را وارد کنید",
    OperationId = "Wallets.Withdraw",
    Tags = new[] { "WalletCQRSController" })]
        public async Task<IActionResult> Withdraw(int walletId, decimal amount)
    {
        await _mediator.Send(new WithdrawCommand(walletId, amount));
        return Ok(new { Message = "برداشت با موفقیت انجام شد" });
    }

    [HttpPost("transfer")]
        [SwaggerOperation(
    Summary = "انتقال وجه",
    Description = "شماره حساب را وارد کنید",
    OperationId = "Wallets.Transfer",
    Tags = new[] { "WalletCQRSController" })]
        public async Task<IActionResult> Transfer(int fromWalletId, int toWalletId, decimal amount)
    {
        await _mediator.Send(new TransferCommand(fromWalletId, toWalletId, amount));
        return Ok(new { Message = "انتقال وجه با موفقیت انجام شد" });
    }

    [HttpGet("transactions/{walletId}")]
        [SwaggerOperation(
  Summary = "سابقه تراکنش ها",
  Description = "شماره حساب را وارد کنید",
  OperationId = "Wallets.GetTransactions",
  Tags = new[] { "WalletCQRSController" })]
        public async Task<IActionResult> GetTransactions(int walletId)
    {
            var cacheKey = $"Transactions_{walletId}";
            var cachedTransactions = await _cacheService.GetAsync<IEnumerable<Transaction>>(cacheKey);

            if (cachedTransactions == null)
            {
                cachedTransactions = await _mediator.Send(new GetTransactionsQuery(walletId));
                await _cacheService.SetAsync(cacheKey, cachedTransactions, TimeSpan.FromMinutes(1));
            }

            return Ok(cachedTransactions);
        }
}
}
