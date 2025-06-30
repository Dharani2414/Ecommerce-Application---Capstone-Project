using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExceptions
{
    public class ProductNotFoundException : System.Exception
    {
        public ProductNotFoundException() : base("Product Not Found") { }
        public ProductNotFoundException(string message) : base(message)
        {
        }
    }
}
