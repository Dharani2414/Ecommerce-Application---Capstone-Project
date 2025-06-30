using Models;
using MyExceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Utils;

namespace DAO
{
    public class OrderProcessorRepositoryImpl : OrderProcessorRepository
    {
        private SqlConnection connection;

        public OrderProcessorRepositoryImpl()
        {
            connection = DBConnUtil.GetConnection();
        }

        public bool CreateProduct(Products product)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = "INSERT INTO products (name, price, description, stockQuantity) VALUES (@name, @price, @desc, @stock)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", product.name);
            cmd.Parameters.AddWithValue("@price", product.price);
            cmd.Parameters.AddWithValue("@desc", product.description);
            cmd.Parameters.AddWithValue("@stock", product.stockQuantity);

            return cmd.ExecuteNonQuery() > 0;
        }
        public Products GetProductById(int productId)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = "SELECT product_id, name, price, description, stockQuantity FROM products WHERE product_id = @id";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", productId);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Products
                    {
                        product_id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        price = reader.GetDecimal(2),
                        description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        stockQuantity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                    };
                }
            }

            return null;
        }

        public bool CreateCustomer(Customers customer)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = @"INSERT INTO customers (name, email, password)
                     OUTPUT INSERTED.customer_id
                     VALUES (@name, @email, @password)";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@name", customer.name);
            cmd.Parameters.AddWithValue("@email", customer.email);
            cmd.Parameters.AddWithValue("@password", customer.password);

            object result = cmd.ExecuteScalar();
            if (result != null)
            {
                customer.customer_id = Convert.ToInt32(result);  
                return true;
            }

            return false;
        }


        public bool DeleteProduct(int productId)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            
            string deleteFromCart = "DELETE FROM cart WHERE product_id = @id";
            SqlCommand cmdCart = new SqlCommand(deleteFromCart, connection);
            cmdCart.Parameters.AddWithValue("@id", productId);
            cmdCart.ExecuteNonQuery();

           
            string query = "DELETE FROM products WHERE product_id = @id";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", productId);
            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
                throw new ProductNotFoundException();

            return true;
        }


        public bool DeleteCustomer(int customerId)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = "DELETE FROM customers WHERE customer_id = @id";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", customerId);
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                throw new CustomerNotFoundException();

            return true;
        }

        public bool AddToCart(Customers customer, Products product, int quantity)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = "INSERT INTO cart (customer_id, product_id, quantity) VALUES (@cid, @pid, @qty)";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", customer.customer_id);
            cmd.Parameters.AddWithValue("@pid", product.product_id);
            cmd.Parameters.AddWithValue("@qty", quantity);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool RemoveFromCart(Customers customer, Products product)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = "DELETE FROM cart WHERE customer_id = @cid AND product_id = @pid";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", customer.customer_id);
            cmd.Parameters.AddWithValue("@pid", product.product_id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public List<Products> GetAllFromCart(Customers customer)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = @"SELECT p.product_id, p.name, p.price, p.description, p.stockQuantity 
                             FROM cart c 
                             JOIN products p ON c.product_id = p.product_id 
                             WHERE c.customer_id = @cid";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", customer.customer_id);

            List<Products> productsInCart = new List<Products>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Products product = new Products
                    {
                        product_id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        price = reader.GetDecimal(2),
                        description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        stockQuantity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                    };

                    productsInCart.Add(product);
                }
            }

            return productsInCart;
        }
        public bool PlaceOrder(Customers customer, List<CartItem> cartItems, string shippingAddress)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            
            foreach (var item in cartItems)
            {
                Products dbProduct = GetProductById(item.Product.product_id); 
                if (dbProduct == null)
                    throw new ProductNotFoundException();
                item.Product = dbProduct;
            }

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                decimal total = cartItems.Sum(item => item.Product.price * item.Quantity);
                Console.WriteLine("Total price: " + total);

                string insertOrderQuery = @"
            INSERT INTO orders (customer_id, order_date, total_price, shipping_address) 
            OUTPUT INSERTED.order_id 
            VALUES (@cid, @date, @total, @address)";

                SqlCommand cmdOrder = new SqlCommand(insertOrderQuery, connection, transaction);
                cmdOrder.Parameters.AddWithValue("@cid", customer.customer_id);
                cmdOrder.Parameters.AddWithValue("@date", DateTime.Now);
                cmdOrder.Parameters.AddWithValue("@total", total);
                cmdOrder.Parameters.AddWithValue("@address", shippingAddress);

                int orderId = (int)cmdOrder.ExecuteScalar();

                foreach (var item in cartItems)
                {
                    string insertItemQuery = @"
                INSERT INTO order_items (order_id, product_id, quantity) 
                VALUES (@oid, @pid, @qty)";

                    SqlCommand cmdItem = new SqlCommand(insertItemQuery, connection, transaction);
                    cmdItem.Parameters.AddWithValue("@oid", orderId);
                    cmdItem.Parameters.AddWithValue("@pid", item.Product.product_id);
                    cmdItem.Parameters.AddWithValue("@qty", item.Quantity);
                    cmdItem.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw new OrderPlacementException();
            }
        }


        public List<CartItem> GetOrdersByCustomer(int customerId)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            string query = @"SELECT p.product_id, p.name, p.price, p.description, p.stockQuantity, oi.quantity
                             FROM orders o
                             JOIN order_items oi ON o.order_id = oi.order_id
                             JOIN products p ON oi.product_id = p.product_id
                             WHERE o.customer_id = @cid";

            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@cid", customerId);

            List<CartItem> orders = new List<CartItem>();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Products product = new Products
                    {
                        product_id = reader.GetInt32(0),
                        name = reader.GetString(1),
                        price = reader.GetDecimal(2),
                        description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        stockQuantity = reader.IsDBNull(4) ? 0 : reader.GetInt32(4)
                    };

                    CartItem item = new CartItem
                    {
                        Product = product,
                        Quantity = reader.GetInt32(5)
                    };

                    orders.Add(item);
                }
            }

            return orders;
        }
    }
}
