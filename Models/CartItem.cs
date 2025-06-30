using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CartItem
    {
        
            public Products Product { get; set; }
            public int Quantity { get; set; }

            public decimal GetTotalPrice()
            {
                return Product.price * Quantity;
            }
        }
    }


