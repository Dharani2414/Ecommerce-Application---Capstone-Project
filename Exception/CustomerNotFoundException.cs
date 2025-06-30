using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExceptions
{
    public class CustomerNotFoundException : System.Exception
    {
        public CustomerNotFoundException() : base("Customer Not Found") { }
        public CustomerNotFoundException(string message) : base(message)
        {
        }
    }
}
