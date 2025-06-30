using System;
using Models; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface OrderProcessorRepository
    {
        Products GetProductById(int productId);

        bool CreateProduct(Products product);

        bool CreateCustomer(Customers customer);

        bool DeleteProduct(int productId);

        bool DeleteCustomer(int customerId);

        bool AddToCart(Customers customer, Products product, int quantity);

        bool RemoveFromCart(Customers customer, Products product);

        List<Products> GetAllFromCart(Customers customer);

        bool PlaceOrder(Customers customer, List<CartItem> cartItems, string shippingAddress);

        List<CartItem > GetOrdersByCustomer(int customerId);
        
    }
}
