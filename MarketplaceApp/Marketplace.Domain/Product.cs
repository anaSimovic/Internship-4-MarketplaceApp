namespace Marketplace.Domain
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "For Sale";
        public string Category { get; set; }
        public Seller Seller { get; set; }

        public Product(int id, string name, string description, decimal price, string category, Seller seller)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            Seller = seller;
        }
    }
}
