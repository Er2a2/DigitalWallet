using DigitalWallet.Application.Interfaces;
using DigitalWallet.Core.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DigitalWallet.Infrastructure.Services
{
    public class WalletService : IWalletService
    {
        private readonly DB _context;

        public WalletService(DB context)
        {
            _context = context;
        }

        public void Deposit(int walletId, decimal amount)
        {
            var wallet = _context.Wallets.Find(walletId);
            if (wallet != null)
            {
                wallet.Deposit(amount);
                _context.Transactions.Add(new Transaction
                {
                    WalletId = walletId,
                    Amount = amount,
                    Date = DateTime.Now,
                    Type = "Deposit",
                    Status = "Completed"
                });
                _context.SaveChanges();
            }
        }

        public void Withdraw(int walletId, decimal amount)
        {
            var wallet = _context.Wallets.Find(walletId);
            if (wallet != null)
            {
                wallet.Withdraw(amount);
                _context.Transactions.Add(new Transaction
                {
                    WalletId = walletId,
                    Amount = amount,
                    Date = DateTime.Now,
                    Type = "Withdraw",
                    Status = "Completed"
                });
                _context.SaveChanges();
            }
        }

        public void Transfer(int fromWalletId, int toWalletId, decimal amount)
        {
            var fromWallet = _context.Wallets.Find(fromWalletId);
            var toWallet = _context.Wallets.Find(toWalletId);

            if (fromWallet != null && toWallet != null && fromWallet.Balance >= amount)
            {
                fromWallet.Withdraw(amount);
                toWallet.Deposit(amount);
                _context.Transactions.Add(new Transaction
                {
                    WalletId = fromWalletId,
                    Amount = amount,
                    Date = DateTime.Now,
                    Type = "Transfer",
                    Status = "Completed"
                });
                _context.SaveChanges();
            }
        }

        public IEnumerable<Transaction> GetTransactions(int walletId)
        {
            return _context.Transactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.Date)
                .ToList();
        }
    }
}
