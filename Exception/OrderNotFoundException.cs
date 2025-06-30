using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExceptions
{
    public class OrderNotFoundException :System.Exception
    {
        public OrderNotFoundException() :base ("Customer Not Found."){ }
        public OrderNotFoundException(string message) :base(message) {

        }
    }
}
