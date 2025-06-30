using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExceptions
{
    public class OrderPlacementException :System.Exception
    {
        public OrderPlacementException() : base("Order cannot be placed") { }
        public OrderPlacementException(string message) : base(message)
        {
        }
    }
}
