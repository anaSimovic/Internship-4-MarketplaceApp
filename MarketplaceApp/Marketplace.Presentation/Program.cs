using Marketplace.Data;
using Marketplace.Domain;
using Marketplace.Utilities;
using System;

namespace Marketplace.Presentation
{
    class Program
    {
        static void Main(string[] args)
        {
            UserRepository userRepo = new();
            ProductRepository productRepo = new();
            TransactionRepository transactionRepo = new();
            List<Promotion> promotions = new()
            {
            new Promotion("SAVE10", "Electronics", 0.10m, new DateTime(2024, 12, 31)),
            new Promotion("BOOKS15", "Books", 0.15m, new DateTime(2024, 12, 31)),
            new Promotion("FASHION20", "Clothing", 0.20m, new DateTime(2024, 12, 31))
            };

            Console.WriteLine("Welcome to the Marketplace!");
            while (true)
            {
                Console.WriteLine("\n1. Register\n2. Login\n3. Exit");
                string choice = InputValidator.GetNonEmptyString("Choose an option:");

                switch (choice)
                {
                    case "1":
                        RegisterUser(userRepo);
                        break;
                    case "2":
                        LoginUser(userRepo, productRepo, transactionRepo, promotions);
                        break;
                    case "3":
                        Console.WriteLine("Thank you for using Marketplace! Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        static void RegisterUser(UserRepository userRepo)
        {
            string name = InputValidator.GetNonEmptyString("Enter your name:");
            string email = InputValidator.GetNonEmptyString("Enter your email:");
            string userType = InputValidator.GetNonEmptyString("Are you a Buyer or Seller? (B/S):").ToUpper();

            if (userType == "B")
            {
                decimal balance = InputValidator.GetPositiveDecimal("Enter your starting balance:");
                userRepo.AddUser(new Buyer(name, email, balance));
                Console.WriteLine("Buyer registered successfully!");
            }
            else if (userType == "S")
            {
                userRepo.AddUser(new Seller(name, email));
                Console.WriteLine("Seller registered successfully!");
            }
            else
            {
                Console.WriteLine("Invalid user type. Registration failed.");
            }
        }

        static void LoginUser(UserRepository userRepo, ProductRepository productRepo, TransactionRepository transactionRepo, List<Promotion> promotions)
        {
            string email = InputValidator.GetNonEmptyString("Enter your email to login:");
            var user = userRepo.GetUserByEmail(email);

            if (user is null)
            {
                Console.WriteLine("User not found. Please register first.");
                return;
            }

            if (user is Buyer buyer)
            {
                BuyerMenu(buyer, productRepo, transactionRepo, promotions);
            }
            else if (user is Seller seller)
            {
                SellerMenu(seller, productRepo);
            }
        }


        static void BuyerMenu(Buyer buyer, ProductRepository productRepo, TransactionRepository transactionRepo, List<Promotion> promotions)

        {
            while (true)
            {
                Console.WriteLine($"\nWelcome, {buyer.Name}!");
                Console.WriteLine("1. View Products\n2. Buy Product\n3. Add to Favorites\n4. View Favorites\n5. View Purchase History\n6. Return Product\n7. Logout");

                string choice = InputValidator.GetNonEmptyString("Choose an option:");

                switch (choice)
                {
                    case "1":
                        ViewAvailableProducts(productRepo);
                        break;
                    case "2":
                        BuyProduct(buyer, productRepo, transactionRepo, promotions);
                        break;
                    case "3":
                        AddToFavorites(buyer, productRepo);
                        break;
                    case "4":
                        ViewFavorites(buyer);
                        break;
                    case "5":
                        ViewPurchaseHistory(buyer);
                        break;
                    case "6":
                        ReturnProduct(buyer, productRepo, transactionRepo);
                        break;
                    case "7":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void SellerMenu(Seller seller, ProductRepository productRepo)
        {
            while (true)
            {
                Console.WriteLine($"\nWelcome, {seller.Name}!");
                Console.WriteLine("1. Add Product\n2. View Your Products\n3. Edit product price\n4. View Sold Products By Category\n5. Logout");

                string choice = InputValidator.GetNonEmptyString("Choose an option:");

                switch (choice)
                {
                    case "1":
                        AddProduct(seller, productRepo);
                        break;
                    case "2":
                        ViewSellerProducts(seller);
                        break;
                    case "3":
                        EditProductPrice(seller, productRepo);
                        break;
                    case "4":
                        ViewSoldProductsByCategory(seller);
                        break;
                    case "5":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }

        static void ViewAvailableProducts(ProductRepository productRepo)
        {
            var products = productRepo.GetAvailableProducts();
            if (products.Count == 0)
            {
                Console.WriteLine("No products available for sale.");
                return;
            }

            foreach (var product in products)
            {
                Console.WriteLine($"{product.Id}. {product.Name} - ${product.Price} ({product.Category})");
            }
        }

        static void BuyProduct(Buyer buyer, ProductRepository productRepo, TransactionRepository transactionRepo, List<Promotion> promotions)
        {
            Console.WriteLine("Available Products:");
            ViewAvailableProducts(productRepo); 

            int productId = InputValidator.GetPositiveInt("Enter the ID of the product to buy:");
            var product = productRepo.GetProductById(productId);

            if (product == null || product.Status != "For Sale")
            {
                Console.WriteLine("Product not found or not available for sale.");
                return;
            }

            decimal finalPrice = product.Price;

            Console.WriteLine("Enter promo code (leave blank if none):");
            string promoCode = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(promoCode))
            {
                var promo = promotions.FirstOrDefault(p => p.Code.Equals(promoCode, StringComparison.OrdinalIgnoreCase));

                if (promo != null && promo.IsValid() && promo.Category.Equals(product.Category, StringComparison.OrdinalIgnoreCase))
                {
                    finalPrice = product.Price * (1 - promo.Discount);
                    Console.WriteLine($"Promo code applied! Discounted price: ${finalPrice:F2}");
                }
                else
                {
                    Console.WriteLine("Invalid or expired promo code.");
                }
            }

            if (buyer.Balance < finalPrice)
            {
                Console.WriteLine("Insufficient balance to buy this product.");
                return;
            }

            buyer.Balance -= finalPrice;
            buyer.PurchaseHistory.Add(product);
            product.Status = "Sold";
            product.Seller.TotalEarnings += finalPrice * 0.95m; 
            transactionRepo.AddTransaction(new Transaction(productId, buyer, product.Seller, finalPrice));

            Console.WriteLine("Product purchased successfully!");
        }


        static void AddToFavorites(Buyer buyer, ProductRepository productRepo)
        {
            Console.WriteLine("Available Products:");
            ViewAvailableProducts(productRepo);
            int productId = InputValidator.GetPositiveInt("Enter the ID of the product to add to favorites:");
            var product = productRepo.GetProductById(productId);

            if (product == null || product.Status != "For Sale")
            {
                Console.WriteLine("Product not found or unavailable.");
                return;
            }

            if (!buyer.Favorites.Contains(product))
            {
                buyer.Favorites.Add(product);
                Console.WriteLine("Product added to favorites!");
            }
            else
            {
                Console.WriteLine("Product is already in your favorites.");
            }
        }


        static void AddProduct(Seller seller, ProductRepository productRepo)
        {
            string name = InputValidator.GetNonEmptyString("Enter product name:");
            string description = InputValidator.GetNonEmptyString("Enter product description:");
            decimal price = InputValidator.GetPositiveDecimal("Enter product price:");
            string category = InputValidator.GetNonEmptyString("Enter product category:");

            var product = new Product(IDGenerator.GenerateId(), name, description, price, category, seller);
            productRepo.AddProduct(product);
            seller.Products.Add(product);
            Console.WriteLine("Product added successfully!");
        }

        static void ViewFavorites(Buyer buyer)
        {
            if (buyer.Favorites.Count == 0)
            {
                Console.WriteLine("You have no favorite products.");
                return;
            }

            foreach (var product in buyer.Favorites)
            {
                Console.WriteLine($"{product.Id}. {product.Name} - ${product.Price}");
            }
        }

        static void ViewPurchaseHistory(Buyer buyer)
        {
            if (buyer.PurchaseHistory.Count == 0)
            {
                Console.WriteLine("You have no purchase history.");
                return;
            }

            foreach (var product in buyer.PurchaseHistory)
            {
                Console.WriteLine($"{product.Id}. {product.Name} - ${product.Price}");
            }
        }
        static void ReturnProduct(Buyer buyer, ProductRepository productRepo, TransactionRepository transactionRepo)
        {
            Console.WriteLine("Your Purchase History:");
            ViewPurchaseHistory(buyer);
            int productId = InputValidator.GetPositiveInt("Enter the ID of the product to return:");
            var product = buyer.PurchaseHistory.FirstOrDefault(p => p.Id == productId);

            if (product == null || product.Status != "Sold")
            {
                Console.WriteLine("You do not own this product or it is not eligible for return.");
                return;
            }
            decimal refund = product.Price * 0.80m;
            decimal marketplaceFee = product.Price * 0.05m;
            decimal sellerRefund = product.Price - refund - marketplaceFee;
            buyer.Balance += refund;
            product.Status = "For Sale";
            product.Seller.TotalEarnings -= sellerRefund;
            buyer.PurchaseHistory.Remove(product);
            transactionRepo.AddTransaction(new Transaction(product.Id, buyer, product.Seller, -refund));

            Console.WriteLine($"Product returned successfully! Refund of ${refund} credited to your account.");
        }


        static void ViewSellerProducts(Seller seller)
        {
            if (seller.Products.Count == 0)
            {
                Console.WriteLine("You have no products listed.");
                return;
            }

            foreach (var product in seller.Products)
            {
                Console.WriteLine($"{product.Id}. {product.Name} - ${product.Price} ({product.Status})");
            }
        }
        static void EditProductPrice(Seller seller, ProductRepository productRepo)
        {
            Console.WriteLine("Your Products:");
            ViewSellerProducts(seller); 

            int productId = InputValidator.GetPositiveInt("Enter the ID of the product to edit:");
            var product = seller.Products.FirstOrDefault(p => p.Id == productId);

            if (product == null)
            {
                Console.WriteLine("Product not found.");
                return;
            }

            decimal newPrice = InputValidator.GetPositiveDecimal("Enter the new price:");
            product.Price = newPrice;

            Console.WriteLine("Product price updated successfully!");
        }

        static void ViewSoldProductsByCategory(Seller seller)
        {
            var soldProducts = seller.Products.Where(p => p.Status == "Sold");
            if (!soldProducts.Any())
            {
                Console.WriteLine("No products sold.");
                return;
            }

            Console.WriteLine("Enter category to filter:");
            string category = Console.ReadLine() ?? string.Empty;

            var filteredProducts = soldProducts.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

            if (!filteredProducts.Any())
            {
                Console.WriteLine($"No sold products in the category '{category}'.");
            }
            else
            {
                foreach (var product in filteredProducts)
                {
                    Console.WriteLine($"{product.Id}. {product.Name} - ${product.Price}");
                }
            }
        }


    }
}
