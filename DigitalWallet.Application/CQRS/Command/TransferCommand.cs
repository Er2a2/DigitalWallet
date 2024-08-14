using MediatR;
using DigitalWallet.Core.Entities;  // فرض بر این است که Transaction و Wallet در این namespace هستند
using DigitalWallet.Infrastructure; // فرض بر این است که DB context در این namespace است
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalWallet.Application.Commands
{
    public class TransferCommand : IRequest<Unit>
    {
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }

        public TransferCommand(int fromWalletId, int toWalletId, decimal amount)
        {
            FromWalletId = fromWalletId;
            ToWalletId = toWalletId;
            Amount = amount;
        }
    }

    public class TransferCommandHandler : IRequestHandler<TransferCommand, Unit>
    {
        private readonly DB _context;

        public TransferCommandHandler(DB context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            // یافتن کیف پول مبدا و مقصد
            var fromWallet = await _context.Wallets.FindAsync(request.FromWalletId);
            var toWallet = await _context.Wallets.FindAsync(request.ToWalletId);

            // بررسی وجود کیف پول‌ها و موجودی کافی در کیف پول مبدا
            if (fromWallet != null && toWallet != null && fromWallet.Balance >= request.Amount)
            {
                // انجام تراکنش انتقال
                fromWallet.Withdraw(request.Amount);
                toWallet.Deposit(request.Amount);

                // افزودن تراکنش به دیتابیس
                _context.Transactions.Add(new Transaction
                {
                    WalletId = request.FromWalletId,
                    Amount = request.Amount,
                    Date = DateTime.Now,
                    Type = "Transfer",
                    Status = "Completed"
                });

                // ذخیره تغییرات
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
