using DigitalWallet.Core.Entities;
using DigitalWallet.Infrastructure;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DigitalWallet.Application.CQRS.Command
{
    public class WithdrawCommand : IRequest<Unit>
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }

        public WithdrawCommand(int walletId, decimal amount)
        {
            WalletId = walletId;
            Amount = amount;
        }
    }

    public class WithdrawCommandHandler : IRequestHandler<WithdrawCommand, Unit>
    {
        private readonly DB _context;

        public WithdrawCommandHandler(DB context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(WithdrawCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _context.Wallets.FindAsync(request.WalletId);
            if (wallet != null)
            {
                wallet.Withdraw(request.Amount);
                _context.Transactions.Add(new Transaction
                {
                    WalletId = request.WalletId,
                    Amount = request.Amount,
                    Date = DateTime.Now,
                    Type = "Withdraw",
                    Status = "Completed"
                });
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
