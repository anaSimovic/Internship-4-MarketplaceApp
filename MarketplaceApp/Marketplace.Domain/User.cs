namespace Marketplace.Domain
{
    public abstract class User
    {
        public string Name { get; set; }
        public string Email { get; set; }

        protected User(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }

    public class Buyer : User
    {
        public decimal Balance { get; set; }
        public List<Product> PurchaseHistory { get; } = new();
        public List<Product> Favorites { get; } = new();

        public Buyer(string name, string email, decimal balance)
            : base(name, email)
        {
            Balance = balance;
        }
    }

    public class Seller : User
    {
        public List<Product> Products { get; } = new();
        public decimal TotalEarnings { get; set; }

        public Seller(string name, string email)
            : base(name, email) { }
    }
}
