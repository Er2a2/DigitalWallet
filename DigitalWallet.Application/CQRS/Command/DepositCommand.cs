using DigitalWallet.Core.Entities;
using DigitalWallet.Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWallet.Application.CQRS.Command
{
    public class DepositCommand : IRequest<Unit>
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }

        public DepositCommand(int walletId, decimal amount)
        {
            WalletId = walletId;
            Amount = amount;
        }
        public class DepositCommandHandler : IRequestHandler<DepositCommand,Unit>
        {
            private readonly DB _context;

            public DepositCommandHandler(DB context)
            {
                _context = context;
            }

            async Task<Unit> IRequestHandler<DepositCommand, Unit>.Handle(DepositCommand request, CancellationToken cancellationToken)
            {
                var wallet = await _context.Wallets.FindAsync(request.WalletId);
                if (wallet != null)
                {
                    wallet.Deposit(request.Amount);
                    _context.Transactions.Add(new Transaction
                    {
                        WalletId = request.WalletId,
                        Amount = request.Amount,
                        Date = DateTime.Now,
                        Type = "Deposit",
                        Status = "Completed"
                    });
                    await _context.SaveChangesAsync(cancellationToken);
                }

                return Unit.Value;
            }
        }
    }
}
