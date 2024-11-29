namespace Marketplace.Domain
{
    public class Transaction
    {
        public int ProductId { get; set; }
        public Buyer Buyer { get; set; }
        public Seller Seller { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }

        public Transaction(int productId, Buyer buyer, Seller seller, decimal amount)
        {
            ProductId = productId;
            Buyer = buyer;
            Seller = seller;
            Amount = amount;
            TransactionDate = DateTime.Now;
        }
    }
}
