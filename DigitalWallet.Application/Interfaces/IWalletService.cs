using DigitalWallet.Core.Entities; 
using System.Collections.Generic;

namespace DigitalWallet.Application.Interfaces
{
    public interface IWalletService
    {
        void Deposit(int walletId, decimal amount);
        void Withdraw(int walletId, decimal amount);
        void Transfer(int fromWalletId, int toWalletId, decimal amount);
        IEnumerable<Transaction> GetTransactions(int walletId);
    }
}
