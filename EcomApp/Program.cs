using System;
using System.Collections.Generic;
using Models;
using DAO;
using MyExceptions;

namespace EcomApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            OrderProcessorRepository repo = new OrderProcessorRepositoryImpl();
            Customers currentCustomer = null;
            List<CartItem> cart = new List<CartItem>();

            Console.WriteLine("--- Welcome to the E-Commerce App ---");

            while (true)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Register Customer");
                Console.WriteLine("2. Create Product");
                Console.WriteLine("3. Delete Product");
                Console.WriteLine("4. Add to Cart");
                Console.WriteLine("5. View Cart");
                Console.WriteLine("6. Place Order");
                Console.WriteLine("7. View Customer Orders");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");
                string input = Console.ReadLine();

                try
                {
                    switch (input)
                    {
                        case "1":
                            currentCustomer = RegisterCustomer(repo);
                            break;

                        case "2":
                            CreateProduct(repo);
                            break;

                        case "3":
                            DeleteProduct(repo);
                            break;

                        case "4":
                            if (EnsureCustomerSet(currentCustomer))
                                AddToCart(repo, currentCustomer, cart);
                            break;

                        case "5":
                            ViewCart(repo, currentCustomer);
                            break;

                        case "6":
                            if (EnsureCustomerSet(currentCustomer))
                                PlaceOrder(repo, currentCustomer, cart);
                            break;

                        case "7":
                            ViewOrders(repo, currentCustomer);
                            break;

                        case "0":
                            Console.WriteLine("Thank you for using E-Commerce App. Goodbye!");
                            return;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }

        static Customers RegisterCustomer(OrderProcessorRepository repo)
        {
            Customers customer = new Customers();
            Console.Write("Enter Name: ");
            customer.name = Console.ReadLine();
            Console.Write("Enter Email: ");
            customer.email = Console.ReadLine();
            Console.Write("Enter Password: ");
            customer.password = Console.ReadLine();

            if (repo.CreateCustomer(customer))
            {
                
                Console.WriteLine("Customer registered successfully.");
            }
            else
            {
                Console.WriteLine("Customer registration failed.");
            }

            return customer;
        }

        static void CreateProduct(OrderProcessorRepository repo)
        {
            Products product = new Products();
            Console.Write("Enter Product Name: ");
            product.name = Console.ReadLine();
            Console.Write("Enter Price: ");
            product.price = Convert.ToDecimal(Console.ReadLine());
            Console.Write("Enter Description: ");
            product.description = Console.ReadLine();
            Console.Write("Enter Stock Quantity: ");
            product.stockQuantity = Convert.ToInt32(Console.ReadLine());

            if (repo.CreateProduct(product))
                Console.WriteLine("Product created successfully.");
            else
                Console.WriteLine("Product creation failed.");
        }

        static void DeleteProduct(OrderProcessorRepository repo)
        {
            Console.Write("Enter Product ID to delete: ");
            int delId = Convert.ToInt32(Console.ReadLine());
            try
            {
                repo.DeleteProduct(delId);
                Console.WriteLine("Product deleted successfully.");
            }
            catch (ProductNotFoundException)
            {
                Console.WriteLine("Product not found.");
            }
        }

        static void AddToCart(OrderProcessorRepository repo, Customers customer, List<CartItem> cart)
        {
            Console.Write("Enter Product ID to add: ");
            int prodId = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Quantity: ");
            int qty = Convert.ToInt32(Console.ReadLine());
            Products prod = new Products { product_id = prodId };
            if (repo.AddToCart(customer, prod, qty))
            {
                cart.Add(new CartItem { Product = prod, Quantity = qty });
                Console.WriteLine("Product added to cart.");
            }
            else
            {
                Console.WriteLine("Failed to add product to cart.");
            }
        }

        static void ViewCart(OrderProcessorRepository repo, Customers customer)
        {
            if (customer == null || customer.customer_id == 0)
            {
                Console.WriteLine("Register first to view cart.");
                return;
            }

            var cartItems = repo.GetAllFromCart(customer);
            Console.WriteLine("Cart Items:");
            foreach (var p in cartItems)
            {
                Console.WriteLine($"ID: {p.product_id}, Name: {p.name}, Price: {p.price}, Stock: {p.stockQuantity}");
            }
        }

        static void PlaceOrder(OrderProcessorRepository repo, Customers customer, List<CartItem> cart)
        {
            Console.Write("Enter Shipping Address: ");
            string address = Console.ReadLine();
            if (repo.PlaceOrder(customer, cart, address))
            {
                Console.WriteLine("Order placed successfully.");
                cart.Clear();
            }
            else
            {
                Console.WriteLine("Order placement failed.");
            }
        }

        static void ViewOrders(OrderProcessorRepository repo, Customers customer)
        {
            if (customer == null || customer.customer_id == 0)
            {
                Console.WriteLine("Register first to view orders.");
                return;
            }

            var orders = repo.GetOrdersByCustomer(customer.customer_id);
            Console.WriteLine("Customer Orders:");
            foreach (var order in orders)
            {
                decimal total = order.Product.price * order.Quantity;
                Console.WriteLine($"Product: {order.Product.name}, Price: {order.Product.price}, Qty: {order.Quantity}, Total: {total}");
            }
        }

        static bool EnsureCustomerSet(Customers customer)
        {
            if (customer == null || customer.customer_id == 0)
            {
                Console.WriteLine("Please register first.");
                return false;
            }
            return true;
        }
    }
}
