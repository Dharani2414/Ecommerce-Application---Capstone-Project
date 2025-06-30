# Ecommerce Console Application – Capstone Project

A console-based Ecommerce application built as part of a Hexaware Capstone Project. It demonstrates core software engineering principles such as object-oriented programming, layered architecture, SQL database integration, exception handling, and unit testing.

## 📁 Project Structure

- `EcommerceApp/`
  - `app/` – Main driver class (`EcomApp.cs`)
  - `dao/` – Repository interfaces and their implementations
  - `entity/` – POCO classes for `Customer`, `Product`, `Order`, etc.
  - `myexceptions/` – Custom exception classes
  - `util/` – Utility classes for DB connection and configuration
  - `tests/` – Unit test cases
 
    
## ✅ Features

### 1. Customer Management
- Register new customers
- Retrieve customer and order details
- Delete customers

### 2. Product Management
- Add new products
- View all products
- Delete existing products

### 3. Cart Management
- Add products to the cart
- Remove products from the cart
- View cart contents

### 4. Order Management
- Place orders from cart
- Calculate total price
- Track order details
- Shipping address input

### 5. Exception Handling
- `CustomerNotFoundException`
- `ProductNotFoundException`
- `OrderNotFoundException`

### 6. Unit Testing
Test cases cover:
- Product creation
- Adding to cart
- Placing an order
- Exception scenarios

## 🧱 SQL Schema

- `customers`: customer_id (PK), name, email, password
- `products`: product_id (PK), name, price, description, stockQuantity
- `cart`: cart_id (PK), customer_id (FK), product_id (FK), quantity
- `orders`: order_id (PK), customer_id (FK), order_date, total_price, shipping_address
- `order_items`: order_item_id (PK), order_id (FK), product_id (FK), quantity

## 🔌 Tech Stack

- Language: C#
- Framework: .NET Core / .NET Framework
- Database: SQL Server
- Testing: NUnit / xUnit
- Tools: Visual Studio, Git

## ⚙️ Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/Ecommerce-Application---Capstone-Project.git
   cd Ecommerce-Application---Capstone-Project
