using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalWallet.Core.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public decimal Balance { get; set; }
        public string Owner { get; set; }

        public void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (Balance >= amount)
                Balance -= amount;
            else
                throw new InvalidOperationException("Insufficient funds");
        }
    }
}
