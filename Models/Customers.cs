using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Models
{
    public class Customers
    {
        public int customer_id { get; set; }

        public string name { get; set; }
        public string email { get; set;}
        public string password { get; set;}
        public Customers() { }
        
    }
    }

