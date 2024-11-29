namespace Marketplace.Domain
{
    public class Promotion
    {
        public string Code { get; set; }
        public string Category { get; set; }
        public decimal Discount { get; set; } 
        public DateTime ExpiryDate { get; set; }

        public Promotion(string code, string category, decimal discount, DateTime expiryDate)
        {
            Code = code;
            Category = category;
            Discount = discount;
            ExpiryDate = expiryDate;
        }

        public bool IsValid() => DateTime.Now <= ExpiryDate;
    }
}
