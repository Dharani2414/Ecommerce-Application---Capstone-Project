using NUnit.Framework;
using DAO;
using Models;
using Moq;
using MyExceptions;
using System.Collections.Generic;

namespace NunitTests
{
    [TestFixture]
    public class UnitTest1
    {
        private Mock<OrderProcessorRepository> mockRepo;

        [SetUp]
        public void Setup()
        {
            mockRepo = new Mock<OrderProcessorRepository>();
        }

        [Test]
        public void CreateProduct_ShouldReturnTrue()
        {
            var product = new Products
            {
                name = "iPhone 15 Pro",
                price = 800,
                description = "Latest AI features enabled iPhone",
                stockQuantity = 20
            };

            mockRepo.Setup(r => r.CreateProduct(product)).Returns(true);
            bool result = mockRepo.Object.CreateProduct(product);

           Assert.IsTrue(result);
        }

        [Test]
        public void AddProductToCart_ShouldReturnTrue()
        {
            var customer = new Customers { customer_id = 1 };
            var product = new Products { product_id = 1 };

            mockRepo.Setup(r => r.AddToCart(customer, product, 10)).Returns(true);
            bool result = mockRepo.Object.AddToCart(customer, product, 10);

            Assert.IsTrue(result);
        }

        [Test]
        public void ProductOrdered_ShouldReturnTrue()
        {
            var customer = new Customers { customer_id = 1 };
            var cart = new List<CartItem>
            {
                new CartItem
                {
                    Product = new Products { product_id = 1 },
                    Quantity = 1
                }
            };

            mockRepo.Setup(r => r.PlaceOrder(customer, cart, "123 Main St")).Returns(true);
            bool result = mockRepo.Object.PlaceOrder(customer, cart, "123 Main St");

            Assert.IsTrue(result);

        }

        [Test]
        public void DeleteProduct_ShouldThrowProductNotFoundException()
        {
            mockRepo.Setup(r => r.DeleteProduct(It.IsAny<int>()))
                    .Throws(new ProductNotFoundException());

            Assert.Throws<ProductNotFoundException>(() => mockRepo.Object.DeleteProduct(79));
        }

        [Test]
        public void DeleteCustomer_ShouldThrowCustomerNotFoundException()
        {
            mockRepo.Setup(r => r.DeleteCustomer(It.IsAny<int>()))
                    .Throws(new CustomerNotFoundException());

            Assert.Throws<CustomerNotFoundException>(() => mockRepo.Object.DeleteCustomer(79));
        }
    }
}
