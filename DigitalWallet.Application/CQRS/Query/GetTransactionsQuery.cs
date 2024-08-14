using DigitalWallet.Core.Entities;
using DigitalWallet.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWallet.Application.CQRS.Query
{
    public class GetTransactionsQuery : IRequest<IEnumerable<Transaction>>
    {
        public int WalletId { get; set; }

        public GetTransactionsQuery(int walletId)
        {
            WalletId = walletId;
        }
    }

    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<Transaction>>
    {
        private readonly DB _context;

        public GetTransactionsQueryHandler(DB context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Transactions
                .Where(t => t.WalletId == request.WalletId)
                .OrderByDescending(t => t.Date)
                .ToListAsync(cancellationToken);
        }
    }
}
