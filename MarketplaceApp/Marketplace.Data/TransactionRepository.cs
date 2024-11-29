using Marketplace.Domain;
using System.Collections.Generic;

namespace Marketplace.Data
{
    public class TransactionRepository
    {
        private readonly List<Transaction> _transactions = new();

        public void AddTransaction(Transaction transaction) =>
            _transactions.Add(transaction);

        public List<Transaction> GetAllTransactions() => _transactions;
    }
}
